using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using Common;

namespace TaxonomyLib
{
	public class Taxonomy : IDisposable
	{
		public Taxonomy(string path) :
			this(path, facade.OpenAndReturn(facade.New(path)))
		{

		}

		public Tag AddTag(Namespace ns, TagName name)
		{
			var tag = new Tag(0, ns, name);
			long id;
			using (var transaction = connection.BeginTransaction())
			{
				using (var command = connection.CreateCommand())
				{
					command.CommandText = @"INSERT OR IGNORE INTO namespaces (name) VALUES (@name)";
					command.Parameters.Add(facade.Parameter("@name", DbType.String));
					command.Parameters["@name"].Value = ns.Component;
					command.ExecuteNonQuery();
				}
				using (var command = connection.CreateCommand())
				{
					command.CommandText =
						@"INSERT INTO tags (name, namespaceId) VALUES (@tagName, (SELECT namespaceId FROM namespaces WHERE name = @nsName))";
					command.Parameters.Add(facade.Parameter("@tagName", DbType.String));
					command.Parameters["@tagName"].Value = name.Name;
					command.Parameters.Add(facade.Parameter("@nsName", DbType.String));
					command.Parameters["@nsName"].Value = ns.Component;
					command.ExecuteNonQuery();
					id = facade.LastInsertRowId(connection);
				}

				transaction.Commit();
				tag.Id = id;
				tagCache[tag.Id] = new WeakReference<Tag>(tag);
				return tag;
			}
		}

		public IEnumerable<Namespace> AllNamespaces()
		{
			using(var command = connection.CreateCommand())
			{
				command.CommandText = @"SELECT * FROM namespaces";
				using(var reader = command.ExecuteReader())
				{
					while(reader.Read())
					{
						yield return new Namespace((string)reader["name"]);
					}
				}
			}
		}

		[Obsolete]
		public static Taxonomy CreateNew(string path)
		{
			return CreateNew(path, Path.GetFileNameWithoutExtension(path));
		}

		public static Taxonomy CreateNew(string path, string shortName)
		{
			var connection = facade.CreateNew(path);
			connection.Open();
			using(var command = connection.CreateCommand())
			{
				command.CommandText = @"
				CREATE TABLE taxonomyMeta (
					version INTEGER,
					shortName TEXT)";
				command.ExecuteNonQuery();
			}
			using(var command = connection.CreateCommand())
			{
				command.CommandText = @"INSERT INTO taxonomyMeta VALUES (1, @shortName)";
				command.Parameters.Add(facade.Parameter("@shortName"));
				command.Parameters["@shortName"].Value = shortName;
				command.ExecuteNonQuery();
			}
			using(var command = connection.CreateCommand())
			{
				command.CommandText = @"
				CREATE TABLE files (
					fileId INTEGER PRIMARY KEY,
					path TEXT UNIQUE NOT NULL,
					hash BLOB NOT NULL)";
				command.ExecuteNonQuery();
			}
			using(var command = connection.CreateCommand())
			{
				command.CommandText = @"
				CREATE TABLE namespaces (
					namespaceId INTEGER PRIMARY KEY,
					name TEXT UNIQUE NOT NULL ON CONFLICT IGNORE)";
				command.ExecuteNonQuery();
			}
			using(var command = connection.CreateCommand())
			{
				command.CommandText = @"
				CREATE TABLE tags (
					tagId INTEGER PRIMARY KEY,
					name TEXT NOT NULL,
					namespaceId INTEGER,
					FOREIGN KEY(namespaceId) REFERENCES namespaces(namespaceId),
					UNIQUE (namespaceId, name) ON CONFLICT IGNORE)";
				command.ExecuteNonQuery();
			}
			using(var command = connection.CreateCommand())
			{
				command.CommandText = @"
				CREATE TABLE tags_files (
					tagId INTEGER,
					fileId INTEGER,
					FOREIGN KEY(tagId) REFERENCES namespaces(tagId),
					FOREIGN KEY(fileId) REFERENCES namespaces(fileId),
					UNIQUE (tagId, fileId) ON CONFLICT IGNORE)";
				command.ExecuteNonQuery();
			}
			return new Taxonomy(path, connection);
		}

