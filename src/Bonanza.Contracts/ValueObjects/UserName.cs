namespace Bonanza.Contracts.ValueObjects
{
	public class UserName
	{
		public string Name { get; }

		public UserName(string name)
		{
			Name = name;
		}
	}
}