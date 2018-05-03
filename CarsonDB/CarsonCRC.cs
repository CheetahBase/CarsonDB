using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace CarsonDB
{
	public class CarsonCRC
	{
		public enum CrcFunction
		{
			Create,
			Compare
		}

		public void CreateCRCFile(string fileName, CarsonBackend database, List<string> dataFields, FileStream readStream, BinaryReader reader, TableInstance tableInstance, bool headerOnly)
		{
			List<Definition> fieldDefinitions = new List<Definition>();

			for (int x = 0; x < dataFields.Count; x++)
			{
				Definition definition = database.FindFieldDefinition(dataFields[x]);

				if (definition == null)
				{
					throw new Exception("Invalid Field Name: " + dataFields[x]);
				}

				fieldDefinitions.Add(definition);
			}

			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}

			using (FileStream writeStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
			using (BinaryWriter writer = new BinaryWriter(writeStream))
			{
				writeStream.Seek(0, SeekOrigin.Begin);
				writer.Write(StringToByte("CARSONDB" + CarsonBackend.DatabaseVersion));

				List<int> fieldOffsets = new List<int>();
				string fieldDefinitionString = database.DatabaseName + "\0";

				for (int x = 0; x < fieldDefinitions.Count; x++)
				{
					fieldDefinitionString += fieldDefinitions[x].FieldName + (x < fieldDefinitions.Count - 1 ? "\0" : "");
					fieldOffsets.Add(Convert.ToInt32(fieldDefinitions[x].FieldOrdinal));
					fieldOffsets.Add(ComputeFieldSize(fieldDefinitions[x].FieldType));
					fieldOffsets.Add(fieldDefinitions[x].FieldType == AVImarkDataType.AVImarkDynamicString ? 1 : 0);
				}

				writer.Write(fieldDefinitionString);

				int[] fieldData = fieldOffsets.ToArray();

				if (!headerOnly)
				{
					using (CarsonLibDll carsonLib = new CarsonLibDll())
					{
						carsonLib.ComputerCompareCrc(readStream.SafeFileHandle, writeStream.SafeFileHandle, (int)tableInstance, ref fieldData[0], fieldDefinitions.Count, CrcFunction.Create, (int)writeStream.Position, database.IsClassic());
					}
				}
			}
		}

		public List<int> FindChangedRecords(string fileName, CarsonBackend database, FileStream readStream, out int recordsFound, TableInstance tableInstance)
		{
			using (FileStream diffStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
			using (BinaryReader diffReader = new BinaryReader(diffStream))
			{
				IntPtr recordPtr;
				List<Definition> fieldDefinitions = new List<Definition>();

				ValidateFileHeader(diffStream, diffReader);

				string fieldDefinitionString = diffReader.ReadString();
				string[] fields = fieldDefinitionString.Split('\0');

				for (int x = 1; x < fields.Length; x++)
				{
					fieldDefinitions.Add(database.FindFieldDefinition(fields[x]));
				}

				List<int> fieldOffsets = new List<int>();

				for (int x = 0; x < fieldDefinitions.Count; x++)
				{
					fieldOffsets.Add(Convert.ToInt32(fieldDefinitions[x].FieldOrdinal));
					fieldOffsets.Add(ComputeFieldSize(fieldDefinitions[x].FieldType));
					fieldOffsets.Add(fieldDefinitions[x].FieldType == AVImarkDataType.AVImarkDynamicString ? 1 : 0);
				}

				int[] fieldData = fieldOffsets.ToArray();

				using (CarsonLibDll carsonLib = new CarsonLibDll())
				{
					recordPtr = carsonLib.ComputerCompareCrc(readStream.SafeFileHandle, diffStream.SafeFileHandle, (int)tableInstance, ref fieldData[0], fieldDefinitions.Count, CrcFunction.Compare, (int)diffStream.Position, database.IsClassic());

					int recordCount = carsonLib.RecordCountFromHandle(recordPtr);
					int[] result = new int[recordCount];
					Marshal.Copy(recordPtr, result, 0, recordCount);

					//Number of records in differential file is calculated on the file length less 16 (diff file header + version) and the field definitions.
					recordsFound = ((int)diffStream.Length - 16 - fieldDefinitionString.Length) / 4;
					return new List<int>(result);
				}
			}
		}

		public void ValidateFileHeader(FileStream diffStream, BinaryReader diffReader)
		{
			diffStream.Seek(0, SeekOrigin.Begin);
			string readHeader;

			readHeader = new string(diffReader.ReadChars(16));

			if (readHeader.Substring(0, 8) != "CARSONDB")
			{
				throw new Exception("Invalid CarsonDB CRC file header.");
			}

			if (readHeader.Substring(8, 8) != CarsonBackend.DatabaseVersion)
			{
				throw new Exception("The installed CarsonDB Database version " + CarsonBackend.DatabaseVersion + " is incompatible with version " + readHeader.Substring(8, 8));
			}
		}

		public int FindNewRecords(string fileName, FileStream readStream, BinaryReader reader)
		{
			using (FileStream diffStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
			using (BinaryReader diffReader = new BinaryReader(diffStream))
			{
				ValidateFileHeader(diffStream, diffReader);
				string fieldDefinitionString = diffReader.ReadString();

				return ((int)diffStream.Length - 16 - fieldDefinitionString.Length) / 4;
			}
		}

		private byte[] StringToByte(string stringToWrite)
		{
			return Encoding.UTF8.GetBytes(stringToWrite);
		}

		private int ComputeFieldSize(AVImarkDataType dataType)
		{
			int fieldSize = 0;

			switch (dataType)
			{
				case AVImarkDataType.AVImarkCharacter:
				case AVImarkDataType.AVImarkBit:
				case AVImarkDataType.AVImarkByte:
				case AVImarkDataType.AVImarkBool:
					fieldSize = 1;
					break;

				case AVImarkDataType.AVImarkDate:
				case AVImarkDataType.AVImarkTime:
				case AVImarkDataType.AVImarkWord:
				case AVImarkDataType.AVImarkInteger:
					fieldSize = 2;
					break;

				case AVImarkDataType.AVImarkSingle:
				case AVImarkDataType.AVImarkDoubleWord:
				case AVImarkDataType.AVImarkImpliedDecimal:
				case AVImarkDataType.AVImarkImpliedDecimal2:
				case AVImarkDataType.AVImarkSignedImpliedDecimal:
				case AVImarkDataType.AVImarkLongInteger:
				case AVImarkDataType.AVImarkLinkToPhrase:
				case AVImarkDataType.AVImarkLinkToWp:
					fieldSize = 4;
					break;

				case AVImarkDataType.AVImarkDouble:
					fieldSize = 8;
					break;

				case AVImarkDataType.AVImarkDynamicString:
					fieldSize = 0;
					break;

				default:
					fieldSize = 0;
					break;
			}

			return fieldSize;
		}
	}
}
