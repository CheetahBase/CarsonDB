using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using System.Globalization;

namespace CarsonDB
{
	public partial class CarsonBackend : IDisposable
	{
		public enum StringComparisonType
		{
			Contains,
			EqualTo,
			NotEqualTo
		}

		public class FilterClause
		{
			public Enum FieldOrdinal;
			public ComparisonType Comparison;
			public string CompareTo;
		}

		public const string DatabaseVersion = "01.00.00";
		public string DatabaseName;
		public string DatabasePath;
		public TableInstance TableId;

		protected List<Definition> DatabaseDefinition = new List<Definition>();
		protected bool ReadError = false;
		protected int? ModernRecordLength = null;
		protected int? ClassicRecordLength = null;
		protected bool ForcePhraseOpen = false;
		protected bool ForceWpOpen = false;

		private FileStream _stream = null;
		private BinaryReader _reader = null;
		private FileStream _phraseStream = null;
		private FileStream _wpStream = null;
		private bool _isClassic = false;
		private int _currentRecord = -1;
		private int _lastRecord = -1;
		private bool _isDataFileOpen = false;
		private bool _queryExecuted = false;

		private List<int> _fieldNames = new List<int>();
		private List<FilterClause> _filterClause = new List<FilterClause>();
		private List<List<int>> _foundRecords = new List<List<int>>();
		private List<int> _dataRecords = new List<int>();
		private DateTime _zeroDate = new DateTime(1900, 1, 1);
		private CarsonLibDll _carsonLib;

		public enum DifferentialData
		{
			NewRecords,
			ModifiedRecords,
			NewAndModifiedRecords
		}

		/// <summary>
		/// Opens the database/table file and allocates FileStream and other related objects.
		/// </summary>
		public void Open()
		{
			string fileName = null;

			if (_stream == null)
			{
				if (File.Exists(Path.Combine(this.DatabasePath, this.DatabaseName + ".V2$")))
				{
					fileName = Path.Combine(this.DatabasePath, this.DatabaseName + ".V2$");
				}
				else if (File.Exists(Path.Combine(this.DatabasePath, this.DatabaseName + ".VM$")))
				{
					fileName = Path.Combine(this.DatabasePath, this.DatabaseName + ".VM$");
					this._isClassic = true;
				}
				else
				{
					throw new FileNotFoundException();
				}

				_stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				_reader = new BinaryReader(_stream, Encoding.ASCII);
				_isDataFileOpen = true;
				_carsonLib = new CarsonLibDll();
			}
		}

		/// <summary>
		/// Disposes of all opened resources.
		/// </summary>
		public void Dispose()
		{
			Close();

			if (_carsonLib != null)
			{
				_carsonLib.Dispose();
			}
		}

		/// <summary>
		/// Closes all open database/table files and disposes of resources.
		/// </summary>
		public void Close()
		{
			if (_reader != null)
			{
				_reader.Close();
				_reader.Dispose();
				_reader = null;
			}

			if (_stream != null)
			{
				_stream.Close();
				_stream.Dispose();
				_stream = null;
			}

			if (_phraseStream != null)
			{
				_phraseStream.Close();
				_phraseStream.Dispose();
				_phraseStream = null;
			}

			if (_wpStream != null)
			{
				_wpStream.Close();
				_wpStream.Dispose();
				_wpStream = null;
			}

			_isDataFileOpen = false;
		}

		/// <summary>
		/// Navigates to the first record of the result set.
		/// </summary>
		public void MoveFirst()
		{
			if (_queryExecuted)
				_currentRecord = 1;
		}
		
		/// <summary>
		/// Navigates to the last record of the result set.
		/// </summary>
		public void MoveLast()
		{
			if (_queryExecuted)
				_currentRecord = _lastRecord;
		}

		/// <summary>
		/// Navigates to the next record in the result set.
		/// </summary>
		public void MoveNext()
		{
			if (_queryExecuted)
			{
				_currentRecord++;

				if (_currentRecord > _lastRecord)
					_currentRecord = _lastRecord;
			}
		}

		/// <summary>
		/// Navigates to the previous record in the result set.
		/// </summary>
		public void MovePrevious()
		{
			if (_queryExecuted)
			{
				_currentRecord--;

				if (_currentRecord < 1)
					_currentRecord = 1;
			}
		}
		
		/// <summary>
		/// Navigates to an absolute record.
		/// </summary>
		/// <param name="recordNumber">The record in which to navigate.</param>
		public void MoveTo(int recordNumber)
		{
			if (_queryExecuted)
			{
				if (recordNumber >= 1 && recordNumber <= _lastRecord)
					_currentRecord = recordNumber;
			}
		}

		/// <summary>
		/// Retrieves the record count for the currently executed filter.
		/// </summary>
		/// <returns></returns>
		public int RecordCount()
		{
			if (_queryExecuted)
				return _lastRecord;
			else
				return -1;
		}

		/// <summary>
		/// Returns the number of minutes elapsed since midnight.
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		private string StringFromTime(string time)
		{
			TimeSpan timeValue;

			TimeSpan.TryParse(time, out timeValue);
			return (new DateTime(1900, 1, 1).Add(timeValue) - new DateTime(1900, 1, 1)).TotalMinutes.ToString();
		}

		/// <summary>
		/// Adds a decimal type filter. 
		/// </summary>
		/// <param name="fieldName">The field name for which a filter is added.</param>
		/// <param name="comparisonType">The type of comparison to use in the filter.</param>
		/// <param name="compareTo">The value to compare to.</param>
		public void AddFilterCriteria(string fieldName, ComparisonType comparisonType, decimal compareTo)
		{
			AddFilterCriteria(fieldName, comparisonType, compareTo.ToString());
		}

