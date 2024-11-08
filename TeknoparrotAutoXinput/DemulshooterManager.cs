﻿using SDL2;
using SerialPortLib2;
using SharpDX.DirectInput;
using SharpDX.Multimedia;
using System.Diagnostics;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Text;
using Effect = SharpDX.DirectInput.Effect;

namespace TeknoparrotAutoXinput
{
	public static class DemulshooterManager
	{
		public static string DemulshooterPath = "";
		public static string Demulshooter32 = "";
		public static string Demulshooter64 = "";
		public static int ParentProcess = -1;
		public static bool ValidPath = false;
		public static bool Is64bits = false;
		public static string TargetProcess = "";
		//public static string ForceMD5 = "";
		public static string Target = "";
		public static string Rom = "";
		public static bool UseMamehooker = false;
		public static bool UseTcp = false;
		public static Thread MonitorDemulshooter;
		private static bool _stopListening = false;
		public static bool HideCrosshair = false;


		public static bool gunARecoil = false;
		public static bool gunBRecoil = false;
		public static bool gunASinden = false;
		public static bool gunBSinden = false;
		public static bool gunAGun4ir = false;
		public static bool gunBGun4ir = false;
		public static bool gunARumble = false;
		public static bool gunBRumble = false;
		public static bool gunALichtknarre = false;
		public static bool gunBLichtknarre = false;

		public static string gunAParameter = "";
		public static string gunBParameter = "";


		public static bool GunA4tiers = false;
		public static bool GunADamageRumble = false;
		public static bool GunAAutoJoy = false;
		public static bool GunB4tiers = false;
		public static bool GunBDamageRumble = false;
		public static bool GunBAutoJoy = false;


		static object lockGunA = new object();
		static object lockGunB = new object();
		static bool isStartedGunA = false;
		static bool isStartedGunB = false;


		//static SerialPort gunASerial;
		static SerialPortInput gunASerial = null;
		static NamedPipeClientStream gunAPipe;
		static StreamWriter gunAStream;
		static bool gunAjoyAttached = false;
		static string GunASDLGuid = string.Empty;
		static int GunASDLIndex = -1;

		//static SerialPort gunBSerial;
		static SerialPortInput gunBSerial = null;
		static NamedPipeClientStream gunBPipe;
		static StreamWriter gunBStream;
		static bool gunBjoyAttached = false;
		static string GunBSDLGuid = string.Empty;
		static int GunBSDLIndex = -1;

		public static string selfRecoilMode = "";
		public static Thread threadSelfManagedGunA;
		public static Thread threadSelfManagedGunB;
		public static bool triggerGunA = false;
		public static bool triggerGunB = false;
		public static long lastRecoilGunA = 0;
		public static long lastRecoilGunB = 0;

		public static Form dummyForm = null;
		public static IntPtr dummyFormHandle = IntPtr.Zero;
		static int[] _actuatorObjectIds_gunA;
		static int[] dirVector_gunA;
		static Effect effect_gunA = null;
		static Joystick gunA_lichtknarrejoy = null;
		static int[] _actuatorObjectIds_gunB;
		static int[] dirVector_gunB;
		static Effect effect_gunB = null;
		static Joystick gunB_lichtknarrejoy = null;

		public static bool SetPath(string demulshooterExe)
		{
			DemulshooterPath = "";
			Demulshooter32 = "";
			Demulshooter64 = "";
			ValidPath = false;
			if (File.Exists(demulshooterExe))
			{

				string dirDemul = Path.GetFullPath(Path.GetDirectoryName(demulshooterExe));
				string demulshooter32 = Path.Combine(dirDemul, "DemulShooter.exe");
				string demulshooter64 = Path.Combine(dirDemul, "DemulShooterX64.exe");
				if (File.Exists(demulshooter32) && File.Exists(demulshooter64))
				{
					DemulshooterPath = dirDemul;
					Demulshooter32 = demulshooter32;
					Demulshooter64 = demulshooter64;
					ValidPath = true;
					return true;
				}
			}
			return false;
		}

