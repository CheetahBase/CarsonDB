using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Visit : CarsonBackend
	{
		private enum VisitFieldOrdinals
		{
			Id = -1,
			VisitRecd = 1,
			VisitInTod = 2,
			VisitOutTod = 4,
			VisitDoctor = 6,
			VisitInDate = 10,
			VisitOutDate = 12,
			VisitAnimal = 14
		}

		public static class VisitFields
		{
			public const string Id = "ID";
			public const string VisitRecd = "VISIT_RECD";
			public const string VisitInTod = "VISIT_IN_TOD";
			public const string VisitOutTod = "VISIT_OUT_TOD";
			public const string VisitDoctor = "VISIT_DOCTOR";
			public const string VisitInDate = "VISIT_IN_DATE";
			public const string VisitOutDate = "VISIT_OUT_DATE";
			public const string VisitAnimal = "VISIT_ANIMAL";
		}

		public class VisitData
		{
			private int _recordNumber;
			private Visit _visit;

			public VisitData(Visit visit, int recordNumber)
			{
				_visit = visit;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int visitId = 0;

					try
					{
						visitId = _visit.IntegerFieldValue(VisitFieldOrdinals.Id, _recordNumber);
						return visitId;
					}
					catch (RecordLockedException)
					{
						visitId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return visitId;
					}
				}
			}

			public string VisitRecd
			{
				get
				{
					return _visit.CharacterFieldValue(VisitFieldOrdinals.VisitRecd, _recordNumber);
				}
			}

			public string VisitInTod
			{
				get
				{
					return _visit.TimeFieldValue(VisitFieldOrdinals.VisitInTod, _recordNumber);
				}
			}

			public string VisitOutTod
			{
				get
				{
					return _visit.TimeFieldValue(VisitFieldOrdinals.VisitOutTod, _recordNumber);
				}
			}

			public string VisitDoctor
			{
				get
				{
					return _visit.StringFieldValue(VisitFieldOrdinals.VisitDoctor, _recordNumber);
				}
			}

			public DateTime VisitInDate
			{
				get
				{
					return _visit.DateFieldValue(VisitFieldOrdinals.VisitInDate, _recordNumber);
				}
			}

			public DateTime VisitOutDate
			{
				get
				{
					return _visit.DateFieldValue(VisitFieldOrdinals.VisitOutDate, _recordNumber);
				}
			}

			public int VisitAnimal
			{
				get
				{
					return _visit.IntegerFieldValue(VisitFieldOrdinals.VisitAnimal, _recordNumber);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Visit()
		{
			AddDatabaseDefinitions(CarsonStaticSettings.DatabasePath);
		}

		public Visit(string databasePath)
		{
			AddDatabaseDefinitions(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "VISIT";
			this.DatabasePath = databasePath;
			this.TableId = TableInstance.MiscDirect;
			this.ModernRecordLength = 256;
			this.ClassicRecordLength = 100;

			this.AddFieldDefinition(VisitFields.Id, VisitFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(VisitFields.VisitRecd, VisitFieldOrdinals.VisitRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(VisitFields.VisitInTod, VisitFieldOrdinals.VisitInTod, AVImarkDataType.AVImarkTime);
			this.AddFieldDefinition(VisitFields.VisitOutTod, VisitFieldOrdinals.VisitOutTod, AVImarkDataType.AVImarkTime);
			this.AddFieldDefinition(VisitFields.VisitDoctor, VisitFieldOrdinals.VisitDoctor, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(VisitFields.VisitInDate, VisitFieldOrdinals.VisitInDate, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(VisitFields.VisitOutDate, VisitFieldOrdinals.VisitOutDate, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(VisitFields.VisitAnimal, VisitFieldOrdinals.VisitAnimal, AVImarkDataType.AVImarkDoubleWord);
		}

		public List<VisitData> VisitList()
		{
			List<VisitData> visitList = new List<VisitData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				VisitData visitData = new VisitData(this, x);
				visitList.Add(visitData);
			}

			return visitList;
		}
	}
}
