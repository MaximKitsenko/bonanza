namespace Bonanza.Contracts.ValueObjects
{
	public class TenantName
	{
		public string Name { get; }

		public TenantName(string name)
		{
			Name = name;
		}
	}
}