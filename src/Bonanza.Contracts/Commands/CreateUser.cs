using System;
using System.Collections.Generic;
using System.Text;
using Bonanza.Contracts.ValueObjects;
using Bonanza.Contracts.ValueObjects.User;
using Bonanza.Infrastructure;

namespace Bonanza.Contracts.Commands
{
	public class CreateUser : ICommand
	{
		public readonly UserId UserId;
		public readonly UserName UserName;

		public CreateUser(UserName userName, UserId userId)
		{
			this.UserName = userName;
			this.UserId = userId;
		}
	}
}
