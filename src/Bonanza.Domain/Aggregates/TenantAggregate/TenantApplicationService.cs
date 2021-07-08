using System;
using Bonanza.Contracts.Commands;
using Bonanza.Contracts.ValueObjects.Tenant;
using Bonanza.Infrastructure;
using Bonanza.Storage;

namespace Bonanza.Domain.Aggregates.TenantAggregate
{
	/// <summary><para>
	/// This is an application service within the current bounded context. 
	/// THis specific application service contains command handlers which load
	/// and operate a Customer aggregate. These handlers also pass required
	/// dependencies to aggregate methods and perform conflict resolution 
	/// </para><para>
	/// Command handlers are usually invoked by an infrastructure of an application
	/// server, which hosts current service. Infrastructure will be responsible
	/// for accepting message calls (in form of web service calls or serialized
	///  command messages) and dispatching them to these handlers.
	/// </para></summary>
	public sealed class CustomerApplicationService : IApplicationService
	{
		// event store for accessing event streams
		readonly IEventStore _eventStore;
		// domain service that is neeeded by aggregate
		readonly IPricingService _pricingService;

		// pass dependencies for this application service via constructor
		public CustomerApplicationService(IEventStore eventStore, IPricingService pricingService)
		{
			_eventStore = eventStore;
			_pricingService = pricingService;
		}


		public void When(CreateTenant c)
		{
			Update(c.TenantId, a => a.Create(c.TenantId, c.TenantName, _pricingService, DateTime.UtcNow));
		}

		public void When(RenameTenant c)
		{
			Update(c.TenantId, a => a.Rename(c.TenantNewName, DateTime.UtcNow));
		}

		// method with direct call, as illustrated in the IDDD Book

		// Step 1: LockCustomerForAccountOverdraft method of 

		// Customer Application Service is called  

		//public void LockCustomerForAccountOverdraft(CustomerId customerId, string comment)
		//{
		//	// Step 2.1: Load event stream for Customer, given its id
		//	var stream = _eventStore.LoadEventStream(customerId);
		//	// Step 2.2: Build aggregate from event stream
		//	var customer = new Tenant(stream.Events);
		//	// Step 3: call aggregate method, passing it arguments and pricing domain service
		//	customer.LockForAccountOverdraft(comment, _pricingService);
		//	// Step 4: commit changes to the event stream by id 
		//	_eventStore.AppendToStream(customerId, stream.Version, customer.Changes);
		//}

		public void Execute(ICommand cmd)
		{
			// pass command to a specific method named when
			// that can handle the command
			((dynamic)this).When((dynamic)cmd);
		}

		void Update(TenantId id, Action<Tenant> execute)
		{
			// Load event stream from the store
			EventStream stream = _eventStore.LoadEventStream(id);
			// create new Customer aggregate from the history
			Tenant tenant = new Tenant(stream.Events);
			// execute delegated action
			execute(tenant);
			// append resulting changes to the stream
			_eventStore.AppendToStream(id, stream.Version, tenant.Changes);
		}
		// Sample of method that would apply simple conflict resolution.
		// see IDDD book or Greg Young's videos for more in-depth explanation  
		void UpdateWithSimpleConflictResolution(TenantId id, Action<Tenant> execute)
		{
			while (true)
			{
				EventStream eventStream = _eventStore.LoadEventStream(id);
				Tenant tenant = new Tenant(eventStream.Events);
				execute(tenant);

				try
				{
					_eventStore.AppendToStream(id, eventStream.Version, tenant.Changes);
					return;
				}
				catch (OptimisticConcurrencyException ex)
				{
					foreach (var clientEvent in tenant.Changes)
					{
						foreach (var actualEvent in ex.ActualEvents)
						{
							if (ConflictsWith(clientEvent, actualEvent))
							{
								var msg = string.Format("Conflict between {0} and {1}",
									clientEvent, actualEvent);
								throw new RealConcurrencyException(msg, ex);
							}
						}
					}
					// there are no conflicts and we can append
					_eventStore.AppendToStream(id, ex.ActualVersion, tenant.Changes);
				}
			}
		}

		static bool ConflictsWith(IEvent x, IEvent y)
		{
			return x.GetType() == y.GetType();
		}
	}
}
