using Bonanza.Contracts.ValueObjects;
using Bonanza.Contracts.ValueObjects.Tenant;
using Bonanza.Infrastructure;

namespace Bonanza.Contracts.Commands
{
    public class RenameTenant : Command
    {
        public readonly TenantId TenantId;
        public readonly TenantName TenantNewName;

        public RenameTenant(TenantName tenantNewName, TenantId tenantId)
        {
            TenantNewName = tenantNewName;
            TenantId = tenantId;
        }
    }
}
