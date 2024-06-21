using Antlr4.Runtime.Tree.Xpath;
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
		public bool useHotasWithWheel { get; set; } = false;

		public bool reasignPedals { get; set; } = false;
		public string gunAType { get; set; } = "<none>";
		public string gunBType { get; set; } = "<none>";
		public string bindingDinputGunAXbox { get; set; } = "";
		public string bindingDinputGunASinden { get; set; } = "";
		public string bindingDinputGunAGuncon1 { get; set; } = "";
		public string bindingDinputGunAGuncon2 { get; set; } = "";
		public string bindingDinputGunAWiimote { get; set; } = "";
		public string bindingDinputGunBXbox { get; set; } = "";
		public string bindingDinputGunBSinden { get; set; } = "";
		public string bindingDinputGunBGuncon1 { get; set; } = "";
		public string bindingDinputGunBGuncon2 { get; set; } = "";
		public string bindingDinputGunBWiimote { get; set; } = "";

		public int indexvjoy { get; set; } = 0;

		public string vjoySettingsGunA { get; set; } = "";
		public string vjoySettingsGunB { get; set; } = "";

		public string gunARecoil { get; set; } = "<none>";
		public string gunBRecoil { get; set; } = "<none>";
		public int gunAComPort { get; set; } = 0;
		public int gunBComPort { get; set; } = 0;
		public int gunASidenPump { get; set; } = 1;
		public int gunBSidenPump { get; set; } = 1;

		public string demulshooterExe { get; set; } = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "demulshooter", "DemulShooter.exe");

		public string sindenExe { get; set; } = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "sinden", "Lightgun.exe");

		public string mamehookerExe { get; set; } = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "mamehooker", "mamehook.exe");

		public string sindenExtraCmd { get; set; } = "";
		public bool gunAAutoJoy { get; set; } = false;
		public bool gunBAutoJoy { get; set; } = false;

		public bool gunACrosshair { get; set; } = false;
		public bool gunBCrosshair { get; set; } = false;

		public bool gunA4tiers { get; set; } = false;
		public bool gunB4tiers { get; set; } = false;

		public bool gunAdomagerumble { get; set; } = false;
		public bool gunBdomagerumble { get; set; } = false;

		public bool gunAvjoy { get; set; } = false;
		public bool gunBvjoy { get; set; } = false;

		public bool reversePedals { get; set; } = false;
		public bool alwaysRunMamehooker { get; set; } = false;

		public string rivatunerExe { get; set; } = "";

		public bool gunAOffscreenReload { get; set; } = false;
		public bool gunBOffscreenReload { get; set; } = false;

		public int TPConsoleAction { get; set; } = 0;

		public string magpieExe { get; set; } = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "magpie", "Magpie.exe");
		public bool useMagpie { get; set; } = true;
		public int magpieScaling { get; set; } = 0;
		public int magpieCapture { get; set; } = 0;
		public int magpieDelay { get; set; } = 0;
		public bool magpieShowFps { get; set; } = false;
		public bool magpieTripleBuffering { get; set; } = false;
		public bool magpieVsync { get; set; } = false;
		//public int magpieLightgun { get; set; } = 0; //Unused
		//public int magpieLightgunCalibration { get; set; } = 0; //Unused

		public bool magpieSinden { get; set; } = true;
		public bool magpieGunCalibration { get; set; } = true;


		public double magpieBorderSize { get; set; } = 1.5;

		public bool useXenos { get; set; } = false;
		public int magpieFsrSharp { get; set; } = 87;
		public bool magpieExclusiveFullscreen { get; set; } = false;

		public int gpuType { get; set; } = 0;
		public bool patchGpuFix { get; set; } = true;
		public bool patchGpuTP { get; set; } = true;

		public int gpuResolution { get; set; } = 1;
		public bool patchResolutionFix { get; set; } = true;
		public bool patchResolutionTP { get; set; } = true;

		public int displayMode { get; set; } = 0;
		public bool patchDisplayModeFix { get; set; } = true;
		public bool patchDisplayModeTP { get; set; } = true;

		public bool patchMagpie { get; set; } = true;
		public bool patchReshade { get; set; } = true;
		public bool patchGameID { get; set; } = true;
		public bool patchNetwork { get; set; } = true;
		public bool patchOtherTPSettings { get; set; } = true;

		public bool patchOthersGameOptions { get; set; } = true;

		public string patch_apm3id { get; set; } = "";
		public string patch_mariokartId { get; set; } = "";

		public string patch_customName { get; set; } = "";
		public bool patch_networkAuto { get; set; } = true;
		public string patch_networkIP { get; set; } = "";
		public string patch_networkMask { get; set; } = "";
		public string patch_networkGateway { get; set; } = "";
		public string patch_networkDns1 { get; set; } = "";
		public string patch_networkDns2 { get; set; } = "";
		public string patch_BroadcastAddress { get; set; } = "";

		public bool patch_FFB { get; set; } = true;


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
				this.useHotasWithWheel = DeserializeData.useHotasWithWheel;
				this.reasignPedals = DeserializeData.reasignPedals;
				this.gunAType = DeserializeData.gunAType;
				this.gunBType = DeserializeData.gunBType;
				this.bindingDinputGunAXbox = DeserializeData.bindingDinputGunAXbox;
				this.bindingDinputGunASinden = DeserializeData.bindingDinputGunASinden;
				this.bindingDinputGunAGuncon1 = DeserializeData.bindingDinputGunAGuncon1;
				this.bindingDinputGunAGuncon2 = DeserializeData.bindingDinputGunAGuncon2;
				this.bindingDinputGunAWiimote = DeserializeData.bindingDinputGunAWiimote;
				this.bindingDinputGunBXbox = DeserializeData.bindingDinputGunBXbox;
				this.bindingDinputGunBSinden = DeserializeData.bindingDinputGunBSinden;
				this.bindingDinputGunBGuncon1 = DeserializeData.bindingDinputGunBGuncon1;
				this.bindingDinputGunBGuncon2 = DeserializeData.bindingDinputGunBGuncon2;
				this.bindingDinputGunBWiimote = DeserializeData.bindingDinputGunBWiimote;
				this.indexvjoy = DeserializeData.indexvjoy;
				this.vjoySettingsGunA = DeserializeData.vjoySettingsGunA;
				this.vjoySettingsGunB = DeserializeData.vjoySettingsGunB;
				this.gunARecoil = DeserializeData.gunARecoil;
				this.gunBRecoil = DeserializeData.gunBRecoil;
				this.gunAComPort = DeserializeData.gunAComPort;
				this.gunBComPort = DeserializeData.gunBComPort;
				this.gunASidenPump = DeserializeData.gunASidenPump;
				this.gunBSidenPump = DeserializeData.gunBSidenPump;
				this.demulshooterExe = DeserializeData.demulshooterExe;
				this.sindenExe = DeserializeData.sindenExe;
				this.mamehookerExe = DeserializeData.mamehookerExe;
				this.sindenExtraCmd = DeserializeData.sindenExtraCmd;
				this.gunAAutoJoy = DeserializeData.gunAAutoJoy;
				this.gunBAutoJoy = DeserializeData.gunBAutoJoy;
				this.gunACrosshair = DeserializeData.gunACrosshair;
				this.gunBCrosshair = DeserializeData.gunBCrosshair;
				this.gunA4tiers = DeserializeData.gunA4tiers;
				this.gunB4tiers = DeserializeData.gunB4tiers;
				this.gunAdomagerumble = DeserializeData.gunAdomagerumble;
				this.gunBdomagerumble = DeserializeData.gunBdomagerumble;
				this.gunAvjoy = DeserializeData.gunAvjoy;
				this.gunBvjoy = DeserializeData.gunBvjoy;
				this.reversePedals = DeserializeData.reversePedals;
				this.alwaysRunMamehooker = DeserializeData.alwaysRunMamehooker;
				this.rivatunerExe = DeserializeData.rivatunerExe;
				this.gunAOffscreenReload = DeserializeData.gunAOffscreenReload;
				this.gunBOffscreenReload = DeserializeData.gunBOffscreenReload;
				this.TPConsoleAction = DeserializeData.TPConsoleAction;
				this.magpieExe = DeserializeData.magpieExe;
				this.magpieScaling = DeserializeData.magpieScaling;
				this.magpieDelay = DeserializeData.magpieDelay;
				this.magpieCapture = DeserializeData.magpieCapture;
				this.useMagpie = DeserializeData.useMagpie;
				this.magpieShowFps = DeserializeData.magpieShowFps;
				this.magpieTripleBuffering = DeserializeData.magpieTripleBuffering;
				this.magpieVsync = DeserializeData.magpieVsync;
				this.magpieSinden = DeserializeData.magpieSinden;
				this.magpieGunCalibration = DeserializeData.magpieGunCalibration;
				this.magpieBorderSize = DeserializeData.magpieBorderSize;
				this.useXenos = DeserializeData.useXenos;
				
				this.magpieFsrSharp = DeserializeData.magpieFsrSharp;
				this.magpieExclusiveFullscreen = DeserializeData.magpieExclusiveFullscreen;

				this.gpuType = DeserializeData.gpuType;
				this.patchGpuFix = DeserializeData.patchGpuFix;
				this.patchGpuTP = DeserializeData.patchGpuTP;

				this.gpuResolution = DeserializeData.gpuResolution;
				this.patchResolutionTP = DeserializeData.patchResolutionTP;
				this.patchResolutionFix = DeserializeData.patchResolutionFix;

				this.displayMode = DeserializeData.displayMode;
				this.patchDisplayModeTP = DeserializeData.patchDisplayModeTP;
				this.patchDisplayModeFix = DeserializeData.patchDisplayModeFix;

				this.patchMagpie = DeserializeData.patchMagpie;
				this.patchReshade = DeserializeData.patchReshade;
				this.patchGameID = DeserializeData.patchGameID;
				this.patchNetwork = DeserializeData.patchNetwork;
				this.patchOtherTPSettings = DeserializeData.patchOtherTPSettings;
				this.patchOthersGameOptions = DeserializeData.patchOthersGameOptions;

				this.patch_apm3id = DeserializeData.patch_apm3id;
				this.patch_mariokartId = DeserializeData.patch_mariokartId;
				this.patch_customName = DeserializeData.patch_customName;
				this.patch_networkAuto = DeserializeData.patch_networkAuto;
				this.patch_networkIP = DeserializeData.patch_networkIP;
				this.patch_networkMask = DeserializeData.patch_networkMask;
				this.patch_networkGateway = DeserializeData.patch_networkGateway;
				this.patch_networkDns1 = DeserializeData.patch_networkDns1;
				this.patch_networkDns2 = DeserializeData.patch_networkDns2;
				this.patch_BroadcastAddress = DeserializeData.patch_BroadcastAddress;
				this.patch_FFB = DeserializeData.patch_FFB;




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
