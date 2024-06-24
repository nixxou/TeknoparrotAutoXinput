using Krypton.Toolkit;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Exceptions;
using SDL2;
using SharpDX.DirectInput;
using System.Buffers.Text;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TestVgme;
using XJoy;

namespace TeknoparrotAutoXinput
{
	public partial class Form1 : KryptonForm
	{
		private PickKeyCombo KeyPicker { get; set; }
		public Keys[] Keys { get; set; }

		private List<string> FFBGuidList = new List<string>();

		private string previous_gunARecoil = "";
		private string previous_gunBRecoil = "";
		private bool DoRedoGunRecoilCombo = false;
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			this.KeyPicker = new PickKeyCombo(this);
			//System.Diagnostics.Debugger.Break();

			txt_tplicence.Text = Utils.Decrypt(ConfigurationManager.MainConfig.tpLicence);
			chk_tplicence_onstart.Checked = ConfigurationManager.MainConfig.tpLicenceRegOnLaunch;
			chk_tplicence_unreg_onlaunch.Checked = ConfigurationManager.MainConfig.tpLicenceUnRegAfterStart;
			chk_tplicence_unreg_onexit.Checked = !chk_tplicence_unreg_onlaunch.Checked;

			cmb_gpu.SelectedIndex = ConfigurationManager.MainConfig.gpuType;
			chk_patchGpuFix.Checked = ConfigurationManager.MainConfig.patchGpuFix;
			chk_patchGpuTP.Checked = ConfigurationManager.MainConfig.patchGpuTP;
			cmb_resolution.SelectedIndex = ConfigurationManager.MainConfig.gpuResolution;
			chk_patchResolutionFix.Checked = ConfigurationManager.MainConfig.patchResolutionFix;
			chk_patchResolutionTP.Checked = ConfigurationManager.MainConfig.patchResolutionTP;
			cmb_displayMode.SelectedIndex = ConfigurationManager.MainConfig.displayMode;
			chk_patchDisplayModeFix.Checked = ConfigurationManager.MainConfig.patchDisplayModeFix;
			chk_patchDisplayModeTP.Checked = ConfigurationManager.MainConfig.patchDisplayModeTP;
			chk_patchFFB.Checked = ConfigurationManager.MainConfig.patch_FFB;
			chk_patchReshade.Checked = ConfigurationManager.MainConfig.patchReshade;
			chk_patchGameID.Checked = ConfigurationManager.MainConfig.patchGameID;
			chk_patchNetwork.Checked = ConfigurationManager.MainConfig.patchNetwork;
			chk_patchOthersTPSettings.Checked = ConfigurationManager.MainConfig.patchOtherTPSettings;
			chk_patchOthersGameOptions.Checked = ConfigurationManager.MainConfig.patchOthersGameOptions;


			txt_apm3id.Text = ConfigurationManager.MainConfig.patch_apm3id;
			txt_mariokartId.Text = ConfigurationManager.MainConfig.patch_mariokartId;
			txt_customName.Text = ConfigurationManager.MainConfig.patch_customName;
			radio_networkModeAuto.Checked = ConfigurationManager.MainConfig.patch_networkAuto;
			if (!ConfigurationManager.MainConfig.patch_networkAuto)
			{
				radio_networkModeManual.Checked = true;
				txt_networkIP.Text = ConfigurationManager.MainConfig.patch_networkIP;
				txt_networkGateway.Text = ConfigurationManager.MainConfig.patch_networkGateway;
				txt_BroadcastAddress.Text = ConfigurationManager.MainConfig.patch_BroadcastAddress;
				txt_networkDns1.Text = ConfigurationManager.MainConfig.patch_networkDns1;
				txt_networkDns2.Text = ConfigurationManager.MainConfig.patch_networkDns2;
				txt_networkMask.Text = ConfigurationManager.MainConfig.patch_networkMask;
			}




			chk_useXenosInjector.Checked = ConfigurationManager.MainConfig.useXenos;

			txt_magpieExe.Text = ConfigurationManager.MainConfig.magpieExe;
			num_magpieDelay.Value = ConfigurationManager.MainConfig.magpieDelay;
			cmb_magpieScaling.SelectedIndex = ConfigurationManager.MainConfig.magpieScaling;
			cmb_magpieCapture.SelectedIndex = ConfigurationManager.MainConfig.magpieCapture;
			chk_useMagpie.Checked = ConfigurationManager.MainConfig.useMagpie;
			chk_magpieShowFps.Checked = ConfigurationManager.MainConfig.magpieShowFps;
			chk_magpieVsync.Checked = ConfigurationManager.MainConfig.magpieVsync;
			chk_magpieTripleBuffering.Checked = ConfigurationManager.MainConfig.magpieTripleBuffering;

			//cmb_useMagpieLightgun.SelectedIndex = ConfigurationManager.MainConfig.magpieLightgun;
			//cmb_MagpieLightgunCalibration.SelectedIndex = ConfigurationManager.MainConfig.magpieLightgunCalibration;

