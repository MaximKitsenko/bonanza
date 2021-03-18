using System;
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
		readonly IDocumentWriter<TenantId, TenantsListDto> _store;

		public void Handle(TenantCreated message)
		{
			_store.Add(TenantId.CreateSystemId(), () => new TenantsListDto(message.Id, message.Name));
		}

		public void Handle(TenantNameChanged message)
		{
			_store.UpdateOrThrow(TenantId.CreateSystemId(), v => v.RenameTenantInList(message.Id, message.NewName));
		}
	}

	internal class TenantsListDto
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

	public enum AddOrUpdateHint
	{
		ProbablyExists,
		ProbablyDoesNotExist
	}
}
