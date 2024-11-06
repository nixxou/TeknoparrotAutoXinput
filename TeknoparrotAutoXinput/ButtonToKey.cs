using Henooh.DeviceEmulator.Net;
using Henooh.DeviceEmulator.Net.Native;
using SDL2;
using SharpDX.DirectInput;
using SharpDX.Multimedia;

namespace TeknoparrotAutoXinput
{
	public static class ButtonToKeyManager
	{
		public static ButtonToKey buttonToKey = new ButtonToKey();
	}

	public class ButtonToKey
	{

		public Dictionary<string, (VirtualKeyCode, Key)> keyToAssign = new Dictionary<string, (VirtualKeyCode, Key)> {
			{"T", (VirtualKeyCode.VK_T,Key.T)},
			{"Y", (VirtualKeyCode.VK_Y,Key.Y)},
			{"U", (VirtualKeyCode.VK_U,Key.U)},
			{"I", (VirtualKeyCode.VK_I,Key.I)},
			{"O", (VirtualKeyCode.VK_O,Key.O)},
			{"G", (VirtualKeyCode.VK_G,Key.G)},
			{"H", (VirtualKeyCode.VK_H,Key.H)},
			{"J", (VirtualKeyCode.VK_J,Key.J)},
			{"K", (VirtualKeyCode.VK_K,Key.K)},
			{"V", (VirtualKeyCode.VK_V,Key.V)},
			{"B", (VirtualKeyCode.VK_B,Key.B)},
			{"N", (VirtualKeyCode.VK_N,Key.N)},
			{"P", (VirtualKeyCode.VK_P,Key.P)}
		};
		public Dictionary<string, List<(string, string)>> assignedKeys = new Dictionary<string, List<(string, string)>>();

		private int keyNbAssigned = 0;
		private List<Thread> threadJoystick = new List<Thread>();

		private static bool _stopListening;
		private List<DeviceInstance> devices = new List<DeviceInstance>();
		private readonly DirectInput _directInput = new DirectInput();

		private List<string> guids = new List<string>();
		private Dictionary<string, int> KeyPressedStatus = new Dictionary<string, int>();
		public Dictionary<int, (JoystickButtonData, JoystickButtonData)> originalJoystickPerGun = new Dictionary<int, (JoystickButtonData, JoystickButtonData)>();

		private (string, int) joystick1X = ("", -100);
		private (string, int) joystick1Y = ("", -100);
		private (string, int) joystick2X = ("", -100);
		private (string, int) joystick2Y = ("", -100);

		public string GunA_coinKey = "";
		public string GunA_startKey = "";
		public string GunA_reloadKey = "";
		public string GunB_coinKey = "";
		public string GunB_startKey = "";
		public string GunB_reloadKey = "";

		public bool EnableGunAOffscreenReload = false;
		public bool EnableGunBOffscreenReload = false;




		private int _GunA_X = 15000;
		public int GunA_X
		{
			get { return (_GunA_X); }
			set
			{
				if (value != _GunA_X)
				{
					_GunA_X = value;
					//if ((_GunA_X < 1000 || GunA_X > 64535) || (_GunA_Y < 1000 || GunA_Y > 64535)) joystick1_offscreen = true;
					if ((_GunA_X < 100 || GunA_X > 65435) || (_GunA_Y < 100 || GunA_Y > 65435)) joystick1_offscreen = true;
					else joystick1_offscreen = false;
				}
			}
		}

		private int _GunA_Y = 15000;
		public int GunA_Y
		{
			get { return (_GunA_Y); }
			set
			{
				if (value != _GunA_Y)
				{
					_GunA_Y = value;
					//if ((_GunA_X < 1000 || GunA_X > 64535) || (_GunA_Y < 1000 || GunA_Y > 64535)) joystick1_offscreen = true;
					if ((_GunA_X < 100 || GunA_X > 65435) || (_GunA_Y < 100 || GunA_Y > 65435)) joystick1_offscreen = true;
					else joystick1_offscreen = false;
				}
			}
		}

		private int _GunB_X = 15000;
		public int GunB_X
		{
			get { return (_GunB_X); }
			set
			{
				if (value != _GunB_X)
				{
					_GunB_X = value;
					//if ((_GunB_X < 1000 || GunB_X > 64535) || (_GunB_Y < 1000 || GunB_Y > 64535)) joystick2_offscreen = true;
					if ((_GunB_X < 100 || GunB_X > 65435) || (_GunB_Y < 100 || GunB_Y > 65435)) joystick2_offscreen = true;
					else joystick2_offscreen = false;
				}
			}
		}

