using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Cysharp.Threading.Tasks;
using OpenMod.Unturned.Plugins;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Users;
using OpenMod.API.Permissions;
using Molyi.OpenSesame.Interfaces;

// For more, visit https://openmod.github.io/openmod-docs/devdoc/guides/getting-started.html

[assembly: PluginMetadata("Molyi.OpenSesame", Author = "Molyi", DisplayName = "OpenSesame")]
namespace Molyi.OpenSesame
{
	public class OpenSesamePlugin : OpenModUnturnedPlugin
    {
		private readonly IConfiguration m_Configuration;
        private readonly IPermissionRegistry m_PermissionRegistry;
		private readonly IEventSender m_EventSender;
		private readonly IUnturnedUserDirectory m_UserDirectory;
		private readonly ILogger<OpenSesamePlugin> m_Logger;

        public OpenSesamePlugin(
			IConfiguration configuration,
            IPermissionRegistry permissionRegistry,
			IEventSender eventSender,
			IUnturnedUserDirectory userDirectory,
            ILogger<OpenSesamePlugin> logger,
            IServiceProvider serviceProvider) : base(serviceProvider)
		{
            m_Configuration = configuration;
            m_PermissionRegistry = permissionRegistry;
			m_EventSender = eventSender;
			m_UserDirectory = userDirectory;
			m_Logger = logger;
        }

        protected override UniTask OnLoadAsync()
        {
			m_PermissionRegistry.RegisterPermission(this, m_Configuration["permission_horn"]!, "Permission used to open a garage door with the horn of your vehicle");
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
