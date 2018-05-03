using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class ServiceUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void ServiceTestInit()
		{
			Shared.BuildServiceRecords();
		}

		[TestCleanup]
		public void ServiceTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void ServiceTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Service service = new Service(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					var serviceList = service.ServiceList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "service.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!ServiceRecordCompare(serviceList, fields, lineNumber))
						{
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}
			}

			Assert.IsFalse(false, "ServiceTest1 Passed!!");
		}

		[TestMethod]
		public void ServiceTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					string[] fields = null;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "service.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "service.csv")).Length;

					if (y > lineCount - 1)
						break;

					var line = new List<string>(lines)[y];

					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					fields = parser.ReadFields();

					using (Service service = new Service(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						if (!fields[0].Contains("1900"))
							service.AddFilterCriteria(Service.ServiceFields.ServiceDate, ComparisonType.EqualTo, DateTime.Parse(fields[0]));

						if (fields[1].Length > 0)
							service.AddFilterCriteria(Service.ServiceFields.ServiceRecd, ComparisonType.EqualTo, fields[1]);

						if (fields[2].Length > 0)
							service.AddFilterCriteria(Service.ServiceFields.ServiceType, ComparisonType.EqualTo, fields[2]);

						if (fields[3].Length > 0)
							service.AddFilterCriteria(Service.ServiceFields.ServiceCode, ComparisonType.EqualTo, fields[3]);

						if (fields[4].Length > 0)
							service.AddFilterCriteria(Service.ServiceFields.ServiceDescription, ComparisonType.EqualTo, fields[4]);

						if (fields[5].Length > 0)
							service.AddFilterCriteria(Service.ServiceFields.ServiceAmount, ComparisonType.EqualTo, Convert.ToDecimal(fields[5]));

						if (fields[6].Length > 0)
							service.AddFilterCriteria(Service.ServiceFields.ServiceQuantity, ComparisonType.EqualTo, Convert.ToDecimal(fields[6]));

						if (fields[7].Length > 0)
							service.AddFilterCriteria(Service.ServiceFields.ServiceNormal, ComparisonType.EqualTo, Convert.ToDecimal(fields[7]));

						if (fields[8].Length > 0)
							service.AddFilterCriteria(Service.ServiceFields.ServiceAccount, ComparisonType.EqualTo, Convert.ToInt32(fields[8]));

						if (fields[9].Length > 0)
							service.AddFilterCriteria(Service.ServiceFields.ServiceNote, ComparisonType.EqualTo, fields[9]);

						if (fields[10].Length > 0)
							service.AddFilterCriteria(Service.ServiceFields.ServiceAnimal, ComparisonType.EqualTo, Convert.ToInt32(fields[10]));

						if (fields[11].Length > 0)
							service.AddFilterCriteria(Service.ServiceFields.ServiceFlags, ComparisonType.EqualTo, Convert.ToInt32(fields[11]));

						if (fields[12].Length > 0)
							service.AddFilterCriteria(Service.ServiceFields.ServiceResults, ComparisonType.EqualTo, Convert.ToInt32(fields[12]));

						var serviceList = service.ServiceList();

						if (serviceList.Count == 0)
						{
							Assert.Fail("Match failed on line: " + (y + 1).ToString());
						}
					}
				}
			}

			Assert.IsTrue(true, "ServiceTest2 Passed!!");
		}

		private bool ServiceRecordCompare(List<Service.ServiceData> serviceList, string[] fields, int lineNumber)
		{
			if ((serviceList[lineNumber].ServiceDate - DateTime.Parse(fields[0])).TotalDays == 0 && serviceList[lineNumber].ServiceRecd == fields[1] && serviceList[lineNumber].ServiceType == fields[2] && serviceList[lineNumber].ServiceCode == fields[3] &&
					serviceList[lineNumber].ServiceDescription == fields[4] && Shared.CompareAmount(serviceList[lineNumber].ServiceAmount, fields[5]) && Shared.CompareAmount(serviceList[lineNumber].ServiceQuantity, fields[6]) &&
					Shared.CompareAmount(serviceList[lineNumber].ServiceNormal, fields[7]) && Shared.CompareAmount(serviceList[lineNumber].ServiceAccount, fields[8]) && serviceList[lineNumber].ServiceNote == fields[9] &&
					Shared.CompareAmount(serviceList[lineNumber].ServiceAnimal, fields[10]) && Shared.CompareAmount(serviceList[lineNumber].ServiceFlags, fields[11]) && Shared.CompareAmount(serviceList[lineNumber].ServiceResults, fields[12]) &&
					Shared.CompareAmount(serviceList[lineNumber].ServiceSfPublicNotes, fields[13]) && Shared.CompareAmount(serviceList[lineNumber].ServiceSfPublic, fields[14])
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