			chk_magpieSindenBorder.Checked = ConfigurationManager.MainConfig.magpieSinden;
			chk_magpieGunCalibration.Checked = ConfigurationManager.MainConfig.magpieGunCalibration;

			num_magpieBorderSize.Value = (decimal)ConfigurationManager.MainConfig.magpieBorderSize;
			chk_magpieExclusiveFullscreen.Checked = ConfigurationManager.MainConfig.magpieExclusiveFullscreen;
			trk_magpieFsrSharp.Value = ConfigurationManager.MainConfig.magpieFsrSharp;
			lbl_magpieFsrSharp.Text = trk_magpieFsrSharp.Value.ToString() + "%";


			cmb_showStartup.SelectedIndex = ConfigurationManager.MainConfig.TPConsoleAction;

			chk_FFB.Checked = ConfigurationManager.MainConfig.FFB;
			chk_showStartup.Checked = ConfigurationManager.MainConfig.showStartup;
			chk_enableVirtualKeyboard.Checked = ConfigurationManager.MainConfig.virtualKeyboard;
			txt_KeyTest.Text = ConfigurationManager.MainConfig.keyTest;
			txt_KeyService1.Text = ConfigurationManager.MainConfig.keyService1;
			txt_KeyService2.Text = ConfigurationManager.MainConfig.keyService2;

			txt_wheelXinputData.Text = ConfigurationManager.MainConfig.wheelXinputData;
			txt_arcadeXinputData.Text = ConfigurationManager.MainConfig.arcadeXinputData;
			txt_gamepadXinputData.Text = ConfigurationManager.MainConfig.gamepadXinputData;


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

			cmb_vjoy.SelectedIndex = ConfigurationManager.MainConfig.indexvjoy;
			vJoyManager vJoyObj;
			vJoyObj = new vJoyManager();
			if (!vJoyObj.vJoyEnabled())
			{
				btn_vjoyconfig.Enabled = false;
				cmb_vjoy.Enabled = false;
			}

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

			txt_rivatunersoft.Text = ConfigurationManager.MainConfig.rivatunerExe;
			txt_demulshootersoft.Text = ConfigurationManager.MainConfig.demulshooterExe;
			txt_sindensoft.Text = ConfigurationManager.MainConfig.sindenExe;
			txt_mamehookersoft.Text = ConfigurationManager.MainConfig.mamehookerExe;

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

			updateStooz();

			if (!chk_enableVirtualKeyboard.Checked)
			{
				btn_ClearService1.Enabled = false;
				btn_ClearService2.Enabled = false;
				btn_ClearTest.Enabled = false;
				btn_setService1.Enabled = false;
				btn_setService2.Enabled = false;
				btn_setTest.Enabled = false;
			}


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
				if (cmb_gunA_type.SelectedItem.ToString() == "wiimote") cmb_gunA_recoil.Items.Add("rumble");
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
				if (cmb_gunB_type.SelectedItem.ToString() == "wiimote") cmb_gunB_recoil.Items.Add("rumble");
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
			//radio_gunA_sindenPump1.Enabled = false;
			//radio_gunA_sindenPump2.Enabled = false;
			//radio_gunA_sindenPump3.Enabled = false;
			//radio_gunB_sindenPump1.Enabled = false;
			//radio_gunB_sindenPump2.Enabled = false;
			//radio_gunB_sindenPump3.Enabled = false;
			if (cmb_gunA_type.SelectedItem != null && cmb_gunA_type.SelectedItem.ToString() == "sinden")
			{
				//radio_gunA_sindenPump1.Enabled = true;
				//radio_gunA_sindenPump2.Enabled = true;
				//radio_gunA_sindenPump3.Enabled = true;
				grp_gunA_sindenOptions.Enabled = true;
			}
			if (cmb_gunB_type.SelectedItem != null && cmb_gunB_type.SelectedItem.ToString() == "sinden")
			{
				//radio_gunB_sindenPump1.Enabled = true;
				//radio_gunB_sindenPump2.Enabled = true;
				//radio_gunB_sindenPump3.Enabled = true;
				grp_gunB_sindenOptions.Enabled = true;
			}

			grp_gunA_gun4irOptions.Enabled = false;
			grp_gunB_gun4irOptions.Enabled = false;
			//chk_gunA_AutoJoy.Enabled = false;
			//chk_gunB_AutoJoy.Enabled = false;
			//cmb_gunA_com.Enabled = false;
			//cmb_gunB_com.Enabled = false;
			if (cmb_gunA_recoil.SelectedItem != null && cmb_gunA_recoil.SelectedItem.ToString() == "gun4ir")
			{
				//cmb_gunA_com.Enabled = true;
				//chk_gunA_AutoJoy.Enabled = true;
				grp_gunA_gun4irOptions.Enabled = true;
			}
			if (cmb_gunB_recoil.SelectedItem != null && cmb_gunB_recoil.SelectedItem.ToString() == "gun4ir")
			{
				//chk_gunB_AutoJoy.Enabled = true;
				//cmb_gunB_com.Enabled = true;
				grp_gunB_gun4irOptions.Enabled = true;
			}
			DoRedoGunRecoilCombo = false;
		}

