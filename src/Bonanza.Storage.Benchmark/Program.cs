using System;
using BenchmarkDotNet.Running;

namespace Bonanza.Storage.Benchmark
{
    class Program
    {
        static void Main(string[] args)
		{
			//var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
			//Console.WriteLine(summary.ToString());

			var test = new PgSqlEventStoreTest();
			test.DataSize = DataSizeEnum._1KByte;
			test.EventsCount = 100000;
			test.SendManyEvents();
		}
    }
}
