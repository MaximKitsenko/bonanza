using System;
using FluentAssertions;

namespace Bonanza.Contracts.ValueObjects.User
{
	[Serializable]
	public class UserName
	{
		public string Name { get; }

		public UserName(string name)
		{
			name.Should().NotBeNullOrWhiteSpace("name");
			Name = name;
		}
	}
}
