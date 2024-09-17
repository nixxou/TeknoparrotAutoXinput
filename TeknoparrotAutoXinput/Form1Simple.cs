using Krypton.Toolkit;
using Newtonsoft.Json;
using SDL2;
using SharpDX.DirectInput;
using SharpDX.Multimedia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using TestVgme;

namespace TeknoparrotAutoXinput
{
	public partial class Form1Simple : KryptonForm
	{

		private List<string> FFBGuidList = new List<string>();

		private string previous_gunARecoil = "";
		private string previous_gunBRecoil = "";
		private bool DoRedoGunRecoilCombo = false;
		bool isInit = true;
		public Form1Simple()
		{
			InitializeComponent();

			//System.Diagnostics.Debugger.Break();
			chk_patchResolutionFix.Checked = ConfigurationManager.MainConfig.patchResolutionFix;
			chk_enableAdvancedOptions.Checked = ConfigurationManager.MainConfig.advancedConfig;

			txt_tplicence.Text = Utils.Decrypt(ConfigurationManager.MainConfig.tpLicence);
			chk_tplicence_onstart.Checked = ConfigurationManager.MainConfig.tpLicenceRegOnLaunch;
			chk_tplicence_unreg_onlaunch.Checked = ConfigurationManager.MainConfig.tpLicenceUnRegAfterStart;
			chk_tplicence_unreg_onexit.Checked = !chk_tplicence_unreg_onlaunch.Checked;
			groupBox17.Visible = ConfigurationManager.MainConfig.tpLicenceShow;

			cmb_performance.SelectedIndex = ConfigurationManager.MainConfig.performanceProfile;
			chk_forcevsync.Checked = ConfigurationManager.MainConfig.forceVsync;

			cmb_gpu.SelectedIndex = ConfigurationManager.MainConfig.gpuType;

			cmb_resolution.SelectedIndex = ConfigurationManager.MainConfig.gpuResolution;
			cmb_displayMode.SelectedIndex = ConfigurationManager.MainConfig.displayMode;
			chk_patchFFB.Checked = ConfigurationManager.MainConfig.patch_FFB;
			chk_useBezel.Checked = ConfigurationManager.MainConfig.useBezel;
			chk_useCrt.Checked = ConfigurationManager.MainConfig.useCrt;
			chk_keepAspectRatio.Checked = ConfigurationManager.MainConfig.keepAspectRatio;
			chk_patchReshade.Checked = ConfigurationManager.MainConfig.patchReshade;
			chk_patchGameID.Checked = ConfigurationManager.MainConfig.patchGameID;


			txt_apm3id.Text = ConfigurationManager.MainConfig.patch_apm3id;
			txt_mariokartId.Text = ConfigurationManager.MainConfig.patch_mariokartId;
			txt_customName.Text = ConfigurationManager.MainConfig.patch_customName;


			cmb_showStartup.SelectedIndex = ConfigurationManager.MainConfig.TPConsoleAction;

			chk_showStartup.Checked = ConfigurationManager.MainConfig.showStartup;

			radio_useCustomStooz_Gamepad.Checked = ConfigurationManager.MainConfig.gamepadStooz;
			radio_useCustomStooz_Wheel.Checked = ConfigurationManager.MainConfig.wheelStooz;
			radio_useCustomStooz_Hotas.Checked = ConfigurationManager.MainConfig.hotasStooz;
			radio_useDefaultStooze_Gamepad.Checked = !radio_useCustomStooz_Gamepad.Checked;
			radio_useDefaultStooze_Wheel.Checked = !radio_useCustomStooz_Wheel.Checked;
			radio_useDefaultStooze_Hotas.Checked = !radio_useCustomStooz_Hotas.Checked;

			chk_enableStoozZone_Gamepad.Checked = ConfigurationManager.MainConfig.enableStoozZone_Gamepad;
			trk_useCustomStooz_Gamepad.Value = ConfigurationManager.MainConfig.valueStooz_Gamepad;
			chk_enableStoozZone_Wheel.Checked = ConfigurationManager.MainConfig.enableStoozZone_Wheel;
			trk_useCustomStooz_Wheel.Value = ConfigurationManager.MainConfig.valueStooz_Wheel;
			chk_enableStoozZone_Hotas.Checked = ConfigurationManager.MainConfig.enableStoozZone_Hotas;
			trk_useCustomStooz_Hotas.Value = ConfigurationManager.MainConfig.valueStooz_Hotas;

			chk_useDinputWheel.Checked = ConfigurationManager.MainConfig.useDinputWheel;
			chk_useDinputShifter.Checked = ConfigurationManager.MainConfig.useDinputShifter;
			chk_useDinputHotas.Checked = ConfigurationManager.MainConfig.useDinputHotas;

			txt_ffbguid.Text = ConfigurationManager.MainConfig.ffbDinputWheel;
			txt_ffbguidHotas.Text = ConfigurationManager.MainConfig.ffbDinputHotas;

			chk_favorAB.Checked = ConfigurationManager.MainConfig.favorAB;
			chk_favorJoystick.Checked = ConfigurationManager.MainConfig.favorJoystick;

			txt_tpfolder.Text = ConfigurationManager.MainConfig.TpFolder;
			txt_monitorswitch.Text = ConfigurationManager.MainConfig.Disposition;

			chk_enableDebug.Checked = ConfigurationManager.MainConfig.debugMode;

			txt_linksourcefolder.Text = ConfigurationManager.MainConfig.perGameLinkFolder;
			txt_linksourcefolderexe.Text = ConfigurationManager.MainConfig.perGameLinkFolderExe;
			chk_useHotasWithWheel.Checked = ConfigurationManager.MainConfig.useHotasWithWheel;

			chk_reasignGunPedal.Checked = ConfigurationManager.MainConfig.reasignPedals;
			cmb_gunA_type.SelectedIndex = 0;
			for (int i = 0; i < cmb_gunA_type.Items.Count; i++)
			{
				if (cmb_gunA_type.Items[i].ToString() == ConfigurationManager.MainConfig.gunAType)
				{
					cmb_gunA_type.SelectedIndex = i;
					break;
				}
			}
			cmb_gunB_type.SelectedIndex = 0;
			for (int i = 0; i < cmb_gunB_type.Items.Count; i++)
			{
				if (cmb_gunB_type.Items[i].ToString() == ConfigurationManager.MainConfig.gunBType)
				{
					cmb_gunB_type.SelectedIndex = i;
					break;
				}
			}
			if (cmb_gunA_type.SelectedIndex <= 0) btn_gunA_configure.Enabled = false;
			if (cmb_gunB_type.SelectedIndex <= 0) btn_gunB_configure.Enabled = false;


			previous_gunARecoil = ConfigurationManager.MainConfig.gunARecoil;
			previous_gunBRecoil = ConfigurationManager.MainConfig.gunBRecoil;


			cmb_gunA_com.Items.Clear();
			cmb_gunA_com.Items.Add("<none>");
			for (int i = 0; i < 256; i++) cmb_gunA_com.Items.Add("COM " + (i + 1));
			cmb_gunB_com.Items.Clear();
			cmb_gunB_com.Items.Add("<none>");
			for (int i = 0; i < 256; i++) cmb_gunB_com.Items.Add("COM " + (i + 1));


			RedoGunRecoilCombo();

			radio_gunA_sindenPump1.Checked = ConfigurationManager.MainConfig.gunASidenPump == 1 ? true : false;
			radio_gunA_sindenPump2.Checked = ConfigurationManager.MainConfig.gunASidenPump == 2 ? true : false;
			radio_gunA_sindenPump3.Checked = ConfigurationManager.MainConfig.gunASidenPump == 3 ? true : false;
			radio_gunB_sindenPump1.Checked = ConfigurationManager.MainConfig.gunBSidenPump == 1 ? true : false;
			radio_gunB_sindenPump2.Checked = ConfigurationManager.MainConfig.gunBSidenPump == 2 ? true : false;
			radio_gunB_sindenPump3.Checked = ConfigurationManager.MainConfig.gunBSidenPump == 3 ? true : false;


			chk_gunA_Crosshair.Checked = ConfigurationManager.MainConfig.gunACrosshair;
			chk_gunB_Crosshair.Checked = ConfigurationManager.MainConfig.gunBCrosshair;

			chk_gunA_AutoJoy.Checked = ConfigurationManager.MainConfig.gunAAutoJoy;
			chk_gunB_AutoJoy.Checked = ConfigurationManager.MainConfig.gunBAutoJoy;

			cmb_gunA_com.SelectedIndex = ConfigurationManager.MainConfig.gunAComPort;
			cmb_gunB_com.SelectedIndex = ConfigurationManager.MainConfig.gunBComPort;

			chk_gunA_Vjoy.Checked = ConfigurationManager.MainConfig.gunAvjoy;
			chk_gunB_Vjoy.Checked = ConfigurationManager.MainConfig.gunBvjoy;

			chk_gunA_4tiers.Checked = ConfigurationManager.MainConfig.gunA4tiers;
			chk_gunB_4tiers.Checked = ConfigurationManager.MainConfig.gunB4tiers;

			chk_gunA_domagerumble.Checked = ConfigurationManager.MainConfig.gunAdomagerumble;
			chk_gunB_domagerumble.Checked = ConfigurationManager.MainConfig.gunBdomagerumble;

			chk_reversePedal.Checked = ConfigurationManager.MainConfig.reversePedals;
			chk_alwaysrunmamehooker.Checked = ConfigurationManager.MainConfig.alwaysRunMamehooker;

			chk_gunA_OffscreenReload.Checked = ConfigurationManager.MainConfig.gunAOffscreenReload;
			chk_gunB_OffscreenReload.Checked = ConfigurationManager.MainConfig.gunBOffscreenReload;

			num_gunA_sindenRecoil1.Value = ConfigurationManager.MainConfig.gunA_sindenRecoil1;
			num_gunA_sindenRecoil2.Value = ConfigurationManager.MainConfig.gunA_sindenRecoil2;
			num_gunA_sindenRecoil3.Value = ConfigurationManager.MainConfig.gunA_sindenRecoil3;
			num_gunB_sindenRecoil1.Value = ConfigurationManager.MainConfig.gunB_sindenRecoil1;
			num_gunB_sindenRecoil2.Value = ConfigurationManager.MainConfig.gunB_sindenRecoil2;
			num_gunB_sindenRecoil3.Value = ConfigurationManager.MainConfig.gunB_sindenRecoil3;


			updateStooz();

			{
				int selectedFFBIndex = -1;
				int selectedFTBIndexHotas = -1;
				SDL2.SDL.SDL_Quit();
				SDL2.SDL.SDL_SetHint(SDL2.SDL.SDL_HINT_JOYSTICK_RAWINPUT, "0");
				SDL2.SDL.SDL_Init(SDL2.SDL.SDL_INIT_JOYSTICK | SDL2.SDL.SDL_INIT_GAMECONTROLLER);
				SDL2.SDL.SDL_JoystickUpdate();
				for (int i = 0; i < SDL2.SDL.SDL_NumJoysticks(); i++)
				{
					var currentJoy = SDL.SDL_JoystickOpen(i);
					string nameController = SDL2.SDL.SDL_JoystickNameForIndex(i).Trim('\0');
					{

						const int bufferSize = 256; // La taille doit être au moins 33 pour stocker le GUID sous forme de chaîne (32 caractères + le caractère nul)
						byte[] guidBuffer = new byte[bufferSize];
						SDL.SDL_JoystickGetGUIDString(SDL.SDL_JoystickGetGUID(currentJoy), guidBuffer, bufferSize);
						string guidString = System.Text.Encoding.UTF8.GetString(guidBuffer).Trim('\0');
						FFBGuidList.Add(guidString);
						cmb_ffbguid.Items.Add(nameController);
						cmb_ffbguidHotas.Items.Add(nameController);
						if (!string.IsNullOrEmpty(guidString) && guidString == txt_ffbguid.Text)
						{
							selectedFFBIndex = i;
						}
						if (!string.IsNullOrEmpty(guidString) && guidString == txt_ffbguidHotas.Text)
						{
							selectedFTBIndexHotas = i;
						}
						SDL.SDL_JoystickClose(currentJoy);
					}
				}
				SDL2.SDL.SDL_Quit();
				if (selectedFFBIndex != -1)
				{
					cmb_ffbguid.SelectedIndex = selectedFFBIndex;
				}
				if (selectedFTBIndexHotas != -1)
				{
					cmb_ffbguidHotas.SelectedIndex = selectedFTBIndexHotas;
				}
			}
			isInit = false;

		}