		public static void WriteConfig(string targetExecutable)
		{
			if (!ValidPath) return;
			var ini = new IniFile(Path.Combine(DemulshooterPath, "config.ini"));
			ini.Write("Launch_Target", Target);
			ini.Write("Launch_Rom", Rom);
			ini.Write("Launch_64bits", Is64bits ? "True" : "False");
			ini.Write("Launch_tprocess", targetExecutable);
			//ini.Write("Launch_md5", forcemd5);
			ini.Write("OutputEnabled", "True");
			ini.Write("ParentProcess", Process.GetCurrentProcess().Id.ToString());
			ini.Write("HideCrosshair", HideCrosshair ? "True" : "False");

			ini.Write("WM_OutputsEnabled", UseMamehooker ? "True" : "False");
			ini.Write("Net_OutputsEnabled", UseTcp ? "True" : "False");


		}

		public static void ReadConfig()
		{
			if (!ValidPath) return;
			if (File.Exists(Path.Combine(DemulshooterPath, "config.ini")))
			{
				var ini = new IniFile(Path.Combine(DemulshooterPath, "config.ini"));
				Rom = ini.Read("Launch_Rom");
				Target = ini.Read("Launch_Target");
				TargetProcess = ini.Read("Launch_tprocess");
				//ForceMD5 = ini.Read("Launch_md5");
				Is64bits = ini.Read("Launch_64bits") == "True" ? true : false;
				ParentProcess = int.Parse(ini.Read("ParentProcess"));
				HideCrosshair = ini.Read("HideCrosshair") == "True" ? true : false;
				UseMamehooker = ini.Read("WM_OutputsEnabled") == "True" ? true : false;
			}

		}

		public static void Start(string targetExecutable)
		{
			Utils.LogMessage("DemulshooterManager Start");
			if (!ValidPath) return;
			WriteConfig(targetExecutable);
			Utils.LogMessage("DemulshooterManager Ini written");
			string selfExe = Process.GetCurrentProcess().MainModule.FileName;
			if (!Utils.CheckTaskExist(selfExe, "--demulshooter"))
			{
				Utils.LogMessage("DemulshooterManager create task");
				string exePath = selfExe;
				string exeDir = Path.GetDirectoryName(exePath);
				Process process = new Process();
				process.StartInfo.FileName = selfExe;
				process.StartInfo.Arguments = "--registerTask " + $"\"{selfExe}\" " + "--demulshooter";
				process.StartInfo.WorkingDirectory = exeDir;
				process.StartInfo.UseShellExecute = true;
				process.StartInfo.Verb = "runas";
				process.Start();
				process.WaitForExit();
				Utils.LogMessage("DemulshooterManager task created");
			}
			Utils.ExecuteTask(Utils.ExeToTaskName(selfExe, "--demulshooter"), -1);
			MonitorDemulshooter = new Thread(() => ClientDemulshooter());
			MonitorDemulshooter.Start();
			Utils.LogMessage("DemulshooterManager task start");

		}

		public static void StartSelfManaged(string recoilMode)
		{
			selfRecoilMode = recoilMode;
			Thread.Sleep(2000);
			threadSelfManagedGunA = new Thread(() => SpawnDirectInputListener(1));
			threadSelfManagedGunA.Start();

			Thread.Sleep(2000);
			threadSelfManagedGunB = new Thread(() => SpawnDirectInputListener(2));
			threadSelfManagedGunB.Start();
			Thread.Sleep(100);

			bool autoMode = false;
			int delayAuto = 0;
			if (int.TryParse(selfRecoilMode, out delayAuto))
			{
				autoMode = true;
			}
			if (recoilMode == "auto")
			{
				delayAuto = 200;
				autoMode = true;
			}
			if (autoMode)
			{
				selfRecoilMode = "auto";
				MonitorDemulshooter = new Thread(() => SelfManagedAutoFire(delayAuto));
				MonitorDemulshooter.Start();

			}

		}

		public static void SelfManagedAutoFire(int delayAuto)
		{
			while (!_stopListening)
			{
				long timestamp = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
				if (triggerGunA)
				{
					long diff = timestamp - lastRecoilGunA;
					if (diff > delayAuto)
					{
						DoRecoil(1);
					}
				}

				if (triggerGunB)
				{
					long diff = timestamp - lastRecoilGunB;
					if (diff > delayAuto)
					{
						DoRecoil(2);
					}
				}

				Thread.Sleep(30);
			}
		}

