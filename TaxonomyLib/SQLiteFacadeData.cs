using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace TaxonomyLib
{
	internal class SQLiteFacadeData : SQLiteFacade
	{
		public override SQLConnection Open(string path)
		{
			return new SQLConnectionData(new SQLiteConnection($"Data Source={path};Version=3").OpenAndReturn());
		}

		public override SQLConnection CreateNew(string path)
		{
			SQLiteConnection.CreateFile(path);
			SQLiteConnection connection = new SQLiteConnection($"Data Source={path};Version=3");
			var c = new SQLConnectionData(connection);
			return c;
		}
	}

	internal class SQLConnectionData : SQLConnection
	{
		private readonly SQLiteConnection connection;

		public SQLConnectionData(SQLiteConnection connection)
		{
			this.connection = connection;
		}

		public override T IssueTransaction<T>(Func<SQLiteFacade.DoCommit, T> func)
		{
			using (var transaction = connection.BeginTransaction())
			{
				return func(() => transaction.Commit());
			}
		}

		public override void Dispose()
		{
			connection.Dispose();
		}

		public override void Open()
		{
			connection.Open();
		}

		public override long LastInsertRowId()
		{
			return connection.LastInsertRowId;
		}

		public override void IssueSimpleCommand(string sql)
		{
			using (var command = new SQLiteCommand(sql, connection))
			{
				command.ExecuteNonQuery();
			}
		}

		public override SQLCommand CreateCommand(string sql)
		{
			return new SQLCommandData(new SQLiteCommand(sql, connection));
		}
	}

	internal class SQLCommandData : SQLCommand
	{
		private readonly SQLiteCommand command;

		public SQLCommandData(SQLiteCommand command)
		{
			this.command = command;
		}

		public override void BindNew(string name)
		{
			command.Parameters.Add(new SQLiteParameter(name));
		}

		public override void BindNew(string name, object value)
		{
			command.Parameters.AddWithValue(name, value);
		}

		public override void BindReplace(string name, object value)
		{
			command.Parameters[name].Value = value;
		}

		public override T ExecuteScalar<T>()
		{
			return (T)command.ExecuteScalar();
		}

		public override void ExecuteNonQuery()
		{
			command.ExecuteNonQuery();
		}

		public override SQLiteDataReader ExecuteReader()
		{
			return command.ExecuteReader();
		}

		public override IEnumerable<IDictionary<string, object>> ExecuteQuery(IReadOnlyCollection<string> names)
		{
			using (var reader = command.ExecuteReader())
			{
				while (reader.Read())
				{
					var dict = new Dictionary<string, object>();
					foreach (var name in names)
					{
						dict.Add(name, reader[name]);
					}
					yield return dict;
				}
			}
		}

		public override void Dispose()
		{
			command.Dispose();
		}
	}
}