		public void RedoGunRecoilCombo()
		{
			DoRedoGunRecoilCombo = true;
			cmb_gunA_recoil.Items.Clear();
			cmb_gunB_recoil.Items.Clear();
			cmb_gunA_recoil.Items.Add("<none>");
			if (cmb_gunA_type.SelectedItem != null)
			{
				if (cmb_gunA_type.SelectedItem.ToString() == "sinden")
				{
					cmb_gunA_recoil.Items.Add("sinden-gun1");
					cmb_gunA_recoil.Items.Add("sinden-gun2");
				}
				if (cmb_gunA_type.SelectedItem.ToString() == "guncon1" || cmb_gunA_type.SelectedItem.ToString() == "guncon2") cmb_gunA_recoil.Items.Add("gun4ir");
				if (cmb_gunA_type.SelectedItem.ToString() == "gamepad") cmb_gunA_recoil.Items.Add("rumble");
				if (cmb_gunA_type.SelectedItem.ToString() == "wiimote") cmb_gunA_recoil.Items.Add("lichtknarre");
				if (cmb_gunA_type.SelectedItem.ToString() != "<none>") cmb_gunA_recoil.Items.Add("mamehooker");
			}

			cmb_gunB_recoil.Items.Add("<none>");
			if (cmb_gunB_type.SelectedItem != null)
			{
				if (cmb_gunB_type.SelectedItem.ToString() == "sinden")
				{
					cmb_gunB_recoil.Items.Add("sinden-gun1");
					cmb_gunB_recoil.Items.Add("sinden-gun2");
				}
				if (cmb_gunB_type.SelectedItem.ToString() == "guncon1" || cmb_gunB_type.SelectedItem.ToString() == "guncon2") cmb_gunB_recoil.Items.Add("gun4ir");
				if (cmb_gunB_type.SelectedItem.ToString() == "gamepad") cmb_gunB_recoil.Items.Add("rumble");
				if (cmb_gunB_type.SelectedItem.ToString() == "wiimote") cmb_gunB_recoil.Items.Add("lichtknarre");
				if (cmb_gunB_type.SelectedItem.ToString() != "<none>") cmb_gunB_recoil.Items.Add("mamehooker");
			}



			cmb_gunA_recoil.SelectedIndex = 0;
			for (int i = 0; i < cmb_gunA_recoil.Items.Count; i++)
			{
				if (cmb_gunA_recoil.Items[i].ToString() == previous_gunARecoil)
				{
					cmb_gunA_recoil.SelectedIndex = i;
					break;
				}
			}
			cmb_gunB_recoil.SelectedIndex = 0;
			for (int i = 0; i < cmb_gunB_recoil.Items.Count; i++)
			{
				if (cmb_gunB_recoil.Items[i].ToString() == previous_gunBRecoil)
				{
					cmb_gunB_recoil.SelectedIndex = i;
					break;
				}
			}

			grp_gunB_sindenOptions.Enabled = false;
			grp_gunA_sindenOptions.Enabled = false;
			if (cmb_gunA_type.SelectedItem != null && cmb_gunA_type.SelectedItem.ToString() == "sinden")
			{
				grp_gunA_sindenOptions.Enabled = true;
			}
			if (cmb_gunB_type.SelectedItem != null && cmb_gunB_type.SelectedItem.ToString() == "sinden")
			{
				grp_gunB_sindenOptions.Enabled = true;
			}

			grp_gunA_gun4irOptions.Enabled = false;
			grp_gunB_gun4irOptions.Enabled = false;
			if (cmb_gunA_recoil.SelectedItem != null && cmb_gunA_recoil.SelectedItem.ToString() == "gun4ir")
			{
				grp_gunA_gun4irOptions.Enabled = true;
			}
			if (cmb_gunB_recoil.SelectedItem != null && cmb_gunB_recoil.SelectedItem.ToString() == "gun4ir")
			{
				grp_gunB_gun4irOptions.Enabled = true;
			}
			DoRedoGunRecoilCombo = false;

			if (!isInit)
			{
				if (cmb_gunA_type.SelectedItem.ToString() == "guncon1" || cmb_gunA_type.SelectedItem.ToString() == "guncon2")
				{
					if (gun_preconfigDPI.isValidGun4ir(cmb_gunA_type.SelectedItem.ToString() == "guncon1" ? ConfigurationManager.MainConfig.bindingDinputGunAGuncon1 : ConfigurationManager.MainConfig.bindingDinputGunAGuncon2, cmb_gunA_com.SelectedIndex))
					{
						try
						{
							cmb_gunA_recoil.SelectedIndex = 1;
						}
						catch { }
					}
				}
				if (cmb_gunB_type.SelectedItem.ToString() == "guncon1" || cmb_gunB_type.SelectedItem.ToString() == "guncon2")
				{
					if (gun_preconfigDPI.isValidGun4ir(cmb_gunB_type.SelectedItem.ToString() == "guncon1" ? ConfigurationManager.MainConfig.bindingDinputGunBGuncon1 : ConfigurationManager.MainConfig.bindingDinputGunBGuncon2, cmb_gunB_com.SelectedIndex))
					{
						try
						{
							cmb_gunB_recoil.SelectedIndex = 1;
						}
						catch { }
					}
				}
			}
		}

