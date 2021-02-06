using Bonanza.Contracts.ValueObjects;
using Bonanza.Contracts.ValueObjects.Tenant;
using Bonanza.Contracts.ValueObjects.User;

namespace Bonanza.Contracts.Events
{
	public class UserCreated
	{
		public TenantId TenantId { get; }

		public UserId UserId { get; }

		public UserName Name { get; }

		public SysInfo SysInfo { get; }

		public UserCreated(TenantId tenantId, UserId userId, UserName userName, SysInfo sysInfo)
		{
			TenantId = tenantId;
			UserId = userId;
			Name = userName;
			SysInfo = sysInfo;
		}
	}
}
