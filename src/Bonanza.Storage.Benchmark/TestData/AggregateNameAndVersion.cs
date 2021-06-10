namespace Bonanza.Storage.Benchmark.TestData
{
	public class AggregateNameAndVersion
	{
		public string name { get; set; }
		public int version { get; set; }

		public AggregateNameAndVersion(string name, int version)
		{
			this.name = name;
			this.version = version;
		}
	}
}
