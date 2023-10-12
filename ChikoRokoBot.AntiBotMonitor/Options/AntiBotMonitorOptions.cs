using System;
namespace ChikoRokoBot.AntiBotMonitor.Options
{
	public class AntiBotMonitorOptions
	{
        public string StorageAccount { get; set; } = "UseDevelopmentStorage=true";
		public string QueueName { get; set; } = "notifyantibot";
        public string UsersTableName { get; set; } = "users";
        public string UsersTablePartition { get; set; } = "primary";
        public string SiteStateTableName { get; set; } = "sitestate";
        public string SiteStateTablePartition { get; set; } = "primary";
		public int MessageTtlMinutes { get; set; } = 3;
    }
}

