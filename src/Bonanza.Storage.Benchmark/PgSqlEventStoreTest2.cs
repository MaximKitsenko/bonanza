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
using Serilog;

namespace Bonanza.Storage.Benchmark
{
	public class PgSqlEventStoreTest2
	{
		private readonly ILogger _logger;

		public PgSqlEventStoreTest2(ILogger logger)
		{
			_logger = logger;
		}

		public void SendStreamBatchToEventStore(StreamsBatch fromStreamsBatch, PgSqlEventStore eventStore)
		{
			_logger.Information(
				"Started {method}", 
				$"{nameof(SendStreamBatchToEventStore)}");

			var eventsStored = -1;
			var sw = Stopwatch.StartNew();
			for (int i = 0; i < fromStreamsBatch.StreamMaxVer; i++)
			{
				foreach (var (streamName, streamNameAndVersion) in fromStreamsBatch.Streams)
				{
					try
					{
						eventStore.Append(streamName, fromStreamsBatch.Data, streamNameAndVersion.Version, true);

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
			_logger.Information(
				"Finished {method}",
				$"{nameof(SendStreamBatchToEventStore)}");
		}

		private void WriteLog(int eventsStored, Stopwatch sw)
		{
			const int batchSize = 1000;
			if (eventsStored != 0 && eventsStored % batchSize == 0)
			{
				var elapsedMilliseconds = sw.ElapsedMilliseconds + 1;
				var elapsedSeconds =  elapsedMilliseconds / 1_000.0;
				var perf = (int) (batchSize / elapsedSeconds);

				_logger.Information(
					"Traced {method}, thread-{thread:D10}, events stored {eventsCount:D10}, speed {speed:D10} op/sec",
					$"{nameof(SendStreamBatchToEventStore)}",
					System.Threading.Thread.CurrentThread.ManagedThreadId,
					eventsStored,
					perf);

				sw.Restart();
			}
		}

		public StreamsBatch[] GenerateTestCases(int testCasesCount,
			int streamsCount,
			int eventsPerStream,
			string streamNamePrefix, 
			bool dropDb)
		{
			var testCases = new StreamsBatch[testCasesCount];
			for (var i = 0; i < testCases.Length; i++)
			{
				testCases[i] = StreamsBatch.Generate(streamsCount, $"{streamNamePrefix}{i:D7}", eventsPerStream, dropDb, DataSizeEnum._1KByte);
			}

			return testCases;
		}

		public void SendStreamsBatchesToEventStore()
		{
			const int streamsCount = 1_000_000;
			const int eventsPerStream = 50;
			const string streamNamePrefix = "TestCase";
			const int batchCount = 10;
			const string connectionString = "Host=localhost;Database=bonanza-test-db-002;Username=root;Password=root";
			const bool dropDb = false;

			var streamsBatches = GenerateTestCases(batchCount, streamsCount, eventsPerStream, streamNamePrefix, dropDb);

			var eventStore = new PostgreSql.PgSqlEventStore(connectionString).Initialize(dropDb);

			var testRuns = new List<Task>();
			Task.Delay(10000).Wait();
			for (int i = 0; i < streamsBatches.Length; i++)
			{
				var temp = i;
				//testRuns.Add(Task.Run(() => (new PgSqlEventStoreTest2(this._logger)).AppendManyEvents(testCases[temp], eventStore)));
				testRuns.Add(Task.Run(() => this.SendStreamBatchToEventStore(streamsBatches[temp], eventStore)));
			}

			Task.WaitAll(testRuns.ToArray());
		}
	}
}
