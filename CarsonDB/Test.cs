using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Test : CarsonBackend
	{
		private enum TestFieldOrdinals
		{
			Id,
			TestRecd,
			TestHistory,
			TestDate,
			TestName,
			TestInstrument,
			TestTime,
			TestStatus,
			TestPatient
		}

		public static class TestFields
		{
			public const string Id = "ID";
			public const string TestRecd = "TEST_RECD";
			public const string TestHistory = "TEST_HISTORY";
			public const string TestDate = "TEST_DATE";
			public const string TestName = "TEST_NAME";
			public const string TestInstrument = "TEST_INSTRUMENT";
			public const string TestTime = "TEST_TOD";
			public const string TestStatus = "TEST_STATUS";
			public const string TestPatient = "TEST_PATIENT";
		}

		public class TestData : ICrc
		{
			private int _recordNumber;
			private Test _test;

			public TestData(Test test, int recordNumber)
			{
				_test = test;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int testId = 0;

					try
					{
						testId = _test.IntegerFieldValue(TestFieldOrdinals.Id, _recordNumber);
						return testId;
					}
					catch (RecordLockedException)
					{
						testId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return testId;
					}
				}
			}

			public string TestRecd
			{
				get
				{
					return _test.CharacterFieldValue(TestFieldOrdinals.TestRecd, _recordNumber);
				}
			}

			public int TestHistory
			{
				get
				{
					return _test.IntegerFieldValue(TestFieldOrdinals.TestHistory, _recordNumber);
				}
			}

			public DateTime TestDate
			{
				get
				{
					return _test.DateFieldValue(TestFieldOrdinals.TestDate, _recordNumber);
				}
			}

			public string TestName
			{
				get
				{
					return _test.StringFieldValue(TestFieldOrdinals.TestName, _recordNumber);
				}
			}

			public string TestInstrument
			{
				get
				{
					return _test.StringFieldValue(TestFieldOrdinals.TestInstrument, _recordNumber);
				}
			}

			public string TestTime
			{
				get
				{
					return _test.TimeFieldValue(TestFieldOrdinals.TestTime, _recordNumber);
				}
			}

			public int TestStatus
			{
				get
				{
					return _test.IntegerFieldValue(TestFieldOrdinals.TestStatus, _recordNumber);
				}
			}

			public int TestPatient
			{
				get
				{
					return _test.IntegerFieldValue(TestFieldOrdinals.TestPatient, _recordNumber);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Test()
		{
			AddDatabaseDefinitions(CarsonStaticSettings.DatabasePath);
		}

		public Test(string databasePath)
		{
			AddDatabaseDefinitions(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "TEST";
			this.DatabasePath = databasePath;
			this.TableId = TableInstance.Test;

			this.AddFieldDefinition(TestFields.Id, TestFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(TestFields.TestRecd, TestFieldOrdinals.TestRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(TestFields.TestHistory, TestFieldOrdinals.TestHistory, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(TestFields.TestDate, TestFieldOrdinals.TestDate, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(TestFields.TestName, TestFieldOrdinals.TestName, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(TestFields.TestInstrument, TestFieldOrdinals.TestInstrument, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(TestFields.TestTime, TestFieldOrdinals.TestTime, AVImarkDataType.AVImarkTime);
			this.AddFieldDefinition(TestFields.TestStatus, TestFieldOrdinals.TestStatus, AVImarkDataType.AVImarkByte);
			this.AddFieldDefinition(TestFields.TestPatient, TestFieldOrdinals.TestPatient, AVImarkDataType.AVImarkDoubleWord);
		}

		public List<TestData> TestList()
		{
			List<TestData> testList = new List<TestData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				TestData testData = new TestData(this, x);
				testList.Add(testData);
			}

			return testList;
		}
	}
}
