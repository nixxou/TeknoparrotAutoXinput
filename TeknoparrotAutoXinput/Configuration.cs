using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeknoparrotAutoXinput
{

	public static class ConfigurationManager
	{
		public static Configuration MainConfig { get; set; } = new Configuration();

		public static void LoadConfig()
		{
			string exePath = Process.GetCurrentProcess().MainModule.FileName;
			string exeDir = Path.GetDirectoryName(exePath);
			string TeknoparrotAutoXinputConfigFile = Path.Combine(exeDir, "TeknoparrotAutoXinput.json");
			if(File.Exists(TeknoparrotAutoXinputConfigFile)) 
			{
				MainConfig = new Configuration(File.ReadAllText(TeknoparrotAutoXinputConfigFile));
			}
		}

		public static void SaveConfig()
		{
			string exePath = Process.GetCurrentProcess().MainModule.FileName;
			string exeDir = Path.GetDirectoryName(exePath);
			string TeknoparrotAutoXinputConfigFile = Path.Combine(exeDir, "TeknoparrotAutoXinput.json");
			File.WriteAllText(TeknoparrotAutoXinputConfigFile,MainConfig.Serialize());
		}
	}
	public class Configuration
	{
		public bool virtualKeyboard { get; set; } = false;
		public string keyTest { get; set; } = "";
		public string keyService1 { get; set; } = "";
		public string keyService2 { get; set; } = "";
		public bool showStartup { get; set; } = false;
		public bool FFB { get; set; } = false;
		public string wheelXinputData { get; set; } = "Type=Wheel";
		public string arcadeXinputData { get; set; } = "Type=ArcadeStick,Type=ArcadePad";
		public string gamepadXinputData { get; set; } = "Type=Gamepad";
		public bool gamepadStooz { get; set; } = false;
		public bool wheelStooz { get; set; } = false;
		public bool enableStoozZone_Gamepad { get; set; } = false;
		public int valueStooz_Gamepad { get; set; } = 0;
		public bool enableStoozZone_Wheel { get; set; } = false;
		public int valueStooz_Wheel { get; set; } = 0;
		public string bindingDinputWheel { get; set; } = "";
		public bool useDinputWheel { get; set; } = false;
		public string ffbDinputWheel { get; set; } = "";
		public bool favorAB { get; set; } = false;
		public string TpFolder { get; set; } = "";
		public bool ShowAllGames { get; set; } = false;
		public string Disposition { get; set; } = "";
		public bool debugMode { get; set; } = false;
		public string perGameLinkFolder { get; set; } = @"Default (<YourTeknoparrotFolder>\AutoXinputLinks)";
		public string bindingDinputShifter { get; set; } = "";
		public bool useDinputShifter { get; set; } = false;
		public string perGameLinkFolderExe { get; set; } = @"";
		public bool hotasStooz { get; set; } = false;
		public bool enableStoozZone_Hotas { get; set; } = false;
		public int valueStooz_Hotas { get; set; } = 0;
		public string bindingDinputHotas { get; set; } = "";
		public bool useDinputHotas { get; set; } = false;
		public string ffbDinputHotas { get; set; } = "";
		public bool reverseYAxis_Hotas { get; set; } = true;
		public bool useHotasWithWheel { get; set; } = false;

		public bool reasignPedals { get; set; } = false;
		public string gunAType { get; set; } = "<none>";
		public string gunBType { get; set; } = "<none>";
		public string bindingDinputGunAXbox { get; set; } = "";
		public string bindingDinputGunASinden { get; set; } = "";
		public string bindingDinputGunAGun4ir { get; set; } = "";
		public string bindingDinputGunAWiimote { get; set; } = "";
		public string bindingDinputGunBXbox { get; set; } = "";
		public string bindingDinputGunBSinden { get; set; } = "";
		public string bindingDinputGunBGun4ir { get; set; } = "";
		public string bindingDinputGunBWiimote { get; set; } = "";

		public int indexvjoy { get; set; } = 0;

		public string vjoySettingsGunA { get; set; } = "";
		public string vjoySettingsGunB { get; set; } = "";


		public Configuration()
		{

		}

		public Configuration(string json)
		{
			try
			{
				Configuration DeserializeData = JsonConvert.DeserializeObject<Configuration>(json);
				this.virtualKeyboard = DeserializeData.virtualKeyboard;
				this.keyTest = DeserializeData.keyTest;
				this.keyService1 = DeserializeData.keyService1;
				this.keyService2 = DeserializeData.keyService2;
				this.showStartup = DeserializeData.showStartup;
				this.FFB = DeserializeData.FFB;
				this.wheelXinputData = DeserializeData.wheelXinputData;
				this.arcadeXinputData = DeserializeData.arcadeXinputData;
				this.gamepadXinputData = DeserializeData.gamepadXinputData;
				this.gamepadStooz = DeserializeData.gamepadStooz;
				this.wheelStooz = DeserializeData.wheelStooz;
				this.enableStoozZone_Gamepad = DeserializeData.enableStoozZone_Gamepad;
				this.valueStooz_Gamepad = DeserializeData.valueStooz_Gamepad;
				this.enableStoozZone_Wheel = DeserializeData.enableStoozZone_Wheel;
				this.valueStooz_Wheel = DeserializeData.valueStooz_Wheel;
				this.bindingDinputWheel = DeserializeData.bindingDinputWheel;
				this.useDinputWheel = DeserializeData.useDinputWheel;
				this.ffbDinputWheel = DeserializeData.ffbDinputWheel;
				this.favorAB = DeserializeData.favorAB;
				this.TpFolder = DeserializeData.TpFolder;
				this.ShowAllGames = DeserializeData.ShowAllGames;
				this.Disposition = DeserializeData.Disposition;
				this.debugMode = DeserializeData.debugMode;
				this.perGameLinkFolder = DeserializeData.perGameLinkFolder;
				this.bindingDinputShifter = DeserializeData.bindingDinputShifter;
				this.useDinputShifter = DeserializeData.useDinputShifter;
				this.perGameLinkFolderExe = DeserializeData.perGameLinkFolderExe;
				this.hotasStooz = DeserializeData.hotasStooz;
				this.enableStoozZone_Hotas = DeserializeData.enableStoozZone_Hotas;
				this.valueStooz_Hotas = DeserializeData.valueStooz_Hotas;
				this.bindingDinputHotas = DeserializeData.bindingDinputHotas;
				this.useDinputHotas = DeserializeData.useDinputHotas;
				this.ffbDinputHotas = DeserializeData.ffbDinputHotas;
				this.reverseYAxis_Hotas = DeserializeData.reverseYAxis_Hotas;
				this.useHotasWithWheel = DeserializeData.useHotasWithWheel;
				this.reasignPedals = DeserializeData.reasignPedals;
				this.gunAType = DeserializeData.gunAType;
				this.gunBType = DeserializeData.gunBType;
				this.bindingDinputGunAXbox = DeserializeData.bindingDinputGunAXbox;
				this.bindingDinputGunASinden = DeserializeData.bindingDinputGunASinden;
				this.bindingDinputGunAGun4ir = DeserializeData.bindingDinputGunAGun4ir;
				this.bindingDinputGunAWiimote = DeserializeData.bindingDinputGunAWiimote;
				this.bindingDinputGunBXbox = DeserializeData.bindingDinputGunBXbox;
				this.bindingDinputGunBSinden = DeserializeData.bindingDinputGunBSinden;
				this.bindingDinputGunBGun4ir = DeserializeData.bindingDinputGunBGun4ir;
				this.bindingDinputGunBWiimote = DeserializeData.bindingDinputGunBWiimote;
				this.indexvjoy = DeserializeData.indexvjoy;
				this.vjoySettingsGunA = DeserializeData.vjoySettingsGunA;
				this.vjoySettingsGunB = DeserializeData.vjoySettingsGunB;

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
	}


}
