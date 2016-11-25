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
		public byte[] Hash { get; internal set; }
		internal long Id { get; set; }
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

		internal File(long id, string rootPath, string path, ICollection<Tag> tagCollection, byte[] hash = null)
		{
			RootPath = rootPath;
			if(Path.IsPathRooted(path))
				RelativePath = path;
			else
				RelativePath = Path.Combine(rootPath, path);
			Tags = tagCollection;
			if(hash != null)
			{
				Hash = hash;
			}
			else
			{
				using (var sha512 = new SHA512Managed())
				using (var file = IO.File.Open(AbsolutePath, IO.FileMode.Open))
				{
					Hash = sha512.ComputeHash(file);
				}
			}
		}
	}
}
