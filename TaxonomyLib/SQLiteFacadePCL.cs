using System;
using System.Collections.Generic;
using SQLite;

namespace TaxonomyLib
{
	internal class SQLiteFacadePCL : SQLiteFacade
	{
		public override SQLConnection Open(string path)
		{
			throw new NotImplementedException();
		}

		public override SQLConnection CreateNew(string path)
		{
			throw new NotImplementedException();
		}
	}

	internal class SQLConnectionPCL : SQLConnection
	{
		private readonly SQLiteConnection connection;

		public SQLConnectionPCL(SQLiteConnection connection)
		{
			this.connection = connection;
		}

		public override long LastInsertRowId()
		{
			return SQLite3.LastInsertRowid(connection.Handle);
		}

		public override void Open()
		{
			throw new NotImplementedException();
		}

		public override void IssueSimpleCommand(string sql)
		{
			throw new NotImplementedException();
		}

		public override SQLCommand CreateCommand(string sql)
		{
			return new SQLCommandPCL(connection.CreateCommand(sql));
		}

		public override T IssueTransaction<T>(Func<SQLiteFacade.DoCommit, T> func)
		{
			connection.BeginTransaction();
			T value;
			bool isCommited = false;
			try
			{
				value = func(() =>
				{
					connection.Commit();
					isCommited = true;
				});
			}
			finally
			{
				if (!isCommited)
					connection.Rollback();
			}
			return value;
		}

		public override void Dispose()
		{
			connection.Dispose();
		}
	}

	internal class SQLCommandPCL : SQLCommand
	{
		private readonly SQLiteCommand command;

		public override System.Data.SQLite.SQLiteDataReader ExecuteReader()
		{
			throw new NotImplementedException();
		}

		public SQLCommandPCL(SQLiteCommand command)
		{
			this.command = command;
		}

		public override void BindNew(string name)
		{
			throw new NotImplementedException();
		}

		public override void BindNew(string name, object value)
		{
			BindReplace(name, value);
		}

		public override void BindReplace(string name, object value)
		{
			command.Bind(name, value);
		}

		public override T ExecuteScalar<T>()
		{
			return command.ExecuteScalar<T>();
		}

		public override void ExecuteNonQuery()
		{
			throw new NotImplementedException();
		}

		public override IEnumerable<IDictionary<string, object>> ExecuteQuery(IReadOnlyCollection<string> names)
		{
			throw new NotImplementedException();
		}

		public override void Dispose()
		{
			
		}
	}
}