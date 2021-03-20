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
		public void Handle(TenantCreated message)
		{
			BullShitDatabase.TenantList.Add( new TenantListDto(message.TenantId.Id, message.TenantName.Name));
		}

		public void Handle(TenantNameChanged message)
		{
			var tenant = BullShitDatabase.TenantList.FirstOrDefault(x => x.Id == message.TenantId.Id);
			tenant.Name = message.NewName.Name;
		}
	}
}
