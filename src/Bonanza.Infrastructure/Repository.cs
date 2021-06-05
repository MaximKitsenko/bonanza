using System;
using Bonanza.Infrastructure.Abstractions;

namespace Bonanza.Infrastructure
{
	public class Repository<TAggregate, TAggregateId> : IRepository<TAggregate, TAggregateId>
		where TAggregateId : IIdentity //shortcut you can do as you see fit with new()
		where TAggregate : AggregateRoot<TAggregateId>, new()
	{
		private readonly IEventStore _storage;

		public Repository(IEventStore storage)
		{
			_storage = storage;
		}

		public void Save(TAggregate aggregate, int expectedVersion)
		{
			_storage.SaveEvents(aggregate.Id, aggregate.GetUncommittedChanges(), expectedVersion);
		}

		/// <summary>
		/// The sequence of Events associated with a specific identity is usually referred to as an Event Stream
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public TAggregate GetById(IIdentity id)
		{
			var obj = new TAggregate();//lots of ways to do this
			var e = _storage.GetEventsForAggregate(id);
			obj.LoadsFromHistory(e);
			return obj;
		}
	}
}
