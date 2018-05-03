using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;


namespace CarsonDBTest
{
	[TestClass]
	public class PriceUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void PriceTestInit()
		{
			Shared.BuildPriceRecords();
		}

		[TestCleanup]
		public void PriceTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void PriceTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Price price = new Price(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					var priceList = price.PriceList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "price.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!PriceRecordCompare(priceList, fields, lineNumber))
						{
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}
			}

			Assert.IsFalse(false, "PriceTest1 Passed!!");
		}

		[TestMethod]
		public void PriceTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					string[] fields = null;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "price.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "price.csv")).Length;

					if (y > lineCount - 1)
						break;

					var line = new List<string>(lines)[y];

					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					fields = parser.ReadFields();

					using (Price price = new Price(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						if (fields[0].Length > 0)
							price.AddFilterCriteria(Price.PriceFields.PriceRecd, ComparisonType.EqualTo, fields[0]);

						if (fields[1].Length > 0)
							price.AddFilterCriteria(Price.PriceFields.PriceSite, ComparisonType.EqualTo, Convert.ToInt32(fields[1]));

						if (fields[2].Length > 0)
							price.AddFilterCriteria(Price.PriceFields.PriceCode, ComparisonType.EqualTo, fields[2]);

						if (!fields[3].Contains("1900"))
							price.AddFilterCriteria(Price.PriceFields.PriceDate, ComparisonType.EqualTo, DateTime.Parse(fields[3]));

						if (fields[4].Contains(":"))
							price.AddFilterCriteria(Price.PriceFields.PriceTod, ComparisonType.EqualTo, fields[4]);

						if (fields[5].Length > 0)
							price.AddFilterCriteria(Price.PriceFields.PriceFrom, ComparisonType.EqualTo, Convert.ToDecimal(fields[5]));

						if (fields[6].Length > 0)
							price.AddFilterCriteria(Price.PriceFields.PriceTo, ComparisonType.EqualTo, Convert.ToDecimal(fields[6]));

						if (fields[7].Length > 0)
							price.AddFilterCriteria(Price.PriceFields.PriceSetBy, ComparisonType.EqualTo, fields[7]);

						if (fields[8].Length > 0)
							price.AddFilterCriteria(Price.PriceFields.PriceAction, ComparisonType.EqualTo, Convert.ToInt32(fields[8]));

						var priceList = price.PriceList();

						if (priceList.Count == 0)
						{
							Assert.Fail("Match failed on line: " + (y + 1).ToString());
						}
					}
				}
			}

			Assert.IsTrue(true, "PriceTest2 Passed!!");
		}

		private bool PriceRecordCompare(List<Price.PriceData> priceList, string[] fields, int lineNumber)
		{
			if (priceList[lineNumber].PriceRecd == fields[0] && Shared.CompareAmount(priceList[lineNumber].PriceSite, fields[1]) && priceList[lineNumber].PriceCode == fields[2] && (priceList[lineNumber].PriceDate - DateTime.Parse(fields[3])).TotalDays == 0 &&
					priceList[lineNumber].PriceTod == fields[4] && Shared.CompareAmount(priceList[lineNumber].PriceFrom, fields[5]) && Shared.CompareAmount(priceList[lineNumber].PriceTo, fields[6]) &&
					priceList[lineNumber].PriceSetBy == fields[7] && Shared.CompareAmount(priceList[lineNumber].PriceAction, fields[8]))
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
