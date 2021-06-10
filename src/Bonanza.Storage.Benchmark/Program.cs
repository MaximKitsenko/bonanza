using System;
using BenchmarkDotNet.Running;
using Bonanza.Storage.Benchmark.TestData;

namespace Bonanza.Storage.Benchmark
{
    class Program
    {
        static void Main(string[] args)
		{
			//var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
			//Console.WriteLine(summary.ToString());

			//var test = new PgSqlEventStoreTest();
			//test.DataSize = DataSizeEnum._1KByte;
			//test.EventsCount = 100000;
			//test.SendManyEventsMultiThread(8);

			var test = new PgSqlEventStoreTest2();
			test.RunMany();
		}
	}
}
