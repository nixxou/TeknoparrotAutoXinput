using NCalc;
using Gma.System.MouseKeyHook;
using Henooh.DeviceEmulator.Net;
using Krypton.Toolkit;
using Newtonsoft.Json;
using SharpDX.DirectInput;
using SharpDX.Multimedia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WiimoteLib;
using XJoy;
using System.IO.Pipes;

namespace TeknoparrotAutoXinput
{
	public partial class VjoyControl : KryptonForm
	{
		private static NotifyIcon notifyIcon;
		private static ContextMenuStrip contextMenu;
		private System.Diagnostics.Process parentProcess = null;

		private bool isActivated = false;

		private string _lightgunA_Type = ConfigurationManager.MainConfig.gunAType;
		private string _lightgunB_Type = ConfigurationManager.MainConfig.gunBType;
		private JoystickButtonData _gunA_AnalogX = null;
		private JoystickButtonData _gunA_AnalogY = null;
		private JoystickButtonData _gunB_AnalogX = null;
		private JoystickButtonData _gunB_AnalogY = null;
		private bool _dinputLightgunAFound = false;
		private bool _dinputLightgunBFound = false;
		private int _indexVjoy = ConfigurationManager.MainConfig.indexvjoy;
		private bool _vjoyFound = false;

		private Thread MonitorGunA = null;
		private Thread MonitorGunB = null;
		private Thread MonitorVjoy = null;
		private static bool _stopListening;
		private List<DeviceInstance> devices = new List<DeviceInstance>();
		private readonly DirectInput _directInput = new DirectInput();

		private vJoyManager vJoyObj;
		List<HID_USAGES> AvailableHidValues;
		Dictionary<HID_USAGES, vJoyManager.AxisExtents> HidExtents;

		private ConfigurationVjoyControl _settingsGunA;
		private ConfigurationVjoyControl _settingsGunB;

		private IKeyboardMouseEvents _globalHook = null;

		private bool _isDialog = false;
		public string GunA_json = "";
		public string GunB_json = "";
		private GameSettings _gameOptions = null;
		private string _game = "";

		private bool _GunA_manual = false;
		public bool GunA_manual
		{
			get { return (_GunA_manual); }
			set
			{
				_GunA_manual = value;
				grp_manual_A.Enabled = value;
				btn_switchModeA.Text = _GunA_manual ? "Switch Mode to Gun (Ctrl+Numpad 0)" : "Switch Mode to Manual (Ctrl+Numpad 0)";
			}
		}

		private bool _GunB_manual = false;
		public bool GunB_manual
		{
			get { return (_GunB_manual); }
			set
			{
				_GunB_manual = value;
				grp_manual_B.Enabled = value;
				btn_switchModeB.Text = _GunB_manual ? "Switch Mode to Gun (Alt+Numpad 0)" : "Switch Mode to Manual (Alt+Numpad 0)";
			}
		}

		int x_original = -1;
		int y_original = -1;
		int x2_original = -1;
		int y2_original = -1;

		private bool _GunA_X_change = false;
		private bool _GunA_Y_change = false;
		private bool _GunB_X_change = false;
		private bool _GunB_Y_change = false;


		private int _GunA_X = 0;
		public int GunA_X
		{
			get { return (_GunA_X); }
			set
			{
				if (value != _GunA_X) _GunA_X_change = true;
				_GunA_X = value;
				lbl_gunAX.Text = value.ToString();
				//lbl_gunA_connected.Text = value.ToString();
			}
		}

		private int _GunA_Y = 0;
		public int GunA_Y
		{
			get { return (_GunA_Y); }
			set
			{
				if (value != _GunA_Y) _GunA_Y_change = true;
				_GunA_Y = value;
				lbl_gunAY.Text = value.ToString();
			}
		}

		private int _GunB_X = 0;
		public int GunB_X
		{
			get { return (_GunB_X); }
			set
			{
				if (value != _GunB_X) _GunB_X_change = true;
				_GunB_X = value;
				lbl_gunBX.Text = value.ToString();
			}
		}

		private int _GunB_Y = 0;
		public int GunB_Y
		{
			get { return (_GunB_Y); }
			set
			{
				if (value != _GunB_Y) _GunB_Y_change = true;
				_GunB_Y = value;
				lbl_gunBY.Text = value.ToString();
			}
		}

		Expression expAX = null;
		Expression expAY = null;
		Expression expBX = null;
		Expression expBY = null;

		private int _vjoyA_X = 0;
		public int vjoyA_X
		{
			get { return (_vjoyA_X); }
			set
			{
				_vjoyA_X = value;
				lbl_vjoyAX.Text = value.ToString();
			}
		}

		private int _vjoyA_Y = 0;
		public int vjoyA_Y
		{
			get { return (_vjoyA_Y); }
			set
			{
				_vjoyA_Y = value;
				lbl_vjoyAY.Text = value.ToString();
			}
		}

		private int _vjoyB_X = 0;
		public int vjoyB_X
		{
			get { return (_vjoyB_X); }
			set
			{
				_vjoyB_X = value;
				lbl_vjoyBX.Text = value.ToString();
			}
		}


		private int _vjoyB_Y = 0;
		public int vjoyB_Y
		{
			get { return (_vjoyB_Y); }
			set
			{
				_vjoyB_Y = value;
				lbl_vjoyBY.Text = value.ToString();
			}
		}

