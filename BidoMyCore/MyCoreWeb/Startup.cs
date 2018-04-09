using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using log4net.Repository;
using log4net.Core;
using log4net.Config;
using log4net;
using System.IO;
using Microsoft.Extensions.Logging;

namespace MyCoreWeb
{
    using MyCoreWeb.Services;

    public class Startup
    {
        public static ILoggerRepository repository { get; set; }

        public Startup(IHostingEnvironment env)//(IConfiguration configuration)
        {
            //Configuration = configuration;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange:true)//增加环境配置文件，新建项目默认有;当debug模式运行调用的是 appsettings.Development.json配置内容
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            repository = LoggerManager.CreateRepository("NETCoreRepsitory");
            XmlConfigurator.Configure(repository,new FileInfo("log4net.config"));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();//Add FrameWork services

            services.AddOptions();//添加options
            services.Configure<Models.ClassConifg>(Configuration.GetSection("ClassConfig"));

            #region 注册服务的N种方式

            services.AddSingleton<Repository.IUserRepository, Repository.UserRepository>();
            services.AddSingleton<Services.IUserServices, Services.UserServices>();

            //services.Add(ServiceDescriptor.Singleton<IUserServices, UserServices>());

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var data = Configuration["Data"];

            //两种读取方式
            var sqlConnStr = Configuration.GetConnectionString("DefaultConnection");
            //var sqlConnStr = Configuration["ConnectionStrings:DefaultConnection"];


            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            //else
            //{
            //    app.UseExceptionHandler("/Error");
            //}

            app.UseMvcWithDefaultRoute();

            //app.UseStaticFiles();

                        
            ////添加MVC中间件
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});
        }
    }
}
