using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using CarsonDB;
using System.Linq;

namespace CarsonDBTest
{
	[TestClass]
	public class AppointmentUnitTest
	{
		private string _legacySourcePath = Path.Combine(Directory.GetCurrentDirectory(), "LEGACY");
		private string _modernSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MODERN");

		[TestInitialize]
		public void AppointmentTestInit()
		{
			Shared.BuildAppointRecords();
		}

		[TestCleanup]
		public void AppointmentTestCleanup()
		{
			Shared.UnloadDll();
		}

		[TestMethod]
		public void AppointmentTest1()
		{
			for (int x = 0; x < 2; x++)
			{
				int lineNumber = 0;

				using (Appointment appointment = new Appointment(x == 0 ? _legacySourcePath : _modernSourcePath))
				{
					var appointmentList = appointment.AppointmentList();

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "appoint.csv"));

					foreach (var line in lines)
					{
						TextFieldParser parser = new TextFieldParser(new StringReader(line));
						parser.HasFieldsEnclosedInQuotes = true;
						parser.SetDelimiters(",");
						string[] fields = parser.ReadFields();

						if (!AppointmentRecordCompare(appointmentList, fields, lineNumber))
						{
							Assert.Fail("Match failed on line: " + (lineNumber + 1).ToString());
						}

						lineNumber++;
					}
				}
			}