		public VjoyControl(bool isDialog, string game = "", GameSettings gameOptions = null)
		{

			_isDialog = isDialog;
			_gameOptions = gameOptions;
			_game = game;
			_indexVjoy = ConfigurationManager.MainConfig.indexvjoy;
			if (_gameOptions != null && _gameOptions.indexvjoy != -1) _indexVjoy = _gameOptions.indexvjoy;




			InitializeComponent();
			this.Activated += (sender, e) => { isActivated = true; };
			this.Deactivate += (sender, e) => { isActivated = false; };
			offset_AX.KeyUp += (sender, e) => offset_AX_ValueChanged(sender, e);
			offset_AY.KeyUp += (sender, e) => offset_AY_ValueChanged(sender, e);
			offset_BX.KeyUp += (sender, e) => offset_BX_ValueChanged(sender, e);
			offset_BY.KeyUp += (sender, e) => offset_BY_ValueChanged(sender, e);

			if (_isDialog)
			{
				this.MinimizeBox = false;
			}
			else
			{
				System.Diagnostics.Process p = ParentProcessUtilities.GetParentProcess();
				if (p != null)
				{
					parentProcess = p;
					timer1.Enabled = true;
				}

				btn_Cancel.Visible = false;
				// Créer le menu contextuel	
				contextMenu = new ContextMenuStrip();
				ToolStripMenuItem closeMenuItem = new ToolStripMenuItem("Close");
				closeMenuItem.Click += CloseMenuItem_Click;
				contextMenu.Items.Add(closeMenuItem);

				try
				{
					// Créer et afficher l'icône dans la barre des tâches
					notifyIcon = new NotifyIcon();
					notifyIcon.Icon = SystemIcons.Information;
					notifyIcon.ContextMenuStrip = contextMenu;
					notifyIcon.Visible = true;
					notifyIcon.Text = "TeknoparrotAutoXinput Vjoy Control Center";
					notifyIcon.DoubleClick += VjoyControl_DoubleClick;
					this.WindowState = FormWindowState.Minimized;
					//this.Hide();

				}
				catch { }



				





			}
			MessageBox.Show("xx");
			if (game == "") lbl_profile.Text = "Global";
			else lbl_profile.Text = game;

			bool checkDinputLightgun = false;

			if (!string.IsNullOrEmpty(_lightgunA_Type) && _lightgunA_Type != "<none>") checkDinputLightgun = true;
			if (!string.IsNullOrEmpty(_lightgunB_Type) && _lightgunB_Type != "<none>") checkDinputLightgun = true;
			string bindingDinputLightgunAJson = "";
			string bindingDinputLightgunBJson = "";
			Dictionary<string, JoystickButtonData> bindingDinputLightGunA = null;
			Dictionary<string, JoystickButtonData> bindingDinputLightGunB = null;


			string GunAGuid = "";
			string GunBGuid = "";
			if (_lightgunA_Type == "sinden" || _lightgunA_Type == "guncon1" || _lightgunA_Type == "guncon2" || _lightgunA_Type == "wiimote" || _lightgunA_Type == "gamepad")
			{
				if (_lightgunA_Type == "gamepad") bindingDinputLightgunAJson = ConfigurationManager.MainConfig.bindingDinputGunAXbox;
				if (_lightgunA_Type == "sinden") bindingDinputLightgunAJson = ConfigurationManager.MainConfig.bindingDinputGunASinden;
				if (_lightgunA_Type == "guncon1") bindingDinputLightgunAJson = ConfigurationManager.MainConfig.bindingDinputGunAGuncon1;
				if (_lightgunA_Type == "guncon2") bindingDinputLightgunAJson = ConfigurationManager.MainConfig.bindingDinputGunAGuncon2;
				if (_lightgunA_Type == "wiimote") bindingDinputLightgunAJson = ConfigurationManager.MainConfig.bindingDinputGunAWiimote;
				bindingDinputLightGunA = (Dictionary<string, JoystickButtonData>)JsonConvert.DeserializeObject<Dictionary<string, JoystickButtonData>>(bindingDinputLightgunAJson);
				if (bindingDinputLightGunA != null && bindingDinputLightGunA.ContainsKey("LightgunX") && bindingDinputLightGunA.ContainsKey("LightgunY"))
				{
					if (bindingDinputLightGunA["LightgunX"].JoystickGuid == bindingDinputLightGunA["LightgunY"].JoystickGuid && bindingDinputLightGunA["LightgunX"].IsAxis && bindingDinputLightGunA["LightgunY"].IsAxis)
					{
						GunAGuid = bindingDinputLightGunA["LightgunX"].JoystickGuid.ToString();
					}
				}
			}
			if (_lightgunB_Type == "sinden" || _lightgunB_Type == "guncon1" || _lightgunB_Type == "guncon2" || _lightgunB_Type == "wiimote" || _lightgunB_Type == "gamepad")
			{
				if (_lightgunB_Type == "gamepad") bindingDinputLightgunBJson = ConfigurationManager.MainConfig.bindingDinputGunBXbox;
				if (_lightgunB_Type == "sinden") bindingDinputLightgunBJson = ConfigurationManager.MainConfig.bindingDinputGunBSinden;
				if (_lightgunB_Type == "guncon1") bindingDinputLightgunBJson = ConfigurationManager.MainConfig.bindingDinputGunBGuncon1;
				if (_lightgunB_Type == "guncon2") bindingDinputLightgunBJson = ConfigurationManager.MainConfig.bindingDinputGunBGuncon2;
				if (_lightgunB_Type == "wiimote") bindingDinputLightgunBJson = ConfigurationManager.MainConfig.bindingDinputGunBWiimote;
				bindingDinputLightGunB = (Dictionary<string, JoystickButtonData>)JsonConvert.DeserializeObject<Dictionary<string, JoystickButtonData>>(bindingDinputLightgunBJson);
				if (bindingDinputLightGunB != null && bindingDinputLightGunB.ContainsKey("LightgunX") && bindingDinputLightGunB.ContainsKey("LightgunY"))
				{
					if (bindingDinputLightGunB["LightgunX"].JoystickGuid == bindingDinputLightGunB["LightgunY"].JoystickGuid && bindingDinputLightGunB["LightgunX"].IsAxis && bindingDinputLightGunB["LightgunY"].IsAxis)
					{
						GunBGuid = bindingDinputLightGunB["LightgunX"].JoystickGuid.ToString();
					}
				}
			}
			if (!string.IsNullOrEmpty(GunAGuid) || !string.IsNullOrEmpty(GunBGuid))
			{
				DirectInput directInput = new DirectInput();
				List<DeviceInstance> devices = new List<DeviceInstance>();
				devices.AddRange(directInput.GetDevices().Where(x => x.Type != SharpDX.DirectInput.DeviceType.Mouse && x.UsagePage != UsagePage.VendorDefinedBegin && x.Usage != UsageId.AlphanumericBitmapSizeX && x.Usage != UsageId.AlphanumericAlphanumericDisplay && x.UsagePage != unchecked((UsagePage)0xffffff43) && x.UsagePage != UsagePage.Vr).ToList());
				foreach (var device in devices)
				{
					if (device.InstanceGuid.ToString() == GunAGuid)
					{
						_dinputLightgunAFound = true;
						_gunA_AnalogX = bindingDinputLightGunA["LightgunX"];
						_gunA_AnalogY = bindingDinputLightGunA["LightgunY"];
					}
					if (device.InstanceGuid.ToString() == GunBGuid)
					{
						_gunB_AnalogX = bindingDinputLightGunB["LightgunX"];
						_gunB_AnalogY = bindingDinputLightGunB["LightgunY"];
						_dinputLightgunBFound = true;
					}
				}
			}
			lbl_gunA_connected.Text = _dinputLightgunAFound ? GunAGuid.ToString() : "Missing";
			lbl_gunB_connected.Text = _dinputLightgunBFound ? GunBGuid.ToString() : "Missing";
			lbl_vjoy.Text = "Vjoy " + _indexVjoy;

			int nbVjoyDevice = 0;

			vJoyObj = new vJoyManager();
			_vjoyFound = false;
			string errorVjoy = "";
			if (vJoyObj.vJoyEnabled())
			{
				VjdStat status = vJoyObj.m_joystick.GetVJDStatus((uint)(_indexVjoy));
				if (status == VjdStat.VJD_STAT_FREE)
				{
					_vjoyFound = true;
					vJoyObj.InitDevice((uint)(_indexVjoy));

					HidExtents = new Dictionary<HID_USAGES, vJoyManager.AxisExtents>();
					AvailableHidValues = vJoyObj.GetExistingAxes(vJoyObj.ActiveVJoyID);
					foreach (HID_USAGES axis in AvailableHidValues)
					{
						HidExtents.Add(axis, vJoyObj.GetAxisExtents(axis));
					}
					//a revoir
					trk_forceA_X.Minimum = 0;
					trk_forceA_X.Maximum = 65535;
					min_AX.Minimum = (int)HidExtents[HID_USAGES.HID_USAGE_X].Min;
					min_AX.Maximum = (int)HidExtents[HID_USAGES.HID_USAGE_X].Max;
					max_AX.Minimum = (int)HidExtents[HID_USAGES.HID_USAGE_X].Min;
					max_AX.Maximum = (int)HidExtents[HID_USAGES.HID_USAGE_X].Max;
					offset_AX.Minimum = (int)(HidExtents[HID_USAGES.HID_USAGE_X].Max * -1);
					offset_AX.Maximum = (int)HidExtents[HID_USAGES.HID_USAGE_X].Max;

					trk_forceA_Y.Minimum = 0;
					trk_forceA_Y.Maximum = 65535;
					min_AX.Minimum = (int)HidExtents[HID_USAGES.HID_USAGE_Y].Min;
					min_AX.Maximum = (int)HidExtents[HID_USAGES.HID_USAGE_Y].Max;
					max_AY.Minimum = (int)HidExtents[HID_USAGES.HID_USAGE_Y].Min;
					max_AY.Maximum = (int)HidExtents[HID_USAGES.HID_USAGE_Y].Max;
					offset_AY.Minimum = (int)(HidExtents[HID_USAGES.HID_USAGE_Y].Max * -1);
					offset_AY.Maximum = (int)HidExtents[HID_USAGES.HID_USAGE_Y].Max;


					trk_forceB_X.Minimum = 0;
					trk_forceB_X.Maximum = 65535;
					min_BX.Minimum = (int)HidExtents[HID_USAGES.HID_USAGE_RX].Min;
					min_BX.Maximum = (int)HidExtents[HID_USAGES.HID_USAGE_RX].Max;
					max_BX.Minimum = (int)HidExtents[HID_USAGES.HID_USAGE_RX].Min;
					max_BX.Maximum = (int)HidExtents[HID_USAGES.HID_USAGE_RX].Max;
					offset_BX.Minimum = (int)(HidExtents[HID_USAGES.HID_USAGE_RX].Max * -1);
					offset_BX.Maximum = (int)HidExtents[HID_USAGES.HID_USAGE_RX].Max;

					trk_forceB_Y.Minimum = 0;
					trk_forceB_Y.Maximum = 65535;
					min_BX.Minimum = (int)HidExtents[HID_USAGES.HID_USAGE_RY].Min;
					min_BX.Maximum = (int)HidExtents[HID_USAGES.HID_USAGE_RY].Max;
					max_BY.Minimum = (int)HidExtents[HID_USAGES.HID_USAGE_RY].Min;
					max_BY.Maximum = (int)HidExtents[HID_USAGES.HID_USAGE_RY].Max;
					offset_BY.Minimum = (int)(HidExtents[HID_USAGES.HID_USAGE_RY].Max * -1);
					offset_BY.Maximum = (int)HidExtents[HID_USAGES.HID_USAGE_RY].Max;
				}
				if (status == VjdStat.VJD_STAT_BUSY)
				{
					errorVjoy = " Vjoy Busy";
				}
			}
			else
			{
				errorVjoy = " Vjoy Missing";
			}
			lbl_vjoy_connected.Text = _vjoyFound ? "Availiable" : "Error" + errorVjoy;


			if (_vjoyFound)
			{
				GunA_manual = false;
				GunB_manual = false;

				btn_Save.Enabled = true;
				string vjoySettingsJsonGunA = ConfigurationManager.MainConfig.vjoySettingsGunA;
				string vjoySettingsJsonGunB = ConfigurationManager.MainConfig.vjoySettingsGunB;
				if (game != "" && _gameOptions != null)
				{
					//string GameOptionsFolder = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "gameoptions");
					//GameSettings gameOptions = new GameSettings();
					//string optionFile = Path.Combine(GameOptionsFolder, game + ".json");
					//if (File.Exists(optionFile))
					//{
					//	Utils.LogMessage($"gameoveride file found : " + optionFile);
					//	gameOptions = new GameSettings(File.ReadAllText(optionFile));
					if (!string.IsNullOrEmpty(_gameOptions.vjoySettingsGunA)) vjoySettingsJsonGunA = _gameOptions.vjoySettingsGunA;
					if (!string.IsNullOrEmpty(_gameOptions.vjoySettingsGunB)) vjoySettingsJsonGunB = _gameOptions.vjoySettingsGunB;
					//}
				}
				if (!string.IsNullOrEmpty(vjoySettingsJsonGunA))
				{
					_settingsGunA = new ConfigurationVjoyControl(vjoySettingsJsonGunA);
				}
				else
				{
					_settingsGunA = new ConfigurationVjoyControl();
					_settingsGunA.min_x = (int)HidExtents[HID_USAGES.HID_USAGE_X].Min;
					_settingsGunA.max_x = (int)HidExtents[HID_USAGES.HID_USAGE_X].Max;
					_settingsGunA.min_y = (int)HidExtents[HID_USAGES.HID_USAGE_Y].Min;
					_settingsGunA.max_y = (int)HidExtents[HID_USAGES.HID_USAGE_Y].Max;
				}
				if (!string.IsNullOrEmpty(vjoySettingsJsonGunB))
				{
					_settingsGunB = new ConfigurationVjoyControl(vjoySettingsJsonGunB);
				}
				else
				{
					_settingsGunB = new ConfigurationVjoyControl();
					_settingsGunB.min_x = (int)HidExtents[HID_USAGES.HID_USAGE_RX].Min;
					_settingsGunB.max_x = (int)HidExtents[HID_USAGES.HID_USAGE_RX].Max;
					_settingsGunB.min_y = (int)HidExtents[HID_USAGES.HID_USAGE_RY].Min;
					_settingsGunB.max_y = (int)HidExtents[HID_USAGES.HID_USAGE_RY].Max;
				}

				if (_dinputLightgunAFound)
				{
					MonitorGunA = new Thread(() => SpawnDirectInputListener(_gunA_AnalogX.JoystickGuid.ToString(), 1));
					MonitorGunA.Start();
					if (_dinputLightgunBFound) Thread.Sleep(500);
				}

				if (_dinputLightgunBFound)
				{
					MonitorGunB = new Thread(() => SpawnDirectInputListener(_gunB_AnalogX.JoystickGuid.ToString(), 2));
					MonitorGunB.Start();
				}

				if (_vjoyFound && _dinputLightgunAFound) grp_gunA.Enabled = true;
				if (_vjoyFound && _dinputLightgunBFound) grp_gunB.Enabled = true;

				LoadSettings();
				AssignNumpad();
			}




		}

