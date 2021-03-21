using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Bonanza.Contracts.Events;
using Bonanza.Contracts.ValueObjects.Tenant;
using Bonanza.Infrastructure;

namespace Bonanza.Domain.Projections.TenantsList
{
	public class TenantsListProjection :
		Handles<TenantCreated>,
		Handles<TenantNameChanged>
	{
		private IBullShitDatabase _bullShitDatabase;

		public TenantsListProjection(IBullShitDatabase bullShitDatabase)
		{
			_bullShitDatabase = bullShitDatabase;
		}

		public void Handle(TenantCreated message)
		{
			_bullShitDatabase.TenantList.Add( new TenantListDto(message.TenantId.Id, message.TenantName.Name));
		}

		public void Handle(TenantNameChanged message)
		{
			var tenant = _bullShitDatabase.TenantList.FirstOrDefault(x => x.Id == message.TenantId.Id);
			tenant.Name = message.NewName.Name;
		}
	}
}
