using System;
using Bonanza.Infrastructure.Abstractions;

namespace Bonanza.Infrastructure
{
	public interface IRepository<TAggregate, TAggregateId> 
		where TAggregateId : IIdentity
		where TAggregate : AggregateRoot<TAggregateId>, new()
	{
		void Save(TAggregate aggregate, int expectedVersion);
		TAggregate GetById(IIdentity id);
	}
}
