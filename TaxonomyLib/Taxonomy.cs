using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxonomyLib
{
	public class Taxonomy : IDisposable
	{
		public IEnumerable<File> LookupFilesByTags(IReadOnlyCollection<Tag> tags)
		{
			return files.Where(file => tags.Any(tag => file.Tags.Contains(tag)));
		}

		public IEnumerable<File> LookupFilesByTagsAndName(IReadOnlyCollection<Tag> tags, string name)
		{
			return LookupFilesByTags(tags).Where(file => file.RelativePath.Contains(name));
		}

		public IEnumerable<Namespace> AllNamespaces()
		{
			return tags.Select(tag => tag.Namespace).Distinct();
		}

		public IEnumerable<Tag> TagsInNamespace(Namespace ns)
		{
			return tags.Where(tag => tag.Namespace == ns);
		}

		public Tag LookupTag(Namespace ns, TagName name)
		{
			return tags.First(tag => tag.Namespace == ns && tag.Name == name);
		}

		public Tag AddTag(Namespace ns, TagName name)
		{
			var tag = new Tag(ns, name);
			tags.Add(tag);
			return tag;
		}

		public File AddFile(string path)
		{
			var file = new File(RootPath, path);
			files.Add(file);
			return file;
		}

		public Taxonomy(string path) :
			this(path, FileMode.Open)
		{
			
		}

		private Taxonomy(string path, FileMode fileMode)
		{
			ManagedFile = path;
		}

		public static Taxonomy CreateNew(string path)
		{
			return new Taxonomy(path, FileMode.CreateNew);
		}

		// TODO: Replace with database connection
		private readonly ISet<Tag> tags = new HashSet<Tag>();
		private readonly ISet<File> files = new HashSet<File>();
		private string RootPath => Path.GetDirectoryName(ManagedFile);
		public string ManagedFile { get; }

		public void Dispose()
		{
			
		}
	}
}
