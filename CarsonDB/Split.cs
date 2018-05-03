using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Split : CarsonBackend
	{
		private enum SplitFieldOrdinals
		{
			Id,
			SplitRecd,
			SplitClient,
			SplitPercent,
			SplitEndDate,
			SplitAnimal,
			SplitUltimateAuthority
		}

		public static class SplitFields
		{
			public const string Id = "ID";
			public const string SplitRecd = "SPLIT_RECD";
			public const string SplitClient = "SPLIT_CLIENT";
			public const string SplitPercent = "SPLIT_PERCENT";
			public const string SplitEndDate = "SPLIT_ENDDATE";
			public const string SplitAnimal = "SPLIT_ANIMAL";
			public const string SplitUltimateAuthority = "SPLIT_ULTIMATEAUTHORITY";
		}

		public class SplitData : ICrc
		{
			private int _recordNumber;
			private Split _split;

			public SplitData(Split split, int recordNumber)
			{
				_split = split;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int splitId = 0;

					try
					{
						splitId = _split.IntegerFieldValue(SplitFieldOrdinals.Id, _recordNumber);
						return splitId;
					}
					catch (RecordLockedException)
					{
						splitId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return splitId;
					}
				}
			}

			public string SplitRecd
			{
				get
				{
					return _split.CharacterFieldValue(SplitFieldOrdinals.SplitRecd, _recordNumber);
				}
			}

			public int SplitClient
			{
				get
				{
					return _split.IntegerFieldValue(SplitFieldOrdinals.SplitClient, _recordNumber);
				}
			}

			public int SplitPercent
			{
				get
				{
					return _split.IntegerFieldValue(SplitFieldOrdinals.SplitPercent, _recordNumber);
				}
			}

			public DateTime SplitEndDate
			{
				get
				{
					return _split.DateFieldValue(SplitFieldOrdinals.SplitEndDate, _recordNumber);
				}
			}

			public int SplitAnimal
			{
				get
				{
					return _split.IntegerFieldValue(SplitFieldOrdinals.SplitAnimal, _recordNumber);
				}
			}

			public bool SplitUltimateAuthority
			{
				get
				{
					return _split.BooleanFieldValue(SplitFieldOrdinals.SplitUltimateAuthority, _recordNumber);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Split()
		{
			AddDatabaseDefinitions(CarsonStaticSettings.DatabasePath);
		}

		public Split(string databasePath)
		{
			AddDatabaseDefinitions(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "SPLIT";
			this.DatabasePath = databasePath;
			this.TableId = TableInstance.Split;

			this.AddFieldDefinition(SplitFields.Id, SplitFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(SplitFields.SplitRecd, SplitFieldOrdinals.SplitRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(SplitFields.SplitClient, SplitFieldOrdinals.SplitClient, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(SplitFields.SplitPercent, SplitFieldOrdinals.SplitPercent, AVImarkDataType.AVImarkByte);
			this.AddFieldDefinition(SplitFields.SplitEndDate, SplitFieldOrdinals.SplitEndDate, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(SplitFields.SplitAnimal, SplitFieldOrdinals.SplitAnimal, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(SplitFields.SplitUltimateAuthority, SplitFieldOrdinals.SplitUltimateAuthority, AVImarkDataType.AVImarkBool);
		}

		public List<SplitData> SplitList()
		{
			List<SplitData> splitList = new List<SplitData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				SplitData splitData = new SplitData(this, x);
				splitList.Add(splitData);
			}

			return splitList;
		}
	}
}
