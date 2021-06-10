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
using Bonanza.Storage.PostgreSql;

namespace Bonanza.Storage.Benchmark
{
	public class PgSqlEventStoreTest2
	{
		public void SendManyEvents(PgSqlEventStoreTestData testData, PgSqlEventStore eventStore)
		{
			Console.WriteLine($"{nameof(PgSqlEventStoreTest2)}{nameof(SendManyEvents)}");
			var eventsStored = -1;
			var sw = Stopwatch.StartNew();
			var sw2 = Stopwatch.StartNew();
			while (eventsStored < testData.ContinueUntil)
			{
				using (var eventsEnumerator = testData.AggregateIds.GetEnumerator())
				{
					while (eventsEnumerator.MoveNext() && eventsStored < testData.ContinueUntil)
					{
						try
						{
							var aggregatesId = eventsEnumerator.Current;
							eventStore.Append(aggregatesId.Key, testData.Data, testData.AggregateIds[aggregatesId.Key].version++, true);
							eventsStored++;

							if (eventsStored != 0 && eventsStored % 100 == 0)
							{
								var time = sw2.ElapsedMilliseconds + 1;
								var perf = (int)((1000 * 1_00.0) / time);
								Console.WriteLine("Thread-{0}, {1:D10} events processed, speed: {2:D10} appends/sec", System.Threading.Thread.CurrentThread.ManagedThreadId, eventsStored, perf);
								sw2.Restart();
							}

							if (eventsStored != 0 && eventsStored % 1000 == 0)
							{
								var time = sw.ElapsedMilliseconds + 1;
								var perf = (int)((1000 * 1_000.0) / time);
								Console.WriteLine("Thread-{0}, {1:D10} events processed, speed: {2:D10} appends/sec", System.Threading.Thread.CurrentThread.ManagedThreadId, eventsStored, perf);
								sw.Restart();

							}
						}
						catch (Exception e)
						{
							Console.WriteLine(e);
							throw;
						}
					}
				}
			}
		}

		public void RunMany()
		{
			var testSet1 = PgSqlEventStoreTestData.Generate(1_0, "Tenant", 1_000_000, true, DataSizeEnum._1KByte);
			var testSet2 = PgSqlEventStoreTestData.Generate(1_0, "Order", 1_000_000, false, DataSizeEnum._1KByte);
			var testSet3 = PgSqlEventStoreTestData.Generate(1_0, "Call", 1_000_000, false, DataSizeEnum._1KByte);
			var testSet4 = PgSqlEventStoreTestData.Generate(1_0, "Dish", 1_000_000, false, DataSizeEnum._1KByte);
			var testCases = new List<PgSqlEventStoreTestData>(){ testSet1, testSet2, testSet3, testSet4};

			var connectionString = "Host=localhost;Database=bonanza-test-db;Username=root;Password=root";
			var eventStore = new PostgreSql.PgSqlEventStore(connectionString);
			eventStore.Initialize(true);

			var testRuns = new List<Task>();
			Task.Delay(10000).Wait();
			for (int i = 0; i < testCases.Count; i++)
			{
				var temp = i;
				testRuns.Add(Task.Run(() => (new PgSqlEventStoreTest2()).SendManyEvents(testCases[temp], eventStore)));
			}

			Task.WaitAll(testRuns.ToArray());
		}
	}
}
