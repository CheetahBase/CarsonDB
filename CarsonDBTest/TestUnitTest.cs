using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class TestUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void TestTestInit()
		{
			Shared.BuildTestRecords();
		}

		[TestCleanup]
		public void TestTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void TestTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Test test = new Test(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					var testList = test.TestList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "test.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!TestRecordCompare(testList, fields, lineNumber))
						{
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}
			}

			Assert.IsFalse(false, "TestTest1 Passed!!");
		}

		[TestMethod]
		public void TestTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					string[] fields = null;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "test.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "test.csv")).Length;

					if (y > lineCount - 1)
						break;

					var line = new List<string>(lines)[y];

					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					fields = parser.ReadFields();

					using (Test test = new Test(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						if (fields[0].Length > 0)
							test.AddFilterCriteria(Test.TestFields.TestRecd, ComparisonType.EqualTo, fields[0]);

						if (fields[1].Length > 0)
							test.AddFilterCriteria(Test.TestFields.TestHistory, ComparisonType.EqualTo, Convert.ToInt32(fields[1]));

						if (!fields[2].Contains("1900"))
							test.AddFilterCriteria(Test.TestFields.TestDate, ComparisonType.EqualTo, DateTime.Parse(fields[2]));

						if (fields[3].Length > 0)
							test.AddFilterCriteria(Test.TestFields.TestName, ComparisonType.EqualTo, fields[3]);

						if (fields[4].Length > 0)
							test.AddFilterCriteria(Test.TestFields.TestInstrument, ComparisonType.EqualTo, fields[4]);

						if (fields[5].Length > 0)
							test.AddFilterCriteria(Test.TestFields.TestStatus, ComparisonType.EqualTo, Convert.ToInt32(fields[5]));

						if (fields[6].Length > 0)
							test.AddFilterCriteria(Test.TestFields.TestPatient, ComparisonType.EqualTo, Convert.ToInt32(fields[6]));

						var testList = test.TestList();

						if (testList.Count == 0)
						{
							Assert.Fail("Match failed on line: " + (y + 1).ToString());
						}
					}
				}
			}

			Assert.IsTrue(true, "TestTest2 Passed!!");
		}

		private bool TestRecordCompare(List<Test.TestData> testList, string[] fields, int lineNumber)
		{
			if (testList[lineNumber].TestRecd == fields[0] && Shared.CompareAmount(testList[lineNumber].TestHistory, fields[1]) && (testList[lineNumber].TestDate - DateTime.Parse(fields[2])).TotalDays == 0 && testList[lineNumber].TestName == fields[3] &&
					testList[lineNumber].TestInstrument == fields[4] && Shared.CompareAmount(testList[lineNumber].TestStatus, fields[5]) && Shared.CompareAmount(testList[lineNumber].TestPatient, fields[6])
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
