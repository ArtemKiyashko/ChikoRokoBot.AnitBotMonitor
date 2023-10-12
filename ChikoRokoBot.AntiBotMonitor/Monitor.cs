using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using ChikoRokoBot.AntiBotMonitor.Interfaces;
using ChikoRokoBot.AntiBotMonitor.Models;
using ChikoRokoBot.AntiBotMonitor.Options;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChikoRokoBot.AntiBotMonitor
{
    public class Monitor
    {
        private readonly IHttpClient _httpClient;
        private readonly INotificationManager _notificationManager;
        private readonly TableClient _siteStateTableClient;
        private readonly AntiBotMonitorOptions _antiBotMonitorOptions;

        public Monitor(
            IHttpClient httpClient,
            INotificationManager notificationManager,
            TableServiceClient tableServiceClient,
            IOptions<AntiBotMonitorOptions> options)
        {
            _httpClient = httpClient;
            _notificationManager = notificationManager;
            _antiBotMonitorOptions = options.Value;

            _siteStateTableClient = tableServiceClient.GetTableClient(_antiBotMonitorOptions.SiteStateTableName);
            _siteStateTableClient.CreateIfNotExists();
        }

        [FunctionName("Monitor")]
        [ExponentialBackoffRetry(3, "00:00:10", "00:01:00")]
        public async Task Run([TimerTrigger("0 */3 * * * *", RunOnStartup = true)]TimerInfo myTimer, ILogger log)
        {
            TargetSiteState targetSiteState;

            var currentStateResponse = await _siteStateTableClient.GetEntityIfExistsAsync<TargetSiteState>(_antiBotMonitorOptions.SiteStateTablePartition, _httpClient.PingUri.Host);
            targetSiteState = GetTargetSiteState(currentStateResponse);
            var antiBotEnabled = await _httpClient.IsAntiBotEnabled();

            if (!targetSiteState.AntiBotEnabled && antiBotEnabled)
            {
                targetSiteState.AntiBotEnabled = true;
                await _notificationManager.NotifyAllUsers(targetSiteState);
            }

            if (!antiBotEnabled)
                targetSiteState.AntiBotEnabled = false;

            await _siteStateTableClient.UpsertEntityAsync(targetSiteState, TableUpdateMode.Replace);
        }

        private TargetSiteState GetTargetSiteState(NullableResponse<TargetSiteState> currentStateResponse)
        {
            TargetSiteState targetSiteState;

            if (currentStateResponse.HasValue)
            {
                targetSiteState = currentStateResponse.Value;
            }
            else
            {
                targetSiteState = new TargetSiteState
                {
                    RowKey = _httpClient.PingUri.Host,
                    PartitionKey = _antiBotMonitorOptions.SiteStateTablePartition,
                    Url = _httpClient.PingUri.AbsoluteUri
                };
            }

            return targetSiteState;
        }
    }
}

