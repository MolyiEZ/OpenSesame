using Microsoft.Extensions.DependencyInjection;
using Molyi.OpenSesame.Interfaces;
using OpenMod.API.Eventing;
using OpenMod.Core.Eventing;
using OpenMod.Unturned.Players.Life.Events;
using System.Threading.Tasks;

namespace Molyi.OpenSesame.Events
{
	[EventListenerLifetime(ServiceLifetime.Singleton)]
	public class PlayerDeath(
		IHornManager hornManager) : IEventListener<UnturnedPlayerDeathEvent>
	{
		private readonly IHornManager m_HornManager = hornManager;

		[EventListener(Priority = EventListenerPriority.Highest)]
		public Task HandleEventAsync(object? sender, UnturnedPlayerDeathEvent @event)
		{
			m_HornManager.PlayerHorn.Remove(@event.Player.SteamId);
			return Task.CompletedTask;
		}
	}
}
