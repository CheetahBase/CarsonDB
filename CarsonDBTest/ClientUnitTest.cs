using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class ClientUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void ClientTestInit()
		{
			Shared.BuildClientRecords();
		}

		[TestCleanup]
		public void ClientTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void ClientTest1()
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
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}
			}

			Assert.IsFalse(false, "ClientTest1 Passed!!");
		}

		[TestMethod]
		public void ClientTest2()
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
							Assert.Fail("Match failed on line: " + (y + 1).ToString());
						}
					}
				}
			}

			Assert.IsTrue(true, "ClientTest2 Passed!!");
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
