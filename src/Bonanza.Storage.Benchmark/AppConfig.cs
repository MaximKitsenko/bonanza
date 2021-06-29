using Bonanza.Storage.PostgreSql;

namespace Bonanza.Storage.Benchmark
{
	public class AppConfig
	{
		public BenchmarkConfig BenchmarkConfig { get; set; }
		public PgSqlEventStoreConfig PgSqlEventStoreConfig { get; set; }
		public PgSql1IndexEventStoreConfig PgSql1IndexEventStoreConfig { get; set; }
		public PgSql2IndexesEventStoreConfig PgSql2IndexesEventStoreConfig { get; set; }
		public PgSqlConstrainedEventStoreConfig PgSqlConstrainedEventStoreConfig { get; set; }
		public TimescaleDbEventStoreConfig TimescaleDbEventStoreConfig { get; set; }
		public SqLiteEventStoreConfig SqLiteEventStoreConfig { get; set; }
	}

	public class BenchmarkConfig
	{
		public int BatchesCount { get; set; }
		public int BatchesStartsFrom { get; set; }
		public int DataSize { get; set; }
		public int StreamsCountInBatch { get; set; }
		public int EventCountInStream { get; set; }
		public string EventsInBatchPrefixName { get; set; }
		public bool DropDb { get; set; }
		public EngineEnum Engine { get; set; }
		public int StreamInBatchStartsFrom { get; set; }
	}

	public class PgSql2IndexesEventStoreConfig
	{
		public AppendStrategy Strategy { get; set; }
		public string ConnectionString { get; set; }
		public int LogEveryNEvents { get; set; }
		public bool CacheConnection { get; set; }
	}

	public class PgSql1IndexEventStoreConfig
	{
		public AppendStrategy Strategy { get; set; }
		public string ConnectionString { get; set; }
		public int LogEveryNEvents { get; set; }
		public bool CacheConnection { get; set; }
	}

	public class PgSqlEventStoreConfig
	{
		public AppendStrategy Strategy { get; set; }
		public string ConnectionString { get; set; }
		public int LogEveryNEvents { get; set; }
		public bool CacheConnection { get; set; }
	}

	public class PgSqlConstrainedEventStoreConfig
	{
		public AppendStrategy Strategy { get; set; }
		public string ConnectionString { get; set; }
		public int LogEveryNEvents { get; set; }
		public bool CacheConnection { get; set; }
	}

	public class TimescaleDbEventStoreConfig
	{
		public AppendStrategy Strategy { get; set; }
		public string ConnectionString { get; set; }
		public int LogEveryNEvents { get; set; }
		public bool CacheConnection { get; set; }
	}

	public class SqLiteEventStoreConfig
	{
		public AppendStrategy Strategy { get; set; }
		public string ConnectionString { get; set; }
		public int LogEveryNEvents { get; set; }
		public bool CacheConnection { get; set; }
	}

	public enum EngineEnum
	{
		PostgreSql,
		PostgreSql1Index,
		PostgreSql2Indexes,
		PgSqlConstrained,
		TimescaleDb,
		SqLite,
		Lmdb,
	}
}
