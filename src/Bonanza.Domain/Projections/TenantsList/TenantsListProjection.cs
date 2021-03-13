using Bonanza.Contracts.Events;
using Bonanza.Infrastructure;

namespace Bonanza.Domain.Projections.TenantsList
{
	public class TenantsListProjection : 
		Handles<TenantCreated>, 
		Handles<TenantNameChanged>, 
		Handles<UserCreated>
	{
		public void Handle(TenantCreated message)
		{
			throw new System.NotImplementedException();
		}

		public void Handle(TenantNameChanged message)
		{
			throw new System.NotImplementedException();
		}

		public void Handle(UserCreated message)
		{
			throw new System.NotImplementedException();
		}
	}
}
