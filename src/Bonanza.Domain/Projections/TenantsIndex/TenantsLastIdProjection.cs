﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Bonanza.Contracts.Events;
using Bonanza.Contracts.ValueObjects.Tenant;
using Bonanza.Infrastructure;

namespace Bonanza.Domain.Projections.TenantsList
{
	public class TenantsLastIdProjection :
		Handles<TenantCreated>
	{
		public void Handle(TenantCreated message)
		{
			BullShitDatabase.LastId.AddOrUpdate(typeof(TenantId), x => message.TenantId.Id, (x, y) => message.TenantId.Id);
		}
	}
}
