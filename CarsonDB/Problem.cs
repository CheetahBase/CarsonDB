using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Problem : CarsonBackend
	{
		private enum ProblemFieldOrdinals
		{
			Id,
			ProblemCode,
			ProblemRecd,
			ProblemDescription
		}

		public static class ProblemFields
		{
			public const string Id = "ID";
			public const string ProblemCode = "PROBLEM_CODE";
			public const string ProblemRecd = "PROBLEM_RECD";
			public const string ProblemDescription = "PROBLEM_DESCRIPTION";
		}

		public class ProblemData : ICrc
		{
			private int _recordNumber;
			private Problem _problem;

			public ProblemData(Problem problem, int recordNumber)
			{
				_problem = problem;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int problemId = 0;

					try
					{
						problemId = _problem.IntegerFieldValue(ProblemFieldOrdinals.Id, _recordNumber);
						return problemId;
					}
					catch (RecordLockedException)
					{
						problemId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return problemId;
					}
				}
			}

			public string ProblemCode
			{
				get
				{
					return _problem.StringFieldValue(ProblemFieldOrdinals.ProblemCode, _recordNumber);
				}
			}

			public string ProblemRecd
			{
				get
				{
					return _problem.CharacterFieldValue(ProblemFieldOrdinals.ProblemRecd, _recordNumber);
				}
			}

			public string ProblemDescription
			{
				get
				{
					return _problem.StringFieldValue(ProblemFieldOrdinals.ProblemDescription, _recordNumber);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Problem()
		{
			AddDatabaseDefinitions(CarsonStaticSettings.DatabasePath);
		}

		public Problem(string databasePath)
		{
			AddDatabaseDefinitions(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "PROBLEM";
			this.DatabasePath = databasePath;
			this.TableId = this.TableId = TableInstance.Problem;

			this.AddFieldDefinition(ProblemFields.Id, ProblemFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(ProblemFields.ProblemCode, ProblemFieldOrdinals.ProblemCode, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(ProblemFields.ProblemRecd, ProblemFieldOrdinals.ProblemRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(ProblemFields.ProblemDescription, ProblemFieldOrdinals.ProblemDescription, AVImarkDataType.AVImarkDynamicString);
		}

		public List<ProblemData> ProblemList()
		{
			List<ProblemData> problemList = new List<ProblemData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				ProblemData problemData = new ProblemData(this, x);
				problemList.Add(problemData);
			}

			return problemList;
		}
	}
}
