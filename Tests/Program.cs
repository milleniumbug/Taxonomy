using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TaxonomyLib;

namespace Tests
{
	[TestFixture]
	class ATest
	{
		[Test]
		public void T()
		{
			using (var taxonomy = Taxonomy.CreateNew(@"testdata\test.sql"))
			{
				var firstFile = taxonomy.GetFile(@"testdata\emptyfile.txt");
				var sameFile = taxonomy.GetFile(@"testdata\emptyfile.txt");
				Debug.Assert(firstFile == sameFile);

				var secondFile = taxonomy.GetFile(@"testdata\😂non😂bmp😂name😂\Audio.png");
				var thirdFile =
					taxonomy.GetFile(@"testdata\zażółć gęślą jaźń\samecontent2.txt");
				var rodzaj = new Namespace("rodzaj");
				var firstTag = taxonomy.AddTag(rodzaj, new TagName("film"));
				var secondTag = taxonomy.AddTag(rodzaj, new TagName("ŚmieszneObrazki"));
				firstFile.Tags.Add(firstTag);
				thirdFile.Tags.Add(firstTag);
				secondFile.Tags.Add(secondTag);
				foreach (var file in taxonomy.LookupFilesByTags(new[] { firstTag }))
				{
					Console.WriteLine(file.AbsolutePath);
				}
				foreach (var ns in taxonomy.AllNamespaces())
				{
					Console.WriteLine(ns.Component);
				}
				foreach (var tags in taxonomy.TagsInNamespace(taxonomy.AllNamespaces().First()))
				{
					Console.WriteLine(tags.Name.Name);
				}
			}
		}

		static void Main()
		{
			
		}
	}
}