		public void Dispose()
		{
			if (connection != null)
			{
				connection.Close();
				connection.Dispose();
				connection = null;
			}
		}

		public File GetFile(string path)
		{
			var relativeToTaxonomyRootPath = PathExt.GetRelativePath(RootPath, Path.GetFullPath(path));
			using(var transaction = connection.BeginTransaction())
			{
				File file;
				using (
					var command = connection.CreateCommand())
				{
					command.CommandText = @"SELECT * FROM files WHERE path = @path";
					command.Parameters.Add(facade.Parameter("@path", DbType.String));
					command.Parameters["@path"].Value = relativeToTaxonomyRootPath;
					file = LookupFiles(command).FirstOrDefault();
				}
				if(file != null)
					return file;

				long id;
				var tagCollection = new Lazy<ICollection<Tag>>(() => new ObservableSet<Tag>());
				file = new File(0, RootPath, relativeToTaxonomyRootPath, tagCollection);
				using (
					var command = connection.CreateCommand())
				{
					command.CommandText = @"INSERT INTO files (path, hash) VALUES (@path, @hash)";
					command.Parameters.Add(facade.Parameter("@path", DbType.String));
					command.Parameters.Add(facade.Parameter("@hash", DbType.Binary));
					command.Parameters["@path"].Value = file.RelativePath;
					command.Parameters["@hash"].Value = file.Hash;
					command.ExecuteNonQuery();
					id = facade.LastInsertRowId(connection);
				}
				file.Id = id;
				transaction.Commit();
				ObserveTagCollectionForFile(file.Id, (ObservableSet<Tag>)tagCollection.Value);
				fileCache.Add(file.Id, new WeakReference<File>(file));
				return file;
			}
		}

		public IEnumerable<File> LookupFilesByTags(IReadOnlyCollection<Tag> tags)
		{
			return LookupFiles(FileLookupQueryGenerate(false, tags.Count, tags, null));
		}

		public IEnumerable<File> LookupFilesByTagsAndName(IReadOnlyCollection<Tag> tags, string name)
		{
			return LookupFiles(FileLookupQueryGenerate(true, tags.Count, tags, name));
		}

		public string ManagedFile { get; }
		public string RootPath => Path.GetDirectoryName(ManagedFile);

		public string ShortName { get; }