		private void Form1Simple_FormClosing(object sender, FormClosingEventArgs e)
		{
			ConfigurationManager.MainConfig.patchResolutionFix = chk_patchResolutionFix.Checked;
			ConfigurationManager.MainConfig.showStartup = chk_showStartup.Checked;
			ConfigurationManager.MainConfig.useDinputWheel = chk_useDinputWheel.Checked;
			ConfigurationManager.MainConfig.favorAB = chk_favorAB.Checked;
			ConfigurationManager.MainConfig.favorJoystick = chk_favorJoystick.Checked;

			ConfigurationManager.MainConfig.useDinputShifter = chk_useDinputShifter.Checked;
			ConfigurationManager.MainConfig.useDinputHotas = chk_useDinputHotas.Checked;
			ConfigurationManager.MainConfig.useHotasWithWheel = chk_useHotasWithWheel.Checked;
			ConfigurationManager.MainConfig.TPConsoleAction = cmb_showStartup.SelectedIndex;


			ConfigurationManager.MainConfig.advancedConfig = chk_enableAdvancedOptions.Checked;


			ConfigurationManager.MainConfig.tpLicence = Utils.Encrypt(txt_tplicence.Text);
			ConfigurationManager.MainConfig.tpLicenceRegOnLaunch = chk_tplicence_onstart.Checked;
			ConfigurationManager.MainConfig.tpLicenceUnRegAfterStart = chk_tplicence_unreg_onlaunch.Checked;

			ConfigurationManager.MainConfig.performanceProfile = cmb_performance.SelectedIndex;
			ConfigurationManager.MainConfig.forceVsync = chk_forcevsync.Checked;

			ConfigurationManager.MainConfig.gpuType = cmb_gpu.SelectedIndex;
			ConfigurationManager.MainConfig.gpuResolution = cmb_resolution.SelectedIndex;
			ConfigurationManager.MainConfig.displayMode = cmb_displayMode.SelectedIndex;
			ConfigurationManager.MainConfig.patch_FFB = chk_patchFFB.Checked;
			if (chk_useCrt.Enabled) ConfigurationManager.MainConfig.useBezel = chk_useBezel.Checked;
			if (chk_useBezel.Enabled) ConfigurationManager.MainConfig.useCrt = chk_useCrt.Checked;
			if (chk_keepAspectRatio.Enabled) ConfigurationManager.MainConfig.keepAspectRatio = chk_keepAspectRatio.Checked;
			ConfigurationManager.MainConfig.patchReshade = chk_patchReshade.Checked;
			ConfigurationManager.MainConfig.patchGameID = chk_patchGameID.Checked;


			ConfigurationManager.MainConfig.patch_apm3id = txt_apm3id.Text;
			ConfigurationManager.MainConfig.patch_mariokartId = txt_mariokartId.Text;
			ConfigurationManager.MainConfig.patch_customName = txt_customName.Text;


			ConfigurationManager.MainConfig.gamepadStooz = radio_useCustomStooz_Gamepad.Checked;
			ConfigurationManager.MainConfig.wheelStooz = radio_useCustomStooz_Wheel.Checked;
			ConfigurationManager.MainConfig.hotasStooz = radio_useCustomStooz_Hotas.Checked;

			ConfigurationManager.MainConfig.enableStoozZone_Gamepad = chk_enableStoozZone_Gamepad.Checked;
			ConfigurationManager.MainConfig.valueStooz_Gamepad = trk_useCustomStooz_Gamepad.Value;
			ConfigurationManager.MainConfig.enableStoozZone_Wheel = chk_enableStoozZone_Wheel.Checked;
			ConfigurationManager.MainConfig.valueStooz_Wheel = trk_useCustomStooz_Wheel.Value;
			ConfigurationManager.MainConfig.enableStoozZone_Hotas = chk_enableStoozZone_Hotas.Checked;
			ConfigurationManager.MainConfig.valueStooz_Hotas = trk_useCustomStooz_Hotas.Value;

			ConfigurationManager.MainConfig.ffbDinputWheel = txt_ffbguid.Text;
			ConfigurationManager.MainConfig.ffbDinputHotas = txt_ffbguidHotas.Text;
			ConfigurationManager.MainConfig.gunAType = cmb_gunA_type.SelectedItem.ToString();
			ConfigurationManager.MainConfig.gunBType = cmb_gunB_type.SelectedItem.ToString();

			ConfigurationManager.MainConfig.gunARecoil = "<none>";
			ConfigurationManager.MainConfig.gunAComPort = 0;
			ConfigurationManager.MainConfig.gunAAutoJoy = false;
			ConfigurationManager.MainConfig.gunA4tiers = false;
			ConfigurationManager.MainConfig.gunAdomagerumble = false;
			if (cmb_gunA_recoil.SelectedItem != null)
			{
				string recoilValue = cmb_gunA_recoil.SelectedItem.ToString();
				ConfigurationManager.MainConfig.gunARecoil = recoilValue;
				//if (recoilValue == "gun4ir")
				{
					ConfigurationManager.MainConfig.gunAComPort = cmb_gunA_com.SelectedIndex;
					ConfigurationManager.MainConfig.gunAAutoJoy = chk_gunA_AutoJoy.Checked;
					ConfigurationManager.MainConfig.gunA4tiers = chk_gunA_4tiers.Checked;
					ConfigurationManager.MainConfig.gunAdomagerumble = chk_gunA_domagerumble.Checked;
				}
			}

			ConfigurationManager.MainConfig.gunBRecoil = "<none>";
			ConfigurationManager.MainConfig.gunBComPort = 0;
			ConfigurationManager.MainConfig.gunBAutoJoy = false;
			ConfigurationManager.MainConfig.gunB4tiers = false;
			ConfigurationManager.MainConfig.gunBdomagerumble = false;
			if (cmb_gunB_recoil.SelectedItem != null)
			{
				string recoilValue = cmb_gunB_recoil.SelectedItem.ToString();
				ConfigurationManager.MainConfig.gunBRecoil = recoilValue;
				//if (recoilValue == "gun4ir")
				{
					ConfigurationManager.MainConfig.gunBComPort = cmb_gunB_com.SelectedIndex;
					ConfigurationManager.MainConfig.gunBAutoJoy = chk_gunB_AutoJoy.Checked;
					ConfigurationManager.MainConfig.gunB4tiers = chk_gunB_4tiers.Checked;
					ConfigurationManager.MainConfig.gunBdomagerumble = chk_gunB_domagerumble.Checked;
				}
			}
			ConfigurationManager.MainConfig.gunACrosshair = chk_gunA_Crosshair.Checked;
			ConfigurationManager.MainConfig.gunBCrosshair = chk_gunB_Crosshair.Checked;

			ConfigurationManager.MainConfig.gunAvjoy = chk_gunA_Vjoy.Checked;
			ConfigurationManager.MainConfig.gunBvjoy = chk_gunB_Vjoy.Checked;

			ConfigurationManager.MainConfig.gunA_sindenRecoil1 = (int)num_gunA_sindenRecoil1.Value;
			ConfigurationManager.MainConfig.gunA_sindenRecoil2 = (int)num_gunA_sindenRecoil2.Value;
			ConfigurationManager.MainConfig.gunA_sindenRecoil3 = (int)num_gunA_sindenRecoil3.Value;
			ConfigurationManager.MainConfig.gunB_sindenRecoil1 = (int)num_gunB_sindenRecoil1.Value;
			ConfigurationManager.MainConfig.gunB_sindenRecoil2 = (int)num_gunB_sindenRecoil2.Value;
			ConfigurationManager.MainConfig.gunB_sindenRecoil3 = (int)num_gunB_sindenRecoil3.Value;

			ConfigurationManager.MainConfig.reasignPedals = chk_reasignGunPedal.Checked;
			ConfigurationManager.MainConfig.reversePedals = chk_reversePedal.Checked;
			if (!chk_reasignGunPedal.Checked) ConfigurationManager.MainConfig.reversePedals = false;


			ConfigurationManager.MainConfig.gunASidenPump = 1;
			//if (ConfigurationManager.MainConfig.gunAType == "sinden")
			{
				if (radio_gunA_sindenPump1.Checked) ConfigurationManager.MainConfig.gunASidenPump = 1;
				if (radio_gunA_sindenPump2.Checked) ConfigurationManager.MainConfig.gunASidenPump = 2;
				if (radio_gunA_sindenPump3.Checked) ConfigurationManager.MainConfig.gunASidenPump = 3;
			}
			ConfigurationManager.MainConfig.gunBSidenPump = 1;
			//if (ConfigurationManager.MainConfig.gunBType == "sinden")
			{
				if (radio_gunB_sindenPump1.Checked) ConfigurationManager.MainConfig.gunBSidenPump = 1;
				if (radio_gunB_sindenPump2.Checked) ConfigurationManager.MainConfig.gunBSidenPump = 2;
				if (radio_gunB_sindenPump3.Checked) ConfigurationManager.MainConfig.gunBSidenPump = 3;
			}

			ConfigurationManager.MainConfig.gunAOffscreenReload = chk_gunA_OffscreenReload.Checked;
			ConfigurationManager.MainConfig.gunBOffscreenReload = chk_gunB_OffscreenReload.Checked;

			ConfigurationManager.SaveConfig();
		}

