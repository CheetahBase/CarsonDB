using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class ItemUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void ItemTestInit()
		{
			Shared.BuildItemRecords();
		}

		[TestCleanup]
		public void ItemTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void ItemTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Item item = new Item(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					var itemList = item.ItemList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "item.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!ItemRecordCompare(itemList, fields, lineNumber))
						{
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}
			}

			Assert.IsFalse(false, "ItemTest1 Passed!!");
		}

		[TestMethod]
		public void ItemTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					string[] fields = null;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "item.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "item.csv")).Length;

					if (y > lineCount - 1)
						break;

					var line = new List<string>(lines)[y];

					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					fields = parser.ReadFields();

					using (Item item = new Item(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						if (fields[0].Length > 0)
							item.AddFilterCriteria(Item.ItemFields.ItemCode, ComparisonType.EqualTo, fields[0]);

						if (fields[1].Length > 0)
							item.AddFilterCriteria(Item.ItemFields.ItemRecd, ComparisonType.EqualTo, fields[1]);

						if (fields[2].Length > 0)
							item.AddFilterCriteria(Item.ItemFields.ItemDescription, ComparisonType.EqualTo, fields[2]);

						var itemList = item.ItemList();

						if (itemList.Count == 0)
						{
							Assert.Fail("Match failed on line: " + (y + 1).ToString());
						}
					}
				}
			}

			Assert.IsTrue(true, "ItemTest2 Passed!!");
		}

		private bool ItemRecordCompare(List<Item.ItemData> itemList, string[] fields, int lineNumber)
		{
			if (itemList[lineNumber].ItemCode == fields[0] && itemList[lineNumber].ItemRecd == fields[1] && itemList[lineNumber].ItemDescription == fields[2])
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
