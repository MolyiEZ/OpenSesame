using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Molyi.OpenSesame.Interfaces;
using OpenMod.API.Permissions;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using System;

[assembly: PluginMetadata("Molyi.OpenSesame", Author = "Molyi", DisplayName = "OpenSesame", Description = "Unturned OpenMod Plugin. Open gates automatically with your car.", Website = "https://github.com/MolyiEZ/OpenSesame")]
namespace Molyi.OpenSesame
{
	public class OpenSesamePlugin(
		IPermissionRegistry permissionRegistry,
		ILogger<OpenSesamePlugin> logger,
		IServiceProvider serviceProvider,
		IHornManager hornManager) : OpenModUnturnedPlugin(serviceProvider)
	{
		private readonly IPermissionRegistry m_PermissionRegistry = permissionRegistry;
		private readonly ILogger<OpenSesamePlugin> m_Logger = logger;
		private readonly IServiceProvider m_ServiceProvider = serviceProvider;
		private readonly IHornManager m_HornManager = hornManager;

		protected override UniTask OnLoadAsync()
		{
			m_ServiceProvider.GetRequiredService<IUnturnedEventListener>()?.Subscribe();
			m_PermissionRegistry.RegisterPermission(this, m_HornManager.Config.HornPermission, "Permission used to open a garage door with the horn of your vehicle");

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
