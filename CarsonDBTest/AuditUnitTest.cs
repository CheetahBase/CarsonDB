using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class AuditUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void AuditTestInit()
		{
			Shared.BuildAuditRecords();
		}

		[TestCleanup]
		public void AuditTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void AuditTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Audit audit = new Audit(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					var auditList = audit.AuditList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "audit.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!AuditRecordCompare(auditList, fields, lineNumber))
						{
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}
			}

			Assert.IsFalse(false, "AuditTest1 Passed!!");
		}

		[TestMethod]
		public void AuditTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					string[] fields = null;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "audit.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "audit.csv")).Length;

					if (y > lineCount - 1)
						break;

					var line = new List<string>(lines)[y];

					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					fields = parser.ReadFields();

					using (Audit audit = new Audit(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						if (fields[0].Length > 0)
							audit.AddFilterCriteria(Audit.AuditFields.AuditRecd, ComparisonType.EqualTo, fields[0]);

						if (!fields[1].Contains("1900"))
							audit.AddFilterCriteria(Audit.AuditFields.AuditDate, ComparisonType.EqualTo, DateTime.Parse(fields[1]));

						if (fields[2].Contains(":"))
							audit.AddFilterCriteria(Audit.AuditFields.AuditTime, ComparisonType.EqualTo, fields[2]);

						if (fields[3].Length > 0)
							audit.AddFilterCriteria(Audit.AuditFields.AuditStation, ComparisonType.EqualTo, fields[3]);

						if (fields[4].Length > 0)
							audit.AddFilterCriteria(Audit.AuditFields.AuditUser, ComparisonType.EqualTo, fields[4]);

						if (fields[5].Length > 0)
							audit.AddFilterCriteria(Audit.AuditFields.AuditClient, ComparisonType.EqualTo, Convert.ToInt32(fields[5]));

						if (fields[6].Length > 0)
							audit.AddFilterCriteria(Audit.AuditFields.AuditAnimal, ComparisonType.EqualTo, Convert.ToInt32(fields[6]));

						if (fields[7].Length > 0)
							audit.AddFilterCriteria(Audit.AuditFields.AuditFunction, ComparisonType.EqualTo, fields[7]);

						if (fields[8].Length > 0)
							audit.AddFilterCriteria(Audit.AuditFields.AuditComments, ComparisonType.EqualTo, fields[8]);

						var auditList = audit.AuditList();

						if (auditList.Count == 0)
						{
							Assert.Fail("Match failed on line: " + (y + 1).ToString());
						}
					}
				}
			}

			Assert.IsTrue(true, "AuditTest2 Passed!!");
		}

		private bool AuditRecordCompare(List<Audit.AuditData> auditList, string[] fields, int lineNumber)
		{
			if (auditList[lineNumber].AuditRecd == fields[0] && (auditList[lineNumber].AuditDate - DateTime.Parse(fields[1])).TotalDays == 0 && auditList[lineNumber].AuditTime == fields[2] && auditList[lineNumber].AuditStation == fields[3] &&
					auditList[lineNumber].AuditUser == fields[4] && Shared.CompareAmount(auditList[lineNumber].AuditClient, fields[5]) && Shared.CompareAmount(auditList[lineNumber].AuditAnimal, fields[6]) &&
					auditList[lineNumber].AuditFunction == fields[7] && auditList[lineNumber].AuditComments == fields[8])
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
