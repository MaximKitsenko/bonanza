using Bonanza.Contracts.Commands;
using Bonanza.Contracts.ValueObjects;
using Bonanza.Infrastructure;

namespace Bonanza.Domain.Aggregates
{
	public class TenantCommandHandlers
	{
		private readonly IRepository<Tenant> _repository;

		public TenantCommandHandlers(IRepository<Tenant> repository)
		{
			_repository = repository;
		}

		public void Handle(CreateTenant message)
		{
			var tenant = new Tenant(message.TenantId, message.TenantName);//create an aggregate
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
