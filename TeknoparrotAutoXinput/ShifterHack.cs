using AryMem;
using Henooh.DeviceEmulator.Net;
using Krypton.Toolkit;
using SharpDX.DirectInput;
using SharpDX.Multimedia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace TeknoparrotAutoXinput
{
	
	public class ShifterHack
	{
		static public Dictionary<string, bool> supportedGames = new Dictionary<string, bool>()
		{
			{ "or2spdlx", true },
			{ "D1GP", false },
			{ "FR", false },
			{ "GtiClub3", false },
			{ "R-Tuned", false }, // Pas sûr, peut-être true
			{ "SR3", false },
			{ "SWDC", false },
			{ "segartv", false },
			{ "FNFSC", false }
			
		};

		private static bool _stopListening;
		private List<DeviceInstance> devices = new List<DeviceInstance>();
		private readonly DirectInput _directInput = new DirectInput();
		private Thread threadShifter;
		private Thread threadWheel;
		private Thread threadGame;

		private string _game = "";
		private string _shifterGuid = "";
		private string _wheelGuid = "";
		private string _keyShiftUp = "";
		private string _keyShiftDown = "";
		private string _keyGear1 = "";
		private string _executableGame = "";
		private string _execWithoutExt = "";

		private int _previous_gear = -1;
		private int _current_gear = -1;
		private int _target_gear = -1;

		private Ary _ary = null;


		Dictionary<string, string> _keysShifter = new Dictionary<string, string>();

		KeyboardController keyboardController;

		const int PROCESS_WM_READ = 0x0010;

		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll")]
		public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

		

		bool shiftUp = false;
		bool shiftDown = false;
		bool gear1 = false;
		bool gear2 = false;
		bool gear3 = false;
		bool gear4 = false;
		bool gear5 = false;
		bool gear6 = false;
		bool gearR = false;

		public ShifterHack()
		{



			_stopListening = false;

			devices = new List<DeviceInstance>();
			devices.AddRange(_directInput.GetDevices().Where(x => x.Type != DeviceType.Mouse && x.UsagePage != UsagePage.VendorDefinedBegin && x.Usage != UsageId.AlphanumericBitmapSizeX && x.Usage != UsageId.AlphanumericAlphanumericDisplay && x.UsagePage != unchecked((UsagePage)0xffffff43) && x.UsagePage != UsagePage.Vr).ToList());

		}

		public static string getShiftUp(string game)
		{
			if (game == "or2spdlx") return "Gear Shift Up";
			if (game == "D1GP") return "Shift Up / Test Menu Up";
			if (game == "FR") return "Shift up";
			if (game == "GtiClub3") return "Shift Up";
			if (game == "R-Tuned") return "Shift Up";
			if (game == "SR3") return "Shift up";
			if (game == "SWDC") return "Shift Up";
			if (game == "segartv") return "Gear Shift Up";
			if (game == "FNFSC") return "Shift Up";
			return "";
		}

		public static string getShifDown(string game)
		{
			if (game == "or2spdlx") return "Gear Shift Down";
			if (game == "D1GP") return "Shift Down / Test Menu Down";
			if (game == "FR") return "Shift Down";
			if (game == "GtiClub3") return "Shift Down";
			if (game == "R-Tuned") return "Shift Down";
			if (game == "SR3") return "Shift Down";
			if (game == "SWDC") return "Shift Down";
			if (game == "segartv") return "Gear Shift Down";
			if (game == "FNFSC") return "Shift Down";
			return "";
		}

		public int GetCurrentGear()
		{
			if (_ary == null) return -1;

			try
			{
				if (_game == "or2spdlx")
				{
					int current_gear = _ary.ReadMemory<int>(0x0827A160);
					if (current_gear == 0 || current_gear > 6) current_gear = -1;
					return current_gear;
				}
				if (_game == "D1GP")
				{
					int current_gear = _ary.ReadMemory<int>(0x006283A0);
					if (current_gear == 0 || current_gear > 6) current_gear = -1;
					return current_gear;
				}
				if (_game == "FR")
				{
					int current_gear = _ary.ReadMemory<int>(0x006B184C);
					if (current_gear == -1) current_gear = 0;
					if (current_gear < 0 || current_gear > 6) current_gear = -1;
					return current_gear;
				}
				if (_game == "R-Tuned")
				{
					int current_gear = _ary.ReadMemory<int>(0x085027A0);
					if (current_gear < 0 || current_gear > 6) current_gear = -1;
					return current_gear;
				}
				if (_game == "SR3")
				{
					ulong baseaddress = _ary.GetModule(Path.GetFileName(_executableGame)).BaseAddress;
					//Utils.LogMessage($"base = {baseaddress}");
					
					//Utils.LogMessage($"base = {baseaddress:x8}");

					int p1 = _ary.ReadMemory<int>(baseaddress + 0x006E03A4);
					//Utils.LogMessage($"p1 = {p1:x8}");
					int p2 = _ary.ReadMemory<int>((ulong)p1 + 0x4);
					//Utils.LogMessage($"p2 = {p2:x8}");
					int p3 = _ary.ReadMemory<int>((ulong)p2 + 0x4);
					//Utils.LogMessage($"p3 = {p3:x8}");
					int p4 = _ary.ReadMemory<int>((ulong)p3 + 0x10);
					//Utils.LogMessage($"p4 = {p4:x8}");
					int p5 = _ary.ReadMemory<int>((ulong)p4 + 0x30);
					//Utils.LogMessage($"p5 = {p5:x8}");
					int current_gear = _ary.ReadMemory<int>((ulong)p5 + 0x814);

					if (current_gear < 0 || current_gear > 7) return -1;
					if (current_gear == 1) return -1;
					if (current_gear > 1 && current_gear <= 7) current_gear--;
					
					return current_gear;
				}
				if (_game == "SWDC")
				{
					ulong baseaddress = _ary.GetModule(Path.GetFileName(_executableGame)).BaseAddress;
					//Utils.LogMessage($"base = {baseaddress}");

					//Utils.LogMessage($"base = {baseaddress:x16}");

					long p1 = _ary.ReadMemory<long>(baseaddress + 0x09E2BEF8);
					//Utils.LogMessage($"p1 = {p1:x16}");
					long p2 = _ary.ReadMemory<long>((ulong)p1 + 0x30);
					//Utils.LogMessage($"p2 = {p2:x16}");
					long p3 = _ary.ReadMemory<long>((ulong)p2 + 0xA0);
					//Utils.LogMessage($"p3 = {p3:x16}");
					long p4 = _ary.ReadMemory<long>((ulong)p3 + 0x220);
					//Utils.LogMessage($"p4 = {p4:x16}");
					long p5 = _ary.ReadMemory<long>((ulong)p4 + 0x388);
					//Utils.LogMessage($"p5 = {p5:x16}");
					long p6 = _ary.ReadMemory<long>((ulong)p5 + 0xB0);
					//Utils.LogMessage($"p6 = {p5:x16}");
					long p7 = _ary.ReadMemory<long>((ulong)p6 + 0x1D0);
					//Utils.LogMessage($"p7 = {p5:x16}");
					long current_gear = _ary.ReadMemory<long>((ulong)p7 + 0x10);
					//Utils.LogMessage($"p8 = {current_gear:x16}");

					
					if (current_gear <= 0 || current_gear > 6) return -1;
					return (int)current_gear;
				}
				if (_game == "segartv")
				{
					ulong baseaddress = _ary.GetModule(Path.GetFileName(_executableGame)).BaseAddress;
					//Utils.LogMessage($"base = {baseaddress}");

					//Utils.LogMessage($"base = {baseaddress:x16}");

					int p1 = _ary.ReadMemory<int>(baseaddress + 0x00016080);
					//Utils.LogMessage($"p1b = {p1:x8}");
					int p2 = _ary.ReadMemory<int>((ulong)p1 + 0x50);
					//Utils.LogMessage($"p2b = {p2:x8}");
					int p3 = _ary.ReadMemory<int>((ulong)p2 + 0x8);
					//Utils.LogMessage($"p3b = {p3:x8}");
					int p4 = _ary.ReadMemory<int>((ulong)p3 + 0xC);
					//Utils.LogMessage($"p4b = {p4:x8}");
					int p5 = _ary.ReadMemory<int>((ulong)p4 + 0x8);
					//Utils.LogMessage($"p5b = {p5:x8}");
					int p6 = _ary.ReadMemory<int>((ulong)p5 + 0x24);
					//Utils.LogMessage($"p6b = {p5:x8}");
					int p7 = _ary.ReadMemory<int>((ulong)p6 + 0x54);
					//Utils.LogMessage($"p7b = {p5:x8}");
					int current_gear = _ary.ReadMemory<int>((ulong)p7 + 0x28);
					//Utils.LogMessage($"p8b = {current_gear:x8}");


					if (current_gear < 0 || current_gear > 5) return -1;
					current_gear++;
					return (int)current_gear;
				}
				if (_game == "FNFSC")
				{
					//Utils.LogMessage($"base = {baseaddress:x16}");

					int p1 = _ary.ReadMemory<int>(0x702804);
					Utils.LogMessage($"p1b = {p1:x8}");
					int current_gear = _ary.ReadMemory<int>((ulong)p1 + 0x7F0);
					//Utils.LogMessage($"p8b = {current_gear:x8}");


					if (current_gear < 0 || current_gear > 6) return -1;
					current_gear++;
					return (int)current_gear;
				}
				if (_game == "GtiClub3")
				{
					//Utils.LogMessage($"base = {baseaddress:x16}");

					int p1 = _ary.ReadMemory<int>(0x00A7FCC8);
					Utils.LogMessage($"p1b = {p1:x8}");
					int current_gear = _ary.ReadMemory<int>((ulong)p1 + 0x644);
					//Utils.LogMessage($"p8b = {current_gear:x8}");


					if (current_gear <= 0 || current_gear > 6) return -1;
					return (int)current_gear;
				}



			}
			catch { }


			return -1;

		}

		public void SetGear(int targetGear,bool doublecheck=false)
		{
			if (_ary == null) return;
			bool validTarget = false;

			try
			{
				if (targetGear >= 0 && targetGear <= GetMaxGear()) validTarget = true;
				if (!validTarget) return;
				int diff = _target_gear - _current_gear;
				Utils.LogMessage($"targetGear  {targetGear}, current= {_current_gear}, diff= {diff} ");
				if (diff > 0)
				{
					for (int i = 0; i < diff; i++)
					{
						keyboardController.Down(Henooh.DeviceEmulator.Net.Native.VirtualKeyCode.PRIOR);
						Thread.Sleep(100);
						keyboardController.Up(Henooh.DeviceEmulator.Net.Native.VirtualKeyCode.PRIOR);
						Thread.Sleep(100);
					}
				}
				if (diff < 0)
				{
					for (int i = 0; i < (diff * -1); i++)
					{
						keyboardController.Down(Henooh.DeviceEmulator.Net.Native.VirtualKeyCode.NEXT);
						Thread.Sleep(100);
						keyboardController.Up(Henooh.DeviceEmulator.Net.Native.VirtualKeyCode.NEXT);
						Thread.Sleep(100);
					}
				}

			}
			catch { }
		}

		public int GetMaxGear()
		{
			return 6;
		}

		public void Start(string game, string shifterGuid, string wheelGuid, string keyShiftUp, string keyShiftDown, string executableGame, Dictionary<string, JoystickButtonData> bindingDinputShifter)
		{
			_game = game;
			if (!supportedGames.ContainsKey(game)) return;

			keyboardController = new Henooh.DeviceEmulator.Net.KeyboardController();
			_shifterGuid = shifterGuid;
			_wheelGuid = wheelGuid;
			_keyShiftUp = keyShiftUp;
			_keyShiftDown = keyShiftDown;
			_executableGame = executableGame;
			_execWithoutExt = Path.GetFileNameWithoutExtension(executableGame);

			foreach (var bind in bindingDinputShifter)
			{
				if(bind.Key == "InputDeviceGear1" || bind.Key == "InputDeviceGear2" || bind.Key == "InputDeviceGear3" || bind.Key == "InputDeviceGear4" || bind.Key == "InputDeviceGear5" || bind.Key == "InputDeviceGear6" || bind.Key == "InputDeviceGearR")
				{
					if(!_keysShifter.ContainsKey(bind.Key)) _keysShifter.Add(bind.Value.Title, bind.Key);
				}
			}

			bool found_shifter = false;
			bool found_wheel = false;
			int i = 0;
			int index_shifter = 0;
			int index_wheel = 0;
			foreach (var device in devices)
			{
				if(device.InstanceGuid.ToString() == shifterGuid)
				{
					found_shifter = true;
					index_shifter = i;
				}
				if(device.InstanceGuid.ToString() == wheelGuid)
				{
					found_wheel = true;
					index_wheel = i;
				}
				i++;
			}
			

			if (found_shifter && found_wheel)
			{
				threadGame = new Thread(() => MonitorGame(executableGame));
				threadGame.Start();
			}
		}

		public void Stop()
		{
			_stopListening = true;

			if (threadShifter != null)
			{
				threadShifter.Join();
				threadShifter = null;
			}
			if (threadWheel != null)
			{
				threadWheel.Join();
				threadWheel = null;
			}
			if (threadGame != null)
			{
				threadGame.Join();
				threadGame = null;
			}
			_stopListening = false;
			Utils.LogMessage("Fin ShifterHack threads");
		}

		public void MonitorGame(string executablePath)
		{
			Utils.LogMessage("Start MonitorGame");
			string execWithoutExt = Path.GetFileNameWithoutExtension(executablePath);
			Process[] processes = Process.GetProcessesByName(execWithoutExt);
			bool is_running = (processes.Length > 0);

			while (!is_running && !_stopListening)
			{
				Thread.Sleep(100);
				processes = Process.GetProcessesByName(execWithoutExt);
				is_running = (processes.Length > 0);
			}

			if (_stopListening) return;
			Utils.LogMessage("Game Start");
			Process processGame = processes.First();

			
			//Wait 20 secs
			for (int i = 0; i < 200; i++)
			{
				Thread.Sleep(100);
				if (_stopListening) return;
			}
			
			bool found_shifter = false;
			bool found_wheel = false;
			int z = 0;
			int index_shifter = 0;
			int index_wheel = 0;
			Guid shifterTrueGuid = Guid.Empty;
			Guid wheelTrueGuid = Guid.Empty;
			foreach (var device in devices)
			{
				if (device.InstanceGuid.ToString() == _shifterGuid)
				{
					found_shifter = true;
					shifterTrueGuid = device.InstanceGuid;
				}
				if (device.InstanceGuid.ToString() == _wheelGuid)
				{
					found_wheel = true;
					wheelTrueGuid = device.InstanceGuid;
				}
			}
			if (found_shifter && found_wheel)
			{

				threadShifter = new Thread(() => SpawnDirectInputListener(shifterTrueGuid.ToString()));
				threadShifter.Start();

				Thread.Sleep(5000);
				if (wheelTrueGuid.ToString() != shifterTrueGuid.ToString())
				{
					threadWheel = new Thread(() => SpawnDirectInputListener(wheelTrueGuid.ToString()));
					threadWheel.Start();
				}
			}

			while (!processGame.HasExited && !_stopListening)
			{
				if(_target_gear != -1 && _ary == null)
				{
					_ary = new Ary(_execWithoutExt);
				}

				if(_ary != null)
				{
					int current_gear = GetCurrentGear();
					//Utils.LogMessage($"current_gear = {current_gear}, _target_gear = {_target_gear}");
					if (current_gear >= 0)
					{
						//_previous_gear = _current_gear;
						_current_gear = current_gear;
					}

					if (_current_gear != _target_gear && _target_gear >= 0)
					{
						SetGear(_target_gear,true);
					}

				}

				Thread.Sleep(100);
			}
			

			Utils.LogMessage("Game Stop");
		}


		private void SpawnDirectInputListener(string joyGuid)
		{
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

			while (!_stopListening)
			{
				try
				{
					int newGear = -1;
					bool updateGear = false;
					joystick.Poll();
					var datas = joystick.GetBufferedData();
					foreach (var key in datas)
					{

						string inputText = "";
						if (deviceInstance.Type == DeviceType.Keyboard)
							inputText = "Button " + ((Key)key.Offset - 47).ToString();
						else
							inputText = key.Offset.ToString();

						if (!inputText.StartsWith("Button")) continue;

						inputText = deviceInstance.Type + " " + inputText;

						bool pressed = (key.Value == 128);

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

						if (_keysShifter.ContainsKey(inputText) && deviceInstance.InstanceGuid.ToString() == _shifterGuid)
						{
							Utils.LogMessage($"BTN  {inputText}");
							string keyValue = _keysShifter[inputText];
							if (keyValue == "InputDeviceGear1")
							{
								if (gear1 != pressed)
								{
									gear1 = pressed;
									updateGear = true;
								}
								if (pressed) newGear = 1;
							}
							if (keyValue == "InputDeviceGear2")
							{
								if (gear2 != pressed)
								{
									gear2 = pressed;
									updateGear = true;
								}
								if (pressed) newGear = 2;
							}
							if (keyValue == "InputDeviceGear3")
							{
								if (gear3 != pressed)
								{
									gear3 = pressed;
									updateGear = true;
								}
								if (pressed) newGear = 3;
							}
							if (keyValue == "InputDeviceGear4")
							{
								if (gear4 != pressed)
								{
									gear4 = pressed;
									updateGear = true;
								}
								if (pressed) newGear = 4;
							}
							if (keyValue == "InputDeviceGear5")
							{
								if (gear5 != pressed)
								{
									gear5 = pressed;
									updateGear = true;
								}
								if (pressed) newGear = 5;
							}
							if (keyValue == "InputDeviceGear6")
							{
								if (gear6 != pressed)
								{
									gear6 = pressed;
									updateGear = true;
								}
								if (pressed) newGear = 6;
							}
							if (keyValue == "InputDeviceGearR")
							{
								if (gearR != pressed)
								{
									gearR = pressed;
									updateGear = true;
								}
								if (pressed) newGear = 0;
							}
						}


					}
					if (updateGear)
					{
						_target_gear = newGear;
						//int currentGear = GetCurrentGear("xxx");
						Utils.LogMessage($"New gear = {newGear}");
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
