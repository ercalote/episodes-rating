using System;
using EpisodesRatingModel;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace EpisodesRatingSchedulerFunctionApp
{
    public static class Scheduler
    {
        static string connectionString = "Endpoint=sb://episodesrating.servicebus.windows.net/;SharedAccessKeyName=send;SharedAccessKey=kxv7Cf04DrM+jVzBM2H+aho591xfb3BgstkzByin3XM=;EntityPath=episodesrating-series-updates";

        static string queueName = "episodesrating-series-updates";

        [FunctionName("EpisodesRatingScheduler")]
        public static async void Run(
            [TimerTrigger("0 0 5 * * *"
            #if DEBUG
            , RunOnStartup=true
            #endif
            )]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            using (var dbContext = new EpisodesRatingContext())
            {
                var seriesToUpdate = dbContext.Series
                    .Where(s => DateTime.UtcNow.AddDays(-30) > s.LastUpdated)
                    .OrderBy(s => s.LastUpdated)
                    .Take(50)
                    .Select(SchedulerRequest.FromSeries)
                    .ToList();

                await using (ServiceBusClient client = new ServiceBusClient(connectionString))
                {
                    ServiceBusSender sender = client.CreateSender(queueName);

                    foreach (var series in seriesToUpdate)
                    {
                        string jsonEntity = JsonSerializer.Serialize(series);
                        ServiceBusMessage serializedContents = new ServiceBusMessage(jsonEntity);
                        await sender.SendMessageAsync(serializedContents);
                    }
                }
            }
        }
    }
}
