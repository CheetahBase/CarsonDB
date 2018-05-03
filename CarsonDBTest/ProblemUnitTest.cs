using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class ProblemUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void ProblemTestInit()
		{
			Shared.BuildProblemRecords();
		}

		[TestCleanup]
		public void ProblemTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void ProblemTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Problem problem = new Problem(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					var problemList = problem.ProblemList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "problem.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!ProblemRecordCompare(problemList, fields, lineNumber))
						{
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}
			}

			Assert.IsFalse(false, "ProblemTest1 Passed!!");
		}

		[TestMethod]
		public void ProblemTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					string[] fields = null;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "problem.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "problem.csv")).Length;

					if (y > lineCount - 1)
						break;

					var line = new List<string>(lines)[y];

					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					fields = parser.ReadFields();

					using (Problem problem = new Problem(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						if (fields[0].Length > 0)
							problem.AddFilterCriteria(Problem.ProblemFields.ProblemCode, ComparisonType.EqualTo, fields[0]);

						if (fields[1].Length > 0)
							problem.AddFilterCriteria(Problem.ProblemFields.ProblemRecd, ComparisonType.EqualTo, fields[1]);

						if (fields[2].Length > 0)
							problem.AddFilterCriteria(Problem.ProblemFields.ProblemDescription, ComparisonType.EqualTo, fields[2]);

						var problemList = problem.ProblemList();

						if (problemList.Count == 0)
						{
							Assert.Fail("Match failed on line: " + (y + 1).ToString());
						}
					}
				}
			}

			Assert.IsTrue(true, "ProblemTest2 Passed!!");
		}

		private bool ProblemRecordCompare(List<Problem.ProblemData> problemList, string[] fields, int lineNumber)
		{
			if (problemList[lineNumber].ProblemCode == fields[0] && problemList[lineNumber].ProblemRecd == fields[1] && problemList[lineNumber].ProblemDescription == fields[2])
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