		private int _GunB_Y = 15000;
		public int GunB_Y
		{
			get { return (_GunB_Y); }
			set
			{
				if (value != _GunB_Y)
				{
					_GunB_Y = value;
					//if ((_GunB_X < 1000 || GunB_X > 64535) || (_GunB_Y < 1000 || GunB_Y > 64535)) joystick2_offscreen = true;
					if ((_GunB_X < 100 || GunB_X > 65435) || (_GunB_Y < 100 || GunB_Y > 65435)) joystick2_offscreen = true;
					else joystick2_offscreen = false;
				}
			}
		}



		private bool _joystick1_offscreen = false;
		public bool joystick1_offscreen
		{
			get { return (_joystick1_offscreen); }
			set
			{
				if (value != _joystick1_offscreen)
				{
					_joystick1_offscreen = value;
					if (_joystick1_offscreen && EnableGunAOffscreenReload)
					{
						if (GunA_reloadKey != "" && keyToAssign.ContainsKey(GunA_reloadKey))
						{
							keyboardController.Press(keyToAssign[GunA_reloadKey].Item1, new TimeSpan(0, 0, 0, 0, 50));
						}

					}
				}
			}
		}


		private bool _joystick2_offscreen = false;
		public bool joystick2_offscreen
		{
			get { return (_joystick2_offscreen); }
			set
			{
				if (value != _joystick2_offscreen)
				{
					_joystick2_offscreen = value;
					if (_joystick2_offscreen && EnableGunBOffscreenReload)
					{
						if (GunB_reloadKey != "" && keyToAssign.ContainsKey(GunB_reloadKey))
							keyboardController.Press(keyToAssign[GunB_reloadKey].Item1, new TimeSpan(0, 0, 0, 0, 50));
					}
				}
			}
		}




		KeyboardController keyboardController;

