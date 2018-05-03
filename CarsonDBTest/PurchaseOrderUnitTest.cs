using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class PurchaseOrderUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void PurchaseOrderTestInit()
		{
			Shared.BuildPoRecords();
		}

		[TestCleanup]
		public void PurchaseOrderTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void PurchaseOrderTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (PurchaseOrder purchaseOrder = new PurchaseOrder(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					var poList = purchaseOrder.PurchaseOrderList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "po.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!PurchaseOrderRecordCompare(poList, fields, lineNumber))
						{
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}
			}

			Assert.IsFalse(false, "PurchaseOrderTest1 Passed!!");
		}

		[TestMethod]
		public void PurchaseOrderTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					string[] fields = null;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "po.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "po.csv")).Length;

					if (y > lineCount - 1)
						break;

					var line = new List<string>(lines)[y];

					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					fields = parser.ReadFields();

					using (PurchaseOrder purchaseOrder = new PurchaseOrder(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						if (fields[0].Length > 0)
							purchaseOrder.AddFilterCriteria(PurchaseOrder.PurchaseOrderFields.PurchaseOrderSite, ComparisonType.EqualTo, Convert.ToInt32(fields[0]));

						if (fields[1].Length > 0)
							purchaseOrder.AddFilterCriteria(PurchaseOrder.PurchaseOrderFields.PurchaseOrderOrder, ComparisonType.EqualTo, fields[1]);

						if (!fields[2].Contains("1900"))
							purchaseOrder.AddFilterCriteria(PurchaseOrder.PurchaseOrderFields.PurchaseOrderDate, ComparisonType.EqualTo, DateTime.Parse(fields[2]));

						if (fields[3].Length > 0)
							purchaseOrder.AddFilterCriteria(PurchaseOrder.PurchaseOrderFields.PurchaseOrderInvoice, ComparisonType.EqualTo, fields[3]);

						if (fields[4].Length > 0)
							purchaseOrder.AddFilterCriteria(PurchaseOrder.PurchaseOrderFields.PurchaseOrderQuantity, ComparisonType.EqualTo, Convert.ToInt32(fields[4]));

						if (fields[5].Length > 0)
							purchaseOrder.AddFilterCriteria(PurchaseOrder.PurchaseOrderFields.PurchaseOrderRecd, ComparisonType.EqualTo, fields[5]);

						if (fields[6].Length > 0)
							purchaseOrder.AddFilterCriteria(PurchaseOrder.PurchaseOrderFields.PurchaseOrderCode, ComparisonType.EqualTo, fields[6]);

						if (fields[7].Length > 0)
							purchaseOrder.AddFilterCriteria(PurchaseOrder.PurchaseOrderFields.PurchaseOrderVendor, ComparisonType.EqualTo, Convert.ToInt32(fields[7]));

						if (fields[8].Length > 0)
							purchaseOrder.AddFilterCriteria(PurchaseOrder.PurchaseOrderFields.PurchaseOrderCost, ComparisonType.EqualTo, Convert.ToDecimal(fields[8]));

						if (fields[9].Length > 0)
							purchaseOrder.AddFilterCriteria(PurchaseOrder.PurchaseOrderFields.PurchaseOrderPack, ComparisonType.EqualTo, Convert.ToInt32(fields[9]));

						var poList = purchaseOrder.PurchaseOrderList();

						if (poList.Count == 0)
						{
							Assert.Fail("Match failed on line: " + (y + 1).ToString());
						}
					}
				}
			}

			Assert.IsTrue(true, "PurchaseOrderTest2 Passed!!");
		}

		private bool PurchaseOrderRecordCompare(List<PurchaseOrder.PurchaseOrderData> purchaseOrderList, string[] fields, int lineNumber)
		{
			if (Shared.CompareAmount(purchaseOrderList[lineNumber].PurchaseOrderSite, fields[0]) && purchaseOrderList[lineNumber].PurchaseOrderOrder == fields[1] && (purchaseOrderList[lineNumber].PurchaseOrderDate - DateTime.Parse(fields[2])).TotalDays == 0 && purchaseOrderList[lineNumber].PurchaseOrderInvoice == fields[3] &&
					Shared.CompareAmount(purchaseOrderList[lineNumber].PurchaseOrderQuantity, fields[4]) && purchaseOrderList[lineNumber].PurchaseOrderRecd == fields[5] && purchaseOrderList[lineNumber].PurchaseOrderCode == fields[6] &&
					Shared.CompareAmount(purchaseOrderList[lineNumber].PurchaseOrderVendor, fields[7]) && Shared.CompareAmount(purchaseOrderList[lineNumber].PurchaseOrderCost, fields[8]) && Shared.CompareAmount(purchaseOrderList[lineNumber].PurchaseOrderPack, fields[9])
					)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
