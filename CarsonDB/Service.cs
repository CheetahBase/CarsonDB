using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Service : CarsonBackend
	{
		private enum ServiceFieldOrdinals
		{
			Id,
			ServiceDate,
			ServiceTod,
			ServiceRecd,
			ServiceType,
			ServiceCode,
			ServiceDescription,
			ServiceAmount,
			ServiceQuantity,
			ServiceNormal,
			ServiceAccount,
			ServiceNote,
			ServiceAnimal,
			ServiceFlags,
			ServiceForm,
			ServiceResults,
			ServiceSfPublicNotes,
			ServiceSfPublic
		}

		public static class ServiceFields
		{
			public const string Id = "ID";
			public const string ServiceDate = "SERVICE_DATE";
			public const string ServiceTod = "SERVICE_TOD";
			public const string ServiceRecd = "SERVICE_RECD";
			public const string ServiceType = "SERVICE_TYPE";
			public const string ServiceCode = "SERVICE_CODE";
			public const string ServiceDescription = "SERVICE_DESCRIPTION";
			public const string ServiceAmount = "SERVICE_AMOUNT";
			public const string ServiceQuantity = "SERVICE_QUANTITY";
			public const string ServiceNormal = "SERVICE_NORMAL";
			public const string ServiceAccount = "SERVICE_ACCOUNT";
			public const string ServiceNote = "SERVICE_NOTE";
			public const string ServiceAnimal = "SERVICE_ANIMAL";
			public const string ServiceFlags = "SERVICE_FLAGS";
			public const string ServiceForm = "SERVICE_FORM";
			public const string ServiceResults = "SERVICE_RESULTS";
			public const string ServiceSfPublicNotes = "SERVICE_SFPUBLICNOTES";
			public const string ServiceSfPublic = "SERVICE_SFPUBLIC";
		}

		public class ServiceData : ICrc
		{
			private int _recordNumber;
			private Service _service;

			public ServiceData(Service service, int recordNumber)
			{
				_service = service;
				_recordNumber = recordNumber;
			}

			private string _serviceNote = null;

			public int Id
			{
				get
				{
					int serviceId = 0;

					try
					{
						serviceId = _service.IntegerFieldValue(ServiceFieldOrdinals.Id, _recordNumber);
						return serviceId;
					}
					catch (RecordLockedException)
					{
						serviceId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return serviceId;
					}
				}
			}

			public DateTime ServiceDate
			{
				get
				{
					return _service.DateFieldValue(ServiceFieldOrdinals.ServiceDate, _recordNumber);
				}
			}

			public string ServiceTod
			{
				get
				{
					return _service.TimeFieldValue(ServiceFieldOrdinals.ServiceTod, _recordNumber);
				}
			}

			public string ServiceRecd
			{
				get
				{
					return _service.CharacterFieldValue(ServiceFieldOrdinals.ServiceRecd, _recordNumber);
				}
			}

			public string ServiceType
			{
				get
				{
					return _service.CharacterFieldValue(ServiceFieldOrdinals.ServiceType, _recordNumber);
				}
			}

			public string ServiceCode
			{
				get
				{
					return _service.StringFieldValue(ServiceFieldOrdinals.ServiceCode, _recordNumber);
				}
			}

			public string ServiceDescription
			{
				get
				{
					return _service.StringFieldValue(ServiceFieldOrdinals.ServiceDescription, _recordNumber);
				}
			}

			public decimal ServiceAmount
			{
				get
				{
					return _service.SignedImpliedDecimalFieldValue(ServiceFieldOrdinals.ServiceAmount, _recordNumber);
				}
			}

			public decimal ServiceQuantity
			{
				get
				{
					return _service.SignedImpliedDecimalFieldValue(ServiceFieldOrdinals.ServiceQuantity, _recordNumber);
				}
			}

			public decimal ServiceNormal
			{
				get
				{
					return _service.SignedImpliedDecimalFieldValue(ServiceFieldOrdinals.ServiceNormal, _recordNumber);
				}
			}

			public int ServiceAccount
			{
				get
				{
					return _service.IntegerFieldValue(ServiceFieldOrdinals.ServiceAccount, _recordNumber);
				}
			}

			public string ServiceNote
			{
				get
				{
					if (_serviceNote == null)
					{
						_serviceNote = _service.StringFieldValue(ServiceFieldOrdinals.ServiceNote, _recordNumber);
					}

					return _serviceNote;
				}
			}

			public int ServiceAnimal
			{
				get
				{
					return _service.IntegerFieldValue(ServiceFieldOrdinals.ServiceAnimal, _recordNumber);
				}
			}

			public int ServiceFlags
			{
				get
				{
					return _service.IntegerFieldValue(ServiceFieldOrdinals.ServiceFlags, _recordNumber);
				}
			}

			public int ServiceForm
			{
				get
				{
					return _service.IntegerFieldValue(ServiceFieldOrdinals.ServiceForm, _recordNumber);
				}
			}

			public int ServiceResults
			{
				get
				{
					return _service.IntegerFieldValue(ServiceFieldOrdinals.ServiceResults, _recordNumber);
				}
			}

			public bool ServiceSfPublicNotes
			{
				get
				{
					return _service.BitFieldValue(ServiceFieldOrdinals.ServiceSfPublicNotes, _recordNumber, 2);
				}
			}

			public bool ServiceSfPublic
			{
				get
				{
					return _service.BitFieldValue(ServiceFieldOrdinals.ServiceSfPublic, _recordNumber, 4);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Service()
		{
			AddDatabaseDefinitions(CarsonStaticSettings.DatabasePath);
		}

		public Service(string databasePath)
		{
			AddDatabaseDefinitions(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "SERVICE";
			this.DatabasePath = databasePath;
			this.TableId = TableInstance.Service;

			this.AddFieldDefinition(ServiceFields.Id, ServiceFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(ServiceFields.ServiceDate, ServiceFieldOrdinals.ServiceDate, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(ServiceFields.ServiceTod, ServiceFieldOrdinals.ServiceTod, AVImarkDataType.AVImarkTime);
			this.AddFieldDefinition(ServiceFields.ServiceRecd, ServiceFieldOrdinals.ServiceRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(ServiceFields.ServiceType, ServiceFieldOrdinals.ServiceType, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(ServiceFields.ServiceCode, ServiceFieldOrdinals.ServiceCode, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(ServiceFields.ServiceDescription, ServiceFieldOrdinals.ServiceDescription, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(ServiceFields.ServiceAmount, ServiceFieldOrdinals.ServiceAmount, AVImarkDataType.AVImarkSignedImpliedDecimal);
			this.AddFieldDefinition(ServiceFields.ServiceQuantity, ServiceFieldOrdinals.ServiceQuantity, AVImarkDataType.AVImarkSignedImpliedDecimal);
			this.AddFieldDefinition(ServiceFields.ServiceNormal, ServiceFieldOrdinals.ServiceNormal, AVImarkDataType.AVImarkSignedImpliedDecimal);
			this.AddFieldDefinition(ServiceFields.ServiceAccount, ServiceFieldOrdinals.ServiceAccount, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(ServiceFields.ServiceNote, ServiceFieldOrdinals.ServiceNote, AVImarkDataType.AVImarkLinkToWp);
			this.AddFieldDefinition(ServiceFields.ServiceAnimal, ServiceFieldOrdinals.ServiceAnimal, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(ServiceFields.ServiceFlags, ServiceFieldOrdinals.ServiceFlags, AVImarkDataType.AVImarkByte);
			this.AddFieldDefinition(ServiceFields.ServiceForm, ServiceFieldOrdinals.ServiceForm, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(ServiceFields.ServiceResults, ServiceFieldOrdinals.ServiceResults, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(ServiceFields.ServiceSfPublicNotes, ServiceFieldOrdinals.ServiceSfPublicNotes, AVImarkDataType.AVImarkBit);
			this.AddFieldDefinition(ServiceFields.ServiceSfPublic, ServiceFieldOrdinals.ServiceSfPublic, AVImarkDataType.AVImarkBit);
		}

		public List<ServiceData> ServiceList()
		{
			List<ServiceData> serviceList = new List<ServiceData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				ServiceData serviceData = new ServiceData(this, x);
				serviceList.Add(serviceData);
			}

			return serviceList;
		}
	}
}
