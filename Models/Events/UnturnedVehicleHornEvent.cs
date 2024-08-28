using OpenMod.API.Eventing;
using OpenMod.Core.Eventing;
using OpenMod.Unturned.Users;
using SDG.Unturned;

namespace Molyi.OpenSesame.Models.Events
{
	public class UnturnedVehicleHornEvent(UnturnedUser user, InteractableVehicle vehicle) : Event, ICancellableEvent
	{
		public UnturnedUser User { get; set; } = user;
		public InteractableVehicle Vehicle { get; set; } = vehicle;
		public bool IsCancelled { get; set; }
	}
}