		/// <summary>
		/// Adds an integer type filter. 
		/// </summary>
		/// <param name="fieldName">The field name for which a filter is added.</param>
		/// <param name="comparisonType">The type of comparison to use in the filter.</param>
		/// <param name="compareTo">The value to compare to.</param>
		public void AddFilterCriteria(string fieldName, ComparisonType comparisonType, int compareTo)
		{
			AddFilterCriteria(fieldName, comparisonType, compareTo.ToString());
		}

		/// <summary>
		/// Adds a float type filter. 
		/// </summary>
		/// <param name="fieldName">The field name for which a filter is added.</param>
		/// <param name="comparisonType">The type of comparison to use in the filter.</param>
		/// <param name="compareTo">The value to compare to.</param>
		public void AddFilterCriteria(string fieldName, ComparisonType comparisonType, float compareTo)
		{
			AddFilterCriteria(fieldName, comparisonType, compareTo.ToString());
		}

		/// <summary>
		/// Adds a DateTime type filter. 
		/// </summary>
		/// <param name="fieldName">The field name for which a filter is added.</param>
		/// <param name="comparisonType">The type of comparison to use in the filter.</param>
		/// <param name="compareTo">The value to compare to.</param>
		public void AddFilterCriteria(string fieldName, ComparisonType compare, DateTime compareTo)
		{
			int numericalDate = (compareTo - _zeroDate).Days;

			AddFilterCriteria(fieldName, compare, numericalDate.ToString());
		}

		/// <summary>
		/// Adds a string type filter. 
		/// </summary>
		/// <param name="fieldName">The field name for which a filter is added.</param>
		/// <param name="comparisonType">The type of comparison to use in the filter.</param>
		/// <param name="compareTo">The value to compare to.</param>
		public void AddFilterCriteria(string fieldName, ComparisonType comparisonType, string compareTo)
		{
			Definition databaseDefinition = FindFieldDefinition(fieldName);

			if (databaseDefinition == null)
			{
				throw new MissingFieldException();
			}

			if (databaseDefinition.FieldType == AVImarkDataType.AVImarkDouble || databaseDefinition.FieldType == AVImarkDataType.AVImarkBool || databaseDefinition.FieldType == AVImarkDataType.AVImarkBit) 
			{
				throw new Exception(databaseDefinition.FieldType.ToString() + " cannot be added as a filter.");
			}

			if (databaseDefinition.FieldType == AVImarkDataType.AVImarkDynamicString || databaseDefinition.FieldType == AVImarkDataType.AVImarkLinkToPhrase || databaseDefinition.FieldType == AVImarkDataType.AVImarkLinkToWp)
			{
				if (comparisonType != ComparisonType.Contains && comparisonType != ComparisonType.EqualTo && comparisonType != ComparisonType.NotEqualTo)
				{
					throw new Exception(databaseDefinition.FieldType.ToString() + " cannot use the comparison type " + comparisonType.ToString() + ".");
				}
			}
			else
			{
				if (comparisonType == ComparisonType.Contains)
				{
					throw new Exception(databaseDefinition.FieldType.ToString() + " cannot use the comparison type " + comparisonType.ToString() + ".");
				}
			}

			if (databaseDefinition.FieldType == AVImarkDataType.AVImarkSingle)
			{
				if (databaseDefinition.Multiplier != 0)
					compareTo = (Convert.ToDecimal(compareTo) / databaseDefinition.Multiplier).ToString();
			}
			else if (databaseDefinition.FieldType == AVImarkDataType.AVImarkImpliedDecimal || databaseDefinition.FieldType == AVImarkDataType.AVImarkSignedImpliedDecimal)
			{
				compareTo = (Convert.ToDecimal(compareTo) * 100).ToString("0");
			}
			else if (databaseDefinition.FieldType == AVImarkDataType.AVImarkImpliedDecimal2)
			{
				compareTo = (Convert.ToDecimal(compareTo) * 1000).ToString("0");
			}
			else if (databaseDefinition.FieldType == AVImarkDataType.AVImarkTime)
			{
				compareTo = StringFromTime(compareTo);
			}

            FilterClause filterClause = new FilterClause();
            filterClause.FieldOrdinal = databaseDefinition.FieldOrdinal;
            filterClause.Comparison = comparisonType;
            filterClause.CompareTo = compareTo;
            _filterClause.Add(filterClause);
        }

		/// <summary>
		/// Returns the number of fields in the current table.
		/// </summary>
		/// <returns></returns>
		public int FieldCount()
		{
			return DatabaseDefinition.Count();
		}

		/// <summary>
		/// Returns the field name based on its ordinal position.
		/// </summary>
		/// <param name="ordinalPosition">The ordinal position for the field.</param>
		/// <returns></returns>
		public string FieldName(int ordinalPosition)
		{
			if (ordinalPosition < DatabaseDefinition.Count)
				return DatabaseDefinition[ordinalPosition].FieldName;
			else
				return "";
		}

		/// <summary>
		/// Returns the field type that CarsonLib.DLL will understand based on the field type.
		/// </summary>
		/// <param name="fieldType">The AVImark field type.</param>
		/// <returns></returns>
		protected int InternalRecordType(AVImarkDataType fieldType)
		{
			switch (fieldType)
			{
				case AVImarkDataType.AVImarkWord:
				case AVImarkDataType.AVImarkTime:
					return 0;

				case AVImarkDataType.AVImarkDate:
					return 1;

				case AVImarkDataType.AVImarkByte:
				case AVImarkDataType.AVImarkCharacter:
					return 2;

				case AVImarkDataType.AVImarkSignedImpliedDecimal:
					return 3;

				case AVImarkDataType.AVImarkImpliedDecimal:
				case AVImarkDataType.AVImarkImpliedDecimal2:
					return 4;

				case AVImarkDataType.AVImarkInteger:
					return 5;

				case AVImarkDataType.AVImarkDoubleWord:
					return 6;

				case AVImarkDataType.AVImarkLongInteger:
					return 7;

				case AVImarkDataType.AVImarkDynamicString:
					return 8;

				case AVImarkDataType.AVImarkSingle:
					return 9;

				case AVImarkDataType.AVImarkLinkToPhrase:
					return 10;

				case AVImarkDataType.AVImarkLinkToWp:
					return 11;

				case AVImarkDataType.AVImarkAutoNumber:
					return 12;

				default:
					return -1;
			}
		}

