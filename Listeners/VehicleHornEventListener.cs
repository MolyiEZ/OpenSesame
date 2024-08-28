using HarmonyLib;
using Microsoft.Extensions.DependencyInjection;
using Molyi.OpenSesame.Interfaces;
using Molyi.OpenSesame.Models.Events;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Users;
using SDG.NetTransport;
using SDG.Unturned;
using System;

namespace Molyi.OpenSesame.Listeners
{
	[PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
	public class VehicleHornEventListener(
		IPluginAccessor<OpenSesamePlugin> plugin,
		IEventBus eventBus,
		IUnturnedUserDirectory userDirectory) : IUnturnedEventListener, IDisposable
	{
		private readonly IPluginAccessor<OpenSesamePlugin> m_Plugin = plugin;
		private readonly IEventBus m_EventBus = eventBus;
		private readonly IUnturnedUserDirectory m_UserDirectory = userDirectory;

		public void Subscribe() => OnVehicleHorn += Events_OnVehicleHorn;
		public void Dispose() => OnVehicleHorn -= Events_OnVehicleHorn;

		public void Events_OnVehicleHorn(Player player, InteractableVehicle vehicle, ref bool isCancelled)
		{
			var @event = new UnturnedVehicleHornEvent(m_UserDirectory.GetUser(player), vehicle);
			m_EventBus.EmitAsync(m_Plugin.Instance!, null, @event);

			isCancelled = @event.IsCancelled;
		}

		private delegate void VehicleHorn(Player player, InteractableVehicle vehicle, ref bool isCancelled);
		private static event VehicleHorn? OnVehicleHorn;

		[HarmonyPatch]
		public static class Patches
		{
			private static readonly ClientStaticMethod<uint> SendVehicleHorn = ClientStaticMethod<uint>.Get(VehicleManager.ReceiveVehicleHorn);

			[HarmonyPatch(typeof(VehicleManager), "ReceiveVehicleHornRequest")]
			[HarmonyPrefix]
			public static bool ReceiveVehicleHornRequest(in ServerInvocationContext context)
			{
				bool isCancelled = false;
				Player player = context.GetPlayer();
				InteractableVehicle vehicle = player.movement.getVehicle();

				OnVehicleHorn?.Invoke(player, vehicle, ref isCancelled);

				if (isCancelled) return false;
				SendVehicleHorn.InvokeAndLoopback(ENetReliability.Unreliable, Provider.GatherRemoteClientConnections(), vehicle.instanceID);
				return false;
			}
		}
	}
}
