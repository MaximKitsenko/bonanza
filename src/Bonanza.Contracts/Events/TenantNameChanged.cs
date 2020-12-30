using Bonanza.Contracts.ValueObjects;
using Bonanza.Contracts.ValueObjects.Tenant;
using Bonanza.Infrastructure;

namespace Bonanza.Contracts.Events
{
    public class TenantNameChanged : Event
    {
        public TenantId Id { get; }

        public TenantName NewName { get; }

        public SysInfo SysInfo { get; }

        public TenantNameChanged(TenantId id, TenantName newName, SysInfo sysInfo)
        {
            Id = id;
            NewName = newName;
            SysInfo = sysInfo;
        }
    }
}
