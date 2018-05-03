using System;
using System.Collections.Generic;
using System.Data.CarsonDatabase;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CarsonDBTest
{
	[TestClass]
	public class AnimalCrcUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void AnimalCrcTestInit()
		{
			Shared.CreateDatabaseFiles();
		}

		[TestMethod]
		public void AnimalCrcTest()
		{
			using (Animal animal1 = new Animal(_legacySourcePath))
			using (Animal animal2 = new Animal(_modernSourcePath))
			{
				animal1.CreateTableCRC("animal1.crc", Shared.GetFields(typeof(Animal.AnimalFields)));
				animal2.CreateTableCRC("animal2.crc", Shared.GetFields(typeof(Animal.AnimalFields)));
				Shared.UpdateAnimalFiles();
				List<Animal.AnimalData> animalRecordsChanged1 = animal1.FindChangedRecords("animal1.crc", Animal.DifferentialData.NewAndModifiedRecords).ConvertAll(x => (Animal.AnimalData)x);
				List<Animal.AnimalData> animalRecordsChanged2 = animal2.FindChangedRecords("animal2.crc", Animal.DifferentialData.NewAndModifiedRecords).ConvertAll(x => (Animal.AnimalData)x);

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

					if (!animalRecordsChanged1.Any(i => i.AnimalId == recordId))
					{
						Assert.Fail("Record: " + recordId.ToString() + " Not found when expected.");
					}
				}
			}

			Assert.IsTrue(true, "Successfull CRC Check on Animal Table");
		}
	}
}
