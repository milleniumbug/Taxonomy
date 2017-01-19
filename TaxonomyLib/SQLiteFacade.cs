using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxonomyLib
{
	// a facade to simplify transition from System.Data.SQLite to sqlite-net-pcl
	internal abstract class SQLiteFacade<TConnection, TCommand>
	{
		public abstract long LastInsertRowIdFor(TConnection connection);
		public abstract T WithCommand<T>(TCommand command, Func<TCommand, T> func);
		public abstract TConnection CreateNew(string path);
		public abstract void IssueSimpleCommand(TConnection connection, string sql);
	}
}
