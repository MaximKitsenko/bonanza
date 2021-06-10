﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace Bonanza.Storage.Benchmark
{
	[MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter]
	public class PgSqlEventStoreTest
	{
		private byte[][] data = new byte[3][];
		private Dictionary<string, AggregateNameAndVersion> aggregatesIds;
		//private static long eventsStored = 0;

		//[Params(1000, 10_000, 100_000, 1_000_000, 10_000_000)]
		[Params(100, 1_000)]
		public int EventsCount { get; set; }

		[Params(DataSizeEnum._1KByte, DataSizeEnum._10KBytes, DataSizeEnum._100KBytes)]
		//[Params(0, 1, 2)]
		public DataSizeEnum DataSize { get; set; }

		public PgSqlEventStoreTest()
		{
			Console.WriteLine("qwe");
			// data in bytes;
			data[(int)DataSizeEnum._1KByte] = Enumerable.Range(1, 1_024).Select(x=> (byte) (x % byte.MaxValue)).ToArray();
			data[(int)DataSizeEnum._10KBytes] = Enumerable.Range(1, 10_240).Select(x => (byte)(x % byte.MaxValue)).ToArray();
			data[(int)DataSizeEnum._100KBytes] = Enumerable.Range(1, 102_400).Select(x => (byte)(x % byte.MaxValue)).ToArray();
			aggregatesIds = Enumerable
				.Range(1, 1_000_000)
				.Select(x => "Tenant-" + x)
				.ToDictionary(x => x, y => new AggregateNameAndVersion {name = y, version = -1});
		}

		[Benchmark]
		public void SendManyEvents()
		{
			Console.WriteLine("qwe2");
			var connectionString = "Host=localhost;Database=bonanza-test-db;Username=root;Password=root";
			var eventStore = new PostgreSql.PgSqlEventStore(connectionString);
			//eventStore.Initialize(true);
			var eventsStored = -1;
			var sw = new Stopwatch();
			sw.Start();
			using (var enumerator = aggregatesIds.GetEnumerator())
			{
				while (enumerator.MoveNext() && eventsStored < EventsCount)
				{
					var aggregatesId = enumerator.Current;
					eventStore.Append(aggregatesId.Key, data[(int)DataSize], aggregatesIds[aggregatesId.Key].version++);
					//aggregatesIds[aggregatesId.Key] = aggregatesIds[aggregatesId.Key] + 1;
					eventsStored++;
					if (eventsStored % 1000 == 0)
					{
						var time = sw.ElapsedMilliseconds+1;
						var perf = (int)((1000 * 1_000.0) / time);
						Console.WriteLine("{0:D10} events processed, speed: {1:D10} appends/sec", eventsStored, perf);
						sw.Restart();
					}
				}
			}
		}
	}

	public class AggregateNameAndVersion
	{
		public string name { get; set; }
		public int version { get; set; }
	}

	public enum DataSizeEnum
	{
		_1KByte = 0,
		_10KBytes = 1,
		_100KBytes = 2,
	}
}