		/// <summary>
		/// Determines if the current record is the last record in the record set.
		/// </summary>
		/// <returns></returns>
		public bool IsLastRecord()
		{
			return (_currentRecord == _lastRecord);
		}

		/// <summary>
		/// Determines if the current record is the first record in the record set.
		/// </summary>
		/// <returns></returns>
		public bool IsFirstRecord()
		{
			return (_currentRecord == 1);
		}

		/// <summary>
		/// Determines if the Phrase table is used in the current table.
		/// </summary>
		/// <returns></returns>
		private bool PhraseLinkExists()
		{
			if (this.ForcePhraseOpen)
				return true;

			foreach (var databaseDefinition in DatabaseDefinition)
			{
				if (databaseDefinition.FieldType == AVImarkDataType.AVImarkLinkToPhrase)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Determines if the Wp table is used in the current table.
		/// </summary>
		/// <returns></returns>
		private bool WpLinkExists()
		{
			if (this.ForceWpOpen)
				return true;

			foreach (var databaseDefinition in DatabaseDefinition)
			{
				if (databaseDefinition.FieldType == AVImarkDataType.AVImarkLinkToWp)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Determines if the automati RECD filter should be used and adds the filter automatically.
		/// </summary>
		/// <returns></returns>
		private void AddRecdFilter()
		{
			var def = (from d in DatabaseDefinition
					  where d.FieldName.Contains("RECD")
					  select d).ToList();

			if (def.Count() > 0)
			{
				Enum fieldOrdinal = def[0].FieldOrdinal;

				int count = (from w in _filterClause
							 where w.FieldOrdinal == fieldOrdinal
							 select w).Count();

				if (count == 0)
				{
					AddFilterCriteria(def[0].FieldName, ComparisonType.NotEqualTo, "D");
				}
			}
		}

		/// <summary>
		/// Retrieves all records based on the currently set filters.
		/// </summary>
		/// <returns>Number of records fetched.</returns>
		public int RetrieveRecords()
		{
			return RetrieveRecords(false);
		}

		/// <summary>
		/// Retrieves all records based on the currently set filters.
		/// </summary>
		/// <param name="skipActiveRecordCheck">Bypass the automatic filter that pulls only active records.</param>
		/// <returns></returns>
		public int RetrieveRecords(bool skipActiveRecordCheck)
		{
			List<IntPtr> recordPtr = new List<IntPtr>();

			if (!skipActiveRecordCheck)
			{
				if (CarsonStaticSettings.FilterActiveRecords)
				{
					AddRecdFilter();
				}
			}

			if (this.PhraseLinkExists())
			{
				string fileName = "";

				if (File.Exists(Path.Combine(this.DatabasePath, "PHRASE.V2$")))
				{
					fileName = Path.Combine(this.DatabasePath, "PHRASE.V2$");
				}
				else if (File.Exists(Path.Combine(this.DatabasePath, "PHRASE.VM$")))
				{
					fileName = Path.Combine(this.DatabasePath, "PHRASE.VM$");
				}

				_phraseStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				_carsonLib.DatabaseHandles.Phrase = _phraseStream.SafeFileHandle;
			}

			if (this.WpLinkExists())
			{
				string fileName = "";

				if (File.Exists(Path.Combine(this.DatabasePath, "WP.V2$")))
				{
					fileName = Path.Combine(this.DatabasePath, "WP.V2$");
				}
				else if (File.Exists(Path.Combine(this.DatabasePath, "WP.VM$")))
				{
					fileName = Path.Combine(this.DatabasePath, "WP.VM$");
				}

				_wpStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				_carsonLib.DatabaseHandles.Wp = _wpStream.SafeFileHandle;
			}

			for (int x = 0; x < _filterClause.Count; x++)
			{
				Definition databaseDefinition = LoadDatabaseDefinition(_filterClause[x].FieldOrdinal);

				switch (databaseDefinition.FieldType)
				{
					case AVImarkDataType.AVImarkDate:
					case AVImarkDataType.AVImarkWord:
					case AVImarkDataType.AVImarkTime:
						FilterWord(x, ref recordPtr, databaseDefinition);
						break;

					case AVImarkDataType.AVImarkByte:
						FilterByte(x, ref recordPtr, databaseDefinition);
						break;

					case AVImarkDataType.AVImarkCharacter:
						FilterCharacter(x, ref recordPtr, databaseDefinition);
						break;

					case AVImarkDataType.AVImarkImpliedDecimal:
					case AVImarkDataType.AVImarkImpliedDecimal2:
						FilterImpliedDecimal(x, ref recordPtr, databaseDefinition);
						break;

					case AVImarkDataType.AVImarkSignedImpliedDecimal:
					case AVImarkDataType.AVImarkLongInteger:
						FilterSignedInteger(x, ref recordPtr, databaseDefinition);
						break;

					case AVImarkDataType.AVImarkDoubleWord:
					case AVImarkDataType.AVImarkAutoNumber:
						FilterDoubleWord(x, ref recordPtr, databaseDefinition);
						break;

					case AVImarkDataType.AVImarkDynamicString:
					case AVImarkDataType.AVImarkLinkToPhrase:
					case AVImarkDataType.AVImarkLinkToWp:
						FilterString(x, ref recordPtr, databaseDefinition);
						break;

					case AVImarkDataType.AVImarkSingle:
						FilterSingle(x, ref recordPtr, databaseDefinition);
						break;
				}

                //this block of code closes all previously opened records except for the last handle opened.  The reason is that subsequent calls to the DLL
                //pass previous records back in so that the DLL will only search those records instead of doing a full table scan.  However, only the most recent
                //call is preserved.  Prior records can be discarded.
                if (recordPtr.Count > 0)
                {
                    for (int y = 0; y < recordPtr.Count - 1; y++)
                    {
                        _carsonLib.CloseRecordHandle(recordPtr[y]);
                        recordPtr.RemoveAt(0);
                    }
                }
			}

			int recordCount;

            if (recordPtr.Count > 1)
            {
                recordCount = _carsonLib.RecordCountFromHandle(recordPtr[0]);
                int recordCount1 = _carsonLib.RecordCountFromHandle(recordPtr[1]);
                int[] result1 = new int[recordCount];
                int[] result2 = new int[recordCount1];
                Marshal.Copy(recordPtr[0], result1, 0, recordCount);
                Marshal.Copy(recordPtr[1], result2, 0, recordCount1);

                _dataRecords = new List<int>(result1);
                _dataRecords.AddRange(new List<int>(result2));
                _lastRecord = _dataRecords.Count;
                _carsonLib.CloseRecordHandle(recordPtr[0]);
                _carsonLib.CloseRecordHandle(recordPtr[1]);
                recordCount += recordCount1;
            }
			else if (recordPtr.Count > 0)
			{
				recordCount = _carsonLib.RecordCountFromHandle(recordPtr[0]);
				int[] result = new int[recordCount];
				Marshal.Copy(recordPtr[0], result, 0, recordCount);

				_dataRecords = new List<int>(result);
				_queryExecuted = true;
				_lastRecord = recordCount;
				_carsonLib.CloseRecordHandle(recordPtr[0]);
			}
			else    //no filters exist, load all records
			{
				if (_stream == null)
				{
					this.Open();
				}

				if (this.ModernRecordLength == null)
					recordCount = _carsonLib.RecordCount(_stream.SafeFileHandle, this.TableId, this.IsClassic());
				else
					recordCount = _carsonLib.RecordCount(_stream, RecordLength());

				for (int x = 1; x <= recordCount; x++)
				{
					_dataRecords.Add(x);
				}

				_queryExecuted = true;
				_lastRecord = recordCount;
			}

			//once a query is executed, we need to clear the _filterClause list so that the previous filters aren't included in subsequent runs
			_filterClause.Clear();

			if (recordCount > 0)
				_currentRecord = 1;
			else
				_currentRecord = -1;

			return recordCount;
		}

		private void FilterWord(int item, ref List<IntPtr> recordPtr, Definition databaseDefinition)
		{
			ushort word1 = ushort.MinValue;
			ushort word2 = ushort.MaxValue;
			bool isNot = false;

			if (_filterClause[item].Comparison == ComparisonType.EqualTo)
			{
				word1 = Convert.ToUInt16(_filterClause[item].CompareTo);
				word2 = Convert.ToUInt16(_filterClause[item].CompareTo);
			}
			else if (_filterClause[item].Comparison == ComparisonType.NotEqualTo)
			{
				word1 = Convert.ToUInt16(_filterClause[item].CompareTo);
				word2 = Convert.ToUInt16(_filterClause[item].CompareTo);
				isNot = true;
			}
			else if (_filterClause[item].Comparison == ComparisonType.LessThan)
			{
				word2 = Convert.ToUInt16(_filterClause[item].CompareTo);
				word2--;
			}
			else if (_filterClause[item].Comparison == ComparisonType.LessThanEqual)
			{
				word2 = Convert.ToUInt16(_filterClause[item].CompareTo);
			}
			else if (_filterClause[item].Comparison == ComparisonType.GreaterThan)
			{
				word1 = Convert.ToUInt16(_filterClause[item].CompareTo);
				word1++;
			}
			else if (_filterClause[item].Comparison == ComparisonType.GreaterThanEqual)
			{
				word1 = Convert.ToUInt16(_filterClause[item].CompareTo);
			}

			if (this.TableId == TableInstance.MiscDirect)
				recordPtr.Add(_carsonLib.RecordSearchWord(_stream.SafeFileHandle, InternalRecordType(databaseDefinition.FieldType), recordPtr.Count == 0 ? IntPtr.Zero : recordPtr[recordPtr.Count - 1], 0, word1, word2, this.TableId, FieldOffset(databaseDefinition.FieldOrdinal), RecordLength(), isNot));
			else
				recordPtr.Add(_carsonLib.RecordSearchWord(_stream.SafeFileHandle, InternalRecordType(databaseDefinition.FieldType), recordPtr.Count == 0 ? IntPtr.Zero : recordPtr[recordPtr.Count - 1], 0, word1, word2, this.TableId, _filterClause[item].FieldOrdinal, _isClassic, isNot));
		}

		private void FilterByte(int item, ref List<IntPtr> recordPtr, Definition databaseDefinition)
		{
			byte byte1 = byte.MinValue;
			byte byte2 = byte.MaxValue;
			bool isNot = false;

			if (_filterClause[item].Comparison == ComparisonType.EqualTo)
			{
				byte1 = StringToByte(_filterClause[item].CompareTo);
				byte2 = StringToByte(_filterClause[item].CompareTo);
			}
			else if (_filterClause[item].Comparison == ComparisonType.NotEqualTo)
			{
				byte1 = StringToByte(_filterClause[item].CompareTo);
				byte2 = StringToByte(_filterClause[item].CompareTo);
				isNot = true;
			}
			else if (_filterClause[item].Comparison == ComparisonType.LessThan)
			{
				byte2 = StringToByte(_filterClause[item].CompareTo);
				byte2--;
			}
			else if (_filterClause[item].Comparison == ComparisonType.LessThanEqual)
			{
				byte2 = StringToByte(_filterClause[item].CompareTo);
			}
			else if (_filterClause[item].Comparison == ComparisonType.GreaterThan)
			{
				byte1 = StringToByte(_filterClause[item].CompareTo);
				byte1++;
			}
			else if (_filterClause[item].Comparison == ComparisonType.GreaterThanEqual)
			{
				byte1 = StringToByte(_filterClause[item].CompareTo);
			}

			if (this.TableId == TableInstance.MiscDirect)
				recordPtr.Add(_carsonLib.RecordSearchByte(_stream.SafeFileHandle, InternalRecordType(databaseDefinition.FieldType), recordPtr.Count == 0 ? IntPtr.Zero : recordPtr[recordPtr.Count - 1], 0, byte1, byte2, this.TableId, FieldOffset(databaseDefinition.FieldOrdinal), RecordLength(), isNot));
			else
				recordPtr.Add(_carsonLib.RecordSearchByte(_stream.SafeFileHandle, InternalRecordType(databaseDefinition.FieldType), recordPtr.Count == 0 ? IntPtr.Zero : recordPtr[recordPtr.Count - 1], 0, byte1, byte2, this.TableId, _filterClause[item].FieldOrdinal, _isClassic, isNot));
		}

		private void FilterCharacter(int item, ref List<IntPtr> recordPtr, Definition databaseDefinition)
		{
			byte byte1 = byte.MinValue;
			byte byte2 = byte.MaxValue;
			bool isNot = false;

			if (_filterClause[item].Comparison == ComparisonType.EqualTo)
			{
				byte1 = StringToChar(_filterClause[item].CompareTo);
				byte2 = StringToChar(_filterClause[item].CompareTo);
			}
			else if (_filterClause[item].Comparison == ComparisonType.NotEqualTo)
			{
				byte1 = StringToChar(_filterClause[item].CompareTo);
				byte2 = StringToChar(_filterClause[item].CompareTo);
				isNot = true;
			}
			else if (_filterClause[item].Comparison == ComparisonType.LessThan)
			{
				byte2 = StringToChar(_filterClause[item].CompareTo);
				byte2--;
			}
			else if (_filterClause[item].Comparison == ComparisonType.LessThanEqual)
			{
				byte2 = StringToChar(_filterClause[item].CompareTo);
			}
			else if (_filterClause[item].Comparison == ComparisonType.GreaterThan)
			{
				byte1 = StringToChar(_filterClause[item].CompareTo);
				byte1++;
			}
			else if (_filterClause[item].Comparison == ComparisonType.GreaterThanEqual)
			{
				byte1 = StringToChar(_filterClause[item].CompareTo);
			}

			if (this.TableId == TableInstance.MiscDirect)
				recordPtr.Add(_carsonLib.RecordSearchByte(_stream.SafeFileHandle, InternalRecordType(databaseDefinition.FieldType), recordPtr.Count == 0 ? IntPtr.Zero : recordPtr[recordPtr.Count - 1], 0, byte1, byte2, this.TableId, FieldOffset(databaseDefinition.FieldOrdinal), RecordLength(), isNot));
			else
				recordPtr.Add(_carsonLib.RecordSearchByte(_stream.SafeFileHandle, InternalRecordType(databaseDefinition.FieldType), recordPtr.Count == 0 ? IntPtr.Zero : recordPtr[recordPtr.Count - 1], 0, byte1, byte2, this.TableId, _filterClause[item].FieldOrdinal, _isClassic, isNot));
		}

		private void FilterImpliedDecimal(int item, ref List<IntPtr> recordPtr, Definition databaseDefinition)
		{
			int signedInt1 = 0;
			int signedInt2 = int.MaxValue;
			bool isNot = false;

			if (_filterClause[item].Comparison == ComparisonType.EqualTo)
			{
				signedInt1 = Convert.ToInt32(_filterClause[item].CompareTo);
				signedInt2 = Convert.ToInt32(_filterClause[item].CompareTo);
			}
			if (_filterClause[item].Comparison == ComparisonType.NotEqualTo)
			{
				signedInt1 = Convert.ToInt32(_filterClause[item].CompareTo);
				signedInt2 = Convert.ToInt32(_filterClause[item].CompareTo);
				isNot = true;
			}
			else if (_filterClause[item].Comparison == ComparisonType.LessThan)
			{
				signedInt2 = Convert.ToInt32(_filterClause[item].CompareTo);
				signedInt2--;
			}
			else if (_filterClause[item].Comparison == ComparisonType.LessThanEqual)
			{
				signedInt2 = Convert.ToInt32(_filterClause[item].CompareTo);
			}
			else if (_filterClause[item].Comparison == ComparisonType.GreaterThan)
			{
				signedInt1 = Convert.ToInt32(_filterClause[item].CompareTo);
				signedInt1++;
			}
			else if (_filterClause[item].Comparison == ComparisonType.GreaterThanEqual)
			{
				signedInt1 = Convert.ToInt32(_filterClause[item].CompareTo);
			}

			if (this.TableId == TableInstance.MiscDirect)
				recordPtr.Add(_carsonLib.RecordSearchInteger(_stream.SafeFileHandle, InternalRecordType(databaseDefinition.FieldType), recordPtr.Count == 0 ? IntPtr.Zero : recordPtr[recordPtr.Count - 1], 0, signedInt1, signedInt2, this.TableId, FieldOffset(databaseDefinition.FieldOrdinal), RecordLength(), isNot));
			else
				recordPtr.Add(_carsonLib.RecordSearchInteger(_stream.SafeFileHandle, InternalRecordType(databaseDefinition.FieldType), recordPtr.Count == 0 ? IntPtr.Zero : recordPtr[recordPtr.Count - 1], 0, signedInt1, signedInt2, this.TableId, _filterClause[item].FieldOrdinal, _isClassic, isNot));
		}

		private void FilterSignedInteger(int item, ref List<IntPtr> recordPtr, Definition databaseDefinition)
		{
			int signedInt1 = int.MinValue;
			int signedInt2 = int.MaxValue;
			bool isNot = false;

			if (_filterClause[item].Comparison == ComparisonType.EqualTo)
			{
				signedInt1 = Convert.ToInt32(_filterClause[item].CompareTo);
				signedInt2 = Convert.ToInt32(_filterClause[item].CompareTo);
			}
			if (_filterClause[item].Comparison == ComparisonType.NotEqualTo)
			{
				signedInt1 = Convert.ToInt32(_filterClause[item].CompareTo);
				signedInt2 = Convert.ToInt32(_filterClause[item].CompareTo);
				isNot = true;
			}
			else if (_filterClause[item].Comparison == ComparisonType.LessThan)
			{
				signedInt2 = Convert.ToInt32(_filterClause[item].CompareTo);
				signedInt2--;
			}
			else if (_filterClause[item].Comparison == ComparisonType.LessThanEqual)
			{
				signedInt2 = Convert.ToInt32(_filterClause[item].CompareTo);
			}
			else if (_filterClause[item].Comparison == ComparisonType.GreaterThan)
			{
				signedInt1 = Convert.ToInt32(_filterClause[item].CompareTo);
				signedInt1++;
			}
			else if (_filterClause[item].Comparison == ComparisonType.GreaterThanEqual)
			{
				signedInt1 = Convert.ToInt32(_filterClause[item].CompareTo);
			}

			if (this.TableId == TableInstance.MiscDirect)
				recordPtr.Add(_carsonLib.RecordSearchInteger(_stream.SafeFileHandle, InternalRecordType(databaseDefinition.FieldType), recordPtr.Count == 0 ? IntPtr.Zero : recordPtr[recordPtr.Count - 1], 0, signedInt1, signedInt2, this.TableId, FieldOffset(databaseDefinition.FieldOrdinal), RecordLength(), isNot));
			else
				recordPtr.Add(_carsonLib.RecordSearchInteger(_stream.SafeFileHandle, InternalRecordType(databaseDefinition.FieldType), recordPtr.Count == 0 ? IntPtr.Zero : recordPtr[recordPtr.Count - 1], 0, signedInt1, signedInt2, this.TableId, _filterClause[item].FieldOrdinal, _isClassic, isNot));
		}

		private void FilterDoubleWord(int item, ref List<IntPtr> recordPtr, Definition databaseDefinition)
		{
			uint unsignedInt1 = uint.MinValue;
			uint unsignedInt2 = uint.MaxValue;
			bool isNot = false;

			if (_filterClause[item].Comparison == ComparisonType.EqualTo)
			{
				unsignedInt1 = Convert.ToUInt32(_filterClause[item].CompareTo);
				unsignedInt2 = Convert.ToUInt32(_filterClause[item].CompareTo);
			}
			else if (_filterClause[item].Comparison == ComparisonType.NotEqualTo)
			{
				unsignedInt1 = Convert.ToUInt32(_filterClause[item].CompareTo);
				unsignedInt2 = Convert.ToUInt32(_filterClause[item].CompareTo);
				isNot = true;
			}
			else if (_filterClause[item].Comparison == ComparisonType.LessThan)
			{
				unsignedInt2 = Convert.ToUInt32(_filterClause[item].CompareTo);
				unsignedInt2--;
			}
			else if (_filterClause[item].Comparison == ComparisonType.LessThanEqual)
			{
				unsignedInt2 = Convert.ToUInt32(_filterClause[item].CompareTo);
			}
			else if (_filterClause[item].Comparison == ComparisonType.GreaterThan)
			{
				unsignedInt1 = Convert.ToUInt32(_filterClause[item].CompareTo);
				unsignedInt1++;
			}
			else if (_filterClause[item].Comparison == ComparisonType.GreaterThanEqual)
			{
				unsignedInt1 = Convert.ToUInt32(_filterClause[item].CompareTo);
			}

			if (this.TableId == TableInstance.MiscDirect)
				recordPtr.Add(_carsonLib.RecordSearchInteger(_stream.SafeFileHandle, InternalRecordType(databaseDefinition.FieldType), recordPtr.Count == 0 ? IntPtr.Zero : recordPtr[recordPtr.Count - 1], 0, (int)unsignedInt1, (int)unsignedInt2, this.TableId, FieldOffset(databaseDefinition.FieldOrdinal), RecordLength(), isNot));
			else
				recordPtr.Add(_carsonLib.RecordSearchInteger(_stream.SafeFileHandle, InternalRecordType(databaseDefinition.FieldType), recordPtr.Count == 0 ? IntPtr.Zero : recordPtr[recordPtr.Count - 1], 0, (int)unsignedInt1, (int)unsignedInt2, this.TableId, _filterClause[item].FieldOrdinal, _isClassic, isNot));
		}

		private void FilterSingle(int item, ref List<IntPtr> recordPtr, Definition databaseDefinition)
		{
			float float1 = float.MinValue;
			float float2 = float.MaxValue;
			bool isNot = false;

			if (_filterClause[item].Comparison == ComparisonType.EqualTo)
			{
				float1 = Convert.ToSingle(_filterClause[item].CompareTo);
				float2 = Convert.ToSingle(_filterClause[item].CompareTo);
				float1 -= 0.001f;
				float2 += 0.001f;
			}
			else if (_filterClause[item].Comparison == ComparisonType.NotEqualTo)
			{
				float1 = Convert.ToSingle(_filterClause[item].CompareTo);
				float2 = Convert.ToSingle(_filterClause[item].CompareTo);
				float1 -= 0.001f;
				float2 += 0.001f;
				isNot = true;
			}
			else if (_filterClause[item].Comparison == ComparisonType.LessThan)
			{
				float2 = Convert.ToSingle(_filterClause[item].CompareTo);
				float2 -= 0.001f;
			}
			else if (_filterClause[item].Comparison == ComparisonType.LessThanEqual)
			{
				float2 = Convert.ToSingle(_filterClause[item].CompareTo);
				float2 += 0.001f;
			}
			else if (_filterClause[item].Comparison == ComparisonType.GreaterThan)
			{
				float1 = Convert.ToSingle(_filterClause[item].CompareTo);
				float1 += 0.001f;
			}
			else if (_filterClause[item].Comparison == ComparisonType.GreaterThanEqual)
			{
				float1 = Convert.ToSingle(_filterClause[item].CompareTo);
			}

			if (this.TableId == TableInstance.MiscDirect)
				recordPtr.Add(_carsonLib.RecordSearchFloat(_stream.SafeFileHandle, InternalRecordType(databaseDefinition.FieldType), recordPtr.Count == 0 ? IntPtr.Zero : recordPtr[recordPtr.Count - 1], 0, float1, float2, this.TableId, FieldOffset(databaseDefinition.FieldOrdinal), RecordLength(), isNot));
			else
				recordPtr.Add(_carsonLib.RecordSearchFloat(_stream.SafeFileHandle, InternalRecordType(databaseDefinition.FieldType), recordPtr.Count == 0 ? IntPtr.Zero : recordPtr[recordPtr.Count - 1], 0, float1, float2, this.TableId, _filterClause[item].FieldOrdinal, _isClassic, isNot));
		}

		private void FilterString(int item, ref List<IntPtr> recordPtr, Definition databaseDefinition)
		{
			int searchType = 0;
			bool isNot = false;

			if (_filterClause[item].Comparison == ComparisonType.EqualTo)
				searchType = (int)StringComparisonType.EqualTo;
			else if (_filterClause[item].Comparison == ComparisonType.NotEqualTo)
				searchType = (int)StringComparisonType.NotEqualTo;

			if (this.TableId == TableInstance.MiscDirect)
				recordPtr.Add(_carsonLib.RecordSearchString(_stream.SafeFileHandle, InternalRecordType(databaseDefinition.FieldType), recordPtr.Count == 0 ? IntPtr.Zero : recordPtr[recordPtr.Count - 1], 0, new StringBuilder(_filterClause[item].CompareTo), searchType, this.TableId, FieldOffset(databaseDefinition.FieldOrdinal), RecordLength(), isNot));
			else
				recordPtr.Add(_carsonLib.RecordSearchString(_stream.SafeFileHandle, InternalRecordType(databaseDefinition.FieldType), recordPtr.Count == 0 ? IntPtr.Zero : recordPtr[recordPtr.Count - 1], 0, new StringBuilder(_filterClause[item].CompareTo), searchType, this.TableId, _filterClause[item].FieldOrdinal, _isClassic, isNot));
		}

		private byte StringToChar(string fieldData)
		{
			byte[] fieldBytes = System.Text.Encoding.Unicode.GetBytes(fieldData);

			if (fieldBytes.Length > 0)
			{
				return fieldBytes[0];
			}
			else
			{
				return (byte)0;
			}
		}

		private byte StringToByte(string byteData)
		{
			byte byteValue = 0;

			if (!byte.TryParse(byteData, out byteValue))
			{
				return 0;
			}
			else
			{
				return byteValue;
			}
		}

		protected void AddFieldDefinition(string fieldName, Enum fieldOrdinal, AVImarkDataType fieldType)
		{
			Definition databaseDefinition = new Definition();
			databaseDefinition.FieldName = fieldName;
			databaseDefinition.FieldOrdinal = fieldOrdinal;
			databaseDefinition.FieldType = fieldType;
			DatabaseDefinition.Add(databaseDefinition);
		}

		protected void AddFieldDefinition(string fieldName, Enum fieldOrdinal, AVImarkDataType fieldType, Decimal multiplier)
		{
			Definition databaseDefinition = new Definition();
			databaseDefinition.FieldName = fieldName;
			databaseDefinition.FieldOrdinal = fieldOrdinal;
			databaseDefinition.FieldType = fieldType;
			databaseDefinition.Multiplier = multiplier;
			DatabaseDefinition.Add(databaseDefinition);
		}

		/// <summary>
		/// Find the Field's Definition by the Field Name
		/// </summary>
		/// <param name="fieldName">The field to find the definition for.</param>
		/// <returns>Returns null if no item found</returns>
		public Definition FindFieldDefinition(string fieldName)
		{
			fieldName = fieldName.Trim().ToUpper();

			for (int x = 0; x < DatabaseDefinition.Count; x++)
			{
				if (DatabaseDefinition[x].FieldName == fieldName)
				{
					return DatabaseDefinition[x];
				}
			}

			return null;
		}

		/// <summary>
		/// Informs as to whether or not this version of AVImark uses the classic database format instead of the modern format.
		/// </summary>
		/// <returns>If it is the classic database format, return true</returns>
		public bool IsClassic()
		{
			return _isClassic;
		}

		/// <summary>
		/// Creates a CRC difference file based on a list of fields passed.
		/// </summary>
		/// <param name="fileName">The full path of the file name to create.</param>
		/// <param name="dataFields">A list of strings that contain names of the fields</param>
		public void CreateTableCRC(string fileName, List<string> dataFields)
		{
			bool alreadyOpen = true;

			if (!_isDataFileOpen)
			{
				this.Open();
				alreadyOpen = false;
			}

			CarsonCRC carsonCrc = new CarsonCRC();
			carsonCrc.CreateCRCFile(fileName, this, dataFields, this._stream, this._reader, this.TableId, false);

			if (!alreadyOpen)
			{
				//close the file if it was not already opened when we started
				this.Close();
			}
		}

		/// <summary>
		/// Finds the records changed since the last CRC file snapshot
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="fileName">The full path of the CRC file to compare against</param>
		/// <param name="dataToReturn">The type of data to return (new records, modified records, or both)</param>
		/// <returns></returns>
		public List<T> FindChangedRecords<T>(string fileName, DifferentialData dataToReturn)
		{
			List<ICrc> tableList = new List<ICrc>();
			int totalRecords;

			if (!_isDataFileOpen)
			{
				this.Open();
			}

			CarsonCRC carsonCrc = new CarsonCRC();
			List<int> changedList = null;

			if (dataToReturn == DifferentialData.NewRecords)
			{
				totalRecords = carsonCrc.FindNewRecords(fileName, this._stream, this._reader);
			}
			else
			{
				changedList = carsonCrc.FindChangedRecords(fileName, this, this._stream, out totalRecords, this.TableId);
			}

			this.RetrieveRecords(true);

			if (dataToReturn == DifferentialData.ModifiedRecords || dataToReturn == DifferentialData.NewAndModifiedRecords)
			{
				for (int x = 0; x < changedList.Count; x++)
				{
					ICrc tableData = ExtensionMethods.GetTableInstance(this.TableId, this, changedList[x]);
					tableData.RecordState = RecordStatus.Modified;
					tableList.Add(tableData);
				}
			}

			if (dataToReturn == DifferentialData.NewRecords || dataToReturn == DifferentialData.NewAndModifiedRecords)
			{
				if (totalRecords < this.RecordCount())
				{
					for (int x = totalRecords + 1; x <= this.RecordCount(); x++)
					{
						ICrc tableData = ExtensionMethods.GetTableInstance(this.TableId, this, x);
						tableData.RecordState = RecordStatus.Added;
						tableList.Add(tableData);
					}
				}
			}

			return tableList.ConvertAll(x => (T)x);
		}

		/// <summary>
		/// Automatically retrieves all record since the last snapshot.  If no prior snapshot exists, it will create one and return all records as "Added".
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>All changed and modified records since the last snapshot</returns>
		public List<T> RecordsChangedSinceLastSnapshot<T>()
		{
			return RecordsChangedSinceLastSnapshot<T>(string.Empty);
		}

		/// <summary>
		/// Automatically retrieves all record since the last snapshot based off a list of fields passed in.  If no prior snapshot exists, it will create one and return all records as "Added".
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dataFields">A list of strings that contain names of the fields</param>
		/// <returns></returns>
		public List<T> RecordsChangedSinceLastSnapshot<T>(List<string> dataFields)
		{
			return RecordsChangedSinceLastSnapshot<T>(dataFields, string.Empty);
		}

		/// <summary>
		/// Automatically retrieves all record since the last snapshot based off a list of fields passed in and with a file path override.  If no prior snapshot exists, it will create one and return all records as "Added".
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dataFields">A list of strings that contain names of the fields</param>
		/// <param name="filePath">The path of where to store the CRC files</param>
		/// <returns></returns>
		public List<T> RecordsChangedSinceLastSnapshot<T>(List<string> dataFields, string filePath)
		{
			Type parentClass = this.GetType();
			string fileName;
			CarsonCRC carsonCrc = new CarsonCRC();

			if (filePath.Length == 0)
			{
				fileName = parentClass.Name + ".crc";
			}
			else
			{
				fileName = Path.Combine(filePath, parentClass.Name + ".crc");
			}

			if (!File.Exists(fileName))
			{
				//create a header only.
				carsonCrc.CreateCRCFile(fileName, this, dataFields, this._stream, this._reader, this.TableId, true);
			}

			List<T> records = FindChangedRecords<T>(fileName, DifferentialData.NewAndModifiedRecords);

			carsonCrc.CreateCRCFile(fileName, this, dataFields, this._stream, this._reader, this.TableId, false);
			return records.ConvertAll(x => x);
		}

		/// <summary>
		/// Automatically retrieves all record since the last snapshot with a file path override.  If no prior snapshot exists, it will create one and return all records as "Added".
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filePath">The path of where to store the CRC files</param>
		/// <returns></returns>
		public List<T> RecordsChangedSinceLastSnapshot<T>(string filePath)
		{
			Type parentClass = typeof(T).DeclaringType;
			Type tableFields = (Type)parentClass.GetMember(parentClass.Name + "Fields")[0];
			string fileName;
			List<string> dataFields = GetFieldConstants(tableFields);
			CarsonCRC carsonCrc = new CarsonCRC();

			if (filePath.Length == 0)
			{
				fileName = parentClass.Name + ".crc";
			}
			else
			{
				fileName = Path.Combine(filePath, parentClass.Name + ".crc");
			}

			if (!File.Exists(fileName))
			{
				//create a header only.
				carsonCrc.CreateCRCFile(fileName, this, dataFields, this._stream, this._reader, this.TableId, true);
			}

			List<T> records = FindChangedRecords<T>(fileName, DifferentialData.NewAndModifiedRecords);

			carsonCrc.CreateCRCFile(fileName, this, dataFields, this._stream, this._reader, this.TableId, false);
			return records.ConvertAll(x => (T)x);
		}

		private List<string> GetFieldConstants(Type type)
		{
			List<string> fieldNames = new List<string>();
			FieldInfo[] fieldInfo = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

			foreach (FieldInfo f in fieldInfo)
			{
				if (f.IsLiteral && !f.IsInitOnly)
				{
					fieldNames.Add(f.GetValue(null).ToString());
				}
			}

			return fieldNames;
		}

		private bool CalculateBit(byte byteValue, int bitNumber)
		{
			return (byteValue & (1 << bitNumber)) != 0;
		}

		/// <summary>
		/// Rerieves a Database Definition based on an enumeration value.
		/// </summary>
		/// <param name="definition">The enum containing the defiition</param>
		/// <returns></returns>
		protected Definition LoadDatabaseDefinition(Enum definition)
		{
			if (this.TableId == TableInstance.MiscDirect)
			{
				for (int x = 0; x < DatabaseDefinition.Count; x++)
				{
					if (Convert.ToInt32(DatabaseDefinition[x].FieldOrdinal) == Convert.ToInt32(definition))
					{
						return DatabaseDefinition[x];
					}
				}

				return null;
			}
			else
			{
				return DatabaseDefinition[Convert.ToInt32(definition)];
			}
		} 
	}
}
