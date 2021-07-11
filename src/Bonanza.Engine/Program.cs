using System;
using System.Collections.Generic;
using Bonanza.Domain.Aggregates.TenantAggregate;
using Bonanza.Infrastructure;
using Bonanza.Storage;
using Serilog;
using ICommand = Bonanza.Infrastructure.ICommand;

namespace Bonanza.Engine
{
    class Program
    {
        static void Main(string[] args)
		{
			var store = CreateEventStore();
			var events = new Bonanza.Storage.EventStore(store);

			var server = new ApplicationServer();
			server.Handlers.Add(new LoggingWrapper(new TenantApplicationService(events, null)));
		}

        private static IAppendOnlyStore CreateEventStore()
        {
	        var temp = new Bonanza.Storage.PostgreSql.PgSqlEventStore(
		        "Host=localhost;Database=bonanza-test-db;Username=root;Password=root",
		        Log.Logger,
		        100,
		        AppendStrategy.OnePhase,
		        true,
		        false);

	        return temp.Initialize(false);
        }
	}

	/// <summary>
	/// This is a simplified representation of real application server. 
	/// In production it is wired to messaging and/or services infrastructure.</summary>
	public sealed class ApplicationServer
    {
	    public void Dispatch(ICommand cmd)
	    {
		    foreach (var handler in Handlers)
		    {
			    handler.Execute(cmd);
		    }
	    }

	    public readonly IList<IApplicationService> Handlers = new List<IApplicationService>();
    }
}
