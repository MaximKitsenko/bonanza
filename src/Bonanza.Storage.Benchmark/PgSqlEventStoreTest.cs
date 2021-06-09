using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonanza.Storage.Benchmark
{
	class PgSqlEventStoreTest
	{
		public void SendManyEvents()
		{
			var connectionString = "Host=localhost;Database=bonanza-test-db;Username=root;Password=root";
			var eventStore = new Bonanza.Storage.PostgreSql.PgSqlEventStore(connectionString);
			var res = eventStore.ReadRecords(0, 1000).ToList();

			var data = new byte[]{1,2,3,4,5,6};
			eventStore.Append("Tenant-1", data,0);

			var res2 = eventStore.ReadRecords(0, 1000).ToList();

		}
	}
}
