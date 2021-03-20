using System;
using Bonanza.Contracts.ValueObjects;
using Bonanza.Contracts.ValueObjects.Tenant;
using Bonanza.Infrastructure;

namespace Bonanza.Contracts.Events
{
    public class TenantCreated : Event
    {
        public TenantId Id { get; }

        public TenantName Name { get; }

        public SysInfo SysInfo { get; }

        public TenantCreated(TenantId id, TenantName name, SysInfo sysInfo)
        {
            Id = id;
            Name = name;
            SysInfo = sysInfo;
        }
    }

}
