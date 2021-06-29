using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace APIHealthCheck.HealthChecks.Processor
{
    public class ProcessorHealthCheck : IHealthCheck
    {
        private readonly IConfiguration config;

        public ProcessorHealthCheck(IConfiguration configuration)
        {
            config = configuration;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            /*
                Pseudocode:
                    1. Get the value of the processor load threshold from the appsettings.Environment.json file.
                    2. Evaluate the current processor load against the threshold load, and provide appropriate responses.
             */

            IConfigurationSection processorHealthCheck = config.GetSection("ProcessorHealthCheck");
            uint maxCPUAllocationInPercentage = processorHealthCheck.GetValue<uint>("MaxCPUAllocationInPercentage");
            uint thresholdCPULoadPercentage = processorHealthCheck.GetValue<uint>("ThresholdCPULoadPercentage");

            string healthyDescription = $"Processor load is below the threshold of {maxCPUAllocationInPercentage}%";
            string degradedDescription = $"Processor load is within {thresholdCPULoadPercentage}% of the set max CPU load of {maxCPUAllocationInPercentage}%";
            string unhealthyDescription = $"Processor load has crossed the threshold of {maxCPUAllocationInPercentage}%";

            double processorLoad = 0.0;

            try
            {
                using (Process proc = Process.GetCurrentProcess())
                {
                    /*
                        Following code segment which calculates the CPU percentage load
                        has been taken from 
                        https://medium.com/@jackwild/getting-cpu-usage-in-net-core-7ef825831b8b
                     */
                    var startTime = DateTime.UtcNow;
                    var startCpuUsage = proc.TotalProcessorTime;

                    await Task.Delay(500);

                    var endTime = DateTime.UtcNow;
                    var endCpuUsage = proc.TotalProcessorTime;

                    var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
                    var totalMsPassed = (endTime - startTime).TotalMilliseconds;
                    var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * cpuUsedMs);

                    processorLoad = cpuUsageTotal * 100;
                }

                if (processorLoad < maxCPUAllocationInPercentage && 
                    processorLoad < maxCPUAllocationInPercentage * thresholdCPULoadPercentage/100)
                {
                    return HealthCheckResult.Healthy(healthyDescription);
                }
                else if (processorLoad < maxCPUAllocationInPercentage &&
                    processorLoad > maxCPUAllocationInPercentage * thresholdCPULoadPercentage / 100)
                {
                    return HealthCheckResult.Degraded(degradedDescription);
                }
                else
                {
                    return HealthCheckResult.Unhealthy(unhealthyDescription);
                }
            }
            catch (Exception exception)
            {
                return HealthCheckResult.Unhealthy(exception.Message, exception, null);
            }
        }
    }
}
