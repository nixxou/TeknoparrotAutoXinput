using Krypton.Toolkit;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Exceptions;
using SDL2;
using SharpDX.DirectInput;
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
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			this.KeyPicker = new PickKeyCombo(this);
			//System.Diagnostics.Debugger.Break();

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
			ConfigurationManager.MainConfig.reverseYAxis_Hotas = chk_reverseYAxis_Hotas.Checked;
			ConfigurationManager.SaveConfig();
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

			var frm = new VjoyControl(true);
			var result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{
				if(!string.IsNullOrEmpty(frm.GunA_json)) ConfigurationManager.MainConfig.vjoySettingsGunA = frm.GunA_json;
				if(!string.IsNullOrEmpty(frm.GunB_json)) ConfigurationManager.MainConfig.vjoySettingsGunB = frm.GunB_json;
				ConfigurationManager.SaveConfig();
			}

		}

		private void cmb_vjoy_SelectedIndexChanged(object sender, EventArgs e)
		{
			ConfigurationManager.MainConfig.indexvjoy = cmb_vjoy.SelectedIndex;
			ConfigurationManager.SaveConfig();
		}
	}
}