		private void trk_useCustomStooz_Gamepad_Scroll(object sender, EventArgs e)
		{
			lbl_useCustomStooz_Gamepad.Text = trk_useCustomStooz_Gamepad.Value.ToString() + "%";
		}

		private void trk_useCustomStooz_Wheel_Scroll(object sender, EventArgs e)
		{
			lbl_useCustomStooz_Wheel.Text = trk_useCustomStooz_Wheel.Value.ToString() + "%";
		}

		private void trk_useCustomStooz_Hotas_Scroll(object sender, EventArgs e)
		{
			lbl_useCustomStooz_Hotas.Text = trk_useCustomStooz_Hotas.Value.ToString() + "%";
		}

		private void updateStooz()
		{
			chk_enableStoozZone_Gamepad.Enabled = radio_useCustomStooz_Gamepad.Checked;
			trk_useCustomStooz_Gamepad.Enabled = radio_useCustomStooz_Gamepad.Checked;
			chk_enableStoozZone_Gamepad.Visible = radio_useCustomStooz_Gamepad.Checked;
			trk_useCustomStooz_Gamepad.Visible = radio_useCustomStooz_Gamepad.Checked;
			lbl_useCustomStooz_Gamepad.Text = trk_useCustomStooz_Gamepad.Value.ToString() + "%";
			lbl_useCustomStooz_Gamepad.Visible = radio_useCustomStooz_Gamepad.Checked;

			chk_enableStoozZone_Wheel.Enabled = radio_useCustomStooz_Wheel.Checked;
			trk_useCustomStooz_Wheel.Enabled = radio_useCustomStooz_Wheel.Checked;
			chk_enableStoozZone_Wheel.Visible = radio_useCustomStooz_Wheel.Checked;
			trk_useCustomStooz_Wheel.Visible = radio_useCustomStooz_Wheel.Checked;
			lbl_useCustomStooz_Wheel.Text = trk_useCustomStooz_Wheel.Value.ToString() + "%";
			lbl_useCustomStooz_Wheel.Visible = radio_useCustomStooz_Wheel.Checked;

			chk_enableStoozZone_Hotas.Enabled = radio_useCustomStooz_Hotas.Checked;
			trk_useCustomStooz_Hotas.Enabled = radio_useCustomStooz_Hotas.Checked;
			chk_enableStoozZone_Hotas.Visible = radio_useCustomStooz_Hotas.Checked;
			trk_useCustomStooz_Hotas.Visible = radio_useCustomStooz_Hotas.Checked;
			lbl_useCustomStooz_Hotas.Text = trk_useCustomStooz_Hotas.Value.ToString() + "%";
			lbl_useCustomStooz_Hotas.Visible = radio_useCustomStooz_Hotas.Checked;
		}

