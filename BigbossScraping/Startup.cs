using System;
using System.Net.Mime;
using System.Reflection;
using BigbossScraping.Extensions;
using BigbossScraping.Helper;
using BigbossScraping.Infrastructures;
using BigbossScraping.Infrastructures.Filters;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BigbossScraping
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().ConfigureApiBehaviorOptions(options => options.InvalidModelStateResponseFactory =
                context =>
                {
                    var result = new BadRequestObjectResult(context.ModelState);

                    result.ContentTypes.Add(MediaTypeNames.Application.Json);
                    result.ContentTypes.Add(MediaTypeNames.Application.Xml);

                    return result;
                });
            services.AddSwagger()
                .AddSerilog(configuration: Configuration)
                .AddSqlContext(configuration: Configuration)
                .LoadModules(configuration: Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
                c.SwaggerEndpoint("/swagger/v1/swagger.json",
                    $"{Assembly.GetExecutingAssembly().GetName().Name} - V1"));

            app.UseRouting();
            // app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            //app.UseHangfireDashboard("/dashboard",
            //    new DashboardOptions { Authorization = new[] { new HangfireDashboardAuthFilter() } });
            //GlobalConfiguration.Configuration.UseActivator(new HangfireActivator(serviceProvider));
        }
    }
}
