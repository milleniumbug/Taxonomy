using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	public abstract class Cache<TKey, TValue> : IDisposable
		where TValue : class
	{
		private readonly IDictionary<TKey, WeakReference<TValue>> cache;

		protected abstract TValue Create(TKey key);

		public abstract void Synchronize(TKey key, TValue cachedValue);

		public TValue Lookup(TKey key)
		{
			TValue value;
			WeakReference<TValue> weakRef;
			if(!(cache.TryGetValue(key, out weakRef) && weakRef.TryGetTarget(out value)))
			{
				value = Create(key);
				cache[key] = new WeakReference<TValue>(value);
			}
			return value;
			/*private TItem Lookup<TItem>(SQLiteDataReader reader, string idName, > cache,
				Func<SQLiteDataReader, TItem> factory, Func<TItem, long> idExtractor) where TItem : class
			{
				TValue item;
				WeakReference<TValue> fileReference;
				if (cache.TryGetValue((long)reader[idName], out fileReference) && fileReference.TryGetTarget(out item))
				{
					return item;
				}
				else
				{
					item = factory(reader);
					cache[idExtractor(item)] = new WeakReference<TItem>(item);
					return item;
				}
			}*/
		}

		public Cache()
		{
			cache = new Dictionary<TKey, WeakReference<TValue>>();
		}

		public void Dispose()
		{
			foreach(var kvp in cache)
			{
				TValue value;
				if(kvp.Value.TryGetTarget(out value))
				{
					Synchronize(kvp.Key, value);
					(value as IDisposable)?.Dispose();
				}
			}
		}
	}
}
