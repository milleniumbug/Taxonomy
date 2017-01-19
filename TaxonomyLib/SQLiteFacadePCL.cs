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
	}
}