using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Data.SQLite.Linq;
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
			return LookupFilesByTags(FileLookupQueryGenerate(false, tags.Count, tags, null));
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
				cache.Add(idExtractor(item), new WeakReference<TItem>(item));
				return item;
			}
		}

		private IEnumerable<File> LookupFilesByTags(SQLiteCommand command)
		{
			using(var cmd = command)
			{
				using(var reader = cmd.ExecuteReader())
				{
					while(reader.Read())
					{
						yield return
							Lookup(reader, "fileId", fileCache,
								dataReader =>
								{
									long fileId = (long) dataReader["fileId"];
									File file = new File(
										fileId,
										RootPath, (string) dataReader["path"],
										ObserveTagCollectionForFile(fileId, new ObservableCollection<Tag>()),
										(byte[]) dataReader["hash"]);
									return file;
								}, file => file.Id);
					}
				}
			}
		}

		private SQLiteCommand FileLookupQueryGenerate(bool shouldLookUpPath, int count, IReadOnlyCollection<Tag> tags, string path)
		{
			string sql = @"SELECT * FROM files WHERE 1=1 " +
				(shouldLookUpPath ? @"path LIKE ""%@path%""" : "") +
				(count >= 0 ? "AND (SELECT tagId FROM tags_files WHERE files.fileId = tags_files.fileId) IN (" : "") +
				string.Join(", ", Enumerable.Range(0, count).Select(x => $"@tag{x}")) +
				(count >= 0 ? ")" : "");
			SQLiteCommand query = new SQLiteCommand(sql, connection);
			foreach (var tagIndex in tags.Zip(Enumerable.Range(0, count), (tag, index) => new { Tag = tag, Index = index }))
			{
				query.Parameters.AddWithValue($"@tag{tagIndex.Index}", tagIndex.Tag.Id);
			}
			if(shouldLookUpPath)
				query.Parameters.AddWithValue("@path", path);
			return query;
		}

		public IEnumerable<File> LookupFilesByTagsAndName(IReadOnlyCollection<Tag> tags, string name)
		{
			return LookupFilesByTags(FileLookupQueryGenerate(true, tags.Count, tags, name));
		}

		public IEnumerable<Namespace> AllNamespaces()
		{
			using(var command = new SQLiteCommand(@"SELECT * FROM namespaces", connection))
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

		public IEnumerable<Tag> TagsInNamespace(Namespace ns)
		{
			using (var command = new SQLiteCommand(@"SELECT * FROM tags WHERE namespaceId = (SELECT namespaceId FROM namespaces WHERE name = @name)", connection))
			{
				command.Parameters.Add(new SQLiteParameter("@name", DbType.String));
				command.Parameters["@name"].Value = ns.Component;
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
			using (var transaction = connection.BeginTransaction())
			{
				using (var command = new SQLiteCommand(@"INSERT OR IGNORE INTO namespaces (name) VALUES (@name)", connection))
				{
					command.Parameters.Add(new SQLiteParameter("@name", DbType.String));
					command.Parameters["@name"].Value = ns.Component;
					command.ExecuteNonQuery();
				}
				using (var command = new SQLiteCommand(@"INSERT INTO tags (name, namespaceId) VALUES (@name, last_insert_rowid())", connection))
				{
					command.Parameters.Add(new SQLiteParameter("@name", DbType.String));
					command.Parameters["@name"].Value = name.Name;
					command.ExecuteNonQuery();
					id = connection.LastInsertRowId;
				}

				transaction.Commit();
				tag.Id = id;
				tagCache[tag.Id] = new WeakReference<Tag>(tag);
				return tag;
			}
		}

		private ObservableCollection<Tag> ObserveTagCollectionForFile(long fileId, ObservableCollection<Tag> collection)
		{
			collection.CollectionChanged += (sender, args) =>
			{
				if(args.NewItems != null)
				{
					using(
						var command = new SQLiteCommand(@"INSERT INTO tags_files (tagId, fileId) VALUES (@tagId, @fileId)", connection))
					{
						command.Parameters.Add(new SQLiteParameter("@tagId", DbType.Int64));
						command.Parameters.Add(new SQLiteParameter("@fileId", DbType.Int64));
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
						var command = new SQLiteCommand(@"DELETE FROM tags_files WHERE tagId = @tagId AND fileId = @fileId", connection))
					{
						command.Parameters.Add(new SQLiteParameter("@tagId", DbType.Int64));
						command.Parameters.Add(new SQLiteParameter("@fileId", DbType.Int64));
						foreach(var newTag in args.OldItems.Cast<Tag>())
						{
							command.Parameters["@fileId"].Value = fileId;
							command.Parameters["@tagId"].Value = newTag.Id;
							command.ExecuteNonQuery();
						}
					}
				}
			};
			return collection;
		}

		public File AddFile(string path)
		{
			var tagCollection = new ObservableCollection<Tag>();
			var file = new File(0, RootPath, path, tagCollection);
			long id;
			using (SQLiteCommand command = new SQLiteCommand(@"INSERT INTO files (path, hash) VALUES (@path, @hash)", connection))
			{
				command.Parameters.Add(new SQLiteParameter("@path", DbType.String));
				command.Parameters.Add(new SQLiteParameter("@hash", DbType.Binary));
				command.Parameters["@path"].Value = file.RelativePath;
				command.Parameters["@hash"].Value = file.Hash;
				command.ExecuteNonQuery();
				id = connection.LastInsertRowId;
			}
			file.Id = id;
			ObserveTagCollectionForFile(file.Id, tagCollection);
			fileCache.Add(file.Id, new WeakReference<File>(file));
			return file;
		}

		public Taxonomy(string path) :
			this(path, new SQLiteConnection($"Data Source={path};Version=3"))
		{
			connection.Open();
		}

		private Taxonomy(string path, SQLiteConnection connection)
		{
			ManagedFile = path;
			this.connection = connection;
		}

		public static Taxonomy CreateNew(string path)
		{
			SQLiteConnection.CreateFile(path);
			SQLiteConnection connection = new SQLiteConnection($"Data Source={path};Version=3");
			connection.Open();
			new SQLiteCommand(@"CREATE TABLE taxonomyMeta (
				version INTEGER)", connection).ExecuteNonQuery();
			new SQLiteCommand(@"INSERT INTO taxonomyMeta VALUES (1)", connection).ExecuteNonQuery();
			new SQLiteCommand(@"CREATE TABLE files (
				fileId INTEGER PRIMARY KEY,
				path TEXT UNIQUE NOT NULL,
				hash BLOB NOT NULL)", connection).ExecuteNonQuery();
			new SQLiteCommand(@"CREATE TABLE namespaces (
				namespaceId INTEGER PRIMARY KEY,
				name TEXT UNIQUE NOT NULL ON CONFLICT IGNORE)", connection).ExecuteNonQuery();
			new SQLiteCommand(@"CREATE TABLE tags (
				tagId INTEGER PRIMARY KEY,
				name TEXT NOT NULL,
				namespaceId INTEGER,
				FOREIGN KEY(namespaceId) REFERENCES namespaces(namespaceId),
				UNIQUE (namespaceId, name) ON CONFLICT IGNORE)", connection).ExecuteNonQuery();
			new SQLiteCommand(@"CREATE TABLE tags_files (
				tagId INTEGER,
				fileId INTEGER,
				FOREIGN KEY(tagId) REFERENCES namespaces(tagId),
				FOREIGN KEY(fileId) REFERENCES namespaces(fileId),
				UNIQUE (tagId, fileId) ON CONFLICT IGNORE)", connection).ExecuteNonQuery();
			return new Taxonomy(path, connection);
		}

		// TODO: Replace with database connection
		private SQLiteConnection connection;
		private readonly Dictionary<long, WeakReference<File>> fileCache = new Dictionary<long, WeakReference<File>>();
		private readonly Dictionary<long, WeakReference<Tag>> tagCache = new Dictionary<long, WeakReference<Tag>>();
		private string RootPath => Path.GetDirectoryName(ManagedFile);
		public string ManagedFile { get; }

		public void Dispose()
		{
			if (connection != null)
			{
				connection.Close();
				connection.Dispose();
				connection = null;
			}
		}
	}
}
