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
using Microsoft.Extensions.Options;
using Octokit;
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
            services.AddMemoryCache();

            services.AddScoped<ProjectInfoService>();
            services.AddScoped<ScrubProjectInfoService>();

            services.AddScoped(sp =>
            {
                var githubOptions = sp.GetRequiredService<IOptions<GitHubOptinos>>();
                var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("GitHubClientFactory");

                var client = new GitHubClient(new ProductHeaderValue(githubOptions.Value.ProductHeader));
                if (string.IsNullOrEmpty(githubOptions.Value.OAuthToken))
                {
                    logger.LogWarning("Using github unauthenticated client with limits, please provide OAuthToken token in options");
                }
                else
                {
                    client.Credentials = new Credentials(githubOptions.Value.OAuthToken);
                }
                return client;
            });

            if (Configuration.GetValue<bool>("SCRUB_GITHUB_PROJECTS"))
            {
                services.AddHostedService<ScrubProjectsService>();
            }

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseWebAppConfigure();
            app.UseCors(config => config
                .AllowAnyMethod()
                .WithOrigins("http://localhost:3000"));
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