		private void radio_useDefaultStooze_Gamepad_CheckedChanged(object sender, EventArgs e)
		{
			updateStooz();
		}

		private void radio_useCustomStooz_Gamepad_CheckedChanged(object sender, EventArgs e)
		{
			updateStooz();
		}

		private void radio_useDefaultStooze_Wheel_CheckedChanged(object sender, EventArgs e)
		{
			updateStooz();
		}

		private void radio_useCustomStooz_Wheel_CheckedChanged(object sender, EventArgs e)
		{
			updateStooz();
		}

		private void radio_useDefaultStooze_Hotas_CheckedChanged(object sender, EventArgs e)
		{
			updateStooz();
		}

		private void radio_useCustomStooz_Hotas_CheckedChanged(object sender, EventArgs e)
		{
			updateStooz();
		}

		private void btn_configureDinputWheel_Click(object sender, EventArgs e)
		{
			var frm = new dinputwheel();
			var result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{

			}
		}

		private void btn_setffbguid_Click(object sender, EventArgs e)
		{
			if (cmb_ffbguid.SelectedIndex >= 0)
			{
				txt_ffbguid.Text = FFBGuidList[cmb_ffbguid.SelectedIndex];
			}
		}

		private void btn_Save_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void btn_editMonitorSwitch_Click(object sender, EventArgs e)
		{
			var frm = new MonitorDispositionConfig();
			var result = frm.ShowDialog();

			if (result == DialogResult.OK)
			{
				txt_monitorswitch.Text = frm.result;
				ConfigurationManager.MainConfig.Disposition = frm.result;
				ConfigurationManager.SaveConfig();
				//Profile.ActiveProfile.SetOption("monitorswitch", frm.result);
				//ReloadDispositionCmb();
			}
		}

