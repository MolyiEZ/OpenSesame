using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Molyi.OpenSesame.Interfaces;
using Molyi.OpenSesame.Models;
using Molyi.OpenSesame.Models.Events;
using OpenMod.API.Eventing;
using OpenMod.API.Permissions;
using OpenMod.Core.Eventing;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using System.Threading.Tasks;
using UnityEngine;
using Color = System.Drawing.Color;

namespace Molyi.OpenSesame.Events
{
	[EventListenerLifetime(ServiceLifetime.Singleton)]
	public class VehicleHorn(
		IHornManager hornManager,
		IPermissionChecker permissionChecker,
		IStringLocalizer localizer) : IEventListener<UnturnedVehicleHornEvent>
	{
		private readonly string imgUrl = "https://imgur.com/GDwj1Tk.png";
		private readonly IPermissionChecker m_PermissionChecker = permissionChecker;
		private readonly IStringLocalizer m_Localizer = localizer;
		private readonly IHornManager m_HornManager = hornManager;
		private readonly Config m_Config = hornManager.Config;

		[EventListener(Priority = EventListenerPriority.Lowest)]
		public async Task HandleEventAsync(object? sender, UnturnedVehicleHornEvent @event)
		{
			UnturnedUser uUser = @event.User;
			InteractableVehicle iVehicle = @event.Vehicle;

			if (await m_PermissionChecker.CheckPermissionAsync(uUser, m_Config.HornPermission) != PermissionGrantResult.Grant) return;
			if (m_HornManager.PlayerHorn.TryGetValue(uUser.SteamId, out BarricadeDrop bDrop))
			{
				m_HornManager.PlayerHorn.Remove(uUser.SteamId);

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
			InteractableDoor door = hitTransform.GetComponent<InteractableDoor>() ?? hitTransform.GetComponentInParent<InteractableDoor>();
			if (door != null) hitTransform = door.transform;

			bDrop = BarricadeManager.FindBarricadeByRootTransform(hitTransform);
			if (bDrop == null || bDrop.asset.build != EBuild.GATE || (bDrop.GetServersideData().owner != uUser.SteamId.m_SteamID && bDrop.GetServersideData().group != uUser.Player.Player.quests.groupID.m_SteamID)) return;
			@event.IsCancelled = m_Config.HornCancel;
			BarricadeManager.ServerSetDoorOpen((InteractableDoor)bDrop.interactable, true);
			m_HornManager.PlayerHorn.Add(uUser.SteamId, bDrop);
			await uUser.PrintMessageAsync(m_Localizer["GateOpened"], Color.White, true, imgUrl);
		}
	}
}
