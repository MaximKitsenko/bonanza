using System;
using System.Collections.Generic;
using System.Text;

namespace Bonanza.Storage
{
	public interface IEventStore
	{
		EventStream LoadEventStream(IIdentity id);
		EventStream LoadEventStream(
			IIdentity id,
			long skipEvents,
			int maxCount);
		/// <summary>
		/// Appends events to server stream for the provided
		/// identity.
		/// </summary>
		/// <param name="id">identity to append to.</param>
		/// <param name="expectedVersion">Expected version
		/// (specify -1 to append anyway).</param>
		/// <param name="events">Events to append</param>
		/// <exception cref="OptimisticConcurrencyException">
		/// when new events were added to server 
		/// since <paramref name="expectedVersion"/>
		/// </exception> 
		void AppendToStream(
			IIdentity id,
			long expectedVersion,
			ICollection<IEvent> events);
	}

	public class EventStream
	{
		// version of the event stream returned
		public long Version;
		// all events in the stream 
		public List<IEvent> Events = new List<IEvent>();
	}

	// todo: implement optimistic concurrency exception
	public class OptimisticConcurrencyException : Exception
	{
		internal static Exception Create(long version, long expectedStreamVersion, IIdentity id, List<IEvent> events)
		{
			throw new NotImplementedException();
		}
	}

	// todo: implement optimistic concurrency exception
	public class RealConcurrencyException : Exception
	{

	}
}
