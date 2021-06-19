using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

namespace Bonanza.Storage.SqLite
{
	/// <summary>
	/// Builder for Sqlite command
	/// </summary>
	public static class SqliteCommandBuilder
	{
		//public static SqliteCommand Create(string commandText, SqliteConnection conn)
		//{
		//	var cmd = conn.CreateCommand();
		//	cmd.CommandText = commandText;
		//	return cmd;
		//}

		public static SqliteCommand CreateCommand(this SqliteConnection conn, string commandText )
		{
			var cmd = conn.CreateCommand();
			cmd.CommandText = commandText;
			return cmd;
		}
	}
}
