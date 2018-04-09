using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using log4net;
using log4net.Repository;
using log4net.Core;
using log4net.Config;

namespace TableStoreWebApp
{
    public class Startup
    {
        public static ILoggerRepository repository { get; set; }
        public static string SqlConnecting { get; set; }
        public Startup(IHostingEnvironment env)
        {
            var builder=new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)//增加环境配置文件，新建项目默认有;当debug模式运行调用的是 appsettings.Development.json配置内容
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            repository = LoggerManager.CreateRepository("NETCoreRepsitory");
            SqlConnecting = Configuration.GetConnectionString("DefaultConnection");
            XmlConfigurator.Configure(repository, new System.IO.FileInfo("log4net.config"));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.Configure<Models.TableStoreModel>(Configuration.GetSection("TableStore"));

            services.AddSingleton<Repository.IUserRepository, Repository.UserRepository>();
            services.AddSingleton<Services.IUserServices, Services.UserServices>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var log = LogManager.GetLogger(repository.Name,typeof(Startup));
            log.Info("开始执行...");
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
