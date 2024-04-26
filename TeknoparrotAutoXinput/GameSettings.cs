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
		public bool EnableLink {  get; set; } = true;
		public string AhkBefore { get; set; } = string.Empty;
		public string AhkAfter { get; set; } = string.Empty;
		public bool WaitForExitAhkBefore { get; set; } = true;
		public bool EnableGearChange { get; set; } = false;
		public string CustomPerGameLinkFolder { get; set; } = "";
		public string CustomTpExe { get; set; } = "";

		public bool UseGlobalStoozZoneHotas { get; set; } = true;
		public bool hotasStooz { get; set; } = false;
		public bool enableStoozZone_Hotas { get; set; } = false;
		public int valueStooz_Hotas { get; set; } = 10;
		public bool reverseYAxis_Hotas { get; set; } = false;

		public bool EnableLinkExe { get; set; } = true;
		public string vjoySettingsGunA { get; set; } = "";
		public string vjoySettingsGunB { get; set; } = "";

		public int indexvjoy { get; set; } = -1;

		public int gunA_recoil { get; set; } = 0;
		public int gunA_pump { get; set; } = 0;
		public int gunA_crosshair { get; set; } = 0;
		public int gunB_recoil { get; set; } = 0;
		public int gunB_pump { get; set; } = 0;
		public int gunB_crosshair { get; set; } = 0;

		public bool gun_useExtraSinden { get; set; } = false;
		public string gun_ExtraSinden { get; set; } = "";

		public int gunA_useVjoy { get; set; } = 0;
		public int gunB_useVjoy { get; set; } = 0;

		public int gunA_4tiers { get; set; } = 0;
		public int gunB_4tiers { get; set; } = 0;

		public bool runRivaTuner { get; set; } = false;



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
				this.CustomPerGameLinkFolder = DeserializeData.CustomPerGameLinkFolder;
				this.CustomTpExe = DeserializeData.CustomTpExe;
				this.UseGlobalStoozZoneHotas = DeserializeData.UseGlobalStoozZoneHotas;
				this.hotasStooz = DeserializeData.hotasStooz;
				this.enableStoozZone_Hotas = DeserializeData.enableStoozZone_Hotas;
				this.valueStooz_Hotas = DeserializeData.valueStooz_Hotas;
				this.reverseYAxis_Hotas = DeserializeData.reverseYAxis_Hotas;
				this.EnableLinkExe = DeserializeData.EnableLinkExe;
				this.vjoySettingsGunA = DeserializeData.vjoySettingsGunA;
				this.vjoySettingsGunB = DeserializeData.vjoySettingsGunB;
				this.indexvjoy = DeserializeData.indexvjoy;
				this.gunA_recoil = DeserializeData.gunA_recoil;
				this.gunA_pump = DeserializeData.gunA_pump;
				this.gunA_crosshair = DeserializeData.gunA_crosshair;
				this.gunB_recoil = DeserializeData.gunB_recoil;
				this.gunB_pump = DeserializeData.gunB_pump;
				this.gunB_crosshair = DeserializeData.gunB_crosshair;
				this.gun_useExtraSinden = DeserializeData.gun_useExtraSinden;
				this.gun_ExtraSinden = DeserializeData.gun_ExtraSinden;
				this.gunA_useVjoy = DeserializeData.gunA_useVjoy;
				this.gunB_useVjoy = DeserializeData.gunB_useVjoy;
				this.gunA_4tiers = DeserializeData.gunA_4tiers;
				this.gunB_4tiers = DeserializeData.gunB_4tiers;
				this.runRivaTuner = DeserializeData.runRivaTuner;

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
