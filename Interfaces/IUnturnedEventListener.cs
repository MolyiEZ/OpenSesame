using OpenMod.API.Ioc;

namespace Molyi.OpenSesame.Interfaces
{
	[Service]
	public interface IUnturnedEventListener
	{
		void Subscribe();
	}
}
