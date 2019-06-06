using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace CarsonDB
{
	public partial class CarsonBackend : IDisposable
	{
		protected string CharacterFieldValue(Enum fieldOrdinal, int recordNumber)
		{
			char c = ' ';

			if (this.ModernRecordLength == null)
			{
				Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);

				_carsonLib.ReadFieldValueCharacter(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref c, databaseDefinition.FieldType, _isClassic);
			}
			else
				_carsonLib.ReadFieldValueCharacter(_stream.SafeFileHandle, this.TableId, FieldOffset(fieldOrdinal), _dataRecords[recordNumber - 1], ref c, AVImarkDataType.AVImarkCharacter, RecordLength());

			return c.ToString();
		}

		protected DateTime DateFieldValue(Enum fieldOrdinal, int recordNumber)
		{
			Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);
			DateTime nullDate = new DateTime(1900, 1, 1);
			ushort date = 0;

			if (this.ModernRecordLength == null)
				_carsonLib.ReadFieldValueDate(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref date, databaseDefinition.FieldType, _isClassic);
			else
				_carsonLib.ReadFieldValueDate(_stream.SafeFileHandle, this.TableId, FieldOffset(fieldOrdinal), _dataRecords[recordNumber - 1], ref date, AVImarkDataType.AVImarkDate, RecordLength());

			return nullDate.AddDays(date);
		}

		protected string TimeFieldValue(Enum fieldOrdinal, int recordNumber)
		{
			Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);
			DateTime nullDate = new DateTime(1900, 1, 1);
			ushort time = 0;

			if (this.ModernRecordLength == null)
				_carsonLib.ReadFieldValueWord(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref time, databaseDefinition.FieldType, _isClassic);
			else
				_carsonLib.ReadFieldValueWord(_stream.SafeFileHandle, this.TableId, FieldOffset(fieldOrdinal), _dataRecords[recordNumber - 1], ref time, databaseDefinition.FieldType, RecordLength());

			return nullDate.AddMinutes(time).ToString("HH:mm:ss");
		}

		protected decimal FloatFieldValue(Enum fieldOrdinal, int recordNumber)
		{
			float f = 0;
			Decimal multiplier = 0;

			if (this.ModernRecordLength == null)
			{
				Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);

				multiplier = databaseDefinition.Multiplier;
				_carsonLib.ReadFieldValueFloat(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref f, databaseDefinition.FieldType, _isClassic);
			}
			else
				_carsonLib.ReadFieldValueFloat(_stream.SafeFileHandle, this.TableId, FieldOffset(fieldOrdinal), _dataRecords[recordNumber - 1], ref f, AVImarkDataType.AVImarkSingle, RecordLength());

			if (float.IsNaN(f))
				return 0;
			else
			{
				if (multiplier != 0)
					return (Decimal)((Decimal)f * multiplier);
				else
					return (Decimal)f;
			}
		}

		protected int IntegerFieldValue(Enum fieldOrdinal, int recordNumber)
		{
			int intValue = 0;

			if (this.ModernRecordLength == null)
			{
				Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);

				if (_carsonLib.ReadFieldValueDoubleWord(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref intValue, databaseDefinition.FieldType, _isClassic) != 0)
				{
					//fieldOrdinal 0 is always the 'ID' field in any object Enum.  If this ever returns 0, the call to retrieve the data was not successful
					if (Convert.ToInt32(fieldOrdinal) == 0)
					{
						throw new RecordLockedException();
					}
				}
			}
			else
			{
				//We need to determine if this is an autonumber if this is a manual 
				AVImarkDataType dataType = AVImarkDataType.AVImarkLongInteger;

				if (Convert.ToInt32(fieldOrdinal) == -1)
				{
					dataType = AVImarkDataType.AVImarkAutoNumber;
				}

				if (_carsonLib.ReadFieldValueDoubleWord(_stream.SafeFileHandle, this.TableId, FieldOffset(fieldOrdinal), _dataRecords[recordNumber - 1], ref intValue, dataType, RecordLength()) != 0)
				{
					//fieldOrdinal 0 is always the 'ID' field in any object Enum.  If this ever returns 0, the call to retrieve the data was not successful
					if (Convert.ToInt32(fieldOrdinal) == 0)
					{
						throw new RecordLockedException();
					}
				}
			}

			return intValue;
		}

		protected decimal SignedImpliedDecimalFieldValue(Enum fieldOrdinal, int recordNumber)
		{
			Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);
			int intValue = 0;

			if (this.ModernRecordLength == null)
				_carsonLib.ReadFieldValueDoubleWord(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref intValue, databaseDefinition.FieldType, _isClassic);
			else
				_carsonLib.ReadFieldValueDoubleWord(_stream.SafeFileHandle, this.TableId, FieldOffset(fieldOrdinal), _dataRecords[recordNumber - 1], ref intValue, databaseDefinition.FieldType, RecordLength());

			return (decimal)intValue / 100;
		}

		protected decimal ImpliedDecimalFieldValue(Enum fieldOrdinal, int recordNumber)
		{
			Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);
			uint intValue = 0;

			if (this.ModernRecordLength == null)
				_carsonLib.ReadFieldValueLong(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref intValue, databaseDefinition.FieldType, _isClassic);
			else
				_carsonLib.ReadFieldValueLong(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref intValue, databaseDefinition.FieldType, RecordLength());

			return (decimal)intValue / 100;
		}

		protected decimal ImpliedDecimal2FieldValue(Enum fieldOrdinal, int recordNumber)
		{
			Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);
			uint intValue = 0;

			if (this.ModernRecordLength == null)
				_carsonLib.ReadFieldValueLong(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref intValue, databaseDefinition.FieldType, _isClassic);
			else
				_carsonLib.ReadFieldValueLong(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref intValue, databaseDefinition.FieldType, RecordLength());

			return (decimal)intValue / 1000;
		}

		protected bool BooleanFieldValue(Enum fieldOrdinal, int recordNumber)
		{
			Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);
			byte byteValue = 0;

			if (this.ModernRecordLength == null)
				_carsonLib.ReadFieldValueByte(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref byteValue, databaseDefinition.FieldType, _isClassic);
			else
				_carsonLib.ReadFieldValueByte(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref byteValue, databaseDefinition.FieldType, RecordLength());

			return Convert.ToBoolean(byteValue);
		}

		protected bool BitFieldValue(Enum fieldOrdinal, int recordNumber, int bitNumber)
		{
			Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);
			byte byteValue = 0;

			if (this.ModernRecordLength == null)
				_carsonLib.ReadFieldValueByte(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref byteValue, databaseDefinition.FieldType, _isClassic);
			else
				_carsonLib.ReadFieldValueByte(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref byteValue, databaseDefinition.FieldType, RecordLength());

			return CalculateBit(byteValue, bitNumber);
		}

		protected byte ByteFieldValue(Enum fieldOrdinal, int recordNumber)
		{
			Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);
			byte byteValue = 0;

			if (this.ModernRecordLength == null)
				_carsonLib.ReadFieldValueByte(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref byteValue, databaseDefinition.FieldType, _isClassic);
			else
				_carsonLib.ReadFieldValueByte(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref byteValue, databaseDefinition.FieldType, RecordLength());

			return byteValue;
		}

		protected string StringFieldValue(Enum fieldOrdinal, int recordNumber, bool linkToPhrase = false, bool linkToWp = false)
		{
			StringBuilder sb = new StringBuilder(1024);

			try
			{
				if (this.ModernRecordLength == null)
				{
					Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);

					_carsonLib.ReadFieldValueString(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], sb, databaseDefinition.FieldType, _isClassic);
				}
				else
				{
					AVImarkDataType dataType = AVImarkDataType.AVImarkDynamicString;

					if (linkToPhrase)
						dataType = AVImarkDataType.AVImarkLinkToPhrase;
					else if (linkToWp)
						dataType = AVImarkDataType.AVImarkLinkToWp;

					_carsonLib.ReadFieldValueString(_stream.SafeFileHandle, this.TableId, FieldOffset(fieldOrdinal), _dataRecords[recordNumber - 1], sb, dataType, RecordLength());
				}

				return sb.ToString();
			}
			catch
			{
				return "";
			}
		}

		private int RecordLength()
		{
			if (_isClassic)
				return (int)this.ClassicRecordLength;
			else
				return (int)this.ModernRecordLength;
		}

		private int FieldOffset(Enum classicOffset)
		{
			if (_isClassic)
				return Convert.ToInt32(classicOffset);
			else
				return Convert.ToInt32(classicOffset) + 40;
		}
	}
}
