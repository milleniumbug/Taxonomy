using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TaxonomyLib;

namespace Tests
{
	[TestFixture]
	class ATest
	{
		private string projectDirectory;

		[SetUp]
		public void SetUp()
		{
			string executablePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
			projectDirectory = new FileInfo(executablePath).Directory.Parent.Parent.FullName;
		}

		[Test]
		public void T()
		{
			using (var taxonomy = Taxonomy.CreateNew(Path.Combine(projectDirectory, @"testdata\test.sql")))
			{
				var firstFile = taxonomy.GetFile(Path.Combine(projectDirectory, @"testdata\emptyfile.txt"));
				var sameFile = taxonomy.GetFile(Path.Combine(projectDirectory, @"testdata\emptyfile.txt"));
				Assert.AreSame(firstFile, sameFile);

				var secondFile = taxonomy.GetFile(Path.Combine(projectDirectory, @"testdata\😂non😂bmp😂name😂\Audio.png"));
				var thirdFile =
					taxonomy.GetFile(Path.Combine(projectDirectory, @"testdata\zażółć gęślą jaźń\samecontent2.txt"));
				var rodzaj = new Namespace("rodzaj");
				var firstTag = taxonomy.AddTag(rodzaj, new TagName("film"));
				var secondTag = taxonomy.AddTag(rodzaj, new TagName("ŚmieszneObrazki"));
				firstFile.Tags.Add(firstTag);
				thirdFile.Tags.Add(firstTag);
				secondFile.Tags.Add(secondTag);
				CollectionAssert.AreEquivalent(new[] {firstFile, thirdFile}, taxonomy.LookupFilesByTags(new[] {firstTag}));
				CollectionAssert.AreEqual(new[] { rodzaj }, taxonomy.AllNamespaces());
				CollectionAssert.AreEquivalent(new[] { firstTag, secondTag }, taxonomy.TagsInNamespace(rodzaj));
			}
		}

		static void Main()
		{
			
		}
	}
}
