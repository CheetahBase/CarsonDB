using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class QuotailUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void QuotailTestInit()
		{
			Shared.BuildQuotailRecords();
		}

		[TestCleanup]
		public void QuotailTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void QuotailTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Quotail quotail = new Quotail(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					var quotailList = quotail.QuotailList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "quotail.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!QuotailRecordCompare(quotailList, fields, lineNumber))
						{
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}
			}

			Assert.IsFalse(false, "QuotailTest1 Passed!!");
		}

		[TestMethod]
		public void QuotailTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					string[] fields = null;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "quotail.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "quotail.csv")).Length;

					if (y > lineCount - 1)
						break;

					var line = new List<string>(lines)[y];

					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					fields = parser.ReadFields();

					using (Quotail quotail = new Quotail(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						if (fields[0].Length > 0)
							quotail.AddFilterCriteria(Quotail.QuotailFields.QuotailRecd, ComparisonType.EqualTo, fields[0]);

						if (fields[1].Length > 0)
							quotail.AddFilterCriteria(Quotail.QuotailFields.QuotailCodes, ComparisonType.EqualTo, fields[1]);

						if (fields[2].Length > 0)
							quotail.AddFilterCriteria(Quotail.QuotailFields.QuotailQuantity, ComparisonType.EqualTo, Convert.ToDecimal(fields[2]));

						if (fields[3].Length > 0)
							quotail.AddFilterCriteria(Quotail.QuotailFields.QuotailDescription, ComparisonType.EqualTo, fields[3]);

						if (fields[4].Length > 0)
							quotail.AddFilterCriteria(Quotail.QuotailFields.QuotailCode, ComparisonType.EqualTo, fields[4]);

						if (fields[5].Length > 0)
							quotail.AddFilterCriteria(Quotail.QuotailFields.QuotailTotal, ComparisonType.EqualTo, Convert.ToDecimal(fields[5]));

						if (fields[6].Length > 0)
							quotail.AddFilterCriteria(Quotail.QuotailFields.QuotailVariance, ComparisonType.EqualTo, Convert.ToInt16(fields[6]));

						if (fields[7].Length > 0)
							quotail.AddFilterCriteria(Quotail.QuotailFields.QuotailQuote, ComparisonType.EqualTo, Convert.ToInt32(fields[7]));

						var quotailList = quotail.QuotailList();

						if (quotailList.Count == 0)
						{
							Assert.Fail("Match failed on line: " + (y + 1).ToString());
						}
					}
				}
			}

			Assert.IsTrue(true, "QuotailTest2 Passed!!");
		}

		private bool QuotailRecordCompare(List<Quotail.QuotailData> quotailList, string[] fields, int lineNumber)
		{
			if (quotailList[lineNumber].QuotailRecd == fields[0] && quotailList[lineNumber].QuotailCodes == fields[1] && Shared.CompareAmount(quotailList[lineNumber].QuotailQuantity, fields[2]) && quotailList[lineNumber].QuotailDescription == fields[3] &&
					quotailList[lineNumber].QuotailCode == fields[4] && Shared.CompareAmount(quotailList[lineNumber].QuotailTotal, fields[5]) && Shared.CompareAmount(quotailList[lineNumber].QuotailVariance, fields[6]) &&
					Shared.CompareAmount(quotailList[lineNumber].QuotailQuote, fields[7]))
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
