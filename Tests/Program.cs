using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Common;
using NUnit.Framework;
using TaxonomyLib;

namespace Tests
{
	[TestFixture]
	class ATest
	{
		private string projectDirectory;
		private Taxonomy taxonomy;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			string executablePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
			projectDirectory = new FileInfo(executablePath).Directory.Parent.Parent.FullName;
			Directory.SetCurrentDirectory(projectDirectory);
		}

		[SetUp]
		public void SetUp()
		{
			taxonomy = Taxonomy.CreateNew(@"testdata\test.sql");
		}

		[TearDown]
		public void TearDown()
		{
			taxonomy.Dispose();
		}

		[Test]
		public void T()
		{
			var firstFile = taxonomy.GetFile(@"testdata\emptyfile.txt");
			var sameFile = taxonomy.GetFile(@"testdata\emptyfile.txt");
			Assert.AreSame(firstFile, sameFile);

			var secondFile = taxonomy.GetFile(@"testdata\😂non😂bmp😂name😂\Audio.png");
			var thirdFile =
				taxonomy.GetFile(@"testdata\zażółć gęślą jaźń\samecontent2.txt");
			var rodzaj = new Namespace("rodzaj");
			var firstTag = taxonomy.AddTag(rodzaj, new TagName("film"));
			var secondTag = taxonomy.AddTag(rodzaj, new TagName("ŚmieszneObrazki"));
			firstFile.Tags.Add(firstTag);
			thirdFile.Tags.Add(firstTag);
			secondFile.Tags.Add(secondTag);
			CollectionAssert.AreEquivalent(new[] {firstFile, thirdFile}, taxonomy.LookupFilesByTags(new[] {firstTag}).ToList());
			CollectionAssert.AreEqual(new[] { rodzaj }, taxonomy.AllNamespaces().ToList());
			CollectionAssert.AreEquivalent(new[] { firstTag, secondTag }, taxonomy.TagsInNamespace(rodzaj).ToList());
		}

		[Test]
		public void RollingWindowTestWindowLongerThanList()
		{
			var t = Enumerable.Repeat(0, 50).Select((_, index) => index).ToList();
			var actual = t.SplitToChunks(100).ToList();
			Assert.AreEqual(1, actual.Count);
			CollectionAssert.AreEqual(t, actual.First().ToList());
		}
	}
}
