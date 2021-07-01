using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Npgsql;
using Serilog;

namespace Bonanza.Storage.PostgreSql2Indexes
{
	/// <summary>
	/// <para>This is a SQL event storage for PgSql, simplified to demonstrate 
	/// essential principles.
	/// If you need more robust SQL implementation, check out Event Store of
	/// Jonathan Oliver</para>
	/// <para>This code is frozen to match IDDD book. For latest practices see Lokad.CQRS Project</para>
	/// </summary>
	public sealed class PgSql2IndexesEventStore : IAppendOnlyStore
	{
		readonly string _connectionString;
		private ConcurrentQueue<NpgsqlConnection> _connections;
		private ILogger _logger;
		private int _logEveryEventsCount;
		private int appendCount = 0;
		private Stopwatch sw = Stopwatch.StartNew();
		private AppendMethod _appendMethod;
		private delegate void AppendMethod(long tenantId, string streamName, byte[] data, long expectedVersion, NpgsqlConnection connection);
		private bool _cacheConnection;
		public bool TenantIdWithName { get; }

		private AppendMethod ChooseStrategy(AppendStrategy strategy)
		{
			var dict = new Dictionary<AppendStrategy, AppendMethod>()
			{
				{AppendStrategy.OnePhase, Append1Phase},
				{AppendStrategy.OnePhaseNoVersionCheck, Append1PhaseNoVersionCheck},
				{AppendStrategy.TwoPhases, Append2Phases},
			};

			var choosenStrategy = dict[strategy];
			return choosenStrategy;
		}

		public PgSql2IndexesEventStore(string connectionString, ILogger logger, int logEveryEventsCount,
			AppendStrategy strategy, bool tenantIdInStreamName, bool cacheConnection)
		{
			_connectionString = connectionString;
			_logger = logger;
			TenantIdWithName = tenantIdInStreamName;
			_cacheConnection = cacheConnection;
			_logEveryEventsCount = logEveryEventsCount;
			_connections = new ConcurrentQueue<NpgsqlConnection>();
			_appendMethod = ChooseStrategy(strategy);
			logger.Information($"[{this.GetType().ToString().Split('.').Last()}] strategy used: {strategy.ToString()}");
		}

		public PgSql2IndexesEventStore Initialize(bool dropDb)
		{
			using (var conn = new NpgsqlConnection(_connectionString))
			{
				conn.Open();
				const string dropTable = @"DROP TABLE IF EXISTS es_events;";
				const string createTable = @"CREATE TABLE IF NOT EXISTS es_events (Id SERIAL,tenantid bigint,Name VARCHAR (50) NOT NULL,Version INT NOT NULL,Data BYTEA NOT NULL);";
				const string createIdx = @"CREATE INDEX IF NOT EXISTS ""name-idx"" ON public.es_events USING btree(tenantId, name COLLATE pg_catalog.""default"" ASC NULLS LAST)TABLESPACE pg_default;";
				const string createIdx2 = @"CREATE INDEX IF NOT EXISTS ""tenant-idx"" ON public.es_events USING btree(tenantId)TABLESPACE pg_default;";
				const string createFunction = @"
CREATE OR REPLACE FUNCTION AppendEvent2Indexes(tid bigint, expectedVersion bigint, aggregateName text, data bytea)
RETURNS int AS 
$$ -- here start procedural part
   DECLARE currentVer int;
   BEGIN
		SELECT INTO currentVer COALESCE(MAX(version),-1)
				FROM public.es_events
				WHERE tenantid = tid and name = aggregateName ;
		IF expectedVersion <> -1 THEN
			IF currentVer <> expectedVersion THEN
				RETURN currentVer;
			END IF;
		END IF;
		INSERT INTO public.es_events (tenantid,Name,Version,Data) VALUES(tid,aggregateName,currentVer+1,data);
				RETURN currentVer;
				--RETURN 0;
   END;
$$ -- here finish procedural part
LANGUAGE plpgsql; -- language specification ";

				const string createTableSql = 
				createTable 
				+ createIdx 
				+ createIdx2 
				+ createFunction;
				const string dropTableCreateTableSql = 
					dropTable 
					+ createTable 
					+ createIdx 
					+ createIdx2 
					+ createFunction;

				using (var cmd = new NpgsqlCommand(dropDb? dropTableCreateTableSql:createTableSql, conn))
				{
					cmd.ExecuteNonQuery();
				}
			}

			return this;
		}

