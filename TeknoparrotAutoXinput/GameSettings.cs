using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
		public bool EnableGearChange { get; set; } = true;
		public string CustomPerGameLinkFolder { get; set; } = "";
		public string CustomTpExe { get; set; } = "";
		public bool UseGlobalStoozZoneHotas { get; set; } = true;
		public bool hotasStooz { get; set; } = false;
		public bool enableStoozZone_Hotas { get; set; } = false;
		public int valueStooz_Hotas { get; set; } = 10;
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

		public int gunA_OffscreenReload { get; set; } = 0;
		public int gunB_OffscreenReload { get; set; } = 0;

		public int gunA_sindenRecoil1 { get; set; } = 0;
		public int gunA_sindenRecoil2 { get; set; } = 0;
		public int gunA_sindenRecoil3 { get; set; } = 0;
		public int gunB_sindenRecoil1 { get; set; } = 0;
		public int gunB_sindenRecoil2 { get; set; } = 0;
		public int gunB_sindenRecoil3 { get; set; } = 0;

		public int useMagpie { get; set; } = 0;
		public int magpieScaling { get; set; } = 0;
		public int magpieCapture { get; set; } = 0;
		public int magpieDelay { get; set; } = 0;
		public int magpieShowFps { get; set; } = 0;
		public int magpieTripleBuffering { get; set; } = 0;
		public int magpieVsync { get; set; } = 0;
		public int magpieSinden { get; set; } = 0;
		public int magpieGunCalibration { get; set; } = 0;
		public bool useInjector { get; set; } = false;
		public string injectorDllList { get; set; } = "";
		public int injectorDelay { get; set; } = 0;
		public int magpieFsrSharp { get; set; } = 0;
		public int magpieExclusiveFullscreen { get; set; } = 0;
		public int patchGpuFix { get; set; } = 0;
		public int patchGpuTP { get; set; } = 0;
		public int gpuResolution { get; set; } = 0;
		public int patchResolutionFix { get; set; } = 0;
		public int patchResolutionTP { get; set; } = 0;
		public int displayMode { get; set; } = 0;
		public int patchDisplayModeFix { get; set; } = 0;
		public int patchDisplayModeTP { get; set; } = 0;
		public int patchReshade { get; set; } = 0;
		public int patchGameID { get; set; } = 0;
		public int patchNetwork { get; set; } = 0;
		public int patchOtherTPSettings { get; set; } = 0;
		public int patchOthersGameOptions { get; set; } = 0;
		public int patchFFB { get; set; } = 0;
		public string tmpGunXFormula { get; set; } = "";
		public string tmpGunYFormula { get; set; } = "";

		public string tmpGunAMinMax { get; set; } = "";
		public string tmpGunBMinMax { get; set; } = "";



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
				this.gunA_sindenRecoil1 = DeserializeData.gunA_sindenRecoil1;
				this.gunA_sindenRecoil2 = DeserializeData.gunA_sindenRecoil2;
				this.gunA_sindenRecoil3 = DeserializeData.gunA_sindenRecoil3;
				this.gunB_sindenRecoil1 = DeserializeData.gunB_sindenRecoil1;
				this.gunB_sindenRecoil2 = DeserializeData.gunB_sindenRecoil2;
				this.gunB_sindenRecoil3 = DeserializeData.gunB_sindenRecoil3;
				this.runRivaTuner = DeserializeData.runRivaTuner;
				this.gunA_OffscreenReload = DeserializeData.gunA_OffscreenReload;
				this.gunB_OffscreenReload = DeserializeData.gunB_OffscreenReload;
				this.useMagpie = DeserializeData.useMagpie;
				this.magpieScaling = DeserializeData.magpieScaling;
				this.magpieCapture = DeserializeData.magpieCapture;
				this.magpieDelay = DeserializeData.magpieDelay;
				this.magpieShowFps = DeserializeData.magpieShowFps;
				this.magpieTripleBuffering = DeserializeData.magpieTripleBuffering;
				this.magpieVsync = DeserializeData.magpieVsync;
				this.magpieSinden = DeserializeData.magpieSinden;
				this.magpieGunCalibration = DeserializeData.magpieGunCalibration;
				this.useInjector = DeserializeData.useInjector;
				this.injectorDllList = DeserializeData.injectorDllList;
				this.injectorDelay = DeserializeData.injectorDelay;
				this.magpieFsrSharp = DeserializeData.magpieFsrSharp;
				this.magpieExclusiveFullscreen = DeserializeData.magpieExclusiveFullscreen;

				this.patchGpuFix = DeserializeData.patchGpuFix;
				this.patchGpuTP = DeserializeData.patchGpuTP;

				this.gpuResolution = DeserializeData.gpuResolution;
				this.patchResolutionTP = DeserializeData.patchResolutionTP;
				this.patchResolutionFix = DeserializeData.patchResolutionFix;

				this.displayMode = DeserializeData.displayMode;
				this.patchDisplayModeTP = DeserializeData.patchDisplayModeTP;
				this.patchDisplayModeFix = DeserializeData.patchDisplayModeFix;

				this.patchReshade = DeserializeData.patchReshade;
				this.patchGameID = DeserializeData.patchGameID;
				this.patchNetwork = DeserializeData.patchNetwork;
				this.patchOthersGameOptions = DeserializeData.patchOthersGameOptions;
				this.patchOtherTPSettings = DeserializeData.patchOtherTPSettings;
				this.patchFFB = DeserializeData.patchFFB;


			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}


		/*
		public void Overwrite(string jsonOverwrite)
		{
			Dictionary<string,string> settings = new Dictionary<string,string>();
			try
			{
				settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonOverwrite);
			}
			catch { }
			

			foreach (var setting in settings)
			{
				var propertyName = setting.Key;
				var propertyValue = setting.Value;

				PropertyInfo property = typeof(GameSettings).GetProperty(propertyName);
				if (property != null && property.CanWrite)
				{
					try
					{
						object convertedValue = null;
						if (property.PropertyType == typeof(bool))
						{
							convertedValue = bool.Parse(propertyValue);
						}
						else if (property.PropertyType == typeof(int))
						{
							convertedValue = int.Parse(propertyValue);
						}
						else if (property.PropertyType == typeof(string))
						{
							convertedValue = propertyValue;
						}

						property.SetValue(this, convertedValue);
					}
					catch (Exception ex)
					{
						Console.WriteLine($"Erreur lors de la conversion de la propriété '{propertyName}': {ex.Message}");
					}
				}
			}
		}

		*/


		public void Overwrite(JObject tpSection, List<string> tags)
		{
			var tagsTrim = new List<string>();
			foreach(string tag in tags)
			{
				tagsTrim.Add(tag.ToLower().Trim());
			}

			try
			{
				//string jsonContent = File.ReadAllText(jsonFilePath);
				//var allSettings = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonContent);

				var allSettings = tpSection.ToObject<Dictionary<string, Dictionary<string, string>>>();


				if (allSettings == null) return;

				if (allSettings.ContainsKey("default"))
				{
					ApplySettings(allSettings["default"]);
				}

				foreach (var allSettingsKeys in allSettings.Keys)
				{
					string allSettingsKeysTrim = allSettingsKeys.Trim().Replace(" ", "").ToLower();
					if (allSettingsKeys == "default") continue;

					if (allSettingsKeysTrim.Contains("&")) continue;

					if (tags.Contains(allSettingsKeysTrim))
					{
						ApplySettings(allSettings[allSettingsKeys]);
					}
				}

				foreach (var allSettingsKeys in allSettings.Keys)
				{
					string allSettingsKeysTrim = allSettingsKeys.Trim().Replace(" ", "").ToLower();
					if (allSettingsKeys == "default") continue;
					if (!allSettingsKeys.Contains("&")) continue;

					bool allTagValid = true;
					foreach (var tagelem in allSettingsKeysTrim.Split('&'))
					{
						if (!tagsTrim.Contains(tagelem))
						{
							allTagValid = false;
							break;
						}
					}

					if (allTagValid)
					{
						ApplySettings(allSettings[allSettingsKeys]);
					}
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine("Error during overwrite: " + ex.Message);
			}
			
		}

		private void ApplySettings(Dictionary<string, string> settings)
		{
			foreach (var setting in settings)
			{
				var propertyName = setting.Key;
				var propertyValue = setting.Value;

				PropertyInfo property = typeof(GameSettings).GetProperty(propertyName);
				if (property != null && property.CanWrite)
				{
					try
					{
						object convertedValue = null;
						if (property.PropertyType == typeof(bool))
						{
							convertedValue = bool.Parse(propertyValue);
						}
						else if (property.PropertyType == typeof(int))
						{
							convertedValue = int.Parse(propertyValue);
						}
						else if (property.PropertyType == typeof(string))
						{
							convertedValue = propertyValue;
						}

						property.SetValue(this, convertedValue);
					}
					catch (Exception ex)
					{
						Console.WriteLine($"Erreur lors de la conversion de la propriété '{propertyName}': {ex.Message}");
					}
				}
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