		private void btn_checkConfig_Click(object sender, EventArgs e)
		{
			bool noerror = true;
			if (string.IsNullOrEmpty(txt_tpfolder.Text))
			{
				MessageBox.Show("You need to setup TP Directory First");
				return;
			}
			string gameProfilesDir = Path.Combine(txt_tpfolder.Text, "GameProfiles");
			if (!Directory.Exists(gameProfilesDir))
			{
				MessageBox.Show("Invalid TP Directory");
				return;
			}
			var gameProfileFiles = Directory.GetFiles(gameProfilesDir, "*.xml");
			foreach (var gameProfileFile in gameProfileFiles)
			{
				// Charger le fichier XML
				XDocument doc = XDocument.Load(gameProfileFile);

				// Récupérer tous les éléments <ButtonName>
				var buttonNames = doc.Descendants("ButtonName")
									 .Select(button => button.Value);


				string gamepadConfig = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "config", Path.GetFileNameWithoutExtension(gameProfileFile) + ".gamepad.txt");
				if (File.Exists(gamepadConfig))
				{
					string gamepadConfigContent = File.ReadAllText(gamepadConfig);
					// Utiliser une expression régulière pour extraire les valeurs des éléments <ButtonName>
					var buttonNameMatches = Regex.Matches(gamepadConfigContent, "<ButtonName>(.*?)</ButtonName>")
												 .Cast<Match>()
												 .Select(match => match.Groups[1].Value);

					if (buttonNameMatches.Count() != buttonNames.Count())
					{
						MessageBox.Show($"{Path.GetFileName(gameProfileFile)} : Button count not match");
						noerror = false;
					}
					foreach (var buttonName in buttonNames)
					{
						bool found = false;
						foreach (var buttonNameMatche in buttonNameMatches)
						{
							if (buttonName == buttonNameMatche)
							{
								found = true;
								break;
							}
						}
						if (!found)
						{
							MessageBox.Show($"{Path.GetFileName(gameProfileFile)} : Button {buttonName} not found");
							noerror = false;
						}
					}


				}

			}
			if (noerror)
			{
				MessageBox.Show("No Error found :)");
			}
		}

		private void btn_configureDinputShifter_Click(object sender, EventArgs e)
		{
			var frm = new dinputshifter();
			var result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{

			}
		}

		private void btn_configureDinputHotas_Click(object sender, EventArgs e)
		{
			var frm = new dinputhotas();
			var result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{

			}
		}

		private void btn_setffbguidHotas_Click(object sender, EventArgs e)
		{
			if (cmb_ffbguidHotas.SelectedIndex >= 0)
			{
				txt_ffbguidHotas.Text = FFBGuidList[cmb_ffbguidHotas.SelectedIndex];
			}
		}

		private void btn_gunA_configure_Click(object sender, EventArgs e)
		{
			var frm = new dinputgun(1, cmb_gunA_type.SelectedItem.ToString());
			var result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{
				if (frm.GunCom > 0)
				{
					cmb_gunA_com.SelectedIndex = frm.GunCom;
					chk_gunA_AutoJoy.Checked = true;
					chk_gunA_domagerumble.Checked = true;
					chk_gunA_4tiers.Checked = false;
					cmb_gunA_recoil.SelectedIndex = 1;
				}
				if (frm.GunRumbleIndex > 0)
				{
					cmb_gunA_recoil.SelectedIndex = frm.GunRumbleIndex;
				}
			}
		}

