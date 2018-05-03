using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class SplitUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void SplitTestInit()
		{
			Shared.BuildSplitRecords();
		}

		[TestCleanup]
		public void SplitTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void SplitTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Split split = new Split(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					var splitList = split.SplitList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "split.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!SplitRecordCompare(splitList, fields, lineNumber))
						{
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}
			}

			Assert.IsFalse(false, "SplitTest1 Passed!!");
		}

		[TestMethod]
		public void SplitTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					string[] fields = null;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "split.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "split.csv")).Length;

					if (y > lineCount - 1)
						break;

					var line = new List<string>(lines)[y];

					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					fields = parser.ReadFields();

					using (Split split = new Split(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						if (fields[0].Length > 0)
							split.AddFilterCriteria(Split.SplitFields.SplitRecd, ComparisonType.EqualTo, fields[0]);

						if (fields[1].Length > 0)
							split.AddFilterCriteria(Split.SplitFields.SplitClient, ComparisonType.EqualTo, Convert.ToInt32(fields[1]));

						if (fields[2].Length > 0)
							split.AddFilterCriteria(Split.SplitFields.SplitPercent, ComparisonType.EqualTo, Convert.ToInt32(fields[2]));

						if (fields[3].Length > 0)
							split.AddFilterCriteria(Split.SplitFields.SplitAnimal, ComparisonType.EqualTo, Convert.ToInt32(fields[3]));

						if (!fields[4].Contains("1900"))
							split.AddFilterCriteria(Split.SplitFields.SplitEndDate, ComparisonType.EqualTo, DateTime.Parse(fields[4]));

						var splitList = split.SplitList();

						if (splitList.Count == 0)
						{
							Assert.Fail("Match failed on line: " + (y + 1).ToString());
						}
					}
				}
			}

			Assert.IsTrue(true, "SplitTest2 Passed!!");
		}

		private bool SplitRecordCompare(List<Split.SplitData> splitList, string[] fields, int lineNumber)
		{
			if (splitList[lineNumber].SplitRecd == fields[0] && Shared.CompareAmount(splitList[lineNumber].SplitClient, fields[1]) && Shared.CompareAmount(splitList[lineNumber].SplitPercent, fields[2]) && Shared.CompareAmount(splitList[lineNumber].SplitAnimal, fields[3]) &&
					(splitList[lineNumber].SplitEndDate - DateTime.Parse(fields[4])).TotalDays == 0 && Shared.CompareBool(splitList[lineNumber].SplitUltimateAuthority, fields[5]))
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
