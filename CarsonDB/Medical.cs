using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Medical : CarsonBackend
	{
		public enum MedicalFieldOrdinals
		{
			Id,
			MedicalRecd,
			MedicalWeight,
			MedicalTemp,
			MedicalResp,
			MedicalPulse,
			MedicalCrt,
			MedicalOther,
			MedicalComplaint,
			MedicalExamNotes,
			MedicalClientInstructions,
			MedicalService,
			MedicalAssessNotes,
			MedicalPlanNotes
		}

		public static class MedicalFields
		{
			public static readonly string Id = "ID";
			public static readonly string MedicalRecd = "MEDICAL_RECD";
			public static readonly string MedicalWeight = "MEDICAL_WEIGHT";
			public static readonly string MedicalTemp = "MEDICAL_TEMP";
			public static readonly string MedicalResp = "MEDICAL_RESP";
			public static readonly string MedicalPulse = "MEDICAL_PULSE";
			public static readonly string MedicalCrt = "MEDICAL_CRT";
			public static readonly string MedicalOther = "MEDICAL_OTHER";
			public static readonly string MedicalComplaint = "MEDICAL_COMPLAINT";
			public static readonly string MedicalExamNotes = "MEDICAL_EXAMNOTES";
			public static readonly string MedicalClientInstructions = "MEDICAL_CLIENTINSTRUCTIONS";
			public static readonly string MedicalService = "MEDICAL_SERVICE";
			public static readonly string MedicalAssessNotes = "MEDICAL_ASSESSNOTES";
			public static readonly string MedicalPlanNotes = "MEDICAL_PLANNOTES";
		}

		public class MedicalData : ICrc
		{
			private int _recordNumber;
			private Medical _medical;

			public MedicalData(Medical medical, int recordNumber)
			{
				_medical = medical;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					return _medical.IntegerFieldValue(MedicalFieldOrdinals.Id, _recordNumber);
				}
			}

			public string MedicalRecd
			{
				get
				{
					return _medical.CharacterFieldValue(MedicalFieldOrdinals.MedicalRecd, _recordNumber);
				}
			}

			public decimal MedicalWeight
			{
				get
				{
					return _medical.FloatFieldValue(MedicalFieldOrdinals.MedicalWeight, _recordNumber); // * 100;
				}
			}

			public decimal MedicalTemp
			{
				get
				{
					return _medical.FloatFieldValue(MedicalFieldOrdinals.MedicalTemp, _recordNumber); // * 100;
				}
			}

			public decimal MedicalResp
			{
				get
				{
					return _medical.FloatFieldValue(MedicalFieldOrdinals.MedicalResp, _recordNumber); // * 100;
				}
			}

			public decimal MedicalPulse
			{
				get
				{
					return _medical.FloatFieldValue(MedicalFieldOrdinals.MedicalPulse, _recordNumber); // * 100;
				}
			}

			public string MedicalCrt
			{
				get
				{
					return _medical.StringFieldValue(MedicalFieldOrdinals.MedicalCrt, _recordNumber);
				}
			}

			public string MedicalOther
			{
				get
				{
					return _medical.StringFieldValue(MedicalFieldOrdinals.MedicalOther, _recordNumber);
				}
			}

			public string MedicalComplaint
			{
				get
				{
					return _medical.StringFieldValue(MedicalFieldOrdinals.MedicalComplaint, _recordNumber);
				}
			}

			public string MedicalExamNotes
			{
				get
				{
					return _medical.StringFieldValue(MedicalFieldOrdinals.MedicalExamNotes, _recordNumber);
				}
			}

			public string MedicalClientInstructions
			{
				get
				{
					return _medical.StringFieldValue(MedicalFieldOrdinals.MedicalClientInstructions, _recordNumber);
				}
			}

			public int MedicalService
			{
				get
				{
					return _medical.IntegerFieldValue(MedicalFieldOrdinals.MedicalService, _recordNumber);
				}
			}

			public string MedicalAssessNotes
			{
				get
				{
					return _medical.StringFieldValue(MedicalFieldOrdinals.MedicalAssessNotes, _recordNumber);
				}
			}

			public string MedicalPlanNotes
			{
				get
				{
					return _medical.StringFieldValue(MedicalFieldOrdinals.MedicalPlanNotes, _recordNumber);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Medical(string databasePath)
		{
			AddDatabaseDefinitions(databasePath);
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "MEDICAL";
			this.DatabasePath = databasePath;
			this.TableId = TableInstance.Medical;

			this.AddFieldDefinition(MedicalFields.Id, MedicalFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(MedicalFields.MedicalRecd, MedicalFieldOrdinals.MedicalRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(MedicalFields.MedicalWeight, MedicalFieldOrdinals.MedicalWeight, AVImarkDataType.AVImarkSingle);
			this.AddFieldDefinition(MedicalFields.MedicalTemp, MedicalFieldOrdinals.MedicalTemp, AVImarkDataType.AVImarkSingle);
			this.AddFieldDefinition(MedicalFields.MedicalResp, MedicalFieldOrdinals.MedicalResp, AVImarkDataType.AVImarkSingle);
			this.AddFieldDefinition(MedicalFields.MedicalPulse, MedicalFieldOrdinals.MedicalPulse, AVImarkDataType.AVImarkSingle);
			this.AddFieldDefinition(MedicalFields.MedicalCrt, MedicalFieldOrdinals.MedicalCrt, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(MedicalFields.MedicalOther, MedicalFieldOrdinals.MedicalOther, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(MedicalFields.MedicalComplaint, MedicalFieldOrdinals.MedicalComplaint, AVImarkDataType.AVImarkLinkToWp);
			this.AddFieldDefinition(MedicalFields.MedicalExamNotes, MedicalFieldOrdinals.MedicalExamNotes, AVImarkDataType.AVImarkLinkToWp);
			this.AddFieldDefinition(MedicalFields.MedicalClientInstructions, MedicalFieldOrdinals.MedicalClientInstructions, AVImarkDataType.AVImarkLinkToWp);
			this.AddFieldDefinition(MedicalFields.MedicalService, MedicalFieldOrdinals.MedicalService, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(MedicalFields.MedicalAssessNotes, MedicalFieldOrdinals.MedicalAssessNotes, AVImarkDataType.AVImarkLinkToWp);
			this.AddFieldDefinition(MedicalFields.MedicalPlanNotes, MedicalFieldOrdinals.MedicalPlanNotes, AVImarkDataType.AVImarkLinkToWp);
		}

		public List<MedicalData> MedicalList()
		{
			List<MedicalData> medicalList = new List<MedicalData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				MedicalData medicalData = new MedicalData(this, x);
				medicalList.Add(medicalData);
			}

			return medicalList;
		}
	}
}
