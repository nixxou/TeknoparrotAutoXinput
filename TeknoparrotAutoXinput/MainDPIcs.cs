using BrightIdeasSoftware;
using Krypton.Toolkit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SevenZip;
using SharpDX.DirectInput;
using SharpDX.Multimedia;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TeknoParrotUi.Common;
using WiimoteLib;
using XInput.Wrapper;
using Image = System.Drawing.Image;


namespace TeknoparrotAutoXinput
{
	public partial class MainDPIcs : KryptonForm
	{
		public string filter_text = "";
		public bool filter_arcade = false;
		public bool filter_wheel = false;
		public bool filter_hotas = false;
		public bool filter_lightgun = false;

		private Dictionary<string, Game> _gameList = new Dictionary<string, Game>();
		private string _tpFolder = "";
		private string _userProfileFolder = "";
		private string _basePath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);

		private Dictionary<int, XinputGamepad> _connectedGamePad = new Dictionary<int, XinputGamepad>();
		private bool _dinputWheelFound = false;
		private string _dinputWheelName = "";
		private bool _haveWheel = false;

		private bool _haveGamepad = false;
		private bool _haveArcade = false;

		private bool _dinputHotasFound = false;
		private string _dinputHotasName = "";
		private bool _haveHotas = false;

		private bool _dinputLightgunAFound = false;
		private bool _dinputLightgunBFound = false;
		private bool _haveLightgun = false;
		private string _dinputGunAName = "";
		private string _dinputGunBName = "";
		private string _dinputGunAType = "";
		private string _dinputGunBType = "";

		private Game _selectedGame = null;
		private Dictionary<string, string> _selectedGameInfo = new Dictionary<string, string>();
		private GameSettings _selectedGameSettings = null;
		private Dictionary<string, string> _selectedGameOption = new Dictionary<string, string>();

		public static bool IsReloading = false;

		private bool _isPlaying = false;
		public bool isPlaying
		{
			get
			{
				return _isPlaying;
			}
			set
			{

				_isPlaying = value;
				if (_isPlaying)
				{
					btn_playgame.Enabled = false;
					//btn_playgamedirect.Enabled = false;
					//btn_playgamedirect2.Enabled = false;
				}
				else
				{
					if (_playAutoEnabled) btn_playgame.Enabled = true;
					//if (_playDirectEnabled) btn_playgamedirect.Enabled = true;
					//if (_playDirectEnabled) btn_playgamedirect2.Enabled = true;
				}

			}
		}


		private bool _playDirectEnabled = false;
		private bool _playAutoEnabled = false;

		List<string> typeConfig = new List<string>();

		public MainDPIcs()
		{
			float dpiVal = CreateGraphics().DpiX;
			if (dpiVal > 120)
			{
				Font = new Font(Font.Name, 8.25f * 0.85f, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);
			}
			InitializeComponent();
			var fontComboBox = cmb_displayMode.Font;
			fontComboBox = new Font(fontComboBox.Name, 9.5f, fontComboBox.Style, fontComboBox.Unit, fontComboBox.GdiCharSet, fontComboBox.GdiVerticalFont);

			if (dpiVal > 120)
			{
				lbl_perf_desc.Font = lbl_perf_high.Font = lbl_perf_low.Font = lbl_perf_normal.Font = lbl_overrideTpController.Font = lbl_overrideTpController_desc.Font = lbl_overrideTpGameSettings.Font = lbl_overrideTpGameSettings_desc.Font = lbl_applyPatches.Font = lbl_applyPatches_desc.Font = lbl_useThirdParty.Font = lbl_useThirdParty_desc.Font = lbl_bezel.Font = lbl_bezel_desc.Font = lbl_ffb.Font = lbl_desc_ffb.Font = lbl_desc_crt.Font = lbl_crt.Font = lbl_aspectratio_desc.Font = lbl_aspectratio.Font = lbl_vsync_desc.Font = lbl_vsync.Font = lbl_translation_desc.Font = lbl_translation_none.Font = lbl_translation_english.Font = lbl_translation_french.Font = lbl_joystick_desc.Font = lbl_joystick.Font = lbl_cross1.Font = lbl_cross1_desc.Font = lbl_cross2.Font = lbl_cross2_desc.Font = lbl_nativeCrosshair.Font = lbl_nativeCrosshair_desc.Font = new Font(lbl_bezel.Font.Name, 8.0f * 0.70f, lbl_bezel.Font.Style, lbl_bezel.Font.Unit, lbl_bezel.Font.GdiCharSet, lbl_bezel.Font.GdiVerticalFont);
				fontComboBox = new Font(fontComboBox.Name, 9.5f * 0.80f, fontComboBox.Style, fontComboBox.Unit, fontComboBox.GdiCharSet, fontComboBox.GdiVerticalFont);
			}

			//cmb_displayMode.Font = new Font(cmb_displayMode.Font.Name, 8.25f * 0.15f, cmb_displayMode.Font.Style, cmb_displayMode.Font.Unit, cmb_displayMode.Font.GdiCharSet, cmb_displayMode.Font.GdiVerticalFont);

			cmb_displayMode.StateActive.ComboBox.Content.Font = fontComboBox;
			cmb_nativeRes.StateActive.ComboBox.Content.Font = fontComboBox;
			cmb_resolution.StateActive.ComboBox.Content.Font = fontComboBox;
			cmb_patchReshade.StateActive.ComboBox.Content.Font = fontComboBox;
			//cmb_patchlink.StateActive.ComboBox.Content.Font = fontComboBox;
			cmb_testMode.StateActive.ComboBox.Content.Font = fontComboBox;



			SevenZipExtractor.SetLibraryPath(Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "thirdparty", "7zip", "7z.dll"));
			PaletteImageScaler.ScalePalette(this, KryptonPalette1);

			this.Activated += VotreForm_Activated;
			this.Deactivate += VotreForm_Deactivate;

			fpanel_perf.DoubleClick += fpanel_perf_DoubleClick;
			lbl_perf_desc.DoubleClick += fpanel_perf_DoubleClick;
			lbl_perf_normal.DoubleClick += fpanel_perf_DoubleClick;
			lbl_perf_high.DoubleClick += fpanel_perf_DoubleClick;
			lbl_perf_low.DoubleClick += fpanel_perf_DoubleClick;

			fpanel_ffb.DoubleClick += fpanel_ffb_DoubleClick;
			lbl_ffb.DoubleClick += fpanel_ffb_DoubleClick;
			lbl_desc_ffb.DoubleClick += fpanel_ffb_DoubleClick;

			fpanel_crt.DoubleClick += fpanel_crt_DoubleClick;
			lbl_crt.DoubleClick += fpanel_crt_DoubleClick;
			lbl_desc_crt.DoubleClick += fpanel_crt_DoubleClick;

			fpanel_bezel.DoubleClick += fpanel_bezel_DoubleClick;
			lbl_bezel.DoubleClick += fpanel_bezel_DoubleClick;
			lbl_bezel_desc.DoubleClick += fpanel_bezel_DoubleClick;

			fpanel_vsync.DoubleClick += fpanel_vsync_DoubleClick;
			lbl_vsync.DoubleClick += fpanel_vsync_DoubleClick;
			lbl_vsync_desc.DoubleClick += fpanel_vsync_DoubleClick;

			fpanel_aspectratio.DoubleClick += fpanel_aspectratio_DoubleClick;
			lbl_aspectratio.DoubleClick += fpanel_aspectratio_DoubleClick;
			lbl_aspectratio_desc.DoubleClick += fpanel_aspectratio_DoubleClick;

			fpanel_translation.DoubleClick += fpanel_translation_DoubleClick;
			lbl_translation_none.DoubleClick += fpanel_translation_DoubleClick;
			lbl_translation_english.DoubleClick += fpanel_translation_DoubleClick;
			lbl_translation_french.DoubleClick += fpanel_translation_DoubleClick;
			lbl_translation_desc.DoubleClick += fpanel_translation_DoubleClick;

			fpanel_joystick.DoubleClick += fpanel_joystick_DoubleClick;
			lbl_joystick.DoubleClick += fpanel_joystick_DoubleClick;
			lbl_joystick_desc.DoubleClick += fpanel_joystick_DoubleClick;

			fpanel_cross1.DoubleClick += fpanel_cross1_DoubleClick;
			lbl_cross1.DoubleClick += fpanel_cross1_DoubleClick;
			lbl_cross1_desc.DoubleClick += fpanel_cross1_DoubleClick;

			fpanel_cross2.DoubleClick += fpanel_cross2_DoubleClick;
			lbl_cross2.DoubleClick += fpanel_cross2_DoubleClick;
			lbl_cross2_desc.DoubleClick += fpanel_cross2_DoubleClick;

			fpanel_nativeCrosshair.DoubleClick += fpanel_nativeCrosshair_DoubleClick;
			lbl_nativeCrosshair.DoubleClick += fpanel_nativeCrosshair_DoubleClick;
			lbl_nativeCrosshair_desc.DoubleClick += fpanel_nativeCrosshair_DoubleClick;

			fpanel_overrideTpController.DoubleClick += fpanel_overrideTpController_DoubleClick;
			lbl_overrideTpController.DoubleClick += fpanel_overrideTpController_DoubleClick;
			lbl_overrideTpController_desc.DoubleClick += fpanel_overrideTpController_DoubleClick;

			fpanel_overrideTpGameSettings.DoubleClick += fpanel_overrideTpGameSettings_DoubleClick;
			lbl_overrideTpGameSettings.DoubleClick += fpanel_overrideTpGameSettings_DoubleClick;
			lbl_overrideTpGameSettings_desc.DoubleClick += fpanel_overrideTpGameSettings_DoubleClick;

			fpanel_applyPatches.DoubleClick += fpanel_applyPatches_DoubleClick;
			lbl_applyPatches.DoubleClick += fpanel_applyPatches_DoubleClick;
			lbl_applyPatches_desc.DoubleClick += fpanel_applyPatches_DoubleClick;

			fpanel_useThirdParty.DoubleClick += fpanel_useThirdParty_DoubleClick;
			lbl_useThirdParty.DoubleClick += fpanel_useThirdParty_DoubleClick;
			lbl_useThirdParty_desc.DoubleClick += fpanel_useThirdParty_DoubleClick;


			// Supposons que tu as plusieurs FlowLayoutPanel : flowLayoutPanel1, flowLayoutPanel2, flowLayoutPanel3
			var flowPanels = new[] { fpanel_perf, fpanel_ffb, fpanel_crt, fpanel_bezel, fpanel_vsync, fpanel_aspectratio, fpanel_translation, fpanel_joystick, fpanel_cross1, fpanel_cross2, fpanel_overrideTpController, fpanel_overrideTpGameSettings, fpanel_overrideTpGameSettings, fpanel_applyPatches, fpanel_useThirdParty, fpanel_nativeCrosshair };
			foreach (var panel in flowPanels)
			{
				panel.MouseEnter += (sender, e) =>
				{
					((FlowLayoutPanel)sender).BackColor = Color.LightYellow; // Changer la couleur de fond au survol
					((FlowLayoutPanel)sender).Cursor = Cursors.Hand; // Changer le curseur de la souris
				};

				panel.MouseLeave += (sender, e) =>
				{
					((FlowLayoutPanel)sender).BackColor = SystemColors.Control; // Remettre la couleur par défaut
					((FlowLayoutPanel)sender).Cursor = Cursors.Default; // Remettre le pointeur par défaut
				};

				// Associer les événements pour chaque Label dans le FlowLayoutPanel
				foreach (Control control in panel.Controls)
				{
					if (control is Label label) // Vérifier si le contrôle est un Label
					{
						label.MouseEnter += (sender, e) =>
						{
							panel.BackColor = Color.LightYellow; // Changer la couleur de fond
							panel.Cursor = Cursors.Hand; // Changer le pointeur de la souris
						};

						label.MouseLeave += (sender, e) =>
						{
							panel.BackColor = SystemColors.Control; // Remettre la couleur par défaut
							panel.Cursor = Cursors.Default; // Remettre le pointeur par défaut
						};
					}
				}
			}

			//chk_showAll.Checked = ConfigurationManager.MainConfig.ShowAllGames;

			typeConfig.Add("gamepad");
			typeConfig.Add("gamepadalt");
			typeConfig.Add("arcade");
			typeConfig.Add("wheel");
			typeConfig.Add("hotas");
			typeConfig.Add("lightgun");

			lbl_player1.Text = "";
			lbl_player2.Text = "";
			lbl_player3.Text = "";
			lbl_player4.Text = "";
			lbl_GameTitle.Text = "";

