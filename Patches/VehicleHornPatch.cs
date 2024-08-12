using HarmonyLib;
using Microsoft.Extensions.DependencyInjection;
using Molyi.OpenSesame.Interfaces;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Users;
using SDG.NetTransport;
using SDG.Unturned;
using Molyi.OpenSesame.Models;

namespace Molyi.OpenSesame.Patches
{
	[HarmonyPatch]
	[ServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
	public class VehicleHornPatch : IEventSender
	{
		private static readonly ClientStaticMethod<uint> SendVehicleHorn = ClientStaticMethod<uint>.Get(VehicleManager.ReceiveVehicleHorn);

		private static IPluginAccessor<OpenSesamePlugin> m_PluginAccessor = null!;
		private static IEventBus m_EventBus = null!;
		private static IUnturnedUserDirectory m_UserDirectory = null!;

		public VehicleHornPatch(
			IPluginAccessor<OpenSesamePlugin> pluginAccessor,
			IEventBus eventBus,
			IUnturnedUserDirectory userDirectory)
		{
			m_PluginAccessor = pluginAccessor;
			m_EventBus = eventBus;
			m_UserDirectory = userDirectory;
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

		public static async void SendEvent(UnturnedVehicleHornEvent @event) => await m_EventBus.EmitAsync(m_PluginAccessor.Instance!, null, @event);
	}
}
