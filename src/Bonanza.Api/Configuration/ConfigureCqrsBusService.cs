using Bonanza.Api.Controllers;
using Bonanza.Contracts.Commands;
using Bonanza.Contracts.Events;
using Bonanza.Contracts.ValueObjects.Tenant;
using Bonanza.Domain.Aggregates;
using Bonanza.Domain.Aggregates.TenantAggregate;
using Bonanza.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bonanza.Api.Configuration
{
	public static class ConfigureCqrsBusService
	{
		public static IServiceCollection AddCqrsBus(
			this IServiceCollection services)
		{
			var bus = new FakeBus();
			var storage = new Storage.EventStore(bus);
			//var rep = new Repository<Tenant, TenantId>(storage);

			//var bulshitDB = new BullShitDatabase();
			//var readModelFacade = new ReadModelFacade(bulshitDB);

			var tenantCommandHandlers = new TenantCommandHandlers(rep, bulshitDB);

			bus.RegisterHandler<CreateTenant>(tenantCommandHandlers.Handle);
			//bus.RegisterHandler<CreateUser>(commands.Handle);
			bus.RegisterHandler<RenameTenant>(tenantCommandHandlers.Handle);

			//var tenantsListDocWriter = new ServiceDescriptor(
			//	typeof(ITenantsListDocumentWriter),
			//	p => docWriter,
			//	ServiceLifetime.Singleton);
			//services.Add(tenantsListDocWriter);


			var tenantsLastProjection = new TenantsLastIdProjection(bulshitDB);
			bus.RegisterHandler<TenantCreated>(tenantsLastProjection.Handle);

			var tenantsListProjection = new TenantsListProjection(bulshitDB);
			bus.RegisterHandler<TenantCreated>(tenantsListProjection.Handle);
			bus.RegisterHandler<TenantRenamed>(tenantsListProjection.Handle);
			//bus.RegisterHandler<UserCreated>(detail.Handle);

			var serviceCommandSender = new ServiceDescriptor(
				typeof(ICommandSender),
				p => bus,
				ServiceLifetime.Singleton);
			services.Add(serviceCommandSender);

			var serviceEventPublisher = new ServiceDescriptor(
				typeof(IEventPublisher),
				p => bus,
				ServiceLifetime.Singleton);
			services.Add(serviceEventPublisher);

			var bullShitDb = new ServiceDescriptor(
				typeof(IBullShitDatabase),
				p => bulshitDB,
				ServiceLifetime.Singleton);
			services.Add(bullShitDb);

			var readmodelFacade = new ServiceDescriptor(
				typeof(IReadModelFacade),
				p => readModelFacade,
				ServiceLifetime.Singleton);
			services.Add(readmodelFacade);

			return services;
		}
	}
}
