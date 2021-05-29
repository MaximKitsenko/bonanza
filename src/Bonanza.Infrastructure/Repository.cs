using System;

namespace Bonanza.Infrastructure
{
	public class Repository<T> : IRepository<T> where T : AggregateRoot, new() //shortcut you can do as you see fit with new()
	{
		private readonly IEventStore _storage;

		public Repository(IEventStore storage)
		{
			_storage = storage;
		}

		public void Save(AggregateRoot aggregate, int expectedVersion)
		{
			_storage.SaveEvents(aggregate.Id, aggregate.GetUncommittedChanges(), expectedVersion);
		}

		/// <summary>
		/// The sequence of Events associated with a specific identity is usually referred to as an Event Stream
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public T GetById(long id)
		{
			var obj = new T();//lots of ways to do this
			var e = _storage.GetEventsForAggregate(id);
			obj.LoadsFromHistory(e);
			return obj;
		}
	}
}
