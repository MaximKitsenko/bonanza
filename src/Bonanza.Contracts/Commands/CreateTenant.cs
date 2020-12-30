using System;
using System.Collections.Generic;
using System.Text;
using Bonanza.Contracts.ValueObjects;
using Bonanza.Contracts.ValueObjects.Tenant;
using Bonanza.Infrastructure;

namespace Bonanza.Contracts.Commands
{
    public class CreateTenant : Command
    {
        public readonly TenantId TenantId;
        public readonly TenantName TenantName;

        public CreateTenant(TenantName tenantName, TenantId tenantId)
        {
            TenantName = tenantName;
            TenantId = tenantId;
        }
    }
}