		private void chk_enableVirtualKeyboard_CheckedChanged(object sender, EventArgs e)
		{
			if (chk_enableVirtualKeyboard.Checked)
			{
				try
				{
					var client = new ViGEmClient();
					client.Dispose();
				}
				catch (VigemBusNotFoundException ex)
				{
					chk_enableVirtualKeyboard.Checked = false;
					MessageBox.Show("ViGEm bus not found, please make sure ViGEm is correctly installed.");
				}
			}


			btn_ClearService1.Enabled = chk_enableVirtualKeyboard.Checked;
			btn_ClearService2.Enabled = chk_enableVirtualKeyboard.Checked;
			btn_ClearTest.Enabled = chk_enableVirtualKeyboard.Checked;
			btn_setService1.Enabled = chk_enableVirtualKeyboard.Checked;
			btn_setService2.Enabled = chk_enableVirtualKeyboard.Checked;
			btn_setTest.Enabled = chk_enableVirtualKeyboard.Checked;
			ConfigurationManager.MainConfig.virtualKeyboard = chk_enableVirtualKeyboard.Checked;
			ConfigurationManager.SaveConfig();
		}
		public void DrawKeyDisplay(string TextBoxName)
		{

			if (TextBoxName == "txt_KeyTest") txt_KeyTest.Text = PickKeyCombo.GetKeyCombo(this.Keys, true);
			if (TextBoxName == "txt_KeyService1") txt_KeyService1.Text = PickKeyCombo.GetKeyCombo(this.Keys, true);
			if (TextBoxName == "txt_KeyService2") txt_KeyService2.Text = PickKeyCombo.GetKeyCombo(this.Keys, true);

			ConfigurationManager.MainConfig.keyTest = txt_KeyTest.Text;
			ConfigurationManager.MainConfig.keyService1 = txt_KeyService1.Text;
			ConfigurationManager.MainConfig.keyService2 = txt_KeyService2.Text;
			ConfigurationManager.SaveConfig();

		}

		private void btn_setTest_Click(object sender, EventArgs e)
		{
			foreach (Control c in this.Controls)
			{
				c.Enabled = false;
			}
			txt_KeyTest.Text = "Press up to three keys...";
			KeyPicker.StartPicking("txt_KeyTest");
			Focus();
			txt_KeyTest.Focus();
			txt_KeyTest.Select(txt_KeyTest.Text.Length, 0);
		}

		private void btn_setService1_Click(object sender, EventArgs e)
		{
			foreach (Control c in this.Controls)
			{
				c.Enabled = false;
			}
			txt_KeyService1.Text = "Press up to three keys...";
			KeyPicker.StartPicking("txt_KeyService1");
			Focus();
			txt_KeyService1.Focus();
			txt_KeyService1.Select(txt_KeyService1.Text.Length, 0);
		}

		private void btn_setService2_Click(object sender, EventArgs e)
		{
			foreach (Control c in this.Controls)
			{
				c.Enabled = false;
			}
			txt_KeyService2.Text = "Press up to three keys...";
			KeyPicker.StartPicking("txt_KeyService2");
			Focus();
			txt_KeyService2.Focus();
			txt_KeyService2.Select(txt_KeyService2.Text.Length, 0);
		}

		private void btn_ClearTest_Click(object sender, EventArgs e)
		{
			txt_KeyTest.Text = "";
			ConfigurationManager.MainConfig.keyTest = txt_KeyTest.Text;
			ConfigurationManager.SaveConfig();
		}

		private void btn_ClearService1_Click(object sender, EventArgs e)
		{
			txt_KeyService1.Text = "";
			ConfigurationManager.MainConfig.keyService1 = txt_KeyService1.Text;
			ConfigurationManager.SaveConfig();
		}

		private void btn_ClearService2_Click(object sender, EventArgs e)
		{
			txt_KeyService2.Text = "";
			ConfigurationManager.MainConfig.keyService2 = txt_KeyService2.Text;
			ConfigurationManager.SaveConfig();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var frm = new Startup();
			var result = frm.ShowDialog();

		}

		private void chk_showStartup_CheckedChanged(object sender, EventArgs e)
		{
			ConfigurationManager.MainConfig.showStartup = chk_showStartup.Checked;
			ConfigurationManager.SaveConfig();
		}

		private void chk_FFB_CheckedChanged(object sender, EventArgs e)
		{
			ConfigurationManager.MainConfig.FFB = chk_FFB.Checked;
			ConfigurationManager.SaveConfig();
		}

		private void btn_testxinput_Click(object sender, EventArgs e)
		{
			txt_xinputdata.Text = HIDInfo.GetXINPUT(true);
		}

		private void kryptonLabel4_Click(object sender, EventArgs e)
		{

		}

