using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Diagnose : CarsonBackend
	{
		private enum DiagnoseFieldOrdinals
		{
			Id,
			DiagnoseRecd,
			DiagnoseCode,
			DiagnoseDescription
		}

		public static class DiagnoseFields
		{
			public const string Id = "ID";
			public const string DiagnoseRecd = "DIAGNOSE_RECD";
			public const string DiagnoseCode = "DIAGNOSE_CODE";
			public const string DiagnoseDescription = "DIAGNOSE_DESCRIPTION";
		}

		public class DiagnoseData : ICrc
		{
			private int _recordNumber;
			private Diagnose _diagnose;

			public DiagnoseData(Diagnose diagnose, int recordNumber)
			{
				_diagnose = diagnose;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int diagnoseId = 0;

					try
					{
						diagnoseId = _diagnose.IntegerFieldValue(DiagnoseFieldOrdinals.Id, _recordNumber);
						return diagnoseId;
					}
					catch (RecordLockedException)
					{
						diagnoseId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return diagnoseId;
					}
				}
			}

			public string DiagnoseRecd
			{
				get
				{
					return _diagnose.CharacterFieldValue(DiagnoseFieldOrdinals.DiagnoseRecd, _recordNumber);
				}
			}

			public string DiagnoseCode
			{
				get
				{
					return _diagnose.StringFieldValue(DiagnoseFieldOrdinals.DiagnoseCode, _recordNumber);
				}
			}

			public string DiagnoseDescription
			{
				get
				{
					return _diagnose.StringFieldValue(DiagnoseFieldOrdinals.DiagnoseDescription, _recordNumber);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Diagnose()
		{
			AddDatabaseDefinitions(CarsonStaticSettings.DatabasePath);
		}

		public Diagnose(string databasePath)
		{
			AddDatabaseDefinitions(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "DIAGNOSE";
			this.DatabasePath = databasePath;
			this.TableId = TableInstance.Diagnose;

			this.AddFieldDefinition(DiagnoseFields.Id, DiagnoseFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(DiagnoseFields.DiagnoseRecd, DiagnoseFieldOrdinals.DiagnoseRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(DiagnoseFields.DiagnoseCode, DiagnoseFieldOrdinals.DiagnoseCode, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(DiagnoseFields.DiagnoseDescription, DiagnoseFieldOrdinals.DiagnoseDescription, AVImarkDataType.AVImarkDynamicString);
		}

		public List<DiagnoseData> DiagnoseList()
		{
			List<DiagnoseData> diagnoseList = new List<DiagnoseData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				DiagnoseData diagnoseData = new DiagnoseData(this, x);
				diagnoseList.Add(diagnoseData);
			}

			return diagnoseList;
		}
	}
}
