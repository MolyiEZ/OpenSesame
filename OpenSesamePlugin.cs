using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Cysharp.Threading.Tasks;
using OpenMod.Unturned.Plugins;
using OpenMod.API.Plugins;
using OpenMod.API.Permissions;
using Molyi.OpenSesame.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Molyi.OpenSesame.Models;
using System.Collections.Generic;
using Steamworks;
using SDG.Unturned;
using UnityEngine;

[assembly: PluginMetadata("Molyi.OpenSesame", Author = "Molyi", DisplayName = "OpenSesame")]
namespace Molyi.OpenSesame
{
	public class OpenSesamePlugin : OpenModUnturnedPlugin
    {
		private readonly IConfiguration m_Configuration;
        private readonly IPermissionRegistry m_PermissionRegistry;
		private readonly ILogger<OpenSesamePlugin> m_Logger;
		private readonly IServiceProvider m_ServiceProvider;

		public readonly Config config = new();
		public readonly Dictionary<CSteamID, BarricadeDrop> playerHorn = [];

		public OpenSesamePlugin(
			IConfiguration configuration,
            IPermissionRegistry permissionRegistry,
            ILogger<OpenSesamePlugin> logger,
            IServiceProvider serviceProvider) : base(serviceProvider)
		{
            m_Configuration = configuration;
            m_PermissionRegistry = permissionRegistry;
			m_Logger = logger;
			m_ServiceProvider = serviceProvider;
			m_Configuration.Bind(config);
			playerHorn = [];
		}

        protected override UniTask OnLoadAsync()
		{ 
			m_PermissionRegistry.RegisterPermission(this, config.HornPermission, "Permission used to open a garage door with the horn of your vehicle");
			m_ServiceProvider.GetRequiredService<IEventSender>();

			m_Logger.LogInformation($"{DisplayName} has been loaded!");
			m_Logger.LogInformation($"This plugin was created and distributed by Molyi at UnturnedStore.com (Discord: molyi)");
			m_Logger.LogInformation($"If you didn't get this plugin on UnturnedStore; contact Molyi.");

			return UniTask.CompletedTask;
        }

        protected override UniTask OnUnloadAsync()
        {
			m_Logger.LogInformation($"{DisplayName} has been unloaded!");

			return UniTask.CompletedTask;
		}
    }
}
