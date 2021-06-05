using System;
using System.Collections.Generic;
using Bonanza.Infrastructure.Abstractions;

namespace Bonanza.Infrastructure
{
	public interface IEventStore
	{
		void SaveEvents(IIdentity aggregateId, IEnumerable<Event> events, int expectedVersion);
		List<Event> GetEventsForAggregate(IIdentity aggregateId);
	}
}
