using AdminPanel.Services;
using Landing.API.Database;
using Landing.API.Models;
using Landing.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanel
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<LandingOptions>(Configuration.GetSection(nameof(LandingOptions)));

            services.AddHttpClient<ApiInteractService>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<LandingOptions>>();
                client.BaseAddress = new Uri(options.Value.ApiBaseAddress);
                client.DefaultRequestHeaders.Add("Authorization", options.Value.AdminPanelAccessToken);
            });

            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddDbContext<LandingDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("POSTGRES"), npgsql => npgsql.MigrationsAssembly("Database"))); services.AddScoped<ProjectInfoService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            app.UsePathBase("/admin");
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
