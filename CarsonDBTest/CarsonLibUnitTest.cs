using System;
using System.Collections.Generic;
using System.Data.CarsonDatabase;
using System.IO;
using System.Text;
using Microsoft.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using System.Linq;
using System.Reflection;

namespace CarsonDBTest
{
	[TestClass]
	public class CarsonLibUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void CarsonLibTestInit()
		{
			//Shared.BuildAccountRecords();
			Shared.BuildAnimalRecords();
			//Shared.BuildAppointRecords();
			//Shared.BuildAuditRecords();
			//Shared.BuildClientRecords();
			//Shared.BuildPoRecords();
			//Shared.BuildQuotailRecords();
			//Shared.BuildServiceRecords();
		}

		[TestCleanup]
		public void CarsonLibTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void CarsonLibTest()
		{
			//string[] testFields = { Account.AccountFields.AccountCode, Account.AccountFields.AccountRecd, Account.AccountFields.AccountDate, Account.AccountFields.AccountTranstax, Account.AccountFields.AccountInvno,
			//	Account.AccountFields.AccountQty, Account.AccountFields.AccountAmount, Appointment.AppointmentFields.AppointmentDuration,
			//	Appointment.AppointmentFields.AppointmentType, Appointment.AppointmentFields.AppointmentNote, Client.ClientFields.ClientEmail,
			//	Quotail.QuotailFields.QuotailVariance, Po.PoFields.PoCost, Audit.AuditFields.AuditTime };

			string[] testFields = { Animal.AnimalFields.AnimalLocator };

			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < testFields.Length; y++)
				{
					var tableObject = GetTableType(testFields[y], x == 0 ? _legacySourcePath : _modernSourcePath);
					var fieldDefinition = tableObject.FindFieldDefinition(testFields[y]);
					var fileName = Path.Combine(Directory.GetCurrentDirectory(), CsvFileName(testFields[y]));

					for (int z = 0; z < Enum.GetNames(typeof(ComparisonType)).Length; z++)     
					{
						if ((fieldDefinition.FieldType == AVImarkDataType.AVImarkDynamicString || fieldDefinition.FieldType == AVImarkDataType.AVImarkLinkToPhrase || 
							fieldDefinition.FieldType == AVImarkDataType.AVImarkLinkToWp || fieldDefinition.FieldType == AVImarkDataType.AVImarkCharacter) && 
                            ((ComparisonType)z != ComparisonType.EqualTo && (ComparisonType)z != ComparisonType.NotEqualTo && (ComparisonType)z != ComparisonType.Contains))
						{
							//If this is type string, the only valid comparisons are equal, not equal, and contains
							continue;
						}
						else if ((fieldDefinition.FieldType != AVImarkDataType.AVImarkDynamicString && fieldDefinition.FieldType != AVImarkDataType.AVImarkLinkToPhrase &&
							fieldDefinition.FieldType != AVImarkDataType.AVImarkLinkToWp) && ((ComparisonType)z == ComparisonType.Contains))
						{
							//'Contains' is not allowed unless it is a string.
							continue;
						}

						string comparator = GetRandomValueFromFile(fileName, testFields[y]);

						if (fieldDefinition.FieldType == AVImarkDataType.AVImarkDate)
						{
							tableObject.AddFilterCriteria(testFields[y], (ComparisonType)z, Convert.ToDateTime(comparator));
						}
						else
						{
							tableObject.AddFilterCriteria(testFields[y], (ComparisonType)z, comparator);
						}

						if (tableObject.GetType() == typeof(Account))
						{
							var tableList = ((Account)tableObject).AccountList();

							if (!CheckValuesForAccuracy(tableList, testFields[y], (ComparisonType)z, comparator))
                            {
								Assert.Fail("Field " + testFields[y] + " Failed.");
							}
						}
						else if (tableObject.GetType() == typeof(Animal))
						{
							var tableList = ((Animal)tableObject).AnimalList();

							if (!CheckValuesForAccuracy(tableList, testFields[y], (ComparisonType)z, comparator))
							{
								Assert.Fail("Field " + testFields[y] + " Failed.");
							}
						}
						else if (tableObject.GetType() == typeof(Appointment))
						{
							var tableList = ((Appointment)tableObject).AppointmentList();

							if (!CheckValuesForAccuracy(tableList, testFields[y], (ComparisonType)z, comparator))
							{
								Assert.Fail("Field " + testFields[y] + " Failed.");
							}
						}
						else if (tableObject.GetType() == typeof(Service))
						{
							var tableList = ((Service)tableObject).ServiceList();

							if (!CheckValuesForAccuracy(tableList, testFields[y], (ComparisonType)z, comparator))
							{
								Assert.Fail("Field " + testFields[y] + " Failed.");
							}
						}
						else if (tableObject.GetType() == typeof(Client))
						{
							var tableList = ((Client)tableObject).ClientList();

							if (!CheckValuesForAccuracy(tableList, testFields[y], (ComparisonType)z, comparator))
							{
								Assert.Fail("Field " + testFields[y] + " Failed.");
							}
						}
						else if (tableObject.GetType() == typeof(Quotail))
						{
							var tableList = ((Quotail)tableObject).QuotailList();

							if (!CheckValuesForAccuracy(tableList, testFields[y], (ComparisonType)z, comparator))
							{
								Assert.Fail("Field " + testFields[y] + " Failed.");
							}
						}
						else if (tableObject.GetType() == typeof(Po))
						{
							var tableList = ((Po)tableObject).PoList();

							if (!CheckValuesForAccuracy(tableList, testFields[y], (ComparisonType)z, comparator))
							{
								Assert.Fail("Field " + testFields[y] + " Failed.");
							}
						}
					}
				}
			}

			Assert.IsFalse(false, "CarsonLibTest Passed!!");
		}

		private CarsonBackend GetTableType(string fieldName, string filePath)
		{
			string tableName = fieldName.Split('_')[0];

			switch (tableName)
			{
				case "ACCOUNT":
					return new Account(filePath);

				case "ANIMAL":
					return new Animal(filePath);

				case "APPOINT":
					return new Appointment(filePath);

				case "SERVICE":
					return new Service(filePath);

				case "CLIENT":
					return new Client(filePath);

				case "QUOTAIL":
					return new Quotail(filePath);

				case "PO":
					return new Po(filePath);

                case "AUDIT":
                    return new Audit(filePath);
			}

			return null;
		}

		private string CsvFileName(string fieldName)
		{
			return fieldName.Split('_')[0].ToLower() + ".csv";
		}

		private string GetRandomValueFromFile(string fileName, string fieldName)
		{
            int iterationCount = 0;
			int fileLineCount = 0;
			string returnValue = "";
			Random random = new Random();

			do
            {
                var file = File.ReadLines(fileName).ToList();
                fileLineCount = file.Count();
                int skip = random.Next(0, fileLineCount);
                string line = file.Skip(skip).First();

                TextFieldParser parser = new TextFieldParser(new StringReader(line));
                parser.HasFieldsEnclosedInQuotes = true;
                parser.SetDelimiters(",");
                string[] fields = parser.ReadFields();

                returnValue = fields[CsvFieldPosition(fieldName)];

                if (returnValue.Length > 0 && returnValue != "0" && returnValue != "0.000")
                    return returnValue;

                iterationCount++;
            } while (iterationCount < fileLineCount);         //loop <File Line Count> times to try to find a real value

			//give the last value found regardless of whether or not it's what we want
            return returnValue;
		}

		private int CsvFieldPosition(string fieldName)
		{
            if (fieldName == Account.AccountFields.AccountCode)
                return 1;
            else if (fieldName == Account.AccountFields.AccountRecd)
                return 0;
            else if (fieldName == Account.AccountFields.AccountDate)
                return 2;
            else if (fieldName == Account.AccountFields.AccountTranstax)
                return 6;
            else if (fieldName == Account.AccountFields.AccountInvno)
                return 8;
            else if (fieldName == Account.AccountFields.AccountQty)
                return 9;
            else if (fieldName == Account.AccountFields.AccountAmount)
                return 10;
            else if (fieldName == Appointment.AppointmentFields.AppointmentDuration)
                return 3;
            else if (fieldName == Appointment.AppointmentFields.AppointmentType)
                return 5;
            else if (fieldName == Appointment.AppointmentFields.AppointmentNote)
                return 7;
            else if (fieldName == Service.ServiceFields.ServiceSfPublicNotes)
                return 7;
            else if (fieldName == Client.ClientFields.ClientSuspend)
                return 16;
            else if (fieldName == Animal.AnimalFields.AnimalLocator)
                return 20;
            else if (fieldName == Quotail.QuotailFields.QuotailVariance)
                return 6;
            else if (fieldName == Po.PoFields.PoCost)
                return 8;
            else if (fieldName == Audit.AuditFields.AuditTime)
                return 2;
            else
                throw new FieldAccessException();
        }

		private PropertyInfo FindPropertyName<T>(string fieldName)
		{
			var properties = typeof(T).GetProperties();

			fieldName = fieldName.ToLower().Replace("_", "");

			foreach (var p in properties)
			{
				if (p.Name.ToLower() == fieldName)
				{
					return p;
				}
			}

			return null;
		}

        private bool CheckValuesForAccuracy(List<Account.AccountData> accounts, string fieldName, ComparisonType comparisonType, string comparator)
        {
            List<int> fileValues = new List<int>();
            fileValues = LoadFromFile(fieldName, comparisonType, comparator);

			if (fileValues.Count != accounts.Count)
			{
				System.Diagnostics.Debug.WriteLine("CheckFailure - Accounts - Field: " + fieldName + " Comparison: " + comparisonType.ToString() + " Compare Value: " + comparator + " Found: " + fileValues.Count + " Expected: " + accounts.Count.ToString());
				return false;
			}

			for (int x = 0; x < fileValues.Count; x++)
			{
				if (!fileValues[x].Equals(accounts[x].AccountId))
				{
					System.Diagnostics.Debug.WriteLine("CheckFailure - Accounts Mismatch on line: " + fileValues[x].ToString());
					return false;
				}
			}

			System.Diagnostics.Debug.WriteLine("CheckSuccess - Accounts - Field: " + fieldName + " Comparison: " + comparisonType.ToString() + " Compare Value: " + comparator + " Found: " + fileValues.Count + " Expected: " + accounts.Count.ToString());
			return true;
		}

		private bool CheckValuesForAccuracy(List<Animal.AnimalData> animals, string fieldName, ComparisonType comparisonType, string comparator)
		{
			List<int> fileValues = new List<int>();
			fileValues = LoadFromFile(fieldName, comparisonType, comparator);

			if (fileValues.Count != animals.Count)
			{
				System.Diagnostics.Debug.WriteLine("CheckFailure - Animals - Field: " + fieldName + " Comparison: " + comparisonType.ToString() + " Compare Value: " + comparator + " Found: " + fileValues.Count + " Expected: " + animals.Count.ToString());
				return false;
			}

			for (int x = 0; x < fileValues.Count; x++)
			{
				if (!fileValues[x].Equals(animals[x].AnimalId))
				{
					System.Diagnostics.Debug.WriteLine("CheckFailure - Animals Mismatch on line: " + fileValues[x].ToString());
					return false;
				}
			}

			System.Diagnostics.Debug.WriteLine("CheckSuccess - Animals - Field: " + fieldName + " Comparison: " + comparisonType.ToString() + " Compare Value: " + comparator + " Found: " + fileValues.Count + " Expected: " + animals.Count.ToString());
			return true;
		}

		private bool CheckValuesForAccuracy(List<Appointment.AppointmentData> appointments, string fieldName, ComparisonType comparisonType, string comparator)
		{
			List<int> fileValues = new List<int>();
			fileValues = LoadFromFile(fieldName, comparisonType, comparator);

			if (fileValues.Count != appointments.Count)
			{
				System.Diagnostics.Debug.WriteLine("CheckFailure - Appointments - Field: " + fieldName + " Comparison: " + comparisonType.ToString() + " Compare Value: " + comparator + " Found: " + fileValues.Count + " Expected: " + appointments.Count.ToString());
				return false;
			}

			for (int x = 0; x < fileValues.Count; x++)
			{
				if (!fileValues[x].Equals(appointments[x].AppointmentId))
				{
					System.Diagnostics.Debug.WriteLine("CheckFailure - Appointments Mismatch on line: " + fileValues[x].ToString());
					return false;
				}
			}

			System.Diagnostics.Debug.WriteLine("CheckSuccess - Appointments - Field: " + fieldName + " Comparison: " + comparisonType.ToString() + " Compare Value: " + comparator + " Found: " + fileValues.Count + " Expected: " + appointments.Count.ToString());
			return true;
		}

		private bool CheckValuesForAccuracy(List<Service.ServiceData> services, string fieldName, ComparisonType comparisonType, string comparator)
		{
			List<int> fileValues = new List<int>();
			fileValues = LoadFromFile(fieldName, comparisonType, comparator);

			if (fileValues.Count != services.Count)
			{
				System.Diagnostics.Debug.WriteLine("CheckFailure - Services - Field: " + fieldName + " Comparison: " + comparisonType.ToString() + " Compare Value: " + comparator + " Found: " + fileValues.Count + " Expected: " + services.Count.ToString());
				return false;
			}

			for (int x = 0; x < fileValues.Count; x++)
			{
				if (!fileValues[x].Equals(services[x].Id))
				{
					System.Diagnostics.Debug.WriteLine("CheckFailure - Services Mismatch on line: " + fileValues[x].ToString());
					return false;
				}
			}

			System.Diagnostics.Debug.WriteLine("CheckSuccess - Services - Field: " + fieldName + " Comparison: " + comparisonType.ToString() + " Compare Value: " + comparator + " Found: " + fileValues.Count + " Expected: " + services.Count.ToString());
			return true;
		}

		private bool CheckValuesForAccuracy(List<Client.ClientData> clients, string fieldName, ComparisonType comparisonType, string comparator)
		{
			List<int> fileValues = new List<int>();
			fileValues = LoadFromFile(fieldName, comparisonType, comparator);

			if (fileValues.Count != clients.Count)
			{
				System.Diagnostics.Debug.WriteLine("CheckFailure - Clients - Field: " + fieldName + " Comparison: " + comparisonType.ToString() + " Compare Value: " + comparator + " Found: " + fileValues.Count + " Expected: " + clients.Count.ToString());
				return false;
			}

			for (int x = 0; x < fileValues.Count; x++)
			{
				if (!fileValues[x].Equals(clients[x].ClientId))
				{
					System.Diagnostics.Debug.WriteLine("CheckFailure - Clients Mismatch on line: " + fileValues[x].ToString());
					return false;
				}
			}

			System.Diagnostics.Debug.WriteLine("CheckSuccess - Clients - Field: " + fieldName + " Comparison: " + comparisonType.ToString() + " Compare Value: " + comparator + " Found: " + fileValues.Count + " Expected: " + clients.Count.ToString());
			return true;
		}

		private bool CheckValuesForAccuracy(List<Quotail.QuotailData> quotails, string fieldName, ComparisonType comparisonType, string comparator)
		{
			List<int> fileValues = new List<int>();
			fileValues = LoadFromFile(fieldName, comparisonType, comparator);

			if (fileValues.Count != quotails.Count)
			{
				System.Diagnostics.Debug.WriteLine("CheckFailure - Quotail - Field: " + fieldName + " Comparison: " + comparisonType.ToString() + " Compare Value: " + comparator + " Found: " + fileValues.Count + " Expected: " + quotails.Count.ToString());
				return false;
			}

			for (int x = 0; x < fileValues.Count; x++)
			{
				if (!fileValues[x].Equals(quotails[x].Id))
				{
					System.Diagnostics.Debug.WriteLine("CheckFailure - Quotail Mismatch on line: " + fileValues[x].ToString());
					return false;
				}
			}

			System.Diagnostics.Debug.WriteLine("CheckSuccess - Quotail - Field: " + fieldName + " Comparison: " + comparisonType.ToString() + " Compare Value: " + comparator + " Found: " + fileValues.Count + " Expected: " + quotails.Count.ToString());
			return true;
		}

		private bool CheckValuesForAccuracy(List<Po.PoData> pos, string fieldName, ComparisonType comparisonType, string comparator)
		{
			List<int> fileValues = new List<int>();
			fileValues = LoadFromFile(fieldName, comparisonType, comparator);

			if (fileValues.Count != pos.Count)
			{
				System.Diagnostics.Debug.WriteLine("CheckFailure - Po - Field: " + fieldName + " Comparison: " + comparisonType.ToString() + " Compare Value: " + comparator + " Found: " + fileValues.Count + " Expected: " + pos.Count.ToString());
				return false;
			}

			for (int x = 0; x < fileValues.Count; x++)
			{
				if (!fileValues[x].Equals(pos[x].Id))
				{
					System.Diagnostics.Debug.WriteLine("CheckFailure - Po Mismatch on line: " + fileValues[x].ToString());
					return false;
				}
			}

			System.Diagnostics.Debug.WriteLine("CheckSuccess - Po - Field: " + fieldName + " Comparison: " + comparisonType.ToString() + " Compare Value: " + comparator + " Found: " + fileValues.Count + " Expected: " + pos.Count.ToString());
			return true;
		}

		private List<int>LoadFromFile(string fieldName, ComparisonType comparisonType, string comparator)
        {
            var fileName = Path.Combine(Directory.GetCurrentDirectory(), CsvFileName(fieldName));
            List<int> fileValues = new List<int>();
            string[] fields = null;

            var lines = File.ReadLines(fileName);
            int lineCount = File.ReadAllLines(fileName).Length;

            for (int x = 0; x < lineCount; x++)
            {
                var line = new List<string>(lines)[x];

                TextFieldParser parser = new TextFieldParser(new StringReader(line));
                parser.HasFieldsEnclosedInQuotes = true;
                parser.SetDelimiters(",");
                fields = parser.ReadFields();

				string compareValue = fields[CsvFieldPosition(fieldName)];

                dynamic compare;
                dynamic compareAgainst;

                if (IsDate(compareValue))
                {
                    compare = Convert.ToDateTime(comparator);
                    compareAgainst = Convert.ToDateTime(compareValue);
                }
                else if (IsNumeric(compareValue))
                {
                    compare = Convert.ToDecimal(comparator);
                    compareAgainst = Convert.ToDecimal(compareValue);
                }
                else
                {
                    compare = comparator;
                    compareAgainst = compareValue;
                }

                bool addRecord = false;

                switch (comparisonType)
                {
                    case ComparisonType.EqualTo:
                        if (compareAgainst == compare)
                            addRecord = true;

                        break;

                    case ComparisonType.GreaterThan:
                        if (compareAgainst > compare)
                            addRecord = true;

                        break;

                    case ComparisonType.GreaterThanEqual:
                        if (compareAgainst >= compare)
                            addRecord = true;

                        break;

                    case ComparisonType.LessThan:
                        if (compareAgainst < compare)
                            addRecord = true;

                        break;

                    case ComparisonType.LessThanEqual:
                        if (compareAgainst <= compare)
                            addRecord = true;

                        break;

                    case ComparisonType.NotEqualTo:
                        if (compareAgainst != compare)
                            addRecord = true;

                        break;

                    case ComparisonType.Contains:
                        if (((string)compareAgainst).Contains(compare))
                            addRecord = true;

                        break;
                }

                if (addRecord)
                {
					//add line number
                    fileValues.Add(x + 1);
                }
            }

            return fileValues;
        }

        private bool IsDate(string test)
        {
            DateTime testDate;

            if (DateTime.TryParse(test, out testDate) && test.Contains("/"))
                return true;
            else
                return false;
        }

        private bool IsNumeric(string test)
        {
            decimal testValue;

            if (decimal.TryParse(test, out testValue))
                return true;
            else
                return false;
        }
    }
}
