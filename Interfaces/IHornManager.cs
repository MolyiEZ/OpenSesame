using Molyi.OpenSesame.Models;
using OpenMod.API.Ioc;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;

namespace Molyi.OpenSesame.Interfaces
{
	[Service]
	public interface IHornManager
	{
		Config Config { get; }
		Dictionary<CSteamID, BarricadeDrop> PlayerHorn { get; }
	}
}
