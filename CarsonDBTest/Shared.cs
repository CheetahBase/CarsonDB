using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDBTest
{
	public static class Shared
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

		private delegate void BuildAccountRecordsDelegate();
		private delegate void BuildAnimalRecordsDelegate();
		private delegate void BuildAppointRecordsDelegate();
		private delegate void BuildAttachRecordsDelegate();
		private delegate void BuildAuditRecordsDelegate();
		private delegate void BuildClientRecordsDelegate();
		private delegate void BuildDiagnoseRecordsDelegate();
		private delegate void BuildEntryRecordsDelegate();
		private delegate void BuildFollowRecordsDelegate();
		private delegate void BuildItemRecordsDelegate();
		private delegate void BuildLabRecordsDelegate();
		private delegate void BuildPoRecordsDelegate();
		private delegate void BuildPriceRecordsDelegate();
		private delegate void BuildProblemRecordsDelegate();
		private delegate void BuildQuotailRecordsDelegate();
		private delegate void BuildQuoteRecordsDelegate();
		private delegate void BuildReminderRecordsDelegate();
		private delegate void BuildServiceRecordsDelegate();
		private delegate void BuildSplitRecordsDelegate();
		private delegate void BuildTableRecordsDelegate();
		private delegate void BuildTestRecordsDelegate();
		private delegate void BuildTreatmentRecordsDelegate();
		private delegate void BuildUserRecordsDelegate();
		private delegate void UpdateAnimalRecordsDelegate();
		private delegate void UpdateAppointmentRecordsDelegate();
		private delegate void ResetFilesDelegate();
		private static IntPtr _aviCreateLib;

		private static BuildAccountRecordsDelegate _buildAccountRecords;
		private static BuildAnimalRecordsDelegate _buildAnimalRecords;
		private static BuildAppointRecordsDelegate _buildAppointRecords;
		private static BuildAttachRecordsDelegate _buildAttachRecords;
		private static BuildAuditRecordsDelegate _buildAuditRecords;
		private static BuildClientRecordsDelegate _buildClientRecords;
		private static BuildDiagnoseRecordsDelegate _buildDiagnoseRecords;
		private static BuildEntryRecordsDelegate _buildEntryRecords;
		private static BuildFollowRecordsDelegate _buildFollowRecords;
		private static BuildItemRecordsDelegate _buildItemRecords;
		private static BuildLabRecordsDelegate _buildLabRecords;
		private static BuildPoRecordsDelegate _buildPoRecords;
		private static BuildPriceRecordsDelegate _buildPriceRecords;
		private static BuildProblemRecordsDelegate _buildProblemRecords;
		private static BuildQuotailRecordsDelegate _buildQuotailRecords;
		private static BuildQuoteRecordsDelegate _buildQuoteRecords;
		private static BuildReminderRecordsDelegate _buildReminderRecords;
		private static BuildServiceRecordsDelegate _buildServiceRecords;
		private static BuildSplitRecordsDelegate _buildSplitRecords;
		private static BuildTableRecordsDelegate _buildTableRecords;
		private static BuildTestRecordsDelegate _buildTestRecords;
		private static BuildTreatmentRecordsDelegate _buildTreatmentRecords;
		private static BuildUserRecordsDelegate _buildUserRecords;
		private static UpdateAnimalRecordsDelegate _updateAnimalRecords;
		private static UpdateAppointmentRecordsDelegate _updateAppointmentRecords;
		private static ResetFilesDelegate _resetFiles;
		private static bool _dllInitialized = false;

		private static void LoadDll()
		{
			if (!_dllInitialized)
			{
				_dllInitialized = true;
				_aviCreateLib = Kernel32.LoadLibrary("AviCreate.DLL");

				if (_aviCreateLib == IntPtr.Zero)
				{
					Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
				}

				_buildAccountRecords = (BuildAccountRecordsDelegate)Load<BuildAccountRecordsDelegate>("BuildAccountRecords");
				_buildAnimalRecords = (BuildAnimalRecordsDelegate)Load<BuildAnimalRecordsDelegate>("BuildAnimalRecords");
				_buildAppointRecords = (BuildAppointRecordsDelegate)Load<BuildAppointRecordsDelegate>("BuildAppointRecords");
				_buildAttachRecords = (BuildAttachRecordsDelegate)Load<BuildAttachRecordsDelegate>("BuildAttachRecords");
				_buildAuditRecords = (BuildAuditRecordsDelegate)Load<BuildAuditRecordsDelegate>("BuildAuditRecords");
				_buildClientRecords = (BuildClientRecordsDelegate)Load<BuildClientRecordsDelegate>("BuildClientRecords");
				_buildDiagnoseRecords = (BuildDiagnoseRecordsDelegate)Load<BuildDiagnoseRecordsDelegate>("BuildDiagnoseRecords");
				_buildEntryRecords = (BuildEntryRecordsDelegate)Load<BuildEntryRecordsDelegate>("BuildEntryRecords");
				_buildFollowRecords = (BuildFollowRecordsDelegate)Load<BuildFollowRecordsDelegate>("BuildFollowRecords");
				_buildItemRecords = (BuildItemRecordsDelegate)Load<BuildItemRecordsDelegate>("BuildItemRecords");
				_buildLabRecords = (BuildLabRecordsDelegate)Load<BuildLabRecordsDelegate>("BuildLabRecords");
				_buildPoRecords = (BuildPoRecordsDelegate)Load<BuildPoRecordsDelegate>("BuildPoRecords");
				_buildPriceRecords = (BuildPriceRecordsDelegate)Load<BuildPriceRecordsDelegate>("BuildPriceRecords");
				_buildProblemRecords = (BuildProblemRecordsDelegate)Load<BuildProblemRecordsDelegate>("BuildProblemRecords");
				_buildQuotailRecords = (BuildQuotailRecordsDelegate)Load<BuildQuotailRecordsDelegate>("BuildQuotailRecords");
				_buildQuoteRecords = (BuildQuoteRecordsDelegate)Load<BuildQuoteRecordsDelegate>("BuildQuoteRecords");
				_buildReminderRecords = (BuildReminderRecordsDelegate)Load<BuildReminderRecordsDelegate>("BuildReminderRecords");
				_buildServiceRecords = (BuildServiceRecordsDelegate)Load<BuildServiceRecordsDelegate>("BuildServiceRecords");
				_buildSplitRecords = (BuildSplitRecordsDelegate)Load<BuildSplitRecordsDelegate>("BuildSplitRecords");
				_buildTableRecords = (BuildTableRecordsDelegate)Load<BuildTableRecordsDelegate>("BuildTableRecords");
				_buildTestRecords = (BuildTestRecordsDelegate)Load<BuildTestRecordsDelegate>("BuildTestRecords");
				_buildTreatmentRecords = (BuildTreatmentRecordsDelegate)Load<BuildTreatmentRecordsDelegate>("BuildTreatmentRecords");
				_buildUserRecords = (BuildUserRecordsDelegate)Load<BuildUserRecordsDelegate>("BuildUserRecords");
				_updateAnimalRecords = (UpdateAnimalRecordsDelegate)Load<UpdateAnimalRecordsDelegate>("UpdateAnimalRecords");
				_updateAppointmentRecords = (UpdateAppointmentRecordsDelegate)Load<UpdateAppointmentRecordsDelegate>("UpdateAppointRecords");
				_resetFiles = (ResetFilesDelegate)Load<ResetFilesDelegate>("ResetFiles");

				_resetFiles();
			}
		}

		public static void UnloadDll()
		{
			if (_aviCreateLib != IntPtr.Zero)
			{
				if (!Kernel32.FreeLibrary(_aviCreateLib))
				{
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}

				_aviCreateLib = IntPtr.Zero;
				_buildAccountRecords = null;
				_buildAnimalRecords = null;
				_buildAppointRecords = null;
				_buildAttachRecords = null;
				_buildAuditRecords = null;
				_buildClientRecords = null;
				_buildDiagnoseRecords = null;
				_buildEntryRecords = null;
				_buildFollowRecords = null;
				_buildItemRecords = null;
				_buildLabRecords = null;
				_buildPoRecords = null;
				_buildPriceRecords = null;
				_buildProblemRecords = null;
				_buildQuotailRecords = null;
				_buildQuoteRecords = null;
				_buildReminderRecords = null;
				_buildServiceRecords = null;
				_buildSplitRecords = null;
				_buildTableRecords = null;
				_buildTestRecords = null;
				_buildTreatmentRecords = null;
				_buildUserRecords = null;
				_updateAnimalRecords = null;
				_updateAppointmentRecords = null;
				_resetFiles = null;
				_dllInitialized = false;
			}
		}

		private static Delegate Load<T>(string functionName)
		where T : class
		{
			// load function pointer
			IntPtr functionPointer = Kernel32.GetProcAddress(_aviCreateLib, functionName);

			if (functionPointer == IntPtr.Zero)
			{
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			}

			// Marshal to requested delegate
			return Marshal.GetDelegateForFunctionPointer(functionPointer, typeof(T));
		}

		public static bool CompareAmount(decimal value1, string value2)
		{
			decimal decimalValue2 = decimal.Parse(value2);

			if (Math.Abs(value1 - decimalValue2) <= 0.0001M)
				return true;
			else
				return false;
		}

		public static bool CompareAmount(bool value, string value2)
		{
			return value == (value2 == "1");
		}

		public static bool CompareBool(bool value, string value2)
		{
			return value == bool.Parse(value2);
		}

		public static void BuildAccountRecords()
		{
			LoadDll();
			_buildAccountRecords();
		}

		public static void BuildAnimalRecords()
		{
			LoadDll();
			_buildAnimalRecords();
		}

		public static void BuildAppointRecords()
		{
			LoadDll();
			_buildAppointRecords();
		}

		public static void BuildAttachRecords()
		{
			LoadDll();
			_buildAttachRecords();
		}

		public static void BuildAuditRecords()
		{
			LoadDll();
			_buildAuditRecords();
		}

		public static void BuildClientRecords()
		{
			LoadDll();
			_buildClientRecords();
		}

		public static void BuildDiagnoseRecords()
		{
			LoadDll();
			_buildDiagnoseRecords();
		}

		public static void BuildEntryRecords()
		{
			LoadDll();
			_buildEntryRecords();
		}

		public static void BuildFollowRecords()
		{
			LoadDll();
			_buildFollowRecords();
		}

		public static void BuildItemRecords()
		{
			LoadDll();
			_buildItemRecords();
		}

		public static void BuildLabRecords()
		{
			LoadDll();
			_buildLabRecords();
		}

		public static void BuildPoRecords()
		{
			LoadDll();
			_buildPoRecords();
		}

		public static void BuildPriceRecords()
		{
			LoadDll();
			_buildPriceRecords();
		}

		public static void BuildProblemRecords()
		{
			LoadDll();
			_buildProblemRecords();
		}

		public static void BuildQuotailRecords()
		{
			LoadDll();
			_buildQuotailRecords();
		}

		public static void BuildQuoteRecords()
		{
			LoadDll();
			_buildQuoteRecords();
		}

		public static void BuildReminderRecords()
		{
			LoadDll();
			_buildReminderRecords();
		}

		public static void BuildServiceRecords()
		{
			LoadDll();
			_buildServiceRecords();
		}

		public static void BuildSplitRecords()
		{
			LoadDll();
			_buildSplitRecords();
		}

		public static void BuildTableRecords()
		{
			LoadDll();
			_buildTableRecords();
		}

		public static void BuildTestRecords()
		{
			LoadDll();
			_buildTestRecords();
		}

		public static void BuildTreatmentRecords()
		{
			LoadDll();
			_buildTreatmentRecords();
		}

		public static void BuildUserRecords()
		{
			LoadDll();
			_buildUserRecords();
		}

		public static void UpdateAnimalFiles()
		{
			LoadDll();
			_updateAnimalRecords();
		}
		
		public static void UpdateAppointmentFiles()
		{
			LoadDll();
			_updateAppointmentRecords();
		}

		public static List<string> GetFields(Type t)
		{
			FieldInfo[] fields = t.GetFields(BindingFlags.Static | BindingFlags.Public);
			List<string> fieldList = new List<string>();

			foreach (FieldInfo fi in fields)
			{
				string fieldName = fi.GetValue(null).ToString();

				if (fieldName != "ID")
				{
					fieldList.Add(fieldName);
				}
			}

			return fieldList;
		} 
	}
}
