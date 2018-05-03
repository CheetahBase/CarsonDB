using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Reminder : CarsonBackend
	{
		private enum ReminderFieldOrdinals
		{
			Id,
			ReminderCode,
			ReminderRecd,
			ReminderDescription,
			ReminderReminded,
			ReminderSuspend,
			ReminderDue,
			ReminderParent,
			ReminderParentis,
			ReminderAppropriate,
			ReminderAdded
		}

		public static class ReminderFields
		{
			public const string Id = "ID";
			public const string ReminderCode = "REMINDER_CODE";
			public const string ReminderRecd = "REMINDER_RECD";
			public const string ReminderDescription = "REMINDER_DESCRIPTION";
			public const string ReminderReminded = "REMINDER_REMINDED";
			public const string ReminderSuspend = "REMINDER_SUSPEND";
			public const string ReminderDue = "REMINDER_DUE";
			public const string ReminderParent = "REMINDER_PARENT";
			public const string ReminderParentis = "REMINDER_PARENTIS";
			public const string ReminderAppropriate = "REMINDER_APPROPRIATE";
			public const string ReminderAdded = "REMINDER_ADDED";
		}

		public class ReminderData : ICrc
		{
			private int _recordNumber;
			private Reminder _reminder;

			public ReminderData(Reminder reminder, int recordNumber)
			{
				_reminder = reminder;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int reminderId = 0;

					try
					{
						reminderId = _reminder.IntegerFieldValue(ReminderFieldOrdinals.Id, _recordNumber);
						return reminderId;
					}
					catch (RecordLockedException)
					{
						reminderId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return reminderId;
					}
				}
			}

			public string ReminderCode
			{
				get
				{
					return _reminder.StringFieldValue(ReminderFieldOrdinals.ReminderCode, _recordNumber);
				}
			}

			public string ReminderRecd
			{
				get
				{
					return _reminder.CharacterFieldValue(ReminderFieldOrdinals.ReminderRecd, _recordNumber);
				}
			}

			public string ReminderDescription
			{
				get
				{
					return _reminder.StringFieldValue(ReminderFieldOrdinals.ReminderDescription, _recordNumber);
				}
			}

			public DateTime ReminderReminded
			{
				get
				{
					return _reminder.DateFieldValue(ReminderFieldOrdinals.ReminderReminded, _recordNumber);
				}
			}

			public bool ReminderSuspend
			{
				get
				{
					return _reminder.BooleanFieldValue(ReminderFieldOrdinals.ReminderSuspend, _recordNumber);
				}
			}

			public DateTime ReminderDue
			{
				get
				{
					return _reminder.DateFieldValue(ReminderFieldOrdinals.ReminderDue, _recordNumber);
				}
			}

			public int ReminderParent
			{
				get
				{
					return _reminder.IntegerFieldValue(ReminderFieldOrdinals.ReminderParent, _recordNumber);
				}
			}

			public int ReminderParentis
			{
				get
				{
					return _reminder.IntegerFieldValue(ReminderFieldOrdinals.ReminderParentis, _recordNumber);
				}
			}

			public int ReminderAppropriate
			{
				get
				{
					return _reminder.IntegerFieldValue(ReminderFieldOrdinals.ReminderAppropriate, _recordNumber);
				}
			}

			public DateTime ReminderAdded
			{
				get
				{
					return _reminder.DateFieldValue(ReminderFieldOrdinals.ReminderAdded, _recordNumber);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Reminder()
		{
			AddDatabaseDefinitions(CarsonStaticSettings.DatabasePath);
		}

		public Reminder(string databasePath)
		{
			AddDatabaseDefinitions(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "PROC";
			this.DatabasePath = databasePath;
			this.TableId = this.TableId = TableInstance.Reminder;

			this.AddFieldDefinition(ReminderFields.Id, ReminderFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(ReminderFields.ReminderCode, ReminderFieldOrdinals.ReminderCode, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(ReminderFields.ReminderRecd, ReminderFieldOrdinals.ReminderRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(ReminderFields.ReminderDescription, ReminderFieldOrdinals.ReminderDescription, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(ReminderFields.ReminderReminded, ReminderFieldOrdinals.ReminderReminded, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(ReminderFields.ReminderSuspend, ReminderFieldOrdinals.ReminderSuspend, AVImarkDataType.AVImarkBool);
			this.AddFieldDefinition(ReminderFields.ReminderDue, ReminderFieldOrdinals.ReminderDue, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(ReminderFields.ReminderParent, ReminderFieldOrdinals.ReminderParent, AVImarkDataType.AVImarkWord);
			this.AddFieldDefinition(ReminderFields.ReminderParentis, ReminderFieldOrdinals.ReminderParentis, AVImarkDataType.AVImarkByte);
			this.AddFieldDefinition(ReminderFields.ReminderAppropriate, ReminderFieldOrdinals.ReminderAppropriate, AVImarkDataType.AVImarkByte);
			this.AddFieldDefinition(ReminderFields.ReminderAdded, ReminderFieldOrdinals.ReminderAdded, AVImarkDataType.AVImarkDate);
		}

		public List<ReminderData> ReminderList()
		{
			List<ReminderData> reminderList = new List<ReminderData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				ReminderData reminderData = new ReminderData(this, x);
				reminderList.Add(reminderData);
			}

			return reminderList;
		}
	}
}
