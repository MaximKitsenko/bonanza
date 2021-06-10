using System.Collections.Generic;
using System.Linq;

namespace Bonanza.Storage.Benchmark.TestData
{
	public class PgSqlEventStoreTestData
	{
		public Dictionary<string, AggregateNameAndVersion> Events { get; }
		public int ContinueUntil { get; }
		public bool InitDb { get; }
		public byte[] Data { get; }

		public PgSqlEventStoreTestData(
			Dictionary<string, AggregateNameAndVersion> events, 
			int continueUntil, 
			bool initDb, 
			byte[] data)
		{
			this.Events = events;
			this.ContinueUntil = continueUntil;
			this.InitDb = initDb;
			this.Data = data;
		}


		public static PgSqlEventStoreTestData Generate(
			int aggregatesCount, 
			string AggregatePrefix,
			int continueUntil,
			bool InitDb,
			DataSizeEnum dataSize
		)
		{
			var dataDummy = new byte[(int)dataSize];
			var tenantEvents = Enumerable.Range(1, aggregatesCount).ToDictionary(
				x => "Tenant-" + x,
				y => new AggregateNameAndVersion( "Tenant-" + y, -1));

			return new PgSqlEventStoreTestData(tenantEvents, continueUntil, InitDb, dataDummy);
		}
	}
}
