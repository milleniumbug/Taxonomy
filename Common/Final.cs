using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	/// <summary>
	/// This class enforces one-time only initialization.
	/// Normally you want to use `readonly` for that, but sometimes you can't
	/// since you're not setting the value from inside a constructor.
	/// </summary>
	/// <typeparam name="TItem">The type of the stored value</typeparam>
	public class Final<TItem>
	{
		private bool isSet = false;
		private TItem value;
		public TItem Value
		{
			get
			{
				if(!isSet)
					throw new ObjectNotSet();
				return value;
			}
			set
			{
				if(isSet)
					throw new ObjectAlreadySet();
				this.value = value;
				isSet = true;
			}
		}
	}

	public class ObjectAlreadySet : InvalidOperationException
	{
		
	}

	public class ObjectNotSet : InvalidOperationException
	{
		
	}
}
