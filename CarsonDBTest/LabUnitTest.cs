using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class LabUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void LabTestInit()
		{
			Shared.BuildLabRecords();
		}

		[TestCleanup]
		public void LabTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void LabTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Lab lab = new Lab(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					var labList = lab.LabList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "lab.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!LabRecordCompare(labList, fields, lineNumber))
						{
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}
			}

			Assert.IsFalse(false, "LabTest1 Passed!!");
		}

		[TestMethod]
		public void LabTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					string[] fields = null;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "lab.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "lab.csv")).Length;

					if (y > lineCount - 1)
						break;

					var line = new List<string>(lines)[y];

					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					fields = parser.ReadFields();

					using (Lab lab = new Lab(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						if (fields[0].Length > 0)
							lab.AddFilterCriteria(Lab.LabFields.LabRecd, ComparisonType.EqualTo, fields[0]);

						if (fields[1].Length > 0)
							lab.AddFilterCriteria(Lab.LabFields.LabTreatment, ComparisonType.EqualTo, Convert.ToInt32(fields[1]));

						if (fields[2].Length > 0)
							lab.AddFilterCriteria(Lab.LabFields.LabCompany, ComparisonType.EqualTo, Convert.ToInt32(fields[2]));

						if (fields[3].Length > 0)
							lab.AddFilterCriteria(Lab.LabFields.LabId, ComparisonType.EqualTo, fields[3]);

						if (fields[4].Length > 0)
							lab.AddFilterCriteria(Lab.LabFields.LabDescription, ComparisonType.EqualTo, fields[4]);

						var labList = lab.LabList();

						if (labList.Count == 0)
						{
							Assert.Fail("Match failed on line: " + (y + 1).ToString());
						}
					}
				}
			}

			Assert.IsTrue(true, "LabTest2 Passed!!");
		}

		private bool LabRecordCompare(List<Lab.LabData> labList, string[] fields, int lineNumber)
		{
			if (labList[lineNumber].LabRecd == fields[0] && Shared.CompareAmount(labList[lineNumber].LabTreatment, fields[1]) && Shared.CompareAmount(labList[lineNumber].LabCompany, fields[2]) && labList[lineNumber].LabId == fields[3] &&
					labList[lineNumber].LabDescription == fields[4])
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
