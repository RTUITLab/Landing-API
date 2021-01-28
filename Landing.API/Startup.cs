using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Landing.API.Configure;
using Landing.API.Database;
using Landing.API.Formatting;
using Landing.API.Models;
using Landing.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RTUITLab.AspNetCore.Configure.Configure;
using RTUITLab.AspNetCore.Configure.Invokations;

namespace Landing.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<GitHubOptinos>(Configuration.GetSection(nameof(GitHubOptinos)));
            services.Configure<ScrubOptions>(Configuration.GetSection(nameof(ScrubOptions)));

            services.AddControllers();

            services.AddDbContext<LandingDbContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("POSTGRES"), npgsql => npgsql.MigrationsAssembly("Database")));
            services.AddAutoMapper(typeof(ResponseProfile));
            services.AddWebAppConfigure()
                .AddTransientConfigure<MigrationWork>();
            services.AddSingleton<ProjectsInfoCache>();
            services.AddHostedService<ScrubProjectsService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseWebAppConfigure();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
