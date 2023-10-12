using System;
using System.Threading.Tasks;

namespace ChikoRokoBot.AntiBotMonitor.Interfaces
{
	public interface IHttpClient
	{
        public Task<bool> IsAntiBotEnabled();
        public Uri PingUri { get; }
    }
}

