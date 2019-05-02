using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Account : CarsonBackend
	{
		private enum AccountFieldOrdinals : int
		{
			Id,
			AccountCode,
			AccountRecd,
			AccountDate,
			AccountType,
			AccountDoctor,
			AccountDescription,
			AccountTranstax,
			AccountTranltax,
			AccountInvno,
			AccountQty,
			AccountAmount,
			AccountFees,
			AccountService,
			AccountAnimal,
			AccountClient,
			AccountAddedBy
		}

		public static class AccountFields
		{
			public const string Id = "ID";
			public const string AccountCode = "ACCOUNT_CODE";
			public const string AccountRecd = "ACCOUNT_RECD";
			public const string AccountDate = "ACCOUNT_DATE";
			public const string AccountType = "ACCOUNT_TYPE";
			public const string AccountDoctor = "ACCOUNT_DOCTOR";
			public const string AccountDescription = "ACCOUNT_DESCRIPTION";
			public const string AccountTranstax = "ACCOUNT_TRANSTAX";
			public const string AccountTranltax = "ACCOUNT_TRANLTAX";
			public const string AccountInvno = "ACCOUNT_INVNO";
			public const string AccountQty = "ACCOUNT_QTY";
			public const string AccountAmount = "ACCOUNT_AMOUNT";
			public const string AccountFees = "ACCOUNT_FEES";
			public const string AccountService = "ACCOUNT_SERVICE";
			public const string AccountAnimal = "ACCOUNT_ANIMAL";
			public const string AccountClient = "ACCOUNT_CLIENT";
			public const string AccountAddedBy = "ACCOUNT_ADDED_BY";
		}

		public class AccountData : ICrc
		{
			private int _recordNumber;
			private Account _account;

			public AccountData(Account account, int recordNumber)
			{
				_account = account;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int accountId = 0;

					try
					{
						accountId = _account.IntegerFieldValue(AccountFieldOrdinals.Id, _recordNumber);
						return accountId;
					}
					catch (RecordLockedException)
					{
						accountId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return accountId;
					}
				}
			}

			public string AccountCode
			{
				get
				{
					return _account.StringFieldValue(AccountFieldOrdinals.AccountCode, _recordNumber);
				}
			}

			public string AccountRecd
			{
				get
				{
					return _account.CharacterFieldValue(AccountFieldOrdinals.AccountRecd, _recordNumber);
				}
			}

			public DateTime AccountDate
			{
				get
				{
					return _account.DateFieldValue(AccountFieldOrdinals.AccountDate, _recordNumber);
				}
			}

			public string AccountType
			{
				get
				{
					return _account.CharacterFieldValue(AccountFieldOrdinals.AccountType, _recordNumber);
				}
			}

			public string AccountDoctor
			{
				get
				{
					return _account.StringFieldValue(AccountFieldOrdinals.AccountDoctor, _recordNumber);
				}
			}

			public string AccountDescription
			{
				get
				{
					return _account.StringFieldValue(AccountFieldOrdinals.AccountDescription, _recordNumber);
				}
			}

			public Decimal AccountTranstax
			{
				get
				{
					return _account.FloatFieldValue(AccountFieldOrdinals.AccountTranstax, _recordNumber);
				}
			}

			public Decimal AccountTranltax
			{
				get
				{
					return _account.FloatFieldValue(AccountFieldOrdinals.AccountTranltax, _recordNumber);
				}
			}

			public int AccountInvno
			{
				get
				{
					return _account.IntegerFieldValue(AccountFieldOrdinals.AccountInvno, _recordNumber);
				}
			}

			public Decimal AccountQty
			{
				get
				{
					return _account.ImpliedDecimalFieldValue(AccountFieldOrdinals.AccountQty, _recordNumber);
				}
			}

			public Decimal AccountAmount
			{
				get
				{
					return _account.SignedImpliedDecimalFieldValue(AccountFieldOrdinals.AccountAmount, _recordNumber);
				}
			}

			public Decimal AccountFees
			{
				get
				{
					return _account.ImpliedDecimalFieldValue(AccountFieldOrdinals.AccountFees, _recordNumber);
				}
			}

			public int AccountService
			{
				get
				{
					return _account.IntegerFieldValue(AccountFieldOrdinals.AccountService, _recordNumber);
				}
			}

			public int AccountAnimal
			{
				get
				{
					return _account.IntegerFieldValue(AccountFieldOrdinals.AccountAnimal, _recordNumber);
				}
			}
			public int AccountClient
			{
				get
				{
					return _account.IntegerFieldValue(AccountFieldOrdinals.AccountClient, _recordNumber);
				}
			}

			public string AccountAddedBy
			{
				get
				{
					return _account.StringFieldValue(AccountFieldOrdinals.AccountAddedBy, _recordNumber);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Account()
		{
			this.AddDatabaseDefinitions(CarsonStaticSettings.DatabasePath);
		}

		public Account(string databasePath)
		{
			this.AddDatabaseDefinitions(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "ACCOUNT";
			this.DatabasePath = databasePath;
			this.TableId = TableInstance.Account;

			this.AddFieldDefinition(AccountFields.Id, AccountFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(AccountFields.AccountCode, AccountFieldOrdinals.AccountCode, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(AccountFields.AccountRecd, AccountFieldOrdinals.AccountRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(AccountFields.AccountDate, AccountFieldOrdinals.AccountDate, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(AccountFields.AccountType, AccountFieldOrdinals.AccountType, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(AccountFields.AccountDoctor, AccountFieldOrdinals.AccountDoctor, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(AccountFields.AccountDescription, AccountFieldOrdinals.AccountDescription, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(AccountFields.AccountTranstax, AccountFieldOrdinals.AccountTranstax, AVImarkDataType.AVImarkSingle, 0.01m);
			this.AddFieldDefinition(AccountFields.AccountTranltax, AccountFieldOrdinals.AccountTranltax, AVImarkDataType.AVImarkSingle, 0.01m);
			this.AddFieldDefinition(AccountFields.AccountInvno, AccountFieldOrdinals.AccountInvno, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(AccountFields.AccountQty, AccountFieldOrdinals.AccountQty, AVImarkDataType.AVImarkImpliedDecimal);
			this.AddFieldDefinition(AccountFields.AccountAmount, AccountFieldOrdinals.AccountAmount, AVImarkDataType.AVImarkSignedImpliedDecimal);
			this.AddFieldDefinition(AccountFields.AccountFees, AccountFieldOrdinals.AccountFees, AVImarkDataType.AVImarkImpliedDecimal);
			this.AddFieldDefinition(AccountFields.AccountService, AccountFieldOrdinals.AccountService, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(AccountFields.AccountAnimal, AccountFieldOrdinals.AccountAnimal, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(AccountFields.AccountClient, AccountFieldOrdinals.AccountClient, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(AccountFields.AccountAddedBy, AccountFieldOrdinals.AccountAddedBy, AVImarkDataType.AVImarkDynamicString);
		}

		public List<AccountData> AccountList()
		{
			List<AccountData> accountList = new List<AccountData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				AccountData accountData = new AccountData(this, x);
				accountList.Add(accountData);
			}

			return accountList;
		}
	}
}
