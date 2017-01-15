using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TaxonomyWpf
{
	class BinaryFilePreviewModel : FilePreviewModel
	{
		public BinaryFilePreviewModel(string path)
		{
			using(var stream = new BinaryReader(File.OpenRead(path)))
			{
				Bytes = stream.ReadBytes(width * length);
			}
		}

		private static IEnumerable<IEnumerable<TItem>> RollingWindow<TItem>(IEnumerable<TItem> @this, int width)
		{
			using(var enumerator = @this.GetEnumerator())
			{
				int i = 0;
				var chunk = new List<TItem>(width);
				while(enumerator.MoveNext())
				{
					chunk.Add(enumerator.Current);
					++i;
					if(i == width)
					{
						yield return chunk;
						chunk = new List<TItem>(width);
						i = 0;
					}
				}
				if(chunk.Count != 0)
					yield return chunk;
			}
		}

		private readonly int length = 2000;
		private readonly int width = 16;

		public IReadOnlyCollection<byte> Bytes { get; }

		public string HexifiedContent
		{
			get
			{
				return string.Join("\n", RollingWindow(Bytes, width)
					.Select(line => line.ToList())
					.Select(line => BitConverter.ToString(line.ToArray()).Replace("-", " ") + " " + new string(line.Select(b => b >= 32 && b < 127 ? (char)b : '.').ToArray())));
			}
		}

		public override void Dispose()
		{
			
		}
	}
}