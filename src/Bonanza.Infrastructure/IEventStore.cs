using System;
using System.Collections.Generic;

namespace Bonanza.Infrastructure
{
	public interface IEventStore
	{
		void SaveEvents(long aggregateId, IEnumerable<Event> events, int expectedVersion);
		List<Event> GetEventsForAggregate(long aggregateId);
	}
}
