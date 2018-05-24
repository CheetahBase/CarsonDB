using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Price : CarsonBackend
	{
		private enum PriceFieldOrdinals
		{
			Id,
			PriceRecd,
			PriceSite,
			PriceCode,
			PriceDate,
			PriceTod,
			PriceFrom,
			PriceTo,
			PriceSetBy,
			PriceAction
		}

		public static class PriceFields
		{
			public const string Id = "ID";
			public const string PriceRecd = "PRICE_RECD";
			public const string PriceSite = "PRICE_SITE";
			public const string PriceCode = "PRICE_CODE";
			public const string PriceDate = "PRICE_DATE";
			public const string PriceTod = "PRICE_TOD";
			public const string PriceFrom = "PRICE_FROM";
			public const string PriceTo = "PRICE_TO";
			public const string PriceSetBy = "PRICE_SET_BY";
			public const string PriceAction = "PRICE_ACTION";
		}

		public class PriceData : ICrc
		{
			private int _recordNumber;
			private Price _price;

			public PriceData(Price price, int recordNumber)
			{
				_price = price;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int priceId = 0;

					try
					{
						priceId = _price.IntegerFieldValue(PriceFieldOrdinals.Id, _recordNumber);
						return priceId;
					}
					catch (RecordLockedException)
					{
						priceId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return priceId;
					}
				}
			}

			public string PriceRecd
			{
				get
				{
					return _price.CharacterFieldValue(PriceFieldOrdinals.PriceRecd, _recordNumber);
				}
			}

			public int PriceSite
			{
				get
				{
					return _price.IntegerFieldValue(PriceFieldOrdinals.PriceSite, _recordNumber);
				}
			}

			public string PriceCode
			{
				get
				{
					return _price.StringFieldValue(PriceFieldOrdinals.PriceCode, _recordNumber);
				}
			}

			public DateTime PriceDate
			{
				get
				{
					return _price.DateFieldValue(PriceFieldOrdinals.PriceDate, _recordNumber);
				}
			}

			public string PriceTod
			{
				get
				{
					return _price.TimeFieldValue(PriceFieldOrdinals.PriceTod, _recordNumber);
				}
			}

			public decimal PriceFrom
			{
				get
				{
					return _price.SignedImpliedDecimalFieldValue(PriceFieldOrdinals.PriceFrom, _recordNumber);
				}
			}

			public decimal PriceTo
			{
				get
				{
					return _price.SignedImpliedDecimalFieldValue(PriceFieldOrdinals.PriceTo, _recordNumber);
				}
			}

			public string PriceSetBy
			{
				get
				{
					return _price.StringFieldValue(PriceFieldOrdinals.PriceSetBy, _recordNumber);
				}
			}

			public int PriceAction
			{
				get
				{
					return _price.IntegerFieldValue(PriceFieldOrdinals.PriceAction, _recordNumber);
				}
			}


			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Price()
		{
			AddDatabaseDefinitions(CarsonStaticSettings.DatabasePath);
		}

		public Price(string databasePath)
		{
			AddDatabaseDefinitions(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "PRICE";
			this.DatabasePath = databasePath;
			this.TableId = TableInstance.Price;

			this.AddFieldDefinition(PriceFields.Id, PriceFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(PriceFields.PriceRecd, PriceFieldOrdinals.PriceRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(PriceFields.PriceSite, PriceFieldOrdinals.PriceSite, AVImarkDataType.AVImarkByte);
			this.AddFieldDefinition(PriceFields.PriceCode, PriceFieldOrdinals.PriceCode, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(PriceFields.PriceDate, PriceFieldOrdinals.PriceDate, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(PriceFields.PriceTod, PriceFieldOrdinals.PriceTod, AVImarkDataType.AVImarkTime);
			this.AddFieldDefinition(PriceFields.PriceFrom, PriceFieldOrdinals.PriceFrom, AVImarkDataType.AVImarkSignedImpliedDecimal);
			this.AddFieldDefinition(PriceFields.PriceTo, PriceFieldOrdinals.PriceTo, AVImarkDataType.AVImarkSignedImpliedDecimal);
			this.AddFieldDefinition(PriceFields.PriceSetBy, PriceFieldOrdinals.PriceSetBy, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(PriceFields.PriceAction, PriceFieldOrdinals.PriceAction, AVImarkDataType.AVImarkByte);
		}

		public List<PriceData> PriceList()
		{
			List<PriceData> priceList = new List<PriceData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				PriceData priceData = new PriceData(this, x);
				priceList.Add(priceData);
			}

			return priceList;
		}
	}
}
