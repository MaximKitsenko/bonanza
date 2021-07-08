using System;
using System.Collections.Generic;
using System.Linq;

namespace Bonanza.Storage.Benchmark.TestData
{
	public class StreamsBatch
	{
		public Dictionary<string, StreamNameAndVersion> Streams { get; }
		public int StreamMaxVer { get; }
		public bool InitDb { get; }
		public byte[] Data { get; }

		public StreamsBatch(
			Dictionary<string, StreamNameAndVersion> streams, 
			int streamMaxVer, 
			bool initDb, 
			byte[] data)
		{
			this.Streams = streams;
			this.StreamMaxVer = streamMaxVer;
			this.InitDb = initDb;
			this.Data = data;
		}


		public static StreamsBatch Generate(
			int streamsCount, 
			string streamNamePrefix,
			int streamMaxVersion,
			bool initDb,
			DataSizeEnum dataSize
		)
		{
			var dataDummy = new byte[(int)dataSize];
			var rnd = new Random();
			rnd.NextBytes(dataDummy);

			var streams = Enumerable.Range(1, streamsCount).ToDictionary(
				x => streamNamePrefix+"-" + x,
				y => new StreamNameAndVersion(streamNamePrefix+"-" + y, -1));

			return new StreamsBatch(streams, streamMaxVersion, initDb, dataDummy);
		}
	}
}