		private void LoadSettings()
		{
			trk_forceA_X.Value = (int)Math.Round((double)trk_forceA_X.Maximum / 2.0);
			min_AX.Value = _settingsGunA.min_x;
			max_AX.Value = _settingsGunA.max_x;
			offset_AX.Value = _settingsGunA.offset_x;
			txt_expAX.Text = _settingsGunA.formula_x;

			trk_forceA_Y.Value = (int)Math.Round((double)trk_forceA_Y.Maximum / 2.0);
			min_AY.Value = _settingsGunA.min_y;
			max_AY.Value = _settingsGunA.max_y;
			offset_AY.Value = _settingsGunA.offset_y;
			txt_expAY.Text = _settingsGunA.formula_y;

			chk_alterManual_A.Checked = _settingsGunA.alterManual;
			chk_enableNumpadA.Checked = _settingsGunA.enableNumpad;

			trk_forceB_X.Value = (int)Math.Round((double)trk_forceB_X.Maximum / 2.0);
			min_BX.Value = _settingsGunB.min_x;
			max_BX.Value = _settingsGunB.max_x;
			offset_BX.Value = _settingsGunB.offset_x;
			txt_expBX.Text = _settingsGunB.formula_x;

			trk_forceB_Y.Value = (int)Math.Round((double)trk_forceB_Y.Maximum / 2.0);
			min_BY.Value = _settingsGunB.min_y;
			max_BY.Value = _settingsGunB.max_y;
			offset_BY.Value = _settingsGunB.offset_y;
			txt_expBY.Text = _settingsGunB.formula_y;

			chk_alterManual_B.Checked = _settingsGunB.alterManual;
			chk_enableNumpadB.Checked = _settingsGunB.enableNumpad;

			if (_settingsGunA.formula_x.Trim() != "") expAX = new Expression(_settingsGunA.formula_x);
			if (_settingsGunA.formula_y.Trim() != "") expAY = new Expression(_settingsGunA.formula_y);
			if (_settingsGunB.formula_x.Trim() != "") expBX = new Expression(_settingsGunB.formula_x);
			if (_settingsGunB.formula_x.Trim() != "") expBY = new Expression(_settingsGunB.formula_y);
		}

		private int RemapValueToVJoy(int Value, int XMin, int XMax, vJoyManager.AxisExtents vJoyLimit)
		{
			float value = (float)vJoyLimit.Min + ((float)Value - ((float)XMin)) / (((float)XMax) - ((float)XMin)) * ((float)vJoyLimit.Max - (float)vJoyLimit.Min);
			if (value >= vJoyLimit.Max) value = vJoyLimit.Max;
			if (value <= vJoyLimit.Min) value = vJoyLimit.Min;
			return (int)Math.Round(value);
		}

		private int ReindexValueToVJoy(int value, int min, int max, int offset, vJoyManager.AxisExtents vJoyLimit)
		{
			double new_pos = 0;
			double max_total = (double)vJoyLimit.Max;
			new_pos = ((double)value * (max - min) / max_total) + min;
			int new_pos_int = (int)Math.Round(new_pos);
			new_pos_int += offset;

			if (new_pos_int < vJoyLimit.Min) new_pos_int = (int)vJoyLimit.Min;
			if (new_pos_int > vJoyLimit.Max) new_pos_int = (int)vJoyLimit.Max;
			return new_pos_int;

		}