		private void kryptonButton1_Click(object sender, EventArgs e)
		{
			txt_wheelXinputData.Text = "Type=Wheel";
			txt_arcadeXinputData.Text = "Type=ArcadeStick,Type=ArcadePad";
			txt_gamepadXinputData.Text = "Type=Gamepad";
			ConfigurationManager.MainConfig.wheelXinputData = txt_wheelXinputData.Text;
			ConfigurationManager.MainConfig.arcadeXinputData = txt_arcadeXinputData.Text;
			ConfigurationManager.MainConfig.gamepadXinputData = txt_gamepadXinputData.Text;

		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{

			ConfigurationManager.MainConfig.tpLicence = Utils.Encrypt(txt_tplicence.Text);
			ConfigurationManager.MainConfig.tpLicenceRegOnLaunch = chk_tplicence_onstart.Checked;
			ConfigurationManager.MainConfig.tpLicenceUnRegAfterStart = chk_tplicence_unreg_onlaunch.Checked;


			ConfigurationManager.MainConfig.gpuType = cmb_gpu.SelectedIndex;
			ConfigurationManager.MainConfig.patchGpuFix = chk_patchGpuFix.Checked;
			ConfigurationManager.MainConfig.patchGpuTP = chk_patchGpuTP.Checked;
			ConfigurationManager.MainConfig.gpuResolution = cmb_resolution.SelectedIndex;
			ConfigurationManager.MainConfig.patchResolutionFix = chk_patchResolutionFix.Checked;
			ConfigurationManager.MainConfig.patchResolutionTP = chk_patchResolutionTP.Checked;
			ConfigurationManager.MainConfig.displayMode = cmb_displayMode.SelectedIndex;
			ConfigurationManager.MainConfig.patchDisplayModeFix = chk_patchDisplayModeFix.Checked;
			ConfigurationManager.MainConfig.patchDisplayModeTP = chk_patchDisplayModeTP.Checked;
			ConfigurationManager.MainConfig.patch_FFB = chk_patchFFB.Checked;
			ConfigurationManager.MainConfig.patchReshade = chk_patchReshade.Checked;
			ConfigurationManager.MainConfig.patchGameID = chk_patchGameID.Checked;
			ConfigurationManager.MainConfig.patchNetwork = chk_patchNetwork.Checked;
			ConfigurationManager.MainConfig.patchOtherTPSettings = chk_patchOthersTPSettings.Checked;
			ConfigurationManager.MainConfig.patchOthersGameOptions = chk_patchOthersGameOptions.Checked;

			ConfigurationManager.MainConfig.patch_apm3id = txt_apm3id.Text;
			ConfigurationManager.MainConfig.patch_mariokartId = txt_mariokartId.Text;
			ConfigurationManager.MainConfig.patch_customName = txt_customName.Text;
			if (radio_networkModeAuto.Checked)
			{
				ConfigurationManager.MainConfig.patch_networkAuto = true;
			}
			else
			{
				ConfigurationManager.MainConfig.patch_networkAuto = false;
				ConfigurationManager.MainConfig.patch_networkIP = txt_networkIP.Text;
				ConfigurationManager.MainConfig.patch_networkGateway = txt_networkGateway.Text;
				ConfigurationManager.MainConfig.patch_BroadcastAddress = txt_BroadcastAddress.Text;
				ConfigurationManager.MainConfig.patch_networkDns1 = txt_networkDns1.Text;
				ConfigurationManager.MainConfig.patch_networkDns2 = txt_networkDns2.Text;
				ConfigurationManager.MainConfig.patch_networkMask = txt_networkMask.Text;
			}


			ConfigurationManager.MainConfig.magpieExe = txt_magpieExe.Text;
			ConfigurationManager.MainConfig.magpieDelay = (int)num_magpieDelay.Value;
			ConfigurationManager.MainConfig.magpieScaling = cmb_magpieScaling.SelectedIndex;
			ConfigurationManager.MainConfig.magpieCapture = cmb_magpieCapture.SelectedIndex;
			ConfigurationManager.MainConfig.useMagpie = chk_useMagpie.Checked;
			ConfigurationManager.MainConfig.magpieShowFps = chk_magpieShowFps.Checked;
			ConfigurationManager.MainConfig.magpieVsync = chk_magpieVsync.Checked;
			ConfigurationManager.MainConfig.magpieTripleBuffering = chk_magpieTripleBuffering.Checked;

			//ConfigurationManager.MainConfig.magpieLightgun = cmb_useMagpieLightgun.SelectedIndex;
			//ConfigurationManager.MainConfig.magpieLightgunCalibration = cmb_MagpieLightgunCalibration.SelectedIndex;

			ConfigurationManager.MainConfig.magpieSinden = chk_magpieSindenBorder.Checked;
			ConfigurationManager.MainConfig.magpieGunCalibration = chk_magpieGunCalibration.Checked;

			ConfigurationManager.MainConfig.magpieBorderSize = (double)num_magpieBorderSize.Value;

			ConfigurationManager.MainConfig.magpieExclusiveFullscreen = chk_magpieExclusiveFullscreen.Checked;
			ConfigurationManager.MainConfig.magpieFsrSharp = trk_magpieFsrSharp.Value;

			ConfigurationManager.MainConfig.wheelXinputData = txt_wheelXinputData.Text;
			ConfigurationManager.MainConfig.arcadeXinputData = txt_arcadeXinputData.Text;
			ConfigurationManager.MainConfig.gamepadXinputData = txt_gamepadXinputData.Text;

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
				if (recoilValue == "gun4ir")
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
				if (recoilValue == "gun4ir")
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

			ConfigurationManager.MainConfig.reasignPedals = chk_reasignGunPedal.Checked;
			ConfigurationManager.MainConfig.reversePedals = chk_reversePedal.Checked;
			if (!chk_reasignGunPedal.Checked) ConfigurationManager.MainConfig.reversePedals = false;


			ConfigurationManager.MainConfig.gunASidenPump = 1;
			if (ConfigurationManager.MainConfig.gunAType == "sinden")
			{
				if (radio_gunA_sindenPump1.Checked) ConfigurationManager.MainConfig.gunASidenPump = 1;
				if (radio_gunA_sindenPump2.Checked) ConfigurationManager.MainConfig.gunASidenPump = 2;
				if (radio_gunA_sindenPump3.Checked) ConfigurationManager.MainConfig.gunASidenPump = 3;
			}
			ConfigurationManager.MainConfig.gunBSidenPump = 1;
			if (ConfigurationManager.MainConfig.gunBType == "sinden")
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

		private void chk_useDinputWheel_CheckedChanged(object sender, EventArgs e)
		{
			ConfigurationManager.MainConfig.useDinputWheel = chk_useDinputWheel.Checked;
			ConfigurationManager.SaveConfig();
		}

		private void btn_setffbguid_Click(object sender, EventArgs e)
		{
			if (cmb_ffbguid.SelectedIndex >= 0)
			{
				txt_ffbguid.Text = FFBGuidList[cmb_ffbguid.SelectedIndex];
			}
		}

		private void chk_favorAB_CheckedChanged(object sender, EventArgs e)
		{
			ConfigurationManager.MainConfig.favorAB = chk_favorAB.Checked;
			ConfigurationManager.SaveConfig();
		}


		private void btn_selectTP_Click(object sender, EventArgs e)
		{
			using (var fbd = new FolderBrowserDialog())
			{
				DialogResult result = fbd.ShowDialog();

				if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
				{
					txt_tpfolder.Text = fbd.SelectedPath;
					ConfigurationManager.MainConfig.TpFolder = fbd.SelectedPath;
					ConfigurationManager.SaveConfig();
				}
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

		private void chk_enableDebug_CheckedChanged(object sender, EventArgs e)
		{
			ConfigurationManager.MainConfig.debugMode = chk_enableDebug.Checked;
			ConfigurationManager.SaveConfig();
		}

		private void kryptonButton3_Click(object sender, EventArgs e)
		{

		}

		private void btn_resetdefaultlinksource_Click(object sender, EventArgs e)
		{
			txt_linksourcefolder.Text = @"Default (<YourTeknoparrotFolder>\AutoXinputLinks)";
			ConfigurationManager.MainConfig.perGameLinkFolder = txt_linksourcefolder.Text;
			ConfigurationManager.SaveConfig();
		}

		private void btn_selectLinkFolder_Click(object sender, EventArgs e)
		{
			MessageBox.Show("You have to make sure that the Link Folder use the same drive as Teknoparrot (That's why, by default, it use a Subfolder of Teknoparrot)");
			using (var fbd = new FolderBrowserDialog())
			{
				DialogResult result = fbd.ShowDialog();

				if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
				{
					txt_linksourcefolder.Text = fbd.SelectedPath;
					ConfigurationManager.MainConfig.perGameLinkFolder = fbd.SelectedPath;
					ConfigurationManager.SaveConfig();
				}
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

		private void chk_useDinputShifter_CheckedChanged(object sender, EventArgs e)
		{
			ConfigurationManager.MainConfig.useDinputShifter = chk_useDinputShifter.Checked;
			ConfigurationManager.SaveConfig();
		}

		private void chk_useDinputHotas_CheckedChanged(object sender, EventArgs e)
		{
			ConfigurationManager.MainConfig.useDinputHotas = chk_useDinputHotas.Checked;
			ConfigurationManager.SaveConfig();
		}

		private void kryptonLabel15_Click(object sender, EventArgs e)
		{

		}

		private void kryptonLabel18_Click(object sender, EventArgs e)
		{

		}

		private void btn_selectLinkFolderExe_Click(object sender, EventArgs e)
		{
			MessageBox.Show("You must select a directory that use the same Drive as your game folder.");
			using (var fbd = new FolderBrowserDialog())
			{
				DialogResult result = fbd.ShowDialog();

				if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
				{
					txt_linksourcefolderexe.Text = fbd.SelectedPath;
					ConfigurationManager.MainConfig.perGameLinkFolderExe = fbd.SelectedPath;
					ConfigurationManager.SaveConfig();
				}
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

		private void chk_reverseYAxis_Hotas_CheckedChanged(object sender, EventArgs e)
		{
			//ConfigurationManager.MainConfig.reverseYAxis_Hotas = chk_reverseYAxis_Hotas.Checked;
			//ConfigurationManager.SaveConfig();
		}

		private void cmb_ffbguid_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void btn_setffbguidHotas_Click(object sender, EventArgs e)
		{
			if (cmb_ffbguidHotas.SelectedIndex >= 0)
			{
				txt_ffbguidHotas.Text = FFBGuidList[cmb_ffbguidHotas.SelectedIndex];
			}
		}

		private void groupBox1_Enter(object sender, EventArgs e)
		{

		}

		private void chk_useHotasWithWheel_CheckedChanged(object sender, EventArgs e)
		{
			ConfigurationManager.MainConfig.useHotasWithWheel = chk_useHotasWithWheel.Checked;
			ConfigurationManager.SaveConfig();
		}

		private void kryptonGroupBox1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void btn_gunA_configure_Click(object sender, EventArgs e)
		{
			var frm = new dinputgun(1, cmb_gunA_type.SelectedItem.ToString());
			var result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{

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

		private void btn_vjoyconfig_Click(object sender, EventArgs e)
		{
			// Recherche de la fenêtre à fermer parmi les fenêtres ouvertes
			foreach (Form form in Application.OpenForms)
			{
				if (form.GetType() == typeof(VjoyControl))
				{
					// La fenêtre Fenetre1 est ouverte, fermez-la
					form.Close();
					Thread.Sleep(100);
					break; // Sortir de la boucle une fois que la fenêtre est trouvée et fermée
				}
			}

			bool vjoy_gunA = chk_gunA_Vjoy.Checked;
			bool vjoy_gunB = chk_gunB_Vjoy.Checked;

			var frm = new VjoyControl(true, "", null, vjoy_gunA, vjoy_gunB);
			var result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{
				if (!string.IsNullOrEmpty(frm.GunA_json)) ConfigurationManager.MainConfig.vjoySettingsGunA = frm.GunA_json;
				if (!string.IsNullOrEmpty(frm.GunB_json)) ConfigurationManager.MainConfig.vjoySettingsGunB = frm.GunB_json;
				ConfigurationManager.SaveConfig();
			}

		}

		private void cmb_vjoy_SelectedIndexChanged(object sender, EventArgs e)
		{
			ConfigurationManager.MainConfig.indexvjoy = cmb_vjoy.SelectedIndex;
			ConfigurationManager.SaveConfig();
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

		private void groupBox9_Enter(object sender, EventArgs e)
		{

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

		private void btn_demulshooter_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "DemulShooter executable (DemulShooter.exe)|DemulShooter.exe"; // Filtre pour n'afficher que RTSS.exe
			openFileDialog.Title = "Select DemulShooter.exe"; // Titre de la boîte de dialogue

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				txt_demulshootersoft.Text = Path.GetFullPath(openFileDialog.FileName); // Stocke le chemin du fichier sélectionné
				ConfigurationManager.MainConfig.demulshooterExe = Path.GetFullPath(openFileDialog.FileName);
				ConfigurationManager.SaveConfig();
			}
		}

		private void chk_gunA_Crosshair_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void kryptonButton4_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Lightgun executable (Lightgun.exe)|Lightgun.exe"; // Filtre pour n'afficher que RTSS.exe
			openFileDialog.Title = "Select Lightgun.exe"; // Titre de la boîte de dialogue

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				txt_sindensoft.Text = Path.GetFullPath(openFileDialog.FileName); // Stocke le chemin du fichier sélectionné
				ConfigurationManager.MainConfig.sindenExe = Path.GetFullPath(openFileDialog.FileName);
				ConfigurationManager.SaveConfig();
			}
		}

		private void chk_reversePedal_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void chk_gunA_AutoJoy_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void btn_rivatuner_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "RTSS exécutable (RTSS.exe)|RTSS.exe"; // Filtre pour n'afficher que RTSS.exe
			openFileDialog.Title = "Sélectionnez le fichier RTSS.exe"; // Titre de la boîte de dialogue

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				txt_rivatunersoft.Text = Path.GetFullPath(openFileDialog.FileName); // Stocke le chemin du fichier sélectionné
				ConfigurationManager.MainConfig.rivatunerExe = Path.GetFullPath(openFileDialog.FileName);
				ConfigurationManager.SaveConfig();
			}
		}

		private void btn_mamehooker_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "mamehooker executable (mamehook.exe)|mamehook.exe"; // Filtre pour n'afficher que RTSS.exe
			openFileDialog.Title = "Select mamehook.exe"; // Titre de la boîte de dialogue

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				txt_mamehookersoft.Text = Path.GetFullPath(openFileDialog.FileName); // Stocke le chemin du fichier sélectionné
				ConfigurationManager.MainConfig.mamehookerExe = Path.GetFullPath(openFileDialog.FileName);
				ConfigurationManager.SaveConfig();
			}
		}

		private void btn_runSinden_Click(object sender, EventArgs e)
		{
			if (File.Exists(txt_sindensoft.Text))
			{
				Process siden_process = new Process();
				siden_process.StartInfo.FileName = txt_sindensoft.Text;
				siden_process.StartInfo.WorkingDirectory = Path.GetDirectoryName(txt_sindensoft.Text);
				siden_process.StartInfo.Arguments = txt_sindenextra.Text;
				siden_process.StartInfo.UseShellExecute = true;
				siden_process.Start();

			}

		}

		private void groupBox13_Enter(object sender, EventArgs e)
		{

		}

		private void cmb_showStartup_SelectedIndexChanged(object sender, EventArgs e)
		{
			ConfigurationManager.MainConfig.TPConsoleAction = cmb_showStartup.SelectedIndex;
			ConfigurationManager.SaveConfig();
		}

		private void btn_magpieExe_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Magpie (Magpie.exe)|Magpie.exe"; // Filtre pour n'afficher que RTSS.exe
			openFileDialog.Title = "Select Magpie.exe"; // Titre de la boîte de dialogue

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				txt_magpieExe.Text = Path.GetFullPath(openFileDialog.FileName); // Stocke le chemin du fichier sélectionné
				ConfigurationManager.MainConfig.magpieExe = Path.GetFullPath(openFileDialog.FileName);
				ConfigurationManager.SaveConfig();
			}
		}

		private void btn_magpieSindenExe_Click(object sender, EventArgs e)
		{

		}

		private void kryptonLabel19_Click(object sender, EventArgs e)
		{

		}

		private void chk_useXenosInjector_CheckedChanged(object sender, EventArgs e)
		{
			if (chk_useXenosInjector.Checked)
			{
				bool valid = true;
				string Xenos7z = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "xenos", "Xenos.7z");
				string XenosPath = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "xenos", "Xenos.exe");
				if (!File.Exists(XenosPath))
				{
					valid = false;
					if (File.Exists(Xenos7z))
					{
						AddXenos addXenos = new AddXenos();
						DialogResult result = addXenos.ShowDialog();
						if (result == DialogResult.OK)
						{
							MessageBox.Show("Xenos Activated, you must configure the injection per game on the game options");
							valid = true;
						}
					}
				}
				if (valid)
				{
					ConfigurationManager.MainConfig.useXenos = chk_useXenosInjector.Checked;
					ConfigurationManager.SaveConfig();
				}
				else
				{
					chk_useXenosInjector.Checked = false;
				}
			}
			else
			{
				ConfigurationManager.MainConfig.useXenos = chk_useXenosInjector.Checked;
				ConfigurationManager.SaveConfig();
			}
		}

