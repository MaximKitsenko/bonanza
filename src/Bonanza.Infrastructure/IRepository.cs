using System;

namespace Bonanza.Infrastructure
{
	public interface IRepository<T> where T : AggregateRoot, new()
	{
		void Save(AggregateRoot aggregate, int expectedVersion);
		T GetById(long id);
	}
}