		public void Dispose()
		{

		}

		public void Append(string name, byte[] data, long expectedVersion, int tenantId)
		{
			if (_cacheConnection)
			{
				NpgsqlConnection conn = null;
				try
				{
					conn = GetFromCacheOrNew();
					_appendMethod(tenantId, name, data, expectedVersion, conn);
				}
				finally
				{
					if (conn !=  null)
					{
						_connections.Enqueue(conn);
					}
				}
			}
			else
			{
				using (var conn = new NpgsqlConnection(_connectionString))
				{
					conn.Open();
					_appendMethod(tenantId, name, data, expectedVersion, conn);
				}
			}
		}

		private void WriteAppendsCountIntoLog()
		{
			if ((_logEveryEventsCount > 0) && (appendCount % _logEveryEventsCount == 0))
			{
					_logger?.Information("[ EventStore ] Events appended {appendCount:D10}, speed: {speed:F1}", appendCount,
					_logEveryEventsCount * 1000 / (sw.ElapsedMilliseconds + 1.0));
				sw.Restart();
			}
		}

		private NpgsqlConnection GetFromCacheOrNew()
		{
			NpgsqlConnection conn;
			if (_connections.TryDequeue(out var temp))
			{
				conn = temp;
			}
			else
			{
				conn = new NpgsqlConnection(_connectionString);
				conn.Open();
			}

			return conn;
		}

