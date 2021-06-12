using System;
using System.IO;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Debugging;
using Serilog.Settings.Configuration;

namespace Bonanza.Storage.Benchmark
{
	class Program
	{
		static void Main(string[] args)
		{
			//var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
			//Console.WriteLine(summary.ToString());

			// var test = new PgSqlEventStoreTest();
			// test.DataSize = DataSizeEnum._1KByte;
			// test.EventsCount = 100000;
			// test.SendManyEvents();

			ConfigureLogging();
			var test2 = new PgSqlEventStoreTest2(Log.Logger);
			test2.SendStreamsBatchesToEventStore();
			Console.WriteLine("Press Enter for pause. ");
			Console.ReadLine();

		}

		private static void ConfigureLogging()
		{
			var configurationBuilder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json");

			var configuration = configurationBuilder
				.Build();

			var loggerConfiguration = new LoggerConfiguration()
				.ReadFrom
				.Configuration(configuration);

			Log.Logger = loggerConfiguration
				.CreateLogger();

			var position = new {pos = 1};
			var elapsedMs = 1;
			Log.Logger.Information("LoggerConfigured {@Position} in {Elapsed} ms.", position, elapsedMs);
		}
	}
}
