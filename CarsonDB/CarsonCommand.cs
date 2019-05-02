using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public sealed class CarsonCommand : DbCommand, ICloneable
	{
		private CommandType _commandType = new CommandType();

		public CarsonCommand(string commandText, CarsonConnection carsonConnection)
		{
			this.DbConnection = carsonConnection;
			this.CommandText = commandText;
			this.CommandType = CommandType.Text;
		}

		public override string CommandText
		{
			get;
			set;
		}

		public override int CommandTimeout
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public override CommandType CommandType
		{
			get
			{
				return _commandType;
			}

			set
			{
				if (value == CommandType.StoredProcedure)
				{
					throw new CarsonException("Stored procedures are not supported.", 0x0003);
				}

				_commandType = value;
			}

		}

		public override bool DesignTimeVisible
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public override UpdateRowSource UpdatedRowSource
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		protected override DbConnection DbConnection
		{
			get;
			set;
		}

		protected override DbParameterCollection DbParameterCollection
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		protected override DbTransaction DbTransaction
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public override void Cancel()
		{
			throw new NotImplementedException();
		}

		public object Clone()
		{
			throw new NotImplementedException();
		}

		public override int ExecuteNonQuery()
		{
			throw new NotImplementedException();
		}

		public override object ExecuteScalar()
		{
			throw new NotImplementedException();
		}

		public override void Prepare()
		{
			throw new NotImplementedException();
		}

		protected override DbParameter CreateDbParameter()
		{
			throw new NotImplementedException();
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			throw new NotImplementedException();
		}
	}
}