		public IEnumerable<DataWithVersion> ReadRecords(string name, long afterVersion, int maxCount)
		{
			using (var conn = new NpgsqlConnection(_connectionString))
			{
				conn.Open();
				const string sql =
					@"SELECT Data,Version FROM ES_Events
                        WHERE Name = @name AND version>@version
                        ORDER BY version
                        LIMIT @take OFFSET 0";
				using (var cmd = new NpgsqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("?name", name);
					cmd.Parameters.AddWithValue("?version", afterVersion);
					cmd.Parameters.AddWithValue("?take", maxCount);
					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							var data = (byte[])reader["Data"];
							var version = (int)reader["Version"];
							yield return new DataWithVersion(version, data);
						}
					}
				}
			}
		}

		public IEnumerable<DataWithName> ReadRecords(int afterVersion, int maxCount)
		{
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                const string sql =
					@"SELECT Data, Name FROM ES_Events
                        WHERE Id>@after
                        ORDER BY Id
                        LIMIT @take OFFSET 0";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@after", afterVersion);
                    cmd.Parameters.AddWithValue("@take", maxCount);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var data = (byte[])reader["Data"];
                            var name = (string)reader["Name"];
                            yield return new DataWithName(name, data);
                        }
                    }
                }
            }
		}

		public void Close()
		{
			throw new System.NotImplementedException();
		}

		[Obsolete]
		public void Append(string name, byte[] data, long expectedVersion = -1)
		{
			throw new NotImplementedException();
		}

		public void Append2Phases(long tenantId, string name, byte[] data, long expectedVersion, NpgsqlConnection conn)
		{
			using (var tx = conn.BeginTransaction())
			{
				const string sql =
					@"SELECT COALESCE (MAX(version),-1)
                        FROM public.es_events
                        WHERE name = @name;";
				int version;
				using (var cmd = new NpgsqlCommand(sql, conn, tx))
				{
					cmd.Parameters.AddWithValue("@name", name);
					version = (int)cmd.ExecuteScalar();
					if (expectedVersion != -1)
					{
						if (version != expectedVersion)
						{
							throw new AppendOnlyStoreConcurrencyException(version, expectedVersion, name);
						}
					}
				}

				const string insertCmd =
					@"INSERT INTO public.es_events (Name,Version,Data) 
                            VALUES(@name, @version, @data)";

				using (var cmd = new NpgsqlCommand(insertCmd, conn, tx))
				{
					cmd.Parameters.AddWithValue("@name", name);
					cmd.Parameters.AddWithValue("@version", version + 1);
					cmd.Parameters.AddWithValue("@data", data);
					cmd.ExecuteNonQuery();
				}
				tx.Commit();

				Interlocked.Increment(ref appendCount);
				WriteAppendsCountIntoLog();
			}
		}

		public void Append1Phase(long tenantId, string name, byte[] data, long expectedVersion, NpgsqlConnection conn)
		{
			try
			{
				using (var tx = conn.BeginTransaction())
				{
					const string sql =
						@"SELECT AppendEvent2Indexes(@tid,@expectedVersion,@name,@data)";

					int version;
					using (var cmd = new NpgsqlCommand(sql, conn, tx))
					{
						cmd.Parameters.AddWithValue("@name", name);
						cmd.Parameters.AddWithValue("@tid", tenantId);
						cmd.Parameters.AddWithValue("@expectedVersion", expectedVersion);
						cmd.Parameters.AddWithValue("@data", data);
						version = (int)cmd.ExecuteScalar();
						if (expectedVersion != -1)
						{
							if (version != expectedVersion)
							{
								throw new AppendOnlyStoreConcurrencyException(version, expectedVersion, name);
							}
						}
					}

					/*
					const string txt =
						@"INSERT INTO public.es_events (Name,Version,Data) 
									VALUES(@name, @version, @data)";

					using (var cmd = new NpgsqlCommand(txt, conn, tx))
					{
						cmd.Parameters.AddWithValue("@name", name);
						cmd.Parameters.AddWithValue("@version", version + 1);
						cmd.Parameters.AddWithValue("@data", data);
						cmd.ExecuteNonQuery();
					}
					*/
					tx.Commit();
					Interlocked.Increment(ref appendCount);
					WriteAppendsCountIntoLog();
				}
			}
			catch (Exception e)
			{
				_logger.Error(e, "error occured during append");
				throw;
			}
			finally
			{
				//_logger.Information("Test!!!");
			}
		}

		public void Append1PhaseNoVersionCheck(long tenantId, string name, byte[] data, long expectedVersion, NpgsqlConnection conn)
		{
			using (var tx = conn.BeginTransaction())
			{
				/*
				const string sql = @"SELECT appendevent(@expectedVersion,@name,@data)";

				int version;
				using (var cmd = new NpgsqlCommand(sql, conn, tx))
				{
					cmd.Parameters.AddWithValue("@name", name);
					cmd.Parameters.AddWithValue("@expectedVersion", expectedVersion);
					cmd.Parameters.AddWithValue("@data", data);
					version = (int)cmd.ExecuteScalar();
					if (expectedVersion != -1)
					{
						if (version != expectedVersion)
						{
							throw new AppendOnlyStoreConcurrencyException(version, expectedVersion, name);
						}
					}
				}
				*/

				
				const string txt =
					@"INSERT INTO public.es_events (Name,Version,Data) 
                                VALUES(@name, @version, @data)";

				using (var cmd = new NpgsqlCommand(txt, conn, tx))
				{
					cmd.Parameters.AddWithValue("@name", name);
					cmd.Parameters.AddWithValue("@version", 1);
					cmd.Parameters.AddWithValue("@data", data);
					cmd.ExecuteNonQuery();
				}
				
				tx.Commit();
				Interlocked.Increment(ref appendCount);
				WriteAppendsCountIntoLog();
			}
		}
	}
}
