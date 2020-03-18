using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagementMVCCoreClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagementMVCCoreClient
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDBContext>(options => options.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppDBContext>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 3;
            });

            services.AddMvc(options => {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddXmlSerializerFormatters();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                //app.UseStatusCodePages();
                //app.UseStatusCodePagesWithRedirects("/Error/{0}");
                app.UseStatusCodePagesWithReExecute("Error/{0}");
            }
        

            app.UseStaticFiles();
            app.UseAuthentication();
            //app.UseMvcWithDefaultRoute(); meme chose que celui en bas

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}"); //? montre que le id est optionnel
            }); //ceci est appeleé Conventionnal routing

            //app.UseMvc(); //pour utiliser attribute routing

            //DefaultFilesOptions dfo = new DefaultFilesOptions();
            //dfo.DefaultFileNames.Clear();
            //dfo.DefaultFileNames.Add("foo.html");

            //FileServerOptions fso = new FileServerOptions();
            //fso.DefaultFilesOptions.DefaultFileNames.Clear();
            //fso.DefaultFilesOptions.DefaultFileNames.Add("foo.html");

            //app.UseDefaultFiles(dfo); //Middleware pour afficher les fichiers par defaut : ici l'ordre est important UseDefaultFiles doit venir avant UseStaticFiles
            //app.UseStaticFiles(); //Middleware pour afficher les static files
            //les deux Middlewares ci haut peuvent etre remplacer par UseFileServer
            //app.UseFileServer();


            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
