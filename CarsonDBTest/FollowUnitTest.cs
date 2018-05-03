using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class FollowUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void FollowTestInit()
		{
			Shared.BuildFollowRecords();
		}

		[TestCleanup]
		public void FollowTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void FollowTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Follow follow = new Follow(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					var followList = follow.FollowList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "follow.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!FollowRecordCompare(followList, fields, lineNumber))
						{
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}
			}

			Assert.IsFalse(false, "FollowTest1 Passed!!");
		}

		[TestMethod]
		public void FollowTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					string[] fields = null;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "follow.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "follow.csv")).Length;

					if (y > lineCount - 1)
						break;

					var line = new List<string>(lines)[y];

					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					fields = parser.ReadFields();

					using (Follow follow = new Follow(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						if (fields[0].Length > 0)
							follow.AddFilterCriteria(Follow.FollowFields.FollowRecd, ComparisonType.EqualTo, fields[0]);

						if (fields[1].Length > 0)
							follow.AddFilterCriteria(Follow.FollowFields.FollowCode, ComparisonType.EqualTo, fields[1]);

						if (!fields[3].Contains("1900"))
							follow.AddFilterCriteria(Follow.FollowFields.FollowDue, ComparisonType.EqualTo, DateTime.Parse(fields[3]));

						if (!fields[4].Contains("1900"))
							follow.AddFilterCriteria(Follow.FollowFields.FollowOrigin, ComparisonType.EqualTo, DateTime.Parse(fields[4]));

						if (fields[5].Length > 0)
							follow.AddFilterCriteria(Follow.FollowFields.FollowSubject, ComparisonType.EqualTo, fields[5]);

						if (fields[6].Length > 0)
							follow.AddFilterCriteria(Follow.FollowFields.FollowDoctor, ComparisonType.EqualTo, fields[6]);

						if (fields[7].Length > 0)
							follow.AddFilterCriteria(Follow.FollowFields.FollowClient, ComparisonType.EqualTo, Convert.ToInt32(fields[7]));

						if (fields[8].Length > 0)
							follow.AddFilterCriteria(Follow.FollowFields.FollowAnimal, ComparisonType.EqualTo, Convert.ToInt32(fields[8]));

						if (fields[9].Length > 0)
							follow.AddFilterCriteria(Follow.FollowFields.FollowNote, ComparisonType.EqualTo, fields[9]);

						if (fields[10].Length > 0)
							follow.AddFilterCriteria(Follow.FollowFields.FollowMoreStuff, ComparisonType.EqualTo, Convert.ToInt32(fields[10]));

						if (fields[11].Length > 0)
							follow.AddFilterCriteria(Follow.FollowFields.FollowConfirm, ComparisonType.EqualTo, Convert.ToInt32(fields[11]));

						var followList = follow.FollowList();

						if (followList.Count == 0)
						{
							Assert.Fail("Match failed on line: " + (y + 1).ToString());
						}
					}
				}
			}

			Assert.IsTrue(true, "FollowTest2 Passed!!");
		}

		private bool FollowRecordCompare(List<Follow.FollowData> followList, string[] fields, int lineNumber)
		{
			if (followList[lineNumber].FollowRecd == fields[0] && followList[lineNumber].FollowCode == fields[1] && Shared.CompareBool(followList[lineNumber].FollowCritical, fields[2]) && (followList[lineNumber].FollowDue - DateTime.Parse(fields[3])).TotalDays == 0 &&
					(followList[lineNumber].FollowOrigin - DateTime.Parse(fields[4])).TotalDays == 0 && followList[lineNumber].FollowSubject == fields[5] && followList[lineNumber].FollowDoctor == fields[6] &&
					Shared.CompareAmount(followList[lineNumber].FollowClient, fields[7]) && Shared.CompareAmount(followList[lineNumber].FollowAnimal, fields[8]) && followList[lineNumber].FollowNote == fields[9] &&
					Shared.CompareAmount(followList[lineNumber].FollowMoreStuff, fields[10]) && Shared.CompareAmount(followList[lineNumber].FollowConfirm, fields[11]) && Shared.CompareBool(followList[lineNumber].FollowGhost, fields[12])
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
