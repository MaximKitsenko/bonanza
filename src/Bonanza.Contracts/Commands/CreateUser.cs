using System;
using System.Collections.Generic;
using System.Text;
using Bonanza.Contracts.ValueObjects;

namespace Bonanza.Contracts.Commands
{
	class CreateUser
    {
        public readonly UserId UserId;
        public readonly UserName UserName;

        public CreateUser(UserName UserName, UserId UserId)
        {
            UserName = UserName;
            UserId = UserId;
        }
    }
}