		public string GetFreeKey()
		{
			foreach (var keyA in keyToAssign)
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

		public void Assign(string newKey, string OriginalGuid, string OriginalLabel, int coinOrStart = 0)
		{
			Console.WriteLine($"Assign({newKey}, {OriginalGuid}, {OriginalLabel}, {coinOrStart})");
			if (coinOrStart == 11) GunA_coinKey = newKey;
			if (coinOrStart == 12) GunA_startKey = newKey;
			if (coinOrStart == 13) GunA_reloadKey = newKey;

			if (coinOrStart == 21) GunB_coinKey = newKey;
			if (coinOrStart == 22) GunB_startKey = newKey;
			if (coinOrStart == 23) GunB_reloadKey = newKey;

			if (!assignedKeys.ContainsKey(newKey))
			{
				assignedKeys.Add(newKey, new List<(string, string)>());
			}
			bool found = false;
			foreach (var tuple in assignedKeys[newKey])
			{
				if (tuple.Item1 == OriginalGuid && tuple.Item2 == OriginalLabel)
				{
					found = true;
				}
			}
			if (!found) assignedKeys[newKey].Add((OriginalGuid, OriginalLabel));
			if (!guids.Contains(OriginalGuid))
			{
				guids.Add(OriginalGuid);
			}
		}

		public string ContainAssigned(string guid, string label)
		{
			foreach (var k in assignedKeys)
			{
				foreach (var tuple in k.Value)
				{
					if (tuple.Item1 == guid && tuple.Item2 == label)
					{
						return k.Key;
					}
				}
			}
			return "";
		}

		public void StartMonitor()
		{
			if (assignedKeys.Count == 0) return;

			//Add joystick GUID
			foreach (var originalJoystick in originalJoystickPerGun)
			{
				if (originalJoystick.Value.Item1 != null && originalJoystick.Value.Item2 != null)
				{
					if (originalJoystick.Value.Item1.JoystickGuid != Guid.Empty && originalJoystick.Value.Item2.JoystickGuid != Guid.Empty)
					{
						if (originalJoystick.Key == 1)
						{
							joystick1X = (originalJoystick.Value.Item1.JoystickGuid.ToString(), originalJoystick.Value.Item1.Button);
							joystick1Y = (originalJoystick.Value.Item2.JoystickGuid.ToString(), originalJoystick.Value.Item2.Button);
						}
						if (originalJoystick.Key == 2)
						{
							joystick2X = (originalJoystick.Value.Item1.JoystickGuid.ToString(), originalJoystick.Value.Item1.Button);
							joystick2Y = (originalJoystick.Value.Item2.JoystickGuid.ToString(), originalJoystick.Value.Item2.Button);
						}

						if (!guids.Contains(originalJoystick.Value.Item1.JoystickGuid.ToString()))
						{
							guids.Add(originalJoystick.Value.Item1.JoystickGuid.ToString());
						}
						if (!guids.Contains(originalJoystick.Value.Item2.JoystickGuid.ToString()))
						{
							guids.Add(originalJoystick.Value.Item2.JoystickGuid.ToString());
						}
					}
				}
			}

			foreach (var assignedKey in assignedKeys)
			{
				KeyPressedStatus.Add(assignedKey.Key, 0);
			}

			foreach (var guid in guids)
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
				foreach (var t in threadJoystick)
				{
					if (t.IsAlive)
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
			foreach (var ak in assignedKeys)
			{
				foreach (var tuple in ak.Value)
				{
					if (tuple.Item1 == joyGuid)
					{
						LabelToMonitor.Add(tuple.Item2, ak.Key);
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
					Dictionary<string, int> keyPressedValue = new Dictionary<string, int>();
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

							if (joyGuid == joystick1X.Item1 && (int)key.Offset == joystick1X.Item2)
							{
								//Utils.LogMessage("JOY1 X=" + key.Value);
								GunA_X = key.Value;
								//if (joystick1_offscreen) Utils.LogMessage("JOY1 OFFSCREEN");
							}
							if (joyGuid == joystick1Y.Item1 && (int)key.Offset == joystick1Y.Item2)
							{
								GunA_Y = key.Value;
								//Utils.LogMessage("JOY1 Y=" + key.Value);
								//if (joystick1_offscreen) Utils.LogMessage("JOY1 OFFSCREEN");
							}
							if (joyGuid == joystick2X.Item1 && (int)key.Offset == joystick2X.Item2)
							{
								GunB_X = key.Value;
								//Utils.LogMessage("JOY2 X=" + key.Value);
								//if (joystick2_offscreen) Utils.LogMessage("JOY2 OFFSCREEN");
							}
							if (joyGuid == joystick2Y.Item1 && (int)key.Offset == joystick2Y.Item2)
							{
								GunB_Y = key.Value;
								//Utils.LogMessage("JOY2 Y=" + key.Value);
								//if (joystick2_offscreen) Utils.LogMessage("JOY2 OFFSCREEN");
							}

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
							else
							{
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
							if (key.Value == 0)
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
							if (inputText.StartsWith("Button")) Utils.LogMessage(inputText + " : " + key.Value + " : " + pressed);

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
					foreach (var keyPressed in keyPressedValue)
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

					foreach (var ltomonitor in LabelToMonitor)
					{
						string strKey = ltomonitor.Value;
						string label = ltomonitor.Key;

						if (keyPressedValue.ContainsKey(label))
						{
							bool state = (keyPressedValue[label] == 0) ? false : true;
							if (LabelState[label] != state)
							{
								LabelState[label] = state;
								//Utils.LogMessage(label + " = " + (state ? "ON" : "OFF"));

								bool oldStateKey = (KeyPressedStatus[strKey] == 0) ? false : true;

								if (state) KeyPressedStatus[strKey]++;
								else KeyPressedStatus[strKey]--;

								bool newStateKey = (KeyPressedStatus[strKey] == 0) ? false : true;

								if (oldStateKey != newStateKey)
								{
									if (newStateKey)
									{
										string targetKey = strKey;
										if (strKey == GunA_startKey && joystick1_offscreen)
										{
											targetKey = GunA_coinKey;
										}
										if (strKey == GunB_startKey && joystick2_offscreen)
										{
											targetKey = GunB_coinKey;
										}
										keyboardController.Down(keyToAssign[targetKey].Item1);
									}
									else
									{
										keyboardController.Up(keyToAssign[strKey].Item1);
										if (strKey == GunA_startKey)
										{
											keyboardController.Up(keyToAssign[GunA_coinKey].Item1);
										}
										if (strKey == GunB_startKey)
										{
											keyboardController.Up(keyToAssign[GunB_coinKey].Item1);
										}

									}

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

		public static string DSharpGuidToSDLGuid(string joyGuid)
		{
			int p_id = -1;
			int v_id = -1;
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
					try
					{
						found_device = true;
						deviceInstance = device;
						device_guid = device.InstanceGuid;
						joystick = new Joystick(directInput, device_guid);
						v_id = joystick.Properties.VendorId;
						p_id = joystick.Properties.ProductId;
					}
					catch { }
					//break;
				}
			}
			if (!found_device)
			{
				return "";
			}

			List<string> suspectDevice = new List<string>();
			List<int> listsuspectIndex = new List<int>();

			SDL2.SDL.SDL_Quit();
			SDL2.SDL.SDL_SetHint(SDL2.SDL.SDL_HINT_JOYSTICK_RAWINPUT, "0");
			SDL2.SDL.SDL_Init(SDL2.SDL.SDL_INIT_JOYSTICK | SDL2.SDL.SDL_INIT_GAMECONTROLLER);
			SDL2.SDL.SDL_JoystickUpdate();
			for (int i = 0; i < SDL2.SDL.SDL_NumJoysticks(); i++)
			{
				if (SDL.SDL_IsGameController(i) == SDL.SDL_bool.SDL_FALSE) continue;
				{
					var currentJoy = SDL.SDL_JoystickOpen(i);
					string nameController = SDL2.SDL.SDL_JoystickNameForIndex(i).Trim('\0');
					ushort vendorId = SDL2.SDL.SDL_JoystickGetVendor(currentJoy);
					ushort productId = SDL2.SDL.SDL_JoystickGetProduct(currentJoy);
					bool isSuspect = false;
					if (vendorId == v_id && productId == p_id)
					{
						const int bufferSize = 256; // La taille doit être au moins 33 pour stocker le GUID sous forme de chaîne (32 caractères + le caractère nul)
						byte[] guidBuffer = new byte[bufferSize];
						SDL.SDL_JoystickGetGUIDString(SDL.SDL_JoystickGetGUID(currentJoy), guidBuffer, bufferSize);
						string guidString = System.Text.Encoding.UTF8.GetString(guidBuffer).Trim('\0');
						suspectDevice.Add(guidString);
						listsuspectIndex.Add(i);
						isSuspect = true;
					}


					SDL.SDL_JoystickClose(currentJoy);
				}
			}
			//SDL2.SDL.SDL_Quit();
			if (suspectDevice.Count == 0) return "";
			if (suspectDevice.Count == 1) return suspectDevice[0];

			if (suspectDevice.Count > 1)
			{
				bool found = false;
				JoystickState stato = new JoystickState();
				joystick = new Joystick(new DirectInput(), new Guid(joyGuid));
				joystick.Properties.BufferSize = 512;
				joystick.Acquire();

				List<IntPtr> gameControllers = new List<IntPtr>();
				foreach (var sdlIndex in listsuspectIndex)
				{
					gameControllers.Add(SDL.SDL_GameControllerOpen(sdlIndex));
				}

				while (!found)
				{
					joystick.Poll();
					joystick.GetCurrentState(ref stato);
					int limit = 0;
					if (Math.Abs(stato.X - 32767) < 1000)
					{
						limit++;
						Thread.Sleep(100);
						if (limit > 100) return "";
						continue;
					}


					Utils.LogMessage($"DSharp GUID = {joyGuid}, X={stato.X}");
					List<int> nearTarget = new List<int>();
					int i = 0;
					foreach (var g in gameControllers)
					{
						SDL.SDL_GameControllerUpdate();
						int valSDL = SDL.SDL_GameControllerGetAxis(g, 0) + 32767;

						Utils.LogMessage($"SDL {i} = {valSDL}");

						if (Math.Abs(valSDL - stato.X) < 1000) nearTarget.Add(i);
						//MessageBox.Show(i.ToString() + "=" + valSDL.ToString());
						i++;
					}

					if (nearTarget.Count == 1)
					{
						joystick.Unacquire();
						foreach (var g in gameControllers)
						{
							SDL.SDL_GameControllerClose(g);
						}
						Utils.LogMessage($"RES = {suspectDevice[nearTarget[0]]}");
						return suspectDevice[nearTarget[0]];
					}



					Thread.Sleep(100);
				}

			}

			return "";
		}


	}
}
