using System;

namespace Bonanza.Storage.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
			new PgSqlEventStoreTest().SendManyEvents();
            Console.WriteLine("Hello World!");
        }
    }
}