		private void kryptonComboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void groupBox4_Enter(object sender, EventArgs e)
		{

		}

		private void cmb_gpu_SelectedIndexChanged(object sender, EventArgs e)
		{
			ConfigurationManager.MainConfig.gpuType = cmb_gpu.SelectedIndex;
			ConfigurationManager.SaveConfig();
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			lbl_magpieFsrSharp.Text = trk_magpieFsrSharp.Value.ToString() + "%";
		}

		private void tabGlobal_Click(object sender, EventArgs e)
		{

		}

		private void chk_magpieReshadeAdaptiveSharpen_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void kryptonButton2_Click(object sender, EventArgs e)
		{
			/*
			NetworkInterface[] networks = NetworkInterface.GetAllNetworkInterfaces();

			var activeAdapter = networks.First(x => x.NetworkInterfaceType != NetworkInterfaceType.Loopback
								&& x.NetworkInterfaceType != NetworkInterfaceType.Tunnel
								&& x.OperationalStatus == OperationalStatus.Up
								&& x.Name.StartsWith("vEthernet") == false);

			var xxx = activeAdapter.GetIPProperties();

			if(xxx != null)
			{
				txt_networkGateway.Text = xxx.GatewayAddresses.First().Address.ToString();


			}
			*/

			/*
			NetworkInterface activeAdapter = null;
			Task.Run(async () =>
			{
				NetworkInterface primaryAdapter = await GetPrimaryNetworkAdapterAsync();
				if (primaryAdapter != null)
				{
					activeAdapter = primaryAdapter;
				}
				else
				{
					Console.WriteLine("No primary network adapter found or it is not connected to the internet.");
				}
			}).GetAwaiter().GetResult();

			var ipProperties = activeAdapter.GetIPProperties();
			var unicastAddress = ipProperties.UnicastAddresses.FirstOrDefault(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

			if (unicastAddress != null)
			{
				var ipAddress = unicastAddress.Address;
				var subnetMask = unicastAddress.IPv4Mask;
				var gatewayAddress = ipProperties.GatewayAddresses.FirstOrDefault()?.Address;
				var dnsAddresses = ipProperties.DnsAddresses.Where(dns => dns.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToArray();

				var broadcastAddress = GetBroadcastAddress(ipAddress, subnetMask);

				txt_networkGateway.Text = gatewayAddress?.ToString();
				txt_networkIP.Text = ipAddress.ToString();
				txt_networkMask.Text = subnetMask.ToString();
				txt_BroadcastAddress.Text = broadcastAddress.ToString();
				if (dnsAddresses.Length > 0)
				{
					txt_networkDns1.Text = dnsAddresses[0].ToString();
				}
				if (dnsAddresses.Length > 1)
				{
					txt_networkDns2.Text = dnsAddresses[1].ToString();
				}
			}
			*/

		}

