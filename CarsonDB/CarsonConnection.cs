using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public sealed class CarsonConnection : DbConnection, ICloneable
	{
		private string _database = null;
		private ConnectionState _connectionState = ConnectionState.Closed;

		public CarsonConnection()
		{

		}

		public CarsonConnection(string connectionString)
		{
			this.ConnectionString = connectionString;
		}

		public override string ConnectionString
		{
			get;
			set;
		}

		public override string Database
		{
			get
			{
				return _database;
			}
		}

		public override string DataSource
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override string ServerVersion
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override ConnectionState State
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override void ChangeDatabase(string databaseName)
		{
			throw new NotImplementedException();
		}

		public object Clone()
		{
			throw new NotImplementedException();
		}

		public override void Close()
		{
			this._connectionState = ConnectionState.Closed;
		}

		public override void Open()
		{
			if (this.ConnectionString == null || this.ConnectionString.Trim() == "")
			{
				throw new CarsonException("Connection string has not been set.", 0x0001);
			}

			if (!Directory.Exists(this.ConnectionString))
			{
				throw new CarsonException("Invalid directory specified in the connection string.", 0x0002);
			}

			this._connectionState = ConnectionState.Open;
		}

		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
		{
			throw new NotImplementedException();
		}

		protected override DbCommand CreateDbCommand()
		{
			throw new NotImplementedException();
		}
	}
}
