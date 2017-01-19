using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace TaxonomyLib
{
	internal class SQLiteFacadeNETFramework : SQLiteFacade
	{
		public override long LastInsertRowId(DbConnection connection)
		{
			return ((SQLiteConnection)connection).LastInsertRowId;
		}

		public override DbConnection CreateNew(string path)
		{
			SQLiteConnection.CreateFile(path);
			return New(path);
		}

		public override DbConnection New(string path)
		{
			return new SQLiteConnection($"Data Source={path};Version=3");
		}

		public override DbParameter Parameter(string name)
		{
			return new SQLiteParameter(name);
		}

		public override DbParameter Parameter(string name, DbType type)
		{
			return new SQLiteParameter(name, type);
		}
	}
}