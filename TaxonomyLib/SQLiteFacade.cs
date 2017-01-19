using System.Data;
using System.Data.Common;

namespace TaxonomyLib
{
	internal abstract class SQLiteFacade
	{
		public abstract long LastInsertRowId(DbConnection connection);

		public abstract DbConnection CreateNew(string path);

		public abstract DbConnection New(string path);

		public abstract DbParameter Parameter(string name);

		public abstract DbParameter Parameter(string name, DbType type);

		public DbConnection OpenAndReturn(DbConnection connection)
		{
			connection.Open();
			return connection;
		}
	}
}