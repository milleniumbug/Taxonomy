using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxonomyLib;

namespace TestApplication
{
	class Program
	{
		static void Main(string[] args)
		{
			using(var taxonomy = Taxonomy.CreateNew(@"F:\mietczynski_masochista\test.sql"))
			{
				var firstFile = taxonomy.AddFile(@"Masochista - niepoważna recenzja 'Gulczas, a jak myślisz... cz.1' [2_2]-nueuBBYtM9w.mp4");
				var secondFile = taxonomy.AddFile(@"F:\1319481780_cat_spinning_a_pillow.webm");
				var thirdFile =
					taxonomy.AddFile(@"Masochista - niepoważna recenzja 'Gulczas, a jak myślisz... cz.1' [1_2]-8ZGabGISdlI.mp4");
				var rodzaj = new Namespace("rodzaj");
				var firstTag = taxonomy.AddTag(rodzaj, new TagName("film"));
				var secondTag = taxonomy.AddTag(rodzaj, new TagName("ŚmieszneObrazki"));
				firstFile.Tags.Add(firstTag);
				thirdFile.Tags.Add(firstTag);
				secondFile.Tags.Add(secondTag);
				foreach(var file in taxonomy.LookupFilesByTags(new[] {firstTag}))
				{
					Console.WriteLine(file.AbsolutePath);
				}
				foreach(var ns in taxonomy.AllNamespaces())
				{
					Console.WriteLine(ns.Component);
				}
				foreach(var tags in taxonomy.TagsInNamespace(taxonomy.AllNamespaces().First()))
				{
					Console.WriteLine(tags.Name.Name);
				}
			}
		}
	}
}
