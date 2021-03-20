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
			BullShitDatabase.TenantList.Add( new TenantListDto(message.Id.ToGuid(), message.Name.Name));
		}

		public void Handle(TenantNameChanged message)
		{
			var tenant = BullShitDatabase.TenantList.FirstOrDefault(x => x.Id == message.Id.ToGuid());
			tenant.Name = message.NewName.Name;
		}
	}
}
