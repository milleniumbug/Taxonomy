using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Common;

namespace TaxonomyLib
{
	public class Taxonomy : IDisposable
	{
		public IEnumerable<File> LookupFilesByTags(IReadOnlyCollection<Tag> tags)
		{
			return LookupFiles(FileLookupQueryGenerate(false, tags.Count, tags, null));
		}

		private TItem Lookup<TItem>(SQLiteDataReader reader, string idName, Dictionary<long, WeakReference<TItem>> cache,
			Func<SQLiteDataReader, TItem> factory, Func<TItem, long> idExtractor) where TItem : class
		{
			TItem item;
			WeakReference<TItem> fileReference;
			if (cache.TryGetValue((long)reader[idName], out fileReference) && fileReference.TryGetTarget(out item))
			{
				return item;
			}
			else
			{
				item = factory(reader);
				cache[idExtractor(item)] = new WeakReference<TItem>(item);
				return item;
			}
		}

		private IEnumerable<File> LookupFiles(SQLCommand command)
		{
			using(var cmd = command)
			{
				using(var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						yield return
							Lookup(reader, "fileId", fileCache,
								dataReader =>
								{
									long fileId = (long) dataReader["fileId"];
									var tagCollection = new Lazy<ICollection<Tag>>(() =>
									{
										var c = new ObservableSet<Tag>();
										foreach(var tag in LookupTagsForFileId(fileId))
										{
											c.Add(tag);
										}
										ObserveTagCollectionForFile(fileId, c);
										return c;
									});
									File file = new File(
										fileId,
										RootPath, (string) dataReader["path"],
										tagCollection,
										(byte[]) dataReader["hash"]);
									return file;
								}, file => file.Id);
					}
				}
			}
		}

		private SQLCommand FileLookupQueryGenerate(bool shouldLookUpPath, int count, IReadOnlyCollection<Tag> tags, string path)
		{
			string sql = @"SELECT * FROM files WHERE 1=1 " +
				(shouldLookUpPath ? @"path LIKE ""%@path%""" : "") +
				(count >= 0 ? "AND (SELECT tagId FROM tags_files WHERE files.fileId = tags_files.fileId) IN (" : "") +
				string.Join(", ", Enumerable.Range(0, count).Select(x => $"@tag{x}")) +
				(count >= 0 ? ")" : "");
			var query = connection.CreateCommand(sql);
			foreach (var tagIndex in tags.Zip(Enumerable.Range(0, count), (tag, index) => new { Tag = tag, Index = index }))
			{
				query.BindNew($"@tag{tagIndex.Index}", tagIndex.Tag.Id);
			}
			if(shouldLookUpPath)
				query.BindNew("@path", path);
			return query;
		}

		public IEnumerable<File> LookupFilesByTagsAndName(IReadOnlyCollection<Tag> tags, string name)
		{
			return LookupFiles(FileLookupQueryGenerate(true, tags.Count, tags, name));
		}

		public IEnumerable<Namespace> AllNamespaces()
		{
			using(var command = connection.CreateCommand(@"SELECT * FROM namespaces"))
			{
				using(var reader = command.ExecuteReader())
				{
					while(reader.Read())
					{
						yield return new Namespace((string)reader["name"]);
					}
				}
			}
		}

		private IEnumerable<Tag> LookupTagsForFile(File file)
		{
			return LookupTagsForFileId(file.Id);
		}

		private IEnumerable<Tag> LookupTagsForFileId(long fileId)
		{
			string sql = @"
				SELECT tags.name, tags.tagId, tags.namespaceId, namespaces.name AS namespaceName
				FROM tags
				JOIN tags_files ON tags_files.tagId = tags.tagId
				JOIN namespaces ON tags.namespaceId = namespaces.namespaceId
				WHERE tags_files.fileId = @fileId";
			using (var command = connection.CreateCommand(sql))
			{
				command.BindNew("@fileId", fileId);
				using(SQLiteDataReader reader = command.ExecuteReader())
				{
					while(reader.Read())
					{
						yield return Lookup(
							reader,
							"tagId",
							tagCache,
							dataReader =>
								new Tag((long) dataReader["tagId"], new Namespace((string) dataReader["namespaceName"]),
									new TagName((string) dataReader["name"])),
							tag => tag.Id);
					}
				}
			}
		}

