using OpenMod.API.Ioc;
using OpenSesame.Models;
using System;

namespace OpenSesame.Interfaces
{
	[Service]
	public interface IEventSender
	{
		static void SendEvent(UnturnedVehicleHornEvent @event) => throw new NotImplementedException();
	}
}