		private static void SpawnDirectInputListener(int gunNumber)
		{
			JoystickButtonData gunBind = null;
			if (gunNumber == 1)
			{
				gunBind = Program.dinputTriggerGunA;
			}
			if (gunNumber == 2)
			{
				gunBind = Program.dinputTriggerGunB;
			}
			if (gunBind == null) { return; }



			Utils.LogMessage($"Demul Spawn JoyListener for gun {gunNumber.ToString()} : {gunBind.JoystickGuid.ToString()} !");
			var directInput = new DirectInput();

			Joystick joystick = null;
			var devicesInstance = new List<DeviceInstance>();
			devicesInstance.AddRange(directInput.GetDevices().Where(x => x.Type != DeviceType.Mouse && x.UsagePage != UsagePage.VendorDefinedBegin && x.Usage != UsageId.AlphanumericBitmapSizeX && x.Usage != UsageId.AlphanumericAlphanumericDisplay && x.UsagePage != unchecked((UsagePage)0xffffff43) && x.UsagePage != UsagePage.Vr).ToList());
			DeviceInstance deviceInstance = null;
			bool found_device = false;
			Guid device_guid = Guid.Empty;
			foreach (var device in devicesInstance)
			{
				if (device.InstanceGuid.ToString() == gunBind.JoystickGuid.ToString())
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

						//Utils.LogMessage(inputText);
						if (inputText == gunBind.Title && deviceInstance.InstanceGuid.ToString() == gunBind.JoystickGuid.ToString())
						{
							if (gunNumber == 1)
							{
								if (triggerGunA != pressed)
								{

									if (pressed && (selfRecoilMode == "gunshot" || selfRecoilMode == "auto"))
									{
										DoRecoil(gunNumber);
									}
									triggerGunA = pressed;
									//Utils.LogMessage($"triggerGunA  {inputText} : {pressed}");
								}
							}
							if (gunNumber == 2)
							{
								if (triggerGunB != pressed)
								{

									if (pressed && (selfRecoilMode == "gunshot" || selfRecoilMode == "auto"))
									{
										DoRecoil(gunNumber);
									}
									triggerGunB = pressed;
									//Utils.LogMessage($"triggerGunB  {inputText} : {pressed}");
								}
							}

						}

					}

					Thread.Sleep(20);
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

		public static void ClientDemulshooter()
		{
			while (!_stopListening)
			{
				TcpClient client = new TcpClient();
				try
				{
					client.Connect("127.0.0.1", 8000);
					using (NetworkStream stream = client.GetStream())
					{
						byte[] buffer = new byte[1024];
						int bytesRead;

						while (!_stopListening)
						{
							bytesRead = stream.Read(buffer, 0, buffer.Length);
							string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
							//Utils.LogMessage(dataReceived);
							if (dataReceived.Contains("P1_CtmRecoil = 1")) Utils.LogMessage("GunShot P1");
							if (dataReceived.Contains("P2_CtmRecoil = 1")) Utils.LogMessage("GunShot P2");
							if (dataReceived.Contains("P1_Damaged = 1")) Utils.LogMessage("Damage P1");
							if (dataReceived.Contains("P2_Damaged = 1")) Utils.LogMessage("Damage P2");

							if (dataReceived.Contains("P1_CtmRecoil = 1")) DoRecoil(1);
							if (dataReceived.Contains("P2_CtmRecoil = 1")) DoRecoil(2);

							if (dataReceived.Contains("P1_Damaged = 1")) DoRecoil(1, true);
							if (dataReceived.Contains("P2_Damaged = 1")) DoRecoil(1, true);
						}
					}
				}
				catch (Exception e) { }
				finally { client.Close(); }
			}

		}

		public static void Stop()
		{
			_stopListening = true;
			Thread.Sleep(1000);
			try
			{
				if (MonitorDemulshooter != null && MonitorDemulshooter.IsAlive)
				{
					MonitorDemulshooter.Abort();
				}
			}
			catch { }



			try
			{
				if (gunASerial != null)
				{
					Release_Gun4ir(1);
					if (gunASerial.IsConnected) gunASerial.Disconnect();
					gunASerial = null;
				}
			}
			catch { }

			try
			{
				if (gunBSerial != null)
				{
					Release_Gun4ir(2);
					if (gunBSerial.IsConnected) gunBSerial.Disconnect();
					gunBSerial = null;
				}
			}
			catch { }


			if (gunAPipe != null)
			{
				try
				{
					gunAStream.Close();
					gunAPipe.Close();

				}
				catch { }

				gunAPipe.Dispose();
				gunAPipe = null;
				gunAStream.Dispose();
				gunAStream = null;
			}

			if (gunBPipe != null)
			{
				try
				{
					gunBStream.Close();
					gunBPipe.Close();

				}
				catch { }

				gunBPipe.Dispose();
				gunBPipe = null;
				gunBStream.Dispose();
				gunBStream = null;
			}

			Console.WriteLine("end Demulshooter");

			if (!ValidPath) return;
		}

		public static void InitGuns(string rumbleTypeA, string rumbleParameterA, string rumbleTypeB, string rumbleParameterB,
			bool gunAAutoJoy, bool gunADamageRumble, bool gunA4tiers, bool gunBAutoJoy, bool gunBDamageRumble, bool gunB4tiers, bool blockrecoil = true
			)
		{


			Utils.LogMessage($"SindenInit {rumbleTypeA} {rumbleParameterA}");
			GunA4tiers = gunA4tiers;
			GunAAutoJoy = gunAAutoJoy;
			GunADamageRumble = gunADamageRumble;
			GunB4tiers = gunB4tiers;
			GunBAutoJoy = gunBAutoJoy;
			GunBDamageRumble = gunBDamageRumble;

			gunARecoil = false;
			gunBRecoil = false;

			if (rumbleTypeA == "gun4ir" && rumbleParameterA != "")
			{
				gunAGun4ir = true;
				gunARecoil = true;
				gunAParameter = rumbleParameterA;
				Init_Gun4ir(1, blockrecoil);
			}

			if (rumbleTypeA == "sinden" && rumbleParameterA != "")
			{
				gunASinden = true;
				gunARecoil = true;
				gunAParameter = rumbleParameterA;
			}

			if (rumbleTypeA == "rumble" && rumbleParameterA != "")
			{
				gunARumble = true;
				gunARecoil = true;
				gunAParameter = rumbleParameterA;
			}

			if (rumbleTypeA == "lichtknarre" && rumbleParameterA != "")
			{
				gunALichtknarre = true;
				gunARecoil = true;
				gunAParameter = rumbleParameterA;
			}

			if (rumbleTypeB == "gun4ir" && rumbleParameterB != "")
			{
				gunBGun4ir = true;
				gunBRecoil = true;
				gunBParameter = rumbleParameterB;
				Init_Gun4ir(2, blockrecoil);
			}

			if (rumbleTypeB == "sinden" && rumbleParameterB != "")
			{
				gunBSinden = true;
				gunBRecoil = true;
				gunBParameter = rumbleParameterB;
			}

			if (rumbleTypeB == "rumble" && rumbleParameterB != "")
			{
				gunBRumble = true;
				gunBRecoil = true;
				gunBParameter = rumbleParameterB;
			}

			if (rumbleTypeB == "lichtknarre" && rumbleParameterB != "")
			{
				gunBLichtknarre = true;
				gunBRecoil = true;
				gunBParameter = rumbleParameterA;
			}

			if (rumbleTypeA == "rumble" || rumbleTypeB == "rumble")
			{
				SDL2.SDL.SDL_Quit();
				SDL2.SDL.SDL_SetHint(SDL2.SDL.SDL_HINT_JOYSTICK_RAWINPUT, "0");
				SDL2.SDL.SDL_Init(SDL2.SDL.SDL_INIT_JOYSTICK | SDL2.SDL.SDL_INIT_GAMECONTROLLER);
			}

			if (rumbleTypeA == "lichtknarre" || rumbleTypeB == "lichtknarre")
			{
				if (dummyForm == null)
				{
					dummyForm = new Form();
					dummyForm.Visible = false;
					dummyForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
					dummyForm.ShowInTaskbar = false;
					dummyForm.Opacity = 0;
					dummyForm.Size = new Size(0, 0);
					dummyForm.Show(); // Montre la fenêtre pour obtenir le handle
					dummyFormHandle = dummyForm.Handle;
					InitLichtknarre();
				}
			}

		}

		public static void InitLichtknarre()
		{
			if (gunALichtknarre)
			{
				Guid deviceGuid;
				if (Guid.TryParse(gunAParameter, out deviceGuid))
				{
					var directInput = new DirectInput();
					gunA_lichtknarrejoy = new Joystick(directInput, deviceGuid);
					var actuatorsLst = new List<int>(1);
					foreach (var obj in gunA_lichtknarrejoy.GetObjects())
					{
						if (obj.ObjectId.Flags.HasFlag(DeviceObjectTypeFlags.ForceFeedbackActuator))
							actuatorsLst.Add((int)obj.ObjectId);

					}
					_actuatorObjectIds_gunA = actuatorsLst.ToArray();
					if (_actuatorObjectIds_gunA.Length > 0)
					{
						dirVector_gunA = Enumerable.Repeat(1, _actuatorObjectIds_gunA.Length).ToArray();
						gunA_lichtknarrejoy.Properties.BufferSize = 128;
						gunA_lichtknarrejoy.SetCooperativeLevel(dummyFormHandle, CooperativeLevel.Exclusive | CooperativeLevel.Background);
						gunA_lichtknarrejoy.Acquire();

						var constantForce = new ConstantForce
						{
							Magnitude = 5000 // Set the force magnitude (range is typically -10,000 to 10,000)
						};

						var effectParameters = new EffectParameters
						{
							Flags = EffectFlags.Cartesian | EffectFlags.ObjectIds,
							Duration = 200,
							SamplePeriod = 0,
							Gain = 10000,
							TriggerButton = -1,
							TriggerRepeatInterval = int.MaxValue,
							Axes = _actuatorObjectIds_gunA,
							Directions = dirVector_gunA,
							StartDelay = 0,
							Envelope = null,
							Parameters = constantForce
						};
						effect_gunA = new Effect(gunA_lichtknarrejoy, EffectGuid.ConstantForce, effectParameters);

						effect_gunA.Start(1, EffectPlayFlags.NoDownload);
						Thread.Sleep(200);
						effect_gunA.Stop();

					}

				}
			}
			if (gunBLichtknarre)
			{
				Guid deviceGuid;
				if (Guid.TryParse(gunBParameter, out deviceGuid))
				{
					var directInput = new DirectInput();
					gunB_lichtknarrejoy = new Joystick(directInput, deviceGuid);
					var actuatorsLst = new List<int>(1);
					foreach (var obj in gunB_lichtknarrejoy.GetObjects())
					{
						if (obj.ObjectId.Flags.HasFlag(DeviceObjectTypeFlags.ForceFeedbackActuator))
							actuatorsLst.Add((int)obj.ObjectId);

					}
					_actuatorObjectIds_gunB = actuatorsLst.ToArray();
					if (_actuatorObjectIds_gunB.Length > 0)
					{
						dirVector_gunB = Enumerable.Repeat(1, _actuatorObjectIds_gunB.Length).ToArray();
						gunB_lichtknarrejoy.Properties.BufferSize = 128;
						gunB_lichtknarrejoy.SetCooperativeLevel(dummyFormHandle, CooperativeLevel.Exclusive | CooperativeLevel.Background);
						gunB_lichtknarrejoy.Acquire();

						var constantForce = new ConstantForce
						{
							Magnitude = 5000 // Set the force magnitude (range is typically -10,000 to 10,000)
						};

						var effectParameters = new EffectParameters
						{
							Flags = EffectFlags.Cartesian | EffectFlags.ObjectIds,
							Duration = 200,
							SamplePeriod = 0,
							Gain = 10000,
							TriggerButton = -1,
							TriggerRepeatInterval = int.MaxValue,
							Axes = _actuatorObjectIds_gunA,
							Directions = dirVector_gunA,
							StartDelay = 0,
							Envelope = null,
							Parameters = constantForce
						};
						effect_gunB = new Effect(gunB_lichtknarrejoy, EffectGuid.ConstantForce, effectParameters);

						effect_gunB.Start(1, EffectPlayFlags.NoDownload);
						Thread.Sleep(200);
						effect_gunB.Stop();
					}

				}
			}



		}

		public static void DoRecoil(int gunIndex, bool rumble = false)
		{
			long timestamp = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;

			if (gunIndex == 1 && gunARecoil)
			{
				Task.Run(() =>
				{
					if (rumble && gunARumble == false && gunALichtknarre == false && (gunAGun4ir && GunADamageRumble) == false)
					{
						return;
					}


					lock (lockGunA)
					{
						if (isStartedGunA)
							return;
						isStartedGunA = true;
					}

					if (!rumble) lastRecoilGunA = timestamp;

					if (gunAGun4ir) Gunshot_Gun4ir(gunIndex, rumble);
					if (gunASinden) Gunshot_Sinden(gunIndex);
					if (gunARumble) Gunshot_Rumble(gunIndex, rumble);
					if (gunALichtknarre) Gunshot_Lichtknarre(gunIndex, rumble);

					lock (lockGunB)
					{
						isStartedGunA = false;
					}
				});
			}
			if (gunIndex == 2 && gunBRecoil)
			{
				if (rumble && gunBRumble == false && gunBLichtknarre == false && (gunBGun4ir && GunBDamageRumble) == false)
				{
					return;
				}
				Task.Run(() =>
				{
					lock (lockGunB)
					{
						if (isStartedGunB)
							return;
						isStartedGunB = true;
					}

					if (!rumble) lastRecoilGunB = timestamp;

					if (gunBGun4ir) Gunshot_Gun4ir(gunIndex, rumble);
					if (gunBSinden) Gunshot_Sinden(gunIndex);
					if (gunBRumble) Gunshot_Rumble(gunIndex, rumble);
					if (gunBLichtknarre) Gunshot_Lichtknarre(gunIndex, rumble);

					lock (lockGunB)
					{
						isStartedGunB = false;
					}
				});
			}
		}

		private static void Gunshot_Lichtknarre(int gunIndex, bool rumble = false)
		{
			if (gunIndex == 1)
			{
				if (effect_gunA != null)
				{
					effect_gunA.Start(1, EffectPlayFlags.NoDownload);
					if (rumble) Thread.Sleep(300);
					else Thread.Sleep(120);
					effect_gunA.Stop();
				}
			}
			if (gunIndex == 2)
			{
				if (effect_gunB != null)
				{
					effect_gunB.Start(1, EffectPlayFlags.NoDownload);
					if (rumble) Thread.Sleep(300);
					else Thread.Sleep(120);
					effect_gunB.Stop();
				}
			}

		}

		private static void Gunshot_Rumble(int gunIndex, bool rumble = false)
		{
			string GunSDLGuid = "";
			bool joyAttached = false;
			if (gunIndex == 1)
			{
				if (GunASDLGuid == "")
				{
					GunASDLGuid = ButtonToKey.DSharpGuidToSDLGuid(gunAParameter);
					Utils.LogMessage($"GUNA : {gunAParameter} => {GunASDLGuid}");
				}

				GunSDLGuid = GunASDLGuid;
				joyAttached = gunAjoyAttached;

			}
			if (gunIndex == 2)
			{
				if (GunBSDLGuid == "")
				{
					GunBSDLGuid = ButtonToKey.DSharpGuidToSDLGuid(gunBParameter);
					Utils.LogMessage($"GUNB : {gunBParameter} => {GunBSDLGuid}");
				}
				GunSDLGuid = GunBSDLGuid;
				joyAttached = gunBjoyAttached;
			}

			if (!joyAttached)
			{
				string target_Guid = GunSDLGuid;
				int found_guid = -1;
				SDL2.SDL.SDL_JoystickUpdate();
				for (int i = 0; i < SDL2.SDL.SDL_NumJoysticks(); i++)
				{
					if (SDL.SDL_IsGameController(i) == SDL.SDL_bool.SDL_FALSE) continue;
					if (i == GunASDLIndex || i == GunBSDLIndex) continue;

					var currentJoy = SDL.SDL_JoystickOpen(i);
					//string nameController = SDL2.SDL.SDL_JoystickNameForIndex(i).Trim('\0');
					{
						const int bufferSize = 256; // La taille doit être au moins 33 pour stocker le GUID sous forme de chaîne (32 caractères + le caractère nul)
						byte[] guidBuffer = new byte[bufferSize];
						SDL.SDL_JoystickGetGUIDString(SDL.SDL_JoystickGetGUID(currentJoy), guidBuffer, bufferSize);
						string guidString = System.Text.Encoding.UTF8.GetString(guidBuffer).Trim('\0');
						if (guidString == target_Guid)
						{
							found_guid = i;
							SDL.SDL_JoystickClose(currentJoy);
							break;
						}
						SDL.SDL_JoystickClose(currentJoy);
					}
				}

				if (found_guid >= 0)
				{
					joyAttached = true;
					if (gunIndex == 1)
					{
						//joy = currentJoy;
						gunAjoyAttached = true;
						GunASDLIndex = found_guid;

					}
					if (gunIndex == 2)
					{
						//joy = currentJoy;
						gunBjoyAttached = true;
						GunBSDLIndex = found_guid;

					}
				}
			}

			if (joyAttached)
			{
				if (gunIndex == 1)
				{
					try
					{
						IntPtr currentJoy = SDL.SDL_GameControllerOpen(GunASDLIndex);
						SDL.SDL_JoystickRumble(SDL.SDL_GameControllerGetJoystick(currentJoy), 0xFFFF, 0xFFFF, 100);

						if (rumble) Thread.Sleep(300);
						else Thread.Sleep(120);

						SDL.SDL_JoystickRumble(SDL.SDL_GameControllerGetJoystick(currentJoy), 0, 0, 0);
						SDL.SDL_GameControllerClose(currentJoy);
					}
					catch { }



				}
				if (gunIndex == 2)
				{
					try
					{
						IntPtr currentJoy = SDL.SDL_GameControllerOpen(GunBSDLIndex);
						SDL.SDL_JoystickRumble(SDL.SDL_GameControllerGetJoystick(currentJoy), 0xFFFF, 0xFFFF, 100);

						if (rumble) Thread.Sleep(300);
						else Thread.Sleep(120);

						SDL.SDL_JoystickRumble(SDL.SDL_GameControllerGetJoystick(currentJoy), 0, 0, 0);
						SDL.SDL_GameControllerClose(currentJoy);
					}
					catch { }

				}
			}
		}

		private static void Gunshot_Sinden(int gunIndex)
		{
			Utils.LogMessage("SindenGun Gunshot");
			if (gunIndex == 1)
			{
				if (gunAPipe == null)
				{
					gunAPipe = new NamedPipeClientStream(".", gunAParameter, PipeDirection.Out);
				}
				if (gunAStream == null)
				{
					gunAStream = new StreamWriter(gunAPipe, Encoding.UTF8);
				}
				if (!gunAPipe.IsConnected)
				{
					try
					{
						gunAPipe.Connect();
					}
					catch (Exception ex)
					{
						try
						{
							gunAStream.Close();
							gunAPipe.Close();

						}
						catch { }

						gunAPipe.Dispose();
						gunAPipe = null;
						gunAStream.Dispose();
						gunAStream = null;
						return;
					}
				}
				try
				{
					gunAStream.Write("1");
					gunAStream.Flush();
				}
				catch (Exception ex)
				{
					try
					{
						gunAStream.Close();
						gunAPipe.Close();
					}
					catch { }

					gunAPipe.Dispose();
					gunAPipe = null;
					gunAStream.Dispose();
					gunAStream = null;
					return;
				}
			}
			if (gunIndex == 2)
			{
				if (gunBPipe == null)
				{
					gunBPipe = new NamedPipeClientStream(".", gunBParameter, PipeDirection.Out);
				}
				if (gunBStream == null)
				{
					gunBStream = new StreamWriter(gunBPipe, Encoding.UTF8);
				}
				if (!gunBPipe.IsConnected)
				{
					try
					{
						gunBPipe.Connect();
					}
					catch (Exception ex)
					{
						try
						{
							gunBStream.Close();
							gunBPipe.Close();

						}
						catch { }

						gunBPipe.Dispose();
						gunBPipe = null;
						gunBStream.Dispose();
						gunBStream = null;
						return;
					}
				}
				try
				{
					gunBStream.Write("1");
					gunBStream.Flush();
				}
				catch (Exception ex)
				{
					try
					{
						gunBStream.Close();
						gunBPipe.Close();
					}
					catch { }

					gunBPipe.Dispose();
					gunBPipe = null;
					gunBStream.Dispose();
					gunBStream = null;
					return;
				}

			}


		}


		private static void Init_Gun4ir(int gunIndex, bool blockrecoil = true)
		{
			bool autoJoy = false;
			bool mode4tiers = false;
			if (gunIndex == 1)
			{
				autoJoy = GunAAutoJoy;
				mode4tiers = GunA4tiers;
			}
			if (gunIndex == 2)
			{
				autoJoy = GunAAutoJoy;
				mode4tiers = GunB4tiers;
			}



			if (blockrecoil) Gun4Ir_SerialSend(gunIndex, "S6");
			Thread.Sleep(100);
			if (autoJoy) Gun4Ir_SerialSend(gunIndex, "M0x1");
			Thread.Sleep(100);
			if (mode4tiers) Gun4Ir_SerialSend(gunIndex, "M3x1");
			else Gun4Ir_SerialSend(gunIndex, "M3x0");
			Thread.Sleep(100);
			//Gun4Ir_SerialSend(gunIndex, "F1x2x2x");
		}

		private static void Release_Gun4ir(int gunIndex)
		{
			Gun4Ir_SerialSend(gunIndex, "E");
		}

		private static void Gun4Ir_SerialSend(int gunIndex, string data)
		{
			/*
			string msg = "F1x2x2x";
			var _serialPort = new SerialPortInput(false);

			_serialPort.SetPort(@"COM28", 9600);
			_serialPort.Connect();
			byte[] bytes = Encoding.ASCII.GetBytes(msg);
			_serialPort.SendMessage(bytes);
			_serialPort.Disconnect();
			*/



			Utils.LogMessage($"Send {data} to {gunIndex}");
			byte[] bytes = Encoding.ASCII.GetBytes(data);

			if (gunIndex == 1)
			{

				if (gunASerial == null)
				{
					gunASerial = new SerialPortInput(false);
					gunASerial.SetPort(gunAParameter, 9600);
				}
				try
				{
					if (!gunASerial.IsConnected) gunASerial.Connect();
					gunASerial.SendMessage(bytes);
				}
				catch (Exception ex) { }

			}
			if (gunIndex == 2)
			{
				if (gunBSerial == null)
				{
					gunBSerial = new SerialPortInput(false);
					gunBSerial.SetPort(gunBParameter, 9600);
				}
				try
				{
					if (!gunBSerial.IsConnected) gunBSerial.Connect();
					gunBSerial.SendMessage(bytes);
				}
				catch (Exception ex) { }
			}

			Utils.LogMessage($"Fin Send");


		}

		private static void Gunshot_Gun4ir(int gunIndex, bool rumble = false)
		{

			if (rumble) Gun4Ir_SerialSend(gunIndex, "F1x2x2x");
			else Gun4Ir_SerialSend(gunIndex, "F0x2x0x");
			/*
			string parameter = "";
			SerialPort serialPort = null;
			if (gunIndex == 1)
			{
				serialPort = gunASerial;
				parameter = gunAParameter;
			}
			if (gunIndex == 2)
			{
				serialPort = gunBSerial;
				parameter = gunBParameter;
			}


			if (serialPort == null)
			{
				serialPort = new SerialPort(parameter);
				serialPort.BaudRate = 9600; // Vitesse de transmission
				serialPort.Parity = Parity.None; // Parité
				serialPort.DataBits = 8; // Nombre de bits de données
				serialPort.StopBits = StopBits.One; // Bits d'arrêt
			}

			try
			{
				if (!serialPort.IsOpen) serialPort.Open();
			}
			catch (Exception ex) { }

			string dataToSend = "Hello, world!";
			bool retry = false;
			try
			{
				serialPort.WriteLine(dataToSend);
			}
			catch (Exception ex)
			{
				serialPort.Dispose();
				serialPort = null;
				retry = true;
			}
			if (retry)
			{
				try
				{
					serialPort = new SerialPort(parameter);
					serialPort.BaudRate = 9600; // Vitesse de transmission
					serialPort.Parity = Parity.None; // Parité
					serialPort.DataBits = 8; // Nombre de bits de données
					serialPort.StopBits = StopBits.One; // Bits d'arrêt
					serialPort.Open();
					serialPort.WriteLine(dataToSend);
				}
				catch (Exception ex) { }
			}
			*/
		}
	}
}
