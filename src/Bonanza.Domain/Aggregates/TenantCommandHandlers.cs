using Bonanza.Contracts.Commands;
using Bonanza.Contracts.ValueObjects;
using Bonanza.Contracts.ValueObjects.Tenant;
using Bonanza.Domain.Projections.TenantsList;
using Bonanza.Infrastructure;

namespace Bonanza.Domain.Aggregates
{
	/// <summary>
	/// The Command Handler effectively replaces the Application Service method,
	/// although it is roughly equivalent and may still be referred to as such.
	/// Implementing DDD
	/// </summary>
	public class TenantCommandHandlers
	{
		private readonly IRepository<Tenant> _repository;
		private IBullShitDatabase _bullShitDatabase;

		public TenantCommandHandlers(IRepository<Tenant> repository, IBullShitDatabase bullShitDatabase)
		{
			_repository = repository;
			_bullShitDatabase = bullShitDatabase;
		}

		public void Handle(CreateTenant message)
		{
			var tenantLastId = _bullShitDatabase.LastId.GetOrAdd(typeof(TenantId), x => 0);
			//create an aggregate
			var tenant = new Tenant(new TenantId(tenantLastId+1), message.TenantName);
			_repository.Save(tenant, -1);
		}

		public void Handle(RenameTenant message)
		{
			var item = _repository.GetById(message.TenantId.Id);
			item.ChangeName(message.TenantNewName, message.SysInfo);
			_repository.Save(item, message.OriginalVersion);
		}
	}
}
