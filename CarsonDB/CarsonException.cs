using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class CarsonException : DbException
	{
		public CarsonException()
			: base() { }

		public CarsonException(string message)
			: base(message) { }

		public CarsonException(string format, params object[] args)
			: base(string.Format(format, args)) { }

		public CarsonException(string message, Exception innerException)
			: base(message, innerException) { }

		public CarsonException(string format, Exception innerException, params object[] args)
			: base(string.Format(format, args), innerException) { }

		protected CarsonException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
	}

	public class RecordLockedException : DbException
	{
		public RecordLockedException()
			: base() { }
	}
}
