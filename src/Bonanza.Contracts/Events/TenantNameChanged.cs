using Bonanza.Contracts.ValueObjects;
using Bonanza.Contracts.ValueObjects.Tenant;
using Bonanza.Infrastructure;

namespace Bonanza.Contracts.Events
{
    public class TenantNameChanged : Event
    {
        public TenantId TenantId { get; }

        public TenantName NewName { get; }

        public SysInfo SysInfo { get; }

        public TenantNameChanged(TenantId tenantId, TenantName newName, SysInfo sysInfo)
        {
            TenantId = tenantId;
            NewName = newName;
            SysInfo = sysInfo;
        }
    }
}