			Reload();
			UpdateGamePadList();
			MenuItem_textBoxFilter.LostFocus += new System.EventHandler(this.MenuItem_textBoxFilter_Leave);
			MenuItem_textBoxFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MenuItem_textBoxFilter_CheckEnterKeyPress);
		}

		private void ChangePalette(PaletteMode palMode)
		{
			KryptonPalette1.SuspendUpdates();
			KryptonPalette1.BasePaletteMode = palMode;
			PaletteImageScaler.ScalePalette(this, KryptonPalette1);
			KryptonPalette1.ResumeUpdates();
		}

		private void VotreForm_Activated(object sender, EventArgs e)
		{
			timer_controllerUpdate.Enabled = true;
		}

		private void VotreForm_Deactivate(object sender, EventArgs e)
		{
			timer_controllerUpdate.Enabled = false;
		}

		private void Reload()
		{
			string selectedGameName = "<none>";
			Game gameToSelect = null;
			if (fastObjectListView1.SelectedIndex >= 0)
			{
				Game DataGame = (Game)fastObjectListView1.SelectedObject;
				selectedGameName = DataGame.Name;
			}

			IsReloading = true;
			fastObjectListView1.Items.Clear();

			_gameList = new Dictionary<string, Game>();
			var gamelist2 = new List<Game>();
			_tpFolder = ConfigurationManager.MainConfig.TpFolder;
			if (Directory.Exists(_tpFolder))
			{
				string UserProfileDir = Path.Combine(Path.GetFullPath(_tpFolder), "UserProfiles");
				if (Directory.Exists(UserProfileDir))
				{
					fastObjectListView1.Enabled = true;
					_userProfileFolder = UserProfileDir;
					var profileList = Directory.GetFiles(_userProfileFolder, "*.xml");
					foreach (var profile in profileList)
					{
						if (profile.ToLower().EndsWith("custom.xml")) continue;
						string gameName = ExtractGameNameInternal(profile);

						if (!_gameList.ContainsKey(gameName))
						{
							string infoFilePath = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "config", Path.GetFileNameWithoutExtension(profile) + ".info.json");
							var newGame = new Game();
							newGame.Name = gameName;
							newGame.UserConfigFile = profile;
							newGame.InfoFile = File.Exists(infoFilePath) ? infoFilePath : "";
							newGame.FileName = Path.GetFileName(profile);
							newGame.Metadata = DeSerializeMetadata(profile);
							newGame.TestMenuIsExecutable = ExtractTestMenuIsExecutable(profile);


							List<string> typeConfig = new List<string>();
							typeConfig.Add("gamepad");
							typeConfig.Add("gamepadalt");
							typeConfig.Add("arcade");
							typeConfig.Add("wheel");
							typeConfig.Add("hotas");
							typeConfig.Add("lightgun");
							string basePath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
							foreach (var type in typeConfig)
							{
								var configPath = Path.Combine(basePath, "config", Path.GetFileNameWithoutExtension(profile) + "." + type + ".txt");
								if (File.Exists(configPath))
								{
									if (type == "wheel")
									{
										newGame.haveWheelSupport = true;
									}
									if (type == "hotas")
									{
										newGame.haveHotasSupport = true;
									}
									if (type == "lightgun")
									{
										newGame.haveLightgunSupport = true;
									}
									if (type == "arcade")
									{
										newGame.haveArcadeStickSupport = true;
									}
								}
							}

							if (newGame.Metadata != null)
							{
								newGame.Name = newGame.Metadata.game_name;
							}

							foreach (var type in typeConfig)
							{
								var configPath = Path.Combine(_basePath, "config", Path.GetFileNameWithoutExtension(profile) + "." + type + ".txt");
								if (File.Exists(configPath))
								{
									newGame.existingConfig.Add(type, configPath);
								}
							}
							if (newGame.existingConfig.Count > 0)
							{
								newGame.isSupported = true;
								newGame.DisplayName = newGame.Name;
							}
							else
							{
								newGame.isSupported = false;
								if (newGame.InfoFile == "") newGame.DisplayName = newGame.Name + " [NOT SUPPORTED]";
								else newGame.DisplayName = newGame.Name + " [NO AUTO CONTROLS]";
							}

							if (newGame.Name == selectedGameName) gameToSelect = newGame;
							gamelist2.Add(newGame);


							_gameList.Add(newGame.Name, newGame);
						}
					}
					fastObjectListView1.SetObjects(gamelist2.ToArray());
					fastObjectListView1.Sort(columnHeader1, SortOrder.Ascending);
					fastObjectListView1.Refresh();

				}
			}
			IsReloading = false;
			if (gameToSelect != null)
			{
				int index = fastObjectListView1.IndexOf(gameToSelect);
				if (index >= 0) fastObjectListView1.SelectedIndex = index;
			}
		}

		private void UpdateGamePadList()
		{
			string wheelGuid = string.Empty;
			string hotasGuid = string.Empty;
			_dinputWheelName = string.Empty;
			_dinputWheelFound = false;
			_haveWheel = _haveArcade = _haveGamepad = _haveHotas = false;

			_connectedGamePad.Clear();
			var gamepad = X.Gamepad_1;
			if (gamepad.Capabilities.Type != 0) _connectedGamePad.Add(0, new XinputGamepad(gamepad, 0, false));
			gamepad = X.Gamepad_2;
			if (gamepad.Capabilities.Type != 0) _connectedGamePad.Add(1, new XinputGamepad(gamepad, 1, false));
			gamepad = X.Gamepad_3;
			if (gamepad.Capabilities.Type != 0) _connectedGamePad.Add(2, new XinputGamepad(gamepad, 2, false));
			gamepad = X.Gamepad_4;
			if (gamepad.Capabilities.Type != 0) _connectedGamePad.Add(3, new XinputGamepad(gamepad, 3, false));
			bool checkDinputWheel = ConfigurationManager.MainConfig.useDinputWheel;
			Dictionary<string, JoystickButtonData> bindingDinputWheel = null;
			string bindingDinputWheelJson = ConfigurationManager.MainConfig.bindingDinputWheel;
			bool checkDinputHotas = ConfigurationManager.MainConfig.useDinputHotas;
			Dictionary<string, JoystickButtonData> bindingDinputHotas = null;
			string bindingDinputHotasJson = ConfigurationManager.MainConfig.bindingDinputHotas;

			bool checkDinputLightgun = false;
			string LightgunA_Type = ConfigurationManager.MainConfig.gunAType;
			string LightgunB_Type = ConfigurationManager.MainConfig.gunBType;
			if (!string.IsNullOrEmpty(LightgunA_Type) && LightgunA_Type != "<none>") checkDinputLightgun = true;
			if (!string.IsNullOrEmpty(LightgunB_Type) && LightgunB_Type != "<none>") checkDinputLightgun = true;
			string bindingDinputLightgunAJson = "";
			string bindingDinputLightgunBJson = "";
			Dictionary<string, JoystickButtonData> bindingDinputLightGunA = null;
			Dictionary<string, JoystickButtonData> bindingDinputLightGunB = null;
			Dictionary<string, JoystickButtonData> bindingDinputLightGun = new Dictionary<string, JoystickButtonData>();
			string gunAGuid = string.Empty;
			string gunBGuid = string.Empty;

			_dinputGunBType = string.Empty;
			_dinputGunAType = string.Empty;
			_dinputGunAName = string.Empty;
			_dinputGunBName = string.Empty;
			_dinputLightgunAFound = false;
			_dinputLightgunBFound = false;
			_haveLightgun = false;
			if (checkDinputLightgun)
			{
				int nb_wiimote = 0;
				int current_wiimote = 0;
				bool wiimoteA_unplugged = false;
				bool wiimoteB_unplugged = false;

				if (LightgunA_Type == "wiimote" || LightgunB_Type == "wiimote")
				{
					WiimoteCollection mWC = new WiimoteCollection();
					try
					{
						mWC.FindAllWiimotes();
					}
					catch (Exception ex)
					{

					}
					foreach (Wiimote wm in mWC)
					{
						nb_wiimote++;
					}
				}

				if (LightgunA_Type == "sinden" || LightgunA_Type == "guncon1" || LightgunA_Type == "guncon2" || LightgunA_Type == "wiimote" || LightgunA_Type == "gamepad")
				{
					_dinputGunAName = $"Gun A [{LightgunA_Type}] ";
					if (LightgunA_Type == "gamepad") bindingDinputLightgunAJson = ConfigurationManager.MainConfig.bindingDinputGunAXbox;
					if (LightgunA_Type == "sinden") bindingDinputLightgunAJson = ConfigurationManager.MainConfig.bindingDinputGunASinden;
					if (LightgunA_Type == "guncon1") bindingDinputLightgunAJson = ConfigurationManager.MainConfig.bindingDinputGunAGuncon1;
					if (LightgunA_Type == "guncon2") bindingDinputLightgunAJson = ConfigurationManager.MainConfig.bindingDinputGunAGuncon2;
					if (LightgunA_Type == "wiimote") bindingDinputLightgunAJson = ConfigurationManager.MainConfig.bindingDinputGunAWiimote;
					bindingDinputLightGunA = (Dictionary<string, JoystickButtonData>)JsonConvert.DeserializeObject<Dictionary<string, JoystickButtonData>>(bindingDinputLightgunAJson);
					if (bindingDinputLightGunA != null && bindingDinputLightGunA.ContainsKey("LightgunX"))
					{
						gunAGuid = bindingDinputLightGunA["LightgunX"].JoystickGuid.ToString();
					}
					if (LightgunA_Type == "wiimote")
					{
						if (bindingDinputLightGunA != null && bindingDinputLightGunA.ContainsKey("LightgunX"))
						{
							if (bindingDinputLightGunA["LightgunX"].DeviceName.ToLower().Contains("vjoy"))
							{
								current_wiimote++;
								if (nb_wiimote < current_wiimote) gunAGuid = "";
							}
						}
					}
				}
				if (LightgunB_Type == "sinden" || LightgunA_Type == "guncon1" || LightgunB_Type == "guncon2" || LightgunB_Type == "wiimote" || LightgunB_Type == "gamepad")
				{
					_dinputGunBName = $"Gun B [{LightgunB_Type}] ";
					if (LightgunB_Type == "gamepad") bindingDinputLightgunBJson = ConfigurationManager.MainConfig.bindingDinputGunAXbox;
					if (LightgunB_Type == "sinden") bindingDinputLightgunBJson = ConfigurationManager.MainConfig.bindingDinputGunBSinden;
					if (LightgunB_Type == "guncon1") bindingDinputLightgunBJson = ConfigurationManager.MainConfig.bindingDinputGunBGuncon1;
					if (LightgunB_Type == "guncon2") bindingDinputLightgunBJson = ConfigurationManager.MainConfig.bindingDinputGunBGuncon2;
					if (LightgunB_Type == "wiimote") bindingDinputLightgunBJson = ConfigurationManager.MainConfig.bindingDinputGunBWiimote;
					bindingDinputLightGunB = (Dictionary<string, JoystickButtonData>)JsonConvert.DeserializeObject<Dictionary<string, JoystickButtonData>>(bindingDinputLightgunBJson);
					if (bindingDinputLightGunB != null && bindingDinputLightGunB.ContainsKey("LightgunX"))
					{
						gunBGuid = bindingDinputLightGunB["LightgunX"].JoystickGuid.ToString();
					}
					if (LightgunB_Type == "wiimote")
					{
						if (bindingDinputLightGunB != null && bindingDinputLightGunB.ContainsKey("LightgunX"))
						{
							if (bindingDinputLightGunB["LightgunX"].DeviceName.ToLower().Contains("vjoy"))
							{
								current_wiimote++;
								if (nb_wiimote < current_wiimote) gunBGuid = "";
							}
						}
					}
				}
				if (!string.IsNullOrEmpty(gunAGuid) || !string.IsNullOrEmpty(gunBGuid))
				{
					DirectInput directInput = new DirectInput();
					List<DeviceInstance> devices = new List<DeviceInstance>();
					devices.AddRange(directInput.GetDevices().Where(x => x.Type != SharpDX.DirectInput.DeviceType.Mouse && x.UsagePage != UsagePage.VendorDefinedBegin && x.Usage != UsageId.AlphanumericBitmapSizeX && x.Usage != UsageId.AlphanumericAlphanumericDisplay && x.UsagePage != unchecked((UsagePage)0xffffff43) && x.UsagePage != UsagePage.Vr).ToList());
					foreach (var device in devices)
					{
						if (device.InstanceGuid.ToString() == gunAGuid)
						{
							Utils.LogMessage($"GunAGuid Found");
							_dinputLightgunAFound = true;
							_haveLightgun = true;
							_dinputGunAName += device.ProductName;
							_dinputGunAType = LightgunA_Type;
						}
						if (device.InstanceGuid.ToString() == gunBGuid)
						{
							Utils.LogMessage($"GunBGuid Found");
							_dinputLightgunBFound = true;
							_haveLightgun = true;
							_dinputGunBName += device.ProductName;
							_dinputGunBType = LightgunB_Type;
						}
					}
				}

			}


			foreach (var cg in _connectedGamePad)
			{
				if (cg.Value.Type == "arcade") _haveArcade = true;
				if (cg.Value.Type == "gamepad") _haveGamepad = true;
				if (cg.Value.Type == "wheel") _haveWheel = true;
			}

			if (checkDinputWheel)
			{
				if (!string.IsNullOrEmpty(bindingDinputWheelJson))
				{
					bindingDinputWheel = (Dictionary<string, JoystickButtonData>)JsonConvert.DeserializeObject<Dictionary<string, JoystickButtonData>>(bindingDinputWheelJson);
					if (bindingDinputWheel.ContainsKey("InputDevice0LeftThumbInputDevice0X+"))
					{
						wheelGuid = bindingDinputWheel["InputDevice0LeftThumbInputDevice0X+"].JoystickGuid.ToString();
					}
				}
				if (!string.IsNullOrEmpty(wheelGuid))
				{
					DirectInput directInput = new DirectInput();
					List<DeviceInstance> devices = new List<DeviceInstance>();
					devices.AddRange(directInput.GetDevices().Where(x => x.Type != DeviceType.Mouse && x.UsagePage != UsagePage.VendorDefinedBegin && x.Usage != UsageId.AlphanumericBitmapSizeX && x.Usage != UsageId.AlphanumericAlphanumericDisplay && x.UsagePage != unchecked((UsagePage)0xffffff43) && x.UsagePage != UsagePage.Vr).ToList());
					foreach (var device in devices)
					{
						if (device.InstanceGuid.ToString() == wheelGuid)
						{
							_dinputWheelFound = true;
							_dinputWheelName = device.ProductName;
							_haveWheel = true;
							break;
						}
					}
				}
			}

			if (checkDinputHotas)
			{
				if (!string.IsNullOrEmpty(bindingDinputHotasJson))
				{
					bindingDinputHotas = (Dictionary<string, JoystickButtonData>)JsonConvert.DeserializeObject<Dictionary<string, JoystickButtonData>>(bindingDinputHotasJson);
					if (bindingDinputHotas.ContainsKey("InputDevice0LeftThumbInputDevice0X+"))
					{
						hotasGuid = bindingDinputHotas["InputDevice0LeftThumbInputDevice0X+"].JoystickGuid.ToString();
					}
				}
				if (!string.IsNullOrEmpty(hotasGuid))
				{
					DirectInput directInput = new DirectInput();
					List<DeviceInstance> devices = new List<DeviceInstance>();
					devices.AddRange(directInput.GetDevices().Where(x => x.Type != DeviceType.Mouse && x.UsagePage != UsagePage.VendorDefinedBegin && x.Usage != UsageId.AlphanumericBitmapSizeX && x.Usage != UsageId.AlphanumericAlphanumericDisplay && x.UsagePage != unchecked((UsagePage)0xffffff43) && x.UsagePage != UsagePage.Vr).ToList());
					foreach (var device in devices)
					{
						if (device.InstanceGuid.ToString() == hotasGuid)
						{
							_dinputHotasFound = true;
							_dinputHotasName = device.ProductName;
							_haveHotas = true;
							break;
						}
					}
				}
			}

		}

		public static string ExtractGameNameInternal(string cheminFichierXml)
		{
			string DefaultName = Path.GetFileName(cheminFichierXml);
			DefaultName = DefaultName.Substring(0, DefaultName.Length - 4);

			try
			{
				using (FileStream fs = new FileStream(cheminFichierXml, FileMode.Open, FileAccess.Read))
				using (StreamReader reader = new StreamReader(fs))
				{
					int bufferSize = 4096;
					char[] buffer = new char[bufferSize];
					string pattern = "<GameNameInternal>(.*?)</GameNameInternal>";
					Regex regex = new Regex(pattern);
					while (!reader.EndOfStream)
					{
						int bytesRead = reader.Read(buffer, 0, bufferSize);
						string bufferContent = new string(buffer, 0, bytesRead);
						Match match = regex.Match(bufferContent);
						if (match.Success)
						{
							return match.Groups[1].Value;
						}
					}
					return DefaultName;
				}
			}
			catch { return DefaultName; }
		}

		static bool ExtractTestMenuIsExecutable(string cheminFichierXml)
		{
			try
			{
				using (FileStream fs = new FileStream(cheminFichierXml, FileMode.Open, FileAccess.Read))
				using (StreamReader reader = new StreamReader(fs))
				{
					int bufferSize = 4096;
					char[] buffer = new char[bufferSize];
					string pattern = "<TestMenuIsExecutable>(.*?)</TestMenuIsExecutable>";
					Regex regex = new Regex(pattern);
					while (!reader.EndOfStream)
					{
						int bytesRead = reader.Read(buffer, 0, bufferSize);
						string bufferContent = new string(buffer, 0, bytesRead);
						Match match = regex.Match(bufferContent);
						if (match.Success)
						{
							if (match.Groups[1].Value.Trim().ToLower() == "true") return true;
							else return false;
						}
					}
					return false;
				}
			}
			catch { return false; }
		}

		public static Metadata DeSerializeMetadata(string fileName)
		{
			string ParentPath = Path.GetDirectoryName(fileName);
			ParentPath = Path.GetDirectoryName(ParentPath);

			var metadataPath = Path.Combine(ParentPath, "Metadata", Path.GetFileNameWithoutExtension(fileName) + ".json");
			if (File.Exists(metadataPath))
			{
				try
				{
					return JsonConvert.DeserializeObject<Metadata>(Utils.ReadAllText(metadataPath));
				}
				catch
				{
					Debug.WriteLine($"Error loading Metadata file {metadataPath}!");
				}
			}
			else
			{
				Debug.WriteLine($"Metadata file {metadataPath} missing!");
			}
			return null;
		}


		private void MainDPIcs_Load(object sender, EventArgs e)
		{
			cmb_displayMode.SelectedIndex = 0;
			cmb_patchReshade.SelectedIndex = 0;
			cmb_resolution.SelectedIndex = 0;
			//cmb_patchlink.SelectedIndex = 0;

		}

		private void btn_globalconfig_Click(object sender, EventArgs e)
		{
			if (ConfigurationManager.MainConfig.advancedConfig)
			{
				var frm = new Form1();
				var result = frm.ShowDialog();
				if (result == DialogResult.OK)
				{
					Reload();
					if (_selectedGame != null) fastObjectListView1_SelectedIndexChanged(null, null);
				}

			}
			else
			{
				var frm = new Form1Simple();
				var result = frm.ShowDialog();
				if (result == DialogResult.OK)
				{
					Reload();
					if (_selectedGame != null) fastObjectListView1_SelectedIndexChanged(null, null);
				}
			}
		}
		/*
		private void chk_showAll_CheckedChanged(object sender, EventArgs e)
		{
			ConfigurationManager.MainConfig.ShowAllGames = chk_showAll.Checked;
			ConfigurationManager.SaveConfig();
			Reload();
		}
		*/

		private void timer_controllerUpdate_Tick(object sender, EventArgs e)
		{
			lbl_gamepadlist.Text = "";
			lbl_arcadelist.Text = "";
			lbl_wheellist.Text = "";
			lbl_gunslist.Text = "";
			UpdateGamePadList();
			foreach (var gp in _connectedGamePad)
			{
				string displayControllerName = "XINPUT" + (gp.Value.XinputSlot + 1).ToString() + " " + gp.Value.ControllerName;

				if (gp.Value.Type == "gamepad") lbl_gamepadlist.Text += $"{displayControllerName}, ";
				if (gp.Value.Type == "arcade") lbl_arcadelist.Text += $"{displayControllerName}, ";
				if (gp.Value.Type == "wheel") lbl_wheellist.Text += $"{displayControllerName}, ";
			}
			if (_dinputWheelFound) lbl_wheellist.Text = _dinputWheelName;
			if (_dinputHotasFound) lbl_hotaslist.Text = _dinputHotasName;
			lbl_arcadelist.Text = lbl_arcadelist.Text.TrimEnd().TrimEnd(',');
			lbl_gamepadlist.Text = lbl_gamepadlist.Text.TrimEnd().TrimEnd(',');
			lbl_wheellist.Text = lbl_wheellist.Text.TrimEnd().TrimEnd(',');

			if (_dinputLightgunAFound) lbl_gunslist.Text += $"{_dinputGunAName}, ";
			if (_dinputLightgunBFound) lbl_gunslist.Text += $"{_dinputGunBName}, ";
			lbl_gunslist.Text = lbl_gunslist.Text.TrimEnd().TrimEnd(',');
		}


		public static Image ResizeImageBest(Image image, Size newSize)
		{
			return image;
			Image imageInMem = new Bitmap(image.Width, image.Height);

			// Utiliser Graphics pour dessiner l'image originale sur la nouvelle image
			using (Graphics graphics = Graphics.FromImage(imageInMem))
			{
				graphics.DrawImage(image, 0, 0, image.Width, image.Height);
			}

			return imageInMem;

			return image;
			if (image == null)
				throw new ArgumentNullException(nameof(image));

			if (newSize.Width <= 0 || newSize.Height <= 0)
				throw new ArgumentException("La taille doit être supérieure à zéro.", nameof(newSize));

			Bitmap newImage = new Bitmap(newSize.Width, newSize.Height);

			using (Graphics graphics = Graphics.FromImage(newImage))
			{

				float aspectRatio = (float)image.Width / image.Height;
				int newWidth = image.Width;
				int newHeight = image.Height;
				/*
				int newWidth = newSize.Width;
				int newHeight = newSize.Height;

				if (aspectRatio > 1)
				{
					// L'image est plus large que haute, ajuster en fonction de la largeur
					newHeight = (int)(newWidth / aspectRatio);
				}
				else
				{
					// L'image est plus haute que large, ajuster en fonction de la hauteur
					newWidth = (int)(newHeight * aspectRatio);
				}
				*/
				if (newWidth > newSize.Width || newHeight > newSize.Height)
				{
					if ((newWidth / newSize.Width) > (newHeight / newSize.Height))
					{
						newHeight = (int)Math.Round(((double)newSize.Width / (double)newWidth) * (double)newHeight);
						newWidth = newSize.Width;
					}
					else
					{
						newWidth = (int)Math.Round(((double)newSize.Height / (double)newHeight) * (double)newWidth);
						newHeight = newSize.Height;
					}
				}

				int x = (int)Math.Round((double)(newSize.Width - newWidth) / 2.0);
				int y = (int)Math.Round((double)(newSize.Height - newHeight) / 2.0);

				graphics.DrawImage(image, x, y, newWidth, newHeight);
			}
			image.Dispose();
			return newImage;
		}

		private void AddPictureBoxesToFlowLayoutPanel(String[] selectedImgList)
		{
			if (flowLayoutPanelThumbs.Visible == false) flowLayoutPanelThumbs.Visible = true;
			// Supprimez tous les contrôles existants dans le FlowLayoutPanel
			flowLayoutPanelThumbs.Controls.Clear();

			// La marge entre chaque PictureBox
			int spacing = 10;

			// Pour chaque image dans votre tableau selectedImgList
			foreach (var imgDetails in selectedImgList)
			{


				if (string.IsNullOrEmpty(imgDetails) || !File.Exists(imgDetails)) continue;

				Image ImageThumb = null;
				try
				{
					Image originalImage = System.Drawing.Image.FromFile(imgDetails);
					ImageThumb = ResizeImageBest(originalImage, new Size(77, 50));
				}
				catch
				{
					continue;
				}
				if (ImageThumb == null) continue;


				// Créez une nouvelle instance de PictureBox
				PictureBox pictureBox = new PictureBox();

				pictureBox.Anchor = AnchorStyles.Left;

				// Définissez la taille de la PictureBox à 77x77 pixels
				pictureBox.Size = new Size(77, 50);

				// Assurez-vous que la taille de l'image est ajustée à la taille de la PictureBox
				pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

				// Définissez l'image à afficher dans la PictureBox (imgDetails.Image représente votre image)
				pictureBox.Image = ImageThumb;

				// Ajoutez une fonction anonyme pour gérer le clic sur la PictureBox
				pictureBox.Click += (sender, e) =>
				{
					try
					{

						Image originalImage = System.Drawing.Image.FromFile(imgDetails);
						pictureBox_gameControls.Image = ResizeImageBest(originalImage, pictureBox_gameControls.Size);
						pictureBox_gameControls.Visible = true;
					}
					catch
					{
						pictureBox_gameControls.Image = null;
					}


				};

				// Ajoutez la PictureBox au FlowLayoutPanel
				flowLayoutPanelThumbs.Controls.Add(pictureBox);

				/*
				// Définissez la marge entre les PictureBox
				flowLayoutPanelThumbs.SetFlowBreak(pictureBox, true);

				// Ajoutez un espacement horizontal entre les PictureBox
				if (flowLayoutPanelThumbs.Controls.Count > 1)
				{
					pictureBox.Margin = new Padding(spacing, 0, 0, 0);
				}
				*/
			}
		}

		private async void btn_playgame_Click(object sender, EventArgs e)
		{



			string teknoparrotExe = Path.Combine(_tpFolder, "TeknoParrotUi.exe");
			if (!File.Exists(teknoparrotExe)) MessageBox.Show($"Can't find {teknoparrotExe}");
			Process[] existingProcesses = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(teknoparrotExe));
			if (existingProcesses.Length > 0)
			{
				foreach (var existingProcess in existingProcesses)
				{
					existingProcess.Kill();
					Thread.Sleep(1000);
				}
			}


			string finalConfig = "";
			if (_selectedGame != null)
			{
				//btn_playgamedirect.Enabled = true;
				//btn_playgamedirect2.Enabled = true;

				finalConfig = _selectedGame.UserConfigFile;

			}
			if (string.IsNullOrEmpty(finalConfig)) return;


			string arguments = "";
			if (cmb_displayMode.SelectedIndex > 0) arguments += $" --displayMode={cmb_displayMode.SelectedIndex}";
			if (cmb_resolution.SelectedIndex > 0) arguments += $" --resolution={cmb_resolution.SelectedIndex}";
			if (cmb_patchReshade.SelectedIndex > 0) arguments += $" --reshade={cmb_patchReshade.SelectedIndex}";
			//if (cmb_patchlink.SelectedIndex > 0) arguments += $" --nolink";
			if (cmb_testMode.SelectedIndex > 0) arguments += $" --testmode";

			arguments += $" \"{finalConfig}\"";
			arguments = arguments.Trim();
			isPlaying = true;

			await Task.Run(() =>
			{
				ProcessStartInfo startInfo = new ProcessStartInfo
				{
					FileName = Process.GetCurrentProcess().MainModule.FileName,
					Arguments = arguments,
					WorkingDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
					UseShellExecute = true
				};

				using (Process process = new Process { StartInfo = startInfo })
				{
					try
					{
						process.Start();
						process.WaitForExit();
					}
					catch (Exception ex)
					{
						MessageBox.Show("Erreur lors du lancement du programme : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			});

			isPlaying = false;
		}

		private async void btn_playgamedirect_Click(object sender, EventArgs e)
		{
			string teknoparrotExe = Path.Combine(_tpFolder, "TeknoParrotUi.exe");
			if (!File.Exists(teknoparrotExe)) MessageBox.Show($"Can't find {teknoparrotExe}");
			Process[] existingProcesses = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(teknoparrotExe));
			if (existingProcesses.Length > 0)
			{
				foreach (var existingProcess in existingProcesses)
				{
					existingProcess.Kill();
					Thread.Sleep(1000);
				}
			}


			string finalConfig = "";
			if (_selectedGame != null)
			{
				//btn_playgamedirect.Enabled = true;
				//btn_playgamedirect2.Enabled = true;

				finalConfig = _selectedGame.UserConfigFile;

			}
			if (string.IsNullOrEmpty(finalConfig)) return;

			string arguments = $"--passthrough ";
			if (cmb_displayMode.SelectedIndex > 0) arguments += $" --displayMode={cmb_displayMode.SelectedIndex}";
			if (cmb_resolution.SelectedIndex > 0) arguments += $" --resolution={cmb_resolution.SelectedIndex}";
			if (cmb_patchReshade.SelectedIndex > 0) arguments += $" --reshade={cmb_patchReshade.SelectedIndex}";
			//if (cmb_patchlink.SelectedIndex > 0) arguments += $" --nolink";
			if (cmb_testMode.SelectedIndex > 0) arguments += $" --testmode";
			arguments += $" \"{finalConfig}\"";
			arguments = arguments.Trim();

			isPlaying = true;

			await Task.Run(() =>
			{
				ProcessStartInfo startInfo = new ProcessStartInfo
				{
					FileName = Process.GetCurrentProcess().MainModule.FileName,
					Arguments = arguments,
					WorkingDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
					UseShellExecute = true
				};

				using (Process process = new Process { StartInfo = startInfo })
				{
					try
					{
						process.Start();
						process.WaitForExit();
					}
					catch (Exception ex)
					{
						MessageBox.Show("Erreur lors du lancement du programme : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			});

			isPlaying = false;

		}

		private void btn_gameoptions_Click(object sender, EventArgs e)
		{
			if (_selectedGame == null)
			{
				return;
			}

			if (_selectedGame != null)
			{
				if (ConfigurationManager.MainConfig.advancedConfig)
				{
					var frm = new GameOptions(_selectedGame);
					var result = frm.ShowDialog();
					if (result == DialogResult.OK)
					{
						fastObjectListView1_SelectedIndexChanged(null, null);
					}
				}
				else
				{
					var frm = new GameOptionsSimple(_selectedGame);
					var result = frm.ShowDialog();
					if (result == DialogResult.OK)
					{
						fastObjectListView1_SelectedIndexChanged(null, null);
					}
				}

			}
		}

		private void btn_tpsettings_Click(object sender, EventArgs e)
		{
			// Chemin de l'application à lancer
			string teknoparrotExe = Path.Combine(_tpFolder, "TeknoParrotUi.exe");
			if (!File.Exists(teknoparrotExe)) MessageBox.Show($"Can't find {teknoparrotExe}");
			Process[] existingProcesses = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(teknoparrotExe));
			if (existingProcesses.Length > 0)
			{
				foreach (var existingProcess in existingProcesses)
				{
					existingProcess.Kill();
					Thread.Sleep(1000);
				}
			}


			Process process = Process.Start(teknoparrotExe);
			process.WaitForInputIdle();
			int tries = 0;
			while (!Utils.IsWindowVisible(process.MainWindowHandle))
			{
				if (tries >= 50) // 50 * 100ms = 5 secondes
				{
					Console.WriteLine("La fenêtre de l'application n'est pas visible après 5 secondes. Sortie du programme.");
					return;
				}

				tries++; // Incrémenter le compteur de tentatives
						 // Attendre un court instant avant de vérifier à nouveau
				System.Threading.Thread.Sleep(100);
			}



			// Envoie du texte caractère par caractère
			string escapedTitle = lbl_GameTitle.Text;
			foreach (char c in escapedTitle)
			{
				if (c.ToString() == "(")
					SendKeys.SendWait("{(}");
				else if (c.ToString() == ")")
					SendKeys.SendWait("{)}");
				else if (c.ToString() == "^")
					SendKeys.SendWait("{^}");
				else if (c.ToString() == "+")
					SendKeys.SendWait("{+}");
				else if (c.ToString() == "%")
					SendKeys.SendWait("{%}");
				else if (c.ToString() == "~")
					SendKeys.SendWait("{~}");
				else if (c.ToString() == "{")
					SendKeys.SendWait("{{}");
				else if (c.ToString() == "}")
					SendKeys.SendWait("{}}");
				else SendKeys.SendWait(c.ToString());
				Thread.Sleep(50); // Attendre un court instant entre chaque caractère (facultatif)
			}

			//SendKeys.SendWait("{TAB}");
			//SendKeys.SendWait("{TAB}");
			//SendKeys.SendWait("{TAB}");
			//SendKeys.SendWait("{ENTER}");


		}

		private async void btn_playgamedirect2_Click(object sender, EventArgs e)
		{
			string teknoparrotExe = Path.Combine(_tpFolder, "TeknoParrotUi.exe");
			if (!File.Exists(teknoparrotExe)) MessageBox.Show($"Can't find {teknoparrotExe}");
			Process[] existingProcesses = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(teknoparrotExe));
			if (existingProcesses.Length > 0)
			{
				foreach (var existingProcess in existingProcesses)
				{
					existingProcess.Kill();
					Thread.Sleep(1000);
				}
			}


			string finalConfig = "";
			if (_selectedGame != null)
			{
				//btn_playgamedirect.Enabled = true;
				//btn_playgamedirect2.Enabled = true;

				finalConfig = _selectedGame.UserConfigFile;

			}
			if (string.IsNullOrEmpty(finalConfig)) return;

			string arguments = $"--fullpassthrough ";
			if (cmb_displayMode.SelectedIndex > 0) arguments += $" --displayMode={cmb_displayMode.SelectedIndex}";
			if (cmb_resolution.SelectedIndex > 0) arguments += $" --resolution={cmb_resolution.SelectedIndex}";
			if (cmb_patchReshade.SelectedIndex > 0) arguments += $" --reshade={cmb_patchReshade.SelectedIndex}";
			//if (cmb_patchlink.SelectedIndex > 0) arguments += $" --nolink";
			if (cmb_testMode.SelectedIndex > 0) arguments += $" --testmode";
			arguments += $" \"{finalConfig}\"";
			arguments = arguments.Trim();

			isPlaying = true;

			await Task.Run(() =>
			{
				ProcessStartInfo startInfo = new ProcessStartInfo
				{
					FileName = Process.GetCurrentProcess().MainModule.FileName,
					Arguments = arguments,
					WorkingDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
					UseShellExecute = true
				};

				using (Process process = new Process { StartInfo = startInfo })
				{
					try
					{
						process.Start();
						process.WaitForExit();
					}
					catch (Exception ex)
					{
						MessageBox.Show("Erreur lors du lancement du programme : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			});

			isPlaying = false;
		}

		private void kryptonButton1_Click(object sender, EventArgs e)
		{
			var frm = new Wizard();
			var result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{

			}

		}

		private void fastObjectListView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (IsReloading) return;

			cmb_testMode.SelectedIndex = 0;
			//	cmb_testMode.Enabled = false;

			fpanel_perf.Visible = false;
			fpanel_ffb.Visible = false;
			fpanel_crt.Visible = false;
			fpanel_bezel.Visible = false;
			fpanel_vsync.Visible = false;
			fpanel_aspectratio.Visible = false;
			fpanel_translation.Visible = false;
			fpanel_joystick.Visible = false;
			fpanel_cross1.Visible = false;
			fpanel_cross2.Visible = false;
			fpanel_nativeCrosshair.Visible = false;

			btn_playgame.Enabled = false;
			//btn_playgamedirect.Enabled = false;
			//btn_playgamedirect2.Enabled = false;
			btn_gameoptions.Enabled = false;
			btn_tpsettings.Enabled = false;

			_playAutoEnabled = false;
			_playDirectEnabled = false;
			lbl_player1.Text = "";
			lbl_player2.Text = "";
			lbl_player3.Text = "";
			lbl_player4.Text = "";
			flowLayoutPanelThumbs.Controls.Clear();

			lbl_GameTitle.Text = string.Empty;
			pictureBox_gameControls.Image = null;
			if (fastObjectListView1.SelectedIndex >= 0)
			{
				//btn_playgamedirect.Enabled = true;
				//btn_playgamedirect2.Enabled = true;

				Game DataGame = (Game)fastObjectListView1.SelectedObject;
				_selectedGame = DataGame;
				lbl_GameTitle.Text = DataGame.Name;

				if (_selectedGame.TestMenuIsExecutable) cmb_testMode.Enabled = true;


				int gpuResolution = ConfigurationManager.MainConfig.gpuResolution;
				int displayMode = ConfigurationManager.MainConfig.displayMode;
				bool patchReshade = ConfigurationManager.MainConfig.patchReshade;

				string gpuResolutionSource = "Global";
				string displayModeSource = "Global";
				string patchReshadeSource = "Global";
				_selectedGameSettings = null;
				string optionFile = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "gameoptions", Path.GetFileNameWithoutExtension(DataGame.UserConfigFile) + ".json");
				if (File.Exists(optionFile))
				{
					_selectedGameSettings = new GameSettings(Utils.ReadAllText(optionFile));
					gpuResolution = _selectedGameSettings.gpuResolution > 0 ? (_selectedGameSettings.gpuResolution - 1) : gpuResolution;
					displayMode = _selectedGameSettings.displayMode > 0 ? (_selectedGameSettings.displayMode - 1) : displayMode;
					patchReshade = _selectedGameSettings.patchReshade > 0 ? (_selectedGameSettings.patchReshade == 1 ? true : false) : patchReshade;
					if (_selectedGameSettings.gpuResolution > 0) gpuResolutionSource = "GameOption";
					if (_selectedGameSettings.displayMode > 0) displayModeSource = "GameOption";
					if (_selectedGameSettings.patchReshade > 0) patchReshadeSource = "GameOption";
				}

				if (displayMode == 0) cmb_displayMode.Items[0] = "Recommanded" + $" ({displayModeSource})";
				if (displayMode == 1) cmb_displayMode.Items[0] = "Fullscreen" + $" ({displayModeSource})";
				if (displayMode == 2) cmb_displayMode.Items[0] = "Windowed" + $" ({displayModeSource})";

				if (gpuResolution == 0) cmb_resolution.Items[0] = "720p" + $" ({gpuResolutionSource})";
				if (gpuResolution == 1) cmb_resolution.Items[0] = "1080p" + $" ({gpuResolutionSource})";
				if (gpuResolution == 2) cmb_resolution.Items[0] = "2k" + $" ({gpuResolutionSource})";
				if (gpuResolution == 3) cmb_resolution.Items[0] = "4k" + $" ({gpuResolutionSource})";
				if (gpuResolution == 4) cmb_resolution.Items[0] = "Native" + $" ({gpuResolutionSource})";

				cmb_patchReshade.Items[0] = (patchReshade ? "Yes" : "No") + $" ({patchReshadeSource})";

				string basePath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);

				cmb_resolution.Visible = true;
				cmb_nativeRes.Visible = false;
				cmb_nativeRes.SelectedIndex = 0;
				_selectedGameOption = new Dictionary<string, string>();
				_selectedGameInfo = new Dictionary<string, string>();
				var infoFile = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "config", Path.GetFileNameWithoutExtension(_selectedGame.UserConfigFile) + ".info.json");
				bool upscaleFullscreen = true;
				bool upscaleWindowed = true;
				string originalRes = "";
				if (File.Exists(infoFile))
				{
					try
					{
						JObject gameInfoParsedJson = JObject.Parse(Utils.ReadAllText(infoFile));
						JObject gameInfoGlobalSection = (JObject)gameInfoParsedJson["global"];
						_selectedGameOption = gameInfoGlobalSection.ToObject<Dictionary<string, string>>();
						{
							string WindowedString = "";
							if (_selectedGameOption.ContainsKey("windowed") && _selectedGameOption["windowed"].Trim() != "") WindowedString = _selectedGameOption["windowed"].Trim().ToLower();
							JObject tpInfoGlobalSection = (JObject)gameInfoParsedJson["tpoptions"];
							var allSettings = tpInfoGlobalSection.ToObject<Dictionary<string, Dictionary<string, string>>>();
							if (allSettings.ContainsKey("set_displaymode_recommanded"))
							{
								int found_recommanded_display = -1;
								if (allSettings["set_displaymode_recommanded"].ContainsKey("General||RegAsWindowed"))
								{
									if (allSettings["set_displaymode_recommanded"]["General||RegAsWindowed"] == "0") found_recommanded_display = 0;
									if (allSettings["set_displaymode_recommanded"]["General||RegAsWindowed"] == "1") found_recommanded_display = 1;
								}
								if (found_recommanded_display == -1)
								{
									string TargetWindowedString = allSettings["set_displaymode_recommanded"].First().Key.Trim().ToLower().Replace("||", ",") + "," + allSettings["set_displaymode_recommanded"].First().Value.Trim().ToLower();
									if (TargetWindowedString == WindowedString)
									{
										if (displayMode == 0) found_recommanded_display = 1;

									}
									else
									{
										if (displayMode == 0) found_recommanded_display = 0;
									}

								}
								if (found_recommanded_display != -1)
								{

									if (found_recommanded_display == 0) cmb_displayMode.Items[0] = "Recommanded FS" + $" ({displayModeSource})";
									if (found_recommanded_display == 1) cmb_displayMode.Items[0] = "Recommanded Windowed" + $" ({displayModeSource})";
								}


							}

						}


						_selectedGameInfo = _selectedGameOption;
						if (_selectedGameOption.ContainsKey("upscaleWindowed") && _selectedGameOption["upscaleWindowed"].ToLower() == "false") upscaleWindowed = false;
						if (_selectedGameOption.ContainsKey("upscaleFullscreen") && _selectedGameOption["upscaleFullscreen"].ToLower() == "false") upscaleFullscreen = false;
						if (_selectedGameOption.ContainsKey("originalRes") && _selectedGameOption["originalRes"] != "") originalRes = _selectedGameOption["originalRes"];
						if (originalRes != "")
						{
							cmb_resolution.Items[5] = "Native" + $" ({originalRes})";
							cmb_nativeRes.Items[0] = "Native" + $" ({originalRes})";
							if (cmb_resolution.Items[0].ToString().StartsWith("Native"))
							{
								string newValNative = cmb_resolution.Items[0].ToString();
								newValNative = newValNative.Replace("Native", "Native : " + originalRes);
								cmb_resolution.Items[0] = newValNative;
							}
						}
						else
						{
							cmb_resolution.Items[5] = "Native";
							cmb_nativeRes.Items[0] = "Native";
						}
						if (cmb_displayMode.SelectedIndex == 0)
						{
							if (displayMode == 0)
							{
								if (!upscaleFullscreen && !upscaleWindowed)
								{
									cmb_nativeRes.Visible = true;
									cmb_resolution.Visible = false;
								}
							}
							if (displayMode == 1)
							{
								if (!upscaleFullscreen)
								{
									cmb_nativeRes.Visible = true;
									cmb_resolution.Visible = false;
								}
							}
							if (displayMode == 2)
							{
								if (!upscaleWindowed)
								{
									cmb_nativeRes.Visible = true;
									cmb_resolution.Visible = false;
								}
							}
						}
						if (cmb_displayMode.SelectedIndex == 1)
						{
							if (!upscaleFullscreen && !upscaleWindowed)
							{
								cmb_nativeRes.Visible = true;
								cmb_resolution.Visible = false;
							}
						}
						if (cmb_displayMode.SelectedIndex == 2)
						{
							if (!upscaleFullscreen)
							{
								cmb_nativeRes.Visible = true;
								cmb_resolution.Visible = false;
							}
						}
						if (cmb_displayMode.SelectedIndex == 3)
						{
							if (!upscaleWindowed)
							{
								cmb_nativeRes.Visible = true;
								cmb_resolution.Visible = false;
							}
						}




						if (_selectedGameOption.ContainsKey("showGameOptionPerf") && (_selectedGameOption["showGameOptionPerf"].ToLower() == "true" || _selectedGameOption["showGameOptionPerf"].ToLower() == "high"))
						{
							fpanel_perf.BorderStyle = BorderStyle.FixedSingle;
							int perf = ConfigurationManager.MainConfig.performanceProfile;
							if (_selectedGameSettings != null)
							{
								if (_selectedGameSettings.performanceProfile > 0) fpanel_perf.BorderStyle = BorderStyle.Fixed3D;
								perf = _selectedGameSettings.performanceProfile > 0 ? (_selectedGameSettings.performanceProfile - 1) : perf;
							}
							if (_selectedGameOption["showGameOptionPerf"].ToLower() != "high")
							{
								if (perf == 2) perf = 0;
								lbl_perf_high.Visible = false;
							}
							else lbl_perf_high.Visible = true;

							lbl_perf_normal.ForeColor = Color.DarkGray;
							lbl_perf_low.ForeColor = Color.DarkGray;
							lbl_perf_high.ForeColor = Color.DarkGray;

							if (perf == 0) lbl_perf_normal.ForeColor = Color.DarkGreen;
							if (perf == 1) lbl_perf_low.ForeColor = Color.DarkOrange;
							if (perf == 2) lbl_perf_high.ForeColor = Color.DarkRed;

							fpanel_perf.Visible = true;
						}

						if (_selectedGameOption.ContainsKey("showGameOptionFFB") && _selectedGameOption["showGameOptionFFB"].ToLower() == "true")
						{
							fpanel_ffb.BorderStyle = BorderStyle.FixedSingle;
							bool patchFFB = ConfigurationManager.MainConfig.patch_FFB;
							if (_selectedGameSettings != null)
							{
								if (_selectedGameSettings.patchFFB > 0) fpanel_ffb.BorderStyle = BorderStyle.Fixed3D;
								patchFFB = _selectedGameSettings.patchFFB > 0 ? (_selectedGameSettings.patchFFB == 1 ? true : false) : patchFFB;
							}

							if (patchFFB)
							{
								lbl_ffb.ForeColor = Color.DarkGreen;
								lbl_ffb.Text = "ON";
							}
							else
							{
								lbl_ffb.ForeColor = Color.Red;
								lbl_ffb.Text = "OFF";
							}
							fpanel_ffb.Visible = true;
						}
						if (_selectedGameOption.ContainsKey("showGameOptionCrt") && _selectedGameOption["showGameOptionCrt"].ToLower() == "true")
						{
							fpanel_crt.BorderStyle = BorderStyle.FixedSingle;
							bool useCrt = ConfigurationManager.MainConfig.useCrt;
							if (_selectedGameSettings != null)
							{
								if (_selectedGameSettings.useCrt > 0) fpanel_crt.BorderStyle = BorderStyle.Fixed3D;
								useCrt = _selectedGameSettings.useCrt > 0 ? (_selectedGameSettings.useCrt == 1 ? true : false) : useCrt;
							}
							if (useCrt)
							{
								lbl_crt.ForeColor = Color.DarkGreen;
								lbl_crt.Text = "ON";
							}
							else
							{
								lbl_crt.ForeColor = Color.Red;
								lbl_crt.Text = "OFF";
							}
							fpanel_crt.Visible = true;
							fpanel_crt.Enabled = true;
							if (_selectedGameOption.ContainsKey("disableCrtForLowPerf") && _selectedGameOption["disableCrtForLowPerf"].ToLower() == "true")
							{
								int perf = _selectedGameSettings.performanceProfile > 0 ? (_selectedGameSettings.performanceProfile - 1) : ConfigurationManager.MainConfig.performanceProfile;
								if (perf == 1)
								{
									fpanel_crt.Enabled = false;
									lbl_crt.Text = "DISABLED";
								}
							}

						}
						if (_selectedGameOption.ContainsKey("showGameOptionBezel") && _selectedGameOption["showGameOptionBezel"].ToLower() == "true")
						{
							fpanel_bezel.BorderStyle = BorderStyle.FixedSingle;
							bool useBezel = ConfigurationManager.MainConfig.useBezel;
							if (_selectedGameSettings != null)
							{
								if (_selectedGameSettings.useBezel > 0) fpanel_bezel.BorderStyle = BorderStyle.Fixed3D;
								useBezel = _selectedGameSettings.useBezel > 0 ? (_selectedGameSettings.useBezel == 1 ? true : false) : useBezel;
							}
							if (useBezel)
							{
								lbl_bezel.ForeColor = Color.DarkGreen;
								lbl_bezel.Text = "ON";
							}
							else
							{
								lbl_bezel.ForeColor = Color.Red;
								lbl_bezel.Text = "OFF";
							}
							fpanel_bezel.Visible = true;
						}
						if (_selectedGameOption.ContainsKey("showGameOptionVsync") && _selectedGameOption["showGameOptionVsync"].ToLower() == "true")
						{
							fpanel_vsync.BorderStyle = BorderStyle.FixedSingle;
							bool forceVsync = ConfigurationManager.MainConfig.forceVsync;
							if (_selectedGameSettings != null)
							{
								if (_selectedGameSettings.forceVsync > 0) fpanel_vsync.BorderStyle = BorderStyle.Fixed3D;
								forceVsync = _selectedGameSettings.forceVsync > 0 ? (_selectedGameSettings.patchDisplayModeFix == 1 ? true : false) : forceVsync;
							}
							if (forceVsync)
							{
								lbl_vsync.ForeColor = Color.DarkGreen;
								lbl_vsync.Text = "ON";
							}
							else
							{
								lbl_vsync.ForeColor = Color.Red;
								lbl_vsync.Text = "OFF";
							}
							fpanel_vsync.Visible = true;
						}
						if (_selectedGameOption.ContainsKey("showGameOptionKeepAspectRatio") && _selectedGameOption["showGameOptionKeepAspectRatio"].ToLower() == "true")
						{
							fpanel_aspectratio.BorderStyle = BorderStyle.FixedSingle;
							bool useKeepAspectRatio = ConfigurationManager.MainConfig.keepAspectRatio;
							if (_selectedGameSettings != null)
							{
								if (_selectedGameSettings.keepAspectRatio > 0) fpanel_aspectratio.BorderStyle = BorderStyle.Fixed3D;
								useKeepAspectRatio = _selectedGameSettings.keepAspectRatio > 0 ? (_selectedGameSettings.keepAspectRatio == 1 ? true : false) : useKeepAspectRatio;
							}
							if (useKeepAspectRatio)
							{
								lbl_aspectratio.ForeColor = Color.DarkGreen;
								lbl_aspectratio.Text = "ON";
							}
							else
							{
								lbl_aspectratio.ForeColor = Color.Red;
								lbl_aspectratio.Text = "OFF";
							}
							fpanel_aspectratio.Visible = true;
						}
						/*
						if (_selectedGameOption.ContainsKey("showGameOptionPatchLang") && _selectedGameOption["showGameOptionPatchLang"].ToLower() != "false")
						{
							fpanel_translation.BorderStyle = BorderStyle.FixedSingle;
							int patchLang = ConfigurationManager.MainConfig.patchLang;
							if (_selectedGameSettings != null)
							{
								if (_selectedGameSettings.patchLang > 0) fpanel_translation.BorderStyle = BorderStyle.Fixed3D;
								patchLang = _selectedGameSettings.patchLang > 0 ? (_selectedGameSettings.patchLang - 1) : patchLang;
							}
							if (patchLang == 0)
							{
								lbl_translation_none.ForeColor = Color.Red;
								lbl_translation_none.Text = "NONE";
							}
							if (patchLang == 1)
							{
								if (_selectedGameOption["showGameOptionPatchLang"].ToLower() == "french")
								{
									lbl_translation_none.ForeColor = Color.Red;
									lbl_translation_none.Text = "NONE";
								}
								else
								{
									lbl_translation_none.ForeColor = Color.DarkGreen;
									lbl_translation_none.Text = "ENGLISH";
								}
							}
							if (patchLang == 2)
							{
								if (_selectedGameOption["showGameOptionPatchLang"].ToLower() == "english")
								{
									lbl_translation_none.ForeColor = Color.DarkGreen;
									lbl_translation_none.Text = "ENGLISH";
								}
								else
								{
									lbl_translation_none.ForeColor = Color.DarkBlue;
									lbl_translation_none.Text = "FRENCH";
								}

							}
							fpanel_translation.Visible = true;
						}
						*/
						if (_selectedGameOption.ContainsKey("showGameOptionPatchLang") && _selectedGameOption["showGameOptionPatchLang"].ToLower() != "false")
						{
							fpanel_translation.BorderStyle = BorderStyle.FixedSingle;
							lbl_translation_english.Visible = true;
							lbl_translation_none.Visible = true;
							lbl_translation_french.Visible = true;

							int patchLang = ConfigurationManager.MainConfig.patchLang;
							if (_selectedGameSettings != null)
							{
								if (_selectedGameSettings.patchLang > 0) fpanel_translation.BorderStyle = BorderStyle.Fixed3D;
								patchLang = _selectedGameSettings.patchLang > 0 ? (_selectedGameSettings.patchLang - 1) : patchLang;
							}
							if (_selectedGameOption["showGameOptionPatchLang"].ToLower() == "french") lbl_translation_english.Visible = false;
							if (_selectedGameOption["showGameOptionPatchLang"].ToLower() == "english") lbl_translation_french.Visible = false;

							lbl_translation_none.ForeColor = Color.DarkGray;
							lbl_translation_english.ForeColor = Color.DarkGray;
							lbl_translation_french.ForeColor = Color.DarkGray;

							if (patchLang == 1 && (_selectedGameOption["showGameOptionPatchLang"].ToLower() == "french")) patchLang = 0;
							if (patchLang == 2 && (_selectedGameOption["showGameOptionPatchLang"].ToLower() == "english")) patchLang = 1;

							if (patchLang == 0) lbl_translation_none.ForeColor = Color.DarkRed;
							if (patchLang == 1) lbl_translation_english.ForeColor = Color.DarkGreen;
							if (patchLang == 2) lbl_translation_french.ForeColor = Color.DarkGreen;

							fpanel_translation.Visible = true;
						}
						if (_selectedGameOption.ContainsKey("canUseJoyInsteadOfDpad") && _selectedGameOption["canUseJoyInsteadOfDpad"].ToLower() == "true")
						{
							fpanel_joystick.BorderStyle = BorderStyle.FixedSingle;
							bool favorJoystick = ConfigurationManager.MainConfig.favorJoystick;
							if (_selectedGameSettings != null)
							{
								if (_selectedGameSettings.favorJoystick > 0) fpanel_joystick.BorderStyle = BorderStyle.Fixed3D;
								favorJoystick = _selectedGameSettings.favorJoystick > 0 ? (_selectedGameSettings.favorJoystick == 1 ? true : false) : favorJoystick;
							}
							if (favorJoystick)
							{
								lbl_joystick.ForeColor = Color.DarkGreen;
								lbl_joystick.Text = "ON";
							}
							else
							{
								lbl_joystick.ForeColor = Color.Red;
								lbl_joystick.Text = "OFF";
							}
							fpanel_joystick.Visible = true;
						}

						/*
						if (_selectedGameOption.ContainsKey("nativeCrosshair") && _selectedGameOption["nativeCrosshair"].ToLower() == "true" && (_dinputLightgunAFound || _dinputLightgunBFound))
						{
							if (_selectedGame.existingConfig.ContainsKey("lightgun") && (_dinputLightgunAFound || _dinputLightgunBFound))
							{
								bool hideCrosshair = true;
								fpanel_nativeCrosshair.BorderStyle = BorderStyle.FixedSingle;
								bool gunACrosshair = false;
								if (_dinputLightgunAFound)
								{
									gunACrosshair = ConfigurationManager.MainConfig.gunACrosshair;
									if (_selectedGameSettings != null)
									{
										if (_selectedGameSettings.gunA_crosshair > 0)
										{
											fpanel_nativeCrosshair.BorderStyle = BorderStyle.Fixed3D;
											if (_selectedGameSettings.gunA_crosshair == 1) gunACrosshair = true;
											if (_selectedGameSettings.gunA_crosshair == 2) gunACrosshair = false;
										}
									}
								}
								bool gunBCrosshair = false;
								if (_dinputLightgunBFound)
								{
									gunBCrosshair = ConfigurationManager.MainConfig.gunBCrosshair;
									if (_selectedGameSettings != null)
									{
										if (_selectedGameSettings.gunB_crosshair > 0)
										{
											fpanel_nativeCrosshair.BorderStyle = BorderStyle.Fixed3D;
											if (_selectedGameSettings.gunB_crosshair == 1) gunBCrosshair = true;
											if (_selectedGameSettings.gunB_crosshair == 2) gunBCrosshair = false;
										}
									}
								}

								if (_dinputLightgunAFound && gunACrosshair) hideCrosshair = false;
								if (_dinputLightgunBFound && gunBCrosshair) hideCrosshair = false;




								if (!hideCrosshair)
								{
									lbl_nativeCrosshair.ForeColor = Color.DarkGreen;
									lbl_nativeCrosshair.Text = "ON";
								}
								else
								{
									lbl_nativeCrosshair.ForeColor = Color.Red;
									lbl_nativeCrosshair.Text = "OFF";
								}
								fpanel_nativeCrosshair.Visible = true;
							}

						}
						else
						{

							string linkSourceFolderExe = "";
							if (_selectedGameSettings != null && _selectedGameSettings.CustomPerGameLinkFolder != null && _selectedGameSettings.CustomPerGameLinkFolder != "")
							{
								string lastFolder = Path.GetFileName(_selectedGameSettings.CustomPerGameLinkFolder);
								if (lastFolder == Path.GetFileNameWithoutExtension(_selectedGame.FileName))
								{
									linkSourceFolderExe = _selectedGameSettings.CustomPerGameLinkFolder;
								}
							}
							else
							{
								linkSourceFolderExe = Path.Combine(ConfigurationManager.MainConfig.perGameLinkFolderExe, Path.GetFileNameWithoutExtension(_selectedGame.FileName));
							}
							if (linkSourceFolderExe != "" && Directory.Exists(Path.Combine(linkSourceFolderExe, "[!!crosshair!!]", "[!crosshair_gun1_and_gun2!]")))
							{
								if (_selectedGame.existingConfig.ContainsKey("lightgun") && _dinputLightgunAFound)
								{
									fpanel_cross1.BorderStyle = BorderStyle.FixedSingle;
									bool gunACrosshair = ConfigurationManager.MainConfig.gunACrosshair;
									if (_selectedGameSettings != null)
									{
										if (_selectedGameSettings.gunA_crosshair > 0)
										{
											fpanel_cross1.BorderStyle = BorderStyle.Fixed3D;
											if (_selectedGameSettings.gunA_crosshair == 1) gunACrosshair = true;
											if (_selectedGameSettings.gunA_crosshair == 2) gunACrosshair = false;
										}
									}
									if (gunACrosshair)
									{
										lbl_cross1.ForeColor = Color.DarkGreen;
										lbl_cross1.Text = "ON";
									}
									else
									{
										lbl_cross1.ForeColor = Color.Red;
										lbl_cross1.Text = "OFF";
									}
									fpanel_cross1.Visible = true;
								}
								if (_selectedGame.existingConfig.ContainsKey("lightgun") && _dinputLightgunAFound && _dinputLightgunBFound)
								{
									fpanel_cross2.BorderStyle = BorderStyle.FixedSingle;
									bool gunBCrosshair = ConfigurationManager.MainConfig.gunBCrosshair;
									if (_selectedGameSettings != null)
									{
										if (_selectedGameSettings.gunA_crosshair > 0)
										{
											fpanel_cross2.BorderStyle = BorderStyle.Fixed3D;
											if (_selectedGameSettings.gunB_crosshair == 1) gunBCrosshair = true;
											if (_selectedGameSettings.gunB_crosshair == 2) gunBCrosshair = false;
										}
									}
									if (gunBCrosshair)
									{
										lbl_cross2.ForeColor = Color.DarkGreen;
										lbl_cross2.Text = "ON";
									}
									else
									{
										lbl_cross2.ForeColor = Color.Red;
										lbl_cross2.Text = "OFF";
									}
									fpanel_cross2.Visible = true;
								}
							}
						}
						*/
						if (_selectedGame.existingConfig.ContainsKey("lightgun") && (_dinputLightgunAFound || _dinputLightgunBFound))
						{
							string linkSourceFolderExe = "";
							if (_selectedGameSettings != null && _selectedGameSettings.CustomPerGameLinkFolder != null && _selectedGameSettings.CustomPerGameLinkFolder != "")
							{
								string lastFolder = Path.GetFileName(_selectedGameSettings.CustomPerGameLinkFolder);
								if (lastFolder == Path.GetFileNameWithoutExtension(_selectedGame.FileName))
								{
									linkSourceFolderExe = _selectedGameSettings.CustomPerGameLinkFolder;
								}
							}
							else
							{
								linkSourceFolderExe = Path.Combine(ConfigurationManager.MainConfig.perGameLinkFolderExe, Path.GetFileNameWithoutExtension(_selectedGame.FileName));
							}
							if (linkSourceFolderExe != "" && Directory.Exists(Path.Combine(linkSourceFolderExe, "[!!crosshair!!]", "[!crosshair_gun1_and_gun2!]")))
							{
								if (_selectedGame.existingConfig.ContainsKey("lightgun") && _dinputLightgunAFound)
								{
									fpanel_cross1.BorderStyle = BorderStyle.FixedSingle;
									bool gunACrosshair = ConfigurationManager.MainConfig.gunACrosshair;
									if (_selectedGameSettings != null)
									{
										if (_selectedGameSettings.gunA_crosshair > 0)
										{
											fpanel_cross1.BorderStyle = BorderStyle.Fixed3D;
											if (_selectedGameSettings.gunA_crosshair == 1) gunACrosshair = true;
											if (_selectedGameSettings.gunA_crosshair == 2) gunACrosshair = false;
										}
									}
									if (gunACrosshair)
									{
										lbl_cross1.ForeColor = Color.DarkGreen;
										lbl_cross1.Text = "ON";
									}
									else
									{
										lbl_cross1.ForeColor = Color.Red;
										lbl_cross1.Text = "OFF";
									}
									fpanel_cross1.Visible = true;
								}
								if (_selectedGame.existingConfig.ContainsKey("lightgun") && _dinputLightgunAFound && _dinputLightgunBFound)
								{
									fpanel_cross2.BorderStyle = BorderStyle.FixedSingle;
									bool gunBCrosshair = ConfigurationManager.MainConfig.gunBCrosshair;
									if (_selectedGameSettings != null)
									{
										if (_selectedGameSettings.gunA_crosshair > 0)
										{
											fpanel_cross2.BorderStyle = BorderStyle.Fixed3D;
											if (_selectedGameSettings.gunB_crosshair == 1) gunBCrosshair = true;
											if (_selectedGameSettings.gunB_crosshair == 2) gunBCrosshair = false;
										}
									}
									if (gunBCrosshair)
									{
										lbl_cross2.ForeColor = Color.DarkGreen;
										lbl_cross2.Text = "ON";
									}
									else
									{
										lbl_cross2.ForeColor = Color.Red;
										lbl_cross2.Text = "OFF";
									}
									fpanel_cross2.Visible = true;
								}

							}
							else
							{
								if (_selectedGame.existingConfig.ContainsKey("lightgun") && (_dinputLightgunAFound || _dinputLightgunBFound))
								{
									bool hideCrosshair = true;
									fpanel_nativeCrosshair.BorderStyle = BorderStyle.FixedSingle;
									bool gunACrosshair = false;
									if (_dinputLightgunAFound)
									{
										gunACrosshair = ConfigurationManager.MainConfig.gunACrosshair;
										if (_selectedGameSettings != null)
										{
											if (_selectedGameSettings.gunA_crosshair > 0)
											{
												fpanel_nativeCrosshair.BorderStyle = BorderStyle.Fixed3D;
												if (_selectedGameSettings.gunA_crosshair == 1) gunACrosshair = true;
												if (_selectedGameSettings.gunA_crosshair == 2) gunACrosshair = false;
											}
										}
									}
									bool gunBCrosshair = false;
									if (_dinputLightgunBFound)
									{
										gunBCrosshair = ConfigurationManager.MainConfig.gunBCrosshair;
										if (_selectedGameSettings != null)
										{
											if (_selectedGameSettings.gunB_crosshair > 0)
											{
												fpanel_nativeCrosshair.BorderStyle = BorderStyle.Fixed3D;
												if (_selectedGameSettings.gunB_crosshair == 1) gunBCrosshair = true;
												if (_selectedGameSettings.gunB_crosshair == 2) gunBCrosshair = false;
											}
										}
									}

									if (_dinputLightgunAFound && gunACrosshair) hideCrosshair = false;
									if (_dinputLightgunBFound && gunBCrosshair) hideCrosshair = false;




									if (!hideCrosshair)
									{
										lbl_nativeCrosshair.ForeColor = Color.DarkGreen;
										lbl_nativeCrosshair.Text = "ON";
									}
									else
									{
										lbl_nativeCrosshair.ForeColor = Color.Red;
										lbl_nativeCrosshair.Text = "OFF";
									}
									fpanel_nativeCrosshair.Visible = true;
								}

							}

						}




					}
					catch { }


				}

				{
					fpanel_overrideTpController.BorderStyle = BorderStyle.FixedSingle;
					bool overrideTpController = false;
					if (_selectedGame.isSupported)
					{
						overrideTpController = true;
						if (_selectedGameSettings != null)
						{
							overrideTpController = _selectedGameSettings.overrideTpController;
							if (!_selectedGameSettings.overrideTpController) fpanel_overrideTpController.BorderStyle = BorderStyle.Fixed3D;
						}
					}
					if (overrideTpController)
					{
						lbl_overrideTpController.ForeColor = Color.DarkGreen;
						lbl_overrideTpController.Text = "ON";
						btn_playgame.Text = "PLAY GAME !";
					}
					else
					{
						lbl_overrideTpController.ForeColor = Color.Red;
						lbl_overrideTpController.Text = "OFF";
						btn_playgame.Text = "PLAY GAME ! (NO AUTO CONTROLS !)";

					}
				}
				{
					fpanel_overrideTpGameSettings.BorderStyle = BorderStyle.FixedSingle;
					bool overrideTpGameSettings = false;
					if (_selectedGame.InfoFile != "")
					{
						overrideTpGameSettings = true;
						if (_selectedGameSettings != null)
						{
							overrideTpGameSettings = _selectedGameSettings.overrideTpGameSettings;
							if (!_selectedGameSettings.overrideTpGameSettings) fpanel_overrideTpGameSettings.BorderStyle = BorderStyle.Fixed3D;
						}
					}
					if (overrideTpGameSettings)
					{
						lbl_overrideTpGameSettings.ForeColor = Color.DarkGreen;
						lbl_overrideTpGameSettings.Text = "ON";
					}
					else
					{
						lbl_overrideTpGameSettings.ForeColor = Color.Red;
						lbl_overrideTpGameSettings.Text = "OFF";
					}
				}
				{
					fpanel_applyPatches.BorderStyle = BorderStyle.FixedSingle;
					bool applyPatches = true;
					if (_selectedGameSettings != null)
					{
						applyPatches = _selectedGameSettings.applyPatches;
						if (!_selectedGameSettings.applyPatches) fpanel_applyPatches.BorderStyle = BorderStyle.Fixed3D;
					}
					if (applyPatches)
					{
						lbl_applyPatches.ForeColor = Color.DarkGreen;
						lbl_applyPatches.Text = "ON";
					}
					else
					{
						lbl_applyPatches.ForeColor = Color.Red;
						lbl_applyPatches.Text = "OFF";
					}
				}
				{
					fpanel_useThirdParty.BorderStyle = BorderStyle.FixedSingle;
					bool useThirdParty = true;
					if (_selectedGameSettings != null)
					{
						useThirdParty = _selectedGameSettings.useThirdParty;
						if (!_selectedGameSettings.useThirdParty) fpanel_useThirdParty.BorderStyle = BorderStyle.Fixed3D;
					}
					if (useThirdParty)
					{
						lbl_useThirdParty.ForeColor = Color.DarkGreen;
						lbl_useThirdParty.Text = "ON";
					}
					else
					{
						lbl_useThirdParty.ForeColor = Color.Red;
						lbl_useThirdParty.Text = "OFF";
					}
				}


				bool uselightgun = false;

				bool usealtgamepad = false;
				bool favorAB = ConfigurationManager.MainConfig.favorAB;
				if (favorAB && DataGame.existingConfig.ContainsKey("gamepadalt") && DataGame.existingConfig.ContainsKey("wheel")) usealtgamepad = true;

				bool useXinput = true;
				bool useDinputWheel = false;
				bool useDinputHotas = false;
				if (_haveWheel && DataGame.existingConfig.ContainsKey("wheel") && _dinputWheelFound)
				{
					useXinput = false;
					useDinputWheel = true;
				}
				if (_haveHotas && DataGame.existingConfig.ContainsKey("hotas") && _dinputHotasFound)
				{
					useXinput = false;
					useDinputWheel = false;
					useDinputHotas = true;
				}

				Dictionary<int, (string, XinputGamepad)> ConfigPerPlayer = new Dictionary<int, (string, XinputGamepad)>();
				if (useXinput)
				{
					if (_haveWheel && DataGame.existingConfig.ContainsKey("wheel"))
					{
						var joystickButtonWheel = Program.ParseConfig(DataGame.existingConfig["wheel"]);
						var PlayerList = Program.GetPlayersList(joystickButtonWheel);
						int nb_wheel = _connectedGamePad.Values.Where(c => c.Type == "wheel").Count();
						int currentlyAttributed = 0;
						List<XinputGamepad> gamepadList = new List<XinputGamepad>();
						foreach (var cgp in _connectedGamePad.Values)
						{
							if (cgp.Type == "wheel")
							{
								gamepadList.Add(cgp);
							}
						}
						foreach (var PlayerXinputSlot in PlayerList)
						{
							if (currentlyAttributed < nb_wheel)
							{
								if (!ConfigPerPlayer.ContainsKey(PlayerXinputSlot))
								{
									ConfigPerPlayer.Add(PlayerXinputSlot, ("wheel", gamepadList[currentlyAttributed]));
									currentlyAttributed++;
								}
							}
						}
					}
					int debugtest = 3;
					if (_haveArcade && DataGame.existingConfig.ContainsKey("arcade") && (!_selectedGameInfo.ContainsKey("priorityGamepadOverArcade") || _selectedGameInfo["priorityGamepadOverArcade"].Trim().ToLower() != "true"))
					{
						var joystickButtonArcade = Program.ParseConfig(DataGame.existingConfig["arcade"]);
						var PlayerList = Program.GetPlayersList(joystickButtonArcade);
						int nb_arcade = _connectedGamePad.Values.Where(c => c.Type == "arcade").Count();
						int currentlyAttributed = 0;
						List<XinputGamepad> gamepadList = new List<XinputGamepad>();
						foreach (var cgp in _connectedGamePad.Values)
						{
							if (cgp.Type == "arcade")
							{
								gamepadList.Add(cgp);
							}
						}

						foreach (var PlayerXinputSlot in PlayerList)
						{
							if (currentlyAttributed < nb_arcade)
							{
								if (!ConfigPerPlayer.ContainsKey(PlayerXinputSlot))
								{
									ConfigPerPlayer.Add(PlayerXinputSlot, ("arcade", gamepadList[currentlyAttributed]));
									currentlyAttributed++;
								}
							}
						}
					}
					if (_haveGamepad && (DataGame.existingConfig.ContainsKey("gamepad") || DataGame.existingConfig.ContainsKey("lightgun")))
					{
						string configname = "gamepad";
						if (usealtgamepad) configname = "gamepadalt";
						if (DataGame.existingConfig.ContainsKey("lightgun"))
						{
							uselightgun = true;
							configname = "lightgun";
						}

						var joystickButtonGamepad = Program.ParseConfig(DataGame.existingConfig[configname]);
						var PlayerList = Program.GetPlayersList(joystickButtonGamepad);
						int nb_gamepad = _connectedGamePad.Values.Where(c => c.Type == "gamepad").Count();
						int currentlyAttributed = 0;
						List<XinputGamepad> gamepadList = new List<XinputGamepad>();
						foreach (var cgp in _connectedGamePad.Values)
						{
							if (cgp.Type == "gamepad")
							{
								gamepadList.Add(cgp);
							}
						}

						foreach (var PlayerXinputSlot in PlayerList)
						{
							if (currentlyAttributed < nb_gamepad)
							{
								if (!ConfigPerPlayer.ContainsKey(PlayerXinputSlot))
								{
									ConfigPerPlayer.Add(PlayerXinputSlot, ("gamepad", gamepadList[currentlyAttributed]));
									currentlyAttributed++;
								}
							}
						}
					}
					if (_haveArcade && DataGame.existingConfig.ContainsKey("arcade") && _selectedGameInfo.ContainsKey("priorityGamepadOverArcade") && _selectedGameInfo["priorityGamepadOverArcade"].Trim().ToLower() == "true")
					{
						var joystickButtonArcade = Program.ParseConfig(DataGame.existingConfig["arcade"]);
						var PlayerList = Program.GetPlayersList(joystickButtonArcade);
						int nb_arcade = _connectedGamePad.Values.Where(c => c.Type == "arcade").Count();
						int currentlyAttributed = 0;
						List<XinputGamepad> gamepadList = new List<XinputGamepad>();
						foreach (var cgp in _connectedGamePad.Values)
						{
							if (cgp.Type == "arcade")
							{
								gamepadList.Add(cgp);
							}
						}

						foreach (var PlayerXinputSlot in PlayerList)
						{
							if (currentlyAttributed < nb_arcade)
							{
								if (!ConfigPerPlayer.ContainsKey(PlayerXinputSlot))
								{
									ConfigPerPlayer.Add(PlayerXinputSlot, ("arcade", gamepadList[currentlyAttributed]));
									currentlyAttributed++;
								}
							}
						}
					}
				}
				else
				{
					if (useDinputWheel)
					{
						var joystickButtonWheel = Program.ParseConfig(DataGame.existingConfig["wheel"]);
						XinputGamepad xinputGamepad = new XinputGamepad(0);
						xinputGamepad.Type = "wheel";
						xinputGamepad.ControllerName = _dinputWheelName;
						ConfigPerPlayer.Add(0, ("wheel", xinputGamepad));
					}
					if (useDinputHotas)
					{
						var joystickButtonHotas = Program.ParseConfig(DataGame.existingConfig["hotas"]);
						XinputGamepad xinputGamepad = new XinputGamepad(0);
						xinputGamepad.Type = "hotas";
						xinputGamepad.ControllerName = _dinputHotasName;
						ConfigPerPlayer.Add(0, ("hotas", xinputGamepad));
					}
				}

				int currentcpp = 0;
				foreach (var cpp in ConfigPerPlayer)
				{
					currentcpp++;
					if (useXinput)
					{
						if (currentcpp == 1) lbl_player1.Text = "Player 1 : " + cpp.Value.Item1 + " -> " + "XINPUT " + (cpp.Value.Item2.XinputSlot + 1).ToString() + $" ({cpp.Value.Item2.ControllerName})";
						if (currentcpp == 2) lbl_player2.Text = "Player 2 : " + cpp.Value.Item1 + " -> " + "XINPUT " + (cpp.Value.Item2.XinputSlot + 1).ToString() + $" ({cpp.Value.Item2.ControllerName})";
						if (currentcpp == 3) lbl_player3.Text = "Player 3 : " + cpp.Value.Item1 + " -> " + "XINPUT " + (cpp.Value.Item2.XinputSlot + 1).ToString() + $" ({cpp.Value.Item2.ControllerName})";
						if (currentcpp == 4) lbl_player4.Text = "Player 4 : " + cpp.Value.Item1 + " -> " + "XINPUT " + (cpp.Value.Item2.XinputSlot + 1).ToString() + $" ({cpp.Value.Item2.ControllerName})";
					}
					else
					{
						if (useDinputWheel)
						{
							lbl_player1.Text = "Player 1 : " + cpp.Value.Item1 + " -> " + "DINPUT " + $" ({cpp.Value.Item2.ControllerName})";
						}
						if (useDinputHotas)
						{
							lbl_player1.Text = "Player 1 : " + cpp.Value.Item1 + " -> " + "DINPUT " + $" ({cpp.Value.Item2.ControllerName})";
						}
					}

					if (currentcpp == 4) break;
				}

				string FirstConfig = string.Empty;
				if (DataGame.existingConfig.ContainsKey("gamepad"))
				{
					FirstConfig = DataGame.existingConfig["gamepad"];
				}
				if (DataGame.existingConfig.ContainsKey("lightgun"))
				{
					FirstConfig = DataGame.existingConfig["lightgun"];
				}
				if (ConfigPerPlayer.Count() > 0)
				{
					var FirstPlayer = ConfigPerPlayer[0];
					string FirstConfigLabel = FirstPlayer.Item1;
					if (FirstConfigLabel == "gamepad" && usealtgamepad) FirstConfigLabel = "gamepadalt";
					if (FirstConfigLabel == "gamepad" && uselightgun) FirstConfigLabel = "lightgun";
					FirstConfig = DataGame.existingConfig[FirstConfigLabel];
				}



				if (FirstConfig != "" && File.Exists(FirstConfig))
				{
					btn_gameoptions.Enabled = true;
					btn_tpsettings.Enabled = true;
					_playAutoEnabled = true;
					_playDirectEnabled = true;
					isPlaying = isPlaying;
					{
						string fileName = FirstConfig;
						string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
						string fileDirectory = Path.GetDirectoryName(fileName);
						fileDirectory = Path.GetDirectoryName(fileDirectory);

						string imgFile = Path.Combine(fileDirectory, "img", fileNameWithoutExt + ".jpg");
						if (imgFile.EndsWith("lightgun.jpg"))
						{
							string oldImgFile = imgFile;
							if (_dinputGunAType != "")
							{
								imgFile = imgFile.Substring(0, imgFile.Length - 4) + "-" + _dinputGunAType + ".jpg";
								if (!File.Exists(imgFile)) { imgFile = oldImgFile; }
							}
						}

						if (File.Exists(imgFile))
						{
							Image originalImage = System.Drawing.Image.FromFile(imgFile);
							pictureBox_gameControls.Image = ResizeImageBest(originalImage, pictureBox_gameControls.Size);
						}
					}

					Dictionary<string, string> existingConfigClone = new Dictionary<string, string>();
					foreach (var cfg in DataGame.existingConfig) existingConfigClone.Add(cfg.Key, cfg.Value);
					if (DataGame.existingConfig.ContainsKey("lightgun"))
					{
						int sindenPump = ConfigurationManager.MainConfig.gunASidenPump;
						if (_selectedGameSettings != null)
						{
							if (_selectedGameSettings.gunA_pump > 0) sindenPump = _selectedGameSettings.gunA_pump;
						}





						string valueExistingConfig = DataGame.existingConfig["lightgun"].Substring(0, DataGame.existingConfig["lightgun"].Length - 4);
						string fileDirectory = Path.GetDirectoryName(valueExistingConfig);
						fileDirectory = Path.GetDirectoryName(fileDirectory);
						string imgFile = Path.Combine(fileDirectory, "img", Path.GetFileName(DataGame.existingConfig["lightgun"].Substring(0, DataGame.existingConfig["lightgun"].Length - 4)));


						existingConfigClone.Add("lightgun-sinden", imgFile + "-sinden" + sindenPump + ".jpg");
						existingConfigClone.Add("lightgun-wiimote", imgFile + "-wiimote.jpg");
						existingConfigClone.Add("lightgun-gamepad", imgFile + "-gamepad.jpg");
						existingConfigClone.Add("lightgun-guncon1", imgFile + "-guncon1.jpg");
						existingConfigClone.Add("lightgun-guncon2", imgFile + "-guncon2.jpg");



						if (_haveLightgun && _dinputLightgunAFound && existingConfigClone.ContainsKey("lightgun-" + _dinputGunAType))
						{
							lbl_player1.Text = "";
							lbl_player2.Text = "";
							lbl_player3.Text = "";
							lbl_player4.Text = "";
							lbl_player1.Text = $"Player 1 : lightgun ({_dinputGunAType})";


							string newMainImgFile = existingConfigClone["lightgun-" + _dinputGunAType];
							if (File.Exists(newMainImgFile))
							{
								Image originalImage = System.Drawing.Image.FromFile(newMainImgFile);
								pictureBox_gameControls.Image = ResizeImageBest(originalImage, pictureBox_gameControls.Size);
							}
						}

						if (_haveLightgun && _dinputLightgunBFound && existingConfigClone.ContainsKey("lightgun-" + _dinputGunBType))
						{
							lbl_player2.Text = $"Player 2 : lightgun ({_dinputGunBType})";
						}

					}


					List<string> AllImages = new List<string>();
					foreach (var configFile in existingConfigClone)
					{
						if (configFile.Key == "gamepad" && usealtgamepad) continue;
						if (configFile.Key == "gamepadalt" && !usealtgamepad) continue;

						string fileName = configFile.Value;
						string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
						string fileDirectory = Path.GetDirectoryName(fileName);
						fileDirectory = Path.GetDirectoryName(fileDirectory);
						string imgFile = Path.Combine(fileDirectory, "img", fileNameWithoutExt + ".jpg");
						if (File.Exists(imgFile))
						{
							AllImages.Add(imgFile);
						}
					}
					AddPictureBoxesToFlowLayoutPanel(AllImages.ToArray());

				}
				else
				{
					_playAutoEnabled = true;
					_playDirectEnabled = true;
					isPlaying = isPlaying;
					btn_gameoptions.Enabled = true;
					btn_tpsettings.Enabled = true;

					string fileDirectory = Path.GetDirectoryName(DataGame.UserConfigFile);
					fileDirectory = Path.GetDirectoryName(fileDirectory);
					string iconFile = string.Empty;
					if (DataGame.Metadata != null)
					{
						iconFile = DataGame.Metadata.icon_name;
					}
					else
					{
						iconFile = Path.GetFileNameWithoutExtension(DataGame.UserConfigFile) + ".png";
					}
					iconFile = Path.Combine(fileDirectory, "Icons", iconFile);
					if (File.Exists(iconFile))
					{
						Image originalImage = System.Drawing.Image.FromFile(iconFile);
						pictureBox_gameControls.Image = ResizeImageBest(originalImage, pictureBox_gameControls.Size);

					}

					lbl_player1.Text = "No Auto-Controller Binding";
					lbl_player2.Text = "Define and use your own control within Teknoparrot UI";
					lbl_player3.Text = "";
					lbl_player4.Text = "";

				}

				if (_selectedGameSettings != null && _selectedGameSettings.overrideTpController == false)
				{
					lbl_player1.Text = "No Auto-Controller Binding";
					lbl_player2.Text = "Define and use your own control within Teknoparrot UI";
					lbl_player3.Text = "";
					lbl_player4.Text = "";

					string fileDirectory = Path.GetDirectoryName(DataGame.UserConfigFile);
					fileDirectory = Path.GetDirectoryName(fileDirectory);
					string iconFile = string.Empty;
					if (DataGame.Metadata != null)
					{
						iconFile = DataGame.Metadata.icon_name;
					}
					else
					{
						iconFile = Path.GetFileNameWithoutExtension(DataGame.UserConfigFile) + ".png";
					}
					iconFile = Path.Combine(fileDirectory, "Icons", iconFile);
					if (File.Exists(iconFile))
					{
						Image originalImage = System.Drawing.Image.FromFile(iconFile);
						pictureBox_gameControls.Image = ResizeImageBest(originalImage, pictureBox_gameControls.Size);

					}

				}

			}
		}

		private void Updatefilter()
		{
			List<IModelFilter> filter_list = new List<IModelFilter>();
			if (this.filter_text != "")
			{
				if (this.filter_text.Contains("*") || this.filter_text.Contains("?"))
				{
					filter_list.Add(new ModelFilter(delegate (object x)
					{
						return ((Game)x).Match(this.filter_text);
					}));
				}
				else
				{
					filter_list.Add(TextMatchFilter.Contains(this.fastObjectListView1, this.filter_text));
				}
				MenuItem_textBoxFilter.BackColor = Color.Yellow;
			}
			else MenuItem_textBoxFilter.BackColor = SystemColors.Control;

			if (this.filter_arcade)
			{
				filter_list.Add(new ModelFilter(delegate (object x)
				{
					return ((Game)x).haveArcadeStickSupport;
				}));
				MenuItem_filterArcade.Font = new Font(MenuItem_filterArcade.Font, FontStyle.Bold);
			}
			else MenuItem_filterArcade.Font = new Font(MenuItem_filterArcade.Font, MenuItem_filterArcade.Font.Style & ~FontStyle.Bold);

			if (this.filter_wheel)
			{
				filter_list.Add(new ModelFilter(delegate (object x)
				{
					return ((Game)x).haveWheelSupport;
				}));
				MenuItem_filterWheel.Font = new Font(MenuItem_filterWheel.Font, FontStyle.Bold);
			}
			else MenuItem_filterWheel.Font = new Font(MenuItem_filterWheel.Font, MenuItem_filterWheel.Font.Style & ~FontStyle.Bold);

			if (this.filter_hotas)
			{
				filter_list.Add(new ModelFilter(delegate (object x)
				{
					return ((Game)x).haveHotasSupport;
				}));
				MenuItem_filterHotas.Font = new Font(MenuItem_filterHotas.Font, FontStyle.Bold);
			}
			else MenuItem_filterHotas.Font = new Font(MenuItem_filterHotas.Font, MenuItem_filterHotas.Font.Style & ~FontStyle.Bold);

			if (this.filter_lightgun)
			{
				filter_list.Add(new ModelFilter(delegate (object x)
				{
					return ((Game)x).haveLightgunSupport;
				}));
				MenuItem_filterLightgun.Font = new Font(MenuItem_filterLightgun.Font, FontStyle.Bold);
			}
			else MenuItem_filterLightgun.Font = new Font(MenuItem_filterLightgun.Font, MenuItem_filterLightgun.Font.Style & ~FontStyle.Bold);

			if (filter_list.Count > 0)
			{
				this.fastObjectListView1.AdditionalFilter = new CompositeAllFilter(filter_list);
				MenuItem_clearFilters.Enabled = true;
			}
			else
			{
				this.fastObjectListView1.AdditionalFilter = null;
				MenuItem_clearFilters.Enabled = false;
			}


		}

		private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			MenuItem_save.Visible = false;
			MenuItem_load.Visible = false;
			MenuItem_edit.Visible = false;

			if (fastObjectListView1.SelectedIndex >= 0)
			{
				MenuItem_save.Visible = true;
				MenuItem_load.Visible = true;
				MenuItem_edit.Visible = true;
			}
		}

		private void MainDPIcs_FormClosing(object sender, FormClosingEventArgs e)
		{


		}

		private void cmb_displayMode_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void contextMenuStrip1_Opening_1(object sender, System.ComponentModel.CancelEventArgs e)
		{

		}

		private void MenuItem_clearFilters_Click(object sender, EventArgs e)
		{
			filter_arcade = filter_hotas = filter_lightgun = filter_wheel = false;
			MenuItem_textBoxFilter.Text = "";
			this.filter_text = "";
			Updatefilter();
		}

		private void MenuItem_textBoxFilter_Leave(object sender, EventArgs e)
		{
			if (MenuItem_textBoxFilter.Text != this.filter_text)
			{
				this.filter_text = MenuItem_textBoxFilter.Text;
				Updatefilter();
			}
		}
		private void MenuItem_textBoxFilter_CheckEnterKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Return)
			{
				// Then Do your Thang
				contextMenuStrip1.Hide();
				if (MenuItem_textBoxFilter.Text != this.filter_text)
				{
					this.filter_text = MenuItem_textBoxFilter.Text;
					Updatefilter();
				}
			}
		}

		private void MenuItem_filterArcade_Click(object sender, EventArgs e)
		{
			filter_hotas = filter_lightgun = filter_wheel = false;
			this.filter_arcade = !this.filter_arcade;
			Updatefilter();
		}

		private void MenuItem_filterWheel_Click(object sender, EventArgs e)
		{
			filter_arcade = filter_hotas = filter_lightgun = false;
			this.filter_wheel = !this.filter_wheel;
			Updatefilter();
		}

		private void MenuItem_filterHotas_Click(object sender, EventArgs e)
		{
			filter_arcade = filter_lightgun = filter_wheel = false;
			this.filter_hotas = !this.filter_hotas;
			Updatefilter();
		}

		private void MenuItem_filterLightgun_Click(object sender, EventArgs e)
		{
			filter_arcade = filter_hotas = filter_wheel = false;
			this.filter_lightgun = !this.filter_lightgun;
			Updatefilter();
		}

		private void MenuItem_save_Click(object sender, EventArgs e)
		{
			if (fastObjectListView1.SelectedIndex >= 0)
			{
				//btn_playgamedirect.Enabled = true;
				//btn_playgamedirect2.Enabled = true;

				Game DataGame = (Game)fastObjectListView1.SelectedObject;


				// Chemin de sortie de l'archive

				// Fichier à ajouter à la racine de l'archive




				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.Filter = "Fichiers 7z (*.7z)|*.7z";
				saveFileDialog.DefaultExt = "7z"; // Extension par défaut
				saveFileDialog.FileName = Path.GetFileNameWithoutExtension(DataGame.UserConfigFile) + ".7z";
				saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					string filePath = saveFileDialog.FileName;
					if (!filePath.EndsWith(".7z", StringComparison.OrdinalIgnoreCase))
					{
						filePath += ".7z";
					}
					string outputArchive = filePath;

					string SevenZipExe = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "7zip", "7z.exe");

					string tmpFolder = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "tmparchive");
					try
					{
						if (Directory.Exists(tmpFolder))
						{
							Directory.Delete(tmpFolder, true);
						}
						Directory.CreateDirectory(tmpFolder);

						string tmpTpPatches = Path.Combine(tmpFolder, "tp-patches");
						string tmpGamePatches = Path.Combine(tmpFolder, "games-patches");
						string tmpConfigDir = Path.Combine(tmpFolder, "config");
						string tmpImageDir = Path.Combine(tmpFolder, "img");
						string tmpMagpieDir = Path.Combine(tmpFolder, "magpie");

						Directory.CreateDirectory(tmpTpPatches);
						Directory.CreateDirectory(tmpGamePatches);
						Directory.CreateDirectory(tmpConfigDir);
						Directory.CreateDirectory(tmpImageDir);
						Directory.CreateDirectory(tmpMagpieDir);

						List<string> magpieReshadeFile = new List<string>();

						UpdatePatchArchive updatePatchArchive = new UpdatePatchArchive();
						updatePatchArchive.Game = Path.GetFileNameWithoutExtension(DataGame.UserConfigFile);



						var infoFile = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "config", Path.GetFileNameWithoutExtension(DataGame.UserConfigFile) + ".info.json");
						if (File.Exists(infoFile))
						{
							//File.Copy(infoFile, Path.Combine(tmpFolder, Path.GetFileName(infoFile)));


							string pathAutoXinputLinks = "";
							string perGameLinkFolder = ConfigurationManager.MainConfig.perGameLinkFolder;
							if (perGameLinkFolder == @"Default (<YourTeknoparrotFolder>\AutoXinputLinks)")
							{
								pathAutoXinputLinks = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "AutoXinputLinks", Path.GetFileNameWithoutExtension(DataGame.FileName));
							}
							else
							{
								pathAutoXinputLinks = Path.Combine(perGameLinkFolder, Path.GetFileNameWithoutExtension(DataGame.FileName));
							}
							if (Directory.Exists(pathAutoXinputLinks))
							{
								List<string> excludeFiles = new List<string>();
								var patchlist = new List<string>();
								patchlist.AddRange(Directory.GetFiles(pathAutoXinputLinks, "patchdata.json", SearchOption.AllDirectories));
								foreach (var patchJsonFile in patchlist)
								{
									string directorySource = Path.GetDirectoryName(patchJsonFile);
									directorySource = Path.GetDirectoryName(directorySource);
									string jsonData = Utils.ReadAllText(patchJsonFile);
									List<PatchInfoJsonElement> patchInfoJson = JsonConvert.DeserializeObject<List<PatchInfoJsonElement>>(jsonData);
									foreach (var patch in patchInfoJson)
									{
										string expectedPatchFile = Path.Combine(directorySource, patch.destination);
										if (File.Exists(expectedPatchFile))
										{
											excludeFiles.Add(expectedPatchFile);
										}
									}
								}
								foreach (var file in Directory.GetFiles(pathAutoXinputLinks, "*", SearchOption.AllDirectories))
								{
									if (Path.GetFileName(file) == "[!magpiereshade!].ini") magpieReshadeFile.Add(file);

									if (excludeFiles.Contains(file)) continue;
									if (file.ToLower().Contains("[!cachereshade!]")) continue;
									if (Path.GetFileName(file) == "patchinfo.php") continue;
									if (Path.GetFileName(file).ToLower() == "dgvoodoo.conf.custom") continue;
									if (File.Exists(file + ".remplaceRefreshRate")) continue;


									string newFile = file.Replace(pathAutoXinputLinks, tmpTpPatches);
									string newDir = Path.GetDirectoryName(newFile);
									updatePatchArchive.tpPatches = true;
									if (!Directory.Exists(newDir))
									{
										Directory.CreateDirectory(newDir);
									}
									if (!Utils.MakeLinkBool(file, newFile))
									{
										File.Copy(file, newFile, true);
									}
								}
							}


							GameSettings gameOptionsSelectedGame = null;
							string optionFile = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "gameoptions", Path.GetFileNameWithoutExtension(DataGame.UserConfigFile) + ".json");
							if (File.Exists(optionFile))
							{
								gameOptionsSelectedGame = new GameSettings(Utils.ReadAllText(optionFile));
							}
							string linkSourceFolderExe = Path.Combine(ConfigurationManager.MainConfig.perGameLinkFolderExe, Path.GetFileNameWithoutExtension(DataGame.FileName));
							if (gameOptionsSelectedGame != null && gameOptionsSelectedGame.CustomPerGameLinkFolder != null && gameOptionsSelectedGame.CustomPerGameLinkFolder != "")
							{
								string lastFolder = Path.GetFileName(gameOptionsSelectedGame.CustomPerGameLinkFolder);
								if (lastFolder == Path.GetFileNameWithoutExtension(DataGame.FileName))
								{
									linkSourceFolderExe = gameOptionsSelectedGame.CustomPerGameLinkFolder;
								}
							}
							if (Directory.Exists(linkSourceFolderExe))
							{
								List<string> excludeFiles = new List<string>();
								var patchlist = new List<string>();
								patchlist.AddRange(Directory.GetFiles(linkSourceFolderExe, "patchdata.json", SearchOption.AllDirectories));
								foreach (var patchJsonFile in patchlist)
								{
									string directorySource = Path.GetDirectoryName(patchJsonFile);
									directorySource = Path.GetDirectoryName(directorySource);
									string jsonData = Utils.ReadAllText(patchJsonFile);
									List<PatchInfoJsonElement> patchInfoJson = JsonConvert.DeserializeObject<List<PatchInfoJsonElement>>(jsonData);
									foreach (var patch in patchInfoJson)
									{
										string expectedPatchFile = Path.Combine(directorySource, patch.destination);
										if (File.Exists(expectedPatchFile))
										{
											excludeFiles.Add(expectedPatchFile);
										}
									}
								}
								foreach (var file in Directory.GetFiles(linkSourceFolderExe, "*", SearchOption.AllDirectories))
								{
									if (Path.GetFileName(file) == "[!magpiereshade!].ini") magpieReshadeFile.Add(file);

									if (excludeFiles.Contains(file)) continue;
									if (file.ToLower().Contains("[!cachereshade!]")) continue;
									if (Path.GetFileName(file) == "patchinfo.php") continue;
									if (Path.GetFileName(file).ToLower() == "dgvoodoo.conf.custom") continue;
									if (File.Exists(file + ".remplaceRefreshRate")) continue;

									string newFile = file.Replace(linkSourceFolderExe, tmpGamePatches);
									string newDir = Path.GetDirectoryName(newFile);
									if (!Directory.Exists(newDir))
									{
										Directory.CreateDirectory(newDir);
									}
									updatePatchArchive.gamePatches = true;
									if (!Utils.MakeLinkBool(file, newFile))
									{
										File.Copy(file, newFile, true);
									}
								}
							}

							var configDir = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "config");
							if (Directory.Exists(configDir))
							{
								foreach (var file in Directory.GetFiles(configDir, Path.GetFileNameWithoutExtension(DataGame.UserConfigFile) + ".*", SearchOption.AllDirectories))
								{
									string newFile = file.Replace(configDir, tmpConfigDir);
									string newDir = Path.GetDirectoryName(newFile);
									if (!Directory.Exists(newDir))
									{
										Directory.CreateDirectory(newDir);
									}
									File.Copy(file, newFile, true);
								}
							}

							var imageDir = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "img");
							if (Directory.Exists(imageDir))
							{
								foreach (var file in Directory.GetFiles(imageDir, Path.GetFileNameWithoutExtension(DataGame.UserConfigFile) + ".*", SearchOption.AllDirectories))
								{
									string newFile = file.Replace(imageDir, tmpImageDir);
									string newDir = Path.GetDirectoryName(newFile);
									if (!Directory.Exists(newDir))
									{
										Directory.CreateDirectory(newDir);
									}
									File.Copy(file, newFile, true);
								}
							}



							string magpieDir = Path.Combine(Path.GetDirectoryName(ConfigurationManager.MainConfig.magpieExe), "reshade-shaders");
							if (Directory.Exists(magpieDir))
							{
								List<string> whitelist = new List<string> { "areatex.png", "areacopy.fx", "areacopypercent.fx", "ascii.fx", "bezelmagpie.fx", "bezelmagpiefirst.fx", "blending.fxh", "border.fx", "bordersinden.fx", "bordersinden2.fx", "cartoon.fx", "cas.fx", "chromaticaberration.fx", "colormatrix.fx", "colourfulness.fx", "crt-guest-advanced.fx", "crt-guest-hd.fx", "crt-guest-ntsc.fx", "crt-guest-preshader.fx", "crt.fx", "crtgeommod.fx", "curves.fx", "daltonize.fx", "deband.fx", "displaydepth.fx", "dpx.fx", "drawtext.fxh", "fakehdr.fx", "filmgrain.fx", "fxaa.fx", "fxaa.fxh", "gridfirst.fx", "gridlast.fx", "hq4x.fx", "layer.fx", "levels.fx", "liftgammagain.fx", "lumasharpen.fx", "lut.fx", "macros.fxh", "monochrome.fx", "nostalgia.fx", "reshade.fxh", "reshadeui.fxh", "sepia.fx", "smaa.fx", "smaa.fxh", "splitscreen.fx", "technicolor.fx", "technicolor2.fx", "tonemap.fx", "tridither.fxh", "uimask.fx", "vibrance.fx", "vignette.fx", "bloominghdr.fx", "clarity.fx", "nfaa.fx", "overwatch.fxh", "smart_sharp.fx", "temporal_aa.fx", "tobiieye_freepie_astrayfx.py", "cdiscblur.fx", "cmotionblur.fx", "ctransform.fx", "ctransformsinden.fx", "cbuffers.fxh", "cgraphics.fxh", "cimageprocessing.fxh", "cmacros.fxh", "cvideoprocessing.fxh", "aspectratio.fx", "bluenoisedither.fxh", "colorconversion.fxh", "lineargammaworkflow.fxh", "background.png", "bezel.png", "bezel43.png", "bezel_off.png", "crt-lut-1.png", "crt-lut-2.png", "crt-lut-3.png", "crt-lut-4.png", "fontatlas.png", "frame.png", "grid.png", "layer.png", "lut.png", "mask.png", "mask2x2.png", "overlay.png", "searchtex.png", "testbezel.png", "uimask.png" };
								List<string> usedShaders = new List<string>();
								var fileList = Directory.GetFiles(magpieDir, "*", SearchOption.AllDirectories);
								List<string> presetFilesName = new List<string>();
								try
								{
									foreach (var file in magpieReshadeFile)
									{
										var content = Utils.ReadAllText(file);
										string existingTechniques = Regex.Match(content, @"^Techniques=(.*)", RegexOptions.Multiline).Groups[1].Value.Trim('\n').Trim('\r').Trim('\n');
										foreach (var technique in existingTechniques.Split(','))
										{
											if (technique.Split("@").Length == 2 && !usedShaders.Contains(technique.Split("@")[1])) usedShaders.Add(technique.Split("@")[1].ToLower());
										}
										string pattern = "\"[^\"]*\\.(png|fx|fxh)\"";
										MatchCollection matches = Regex.Matches(content, pattern);
										foreach (Match match in matches)
										{
											string newFile = Path.GetFileName(match.Value.ToLower().Trim('"'));
											foreach (var f in fileList)
											{
												if (Path.GetFileName(f).ToLower() == newFile.ToLower())
												{
													if (!usedShaders.Contains(newFile)) usedShaders.Add(newFile);
													break;
												}
											}

										}

									}

									var fileCount = usedShaders.Count;
									var usedShaders2 = new List<string>(usedShaders.ToArray());
									int compteur = 0;
									while (true)
									{
										compteur++;
										Console.WriteLine("Turn " + compteur);
										foreach (var file in fileList)
										{
											if (file.ToLower().EndsWith(".png")) continue;
											if (usedShaders.Contains(Path.GetFileName(file).ToLower()))
											{
												string fileContent = Utils.ReadAllText(file);
												string pattern = "\"[^\"]*\\.(png|fx|fxh)\"";
												MatchCollection matches = Regex.Matches(fileContent, pattern);
												foreach (Match match in matches)
												{
													string newFile = Path.GetFileName(match.Value.ToLower().Trim('"'));
													if (!usedShaders2.Contains(newFile)) usedShaders2.Add(newFile);
												}
											}
										}
										if (usedShaders2.Count == usedShaders.Count)
										{
											break;
										}
										usedShaders = new List<string>(usedShaders2.ToArray());
									}

								}
								catch
								{
									usedShaders = new List<string> { };
								}



								foreach (var shad in usedShaders)
								{
									if (whitelist.Contains(shad.ToLower())) continue;
									foreach (var file in fileList)
									{
										if (Path.GetFileName(file).ToLower() == shad.ToLower())
										{
											string newFile = file.Replace(magpieDir, tmpMagpieDir);
											string newDir = Path.GetDirectoryName(newFile);
											if (!Directory.Exists(newDir))
											{
												Directory.CreateDirectory(newDir);
											}
											File.Copy(file, newFile, true);
											updatePatchArchive.magpiePatches = true;
										}
									}
								}
							}

							string jsondata = JsonConvert.SerializeObject(updatePatchArchive, Newtonsoft.Json.Formatting.Indented);
							File.WriteAllText(Path.Combine(tmpFolder, "info.json"), jsondata);

							if (File.Exists(outputArchive)) File.Delete(outputArchive);

							{
								// Construire la commande pour 7z.exe
								ProcessStartInfo processStartInfo = new ProcessStartInfo
								{
									FileName = SevenZipExe, // Si 7z.exe n'est pas dans PATH, spécifiez le chemin complet ici
									Arguments = $"a -mx=0 {outputArchive} info.json",
									WorkingDirectory = tmpFolder,
									RedirectStandardOutput = true,
									UseShellExecute = false,
									CreateNoWindow = true
								};

								// Exécuter la commande
								using (Process process = new Process())
								{
									process.StartInfo = processStartInfo;
									process.Start();
									process.WaitForExit();

									// Lire la sortie de la commande si nécessaire
									string output = process.StandardOutput.ReadToEnd();
									Console.WriteLine(output);
								}
							}

							{
								// Construire la commande pour 7z.exe
								ProcessStartInfo processStartInfo = new ProcessStartInfo
								{
									FileName = SevenZipExe, // Si 7z.exe n'est pas dans PATH, spécifiez le chemin complet ici
									Arguments = $"a -mx=9 {outputArchive} . -x!info.json",
									WorkingDirectory = tmpFolder,
									RedirectStandardOutput = true,
									UseShellExecute = false,
									CreateNoWindow = true
								};

								// Exécuter la commande
								using (Process process = new Process())
								{
									process.StartInfo = processStartInfo;
									process.Start();
									process.WaitForExit();

									// Lire la sortie de la commande si nécessaire
									string output = process.StandardOutput.ReadToEnd();
									Console.WriteLine(output);
								}
							}


						}
						Directory.Delete(tmpFolder, true);
					}
					catch { };

					MessageBox.Show("Saved in " + filePath);
				}
			}
		}

		private void MenuItem_load_Click(object sender, EventArgs e)
		{
			if (fastObjectListView1.SelectedIndex >= 0)
			{

				Game DataGame = (Game)fastObjectListView1.SelectedObject;

				// Créez une instance de OpenFileDialog
				OpenFileDialog openFileDialog = new OpenFileDialog();

				// Définissez le filtre pour afficher uniquement les fichiers .7z
				openFileDialog.Filter = "Fichiers 7z (*.7z)|*.7z";

				// Définissez le répertoire initial sur le dossier Documents de l'utilisateur
				openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

				// Si l'utilisateur clique sur OK
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					// Récupérez le chemin du fichier sélectionné par l'utilisateur
					string filePath = openFileDialog.FileName;

					string fileContent = "";
					try
					{
						using (SevenZipExtractor extractor = new SevenZipExtractor(filePath))
						{
							string fileToExtract = "info.json";

							using (MemoryStream ms = new MemoryStream())
							{
								extractor.ExtractFile(fileToExtract, ms);
								ms.Seek(0, SeekOrigin.Begin);
								using (StreamReader reader = new StreamReader(ms))
								{
									fileContent = reader.ReadToEnd();
								}
							}
						}
					}
					catch
					{
						fileContent = "";
					}
					if (string.IsNullOrEmpty(fileContent))
					{
						MessageBox.Show("Invalid archive");
						return;
					}
					else
					{
						UpdatePatchArchive patchInfoJson = JsonConvert.DeserializeObject<UpdatePatchArchive>(fileContent);
						if (patchInfoJson != null && !string.IsNullOrEmpty(patchInfoJson.Game))
						{
							if (patchInfoJson.Game.ToLower() != Path.GetFileNameWithoutExtension(DataGame.UserConfigFile).ToLower())
							{
								MessageBox.Show("Wrong game for patch");
								return;
							}

							MessageBox.Show(fileContent);
							string SevenZipExe = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "7zip", "7z.exe");

							GameSettings gameOptionsSelectedGame = null;
							string optionFile = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "gameoptions", patchInfoJson.Game + ".json");
							if (File.Exists(optionFile))
							{
								gameOptionsSelectedGame = new GameSettings(Utils.ReadAllText(optionFile));
							}
							string linkSourceFolderExe = Path.Combine(ConfigurationManager.MainConfig.perGameLinkFolderExe, patchInfoJson.Game);
							if (gameOptionsSelectedGame != null && gameOptionsSelectedGame.CustomPerGameLinkFolder != null && gameOptionsSelectedGame.CustomPerGameLinkFolder != "")
							{
								string lastFolder = Path.GetFileName(gameOptionsSelectedGame.CustomPerGameLinkFolder);
								if (lastFolder == patchInfoJson.Game)
								{
									linkSourceFolderExe = gameOptionsSelectedGame.CustomPerGameLinkFolder;
								}
							}

							string pathAutoXinputLinks = "";
							string perGameLinkFolder = ConfigurationManager.MainConfig.perGameLinkFolder;
							if (perGameLinkFolder == @"Default (<YourTeknoparrotFolder>\AutoXinputLinks)")
							{
								pathAutoXinputLinks = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "AutoXinputLinks", patchInfoJson.Game);
							}
							else
							{
								pathAutoXinputLinks = Path.Combine(perGameLinkFolder, patchInfoJson.Game);
							}

							string configDir = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "config");
							string imageDir = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "img");
							string magpieDir = Path.Combine(Path.GetDirectoryName(ConfigurationManager.MainConfig.magpieExe), "reshade-shaders");

							string tmpFolder = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "tmparchive");
							try
							{
								if (Directory.Exists(tmpFolder))
								{
									Directory.Delete(tmpFolder, true);
								}
								Directory.CreateDirectory(tmpFolder);


								ProcessStartInfo processStartInfo = new ProcessStartInfo
								{
									FileName = SevenZipExe, // Si 7z.exe n'est pas dans PATH, spécifiez le chemin complet ici
									Arguments = $"x \"{filePath}\" -o\"{tmpFolder}\"",
									RedirectStandardOutput = true,
									UseShellExecute = false,
									CreateNoWindow = true
								};

								// Exécuter la commande
								using (Process process = new Process())
								{
									process.StartInfo = processStartInfo;
									process.Start();
									process.WaitForExit();
								}

							}
							catch
							{
								MessageBox.Show("error occured");
								return;
							}

							string tmpTpPatches = Path.Combine(tmpFolder, "tp-patches");
							string tmpGamePatches = Path.Combine(tmpFolder, "games-patches");
							string tmpConfigDir = Path.Combine(tmpFolder, "config");
							string tmpImageDir = Path.Combine(tmpFolder, "img");
							string tmpMagpieDir = Path.Combine(tmpFolder, "magpie");


							if (patchInfoJson.gamePatches && (!Directory.Exists(tmpGamePatches) || Directory.GetFiles(tmpGamePatches, "*", SearchOption.AllDirectories).Count() == 0))
							{
								MessageBox.Show("Invalid archive (gamepatch dont match)");
								return;
							}
							if (patchInfoJson.tpPatches && (!Directory.Exists(tmpTpPatches) || Directory.GetFiles(tmpTpPatches, "*", SearchOption.AllDirectories).Count() == 0))
							{
								MessageBox.Show("Invalid archive (tppatch dont match)");
								return;
							}
							if (patchInfoJson.magpiePatches && (!Directory.Exists(tmpMagpieDir) || Directory.GetFiles(tmpMagpieDir, "*", SearchOption.AllDirectories).Count() == 0))
							{
								MessageBox.Show("Invalid archive (Magpiepatch dont match)");
								return;
							}


							foreach (Control control in flowLayoutPanelThumbs.Controls)
							{
								if (control is PictureBox pictureBox)
								{
									if (pictureBox.Image != null)
									{
										Image copiedImage = CreateImageCopy(pictureBox.Image);
										pictureBox.Image.Dispose();
										pictureBox.Image = copiedImage;
									}

								}
							}
							if (pictureBox_gameControls.Image != null)
							{
								Image copiedImage = CreateImageCopy(pictureBox_gameControls.Image);
								pictureBox_gameControls.Image.Dispose();
								pictureBox_gameControls.Image = copiedImage;

							}

							if (Directory.Exists(pathAutoXinputLinks))
							{
								Directory.Delete(pathAutoXinputLinks, true);
							}
							if (Directory.Exists(linkSourceFolderExe))
							{
								Directory.Delete(linkSourceFolderExe, true);
							}

							if (Directory.Exists(tmpConfigDir) || Directory.GetFiles(tmpConfigDir, "*", SearchOption.AllDirectories).Count() > 0)
							{
								if (Directory.Exists(configDir))
								{
									foreach (var file in Directory.GetFiles(configDir, patchInfoJson.Game + ".*", SearchOption.AllDirectories))
									{
										File.Delete(file);
									}
								}
							}

							if (Directory.Exists(tmpImageDir) || Directory.GetFiles(tmpImageDir, "*", SearchOption.AllDirectories).Count() > 0)
							{
								if (Directory.Exists(imageDir))
								{
									foreach (var file in Directory.GetFiles(imageDir, patchInfoJson.Game + ".*", SearchOption.AllDirectories))
									{
										File.Delete(file);
									}
								}
							}


							if (Directory.Exists(configDir) && Directory.Exists(tmpConfigDir))
							{
								foreach (var file in Directory.GetFiles(tmpConfigDir, "*", SearchOption.AllDirectories))
								{
									string newFile = file.Replace(tmpConfigDir, configDir);
									string newDir = Path.GetDirectoryName(newFile);
									if (!Directory.Exists(newDir))
									{
										Directory.CreateDirectory(newDir);
									}
									File.Copy(file, newFile, true);
								}
							}


							if (Directory.Exists(imageDir) && Directory.Exists(tmpImageDir))
							{
								foreach (var file in Directory.GetFiles(tmpImageDir, "*", SearchOption.AllDirectories))
								{
									string newFile = file.Replace(tmpImageDir, imageDir);
									string newDir = Path.GetDirectoryName(newFile);
									if (!Directory.Exists(newDir))
									{
										Directory.CreateDirectory(newDir);
									}
									File.Copy(file, newFile, true);
								}
							}


							if (patchInfoJson.tpPatches && Directory.Exists(tmpTpPatches))
							{
								Directory.CreateDirectory(pathAutoXinputLinks);
								foreach (var file in Directory.GetFiles(tmpTpPatches, "*", SearchOption.AllDirectories))
								{
									string newFile = file.Replace(tmpTpPatches, pathAutoXinputLinks);
									string newDir = Path.GetDirectoryName(newFile);
									if (!Directory.Exists(newDir))
									{
										Directory.CreateDirectory(newDir);
									}
									File.Copy(file, newFile, true);
								}
							}


							if (patchInfoJson.gamePatches && Directory.Exists(tmpGamePatches))
							{
								Directory.CreateDirectory(linkSourceFolderExe);
								foreach (var file in Directory.GetFiles(tmpGamePatches, "*", SearchOption.AllDirectories))
								{
									string newFile = file.Replace(tmpGamePatches, linkSourceFolderExe);
									string newDir = Path.GetDirectoryName(newFile);
									if (!Directory.Exists(newDir))
									{
										Directory.CreateDirectory(newDir);
									}
									File.Copy(file, newFile, true);
								}
							}


							if (patchInfoJson.magpiePatches && Directory.Exists(tmpMagpieDir) && Directory.Exists(magpieDir))
							{
								foreach (var file in Directory.GetFiles(tmpMagpieDir, "*", SearchOption.AllDirectories))
								{
									string newFile = file.Replace(tmpMagpieDir, magpieDir);
									string newDir = Path.GetDirectoryName(newFile);
									if (!Directory.Exists(newDir))
									{
										Directory.CreateDirectory(newDir);
									}
									File.Copy(file, newFile, true);
								}
							}

							Reload();

						}
						else
						{
							MessageBox.Show("Invalid archive");
							return;
						}

					}

				}

			}
		}

		private void MenuItem_edit_Click(object sender, EventArgs e)
		{
			if (_selectedGame != null)
			{
				string exeNotepad = "notepad";

				string notepadPath = Utils.checkInstalled("Notepad++");
				if (notepadPath != null)
				{
					notepadPath = Path.Combine(Path.GetDirectoryName(notepadPath), "notepad++.exe");
					if (File.Exists(notepadPath)) exeNotepad = notepadPath;
				}

				string basePath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);

				var infoFile = Path.Combine(basePath, "config", Path.GetFileNameWithoutExtension(_selectedGame.UserConfigFile) + ".info.json");
				if (File.Exists(infoFile))
				{
					Process process = new Process();
					process.StartInfo.FileName = exeNotepad;
					process.StartInfo.Arguments = infoFile;
					process.StartInfo.UseShellExecute = true;
					process.Start();
				}

			}
		}

		private Image CreateImageCopy(Image originalImage)
		{
			// Créer une nouvelle image de la même taille que l'originale
			Image newImage = new Bitmap(originalImage.Width, originalImage.Height);

			// Utiliser Graphics pour dessiner l'image originale sur la nouvelle image
			using (Graphics graphics = Graphics.FromImage(newImage))
			{
				graphics.DrawImage(originalImage, 0, 0, originalImage.Width, originalImage.Height);
			}

			return newImage;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (_selectedGame != null)
			{
				string notepadPath = Utils.checkInstalled("Notepad++");
				notepadPath = Path.Combine(Path.GetDirectoryName(notepadPath), "notepad++.exe");
				string basePath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);

				var infoFile = Path.Combine(basePath, "config", Path.GetFileNameWithoutExtension(_selectedGame.UserConfigFile) + ".info.json");
				if (File.Exists(infoFile))
				{
					Process process = new Process();
					process.StartInfo.FileName = notepadPath;
					process.StartInfo.Arguments = infoFile;
					process.StartInfo.UseShellExecute = true;
					process.Start();
				}

			}
		}

		private void cmb_displayMode_SelectedIndexChanged_1(object sender, EventArgs e)
		{
			int displayMode = ConfigurationManager.MainConfig.displayMode;
			if (_selectedGameSettings != null)
			{
				displayMode = _selectedGameSettings.displayMode > 0 ? (_selectedGameSettings.displayMode - 1) : displayMode;
			}
			if (_selectedGameInfo.Count() > 0)
			{
				try
				{
					bool upscaleWindowed = true;
					bool upscaleFullscreen = true;
					string originalRes = "";
					var GameInfo = _selectedGameInfo;
					if (GameInfo.ContainsKey("upscaleWindowed") && GameInfo["upscaleWindowed"].ToLower() == "false") upscaleWindowed = false;
					if (GameInfo.ContainsKey("upscaleFullscreen") && GameInfo["upscaleFullscreen"].ToLower() == "false") upscaleFullscreen = false;
					//cmb_nativeRes.Items[0] = "Native" + $" ({originalRes})";
					cmb_nativeRes.Visible = false;
					cmb_resolution.Visible = true;
					if (cmb_displayMode.SelectedIndex == 0)
					{
						if (displayMode == 0)
						{
							if (!upscaleFullscreen && !upscaleWindowed)
							{
								cmb_nativeRes.Visible = true;
								cmb_resolution.Visible = false;
							}
						}
						if (displayMode == 1)
						{
							if (!upscaleFullscreen)
							{
								cmb_nativeRes.Visible = true;
								cmb_resolution.Visible = false;
							}
						}
						if (displayMode == 2)
						{
							if (!upscaleWindowed)
							{
								cmb_nativeRes.Visible = true;
								cmb_resolution.Visible = false;
							}
						}
					}
					if (cmb_displayMode.SelectedIndex == 1)
					{
						if (!upscaleFullscreen && !upscaleWindowed)
						{
							cmb_nativeRes.Visible = true;
							cmb_resolution.Visible = false;
						}
					}
					if (cmb_displayMode.SelectedIndex == 2)
					{
						if (!upscaleFullscreen)
						{
							cmb_nativeRes.Visible = true;
							cmb_resolution.Visible = false;
						}
					}
					if (cmb_displayMode.SelectedIndex == 3)
					{
						if (!upscaleWindowed)
						{
							cmb_nativeRes.Visible = true;
							cmb_resolution.Visible = false;
						}
					}


				}
				catch { }
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			var frm = new FormTest();
			var result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{

			}
		}

		private void kryptonLabel4_Click(object sender, EventArgs e)
		{

		}

		private void flowLayoutPanelThumbs_Paint(object sender, PaintEventArgs e)
		{

		}

		private void fpanel_perf_DoubleClick(object sender, EventArgs e)
		{
			if (_selectedGame != null)
			{
				var PerGameConfigFile = Path.Combine(Program.GameOptionsFolder, Path.GetFileNameWithoutExtension(_selectedGame.UserConfigFile) + ".json");
				if (_selectedGameSettings == null) _selectedGameSettings = new GameSettings();
				int newVal = _selectedGameSettings.performanceProfile + 1;
				if (_selectedGameOption.ContainsKey("showGameOptionPerf") && _selectedGameOption["showGameOptionPerf"].ToLower() != "high" && newVal == 3) newVal++;
				if (newVal > 3) newVal = 0;
				_selectedGameSettings.performanceProfile = newVal;
				_selectedGameSettings.Save(PerGameConfigFile);
				fastObjectListView1_SelectedIndexChanged(null, null);
			}
		}

		private void fpanel_ffb_DoubleClick(object sender, EventArgs e)
		{
			if (_selectedGame != null)
			{
				var PerGameConfigFile = Path.Combine(Program.GameOptionsFolder, Path.GetFileNameWithoutExtension(_selectedGame.UserConfigFile) + ".json");
				if (_selectedGameSettings == null) _selectedGameSettings = new GameSettings();
				int newVal = _selectedGameSettings.patchFFB + 1;
				if (newVal > 2) newVal = 0;
				_selectedGameSettings.patchFFB = newVal;
				_selectedGameSettings.Save(PerGameConfigFile);
				fastObjectListView1_SelectedIndexChanged(null, null);
			}
		}

		private void fpanel_crt_DoubleClick(object sender, EventArgs e)
		{
			if (_selectedGame != null)
			{
				var PerGameConfigFile = Path.Combine(Program.GameOptionsFolder, Path.GetFileNameWithoutExtension(_selectedGame.UserConfigFile) + ".json");
				if (_selectedGameSettings == null) _selectedGameSettings = new GameSettings();
				int newVal = _selectedGameSettings.useCrt + 1;
				if (newVal > 2) newVal = 0;
				_selectedGameSettings.useCrt = newVal;
				_selectedGameSettings.Save(PerGameConfigFile);
				fastObjectListView1_SelectedIndexChanged(null, null);
			}
		}

		private void fpanel_bezel_DoubleClick(object sender, EventArgs e)
		{
			if (_selectedGame != null)
			{
				var PerGameConfigFile = Path.Combine(Program.GameOptionsFolder, Path.GetFileNameWithoutExtension(_selectedGame.UserConfigFile) + ".json");
				if (_selectedGameSettings == null) _selectedGameSettings = new GameSettings();
				int newVal = _selectedGameSettings.useBezel + 1;
				if (newVal > 2) newVal = 0;
				_selectedGameSettings.useBezel = newVal;
				_selectedGameSettings.Save(PerGameConfigFile);
				fastObjectListView1_SelectedIndexChanged(null, null);
			}
		}

		private void fpanel_vsync_DoubleClick(object sender, EventArgs e)
		{
			if (_selectedGame != null)
			{
				var PerGameConfigFile = Path.Combine(Program.GameOptionsFolder, Path.GetFileNameWithoutExtension(_selectedGame.UserConfigFile) + ".json");
				if (_selectedGameSettings == null) _selectedGameSettings = new GameSettings();
				int newVal = _selectedGameSettings.forceVsync + 1;
				if (newVal > 2) newVal = 0;
				_selectedGameSettings.forceVsync = newVal;
				_selectedGameSettings.Save(PerGameConfigFile);
				fastObjectListView1_SelectedIndexChanged(null, null);
			}
		}

		private void fpanel_aspectratio_DoubleClick(object sender, EventArgs e)
		{
			if (_selectedGame != null)
			{
				var PerGameConfigFile = Path.Combine(Program.GameOptionsFolder, Path.GetFileNameWithoutExtension(_selectedGame.UserConfigFile) + ".json");
				if (_selectedGameSettings == null) _selectedGameSettings = new GameSettings();
				int newVal = _selectedGameSettings.keepAspectRatio + 1;
				if (newVal > 2) newVal = 0;
				_selectedGameSettings.keepAspectRatio = newVal;
				_selectedGameSettings.Save(PerGameConfigFile);
				fastObjectListView1_SelectedIndexChanged(null, null);
			}
		}
		private void fpanel_translation_DoubleClick(object sender, EventArgs e)
		{
			if (_selectedGame != null)
			{
				var PerGameConfigFile = Path.Combine(Program.GameOptionsFolder, Path.GetFileNameWithoutExtension(_selectedGame.UserConfigFile) + ".json");
				if (_selectedGameSettings == null) _selectedGameSettings = new GameSettings();
				int newVal = _selectedGameSettings.patchLang + 1;
				if (_selectedGameOption.ContainsKey("showGameOptionPatchLang") && _selectedGameOption["showGameOptionPatchLang"].ToLower() == "french" && newVal == 2) newVal++;
				if (_selectedGameOption.ContainsKey("showGameOptionPatchLang") && _selectedGameOption["showGameOptionPatchLang"].ToLower() == "english" && newVal == 3) newVal++;
				if (newVal > 3) newVal = 0;

				_selectedGameSettings.patchLang = newVal;
				_selectedGameSettings.Save(PerGameConfigFile);
				fastObjectListView1_SelectedIndexChanged(null, null);
			}
		}

		private void fpanel_joystick_DoubleClick(object sender, EventArgs e)
		{
			if (_selectedGame != null)
			{
				var PerGameConfigFile = Path.Combine(Program.GameOptionsFolder, Path.GetFileNameWithoutExtension(_selectedGame.UserConfigFile) + ".json");
				if (_selectedGameSettings == null) _selectedGameSettings = new GameSettings();
				int newVal = _selectedGameSettings.favorJoystick + 1;
				if (newVal > 2) newVal = 0;
				_selectedGameSettings.favorJoystick = newVal;
				_selectedGameSettings.Save(PerGameConfigFile);
				fastObjectListView1_SelectedIndexChanged(null, null);
			}
		}

		private void fpanel_cross1_DoubleClick(object sender, EventArgs e)
		{
			if (_selectedGame != null)
			{
				var PerGameConfigFile = Path.Combine(Program.GameOptionsFolder, Path.GetFileNameWithoutExtension(_selectedGame.UserConfigFile) + ".json");
				if (_selectedGameSettings == null) _selectedGameSettings = new GameSettings();
				int newVal = _selectedGameSettings.gunA_crosshair + 1;
				if (newVal > 2) newVal = 0;
				_selectedGameSettings.gunA_crosshair = newVal;
				_selectedGameSettings.Save(PerGameConfigFile);
				fastObjectListView1_SelectedIndexChanged(null, null);
			}
		}

		private void fpanel_cross2_DoubleClick(object sender, EventArgs e)
		{
			if (_selectedGame != null)
			{
				var PerGameConfigFile = Path.Combine(Program.GameOptionsFolder, Path.GetFileNameWithoutExtension(_selectedGame.UserConfigFile) + ".json");
				if (_selectedGameSettings == null) _selectedGameSettings = new GameSettings();
				int newVal = _selectedGameSettings.gunB_crosshair + 1;
				if (newVal > 2) newVal = 0;
				_selectedGameSettings.gunB_crosshair = newVal;
				_selectedGameSettings.Save(PerGameConfigFile);
				fastObjectListView1_SelectedIndexChanged(null, null);
			}
		}

		private void fpanel_nativeCrosshair_DoubleClick(object sender, EventArgs e)
		{
			if (_selectedGame != null)
			{
				var PerGameConfigFile = Path.Combine(Program.GameOptionsFolder, Path.GetFileNameWithoutExtension(_selectedGame.UserConfigFile) + ".json");
				if (_selectedGameSettings == null) _selectedGameSettings = new GameSettings();
				if (_selectedGameSettings == null) _selectedGameSettings = new GameSettings();
				int newVal = _selectedGameSettings.gunA_crosshair + 1;
				if (newVal > 2) newVal = 0;
				_selectedGameSettings.gunA_crosshair = newVal;
				_selectedGameSettings.gunB_crosshair = newVal;
				_selectedGameSettings.Save(PerGameConfigFile);
				fastObjectListView1_SelectedIndexChanged(null, null);
			}
		}



		private void fpanel_overrideTpController_DoubleClick(object sender, EventArgs e)
		{
			if (_selectedGame != null)
			{
				var PerGameConfigFile = Path.Combine(Program.GameOptionsFolder, Path.GetFileNameWithoutExtension(_selectedGame.UserConfigFile) + ".json");
				if (_selectedGameSettings == null) _selectedGameSettings = new GameSettings();
				_selectedGameSettings.overrideTpController = !_selectedGameSettings.overrideTpController;
				_selectedGameSettings.Save(PerGameConfigFile);
				fastObjectListView1_SelectedIndexChanged(null, null);
			}
		}

		private void fpanel_overrideTpGameSettings_DoubleClick(object sender, EventArgs e)
		{
			if (_selectedGame != null)
			{
				var PerGameConfigFile = Path.Combine(Program.GameOptionsFolder, Path.GetFileNameWithoutExtension(_selectedGame.UserConfigFile) + ".json");
				if (_selectedGameSettings == null) _selectedGameSettings = new GameSettings();
				_selectedGameSettings.overrideTpGameSettings = !_selectedGameSettings.overrideTpGameSettings;
				_selectedGameSettings.Save(PerGameConfigFile);
				fastObjectListView1_SelectedIndexChanged(null, null);
			}
		}

		private void fpanel_applyPatches_DoubleClick(object sender, EventArgs e)
		{
			if (_selectedGame != null)
			{
				var PerGameConfigFile = Path.Combine(Program.GameOptionsFolder, Path.GetFileNameWithoutExtension(_selectedGame.UserConfigFile) + ".json");
				if (_selectedGameSettings == null) _selectedGameSettings = new GameSettings();
				_selectedGameSettings.applyPatches = !_selectedGameSettings.applyPatches;
				_selectedGameSettings.Save(PerGameConfigFile);
				fastObjectListView1_SelectedIndexChanged(null, null);
			}
		}

		private void fpanel_useThirdParty_DoubleClick(object sender, EventArgs e)
		{
			if (_selectedGame != null)
			{
				var PerGameConfigFile = Path.Combine(Program.GameOptionsFolder, Path.GetFileNameWithoutExtension(_selectedGame.UserConfigFile) + ".json");
				if (_selectedGameSettings == null) _selectedGameSettings = new GameSettings();
				_selectedGameSettings.useThirdParty = !_selectedGameSettings.useThirdParty;
				_selectedGameSettings.Save(PerGameConfigFile);
				fastObjectListView1_SelectedIndexChanged(null, null);
			}
		}

		private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void flowLayoutPanel5_Paint(object sender, PaintEventArgs e)
		{

		}

		private void lbl_joystick_Click(object sender, EventArgs e)
		{

		}

		private void label2_Click(object sender, EventArgs e)
		{

		}

		private void lbl_nativeCrosshair_Click(object sender, EventArgs e)
		{

		}
	}

	public class UpdatePatchArchive
	{
		public float version = Program.version;
		public string Game = "";
		public bool tpPatches = false;
		public bool gamePatches = false;
		public bool magpiePatches = false;
	}

}
