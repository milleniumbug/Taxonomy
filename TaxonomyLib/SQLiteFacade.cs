using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;

namespace TaxonomyLib
{
	// a facade to simplify transition from System.Data.SQLite to sqlite-net-pcl
	internal abstract class SQLiteFacade
	{
		public abstract SQLConnection Open(string path);
		public abstract SQLConnection CreateNew(string path);

		public delegate void DoCommit();
	}

	internal abstract class SQLConnection : IDisposable
	{
		public abstract long LastInsertRowId();
		public abstract void Open();
		public abstract void IssueSimpleCommand(string sql);
		public abstract SQLCommand CreateCommand(string sql);
		public abstract T IssueTransaction<T>(Func<SQLiteFacade.DoCommit, T> func);
		public abstract void Dispose();
	}

	internal abstract class SQLCommand : IDisposable
	{
		public abstract void BindNew(string name);
		public abstract void BindNew(string name, object value);
		public abstract void BindReplace(string name, object value);
		public abstract T ExecuteScalar<T>();
		public abstract void ExecuteNonQuery();
		public abstract SQLReader ExecuteReader(IReadOnlyCollection<string> names);
		public SQLReader ExecuteReader(params string[] names)
		{
			return ExecuteReader((IReadOnlyCollection<string>)names);
		}
		public abstract void Dispose();
	}

	internal abstract class SQLReader : IDisposable
	{
		public abstract bool Read();
		public abstract object this[string key] { get; }
		public abstract void Dispose();
	}
}
