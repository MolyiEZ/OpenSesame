using Molyi.OpenSesame.Models;
using OpenMod.API.Ioc;
using System;

namespace Molyi.OpenSesame.Interfaces
{
	[Service]
	public interface IEventSender
	{
		static void SendEvent(UnturnedVehicleHornEvent @event) => throw new NotImplementedException();
	}
}
