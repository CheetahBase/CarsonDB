using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class TableUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void TableTestInit()
		{
			Shared.BuildTableRecords();
		}

		[TestCleanup]
		public void TableTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void TableTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Table table = new Table(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					var tableList = table.TableList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "table.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!TableRecordCompare(tableList, fields, lineNumber))
						{
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}
			}

			Assert.IsFalse(false, "TableTest1 Passed!!");
		}

		[TestMethod]
		public void TableTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					string[] fields = null;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "table.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "table.csv")).Length;

					if (y > lineCount - 1)
						break;

					var line = new List<string>(lines)[y];

					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					fields = parser.ReadFields();

					using (Table table = new Table(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						if (fields[0].Length > 0)
							table.AddFilterCriteria(Table.TableFields.TableRecd, ComparisonType.EqualTo, fields[0]);

						if (fields[1].Length > 0)
							table.AddFilterCriteria(Table.TableFields.TableCode, ComparisonType.EqualTo, fields[1]);

						if (fields[2].Length > 0)
							table.AddFilterCriteria(Table.TableFields.TableName, ComparisonType.EqualTo, fields[2]);

						var tableList = table.TableList();

						if (tableList.Count == 0)
						{
							Assert.Fail("Match failed on line: " + (y + 1).ToString());
						}
					}
				}
			}

			Assert.IsTrue(true, "TableTest2 Passed!!");
		}

		private bool TableRecordCompare(List<Table.TableData> tableList, string[] fields, int lineNumber)
		{
			if (tableList[lineNumber].TableRecd == fields[0] && tableList[lineNumber].TableCode == fields[1] && tableList[lineNumber].TableName == fields[2])
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
