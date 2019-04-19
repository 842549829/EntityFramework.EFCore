using System;
using EntityFramework.EFCore.ConsoleApp.Domain;
using EntityFramework.EFCore.Entity.Filter;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFramework.EFCore.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = ServiceCollectionExtension.BuildDefaultServiceProvider();
            var service = serviceProvider.GetRequiredService<IService>();
            // 分页查询
            var queryPaging = service.QueryPaging(new OrderFilter{ ProductName = "1"});
            //查询 
            var data = service.Query();
            //更新
            var update = service.UpdateWithSql();
            //添加
            var add = service.InsertWithSqlLists();
            //删除
            var deleted = service.DeleteWithSql();
        }
    }
}
