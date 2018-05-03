using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Quote : CarsonBackend
	{
		private enum QuoteFieldOrdinals
		{
			Id,
			QuoteName,
			QuoteRecd,
			QuoteDate,
			QuoteDoctor,
			QuoteActive,
			QuoteExpiresIn,
			QuoteAnimal
		}

		public static class QuoteFields
		{
			public const string Id = "ID";
			public const string QuoteName = "QUOTE_NAME";
			public const string QuoteRecd = "QUOTE_RECD";
			public const string QuoteDate = "QUOTE_DATE";
			public const string QuoteDoctor = "QUOTE_DOCTOR";
			public const string QuoteActive = "QUOTE_ACTIVE";
			public const string QuoteExpiresIn = "QUOTE_EXPIRESIN";
			public const string QuoteAnimal = "QUOTE_ANIMAL";
		}

		public class QuoteData : ICrc
		{
			private int _recordNumber;
			private Quote _quote;

			public QuoteData(Quote quote, int recordNumber)
			{
				_quote = quote;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int quoteId = 0;

					try
					{
						quoteId = _quote.IntegerFieldValue(QuoteFieldOrdinals.Id, _recordNumber);
						return quoteId;
					}
					catch (RecordLockedException)
					{
						quoteId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return quoteId;
					}
				}
			}

			public string QuoteName
			{
				get
				{
					return _quote.StringFieldValue(QuoteFieldOrdinals.QuoteName, _recordNumber);
				}
			}

			public string QuoteRecd
			{
				get
				{
					return _quote.CharacterFieldValue(QuoteFieldOrdinals.QuoteRecd, _recordNumber);
				}
			}

			public DateTime QuoteDate
			{
				get
				{
					return _quote.DateFieldValue(QuoteFieldOrdinals.QuoteDate, _recordNumber);
				}
			}

			public string QuoteDoctor
			{
				get
				{
					return _quote.StringFieldValue(QuoteFieldOrdinals.QuoteDoctor, _recordNumber);
				}
			}

			public bool QuoteActive
			{
				get
				{
					return _quote.BooleanFieldValue(QuoteFieldOrdinals.QuoteActive, _recordNumber);
				}
			}

			public int QuoteExpiresIn
			{
				get
				{
					return _quote.IntegerFieldValue(QuoteFieldOrdinals.QuoteExpiresIn, _recordNumber);
				}
			}

			public int QuoteAnimal
			{
				get
				{
					return _quote.IntegerFieldValue(QuoteFieldOrdinals.QuoteAnimal, _recordNumber);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Quote()
		{
			AddDatabaseDefinitions(CarsonStaticSettings.DatabasePath);
		}

		public Quote(string databasePath)
		{
			AddDatabaseDefinitions(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "QUOTE";
			this.DatabasePath = databasePath;
			this.TableId = this.TableId = TableInstance.Quote;

			this.AddFieldDefinition(QuoteFields.Id, QuoteFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(QuoteFields.QuoteName, QuoteFieldOrdinals.QuoteName, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(QuoteFields.QuoteRecd, QuoteFieldOrdinals.QuoteRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(QuoteFields.QuoteDate, QuoteFieldOrdinals.QuoteDate, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(QuoteFields.QuoteDoctor, QuoteFieldOrdinals.QuoteDoctor, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(QuoteFields.QuoteActive, QuoteFieldOrdinals.QuoteActive, AVImarkDataType.AVImarkBool);
			this.AddFieldDefinition(QuoteFields.QuoteExpiresIn, QuoteFieldOrdinals.QuoteExpiresIn, AVImarkDataType.AVImarkWord);
			this.AddFieldDefinition(QuoteFields.QuoteAnimal, QuoteFieldOrdinals.QuoteAnimal, AVImarkDataType.AVImarkDoubleWord);
		}

		public List<QuoteData> QuoteList()
		{
			List<QuoteData> quoteList = new List<QuoteData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				QuoteData quoteData = new QuoteData(this, x);
				quoteList.Add(quoteData);
			}

			return quoteList;
		}
	}
}
