using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class ServMemoUnitTest
	{
		//there is no legacy for Prescrip - it did not exist back then.
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void ServMemoTestInit()
		{
			Shared.BuildServMemoRecords();
		}

		[TestCleanup]
		public void ServMemoTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void ServMemoTest1()
		{
			int lineNumber = 0;

			using (ServMemo servMemo = new ServMemo(_modernSourcePath))
			{
				var servMemoList = servMemo.ServMemoList();

				var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "servmemo.csv"));

				foreach (var line in lines)
				{
					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					string[] fields = parser.ReadFields();

					if (!ServMemoRecordCompare(servMemoList, fields, lineNumber))
					{
						Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
					}

					lineNumber++;
				}
			}

			Assert.IsFalse(false, "ServMemoTest1 Passed!!");
		}

		[TestMethod]
		public void ServMemoTest2()
		{
			for (int y = 0; y < 200; y++)
			{
				string[] fields = null;

				var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "servmemo.csv"));
				int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "servmemo.csv")).Length;

				if (y > lineCount - 1)
					break;

				var line = new List<string>(lines)[y];

				TextFieldParser parser = new TextFieldParser(new StringReader(line));
				parser.HasFieldsEnclosedInQuotes = true;
				parser.SetDelimiters(",");
				fields = parser.ReadFields();

				using (ServMemo servMemo = new ServMemo(_modernSourcePath))
				{
					if (fields[0].Length > 0)
						servMemo.AddFilterCriteria(ServMemo.ServMemoFields.MemoTextRecd, ComparisonType.EqualTo, fields[0]);

					if (fields[1].Length > 0)
						servMemo.AddFilterCriteria(ServMemo.ServMemoFields.MemoTextText, ComparisonType.EqualTo, fields[1]);

					if (fields[2].Length > 0)
						servMemo.AddFilterCriteria(ServMemo.ServMemoFields.MemoTextNext, ComparisonType.EqualTo, Convert.ToInt32(fields[2]));

					if (fields[3].Length > 0)
						servMemo.AddFilterCriteria(ServMemo.ServMemoFields.MemoTextPrev, ComparisonType.EqualTo, Convert.ToInt32(fields[3]));

					if (fields[4].Length > 0)
						servMemo.AddFilterCriteria(ServMemo.ServMemoFields.MemoTextParent, ComparisonType.EqualTo, Convert.ToInt32(fields[4]));

					var serviceList = servMemo.ServMemoList();

					if (serviceList.Count == 0)
					{
						Assert.Fail("Match failed on line: " + (y + 1).ToString());
					}
				}
			}

			Assert.IsTrue(true, "ServMemoTest2 Passed!!");
		}

		private bool ServMemoRecordCompare(List<ServMemo.ServMemoData> servMemoList, string[] fields, int lineNumber)
		{
			if (servMemoList[lineNumber].MemoTextRecd == fields[0] && servMemoList[lineNumber].MemoTextText == fields[1] && Shared.CompareAmount(servMemoList[lineNumber].MemoTextNext, fields[2]) &&
				Shared.CompareAmount(servMemoList[lineNumber].MemoTextPrev, fields[3]) && Shared.CompareAmount(servMemoList[lineNumber].MemoTextParent, fields[4]))
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
