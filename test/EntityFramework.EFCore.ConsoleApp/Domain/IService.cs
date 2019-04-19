using System.Collections.Generic;
using EntityFramework.EFCore.Entity.Data;
using EntityFramework.EFCore.Entity.Db;
using EntityFramework.EFCore.Entity.Db.Extensions;
using EntityFramework.EFCore.Entity.Db.Query;
using EntityFramework.EFCore.Entity.Filter;

namespace EntityFramework.EFCore.ConsoleApp.Domain
{
    public interface IService
    {
        /// <summary>
        /// 添加单条数据
        /// </summary>
        /// <returns>结果</returns>
        int InsertWithSql();

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <returns>结果</returns>
        int InsertWithSqlList();

        /// <summary>
        ///  多表添加
        /// </summary>
        /// <returns>结果</returns>
        int InsertWithSqlLists();

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns>结果</returns>
        int DeleteWithSql();

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns>结果</returns>
        int UpdateWithSql();

        /// <summary>
        /// 单条数据查询
        /// </summary>
        /// <returns>Order</returns>
        Orders Query();

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns>Order</returns>
        IEnumerable<Orders> QueryOrders();

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="filter">订单查询条件</param>
        /// <returns>结果集</returns>
        Pagination<OrdersPagination> QueryPaging(OrderFilter filter);
    }
}