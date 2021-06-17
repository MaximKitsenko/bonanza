namespace Bonanza.Storage.Benchmark
{
	public class AppConfig
	{
		public BenchmarkConfig BenchmarkConfig { get; set; }
		public string ConnectionString { get; set; }
	}

	public class BenchmarkConfig
	{
		public int BatchesCount { get; set; }
		public int StreamsCountInBatch { get; set; }
		public int EventCountInStream { get; set; }
		public string EventsInBatchPrefixName { get; set; }
		public bool DropDb { get; set; }
	}
}
