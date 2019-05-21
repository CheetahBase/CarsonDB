using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Lab : CarsonBackend
	{
		private enum LabFieldOrdinals
		{
			Id,
			LabRecd,
			LabTreatment,
			LabCompany,
			LabId,
			LabDescription
		}

		public static class LabFields
		{
			public const string Id = "ID";
			public const string LabRecd = "LAB_RECD";
			public const string LabTreatment = "LAB_TREATMENT";
			public const string LabCompany = "LAB_COMPANY";
			public const string LabId = "LAB_ID";
			public const string LabDescription = "LAB_DESCRIPTION";
		}

		public class LabData : ICrc
		{
			private int _recordNumber;
			private Lab _lab;

			public LabData(Lab lab, int recordNumber)
			{
				_lab = lab;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int labId = 0;

					try
					{
						labId = _lab.IntegerFieldValue(LabFieldOrdinals.Id, _recordNumber);
						return labId;
					}
					catch (RecordLockedException)
					{
						labId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return labId;
					}
				}
			}

			public string LabRecd
			{
				get
				{
					return _lab.CharacterFieldValue(LabFieldOrdinals.LabRecd, _recordNumber);
				}
			}

			public int LabTreatment
			{
				get
				{
					return _lab.IntegerFieldValue(LabFieldOrdinals.LabTreatment, _recordNumber);
				}
			}

			public int LabCompany
			{
				get
				{
					return _lab.IntegerFieldValue(LabFieldOrdinals.LabCompany, _recordNumber);
				}
			}

			public string LabId
			{
				get
				{
					return _lab.StringFieldValue(LabFieldOrdinals.LabId, _recordNumber);
				}
			}

			public string LabDescription
			{
				get
				{
					return _lab.StringFieldValue(LabFieldOrdinals.LabDescription, _recordNumber);
				}
			}


			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Lab()
		{
			AddDatabaseDefinitions(CarsonStaticSettings.DatabasePath);
		}

		public Lab(string databasePath)
		{
			AddDatabaseDefinitions(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "LAB";
			this.DatabasePath = databasePath;
			this.TableId = TableInstance.Lab;

			this.AddFieldDefinition(LabFields.Id, LabFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(LabFields.LabRecd, LabFieldOrdinals.LabRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(LabFields.LabTreatment, LabFieldOrdinals.LabTreatment, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(LabFields.LabCompany, LabFieldOrdinals.LabCompany, AVImarkDataType.AVImarkByte);
			this.AddFieldDefinition(LabFields.LabId, LabFieldOrdinals.LabId, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(LabFields.LabDescription, LabFieldOrdinals.LabDescription, AVImarkDataType.AVImarkDynamicString);
		}

		public List<LabData> LabList()
		{
			List<LabData> labList = new List<LabData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				LabData labData = new LabData(this, x);
				labList.Add(labData);
			}

			return labList;
		}
	}
}
