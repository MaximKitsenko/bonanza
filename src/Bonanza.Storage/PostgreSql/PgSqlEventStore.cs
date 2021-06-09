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

		public PgSqlEventStore(string connectionString)
		{
			_connectionString = connectionString;

		}

		public void Initialize()
		{
			using (var conn = new NpgsqlConnection(_connectionString))
			{
				conn.Open();

				const string txt = @"
CREATE TABLE IF NOT EXISTS ES_Events (
  Id SERIAL,
  Name VARCHAR (50) NOT NULL,
  Version INT NOT NULL,
  Data BYTEA NOT NULL
)";
				using (var cmd = new NpgsqlCommand(txt, conn))
				{
					cmd.ExecuteNonQuery();
				}
			}
		}

		public void Dispose()
		{

		}

		public void Append(string name, byte[] data, long expectedVersion)
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

		public IEnumerable<DataWithVersion> ReadRecords(string name, long afterVersion, int maxCount)
		{
			using (var conn = new NpgsqlConnection(_connectionString))
			{
				conn.Open();
				const string sql =
					@"SELECT `Data`, `Version` FROM `ES_Events`
                        WHERE `Name` = ?name AND `Version`>?version
                        ORDER BY `Version`
                        LIMIT 0,?take";
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
