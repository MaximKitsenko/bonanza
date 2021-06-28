using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Npgsql;
using Serilog;

namespace Bonanza.Storage.PostgreSqlWithConstraint
{
	/// <summary>
	/// <para>This is a SQL event storage for PgSql, simplified to demonstrate 
	/// essential principles.
	/// If you need more robust SQL implementation, check out Event Store of
	/// Jonathan Oliver</para>
	/// <para>This code is frozen to match IDDD book. For latest practices see Lokad.CQRS Project</para>
	/// </summary>
	public sealed class PgSqlEventStore : IAppendOnlyStore
	{
		readonly string _connectionString;
		private ConcurrentQueue<NpgsqlConnection> _connections;
		private ILogger _logger;
		private int _logEveryEventsCount;
		private int appendCount = 0;
		private Stopwatch sw = Stopwatch.StartNew();
		private Action<string, byte[], long, NpgsqlConnection> _appendMethod;
		private bool _cacheConnection;

		private Action<string, byte[], long, NpgsqlConnection> ChooseStrategy(AppendStrategy strategy)
		{
			var dict = new Dictionary<AppendStrategy, Action<string, byte[], long, NpgsqlConnection>>()
			{
				{AppendStrategy.OnePhase, Append1Phase},
				{AppendStrategy.OnePhaseNoVersionCheck, Append1PhaseNoVersionCheck},
				{AppendStrategy.TwoPhases, Append2Phases},
			};

			var choosenStrategy = dict[strategy];
			return choosenStrategy;
		}

		public PgSqlEventStore(string connectionString, ILogger logger, int logEveryEventsCount, AppendStrategy strategy, bool cacheConnection)
		{
			_connectionString = connectionString;
			_logger = logger;
			_cacheConnection = cacheConnection;
			_logEveryEventsCount = logEveryEventsCount;
			_connections = new ConcurrentQueue<NpgsqlConnection>();
			_appendMethod = ChooseStrategy(strategy);
			logger.Information($"[PgSqlEventStore] strategy used: {strategy.ToString()}");
		}

		public PgSqlEventStore Initialize(bool dropDb)
		{
			using (var conn = new NpgsqlConnection(_connectionString))
			{
				conn.Open();
				const string dropTable = @"DROP TABLE IF EXISTS es_events;";
				const string createTable = @"CREATE TABLE IF NOT EXISTS es_events (Id SERIAL,tenantid INT NOT NULL,Name VARCHAR (50) NOT NULL,Version INT NOT NULL,Data BYTEA NOT NULL);";
				const string createIdx = @"CREATE INDEX IF NOT EXISTS ""name-idx"" ON public.es_events USING btree(tenantid,name COLLATE pg_catalog.""default"" ASC NULLS LAST)TABLESPACE pg_default;";
				const string createFunction = @"
CREATE OR REPLACE FUNCTION AppendEvent(expectedVersion bigint, aggregateName text, data bytea)
RETURNS int AS 
$$ -- here start procedural part
   DECLARE currentVer int;
   BEGIN
		SELECT INTO currentVer COALESCE(MAX(version),-1)
				FROM public.es_events
				WHERE name = aggregateName;
		IF expectedVersion <> -1 THEN
			IF currentVer <> expectedVersion THEN
				RETURN currentVer;
			END IF;
		END IF;
		INSERT INTO public.es_events (Name,Version,Data) VALUES(aggregateName,currentVer+1,data);
				RETURN currentVer;
				--RETURN 0;
   END;
$$ -- here finish procedural part
LANGUAGE plpgsql; -- language specification ";

				const string createTableSql = 
				createTable 
				+ createIdx 
				+ createFunction;
				const string dropTableCreateTableSql = 
					dropTable 
					+ createTable 
					+ createIdx 
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
					_appendMethod(name, data, expectedVersion, conn);
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
					_appendMethod(name, data, expectedVersion, conn);
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

		public void Append2Phases(string name, byte[] data, long expectedVersion, NpgsqlConnection conn)
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

		public void Append1Phase(string name, byte[] data, long expectedVersion, NpgsqlConnection conn)
		{
			try
			{
				using (var tx = conn.BeginTransaction())
				{
					const string sql =
						@"SELECT appendevent(@expectedVersion,@name,@data)";

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

		public void Append1PhaseNoVersionCheck(string name, byte[] data, long expectedVersion, NpgsqlConnection conn)
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
