using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class ServMemo : CarsonBackend
	{
		private enum ServMemoFieldOrdinals
		{
			Id,
			MemoTextRecd,
			MemoTextText,
			MemoTextNext,
			MemoTextPrev,
			MemoTextParent
		}

		public static class ServMemoFields
		{
			public const string Id = "ID";
			public const string MemoTextRecd = "MEMOTEXT_RECD";
			public const string MemoTextText = "MEMOTEXT_TEXT";
			public const string MemoTextNext = "MEMOTEXT_NEXT";
			public const string MemoTextPrev = "MEMOTEXT_PREV";
			public const string MemoTextParent = "MEMOTEXT_PARENT";
		}

		public class ServMemoData : ICrc
		{
			private int _recordNumber;
			private ServMemo _servMemo;

			public ServMemoData(ServMemo servMemo, int recordNumber)
			{
				_servMemo = servMemo;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int servMemoId = 0;

					try
					{
						servMemoId = _servMemo.IntegerFieldValue(ServMemoFieldOrdinals.Id, _recordNumber);
						return servMemoId;
					}
					catch (RecordLockedException)
					{
						servMemoId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return servMemoId;
					}
				}
			}

			public string MemoTextRecd
			{
				get
				{
					return _servMemo.CharacterFieldValue(ServMemoFieldOrdinals.MemoTextRecd, _recordNumber);
				}
			}

			public string MemoTextText
			{
				get
				{
					return _servMemo.StringFieldValue(ServMemoFieldOrdinals.MemoTextText, _recordNumber);
				}
			}

			public int MemoTextNext
			{
				get
				{
					return _servMemo.IntegerFieldValue(ServMemoFieldOrdinals.MemoTextNext, _recordNumber);
				}
			}

			public int MemoTextPrev
			{
				get
				{
					return _servMemo.IntegerFieldValue(ServMemoFieldOrdinals.MemoTextPrev, _recordNumber);
				}
			}

			public int MemoTextParent
			{
				get
				{
					return _servMemo.IntegerFieldValue(ServMemoFieldOrdinals.MemoTextParent, _recordNumber);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public ServMemo()
		{
			AddDatabaseDefinitions(CarsonStaticSettings.DatabasePath);
		}

		public ServMemo(string databasePath)
		{
			AddDatabaseDefinitions(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "SERVMEMO";
			this.DatabasePath = databasePath;
			this.TableId = TableInstance.ServMemo;

			this.AddFieldDefinition(ServMemoFields.Id, ServMemoFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(ServMemoFields.MemoTextRecd, ServMemoFieldOrdinals.MemoTextRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(ServMemoFields.MemoTextText, ServMemoFieldOrdinals.MemoTextText, AVImarkDataType.AVImarkLongText);
			this.AddFieldDefinition(ServMemoFields.MemoTextNext, ServMemoFieldOrdinals.MemoTextNext, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(ServMemoFields.MemoTextPrev, ServMemoFieldOrdinals.MemoTextPrev, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(ServMemoFields.MemoTextParent, ServMemoFieldOrdinals.MemoTextParent, AVImarkDataType.AVImarkDoubleWord);
		}

		public List<ServMemoData> ServMemoList()
		{
			List<ServMemoData> serviceList = new List<ServMemoData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				ServMemoData serviceData = new ServMemoData(this, x);
				serviceList.Add(serviceData);
			}

			return serviceList;
		}
	}
}
