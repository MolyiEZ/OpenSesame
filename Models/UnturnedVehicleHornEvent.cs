using OpenMod.API.Eventing;
using OpenMod.Core.Eventing;
using OpenMod.Unturned.Users;
using SDG.Unturned;

namespace OpenSesame.Models
{
	public class UnturnedVehicleHornEvent : Event, ICancellableEvent
	{
		public UnturnedUser User { get; set; }
		public InteractableVehicle Vehicle { get; set; }
		public bool IsCancelled { get; set; }

		public UnturnedVehicleHornEvent(UnturnedUser user, InteractableVehicle vehicle)
		{
			User = user;
			Vehicle = vehicle;
		}
	}
}
