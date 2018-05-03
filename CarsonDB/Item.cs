using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Item : CarsonBackend
	{
		private enum ItemFieldOrdinals
		{
			Id,
			ItemCode,
			ItemRecd,
			ItemDescription
		}

		public static class ItemFields
		{
			public const string Id = "ID";
			public const string ItemCode = "ITEM_CODE";
			public const string ItemRecd = "ITEM_RECD";
			public const string ItemDescription = "ITEM_DESCRIPTION";
		}

		public class ItemData : ICrc
		{
			private int _recordNumber;
			private Item _item;

			public ItemData(Item item, int recordNumber)
			{
				_item = item;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int itemId = 0;

					try
					{
						itemId = _item.IntegerFieldValue(ItemFieldOrdinals.Id, _recordNumber);
						return itemId;
					}
					catch (RecordLockedException)
					{
						itemId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return itemId;
					}
				}
			}

			public string ItemCode
			{
				get
				{
					return _item.StringFieldValue(ItemFieldOrdinals.ItemCode, _recordNumber);
				}
			}

			public string ItemRecd
			{
				get
				{
					return _item.CharacterFieldValue(ItemFieldOrdinals.ItemRecd, _recordNumber);
				}
			}

			public string ItemDescription
			{
				get
				{
					return _item.StringFieldValue(ItemFieldOrdinals.ItemDescription, _recordNumber);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Item()
		{
			AddDatabaseDefinitions(CarsonStaticSettings.DatabasePath);
		}

		public Item(string databasePath)
		{
			AddDatabaseDefinitions(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "ITEM";
			this.DatabasePath = databasePath;
			this.TableId = TableInstance.Item;

			this.AddFieldDefinition(ItemFields.Id, ItemFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(ItemFields.ItemCode, ItemFieldOrdinals.ItemCode, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(ItemFields.ItemRecd, ItemFieldOrdinals.ItemRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(ItemFields.ItemDescription, ItemFieldOrdinals.ItemDescription, AVImarkDataType.AVImarkDynamicString);
		}

		public List<ItemData> ItemList()
		{
			List<ItemData> itemList = new List<ItemData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				ItemData itemData = new ItemData(this, x);
				itemList.Add(itemData);
			}

			return itemList;
		}
	}
}
