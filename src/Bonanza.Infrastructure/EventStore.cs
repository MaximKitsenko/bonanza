using System;
using System.Collections.Generic;
using System.Linq;
using Bonanza.Infrastructure.Abstractions;
using Bonanza.Infrastructure.Exceptions;

namespace Bonanza.Infrastructure
{
    public class EventStore : IEventStore
    {
        private readonly IEventPublisher _publisher;

		private struct EventDescriptor
		{
			public readonly Event EventData;
			public readonly string Id;
			public readonly int Version;

			public EventDescriptor(string id, Event eventData, int version)
			{
				EventData = eventData;
				Version = version;
				Id = id;
			}
		}

		private static string IdentityToKey(IIdentity id)
		{
			return id == null ? "func" : (id.GetTag() + ":" + id.GetId());
		}

		public EventStore(IEventPublisher publisher)
        {
            _publisher = publisher;
        }

        private readonly Dictionary<string, List<EventDescriptor>> _current = new Dictionary<string, List<EventDescriptor>>();

        public void SaveEvents(IIdentity aggregateId, IEnumerable<Event> events, int expectedVersion)
        {
            List<EventDescriptor> eventDescriptors;

            var aggregateIdKey = IdentityToKey(aggregateId);
			// try to get event descriptors list for given aggregate id
			// otherwise -> create empty dictionary
			if (!_current.TryGetValue(aggregateIdKey, out eventDescriptors))
            {
                eventDescriptors = new List<EventDescriptor>();
                _current.Add(aggregateIdKey, eventDescriptors);
            }
            // check whether latest event version matches current aggregate version
            // otherwise -> throw exception
            else if (eventDescriptors[eventDescriptors.Count - 1].Version != expectedVersion && expectedVersion != -1)
            {
                throw new ConcurrencyException();
            }
            var i = expectedVersion;

            // iterate through current aggregate events increasing version with each processed event
            foreach (var @event in events)
            {
                i++;
                @event.Version = i;

                // push event to the event descriptors list for current aggregate
                eventDescriptors.Add(new EventDescriptor(aggregateIdKey, @event, i));

                // publish current event to the bus for further processing by subscribers
                _publisher.Publish(@event);
            }
        }

        // collect all processed events for given aggregate and return them as a list
        // used to build up an aggregate from its history (Domain.LoadsFromHistory)
        public List<Event> GetEventsForAggregate(IIdentity aggregateId)
        {
            List<EventDescriptor> eventDescriptors;
            var aggregateIdKey = IdentityToKey(aggregateId);
			if (!_current.TryGetValue(aggregateIdKey, out eventDescriptors))
            {
                throw new AggregateNotFoundException();
            }

            return eventDescriptors.Select(desc => desc.EventData).ToList();
        }
    }
}
