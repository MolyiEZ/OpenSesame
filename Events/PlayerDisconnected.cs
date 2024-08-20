using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Eventing;
using OpenMod.API.Plugins;
using OpenMod.Core.Eventing;
using OpenMod.Unturned.Players.Connections.Events;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Molyi.OpenSesame.Events
{
	[EventListenerLifetime(ServiceLifetime.Singleton)]
	public class PlayerDisconnected(
		IPluginAccessor<OpenSesamePlugin> plugin) : IEventListener<UnturnedPlayerDisconnectedEvent>
	{
		private Dictionary<CSteamID, BarricadeDrop> playerHorn = plugin.Instance?.playerHorn!;

		[EventListener(Priority = EventListenerPriority.Highest)]
		public Task HandleEventAsync(object? sender, UnturnedPlayerDisconnectedEvent @event)
		{
			playerHorn.Remove(@event.Player.SteamId);
			return Task.CompletedTask;
		}
	}
}
