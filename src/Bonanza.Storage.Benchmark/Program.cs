using System;
using System.IO;
using BenchmarkDotNet.Running;
using Bonanza.Storage.PostgreSql;
using Bonanza.Storage.SqLite;
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

			var test = new PgSqlEventStoreSmokeTest(Log.Logger);

			var eventStore = CreateEventStore(config);

			test.SendStreamBatchesToEventStore(
				config.BenchmarkConfig.BatchesCount,
				config.BenchmarkConfig.BatchesStartsFrom,
				config.BenchmarkConfig.StreamsCountInBatch,
				config.BenchmarkConfig.EventCountInStream,
				config.BenchmarkConfig.EventsInBatchPrefixName,
				config.BenchmarkConfig.DataSize,
				config.BenchmarkConfig.StreamInBatchStartsFrom,
				eventStore);

			//while (true)
			{
				Console.WriteLine("Press Enter to exit. ");
				Console.ReadLine();
			}
		}

		private static IAppendOnlyStore CreateEventStore(AppConfig config)
		{
			return config.BenchmarkConfig.Engine switch
			{
				EngineEnum.SqLite => (IAppendOnlyStore)new SqLiteEventStore(
						config.SqLiteEventStoreConfig.ConnectionString,
						Log.Logger,
						config.SqLiteEventStoreConfig.LogEveryNEvents,
						config.SqLiteEventStoreConfig.Strategy,
						config.SqLiteEventStoreConfig.TenantIdInStreamName,
						config.SqLiteEventStoreConfig.CacheConnection)
					.Initialize(config.BenchmarkConfig.DropDb),

				EngineEnum.PostgreSql => new PostgreSql.PgSqlEventStore(
						config.PgSqlEventStoreConfig.ConnectionString,
						Log.Logger,
						config.PgSqlEventStoreConfig.LogEveryNEvents,
						config.PgSqlEventStoreConfig.Strategy,
						config.PgSqlEventStoreConfig.TenantIdInStreamName, 
						config.PgSqlEventStoreConfig.CacheConnection)
					.Initialize(config.BenchmarkConfig.DropDb),

				EngineEnum.PostgreSql1Index => new PostgreSqlWith1Index.PgSql1IndexEventStore(
						config.PgSql1IndexEventStoreConfig.ConnectionString,
						Log.Logger,
						config.PgSql1IndexEventStoreConfig.LogEveryNEvents,
						config.PgSql1IndexEventStoreConfig.Strategy,
						config.PgSql1IndexEventStoreConfig.TenantIdInStreamName,
						config.PgSql1IndexEventStoreConfig.CacheConnection)
					.Initialize(config.BenchmarkConfig.DropDb),

				EngineEnum.PostgreSql2Indexes => new PostgreSql2Indexes.PgSql2IndexesEventStore(
						config.PgSql2IndexesEventStoreConfig.ConnectionString,
						Log.Logger,
						config.PgSql2IndexesEventStoreConfig.LogEveryNEvents,
						config.PgSql2IndexesEventStoreConfig.Strategy,
						config.PgSql2IndexesEventStoreConfig.TenantIdInStreamName,
						config.PgSql2IndexesEventStoreConfig.CacheConnection)
					.Initialize(config.BenchmarkConfig.DropDb),

				EngineEnum.PgSqlConstrained => new Bonanza.Storage.PostgreSqlWithConstraint.PgSqlConstrainedEventStore(
						config.PgSqlConstrainedEventStoreConfig.ConnectionString,
						Log.Logger,
						config.PgSqlConstrainedEventStoreConfig.LogEveryNEvents,
						config.PgSqlConstrainedEventStoreConfig.Strategy,
						config.PgSqlConstrainedEventStoreConfig.TenantIdInStreamName,
						config.PgSqlConstrainedEventStoreConfig.CacheConnection)
					.Initialize(config.BenchmarkConfig.DropDb),

				EngineEnum.TimescaleDb => new Timescale.TimescaleEventStore(
						config.TimescaleDbEventStoreConfig.ConnectionString,
						Log.Logger,
						config.TimescaleDbEventStoreConfig.LogEveryNEvents,
						config.TimescaleDbEventStoreConfig.Strategy,
						config.TimescaleDbEventStoreConfig.TenantIdInStreamName,
						config.TimescaleDbEventStoreConfig.CacheConnection
						)
					.Initialize(config.BenchmarkConfig.DropDb),

				EngineEnum.GEventStore => new GregEventStore.GEventStore(
						config.GEventStoreConfig.ConnectionString,
						Log.Logger,
						config.GEventStoreConfig.LogEveryNEvents,
						config.GEventStoreConfig.Strategy,
						config.GEventStoreConfig.TenantIdInStreamName,
						config.GEventStoreConfig.CacheConnection
					)
					.Initialize(config.BenchmarkConfig.DropDb),

				_ => new PostgreSql.PgSqlEventStore(
						config.PgSqlEventStoreConfig.ConnectionString,
						Log.Logger,
						config.PgSqlEventStoreConfig.LogEveryNEvents,
						config.PgSqlEventStoreConfig.Strategy, 
						config.PgSqlEventStoreConfig.TenantIdInStreamName, 
						config.PgSqlEventStoreConfig.CacheConnection)
					.Initialize(config.BenchmarkConfig.DropDb)
			};
		}

		private static AppConfig GetTypedConfiguration(IConfigurationRoot configuration)
		{
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
