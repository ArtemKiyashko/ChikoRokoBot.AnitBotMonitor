using System;
using Azure;
using Azure.Data.Tables;

namespace ChikoRokoBot.AntiBotMonitor.Models
{
	public class TargetSiteState : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public bool AntiBotEnabled { get; set; }
		public string Url { get; set; }
	}
}

