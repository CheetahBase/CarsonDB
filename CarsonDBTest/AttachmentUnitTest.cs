using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;

namespace CarsonDBTest
{
	[TestClass]
	public class AttachmentUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void AttachmentTestInit()
		{
			Shared.BuildAttachRecords();
		}

		[TestCleanup]
		public void AttachmentTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void AttachmentTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Attachment attachment = new Attachment(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					var attachmentList = attachment.AttachmentList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "attach.csv"));

					foreach (var line in lines)
					{	
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!AttachmentRecordCompare(attachmentList, fields, lineNumber))
						{
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}
			}

			Assert.IsFalse(false, "AttachmentTest1 Passed!!");
		}

		[TestMethod]
		public void AttachmentTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					string[] fields = null;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "attach.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "attach.csv")).Length;

					if (y > lineCount - 1)
						break;

					var line = new List<string>(lines)[y];

					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					fields = parser.ReadFields();

					using (Attachment attachment = new Attachment(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						if (fields[0].Length > 0)
							attachment.AddFilterCriteria(Attachment.AttachmentFields.AttachmentRecd, ComparisonType.EqualTo, fields[0]);

						if (fields[1].Length > 0)
							attachment.AddFilterCriteria(Attachment.AttachmentFields.AttachmentFileName, ComparisonType.EqualTo, fields[1]);

						if (fields[2].Length > 0)
							attachment.AddFilterCriteria(Attachment.AttachmentFields.AttachmentDescription, ComparisonType.EqualTo, fields[2]);

						if (fields[3].Length > 0)
							attachment.AddFilterCriteria(Attachment.AttachmentFields.AttachmentParentis, ComparisonType.EqualTo, Convert.ToInt32(fields[3]));

						if (fields[4].Length > 0)
							attachment.AddFilterCriteria(Attachment.AttachmentFields.AttachmentParent, ComparisonType.EqualTo, Convert.ToInt32(fields[4]));

						var attachmentList = attachment.AttachmentList();

						if (attachmentList.Count == 0)
						{
							Assert.Fail("Match failed on line: " + (y + 1).ToString());
						}
					}
				}
			}

			Assert.IsTrue(true, "AttachmentTest2 Passed!!");
		}

		private bool AttachmentRecordCompare(List<Attachment.AttachmentData> attachmentList, string[] fields, int lineNumber)
		{
			if (attachmentList[lineNumber].AttachmentRecd == fields[0] && attachmentList[lineNumber].AttachmentFileName == fields[1] && attachmentList[lineNumber].AttachmentDescription == fields[2] && Shared.CompareAmount(attachmentList[lineNumber].AttachmentParentis, fields[3]) &&
					Shared.CompareAmount(attachmentList[lineNumber].AttachmentParent, fields[4]))
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