		private void AssignNumpad()
		{
			var assignment = new Dictionary<Combination, Action>();



			if (_dinputLightgunBFound && _settingsGunB.enableNumpad)
			{

				try
				{
					var keycombi = Combination.FromString("Alt+NumPad5");
					Action actionPauseMenu = () =>
					{
						if (GunB_manual && !isActivated)
						{
							btn_Center_B_Click(null, null);
						}
					};
					assignment.Add(keycombi, actionPauseMenu);

				}
				catch (Exception ex) { }
				try
				{
					var keycombi = Combination.FromString("Alt+NumPad8");
					Action actionPauseMenu = () =>
					{
						if (GunB_manual && !isActivated)
						{
							btn_Up_B_Click(null, null);
						}

					};
					assignment.Add(keycombi, actionPauseMenu);

				}
				catch (Exception ex) { }
				try
				{
					var keycombi = Combination.FromString("Alt+NumPad2");
					Action actionPauseMenu = () =>
					{
						if (GunB_manual && !isActivated)
						{
							btn_Down_B_Click(null, null);
						}
					};
					assignment.Add(keycombi, actionPauseMenu);

				}
				catch (Exception ex) { }
				try
				{
					var keycombi = Combination.FromString("Alt+NumPad4");
					Action actionPauseMenu = () =>
					{
						if (GunB_manual && !isActivated)
						{
							btn_Left_B_Click(null, null);
						}
					};
					assignment.Add(keycombi, actionPauseMenu);

				}
				catch (Exception ex) { }
				try
				{
					var keycombi = Combination.FromString("Alt+NumPad6");
					Action actionPauseMenu = () =>
					{
						if (GunB_manual && !isActivated)
						{
							btn_Right_B_Click(null, null);
						}
					};
					assignment.Add(keycombi, actionPauseMenu);

				}
				catch (Exception ex) { }
				try
				{
					var keycombi = Combination.FromString("Alt+NumPad0");
					Action actionPauseMenu = () =>
					{
						if (!isActivated)
						{
							btn_switchModeB_Click(null, null);
						}
					};
					assignment.Add(keycombi, actionPauseMenu);

				}
				catch (Exception ex) { }
				try
				{
					var keycombi = Combination.FromString("Alt+Subtract");
					Action actionPauseMenu = () =>
					{
						if (!isActivated)
						{
							chk_alterManual_B.Checked = !chk_alterManual_B.Checked;
						}
					};
					assignment.Add(keycombi, actionPauseMenu);
				}
				catch (Exception ex) { }
			}

			if (_dinputLightgunAFound && _settingsGunA.enableNumpad)
			{
				try
				{
					var keycombi = Combination.FromString("NumPad5");
					Action actionPauseMenu = () =>
					{
						if (GunA_manual && !isActivated)
						{
							btn_Center_A_Click(null, null);
						}
					};
					assignment.Add(keycombi, actionPauseMenu);

				}
				catch (Exception ex) { }
				try
				{
					var keycombi = Combination.FromString("NumPad8");
					Action actionPauseMenu = () =>
					{
						if (GunA_manual && !isActivated)
						{
							btn_Up_A_Click(null, null);
						}
					};
					assignment.Add(keycombi, actionPauseMenu);

				}
				catch (Exception ex) { }
				try
				{
					var keycombi = Combination.FromString("NumPad2");
					Action actionPauseMenu = () =>
					{
						if (GunA_manual && !isActivated)
						{
							btn_Down_A_Click(null, null);
						}
					};
					assignment.Add(keycombi, actionPauseMenu);

				}
				catch (Exception ex) { }
				try
				{
					var keycombi = Combination.FromString("NumPad4");
					Action actionPauseMenu = () =>
					{
						if (GunA_manual && !isActivated)
						{
							btn_Left_A_Click(null, null);
						}
					};
					assignment.Add(keycombi, actionPauseMenu);

				}
				catch (Exception ex) { }
				try
				{
					var keycombi = Combination.FromString("NumPad6");
					Action actionPauseMenu = () =>
					{
						if (GunA_manual && !isActivated)
						{
							btn_Right_A_Click(null, null);
						}
					};
					assignment.Add(keycombi, actionPauseMenu);

				}
				catch (Exception ex) { }
				try
				{
					var keycombi = Combination.FromString("NumPad0");
					Action actionPauseMenu = () =>
					{
						if (!isActivated)
						{
							btn_switchModeA_Click(null, null);
						}


					};
					assignment.Add(keycombi, actionPauseMenu);
				}
				catch (Exception ex) { }
				try
				{
					var keycombi = Combination.FromString("Subtract");
					Action actionPauseMenu = () =>
					{
						if (!isActivated)
						{
							chk_alterManual_A.Checked = !chk_alterManual_A.Checked;
						}
					};
					assignment.Add(keycombi, actionPauseMenu);
				}
				catch (Exception ex) { }


			}

			try
			{
				if (assignment.Count > 0)
				{
					_globalHook = Hook.GlobalEvents();
					_globalHook.OnCombination(assignment);
				}
			}
			catch { }

		}

		private void CloseMenuItem_Click(object? sender, EventArgs e)
		{
			this.Close();
		}

		private void VjoyControl_DoubleClick(object? sender, EventArgs e)
		{
			this.Show();
			this.WindowState = FormWindowState.Normal;
			notifyIcon.Visible = false;
		}

		private void VjoyControl_Resize(object sender, EventArgs e)
		{
			if (!_isDialog)
			{
				if (FormWindowState.Minimized == this.WindowState)
				{
					notifyIcon.Visible = true;

					this.Hide();
				}
				else if (FormWindowState.Normal == this.WindowState)
				{
					notifyIcon.Visible = false;
				}

			}

		}

