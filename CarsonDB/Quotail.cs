using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Quotail : CarsonBackend
	{
		private enum QuotailFieldOrdinals
		{
			Id,
			QuotailRecd,
			QuotailCodes,
			QuotailQuantity,
			QuotailVariance,
			QuotailTotal,
			QuotailDescription,
			QuotailCode,
			QuotailQuote
		}

		public static class QuotailFields
		{
			public const string Id = "ID";
			public const string QuotailRecd = "QUOTAIL_RECD";
			public const string QuotailCodes = "QUOTAIL_CODES";
			public const string QuotailQuantity = "QUOTAIL_QUANTITY";
			public const string QuotailVariance = "QUOTAIL_VARIANCE";
			public const string QuotailTotal = "QUOTAIL_TOTAL";
			public const string QuotailDescription = "QUOTAIL_DESCRIPTION";
			public const string QuotailCode = "QUOTAIL_CODE";
			public const string QuotailQuote = "QUOTAIL_QUOTE";
		}

		public class QuotailData : ICrc
		{
			private int _recordNumber;
			private Quotail _quotail;

			public QuotailData(Quotail quotail, int recordNumber)
			{
				_quotail = quotail;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int quotailId = 0;

					try
					{
						quotailId = _quotail.IntegerFieldValue(QuotailFieldOrdinals.Id, _recordNumber);
						return quotailId;
					}
					catch (RecordLockedException)
					{
						quotailId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return quotailId;
					}
				}
			}

			public string QuotailRecd
			{
				get
				{
					return _quotail.CharacterFieldValue(QuotailFieldOrdinals.QuotailRecd, _recordNumber);
				}
			}

			public string QuotailCodes
			{
				get
				{
					return _quotail.StringFieldValue(QuotailFieldOrdinals.QuotailCodes, _recordNumber);
				}
			}

			public decimal QuotailQuantity
			{
				get
				{
					return _quotail.SignedImpliedDecimalFieldValue(QuotailFieldOrdinals.QuotailQuantity, _recordNumber);
				}
			}

			public int QuotailVariance
			{
				get
				{
					return _quotail.IntegerFieldValue(QuotailFieldOrdinals.QuotailVariance, _recordNumber);
				}
			}

			public decimal QuotailTotal
			{
				get
				{
					return _quotail.SignedImpliedDecimalFieldValue(QuotailFieldOrdinals.QuotailTotal, _recordNumber);
				}
			}

			public string QuotailDescription
			{
				get
				{
					return _quotail.StringFieldValue(QuotailFieldOrdinals.QuotailDescription, _recordNumber);
				}
			}

			public string QuotailCode
			{
				get
				{
					return _quotail.StringFieldValue(QuotailFieldOrdinals.QuotailCode, _recordNumber);
				}
			}

			public int QuotailQuote
			{
				get
				{
					return _quotail.IntegerFieldValue(QuotailFieldOrdinals.QuotailQuote, _recordNumber);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Quotail()
		{
			AddDatabaseDefinitions(CarsonStaticSettings.DatabasePath);
		}

		public Quotail(string databasePath)
		{
			AddDatabaseDefinitions(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "QUOTAIL";
			this.DatabasePath = databasePath;
			this.TableId = TableInstance.Quotail;

			this.AddFieldDefinition(QuotailFields.Id, QuotailFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(QuotailFields.QuotailRecd, QuotailFieldOrdinals.QuotailRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(QuotailFields.QuotailCodes, QuotailFieldOrdinals.QuotailCodes, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(QuotailFields.QuotailQuantity, QuotailFieldOrdinals.QuotailQuantity, AVImarkDataType.AVImarkSignedImpliedDecimal);
			this.AddFieldDefinition(QuotailFields.QuotailVariance, QuotailFieldOrdinals.QuotailVariance, AVImarkDataType.AVImarkLongInteger);
			this.AddFieldDefinition(QuotailFields.QuotailTotal, QuotailFieldOrdinals.QuotailTotal, AVImarkDataType.AVImarkSignedImpliedDecimal);
			this.AddFieldDefinition(QuotailFields.QuotailDescription, QuotailFieldOrdinals.QuotailDescription, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(QuotailFields.QuotailCode, QuotailFieldOrdinals.QuotailCode, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(QuotailFields.QuotailQuote, QuotailFieldOrdinals.QuotailQuote, AVImarkDataType.AVImarkDoubleWord);
		}

		public List<QuotailData> QuotailList()
		{
			List<QuotailData> quotailList = new List<QuotailData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				QuotailData quotailData = new QuotailData(this, x);
				quotailList.Add(quotailData);
			}

			return quotailList;
		}
	}
}
