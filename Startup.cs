using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using tourismAPi.App_Code;

namespace tourismAPi
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
            string[] items = new string[] { };
            items = new corsorigins().connectionString();
            services.AddCors(options =>
            {
                options.AddPolicy("Client",
                    builder => builder.WithOrigins(items[0], items[1], items[2], items[3], items[4]).WithHeaders("Accept").WithMethods("GET"));
                options.AddPolicy("Insert",
                    builder => builder.WithOrigins(items[0], items[1], items[2], items[3], items[4]).WithHeaders("Accept").WithMethods("POST"));
                options.AddPolicy("WeatherForecast",
                    builder => builder.WithOrigins(items[0], items[1], items[2], items[3], items[4]).WithHeaders("Accept").WithMethods("GET", "POST"));
            });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
