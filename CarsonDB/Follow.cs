using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Follow : CarsonBackend
	{
		private enum FollowFieldOrdinals
		{
			Id,
			FollowRecd,
			FollowCode,
			FollowCritical,
			FollowDue,
			FollowOrigin,
			FollowSubject,
			FollowDoctor,
			FollowClient,
			FollowAnimal,
			FollowNote,
			FollowMoreStuff,
			FollowConfirm,
			FollowGhost
		}

		public static class FollowFields
		{
			public const string Id = "ID";
			public const string FollowRecd = "FOLLOW_RECD";
			public const string FollowCode = "FOLLOW_CODE";
			public const string FollowCritical = "FOLLOW_CRITICAL";
			public const string FollowDue = "FOLLOW_DUE";
			public const string FollowOrigin = "FOLLOW_ORIGIN";
			public const string FollowSubject = "FOLLOW_SUBJECT";
			public const string FollowDoctor = "FOLLOW_DOCTOR";
			public const string FollowClient = "FOLLOW_CLIENT";
			public const string FollowAnimal = "FOLLOW_ANIMAL";
			public const string FollowNote = "FOLLOW_NOTE";
			public const string FollowMoreStuff = "FOLLOW_MORESTUFF";
			public const string FollowConfirm = "FOLLOW_CONFIRM";
			public const string FollowGhost = "FOLLOW_GHOST";
		}

		public class FollowData : ICrc
		{
			private int _recordNumber;
			private Follow _follow;

			public FollowData(Follow follow, int recordNumber)
			{
				_follow = follow;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int followId = 0;

					try
					{
						followId = _follow.IntegerFieldValue(FollowFieldOrdinals.Id, _recordNumber);
						return followId;
					}
					catch (RecordLockedException)
					{
						followId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return followId;
					}
				}
			}

			public string FollowRecd
			{
				get
				{
					return _follow.CharacterFieldValue(FollowFieldOrdinals.FollowRecd, _recordNumber);
				}
			}

			public string FollowCode
			{
				get
				{
					return _follow.StringFieldValue(FollowFieldOrdinals.FollowCode, _recordNumber);
				}
			}

			public bool FollowCritical
			{
				get
				{
					return _follow.BooleanFieldValue(FollowFieldOrdinals.FollowCritical, _recordNumber);
				}
			}

			public DateTime FollowDue
			{
				get
				{
					return _follow.DateFieldValue(FollowFieldOrdinals.FollowDue, _recordNumber);
				}
			}

			public DateTime FollowOrigin
			{
				get
				{
					return _follow.DateFieldValue(FollowFieldOrdinals.FollowOrigin, _recordNumber);
				}
			}

			public string FollowSubject
			{
				get
				{
					return _follow.StringFieldValue(FollowFieldOrdinals.FollowSubject, _recordNumber);
				}
			}

			public string FollowDoctor
			{
				get
				{
					return _follow.StringFieldValue(FollowFieldOrdinals.FollowDoctor, _recordNumber);
				}
			}

			public int FollowClient
			{
				get
				{
					return _follow.IntegerFieldValue(FollowFieldOrdinals.FollowClient, _recordNumber);
				}
			}

			public int FollowAnimal
			{
				get
				{
					return _follow.IntegerFieldValue(FollowFieldOrdinals.FollowAnimal, _recordNumber);
				}
			}

			public string FollowNote
			{
				get
				{
					return _follow.StringFieldValue(FollowFieldOrdinals.FollowNote, _recordNumber);
				}
			}

			public int FollowMoreStuff
			{
				get
				{
					return _follow.IntegerFieldValue(FollowFieldOrdinals.FollowMoreStuff, _recordNumber);
				}
			}

			public int FollowConfirm
			{
				get
				{
					return _follow.IntegerFieldValue(FollowFieldOrdinals.FollowConfirm, _recordNumber);
				}
			}

			public bool FollowGhost
			{
				get
				{
					return _follow.BooleanFieldValue(FollowFieldOrdinals.FollowGhost, _recordNumber);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Follow()
		{
			AddDatabaseDefinitions(CarsonStaticSettings.DatabasePath);
		}

		public Follow(string databasePath)
		{
			AddDatabaseDefinitions(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "FOLLOW";
			this.DatabasePath = databasePath;
			this.TableId = TableInstance.Follow;

			this.AddFieldDefinition(FollowFields.Id, FollowFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(FollowFields.FollowRecd, FollowFieldOrdinals.FollowRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(FollowFields.FollowCode, FollowFieldOrdinals.FollowCode, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(FollowFields.FollowCritical, FollowFieldOrdinals.FollowCritical, AVImarkDataType.AVImarkBool);
			this.AddFieldDefinition(FollowFields.FollowDue, FollowFieldOrdinals.FollowDue, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(FollowFields.FollowOrigin, FollowFieldOrdinals.FollowOrigin, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(FollowFields.FollowSubject, FollowFieldOrdinals.FollowSubject, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(FollowFields.FollowDoctor, FollowFieldOrdinals.FollowDoctor, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(FollowFields.FollowClient, FollowFieldOrdinals.FollowClient, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(FollowFields.FollowAnimal, FollowFieldOrdinals.FollowAnimal, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(FollowFields.FollowNote, FollowFieldOrdinals.FollowNote, AVImarkDataType.AVImarkLinkToWp);
			this.AddFieldDefinition(FollowFields.FollowMoreStuff, FollowFieldOrdinals.FollowMoreStuff, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(FollowFields.FollowConfirm, FollowFieldOrdinals.FollowConfirm, AVImarkDataType.AVImarkByte);
			this.AddFieldDefinition(FollowFields.FollowGhost, FollowFieldOrdinals.FollowGhost, AVImarkDataType.AVImarkBool);
		}

		public List<FollowData> FollowList()
		{
			List<FollowData> followList = new List<FollowData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				FollowData followData = new FollowData(this, x);
				followList.Add(followData);
			}

			return followList;
		}
	}
}
