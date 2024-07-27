using Henooh.DeviceEmulator.Net;
using Krypton.Toolkit;
using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SDL2;
using SerialPortLib2;
using SharpDX.DirectInput;
using SharpDX.Multimedia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Net.Sockets;
using System.Runtime.Intrinsics.X86;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Effects;
using System.Xml;
using System.Xml.Linq;
using TeknoparrotAutoXinput.Properties;
using TeknoParrotUi.Common;
using WiimoteLib;
using XInput.Wrapper;
using Image = System.Drawing.Image;

namespace TeknoparrotAutoXinput
{
	public partial class Main : KryptonForm
	{
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
					btn_playgamedirect.Enabled = false;
					btn_playgamedirect2.Enabled = false;
				}
				else
				{
					if (_playAutoEnabled) btn_playgame.Enabled = true;
					if (_playDirectEnabled) btn_playgamedirect.Enabled = true;
					if (_playDirectEnabled) btn_playgamedirect2.Enabled = true;
				}

			}
		}


		private bool _playDirectEnabled = false;
		private bool _playAutoEnabled = false;

		List<string> typeConfig = new List<string>();

		int testval = 0;

		public Main()
		{
			/*
			string[] portNames = SerialPortLib2.SerialPortInput.GetPorts();
			if (portNames.Length != 0)
				Array.Sort<string>(portNames);
			ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("Select * from WIN32_SerialPort");
			*/
			//this.AutoScaleMode = AutoScaleMode.None;
			//Font = new Font(Font.Name, 8.25f * 96f / CreateGraphics().DpiX, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);
			//Font = new Font(Font.Name, Font.Size * 96f / CreateGraphics().DpiX, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);

			//this.AutoScaleMode = AutoScaleMode.Dpi;
			InitializeComponent();
			this.Activated += VotreForm_Activated;
			this.Deactivate += VotreForm_Deactivate;

			chk_showAll.Checked = ConfigurationManager.MainConfig.ShowAllGames;

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
			list_games.Items.Clear();
			_gameList = new Dictionary<string, Game>();
			_tpFolder = ConfigurationManager.MainConfig.TpFolder;
			if (Directory.Exists(_tpFolder))
			{
				string UserProfileDir = Path.Combine(Path.GetFullPath(_tpFolder), "UserProfiles");
				if (Directory.Exists(UserProfileDir))
				{
					list_games.Enabled = true;
					_userProfileFolder = UserProfileDir;
					var profileList = Directory.GetFiles(_userProfileFolder, "*.xml");
					foreach (var profile in profileList)
					{
						if (profile.ToLower().EndsWith("custom.xml")) continue;
						string gameName = ExtractGameNameInternal(profile);
						if (!_gameList.ContainsKey(gameName))
						{
							var newGame = new Game();
							newGame.Name = gameName;
							newGame.UserConfigFile = profile;
							newGame.FileName = Path.GetFileName(profile);
							newGame.Metadata = DeSerializeMetadata(profile);
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

							_gameList.Add(newGame.Name, newGame);
						}
					}
					List<string> sortedGameList = _gameList.Keys.OrderBy(key => key).ToList();
					foreach (var gameName in sortedGameList)
					{
						if (_gameList[gameName].existingConfig.Count > 0)
						{
							list_games.Items.Add(gameName);
						}
						else
						{
							if (chk_showAll.Checked)
							{
								list_games.Items.Add(gameName + " [NOT SUPPORTED]");
							}

						}



					}




				}

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

		static string ExtractGameNameInternal(string cheminFichierXml)
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

		public static Metadata DeSerializeMetadata(string fileName)
		{
			string ParentPath = Path.GetDirectoryName(fileName);
			ParentPath = Path.GetDirectoryName(ParentPath);

			var metadataPath = Path.Combine(ParentPath, "Metadata", Path.GetFileNameWithoutExtension(fileName) + ".json");
			if (File.Exists(metadataPath))
			{
				try
				{
					return JsonConvert.DeserializeObject<Metadata>(File.ReadAllText(metadataPath));
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

		private void Main_Load(object sender, EventArgs e)
		{
			cmb_displayMode.SelectedIndex = 0;
			cmb_patchReshade.SelectedIndex = 0;
			cmb_resolution.SelectedIndex = 0;
			cmb_patchlink.SelectedIndex = 0;
		}

		private void btn_globalconfig_Click(object sender, EventArgs e)
		{
			var frm = new Form1();
			var result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{
				Reload();
			}
		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void chk_showAll_CheckedChanged(object sender, EventArgs e)
		{
			ConfigurationManager.MainConfig.ShowAllGames = chk_showAll.Checked;
			ConfigurationManager.SaveConfig();
			Reload();
		}

		private void hg_controllerState_Paint(object sender, PaintEventArgs e)
		{

		}

		private void kryptonLabel6_Click(object sender, EventArgs e)
		{

		}

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

		private void pictureBox1_Click(object sender, EventArgs e)
		{

		}

		private void list_games_SelectedIndexChanged(object sender, EventArgs e)
		{
			btn_playgame.Enabled = false;
			btn_playgamedirect.Enabled = false;
			btn_playgamedirect2.Enabled = false;
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
			if (list_games.SelectedItems.Count > 0)
			{
				btn_playgamedirect.Enabled = true;
				btn_playgamedirect2.Enabled = true;
				string GameSelected = list_games.SelectedItems[0].ToString();
				GameSelected = GameSelected.Replace(" [NOT SUPPORTED]", "");
				if (_gameList.ContainsKey(GameSelected))
				{
					var DataGame = _gameList[GameSelected];
					lbl_GameTitle.Text = DataGame.Name;





					int gpuResolution = ConfigurationManager.MainConfig.gpuResolution;
					int displayMode = ConfigurationManager.MainConfig.displayMode;
					bool patchReshade = ConfigurationManager.MainConfig.patchReshade;

					string gpuResolutionSource = "Global";
					string displayModeSource = "Global";
					string patchReshadeSource = "Global";
					GameSettings gameOptions = null;
					string optionFile = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "gameoptions", Path.GetFileNameWithoutExtension(DataGame.UserConfigFile) + ".json");
					if (File.Exists(optionFile))
					{
						gameOptions = new GameSettings(File.ReadAllText(optionFile));
						gpuResolution = gameOptions.gpuResolution > 0 ? (gameOptions.gpuResolution - 1) : gpuResolution;
						displayMode = gameOptions.displayMode > 0 ? (gameOptions.displayMode - 1) : displayMode;
						patchReshade = gameOptions.patchReshade > 0 ? (gameOptions.patchReshade == 1 ? true : false) : patchReshade;
						if (gameOptions.gpuResolution > 0) gpuResolutionSource = "GameOption";
						if (gameOptions.displayMode > 0) displayModeSource = "GameOption";
						if (gameOptions.patchReshade > 0) patchReshadeSource = "GameOption";
					}

					if (displayMode == 0) cmb_displayMode.Items[0] = "Recommanded" + $" ({displayModeSource})";
					if (displayMode == 1) cmb_displayMode.Items[0] = "Fullscreen" + $" ({displayModeSource})";
					if (displayMode == 2) cmb_displayMode.Items[0] = "Windowed" + $" ({displayModeSource})";

					if (gpuResolution == 0) cmb_resolution.Items[0] = "720p" + $" ({gpuResolutionSource})";
					if (gpuResolution == 1) cmb_resolution.Items[0] = "1080p" + $" ({gpuResolutionSource})";
					if (gpuResolution == 2) cmb_resolution.Items[0] = "2k" + $" ({gpuResolutionSource})";
					if (gpuResolution == 3) cmb_resolution.Items[0] = "4k" + $" ({gpuResolutionSource})";

					cmb_patchReshade.Items[0] = (patchReshade ? "Yes" : "No") + $" ({patchReshadeSource})";

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
						if (_haveArcade && DataGame.existingConfig.ContainsKey("arcade"))
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
							if (gameOptions != null)
							{
								if (gameOptions.gunA_pump > 0) sindenPump = gameOptions.gunA_pump;
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
						_playAutoEnabled = false;
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



					}
				}
				else
				{

				}

			}
		}

		public static Image ResizeImageBest(Image image, Size newSize)
		{
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

		private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
		{

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

		private void kryptonLabel6_Click_1(object sender, EventArgs e)
		{

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
			if (list_games.SelectedItems.Count > 0)
			{
				btn_playgamedirect.Enabled = true;
				btn_playgamedirect2.Enabled = true;
				string GameSelected = list_games.SelectedItems[0].ToString();
				GameSelected = GameSelected.Replace(" [NOT SUPPORTED]", "");
				if (_gameList.ContainsKey(GameSelected))
				{
					finalConfig = _gameList[GameSelected].UserConfigFile;
				}
			}
			if (string.IsNullOrEmpty(finalConfig)) return;


			string arguments = "";
			if (cmb_displayMode.SelectedIndex > 0) arguments += $" --displayMode={cmb_displayMode.SelectedIndex}";
			if (cmb_resolution.SelectedIndex > 0) arguments += $" --resolution={cmb_resolution.SelectedIndex}";
			if (cmb_patchReshade.SelectedIndex > 0) arguments += $" --reshade={cmb_patchReshade.SelectedIndex}";
			if (cmb_patchlink.SelectedIndex > 0) arguments += $" --nolink";

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
			if (list_games.SelectedItems.Count > 0)
			{
				btn_playgamedirect.Enabled = true;
				btn_playgamedirect2.Enabled = true;
				string GameSelected = list_games.SelectedItems[0].ToString();
				GameSelected = GameSelected.Replace(" [NOT SUPPORTED]", "");
				if (_gameList.ContainsKey(GameSelected))
				{
					finalConfig = _gameList[GameSelected].UserConfigFile;
				}
			}
			if (string.IsNullOrEmpty(finalConfig)) return;

			string arguments = $"--passthrough ";
			if (cmb_displayMode.SelectedIndex > 0) arguments += $" --displayMode={cmb_displayMode.SelectedIndex}";
			if (cmb_resolution.SelectedIndex > 0) arguments += $" --resolution={cmb_resolution.SelectedIndex}";
			if (cmb_patchReshade.SelectedIndex > 0) arguments += $" --reshade={cmb_patchReshade.SelectedIndex}";
			if (cmb_patchlink.SelectedIndex > 0) arguments += $" --nolink";
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
			/*
			string finalConfig = "";
			if (list_games.SelectedItems.Count > 0)
			{
				btn_playgamedirect.Enabled = true;
				string GameSelected = list_games.SelectedItems[0].ToString();
				GameSelected = GameSelected.Replace(" [NOT SUPPORTED]", "");
				if (_gameList.ContainsKey(GameSelected))
				{
					finalConfig = _gameList[GameSelected].UserConfigFile;
				}
			}
			if (string.IsNullOrEmpty(finalConfig)) return;


			string teknoparrotExe = Path.Combine(_tpFolder, "TeknoParrotUi.exe");
			if (!File.Exists(teknoparrotExe)) return;

			isPlaying = true;

			await Task.Run(() =>
			{
				ProcessStartInfo startInfo = new ProcessStartInfo
				{
					FileName = teknoparrotExe,
					Arguments = "--profile=\"" + finalConfig + "\"",
					WorkingDirectory = Path.GetDirectoryName(teknoparrotExe),
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
			*/
		}

		private void btn_gameoptions_Click(object sender, EventArgs e)
		{
			if (list_games.SelectedItems.Count == 0)
			{
				return;
			}

			string GameSelected = list_games.SelectedItems[0].ToString();
			GameSelected = GameSelected.Replace(" [NOT SUPPORTED]", "");
			if (_gameList.ContainsKey(GameSelected))
			{
				var frm = new GameOptions(_gameList[GameSelected]);
				var result = frm.ShowDialog();
				if (result == DialogResult.OK)
				{

				}
			}



		}

		private void button1_Click(object sender, EventArgs e)
		{
			Thread.Sleep(2000);

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

		private void button1_Click_1(object sender, EventArgs e)
		{
			string finalConfig = "";
			if (list_games.SelectedItems.Count > 0)
			{
				string GameSelected = list_games.SelectedItems[0].ToString();
				GameSelected = GameSelected.Replace(" [NOT SUPPORTED]", "");
				if (_gameList.ContainsKey(GameSelected))
				{
					finalConfig = _gameList[GameSelected].UserConfigFile;
				}
			}
			if (string.IsNullOrEmpty(finalConfig)) return;
			finalConfig = finalConfig.Replace("UserProfiles", "GameProfiles");
			if (File.Exists(finalConfig))
			{
				string finalConfigData = File.ReadAllText(finalConfig);
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(finalConfigData);
				XmlNodeList joystickButtonsNodes = xmlDoc.SelectNodes("/GameProfile/JoystickButtons/JoystickButtons");
				string result = "";
				foreach (XmlNode node in joystickButtonsNodes)
				{
					XmlNode buttonNameNode = node.SelectSingleNode("ButtonName");
					string buttonName = "";
					if (buttonNameNode != null && !string.IsNullOrEmpty(buttonNameNode.InnerText)) buttonName = buttonNameNode.InnerText;

					bool hideWithDirectInput = false;
					XmlNode HideWithDirectInputNode = node.SelectSingleNode("HideWithDirectInput");
					if (HideWithDirectInputNode != null && !string.IsNullOrEmpty(HideWithDirectInputNode.InnerText)) hideWithDirectInput = HideWithDirectInputNode.InnerText.ToLower() == "true" ? true : false;

					bool hideWithoutRelativeAxis = false;
					XmlNode HideWithoutRelativeAxisNode = node.SelectSingleNode("HideWithoutRelativeAxis");
					if (HideWithoutRelativeAxisNode != null && !string.IsNullOrEmpty(HideWithoutRelativeAxisNode.InnerText)) hideWithoutRelativeAxis = HideWithoutRelativeAxisNode.InnerText.ToLower() == "true" ? true : false;


					bool hideWithoutKeyboardForAxis = false;
					XmlNode HideWithoutKeyboardForAxisNode = node.SelectSingleNode("HideWithoutKeyboardForAxis");
					if (HideWithoutKeyboardForAxisNode != null && !string.IsNullOrEmpty(HideWithoutKeyboardForAxisNode.InnerText)) hideWithoutKeyboardForAxis = HideWithoutKeyboardForAxisNode.InnerText.ToLower() == "true" ? true : false;




					if (buttonName != "" && !hideWithDirectInput && !hideWithoutRelativeAxis && !hideWithoutKeyboardForAxis) result += buttonName + "\n";
				}
				MessageBox.Show(result);
				Clipboard.SetText(result);

			}


		}

		private void button2_Click(object sender, EventArgs e)
		{
			string finalConfig = "";
			if (list_games.SelectedItems.Count > 0)
			{
				string GameSelected = list_games.SelectedItems[0].ToString();
				GameSelected = GameSelected.Replace(" [NOT SUPPORTED]", "");
				if (_gameList.ContainsKey(GameSelected))
				{
					finalConfig = _gameList[GameSelected].UserConfigFile;
				}
			}
			if (string.IsNullOrEmpty(finalConfig)) return;
			Dictionary<string, string> XiToDi = new Dictionary<string, string>();
			//finalConfig = finalConfig.Replace("UserProfiles", "GameProfiles");
			if (File.Exists(finalConfig))
			{
				string finalConfigData = File.ReadAllText(finalConfig);
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(finalConfigData);
				XmlNodeList joystickButtonsNodes = xmlDoc.SelectNodes("/GameProfile/JoystickButtons/JoystickButtons");
				string result = "";
				foreach (XmlNode node in joystickButtonsNodes)
				{
					XmlNode buttonNameNodeXi = node.SelectSingleNode("BindNameXi");
					string buttonNameXi = "";
					if (buttonNameNodeXi != null && !string.IsNullOrEmpty(buttonNameNodeXi.InnerText)) buttonNameXi = buttonNameNodeXi.InnerText;

					XmlNode buttonNameNodeDi = node.SelectSingleNode("BindNameDi");
					string buttonNameDi = "";
					if (buttonNameNodeDi != null && !string.IsNullOrEmpty(buttonNameNodeDi.InnerText)) buttonNameDi = buttonNameNodeDi.InnerText;

					result += $"{buttonNameXi} => {buttonNameDi} \n";
					if (buttonNameXi != "")
					{
						XiToDi[buttonNameXi] = buttonNameDi;
					}

				}
				string json = JsonConvert.SerializeObject(XiToDi, Newtonsoft.Json.Formatting.Indented);
				File.WriteAllText("XiToDi.json", json);
				MessageBox.Show(json);
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			var frm = new Wizard();
			var result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{

			}
		}

		private void button4_Click(object sender, EventArgs e)
		{
			string msg = "F1x2x2x";
			var _serialPort = new SerialPortInput(false);

			_serialPort.SetPort(@"COM28", 9600);
			_serialPort.Connect();
			byte[] bytes = Encoding.ASCII.GetBytes(msg);
			_serialPort.SendMessage(bytes);
			_serialPort.Disconnect();
			/*
			if (!_serialPort.IsConnected)
			{
				_serialPort.Connect();
			}

			_serialPort.SendMessage(bytes);
			*/
		}

		private void button5_Click(object sender, EventArgs e)
		{
			NamedPipeClientStream sindengunA = null;
			StreamWriter writer = null;
			if (sindengunA == null)
			{
				sindengunA = new NamedPipeClientStream(".", "RecoilSindenGunA", PipeDirection.Out);
			}
			if (writer == null)
			{
				writer = new StreamWriter(sindengunA, Encoding.UTF8);
			}

			if (!sindengunA.IsConnected)
			{
				try
				{
					sindengunA.Connect();
				}
				catch (Exception ex)
				{
					try
					{
						writer.Close();
						sindengunA.Close();

					}
					catch { }

					sindengunA.Dispose();
					sindengunA = null;
					writer.Dispose();
					writer = null;
					return;
				}
			}
			try
			{
				writer.Write("1");
				writer.Flush();
			}
			catch (Exception ex)
			{
				try
				{
					writer.Close();
					sindengunA.Close();
				}
				catch { }

				sindengunA.Dispose();
				sindengunA = null;
				writer.Dispose();
				writer = null;
				return;
			}
		}

		private void button6_Click(object sender, EventArgs e)
		{
			string target_Guid = ButtonToKey.DSharpGuidToSDLGuid("753c3020-37ae-11ee-8001-444553540000");
			//0300fa675e0400008e02000000007801
			int found_guid = -1;

			SDL2.SDL.SDL_Quit();
			SDL2.SDL.SDL_SetHint(SDL2.SDL.SDL_HINT_JOYSTICK_RAWINPUT, "0");
			SDL2.SDL.SDL_Init(SDL2.SDL.SDL_INIT_JOYSTICK | SDL2.SDL.SDL_INIT_GAMECONTROLLER);


			SDL2.SDL.SDL_JoystickUpdate();
			for (int i = 0; i < SDL2.SDL.SDL_NumJoysticks(); i++)
			{
				if (SDL.SDL_IsGameController(i) == SDL.SDL_bool.SDL_FALSE) continue;
				var currentJoy = SDL.SDL_JoystickOpen(i);
				string nameController = SDL2.SDL.SDL_JoystickNameForIndex(i).Trim('\0');
				{
					const int bufferSize = 256; // La taille doit être au moins 33 pour stocker le GUID sous forme de chaîne (32 caractères + le caractère nul)
					byte[] guidBuffer = new byte[bufferSize];
					SDL.SDL_JoystickGetGUIDString(SDL.SDL_JoystickGetGUID(currentJoy), guidBuffer, bufferSize);
					string guidString = System.Text.Encoding.UTF8.GetString(guidBuffer).Trim('\0');
					if (guidString == target_Guid)
					{
						found_guid = i;
						SDL.SDL_JoystickClose(currentJoy);
						//break;
					}
					SDL.SDL_JoystickClose(currentJoy);
				}
			}

			if (found_guid >= 0)
			{
				if (SDL.SDL_IsGameController(found_guid) == SDL.SDL_bool.SDL_FALSE)
				{
					//var currentJoy = SDL.SDL_JoystickOpen(found_guid);
					var gGameController = SDL.SDL_GameControllerOpen(found_guid);
					//currentJoy = SDL.SDL_GameControllerGetJoystick(gGameController);

					//string nameController = SDL2.SDL.SDL_JoystickName(currentJoy).Trim('\0');

					SDL.SDL_JoystickRumble(SDL.SDL_GameControllerGetJoystick(gGameController), 0xFFFF, 0xFFFF, 100);

					//SDL.SDL_JoystickRumble(currentJoy, 0xAAAA, 0xAAAA, 1000);
					Thread.Sleep(120);
					SDL.SDL_JoystickRumble(SDL.SDL_GameControllerGetJoystick(gGameController), 0, 0, 0);

				}


			}






			SDL2.SDL.SDL_Quit();



		}

		private void button7_Click(object sender, EventArgs e)
		{

		}

		private void button8_Click(object sender, EventArgs e)
		{
			/*
			GameSettings testgs = new GameSettings();

			string json = @"
{
    ""RunAsRoot"": ""true"",
    ""valueStooz_Wheel"": ""100"",
	""injectorDllList"": ""zog.dll""
}
";

			testgs.Overwrite(@"K:\test.json",new List<string> { "1080p" });
			*/





		}

		private void kryptonRadioButton1_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void kryptonLabel4_Click(object sender, EventArgs e)
		{

		}

		private void button9_Click(object sender, EventArgs e)
		{
			/*
			string baseTpDir = Directory.GetParent(Path.GetDirectoryName(Path.GetFullPath(xmlFile))).FullName;
			string originalConfigFileName = Path.GetFileName(xmlFile);
			string originalConfigFileNameWithoutExt = Path.GetFileNameWithoutExtension(xmlFile);
			string teknoparrotExe = Path.Combine(baseTpDir, "TeknoParrotUi.exe");

			GameSettings gameOptions = new GameSettings();
			string optionFile = Path.Combine(GameOptionsFolder, originalConfigFileNameWithoutExt + ".json");
			if (File.Exists(optionFile))
			{
				Utils.LogMessage($"gameoveride file found : " + optionFile);
				gameOptions = new GameSettings(File.ReadAllText(optionFile));
			}

			bool patchGpuFix = ConfigurationManager.MainConfig.patchGpuFix;
			bool patchGpuTP = ConfigurationManager.MainConfig.patchGpuTP;

			int gpuResolution = ConfigurationManager.MainConfig.gpuResolution;
			bool patchResolutionFix = ConfigurationManager.MainConfig.patchResolutionFix;
			bool patchResolutionTP = ConfigurationManager.MainConfig.patchResolutionTP;

			int displayMode = ConfigurationManager.MainConfig.displayMode;
			bool patchDisplayModeFix = ConfigurationManager.MainConfig.patchDisplayModeFix;
			bool patchDisplayModeTP = ConfigurationManager.MainConfig.patchDisplayModeTP;


			bool patchReshade = ConfigurationManager.MainConfig.patchReshade;
			bool patchGameID = ConfigurationManager.MainConfig.patchGameID;
			bool patchNetwork = ConfigurationManager.MainConfig.patchNetwork;
			bool patchOtherTPSettings = ConfigurationManager.MainConfig.patchOtherTPSettings;
			bool patchOthersGameOptions = ConfigurationManager.MainConfig.patchOthersGameOptions;
			bool patchFFB = ConfigurationManager.MainConfig.patch_FFB;

			patchGpuFix = gameOptions.patchGpuFix > 0 ? (gameOptions.patchGpuFix == 1 ? true : false) : patchGpuFix;
			patchGpuTP = gameOptions.patchGpuTP > 0 ? (gameOptions.patchGpuTP == 1 ? true : false) : patchGpuTP;

			gpuResolution = gameOptions.gpuResolution > 0 ? (gameOptions.gpuResolution - 1) : gpuResolution;
			patchResolutionFix = gameOptions.patchResolutionFix > 0 ? (gameOptions.patchResolutionFix == 1 ? true : false) : patchResolutionFix;
			patchResolutionTP = gameOptions.patchResolutionTP > 0 ? (gameOptions.patchResolutionTP == 1 ? true : false) : patchResolutionTP;

			displayMode = gameOptions.displayMode > 0 ? (gameOptions.displayMode - 1) : displayMode;
			patchDisplayModeFix = gameOptions.patchDisplayModeFix > 0 ? (gameOptions.patchDisplayModeFix == 1 ? true : false) : patchDisplayModeFix;
			patchDisplayModeTP = gameOptions.patchDisplayModeTP > 0 ? (gameOptions.patchDisplayModeTP == 1 ? true : false) : patchDisplayModeTP;

			patchReshade = gameOptions.patchReshade > 0 ? (gameOptions.patchReshade == 1 ? true : false) : patchReshade;
			patchGameID = gameOptions.patchGameID > 0 ? (gameOptions.patchGameID == 1 ? true : false) : patchGameID;
			patchFFB = gameOptions.patchFFB > 0 ? (gameOptions.patchFFB == 1 ? true : false) : patchFFB;
			patchNetwork = gameOptions.patchNetwork > 0 ? (gameOptions.patchNetwork == 1 ? true : false) : patchNetwork;
			patchOtherTPSettings = gameOptions.patchOtherTPSettings > 0 ? (gameOptions.patchOtherTPSettings == 1 ? true : false) : patchOtherTPSettings;
			if (gameOptions.patchOthersGameOptions > 0)
			{
				if (gameOptions.patchOthersGameOptions == 1)
				{
					patchOtherTPSettings = true;
				}
				if (gameOptions.patchOthersGameOptions == 2)
				{
					patchOtherTPSettings = true;
				}
				if (gameOptions.patchOthersGameOptions == 3)
				{
					patchOtherTPSettings = false;
				}
			}


			TpSettingsManager.tags = new List<string>();
			if (patchGpuTP)
			{
				if (ConfigurationManager.MainConfig.gpuType == 0) TpSettingsManager.tags.Add("nvidia");
				if (ConfigurationManager.MainConfig.gpuType == 1) TpSettingsManager.tags.Add("intel");
				if (ConfigurationManager.MainConfig.gpuType == 2) TpSettingsManager.tags.Add("amdold");
				if (ConfigurationManager.MainConfig.gpuType == 3) TpSettingsManager.tags.Add("amdnew");
				if (ConfigurationManager.MainConfig.gpuType == 4) TpSettingsManager.tags.Add("amdrid");
				if (ConfigurationManager.MainConfig.gpuType >= 2) TpSettingsManager.tags.Add("amd");

				if (ConfigurationManager.MainConfig.gpuType != 0)
				{
					TpSettingsManager.tags.Add("!intel");
					TpSettingsManager.tags.Add("!amd");
				}
				if (ConfigurationManager.MainConfig.gpuType != 1)
				{
					TpSettingsManager.tags.Add("!nvidia");
					TpSettingsManager.tags.Add("!amd");
				}
				if (ConfigurationManager.MainConfig.gpuType < 2)
				{
					TpSettingsManager.tags.Add("!nvidia");
					TpSettingsManager.tags.Add("!intel");
				}

			}

			if (patchGpuTP) TpSettingsManager.tags.Add("use_gpu_fix_in_tp_settings"); //Apply gpu amd/intel/nvidia fix in TP
			else TpSettingsManager.tags.Add("!use_gpu_fix_in_tp_settings");

			if (patchGpuFix) TpSettingsManager.tags.Add("use_gpu_fix_in_patches"); //Apply gpu amd/intel/nvidia fix in TP
			else TpSettingsManager.tags.Add("!use_gpu_fix_in_patches"); //Apply gpu amd/intel/nvidia fix in TP


			if (gpuResolution == 0) TpSettingsManager.tags.Add("720p");
			if (gpuResolution == 1) TpSettingsManager.tags.Add("1080p");
			if (gpuResolution == 2) TpSettingsManager.tags.Add("2k");
			if (gpuResolution == 3) TpSettingsManager.tags.Add("4k");

			if (gpuResolution != 0) TpSettingsManager.tags.Add("!720p");
			if (gpuResolution != 1) TpSettingsManager.tags.Add("!1080p");
			if (gpuResolution != 2) TpSettingsManager.tags.Add("!2k");
			if (gpuResolution != 3) TpSettingsManager.tags.Add("!4k");

			if (patchResolutionTP) TpSettingsManager.tags.Add("set_res_in_tp_settings");
			if (patchResolutionFix) TpSettingsManager.tags.Add("fix_res_in_patches");


			if (displayMode == 0) TpSettingsManager.tags.Add("set_displaymode_recommanded"); //Apply Res & Fullscreen in TP
			if (displayMode == 1) TpSettingsManager.tags.Add("set_fullscreen"); //Apply Res & Fullscreen in TP
			if (displayMode == 2) TpSettingsManager.tags.Add("set_windowed"); //Apply Res & Fullscreen in TP
			if (patchDisplayModeTP) TpSettingsManager.tags.Add("set_displaymode_in_tp_settings");
			else TpSettingsManager.tags.Add("!set_displaymode_in_tp_settings");

			if (patchDisplayModeFix) TpSettingsManager.tags.Add("fix_displaymode_in_patches");
			else TpSettingsManager.tags.Add("!fix_displaymode_in_patches");

			if (patchReshade) TpSettingsManager.tags.Add("use_optional_reshade");
			else TpSettingsManager.tags.Add("!use_optional_reshade");


			if (patchGameID) TpSettingsManager.tags.Add("replace_gameid");
			if (patchNetwork) TpSettingsManager.tags.Add("replace_network");
			if (patchOtherTPSettings) TpSettingsManager.tags.Add("recommanded_tp_settings");
			if (patchOthersGameOptions) TpSettingsManager.tags.Add("recommanded_gameoptions");
			if (patchFFB) TpSettingsManager.tags.Add("ffb");


			//Tag Define Part 2
			if (useXinput) TpSettingsManager.tags.Add("xinput");
			if (_useDinputWheel) TpSettingsManager.tags.Add("dwheel");
			if (useDinputHotas) TpSettingsManager.tags.Add("dhotas");
			if (useDinputLightGun) TpSettingsManager.tags.Add("dlightgun");
			if (shifterGuidFound) TpSettingsManager.tags.Add("shifter");
			else TpSettingsManager.tags.Add("!shifter");


			if (!hideCrosshair) TpSettingsManager.tags.Add("show_crosshair");
			if (hideCrosshair) TpSettingsManager.tags.Add("hide_crosshair");
			if (crosshairA && crosshairB) TpSettingsManager.tags.Add("crosshair_gun1_and_gun2");
			if (crosshairA && !crosshairB) TpSettingsManager.tags.Add("crosshair_gun1_only");
			if (!crosshairA && crosshairB) TpSettingsManager.tags.Add("crosshair_gun2_only");

			if (useDinputLightGun && (GunAType == "sinden" || GunBType == "sinden"))
			{
				TpSettingsManager.tags.Add("at_least_one_sinden");
				atLeastOneSinden = true;
			}
			else
			{
				atLeastOneSinden = false;
				TpSettingsManager.tags.Add("!at_least_one_sinden");
				if (useDinputLightGun) TpSettingsManager.tags.Add("no_sinden");
			}

			//We only set sinden soft calibration if all sinden are not using vjoy
			if (useDinputLightGun && dinputLightgunAFound)
			{
				if (GunAType == "sinden" && vjoy_gunA) allSindenWithoutVjoy = false;
			}
			if (useDinputLightGun && dinputLightgunBFound)
			{
				if (GunBType == "sinden" && vjoy_gunB) allSindenWithoutVjoy = false;
			}
			if (allSindenWithoutVjoy) TpSettingsManager.tags.Add("no_sinden_using_vjoy");

			*/
		}

		private void cmb_resolution_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void kryptonLabel8_Click(object sender, EventArgs e)
		{

		}

		private void kryptonComboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{

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
			if (list_games.SelectedItems.Count > 0)
			{
				btn_playgamedirect.Enabled = true;
				btn_playgamedirect2.Enabled = true;
				string GameSelected = list_games.SelectedItems[0].ToString();
				GameSelected = GameSelected.Replace(" [NOT SUPPORTED]", "");
				if (_gameList.ContainsKey(GameSelected))
				{
					finalConfig = _gameList[GameSelected].UserConfigFile;
				}
			}
			if (string.IsNullOrEmpty(finalConfig)) return;

			string arguments = $"--fullpassthrough ";
			if (cmb_displayMode.SelectedIndex > 0) arguments += $" --displayMode={cmb_displayMode.SelectedIndex}";
			if (cmb_resolution.SelectedIndex > 0) arguments += $" --resolution={cmb_resolution.SelectedIndex}";
			if (cmb_patchReshade.SelectedIndex > 0) arguments += $" --reshade={cmb_patchReshade.SelectedIndex}";
			if (cmb_patchlink.SelectedIndex > 0) arguments += $" --nolink";
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
	}

	public class Game
	{
		public string Name;
		public string FileName;
		public string UserConfigFile;
		public Metadata Metadata;
		public Dictionary<string, string> existingConfig = new Dictionary<string, string>();
	}
}
