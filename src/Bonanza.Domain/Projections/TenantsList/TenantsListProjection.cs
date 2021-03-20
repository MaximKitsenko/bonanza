using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Bonanza.Contracts.Events;
using Bonanza.Contracts.ValueObjects.Tenant;
using Bonanza.Infrastructure;

namespace Bonanza.Domain.Projections.TenantsList
{
	public class TenantsListProjection :
		Handles<TenantCreated>,
		Handles<TenantNameChanged>
	{
		readonly ITenantsListDocumentWriter _store;

		public TenantsListProjection(ITenantsListDocumentWriter store)
		{
			_store = store;
		}

		public void Handle(TenantCreated message)
		{
			_store.Add(TenantId.CreateSystemId(), () => new TenantsListDto(message.Id, message.Name));
		}

		public void Handle(TenantNameChanged message)
		{
			_store.UpdateOrThrow(TenantId.CreateSystemId(), v => v.RenameTenantInList(message.Id, message.NewName));
		}
	}

	public class TenantsListDto
	{
		public Dictionary<TenantId,TenantName> Tenants { get; }

		public TenantsListDto(TenantId id, TenantName name)
		{
			Tenants = new Dictionary<TenantId, TenantName>(){{id, name}};
		}

		public TenantsListDto RenameTenantInList(TenantId tId, TenantName tenantName)
		{
			Tenants[tId] = tenantName;
			return this;
		}
	}

	public interface IDocumentWriter<in TKey, TEntity>
	{
		TEntity Add(TKey key, Func<TEntity> addFactory);
		TEntity AddOrUpdate(TKey key, Func<TEntity> addFactory, Func<TEntity, TEntity> update, AddOrUpdateHint hint = AddOrUpdateHint.ProbablyExists);
		TEntity UpdateOrThrow(TKey key, Func<TEntity, TEntity> update, AddOrUpdateHint hint = AddOrUpdateHint.ProbablyExists);
		bool TryDelete(TKey key);
	}

	public interface ITenantsListDocumentWriter : IDocumentWriter<TenantId, TenantsListDto>
	{

	}

	public class TenantsListDocumentWriter: InMemoryDocumentWriter<TenantId, TenantsListDto>, ITenantsListDocumentWriter
	{

	}

	public class InMemoryDocumentWriter<TKey, TEntity> : IDocumentWriter<TKey, TEntity>
	{
		private static ConcurrentDictionary<TKey, TEntity> _store = new ConcurrentDictionary<TKey, TEntity>();

		public TEntity Add(TKey key, Func<TEntity> addFactory)
		{
			var entity = addFactory();
			_store.TryAdd(key, entity);
			return entity;
		}

		public TEntity AddOrUpdate(TKey key, Func<TEntity> addFactory, Func<TEntity, TEntity> update, AddOrUpdateHint hint = AddOrUpdateHint.ProbablyExists)
		{
			var entity = _store.AddOrUpdate(key, x => addFactory(), (k, v) => update(v));
			return entity;
		}

		public bool TryDelete(TKey key)
		{
			throw new NotImplementedException();
		}

		public TEntity UpdateOrThrow(TKey key, Func<TEntity, TEntity> update, AddOrUpdateHint hint = AddOrUpdateHint.ProbablyExists)
		{
			throw new NotImplementedException();
		}
	}

	public enum AddOrUpdateHint
	{
		ProbablyExists,
		ProbablyDoesNotExist
	}
}
