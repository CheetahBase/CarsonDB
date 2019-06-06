using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Client : CarsonBackend
	{
		private enum ClientFieldOrdinals
		{
			Id,
			ClientRecd,
			ClientTitle,
			ClientLast,
			ClientAddress,
			ClientAddress2,
			ClientCity,
			ClientState,
			ClientZip,
			ClientArea,
			ClientPhone,
			ClientFirst,
			ClientBusiness,
			ClientCodes,
			ClientClass,
			ClientReference,
			ClientUntil,
			ClientSuspend,
			ClientFlags,
			ClientAdded,
			ClientDoctor,
			ClientCompany,
			ClientSpouse,
			ClientEmail,
			ClientCounty,
			ClientFolder,
			ClientCell
		}

		public static class ClientFields
		{
			public const string Id = "CLIENT_ID";
			public const string ClientRecd = "CLIENT_RECD";
			public const string ClientTitle = "CLIENT_TITLE";
			public const string ClientLast = "CLIENT_LAST";
			public const string ClientAddress = "CLIENT_ADDRESS";
			public const string ClientAddress2 = "CLIENT_ADDRESS2";
			public const string ClientCity = "CLIENT_CITY";
			public const string ClientState = "CLIENT_STATE";
			public const string ClientZip = "CLIENT_ZIP";
			public const string ClientArea = "CLIENT_AREA";
			public const string ClientPhone = "CLIENT_PHONE";
			public const string ClientFirst = "CLIENT_FIRST";
			public const string ClientBusiness = "CLIENT_BUSINESS";
			public const string ClientCodes = "CLIENT_CODES";
			public const string ClientClass = "CLIENT_CLASS";
			public const string ClientReference = "CLIENT_REFERENCE";
			public const string ClientUntil = "CLIENT_UNTIL";
			public const string ClientSuspend = "CLIENT_SUSPEND";
			public const string ClientFlags = "CLIENT_FLAGS";
			public const string ClientAdded = "CLIENT_ADDED";
			public const string ClientDoctor = "CLIENT_DOCTOR";
			public const string ClientCompany = "CLIENT_COMPANY";
			public const string ClientSpouse = "CLIENT_SPOUSE";
			public const string ClientEmail = "CLIENT_EMAIL";
			public const string ClientCounty = "CLIENT_COUNTY";
			public const string ClientFolder = "CLIENT_FOLDER";
			public const string ClientCell = "CLIENT_CELL";
		}

		public class ClientData : ICrc
		{
			private int _recordNumber;
			private Client _client;

			public ClientData(Client client, int recordNumber)
			{
				_client = client;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int clientId = 0;

					try
					{
						clientId = _client.IntegerFieldValue(ClientFieldOrdinals.Id, _recordNumber);
						return clientId;
					}
					catch (RecordLockedException)
					{
						clientId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return clientId;
					}
				}
			}

			public string ClientRecd
			{
				get
				{
					return _client.CharacterFieldValue(ClientFieldOrdinals.ClientRecd, _recordNumber);
				}
			}

			public string ClientTitle
			{
				get
				{
					return _client.StringFieldValue(ClientFieldOrdinals.ClientTitle, _recordNumber);
				}
			}

			public string ClientLast
			{
				get
				{
					return _client.StringFieldValue(ClientFieldOrdinals.ClientLast, _recordNumber);
				}
			}

			public string ClientAddress
			{
				get
				{
					return _client.StringFieldValue(ClientFieldOrdinals.ClientAddress, _recordNumber);
				}
			}

			public string ClientAddress2
			{
				get
				{
					return _client.StringFieldValue(ClientFieldOrdinals.ClientAddress2, _recordNumber);
				}
			}

			public string ClientCity
			{
				get
				{
					return _client.StringFieldValue(ClientFieldOrdinals.ClientCity, _recordNumber);
				}
			}

			public string ClientState
			{
				get
				{
					return _client.StringFieldValue(ClientFieldOrdinals.ClientState, _recordNumber);
				}
			}

			public string ClientZip
			{
				get
				{
					return _client.StringFieldValue(ClientFieldOrdinals.ClientZip, _recordNumber);
				}
			}

			public string ClientArea
			{
				get
				{
					return _client.StringFieldValue(ClientFieldOrdinals.ClientArea, _recordNumber);
				}
			}

			public string ClientPhone
			{
				get
				{
					return _client.StringFieldValue(ClientFieldOrdinals.ClientPhone, _recordNumber);
				}
			}

			public string ClientFirst
			{
				get
				{
					return _client.StringFieldValue(ClientFieldOrdinals.ClientFirst, _recordNumber);
				}
			}

			public string ClientBusiness
			{
				get
				{
					return _client.StringFieldValue(ClientFieldOrdinals.ClientBusiness, _recordNumber);
				}
			}

			public string ClientCodes
			{
				get
				{
					return _client.StringFieldValue(ClientFieldOrdinals.ClientCodes, _recordNumber);
				}
			}

			public int ClientClass
			{
				get
				{
					return _client.IntegerFieldValue(ClientFieldOrdinals.ClientClass, _recordNumber);
				}
			}

			public string ClientReference
			{
				get
				{
					return _client.StringFieldValue(ClientFieldOrdinals.ClientReference, _recordNumber);
				}
			}

			public DateTime ClientUntil
			{
				get
				{
					return _client.DateFieldValue(ClientFieldOrdinals.ClientUntil, _recordNumber);
				}
			}

			public bool ClientSuspend
			{
				get
				{
					return _client.BooleanFieldValue(ClientFieldOrdinals.ClientSuspend, _recordNumber);
				}
			}

			public int ClientFlags
			{
				get
				{
					return _client.ByteFieldValue(ClientFieldOrdinals.ClientFlags, _recordNumber);
				}
			}

			public DateTime ClientAdded
			{
				get
				{
					return _client.DateFieldValue(ClientFieldOrdinals.ClientAdded, _recordNumber);
				}
			}

			public string ClientDoctor
			{
				get
				{
					return _client.StringFieldValue(ClientFieldOrdinals.ClientDoctor, _recordNumber);
				}
			}

			public int ClientCompany
			{
				get
				{
					return _client.IntegerFieldValue(ClientFieldOrdinals.ClientCompany, _recordNumber);
				}
			}

			public string ClientSpouse
			{
				get
				{
					return _client.StringFieldValue(ClientFieldOrdinals.ClientSpouse, _recordNumber);
				}
			}

			public string ClientEmail
			{
				get
				{
					return _client.StringFieldValue(ClientFieldOrdinals.ClientEmail, _recordNumber);
				}
			}

			public string ClientCounty
			{
				get
				{
					return _client.StringFieldValue(ClientFieldOrdinals.ClientCounty, _recordNumber);
				}
			}

			public int ClientFolder
			{
				get
				{
					return _client.IntegerFieldValue(ClientFieldOrdinals.ClientFolder, _recordNumber);
				}
			}

			public string ClientCell
			{
				get
				{
					return _client.StringFieldValue(ClientFieldOrdinals.ClientCell, _recordNumber);
				}
			}


			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Client()
		{
			AddDatabaseDefinitions(CarsonStaticSettings.DatabasePath);
		}

		public Client(string databasePath)
		{
			AddDatabaseDefinitions(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "CLIENT";
			this.DatabasePath = databasePath;
			this.TableId =TableInstance.Client;

			this.AddFieldDefinition(ClientFields.Id, ClientFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(ClientFields.ClientRecd, ClientFieldOrdinals.ClientRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(ClientFields.ClientTitle, ClientFieldOrdinals.ClientTitle, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(ClientFields.ClientLast, ClientFieldOrdinals.ClientLast, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(ClientFields.ClientAddress, ClientFieldOrdinals.ClientAddress, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(ClientFields.ClientAddress2, ClientFieldOrdinals.ClientAddress2, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(ClientFields.ClientCity, ClientFieldOrdinals.ClientCity, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(ClientFields.ClientState, ClientFieldOrdinals.ClientState, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(ClientFields.ClientZip, ClientFieldOrdinals.ClientZip, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(ClientFields.ClientArea, ClientFieldOrdinals.ClientArea, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(ClientFields.ClientPhone, ClientFieldOrdinals.ClientPhone, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(ClientFields.ClientFirst, ClientFieldOrdinals.ClientFirst, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(ClientFields.ClientBusiness, ClientFieldOrdinals.ClientBusiness, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(ClientFields.ClientCodes, ClientFieldOrdinals.ClientCodes, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(ClientFields.ClientClass, ClientFieldOrdinals.ClientClass, AVImarkDataType.AVImarkByte);
			this.AddFieldDefinition(ClientFields.ClientReference, ClientFieldOrdinals.ClientReference, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(ClientFields.ClientUntil, ClientFieldOrdinals.ClientUntil, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(ClientFields.ClientSuspend, ClientFieldOrdinals.ClientSuspend, AVImarkDataType.AVImarkBool);
			this.AddFieldDefinition(ClientFields.ClientFlags, ClientFieldOrdinals.ClientFlags, AVImarkDataType.AVImarkByte);
			this.AddFieldDefinition(ClientFields.ClientAdded, ClientFieldOrdinals.ClientAdded, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(ClientFields.ClientDoctor, ClientFieldOrdinals.ClientDoctor, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(ClientFields.ClientCompany, ClientFieldOrdinals.ClientCompany, AVImarkDataType.AVImarkByte);
			this.AddFieldDefinition(ClientFields.ClientSpouse, ClientFieldOrdinals.ClientSpouse, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(ClientFields.ClientEmail, ClientFieldOrdinals.ClientEmail, AVImarkDataType.AVImarkLinkToPhrase);
			this.AddFieldDefinition(ClientFields.ClientCounty, ClientFieldOrdinals.ClientCounty, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(ClientFields.ClientFolder, ClientFieldOrdinals.ClientFolder, AVImarkDataType.AVImarkWord);
			this.AddFieldDefinition(ClientFields.ClientCell, ClientFieldOrdinals.ClientCell, AVImarkDataType.AVImarkLinkToPhrase);
		}

		public List<ClientData> ClientList()
		{
			List<ClientData> clientList = new List<ClientData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				ClientData clientData = new ClientData(this, x);
				clientList.Add(clientData);
			}

			return clientList;
		}
	}
}
