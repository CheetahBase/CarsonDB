using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class ReminderUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void ReminderTestInit()
		{
			Shared.BuildReminderRecords();
		}

		[TestCleanup]
		public void ReminderTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void ReminderTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Reminder reminder = new Reminder(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					var reminderList = reminder.ReminderList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "reminder.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!ReminderRecordCompare(reminderList, fields, lineNumber))
						{
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}
			}

			Assert.IsFalse(false, "ReminderTest1 Passed!!");
		}

		[TestMethod]
		public void ReminderTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					string[] fields = null;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "reminder.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "reminder.csv")).Length;

					if (y > lineCount - 1)
						break;

					var line = new List<string>(lines)[y];

					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					fields = parser.ReadFields();

					using (Reminder reminder = new Reminder(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						if (fields[0].Length > 0)
							reminder.AddFilterCriteria(Reminder.ReminderFields.ReminderCode, ComparisonType.EqualTo, fields[0]);

						if (fields[1].Length > 0)
							reminder.AddFilterCriteria(Reminder.ReminderFields.ReminderRecd, ComparisonType.EqualTo, fields[1]);

						if (fields[2].Length > 0)
							reminder.AddFilterCriteria(Reminder.ReminderFields.ReminderDescription, ComparisonType.EqualTo, fields[2]);

						if (!fields[3].Contains("1900"))
							reminder.AddFilterCriteria(Reminder.ReminderFields.ReminderReminded, ComparisonType.EqualTo, DateTime.Parse(fields[3]));

						if (!fields[5].Contains("1900"))
							reminder.AddFilterCriteria(Reminder.ReminderFields.ReminderDue, ComparisonType.EqualTo, DateTime.Parse(fields[5]));

						if (fields[6].Length > 0)
							reminder.AddFilterCriteria(Reminder.ReminderFields.ReminderParent, ComparisonType.EqualTo, Convert.ToInt32(fields[6]));

						if (fields[7].Length > 0)
							reminder.AddFilterCriteria(Reminder.ReminderFields.ReminderParentis, ComparisonType.EqualTo, Convert.ToInt32(fields[7]));

						if (fields[8].Length > 0)
							reminder.AddFilterCriteria(Reminder.ReminderFields.ReminderAppropriate, ComparisonType.EqualTo, Convert.ToInt32(fields[8]));

						if (!fields[9].Contains("1900"))
							reminder.AddFilterCriteria(Reminder.ReminderFields.ReminderAdded, ComparisonType.EqualTo, DateTime.Parse(fields[9]));

						var reminderList = reminder.ReminderList();

						if (reminderList.Count == 0)
						{
							Assert.Fail("Match failed on line: " + (y + 1).ToString());
						}
					}
				}
			}

			Assert.IsTrue(true, "ReminderTest2 Passed!!");
		}

		private bool ReminderRecordCompare(List<Reminder.ReminderData> reminderList, string[] fields, int lineNumber)
		{
			if (reminderList[lineNumber].ReminderCode == fields[0] && reminderList[lineNumber].ReminderRecd == fields[1] && reminderList[lineNumber].ReminderDescription == fields[2] && (reminderList[lineNumber].ReminderReminded - DateTime.Parse(fields[3])).TotalDays == 0 &&
					Shared.CompareBool(reminderList[lineNumber].ReminderSuspend, fields[4]) && (reminderList[lineNumber].ReminderDue - DateTime.Parse(fields[5])).TotalDays == 0 && Shared.CompareAmount(reminderList[lineNumber].ReminderParent, fields[6]) &&
					Shared.CompareAmount(reminderList[lineNumber].ReminderParentis, fields[7]) && Shared.CompareAmount(reminderList[lineNumber].ReminderAppropriate, fields[8]) && (reminderList[lineNumber].ReminderAdded - DateTime.Parse(fields[9])).TotalDays == 0
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
