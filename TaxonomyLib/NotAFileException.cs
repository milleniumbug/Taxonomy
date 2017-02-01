using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TaxonomyLib
{
	public class NotAFileException : IOException
	{
		public NotAFileException()
		{

		}

		public NotAFileException(string message) :
			base(message)
		{

		}

		public NotAFileException(string message, int hresult) :
			base(message, hresult)
		{

		}

		public NotAFileException(string message, Exception innerException) :
			base(message, innerException)
		{

		}

		protected NotAFileException(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}
	}
}
