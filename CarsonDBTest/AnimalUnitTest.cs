using System;
using System.Collections.Generic;
using CarsonDB;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;

namespace CarsonDBTest
{
	[TestClass]
	public class AnimalUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void AnimalTestInit()
		{
			Shared.BuildAnimalRecords();
		}

		[TestCleanup]
		public void AnimalTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void AnimalTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Animal animal = new Animal(x == 0 ?_legacySourcePath : _modernSourcePath))
				{
					var animalList = animal.AnimalList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "animal.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!AnimalRecordCompare(animalList, fields, lineNumber))
						{
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}
			}

			Assert.IsFalse(false, "AnimalTest1 Passed!!");
		}

		[TestMethod]
		public void AnimalTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					string[] fields = null;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "animal.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "animal.csv")).Length;

					if (y > lineCount - 1)
						break;

					var line = new List<string>(lines)[y];

					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					fields = parser.ReadFields();

					using (Animal animal = new Animal(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						animal.AddFilterCriteria(Animal.AnimalFields.AnimalName, ComparisonType.EqualTo, fields[0]);
						animal.AddFilterCriteria(Animal.AnimalFields.AnimalRecd, ComparisonType.EqualTo, fields[1]);

						if (fields[2].Length > 0)
							animal.AddFilterCriteria(Animal.AnimalFields.AnimalCodes, ComparisonType.EqualTo, fields[2]);

						animal.AddFilterCriteria(Animal.AnimalFields.AnimalAdded, ComparisonType.EqualTo, DateTime.Parse(fields[3]));

						if (fields[4].Length > 0)
							animal.AddFilterCriteria(Animal.AnimalFields.AnimalRabies, ComparisonType.EqualTo, fields[4]);

						animal.AddFilterCriteria(Animal.AnimalFields.AnimalBirthday, ComparisonType.EqualTo, DateTime.Parse(fields[5]));
						animal.AddFilterCriteria(Animal.AnimalFields.AnimalVisit, ComparisonType.EqualTo, int.Parse(fields[6]));
						animal.AddFilterCriteria(Animal.AnimalFields.AnimalFlags, ComparisonType.EqualTo, int.Parse(fields[7]));
						animal.AddFilterCriteria(Animal.AnimalFields.AnimalSex, ComparisonType.EqualTo, fields[8]);

						if (fields[9].Length > 0)
							animal.AddFilterCriteria(Animal.AnimalFields.AnimalAllergy, ComparisonType.EqualTo, fields[9]);

						if (fields[10].Length > 0)
							animal.AddFilterCriteria(Animal.AnimalFields.AnimalBreed, ComparisonType.EqualTo, fields[10]);

						if (fields[11].Length > 0)
							animal.AddFilterCriteria(Animal.AnimalFields.AnimalSpecies, ComparisonType.EqualTo, fields[11]);

						animal.AddFilterCriteria(Animal.AnimalFields.AnimalWeight, ComparisonType.EqualTo, fields[12]);

						if (fields[13].Length > 0)
							animal.AddFilterCriteria(Animal.AnimalFields.AnimalRegistration, ComparisonType.EqualTo, fields[13]);

						if (fields[14].Length > 0)
							animal.AddFilterCriteria(Animal.AnimalFields.AnimalMeasure, ComparisonType.EqualTo, fields[14]);

						if (fields[15].Length > 0)
							animal.AddFilterCriteria(Animal.AnimalFields.AnimalColor, ComparisonType.EqualTo, fields[15]);

						animal.AddFilterCriteria(Animal.AnimalFields.AnimalClient, ComparisonType.EqualTo, fields[16]);

						if (fields[17].Length > 0)
							animal.AddFilterCriteria(Animal.AnimalFields.AnimalPhoto, ComparisonType.EqualTo, fields[17]);

						if (!fields[18].Contains("1900"))
							animal.AddFilterCriteria(Animal.AnimalFields.AnimalSuspend, ComparisonType.EqualTo, DateTime.Parse(fields[18]));

						if (!fields[19].Contains("1900"))
							animal.AddFilterCriteria(Animal.AnimalFields.AnimalDeath, ComparisonType.EqualTo, DateTime.Parse(fields[19]));

						if (fields[20].Length > 0)
							animal.AddFilterCriteria(Animal.AnimalFields.AnimalLocator, ComparisonType.EqualTo, fields[20]);

						animal.AddFilterCriteria(Animal.AnimalFields.AnimalLastCompany, ComparisonType.EqualTo, int.Parse(fields[21]));

						var animalList = animal.AnimalList();

						if (animalList.Count == 0)
						{
							Assert.Fail("AnimalTest2 Failed!!");
						}
					}
				}
			}

			Assert.IsTrue(true, "AnimalTest2 Passed!!");
		}

		[TestMethod]
		public void AnimalTest3CRC()
		{
			using (Animal animal1 = new Animal(_legacySourcePath))
			using (Animal animal2 = new Animal(_modernSourcePath))
			{
				animal1.CreateTableCRC("animal1.crc", Shared.GetFields(typeof(Animal.AnimalFields)));
				animal2.CreateTableCRC("animal2.crc", Shared.GetFields(typeof(Animal.AnimalFields)));
				Shared.UpdateAnimalFiles();

				var animalRecordsChanged1 = animal1.FindChangedRecords<Animal.AnimalData>("animal1.crc", Animal.DifferentialData.NewAndModifiedRecords);
				var animalRecordsChanged2 = animal2.FindChangedRecords<Animal.AnimalData>("animal2.crc", Animal.DifferentialData.NewAndModifiedRecords);

				string updatedRecords = File.ReadAllText("updated.csv");

				updatedRecords = updatedRecords.Replace("\r\n", "");
				string[] records = updatedRecords.Split(',');

				if (records.Length != animalRecordsChanged1.Count || records.Length != animalRecordsChanged2.Count)
				{
					Assert.Fail("Record Lengths do not match.  Found: " + animalRecordsChanged1.Count.ToString() + ", " + animalRecordsChanged2.Count.ToString() + ". Expected: " + records.Length.ToString());
				}

				for (int x = 0; x < records.Length; x++)
				{
					int recordId = Convert.ToInt32(records[x]);

					if (!animalRecordsChanged1.Any(i => i.Id == recordId))
					{
						Assert.Fail("Record: " + recordId.ToString() + " Not found when expected.");
					}
				}
			}

			Assert.IsTrue(true, "Successful CRC Check on Animal Table");
		}

		private bool AnimalRecordCompare(List<Animal.AnimalData> animalList, string[] fields, int lineNumber)
		{
			if (animalList[lineNumber].AnimalName == fields[0] && animalList[lineNumber].AnimalRecd == fields[1] && animalList[lineNumber].AnimalCodes == fields[2] &&
				(animalList[lineNumber].AnimalAdded - DateTime.Parse(fields[3])).TotalDays == 0 && animalList[lineNumber].AnimalRabies == fields[4] &&
				(animalList[lineNumber].AnimalBirthday - DateTime.Parse(fields[5])).TotalDays == 0 && Shared.CompareAmount(animalList[lineNumber].AnimalVisit, fields[6]) &&
				Shared.CompareAmount(animalList[lineNumber].AnimalFlags, fields[7]) && animalList[lineNumber].AnimalSex == fields[8] && animalList[lineNumber].AnimalAllergy == fields[9] &&
				animalList[lineNumber].AnimalBreed == fields[10] && animalList[lineNumber].AnimalSpecies == fields[11] && Shared.CompareAmount(animalList[lineNumber].AnimalWeight, fields[12]) &&
				animalList[lineNumber].AnimalRegistration == fields[13] && animalList[lineNumber].AnimalMeasure == fields[14] && animalList[lineNumber].AnimalColor == fields[15] &&
				Shared.CompareAmount(animalList[lineNumber].AnimalClient, fields[16]) && animalList[lineNumber].AnimalPhoto == fields[17]  &&
				(animalList[lineNumber].AnimalSuspend - DateTime.Parse(fields[18])).TotalDays == 0 && (animalList[lineNumber].AnimalDeath - DateTime.Parse(fields[19])).TotalDays == 0 && 
				animalList[lineNumber].AnimalLocator == fields[20] && Shared.CompareAmount(animalList[lineNumber].AnimalLastCompany, fields[21]))
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