		private void radio_networkModeAuto_CheckedChanged(object sender, EventArgs e)
		{
			txt_networkIP.Enabled = txt_networkMask.Enabled = txt_networkGateway.Enabled = txt_networkDns1.Enabled = txt_networkDns2.Enabled = txt_BroadcastAddress.Enabled = false;

			var ThreadNetWork = new Thread(() =>
			{
				var networkInfo = Utils.GetFirstNetworkAdapterInfo();
				txt_networkIP.Text = networkInfo.ContainsKey("networkIP") ? networkInfo["networkIP"] : "0.0.0.0";
				txt_networkMask.Text = networkInfo.ContainsKey("networkMask") ? networkInfo["networkMask"] : "0.0.0.0";
				txt_networkGateway.Text = networkInfo.ContainsKey("networkGateway") ? networkInfo["networkGateway"] : "0.0.0.0";
				txt_networkDns1.Text = networkInfo.ContainsKey("networkDns1") ? networkInfo["networkDns1"] : "0.0.0.0";
				txt_networkDns2.Text = networkInfo.ContainsKey("networkDns2") ? networkInfo["networkDns2"] : "0.0.0.0";
				txt_BroadcastAddress.Text = networkInfo.ContainsKey("BroadcastAddress") ? networkInfo["BroadcastAddress"] : "0.0.0.0";
			});
			ThreadNetWork.Start();


		}

