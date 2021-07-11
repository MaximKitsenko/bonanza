using System;
using System.Collections.Generic;
using Bonanza.Contracts.Commands;
using Bonanza.Contracts.ValueObjects;
using Bonanza.Contracts.ValueObjects.Tenant;
using Bonanza.Contracts.ValueObjects.User;
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

			server.Dispatch(new CreateTenant( new TenantName("qwe"), new TenantId(1) ));
			server.Dispatch(new RenameTenant(new TenantName("asd"), new TenantId(1), SysInfo.CreateSysInfo(new TenantId(1)) ));
			server.Dispatch(new CreateUser( new UserName("asd"),new UserId(1) ) );


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
