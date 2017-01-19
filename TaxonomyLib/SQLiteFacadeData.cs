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

		public override SQLiteConnection Open(string path)
		{
			return new SQLiteConnection($"Data Source={path};Version=3").OpenAndReturn();
		}

		public override void Open(SQLiteConnection connection)
		{
			connection.Open();
		}

		public override SQLiteCommand CreateCommand(SQLiteConnection connection, string sql)
		{
			return new SQLiteCommand(sql, connection);
		}

		public override void BindNew(SQLiteCommand command, string name)
		{
			command.Parameters.Add(new SQLiteParameter(name));
		}

		public override void BindNew(SQLiteCommand command, string name, object value)
		{
			command.Parameters.AddWithValue(name, value);
		}

		public override void BindReplace(SQLiteCommand command, string name, object value)
		{
			command.Parameters[name].Value = value;
		}

		public override T ExecuteScalar<T>(SQLiteCommand command)
		{
			return (T)command.ExecuteScalar();
		}

		public override T IssueTransaction<T>(SQLiteConnection connection, Func<DoCommit, T> func)
		{
			using(var transaction = connection.BeginTransaction())
			{
				return func(() => transaction.Commit());
			}
		}
	}
}