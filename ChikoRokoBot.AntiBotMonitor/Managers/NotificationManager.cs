using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using ChikoRokoBot.AntiBotMonitor.Interfaces;
using ChikoRokoBot.AntiBotMonitor.Models;
using ChikoRokoBot.AntiBotMonitor.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChikoRokoBot.AntiBotMonitor.Managers
{
	public class NotificationManager : INotificationManager
	{
        private readonly TableClient _usersTableClient;
        private readonly AntiBotMonitorOptions _antiBotMonitorOptions;
        private readonly QueueClient _queueClient;
        private readonly ILogger<NotificationManager> _logger;

        public NotificationManager(
            TableServiceClient tableServiceClient,
            QueueClient queueClient,
            IOptions<AntiBotMonitorOptions> options,
            ILogger<NotificationManager> logger)
		{
            _antiBotMonitorOptions = options.Value;

            _usersTableClient = tableServiceClient.GetTableClient(_antiBotMonitorOptions.UsersTableName);
            _usersTableClient.CreateIfNotExists();
            _queueClient = queueClient;
            _logger = logger;
        }

        public async Task NotifyAllUsers(TargetSiteState targetSiteState)
        {
            var users = _usersTableClient.QueryAsync<UserTableEntity>(u => u.PartitionKey == _antiBotMonitorOptions.UsersTablePartition);
            await SendToAllUsers(users, targetSiteState);
        }

        private async Task SendToAllUsers(AsyncPageable<UserTableEntity> users, TargetSiteState siteState)
        {
            await foreach (var user in users)
            {
                var notification = new Notification(siteState, user);
                try
                {
                    await _queueClient.SendMessageAsync(JsonSerializer.Serialize(notification), timeToLive: TimeSpan.FromMinutes(_antiBotMonitorOptions.MessageTtlMinutes));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending to notification queue. ChatId: {0}; TopicId: {1}; TargetSite: {2}", user.ChatId, user.TopicId, siteState.RowKey);
                }
            }
        }
    }
}

