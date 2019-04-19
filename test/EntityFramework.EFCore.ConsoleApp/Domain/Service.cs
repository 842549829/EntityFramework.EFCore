using System;
using System.Collections.Generic;
using System.Linq;
using EntityFramework.EFCore.Entity;
using EntityFramework.EFCore.Entity.Db;
using EntityFramework.EFCore.Entity.Db.Extensions;
using EntityFramework.EFCore.Entity.Filter;
using Microsoft.EntityFrameworkCore;
using EntityFramework.EFCore.ConsoleApp.Extensions;
using EntityFramework.EFCore.Entity.Data;
using EntityFramework.EFCore.Entity.Db.Query;
using Microsoft.Extensions.Logging;
using Z.EntityFramework.Plus;

namespace EntityFramework.EFCore.ConsoleApp.Domain
{
    public class Service : IService
    {
        private readonly OrderDbContext _context;

        public Service(OrderDbContext context, ILogger<Service> logger)
        {
            _context = context;
            logger.LogDebug("文本");
        }

        /// <summary>
        /// 添加单条数据
        /// </summary>
        /// <returns>结果</returns>
        public int InsertWithSql()
        {
            _context.Orders.Add(new Orders { Id = Guid.NewGuid().ToString(), ProductId = "2", Remake = "dd", Status = 1 });
            return _context.SaveChanges();
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <returns>结果</returns>
        public int InsertWithSqlList()
        {
            _context.Orders.AddRange(
                new Orders { Id = Guid.NewGuid().ToString(), ProductId = "2", Remake = "dd", Status = 1 },
                new Orders { Id = Guid.NewGuid().ToString(), ProductId = "2", Remake = "dd1", Status = 12 });
            return _context.SaveChanges();
        }

        /// <summary>
        ///  多表添加
        /// </summary>
        /// <returns>结果</returns>
        public int InsertWithSqlLists()
        {
            using (var tran = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Orders.AddRange(
                        new Orders { Id = Guid.NewGuid().ToString(), ProductId = "2", Remake = "dd", Status = 1 },
                        new Orders { Id = Guid.NewGuid().ToString(), ProductId = "1", Remake = "dd2", Status = 1 },
                        new Orders { Id = Guid.NewGuid().ToString(), ProductId = "1", Remake = "dds", Status = 1 },
                        new Orders { Id = Guid.NewGuid().ToString(), ProductId = "2", Remake = "dd1", Status = 12 });
                    _context.Product.Add(new Product { Id = Guid.NewGuid().ToString(), Name = "2", Price = 0.5M });
                    var r = _context.SaveChanges();
                    tran.Commit();
                    return r;
                }
                catch (Exception)
                {
                    tran.Rollback();
                    return 0;
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns>结果</returns>
        public int DeleteWithSql()
        {
            var up = _context.Orders.AsNoTracking().Where(d => d.ProductId == "2").Delete();
            return up;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns>结果</returns>
        public int UpdateWithSql()
        {
            var up = _context.Orders.AsNoTracking().Where(d => d.ProductId == "2").Update(d => new Orders { Remake = "开发就f" });
            return up;
        }

        /// <summary>
        /// 单条数据查询
        /// </summary>
        /// <returns>Order</returns>
        public Orders Query()
        {
            var order = _context.Orders.AsNoTracking().Select(d => new OrdersExtensions
            {
                Id = d.Id,
                ProductId = d.ProductId,
                Remake = d.Remake,
                Status = d.Status,
                Product = _context.Product.AsNoTracking().Where(p => p.Id == d.ProductId)
            }).FirstOrDefault(d => d.Id == "1");
            //string sql = order.ToSql();
            return order;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns>Order</returns>
        public IEnumerable<Orders> QueryOrders()
        {
            var order = _context.Orders.AsNoTracking().Select(d => new OrdersExtensions
            {
                Id = d.Id,
                ProductId = d.ProductId,
                Remake = d.Remake,
                Status = d.Status,
                Product = _context.Product.AsNoTracking().Where(p => p.Id == d.ProductId)
            }).Where(d => d.Id == "1");
            string sql = order.ToSql();
            return order;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="filter">订单查询条件</param>
        /// <returns>结果集</returns>
        public Pagination<OrdersPagination> QueryPaging(OrderFilter filter)
        {
            var filters = PredicateBuilder.True<OrdersPagination>()
                .AndIf(!string.IsNullOrWhiteSpace(filter.ProductName), d => d.ProductName == filter.ProductName)
                .AndIf(!string.IsNullOrWhiteSpace(filter.Remake), d => d.Remake == filter.Remake);
            var query = (from order in _context.Orders.AsNoTracking()
                         join product in _context.Product.AsNoTracking() on order.ProductId equals product.Id
                         select new OrdersPagination
                         {
                             Id = order.Id,
                             Remake = order.Remake,
                             ProductName = product.Name,
                             Status = order.Status
                         })
                .SortBy(filter.Sort, (int)filter.SortType)
                .Where(filters);
            var count = query.Count();
            var data = query.Skip(filter.Skip).Take(filter.Take);
            var result = new Pagination<OrdersPagination>
            {
                Count = count,
                Data = data
            };
            string sql = query.ToSql();
            string dataSql = data.ToSql();
            return result;
        }
    }
}
