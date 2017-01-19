using System;
using System.Data.SQLite;

namespace TaxonomyLib
{
	internal class SQLiteFacadeData : SQLiteFacade<SQLiteConnection, SQLiteCommand>
	{
		public override long LastInsertRowIdFor(SQLiteConnection connection)
		{
			return connection.LastInsertRowId;
		}

		public override TOut WithCommand<TOut>(SQLiteCommand command, Func<SQLiteCommand, TOut> func)
		{
			using(var cmd = command)
			{
				return func(command);
			}
		}

		public override SQLiteConnection CreateNew(string path)
		{
			SQLiteConnection.CreateFile(path);
			SQLiteConnection connection = new SQLiteConnection($"Data Source={path};Version=3");
			return connection;
		}

		public override void IssueSimpleCommand(SQLiteConnection connection, string sql)
		{
			using(var command = new SQLiteCommand(sql, connection))
			{
				command.ExecuteNonQuery();
			}
		}
	}
}