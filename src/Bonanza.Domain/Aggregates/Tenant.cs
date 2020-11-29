using System;
using Bonanza.Contracts.Events;
using Bonanza.Contracts.ValueObjects;
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

		//public void ChangeName(string newName, SysInfo sysInfo)
		//{
		//	if (string.IsNullOrEmpty(newName)) throw new ArgumentException("newName");
		//	this.ApplyChange(new ChatRoomRenamed(_id, newName, DebugMode ? sysInfo : SysInfo.CreateSysInfo(sysInfo.UserId)));
		//}

		public override Guid Id
		{
			get { return TenantId.Id; }
		}

		public Tenant()
		{
			// used to create in repository ... many ways to avoid this, eg making private constructor
		}

		public Tenant(TenantId id, TenantName name)
		{
			ApplyChange(new TenantCreated(id, name, SysInfo.CreateSysInfo()));
		}
	}
}