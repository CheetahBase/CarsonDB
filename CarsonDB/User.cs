using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class User : CarsonBackend
	{
		private enum UserFieldOrdinals
		{
			Id,
			UserRecd,
			UserId,
			UserFirst,
			UserStaff,
			UserInactive,
			UserDoctor,
			UserLast,
			UserAdded
		}

		public static class UserFields
		{
			public const string Id = "ID";
			public const string UserRecd = "USER_RECD";
			public const string UserId = "USER_ID";
			public const string UserFirst = "USER_FIRST";
			public const string UserStaff = "USER_STAFF";
			public const string UserInactive = "USER_INACTIVE";
			public const string UserDoctor = "USER_DOCTOR";
			public const string UserLast = "USER_LAST";
			public const string UserAdded = "USER_ADDED";
		}

		public class UserData : ICrc
		{
			private int _recordNumber;
			private User _user;

			public UserData(User user, int recordNumber)
			{
				_user = user;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int userId = 0;

					try
					{
						userId = _user.IntegerFieldValue(UserFieldOrdinals.Id, _recordNumber);
						return userId;
					}
					catch (RecordLockedException)
					{
						userId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return userId;
					}
				}
			}

			public string UserRecd
			{
				get
				{
					return _user.CharacterFieldValue(UserFieldOrdinals.UserRecd, _recordNumber);
				}
			}

			public string UserId
			{
				get
				{
					return _user.StringFieldValue(UserFieldOrdinals.UserId, _recordNumber);
				}
			}

			public string UserFirst
			{
				get
				{
					return _user.StringFieldValue(UserFieldOrdinals.UserFirst, _recordNumber);
				}
			}

			public int UserStaff
			{
				get
				{
					return _user.IntegerFieldValue(UserFieldOrdinals.UserStaff, _recordNumber);
				}
			}

			public bool UserInactive
			{
				get
				{
					return _user.BooleanFieldValue(UserFieldOrdinals.UserInactive, _recordNumber);
				}
			}

			public bool UserDoctor
			{
				get
				{
					return _user.BooleanFieldValue(UserFieldOrdinals.UserDoctor, _recordNumber);
				}
			}

			public string UserLast
			{
				get
				{
					return _user.StringFieldValue(UserFieldOrdinals.UserLast, _recordNumber);
				}
			}

			public DateTime UserAdded
			{
				get
				{
					return _user.DateFieldValue(UserFieldOrdinals.UserAdded, _recordNumber);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public User()
		{
			AddDatabaseDefinitions(CarsonStaticSettings.DatabasePath);
		}

		public User(string databasePath)
		{
			AddDatabaseDefinitions(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "USER";
			this.DatabasePath = databasePath;
			this.TableId = TableInstance.User;

			this.AddFieldDefinition(UserFields.Id, UserFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(UserFields.UserRecd, UserFieldOrdinals.UserRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(UserFields.UserId, UserFieldOrdinals.UserId, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(UserFields.UserFirst, UserFieldOrdinals.UserFirst, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(UserFields.UserStaff, UserFieldOrdinals.UserStaff, AVImarkDataType.AVImarkByte);
			this.AddFieldDefinition(UserFields.UserInactive, UserFieldOrdinals.UserInactive, AVImarkDataType.AVImarkBool);
			this.AddFieldDefinition(UserFields.UserDoctor, UserFieldOrdinals.UserDoctor, AVImarkDataType.AVImarkBool);
			this.AddFieldDefinition(UserFields.UserLast, UserFieldOrdinals.UserLast, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(UserFields.UserAdded, UserFieldOrdinals.UserAdded, AVImarkDataType.AVImarkDate);
		}

		public List<UserData> UserList()
		{
			List<UserData> userList = new List<UserData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				UserData userData = new UserData(this, x);
				userList.Add(userData);
			}

			return userList;
		}
	}
}
