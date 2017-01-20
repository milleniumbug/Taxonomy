using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class LinqExtensions
    {
	    public static IEnumerable<IEnumerable<TItem>> RollingWindow<TItem>(this IEnumerable<TItem> @this, int width)
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
    }
}
