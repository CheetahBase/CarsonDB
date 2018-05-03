using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Entry : CarsonBackend
	{
		private enum EntryFieldOrdinals
		{
			Id,
			EntryRecd,
			EntryCode,
			EntryDescription,
			EntryTable
		}

		public static class EntryFields
		{
			public const string Id = "ID";
			public const string EntryRecd = "ENTRY_RECD";
			public const string EntryCode = "ENTRY_CODE";
			public const string EntryDescription = "ENTRY_DESCRIPTION";
			public const string EntryTable = "ENTRY_TABLE";
		}

		public class EntryData : ICrc
		{
			private int _recordNumber;
			private Entry _entry;

			public EntryData(Entry entry, int recordNumber)
			{
				_entry = entry;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int entryId = 0;

					try
					{
						entryId = _entry.IntegerFieldValue(EntryFieldOrdinals.Id, _recordNumber);
						return entryId;
					}
					catch (RecordLockedException)
					{
						entryId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return entryId;
					}
				}
			}

			public string EntryRecd
			{
				get
				{
					return _entry.CharacterFieldValue(EntryFieldOrdinals.EntryRecd, _recordNumber);
				}
			}

			public string EntryCode
			{
				get
				{
					return _entry.StringFieldValue(EntryFieldOrdinals.EntryCode, _recordNumber);
				}
			}

			public string EntryDescription
			{
				get
				{
					return _entry.StringFieldValue(EntryFieldOrdinals.EntryDescription, _recordNumber);
				}
			}

			public int EntryTable
			{
				get
				{
					return _entry.IntegerFieldValue(EntryFieldOrdinals.EntryTable, _recordNumber);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Entry()
		{
			AddDatabaseDefinitions(CarsonStaticSettings.DatabasePath);
		}

		public Entry(string databasePath)
		{
			AddDatabaseDefinitions(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "ENTRY";
			this.DatabasePath = databasePath;
			this.TableId = TableInstance.Entry;

			this.AddFieldDefinition(EntryFields.Id, EntryFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(EntryFields.EntryRecd, EntryFieldOrdinals.EntryRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(EntryFields.EntryCode, EntryFieldOrdinals.EntryCode, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(EntryFields.EntryDescription, EntryFieldOrdinals.EntryDescription, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(EntryFields.EntryTable, EntryFieldOrdinals.EntryTable, AVImarkDataType.AVImarkByte);
		}

		public List<EntryData> EntryList()
		{
			List<EntryData> entryList = new List<EntryData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				EntryData entryData = new EntryData(this, x);
				entryList.Add(entryData);
			}

			return entryList;
		}
	}
}
