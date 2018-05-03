using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Table : CarsonBackend
	{
		private enum TableFieldOrdinals
		{
			Id,
			TableRecd,
			TableCode,
			TableName
		}

		public static class TableFields
		{
			public const string Id = "ID";
			public const string TableRecd = "TABLE_RECD";
			public const string TableCode = "TABLE_CODE";
			public const string TableName = "TABLE_NAME";
		}

		public class TableData : ICrc
		{
			private int _recordNumber;
			private Table _table;

			public TableData(Table table, int recordNumber)
			{
				_table = table;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int tableId = 0;

					try
					{
						tableId = _table.IntegerFieldValue(TableFieldOrdinals.Id, _recordNumber);
						return tableId;
					}
					catch (RecordLockedException)
					{
						tableId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return tableId;
					}
				}
			}

			public string TableRecd
			{
				get
				{
					return _table.CharacterFieldValue(TableFieldOrdinals.TableRecd, _recordNumber);
				}
			}

			public string TableCode
			{
				get
				{
					return _table.StringFieldValue(TableFieldOrdinals.TableCode, _recordNumber);
				}
			}

			public string TableName
			{
				get
				{
					return _table.StringFieldValue(TableFieldOrdinals.TableName, _recordNumber);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Table()
		{
			AddDatabaseDefinitions(CarsonStaticSettings.DatabasePath);
		}

		public Table(string databasePath)
		{
			AddDatabaseDefinitions(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "TABLE";
			this.DatabasePath = databasePath;
			this.TableId = TableInstance.Table;

			this.AddFieldDefinition(TableFields.Id, TableFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(TableFields.TableRecd, TableFieldOrdinals.TableRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(TableFields.TableCode, TableFieldOrdinals.TableCode, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(TableFields.TableName, TableFieldOrdinals.TableName, AVImarkDataType.AVImarkDynamicString);
		}

		public List<TableData> TableList()
		{
			List<TableData> tableList = new List<TableData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				TableData tableData = new TableData(this, x);
				tableList.Add(tableData);
			}

			return tableList;
		}
	}
}
