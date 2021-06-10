using System.Collections.Concurrent;
using System.Collections.Generic;
using Npgsql;

namespace Bonanza.Storage.PostgreSql
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

		public PgSqlEventStore(string connectionString)
		{
			_connectionString = connectionString;
			_connections = new ConcurrentQueue<NpgsqlConnection>();

		}

		public void Initialize(bool dropDb)
		{
			using (var conn = new NpgsqlConnection(_connectionString))
			{
				conn.Open();
				const string dropTable = @"DROP TABLE es_events;";
				const string createTable = @"CREATE TABLE IF NOT EXISTS ES_Events (Id SERIAL,Name VARCHAR (50) NOT NULL,Version INT NOT NULL,Data BYTEA NOT NULL);";
				const string createIdx = @"CREATE INDEX ""name-idx"" ON public.es_events USING btree(name COLLATE pg_catalog.""default"" ASC NULLS LAST)TABLESPACE pg_default;";
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

				const string createTableSql = createTable + createIdx + createFunction;
				const string dropTableCreateTableSql = dropTable + createTable + createIdx + createFunction;

				using (var cmd = new NpgsqlCommand(dropDb? dropTableCreateTableSql:createTableSql, conn))
				{
					cmd.ExecuteNonQuery();
				}
			}
		}

		public void Dispose()
		{

		}

		public void Append(string name, byte[] data, long expectedVersion = -1)
		{
			using (var conn = new NpgsqlConnection(_connectionString))
			{
				conn.Open();
				using (var tx = conn.BeginTransaction())
				{
					const string sql =
						@"SELECT COALESCE (MAX(version),0)
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
					tx.Commit();
				}
			}
		}

		public void Append(string name, byte[] data, long expectedVersion, bool cacheConnection)
		{
			if (cacheConnection)
			{
				NpgsqlConnection conn = null;
				try
				{
					conn = GetFromCacheOrNew();
					Append(name, data, expectedVersion, conn);
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
					Append(name, data, expectedVersion, conn);
				}
			}
		}

		public void Append(string name, byte[] data, long expectedVersion, NpgsqlConnection conn)
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
					version = (int) cmd.ExecuteScalar();
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
	}
}
