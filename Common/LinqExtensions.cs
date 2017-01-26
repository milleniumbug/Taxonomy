using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	public static class LinqExtensions
	{
		public static IEnumerable<IEnumerable<TItem>> SplitToChunks<TItem>(this IEnumerable<TItem> @this, int width)
		{
			using (var enumerator = @this.GetEnumerator())
			{
				int i = 0;
				var chunk = new List<TItem>(width);
				while (enumerator.MoveNext())
				{
					chunk.Add(enumerator.Current);
					++i;
					if (i == width)
					{
						yield return chunk;
						chunk = new List<TItem>(width);
						i = 0;
					}
				}
				if (chunk.Count != 0)
					yield return chunk;
			}
		}

		// expected usage:
		// enumerable_of_derived.Upcast<Base>().DoOperationsOnIEnumerableOfBase()
		// As opposed to .Cast<Target>(), this one will not compile if the
		// target type can't be converted in all cases
		public static IEnumerable<T> Upcast<T>(this IEnumerable<T> @this)
		{
			return @this;
		}
	}
}
