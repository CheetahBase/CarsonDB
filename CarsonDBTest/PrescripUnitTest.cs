using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class PrescripUnitTest
	{
		//there is no legacy for Prescrip - it did not exist back then.
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void PrescripTestInit()
		{
			Shared.BuildPrescripRecords();
		}

		[TestCleanup]
		public void PrescripTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void PrescripTest1()
		{
			int lineNumber = 0;

			using (Prescrip prescrip = new Prescrip(_modernSourcePath))
			{
				var prescripList = prescrip.PrescripList();

				var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "prescrip.csv"));

				foreach (var line in lines)
				{
					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					string[] fields = parser.ReadFields();

					if (!PrescripRecordCompare(prescripList, fields, lineNumber))
					{
						Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
					}

					lineNumber++;
				}
			}

			Assert.IsFalse(false, "PrescripTest1 Passed!!");
		}

		[TestMethod]
		public void PrescripTest2()
		{
			for (int y = 0; y < 1000; y++)
			{
				string[] fields = null;

				var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "prescrip.csv"));
				int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "prescrip.csv")).Length;

				if (y > lineCount - 1)
					break;

				var line = new List<string>(lines)[y];

				TextFieldParser parser = new TextFieldParser(new StringReader(line));
				parser.HasFieldsEnclosedInQuotes = true;
				parser.SetDelimiters(",");
				fields = parser.ReadFields();

				using (Prescrip prescrip = new Prescrip(_modernSourcePath))
				{
					if (fields[0].Length > 0)
						prescrip.AddFilterCriteria(Prescrip.PrescripFields.PrescripRecd, ComparisonType.EqualTo, fields[0]);

					if (fields[1].Length > 0)
						prescrip.AddFilterCriteria(Prescrip.PrescripFields.PrescripPatient, ComparisonType.EqualTo, fields[1]);

					if (fields[2].Length > 0)
						prescrip.AddFilterCriteria(Prescrip.PrescripFields.PrescripItem, ComparisonType.EqualTo, fields[2]);

					if (fields[3].Length > 0)
						prescrip.AddFilterCriteria(Prescrip.PrescripFields.PrescripDateIssued, ComparisonType.EqualTo, DateTime.Parse(fields[3]));

					if (fields[4].Length > 0)
						prescrip.AddFilterCriteria(Prescrip.PrescripFields.PrescripDateExpires, ComparisonType.EqualTo, DateTime.Parse(fields[4]));

					if (fields[5].Length > 0)
						prescrip.AddFilterCriteria(Prescrip.PrescripFields.PrescripStatus, ComparisonType.EqualTo, fields[5]);

					if (fields[6].Length > 0)
						prescrip.AddFilterCriteria(Prescrip.PrescripFields.PrescripLabelPrintDate, ComparisonType.EqualTo, DateTime.Parse(fields[6]));

					if (fields[7].Length > 0)
						prescrip.AddFilterCriteria(Prescrip.PrescripFields.PrescripClosedDate, ComparisonType.EqualTo, DateTime.Parse(fields[7]));

					if (fields[8].Length > 0)
						prescrip.AddFilterCriteria(Prescrip.PrescripFields.PrescripLastRefillDate, ComparisonType.EqualTo, DateTime.Parse(fields[8]));

					if (fields[9].Length > 0)
						prescrip.AddFilterCriteria(Prescrip.PrescripFields.PrescripFillLocation, ComparisonType.EqualTo, fields[9]);

					var prescripList = prescrip.PrescripList();

					if (prescripList.Count == 0)
					{
						Assert.Fail("Match failed on line: " + (y + 1).ToString());
					}
				}
			}

			Assert.IsTrue(true, "PrescripTest2 Passed!!");
		}

		private bool PrescripRecordCompare(List<Prescrip.PrescripData> prescripList, string[] fields, int lineNumber)
		{
			if (prescripList[lineNumber].PrescripRecd == fields[0] && Shared.CompareAmount(prescripList[lineNumber].PrescripPatient, fields[1]) && 
				Shared.CompareAmount(prescripList[lineNumber].PrescripItem, fields[2]) && (prescripList[lineNumber].PrescripDateIssued - DateTime.Parse(fields[3])).TotalDays == 0 &&
				(prescripList[lineNumber].PrescripDateExpires - DateTime.Parse(fields[4])).TotalDays == 0 && Shared.CompareAmount(prescripList[lineNumber].PrescripStatus, fields[5]) &&
				(prescripList[lineNumber].PrescripLabelPrintDate - DateTime.Parse(fields[6])).TotalDays == 0 && (prescripList[lineNumber].PrescripClosedDate - DateTime.Parse(fields[7])).TotalDays == 0 &&
				(prescripList[lineNumber].PrescripLastRefillDate - DateTime.Parse(fields[8])).TotalDays == 0 && Shared.CompareAmount(prescripList[lineNumber].PrescripFillLocation, fields[9]))
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