			Assert.IsFalse(false, "AppointmentTest1 Passed!!");
		}

		[TestMethod]
		public void AppointmentTest2()
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					string[] fields = null;

					var lines = File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), "appoint.csv"));
					int lineCount = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "appoint.csv")).Length;

					if (y > lineCount - 1)
						break;

					var line = new List<string>(lines)[y];

					TextFieldParser parser = new TextFieldParser(new StringReader(line));
					parser.HasFieldsEnclosedInQuotes = true;
					parser.SetDelimiters(",");
					fields = parser.ReadFields();

					using (Appointment appointment = new Appointment(x == 0 ? _legacySourcePath : _modernSourcePath))
					{
						if (fields[0].Length > 0)
							appointment.AddFilterCriteria(Appointment.AppointmentFields.AppointmentRecd, ComparisonType.EqualTo, fields[0]);

						if (!fields[1].Contains("1900"))
							appointment.AddFilterCriteria(Appointment.AppointmentFields.AppointmentDate, ComparisonType.EqualTo, DateTime.Parse(fields[1]));

						if (!fields[2].Contains("1900"))
							appointment.AddFilterCriteria(Appointment.AppointmentFields.AppointmentDateMade, ComparisonType.EqualTo, DateTime.Parse(fields[2]));

						if (fields[3].Length > 0)
							appointment.AddFilterCriteria(Appointment.AppointmentFields.AppointmentDuration, ComparisonType.EqualTo, Convert.ToInt32(fields[3]));

						if (fields[4].Length > 0)
							appointment.AddFilterCriteria(Appointment.AppointmentFields.AppointmentDoctor, ComparisonType.EqualTo, fields[4]);

						if (fields[5].Length > 0)
							appointment.AddFilterCriteria(Appointment.AppointmentFields.AppointmentType, ComparisonType.EqualTo, Convert.ToInt32(fields[5]));

						if (fields[6].Length > 0)
							appointment.AddFilterCriteria(Appointment.AppointmentFields.AppointmentClient, ComparisonType.EqualTo, Convert.ToInt32(fields[6]));

						if (fields[7].Length > 0)
							appointment.AddFilterCriteria(Appointment.AppointmentFields.AppointmentNote, ComparisonType.EqualTo, fields[7]);

						if (fields[8].Length > 0)
							appointment.AddFilterCriteria(Appointment.AppointmentFields.AppointmentParent, ComparisonType.EqualTo, Convert.ToInt32(fields[8]));

						if (fields[9].Length > 0)
							appointment.AddFilterCriteria(Appointment.AppointmentFields.AppointmentParentis, ComparisonType.EqualTo, Convert.ToInt32(fields[9]));

						if (fields[10].Length > 0)
							appointment.AddFilterCriteria(Appointment.AppointmentFields.AppointmentTypeCode, ComparisonType.EqualTo, fields[10]);

						if (fields[11].Length > 0)
							appointment.AddFilterCriteria(Appointment.AppointmentFields.AppointmentNewClient, ComparisonType.EqualTo, fields[11]);

						if (fields[12].Length > 0)
							appointment.AddFilterCriteria(Appointment.AppointmentFields.AppointmentNewPatient, ComparisonType.EqualTo, fields[12]); 

						if (fields[13].Length > 0)
							appointment.AddFilterCriteria(Appointment.AppointmentFields.AppointmentNewSpecies, ComparisonType.EqualTo, fields[13]);

						if (fields[14].Length > 0)
							appointment.AddFilterCriteria(Appointment.AppointmentFields.AppointmentNewPhone, ComparisonType.EqualTo, fields[14]);

						if (fields[15].Length > 0)
							appointment.AddFilterCriteria(Appointment.AppointmentFields.AppointmentNewArea, ComparisonType.EqualTo, fields[15]);

						if (fields[16].Length > 0)
							appointment.AddFilterCriteria(Appointment.AppointmentFields.AppointmentMadeBy, ComparisonType.EqualTo, fields[16]);

						var appointmentList = appointment.AppointmentList();

						if (appointmentList.Count == 0)
						{
							Assert.Fail("Match failed on line: " + (y + 1).ToString());
						}
					}
				}
			}

			Assert.IsTrue(true, "AppointmentTest2 Passed!!");
		}

		[TestMethod]
		public void AppointmentTest3CRC()
		{
			using (Appointment appointment1 = new Appointment(_legacySourcePath))
			using (Appointment appointment2 = new Appointment(_modernSourcePath))
			{
				appointment1.CreateTableCRC("appt1.crc", Shared.GetFields(typeof(Appointment.AppointmentFields)));
				appointment2.CreateTableCRC("appt2.crc", Shared.GetFields(typeof(Appointment.AppointmentFields)));
				Shared.UpdateAppointmentFiles();
				var appointmentRecordsChanged1 = appointment1.FindChangedRecords<Appointment.AppointmentData>("appt1.crc", Appointment.DifferentialData.NewAndModifiedRecords);
				var appointmentRecordsChanged2 = appointment2.FindChangedRecords<Appointment.AppointmentData>("appt2.crc", Appointment.DifferentialData.NewAndModifiedRecords);

				string updatedRecords = File.ReadAllText("updated.csv");

				updatedRecords = updatedRecords.Replace("\r\n", "");
				string[] records = updatedRecords.Split(',');

				if (records.Length != appointmentRecordsChanged1.Count || records.Length != appointmentRecordsChanged2.Count)
				{
					Assert.Fail("Record Lengths do not match.  Found: " + appointmentRecordsChanged1.Count.ToString() + ", " + appointmentRecordsChanged2.Count.ToString() + ". Expected: " + records.Length.ToString());
				}

				string values = "";

				for (int x = 0; x < records.Length; x++)
				{
					int recordId = Convert.ToInt32(records[x]);

					values += recordId.ToString() + ", ";

					if (!appointmentRecordsChanged1.Any(i => i.Id == recordId))
					{
						Assert.Fail("Record: " + recordId.ToString() + " Not found when expected.");
					}
				}
			}

			Assert.IsTrue(true, "Successful CRC Check on Appointment Table");
		}

		private bool AppointmentRecordCompare(List<Appointment.AppointmentData> appointmentList, string[] fields, int lineNumber)
		{
			if (appointmentList[lineNumber].AppointmentRecd == fields[0] && (appointmentList[lineNumber].AppointmentDate - DateTime.Parse(fields[1])).TotalDays == 0 && 
				(appointmentList[lineNumber].AppointmentDateMade - DateTime.Parse(fields[2])).TotalDays == 0 && Shared.CompareAmount(appointmentList[lineNumber].AppointmentDuration, fields[3]) &&
					appointmentList[lineNumber].AppointmentDoctor == fields[4] && Shared.CompareAmount(appointmentList[lineNumber].AppointmentType, fields[5]) && 
					Shared.CompareAmount(appointmentList[lineNumber].AppointmentClient, fields[6]) && appointmentList[lineNumber].AppointmentNote == fields[7] && 
					Shared.CompareAmount(appointmentList[lineNumber].AppointmentParent, fields[8]) && Shared.CompareAmount(appointmentList[lineNumber].AppointmentParentis, fields[9]) &&
					appointmentList[lineNumber].AppointmentTypeCode == fields[10] && appointmentList[lineNumber].AppointmentNewClient == fields[11] &&
					appointmentList[lineNumber].AppointmentNewPatient == fields[12] && appointmentList[lineNumber].AppointmentNewSpecies == fields[13] &&
					appointmentList[lineNumber].AppointmentNewPhone == fields[14] && appointmentList[lineNumber].AppointmentNewArea == fields[15] &&
					appointmentList[lineNumber].AppointmentMadeBy == fields[16])
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
