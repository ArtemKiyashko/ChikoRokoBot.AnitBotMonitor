using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ChikoRokoBot.AntiBotMonitor.Options;
using Microsoft.Extensions.Options;

namespace ChikoRokoBot.AntiBotMonitor.Clients
{
	public class ChikoRokoClient
    {
        private readonly HttpClient _httpClient;
        private readonly ChikoRokoClientOptions _options;

        public ChikoRokoClient(HttpClient httpClient, IOptions<ChikoRokoClientOptions> options)
		{
            _httpClient = httpClient;
            _options = options.Value;

            _httpClient.BaseAddress = _options.TargetUri;
        }

        public Uri PingUri => _httpClient.BaseAddress;

        public async Task<bool> IsAntiBotEnabled()
        {
            var response = await _httpClient.GetAsync((string)null);

            return response.StatusCode == HttpStatusCode.Forbidden;
        }
    }
}

