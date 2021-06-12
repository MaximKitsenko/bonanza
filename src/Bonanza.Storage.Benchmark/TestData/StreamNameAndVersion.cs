namespace Bonanza.Storage.Benchmark.TestData
{
	public class StreamNameAndVersion
	{
		public string Name { get; private set; }
		public int Version { get; private set; }

		public StreamNameAndVersion(string name, int version)
		{
			this.Name = name;
			this.Version = version;
		}

		public void VersionIncrement()
		{
			this.Version++;
		}
	}
}
