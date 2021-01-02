using System;
using System.Collections.Generic;

namespace Bonanza.Infrastructure
{
	public interface IReadModelFacade
	{
		IEnumerable<InventoryItemListDto> GetInventoryItems();
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

	public class ReadModelFacade : IReadModelFacade
	{
		public IEnumerable<InventoryItemListDto> GetInventoryItems()
		{
			return BullShitDatabase.list;
		}

		public InventoryItemDetailsDto GetInventoryItemDetails(Guid id)
		{
			return BullShitDatabase.details[id];
		}
	}

	public static class BullShitDatabase
	{
		public static Dictionary<Guid, InventoryItemDetailsDto> details = new Dictionary<Guid, InventoryItemDetailsDto>();
		public static List<InventoryItemListDto> list = new List<InventoryItemListDto>();
	}
}
