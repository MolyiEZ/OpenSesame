using Autofac;
using HarmonyLib;
using Microsoft.Extensions.DependencyInjection;
using Molyi.OpenSesame.Interfaces;
using Molyi.OpenSesame;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Users;
using SDG.NetTransport;
using SDG.Unturned;
using System;
using Molyi.OpenSesame.Models;

namespace Molyi.OpenSesame.Patches
{
	[HarmonyPatch]
	[ServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
	public class VehicleHornPatch : IEventSender
	{
		private static readonly ClientStaticMethod<uint> SendVehicleHorn = ClientStaticMethod<uint>.Get(VehicleManager.ReceiveVehicleHorn);

		private static Lazy<IPluginAccessor<OpenSesamePlugin>> m_PluginAccessor = null!;
		private static IEventBus m_EventBus => m_PluginAccessor.Value.Instance?.EventBus!;
		private static IUnturnedUserDirectory m_UserDirectory => m_PluginAccessor.Value.Instance?.LifetimeScope.Resolve<IUnturnedUserDirectory>()!;

		public VehicleHornPatch(Lazy<IPluginAccessor<OpenSesamePlugin>> pluginAccessor)
		{
			m_PluginAccessor = pluginAccessor;
		}

		[HarmonyPatch(typeof(VehicleManager))]
		[HarmonyPatch("ReceiveVehicleHornRequest")]
		[HarmonyPrefix]
		public static bool ReceiveVehicleHornRequest(in ServerInvocationContext context)
		{
			Player player = context.GetPlayer();
			InteractableVehicle vehicle = player.movement.getVehicle();

			var @event = new UnturnedVehicleHornEvent(m_UserDirectory.GetUser(player), vehicle);
			SendEvent(@event);
			if (@event.IsCancelled) return false;
			SendVehicleHorn.InvokeAndLoopback(ENetReliability.Unreliable, Provider.GatherRemoteClientConnections(), vehicle.instanceID);
			return false;
		}

		public static async void SendEvent(UnturnedVehicleHornEvent @event) => await m_EventBus.EmitAsync(m_PluginAccessor.Value.Instance!, null, @event);
	}
}
