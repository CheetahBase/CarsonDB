using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Audit : CarsonBackend
	{
		private enum AuditFieldOrdinals
		{
			Id,
			AuditRecd,
			AuditDate,
			AuditTime,
			AuditStation,
			AuditUser,
			AuditClient,
			AuditAnimal,
			AuditFunction,
			AuditComments
		}

		public static class AuditFields
		{
			public const string Id = "ID";
			public const string AuditRecd = "AUDIT_RECD";
			public const string AuditDate = "AUDIT_DATE";
			public const string AuditTime = "AUDIT_TIME";
			public const string AuditStation = "AUDIT_STATION";
			public const string AuditUser = "AUDIT_USER";
			public const string AuditClient = "AUDIT_CLIENT";
			public const string AuditAnimal = "AUDIT_ANIMAL";
			public const string AuditFunction = "AUDIT_FUNCTION";
			public const string AuditComments = "AUDIT_COMMENTS";
		}

		public class AuditData : ICrc
		{
			private int _recordNumber;
			private Audit _audit;

			public AuditData(Audit audit, int recordNumber)
			{
				_audit = audit;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int auditId = 0;

					try
					{
						auditId = _audit.IntegerFieldValue(AuditFieldOrdinals.Id, _recordNumber);
						return auditId;
					}
					catch (RecordLockedException)
					{
						auditId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return auditId;
					}
				}
			}

			public string AuditRecd
			{
				get
				{
					return _audit.CharacterFieldValue(AuditFieldOrdinals.AuditRecd, _recordNumber);
				}
			}

			public DateTime AuditDate
			{
				get
				{
					return _audit.DateFieldValue(AuditFieldOrdinals.AuditDate, _recordNumber);
				}
			}

			public string AuditTime
			{
				get
				{
					return _audit.TimeFieldValue(AuditFieldOrdinals.AuditTime, _recordNumber);
				}
			}

			public string AuditStation
			{
				get
				{
					return _audit.StringFieldValue(AuditFieldOrdinals.AuditStation, _recordNumber);
				}
			}

			public string AuditUser
			{
				get
				{
					return _audit.StringFieldValue(AuditFieldOrdinals.AuditUser, _recordNumber);
				}
			}

			public int AuditClient
			{
				get
				{
					return _audit.IntegerFieldValue(AuditFieldOrdinals.AuditClient, _recordNumber);
				}
			}

			public int AuditAnimal
			{
				get
				{
					return _audit.IntegerFieldValue(AuditFieldOrdinals.AuditAnimal, _recordNumber);
				}
			}

			public string AuditFunction
			{
				get
				{
					return _audit.StringFieldValue(AuditFieldOrdinals.AuditFunction, _recordNumber);
				}
			}

			public string AuditComments
			{
				get
				{
					return _audit.StringFieldValue(AuditFieldOrdinals.AuditComments, _recordNumber);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Audit()
		{
			AddDatabaseDefinition(CarsonStaticSettings.DatabasePath);
		}

		public Audit(string databasePath)
		{
			AddDatabaseDefinition(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinition(string databasePath)
		{
			this.DatabaseName = "AUDIT";
			this.DatabasePath = databasePath;
			this.TableId = TableInstance.Audit;

			this.AddFieldDefinition(AuditFields.Id, AuditFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(AuditFields.AuditRecd, AuditFieldOrdinals.AuditRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(AuditFields.AuditDate, AuditFieldOrdinals.AuditDate, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(AuditFields.AuditTime, AuditFieldOrdinals.AuditTime, AVImarkDataType.AVImarkTime);
			this.AddFieldDefinition(AuditFields.AuditStation, AuditFieldOrdinals.AuditStation, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(AuditFields.AuditUser, AuditFieldOrdinals.AuditUser, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(AuditFields.AuditClient, AuditFieldOrdinals.AuditClient, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(AuditFields.AuditAnimal, AuditFieldOrdinals.AuditAnimal, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(AuditFields.AuditFunction, AuditFieldOrdinals.AuditFunction, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(AuditFields.AuditComments, AuditFieldOrdinals.AuditComments, AVImarkDataType.AVImarkDynamicString);
		}

		public List<AuditData> AuditList()
		{
			List<AuditData> auditList = new List<AuditData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				AuditData auditData = new AuditData(this, x);
				auditList.Add(auditData);
			}

			return auditList;
		}
	}
}
