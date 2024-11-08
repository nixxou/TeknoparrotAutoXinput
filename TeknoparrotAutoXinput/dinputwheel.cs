﻿using Krypton.Toolkit;
using Newtonsoft.Json;
using SharpDX.DirectInput;
using SharpDX.Multimedia;
using System.Data;

using DeviceType = SharpDX.DirectInput.DeviceType;
using Key = SharpDX.DirectInput.Key;

namespace TeknoparrotAutoXinput
{

	public partial class dinputwheel : KryptonForm
	{
		private List<Joystick> _joystickCollection = new List<Joystick>();
		private readonly DirectInput _directInput = new DirectInput();
		private static bool _stopListening;
		private List<DeviceInstance> devices = new List<DeviceInstance>();
		private Thread threadJoystick;

		private Dictionary<string, JoystickButtonData> buttonData = new Dictionary<string, JoystickButtonData>();

		public string Dialogconfig = null;
		public bool IsConfigured = false;

		public dinputwheel(string dialogconfig = null)
		{
			Dialogconfig = dialogconfig;
			InitializeComponent();
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
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
									inputText = "Button " + ((Key)key.Offset - 47).ToString();
								else
									inputText = key.Offset.ToString();

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

		private void dinputwheel_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_stopListening == false && threadJoystick != null)
			{
				_stopListening = true;
				threadJoystick.Join();
				threadJoystick = null;
				_stopListening = false;
			}
		}

		private void SaveConfig()
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

			IsConfigured = true;
			if (txt_InputDevice0LeftThumbInputDevice0Xplus.Text == "") IsConfigured = false;
			if (txt_InputDevice0LeftTrigger.Text == "") IsConfigured = false;
			if (txt_InputDevice0RightTrigger.Text == "") IsConfigured = false;

			string json = JsonConvert.SerializeObject(buttonDataFinal, Newtonsoft.Json.Formatting.Indented);
			if (Dialogconfig != null)
			{
				Dialogconfig = json;
				return;
			}
			ConfigurationManager.MainConfig.bindingDinputWheel = json;
			ConfigurationManager.SaveConfig();

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
			if (Dialogconfig != null) json = Dialogconfig;
			else json = ConfigurationManager.MainConfig.bindingDinputWheel;
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

		private void btn_Save_Click(object sender, EventArgs e)
		{
			SaveConfig();
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}
	}
}
