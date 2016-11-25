using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IO = System.IO;
using System.Security.Cryptography;

namespace TaxonomyLib
{
	public class File
	{
		public byte[] Hash { get; }

		public string AbsolutePath => Path.Combine(RootPath, RelativePath);

		private string relativePath;
		public string RelativePath
		{
			get
			{
				return relativePath;
			}

			private set { relativePath = PathExt.GetRelativePath(RootPath, value); }
		}

		private string RootPath { get; }

		private void UpdateHash()
		{
			using (var sha512 = new SHA512Managed())
			using (var file = IO.File.Open(AbsolutePath, IO.FileMode.Open))
			{
				var newFileHash = sha512.ComputeHash(file);
			}
		}

		public bool UpdateLocation(string newPath)
		{
			using(var sha512 = new SHA512Managed())
			using(var file = IO.File.Open(newPath, IO.FileMode.Open))
			{
				var oldFileHash = Hash;
				var newFileHash = sha512.ComputeHash(file);
				if(oldFileHash.SequenceEqual(newFileHash))
				{
					RelativePath = newPath;
					return true;
				}
			}
			return false;
		}

		public ICollection<Tag> Tags { get; }

		internal File(string rootPath, string path)
		{
			RootPath = rootPath;
			if(Path.IsPathRooted(path))
				RelativePath = path;
			else
				RelativePath = Path.Combine(rootPath, path);
			Tags = new List<Tag>();
		}
	}
}