		private void SpawnDirectInputListener(string joyGuid, int gunIndex)
		{
			int keyOffsetX = -1;
			int keyOffsetY = -1;

			if (gunIndex == 1)
			{
				keyOffsetX = _gunA_AnalogX.Button;
				keyOffsetY = _gunA_AnalogY.Button;
			}
			if (gunIndex == 2)
			{
				keyOffsetX = _gunB_AnalogX.Button;
				keyOffsetY = _gunB_AnalogY.Button;
			}
			var directInput = new DirectInput();
			Joystick joystick = null;
			var devicesInstance = new List<DeviceInstance>();
			devicesInstance.AddRange(directInput.GetDevices().Where(x => x.Type != DeviceType.Mouse && x.UsagePage != UsagePage.VendorDefinedBegin && x.Usage != UsageId.AlphanumericBitmapSizeX && x.Usage != UsageId.AlphanumericAlphanumericDisplay && x.UsagePage != unchecked((UsagePage)0xffffff43) && x.UsagePage != UsagePage.Vr).ToList());
			DeviceInstance deviceInstance = null;
			bool found_device = false;
			Guid device_guid = Guid.Empty;
			foreach (var device in devicesInstance)
			{
				if (device.InstanceGuid.ToString() == joyGuid)
				{
					found_device = true;
					deviceInstance = device;
					device_guid = device.InstanceGuid;
					break;
				}
			}
			if (!found_device)
			{
				return;
			}
			while (!directInput.IsDeviceAttached(device_guid))
			{
				Thread.Sleep(100);
				if (_stopListening) return;
			}
			joystick = new Joystick(directInput, device_guid);
			joystick.Properties.BufferSize = 512;
			joystick.Acquire();
			//MessageBox.Show("gun " + gunIndex.ToString());
			while (!_stopListening)
			{
				try
				{
					Dictionary<string, int> keyPressedValue = new Dictionary<string, int>();
					joystick.Poll();
					var datas = joystick.GetBufferedData();
					foreach (var key in datas)
					{
						int pressed = 0;
						string inputText = "";
						if (key.Offset == JoystickOffset.X ||
								key.Offset == JoystickOffset.Y ||
								key.Offset == JoystickOffset.Z ||
								key.Offset == JoystickOffset.RotationX ||
								key.Offset == JoystickOffset.RotationY ||
								key.Offset == JoystickOffset.RotationZ ||
								key.Offset == JoystickOffset.Sliders0 ||
								key.Offset == JoystickOffset.Sliders1 ||
								key.Offset == JoystickOffset.AccelerationX ||
								key.Offset == JoystickOffset.AccelerationY ||
								key.Offset == JoystickOffset.AccelerationZ)
						{
							if ((int)key.Offset == keyOffsetX)
							{
								if (gunIndex == 1)
								{
									GunA_X = key.Value;
								}
								if (gunIndex == 2)
								{
									GunB_X = key.Value;
								}
							}
							if ((int)key.Offset == keyOffsetY)
							{
								if (gunIndex == 1)
								{
									GunA_Y = key.Value;
								}
								if (gunIndex == 2)
								{
									GunB_Y = key.Value;
								}
							}
						}
						// Digital input
					}


					if (_vjoyFound)
					{
						int new_Ax = -1;
						int new_Ay = -1;
						int new_Bx = -1;
						int new_By = -1;

						//Debut Traitement
						if (!GunA_manual)
						{
							if (_GunA_X_change)
							{
								int new_value = RemapValueToVJoy(GunA_X, 0, 65535, HidExtents[HID_USAGES.HID_USAGE_X]);
								x_original = new_value;
								new_value = ReindexValueToVJoy(new_value, _settingsGunA.min_x, _settingsGunA.max_x, _settingsGunA.offset_x, HidExtents[HID_USAGES.HID_USAGE_X]);
								new_Ax = new_value;
								_GunA_X_change = false;
							}
							if (_GunA_Y_change)
							{
								int new_value = RemapValueToVJoy(GunA_Y, 0, 65535, HidExtents[HID_USAGES.HID_USAGE_Y]);
								y_original = new_value;
								new_value = ReindexValueToVJoy(new_value, _settingsGunA.min_y, _settingsGunA.max_y, _settingsGunA.offset_y, HidExtents[HID_USAGES.HID_USAGE_Y]);
								new_Ay = new_value;
								_GunA_Y_change = false;
							}
						}
						if (!GunB_manual)
						{
							if (_GunB_X_change)
							{
								int new_value = RemapValueToVJoy(GunB_X, 0, 65535, HidExtents[HID_USAGES.HID_USAGE_RX]);
								x2_original = new_value;
								new_value = ReindexValueToVJoy(new_value, _settingsGunB.min_x, _settingsGunB.max_x, _settingsGunB.offset_x, HidExtents[HID_USAGES.HID_USAGE_RX]);
								new_Bx = new_value;
								_GunB_X_change = false;
							}
							if (_GunB_Y_change)
							{
								int new_value = RemapValueToVJoy(GunB_Y, 0, 65535, HidExtents[HID_USAGES.HID_USAGE_RY]);
								y2_original = new_value;
								new_value = ReindexValueToVJoy(new_value, _settingsGunB.min_y, _settingsGunB.max_y, _settingsGunB.offset_y, HidExtents[HID_USAGES.HID_USAGE_RY]);
								new_By = new_value;
								_GunB_Y_change = false;
							}
						}
						if (new_Ax >= 0 && expAX != null)
						{
							var vJoyLimit = HidExtents[HID_USAGES.HID_USAGE_X];
							expAX.Parameters["X"] = new_Ax;
							expAX.Parameters["Y"] = new_Ay >= 0 ? new_Ay : vjoyA_Y;
							expAX.Parameters["OX"] = x_original;
							expAX.Parameters["OY"] = y_original;
							try
							{
								var resultEvaluate = double.Parse(expAX.Evaluate().ToString());
								new_Ax = (int)Math.Round(resultEvaluate);
								if (new_Ax < vJoyLimit.Min) new_Ax = (int)vJoyLimit.Min;
								if (new_Ax > vJoyLimit.Max) new_Ax = (int)vJoyLimit.Max;
							}
							catch { }
						}
						if (new_Ay >= 0 && expAY != null)
						{
							int original = new_Ay;
							var vJoyLimit = HidExtents[HID_USAGES.HID_USAGE_Y];
							expAY.Parameters["Y"] = new_Ay;
							expAY.Parameters["X"] = new_Ax >= 0 ? new_Ax : vjoyA_X;
							expAY.Parameters["OX"] = x_original;
							expAY.Parameters["OY"] = y_original;

							try
							{
								var resultEvaluate = double.Parse(expAY.Evaluate().ToString());
								new_Ay = (int)Math.Round(resultEvaluate);
								if (new_Ay < vJoyLimit.Min) new_Ay = (int)vJoyLimit.Min;
								if (new_Ay > vJoyLimit.Max) new_Ay = (int)vJoyLimit.Max;
							}
							catch { }
						}
						if (new_Bx >= 0 && expBX != null)
						{
							var vJoyLimit = HidExtents[HID_USAGES.HID_USAGE_RX];
							expBX.Parameters["X"] = new_Bx;
							expBX.Parameters["Y"] = new_By >= 0 ? new_By : vjoyB_Y;
							expBX.Parameters["OX"] = x2_original;
							expBX.Parameters["OY"] = y2_original;

							try
							{
								var resultEvaluate = double.Parse(expBX.Evaluate().ToString());
								new_Bx = (int)Math.Round(resultEvaluate);
								if (new_Bx < vJoyLimit.Min) new_Bx = (int)vJoyLimit.Min;
								if (new_Bx > vJoyLimit.Max) new_Bx = (int)vJoyLimit.Max;
							}
							catch { }
						}
						if (new_By >= 0 && expBY != null)
						{
							var vJoyLimit = HidExtents[HID_USAGES.HID_USAGE_RY];
							expBY.Parameters["Y"] = new_By;
							expBY.Parameters["X"] = new_Bx >= 0 ? new_Bx : vjoyB_X;
							expBY.Parameters["OX"] = x2_original;
							expBY.Parameters["OY"] = y2_original;

							try
							{
								var resultEvaluate = double.Parse(expBY.Evaluate().ToString());
								new_By = (int)Math.Round(resultEvaluate);
								if (new_By < vJoyLimit.Min) new_By = (int)vJoyLimit.Min;
								if (new_By > vJoyLimit.Max) new_By = (int)vJoyLimit.Max;
							}
							catch { }
						}
						if (new_Ax >= 0)
						{
							vJoyObj.SetAxis(HID_USAGES.HID_USAGE_X, new_Ax);
							vjoyA_X = new_Ax;
						}
						if (new_Ay >= 0)
						{
							vJoyObj.SetAxis(HID_USAGES.HID_USAGE_Y, new_Ay);
							vjoyA_Y = new_Ay;
						}
						if (new_Bx >= 0)
						{
							vJoyObj.SetAxis(HID_USAGES.HID_USAGE_RX, new_Bx);
							vjoyB_X = new_Bx;
						}
						if (new_By >= 0)
						{
							vJoyObj.SetAxis(HID_USAGES.HID_USAGE_RY, new_By);
							vjoyB_Y = new_By;
						}


					}

					//Fin Traitement
					Thread.Sleep(10);

				}
				catch (Exception)
				{
					try
					{
						joystick.Dispose();
					}
					catch
					{

					}
					joyGuid = null;
					while (!directInput.IsDeviceAttached(device_guid))
					{
						Thread.Sleep(100);
						if (_stopListening) return;
					}
					joystick = new Joystick(new DirectInput(), device_guid);
					joystick.Properties.BufferSize = 512;
					joystick.Acquire();
				}
			}

			joystick.Unacquire();

		}

