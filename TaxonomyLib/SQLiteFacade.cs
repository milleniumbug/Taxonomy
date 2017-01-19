using System;
using System.Collections.Generic;

namespace TaxonomyLib
{
	// a facade to simplify transition from System.Data.SQLite to sqlite-net-pcl
	internal abstract class SQLiteFacade<TConnection, TCommand>
	{
		public abstract long LastInsertRowIdFor(TConnection connection);
		public abstract T WithCommand<T>(TCommand command, Func<TCommand, T> func);
		public abstract TConnection CreateNew(string path);
		public abstract void IssueSimpleCommand(TConnection connection, string sql);
		public abstract TConnection Open(string path);
		public abstract void Open(TConnection connection);

		public abstract TCommand CreateCommand(TConnection connection, string sql);
		public abstract void BindNew(TCommand command, string name);
		public abstract void BindNew(TCommand command, string name, object value);
		public abstract void BindReplace(TCommand command, string name, object value);
		public abstract T ExecuteScalar<T>(TCommand command);
		public abstract IEnumerable<IDictionary<string, object>> ExecuteQuery(TCommand query, IReadOnlyCollection<string> names);

		public IEnumerable<IDictionary<string, object>> ExecuteQuery(TCommand query, params string[] names)
		{
			return ExecuteQuery(query, (IReadOnlyCollection<string>)names);
		}

		public delegate void DoCommit();
		public abstract T IssueTransaction<T>(TConnection connection, Func<DoCommit, T> func);
	}
}
