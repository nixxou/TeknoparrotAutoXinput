using Krypton.Toolkit;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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

		public dinputwheel()
		{

			InitializeComponent();
			_joystickCollection.Clear();
			devices = new List<DeviceInstance>();
			_stopListening = false;
			devices.AddRange(_directInput.GetDevices().Where(x => x.Type != DeviceType.Mouse && x.UsagePage != UsagePage.VendorDefinedBegin && x.Usage != UsageId.AlphanumericBitmapSizeX && x.Usage != UsageId.AlphanumericAlphanumericDisplay && x.UsagePage != unchecked((UsagePage)0xffffff43) && x.UsagePage != UsagePage.Vr).ToList());
			foreach (var device in devices)
			{
				cmb_devicelist.Items.Add(device.InstanceName);
			}
		}

		private void cmb_devicelist_SelectedIndexChanged(object sender, EventArgs e)
		{
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
	}
}
