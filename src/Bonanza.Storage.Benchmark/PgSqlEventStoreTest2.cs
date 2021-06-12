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
		public void SendManyEvents(TestCase testCase, PgSqlEventStore eventStore)
		{
			Console.WriteLine($"{nameof(PgSqlEventStoreTest2)}{nameof(SendManyEvents)}");
			var eventsStored = -1;
			var sw = Stopwatch.StartNew();
			for (int i = 0; i < testCase.StreamMaxVer; i++)
			{
				foreach (var (streamName, streamNameAndVersion) in testCase.Streams)
				{
					try
					{
						eventStore.Append(streamName, testCase.Data, streamNameAndVersion.Version, true);

						streamNameAndVersion.VersionIncrement();
						eventsStored++;

						WriteLog(eventsStored, sw);
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						throw;
					}
				}
			}
		}

		private static void WriteLog(int eventsStored, Stopwatch sw)
		{
			if (eventsStored != 0 && eventsStored % 1000 == 0)
			{
				var time = sw.ElapsedMilliseconds + 1;
				var perf = (int) ((1000 * 1_000.0) / time);
				Console.WriteLine("Thread-{0}, {1:D10} events processed, speed: {2:D10} appends/sec",
					System.Threading.Thread.CurrentThread.ManagedThreadId, eventsStored, perf);
				sw.Restart();
			}
		}

		public void RunMany()
		{
			var testSet1 = TestCase.Generate(100_000, "Tenant_2", 5_000_000, false, DataSizeEnum._1KByte);
			var testSet2 = TestCase.Generate(100_000, "Order_2", 5_000_000, false, DataSizeEnum._1KByte);
			var testSet3 = TestCase.Generate(100_000, "Call_2", 5_000_000, false, DataSizeEnum._1KByte);
			var testSet4 = TestCase.Generate(100_000, "Dish_2", 5_000_000, false, DataSizeEnum._1KByte);
			var testSet5 = TestCase.Generate(100_000, "Something_2", 5_000_000, false, DataSizeEnum._1KByte);
			var testSet6 = TestCase.Generate(100_000, "Else_2", 5_000_000, false, DataSizeEnum._1KByte);
			var testCases = new List<TestCase>()
			{
				testSet1, 
				testSet2, 
				testSet3, 
				testSet4,
				testSet5,
				testSet6,
			};

			var connectionString = "Host=localhost;Database=bonanza-test-db2;Username=root;Password=root";
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

		public void RunMany2()
		{
			const int streamsCount = 1_000_000;
			const int eventsPerStream = 50;
			const string streamNamePrefix = "TestCase";
			const int testCasesCount = 10;
			const string connectionString = "Host=localhost;Database=bonanza-test-db-002;Username=root;Password=root";
			const bool dropDb = false;

			var testCases = new TestCase[testCasesCount];
			for(var i = 0; i < testCases.Length; i++)
			{
				testCases[i] = TestCase.Generate(streamsCount, $"{streamNamePrefix}{i:D7}", eventsPerStream, false, DataSizeEnum._1KByte);
			}

			var eventStore = new PostgreSql.PgSqlEventStore(connectionString);
			eventStore.Initialize(dropDb);

			var testRuns = new List<Task>();
			Task.Delay(10000).Wait();
			for (int i = 0; i < testCases.Length; i++)
			{
				var temp = i;
				testRuns.Add(Task.Run(() => (new PgSqlEventStoreTest2()).SendManyEvents(testCases[temp], eventStore)));
			}

			Task.WaitAll(testRuns.ToArray());
		}
	}
}
