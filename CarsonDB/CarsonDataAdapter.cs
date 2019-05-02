using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public sealed class CarsonDataAdapter : DbDataAdapter, IDbDataAdapter, IDataAdapter, ICloneable
	{
		public override int Fill(DataSet dataset)
		{
			DateTime zeroDate = new DateTime(1900, 12, 31);
			DateTime dateToFind1 = new DateTime(1990, 1, 1);
			DateTime dateToFind2 = new DateTime(2020, 12, 31);
			int numericalDate1 = (int)((dateToFind1 - zeroDate).Days);
			int numericalDate2 = (int)((dateToFind2 - zeroDate).Days);

			if (this.SelectCommand == null || this.SelectCommand.Connection == null || this.SelectCommand.Connection.ConnectionString == "" || this.SelectCommand.CommandText == null || this.SelectCommand.CommandText == "")
			{
				throw new CarsonException("Command text not initialized.", 0x0004);
			}

			Client client = new Client(this.SelectCommand.Connection.ConnectionString);
			client.Open();

			DataTable dataTable = new DataTable("CLIENT");

			for (int x = 0; x < client.FieldCount(); x++)
			{
				dataTable.Columns.Add(new DataColumn(client.FieldName(x), typeof(string)));
			}

			client.AddFilterCriteria("CLIENT_ADDED", ComparisonType.GreaterThan, numericalDate1.ToString());
			//client.AddFilterCriteria("CLIENT_RECD", ComparisonType.EqualTo, "D");
			int recordCount = client.RetrieveRecords();
			client.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				DataRow dataRow = dataTable.NewRow();

				for (int y = 0; y < client.FieldCount(); y++)
				{
					//dataRow[client.FieldName(y)] = client.Value(client.FieldName(y));	
				}

				dataTable.Rows.Add(dataRow);
				client.MoveNext();
			}

			dataset.Tables.Add(dataTable);

			return recordCount;
		}
	}
}
