using System;
using Bonanza.Contracts.Events;
using Bonanza.Contracts.ValueObjects;
using Bonanza.Contracts.ValueObjects.Tenant;
using Bonanza.Infrastructure;

namespace Bonanza.Domain.Aggregates
{
	public class Tenant : AggregateRoot<TenantId>
	{
		private TenantName TenantName;
		private TenantId TenantId;

		public void Apply(TenantCreated e)
		{
			TenantId = e.TenantId;
			TenantName = e.TenantName;
		}

		public void Apply(TenantNameChanged e)
		{
			TenantId = e.TenantId;
			TenantName = e.NewName;
		}

		public void ChangeName(TenantName newName, SysInfo sysInfo)
		{
			if (this.TenantName != null && this.TenantName.Equals(newName))
				return;

			// check there is no tenant with the same name

			this.ApplyChange(new TenantNameChanged(TenantId, newName, sysInfo));
		}

		// todo: make id generic!
		public override TenantId Id
		{
			get { return TenantId; }
		}

		public Tenant()
		{
			// used to create in repository ... many ways to avoid this, eg making private constructor
		}

		public Tenant(TenantId id, TenantName name)
		{
			ApplyChange(new TenantCreated(id, name, SysInfo.CreateSysInfo(TenantId.CreateSystemId())));
		}
	}
}
