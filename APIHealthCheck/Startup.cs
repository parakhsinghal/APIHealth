using APIHealthCheck.Repository;
using APIHealthCheck.Repository.Interfaces;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using APIHealthCheck.HealthChecks.SQLServer;
using Microsoft.AspNetCore.Http;
using APIHealthCheck.HealthChecks.Disk;
using APIHealthCheck.HealthChecks.Memory;
using APIHealthCheck.HealthChecks.Processor;
using APIHealthCheck.HealthChecks.URL;

namespace APIHealthCheck
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIHealthCheck", Version = "v1" });
            });

            services.AddHealthChecks()
                     .AddCheck<SQLServerHealthCheck>("LibraryDB health check", HealthStatus.Unhealthy, tags: new string[] { "database", "db", "SQL Server", "sql", "detailed check"})
                     .AddCheck<DiskHealthCheck>("Disk Health Check", HealthStatus.Unhealthy, tags: new string[] { "disk", "filesystem", "detailed check" })
                     .AddCheck<MemoryHealthCheck>("Memory Health Check", HealthStatus.Unhealthy, tags: new string[] { "memory", "total memory allocated", "detailed check" })
                     .AddCheck<ProcessorHealthCheck>("Processor Health Check", HealthStatus.Unhealthy, tags: new string[] { "processor health", "processor load", "detailed check" })
                     .AddCheck<URLHealthCheck>("URL Health Check", HealthStatus.Unhealthy, tags: new string[] { "url health check", "quickcheck"});
            
            services.AddScoped<ILibraryRepository, SQLServerRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "APIHealthCheck v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/api/health/quickcheck", new HealthCheckOptions()
                {
                    Predicate = (check)=>(check.Tags.Contains("quickcheck")),
                    AllowCachingResponses = false,
                    ResultStatusCodes =  { 
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    }
                });
                endpoints.MapHealthChecks("/api/health/detailedcheck", new HealthCheckOptions()
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                    AllowCachingResponses = false,
                    ResultStatusCodes =  {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    }
                });
            });
        }
    }
}
