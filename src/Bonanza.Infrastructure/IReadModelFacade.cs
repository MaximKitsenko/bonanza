using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Bonanza.Infrastructure
{
	public interface IReadModelFacade
	{
		IEnumerable<InventoryItemListDto> GetInventoryItems();
		IEnumerable<TenantListDto> GetTenants();
		InventoryItemDetailsDto GetInventoryItemDetails(Guid id);
	}

	public class InventoryItemDetailsDto
	{
		public Guid Id;
		public string Name;
		public int CurrentCount;
		public int Version;

		public InventoryItemDetailsDto(Guid id, string name, int currentCount, int version)
		{
			Id = id;
			Name = name;
			CurrentCount = currentCount;
			Version = version;
		}
	}

	public class InventoryItemListDto
	{
		public Guid Id;
		public string Name;

		public InventoryItemListDto(Guid id, string name)
		{
			Id = id;
			Name = name;
		}
	}

	public class TenantListDto
	{
		public long Id;
		public string Name;

		public TenantListDto(long id, string name)
		{
			Id = id;
			Name = name;
		}
	}

	public class ReadModelFacade : IReadModelFacade
	{
		private IBullShitDatabase _bullShitDatabase;

		public ReadModelFacade(IBullShitDatabase bullShitDatabase)
		{
			this._bullShitDatabase = bullShitDatabase;
		}

		public IEnumerable<InventoryItemListDto> GetInventoryItems()
		{
			return _bullShitDatabase.InventoryList;
		}

		public IEnumerable<TenantListDto> GetTenants()
		{
			return _bullShitDatabase.TenantList;
		}

		public InventoryItemDetailsDto GetInventoryItemDetails(Guid id)
		{
			return _bullShitDatabase.details[id];
		}
	}

	public interface IBullShitDatabase
	{
		Dictionary<Guid, InventoryItemDetailsDto> details { get; }
		List<InventoryItemListDto> InventoryList { get; }
		List<TenantListDto> TenantList { get; }
		ConcurrentDictionary<Type, long> LastId { get;}
	}

	public class BullShitDatabase : IBullShitDatabase
	{
		public Dictionary<Guid, InventoryItemDetailsDto> details { get; } = new Dictionary<Guid, InventoryItemDetailsDto>();
		public List<InventoryItemListDto> InventoryList { get; } = new List<InventoryItemListDto>();
		public List<TenantListDto> TenantList { get; } = new List<TenantListDto>();
		public ConcurrentDictionary<Type,long> LastId { get; } = new ConcurrentDictionary<Type, long>();
	}
}
