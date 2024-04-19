using Krypton.Toolkit;
using Newtonsoft.Json;
using SharpDX.DirectInput;
using SharpDX.Multimedia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeknoparrotAutoXinput
{
	public partial class dinputgun : Form
	{
		public int GunIndex;
		public string GunType;

		private List<Joystick> _joystickCollection = new List<Joystick>();
		private readonly DirectInput _directInput = new DirectInput();
		private static bool _stopListening;
		private List<DeviceInstance> devices = new List<DeviceInstance>();
		private Thread threadJoystick;
		private Dictionary<string, JoystickButtonData> buttonData = new Dictionary<string, JoystickButtonData>();

		public string FocusedTextBoxName = "";


		public dinputgun(int gunIndex, string gunType)
		{
			InitializeComponent();
			if (gunIndex != 1 && gunIndex != 2) { return; }
			switch (gunType)
			{
				case "sinden":
					BackgroundImage = Properties.Resources.sinden;
					break;
				case "gamepad":
					BackgroundImage = Properties.Resources.gun360;
					label11.Text = "Y";
					label6.Text = "A";
					label5.Text = "X";
					label3.Text = "LB";

					label11.Location = new Point(label11.Location.X+90, label11.Location.Y);
					label6.Location = new Point(label6.Location.X + 82, label6.Location.Y);
					label5.Location = new Point(label5.Location.X + 80, label5.Location.Y);
					label3.Location = new Point(label3.Location.X + 20, label3.Location.Y);

					break;
				case "gun4ir":
					BackgroundImage = Properties.Resources.gun4ir;
					label11.Text = "B";
					label6.Text = "2";
					label5.Text = "A";
					label3.Text = "1";
					label11.Location = new Point(label11.Location.X + 90, label11.Location.Y);
					label6.Location = new Point(label6.Location.X + 82, label6.Location.Y);
					label5.Location = new Point(label5.Location.X + 80, label5.Location.Y);
					label3.Location = new Point(label3.Location.X + 20, label3.Location.Y);
					break;
				case "wiimote":
					BackgroundImage = Properties.Resources.wiimote;
					label11.Text = "minus";
					label6.Text = "2";
					label5.Text = "1";
					label3.Text = "A";
					label11.Location = new Point(label11.Location.X + 60, label11.Location.Y);
					label6.Location = new Point(label6.Location.X + 82, label6.Location.Y);
					label5.Location = new Point(label5.Location.X + 80, label5.Location.Y);
					label3.Location = new Point(label3.Location.X + 20, label3.Location.Y);
					break;
			}
			GunIndex = gunIndex;
			GunType = gunType;

			_joystickCollection.Clear();
			devices = new List<DeviceInstance>();
			_stopListening = false;
			devices.AddRange(_directInput.GetDevices().Where(x => x.Type != DeviceType.Mouse && x.UsagePage != UsagePage.VendorDefinedBegin && x.Usage != UsageId.AlphanumericBitmapSizeX && x.Usage != UsageId.AlphanumericAlphanumericDisplay && x.UsagePage != unchecked((UsagePage)0xffffff43) && x.UsagePage != UsagePage.Vr).ToList());
			foreach (var device in devices)
			{
				cmb_devicelist.Items.Add(device.InstanceName);
			}
			LoadConfig();

		}

		private void dinputgun_Load(object sender, EventArgs e)
		{
			
		}

		private void cmb_devicelist_SelectedIndexChanged(object sender, EventArgs e)
		{
			foreach (Control control in this.Controls)
			{
				if (control is KryptonTextBox)
				{
					control.Enabled = true;
				}
			}

			if (_stopListening == false && threadJoystick != null)
			{
				_stopListening = true;
				threadJoystick.Join();
				threadJoystick = null;
				_stopListening = false;
			}
			int index = cmb_devicelist.SelectedIndex;
			var t = devices[index];
			var joystick = new Joystick(new DirectInput(), t.InstanceGuid);
			joystick.Properties.BufferSize = 512;
			joystick.Acquire();
			threadJoystick = new Thread(() => SpawnDirectInputListener(joystick, t));
			threadJoystick.Start();
		}

		private void SpawnDirectInputListener(Joystick joystick, DeviceInstance deviceInstance)
		{
			_joystickCollection.Add(joystick);
			// Acquire the joystick
			try
			{
				while (!_stopListening)
				{
					joystick.Poll();
					var datas = joystick.GetBufferedData();
					foreach (var key in datas)
					{
						//SetTextBoxText(state, deviceInstance);
						JoystickButtonData button = null;
						string inputText = "";

						// 4 Direction input
						if (key.Offset == JoystickOffset.PointOfViewControllers0 ||
							key.Offset == JoystickOffset.PointOfViewControllers1 ||
							key.Offset == JoystickOffset.PointOfViewControllers2 ||
							key.Offset == JoystickOffset.PointOfViewControllers3)
						{
							// Not neutral
							if (key.Value != -1)
							{
								if (key.Value == 0)
									inputText = key.Offset + " Up";
								else if (key.Value == 9000)
									inputText = key.Offset + " Right";
								else if (key.Value == 18000)
									inputText = key.Offset + " Down";
								else if (key.Value == 27000)
									inputText = key.Offset + " Left";

								button = new JoystickButtonData
								{
									Button = (int)key.Offset,
									IsAxis = false,
									PovDirection = key.Value,
									JoystickGuid = deviceInstance.InstanceGuid
								};
							}
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


							if (FocusedTextBoxName == "txt_LightgunX" && key.Offset == JoystickOffset.Y) continue;
							if (FocusedTextBoxName == "txt_LightgunY" && key.Offset == JoystickOffset.X) continue;

							if (FocusedTextBoxName != "txt_LightgunX" && FocusedTextBoxName != "txt_LightgunY" && FocusedTextBoxName != "txt_LightgunPedalRight" && FocusedTextBoxName != "txt_LightgunPedalLeft" && FocusedTextBoxName != "txt_LightgunRightX" && FocusedTextBoxName != "txt_LightgunRightY" && FocusedTextBoxName != "txt_LightgunWheelX") continue;
							// Positive direction
							if (key.Value > short.MaxValue + 15000)
							{
								inputText = key.Offset + " +";

								button = new JoystickButtonData
								{
									Button = (int)key.Offset,
									IsAxis = true,
									IsAxisMinus = false,
									JoystickGuid = deviceInstance.InstanceGuid
								};
							}
							// Negative direction
							else if (key.Value < short.MaxValue - 15000)
							{
								if (key.Offset == JoystickOffset.X ||
								key.Offset == JoystickOffset.Y)
								{
									if (FocusedTextBoxName == "txt_LightgunX") continue;
									if (FocusedTextBoxName == "txt_LightgunY") continue;
								}


								inputText = key.Offset + " -";

								button = new JoystickButtonData
								{
									Button = (int)key.Offset,
									IsAxis = true,
									IsAxisMinus = true,
									JoystickGuid = deviceInstance.InstanceGuid
								};
							}
						}
						// Digital input
						else
						{
							if (key.Value == 128)
							{
								if (deviceInstance.Type == DeviceType.Keyboard)
								{
									inputText = "Button " + ((Key)key.Offset - 47).ToString();
								}
								else
								{
									inputText = key.Offset.ToString();
								}

								button = new JoystickButtonData
								{
									Button = (int)key.Offset,
									IsAxis = false,
									JoystickGuid = deviceInstance.InstanceGuid
								};
							}
						}

						if (button != null)
						{
							this.Invoke((MethodInvoker)delegate ()
							{
								foreach (Control control in this.Controls)
								{
									// Vérifier si le contrôle est un TextBox et s'il a le focus
									if (control is KryptonTextBox textBox && textBox.Focused)
									{
										button.Title = deviceInstance.Type + " " + inputText;
										string XinputTitle = control.Name.Substring(4).Trim();
										if (XinputTitle.EndsWith("plus"))
										{
											XinputTitle = XinputTitle.Substring(0, XinputTitle.Length - 4) + "+";
										}
										button.XinputTitle = XinputTitle;

										if (buttonData.ContainsKey(XinputTitle))
										{
											buttonData[XinputTitle] = button;
										}
										else
										{
											buttonData.Add(XinputTitle, button);
										}

										textBox.Text = deviceInstance.Type + " " + inputText;
										break;

										//textBox1.Invoke((MethodInvoker)delegate { textBox1.Text = inputText; });
									}
								}
							});
						}



					}
					Thread.Sleep(10);
				}
			}
			catch (Exception)
			{

			}
			joystick.Unacquire();
		}

		private void LoadConfig()
		{
			foreach (Control control in this.Controls)
			{
				// Vérifier si le contrôle est un TextBox et s'il a le focus
				if (control is KryptonTextBox)
				{
					control.Enabled = false;
					KryptonTextBox txtControl = (KryptonTextBox)control;
					txtControl.ReadOnly = true;
				}
			}

			string json = "";
			if (GunIndex == 1)
			{
				switch (GunType)
				{
					case "sinden":
						json = ConfigurationManager.MainConfig.bindingDinputGunASinden;
						break;
					case "gamepad":
						json = ConfigurationManager.MainConfig.bindingDinputGunAXbox;
						break;
					case "gun4ir":
						json = ConfigurationManager.MainConfig.bindingDinputGunAGun4ir;
						break;
					case "wiimote":
						json = ConfigurationManager.MainConfig.bindingDinputGunAWiimote;
						break;
					default:
						json = "";
						break;
				}
			}
			if (GunIndex == 2)
			{
				switch (GunType)
				{
					case "sinden":
						json = ConfigurationManager.MainConfig.bindingDinputGunBSinden;
						break;
					case "gamepad":
						json = ConfigurationManager.MainConfig.bindingDinputGunBXbox;
						break;
					case "gun4ir":
						json = ConfigurationManager.MainConfig.bindingDinputGunBGun4ir;
						break;
					case "wiimote":
						json = ConfigurationManager.MainConfig.bindingDinputGunBWiimote;
						break;
					default:
						json = "";
						break;
				}
			}


			if (!string.IsNullOrEmpty(json))
			{
				buttonData = (Dictionary<string, JoystickButtonData>)JsonConvert.DeserializeObject<Dictionary<string, JoystickButtonData>>(json);
				foreach (Control control in this.Controls)
				{
					// Vérifier si le contrôle est un TextBox et s'il a le focus
					if (control is KryptonTextBox)
					{
						string XinputTitle = control.Name.Substring(4).Trim();
						if (XinputTitle.EndsWith("plus"))
						{
							XinputTitle = XinputTitle.Substring(0, XinputTitle.Length - 4) + "+";
						}
						if (buttonData.ContainsKey(XinputTitle))
						{
							control.Text = buttonData[XinputTitle].Title;
						}
					}
				}
			}
		}

		private bool SaveConfig()
		{
			Dictionary<string, JoystickButtonData> buttonDataFinal = new Dictionary<string, JoystickButtonData>();
			foreach (Control control in this.Controls)
			{
				// Vérifier si le contrôle est un TextBox et s'il a le focus
				if (control is KryptonTextBox)
				{
					string XinputTitle = control.Name.Substring(4).Trim();
					if (XinputTitle.EndsWith("plus"))
					{
						XinputTitle = XinputTitle.Substring(0, XinputTitle.Length - 4) + "+";
					}
					if (buttonData.ContainsKey(XinputTitle))
					{
						buttonDataFinal.Add(XinputTitle, buttonData[XinputTitle]);
					}
				}
			}
			string json = JsonConvert.SerializeObject(buttonDataFinal, Newtonsoft.Json.Formatting.Indented);

			bool validConfig = true;
			string errorConfig = "ERROR ! \n";
			if (!buttonDataFinal.ContainsKey("LightgunX") || !buttonDataFinal.ContainsKey("LightgunY") || !buttonDataFinal["LightgunX"].IsAxis || !buttonDataFinal["LightgunY"].IsAxis)
			{
				validConfig = false;
				errorConfig += "You must define axis for Analog X and Y \r\n";
			}

			Dictionary<string, string> AssignedButtons = new Dictionary<string, string>();
			foreach (var btnData in buttonDataFinal)
			{
				string assignedButton = btnData.Value.JoystickGuid.ToString() + "===" + btnData.Value.Button.ToString() + "===" + btnData.Value.PovDirection + "===" + (btnData.Value.IsAxis ? "true" : "false");
				if (AssignedButtons.ContainsKey(assignedButton))
				{
					bool exceptionCase = false;
					if(btnData.Key == "LightgunRightX" || btnData.Key == "LightgunWheelX")
					{
						if (AssignedButtons[assignedButton] == "LightgunRightX" || AssignedButtons[assignedButton] == "LightgunWheelX")
						{
							exceptionCase = true;
						}
					}
					if(!exceptionCase)
					{
						validConfig = false;
						errorConfig = $"{btnData.Value.Title} in {btnData.Key} is asigned multiple time \r\n";
					}

				}
				else
				{
					AssignedButtons.Add(assignedButton, btnData.Key);
				}
			}

			if (!validConfig)
			{
				MessageBox.Show(errorConfig);
				return false;
			}



			if (GunIndex == 1)
			{
				switch (GunType)
				{
					case "sinden":
						ConfigurationManager.MainConfig.bindingDinputGunASinden = json;
						break;
					case "gamepad":
						json = ConfigurationManager.MainConfig.bindingDinputGunAXbox = json;
						break;
					case "gun4ir":
						json = ConfigurationManager.MainConfig.bindingDinputGunAGun4ir = json;
						break;
					case "wiimote":
						json = ConfigurationManager.MainConfig.bindingDinputGunAWiimote = json;
						break;
					default:
						json = "";
						break;
				}
			}
			if (GunIndex == 2)
			{
				switch (GunType)
				{
					case "sinden":
						json = ConfigurationManager.MainConfig.bindingDinputGunBSinden = json;
						break;
					case "gamepad":
						json = ConfigurationManager.MainConfig.bindingDinputGunBXbox = json;
						break;
					case "gun4ir":
						json = ConfigurationManager.MainConfig.bindingDinputGunBGun4ir = json;
						break;
					case "wiimote":
						json = ConfigurationManager.MainConfig.bindingDinputGunBWiimote = json;
						break;
					default:
						json = "";
						break;
				}
			}
			//MessageBox.Show(json);
			//ConfigurationManager.MainConfig.bindingDinputWheel = json;
			ConfigurationManager.SaveConfig();
			return true;

		}

		private void btn_Save_Click(object sender, EventArgs e)
		{
			if (SaveConfig())
			{
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void dinputgun_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_stopListening == false && threadJoystick != null)
			{
				_stopListening = true;
				threadJoystick.Join();
				threadJoystick = null;
				_stopListening = false;
			}
		}

		private void txt_focus(object sender, EventArgs e)
		{
			var txtbox = (Krypton.Toolkit.KryptonTextBox)sender;
			FocusedTextBoxName = txtbox.Name;
		}

		private void txt_Unfocus(object sender, EventArgs e)
		{
			FocusedTextBoxName = "";
		}

		private void txt_InputDevice0X_TextChanged(object sender, EventArgs e)
		{

		}

		private void label2_Click(object sender, EventArgs e)
		{

		}

		private void txt_LightgunTrigger_TextChanged(object sender, EventArgs e)
		{

		}

		private void dinputgun_Click(object sender, EventArgs e)
		{
			cmb_devicelist.Focus();
		}

		private void txt_clear(object sender, MouseEventArgs e)
		{
			if (sender is KryptonTextBox)
			{
				KryptonTextBox control = (KryptonTextBox)sender;
				string XinputTitle = control.Name.Substring(4).Trim();
				if (XinputTitle.EndsWith("plus"))
				{
					XinputTitle = XinputTitle.Substring(0, XinputTitle.Length - 4) + "+";
				}
				if (buttonData.ContainsKey(XinputTitle))
				{
					buttonData.Remove(XinputTitle);
					control.Text = "";
				}
				
			}
		}
	}
}
