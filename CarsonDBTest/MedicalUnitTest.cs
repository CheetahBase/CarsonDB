using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class MedicalUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void MedicalTestInit()
		{
			Shared.BuildMedicalRecords();
		}

		[TestCleanup]
		public void MedicalTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void MedicalTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Medical medical = new Medical(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					var medicalList = medical.MedicalList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "medical.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!MedicalRecordCompare(medicalList, fields, lineNumber))
						{
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}
			}

			Assert.IsFalse(false, "MedicalTest1 Passed!!");
		}

		[TestMethod]
		public void MedicalTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					string[] fields = null;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "medical.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "medical.csv")).Length;

					if (y > lineCount - 1)
						break;

					var line = new List<string>(lines)[y];

					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					fields = parser.ReadFields();

					using (Medical medical = new Medical(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						if (fields[0].Length > 0)
							medical.AddFilterCriteria(Medical.MedicalFields.MedicalRecd, ComparisonType.EqualTo, fields[0]);

						if (fields[1].Length > 0)
							medical.AddFilterCriteria(Medical.MedicalFields.MedicalWeight, ComparisonType.EqualTo, fields[1]);

						if (fields[2].Length > 0)
							medical.AddFilterCriteria(Medical.MedicalFields.MedicalTemp, ComparisonType.EqualTo, fields[2]);

						if (fields[3].Length > 0)
							medical.AddFilterCriteria(Medical.MedicalFields.MedicalResp, ComparisonType.EqualTo, fields[3]);

						if (fields[4].Length > 0)
							medical.AddFilterCriteria(Medical.MedicalFields.MedicalPulse, ComparisonType.EqualTo, fields[4]);

						if (fields[5].Length > 0)
							medical.AddFilterCriteria(Medical.MedicalFields.MedicalCrt, ComparisonType.EqualTo, fields[5]);

						if (fields[6].Length > 0)
							medical.AddFilterCriteria(Medical.MedicalFields.MedicalOther, ComparisonType.EqualTo, fields[6]);

						if (fields[7].Length > 0)
							medical.AddFilterCriteria(Medical.MedicalFields.MedicalComplaint, ComparisonType.EqualTo, fields[7]);

						if (fields[8].Length > 0)
							medical.AddFilterCriteria(Medical.MedicalFields.MedicalExamNotes, ComparisonType.EqualTo, fields[8]);

						if (fields[9].Length > 0)
							medical.AddFilterCriteria(Medical.MedicalFields.MedicalClientInstructions, ComparisonType.EqualTo, fields[9]);

						if (fields[10].Length > 0)
							medical.AddFilterCriteria(Medical.MedicalFields.MedicalService, ComparisonType.EqualTo, fields[10]);

						if (fields[11].Length > 0)
							medical.AddFilterCriteria(Medical.MedicalFields.MedicalAssessNotes, ComparisonType.EqualTo, fields[11]);

						if (fields[12].Length > 0)
							medical.AddFilterCriteria(Medical.MedicalFields.MedicalPlanNotes, ComparisonType.EqualTo, fields[12]);

						var medicalList = medical.MedicalList();

						if (medicalList.Count == 0)
						{
							Assert.Fail("Match failed on line: " + (y + 1).ToString());
						}
					}
				}
			}

			Assert.IsTrue(true, "MedicalTest2 Passed!!");
		}

		//MID$(outrec, 1, 1) = "A"       ' MEDICAL_RECD
		//MID$(outrec, 2, 4) = RandomSingle2()       ' MEDICAL_WEIGHT
		//MID$(outrec, 6, 4) = RandomSingle2()       ' MEDICAL_TEMP
		//MID$(outrec, 10, 4) = RandomSingle2()       ' MEDICAL_RESP
		//MID$(outrec, 14, 4) = RandomSingle2()       ' MEDICAL_PULSE
		//MID$(outrec, 18, 16) = StringText(RandomDescription()) 'MEDICAL_CRT
		//MID$(outrec, 35, 30) = StringText(RandomDescription()) 'MEDICAL_OTHER
		//MID$(outrec, 66, 4) = RandomNote(4, 5, 0) 'MEDICAL_COMPLAINT
		//MID$(outrec, 70, 4) = RandomNote(4, 5, 0) 'MEDICAL_EXAMNOTES
		//MID$(outrec, 74, 4) = RandomNote(4, 5, 0) 'MEDICAL_CLIENTINSTRUCTIONS
		//MID$(outrec, 78, 4) = DoubleWord(INT(RND * 3000) + 1) 'MEDICAL_SERVICE
		//MID$(outrec, 82, 3) = RandomNote(4, 5, 0) 'MEDICAL_ASSESSNOTES
		//MID$(outrec, 86, 4) = RandomNote(4, 5, 0) 'MEICAL_PLANNOTES    

		private bool MedicalRecordCompare(List<Medical.MedicalData> medicalList, string[] fields, int lineNumber)
		{
			if (medicalList[lineNumber].MedicalRecd == fields[0] && Shared.CompareAmount(medicalList[lineNumber].MedicalWeight, fields[1]) && Shared.CompareAmount(medicalList[lineNumber].MedicalTemp, fields[2]) &&
				Shared.CompareAmount(medicalList[lineNumber].MedicalResp, fields[3]) && Shared.CompareAmount(medicalList[lineNumber].MedicalPulse, fields[4]) &&
				medicalList[lineNumber].MedicalCrt == fields[5] && medicalList[lineNumber].MedicalOther == fields[6] && medicalList[lineNumber].MedicalComplaint == fields[7] &&
				medicalList[lineNumber].MedicalExamNotes == fields[8] && medicalList[lineNumber].MedicalClientInstructions == fields[9] && Shared.CompareAmount(medicalList[lineNumber].MedicalService, fields[10]) &&
				medicalList[lineNumber].MedicalAssessNotes == fields[11] && medicalList[lineNumber].MedicalPlanNotes == fields[12])
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
