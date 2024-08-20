namespace Molyi.OpenSesame.Models
{
	public class Config
	{
		public string HornPermission { get; set; } = "opensesame.horn";
		public ushort HornDistance { get; set; }
		public bool HornCancel { get; set; }
	}
}
