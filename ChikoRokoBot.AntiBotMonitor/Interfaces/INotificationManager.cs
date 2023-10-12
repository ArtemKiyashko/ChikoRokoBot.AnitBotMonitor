using System;
using System.Threading.Tasks;
using ChikoRokoBot.AntiBotMonitor.Models;

namespace ChikoRokoBot.AntiBotMonitor.Interfaces
{
	public interface INotificationManager
	{
		public Task NotifyAllUsers(TargetSiteState targetSiteState);
	}
}

