using System.Collections.Generic;
using PgSql.Data.PgSqlClient;

namespace Sample.Storage.PgSql
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
            using (var conn = new PgSqlConnection(_connectionString))
            {
                conn.Open();

                const string txt = @"
CREATE TABLE IF NOT EXISTS ES_Events (
  Id SERIAL,
  Name VARCHAR (50) NOT NULL,
  Version INT NOT NULL,
  Data BYTEA NOT NULL
)";
                using (var cmd = new PgSqlCommand(txt, conn))
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
            using (var conn = new PgSqlConnection(_connectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    const string sql =
                        @"SELECT COALESCE(MAX(""  version""),0)
                            FROM public.es_events
                            WHERE ""  name"" = 'asd';";
                    int version;
                    using (var cmd = new PgSqlCommand(sql, conn, tx))
                    {
                        cmd.Parameters.AddWithValue("?name", name);
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
                           @"INSERT INTO `ES_Events` (`Name`, `Version`, `Data`) 
                                VALUES(?name, ?version, ?data)";

                    using (var cmd = new PgSqlCommand(txt, conn, tx))
                    {
                        cmd.Parameters.AddWithValue("?name", name);
                        cmd.Parameters.AddWithValue("?version", version+1);
                        cmd.Parameters.AddWithValue("?data", data);
                        cmd.ExecuteNonQuery();
                    }
                    tx.Commit();
                }
            }
        }