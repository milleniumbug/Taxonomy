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
		private readonly string taxonomyRelativePath = @"testdata\test.sql";
		private string taxonomyAbsolutePath;
		private readonly string shortName = "test taxonomy";
		private Taxonomy taxonomy;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			string executablePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
			projectDirectory = new FileInfo(executablePath).Directory.Parent.Parent.FullName;
			Directory.SetCurrentDirectory(projectDirectory);
			taxonomyAbsolutePath = Path.Combine(projectDirectory, taxonomyRelativePath);
		}

		[SetUp]
		public void SetUp()
		{
			taxonomy = Taxonomy.CreateNew(taxonomyRelativePath, shortName);
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
			Assert.AreEqual(firstFile, sameFile);

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
			CollectionAssert.AreEqual(new[] {rodzaj}, taxonomy.AllNamespaces().ToList());
			CollectionAssert.AreEquivalent(new[] {firstTag, secondTag}, taxonomy.TagsInNamespace(rodzaj).ToList());
		}

		[Test]
		public void RollingWindowTestWindowLongerThanList()
		{
			var t = Enumerable.Repeat(0, 50).Select((_, index) => index).ToList();
			var actual = t.SplitToChunks(100).ToList();
			Assert.AreEqual(1, actual.Count);
			CollectionAssert.AreEqual(t, actual.First().ToList());
		}

		[Test]
		public void HashesAreImmutable()
		{
			var firstFile = taxonomy.GetFile(@"testdata\emptyfile.txt");
			var hash = firstFile.Hash;
			var original = (byte[]) hash.Clone();
			hash[0] = 12;
			var actual = firstFile.Hash;
			CollectionAssert.AreEqual(original, actual);
		}

		[Test]
		public void ShouldFailOnCreatingNewTaxonomyWhenAFileExists()
		{
			var file = new FileInfo(@"testdata/a_file");
			using(var fileStream = file.Create())
			{
				// do nothing, let the file exist
			}
			try
			{
				Assert.Throws<IOException>(() =>
				{
					using(var t = Taxonomy.CreateNew(file.FullName, "test taxonomy"))
					{

					}
				});
			}
			finally
			{
				file.Delete();
			}
		}

		[Test]
		public void ShouldFailOnOpeningNonExistingTaxonomy()
		{
			string nonexistingPath = @"testdata/does_not_exist.aaaaaa";
			System.IO.File.Delete(nonexistingPath);
			Assert.Throws<FileNotFoundException>(() =>
			{
				using(var t = Taxonomy.CreateNew(nonexistingPath, "test taxonomy"))
				{

				}
			});
		}

		[Test]
		public void TaxonomyProperties()
		{
			Assert.AreEqual(taxonomyAbsolutePath, taxonomy.ManagedFile);
			Assert.AreEqual(shortName, taxonomy.ShortName);
			Assert.AreEqual(Path.Combine(projectDirectory, @"testdata"), taxonomy.RootPath);
		}
	}
}
