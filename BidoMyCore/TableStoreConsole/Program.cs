using System;

using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using TableStoreConsole.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using log4net.Appender;
using log4net.Config;
using log4net.Repository;
using log4net;

namespace TableStoreConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //var setting = new Dictionary<string, string> {
            //    { "name","huyadong"},
            //    { "age","80"}
            //};

            var builder = new ConfigurationBuilder()
                .AddCommandLine(args)
                .AddJsonFile("appSetting.json");
            var configuration = builder.Build();

            TableStoreModel.endPointPublic = configuration["TableStore:endPointPublic"];
            TableStoreModel.endPointPrivate = configuration["TableStore:endPointPrivate"];
            TableStoreModel.accessKeyID = configuration["TableStore:accessKeyID"];
            TableStoreModel.accessKeySecret = configuration["TableStore:accessKeySecret"];
            TableStoreModel.instanceName = configuration["TableStore:instanceName"];
            TableStoreModel.SqlServerConnection = configuration.GetConnectionString("DefaultConnection");

            #region 注释代码

            //Console.WriteLine($"name:{configuration["name"]}");
            //Console.WriteLine($"age:{configuration["age"]}");

            //Console.WriteLine($"数据库:{configuration.GetConnectionString("DefaultConnection")}");

            //DbContextOptions<BidoDBContext> dbContext = new DbContextOptions<BidoDBContext>();
            //DbContextOptionsBuilder<BidoDBContext> dbContextBuilder = new DbContextOptionsBuilder<BidoDBContext>();
            //BidoDBContext _dbContext = new BidoDBContext(dbContextBuilder.UseSqlServer().Options);


            //log日志处理

            //ILoggerRepository r = LogManager.CreateRepository("123123");
            //string path = @"D:\ProjectCode\C#Test\NetCore\BidoMyCore\TableStoreConsole\bin\Release\PublishOutput\log4Net.Config";
            //Console.WriteLine(path);
            //XmlConfigurator.Configure(r, new System.IO.FileInfo(path));
            //ILog log1 = LogManager.GetLogger(r.Name, "Bidohu");
            //log1.Info("-------开始执行Bidohu------");
            #endregion

            TableStoreModel.repository = LogManager.CreateRepository("NETCoreRepository");
            XmlConfigurator.Configure(TableStoreModel.repository, new System.IO.FileInfo("log4Net.Config"));
            ILog log = LogManager.GetLogger(TableStoreModel.repository.Name,"Bido");
            log.Info("-------开始执行Bido123------");
            TableStoreHelper tb = new TableStoreHelper();
            //tb.MainData();

            log.Info("---------------END--------------");
            Console.WriteLine("---------------END--------------");

            Console.ReadLine();
        }
    }
}
