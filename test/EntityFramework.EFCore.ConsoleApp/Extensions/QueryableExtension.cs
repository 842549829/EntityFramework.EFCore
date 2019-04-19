using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFramework.EFCore.ConsoleApp.Extensions
{

    /// <summary>
    /// EF下IQueryable扩展
    /// </summary>
    public static class QueryableExtensions
    {
        private static readonly TypeInfo QueryCompilerTypeInfo = typeof(QueryCompiler).GetTypeInfo();

        private static readonly FieldInfo QueryCompilerField = typeof(EntityQueryProvider).GetTypeInfo().DeclaredFields.First(x => x.Name == "_queryCompiler");

        private static readonly FieldInfo QueryModelGeneratorField = QueryCompilerTypeInfo.DeclaredFields.First(x => x.Name == "_queryModelGenerator");

        private static readonly FieldInfo DataBaseField = QueryCompilerTypeInfo.DeclaredFields.Single(x => x.Name == "_database");

        private static readonly PropertyInfo DatabaseDependenciesField = typeof(Database).GetTypeInfo().DeclaredProperties.Single(x => x.Name == "Dependencies");

        /// <summary>
        /// 生成SQL语句
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static string ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            var queryCompiler = (QueryCompiler)QueryCompilerField.GetValue(query.Provider);
            var modelGenerator = (QueryModelGenerator)QueryModelGeneratorField.GetValue(queryCompiler);
            var queryModel = modelGenerator.ParseQuery(query.Expression);
            var database = (IDatabase)DataBaseField.GetValue(queryCompiler);
            var databaseDependencies = (DatabaseDependencies)DatabaseDependenciesField.GetValue(database);
            var queryCompilationContext = databaseDependencies.QueryCompilationContextFactory.Create(false);
            var modelVisitor = (RelationalQueryModelVisitor)queryCompilationContext.CreateQueryModelVisitor();
            modelVisitor.CreateQueryExecutor<TEntity>(queryModel);
            var sql = modelVisitor.Queries.First().ToString();

            return sql;
        }

        /// <summary>
        /// 对查询结果进行排序
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="query">查询接口</param>
        /// <param name="name">字段名称</param>
        /// <param name="direction">排序方式</param>
        /// <returns></returns>
        public static IEnumerable<T> SortBy<T>(this IEnumerable<T> query, string name, int direction)
        {
            var propInfo = GetPropertyInfo(typeof(T), name);
            var expr = GetOrderExpression(typeof(T), propInfo);
            var orderMethod = direction == 0 ? "OrderBy" : "OrderByDescending";
            var method = typeof(Enumerable).GetMethods().FirstOrDefault(m => m.Name == orderMethod && m.GetParameters().Length == 2);
            var genericMethod = method.MakeGenericMethod(typeof(T), propInfo.PropertyType);
            return (IEnumerable<T>)genericMethod.Invoke(null, new object[] { query, expr.Compile() });
        }

        /// <summary>
        /// 对查询结果进行排序，默认降序
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="query">查询接口</param>
        /// <param name="name">字段名称</param>
        public static IQueryable<T> SortBy<T>(this IQueryable<T> query, string name)
        {
            return SortBy(query, name, 1);
        }

        /// <summary>
        /// 对查询结果进行排序
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="query">查询接口</param>
        /// <param name="name">字段名称</param>
        /// <param name="direction">排序方式</param>
        public static IQueryable<T> SortBy<T>(this IQueryable<T> query, string name, int direction)
        {
            var propInfo = GetPropertyInfo(typeof(T), name);
            var expr = GetOrderExpression(typeof(T), propInfo);
            var orderMethod = direction == 0 ? "OrderBy" : "OrderByDescending";
            var method = typeof(Queryable).GetMethods().FirstOrDefault(m => m.Name == orderMethod && m.GetParameters().Length == 2);
            var genericMethod = method.MakeGenericMethod(typeof(T), propInfo.PropertyType);
            return (IQueryable<T>)genericMethod.Invoke(null, new object[] { query, expr });
        }

        /// <summary>
        /// 返回查询的数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="database">数据上下文</param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">sql参数</param>
        /// <returns></returns>
        public static IEnumerable<T> SqlQuery<T>(this DatabaseFacade database, RawSqlString sql, params object[] parameters) where T : new()
        {
            var conn = database.GetDbConnection();
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }
            var command = conn.CreateCommand();
            command.CommandText = sql.Format;
            command.Parameters.AddRange(parameters);
            var reader = command.ExecuteReader();
            var properties = typeof(T).GetProperties();

            var result = new List<T>();
            while (reader.Read())
            {
                var row = new Dictionary<string, object>();
                for (var fieldCount = 0; fieldCount < reader.FieldCount; fieldCount++)
                {
                    row.Add(reader.GetName(fieldCount).ToLower(), reader[fieldCount]);
                }
                var data = new T();
                foreach (var prop in properties)
                {
                    if (row.TryGetValue(prop.Name.ToLower(), out object value))
                    {
                        prop.SetValue(data, value);
                    }
                }
                result.Add(data);
            }
            return result;
        }

        private static PropertyInfo GetPropertyInfo(Type objType, string name)
        {
            var properties = objType.GetProperties();
            var matchedProperty = properties.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (matchedProperty == null)
            {
                return properties.First();
            }

            return matchedProperty;
        }

        private static LambdaExpression GetOrderExpression(Type objType, PropertyInfo pi)
        {
            var paramExpr = Expression.Parameter(objType);
            var propAccess = Expression.PropertyOrField(paramExpr, pi.Name);
            var expr = Expression.Lambda(propAccess, paramExpr);
            return expr;
        }
    }
}
