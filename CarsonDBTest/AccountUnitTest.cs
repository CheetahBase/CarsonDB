using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class AccountUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void AccountTestInit()
		{
			Shared.BuildAccountRecords();
		}

		[TestCleanup]
		public void AccountTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void AccountTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Account account = new Account(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					var accountList = account.AccountList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "account.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!AccountRecordCompare(accountList, fields, lineNumber))
						{
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}

				Assert.IsFalse(false, "AccountTest1 Passed!!");
			}
		}

		[TestMethod]
		public void AccountTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;
				Random random = new Random();
				decimal greaterThanAmount = (decimal)(random.Next(1000, 10000)) / 100;
				decimal lessThanAmount = greaterThanAmount * 2;

				if (lessThanAmount < 50)
				{
					lessThanAmount = greaterThanAmount + 50;
				}

				using (Account account = new Account(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					account.AddFilterCriteria(Account.AccountFields.AccountAmount, ComparisonType.GreaterThan, greaterThanAmount);
					account.AddFilterCriteria(Account.AccountFields.AccountAmount, ComparisonType.LessThan, lessThanAmount);
					account.AddFilterCriteria(Account.AccountFields.AccountDescription, ComparisonType.Contains, "food");
					var accountList = account.AccountList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "account.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (decimal.Parse(fields[10]) > greaterThanAmount && decimal.Parse(fields[10]) < lessThanAmount && fields[5].ToLower().Trim().Contains("food"))
						{
							if (!AccountRecordCompare(accountList, fields, lineNumber))
							{
								Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
							}

							lineNumber++;
						}
					}
				}
			}

			Assert.IsFalse(false, "AccountTest2 Passed!!");
		}

		[TestMethod]
		public void AccountTest3()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Account account = new Account(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					account.AddFilterCriteria(Account.AccountFields.AccountDescription, ComparisonType.Contains, "food");
					var accountList = account.AccountList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "account.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (fields[5].ToLower().Trim().Contains("food"))
						{
							if (!AccountRecordCompare(accountList, fields, lineNumber))
							{
								Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
							}

							lineNumber++;
						}
					}
				}
			}

			Assert.IsFalse(false, "AccountTest3 Passed!!");
		}

		[TestMethod]
		public void AccountTest4()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					Random random = new Random();
					string[] fields = null;
					bool done = false;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "account.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "account.csv")).Length;
					int randomLine = random.Next(0, lineCount);
					var line = new List<string>(lines)[randomLine];

					do
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						fields = parser.ReadFields();

						if (fields[6] != "0.000")
							done = true;
						else
						{
							randomLine++;

							//Greater than number of lines, go back to top of file
							if (randomLine > lineCount - 1)
								randomLine = 0;

							line = new List<string>(lines)[randomLine];
						}
					} while (!done);

					using (Account account = new Account(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						account.AddFilterCriteria(Account.AccountFields.AccountRecd, ComparisonType.EqualTo, fields[0]);

						if (fields[1].Length > 0)
							account.AddFilterCriteria(Account.AccountFields.AccountCode, ComparisonType.EqualTo, fields[1]);

						account.AddFilterCriteria(Account.AccountFields.AccountDate, ComparisonType.EqualTo, DateTime.Parse(fields[2]));

						if (fields[3].Length > 0)
							account.AddFilterCriteria(Account.AccountFields.AccountType, ComparisonType.EqualTo, fields[3]);

						if (fields[4].Length > 0)
							account.AddFilterCriteria(Account.AccountFields.AccountDoctor, ComparisonType.EqualTo, fields[4]);

						if (fields[5].Length > 0)
							account.AddFilterCriteria(Account.AccountFields.AccountDescription, ComparisonType.EqualTo, fields[5]);

						account.AddFilterCriteria(Account.AccountFields.AccountTranstax, ComparisonType.EqualTo, Convert.ToSingle(fields[6]));
						account.AddFilterCriteria(Account.AccountFields.AccountTranltax, ComparisonType.EqualTo, Convert.ToSingle(fields[7]));
						account.AddFilterCriteria(Account.AccountFields.AccountInvno, ComparisonType.EqualTo, Convert.ToInt32(fields[8]));
						account.AddFilterCriteria(Account.AccountFields.AccountQty, ComparisonType.EqualTo, Convert.ToSingle(fields[9]));
						account.AddFilterCriteria(Account.AccountFields.AccountAmount, ComparisonType.EqualTo, Convert.ToDecimal(fields[10]));
						account.AddFilterCriteria(Account.AccountFields.AccountService, ComparisonType.EqualTo, Convert.ToInt32(fields[11]));
						account.AddFilterCriteria(Account.AccountFields.AccountAnimal, ComparisonType.EqualTo, Convert.ToInt32(fields[12]));
						account.AddFilterCriteria(Account.AccountFields.AccountClient, ComparisonType.EqualTo, Convert.ToInt32(fields[13]));

						if (fields[14].Length > 0)
							account.AddFilterCriteria(Account.AccountFields.AccountAddedBy, ComparisonType.EqualTo, fields[14]);

						var accountList = account.AccountList();

						if (accountList.Count == 0)
						{
							Assert.Fail("Match failed on line: " + (randomLine + 1).ToString());
						}
					}
				}
			}

			Assert.IsTrue(true, "AccountTest4 Passed!!");
		}

		private bool AccountRecordCompare(List<Account.AccountData> accountList, string[] fields, int lineNumber)
		{
			if (accountList[lineNumber].AccountRecd == fields[0] && accountList[lineNumber].AccountCode == fields[1] && (accountList[lineNumber].AccountDate - DateTime.Parse(fields[2])).TotalDays == 0 &&
				accountList[lineNumber].AccountType == fields[3] && accountList[lineNumber].AccountDoctor == fields[4] && accountList[lineNumber].AccountDescription == fields[5] &&
				Shared.CompareAmount(accountList[lineNumber].AccountTranstax, fields[6]) && Shared.CompareAmount(accountList[lineNumber].AccountTranltax, fields[7]) &&
				accountList[lineNumber].AccountInvno.ToString() == fields[8] && Shared.CompareAmount(accountList[lineNumber].AccountQty, fields[9]) &&
				Shared.CompareAmount(accountList[lineNumber].AccountAmount, fields[10]) && accountList[lineNumber].AccountService.ToString() == fields[11] &&
				accountList[lineNumber].AccountAnimal.ToString() == fields[12] && accountList[lineNumber].AccountClient.ToString() == fields[13] &&
				accountList[lineNumber].AccountAddedBy == fields[14])
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
