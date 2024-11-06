using Microsoft.Win32;
using SerialPortLib2;
using SharpDX.DirectInput;
using SharpDX.Multimedia;
using System.Data;
using System.Diagnostics;
using System.Management;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace TeknoparrotAutoXinput
{
	public partial class gun_preconfig : Form
	{
		private List<Thread> threadJoystick = new List<Thread>();
		List<Task> taskJoystick = new List<Task>();

		private static bool _stopListening;
		private List<DeviceInstance> devices = new List<DeviceInstance>();
		private readonly DirectInput _directInput = new DirectInput();
		private List<string> guids = new List<string>();

		List<(string, string, string)> listSinden = new List<(string, string, string)>();

		public string Dialogconfig = "";

		public string gunConfig = "";
		public string gunGuid = "";
		public string gunName = "";
		public int gunCom = 0;
		public int gunSindenType = 0;

		private int _gunType;
		private int _gunIndex;
		private string _gunConfig;

		public static string[,] gun4irBoards = new string[8, 2]
		{
			  {
				"2341",
				"8046"
			  },
			  {
				"2341",
				"8047"
			  },
			  {
				"2341",
				"8048"
			  },
			  {
				"2341",
				"8049"
			  },
			  {
				"2341",
				"8042"
			  },
			  {
				"2341",
				"8043"
			  },
			  {
				"2341",
				"8044"
			  },
			  {
				"2341",
				"8045"
			  }
		};

		public gun_preconfig(int gunType, int gunIndex, string gunConfig)
		{
			_gunType = gunType;
			_gunIndex = gunIndex;
			_gunConfig = gunConfig;

			InitializeComponent();
			this.Size = new Size(858, 371);
			grp_gun4ir.Visible = false;
			grp_sinden.Visible = false;

			lbl_gunindex.Text = gunIndex.ToString();
			string gunTypeStr = "";
			switch (gunType)
			{
				case 0:
					gunTypeStr = "Gun4IR: GunCon2";
					break;
				case 1:
					gunTypeStr = "Gun4IR : GunCon1";
					break;
				case 2:
					gunTypeStr = "Sinden Lightgun";
					break;
				case 3:
					gunTypeStr = "Wiimote(Lichtknarre)";
					break;
				default:
					gunTypeStr = "";
					break;
			}
			lbl_guntype.Text = gunTypeStr;

			if (gunType == 0 || gunType == 1)
			{
				grp_gun4ir.Location = new Point(12, 64);
				grp_gun4ir.Visible = true;

				var boardList = gun4irBoards;
				for (int index = 0; index < boardList.GetUpperBound(0) + 1; ++index)
				{
					List<string> stringList = ComPortNames(boardList[index, 0], boardList[index, 1]);
					if (stringList.Count > 0)
					{
						foreach (string portName in SerialPortLib2.SerialPortInput.GetPorts())
						{
							if (stringList.Contains(portName))
							{
								cmb_comport.Items.Add(portName);
								try
								{
									var gunSerial = new SerialPortInput(false);
									gunSerial.SetPort(portName, 9600);
									gunSerial.Connect();
									gunSerial.SendMessage(Encoding.ASCII.GetBytes("M0x1"));
									Thread.Sleep(100);
									gunSerial.Disconnect();
								}
								catch (Exception e) { }
							}
						}
					}
				}

			}

			if (gunType == 2)
			{
				grp_sinden.Location = new Point(12, 64);
				grp_sinden.Visible = true;
				if (Process.GetProcesses().Any(p => p.ProcessName.Equals("Lightgun", StringComparison.OrdinalIgnoreCase)))
				{
					Process.Start(new ProcessStartInfo
					{
						FileName = "taskkill",
						Arguments = $"/F /IM Lightgun.exe",
						CreateNoWindow = true,
						UseShellExecute = false
					});
					Thread.Sleep(1000);
				}


				listSinden.Clear();




				string[] portNames = SerialPortLib2.SerialPortInput.GetPorts();
				if (portNames.Length != 0)
					Array.Sort<string>(portNames);
				ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("Select * from WIN32_SerialPort");
				foreach (ManagementObject managementObject in managementObjectSearcher.Get())
				{
					string str1 = managementObject.GetPropertyValue("DeviceID").ToString();
					string str2 = managementObject.GetPropertyValue("PNPDeviceID").ToString();
					if (str2.Contains("VID_16C0&PID_0F01"))
						listSinden.Add(("SindenLightgun-Blue " + str1, str1, "VID_16C0&PID_0F01"));
					if (str2.Contains("VID_16C0&PID_0F02"))
						listSinden.Add(("SindenLightgun-Red " + str1, str1, "VID_16C0&PID_0F02"));
					if (str2.Contains("VID_16C0&PID_0F38"))
						listSinden.Add(("SindenLightgun-Black " + str1, str1, "VID_16C0&PID_0F38"));
					if (str2.Contains("VID_16C0&PID_0F39"))
						listSinden.Add(("SindenLightgun-Player2 " + str1, str1, "VID_16C0&PID_0F39"));

				}
				foreach (var sindengun in listSinden)
				{
					cmb_selectSinden.Items.Add(sindengun.Item1);
				}


			}
			if (gunType == 3)
			{
				grp_wiimote.Location = new Point(12, 64);
				grp_wiimote.Visible = true;
				InitializeFormAsync();
			}


		}

		private List<string> ComPortNames(string VID, string PID)
		{
			Regex regex = new Regex(string.Format("^VID_{0}.PID_{1}", (object)VID, (object)PID), RegexOptions.IgnoreCase);
			List<string> stringList = new List<string>();
			RegistryKey registryKey1 = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
			foreach (string subKeyName1 in registryKey1.GetSubKeyNames())
			{
				RegistryKey registryKey2 = registryKey1.OpenSubKey(subKeyName1);
				foreach (string subKeyName2 in registryKey2.GetSubKeyNames())
				{
					if (regex.Match(subKeyName2).Success)
					{
						RegistryKey registryKey3 = registryKey2.OpenSubKey(subKeyName2);
						foreach (string subKeyName3 in registryKey3.GetSubKeyNames())
						{
							try
							{
								RegistryKey registryKey4 = registryKey3.OpenSubKey(subKeyName3).OpenSubKey("Device Parameters");
								stringList.Add((string)registryKey4.GetValue("PortName"));
							}
							catch
							{
							}
						}
					}
				}
			}
			return stringList;
		}


		private void cmb_comport_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cmb_comport.SelectedItem.ToString() != "")
			{
				btn_savecomport.Enabled = true;
				try
				{
					var gunSerial = new SerialPortInput(false);
					gunSerial.SetPort(cmb_comport.SelectedItem.ToString(), 9600);
					gunSerial.Connect();
					gunSerial.SendMessage(Encoding.ASCII.GetBytes("F1x2x2x"));
					Thread.Sleep(100);
					gunSerial.Disconnect();
				}
				catch { }
			}
			else btn_savecomport.Enabled = false;
		}

		private async void btn_savecomport_Click(object sender, EventArgs e)
		{
			int com_port = 0;
			try
			{
				int.TryParse(cmb_comport.SelectedItem.ToString().Replace("COM", "").Trim(), out com_port);
			}
			catch { }
			gunCom = com_port;




			btn_savecomport.Enabled = false;
			_stopListening = false;
			var directInput = _directInput;
			Joystick joystick = null;
			var devicesInstance = new List<DeviceInstance>();
			devicesInstance.AddRange(directInput.GetDevices().Where(x => x.Type != DeviceType.Mouse && x.UsagePage != UsagePage.VendorDefinedBegin && x.Usage != UsageId.AlphanumericBitmapSizeX && x.Usage != UsageId.AlphanumericAlphanumericDisplay && x.UsagePage != unchecked((UsagePage)0xffffff43) && x.UsagePage != UsagePage.Vr).ToList());
			List<string> guids = new List<string>();

			foreach (var device in devicesInstance)
			{
				if (device.InstanceName.ToLower().Contains("gun4ir"))
				{
					guids.Add(device.InstanceGuid.ToString());
				}
			}

			if (guids.Count > 0)
			{
				foreach (var guid in guids)
				{
					taskJoystick.Add(Task.Run(() => SpawnDirectInputListener(guid)));
					await Task.Delay(1000); // Attendre 1 seconde entre les démarrages
				}
				lbl_presstrigger_gun4ir.Visible = true;

				await Task.WhenAll(taskJoystick);
				taskJoystick.Clear();
			}
			btn_savecomport.Enabled = true;

			btn_configure_key_gun4ir.Enabled = true;

			if (_gunType == 0) btn_gunir_done.Enabled = true;
			if (_gunType == 1) label_warning_guncon1.Visible = true;
		}

		public void gun4irFound()
		{
			//StopMonitor();
			this.Invoke(new Action(() =>
			{
				lbl_presstrigger_gun4ir.Visible = false;
				lbl_gunguid.Text = gunGuid;
			}));
			_stopListening = true;


		}

		public void wiimoteFound()
		{
			//StopMonitor();
			this.Invoke(new Action(() =>
			{
				lbl_presstrigger_wiimote.Visible = false;
				lbl_gunguidwiimote.Text = gunGuid;
			}));
			_stopListening = true;


		}

		public void StopMonitor()
		{
			if (_stopListening == false && threadJoystick != null)
			{
				_stopListening = true;
				foreach (var t in threadJoystick)
				{
					if (t.IsAlive)
					{
						t.Join();
					}
				}
			}
			threadJoystick.Clear();
		}

		private async Task SpawnDirectInputListener(string joyGuid)
		{
			string deviceName = "";
			var directInput = _directInput;
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
					deviceName = device.InstanceName;
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
			}
			joystick = new Joystick(directInput, device_guid);
			joystick.Properties.BufferSize = 512;
			joystick.Acquire();


			while (!_stopListening)
			{
				try
				{
					joystick.Poll();
					var datas = joystick.GetBufferedData();
					foreach (var key in datas)
					{
						int pressed = 0;
						string inputText = "";
						// 4 Direction input
						if (key.Offset == JoystickOffset.PointOfViewControllers0 ||
							key.Offset == JoystickOffset.PointOfViewControllers1 ||
							key.Offset == JoystickOffset.PointOfViewControllers2 ||
							key.Offset == JoystickOffset.PointOfViewControllers3)
						{

						}
						// 2 Direction input
						else if (key.Offset == JoystickOffset.X ||
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


						}
						// Digital input
						else
						{
							if (key.Value == 128)
							{
								inputText = key.Offset.ToString();
								pressed = 1;
								if ((_gunType == 0 || _gunType == 1) && inputText == "Buttons0")
								{
									gunGuid = device_guid.ToString();
									gunName = deviceName;
									gun4irFound();
								}
								if (_gunType == 3 && inputText.StartsWith("Buttons"))
								{
									gunGuid = device_guid.ToString();
									gunName = deviceName;
									wiimoteFound();
								}
							}
						}


					}
					await Task.Delay(10);
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
					}
					joystick = new Joystick(new DirectInput(), device_guid);
					joystick.Properties.BufferSize = 512;
					joystick.Acquire();
				}
			}
			joystick.Unacquire();
		}

		private void btn_configure_key_gun4ir_Click(object sender, EventArgs e)
		{
			//StopMonitor();
			string base_content = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("gunpreset_" + _gunType + ".json")))).ReadToEnd();
			base_content = base_content.Replace("[gunguid]", gunGuid);
			base_content = base_content.Replace("[gunname]", gunName);
			string typeGunTxt = "guncon2";
			if (_gunType == 1) typeGunTxt = "guncon1";



			var frm = new dinputgun(_gunIndex, typeGunTxt, base_content);
			var result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{
				var json = frm.Dialogconfig;
				Dialogconfig = json;
				this.DialogResult = DialogResult.OK;
				this.Close();

			}


		}

		private void btn_gunir_done_Click(object sender, EventArgs e)
		{
			string base_content = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("gunpreset_" + _gunType + ".json")))).ReadToEnd();
			base_content = base_content.Replace("[gunguid]", gunGuid);
			base_content = base_content.Replace("[gunname]", gunName);
			string typeGunTxt = "guncon2";
			if (_gunType == 1) typeGunTxt = "guncon1";



			Dialogconfig = base_content;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void kryptonButton1_Click(object sender, EventArgs e)
		{
			var frm = new dinputgun(_gunIndex, "gamepad", "");
			var result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{
				var json = frm.Dialogconfig;
				File.WriteAllText(@"E:/out.json", json);

			}
		}

		private void cmb_selectSinden_SelectedIndexChanged(object sender, EventArgs e)
		{
			btn_siden_enable_joystick.Visible = false;
			btn_siden_enable_joystick.Enabled = false;
			lbl_sinden_wrongversion.Visible = false;
			lbl_sinden_nojoystick.Visible = false;
			btn_sinden_configurekeys.Enabled = false;
			btn_sinden_done.Enabled = false;

			if (cmb_selectSinden.SelectedIndex >= 0)
			{
				if (cmb_selectSinden.SelectedItem.ToString().StartsWith("SindenLightgun-Black")) gunSindenType = 1;
				if (cmb_selectSinden.SelectedItem.ToString().StartsWith("SindenLightgun-Blue")) gunSindenType = 2;
				if (cmb_selectSinden.SelectedItem.ToString().StartsWith("SindenLightgun-Red")) gunSindenType = 3;
				if (cmb_selectSinden.SelectedItem.ToString().StartsWith("SindenLightgun-Player2")) gunSindenType = 4;
			}
			else
			{
				gunSindenType = 0;
			}

			int selectedIndex = cmb_selectSinden.SelectedIndex;
			if (selectedIndex >= 0 && selectedIndex < listSinden.Count)
			{
				string portName = listSinden[cmb_selectSinden.SelectedIndex].Item2;
				string pidvid = listSinden[cmb_selectSinden.SelectedIndex].Item3;

				string guid = "";
				string Name = "";
				var directInput = _directInput;
				var ddevices = directInput.GetDevices();
				foreach (var deviceInstance in ddevices)
				{
					if (!IsStickType(deviceInstance))
						continue;

					var joystick = new Joystick(directInput, deviceInstance.InstanceGuid);
					if (joystick.Properties.InterfacePath.ToUpper().Contains(pidvid))
					{

						guid = deviceInstance.InstanceGuid.ToString();
						Name = deviceInstance.InstanceName;


					}
				}
				gunGuid = guid;
				gunName = Name;
				lbl_gunguidsinden.Text = guid;

				if (gunGuid != "")
				{
					btn_sinden_configurekeys.Enabled = true;
					btn_sinden_done.Enabled = true;

				}
				else
				{
					btn_sinden_configurekeys.Enabled = false;
					btn_sinden_done.Enabled = false;

					if (Process.GetProcesses().Any(p => p.ProcessName.Equals("Lightgun", StringComparison.OrdinalIgnoreCase)))
					{
						Process.Start(new ProcessStartInfo
						{
							FileName = "taskkill",
							Arguments = $"/F /IM Lightgun.exe",
							CreateNoWindow = true,
							UseShellExecute = false
						});
						Thread.Sleep(2000);
					}

					var gunSerial = new SerialPortInput(false);
					gunSerial.SetPort(portName, 115200);
					var tcs = new TaskCompletionSource<bool>();
					string version = "";

					gunSerial.MessageReceived += (object sender, MessageReceivedEventArgs args) =>
					{
						if (args.Data.Length >= 2)
						{
							byte mainVersion = args.Data[0];
							byte mainVersionSub = args.Data[1];

							// Enregistrer la version dans une chaîne de caractères
							version = $"{mainVersion}.{mainVersionSub}";

							tcs.SetResult(true);
						}
					};
					byte[] message = new byte[7]
					{
					Convert.ToByte(170),
					Convert.ToByte(101),
					Convert.ToByte(0),
					Convert.ToByte(0),
					Convert.ToByte(0),
					Convert.ToByte(0),
					Convert.ToByte(187)
					};
					gunSerial.Connect();
					gunSerial.SendMessage(message);

					if (!tcs.Task.Wait(500)) // 1000 millisecondes = 1 seconde
					{

					}

					gunSerial.Disconnect();

					if (version != "1.9")
					{
						lbl_sinden_wrongversion.Visible = true;
					}
					else
					{
						lbl_sinden_nojoystick.Visible = true;
						btn_siden_enable_joystick.Visible = true;
						btn_siden_enable_joystick.Enabled = true;

					}

				}





				/*
				var gunSerial = new SerialPortInput(false);
				gunSerial.SetPort(portName, 115200);
				var tcs = new TaskCompletionSource<bool>();
				string version = "";

				gunSerial.MessageReceived += (object sender, MessageReceivedEventArgs args) =>
				{
					if (args.Data.Length >= 2)
					{
						byte mainVersion = args.Data[0];
						byte mainVersionSub = args.Data[1];

						// Enregistrer la version dans une chaîne de caractères
						version = $"{mainVersion}.{mainVersionSub}";

						tcs.SetResult(true);
					}
				};

				byte[] message = new byte[7]
				{
					Convert.ToByte(170),
					Convert.ToByte(101),
					Convert.ToByte(0),
					Convert.ToByte(0),
					Convert.ToByte(0),
					Convert.ToByte(0),
					Convert.ToByte(187)
				};

				gunSerial.Connect();
				gunSerial.SendMessage(message);

				if (!tcs.Task.Wait(1000)) // 1000 millisecondes = 1 seconde
				{
					
				}

				gunSerial.Disconnect();
				MessageBox.Show(version);
				*/
			}


		}

		private static bool IsStickType(DeviceInstance deviceInstance)
		{
			return deviceInstance.Type == SharpDX.DirectInput.DeviceType.Joystick
					|| deviceInstance.Type == SharpDX.DirectInput.DeviceType.Gamepad
					|| deviceInstance.Type == SharpDX.DirectInput.DeviceType.FirstPerson
					|| deviceInstance.Type == SharpDX.DirectInput.DeviceType.Flight
					|| deviceInstance.Type == SharpDX.DirectInput.DeviceType.Driving
					|| deviceInstance.Type == SharpDX.DirectInput.DeviceType.Supplemental;
		}

		private void gun_preconfig_Load(object sender, EventArgs e)
		{

		}

		private void btn_siden_enable_joystick_Click(object sender, EventArgs e)
		{
			int selectedIndex = cmb_selectSinden.SelectedIndex;
			if (selectedIndex >= 0 && selectedIndex < listSinden.Count)
			{
				string portName = listSinden[cmb_selectSinden.SelectedIndex].Item2;
				string pidvid = listSinden[cmb_selectSinden.SelectedIndex].Item3;

				string guid = "";
				var directInput = _directInput;
				var ddevices = directInput.GetDevices();
				foreach (var deviceInstance in ddevices)
				{
					if (!IsStickType(deviceInstance))
						continue;

					var joystick = new Joystick(directInput, deviceInstance.InstanceGuid);
					if (joystick.Properties.InterfacePath.ToUpper().Contains(pidvid))
					{
						guid = deviceInstance.InstanceGuid.ToString();
					}
				}
				if (guid == "")
				{

					if (Process.GetProcesses().Any(p => p.ProcessName.Equals("Lightgun", StringComparison.OrdinalIgnoreCase)))
					{
						Process.Start(new ProcessStartInfo
						{
							FileName = "taskkill",
							Arguments = $"/F /IM Lightgun.exe",
							CreateNoWindow = true,
							UseShellExecute = false
						});
						Thread.Sleep(2000);
					}

					var gunSerial = new SerialPortInput(false);
					gunSerial.SetPort(portName, 115200);
					var tcs = new TaskCompletionSource<bool>();
					string version = "";

					gunSerial.MessageReceived += (object sender, MessageReceivedEventArgs args) =>
					{
						if (args.Data.Length >= 2)
						{
							byte mainVersion = args.Data[0];
							byte mainVersionSub = args.Data[1];

							// Enregistrer la version dans une chaîne de caractères
							version = $"{mainVersion}.{mainVersionSub}";

							tcs.SetResult(true);
						}
					};
					byte[] message = new byte[7]
					{
					Convert.ToByte(170),
					Convert.ToByte(101),
					Convert.ToByte(0),
					Convert.ToByte(0),
					Convert.ToByte(0),
					Convert.ToByte(0),
					Convert.ToByte(187)
					};
					gunSerial.Connect();
					gunSerial.SendMessage(message);

					if (!tcs.Task.Wait(500)) // 1000 millisecondes = 1 seconde
					{

					}
					gunSerial.Disconnect();

					bool doMessageBox = false;
					if (version == "1.9")
					{
						Thread.Sleep(1000);
						gunSerial = new SerialPortInput(false);
						gunSerial.SetPort(portName, 115200);
						byte[] buffer = new byte[7]
							{
							  Convert.ToByte(170),
							  Convert.ToByte(184),
							  Convert.ToByte(1),
							  Convert.ToByte(0),
							  Convert.ToByte(0),
							  Convert.ToByte(0),
							  Convert.ToByte(187)
							};
						gunSerial.Connect();
						gunSerial.SendMessage(buffer);
						Thread.Sleep(500);
						gunSerial.Disconnect();
						doMessageBox = true;
					}

					if (doMessageBox)
					{
						MessageBox.Show("Please disconect/reconnect your sinden gun from usb, wait few seconds and press OK");
						cmb_selectSinden_SelectedIndexChanged(null, null);
					}

				}
			}
		}

		private void btn_sinden_configurekeys_Click(object sender, EventArgs e)
		{
			//StopMonitor();
			string base_content = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("gunpreset_" + _gunType + ".json")))).ReadToEnd();
			base_content = base_content.Replace("[gunguid]", gunGuid);
			base_content = base_content.Replace("[gunname]", gunName);
			string typeGunTxt = "sinden";



			var frm = new dinputgun(_gunIndex, typeGunTxt, base_content);
			var result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{
				var json = frm.Dialogconfig;
				Dialogconfig = json;
				this.DialogResult = DialogResult.OK;
				this.Close();

			}
		}

		private void btn_sinden_done_Click(object sender, EventArgs e)
		{
			string base_content = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("gunpreset_" + _gunType + ".json")))).ReadToEnd();
			base_content = base_content.Replace("[gunguid]", gunGuid);
			base_content = base_content.Replace("[gunname]", gunName);
			string typeGunTxt = "sinden";


			Dialogconfig = base_content;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private async void InitializeFormAsync()
		{
			// Appelez votre méthode asynchrone
			await StartListeningAsync();
		}
		private async Task StartListeningAsync()
		{
			_stopListening = false;
			var directInput = _directInput;
			var devicesInstance = new List<DeviceInstance>();

			devicesInstance.AddRange(directInput.GetDevices()
				.Where(x => x.Type != DeviceType.Mouse &&
							x.UsagePage != UsagePage.VendorDefinedBegin &&
							x.Usage != UsageId.AlphanumericBitmapSizeX &&
							x.Usage != UsageId.AlphanumericAlphanumericDisplay &&
							x.UsagePage != unchecked((UsagePage)0xffffff43) &&
							x.UsagePage != UsagePage.Vr)
				.ToList());

			List<string> guids = new List<string>();

			foreach (var device in devicesInstance)
			{
				if (device.InstanceName.ToLower().Contains("vjoy"))
				{
					guids.Add(device.InstanceGuid.ToString());
				}
			}

			if (guids.Count > 0)
			{
				var taskJoystick = new List<Task>();

				foreach (var guid in guids)
				{
					taskJoystick.Add(Task.Run(() => SpawnDirectInputListener(guid)));
					await Task.Delay(1000); // Attendre 1 seconde entre les démarrages
				}

				lbl_presstrigger_wiimote.Visible = true;

				await Task.WhenAll(taskJoystick);
				taskJoystick.Clear();
			}
			btn_wiimote_configurekeys.Enabled = true;
			btn_wiimote_done.Enabled = true;
		}

		private void btn_wiimote_done_Click(object sender, EventArgs e)
		{
			string base_content = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("gunpreset_" + _gunType + ".json")))).ReadToEnd();
			base_content = base_content.Replace("[gunguid]", gunGuid);
			base_content = base_content.Replace("[gunname]", gunName);
			string typeGunTxt = "wiimote";



			Dialogconfig = base_content;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void btn_wiimote_configurekeys_Click(object sender, EventArgs e)
		{
			string base_content = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("gunpreset_" + _gunType + ".json")))).ReadToEnd();
			base_content = base_content.Replace("[gunguid]", gunGuid);
			base_content = base_content.Replace("[gunname]", gunName);
			string typeGunTxt = "wiimote";



			var frm = new dinputgun(_gunIndex, typeGunTxt, base_content);
			var result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{
				var json = frm.Dialogconfig;
				Dialogconfig = json;
				this.DialogResult = DialogResult.OK;
				this.Close();

			}
		}
	}
}
