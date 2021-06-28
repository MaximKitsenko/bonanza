using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Bonanza.Storage.Benchmark.TestData;
using Serilog;

namespace Bonanza.Storage.Benchmark
{
	public class PgSqlEventStoreSmokeTest
	{
		private readonly ILogger _logger;

		public PgSqlEventStoreSmokeTest(ILogger logger)
		{
			_logger = logger;
		}

		public void SendStreamBatchToEventStore(
			StreamsBatch fromStreamsBatch, 
			IAppendOnlyStore eventStore, 
			int tenantId, 
			bool cacheConnection)
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
						eventStore.Append(streamName, fromStreamsBatch.Data, streamNameAndVersion.Version, tenantId);

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

		private void WriteLog(
			int eventsStored, 
			Stopwatch sw)
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

		public void SendStreamBatchesToEventStore(
			int batchesCount,
			int batchesStartsFrom,
			int streamsInBatchCount,
			int eventCountPerStream,
			string eventsInBatchPrefixName,
			int dataSize,
			int streamInBatchStartsFrom,
			IAppendOnlyStore eventStore)
		{
			var data = new byte[dataSize];
			var tasks = new List<Task>();
			for (var i = batchesStartsFrom; i < batchesStartsFrom+batchesCount; i++)
			{
				var temp = i;
				var task = Task.Run((() => AppendBatchToEventStore(
					streamsInBatchCount,
					eventCountPerStream,
					eventsInBatchPrefixName,
					temp,
					streamInBatchStartsFrom,
					eventStore,
					data)));
				tasks.Add(task);
			}

			Task.WaitAll(tasks.ToArray());
		}

		private static void AppendBatchToEventStore(
			int streamsInBatchCount,
			int eventCountPerStream,
			string eventsInBatchPrefixName,
			int tenantId,
			int streamInBatchStartsFrom,
			IAppendOnlyStore eventStore,
			byte[] data)
		{
			var streamNameAndVersion = new Dictionary<string, int>();
			for (int j = 0; j < eventCountPerStream; j++)
			{
				for (int k = streamInBatchStartsFrom; k < streamInBatchStartsFrom + streamsInBatchCount; k++)
				{
					try
					{
						var streamName = $"{eventsInBatchPrefixName}-tenant-{tenantId:D5}-stream-{k:D7}";
						if (!streamNameAndVersion.TryGetValue(streamName, out var version))
						{
							version = -1;
						}
						eventStore.Append(streamName, data, version, tenantId);
						streamNameAndVersion[streamName] = version + 1;
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
}
