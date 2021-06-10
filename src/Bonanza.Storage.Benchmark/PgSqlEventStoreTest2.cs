using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Toolchains.Results;
using Bonanza.Storage.Benchmark.TestData;

namespace Bonanza.Storage.Benchmark
{
	public class PgSqlEventStoreTest2
	{
		public void SendManyEvents(PgSqlEventStoreTestData testData)
		{
			Console.WriteLine($"{nameof(PgSqlEventStoreTest2)}{nameof(SendManyEvents)}");
			var connectionString = "Host=localhost;Database=bonanza-test-db;Username=root;Password=root";
			var eventStore = new PostgreSql.PgSqlEventStore(connectionString);
			eventStore.Initialize(testData.InitDb);
			var eventsStored = -1;
			var sw = new Stopwatch();
			sw.Start();
			using (var eventsEnumerator = testData.Events.GetEnumerator())
			{
				while (eventsEnumerator.MoveNext() && eventsStored < testData.ContinueUntil)
				{
					var aggregatesId = eventsEnumerator.Current;
					eventStore.Append(aggregatesId.Key, testData.Data, testData.Events[aggregatesId.Key].version++, true);
					eventsStored++;
					if (eventsStored != 0 && eventsStored % 1000 == 0)
					{
						var time = sw.ElapsedMilliseconds+1;
						var perf = (int)((1000 * 1_000.0) / time);
						Console.WriteLine("Thread-{0}, {1:D10} events processed, speed: {2:D10} appends/sec",System.Threading.Thread.CurrentThread.ManagedThreadId, eventsStored, perf);
						sw.Restart();
					}
				}
			}
		}

		public void RunMany()
		{
			var testSet1 = PgSqlEventStoreTestData.Generate(1_000, "Tenant", 1_000_000, true, DataSizeEnum._1KByte);
			var testSet2 = PgSqlEventStoreTestData.Generate(1_000, "Order", 1_000_000, false, DataSizeEnum._1KByte);
			var testSet3 = PgSqlEventStoreTestData.Generate(1_000, "Call", 1_000_000, false, DataSizeEnum._1KByte);
			var testSet4 = PgSqlEventStoreTestData.Generate(1_000, "Dish", 1_000_000, false, DataSizeEnum._1KByte);
			var testCases = new List<PgSqlEventStoreTestData>(){ testSet1, testSet2, testSet3, testSet4};

			var testRuns = new List<Task>();
			Task.Delay(10000).Wait();
			for (int i = 0; i < testCases.Count; i++)
			{
				var temp = i;
				testRuns.Add(Task.Run(() => (new PgSqlEventStoreTest2()).SendManyEvents(testCases[temp])));
				if (i == 0)
				{
					Task.Delay(10000);
				}
			}

			Task.WaitAll(testRuns.ToArray());
		}
	}
}
