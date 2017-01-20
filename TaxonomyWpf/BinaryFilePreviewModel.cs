using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;

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

		private readonly int length = 2000;
		private readonly int width = 16;

		public IReadOnlyCollection<byte> Bytes { get; }

		public string HexifiedContent
		{
			get
			{
				return string.Join("\n", Bytes.RollingWindow(width)
					.Select(line => line.ToList())
					.Select(line => BitConverter.ToString(line.ToArray()).Replace("-", " ") + " " + new string(line.Select(b => b >= 32 && b < 127 ? (char)b : '.').ToArray())));
			}
		}

		public override void Dispose()
		{
			
		}
	}
}