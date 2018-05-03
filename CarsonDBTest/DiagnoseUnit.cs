using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class DiagnoseUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void DiagnoseTestInit()
		{
			Shared.BuildDiagnoseRecords();
		}

		[TestCleanup]
		public void DiagnoseTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void DiagnoseTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Diagnose diagnose = new Diagnose(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					var diagnoseList = diagnose.DiagnoseList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "diagnose.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!DiagnoseRecordCompare(diagnoseList, fields, lineNumber))
						{
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}
			}

			Assert.IsFalse(false, "DiagnoseTest1 Passed!!");
		}

		[TestMethod]
		public void DiagnoseTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					string[] fields = null;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "diagnose.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "diagnose.csv")).Length;

					if (y > lineCount - 1)
						break;

					var line = new List<string>(lines)[y];

					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					fields = parser.ReadFields();

					using (Diagnose diagnose = new Diagnose(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						if (fields[0].Length > 0)
							diagnose.AddFilterCriteria(Diagnose.DiagnoseFields.DiagnoseRecd, ComparisonType.EqualTo, fields[0]);

						if (fields[1].Length > 0)
							diagnose.AddFilterCriteria(Diagnose.DiagnoseFields.DiagnoseCode, ComparisonType.EqualTo, fields[1]);

						if (fields[2].Length > 0)
							diagnose.AddFilterCriteria(Diagnose.DiagnoseFields.DiagnoseDescription, ComparisonType.EqualTo, fields[2]);

						var diagnoseList = diagnose.DiagnoseList();

						if (diagnoseList.Count == 0)
						{
							Assert.Fail("Match failed on line: " + (y + 1).ToString());
						}
					}
				}
			}

			Assert.IsTrue(true, "DiagnoseTest2 Passed!!");
		}

		private bool DiagnoseRecordCompare(List<Diagnose.DiagnoseData> diagnoseList, string[] fields, int lineNumber)
		{
			if (diagnoseList[lineNumber].DiagnoseRecd == fields[0] && diagnoseList[lineNumber].DiagnoseCode == fields[1] && diagnoseList[lineNumber].DiagnoseDescription == fields[2])
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
