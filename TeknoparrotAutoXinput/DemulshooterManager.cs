using SDL2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
		public static string Target = "";
		public static string Rom = "";
		public static bool UseMamehooker = false;
		public static bool UseTcp = false;
		public static Thread MonitorDemulshooter;
		private static bool _stopListening = false;


		public static bool gunARecoil = false;
		public static bool gunBRecoil = false;
		public static bool gunASinden = false;
		public static bool gunBSinden = false;
		public static bool gunAGun4ir = false;
		public static bool gunBGun4ir = false;
		public static bool gunARumble = false;
		public static bool gunBRumble = false;
		public static string gunAParameter = "";
		public static string gunBParameter = "";

		static object lockGunA = new object();
		static object lockGunB = new object();
		static bool isStartedGunA = false;
		static bool isStartedGunB = false;


		static SerialPort gunASerial;
		static NamedPipeClientStream gunAPipe;
		static StreamWriter gunAStream;
		static bool gunAjoyAttached = false;
		static string GunASDLGuid = string.Empty;
		static int GunASDLIndex = -1;

		static SerialPort gunBSerial;
		static NamedPipeClientStream gunBPipe;
		static StreamWriter gunBStream;
		static bool gunBjoyAttached = false;
		static string GunBSDLGuid = string.Empty;
		static int GunBSDLIndex = -1;

		public static bool SetPath(string demulshooterExe)
		{
			DemulshooterPath = "";
			Demulshooter32 = "";
			Demulshooter64 = "";
			ValidPath = false;
			if(File.Exists(demulshooterExe))
			{

				string dirDemul = Path.GetFullPath(Path.GetDirectoryName(demulshooterExe));
				string demulshooter32 = Path.Combine(dirDemul, "DemulShooter.exe");
				string demulshooter64 = Path.Combine(dirDemul, "DemulShooterX64.exe");
				if(File.Exists(demulshooter32) && File.Exists(demulshooter64))
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

		public static void WriteConfig()
		{
			if (!ValidPath) return;
			var ini = new IniFile(Path.Combine(DemulshooterPath, "config.ini"));
			ini.Write("Launch_Target", Target);
			ini.Write("Launch_Rom", Rom);
			ini.Write("Launch_64bits", Is64bits ? "True" : "False");
			ini.Write("OutputEnabled", "True");
			ini.Write("ParentProcess", Process.GetCurrentProcess().Id.ToString());
			
			ini.Write("WM_OutputsEnabled", UseMamehooker ? "True" : "False");
			ini.Write("Net_OutputsEnabled", UseTcp ? "True" : "False");



		}

		public static void ReadConfig()
		{
			if (!ValidPath) return;
			if(File.Exists(Path.Combine(DemulshooterPath, "config.ini")))
			{
				var ini = new IniFile(Path.Combine(DemulshooterPath, "config.ini"));
				Rom = ini.Read("Launch_Rom");
				Target = ini.Read("Launch_Target");
				Is64bits = ini.Read("Launch64bits") == "True" ? true : false;
				ParentProcess = int.Parse(ini.Read("ParentProcess"));
			}

		}

		public static void Start()
		{
			if (!ValidPath) return;
			WriteConfig();
			string selfExe = Process.GetCurrentProcess().MainModule.FileName;
			if (!Utils.CheckTaskExist(selfExe, "--demulshooter"))
			{

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
			}
			Utils.ExecuteTask(Utils.ExeToTaskName(selfExe, "--demulshooter"),-1);
			MonitorDemulshooter = new Thread(() => ClientDemulshooter());
			MonitorDemulshooter.Start();

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
							if(dataReceived.Contains("P1_CtmRecoil = 1")) Utils.LogMessage("GunShot P1");
							if(dataReceived.Contains("P2_CtmRecoil = 1")) Utils.LogMessage("GunShot P2");
							if(dataReceived.Contains("P1_Damaged = 1")) Utils.LogMessage("Damage P1");
							if(dataReceived.Contains("P2_Damaged = 1")) Utils.LogMessage("Damage P2");

							if (dataReceived.Contains("P1_CtmRecoil = 1")) DoRecoil(1);
							if (dataReceived.Contains("P2_CtmRecoil = 1")) DoRecoil(2);
						}
					}
				}
				catch (Exception e){}
				finally{client.Close();}
			}

		}

		public static void Stop()
		{
			_stopListening = true;
			if (!ValidPath) return;
		}

		public static void InitGuns(string rumbleTypeA, string rumbleParameterA, string rumbleTypeB, string rumbleParameterB)
		{
			gunARecoil = false;
			gunBRecoil = false;

			if(rumbleTypeA == "gun4ir" && rumbleParameterA != "")
			{
				gunAGun4ir = true;
				gunARecoil = true;
				gunAParameter = rumbleParameterA;
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

			if (rumbleTypeB == "gun4ir" && rumbleParameterB != "")
			{
				gunBGun4ir = true;
				gunBRecoil = true;
				gunBParameter = rumbleParameterB;
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

			if(rumbleTypeA == "rumble" || rumbleTypeB == "rumble")
			{
				SDL2.SDL.SDL_Quit();
				SDL2.SDL.SDL_SetHint(SDL2.SDL.SDL_HINT_JOYSTICK_RAWINPUT, "0");
				SDL2.SDL.SDL_Init(SDL2.SDL.SDL_INIT_JOYSTICK | SDL2.SDL.SDL_INIT_GAMECONTROLLER);
			}


		}

		public static void DoRecoil(int gunIndex)
		{
			if (gunIndex == 1 && gunARecoil)
			{
				Task.Run(() =>
				{
					lock (lockGunA)
					{
						if (isStartedGunA)
							return;
						isStartedGunA = true;
					}

					if (gunAGun4ir) Gunshot_Gun4ir(gunIndex);
					if (gunASinden) Gunshot_Sinden(gunIndex);
					if (gunARumble) Gunshot_Rumble(gunIndex);

					lock (lockGunB)
					{
						isStartedGunA = false;
					}
				});
			}
			if (gunIndex == 2 && gunBRecoil)
			{
				Task.Run(() =>
				{
					lock (lockGunB)
					{
						if (isStartedGunB)
							return;
						isStartedGunB = true;
					}

					if (gunBGun4ir) Gunshot_Gun4ir(gunIndex);
					if (gunBSinden) Gunshot_Sinden(gunIndex);
					if (gunBRumble) Gunshot_Rumble(gunIndex);

					lock (lockGunB)
					{
						isStartedGunB = false;
					}
				});
			}
		}

		private static void Gunshot_Rumble(int gunIndex)
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
						Thread.Sleep(120);
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
						Thread.Sleep(120);
						SDL.SDL_JoystickRumble(SDL.SDL_GameControllerGetJoystick(currentJoy), 0, 0, 0);
						SDL.SDL_GameControllerClose(currentJoy);
					}
					catch { }
					
				}
			}




		}

		private static void Gunshot_Sinden(int gunIndex)
		{
			throw new NotImplementedException();
		}

		private static void Gunshot_Gun4ir(int gunIndex)
		{
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
		}
	}
}
