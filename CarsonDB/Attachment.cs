using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Attachment : CarsonBackend
	{
		private enum AttachFieldOrdinals
		{
			Id,
			AttachmentRecd,
			AttachmentFileName,
			AttachmentDescription,
			AttachmentParentis,
			AttachmentParent
		}

		public static class AttachmentFields
		{
			public const string Id = "ID";
			public const string AttachmentRecd = "ATTACHMENT_RECD";
			public const string AttachmentFileName = "ATTACHMENT_FILENAME";
			public const string AttachmentDescription = "ATTACHMENT_DESCRIPTION";
			public const string AttachmentParentis = "ATTACHMENT_PARENTIS";
			public const string AttachmentParent = "ATTACHMENT_PARENT";
		}

		public class AttachmentData : ICrc
		{
			private int _recordNumber;
			private Attachment _attachment;

			public AttachmentData(Attachment attachment, int recordNumber)
			{
				_attachment = attachment;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int attachmentId = 0;

					try
					{
						attachmentId = _attachment.IntegerFieldValue(AttachFieldOrdinals.Id, _recordNumber);
						return attachmentId;
					}
					catch (RecordLockedException)
					{
						attachmentId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return attachmentId;
					}
				}
			}

			public string AttachmentRecd
			{
				get
				{
					return _attachment.CharacterFieldValue(AttachFieldOrdinals.AttachmentRecd, _recordNumber);
				}
			}

			public string AttachmentFileName
			{
				get
				{
					return _attachment.StringFieldValue(AttachFieldOrdinals.AttachmentFileName, _recordNumber);
				}
			}

			public string AttachmentDescription
			{
				get
				{
					return _attachment.StringFieldValue(AttachFieldOrdinals.AttachmentDescription, _recordNumber);
				}
			}

			public int AttachmentParentis
			{
				get
				{
					return _attachment.IntegerFieldValue(AttachFieldOrdinals.AttachmentParentis, _recordNumber);
				}
			}

			public int AttachmentParent
			{
				get
				{
					return _attachment.IntegerFieldValue(AttachFieldOrdinals.AttachmentParent, _recordNumber);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Attachment()
		{
			AddDatabaseDefinition(CarsonStaticSettings.DatabasePath);
		}

		public Attachment(string databasePath)
		{
			AddDatabaseDefinition(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinition(string databasePath)
		{
			this.DatabaseName = "ATTACH";
			this.DatabasePath = databasePath;
			this.TableId = TableInstance.Attach;

			this.AddFieldDefinition(AttachmentFields.Id, AttachFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(AttachmentFields.AttachmentRecd, AttachFieldOrdinals.AttachmentRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(AttachmentFields.AttachmentFileName, AttachFieldOrdinals.AttachmentFileName, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(AttachmentFields.AttachmentDescription, AttachFieldOrdinals.AttachmentDescription, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(AttachmentFields.AttachmentParentis, AttachFieldOrdinals.AttachmentParentis, AVImarkDataType.AVImarkByte);
			this.AddFieldDefinition(AttachmentFields.AttachmentParent, AttachFieldOrdinals.AttachmentParent, AVImarkDataType.AVImarkDoubleWord);
		}

		public List<AttachmentData> AttachmentList()
		{
			List<AttachmentData> attachmentList = new List<AttachmentData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				AttachmentData attachmentData = new AttachmentData(this, x);
				attachmentList.Add(attachmentData);
			}

			return attachmentList;
		}
	}
}
