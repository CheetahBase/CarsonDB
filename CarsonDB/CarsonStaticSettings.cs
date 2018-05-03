using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public static class CarsonStaticSettings
	{
		public static string DatabasePath
		{
			get; set;
		}

		public static bool FilterActiveRecords
		{
			get; set;
		} = true;
	}
}
