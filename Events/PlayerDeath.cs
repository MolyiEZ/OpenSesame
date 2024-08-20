using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Eventing;
using OpenMod.API.Plugins;
using OpenMod.Core.Eventing;
using OpenMod.Unturned.Players.Life.Events;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Molyi.OpenSesame.Events
{
	[EventListenerLifetime(ServiceLifetime.Singleton)]
	public class PlayerDeath(
		IPluginAccessor<OpenSesamePlugin> plugin) : IEventListener<UnturnedPlayerDeathEvent>
	{
		private Dictionary<CSteamID, BarricadeDrop> playerHorn = plugin.Instance?.playerHorn!;

		[EventListener(Priority = EventListenerPriority.Highest)]
		public Task HandleEventAsync(object? sender, UnturnedPlayerDeathEvent @event)
		{
			playerHorn.Remove(@event.Player.SteamId);
			return Task.CompletedTask;
		}
	}
}
