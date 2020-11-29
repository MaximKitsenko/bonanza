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
	}
}