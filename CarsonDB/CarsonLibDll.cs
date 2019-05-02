using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using CarsonDB;

namespace CarsonDB
{
	internal static class Kernel32
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		internal extern static IntPtr LoadLibrary(string libraryName);
		[DllImport("kernel32.dll", SetLastError = true)]
		internal extern static bool FreeLibrary(IntPtr hModule);
		[DllImport("kernel32.dll", SetLastError = true)]
		internal extern static IntPtr GetProcAddress(IntPtr hModule, string procName);
	}

	public class CarsonLibDll : CarsonBackend, IDisposable
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct DatabaseHandle
		{
			public SafeFileHandle Main;
			public SafeFileHandle Phrase;
			public SafeFileHandle Wp;
		}

		public DatabaseHandle DatabaseHandles;

		private static readonly object _singleThread = new object();

		private delegate void CloseRecordHandleDelegate(IntPtr recordHandle);
		private delegate IntPtr RecordSearchIntDelegate(DatabaseHandle fileHandle, int recordType, IntPtr memoryPtr, int maximumRecords, ref int pointerValueFrom, ref int pointerValueTo, TableInstance table, int fieldOrdinal, bool isClassic, bool isNot);
		private delegate IntPtr RecordSearchStringDelegate(DatabaseHandle fileHandle, int recordType, IntPtr memoryPtr, int maximumRecords, StringBuilder pointerValue, int comparisonType, TableInstance table, int fieldOrdinal, bool isClassic, bool isNot);
		private delegate IntPtr RecordSearchFloatDelegate(DatabaseHandle fileHandle, int recordType, IntPtr memoryPtr, int maximumRecords, ref float pointerValueFrom, ref float pointerValueTo, TableInstance table, int fieldOrdinal, bool isClassic, bool isNot);
		private delegate IntPtr RecordSearchWordDelegate(DatabaseHandle fileHandle, int recordType, IntPtr memoryPtr, int maximumRecords, ref ushort pointerValueFrom, ref ushort pointerValueTo, TableInstance table, int fieldOrdinal, bool isClassic, bool isNot);
		private delegate IntPtr RecordSearchByteDelegate(DatabaseHandle fileHandle, int recordType, IntPtr memoryPtr, int maximumRecords, ref byte pointerValueFrom, ref byte pointerValueTo, TableInstance table, int fieldOrdinal, bool isClassic, bool isNot);
		private delegate int RecordCountFromHandleDelegate(IntPtr recordHandle);
		private delegate int RecordCountDelegate(SafeFileHandle fileHandle, TableInstance table, bool isClassic);

		private delegate IntPtr RecordSearchByte2Delegate(DatabaseHandle fileHandle, int recordType, IntPtr memoryPtr, int maximumRecords, ref byte pointerValueFrom, ref byte pointerValueTo, TableInstance table, int fieldOrdinal, int fieldLength, bool isNot);
		private delegate IntPtr RecordSearchFloat2Delegate(DatabaseHandle fileHandle, int recordType, IntPtr memoryPtr, int maximumRecords, ref float pointerValueFrom, ref float pointerValueTo, TableInstance table, int fieldOrdinal, int fieldLength, bool isNot);
		private delegate IntPtr RecordSearchInt2Delegate(DatabaseHandle fileHandle, int recordType, IntPtr memoryPtr, int maximumRecords, ref int pointerValueFrom, ref int pointerValueTo, TableInstance table, int fieldOrdinal, int fieldLength, bool isNot);
		private delegate IntPtr RecordSearchString2Delegate(DatabaseHandle fileHandle, int recordType, IntPtr memoryPtr, int maximumRecords, StringBuilder pointerValue, int comparisonType, TableInstance table, int fieldOrdinal, int fieldLength, bool isNot);
		private delegate IntPtr RecordSearchWord2Delegate(DatabaseHandle fileHandle, int recordType, IntPtr memoryPtr, int maximumRecords, ref ushort pointerValueFrom, ref ushort pointerValueTo, TableInstance table, int fieldOrdinal, int fieldLength, bool isNot);

		private delegate int ReadFieldValueCharacterDelegate(DatabaseHandle fileHandle, TableInstance table, int fieldOrdinal, int recordNumber, ref char dataPointer, AVImarkDataType dataType, bool isClassic);
		private delegate int ReadFieldValueStringDelegate(DatabaseHandle fileHandle, TableInstance table, int fieldOrdinal, int recordNumber, StringBuilder dataPointer, AVImarkDataType dataType, bool isClassic);
		private delegate int ReadFieldValueDoubleWordDelegate(DatabaseHandle fileHandle, TableInstance table, int fieldOrdinal, int recordNumber, ref int dataPointer, AVImarkDataType dataType, bool isClassic);
		private delegate int ReadFieldValueFloatDelegate(DatabaseHandle fileHandle, TableInstance table, int fieldOrdinal, int recordNumber, ref float dataPointer, AVImarkDataType dataType, bool isClassic);
		private delegate int ReadFieldValueDateDelegate(DatabaseHandle fileHandle, TableInstance table, int fieldOrdinal, int recordNumber, ref ushort datePointer, AVImarkDataType dataType, bool isClassic);
		private delegate int ReadFieldValueLongDelegate(DatabaseHandle fileHandle, TableInstance table, int fieldOrdinal, int recordNumber, ref UInt32 datePointer, AVImarkDataType dataType, bool isClassic);
		private delegate int ReadFieldValueWordDelegate(DatabaseHandle fileHandle, TableInstance table, int fieldOrdinal, int recordNumber, ref UInt16 datePointer, AVImarkDataType dataType, bool isClassic);
		private delegate int ReadFieldValueByteDelegate(DatabaseHandle fileHandle, TableInstance table, int fieldOrdinal, int recordNumber, ref byte bytePointer, AVImarkDataType dataType, bool isClassic);

		private delegate int ReadFieldValueCharacter2Delegate(DatabaseHandle fileHandle, TableInstance table, int fieldOrdinal, int recordNumber, ref char dataPointer, AVImarkDataType dataType, int fieldLength);
		private delegate int ReadFieldValueString2Delegate(DatabaseHandle fileHandle, TableInstance table, int fieldOrdinal, int recordNumber, StringBuilder dataPointer, AVImarkDataType dataType, int fieldLength);
		private delegate int ReadFieldValueDoubleWord2Delegate(DatabaseHandle fileHandle, TableInstance table, int fieldOrdinal, int recordNumber, ref int dataPointer, AVImarkDataType dataType, int fieldLength);
		private delegate int ReadFieldValueFloat2Delegate(DatabaseHandle fileHandle, TableInstance table, int fieldOrdinal, int recordNumber, ref float dataPointer, AVImarkDataType dataType, int fieldLength);
		private delegate int ReadFieldValueDate2Delegate(DatabaseHandle fileHandle, TableInstance table, int fieldOrdinal, int recordNumber, ref ushort datePointer, AVImarkDataType dataType, int fieldLength);
		private delegate int ReadFieldValueLong2Delegate(DatabaseHandle fileHandle, TableInstance table, int fieldOrdinal, int recordNumber, ref UInt32 datePointer, AVImarkDataType dataType, int fieldLength);
		private delegate int ReadFieldValueWord2Delegate(DatabaseHandle fileHandle, TableInstance table, int fieldOrdinal, int recordNumber, ref UInt16 datePointer, AVImarkDataType dataType, int fieldLength);
		private delegate int ReadFieldValueByte2Delegate(DatabaseHandle fileHandle, TableInstance table, int fieldOrdinal, int recordNumber, ref byte bytePointer, AVImarkDataType dataType, int fieldLength);

		private delegate IntPtr ComputeCompareCRCDelegate(SafeFileHandle fileHandleMain, SafeFileHandle fileHandleCrc, int table, ref int fieldDataPtr, int fieldDataLength, CarsonCRC.CrcFunction action, int padding, bool isClassic);

		private IntPtr _carsonLib;
		private CloseRecordHandleDelegate _closeRecordHandle;
		private RecordSearchIntDelegate _recordSearchInt;
		private RecordSearchStringDelegate _recordSearchString;
		private RecordSearchFloatDelegate _recordSearchFloat;
		private RecordSearchWordDelegate _recordSearchWord;
		private RecordSearchByteDelegate _recordSearchByte;
		private RecordCountFromHandleDelegate _recordCountFromHandle;
		private RecordCountDelegate _recordCount;

		private RecordSearchInt2Delegate _recordSearchInt2;
		private RecordSearchString2Delegate _recordSearchString2;
		private RecordSearchFloat2Delegate _recordSearchFloat2;
		private RecordSearchWord2Delegate _recordSearchWord2;
		private RecordSearchByte2Delegate _recordSearchByte2;

		private ReadFieldValueCharacterDelegate _readFieldValueCharacter;
		private ReadFieldValueStringDelegate _readFieldValueString;
		private ReadFieldValueDoubleWordDelegate _readFieldValueDoubleWord;
		private ReadFieldValueFloatDelegate _readFieldValueFloat;
		private ReadFieldValueDateDelegate _readFieldValueDate;
		private ReadFieldValueLongDelegate _readFieldValueLong;
		private ReadFieldValueWordDelegate _readFieldValueWord;
		private ReadFieldValueByteDelegate _readFieldValueByte;

		private ReadFieldValueCharacter2Delegate _readFieldValueCharacter2;
		private ReadFieldValueString2Delegate _readFieldValueString2;
		private ReadFieldValueDoubleWord2Delegate _readFieldValueDoubleWord2;
		private ReadFieldValueFloat2Delegate _readFieldValueFloat2;
		private ReadFieldValueDate2Delegate _readFieldValueDate2;
		private ReadFieldValueLong2Delegate _readFieldValueLong2;
		private ReadFieldValueWord2Delegate _readFieldValueWord2;
		private ReadFieldValueByte2Delegate _readFieldValueByte2;

		private ComputeCompareCRCDelegate _computeCompareCrc;

		public CarsonLibDll()
		{
			_carsonLib = Kernel32.LoadLibrary("CarsonLib.DLL");

			if (_carsonLib == IntPtr.Zero)
			{
				Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
			}

			//This has to be done since a null SafeFileHandle cannot be passed to a DLL.  This is a kludge workaround.
			this.DatabaseHandles.Main = new SafeFileHandle((IntPtr)0, true);
			this.DatabaseHandles.Phrase = new SafeFileHandle((IntPtr)0, true);
			this.DatabaseHandles.Wp = new SafeFileHandle((IntPtr)0, true);

			_closeRecordHandle = (CloseRecordHandleDelegate)Load<CloseRecordHandleDelegate>("CloseRecordHandle");
			_recordSearchInt = (RecordSearchIntDelegate)Load<RecordSearchIntDelegate>("RecordSearch");
			_recordSearchString = (RecordSearchStringDelegate)Load<RecordSearchStringDelegate>("RecordSearch");
			_recordSearchFloat = (RecordSearchFloatDelegate)Load<RecordSearchFloatDelegate>("RecordSearch");
			_recordSearchWord = (RecordSearchWordDelegate)Load<RecordSearchWordDelegate>("RecordSearch");
			_recordSearchByte = (RecordSearchByteDelegate)Load<RecordSearchByteDelegate>("RecordSearch");
			_recordCountFromHandle = (RecordCountFromHandleDelegate)Load<RecordCountFromHandleDelegate>("RecordCountFromHandle");
			_recordCount = (RecordCountDelegate)Load<RecordCountDelegate>("RecordCount");

			_recordSearchInt2 = (RecordSearchInt2Delegate)Load<RecordSearchInt2Delegate>("RecordSearch");
			_recordSearchString2 = (RecordSearchString2Delegate)Load<RecordSearchString2Delegate>("RecordSearch");
			_recordSearchFloat2 = (RecordSearchFloat2Delegate)Load<RecordSearchFloat2Delegate>("RecordSearch");
			_recordSearchWord2 = (RecordSearchWord2Delegate)Load<RecordSearchWord2Delegate>("RecordSearch");
			_recordSearchByte2 = (RecordSearchByte2Delegate)Load<RecordSearchByte2Delegate>("RecordSearch");

			_readFieldValueCharacter = (ReadFieldValueCharacterDelegate)Load<ReadFieldValueCharacterDelegate>("ReadFieldValue");
			_readFieldValueString = (ReadFieldValueStringDelegate)Load<ReadFieldValueStringDelegate>("ReadFieldValue");
			_readFieldValueDoubleWord = (ReadFieldValueDoubleWordDelegate)Load<ReadFieldValueDoubleWordDelegate>("ReadFieldValue");
			_readFieldValueFloat = (ReadFieldValueFloatDelegate)Load<ReadFieldValueFloatDelegate>("ReadFieldValue");
			_readFieldValueDate = (ReadFieldValueDateDelegate)Load<ReadFieldValueDateDelegate>("ReadFieldValue");
			_readFieldValueLong = (ReadFieldValueLongDelegate)Load<ReadFieldValueLongDelegate>("ReadFieldValue");
			_readFieldValueWord = (ReadFieldValueWordDelegate)Load<ReadFieldValueWordDelegate>("ReadFieldValue");
			_readFieldValueByte = (ReadFieldValueByteDelegate)Load<ReadFieldValueByteDelegate>("ReadFieldValue");

			_readFieldValueCharacter2 = (ReadFieldValueCharacter2Delegate)Load<ReadFieldValueCharacter2Delegate>("ReadFieldValue");
			_readFieldValueString2 = (ReadFieldValueString2Delegate)Load<ReadFieldValueString2Delegate>("ReadFieldValue");
			_readFieldValueDoubleWord2 = (ReadFieldValueDoubleWord2Delegate)Load<ReadFieldValueDoubleWord2Delegate>("ReadFieldValue");
			_readFieldValueFloat2 = (ReadFieldValueFloat2Delegate)Load<ReadFieldValueFloat2Delegate>("ReadFieldValue");
			_readFieldValueDate2 = (ReadFieldValueDate2Delegate)Load<ReadFieldValueDate2Delegate>("ReadFieldValue");
			_readFieldValueLong2 = (ReadFieldValueLong2Delegate)Load<ReadFieldValueLong2Delegate>("ReadFieldValue");
			_readFieldValueWord2 = (ReadFieldValueWord2Delegate)Load<ReadFieldValueWord2Delegate>("ReadFieldValue");
			_readFieldValueByte2 = (ReadFieldValueByte2Delegate)Load<ReadFieldValueByte2Delegate>("ReadFieldValue");

			_computeCompareCrc = (ComputeCompareCRCDelegate)Load<ComputeCompareCRCDelegate>("ComputeCompareCRC");
		}

		~CarsonLibDll()
		{
			Dispose(false);
		}

		private Delegate Load<T>(string functionName)
			where T : class
		{
			Debug.Assert(!string.IsNullOrEmpty(functionName));

			// load function pointer
			IntPtr functionPointer = Kernel32.GetProcAddress(_carsonLib, functionName);

			if (functionPointer == IntPtr.Zero)
			{
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			}

			// Marshal to requested delegate
			return Marshal.GetDelegateForFunctionPointer(functionPointer, typeof(T));
		}

		public new void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				_closeRecordHandle = null;
				_recordSearchInt = null;
				_recordSearchString = null;
				_recordSearchFloat = null;
				_recordSearchWord = null;
				_recordSearchByte = null;
				_recordCountFromHandle = null;
				_recordCount = null;

				_recordSearchInt2 = null;
				_recordSearchString2 = null;
				_recordSearchFloat2 = null;
				_recordSearchWord2 = null;
				_recordSearchByte2 = null;

				_readFieldValueCharacter = null;
				_readFieldValueString = null;
				_readFieldValueDoubleWord = null;
				_readFieldValueFloat = null;
				_readFieldValueDate = null;
				_readFieldValueLong = null;
				_readFieldValueWord = null;
				_readFieldValueByte = null;

				_readFieldValueCharacter2 = null;
				_readFieldValueString2 = null;
				_readFieldValueDoubleWord2 = null;
				_readFieldValueFloat2 = null;
				_readFieldValueDate2 = null;
				_readFieldValueLong2 = null;
				_readFieldValueWord2 = null;
				_readFieldValueByte2 = null;

				_computeCompareCrc = null;
			}

			if (_carsonLib != IntPtr.Zero)
			{
				if (!Kernel32.FreeLibrary(_carsonLib))
				{
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}

				_carsonLib = IntPtr.Zero;
			}
		}

		public void CloseRecordHandle(IntPtr recordHandle)
		{
			_closeRecordHandle(recordHandle);
		}

		public IntPtr RecordSearchInteger(SafeFileHandle fileHandle, int recordType, IntPtr memoryPtr, int maximumRecords, int pointerValueFrom, int pointerValueTo, TableInstance table, Enum fieldOrdinal, bool isClassic, bool isNot)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _recordSearchInt(DatabaseHandles, recordType, memoryPtr, maximumRecords, ref pointerValueFrom, ref pointerValueTo, table, Convert.ToInt32(fieldOrdinal), isClassic, isNot);
			}
		}

		public IntPtr RecordSearchInteger(SafeFileHandle fileHandle, int recordType, IntPtr memoryPtr, int maximumRecords, int pointerValueFrom, int pointerValueTo, TableInstance table, int? fieldOffset, int fieldLength, bool isNot)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _recordSearchInt2(DatabaseHandles, recordType, memoryPtr, maximumRecords, ref pointerValueFrom, ref pointerValueTo, table, (int)fieldOffset, fieldLength, isNot);
			}
		}

		public IntPtr RecordSearchString(SafeFileHandle fileHandle, int recordType, IntPtr memoryPtr, int maximumRecords, StringBuilder pointerValue, int comparisonType, TableInstance table, Enum fieldOrdinal, bool isClassic, bool isNot)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _recordSearchString(DatabaseHandles, recordType, memoryPtr, maximumRecords, pointerValue, comparisonType, table, Convert.ToInt32(fieldOrdinal), isClassic, isNot);
			}
		}

		public IntPtr RecordSearchString(SafeFileHandle fileHandle, int recordType, IntPtr memoryPtr, int maximumRecords, StringBuilder pointerValue, int comparisonType, TableInstance table, int? fieldOffset, int fieldLength, bool isNot)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _recordSearchString2(DatabaseHandles, recordType, memoryPtr, maximumRecords, pointerValue, comparisonType, table, (int)fieldOffset, fieldLength, isNot);
			}
		}

		public IntPtr RecordSearchFloat(SafeFileHandle fileHandle, int recordType, IntPtr memoryPtr, int maximumRecords, float pointerValueFrom, float pointerValueTo, TableInstance table, Enum fieldOrdinal, bool isClassic, bool isNot)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _recordSearchFloat(DatabaseHandles, recordType, memoryPtr, maximumRecords, ref pointerValueFrom, ref pointerValueTo, table, Convert.ToInt32(fieldOrdinal), isClassic, isNot);
			}
		}

		public IntPtr RecordSearchFloat(SafeFileHandle fileHandle, int recordType, IntPtr memoryPtr, int maximumRecords, float pointerValueFrom, float pointerValueTo, TableInstance table, int? fieldOffset, int fieldLength, bool isNot)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _recordSearchFloat2(DatabaseHandles, recordType, memoryPtr, maximumRecords, ref pointerValueFrom, ref pointerValueTo, table, (int)fieldOffset, fieldLength, isNot);
			}
		}

		public IntPtr RecordSearchWord(SafeFileHandle fileHandle, int recordType, IntPtr memoryPtr, int maximumRecords, ushort pointerValueFrom, ushort pointerValueTo, TableInstance table, Enum fieldOrdinal, bool isClassic, bool isNot)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _recordSearchWord(DatabaseHandles, recordType, memoryPtr, maximumRecords, ref pointerValueFrom, ref pointerValueTo, table, Convert.ToInt32(fieldOrdinal), isClassic, isNot);
			}
		}

		public IntPtr RecordSearchWord(SafeFileHandle fileHandle, int recordType, IntPtr memoryPtr, int maximumRecords, ushort pointerValueFrom, ushort pointerValueTo, TableInstance table, int? fieldOffset, int fieldLength, bool isNot)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _recordSearchWord2(DatabaseHandles, recordType, memoryPtr, maximumRecords, ref pointerValueFrom, ref pointerValueTo, table, (int)fieldOffset, fieldLength, isNot);
			}
		}

		public IntPtr RecordSearchByte(SafeFileHandle fileHandle, int recordType, IntPtr memoryPtr, int maximumRecords, byte pointerValueFrom, byte pointerValueTo, TableInstance table, Enum fieldOrdinal, bool isClassic, bool isNot)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _recordSearchByte(DatabaseHandles, recordType, memoryPtr, maximumRecords, ref pointerValueFrom, ref pointerValueTo, table, Convert.ToInt32(fieldOrdinal), isClassic, isNot);
			}
		}

		public IntPtr RecordSearchByte(SafeFileHandle fileHandle, int recordType, IntPtr memoryPtr, int maximumRecords, byte pointerValueFrom, byte pointerValueTo, TableInstance table, int? fieldOffset, int fieldLength, bool isNot)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _recordSearchByte2(DatabaseHandles, recordType, memoryPtr, maximumRecords, ref pointerValueFrom, ref pointerValueTo, table, (int)fieldOffset, fieldLength, isNot);
			}
		}

		public int RecordCountFromHandle(IntPtr recordHandle)
		{
			lock (_singleThread)
			{
				return _recordCountFromHandle(recordHandle);
			}
		}

		public int RecordCount(SafeFileHandle fileHandle, TableInstance table, bool isClassic)
		{
			lock (_singleThread)
			{
				return _recordCount(fileHandle, table, isClassic);
			}
		}

		public int RecordCount(FileStream fileStream, int? recordLength)
		{
			lock (_singleThread)
			{
				return ((int)fileStream.Length / (int)recordLength);
			}
		}

		public int ReadFieldValueCharacter(SafeFileHandle fileHandle, TableInstance table, Enum fieldOrdinal, int recordNumber, ref char dataPointer, AVImarkDataType dataType, bool isClassic)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _readFieldValueCharacter(DatabaseHandles, table, Convert.ToInt32(fieldOrdinal), recordNumber, ref dataPointer, dataType, isClassic);
			}
		}

		public int ReadFieldValueCharacter(SafeFileHandle fileHandle, TableInstance table, int fieldOrdinal, int recordNumber, ref char dataPointer, AVImarkDataType dataType, int? fieldLength)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _readFieldValueCharacter2(DatabaseHandles, table, fieldOrdinal, recordNumber, ref dataPointer, dataType, (int)fieldLength);
			}
		}

		public int ReadFieldValueString(SafeFileHandle fileHandle, TableInstance table, Enum fieldOrdinal, int recordNumber, StringBuilder dataPointer, AVImarkDataType dataType, bool isClassic)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _readFieldValueString(DatabaseHandles, table, Convert.ToInt32(fieldOrdinal), recordNumber, dataPointer, dataType, isClassic);
			}
		}

		public int ReadFieldValueString(SafeFileHandle fileHandle, TableInstance table, int fieldOrdinal, int recordNumber, StringBuilder dataPointer, AVImarkDataType dataType, int? fieldLength)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _readFieldValueString2(DatabaseHandles, table, fieldOrdinal, recordNumber, dataPointer, dataType, (int)fieldLength);
			}
		}

		public int ReadFieldValueDoubleWord(SafeFileHandle fileHandle, TableInstance table, Enum fieldOrdinal, int recordNumber, ref int dataPointer, AVImarkDataType dataType, bool isClassic)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _readFieldValueDoubleWord(DatabaseHandles, table, Convert.ToInt32(fieldOrdinal), recordNumber, ref dataPointer, dataType, isClassic);
			}
		}

		public int ReadFieldValueDoubleWord(SafeFileHandle fileHandle, TableInstance table, int fieldOrdinal, int recordNumber, ref int dataPointer, AVImarkDataType dataType, int? fieldLength)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _readFieldValueDoubleWord2(DatabaseHandles, table, Convert.ToInt32(fieldOrdinal), recordNumber, ref dataPointer, dataType, (int)fieldLength);
			}
		}

		public int ReadFieldValueFloat(SafeFileHandle fileHandle, TableInstance table, Enum fieldOrdinal, int recordNumber, ref float dataPointer, AVImarkDataType dataType, bool isClassic)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _readFieldValueFloat(DatabaseHandles, table, Convert.ToInt32(fieldOrdinal), recordNumber, ref dataPointer, dataType, isClassic);
			}
		}

		public int ReadFieldValueFloat(SafeFileHandle fileHandle, TableInstance table, int fieldOrdinal, int recordNumber, ref float dataPointer, AVImarkDataType dataType, int? fieldLength)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _readFieldValueFloat2(DatabaseHandles, table, fieldOrdinal, recordNumber, ref dataPointer, dataType, (int)fieldLength);
			}
		}

		public int ReadFieldValueDate(SafeFileHandle fileHandle, TableInstance table, Enum fieldOrdinal, int recordNumber, ref ushort dataPointer, AVImarkDataType dataType, bool isClassic)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _readFieldValueDate(DatabaseHandles, table, Convert.ToInt32(fieldOrdinal), recordNumber, ref dataPointer, dataType, isClassic);
			}
		}

		public int ReadFieldValueDate(SafeFileHandle fileHandle, TableInstance table, int fieldOrdinal, int recordNumber, ref ushort dataPointer, AVImarkDataType dataType, int? fieldLength)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _readFieldValueDate2(DatabaseHandles, table, Convert.ToInt32(fieldOrdinal), recordNumber, ref dataPointer, dataType, (int)fieldLength);
			}
		}

		public int ReadFieldValueLong(SafeFileHandle fileHandle, TableInstance table, Enum fieldOrdinal, int recordNumber, ref uint dataPointer, AVImarkDataType dataType, bool isClassic)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _readFieldValueLong(DatabaseHandles, table, Convert.ToInt32(fieldOrdinal), recordNumber, ref dataPointer, dataType, isClassic);
			}
		}

		public int ReadFieldValueLong(SafeFileHandle fileHandle, TableInstance table, Enum fieldOrdinal, int recordNumber, ref uint dataPointer, AVImarkDataType dataType, int? fieldLength)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _readFieldValueLong2(DatabaseHandles, table, Convert.ToInt32(fieldOrdinal), recordNumber, ref dataPointer, dataType, (int)fieldLength);
			}
		}

		public int ReadFieldValueWord(SafeFileHandle fileHandle, TableInstance table, Enum fieldOrdinal, int recordNumber, ref ushort dataPointer, AVImarkDataType dataType, bool isClassic)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _readFieldValueWord(DatabaseHandles, table, Convert.ToInt32(fieldOrdinal), recordNumber, ref dataPointer, dataType, isClassic);
			}
		}

		public int ReadFieldValueWord(SafeFileHandle fileHandle, TableInstance table, int fieldOrdinal, int recordNumber, ref ushort dataPointer, AVImarkDataType dataType, int? fieldLength)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _readFieldValueWord2(DatabaseHandles, table, Convert.ToInt32(fieldOrdinal), recordNumber, ref dataPointer, dataType, (int)fieldLength);
			}
		}

		public int ReadFieldValueByte(SafeFileHandle fileHandle, TableInstance table, Enum fieldOrdinal, int recordNumber, ref byte dataPointer, AVImarkDataType dataType, bool isClassic)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _readFieldValueByte(DatabaseHandles, table, Convert.ToInt32(fieldOrdinal), recordNumber, ref dataPointer, dataType, isClassic);
			}
		}

		public int ReadFieldValueByte(SafeFileHandle fileHandle, TableInstance table, Enum fieldOrdinal, int recordNumber, ref byte dataPointer, AVImarkDataType dataType, int? fieldLength)
		{
			lock (_singleThread)
			{
				DatabaseHandles.Main = fileHandle;
				return _readFieldValueByte2(DatabaseHandles, table, Convert.ToInt32(fieldOrdinal), recordNumber, ref dataPointer, dataType, (int)fieldLength);
			}
		}

		public IntPtr ComputerCompareCrc(SafeFileHandle fileHandleMain, SafeFileHandle fileHandleCrc, int table, ref int fieldDataPtr, int fieldDataLength, CarsonCRC.CrcFunction action, int padding, bool isClassic)
		{
			lock (_singleThread)
			{
				return _computeCompareCrc(fileHandleMain, fileHandleCrc, table, ref fieldDataPtr, fieldDataLength, action, padding, isClassic);
			}
		}
	}
}

