using Henooh.DeviceEmulator.Net;
using Henooh.DeviceEmulator.Net.Native;
using SharpDX.DirectInput;
using SharpDX.Multimedia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace TeknoparrotAutoXinput
{
	public static class ButtonToKeyManager
	{
		public static ButtonToKey buttonToKey = new ButtonToKey();
	}

	public class ButtonToKey
	{

		public Dictionary<string, (VirtualKeyCode, Key)> keyToAssign = new Dictionary<string, (VirtualKeyCode, Key)> {
			{"U", (VirtualKeyCode.VK_U,Key.U)},
			{"I", (VirtualKeyCode.VK_I,Key.I)},
			{"O", (VirtualKeyCode.VK_O,Key.O)},
			{"P", (VirtualKeyCode.VK_P,Key.P)},
			{"J", (VirtualKeyCode.VK_J,Key.J)},
			{"K", (VirtualKeyCode.VK_K,Key.K)},
			{"L", (VirtualKeyCode.VK_L,Key.L)}
		};
		public Dictionary<string, List<(string, string)>> assignedKeys = new Dictionary<string, List<(string,string)>>();

		private int keyNbAssigned = 0;
		private List<Thread> threadJoystick = new List<Thread>();

		private static bool _stopListening;
		private List<DeviceInstance> devices = new List<DeviceInstance>();
		private readonly DirectInput _directInput = new DirectInput();

		private List<string> guids = new List<string>();
		private Dictionary<string, int> KeyPressedStatus = new Dictionary<string, int>();

		KeyboardController keyboardController;

		public string GetFreeKey()
		{
			foreach(var keyA in keyToAssign)
			{
				string keyvalue = keyA.Key;
				if (!assignedKeys.ContainsKey(keyA.Key))
				{
					keyNbAssigned++;
					return keyvalue;
				}
			}
			return "";
		}

		public void Assign(string newKey, string OriginalGuid, string OriginalLabel)
		{
			if (!assignedKeys.ContainsKey(newKey))
			{
				assignedKeys.Add(newKey, new List<(string,string)>());
			}
			bool found = false;
			foreach(var tuple in assignedKeys[newKey])
			{
				if(tuple.Item1 == OriginalGuid && tuple.Item2 == OriginalLabel)
				{
					found = true;
				}
			}
			if(!found) assignedKeys[newKey].Add((OriginalGuid, OriginalLabel));
			if (!guids.Contains(OriginalGuid))
			{
				guids.Add(OriginalGuid);
			}
		}

		public string ContainAssigned(string guid, string label)
		{
			foreach(var k in assignedKeys)
			{
				foreach(var tuple in k.Value)
				{
					if(tuple.Item1 == guid && tuple.Item2 == label)
					{
						return k.Key;
					}
				}
			}
			return "";
		}

		public void StartMonitor()
		{

			foreach(var assignedKey in assignedKeys)
			{
				KeyPressedStatus.Add(assignedKey.Key, 0);
			}

			foreach(var guid in guids)
			{
				threadJoystick.Add(new Thread(() => SpawnDirectInputListener(guid)));
			}
			keyboardController = new Henooh.DeviceEmulator.Net.KeyboardController();
			foreach (var t in threadJoystick)
			{
				t.Start();
				Thread.Sleep(1000);
			}

		}

		public void StopMonitor()
		{
			if (_stopListening == false && threadJoystick != null)
			{
				_stopListening = true;
				foreach(var t in threadJoystick)
				{
					if(t.IsAlive)
					{
						t.Join();
					}
				}
			}
		}

		private void SpawnDirectInputListener(string joyGuid)
		{
			var LabelState = new Dictionary<string, bool>();
			var LabelToMonitor = new Dictionary<string, string>();
			foreach(var ak in assignedKeys)
			{
				foreach(var tuple in ak.Value)
				{
					if(tuple.Item1 == joyGuid)
					{
						LabelToMonitor.Add(tuple.Item2,ak.Key);
						LabelState.Add(tuple.Item2, false);
					}
				}
			}
			



			Utils.LogMessage($"Spawn JoyListener for {joyGuid} !");
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
			//var info1 = joystick.GetObjectInfoByOffset((int)JoystickOffset.Z);
			//var info2 = joystick.GetObjectInfoByOffset((int)JoystickOffset.Y);
			//var info3 = joystick.GetObjectInfoByOffset((int)JoystickOffset.RotationZ);

			

			while (!_stopListening)
			{
				try
				{
					Dictionary<string,int> keyPressedValue = new Dictionary<string,int>();
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
							// Not neutral
							if (key.Value != -1)
							{
								pressed = 1;
								if (key.Value == 0)
									inputText = key.Offset + " Up";
								else if (key.Value == 9000)
									inputText = key.Offset + " Right";
								else if (key.Value == 18000)
									inputText = key.Offset + " Down";
								else if (key.Value == 27000)
									inputText = key.Offset + " Left";
							}
							else
							{
								pressed = 0;
								inputText = key.Offset + " DirNeutral";
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
								pressed = 1;
							}
							// Negative direction
							else if (key.Value < short.MaxValue - 15000)
							{
								inputText = key.Offset + " -";
								pressed = 1;
							}
							else {
								inputText = key.Offset + " =";
								pressed = 0; 
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
								pressed = 1;
							}
							if(key.Value == 0)
							{
								if (deviceInstance.Type == DeviceType.Keyboard)
									inputText = "Button " + ((Key)key.Offset - 47).ToString();
								else
									inputText = key.Offset.ToString();
								pressed = 0;
							}
						}

						/*
						if (inputText.EndsWith("plus"))
						{
							inputText = inputText.Substring(0, inputText.Length - 4) + "+";
						}
						*/
						
						if (!string.IsNullOrEmpty(inputText))
						{
							inputText = deviceInstance.Type + " " + inputText;
							Utils.LogMessage(inputText + " : " + key.Value + " : " + pressed);

							if (inputText.EndsWith(" ="))
							{
								string direction = inputText.Replace(" =", " +");
								if (!keyPressedValue.ContainsKey(direction)) keyPressedValue.Add(direction, pressed);
								else keyPressedValue[direction] = pressed;

								direction = inputText.Replace(" =", " -");
								if (!keyPressedValue.ContainsKey(direction)) keyPressedValue.Add(direction, pressed);
								else keyPressedValue[direction] = pressed;
							}

							if (inputText.EndsWith("DirNeutral"))
							{
								string direction = inputText.Replace("DirNeutral", "Up");
								if (!keyPressedValue.ContainsKey(direction)) keyPressedValue.Add(direction, pressed);
								else keyPressedValue[direction] = pressed;

								direction = inputText.Replace("DirNeutral", "Down");
								if (!keyPressedValue.ContainsKey(direction)) keyPressedValue.Add(direction, pressed);
								else keyPressedValue[direction] = pressed;

								direction = inputText.Replace("DirNeutral", "Right");
								if (!keyPressedValue.ContainsKey(direction)) keyPressedValue.Add(direction, pressed);
								else keyPressedValue[direction] = pressed;

								direction = inputText.Replace("DirNeutral", "Left");
								if (!keyPressedValue.ContainsKey(direction)) keyPressedValue.Add(direction, pressed);
								else keyPressedValue[direction] = pressed;

							}
							else
							{
								if (!keyPressedValue.ContainsKey(inputText)) keyPressedValue.Add(inputText, pressed);
								else keyPressedValue[inputText] = pressed;
							}



							/*
							if(key.Value != 0 && inputText.EndsWith("+"))
							{
								string reverseText = inputText.Substring(0, inputText.Length - 1) + "-";
								if (!keyPressedValue.ContainsKey(reverseText)) keyPressedValue.Add(reverseText, 0);
								else keyPressedValue[reverseText] = 0;
							}
							if (key.Value != 0 && inputText.EndsWith("-"))
							{
								string reverseText = inputText.Substring(0, inputText.Length - 1) + "+";
								if (!keyPressedValue.ContainsKey(reverseText)) keyPressedValue.Add(reverseText, 0);
								else keyPressedValue[reverseText] = 0;
							}
							*/
						}

						/*
						if (inputText == "") continue;
						if (inputText == _keyShiftUp && deviceInstance.InstanceGuid.ToString() == _wheelGuid)
						{
							if (shiftUp != pressed)
							{
								shiftUp = pressed;
								if (pressed) keyboardController.Down(Henooh.DeviceEmulator.Net.Native.VirtualKeyCode.PRIOR);
								else keyboardController.Up(Henooh.DeviceEmulator.Net.Native.VirtualKeyCode.PRIOR);

								Utils.LogMessage($"shiftUp  {inputText} : {pressed}");
							}

						}
						if (inputText == _keyShiftDown && deviceInstance.InstanceGuid.ToString() == _wheelGuid)
						{
							if (shiftDown != pressed)
							{
								shiftDown = pressed;
								if (pressed) keyboardController.Down(Henooh.DeviceEmulator.Net.Native.VirtualKeyCode.NEXT);
								else keyboardController.Up(Henooh.DeviceEmulator.Net.Native.VirtualKeyCode.NEXT);
								Utils.LogMessage($"shiftDown  {inputText} : {pressed}");
							}
						}
						*/
					}
					foreach(var keyPressed in keyPressedValue)
					{
						string assignedKey = ContainAssigned(joyGuid, keyPressed.Key);
						if (assignedKey != "")
						{
							int value = keyPressed.Value;
							//Utils.LogMessage("val=" + value.ToString() + " for " + assignedKey);
							/*
							if (value != 0)
							{
								if (!KeyPressedStatus[aData.Key])
								{
									KeyPressedStatus[aData.Key] = true;
									Utils.LogMessage(aData.Key + " pressed");

								}
							}
							else
							{
								if (KeyPressedStatus[aData.Key])
								{
									KeyPressedStatus[aData.Key] = false;
									Utils.LogMessage(aData.Key + " rlz");
								}
							}
							*/
						}
					}

					foreach(var ltomonitor in LabelToMonitor)
					{
						string strKey = ltomonitor.Value;
						string label = ltomonitor.Key;

						if (keyPressedValue.ContainsKey(label))
						{
							bool state = (keyPressedValue[label] == 0) ? false : true;
							if (LabelState[label] != state)
							{
								LabelState[label] = state;
								Utils.LogMessage(label + " = " + (state ? "ON" : "OFF"));

								bool oldStateKey = (KeyPressedStatus[strKey] == 0) ? false : true;

								if (state) KeyPressedStatus[strKey]++;
								else KeyPressedStatus[strKey]--;

								bool newStateKey = (KeyPressedStatus[strKey] == 0) ? false : true;

								if (oldStateKey != newStateKey)
								{
									if (newStateKey) keyboardController.Down(keyToAssign[strKey].Item1);
									else keyboardController.Up(keyToAssign[strKey].Item1);

									Utils.LogMessage(strKey + " -> " + (newStateKey ? "press" : "rlz"));
								}
							}
							//Utils.LogMessage("val=" + value.ToString() + " for " + ltomonitor.Value);
						}
					}

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
					}
					joystick = new Joystick(new DirectInput(), device_guid);
					joystick.Properties.BufferSize = 512;
					joystick.Acquire();
				}
			}

			joystick.Unacquire();

		}
	}
}
