using APIHealthCheck.Repository;
using APIHealthCheck.Repository.Interfaces;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ThinkingCog.AspNetCore.DiskHealthCheck;
using ThinkingCog.AspNetCore.MemoryHealthCheck;
using ThinkingCog.AspNetCore.ProcessorLoadHealthCheck;
using ThinkingCog.AspNetCore.SQLServerHealthCheck;
using ThinkingCog.AspNetCore.URLHealthCheck;

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

            var diskHealthCheckSettingsFromConfig = Configuration.GetSection("DiskHealthCheckSettings").Get<DiskHealthCheckOptions>();
            var sqlServerHealthCheckSettingsFromConfig = Configuration.GetSection("SQLServerHealthCheckSettings").Get<SQLServerHealthCheckOptions>();
            var memoryHealthCheckSettingsFromConfig = Configuration.GetSection("MemoryHealthCheckSettings").Get<MemoryHealthCheckOptions>();
            var processorHealthCheckSettingsFromConfig = Configuration.GetSection("ProcessorLoadHealthCheckSettings").Get<ProcessorLoadHealthCheckOptions>(); 
            var urlHealthCheckSettingsFromConfig = Configuration.GetSection("URLHealthCheckSettings").Get<URLHealthCheckOptions>();

            services.AddHealthChecks()
                    .AddDiskHealthCheck(
                                diskHealthCheckSettings: diskHealthCheckSettingsFromConfig,
                                name: "Disk Health Check",
                                failureStatus: HealthStatus.Unhealthy,
                                tags: new string[] { "disk health check", "hard disk health check", "quickcheck" })
                    .AddSQLServerHealthCheck(
                                sqlServerHealthCheckSettings: sqlServerHealthCheckSettingsFromConfig,
                                name: "SQL Server Health Check",
                                failureStatus: HealthStatus.Unhealthy,
                                tags: new string[] { "sql server", "database" })
                    .AddMemoryHealthCheck(
                                memoryHealthCheckSettings: memoryHealthCheckSettingsFromConfig,
                                name: "Memory Health Check",
                                failureStatus: HealthStatus.Unhealthy,
                                tags: new string[] { "memory health check", "memory", "quickcheck" })
                    .AddProcessorLoadhealthCheck(
                                processorLoadHealthCheckSettings: processorHealthCheckSettingsFromConfig,
                                name: "Processor Load Health Check",
                                failureStatus: HealthStatus.Unhealthy,
                                tags: new string[] { "processor health", "processor load", "quickcheck" })
                    .AddURLHealthCheck(
                                urlHealthCheckSettings: urlHealthCheckSettingsFromConfig,
                                name: "URL Health Check",
                                failureStatus: HealthStatus.Unhealthy,
                                tags: new string[] { "url", "url health check", "quickcheck" });

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
                    Predicate = (check) => (check.Tags.Contains("quickcheck")),
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
