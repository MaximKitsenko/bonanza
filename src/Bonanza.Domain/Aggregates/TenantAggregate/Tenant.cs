using System;
using System.Collections.Generic;
using Bonanza.Contracts.Events;
using Bonanza.Contracts.ValueObjects;
using Bonanza.Contracts.ValueObjects.Tenant;
using Bonanza.Infrastructure;

namespace Bonanza.Domain.Aggregates.TenantAggregate
{
	/// <summary>
	/// <para>Implementation of customer aggregate. In production it is loaded and 
	/// operated by an <see cref="TenantApplicationService"/>, which loads it from
	/// the event storage and calls appropriate methods, passing needed arguments in.</para>
	/// <para>In test environments (e.g. in unit tests), this aggregate can be
	/// instantiated directly.</para>
	/// </summary>
	public class Tenant
	{
		/// <summary> List of uncommitted changes </summary>
		public readonly IList<IEvent> Changes = new List<IEvent>();

		/// <summary>
		/// Aggregate state, which is separate from this class in order to ensure,
		/// that we modify it ONLY by passing events.
		/// </summary>
		readonly TenantState _state;

		public Tenant(IEnumerable<IEvent> events)
		{
			_state = new TenantState(events);
		}

		void Apply(IEvent e)
		{
			// append event to change list for further persistence
			Changes.Add(e);
			// pass each event to modify current in-memory state
			_state.Mutate(e);
		}


		public void Create(TenantId id, TenantName name, IPricingService service, DateTime utc)
		{
			if (_state.Created)
				throw new InvalidOperationException("Customer was already created");

			Apply(new TenantCreated(
				id,
				name,
				SysInfo.CreateSysInfo(id))
			);

			var bonus = service.GetWelcomeBonus();

			if (bonus > 0)
				AddPayment("Welcome bonus", bonus, utc);
		}

		public void Rename(TenantName name, DateTime dateTime)
		{
			if (_state.Name == name)
				return;
			Apply(new TenantRenamed
			(
				_state.Id,
				name,
				_state.Name,
				SysInfo.CreateSysInfo(_state.Id)
			));
		}

		public void AddPayment(string name, decimal amount, DateTime utc)
		{
			//Apply(new CustomerPaymentAdded()
			//{
			//	Id = _state.Id,
			//	Payment = amount,
			//	NewBalance = _state.Balance + amount,
			//	PaymentName = name,
			//	Transaction = _state.MaxTransactionId + 1,
			//	TimeUtc = utc
			//});
		}
	}
}
