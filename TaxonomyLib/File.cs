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
		private byte[] hash;
		public byte[] Hash
		{
			get
			{
				return (byte[])hash.Clone();
			}
			internal set
			{
				hash = value;
			}
		}
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

		private readonly Lazy<ICollection<Tag>> tags;
		public ICollection<Tag> Tags => tags.Value;

		internal File(long id, string rootPath, string path, Lazy<ICollection<Tag>> tagCollection, byte[] hash = null)
		{
			RootPath = rootPath;
			if(Path.IsPathRooted(path))
				RelativePath = path;
			else
				RelativePath = Path.Combine(rootPath, path);
			tags = tagCollection;
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
