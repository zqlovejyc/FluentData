﻿using System;
using System.Collections.Generic;
using System.Data;

namespace FluentData
{
	internal abstract class BaseStoredProcedureBuilder
	{
		protected BuilderData Data { get; set; }
		protected ActionsHandler Actions { get; set; }

		private IDbCommand Command
		{
			get
			{
				Data.Command.CommandType(DbCommandTypes.StoredProcedure);
				Data.Command.Sql(Data.Provider.GetSqlForStoredProcedureBuilder(Data));
				return Data.Command;
			}
		}

		public BaseStoredProcedureBuilder(IDbProvider provider, IDbCommand command, string name)
		{
			Data =  new BuilderData(provider, command, name);
			Actions = new ActionsHandler(Data);
		}


		public void Dispose()
		{
			Command.Dispose();
		}

		public TParameterType ParameterValue<TParameterType>(string outputParameterName)
		{
			return Command.ParameterValue<TParameterType>(outputParameterName);
		}

		public int Execute()
		{
			return Command.Execute();
		}

		public List<dynamic> Query()
		{
			return Command.Query();
		}

		public TList Query<TEntity, TList>(Action<IDataReader, TEntity> customMapper = null) where TList : IList<TEntity>
		{
			return Command.Query<TEntity, TList>(customMapper);
		}

		public List<TEntity> Query<TEntity>(Action<IDataReader, TEntity> customMapper = null)
		{
			return Command.Query<TEntity>(customMapper);
		}

		public void QueryComplex<TEntity>(IList<TEntity> list, Action<IDataReader, IList<TEntity>> customMapper)
		{
			Command.QueryComplex<TEntity>(list, customMapper);
		}

		public dynamic QuerySingle()
		{
			return Command.QuerySingle();
		}

		public TEntity QuerySingle<TEntity>(Action<IDataReader, TEntity> customMapper = null)
		{
			return Command.QuerySingle<TEntity>(customMapper);
		}

		public TEntity QuerySingleComplex<TEntity>(Func<IDataReader, TEntity> customMapper)
		{
			return Command.QuerySingleComplex(customMapper);
		}

		public DataTable QueryDataTable()
		{
			return Command.QueryDataTable();
		}
	}
}
