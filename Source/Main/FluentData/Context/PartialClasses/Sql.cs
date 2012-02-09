﻿using System.Data;

namespace FluentData
{
	public partial class DbContext : IDbContext
	{
		private DbCommand CreateCommand
		{
			get
			{
				IDbConnection connection = null;

				if (ContextData.UseTransaction)
				{
					if (ContextData.Connection == null)
						ContextData.Connection = ContextData.DbProvider.CreateConnection(ContextData.ConnectionString);
					connection = ContextData.Connection;
				}
				else
					connection = ContextData.DbProvider.CreateConnection(ContextData.ConnectionString);

				var cmd = connection.CreateCommand();
				cmd.Connection = connection;

				return new DbCommand(this, cmd, ContextData);
			}
		}

		public IDbCommand Sql(string sql, params object[] parameters)
		{
			var command = CreateCommand.Sql(sql);
			if (parameters != null)
				command.Parameters(parameters);
			return command;
		}

		public IDbCommand MultiResultSql()
		{
			return CreateCommand.UseMultipleResultset;
		}

		public IDbCommand MultiResultSql(string sql, params object[] parameters)
		{
			var command = CreateCommand.UseMultipleResultset.Sql(sql);
			if (parameters != null)
				command.Parameters(parameters);
			return command;
		}
	}
}