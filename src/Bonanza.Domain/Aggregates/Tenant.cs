using System;
using Bonanza.Contracts.Events;
using Bonanza.Contracts.ValueObjects;
using Bonanza.Contracts.ValueObjects.Tenant;
using Bonanza.Infrastructure;

namespace Bonanza.Domain.Aggregates
{
	public class Tenant : AggregateRoot
	{
		private TenantName TenantName;
		private TenantId TenantId;

		public void Apply(TenantCreated e)
		{
			TenantId = e.Id;
			TenantName = e.Name;
		}

		public void Apply(TenantNameChanged e)
		{
			TenantId = e.Id;
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
		public override Guid Id
		{
			get { return Guid.Empty; }
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
