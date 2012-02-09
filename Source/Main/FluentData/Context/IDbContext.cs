using System;
using System.Data;
using System.Dynamic;
using FluentData;

namespace FluentData
{
	public interface IDbContext : IDisposable
	{
		IDbCommand Sql(string sql, params object[] parameters);
		IDbCommand MultiResultSql();
		IDbCommand MultiResultSql(string sql, params object[] parameters);
		IDbContext ThrowExceptionIfAutoMapFails { get; }
		IDbContext UseTransaction { get; }
		IInsertBuilder Insert(string tableName);
		IInsertBuilder<T> Insert<T>(string tableName, T item);
		IInsertBuilderDynamic Insert(string tableName, ExpandoObject item);
		IUpdateBuilder Update(string tableName);
		IUpdateBuilder<T> Update<T>(string tableName, T item);
		IUpdateBuilderDynamic Update(string tableName, ExpandoObject item);
		IDeleteBuilder Delete(string tableName);
		IDeleteBuilder<T> Delete<T>(string tableName, T item);
		IStoredProcedureBuilder StoredProcedure(string storedProcedureName);
		IStoredProcedureBuilder MultiResultStoredProcedure(string storedProcedureName);
		IStoredProcedureBuilder<T> StoredProcedure<T>(string storedProcedureName, T item);
		IStoredProcedureBuilder<T> MultiResultStoredProcedure<T>(string storedProcedureName, T item);
		IStoredProcedureBuilderDynamic StoredProcedure(string storedProcedureName, ExpandoObject item);
		IStoredProcedureBuilderDynamic MultiResultStoredProcedure(string storedProcedureName, ExpandoObject item);
		IDbContext EntityFactory(IEntityFactory entityFactory);
		IDbContext ConnectionString(string connectionString, DbProviderTypes dbProviderType);
		IDbContext ConnectionString(string connectionString, IDbProvider dbProvider);
		IDbContext ConnectionStringName(string connectionstringName, DbProviderTypes dbProviderType);
		IDbContext ConnectionStringName(string connectionstringName, IDbProvider dbProvider);
		IDbContext IsolationLevel(IsolationLevel isolationLevel);
		IDbContext Commit();
		IDbContext Rollback();
	}
}