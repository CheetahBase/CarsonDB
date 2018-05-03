using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class QuoteUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void QuoteTestInit()
		{
			Shared.BuildQuoteRecords();
		}

		[TestCleanup]
		public void QuoteTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void QuoteTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Quote quote = new Quote(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					var quoteList = quote.QuoteList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "quote.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!QuoteRecordCompare(quoteList, fields, lineNumber))
						{
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}
			}

			Assert.IsFalse(false, "QuoteTest1 Passed!!");
		}

		[TestMethod]
		public void QuoteTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					string[] fields = null;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "quote.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "quote.csv")).Length;

					if (y > lineCount - 1)
						break;

					var line = new List<string>(lines)[y];

					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					fields = parser.ReadFields();

					using (Quote quote = new Quote(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						if (fields[0].Length > 0)
							quote.AddFilterCriteria(Quote.QuoteFields.QuoteName, ComparisonType.EqualTo, fields[0]);

						if (fields[1].Length > 0)
							quote.AddFilterCriteria(Quote.QuoteFields.QuoteRecd, ComparisonType.EqualTo, fields[1]);

						if (!fields[2].Contains("1900"))
							quote.AddFilterCriteria(Quote.QuoteFields.QuoteDate, ComparisonType.EqualTo, DateTime.Parse(fields[2]));

						if (fields[3].Length > 0)
							quote.AddFilterCriteria(Quote.QuoteFields.QuoteDoctor, ComparisonType.EqualTo, fields[3]);

						if (fields[5].Length > 0)
							quote.AddFilterCriteria(Quote.QuoteFields.QuoteExpiresIn, ComparisonType.EqualTo, Convert.ToInt32(fields[5]));

						if (fields[6].Length > 0)
							quote.AddFilterCriteria(Quote.QuoteFields.QuoteAnimal, ComparisonType.EqualTo, Convert.ToInt32(fields[6]));

						var quoteList = quote.QuoteList();

						if (quoteList.Count == 0)
						{
							Assert.Fail("Match failed on line: " + (y + 1).ToString());
						}
					}
				}
			}

			Assert.IsTrue(true, "QuoteTest2 Passed!!");
		}

		private bool QuoteRecordCompare(List<Quote.QuoteData> quoteList, string[] fields, int lineNumber)
		{
			if (quoteList[lineNumber].QuoteName == fields[0] && quoteList[lineNumber].QuoteRecd == fields[1] && (quoteList[lineNumber].QuoteDate - DateTime.Parse(fields[2])).TotalDays == 0 && quoteList[lineNumber].QuoteDoctor == fields[3] &&
					Shared.CompareBool(quoteList[lineNumber].QuoteActive, fields[4]) && Shared.CompareAmount(quoteList[lineNumber].QuoteExpiresIn, fields[5]) && Shared.CompareAmount(quoteList[lineNumber].QuoteAnimal, fields[6])
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
