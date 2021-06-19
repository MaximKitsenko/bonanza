using Bonanza.Storage.PostgreSql;

namespace Bonanza.Storage.Benchmark
{
	public class AppConfig
	{
		public BenchmarkConfig BenchmarkConfig { get; set; }
		public PgSqlEventStoreConfig PgSqlEventStoreConfig { get; set; }
		public SqLiteEventStoreConfig SqLiteEventStoreConfig { get; set; }
	}

	public class BenchmarkConfig
	{
		public int BatchesCount { get; set; }
		public int DataSize { get; set; }
		public int StreamsCountInBatch { get; set; }
		public int EventCountInStream { get; set; }
		public string EventsInBatchPrefixName { get; set; }
		public bool DropDb { get; set; }
		public EngineEnum Engine { get; set; }
	}

	public class PgSqlEventStoreConfig
	{
		public AppendStrategy Strategy { get; set; }
		public string ConnectionString { get; set; }
		public int LogEveryNEvents { get; set; }
	}

	public class SqLiteEventStoreConfig
	{
		public AppendStrategy Strategy { get; set; }
		public string ConnectionString { get; set; }
		public int LogEveryNEvents { get; set; }
	}

	public enum EngineEnum
	{
		PostgreSql,
		SqLite,
	}

	public enum AppendStrategy
	{
		OnePhase,
		OnePhaseNoVersionCheck,
		TwoPhases
	}
}
