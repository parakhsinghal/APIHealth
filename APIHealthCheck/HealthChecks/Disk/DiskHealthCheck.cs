using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace APIHealthCheck.HealthChecks.Disk
{
    public class DiskHealthCheck : IHealthCheck
    {
        private readonly IConfiguration configuration;

        public DiskHealthCheck(IConfiguration config)
        {
            configuration = config;
        }


        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            /*
             Pseudocode:
                1. Pick the disk location, folder where the test file needs to be created and the name of the file 
                   that needs to be created from the appsettings.Environment.json file.
                2. Create the file at the desired location.
                3. Based on the result and response times, create the HealthCheckResult values and pass back the 
                   response.
            */

            IConfigurationSection diskHealthCheck = configuration.GetSection("DiskHealthCheck");

            string diskName = diskHealthCheck.GetValue<string>("DiskLocation");
            string folderName = diskHealthCheck.GetValue<string>("FolderName");
            string fileName = diskHealthCheck.GetValue<string>("FileName");
            string folderLocation = Path.Combine(new string[] { diskName, folderName });
            string fileLocation = Path.Combine(new string[] { diskName, folderName, fileName });

            uint healthyUpperBound = diskHealthCheck.GetValue<uint>("HealthyUpperBoundInMilliseconds");
            uint degradedLowerbound = diskHealthCheck.GetValue<uint>("DegradedLowerBoundInMilliseconds");
            uint degradedUpperBound = diskHealthCheck.GetValue<uint>("DegradedUpperBoundInMilliseconds");

            string healthyDescription = $"The disk {diskName} is working normally.";
            string degradedDescription = $"The disk {diskName} is in an un-optimal state.";
            string unhealthyDescription = $"The disk {diskName} is not available or not functioning as expected.";

            Stopwatch timer = new Stopwatch();

            try
            {
                /*
                    1. Check for the existence of the disk.
                    2. Check for the existence of the folder location. If not available, create one.
                    3. Write the file. If already available, over-write the existing file.
                 */

                DriveInfo driveObject = new DriveInfo(diskName);

                if (driveObject != null)
                {
                    if (Directory.Exists(folderLocation))
                    {
                        FileStream fileStream = File.Create(fileLocation);
                        fileStream.Close();
                        File.Delete(fileLocation);
                    }
                    else
                    {
                        Directory.CreateDirectory(folderLocation);
                        FileStream fileStream = File.Create(fileLocation);
                        fileStream.Close();
                        File.Delete(fileLocation);
                    }

                    if (timer.ElapsedMilliseconds <= healthyUpperBound)
                    {
                        return Task.FromResult(HealthCheckResult.Healthy(healthyDescription));
                    }
                    else if (timer.ElapsedMilliseconds >= degradedLowerbound && timer.ElapsedMilliseconds <= degradedUpperBound)
                    {
                        return Task.FromResult(HealthCheckResult.Degraded(degradedDescription));
                    }
                    else
                    {
                        return Task.FromResult(HealthCheckResult.Unhealthy(unhealthyDescription));
                    }
                }
                else
                {
                    IOException exception = new IOException("The drive location does not exists or is not accessible");

                    return Task.FromResult(HealthCheckResult.Unhealthy(exception.Message, exception, null));
                }

            }
            catch (IOException exception)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(exception.Message, exception, null));
            }
            catch (Exception exception)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(exception.Message, exception, null));
            }
        }
    }
}