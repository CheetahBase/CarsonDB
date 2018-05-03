using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class PurchaseOrder : CarsonBackend
	{
		private enum PurchaseOrderFieldOrdinals
		{
			Id,
			PurchaseOrderSite,
			PurchaseOrderOrder,
			PurchaseOrderDate,
			PurchaseOrderInvoice,
			PurchaseOrderQuantity,
			PurchaseOrderRecd,
			PurchaseOrderCode,
			PurchaseOrderVendor,
			PurchaseOrderCost,
			PurchaseOrderPack
		}

		public static class PurchaseOrderFields
		{
			public const string Id = "ID";
			public const string PurchaseOrderSite = "PO_SITE";
			public const string PurchaseOrderOrder = "PO_ORDER";
			public const string PurchaseOrderDate = "PO_DATE";
			public const string PurchaseOrderInvoice = "PO_INVOICE";
			public const string PurchaseOrderQuantity = "PO_QUANTITY";
			public const string PurchaseOrderRecd = "PO_RECD";
			public const string PurchaseOrderCode = "PO_CODE";
			public const string PurchaseOrderTax = "PO_TAX";
			public const string PurchaseOrderVendor = "PO_VENDOR";
			public const string PurchaseOrderCost = "PO_COST";
			public const string PurchaseOrderPack = "PO_PACK";
		}

		public class PurchaseOrderData : ICrc
		{
			private int _recordNumber;
			private PurchaseOrder _purchaseOrder;

			public PurchaseOrderData(PurchaseOrder purchaseOrder, int recordNumber)
			{
				_purchaseOrder = purchaseOrder;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int purchaseOrderId = 0;

					try
					{
						purchaseOrderId = _purchaseOrder.IntegerFieldValue(PurchaseOrderFieldOrdinals.Id, _recordNumber);
						return purchaseOrderId;
					}
					catch (RecordLockedException)
					{
						purchaseOrderId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return purchaseOrderId;
					}
				}
			}

			public int PurchaseOrderSite
			{
				get
				{
					return _purchaseOrder.IntegerFieldValue(PurchaseOrderFieldOrdinals.PurchaseOrderSite, _recordNumber);
				}
			}

			public string PurchaseOrderOrder
			{
				get
				{
					return _purchaseOrder.StringFieldValue(PurchaseOrderFieldOrdinals.PurchaseOrderOrder, _recordNumber);
				}
			}

			public DateTime PurchaseOrderDate
			{
				get
				{
					return _purchaseOrder.DateFieldValue(PurchaseOrderFieldOrdinals.PurchaseOrderDate, _recordNumber);
				}
			}

			public string PurchaseOrderInvoice
			{
				get
				{
					return _purchaseOrder.StringFieldValue(PurchaseOrderFieldOrdinals.PurchaseOrderInvoice, _recordNumber);
				}
			}

			public int PurchaseOrderQuantity
			{
				get
				{
					return _purchaseOrder.IntegerFieldValue(PurchaseOrderFieldOrdinals.PurchaseOrderQuantity, _recordNumber);
				}
			}

			public string PurchaseOrderRecd
			{
				get
				{
					return _purchaseOrder.CharacterFieldValue(PurchaseOrderFieldOrdinals.PurchaseOrderRecd, _recordNumber);
				}
			}

			public string PurchaseOrderCode
			{
				get
				{
					return _purchaseOrder.StringFieldValue(PurchaseOrderFieldOrdinals.PurchaseOrderCode, _recordNumber);
				}
			}

			public int PurchaseOrderVendor
			{
				get
				{
					return _purchaseOrder.IntegerFieldValue(PurchaseOrderFieldOrdinals.PurchaseOrderVendor, _recordNumber);
				}
			}

			public decimal PurchaseOrderCost
			{
				get
				{
					return _purchaseOrder.ImpliedDecimal2FieldValue(PurchaseOrderFieldOrdinals.PurchaseOrderCost, _recordNumber);
				}
			}

			public int PurchaseOrderPack
			{
				get
				{
					return _purchaseOrder.IntegerFieldValue(PurchaseOrderFieldOrdinals.PurchaseOrderPack, _recordNumber);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public PurchaseOrder()
		{
			AddDatabaseDefinitions(CarsonStaticSettings.DatabasePath);
		}

		public PurchaseOrder(string databasePath)
		{
			AddDatabaseDefinitions(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "PO";
			this.DatabasePath = databasePath;
			this.TableId = TableInstance.PurchaseOrder;

			this.AddFieldDefinition(PurchaseOrderFields.Id, PurchaseOrderFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(PurchaseOrderFields.PurchaseOrderSite, PurchaseOrderFieldOrdinals.PurchaseOrderSite, AVImarkDataType.AVImarkByte);
			this.AddFieldDefinition(PurchaseOrderFields.PurchaseOrderOrder, PurchaseOrderFieldOrdinals.PurchaseOrderOrder, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(PurchaseOrderFields.PurchaseOrderDate, PurchaseOrderFieldOrdinals.PurchaseOrderDate, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(PurchaseOrderFields.PurchaseOrderInvoice, PurchaseOrderFieldOrdinals.PurchaseOrderInvoice, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(PurchaseOrderFields.PurchaseOrderQuantity, PurchaseOrderFieldOrdinals.PurchaseOrderQuantity, AVImarkDataType.AVImarkWord);
			this.AddFieldDefinition(PurchaseOrderFields.PurchaseOrderRecd, PurchaseOrderFieldOrdinals.PurchaseOrderRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(PurchaseOrderFields.PurchaseOrderCode, PurchaseOrderFieldOrdinals.PurchaseOrderCode, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(PurchaseOrderFields.PurchaseOrderVendor, PurchaseOrderFieldOrdinals.PurchaseOrderVendor, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(PurchaseOrderFields.PurchaseOrderCost, PurchaseOrderFieldOrdinals.PurchaseOrderCost, AVImarkDataType.AVImarkImpliedDecimal2);
			this.AddFieldDefinition(PurchaseOrderFields.PurchaseOrderPack, PurchaseOrderFieldOrdinals.PurchaseOrderPack, AVImarkDataType.AVImarkDoubleWord);
		}

		public List<PurchaseOrderData> PurchaseOrderList()
		{
			List<PurchaseOrderData> purchaseOrderList = new List<PurchaseOrderData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				PurchaseOrderData purchaseOrderData = new PurchaseOrderData(this, x);
				purchaseOrderList.Add(purchaseOrderData);
			}

			return purchaseOrderList;
		}
	}
}
