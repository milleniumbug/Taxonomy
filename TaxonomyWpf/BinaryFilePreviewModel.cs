using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

		private static readonly List<Encoding> encodings = new List<Encoding>
		{
			Encoding.UTF8,
			Encoding.Unicode,
			Encoding.BigEndianUnicode,
			Encoding.UTF32,
			Encoding.GetEncoding("windows-1252"),
			Encoding.GetEncoding("windows-1250")
		};

		public IReadOnlyCollection<Encoding> Encodings => encodings;

		public Encoding SelectedEncoding { get; set; }

		public override void Dispose()
		{
			
		}
	}
}