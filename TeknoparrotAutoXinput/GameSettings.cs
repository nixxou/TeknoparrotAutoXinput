using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeknoparrotAutoXinput
{
	public class GameSettings
	{
		public bool RunAsRoot { get; set; } = false;
		public bool UseGlobalDisposition { get; set; } = true;
		public bool UseGlobalStoozZoneWheel { get; set; } = true;
		public bool UseGlobalStoozZoneGamepad { get; set; } = true;
		public bool gamepadStooz { get; set; } = false;
		public bool wheelStooz { get; set; } = false;
		public bool enableStoozZone_Gamepad { get; set; } = false;
		public int valueStooz_Gamepad { get; set; } = 10;
		public bool enableStoozZone_Wheel { get; set; } = false;
		public int valueStooz_Wheel { get; set; } = 10;
		public string Disposition { get; set; } = "";
		public bool EnableLink {  get; set; } = false;
		public string AhkBefore { get; set; } = string.Empty;
		public string AhkAfter { get; set; } = string.Empty;
		public bool WaitForExitAhkBefore { get; set; } = true;
		public bool EnableGearChange { get; set; } = false;


		public GameSettings() 
		{
			

		}

		public GameSettings(string json)
		{
			try
			{
				GameSettings DeserializeData = JsonConvert.DeserializeObject<GameSettings>(json);
				this.RunAsRoot = DeserializeData.RunAsRoot;
				this.UseGlobalDisposition = DeserializeData.UseGlobalDisposition;
				this.UseGlobalStoozZoneWheel = DeserializeData.UseGlobalStoozZoneWheel;
				this.UseGlobalStoozZoneGamepad = DeserializeData.UseGlobalStoozZoneGamepad;
				this.gamepadStooz = DeserializeData.gamepadStooz;
				this.wheelStooz = DeserializeData.wheelStooz;
				this.enableStoozZone_Gamepad = DeserializeData.enableStoozZone_Gamepad;
				this.valueStooz_Gamepad = DeserializeData.valueStooz_Gamepad;
				this.enableStoozZone_Wheel = DeserializeData.enableStoozZone_Wheel;
				this.valueStooz_Wheel = DeserializeData.valueStooz_Wheel;
				this.Disposition = DeserializeData.Disposition;
				this.EnableLink = DeserializeData.EnableLink;
				this.AhkBefore = DeserializeData.AhkBefore;
				this.AhkAfter = DeserializeData.AhkAfter;
				this.WaitForExitAhkBefore = DeserializeData.WaitForExitAhkBefore;
				this.EnableGearChange = DeserializeData.EnableGearChange;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public string Serialize()
		{
			string json = JsonConvert.SerializeObject(this, Formatting.Indented);
			return json;
		}

		public void Save(string filename)
		{
			File.WriteAllText(filename, this.Serialize());
		}

	}
}
