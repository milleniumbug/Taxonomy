using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Tests
{
	[TestFixture]
	class ATest
	{
		[Test]
		public void T()
		{
			Assert.AreEqual(2+2, 4);
		}

		static void Main()
		{
			
		}
	}
}