		public IEnumerable<Tag> TagsInNamespace(Namespace ns)
		{
			using (var command = connection.CreateCommand(@"SELECT * FROM tags WHERE namespaceId = (SELECT namespaceId FROM namespaces WHERE name = @name)"))
			{
				command.BindNew("@name", ns.Component);
				using (SQLiteDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						yield return Lookup(
							reader,
							"tagId",
							tagCache,
							dataReader => new Tag((long)dataReader["tagId"], ns, new TagName((string)dataReader["name"])),
							tag => tag.Id);
					}
				}
			}

		}

		public Tag AddTag(Namespace ns, TagName name)
		{
			var tag = new Tag(0, ns, name);
			long id;
			return connection.IssueTransaction(commit =>
			{
				using (var command = connection.CreateCommand(@"INSERT OR IGNORE INTO namespaces (name) VALUES (@name)"))
				{
					command.BindNew("@name", ns.Component);
					command.ExecuteNonQuery();
				}
				using (var command = connection.CreateCommand(@"INSERT INTO tags (name, namespaceId) VALUES (@tagName, (SELECT namespaceId FROM namespaces WHERE name = @nsName))"))
				{
					command.BindNew("@tagName", name.Name);
					command.BindNew("@nsName", ns.Component);
					command.ExecuteNonQuery();
					id = connection.LastInsertRowId();
				}

				commit();
				tag.Id = id;
				tagCache[tag.Id] = new WeakReference<Tag>(tag);
				return tag;
			});
		}

		private void ObserveTagCollectionForFile(long fileId, ObservableSet<Tag> collection)
		{
			collection.CollectionChanged += (sender, args) =>
			{
				if(args.Action == NotifyCollectionChangedAction.Reset)
				{
					connection.IssueTransaction(commit =>
					{
						using(var command = connection.CreateCommand(@"DELETE FROM tags_files WHERE fileId = @fileId"))
						{
							command.BindNew("@fileId", fileId);
							command.ExecuteNonQuery();
						}
						using(
							var command = connection.CreateCommand(@"INSERT INTO tags_files (tagId, fileId) VALUES (@tagId, @fileId)")
						)
						{
							command.BindNew("@tagId");
							command.BindNew("@fileId", fileId);
							var c = (ObservableSet<Tag>) sender;
							foreach(var tag in c)
							{
								command.BindReplace("@tagId", tag.Id);
								command.ExecuteNonQuery();
							}
						}
						commit();
						return Nothing.AtAll;
					});
				}
				if(args.NewItems != null)
				{
					using(
						var command = connection.CreateCommand(@"INSERT INTO tags_files (tagId, fileId) VALUES (@tagId, @fileId)"))
					{
						command.BindNew("@tagId");
						command.BindNew("@fileId");
						foreach (var newTag in args.NewItems.Cast<Tag>())
						{
							command.BindReplace("@fileId", fileId);
							command.BindReplace("@tagId", newTag.Id);
							command.ExecuteNonQuery();
						}
					}
				}
				if(args.OldItems != null)
				{
					using(
						var command = connection.CreateCommand(@"DELETE FROM tags_files WHERE tagId = @tagId AND fileId = @fileId"))
					{
						command.BindNew("@tagId");
						command.BindNew("@fileId");
						foreach (var newTag in args.OldItems.Cast<Tag>())
						{
							command.BindReplace("@fileId", fileId);
							command.BindReplace("@tagId", newTag.Id);
							command.ExecuteNonQuery();
						}
					}
				}
			};
		}

		public File GetFile(string path)
		{
			var relativeToTaxonomyRootPath = PathExt.GetRelativePath(RootPath, Path.GetFullPath(path));
			return connection.IssueTransaction(commit =>
			{
				File file;
				using(
					var command = connection.CreateCommand(@"SELECT * FROM files WHERE path = @path"))
				{
					command.BindNew("@path", relativeToTaxonomyRootPath);
					file = LookupFiles(command).FirstOrDefault();
				}
				if(file != null)
					return file;

				long id;
				var tagCollection = new Lazy<ICollection<Tag>>(() => new ObservableSet<Tag>());
				file = new File(0, RootPath, relativeToTaxonomyRootPath, tagCollection);
				using(
					var command = connection.CreateCommand(@"INSERT INTO files (path, hash) VALUES (@path, @hash)"))
				{
					command.BindNew("@path", file.RelativePath);
					command.BindNew("@hash", file.Hash);
					command.ExecuteNonQuery();
					id = connection.LastInsertRowId();
				}
				file.Id = id;
				commit();
				ObserveTagCollectionForFile(file.Id, (ObservableSet<Tag>) tagCollection.Value);
				fileCache.Add(file.Id, new WeakReference<File>(file));
				return file;
			});
		}

		public Taxonomy(string path) :
			this(path, facade.Open(path))
		{

		}

		private Taxonomy(string path, SQLConnection connection)
		{
			ManagedFile = Path.GetFullPath(path);
			this.connection = connection;
			using(var command = connection.CreateCommand(@"SELECT shortName FROM taxonomyMeta"))
			{
				ShortName = command.ExecuteScalar<string>();
			}
		}

		public static Taxonomy CreateNew(string path)
		{
			return CreateNew(path, Path.GetFileNameWithoutExtension(path));
		}

		public static Taxonomy CreateNew(string path, string shortName)
		{
			var connection = facade.CreateNew(path);
			connection.Open();
			connection.IssueSimpleCommand(@"CREATE TABLE taxonomyMeta (
				version INTEGER,
				shortName TEXT)");
			using(var command = connection.CreateCommand(@"INSERT INTO taxonomyMeta VALUES (1, @shortName)"))
			{
				command.BindNew("@shortName", shortName);
				command.ExecuteNonQuery();
			}
			connection.IssueSimpleCommand(@"CREATE TABLE files (
				fileId INTEGER PRIMARY KEY,
				path TEXT UNIQUE NOT NULL,
				hash BLOB NOT NULL)");
			connection.IssueSimpleCommand(@"CREATE TABLE namespaces (
				namespaceId INTEGER PRIMARY KEY,
				name TEXT UNIQUE NOT NULL ON CONFLICT IGNORE)");
			connection.IssueSimpleCommand(@"CREATE TABLE tags (
				tagId INTEGER PRIMARY KEY,
				name TEXT NOT NULL,
				namespaceId INTEGER,
				FOREIGN KEY(namespaceId) REFERENCES namespaces(namespaceId),
				UNIQUE (namespaceId, name) ON CONFLICT IGNORE)");
			connection.IssueSimpleCommand(@"CREATE TABLE tags_files (
				tagId INTEGER,
				fileId INTEGER,
				FOREIGN KEY(tagId) REFERENCES namespaces(tagId),
				FOREIGN KEY(fileId) REFERENCES namespaces(fileId),
				UNIQUE (tagId, fileId) ON CONFLICT IGNORE)");
			return new Taxonomy(path, connection);
		}

		private static readonly SQLiteFacade facade = new SQLiteFacadeData();
		private SQLConnection connection;
		private readonly Dictionary<long, WeakReference<File>> fileCache = new Dictionary<long, WeakReference<File>>();
		private readonly Dictionary<long, WeakReference<Tag>> tagCache = new Dictionary<long, WeakReference<Tag>>();

		public string ShortName { get; }
		public string RootPath => Path.GetDirectoryName(ManagedFile);
		public string ManagedFile { get; }

		public void Dispose()
		{
			if (connection != null)
			{
				connection.Dispose();
				connection = null;
			}
		}
	}
}
