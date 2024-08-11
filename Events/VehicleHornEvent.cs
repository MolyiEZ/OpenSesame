using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Molyi.OpenSesame.Models;
using OpenMod.API.Eventing;
using OpenMod.API.Permissions;
using OpenMod.Core.Eventing;
using OpenMod.Core.Helpers;
using OpenMod.Unturned.Players.Connections.Events;
using OpenMod.Unturned.Players.Life.Events;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Molyi.OpenSesame.Events
{
	public class VehicleHornEvent : IEventListener<UnturnedVehicleHornEvent>, IEventListener<UnturnedPlayerDeathEvent>, IEventListener<UnturnedPlayerDisconnectedEvent>
	{
		private readonly string imgUrl = "https://imgur.com/GDwj1Tk.png";
		private static Dictionary<CSteamID, BarricadeDrop> playerHorn = new Dictionary<CSteamID, BarricadeDrop>();
		private readonly IPermissionChecker m_PermissionChecker;
		private readonly IStringLocalizer m_StringLocalizer;
		private readonly IConfiguration m_Configuration;

		public VehicleHornEvent(
			IConfiguration configuration,
			IPermissionChecker permissionChecker,
			IStringLocalizer stringLocalizer)
		{
			m_PermissionChecker = permissionChecker;
			m_StringLocalizer = stringLocalizer;
			m_Configuration = configuration;
		}

		[EventListener(Priority = EventListenerPriority.Lowest)]
		public async Task HandleEventAsync(object? sender, UnturnedVehicleHornEvent @event)
		{
			UnturnedUser uUser = @event.User;
			InteractableVehicle iVehicle = @event.Vehicle;
			int distanceAction = m_Configuration.GetSection("distance_horn").Get<int>();
			bool isCancelled = m_Configuration.GetSection("cancel_horn").Get<bool>();

			if (await m_PermissionChecker.CheckPermissionAsync(uUser, m_Configuration.GetSection("permission_horn").Get<string>()!) != PermissionGrantResult.Grant) return;
			BarricadeDrop bDrop;
			if(playerHorn.TryGetValue(uUser.SteamId, out bDrop))
			{
				playerHorn.Remove(uUser.SteamId);
				if (bDrop == null)
				{
					await uUser.PrintMessageAsync(m_StringLocalizer["internal:gate_not_found"], System.Drawing.Color.White, true, imgUrl);
					return;
				}

				if(Vector3.Distance(bDrop.GetServersideData().point, uUser.Player.Player.transform.position) > distanceAction)
				{
					await uUser.PrintMessageAsync(m_StringLocalizer["internal:not_in_distance"], System.Drawing.Color.White, true, imgUrl);
					return;
				}

				@event.IsCancelled = isCancelled;
				BarricadeManager.ServerSetDoorOpen((InteractableDoor)bDrop.interactable, false);
				await uUser.PrintMessageAsync(m_StringLocalizer["internal:gate_closed"], System.Drawing.Color.White, true, imgUrl);
				return;
			}

			RaycastHit hit;
			Physics.Raycast(new Ray(iVehicle.transform.position, iVehicle.transform.forward), out hit, distanceAction, RayMasks.BLOCK_COLLISION);
			Transform hitTransform = hit.transform;
			if (hitTransform == null)
			{
				Physics.Raycast(new Ray(iVehicle.transform.position, -iVehicle.transform.forward), out hit, distanceAction, RayMasks.BLOCK_COLLISION);
				hitTransform = hit.transform;
				if (hitTransform == null) return;
			}

			InteractableDoorHinge doorHinge = hit.transform.GetComponent<InteractableDoorHinge>();
			if(doorHinge != null) hitTransform = doorHinge.door.transform;

			bDrop = BarricadeManager.FindBarricadeByRootTransform(hitTransform);
			if (bDrop == null || bDrop.asset.build != EBuild.GATE || (bDrop.GetServersideData().owner != uUser.SteamId.m_SteamID && bDrop.GetServersideData().group != uUser.Player.Player.quests.groupID.m_SteamID)) return;
			@event.IsCancelled = isCancelled;
			BarricadeManager.ServerSetDoorOpen((InteractableDoor)bDrop.interactable, true);
			playerHorn.Add(uUser.SteamId, bDrop);
			await uUser.PrintMessageAsync(m_StringLocalizer["internal:gate_opened"], System.Drawing.Color.White, true, imgUrl);
		}

		[EventListener(Priority = EventListenerPriority.Highest)]
		public Task HandleEventAsync(object? sender, UnturnedPlayerDeathEvent @event)
		{
			AsyncHelper.Schedule("OpenSesame.PlayerDeath", async () => playerHorn.Remove(@event.Player.SteamId));
			return Task.CompletedTask;
		}

		[EventListener(Priority = EventListenerPriority.Highest)]
		public Task HandleEventAsync(object? sender, UnturnedPlayerDisconnectedEvent @event)
		{
			AsyncHelper.Schedule("OpenSesame.PlayerDeath", async () => playerHorn.Remove(@event.Player.SteamId));
			return Task.CompletedTask;
		}
	}
}
