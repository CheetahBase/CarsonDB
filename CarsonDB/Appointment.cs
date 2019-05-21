using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Appointment : CarsonBackend
	{
		private enum AppointmentFieldOrdinals
		{
			Id,
			AppointmentRecd,
			AppointmentDate,
			AppointmentTime,
			AppointmentDuration,
			AppointmentDateMade,
			AppointmentDoctor,
			AppointmentType,
			AppointmentClient,
			AppointmentNote, 
			AppointmentParent,
			AppointmentParentis,
			AppointmentTypeCode, 
			AppointmentNewClient,
			AppointmentNewPatient,
			AppointmentNewSpecies,
			AppointmentNewPhone,
			AppointmentNewArea
		}

		public static class AppointmentFields
		{
			public const string Id = "ID";
			public const string AppointmentRecd = "APPOINT_RECD";
			public const string AppointmentDate = "APPOINT_DATE";
			public const string AppointmentTime = "APPOINT_TIME";
			public const string AppointmentDuration = "APPOINT_DURATION";
			public const string AppointmentDateMade = "APPOINT_DATE_MADE";
			public const string AppointmentDoctor = "APPOINT_DOCTOR";
			public const string AppointmentType = "APPOINT_TYPE";
			public const string AppointmentClient = "APPOINT_CLIENT";
			public const string AppointmentNewClient = "APPOINT_NEWCLIENT";
			public const string AppointmentNewPatient = "APPOINT_NEWPATIENT";
			public const string AppointmentNewSpecies = "APPOINT_NEWSPECIES";
			public const string AppointmentNewPhone = "APPOINT_NEWPHONE";
			public const string AppointmentNewArea = "APPOINT_NEWAREA";
			public const string AppointmentNote = "APPOINT_NOTE";
			public const string AppointmentParent = "APPOINT_PARENT";
			public const string AppointmentParentis = "APPOINT_PARENTIS";
			public const string AppointmentTypeCode = "APPOINT_TYPE_CODE";
		}

		public class AppointmentData : ICrc
		{
			private int _recordNumber;
			private Appointment _appointment;

			public AppointmentData(Appointment appointment, int recordNumber)
			{
				_appointment = appointment;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int appointmentId = 0;

					try
					{
						appointmentId = _appointment.IntegerFieldValue(AppointmentFieldOrdinals.Id, _recordNumber);
						return appointmentId;
					}
					catch (RecordLockedException)
					{
						appointmentId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return appointmentId;
					}
				}
			}

			public string AppointmentRecd
			{
				get
				{
					return _appointment.CharacterFieldValue(AppointmentFieldOrdinals.AppointmentRecd, _recordNumber);
				}
			}

			public DateTime AppointmentDate
			{
				get
				{
					return _appointment.DateFieldValue(AppointmentFieldOrdinals.AppointmentDate, _recordNumber);
				}
			}

			public string AppointmentTime
			{
				get
				{
					return _appointment.TimeFieldValue(AppointmentFieldOrdinals.AppointmentTime, _recordNumber);
				}
			}

			public int AppointmentDuration
			{
				get
				{
					return _appointment.IntegerFieldValue(AppointmentFieldOrdinals.AppointmentDuration, _recordNumber);
				}
			}

			public DateTime AppointmentDateMade
			{
				get
				{
					return _appointment.DateFieldValue(AppointmentFieldOrdinals.AppointmentDateMade, _recordNumber);
				}
			}

			public string AppointmentDoctor
			{
				get
				{
					return _appointment.StringFieldValue(AppointmentFieldOrdinals.AppointmentDoctor, _recordNumber);
				}
			}

			public int AppointmentType
			{
				get
				{
					return _appointment.IntegerFieldValue(AppointmentFieldOrdinals.AppointmentType, _recordNumber);
				}
			}

			public int AppointmentClient
			{
				get
				{
					return _appointment.IntegerFieldValue(AppointmentFieldOrdinals.AppointmentClient, _recordNumber);
				}
			}

			public string AppointmentNote
			{
				get
				{
					return _appointment.StringFieldValue(AppointmentFieldOrdinals.AppointmentNote, _recordNumber);
				}
			}

			public int AppointmentParent
			{
				get
				{
					return _appointment.IntegerFieldValue(AppointmentFieldOrdinals.AppointmentParent, _recordNumber);
				}
			}

			public int AppointmentParentis
			{
				get
				{
					return _appointment.IntegerFieldValue(AppointmentFieldOrdinals.AppointmentParentis, _recordNumber);
				}
			}

			public string AppointmentTypeCode
			{
				get
				{
					return _appointment.StringFieldValue(AppointmentFieldOrdinals.AppointmentTypeCode, _recordNumber);
				}
			}

			public string AppointmentNewClient
			{
				get
				{
					return _appointment.StringFieldValue(AppointmentFieldOrdinals.AppointmentNewClient, _recordNumber);
				}
			}

			public string AppointmentNewPatient
			{
				get
				{
					return _appointment.StringFieldValue(AppointmentFieldOrdinals.AppointmentNewPatient, _recordNumber);
				}
			}

			public string AppointmentNewSpecies
			{
				get
				{
					return _appointment.StringFieldValue(AppointmentFieldOrdinals.AppointmentNewSpecies, _recordNumber);
				}
			}

			public string AppointmentNewPhone
			{
				get
				{
					return _appointment.StringFieldValue(AppointmentFieldOrdinals.AppointmentNewPhone, _recordNumber);
				}
			}

			public string AppointmentNewArea
			{
				get
				{
					return _appointment.StringFieldValue(AppointmentFieldOrdinals.AppointmentNewArea, _recordNumber);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Appointment()
		{
			AddDatabaseDefinition(CarsonStaticSettings.DatabasePath);
		}

		public Appointment(string databasePath)
		{
			AddDatabaseDefinition(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinition(string databasePath)
		{
			this.DatabaseName = "APPOINT";
			this.DatabasePath = databasePath;
			this.TableId = TableInstance.Appointment;

			this.AddFieldDefinition(AppointmentFields.Id, AppointmentFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(AppointmentFields.AppointmentRecd, AppointmentFieldOrdinals.AppointmentRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(AppointmentFields.AppointmentDate, AppointmentFieldOrdinals.AppointmentDate, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(AppointmentFields.AppointmentTime, AppointmentFieldOrdinals.AppointmentTime, AVImarkDataType.AVImarkTime);
			this.AddFieldDefinition(AppointmentFields.AppointmentDuration, AppointmentFieldOrdinals.AppointmentDuration, AVImarkDataType.AVImarkWord);
			this.AddFieldDefinition(AppointmentFields.AppointmentDateMade, AppointmentFieldOrdinals.AppointmentDateMade, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(AppointmentFields.AppointmentDoctor, AppointmentFieldOrdinals.AppointmentDoctor, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(AppointmentFields.AppointmentType, AppointmentFieldOrdinals.AppointmentType, AVImarkDataType.AVImarkByte);
			this.AddFieldDefinition(AppointmentFields.AppointmentClient, AppointmentFieldOrdinals.AppointmentClient, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(AppointmentFields.AppointmentNote, AppointmentFieldOrdinals.AppointmentNote, AVImarkDataType.AVImarkLinkToWp);
			this.AddFieldDefinition(AppointmentFields.AppointmentParent, AppointmentFieldOrdinals.AppointmentParent, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(AppointmentFields.AppointmentParentis, AppointmentFieldOrdinals.AppointmentParentis, AVImarkDataType.AVImarkByte);
			this.AddFieldDefinition(AppointmentFields.AppointmentTypeCode, AppointmentFieldOrdinals.AppointmentTypeCode, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(AppointmentFields.AppointmentNewClient, AppointmentFieldOrdinals.AppointmentNewClient, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(AppointmentFields.AppointmentNewPatient, AppointmentFieldOrdinals.AppointmentNewPatient, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(AppointmentFields.AppointmentNewSpecies, AppointmentFieldOrdinals.AppointmentNewSpecies, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(AppointmentFields.AppointmentNewPhone, AppointmentFieldOrdinals.AppointmentNewPhone, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(AppointmentFields.AppointmentNewArea, AppointmentFieldOrdinals.AppointmentNewArea, AVImarkDataType.AVImarkDynamicString);
		}

		public List<AppointmentData> AppointmentList()
		{
			List<AppointmentData> appointmentList = new List<AppointmentData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				AppointmentData appointmentData = new AppointmentData(this, x);
				appointmentList.Add(appointmentData);
			}

			return appointmentList;
		}
	}
}
