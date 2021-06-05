using System;
using Bonanza.Infrastructure.Abstractions;
using FluentAssertions;

namespace Bonanza.Contracts.ValueObjects.Tenant
{
	public sealed class TenantId : AbstractIdentity<long>, IIdentity
    {
        private const long SystemId = long.MaxValue - 1;

        public override long Id { get; protected set; }

        public override string GetTag()
        {
            return "tenant";
        }

        public static TenantId CreateSystemId()
        {
            return new TenantId(SystemId);
        }

        public TenantId(long id)
        {
            id.Should().BeGreaterThan(0, "Tried to assemble non-existent tenant");

            this.Id = id;
        }
    }

	public static class TenantIdExtensions
	{
		public static Guid ToGuid(this TenantId tenantId)
		{
			var bytes = BitConverter.GetBytes(tenantId.Id);
			var bytesExtended = new byte[16];
			bytes.CopyTo(bytesExtended, 0);
			var guid = new Guid(bytesExtended);
			return guid;
		}
	}
}
