using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class EntryUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void EntryTestInit()
		{
			Shared.BuildEntryRecords();
		}

		[TestCleanup]
		public void EntryTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void EntryTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Entry entry = new Entry(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					var entryList = entry.EntryList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "entry.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!EntryRecordCompare(entryList, fields, lineNumber))
						{
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}
			}

			Assert.IsFalse(false, "EntryTest1 Passed!!");
		}

		[TestMethod]
		public void EntryTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					string[] fields = null;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "entry.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "entry.csv")).Length;

					if (y > lineCount - 1)
						break;

					var line = new List<string>(lines)[y];

					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					fields = parser.ReadFields();

					using (Entry entry = new Entry(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						if (fields[0].Length > 0)
							entry.AddFilterCriteria(Entry.EntryFields.EntryRecd, ComparisonType.EqualTo, fields[0]);

						if (fields[1].Length > 0)
							entry.AddFilterCriteria(Entry.EntryFields.EntryCode, ComparisonType.EqualTo, fields[1]);

						if (fields[2].Length > 0)
							entry.AddFilterCriteria(Entry.EntryFields.EntryDescription, ComparisonType.EqualTo, fields[2]);

						if (fields[3].Length > 0)
							entry.AddFilterCriteria(Entry.EntryFields.EntryTable, ComparisonType.EqualTo, Convert.ToInt32(fields[3]));

						var entryList = entry.EntryList();

						if (entryList.Count == 0)
						{
							Assert.Fail("Match failed on line: " + (y + 1).ToString());
						}
					}
				}
			}

			Assert.IsTrue(true, "EntryTest2 Passed!!");
		}

		private bool EntryRecordCompare(List<Entry.EntryData> entryList, string[] fields, int lineNumber)
		{
			if (entryList[lineNumber].EntryRecd == fields[0] && entryList[lineNumber].EntryCode == fields[1] && entryList[lineNumber].EntryDescription == fields[2] &&
					Shared.CompareAmount(entryList[lineNumber].EntryTable, fields[3]))
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
