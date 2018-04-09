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
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange:true)//���ӻ��������ļ����½���ĿĬ����;��debugģʽ���е��õ��� appsettings.Development.json��������
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

            services.AddOptions();//���options
            services.Configure<Models.ClassConifg>(Configuration.GetSection("ClassConfig"));

            #region ע������N�ַ�ʽ

            services.AddSingleton<Repository.IUserRepository, Repository.UserRepository>();
            services.AddSingleton<Services.IUserServices, Services.UserServices>();

            //services.Add(ServiceDescriptor.Singleton<IUserServices, UserServices>());

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var data = Configuration["Data"];

            //���ֶ�ȡ��ʽ
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

                        
            ////���MVC�м��
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});
        }
    }
}
