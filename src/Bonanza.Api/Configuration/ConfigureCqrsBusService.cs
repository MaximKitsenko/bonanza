using Bonanza.Contracts.Commands;
using Bonanza.Contracts.Events;
using Bonanza.Domain.Aggregates;
using Bonanza.Domain.Projections.TenantsList;
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

			var storage = new EventStore(bus);
			var rep = new Repository<Tenant>(storage);
			var tenantCommandHandlers = new TenantCommandHandlers(rep);

			bus.RegisterHandler<CreateTenant>(tenantCommandHandlers.Handle);
			//bus.RegisterHandler<CreateUser>(commands.Handle);
			bus.RegisterHandler<RenameTenant>(tenantCommandHandlers.Handle);

			var tenantsListProjection = new TenantsListProjection();
			bus.RegisterHandler<TenantCreated>(tenantsListProjection.Handle);
			bus.RegisterHandler<TenantNameChanged>(tenantsListProjection.Handle);
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

			return services;
		}
	}
}
