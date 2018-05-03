using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class UserUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void UserTestInit()
		{
			Shared.BuildUserRecords();
		}

		[TestCleanup]
		public void UserTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void UserTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (User user = new User(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					var userList = user.UserList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "user.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!UserRecordCompare(userList, fields, lineNumber))
						{
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}
			}

			Assert.IsFalse(false, "UserTest1 Passed!!");
		}

		[TestMethod]
		public void UserTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					string[] fields = null;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "user.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "user.csv")).Length;

					if (y > lineCount - 1)
						break;

					var line = new List<string>(lines)[y];

					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					fields = parser.ReadFields();

					using (User user = new User(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						if (fields[0].Length > 0)
							user.AddFilterCriteria(User.UserFields.UserRecd, ComparisonType.EqualTo, fields[0]);

						if (fields[1].Length > 0)
							user.AddFilterCriteria(User.UserFields.UserId, ComparisonType.EqualTo, fields[1]);

						if (fields[2].Length > 0)
							user.AddFilterCriteria(User.UserFields.UserFirst, ComparisonType.EqualTo, fields[2]);

						if (fields[3].Length > 0)
							user.AddFilterCriteria(User.UserFields.UserStaff, ComparisonType.EqualTo, Convert.ToInt32(fields[3]));

						if (fields[4].Length > 0)
							user.AddFilterCriteria(User.UserFields.UserLast, ComparisonType.EqualTo, fields[4]);

						if (!fields[7].Contains("1900"))
							user.AddFilterCriteria(User.UserFields.UserAdded, ComparisonType.EqualTo, DateTime.Parse(fields[7]));

						var userList = user.UserList();

						if (userList.Count == 0)
						{
							Assert.Fail("Match failed on line: " + (y + 1).ToString());
						}
					}
				}
			}

			Assert.IsTrue(true, "UserTest2 Passed!!");
		}

		private bool UserRecordCompare(List<User.UserData> userList, string[] fields, int lineNumber)
		{
			if (userList[lineNumber].UserRecd == fields[0] && userList[lineNumber].UserId == fields[1] && userList[lineNumber].UserFirst == fields[2] && Shared.CompareAmount(userList[lineNumber].UserStaff, fields[3]) &&
					userList[lineNumber].UserLast == fields[4] && Shared.CompareBool(userList[lineNumber].UserDoctor, fields[5]) && Shared.CompareBool(userList[lineNumber].UserInactive, fields[6]) &&
					(userList[lineNumber].UserAdded - DateTime.Parse(fields[7])).TotalDays == 0)
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
