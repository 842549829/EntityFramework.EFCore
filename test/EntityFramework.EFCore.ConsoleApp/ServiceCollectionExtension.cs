using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EntityFramework.EFCore.ConsoleApp.Domain;
using EntityFramework.EFCore.Entity.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EntityFramework.EFCore.ConsoleApp
{
    public class ServiceCollectionExtension
    {
        private const string AppSettings = "appsettings";

        public static IServiceProvider BuildDefaultServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();
            var containerBuilder = new ContainerBuilder();
            // 配置文件
            var config = new ConfigurationBuilder()
                .AddJsonFile($"{AppSettings}.json", true, true)
                .Build();
            services.AddSingleton(typeof(IConfiguration), config);
            //数据库链接
            services.AddDbContext<OrderDbContext>(option =>
            {
                option.UseMySql(config.GetConnectionString("Db"));
            });
            services.AddScoped<IService, Service>();
            services.AddLogging(loggingBuilder =>
            {
                // 注意一定要添加Logging
                loggingBuilder.AddConfiguration(config.GetSection("Logging"));
                loggingBuilder.AddLog4Net();
            });
            containerBuilder.Populate(services);
            return new AutofacServiceProvider(containerBuilder.Build());
        }
    }
}