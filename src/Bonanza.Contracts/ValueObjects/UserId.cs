using System;

namespace Bonanza.Contracts.ValueObjects
{
	public class UserId
	{
		public Guid Id { get; }

		public static UserId SystemUserId => _systemUserId;

		protected static UserId _systemUserId = new UserId(Guid.Empty);

		public UserId(Guid id)
		{
			Id = id;
		}
	}
}