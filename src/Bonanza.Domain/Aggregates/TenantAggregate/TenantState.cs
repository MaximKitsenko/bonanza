using System.Collections.Generic;
using Bonanza.Contracts.Events;
using Bonanza.Contracts.ValueObjects.Tenant;
using Bonanza.Infrastructure;

namespace Bonanza.Domain.Aggregates.TenantAggregate
{
	/// <summary>
	/// This is the state of the tenant aggregate.
	/// It can be mutated only by passing events to it.
	/// </summary>
	public class TenantState
	{
		public TenantName Name { get; private set; }
		public bool Created { get; private set; }
		public TenantId Id { get; private set; }

		public int MaxTransactionId { get; private set; }

		public TenantState(IEnumerable<IEvent> events)
		{
			foreach (var e in events)
			{
				Mutate(e);
			}
		}

		public void When(TenantCreated e)
		{
			Created = true;
			Name = e.TenantName;
			Id = e.TenantId;
		}

		public void When(TenantRenamed e)
		{
			Name = e.NewName;
		}

		public void Mutate(IEvent e)
		{
			// .NET magic to call one of the 'When' handlers with 
			// matching signature 
			((dynamic)this).When((dynamic)e);
		}
	}
}
