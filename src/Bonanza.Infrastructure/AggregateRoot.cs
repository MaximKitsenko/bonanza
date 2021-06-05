using System.Collections.Generic;
using Bonanza.Infrastructure.Abstractions;

namespace Bonanza.Infrastructure
{
	public abstract class AggregateRoot
	{
		private readonly List<Event> _changes = new List<Event>();

		public abstract long Id { get; }
		public int Version { get; internal set; }

		public IEnumerable<Event> GetUncommittedChanges()
		{
			return _changes;
		}

		public void MarkChangesAsCommitted()
		{
			_changes.Clear();
		}

		public void LoadsFromHistory(IEnumerable<Event> history)
		{
			foreach (var e in history)
			{
				this.Version = e.Version;
				// or? this.Version++;
				// or? #1
				ApplyChange(e, false);
			}
		}

		protected void ApplyChange(Event @event)
		{
			// #1
			// this.Version += 1;
			ApplyChange(@event, true);
		}

		// push atomic aggregate changes to local history for further processing (EventStore.SaveEvents)
		private void ApplyChange(Event @event, bool isNew)
		{
			//((dynamic) this).Apply(@event);
			// .NET magic to call one of the 'When' handlers with 
			// matching signature 

			((dynamic)this).Apply(@event as dynamic);
			//((dynamic)this).Apply( (dynamic )@event );
			//((dynamic)this).Apply(@event);

			// we need to add events to changes only for new events,
			// events received during dehydration should be skipped, to avoid duplication
			if (isNew) _changes.Add(@event);
		}
	}
}
