using System;

namespace Bonanza.Contracts.ValueObjects
{
	public class TenantId
	{
		public Guid Id { get; }

		public TenantId(Guid id)
		{
			Id = id;
		}
	}
}