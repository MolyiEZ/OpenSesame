using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Molyi.OpenSesame.Models;
using OpenMod.API.Eventing;
using OpenMod.API.Permissions;
using OpenMod.API.Plugins;
using OpenMod.Core.Eventing;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using Steamworks;
using Color = System.Drawing.Color;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Microsoft.Extensions.DependencyInjection;
using Cysharp.Threading.Tasks;

namespace Molyi.OpenSesame.Events
{
	[EventListenerLifetime(ServiceLifetime.Singleton)]
	public class VehicleHorn(
		IPluginAccessor<OpenSesamePlugin> plugin,
		IPermissionChecker permissionChecker,
		IStringLocalizer localizer) : IEventListener<UnturnedVehicleHornEvent>
	{
		private readonly IPermissionChecker m_PermissionChecker = permissionChecker;
		private readonly IStringLocalizer m_Localizer = localizer;

		private readonly string imgUrl = "https://imgur.com/GDwj1Tk.png";
		private readonly Config m_Config = plugin.Instance?.config!;
		private readonly Dictionary<CSteamID, BarricadeDrop> playerHorn = plugin.Instance?.playerHorn!;

		[EventListener(Priority = EventListenerPriority.Lowest)]
		public async Task HandleEventAsync(object? sender, UnturnedVehicleHornEvent @event)
		{
			UnturnedUser uUser = @event.User;
			InteractableVehicle iVehicle = @event.Vehicle;

			if (await m_PermissionChecker.CheckPermissionAsync(uUser, m_Config.HornPermission) != PermissionGrantResult.Grant) return;
			if (playerHorn.TryGetValue(uUser.SteamId, out BarricadeDrop bDrop))
			{
				playerHorn.Remove(uUser.SteamId);

				if (bDrop == null || !bDrop.model.gameObject.activeSelf)
				{
					await uUser.PrintMessageAsync(m_Localizer["GateNotFound"], Color.White, true, imgUrl);
					return;
				}

				if (!((InteractableDoor)bDrop.interactable).isOpen)
				{
					await uUser.PrintMessageAsync(m_Localizer["GateIsClosed"], Color.White, true, imgUrl);
					return;
				}

				if (Vector3.Distance(bDrop.GetServersideData().point, uUser.Player.Player.transform.position) > m_Config.HornDistance)
				{
					await uUser.PrintMessageAsync(m_Localizer["NotInDistance"], Color.White, true, imgUrl);
					return;
				}

				@event.IsCancelled = m_Config.HornCancel;
				BarricadeManager.ServerSetDoorOpen((InteractableDoor)bDrop.interactable, false);
				await uUser.PrintMessageAsync(m_Localizer["GateClosed"], Color.White, true, imgUrl);
				return;
			}

			if (!Physics.Raycast(iVehicle.transform.position, iVehicle.transform.forward, out RaycastHit hit, m_Config.HornDistance, RayMasks.BLOCK_COLLISION) &&
				!Physics.Raycast(iVehicle.transform.position, -iVehicle.transform.forward, out hit, m_Config.HornDistance, RayMasks.BLOCK_COLLISION))
				return;

			Transform hitTransform = hit.transform;
			InteractableDoorHinge doorHinge = hit.transform.GetComponent<InteractableDoorHinge>();
			if (doorHinge != null) hitTransform = doorHinge.door.transform;

			bDrop = BarricadeManager.FindBarricadeByRootTransform(hitTransform);
			if (bDrop == null || bDrop.asset.build != EBuild.GATE || (bDrop.GetServersideData().owner != uUser.SteamId.m_SteamID && bDrop.GetServersideData().group != uUser.Player.Player.quests.groupID.m_SteamID)) return;
			@event.IsCancelled = m_Config.HornCancel;
			BarricadeManager.ServerSetDoorOpen((InteractableDoor)bDrop.interactable, true);
			playerHorn.Add(uUser.SteamId, bDrop);
			await uUser.PrintMessageAsync(m_Localizer["GateOpened"], Color.White, true, imgUrl);
		}
	}
}
