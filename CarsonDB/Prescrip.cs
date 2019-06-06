using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Prescrip : CarsonBackend
	{
		public enum PrescripFieldOrdinals
		{
			Id,
			PrescripRecd,
			PrescripPatient,
			PrescripItem,
			PrescripDateIssued,
			PrescripDateExpires,
			PrescripStatus,
			PrescripLabelPrintDate,
			PrescripClosedDate,
			PrescripLastRefillDate,
			PrescripFillLocation
		}

		public static class PrescripFields
		{
			public static readonly string Id = "ID";
			public static readonly string PrescripRecd = "PRESCRIPTION_RECD";
			public static readonly string PrescripPatient = "PRESCRIPTION_PATIENT";
			public static readonly string PrescripItem = "PRESCRIPTION_ITEM";
			public static readonly string PrescripDateIssued = "PRESCRIPTION_DATE_ISSUED";
			public static readonly string PrescripDateExpires = "PRESCRIPTION_DATE_EXPIRES";
			public static readonly string PrescripStatus = "PRESCRIPTION_STATUS";
			public static readonly string PrescripLabelPrintDate = "PRESCRIPTION_LABEL_PRINT_DATE";
			public static readonly string PrescripClosedDate = "PRESCRIPTION_CLOSED_DATE";
			public static readonly string PrescripLastRefillDate = "PRESCRIPTION_LAST_REFILL_DATE";
			public static readonly string PrescripFillLocation = "PRESCRIPTION_FILL_LOCATION";
		}

		public class PrescripData : ICrc
		{
			private int _recordNumber;
			private Prescrip _prescrip;

			public PrescripData(Prescrip prescrip, int recordNumber)
			{
				_prescrip = prescrip;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					return _prescrip.IntegerFieldValue(PrescripFieldOrdinals.Id, _recordNumber);
				}
			}

			public string PrescripRecd
			{
				get
				{
					return _prescrip.CharacterFieldValue(PrescripFieldOrdinals.PrescripRecd, _recordNumber);
				}
			}

			public int PrescripPatient
			{
				get
				{
					return _prescrip.IntegerFieldValue(PrescripFieldOrdinals.PrescripPatient, _recordNumber);
				}
			}

			public int PrescripItem
			{
				get
				{
					return _prescrip.IntegerFieldValue(PrescripFieldOrdinals.PrescripItem, _recordNumber);
				}
			}

			public DateTime PrescripDateIssued
			{
				get
				{
					return _prescrip.DateFieldValue(PrescripFieldOrdinals.PrescripDateIssued, _recordNumber);
				}
			}

			public DateTime PrescripDateExpires
			{
				get
				{
					return _prescrip.DateFieldValue(PrescripFieldOrdinals.PrescripDateExpires, _recordNumber);
				}
			}

			public int PrescripStatus
			{
				get
				{
					return _prescrip.IntegerFieldValue(PrescripFieldOrdinals.PrescripStatus, _recordNumber);
				}
			}

			public DateTime PrescripLabelPrintDate
			{
				get
				{
					return _prescrip.DateFieldValue(PrescripFieldOrdinals.PrescripLabelPrintDate, _recordNumber);
				}
			}

			public DateTime PrescripClosedDate
			{
				get
				{
					return _prescrip.DateFieldValue(PrescripFieldOrdinals.PrescripClosedDate, _recordNumber);
				}
			}

			public DateTime PrescripLastRefillDate
			{
				get
				{
					return _prescrip.DateFieldValue(PrescripFieldOrdinals.PrescripLastRefillDate, _recordNumber);
				}
			}

			public int PrescripFillLocation
			{
				get
				{
					return _prescrip.IntegerFieldValue(PrescripFieldOrdinals.PrescripFillLocation, _recordNumber);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Prescrip(string databasePath)
		{
			AddDatabaseDefinitions(databasePath);
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "PRESCRIP";
			this.DatabasePath = databasePath;
			this.TableId = TableInstance.Prescrip;

			this.AddFieldDefinition(PrescripFields.Id, PrescripFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(PrescripFields.PrescripRecd, PrescripFieldOrdinals.PrescripRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(PrescripFields.PrescripPatient, PrescripFieldOrdinals.PrescripPatient, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(PrescripFields.PrescripItem, PrescripFieldOrdinals.PrescripItem, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(PrescripFields.PrescripDateIssued, PrescripFieldOrdinals.PrescripDateIssued, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(PrescripFields.PrescripDateExpires, PrescripFieldOrdinals.PrescripDateExpires, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(PrescripFields.PrescripStatus, PrescripFieldOrdinals.PrescripStatus, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(PrescripFields.PrescripLabelPrintDate, PrescripFieldOrdinals.PrescripLabelPrintDate, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(PrescripFields.PrescripClosedDate, PrescripFieldOrdinals.PrescripClosedDate, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(PrescripFields.PrescripLastRefillDate, PrescripFieldOrdinals.PrescripLastRefillDate, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(PrescripFields.PrescripFillLocation, PrescripFieldOrdinals.PrescripFillLocation, AVImarkDataType.AVImarkDoubleWord);
		}

		public List<PrescripData> PrescripList()
		{
			List<PrescripData> prescripList = new List<PrescripData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				PrescripData prescripData = new PrescripData(this, x);
				prescripList.Add(prescripData);
			}

			return prescripList;
		}
	}
}
