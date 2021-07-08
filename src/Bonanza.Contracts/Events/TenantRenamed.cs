using Bonanza.Contracts.ValueObjects;
using Bonanza.Contracts.ValueObjects.Tenant;
using Bonanza.Infrastructure;

namespace Bonanza.Contracts.Events
{
    public class TenantRenamed : IEvent
    {
        public TenantId TenantId { get; }

        public TenantName NewName { get; }

        public SysInfo SysInfo { get; }

        public TenantRenamed(TenantId tenantId, TenantName newName, TenantName oldName, SysInfo sysInfo)
        {
            TenantId = tenantId;
            NewName = newName;
            SysInfo = sysInfo;
        }
    }
}
