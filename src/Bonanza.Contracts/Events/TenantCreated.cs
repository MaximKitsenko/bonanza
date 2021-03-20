using System;
using Bonanza.Contracts.ValueObjects;
using Bonanza.Contracts.ValueObjects.Tenant;
using Bonanza.Infrastructure;

namespace Bonanza.Contracts.Events
{
    public class TenantCreated : Event
    {
        public TenantId TenantId { get; }

        public TenantName TenantName { get; }

        public SysInfo SysInfo { get; }

        public TenantCreated(TenantId tenantId, TenantName tenantName, SysInfo sysInfo)
        {
            TenantId = tenantId;
            TenantName = tenantName;
            SysInfo = sysInfo;
        }
    }

}