		private void VjoyControl_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_stopListening == false)
			{
				_stopListening = true;
				if (MonitorGunA != null)
				{
					MonitorGunA.Join();
					MonitorGunA = null;
				}
				if (MonitorGunB != null)
				{
					MonitorGunB.Join();
					MonitorGunB = null;
				}
				try
				{
					vJoyObj.ReleaseDevice();
				}
				catch (Exception) { }

				_stopListening = false;
			}
		}

		private void trk_forceA_X_ValueChanged(object sender, EventArgs e)
		{
			lbl_trkA_X.Text = $"{trk_forceA_X.Value} ({Math.Round((double)trk_forceA_X.Value * 100.0 / (double)trk_forceA_X.Maximum)}%)";
			if (GunA_manual)
			{
				int new_value = RemapValueToVJoy(trk_forceA_X.Value, 0, 65535, HidExtents[HID_USAGES.HID_USAGE_X]);
				x_original = new_value;
				if (_settingsGunA.alterManual)
				{
					new_value = ReindexValueToVJoy(new_value, _settingsGunA.min_x, _settingsGunA.max_x, _settingsGunA.offset_x, HidExtents[HID_USAGES.HID_USAGE_X]);
					if (new_value >= 0 && expAX != null)
					{
						var vJoyLimit = HidExtents[HID_USAGES.HID_USAGE_X];
						expAX.Parameters["X"] = new_value;
						expAX.Parameters["Y"] = vjoyA_Y;
						expAX.Parameters["OX"] = x_original;
						expAX.Parameters["OY"] = y_original;
						try
						{
							var resultEvaluate = double.Parse(expAX.Evaluate().ToString());
							new_value = (int)Math.Round(resultEvaluate);
							if (new_value < vJoyLimit.Min) new_value = (int)vJoyLimit.Min;
							if (new_value > vJoyLimit.Max) new_value = (int)vJoyLimit.Max;
						}
						catch { }
					}
				}
				if (new_value >= 0)
				{
					vJoyObj.SetAxis(HID_USAGES.HID_USAGE_X, new_value);
					vjoyA_X = new_value;
				}
			}

		}

		private void trk_forceA_Y_ValueChanged(object sender, EventArgs e)
		{
			lbl_trkA_Y.Text = $"{trk_forceA_Y.Value} ({Math.Round((double)trk_forceA_Y.Value * 100.0 / (double)trk_forceA_Y.Maximum)}%)";
			if (GunA_manual)
			{
				int new_value = RemapValueToVJoy(trk_forceA_Y.Value, 0, 65535, HidExtents[HID_USAGES.HID_USAGE_Y]);
				y_original = new_value;
				if (_settingsGunA.alterManual)
				{
					new_value = ReindexValueToVJoy(new_value, _settingsGunA.min_y, _settingsGunA.max_y, _settingsGunA.offset_y, HidExtents[HID_USAGES.HID_USAGE_Y]);
					if (new_value >= 0 && expAY != null)
					{
						var vJoyLimit = HidExtents[HID_USAGES.HID_USAGE_Y];
						expAY.Parameters["X"] = vjoyA_X;
						expAY.Parameters["Y"] = new_value;
						expAY.Parameters["OX"] = x_original;
						expAY.Parameters["OY"] = y_original;
						try
						{
							var resultEvaluate = double.Parse(expAY.Evaluate().ToString());
							new_value = (int)Math.Round(resultEvaluate);
							if (new_value < vJoyLimit.Min) new_value = (int)vJoyLimit.Min;
							if (new_value > vJoyLimit.Max) new_value = (int)vJoyLimit.Max;
						}
						catch { }
					}
				}
				if (new_value >= 0)
				{
					vJoyObj.SetAxis(HID_USAGES.HID_USAGE_Y, new_value);
					vjoyA_Y = new_value;
				}
			}
		}

		private void trk_forceB_X_ValueChanged(object sender, EventArgs e)
		{
			lbl_trkB_X.Text = $"{trk_forceB_X.Value} ({Math.Round((double)trk_forceB_X.Value * 100.0 / (double)trk_forceB_X.Maximum)}%)";
			if (GunB_manual)
			{
				int new_value = RemapValueToVJoy(trk_forceB_X.Value, 0, 65535, HidExtents[HID_USAGES.HID_USAGE_RX]);
				x2_original = new_value;
				if (_settingsGunB.alterManual)
				{
					new_value = ReindexValueToVJoy(new_value, _settingsGunB.min_x, _settingsGunB.max_x, _settingsGunB.offset_x, HidExtents[HID_USAGES.HID_USAGE_RX]);
					if (new_value >= 0 && expBX != null)
					{
						var vJoyLimit = HidExtents[HID_USAGES.HID_USAGE_RX];
						expBX.Parameters["X"] = new_value;
						expBX.Parameters["Y"] = vjoyB_Y;
						expBX.Parameters["OX"] = x2_original;
						expBX.Parameters["OY"] = y2_original;
						try
						{
							var resultEvaluate = double.Parse(expBX.Evaluate().ToString());
							new_value = (int)Math.Round(resultEvaluate);
							if (new_value < vJoyLimit.Min) new_value = (int)vJoyLimit.Min;
							if (new_value > vJoyLimit.Max) new_value = (int)vJoyLimit.Max;
						}
						catch { }
					}
				}
				if (new_value >= 0)
				{
					vJoyObj.SetAxis(HID_USAGES.HID_USAGE_RX, new_value);
					vjoyB_X = new_value;
				}
			}
		}

		private void trk_forceB_Y_ValueChanged(object sender, EventArgs e)
		{
			lbl_trkB_Y.Text = $"{trk_forceB_Y.Value} ({Math.Round((double)trk_forceB_Y.Value * 100.0 / (double)trk_forceB_Y.Maximum)}%)";
			if (GunB_manual)
			{
				int new_value = RemapValueToVJoy(trk_forceB_Y.Value, 0, 65535, HidExtents[HID_USAGES.HID_USAGE_RY]);
				y2_original = new_value;
				if (_settingsGunB.alterManual)
				{
					new_value = ReindexValueToVJoy(new_value, _settingsGunB.min_y, _settingsGunB.max_y, _settingsGunB.offset_y, HidExtents[HID_USAGES.HID_USAGE_RY]);
					if (new_value >= 0 && expBY != null)
					{
						var vJoyLimit = HidExtents[HID_USAGES.HID_USAGE_RY];
						expBY.Parameters["X"] = vjoyB_X;
						expBY.Parameters["Y"] = new_value;
						expBY.Parameters["OX"] = x2_original;
						expBY.Parameters["OY"] = y2_original;
						try
						{
							var resultEvaluate = double.Parse(expBY.Evaluate().ToString());
							new_value = (int)Math.Round(resultEvaluate);
							if (new_value < vJoyLimit.Min) new_value = (int)vJoyLimit.Min;
							if (new_value > vJoyLimit.Max) new_value = (int)vJoyLimit.Max;
						}
						catch { }
					}
				}
				if (new_value >= 0)
				{
					vJoyObj.SetAxis(HID_USAGES.HID_USAGE_RY, new_value);
					vjoyB_Y = new_value;
				}
			}
		}

		private void min_AX_ValueChanged(object sender, EventArgs e)
		{
			_settingsGunA.min_x = (int)min_AX.Value;
			trk_forceA_X_ValueChanged(sender, e);
		}

		private void max_AX_ValueChanged(object sender, EventArgs e)
		{
			_settingsGunA.max_x = (int)max_AX.Value;
			trk_forceA_X_ValueChanged(sender, e);
		}

		private void offset_AX_ValueChanged(object sender, EventArgs e)
		{
			_settingsGunA.offset_x = (int)offset_AX.Value;
			trk_forceA_X_ValueChanged(sender, e);
		}

		private void min_AY_ValueChanged(object sender, EventArgs e)
		{
			_settingsGunA.min_y = (int)min_AY.Value;
			trk_forceA_Y_ValueChanged(sender, e);
		}

		private void max_AY_ValueChanged(object sender, EventArgs e)
		{
			_settingsGunA.max_y = (int)max_AY.Value;
			trk_forceA_Y_ValueChanged(sender, e);
		}

		private void offset_AY_ValueChanged(object sender, EventArgs e)
		{
			_settingsGunA.offset_y = (int)offset_AY.Value;
			trk_forceA_Y_ValueChanged(sender, e);
		}

		private void chk_alterManual_A_CheckedChanged(object sender, EventArgs e)
		{
			_settingsGunA.alterManual = chk_alterManual_A.Checked;
			trk_forceA_X_ValueChanged(sender, e);
			trk_forceA_Y_ValueChanged(sender, e);
		}

		private void chk_enableNumpadA_CheckedChanged(object sender, EventArgs e)
		{
			_settingsGunA.enableNumpad = chk_enableNumpadA.Checked;
		}

		private void min_BX_ValueChanged(object sender, EventArgs e)
		{
			_settingsGunB.min_x = (int)min_BX.Value;
			trk_forceB_X_ValueChanged(sender, e);
		}

		private void max_BX_ValueChanged(object sender, EventArgs e)
		{
			_settingsGunB.max_x = (int)max_BX.Value;
			trk_forceB_X_ValueChanged(sender, e);
		}

		private void offset_BX_ValueChanged(object sender, EventArgs e)
		{
			_settingsGunB.offset_x = (int)offset_BX.Value;
			trk_forceB_X_ValueChanged(sender, e);
		}

		private void min_BY_ValueChanged(object sender, EventArgs e)
		{
			_settingsGunB.min_y = (int)min_BY.Value;
			trk_forceB_Y_ValueChanged(sender, e);
		}

		private void max_BY_ValueChanged(object sender, EventArgs e)
		{
			_settingsGunB.max_y = (int)max_BY.Value;
			trk_forceB_Y_ValueChanged(sender, e);
		}

		private void offset_BY_ValueChanged(object sender, EventArgs e)
		{
			_settingsGunB.offset_y = (int)offset_BY.Value;
			trk_forceB_Y_ValueChanged(sender, e);
		}

		private void chk_alterManual_B_CheckedChanged(object sender, EventArgs e)
		{
			_settingsGunB.alterManual = chk_alterManual_B.Checked;
			trk_forceB_X_ValueChanged(sender, e);
			trk_forceB_Y_ValueChanged(sender, e);
		}

		private void chk_enableNumpadB_CheckedChanged(object sender, EventArgs e)
		{
			_settingsGunB.enableNumpad = chk_enableNumpadB.Checked;
		}

		private void btn_updateAX_Click(object sender, EventArgs e)
		{
			if (txt_expAX.Text != "")
			{
				expAX = new Expression(txt_expAX.Text);
				if (expAX.HasErrors())
				{
					MessageBox.Show("Invalid Expression");
					expAX = null;
				}
				else
				{
					_settingsGunA.formula_x = txt_expAX.Text;
					trk_forceA_X_ValueChanged(sender, e);
				}
			}
			else
			{
				_settingsGunA.formula_x = "";
				expAX = null;
			}
		}

		private void btn_updateAY_Click(object sender, EventArgs e)
		{
			if (txt_expAY.Text != "")
			{
				expAY = new Expression(txt_expAY.Text);
				if (expAY.HasErrors())
				{
					MessageBox.Show("Invalid Expression");
					expAY = null;
				}
				else
				{
					_settingsGunA.formula_y = txt_expAY.Text;
					trk_forceA_Y_ValueChanged(sender, e);
				}
			}
			else
			{
				_settingsGunA.formula_y = "";
				expAY = null;
			}
		}

		private void btn_updateBX_Click(object sender, EventArgs e)
		{
			if (txt_expBX.Text != "")
			{
				expBX = new Expression(txt_expBX.Text);
				if (expBX.HasErrors())
				{
					MessageBox.Show("Invalid Expression");
					expBX = null;
				}
				else
				{
					_settingsGunB.formula_x = txt_expBX.Text;
					trk_forceB_X_ValueChanged(sender, e);
				}
			}
			else
			{
				_settingsGunB.formula_x = "";
				expBX = null;
			}
		}

		private void btn_updateBY_Click(object sender, EventArgs e)
		{
			if (txt_expBY.Text != "")
			{
				expBY = new Expression(txt_expBY.Text);
				if (expBY.HasErrors())
				{
					MessageBox.Show("Invalid Expression");
					expBY = null;
				}
				else
				{
					_settingsGunB.formula_y = txt_expBY.Text;
					trk_forceB_Y_ValueChanged(sender, e);
				}
			}
			else
			{
				_settingsGunB.formula_y = "";
				expBY = null;
			}
		}

		private void btn_Save_Click(object sender, EventArgs e)
		{
			if (grp_gunA.Enabled)
			{
				GunA_json = _settingsGunA.Serialize();

			}
			if (grp_gunB.Enabled)
			{
				GunB_json = _settingsGunB.Serialize();
			}

			if (_isDialog)
			{
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
			else
			{
				if (_gameOptions != null)
				{
					string GameOptionsFolder = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "gameoptions");
					string optionFile = Path.Combine(GameOptionsFolder, _game + ".json");
					_gameOptions.Save(optionFile);
				}
			}


		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void btn_defaultA_Click(object sender, EventArgs e)
		{

			string vjoySettingsJsonGunA = ConfigurationManager.MainConfig.vjoySettingsGunA;
			if (!string.IsNullOrEmpty(vjoySettingsJsonGunA))
			{
				_settingsGunA = new ConfigurationVjoyControl(vjoySettingsJsonGunA);
			}
			else
			{
				_settingsGunA = new ConfigurationVjoyControl();
				_settingsGunA.min_x = (int)HidExtents[HID_USAGES.HID_USAGE_X].Min;
				_settingsGunA.max_x = (int)HidExtents[HID_USAGES.HID_USAGE_X].Max;
				_settingsGunA.min_y = (int)HidExtents[HID_USAGES.HID_USAGE_Y].Min;
				_settingsGunA.max_y = (int)HidExtents[HID_USAGES.HID_USAGE_Y].Max;
			}

			trk_forceA_X.Value = (int)Math.Round((double)trk_forceA_X.Maximum / 2.0);
			min_AX.Value = _settingsGunA.min_x;
			max_AX.Value = _settingsGunA.max_x;
			offset_AX.Value = _settingsGunA.offset_x;
			txt_expAX.Text = _settingsGunA.formula_x;

			trk_forceA_Y.Value = (int)Math.Round((double)trk_forceA_Y.Maximum / 2.0);
			min_AY.Value = _settingsGunA.min_y;
			max_AY.Value = _settingsGunA.max_y;
			offset_AY.Value = _settingsGunA.offset_y;
			txt_expAY.Text = _settingsGunA.formula_y;

			chk_alterManual_A.Checked = _settingsGunA.alterManual;
			chk_enableNumpadA.Checked = _settingsGunA.enableNumpad;

			expAX = null;
			expAY = null;
			if (_settingsGunA.formula_x.Trim() != "") expAX = new Expression(_settingsGunA.formula_x);
			if (_settingsGunA.formula_y.Trim() != "") expAY = new Expression(_settingsGunA.formula_y);

		}

		private void btn_clearA_Click(object sender, EventArgs e)
		{
			min_AX.Value = (int)HidExtents[HID_USAGES.HID_USAGE_X].Min;
			max_AX.Value = (int)HidExtents[HID_USAGES.HID_USAGE_X].Max;
			offset_AX.Value = 0;

			min_AY.Value = (int)HidExtents[HID_USAGES.HID_USAGE_Y].Min;
			max_AY.Value = (int)HidExtents[HID_USAGES.HID_USAGE_Y].Max;
			offset_AY.Value = 0;
			txt_expAX.Text = "";
			txt_expAY.Text = "";
			expAX = null;
			expAY = null;
		}

		private void btn_defaultB_Click(object sender, EventArgs e)
		{
			string vjoySettingsJsonGunB = ConfigurationManager.MainConfig.vjoySettingsGunB;
			if (!string.IsNullOrEmpty(vjoySettingsJsonGunB))
			{
				_settingsGunB = new ConfigurationVjoyControl(vjoySettingsJsonGunB);
			}
			else
			{
				_settingsGunB = new ConfigurationVjoyControl();
				_settingsGunB.min_x = (int)HidExtents[HID_USAGES.HID_USAGE_RX].Min;
				_settingsGunB.max_x = (int)HidExtents[HID_USAGES.HID_USAGE_RX].Max;
				_settingsGunB.min_y = (int)HidExtents[HID_USAGES.HID_USAGE_RY].Min;
				_settingsGunB.max_y = (int)HidExtents[HID_USAGES.HID_USAGE_RY].Max;
			}

			trk_forceB_X.Value = (int)Math.Round((double)trk_forceB_X.Maximum / 2.0);
			min_BX.Value = _settingsGunB.min_x;
			max_BX.Value = _settingsGunB.max_x;
			offset_BX.Value = _settingsGunB.offset_x;
			txt_expBX.Text = _settingsGunB.formula_x;

			trk_forceB_Y.Value = (int)Math.Round((double)trk_forceB_Y.Maximum / 2.0);
			min_BY.Value = _settingsGunB.min_y;
			max_BY.Value = _settingsGunB.max_y;
			offset_BY.Value = _settingsGunB.offset_y;
			txt_expBY.Text = _settingsGunB.formula_y;

			chk_alterManual_B.Checked = _settingsGunB.alterManual;
			chk_enableNumpadB.Checked = _settingsGunB.enableNumpad;

			expBX = null;
			expBY = null;
			if (_settingsGunB.formula_x.Trim() != "") expBX = new Expression(_settingsGunB.formula_x);
			if (_settingsGunB.formula_x.Trim() != "") expBY = new Expression(_settingsGunB.formula_y);
		}

		private void btn_clearB_Click(object sender, EventArgs e)
		{
			min_BX.Value = (int)HidExtents[HID_USAGES.HID_USAGE_RX].Min;
			max_BX.Value = (int)HidExtents[HID_USAGES.HID_USAGE_RX].Max;
			offset_BX.Value = 0;

			min_BY.Value = (int)HidExtents[HID_USAGES.HID_USAGE_RY].Min;
			max_BY.Value = (int)HidExtents[HID_USAGES.HID_USAGE_RY].Max;
			offset_BY.Value = 0;
			txt_expBX.Text = "";
			txt_expBY.Text = "";
			expBX = null;
			expBY = null;
		}

		private void btn_switchModeA_Click(object sender, EventArgs e)
		{
			GunA_manual = !GunA_manual;
			_GunA_X_change = true;
			_GunA_Y_change = true;
			_GunB_X_change = true;
			_GunB_Y_change = true;
			if (GunA_manual)
			{
				trk_forceA_X_ValueChanged(sender, e);
				trk_forceA_Y_ValueChanged(sender, e);
			}
		}

		private void btn_switchModeB_Click(object sender, EventArgs e)
		{
			GunB_manual = !GunB_manual;
			_GunB_X_change = true;
			_GunB_Y_change = true;
			_GunB_X_change = true;
			_GunB_Y_change = true;
			if (GunB_manual)
			{
				trk_forceB_X_ValueChanged(sender, e);
				trk_forceB_Y_ValueChanged(sender, e);
			}
		}

		private void btn_Up_A_Click(object sender, EventArgs e)
		{
			if (GunA_manual)
			{
				List<int> percents = new List<int>();
				for (int i = 0; i <= 10; i++)
				{
					int val = (int)Math.Round((double)i * ((double)trk_forceA_Y.Maximum / 10.0));
					percents.Add(val);
				}
				percents.Reverse();
				bool found = false;
				foreach (int p in percents)
				{
					if (p < trk_forceA_Y.Value)
					{
						found = true;
						trk_forceA_Y.Value = p;
						break;
					}
				}
				if (!found)
				{
					trk_forceA_Y.Value = percents.First();
				}
			}

		}

		private void btn_Left_A_Click(object sender, EventArgs e)
		{
			if (GunA_manual)
			{
				List<int> percents = new List<int>();
				for (int i = 0; i <= 10; i++)
				{
					int val = (int)Math.Round((double)i * ((double)trk_forceA_X.Maximum / 10.0));
					percents.Add(val);
				}
				percents.Reverse();
				bool found = false;
				foreach (int p in percents)
				{
					if (p < trk_forceA_X.Value)
					{
						found = true;
						trk_forceA_X.Value = p;
						break;
					}
				}
				if (!found)
				{
					trk_forceA_X.Value = percents.First();
				}
			}
		}

		private void btn_Right_A_Click(object sender, EventArgs e)
		{
			if (GunA_manual)
			{
				List<int> percents = new List<int>();
				for (int i = 0; i <= 10; i++)
				{
					int val = (int)Math.Round((double)i * ((double)trk_forceA_X.Maximum / 10.0));
					percents.Add(val);
				}
				bool found = false;
				foreach (int p in percents)
				{
					if (p > trk_forceA_X.Value)
					{
						found = true;
						trk_forceA_X.Value = p;
						break;
					}
				}
				if (!found)
				{
					trk_forceA_X.Value = percents.First();
				}
			}

		}

		private void btn_Down_A_Click(object sender, EventArgs e)
		{
			if (GunA_manual)
			{
				List<int> percents = new List<int>();
				for (int i = 0; i <= 10; i++)
				{
					int val = (int)Math.Round((double)i * ((double)trk_forceA_Y.Maximum / 10.0));
					percents.Add(val);
				}
				bool found = false;
				foreach (int p in percents)
				{
					if (p > trk_forceA_Y.Value)
					{
						found = true;
						trk_forceA_Y.Value = p;
						break;
					}
				}
				if (!found)
				{
					trk_forceA_Y.Value = percents.First();
				}
			}
		}

		private void btn_Center_A_Click(object sender, EventArgs e)
		{
			trk_forceA_X.Value = (int)Math.Round((double)trk_forceA_X.Maximum / 2.0);
			trk_forceA_Y.Value = (int)Math.Round((double)trk_forceA_Y.Maximum / 2.0);
		}

		private void btn_Up_B_Click(object sender, EventArgs e)
		{
			if (GunB_manual)
			{
				List<int> percents = new List<int>();
				for (int i = 0; i <= 10; i++)
				{
					int val = (int)Math.Round((double)i * ((double)trk_forceB_Y.Maximum / 10.0));
					percents.Add(val);
				}
				percents.Reverse();
				bool found = false;
				foreach (int p in percents)
				{
					if (p < trk_forceB_Y.Value)
					{
						found = true;
						trk_forceB_Y.Value = p;
						break;
					}
				}
				if (!found)
				{
					trk_forceB_Y.Value = percents.First();
				}
			}
		}

		private void btn_Left_B_Click(object sender, EventArgs e)
		{
			if (GunB_manual)
			{
				List<int> percents = new List<int>();
				for (int i = 0; i <= 10; i++)
				{
					int val = (int)Math.Round((double)i * ((double)trk_forceB_X.Maximum / 10.0));
					percents.Add(val);
				}
				percents.Reverse();
				bool found = false;
				foreach (int p in percents)
				{
					if (p < trk_forceB_X.Value)
					{
						found = true;
						trk_forceB_X.Value = p;
						break;
					}
				}
				if (!found)
				{
					trk_forceB_X.Value = percents.First();
				}
			}
		}

		private void btn_Right_B_Click(object sender, EventArgs e)
		{
			if (GunB_manual)
			{
				List<int> percents = new List<int>();
				for (int i = 0; i <= 10; i++)
				{
					int val = (int)Math.Round((double)i * ((double)trk_forceB_X.Maximum / 10.0));
					percents.Add(val);
				}
				bool found = false;
				foreach (int p in percents)
				{
					if (p > trk_forceB_X.Value)
					{
						found = true;
						trk_forceB_X.Value = p;
						break;
					}
				}
				if (!found)
				{
					trk_forceB_X.Value = percents.First();
				}
			}
		}

		private void btn_Down_B_Click(object sender, EventArgs e)
		{
			if (GunB_manual)
			{
				List<int> percents = new List<int>();
				for (int i = 0; i <= 10; i++)
				{
					int val = (int)Math.Round((double)i * ((double)trk_forceB_Y.Maximum / 10.0));
					percents.Add(val);
				}
				bool found = false;
				foreach (int p in percents)
				{
					if (p > trk_forceB_Y.Value)
					{
						found = true;
						trk_forceB_Y.Value = p;
						break;
					}
				}
				if (!found)
				{
					trk_forceB_Y.Value = percents.First();
				}
			}
		}

		private void btn_Center_B_Click(object sender, EventArgs e)
		{
			trk_forceB_X.Value = (int)Math.Round((double)trk_forceB_X.Maximum / 2.0);
			trk_forceB_Y.Value = (int)Math.Round((double)trk_forceB_Y.Maximum / 2.0);
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			if (parentProcess.HasExited)
			{
				this.Close();
			}
		}
	}


	public class ConfigurationVjoyControl
	{
		public int min_x { get; set; } = 0;
		public int max_x { get; set; } = 0;
		public int offset_x { get; set; } = 0;
		public string formula_x { get; set; } = "";
		public int min_y { get; set; } = 0;
		public int max_y { get; set; } = 0;
		public int offset_y { get; set; } = 0;
		public string formula_y { get; set; } = "";
		public bool alterManual { get; set; } = false;
		public bool enableNumpad { get; set; } = true;

		public ConfigurationVjoyControl()
		{
		}

		public ConfigurationVjoyControl(string json)
		{
			ConfigurationVjoyControl DeserializeData = JsonConvert.DeserializeObject<ConfigurationVjoyControl>(json);
			this.min_x = DeserializeData.min_x;
			this.max_x= DeserializeData.max_x;
			this.offset_x = DeserializeData.offset_x;
			this.formula_x = DeserializeData.formula_x;
			this.min_y = DeserializeData.min_y;
			this.max_y= DeserializeData.max_y;
			this.offset_y = DeserializeData.offset_y;
			this.formula_y = DeserializeData.formula_y;
			this.alterManual = DeserializeData.alterManual;
			this.enableNumpad = DeserializeData.enableNumpad;
		}

		public string Serialize()
		{
			string json = JsonConvert.SerializeObject(this, Formatting.Indented);
			return json;
		}
	}
}
