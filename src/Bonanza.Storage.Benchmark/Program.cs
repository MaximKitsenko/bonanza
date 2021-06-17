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

			var configuration = GetRawConfiguration();
			var config = GetTypedConfiguration(configuration);
			ConfigureLogging(configuration);

			var test = new PgSqlEventStoreTest2(Log.Logger);
			test.SendStreamBatchesToEventStore(
				config.BenchmarkConfig.BatchesCount,
				config.BenchmarkConfig.StreamsCountInBatch,
				config.BenchmarkConfig.EventCountInStream,
				config.BenchmarkConfig.EventsInBatchPrefixName,
				config.ConnectionString,
				config.BenchmarkConfig.DropDb);

			//while (true)
			{
				Console.WriteLine("Press Enter to exit. ");
				Console.ReadLine();
			}

		}

		private static AppConfig GetTypedConfiguration(IConfigurationRoot configuration)
		{
			//var rmqHost = config["RabbitMQ:Host"];
			var cfg = configuration.Get<AppConfig>();
			return cfg;
		}

		private static IConfigurationRoot GetRawConfiguration()
		{
			var configurationBuilder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json");

			var configuration = configurationBuilder
				.Build();
			return configuration;
		}

		private static void ConfigureLogging(IConfigurationRoot configuration)
		{
			//var configurationBuilder = new ConfigurationBuilder()
			//	.AddJsonFile("appsettings.json");

			//var configuration = configurationBuilder
			//	.Build();

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
