using System;
using SQLite;

namespace TaxonomyLib
{
	internal class SQLiteFacadePCL : SQLiteFacade<SQLiteConnection, SQLiteCommand>
	{
		public override long LastInsertRowIdFor(SQLiteConnection connection)
		{
			return SQLite3.LastInsertRowid(connection.Handle);
		}

		public override T WithCommand<T>(SQLiteCommand command, Func<SQLiteCommand, T> func)
		{
			return func(command);
		}

		public override SQLiteConnection CreateNew(string path)
		{
			throw new NotImplementedException();
		}

		public override void IssueSimpleCommand(SQLiteConnection connection, string sql)
		{
			throw new NotImplementedException();
		}

		public override SQLiteCommand CreateCommand(SQLiteConnection connection, string sql)
		{
			return connection.CreateCommand(sql);
		}

		public override void BindNew(SQLiteCommand command, string name)
		{
			
		}

		public override void BindNew(SQLiteCommand command, string name, object value)
		{
			BindReplace(command, name, value);
		}

		public override void BindReplace(SQLiteCommand command, string name, object value)
		{
			command.Bind(name, value);
		}

		public override T ExecuteScalar<T>(SQLiteCommand command)
		{
			return command.ExecuteScalar<T>();
		}

		public override T IssueTransaction<T>(SQLiteConnection connection, Func<DoCommit, T> func)
		{
			throw new NotImplementedException();
		}
	}
}