		public IEnumerable<Tag> TagsInNamespace(Namespace ns)
		{
			using(var command = connection.CreateCommand())
			{
				command.CommandText =
					@"SELECT * FROM tags WHERE namespaceId = (SELECT namespaceId FROM namespaces WHERE name = @name)";
				command.Parameters.Add(facade.Parameter("@name", DbType.String));
				command.Parameters["@name"].Value = ns.Component;
				using (DbDataReader reader = command.ExecuteReader())
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

		private Taxonomy(string path, DbConnection connection)
		{
			ManagedFile = Path.GetFullPath(path);
			this.connection = connection;
			using (var command = connection.CreateCommand())
			{
				command.CommandText = @"SELECT shortName FROM taxonomyMeta";
				ShortName = (string)command.ExecuteScalar();
			}
		}

		private DbConnection connection;
		private static readonly SQLiteFacade facade = new SQLiteFacadeNETFramework();
		private readonly Dictionary<long, WeakReference<File>> fileCache = new Dictionary<long, WeakReference<File>>();

		private DbCommand FileLookupQueryGenerate(bool shouldLookUpPath, int count, IReadOnlyCollection<Tag> tags, string path)
		{
			string sql = @"SELECT * FROM files WHERE 1=1 " +
				(shouldLookUpPath ? @"path LIKE ""%@path%""" : "") +
				(count >= 0 ? "AND (SELECT tagId FROM tags_files WHERE files.fileId = tags_files.fileId) IN (" : "") +
				string.Join(", ", Enumerable.Range(0, count).Select(x => $"@tag{x}")) +
				(count >= 0 ? ")" : "");
			var query = connection.CreateCommand();
			query.CommandText = sql;
			foreach (var tagIndex in tags.Zip(Enumerable.Range(0, count), (tag, index) => new { Tag = tag, Index = index }))
			{
				query.Parameters.Add(facade.Parameter($"@tag{tagIndex.Index}"));
				query.Parameters[$"@tag{tagIndex.Index}"].Value = tagIndex.Tag.Id;
			}
			if(shouldLookUpPath)
			{
				query.Parameters.Add(facade.Parameter("@path"));
				query.Parameters["@path"].Value = path;
			}
			return query;
		}

		private TItem Lookup<TItem>(DbDataReader reader, string idName, Dictionary<long, WeakReference<TItem>> cache,
			Func<DbDataReader, TItem> factory, Func<TItem, long> idExtractor) where TItem : class
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

		private IEnumerable<File> LookupFiles(DbCommand command)
		{
			using(var cmd = command)
			{
				using (var reader = cmd.ExecuteReader())
				{
					while(reader.Read())
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
			using (var command = connection.CreateCommand())
			{
				command.CommandText = sql;
				command.Parameters.Add(facade.Parameter("@fileId"));
				command.Parameters["@fileId"].Value = fileId;
				using (DbDataReader reader = command.ExecuteReader())
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

		private void ObserveTagCollectionForFile(long fileId, ObservableSet<Tag> collection)
		{
			collection.CollectionChanged += (sender, args) =>
			{
				if(args.Action == NotifyCollectionChangedAction.Reset)
				{
					using(var transaction = connection.BeginTransaction())
					{
						using(var command = connection.CreateCommand())
						{
							command.CommandText = @"DELETE FROM tags_files WHERE fileId = @fileId";
							command.Parameters.Add(facade.Parameter("@fileId"));
							command.Parameters["@fileId"].Value = fileId;
							command.ExecuteNonQuery();
						}
						using(var command = connection.CreateCommand())
						{
							command.CommandText = @"INSERT INTO tags_files (tagId, fileId) VALUES (@tagId, @fileId)";
							command.Parameters.Add(facade.Parameter("@tagId", DbType.Int64));
							command.Parameters.Add(facade.Parameter("@fileId"));
							command.Parameters["@fileId"].Value = fileId;
							var c = (ObservableSet<Tag>)sender;
							foreach(var tag in c)
							{
								command.Parameters["@tagId"].Value = tag.Id;
								command.ExecuteNonQuery();
							}
						}
						transaction.Commit();
					}
				}
				if(args.NewItems != null)
				{
					using(
						var command = connection.CreateCommand())
					{
						command.CommandText = @"INSERT INTO tags_files (tagId, fileId) VALUES (@tagId, @fileId)";
						command.Parameters.Add(facade.Parameter("@tagId", DbType.Int64));
						command.Parameters.Add(facade.Parameter("@fileId", DbType.Int64));
						foreach(var newTag in args.NewItems.Cast<Tag>())
						{
							command.Parameters["@fileId"].Value = fileId;
							command.Parameters["@tagId"].Value = newTag.Id;
							command.ExecuteNonQuery();
						}
					}
				}
				if(args.OldItems != null)
				{
					using(
						var command = connection.CreateCommand())
					{
						command.CommandText = @"DELETE FROM tags_files WHERE tagId = @tagId AND fileId = @fileId";
						command.Parameters.Add(facade.Parameter("@tagId", DbType.Int64));
						command.Parameters.Add(facade.Parameter("@fileId", DbType.Int64));
						foreach(var newTag in args.OldItems.Cast<Tag>())
						{
							command.Parameters["@fileId"].Value = fileId;
							command.Parameters["@tagId"].Value = newTag.Id;
							command.ExecuteNonQuery();
						}
					}
				}
			};
		}

		private readonly Dictionary<long, WeakReference<Tag>> tagCache = new Dictionary<long, WeakReference<Tag>>();
	}
}
