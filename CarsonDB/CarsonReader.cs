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
			Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);
			char c = ' ';

			_carsonLib.ReadFieldValueCharacter(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref c, databaseDefinition.FieldType, _isClassic);

			return c.ToString();
		}

		protected DateTime DateFieldValue(Enum fieldOrdinal, int recordNumber)
		{
			Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);
			DateTime nullDate = new DateTime(1900, 1, 1);
			ushort date = 0;

			_carsonLib.ReadFieldValueDate(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref date, databaseDefinition.FieldType, _isClassic);

			return nullDate.AddDays(date);
		}

		protected string TimeFieldValue(Enum fieldOrdinal, int recordNumber)
		{
			Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);
			DateTime nullDate = new DateTime(1900, 1, 1);
			ushort time = 0;

			_carsonLib.ReadFieldValueWord(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref time, databaseDefinition.FieldType, _isClassic);

			return nullDate.AddMinutes(time).ToString("HH:mm:ss");
		}

		protected decimal FloatFieldValue(Enum fieldOrdinal, int recordNumber)
		{
			Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);
			float f = 0;

			_carsonLib.ReadFieldValueFloat(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref f, databaseDefinition.FieldType, _isClassic);

			if (float.IsNaN(f))
				return 0;
			else
				return (Decimal)(f / 100);
		}

		protected int IntegerFieldValue(Enum fieldOrdinal, int recordNumber)
		{
			Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);
			int intValue = 0;

				if (_carsonLib.ReadFieldValueDoubleWord(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref intValue, databaseDefinition.FieldType, _isClassic) != 0)
				{
					//fieldOrdinal 0 is always the 'ID' field in any object Enum.  If this ever returns 0, the call to retrieve the data was not successful
					if (Convert.ToInt32(fieldOrdinal) == 0)
					{
						throw new RecordLockedException();
					}
				}

			return intValue;
		}

		protected decimal SignedImpliedDecimalFieldValue(Enum fieldOrdinal, int recordNumber)
		{
			Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);
			int intValue = 0;

			_carsonLib.ReadFieldValueDoubleWord(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref intValue, databaseDefinition.FieldType, _isClassic);

			return (decimal)intValue / 100;
		}

		protected decimal ImpliedDecimalFieldValue(Enum fieldOrdinal, int recordNumber)
		{
			Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);
			uint intValue = 0;

			_carsonLib.ReadFieldValueLong(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref intValue, databaseDefinition.FieldType, _isClassic);

			return (decimal)intValue / 100;
		}

		protected decimal ImpliedDecimal2FieldValue(Enum fieldOrdinal, int recordNumber)
		{
			Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);
			uint intValue = 0;

			_carsonLib.ReadFieldValueLong(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref intValue, databaseDefinition.FieldType, _isClassic);

			return (decimal)intValue / 1000;
		}

		protected bool BooleanFieldValue(Enum fieldOrdinal, int recordNumber)
		{
			Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);
			byte byteValue = 0;

			_carsonLib.ReadFieldValueByte(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref byteValue, databaseDefinition.FieldType, _isClassic);

			return Convert.ToBoolean(byteValue);
		}

		protected bool BitFieldValue(Enum fieldOrdinal, int recordNumber, int bitNumber)
		{
			Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);
			byte byteValue = 0;

			_carsonLib.ReadFieldValueByte(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref byteValue, databaseDefinition.FieldType, _isClassic);

			return CalculateBit(byteValue, bitNumber);
		}

		protected byte ByteFieldValue(Enum fieldOrdinal, int recordNumber)
		{
			Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);
			byte byteValue = 0;

			_carsonLib.ReadFieldValueByte(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], ref byteValue, databaseDefinition.FieldType, _isClassic);

			return byteValue;
		}

		protected string StringFieldValue(Enum fieldOrdinal, int recordNumber)
		{
			Definition databaseDefinition = LoadDatabaseDefinition(fieldOrdinal);
			StringBuilder sb = new StringBuilder(255);

			try
			{
				_carsonLib.ReadFieldValueString(_stream.SafeFileHandle, this.TableId, fieldOrdinal, _dataRecords[recordNumber - 1], sb, databaseDefinition.FieldType, _isClassic);

				return sb.ToString();
			}
			catch
			{
				return "";
			}
		}
	}
}
