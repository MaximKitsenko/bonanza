using FluentAssertions;

namespace Bonanza.Contracts.ValueObjects.Tenant
{
	public class TenantName
	{
		public string Name { get; }

		public TenantName(string name)
		{
			name.Should().NotBeNullOrWhiteSpace("name");
			Name = name.Trim();
		}
	}
}
