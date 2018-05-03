using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Treatment : CarsonBackend
	{
		private enum TreatmentFieldOrdinals
		{
			Id,
			TreatmentCode,
			TreatmentRecd,
			TreatmentClass,
			TreatmentDescription,
			TreatmentPrice,
			TreatmentCodes,
			TreatmentChanged
		}

		public static class TreatmentFields
		{
			public const string Id = "ID";
			public const string TreatmentCode = "TREATMENT_CODE";
			public const string TreatmentRecd = "TREATMENT_RECD";
			public const string TreatmentClass = "TREATMENT_CLASS";
			public const string TreatmentDescription = "TREATMENT_DESCRIPTION";
			public const string TreatmentPrice = "TREATMENT_PRICE";
			public const string TreatmentCodes = "TREATMENT_CODES";
			public const string TreatmentChanged = "TREATMENT_CHANGED";
		}

		public class TreatmentData : ICrc
		{
			private int _recordNumber;
			private Treatment _treatment;

			public TreatmentData(Treatment treatment, int recordNumber)
			{
				_treatment = treatment;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int treatmentId = 0;

					try
					{
						treatmentId = _treatment.IntegerFieldValue(TreatmentFieldOrdinals.Id, _recordNumber);
						return treatmentId;
					}
					catch (RecordLockedException)
					{
						treatmentId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return treatmentId;
					}
				}
			}

			public string TreatmentCode
			{
				get
				{
					return _treatment.StringFieldValue(TreatmentFieldOrdinals.TreatmentCode, _recordNumber);
				}
			}

			public string TreatmentRecd
			{
				get
				{
					return _treatment.CharacterFieldValue(TreatmentFieldOrdinals.TreatmentRecd, _recordNumber);
				}
			}

			public int TreatmentClass
			{
				get
				{
					return _treatment.IntegerFieldValue(TreatmentFieldOrdinals.TreatmentClass, _recordNumber);
				}
			}

			public string TreatmentDescription
			{
				get
				{
					return _treatment.StringFieldValue(TreatmentFieldOrdinals.TreatmentDescription, _recordNumber);
				}
			}

			public decimal TreatmentPrice
			{
				get
				{
					return _treatment.ImpliedDecimalFieldValue(TreatmentFieldOrdinals.TreatmentPrice, _recordNumber);
				}
			}

			public string TreatmentCodes
			{
				get
				{
					return _treatment.StringFieldValue(TreatmentFieldOrdinals.TreatmentCodes, _recordNumber);
				}
			}

			public DateTime TreatmentChanged
			{
				get
				{
					return _treatment.DateFieldValue(TreatmentFieldOrdinals.TreatmentChanged, _recordNumber);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Treatment()
		{
			AddDatabaseDefinitions(CarsonStaticSettings.DatabasePath);
		}

		public Treatment(string databasePath)
		{
			AddDatabaseDefinitions(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "TREAT";
			this.DatabasePath = databasePath;
			this.TableId = TableInstance.Treatment;

			this.AddFieldDefinition(TreatmentFields.Id, TreatmentFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(TreatmentFields.TreatmentCode, TreatmentFieldOrdinals.TreatmentCode, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(TreatmentFields.TreatmentRecd, TreatmentFieldOrdinals.TreatmentRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(TreatmentFields.TreatmentClass, TreatmentFieldOrdinals.TreatmentClass, AVImarkDataType.AVImarkByte);
			this.AddFieldDefinition(TreatmentFields.TreatmentDescription, TreatmentFieldOrdinals.TreatmentDescription, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(TreatmentFields.TreatmentPrice, TreatmentFieldOrdinals.TreatmentPrice, AVImarkDataType.AVImarkImpliedDecimal);
			this.AddFieldDefinition(TreatmentFields.TreatmentCodes, TreatmentFieldOrdinals.TreatmentCodes, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(TreatmentFields.TreatmentChanged, TreatmentFieldOrdinals.TreatmentChanged, AVImarkDataType.AVImarkDate);
		}

		public List<TreatmentData> TreatmentList()
		{
			List<TreatmentData> treatmentList = new List<TreatmentData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				TreatmentData treatmentData = new TreatmentData(this, x);
				treatmentList.Add(treatmentData);
			}

			return treatmentList;
		}
	}
}
