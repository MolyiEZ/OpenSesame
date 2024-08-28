using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Molyi.OpenSesame.Interfaces;
using Molyi.OpenSesame.Models;
using OpenMod.API.Ioc;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;

namespace Molyi.OpenSesame.Services
{
	[PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
	public class HornManager : IHornManager
	{
		public Config Config { get; } = new();
		public Dictionary<CSteamID, BarricadeDrop> PlayerHorn { get; }

		public HornManager(
			IConfiguration configuration)
		{
			configuration.Bind(Config);
			PlayerHorn = [];
		}
	}
}
