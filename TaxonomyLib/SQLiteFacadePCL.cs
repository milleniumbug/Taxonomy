using System;
using System.Collections.Generic;
using SQLite;

namespace TaxonomyLib
{
	internal class SQLiteFacadePCL : SQLiteFacade
	{
		public override SQLConnection Open(string path)
		{
			return new SQLConnectionPCL(new SQLiteConnection(path));
		}

		public override SQLConnection CreateNew(string path)
		{
			return new SQLConnectionPCL(new SQLiteConnection(path, SQLiteOpenFlags.Create));
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
			
		}

		public override void IssueSimpleCommand(string sql)
		{
			connection.Execute(sql);
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

		public override SQLReader ExecuteReader(IReadOnlyCollection<string> names)
		{
			throw new NotImplementedException();
		}

		public SQLCommandPCL(SQLiteCommand command)
		{
			this.command = command;
		}

		public override void BindNew(string name)
		{
			
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
			command.ExecuteNonQuery();
		}

		public override void Dispose()
		{
			
		}
	}

	internal class SQLReaderPCL : SQLReader
	{
		public override bool Read()
		{
			throw new NotImplementedException();
		}

		public override object this[string key]
		{
			get { throw new NotImplementedException(); }
		}

		public override void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}