		private void radio_networkModeManual_CheckedChanged(object sender, EventArgs e)
		{
			txt_networkIP.Enabled = txt_networkMask.Enabled = txt_networkGateway.Enabled = txt_networkDns1.Enabled = txt_networkDns2.Enabled = txt_BroadcastAddress.Enabled = true;
			txt_networkIP.Text = ConfigurationManager.MainConfig.patch_networkIP == "" ? txt_networkIP.Text : ConfigurationManager.MainConfig.patch_networkIP;
			txt_networkGateway.Text = ConfigurationManager.MainConfig.patch_networkGateway == "" ? txt_networkGateway.Text : ConfigurationManager.MainConfig.patch_networkGateway;
			txt_BroadcastAddress.Text = ConfigurationManager.MainConfig.patch_BroadcastAddress == "" ? txt_BroadcastAddress.Text : ConfigurationManager.MainConfig.patch_BroadcastAddress;
			txt_networkDns1.Text = ConfigurationManager.MainConfig.patch_networkDns1 == "" ? txt_networkDns1.Text : ConfigurationManager.MainConfig.patch_networkDns1;
			txt_networkDns2.Text = ConfigurationManager.MainConfig.patch_networkDns2 == "" ? txt_networkDns2.Text : ConfigurationManager.MainConfig.patch_networkDns2;
			txt_networkMask.Text = ConfigurationManager.MainConfig.patch_networkMask == "" ? txt_networkMask.Text : ConfigurationManager.MainConfig.patch_networkMask;

		}

		private void kryptonLabel60_Click(object sender, EventArgs e)
		{

		}

		private void tabPatch_Click(object sender, EventArgs e)
		{

		}

		private void txt_networkMask_TextChanged(object sender, EventArgs e)
		{

		}

		private void cmb_gpu_SelectedIndexChanged_1(object sender, EventArgs e)
		{

		}

		private void cmb_reverseYAxis_Hotas_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void cmb_patchGpuTP_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void kryptonCheckBox1_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void chk_patchOthersGameOptions_CheckedChanged(object sender, EventArgs e)
		{

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

		private void kryptonCheckBox1_CheckedChanged_1(object sender, EventArgs e)
		{

		}

		private void groupBox17_Enter(object sender, EventArgs e)
		{

		}

		private void chk_tplicence_unreg_onexit_CheckedChanged(object sender, EventArgs e)
		{
			chk_tplicence_unreg_onlaunch.Checked = !chk_tplicence_unreg_onexit.Checked;
		}

		private void chk_tplicence_unreg_onlaunch_CheckedChanged(object sender, EventArgs e)
		{
			chk_tplicence_unreg_onexit.Checked = !chk_tplicence_unreg_onlaunch.Checked;
		}
	}
}