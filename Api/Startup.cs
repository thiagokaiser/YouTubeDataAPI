using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using Core.Services;
using Core.Interfaces;
using Api.Apis;
using Infrastructure.Repositories;

namespace YouTubeDataAPI
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
            string connectionString = "User ID=postgres;Password='mypass';Host=localhost;Port=5432;Database=YoutubeDB;Pooling=true;";
            var youtubeApiKey = Configuration["Youtube:ApiKey"];

            services.AddControllersWithViews();

            services.AddScoped<YouTubeService>(x => new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = youtubeApiKey
                })                
            );            
            services.AddScoped<YoutubeService>();
            services.AddScoped<PlaylistService>();
            services.AddScoped<IRepositoryPlaylist>(x => new PlaylistRepository(connectionString));
            services.AddScoped<IRepositoryYoutube>(x => new YoutubeRepository(connectionString));
            services.AddScoped<YoutubeApi>();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=YouTube}/{action=Index}/{id?}");
            });
        }
    }
}
