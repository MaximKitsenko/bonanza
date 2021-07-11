using Bonanza.Contracts.ValueObjects;
using Bonanza.Contracts.ValueObjects.Tenant;
using Bonanza.Infrastructure;

namespace Bonanza.Contracts.Commands
{
    public class RenameTenant : ICommand
    {
		public readonly TenantId TenantId;
		public readonly TenantName TenantNewName;
        public readonly SysInfo SysInfo;
        //todo: remove it, use optimistic concurency
        //public readonly int OriginalVersion;

		public RenameTenant(TenantName tenantNewName, TenantId tenantId, SysInfo sysInfo/*, int originalVersion*/)
        {
            TenantNewName = tenantNewName;
			TenantId = tenantId;
			SysInfo = sysInfo;
            //OriginalVersion = originalVersion;
        }
    }
}
