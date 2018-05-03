using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class MultithreadUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void MultithreadTestInit()
		{
			Shared.BuildAnimalRecords();
			Shared.BuildClientRecords();
		}

		[TestCleanup]
		public void MultithreadTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void MultithreadTest1()
		{
			bool returnValue1 = false;
			bool returnValue2 = false;

			var thread1 = new Thread(
				() =>
				{
					returnValue1 = AnimalTest1();
				});

			var thread2 = new Thread(
				() =>
				{
					returnValue2 = ClientTest1();
				});

			thread1.Start();
			thread2.Start();
			thread1.Join();
			thread2.Join();

			Assert.IsTrue(returnValue1 && returnValue2);
		}

		[TestMethod]
		public void MultithreadTest2()
		{
			bool returnValue1 = false;
			bool returnValue2 = false;

			var thread1 = new Thread(
				() =>
				{
					returnValue1 = AnimalTest2();
				});

			var thread2 = new Thread(
				() =>
				{
					returnValue2 = ClientTest2();
				});

			thread1.Start();
			thread2.Start();
			thread1.Join();
			thread2.Join();

			Assert.IsTrue(returnValue1 && returnValue2);
		}

		private bool AnimalTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Animal animal = new Animal(x == 0 ? _legacySourcePath : _modernSourcePath))
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
							return false;
						}

						lineNumber++;
					}
				}
			}

			return true;
		}

		private bool ClientTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Client client = new Client(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					var clientList = client.ClientList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "client.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!ClientRecordCompare(clientList, fields, lineNumber))
						{
							return false;
						}

						lineNumber++;
					}
				}
			}

			return true;
		}

		private bool AnimalTest2()
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
							return false;
						}
					}
				}
			}

			return true;
		}

		private bool ClientTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					string[] fields = null;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "client.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "client.csv")).Length;

					if (y > lineCount - 1)
						break;

					var line = new List<string>(lines)[y];

					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					fields = parser.ReadFields();

					using (Client client = new Client(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						if (fields[0].Length > 0)
							client.AddFilterCriteria(Client.ClientFields.ClientRecd, ComparisonType.EqualTo, fields[0]);

						if (fields[1].Length > 0)
							client.AddFilterCriteria(Client.ClientFields.ClientTitle, ComparisonType.EqualTo, fields[1]);

						if (fields[2].Length > 0)
							client.AddFilterCriteria(Client.ClientFields.ClientLast, ComparisonType.EqualTo, fields[2]);

						if (fields[3].Length > 0)
							client.AddFilterCriteria(Client.ClientFields.ClientAddress, ComparisonType.EqualTo, fields[3]);

						if (fields[4].Length > 0)
							client.AddFilterCriteria(Client.ClientFields.ClientAddress2, ComparisonType.EqualTo, fields[4]);

						if (fields[5].Length > 0)
							client.AddFilterCriteria(Client.ClientFields.ClientCity, ComparisonType.EqualTo, fields[5]);

						if (fields[6].Length > 0)
							client.AddFilterCriteria(Client.ClientFields.ClientState, ComparisonType.EqualTo, fields[6]);

						if (fields[7].Length > 0)
							client.AddFilterCriteria(Client.ClientFields.ClientZip, ComparisonType.EqualTo, fields[7]);

						if (fields[8].Length > 0)
							client.AddFilterCriteria(Client.ClientFields.ClientArea, ComparisonType.EqualTo, fields[8]);

						if (fields[9].Length > 0)
							client.AddFilterCriteria(Client.ClientFields.ClientPhone, ComparisonType.EqualTo, fields[9]);

						if (fields[10].Length > 0)
							client.AddFilterCriteria(Client.ClientFields.ClientFirst, ComparisonType.EqualTo, fields[10]);

						if (fields[11].Length > 0)
							client.AddFilterCriteria(Client.ClientFields.ClientBusiness, ComparisonType.EqualTo, fields[11]);

						if (fields[12].Length > 0)
							client.AddFilterCriteria(Client.ClientFields.ClientCodes, ComparisonType.EqualTo, fields[12]);

						if (fields[13].Length > 0)
							client.AddFilterCriteria(Client.ClientFields.ClientClass, ComparisonType.EqualTo, Convert.ToInt32(fields[13]));

						if (fields[14].Length > 0)
							client.AddFilterCriteria(Client.ClientFields.ClientReference, ComparisonType.EqualTo, fields[14]);

						if (!fields[15].Contains("1900"))
							client.AddFilterCriteria(Client.ClientFields.ClientUntil, ComparisonType.EqualTo, DateTime.Parse(fields[15]));

						if (fields[17].Length > 0)
							client.AddFilterCriteria(Client.ClientFields.ClientFlags, ComparisonType.EqualTo, Convert.ToInt32(fields[17]));

						if (!fields[18].Contains("1900"))
							client.AddFilterCriteria(Client.ClientFields.ClientAdded, ComparisonType.EqualTo, DateTime.Parse(fields[18]));

						if (fields[19].Length > 0)
							client.AddFilterCriteria(Client.ClientFields.ClientDoctor, ComparisonType.EqualTo, fields[19]);

						if (fields[20].Length > 0)
							client.AddFilterCriteria(Client.ClientFields.ClientCompany, ComparisonType.EqualTo, Convert.ToInt32(fields[20]));

						if (fields[21].Length > 0)
							client.AddFilterCriteria(Client.ClientFields.ClientSpouse, ComparisonType.EqualTo, fields[21]);

						if (fields[22].Length > 0)
							client.AddFilterCriteria(Client.ClientFields.ClientEmail, ComparisonType.EqualTo, fields[22]);

						if (fields[23].Length > 0)
							client.AddFilterCriteria(Client.ClientFields.ClientCounty, ComparisonType.EqualTo, fields[23]);

						if (fields[24].Length > 0)
							client.AddFilterCriteria(Client.ClientFields.ClientFolder, ComparisonType.EqualTo, Convert.ToInt32(fields[24]));

						if (fields[25].Length > 0)
							client.AddFilterCriteria(Client.ClientFields.ClientCell, ComparisonType.EqualTo, fields[25]);

						var clientList = client.ClientList();

						if (clientList.Count == 0)
						{
							return false;
						}
					}
				}
			}

			return true;
		}

		private bool AnimalRecordCompare(List<Animal.AnimalData> animalList, string[] fields, int lineNumber)
		{
			if (animalList[lineNumber].AnimalName == fields[0] && animalList[lineNumber].AnimalRecd == fields[1] && animalList[lineNumber].AnimalCodes == fields[2] &&
				(animalList[lineNumber].AnimalAdded - DateTime.Parse(fields[3])).TotalDays == 0 && animalList[lineNumber].AnimalRabies == fields[4] &&
				(animalList[lineNumber].AnimalBirthday - DateTime.Parse(fields[5])).TotalDays == 0 && Shared.CompareAmount(animalList[lineNumber].AnimalVisit, fields[6]) &&
				Shared.CompareAmount(animalList[lineNumber].AnimalFlags, fields[7]) && animalList[lineNumber].AnimalSex == fields[8] && animalList[lineNumber].AnimalAllergy == fields[9] &&
				animalList[lineNumber].AnimalBreed == fields[10] && animalList[lineNumber].AnimalSpecies == fields[11] && Shared.CompareAmount(animalList[lineNumber].AnimalWeight, fields[12]) &&
				animalList[lineNumber].AnimalRegistration == fields[13] && animalList[lineNumber].AnimalMeasure == fields[14] && animalList[lineNumber].AnimalColor == fields[15] &&
				Shared.CompareAmount(animalList[lineNumber].AnimalClient, fields[16]) && animalList[lineNumber].AnimalPhoto == fields[17] &&
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

		private bool ClientRecordCompare(List<Client.ClientData> clientList, string[] fields, int lineNumber)
		{
			if (clientList[lineNumber].ClientRecd == fields[0] && clientList[lineNumber].ClientTitle == fields[1] && clientList[lineNumber].ClientLast == fields[2] && clientList[lineNumber].ClientAddress == fields[3] &&
					clientList[lineNumber].ClientAddress2 == fields[4] && clientList[lineNumber].ClientCity == fields[5] && clientList[lineNumber].ClientState == fields[6] &&
					clientList[lineNumber].ClientZip == fields[7] && clientList[lineNumber].ClientArea == fields[8] && clientList[lineNumber].ClientPhone == fields[9] &&
					clientList[lineNumber].ClientFirst == fields[10] && clientList[lineNumber].ClientBusiness == fields[11] && clientList[lineNumber].ClientCodes == fields[12] &&
					Shared.CompareAmount(clientList[lineNumber].ClientClass, fields[13]) && clientList[lineNumber].ClientReference == fields[14] && (clientList[lineNumber].ClientUntil - DateTime.Parse(fields[15])).TotalDays == 0 &&
					Shared.CompareBool(clientList[lineNumber].ClientSuspend, fields[16]) && Shared.CompareAmount(clientList[lineNumber].ClientFlags, fields[17]) && (clientList[lineNumber].ClientAdded - DateTime.Parse(fields[18])).TotalDays == 0 &&
					clientList[lineNumber].ClientDoctor == fields[19] && Shared.CompareAmount(clientList[lineNumber].ClientCompany, fields[20]) && clientList[lineNumber].ClientSpouse == fields[21] &&
					clientList[lineNumber].ClientEmail == fields[22] && clientList[lineNumber].ClientCounty == fields[23] && Shared.CompareAmount(clientList[lineNumber].ClientFolder, fields[24]) &&
					clientList[lineNumber].ClientCell == fields[25])
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
