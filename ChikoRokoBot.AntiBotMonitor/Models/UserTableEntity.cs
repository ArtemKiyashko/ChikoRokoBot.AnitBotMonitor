using System;
using Azure;
using Azure.Data.Tables;

namespace ChikoRokoBot.AntiBotMonitor.Models
{
	public class UserTableEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public long? ChatId { get; set; }
        public int? TopicId { get; set; }
    }
}

