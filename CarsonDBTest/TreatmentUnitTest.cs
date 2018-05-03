using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class TreatmentUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void TreatmentTestInit()
		{
			Shared.BuildTreatmentRecords();
		}

		[TestCleanup]
		public void TreatmentTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void TreatmentTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Treatment treatment = new Treatment(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					var treatmentList = treatment.TreatmentList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "treat.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!TreatmentRecordCompare(treatmentList, fields, lineNumber))
						{
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}
			}

			Assert.IsFalse(false, "TreatmentTest1 Passed!!");
		}

		[TestMethod]
		public void TreatmentTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					string[] fields = null;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "treat.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "treat.csv")).Length;

					if (y > lineCount - 1)
						break;

					var line = new List<string>(lines)[y];

					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					fields = parser.ReadFields();

					using (Treatment treatment = new Treatment(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						if (fields[0].Length > 0)
							treatment.AddFilterCriteria(Treatment.TreatmentFields.TreatmentCode, ComparisonType.EqualTo, fields[0]);

						if (fields[1].Length > 0)
							treatment.AddFilterCriteria(Treatment.TreatmentFields.TreatmentRecd, ComparisonType.EqualTo, fields[1]);

						if (fields[2].Length > 0)
							treatment.AddFilterCriteria(Treatment.TreatmentFields.TreatmentClass, ComparisonType.EqualTo, Convert.ToInt32(fields[2]));

						if (fields[3].Length > 0)
							treatment.AddFilterCriteria(Treatment.TreatmentFields.TreatmentCodes, ComparisonType.EqualTo, fields[3]);

						if (fields[4].Length > 0)
							treatment.AddFilterCriteria(Treatment.TreatmentFields.TreatmentDescription, ComparisonType.EqualTo, fields[4]);

						if (!fields[5].Contains("1900"))
							treatment.AddFilterCriteria(Treatment.TreatmentFields.TreatmentChanged, ComparisonType.EqualTo, DateTime.Parse(fields[5]));

						if (fields[6].Length > 0)
							treatment.AddFilterCriteria(Treatment.TreatmentFields.TreatmentPrice, ComparisonType.EqualTo, Convert.ToDecimal(fields[6]));

						var treatmentList = treatment.TreatmentList();

						if (treatmentList.Count == 0)
						{
							Assert.Fail("Match failed on line: " + (y + 1).ToString());
						}
					}
				}
			}

			Assert.IsTrue(true, "TreatmentTest2 Passed!!");
		}

		private bool TreatmentRecordCompare(List<Treatment.TreatmentData> treatmentList, string[] fields, int lineNumber)
		{
			if (treatmentList[lineNumber].TreatmentCode == fields[0] && treatmentList[lineNumber].TreatmentRecd == fields[1] && Shared.CompareAmount(treatmentList[lineNumber].TreatmentClass, fields[2]) && treatmentList[lineNumber].TreatmentCodes == fields[3] &&
					treatmentList[lineNumber].TreatmentDescription == fields[4] && (treatmentList[lineNumber].TreatmentChanged - DateTime.Parse(fields[5])).TotalDays == 0 && Shared.CompareAmount(treatmentList[lineNumber].TreatmentPrice, fields[6])
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