		private void cmb_gunA_type_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cmb_gunA_type.SelectedIndex > 0)
			{
				btn_gunA_configure.Enabled = true;
			}
			else
			{
				btn_gunA_configure.Enabled = false;
			}
			RedoGunRecoilCombo();
		}

		private void btn_gunB_configure_Click(object sender, EventArgs e)
		{
			var frm = new dinputgun(2, cmb_gunB_type.SelectedItem.ToString());
			var result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{
				if (frm.GunCom > 0)
				{
					cmb_gunB_com.SelectedIndex = frm.GunCom;
					chk_gunB_AutoJoy.Checked = true;
					chk_gunB_domagerumble.Checked = true;
					chk_gunB_4tiers.Checked = false;
					cmb_gunB_recoil.SelectedIndex = 1;
				}
				if (frm.GunRumbleIndex > 0)
				{
					cmb_gunB_recoil.SelectedIndex = frm.GunRumbleIndex;
				}
			}
		}

		private void cmb_gunB_type_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cmb_gunB_type.SelectedIndex > 0)
			{
				btn_gunB_configure.Enabled = true;
			}
			else
			{
				btn_gunB_configure.Enabled = false;
			}
			RedoGunRecoilCombo();
		}

		private void chk_reasignGunPedal_CheckedChanged(object sender, EventArgs e)
		{
			if (chk_reversePedal.Checked)
			{
				chk_reversePedal.Enabled = false;
			}
			else
			{
				chk_reversePedal.Enabled = true;
			}
		}

		private void cmb_gunA_recoil_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!DoRedoGunRecoilCombo)
			{
				if (cmb_gunA_recoil.SelectedItem != null)
				{
					previous_gunARecoil = cmb_gunA_recoil.SelectedItem.ToString();
				}
				RedoGunRecoilCombo();
			}

			//ConfigurationManager.MainConfig.gunARecoil = cmb_gunA_recoil.SelectedItem.ToString();
			//ConfigurationManager.SaveConfig();
			//RedoGunRecoilCombo();
		}

		private void cmb_gunB_recoil_SelectedIndexChanged(object sender, EventArgs e)
		{
			//ConfigurationManager.MainConfig.gunBRecoil = cmb_gunB_recoil.SelectedItem.ToString();
			//ConfigurationManager.SaveConfig();
			//RedoGunRecoilCombo();
			if (!DoRedoGunRecoilCombo)
			{
				if (cmb_gunB_recoil.SelectedItem != null)
				{
					previous_gunBRecoil = cmb_gunB_recoil.SelectedItem.ToString();
				}
				RedoGunRecoilCombo();
			}

		}


		private void btn_runSinden_Click(object sender, EventArgs e)
		{
			if (File.Exists(ConfigurationManager.MainConfig.sindenExe))
			{
				string GunAGuid = "";
				string GunBGuid = "";
				bool dinputLightgunAFound = false;
				bool dinputLightgunBFound = false;
				int GunASindenType = 0;
				int GunBSindenType = 0;

				if (cmb_gunA_type.SelectedItem.ToString() == "sinden")
				{
					var bindingDinputLightgunAJson = ConfigurationManager.MainConfig.bindingDinputGunASinden;
					if (!string.IsNullOrEmpty(bindingDinputLightgunAJson))
					{
						var bindingDinputLightGunA = (Dictionary<string, JoystickButtonData>)JsonConvert.DeserializeObject<Dictionary<string, JoystickButtonData>>(bindingDinputLightgunAJson);
						if (bindingDinputLightGunA != null && bindingDinputLightGunA.ContainsKey("LightgunX"))
						{
							GunAGuid = bindingDinputLightGunA["LightgunX"].JoystickGuid.ToString();
						}
					}
				}
				if (cmb_gunB_type.SelectedItem.ToString() == "sinden")
				{
					var bindingDinputLightgunBJson = ConfigurationManager.MainConfig.bindingDinputGunBSinden;
					if (!string.IsNullOrEmpty(bindingDinputLightgunBJson))
					{
						var bindingDinputLightGunB = (Dictionary<string, JoystickButtonData>)JsonConvert.DeserializeObject<Dictionary<string, JoystickButtonData>>(bindingDinputLightgunBJson);
						if (bindingDinputLightGunB != null && bindingDinputLightGunB.ContainsKey("LightgunX"))
						{
							GunBGuid = bindingDinputLightGunB["LightgunX"].JoystickGuid.ToString();
						}
					}
				}
				if (!string.IsNullOrEmpty(GunAGuid) || !string.IsNullOrEmpty(GunBGuid))
				{
					DirectInput directInput = new DirectInput();
					List<DeviceInstance> devices = new List<DeviceInstance>();
					devices.AddRange(directInput.GetDevices().Where(x => x.Type == SharpDX.DirectInput.DeviceType.FirstPerson && x.UsagePage != UsagePage.VendorDefinedBegin && x.Usage != UsageId.AlphanumericBitmapSizeX && x.Usage != UsageId.AlphanumericAlphanumericDisplay && x.UsagePage != unchecked((UsagePage)0xffffff43) && x.UsagePage != UsagePage.Vr).ToList());
					foreach (var device in devices)
					{
						if (device.InstanceGuid.ToString() == GunAGuid)
						{
							Utils.LogMessage($"GunAGuid Found");
							dinputLightgunAFound = true;
							if (GunAGuid != "")
							{
								var joystick = new Joystick(directInput, device.InstanceGuid);
								var sindentype = 0;
								if (joystick.Properties.InterfacePath.ToUpper().Contains("VID_16C0&PID_0F38")) sindentype = 1; //Black
								if (joystick.Properties.InterfacePath.ToUpper().Contains("VID_16C0&PID_0F01")) sindentype = 2; //Blue
								if (joystick.Properties.InterfacePath.ToUpper().Contains("VID_16C0&PID_0F02")) sindentype = 3; //Red
								if (joystick.Properties.InterfacePath.ToUpper().Contains("VID_16C0&PID_0F39")) sindentype = 4; //Player2
								GunASindenType = sindentype;
								joystick.Dispose();
							}
						}
						if (device.InstanceGuid.ToString() == GunBGuid)
						{
							Utils.LogMessage($"GunBGuid Found");
							dinputLightgunBFound = true;
							if (GunBGuid != "")
							{
								var joystick = new Joystick(directInput, device.InstanceGuid);
								var sindentype = 0;
								if (joystick.Properties.InterfacePath.ToUpper().Contains("VID_16C0&PID_0F38")) sindentype = 1; //Black
								if (joystick.Properties.InterfacePath.ToUpper().Contains("VID_16C0&PID_0F01")) sindentype = 2; //Blue
								if (joystick.Properties.InterfacePath.ToUpper().Contains("VID_16C0&PID_0F02")) sindentype = 3; //Red
								if (joystick.Properties.InterfacePath.ToUpper().Contains("VID_16C0&PID_0F39")) sindentype = 4; //Player2
								GunBSindenType = sindentype;
								joystick.Dispose();
							}
						}
					}
				}
				if (!dinputLightgunAFound) GunAGuid = "";
				if (!dinputLightgunBFound) GunBGuid = "";

				int firstSindenGun = 0;
				int secondSindenGun = 0;

				string RumbleTypeA = cmb_gunA_recoil.SelectedItem.ToString();
				string RumbleTypeB = cmb_gunB_recoil.SelectedItem.ToString();

				if (RumbleTypeA == RumbleTypeB) RumbleTypeB = "";

				if (GunAGuid != "" && GunASindenType > 0)
				{

					if (RumbleTypeA == "sinden-gun1" && firstSindenGun == 0) firstSindenGun = 1;
					if (RumbleTypeA == "sinden-gun2" && secondSindenGun == 0) secondSindenGun = 1;
				}
				if (GunBGuid != "" && GunBSindenType > 0)
				{

					if (RumbleTypeB == "sinden-gun1" && firstSindenGun == 0) firstSindenGun = 2;
					if (RumbleTypeB == "sinden-gun2" && secondSindenGun == 0) secondSindenGun = 2;
				}
				if (GunAGuid != "" && GunASindenType > 0 && firstSindenGun != 1 && secondSindenGun != 1)
				{
					if (firstSindenGun == 0) firstSindenGun = 1;
					else if (secondSindenGun == 0) secondSindenGun = 1;
				}
				if (GunBGuid != "" && GunBSindenType > 0 && firstSindenGun != 2 && secondSindenGun != 2)
				{
					if (firstSindenGun == 0) firstSindenGun = 2;
					else if (secondSindenGun == 0) secondSindenGun = 2;
				}

				if (firstSindenGun == secondSindenGun) secondSindenGun = 0; //shouldn't happen


				int firstSindenType = 0;
				int secondSindenType = 0;

				if (firstSindenGun > 0)
				{
					if (firstSindenGun == 1) firstSindenType = GunASindenType;
					if (firstSindenGun == 2) firstSindenType = GunBSindenType;
				}
				if (secondSindenGun > 0)
				{
					if (secondSindenGun == 1) secondSindenType = GunASindenType;
					if (secondSindenGun == 2) secondSindenType = GunBSindenType;
				}

				string argument_sinden = ConfigurationManager.MainConfig.sindenExtraCmd;
				if (argument_sinden != "") argument_sinden += " ";
				argument_sinden += $"--p1-{firstSindenType}-7-6-6 --p2-{secondSindenType}-7-6-6";


				Process siden_process = new Process();
				siden_process.StartInfo.FileName = ConfigurationManager.MainConfig.sindenExe;
				siden_process.StartInfo.WorkingDirectory = Path.GetDirectoryName(ConfigurationManager.MainConfig.sindenExe);
				siden_process.StartInfo.Arguments = argument_sinden;
				siden_process.StartInfo.UseShellExecute = true;
				siden_process.Start();

			}

		}




		private void btn_apm3id_show_Click(object sender, EventArgs e)
		{
			if (txt_apm3id.PasswordChar == '*') txt_apm3id.PasswordChar = '\0';
			else txt_apm3id.PasswordChar = '*';
		}

		private void btn_mariokartId_show_Click(object sender, EventArgs e)
		{
			if (txt_mariokartId.PasswordChar == '*') txt_mariokartId.PasswordChar = '\0';
			else txt_mariokartId.PasswordChar = '*';
		}

		private void btn_tplicence_show_Click(object sender, EventArgs e)
		{
			if (txt_tplicence.PasswordChar == '*') txt_tplicence.PasswordChar = '\0';
			else txt_tplicence.PasswordChar = '*';
		}

		private void chk_tplicence_unreg_onexit_CheckedChanged(object sender, EventArgs e)
		{
			chk_tplicence_unreg_onlaunch.Checked = !chk_tplicence_unreg_onexit.Checked;
		}

		private void chk_tplicence_unreg_onlaunch_CheckedChanged(object sender, EventArgs e)
		{
			chk_tplicence_unreg_onexit.Checked = !chk_tplicence_unreg_onlaunch.Checked;
		}

		private void Form1Simple_Load(object sender, EventArgs e)
		{

		}

		private void kryptonButton2_Click(object sender, EventArgs e)
		{
			if (File.Exists(ConfigurationManager.MainConfig.mamehookerExe))
			{
				Process siden_process = new Process();
				siden_process.StartInfo.FileName = ConfigurationManager.MainConfig.mamehookerExe;
				siden_process.StartInfo.WorkingDirectory = Path.GetDirectoryName(ConfigurationManager.MainConfig.mamehookerExe);
				siden_process.StartInfo.UseShellExecute = true;
				siden_process.Start();

			}
		}

		private void chk_patchFFB_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void cmb_displayMode_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void cmb_performance_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cmb_performance.SelectedIndex == 1)
			{
				chk_useCrt.Enabled = false;
			}
			else if (chk_patchReshade.Checked)
			{
				chk_useCrt.Enabled = true;
			}
		}

		private void chk_patchReshade_CheckedChanged(object sender, EventArgs e)
		{
			if (chk_patchReshade.Checked == false)
			{
				chk_useCrt.Enabled = false;
				chk_useBezel.Enabled = false;
				chk_keepAspectRatio.Enabled = false;
			}
			else if (cmb_performance.SelectedIndex != 1)
			{
				chk_useCrt.Enabled = true;
				chk_useBezel.Enabled = true;
				chk_keepAspectRatio.Enabled = true;
			}
		}

	}
}
