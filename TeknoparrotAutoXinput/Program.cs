using ControllerToMouseMapper.Input;
using Gma.System.MouseKeyHook;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using MonitorSwitcherGUI;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Exceptions;
using Nefarius.ViGEm.Client.Targets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SDL2;
using SharpDX.DirectInput;
using SharpDX.Multimedia;
using System.Diagnostics;
using System.Globalization;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using WiimoteLib;
using WindowsDisplayAPI;
using XInput.Wrapper;
using XInputium.ModifierFunctions;
using XInputium.XInput;
using XJoy;
using static XInput.Wrapper.X;


namespace TeknoparrotAutoXinput
{
	internal static class Program
	{
		public static float version = 0.90f;

		public static Dictionary<string, string> cacheFile = new Dictionary<string, string>();

		// Importer la fonction AllocConsole depuis Kernel32.dll
		[DllImport("kernel32.dll")]
		public static extern bool AllocConsole();

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetStdHandle(int nStdHandle);


		private static readonly string mutexName = "Global\\TAX_Launched";

		//private static NamedPipeClientStream pipeClient;
		public static Dictionary<int, Dictionary<int, string>> joysticksIds = new Dictionary<int, Dictionary<int, string>>();

		public static ViGEmClient client = null;
		public static IXbox360Controller controller = null;
		public static int virtualKeyboardXinputSlot = -1;
		private static IKeyboardMouseEvents _globalHook = null;
		private static readonly Mutex Mutex = new Mutex();

		private static Startup startupForm;
		private static CancellationTokenSource cancellationTokenSource;

		public static string wheelXinputData;
		public static string arcadeXinputData;
		public static string gamepadXinputData;

		public static Dictionary<int, string> forceTypeController;

		public static string WheelGuid = "";
		public static string WheelFFBGuid = "";

		public static string HotasGuid = "";
		public static string HotasFFBGuid = "";

		public static string GunAGuid = "";
		public static string GunBGuid = "";
		public static string GunAType = "";
		public static string GunBType = "";

		public static int GunASindenType = 0;
		public static int GunBSindenType = 0;


		public static string VjoyGuid = "";

		public static Guid? FirstKeyboardGuid = null;

		public static string ParrotDataOriginal = "";
		public static string ParrotDataBackup = "";
		public static string FFBPluginIniFile = "";
		public static string FFBPluginIniBackup = "";

		private static bool _restoreSwitch = false;
		private static string _dispositionToSwitch = "";

		public static List<int> ProcessToKill = new List<int>();
		public static List<string> FilesToDelete = new List<string>();

		//Folder List
		public static string DispositionFolder = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "dispositions");
		public static string GameOptionsFolder = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "gameoptions");

		public static bool DebugMode = false;

		public static bool useXinput = true;
		public static bool useDinputWheel = false;
		public static bool useDinputHotas = false;
		public static bool useDinputLightGun = false;

		public static bool shifterGuidFound = false;
		public static bool throttleGuidFound = false;
		public static bool dinputLightgunAFound = false;
		public static bool dinputLightgunBFound = false;


		public static bool favorJoystick = false;

		public static Dictionary<string, string> GameInfo = new Dictionary<string, string>();

		public static bool vjoy_gunA = false;
		public static bool vjoy_gunB = false;

		public static bool allSindenWithoutVjoy = true;
		public static bool atLeastOneSinden = false;

		public static bool crosshairA = true;
		public static bool crosshairB = true;
		public static bool hideCrosshair = false;

		//public static bool IsWindowed = false;

		public static bool isExiting = false;
		public static int magpie_process_pid = -1;
		public static string magpieIni = "";

		public static bool patchGpuFix = ConfigurationManager.MainConfig.patchGpuFix;
		public static bool patchGpuTP = ConfigurationManager.MainConfig.patchGpuTP;

		public static int gpuType = ConfigurationManager.MainConfig.gpuType;
		public static int gpuResolution = ConfigurationManager.MainConfig.gpuResolution;
		public static bool patchResolutionFix = ConfigurationManager.MainConfig.patchResolutionFix;
		public static bool patchResolutionTP = ConfigurationManager.MainConfig.patchResolutionTP;

		public static int performanceProfile = ConfigurationManager.MainConfig.performanceProfile;
		public static bool forceVsync = ConfigurationManager.MainConfig.forceVsync;
		public static int refreshRate = 60;
		public static int currentResX = 0;
		public static int currentResY = 0;

		public static int displayMode = ConfigurationManager.MainConfig.displayMode;
		public static bool patchDisplayModeFix = ConfigurationManager.MainConfig.patchDisplayModeFix;
		public static bool patchDisplayModeTP = ConfigurationManager.MainConfig.patchDisplayModeTP;


		public static bool patchReshade = ConfigurationManager.MainConfig.patchReshade;
		public static bool patchGameID = ConfigurationManager.MainConfig.patchGameID;
		public static bool patchNetwork = ConfigurationManager.MainConfig.patchNetwork;
		public static bool patchOtherTPSettings = ConfigurationManager.MainConfig.patchOtherTPSettings;
		public static bool patchOthersGameOptions = ConfigurationManager.MainConfig.patchOthersGameOptions;
		public static bool patchFFB = ConfigurationManager.MainConfig.patch_FFB;

		public static bool useBezel = ConfigurationManager.MainConfig.useBezel;
		public static bool useCrt = ConfigurationManager.MainConfig.useCrt;
		public static bool useKeepAspectRatio = ConfigurationManager.MainConfig.keepAspectRatio;
		public static int patchLang = ConfigurationManager.MainConfig.patchLang;

		public static string patch_networkIP = ConfigurationManager.MainConfig.patch_networkIP;
		public static string patch_networkMask = ConfigurationManager.MainConfig.patch_networkMask;
		public static string patch_BroadcastAddress = ConfigurationManager.MainConfig.patch_BroadcastAddress;
		public static string patch_networkDns1 = ConfigurationManager.MainConfig.patch_networkDns1;
		public static string patch_networkDns2 = ConfigurationManager.MainConfig.patch_networkDns2;
		public static string patch_networkGateway = ConfigurationManager.MainConfig.patch_networkGateway;

		public static bool isPatreon = false;


		public static JoystickButtonData dinputTriggerGunA = null;
		public static JoystickButtonData dinputTriggerGunB = null;

		public static int vjoyIndex;

		public static bool linkAutoWritable = false;
		public static bool linkAutoHardLink = false;
		public static bool linkAutoSoftLink = false;

		public static bool linkExeWritable = false;
		public static bool linkExeHardLink = false;
		public static bool linkExeSoftLink = false;
		public static bool linkExePrecheck = false;

		public static string linkSourceFolderExe = "";

		public static Dictionary<int, string> vjoyData = new Dictionary<int, string>();

		public static bool usePresetDisposition = false;
		public static int presetDispositionWith = 0;
		public static int presetDispositionHeight = 0;
		public static int presetDispositionFrequency = 60;
		public static bool upscaleFullscreen = false;
		public static bool upscaleWindowed = false;
		public static bool need60hz = false;
		public static bool need60hzFullscreen = false;
		public static bool need60hzWindowed = false;

		public static bool forceResSwitchFullscreen = false;
		public static bool forceResSwitchWindowed = false;

		public static bool moveWindowToOriginalMonitor = false;
		public static string originalMonitorDeviceName = "";

		public static int forceResizeWidth = 0;
		public static int forceResizeHeight = 0;

		public static bool testMode = false;

		public static Dictionary<string, string> movetoFiles = new Dictionary<string, string>();

		public static int firstPlayerIsXinputGamepad = -1;
		public static int firstPlayerIsXinputArcade = -1;

		//public static string xmlFileContent = "";
		[DllImport("User32.dll")]
		private static extern uint GetDpiForSystem();

		[DllImport("shcore.dll")]
		private static extern int SetProcessDpiAwareness(ProcessDpiAwareness awareness);

		// Enum�ration pour la sensibilisation DPI
		public enum ProcessDpiAwareness
		{
			Unaware = 0,
			SystemDpiAware = 1,
			PerMonitorDpiAware = 2
		}

		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{


#if DEBUG
			List<string> fakeArgs = new List<string>();
			fakeArgs.Add(@"C:\teknoparrot\UserProfiles\AkaiKatanaShinNesica.xml");
			//args = fakeArgs.ToArray();
#endif
			//Up there to be load before demulshooter start
			ConfigurationManager.LoadConfig();

			//moveadmin : Used in patch to copy file to folder that need admin rights
			if (args.Length >= 1 && args.First() == "--moveadmin")
			{
				string tempFilePath = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "moveadmin.tmp.ahk");
				if (File.Exists(tempFilePath))
				{
					try
					{
						string currentDir = Path.GetFullPath(Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
						string ahkExe = Path.Combine(currentDir, "AutoHotkeyU32.exe");
						if (File.Exists(tempFilePath))
						{
							Process process = new Process();
							process.StartInfo.FileName = ahkExe;
							process.StartInfo.Arguments = tempFilePath;
							process.StartInfo.UseShellExecute = false;
							process.StartInfo.CreateNoWindow = true;
							process.Start();
							process.WaitForExit();
							Thread.Sleep(100);
							File.Delete(tempFilePath);
						}
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message);
					}
				}
				return;
			}

			if (args.Length >= 1 && args.First() == "--listvjoy")
			{
				string json = Utils.getVjoyData();
				File.WriteAllText(Path.Combine(Path.GetTempPath(), "list_vjoy.json"), json);
				return;
			}

			//rivatuner run as admin
			if (args.Length >= 1 && args.First() == "--xenos")
			{
				string XenosDir = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "xenos");
				if (!Directory.Exists(XenosDir))
				{
					MessageBox.Show("Xenos Dir does not exist");
					return;
				}
				if (args.Length == 1)
				{
					string Xenos32 = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "xenos", "Xenos.exe");
					string Xenos64 = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "xenos", "Xenos64.exe");
					string xenosConf32 = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "xenos", "teknoparrot32.xpr");
					string xenosConf64 = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "xenos", "teknoparrot64.xpr");

					string xenosExe = "";
					string xenosConfig = "";
					if (File.Exists(xenosConf32) && File.Exists(Xenos32))
					{
						xenosExe = Xenos32;
						xenosConfig = xenosConf32;
					}
					if (File.Exists(xenosConf64) && File.Exists(Xenos64))
					{
						xenosExe = Xenos64;
						xenosConfig = xenosConf64;
					}
					if (xenosExe != "")
					{
						Process process = new Process();
						process.StartInfo.FileName = xenosExe;
						process.StartInfo.Arguments = $@"--run ""{xenosConfig}""";
						process.StartInfo.WorkingDirectory = XenosDir;
						process.StartInfo.UseShellExecute = true;
						process.StartInfo.Verb = "runas";
						process.Start();
						process.WaitForExit();

					}


				}


				if (args.Length == 2 && args.Last() == "register")
				{
					string registryKeyPath = @"SOFTWARE\Microsoft\Windows Defender\Exclusions\Paths";
					using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKeyPath))
					{
						if (key != null)
						{
							// R�cup�rer les noms de toutes les sous-cl�s
							string[] subKeyNames = key.GetValueNames();

							// Afficher les noms de toutes les sous-cl�s
							foreach (string subKeyName in subKeyNames)
							{
								if (subKeyName == XenosDir)
								{
									MessageBox.Show("Directory already added in the exclusion list");
									return;
								}
							}
						}
					}
					string command = @$"Add-MpPreference -ExclusionPath """"{XenosDir}""""";
					var processStartInfo = new ProcessStartInfo();
					processStartInfo.FileName = "powershell.exe";
					processStartInfo.Arguments = $"-Command \"{command}\"";
					processStartInfo.UseShellExecute = false;
					processStartInfo.RedirectStandardOutput = true;

					using var process = new Process();
					process.StartInfo = processStartInfo;
					process.Start();
					string output = process.StandardOutput.ReadToEnd();

					using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKeyPath))
					{
						if (key != null)
						{
							// R�cup�rer les noms de toutes les sous-cl�s
							string[] subKeyNames = key.GetValueNames();

							// Afficher les noms de toutes les sous-cl�s
							foreach (string subKeyName in subKeyNames)
							{
								if (subKeyName == XenosDir)
								{
									MessageBox.Show("Path Added to the exclusion list");
									return;
								}
							}
						}
					}
					MessageBox.Show("Error adding path to exclusion list, do it manually");

				}
				return;
			}
			//Demulshooter run as admin
			if (args.Length == 1 && args.First() == "--demulshooter")
			{
				DemulshooterManager.SetPath(ConfigurationManager.MainConfig.demulshooterExe);
				if (DemulshooterManager.ValidPath)
				{
					DemulshooterManager.ReadConfig();
					ProcessStartInfo psi = new ProcessStartInfo
					{
						FileName = "taskkill",
						Arguments = $"/F /IM DemulShooter.exe",
						CreateNoWindow = true,
						UseShellExecute = false
					};
					Process.Start(psi);
					ProcessStartInfo psi2 = new ProcessStartInfo
					{
						FileName = "taskkill",
						Arguments = $"/F /IM DemulShooterX64.exe",
						CreateNoWindow = true,
						UseShellExecute = false
					};
					Process.Start(psi2);

					ProcessStartInfo psi3 = new ProcessStartInfo
					{
						FileName = "taskkill",
						Arguments = $"/F /IM " + Path.GetFileName(ConfigurationManager.MainConfig.mamehookerExe),
						CreateNoWindow = true,
						UseShellExecute = false
					};
					if (DemulshooterManager.UseMamehooker && ConfigurationManager.MainConfig.mamehookerExe != "" && File.Exists(ConfigurationManager.MainConfig.mamehookerExe))
					{

						Process.Start(psi3);
					}
					int pid = DemulshooterManager.ParentProcess;

					try
					{
						//	if (pid == -1) return;
						Process processParent = Process.GetProcessById(pid);
						string processName = processParent.ProcessName;

						if (processParent != null && processName == Process.GetCurrentProcess().ProcessName)
						{
							Thread monitoringThread = new Thread(() =>
							{

								// Attendre que le processus associ� au PID se termine
								processParent.WaitForExit();
								Process.Start(psi);
								Process.Start(psi2);
								Process.Start(psi3);
								Thread.Sleep(100);
								Utils.KillProcessById(Process.GetCurrentProcess().Id);

							});
							monitoringThread.Start();
						}
					}
					catch { }

					Thread.Sleep(1000);
					//MessageBox.Show($"mamehooker debug1 {DemulshooterManager.UseMamehooker} {ConfigurationManager.MainConfig.mamehookerExe}");
					if (DemulshooterManager.UseMamehooker && ConfigurationManager.MainConfig.mamehookerExe != "" && File.Exists(ConfigurationManager.MainConfig.mamehookerExe))
					{
						Process processMamehooker = new Process();
						processMamehooker.StartInfo.FileName = ConfigurationManager.MainConfig.mamehookerExe;
						processMamehooker.StartInfo.WorkingDirectory = Path.GetDirectoryName(ConfigurationManager.MainConfig.mamehookerExe);
						processMamehooker.StartInfo.UseShellExecute = true;
						processMamehooker.StartInfo.Verb = "runas";
						processMamehooker.Start();
						Thread.Sleep(1000);

					}

					string exePath = DemulshooterManager.Is64bits ? DemulshooterManager.Demulshooter64 : DemulshooterManager.Demulshooter32;
					string exeDir = Path.GetDirectoryName(exePath);
					Process process = new Process();
					process.StartInfo.FileName = exePath;
					if (DemulshooterManager.TargetProcess != "") process.StartInfo.Arguments = $"-target={DemulshooterManager.Target} -rom={DemulshooterManager.Rom} -pname=\"{DemulshooterManager.TargetProcess}\" -noinput" + (DemulshooterManager.HideCrosshair ? " -nocrosshair" : "");
					else process.StartInfo.Arguments = $"-target={DemulshooterManager.Target} -rom={DemulshooterManager.Rom} -noinput" + (DemulshooterManager.HideCrosshair ? " -nocrosshair" : "");

					/*
					if(DemulshooterManager.ForceMD5 != "")
					{
						process.StartInfo.Arguments += $" -forcemd5={DemulshooterManager.ForceMD5}";
					}
					*/

					process.StartInfo.WorkingDirectory = exeDir;
					process.StartInfo.UseShellExecute = true;
					process.StartInfo.Verb = "runas";
					process.Start();
					process.WaitForExit();
				}
				return;
			}

			//rivatuner run as admin
			if (args.Length == 1 && args.First() == "--rivatuner")
			{
				string rivaTunerExe = ConfigurationManager.MainConfig.rivatunerExe;
				string rivaTunerIniFile = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "rivatuner.ini");

				if (File.Exists(rivaTunerExe) && File.Exists(rivaTunerIniFile))
				{
					int pid = 0;
					try
					{
						rivaTunerIniFile = Path.GetFullPath(rivaTunerIniFile);
						IniFile ini = new IniFile(rivaTunerIniFile);
						int.TryParse(ini.Read("ParentProcess"), out pid);
					}
					catch
					{
						return;
					}
					if (pid <= 0) return;

					ProcessStartInfo psi = new ProcessStartInfo
					{
						FileName = "taskkill",
						Arguments = $"/F /IM RTSS.exe",
						CreateNoWindow = true,
						UseShellExecute = false
					};
					Process.Start(psi);
					Thread.Sleep(3000);
					ProcessStartInfo psi2 = new ProcessStartInfo
					{
						FileName = "taskkill",
						Arguments = $"/F /IM RTSS.exe",
						CreateNoWindow = true,
						UseShellExecute = false
					};
					Process.Start(psi2);

					Process processParent = Process.GetProcessById(pid);
					if (processParent != null)
					{
						Thread monitoringThread = new Thread(() =>
						{
							// Attendre que le processus associ� au PID se termine
							processParent.WaitForExit();

							Process.Start(psi);
							Thread.Sleep(3000);
							Process.Start(psi2);
							Thread.Sleep(100);
							Utils.KillProcessById(Process.GetCurrentProcess().Id);
						});
						monitoringThread.Start();
					}
					Thread.Sleep(1000);


					string exePath = rivaTunerExe;
					string exeDir = Path.GetDirectoryName(exePath);

					string profileDir = Path.Combine(exeDir, "Profiles");
					string profileDirBack = Path.Combine(exeDir, "ProfilesBak");
					//Restore first if anything went wrong
					if (Directory.Exists(profileDirBack) && Directory.Exists(profileDir))
					{
						Directory.Delete(profileDir, true);
						Thread.Sleep(100);
						Directory.Move(profileDirBack, profileDir);
						Thread.Sleep(100);
					}

					if (Directory.Exists(profileDir)) Directory.Move(profileDir, profileDirBack);
					Directory.CreateDirectory(profileDir);
					File.Copy(Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "thirdparty", "rivaconfig", "Config"), Path.Combine(profileDir, "Config"));
					File.Copy(Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "thirdparty", "rivaconfig", "Global"), Path.Combine(profileDir, "Global"));



					Process process = new Process();
					process.StartInfo.FileName = exePath;
					//process.StartInfo.Arguments = $"-target={DemulshooterManager.Target} -rom={DemulshooterManager.Rom} -noinput -nocrosshair";
					process.StartInfo.WorkingDirectory = exeDir;
					process.StartInfo.UseShellExecute = true;
					process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized; // Ajout de cette ligne pour minimiser la fen�tre
					process.StartInfo.Verb = "runas";
					process.Start();
					process.WaitForExit();

					Thread.Sleep(500);
					if (Directory.Exists(profileDirBack) && Directory.Exists(profileDir))
					{
						Directory.Delete(profileDir, true);
						Thread.Sleep(100);
						Directory.Move(profileDirBack, profileDir);
						Thread.Sleep(100);
					}

				}
				return;
			}

			//RegisterTask As admin
			if (args.Length > 2 && args.First() == "--registerTask")
			{
				var filteredArgs = Utils.ArgsWithoutFirstElement(args);
				var taskExe = filteredArgs[0];
				var taskArguments = Utils.ArgsToCommandLine(Utils.ArgsWithoutFirstElement(filteredArgs));
				Utils.RegisterTask(taskExe, taskArguments);
				return;
			}

			if (args.Length >= 1 && args.First() == "--registerAllTask")
			{

				string selfExe = Process.GetCurrentProcess().MainModule.FileName;
				if (!Utils.CheckTaskExist(selfExe, "--rivatuner")) Utils.RegisterTask(selfExe, "--rivatuner");
				if (!Utils.CheckTaskExist(selfExe, "--xenos")) Utils.RegisterTask(selfExe, "--xenos");
				if (!Utils.CheckTaskExist(selfExe, "--demulshooter")) Utils.RegisterTask(selfExe, "--demulshooter");
				if (!Utils.CheckTaskExist(selfExe, "--listvjoy")) Utils.RegisterUserTaskAtLogon(selfExe, "--listvjoy");
				return;
			}

			//Vjoy
			if (args.Length >= 2 && args.First() == "--runvjoy")
			{
				string gunOption = args[2];
				bool enableGunA = false;
				bool enableGunB = false;

				if (gunOption == "gunA") enableGunA = true;
				if (gunOption == "gunB") enableGunB = true;
				if (gunOption == "all")
				{
					enableGunA = true;
					enableGunB = true;
				}

				string formula_X = "";
				string formula_Y = "";
				string gunAMinMax = "";
				string gunBMinMax = "";

				string formula_AX_before = "";
				string formula_AY_before = "";
				string formula_BX_before = "";
				string formula_BY_before = "";


				int vjoyindexvalue = 0;
				try
				{
					int.TryParse(args[1], out vjoyindexvalue);
				}
				catch { vjoyindexvalue = 0; }

				if (args.Length == 12)
				{
					formula_X = args[3];
					formula_Y = args[4];
					gunAMinMax = args[5];
					gunBMinMax = args[6];
					formula_AX_before = args[7];
					formula_AY_before = args[8];
					formula_BX_before = args[9];
					formula_BY_before = args[10];
				}

				ConfigurationManager.LoadConfig();
				string xmlFile = args.Last();
				if (xmlFile.ToLower().EndsWith(".xml") && File.Exists(xmlFile))
				{
					Application.SetHighDpiMode(HighDpiMode.DpiUnaware);
					string baseTpDir = Directory.GetParent(Path.GetDirectoryName(Path.GetFullPath(xmlFile))).FullName;
					string originalConfigFileName = Path.GetFileName(xmlFile);
					string originalConfigFileNameWithoutExt = Path.GetFileNameWithoutExtension(xmlFile);
					GameSettings gameOptions = new GameSettings();
					string optionFile = Path.Combine(GameOptionsFolder, originalConfigFileNameWithoutExt + ".json");
					if (File.Exists(optionFile))
					{
						gameOptions = new GameSettings(Utils.ReadAllText(optionFile));
						var frm = new VjoyControl(false, originalConfigFileNameWithoutExt, gameOptions, enableGunA, enableGunB, formula_X, formula_Y, gunAMinMax, gunBMinMax, formula_AX_before, formula_AY_before, formula_BX_before, formula_BY_before, vjoyindexvalue); ;
						Application.Run(frm);
					}
					else
					{
						var frm = new VjoyControl(false, originalConfigFileNameWithoutExt, null, enableGunA, enableGunB, formula_X, formula_Y, gunAMinMax, gunBMinMax, formula_AX_before, formula_AY_before, formula_BX_before, formula_BY_before, vjoyindexvalue);
						Application.Run(frm);
					}
				}
				else
				{
					var frm = new VjoyControl(false);
					Application.Run(frm);
				}
				return;
			}

			joysticksIds.Clear();
			InitJoyList();

			string runtimedir = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "runtimes");
			if (Directory.Exists(runtimedir))
			{
				var oldsdl2 = Directory.GetFiles(runtimedir, "*sdl2*", SearchOption.AllDirectories);
				foreach (var oldsdl in oldsdl2) { File.Delete(oldsdl); }
				//Directory.Delete(runtimedir, true);
			}


			Application.ApplicationExit += new EventHandler(OnApplicationExit);

			string markerFilePath = Path.Combine(Path.GetTempPath(), "list_vjoy.json");
			if (File.Exists(markerFilePath))
			{
				vjoyData = JsonConvert.DeserializeObject<Dictionary<int, string>>(Utils.ReadAllText(markerFilePath));
			}

			wheelXinputData = ConfigurationManager.MainConfig.wheelXinputData;
			arcadeXinputData = ConfigurationManager.MainConfig.arcadeXinputData;
			gamepadXinputData = ConfigurationManager.MainConfig.gamepadXinputData;

			DemulshooterManager.SetPath(ConfigurationManager.MainConfig.demulshooterExe);

			if (args.Length == 0)
			{
				ApplicationConfiguration.Initialize();

				string TeknoparrotAutoXinputConfigFile = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "TeknoparrotAutoXinput.json");
				if (!File.Exists(TeknoparrotAutoXinputConfigFile)) Application.Run(new Wizard());
				else Application.Run(new MainDPIcs());
			}
			if (args.Length > 0)
			{

				DebugMode = ConfigurationManager.MainConfig.debugMode;
				if (DebugMode)
				{
					// Allouer une nouvelle console
					AllocConsole();
					RedirectConsoleOutput();
					if (File.Exists("debug.log.txt")) File.Delete("debug.log.txt");

					Utils.LogMessage("Starting in DebugMode");
					Utils.LogMessage("Exe : " + Process.GetCurrentProcess().MainModule.FileName);
					Utils.LogMessage("Args : " + Utils.ArgsToCommandLine(args));

				}


				bool changeFFBConfig = ConfigurationManager.MainConfig.FFB;
				bool showStartup = ConfigurationManager.MainConfig.showStartup;

				bool useVirtualKeyboard = ConfigurationManager.MainConfig.virtualKeyboard;
				string keyTest = ConfigurationManager.MainConfig.keyTest;
				string keyService1 = ConfigurationManager.MainConfig.keyService1;
				string keyService2 = ConfigurationManager.MainConfig.keyService2;

				bool favorAB = ConfigurationManager.MainConfig.favorAB;

				bool gamepadStooz = ConfigurationManager.MainConfig.gamepadStooz;
				bool wheelStooz = ConfigurationManager.MainConfig.wheelStooz;
				bool hotasStooz = ConfigurationManager.MainConfig.hotasStooz;
				bool enableStoozZone_Gamepad = ConfigurationManager.MainConfig.enableStoozZone_Gamepad;
				int valueStooz_Gamepad = ConfigurationManager.MainConfig.valueStooz_Gamepad;
				bool enableStoozZone_Wheel = ConfigurationManager.MainConfig.enableStoozZone_Wheel;
				int valueStooz_Wheel = ConfigurationManager.MainConfig.valueStooz_Wheel;
				bool enableStoozZone_Hotas = ConfigurationManager.MainConfig.enableStoozZone_Hotas;
				int valueStooz_Hotas = ConfigurationManager.MainConfig.valueStooz_Hotas;

				bool useXenos = ConfigurationManager.MainConfig.useXenos;

				WheelFFBGuid = ConfigurationManager.MainConfig.ffbDinputWheel;
				HotasFFBGuid = ConfigurationManager.MainConfig.ffbDinputHotas;

				bool passthrough = false;
				bool fullpassthrough = false;


				Utils.LogMessage("Initial values : ");
				Utils.LogMessage($"changeFFBConfig = {changeFFBConfig}");
				Utils.LogMessage($"showStartup = {changeFFBConfig}");
				Utils.LogMessage($"useVirtualKeyboard = {useVirtualKeyboard}");
				Utils.LogMessage($"keyTest = {keyTest}");
				Utils.LogMessage($"keyService1 = {keyService1}");
				Utils.LogMessage($"keyService2 = {keyService2}");
				Utils.LogMessage($"favorAB = {favorAB}");
				Utils.LogMessage($"gamepadStooz = {gamepadStooz}");
				Utils.LogMessage($"wheelStooz = {wheelStooz}");
				Utils.LogMessage($"enableStoozZone_Gamepad = {enableStoozZone_Gamepad}");
				Utils.LogMessage($"enableStoozZone_Wheel = {enableStoozZone_Wheel}");
				Utils.LogMessage($"enableStoozZone_Hotas = {enableStoozZone_Hotas}");
				Utils.LogMessage($"valueStooz_Gamepad = {valueStooz_Gamepad}");
				Utils.LogMessage($"enableStoozZone_Wheel = {enableStoozZone_Wheel}");
				Utils.LogMessage($"valueStooz_Wheel = {valueStooz_Wheel}");
				Utils.LogMessage($"enableStoozZone_Hotas = {enableStoozZone_Hotas}");
				Utils.LogMessage($"valueStooz_Hotas = {valueStooz_Hotas}");
				Utils.LogMessage($"WheelFFBGuid = {WheelFFBGuid}");
				Utils.LogMessage($"WheelFFBGuid = {HotasFFBGuid}");


				List<string> typeConfig = new List<string>();
				typeConfig.Add("gamepad");
				typeConfig.Add("gamepadalt");
				typeConfig.Add("arcade");
				typeConfig.Add("wheel");
				typeConfig.Add("hotas");
				typeConfig.Add("lightgun");

				int forced_displayMode = 0;
				int forced_resolution = 0;
				int forced_reshade = 0;
				bool nolink = false;

				/*
				{
					string processName = "TeknoParrotUI";
					Process targetProcess = Process.GetProcessesByName(processName).FirstOrDefault();

					if (targetProcess != null)
					{
						Process TPAutoXinputProcess = null;
						var tparent = ParentProcessUtilities.GetParentProcess(targetProcess.Id);
						if(tparent != null && tparent.ProcessName.ToLower().Contains("teknoparrotautoxinput"))
						{
							TPAutoXinputProcess = tparent;
						}

						// Liste pour stocker les processus fils
						List<Process> childProcesses = new List<Process>();

						// Obtenir tous les processus en cours
						foreach (Process process in Process.GetProcesses())
						{
							try
							{
								// Ajouter le processus � la liste s'il est un fils du processus cible
								if (ParentProcessUtilities.GetParentProcess(process.Id).Id == targetProcess.Id)
								{
									childProcesses.Add(process);
								}
							}
							catch
							{
								// Ignorer les exceptions pour les processus inaccessibles
							}
						}



						foreach(var p in childProcesses)
						{
							p.Kill();
						}
						targetProcess.Kill();
						Thread.Sleep(3000);
						for (int i = 0; i < 15; i++)
						{
							if (TPAutoXinputProcess != null && !TPAutoXinputProcess.HasExited)
							{
								break;
								Thread.Sleep(1000);
							}
						}
						Thread.Sleep(500);



					}
				}
				*/

				Dictionary<string, string> existingConfig = new Dictionary<string, string>();
				if (args.Length > 0)
				{

					try
					{
						int result = SetProcessDpiAwareness(ProcessDpiAwareness.PerMonitorDpiAware);
					}
					catch (Exception ex)
					{
					}
					/*
					string xmlFile1 = args.Last();
					Startup.tpBasePath = Directory.GetParent(Path.GetDirectoryName(Path.GetFullPath(xmlFile1))).FullName;
					Startup.gameTitle = "Ceci est un tres tres tres long test car je test le maximum POSSIBLE pour le titre et voila voila c'est bon je pense";
					Startup.gameTitle = "Akuma : The Game";
					Startup.xmlFile = xmlFile1;
					Startup.logoPath = Path.Combine(Directory.GetParent(Path.GetDirectoryName(Path.GetFullPath(xmlFile1))).FullName, "Icons", Path.GetFileNameWithoutExtension(xmlFile1) + ".png");
					Startup.playerAttributionDesc = "";
					Startup.imagePaths = new List<string>();
					Startup.playerAttributionDesc += $"P1=GAMEPAD\n";
					var imgPath2 = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "img", Path.GetFileNameWithoutExtension(xmlFile1) + "." + "gamepad" + ".jpg");
					Startup.imagePaths.Add(imgPath2);
					Startup.imagePaths.Add(imgPath2);
					Startup.imagePaths.Add(imgPath2);
					Startup.playerAttributionDesc.TrimEnd('\n');
					startupForm = new Startup();
					Application.Run(startupForm);

					Thread.Sleep(2000);
					return;
					*/

					using (Mutex mutex = new Mutex(false, mutexName, out bool createdNew))
					{

						if (!createdNew)
						{
							Utils.LogMessage("An other instance is running");
							return;
						}
						try
						{
							forceTypeController = new Dictionary<int, string>();
							foreach (string arg in args)
							{
								Match match = Regex.Match(arg, @"--forceslot([0-3])=(arcade|wheel|gamepad)");
								if (match.Success)
								{
									int slotNumber = int.Parse(match.Groups[1].Value);
									string deviceType = match.Groups[2].Value.ToLower().Trim();
									forceTypeController.Add(slotNumber, deviceType);
									Utils.LogMessage($"CMDLINE Option : forceslot = {slotNumber} : {deviceType}");
								}

								if (arg.ToLower().Trim() == "--passthrough")
								{
									passthrough = true;
									Utils.LogMessage($"CMDLINE Option : passthrough = {passthrough}");
								}

								if (arg.ToLower().Trim() == "--fullpassthrough")
								{
									passthrough = true;
									fullpassthrough = true;
									Utils.LogMessage($"CMDLINE Option : fullpassthrough = {fullpassthrough}");
								}

								match = Regex.Match(arg, @"--disposition=[0-9a-zA-Z_]");
								if (match.Success)
								{
									string folderDisposition = match.Groups[1].Value.Trim();
									string newDispositionFolder = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), folderDisposition);
									Utils.LogMessage($"CMDLINE Option : newDispositionFolder = {newDispositionFolder}");

									if (Directory.Exists(newDispositionFolder))
									{
										DispositionFolder = newDispositionFolder;
									}
									else
									{
										Utils.LogMessage($"CMDLINE Option : Disposition folder invalid");
									}
								}

								match = Regex.Match(arg, @"--displayMode=([0-9]+)");
								if (match.Success)
								{
									forced_displayMode = int.Parse(match.Groups[1].Value);
								}

								match = Regex.Match(arg, @"--resolution=([0-9]+)");
								if (match.Success)
								{
									forced_resolution = int.Parse(match.Groups[1].Value);
								}

								match = Regex.Match(arg, @"--reshade=([0-9]+)");
								if (match.Success)
								{
									forced_reshade = int.Parse(match.Groups[1].Value);
								}

								if (arg.ToLower().Trim() == "--nolink")
								{
									nolink = true;
									Utils.LogMessage($"CMDLINE Option : nolink = {nolink}");
								}
								if (arg.ToLower().Trim() == "--testmode")
								{
									testMode = true;
									Utils.LogMessage($"CMDLINE Option : testmode = {testMode}");
								}


							}


							string finalConfig = "";
							string basePath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
							string customConfigPath = "";
							Dictionary<string, string> shifterData = new Dictionary<string, string>();
							Dictionary<string, string> throttleData = new Dictionary<string, string>();


							string xmlFile = args.Last();
							Utils.LogMessage($"basePath : {basePath}");
							Utils.LogMessage($"xmlFile : {xmlFile}");


							if (!xmlFile.ToLower().EndsWith(".xml") || !File.Exists(xmlFile))
							{
								Utils.LogMessage($"Invalid XML File");
								MessageBox.Show("Invalid parameters");
								return;
							}




							string baseTpDir = Directory.GetParent(Path.GetDirectoryName(Path.GetFullPath(xmlFile))).FullName;
							string originalConfigFileName = Path.GetFileName(xmlFile);
							string originalConfigFileNameWithoutExt = Path.GetFileNameWithoutExtension(xmlFile);
							string teknoparrotExe = Path.Combine(baseTpDir, "TeknoParrotUi.exe");

							GameSettings gameOptions = new GameSettings();
							string optionFile = Path.Combine(GameOptionsFolder, originalConfigFileNameWithoutExt + ".json");
							if (File.Exists(optionFile))
							{
								Utils.LogMessage($"gameoveride file found : " + optionFile);
								gameOptions = new GameSettings(Utils.ReadAllText(optionFile));
							}

							if (fullpassthrough)
							{
								gameOptions.useMagpie = 2;
								gameOptions.runRivaTuner = false;
								useXenos = false;
							}

							if (!gameOptions.overrideTpController) passthrough = true;
							if (!gameOptions.overrideTpGameSettings) fullpassthrough = true;
							if (!gameOptions.useThirdParty)
							{
								gameOptions.useMagpie = 2;
								gameOptions.runRivaTuner = false;
								useXenos = false;
								gameOptions.AhkBefore = "";
								gameOptions.AhkAfter = "";
								gameOptions.gunA_recoil = 1;
								gameOptions.gunB_recoil = 1;
								gameOptions.gunA_useVjoy = 1;
								gameOptions.gunB_useVjoy = 1;
							}
							if (!gameOptions.applyPatches)
							{
								gameOptions.EnableLink = false;
								gameOptions.EnableLinkExe = false;
							}

							patchGpuFix = gameOptions.patchGpuFix > 0 ? (gameOptions.patchGpuFix == 1 ? true : false) : patchGpuFix;
							patchGpuTP = gameOptions.patchGpuTP > 0 ? (gameOptions.patchGpuTP == 1 ? true : false) : patchGpuTP;

							gpuType = gameOptions.gpuType > 0 ? (gameOptions.gpuType - 1) : gpuType;
							gpuResolution = gameOptions.gpuResolution > 0 ? (gameOptions.gpuResolution - 1) : gpuResolution;
							patchResolutionFix = gameOptions.patchResolutionFix > 0 ? (gameOptions.patchResolutionFix == 1 ? true : false) : patchResolutionFix;
							patchResolutionTP = gameOptions.patchResolutionTP > 0 ? (gameOptions.patchResolutionTP == 1 ? true : false) : patchResolutionTP;

							displayMode = gameOptions.displayMode > 0 ? (gameOptions.displayMode - 1) : displayMode;
							patchDisplayModeFix = gameOptions.patchDisplayModeFix > 0 ? (gameOptions.patchDisplayModeFix == 1 ? true : false) : patchDisplayModeFix;
							patchDisplayModeTP = gameOptions.patchDisplayModeTP > 0 ? (gameOptions.patchDisplayModeTP == 1 ? true : false) : patchDisplayModeTP;

							patchReshade = gameOptions.patchReshade > 0 ? (gameOptions.patchReshade == 1 ? true : false) : patchReshade;
							patchGameID = gameOptions.patchGameID > 0 ? (gameOptions.patchGameID == 1 ? true : false) : patchGameID;
							patchFFB = gameOptions.patchFFB > 0 ? (gameOptions.patchFFB == 1 ? true : false) : patchFFB;
							patchNetwork = gameOptions.patchNetwork > 0 ? (gameOptions.patchNetwork == 1 ? true : false) : patchNetwork;
							patchOtherTPSettings = gameOptions.patchOtherTPSettings > 0 ? (gameOptions.patchOtherTPSettings == 1 ? true : false) : patchOtherTPSettings;
							if (gameOptions.patchOthersGameOptions > 0)
							{
								if (gameOptions.patchOthersGameOptions == 1)
								{
									patchOtherTPSettings = true;
								}
								if (gameOptions.patchOthersGameOptions == 2)
								{
									patchOtherTPSettings = true;
								}
								if (gameOptions.patchOthersGameOptions == 3)
								{
									patchOtherTPSettings = false;
								}
							}

							performanceProfile = gameOptions.performanceProfile > 0 ? (gameOptions.performanceProfile - 1) : performanceProfile;
							forceVsync = gameOptions.forceVsync > 0 ? (gameOptions.patchDisplayModeFix == 1 ? true : false) : forceVsync;

							useBezel = gameOptions.useBezel > 0 ? (gameOptions.useBezel == 1 ? true : false) : useBezel;
							useCrt = gameOptions.useCrt > 0 ? (gameOptions.useCrt == 1 ? true : false) : useCrt;
							useKeepAspectRatio = gameOptions.keepAspectRatio > 0 ? (gameOptions.keepAspectRatio == 1 ? true : false) : useKeepAspectRatio;
							patchLang = gameOptions.patchLang > 0 ? (gameOptions.patchLang - 1) : patchLang;

							if (!patchReshade)
							{
								useBezel = false;
								useCrt = false;
								useKeepAspectRatio = false;
							}

							//if (performanceProfile == 1) useCrt = false;

							if (patchNetwork && ConfigurationManager.MainConfig.patch_networkAuto)
							{
								var networkInfo = Utils.GetFirstNetworkAdapterInfo();
								patch_networkIP = networkInfo.ContainsKey("networkIP") ? networkInfo["networkIP"] : "0.0.0.0";
								patch_networkMask = networkInfo.ContainsKey("networkMask") ? networkInfo["networkMask"] : "0.0.0.0";
								patch_networkGateway = networkInfo.ContainsKey("networkGateway") ? networkInfo["networkGateway"] : "0.0.0.0";
								patch_networkDns1 = networkInfo.ContainsKey("networkDns1") ? networkInfo["networkDns1"] : "0.0.0.0";
								patch_networkDns2 = networkInfo.ContainsKey("networkDns2") ? networkInfo["networkDns2"] : "0.0.0.0";
								patch_BroadcastAddress = networkInfo.ContainsKey("BroadcastAddress") ? networkInfo["BroadcastAddress"] : "0.0.0.0";
							}

							displayMode = forced_displayMode > 0 ? (forced_displayMode - 1) : displayMode;
							gpuResolution = forced_resolution > 0 ? (forced_resolution - 1) : gpuResolution;
							patchReshade = forced_reshade > 0 ? (forced_reshade == 1 ? true : false) : patchReshade;

							if (gpuResolution == 4)
							{
								patchResolutionFix = false;
								patchResolutionTP = false;
							}

							string baseTpDirOriginal = baseTpDir;

							bool tpDirChange = false;
							if (!string.IsNullOrEmpty(gameOptions.CustomTpExe) && File.Exists(gameOptions.CustomTpExe))
							{
								tpDirChange = true;
								teknoparrotExe = Path.GetFullPath(gameOptions.CustomTpExe);
								baseTpDir = Path.GetDirectoryName(teknoparrotExe);
							}

							if (tpDirChange)
							{
								string newXmlFile = Path.Combine(baseTpDir, "UserProfiles", originalConfigFileName);
							}

							TpSettingsManager.setOriginalXML(xmlFile);

							int tp_version = 100000;
							try
							{
								if (File.Exists(teknoparrotExe))
								{
									FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(teknoparrotExe);
									if (!string.IsNullOrEmpty(fileVersionInfo.FileVersion))
									{
										int lastIndex = fileVersionInfo.FileVersion.LastIndexOf('.');
										if (lastIndex != -1)
										{
											if (int.TryParse(fileVersionInfo.FileVersion.Substring(lastIndex + 1), out int lastNumber))
											{
												tp_version = lastNumber;
											}
										}
									}
								}
							}
							catch { }
							Utils.LogMessage($"TP version = {tp_version}");

							string potentialAltConfigDir = "";
							if (Directory.Exists(Path.Combine(basePath, "config")))
							{
								// Obtenez tous les sous-r�pertoires
								string[] directories = Directory.GetDirectories(Path.Combine(basePath, "config"));

								// Cr�ez une liste pour les r�pertoires dont le nom est un chiffre et <= max_productid
								List<string> numericDirectories = new List<string>();

								foreach (string directory in directories)
								{
									DirectoryInfo dirInfo = new DirectoryInfo(directory);
									// V�rifiez si le nom du r�pertoire est un chiffre
									if (dirInfo.Name.All(char.IsDigit))
									{
										// Convertissez le nom du r�pertoire en entier
										int directoryNumber = int.Parse(dirInfo.Name);

										// V�rifiez si le num�ro du r�pertoire est inf�rieur ou �gal � max_productid
										if (directoryNumber >= tp_version)
										{
											// Ajoutez le chemin du r�pertoire � la liste
											numericDirectories.Add(directory);
										}
									}
								}

								// Triez la liste dans l'ordre d�croissant
								numericDirectories.Sort((x, y) => y.CompareTo(x));

								// Affichez les r�pertoires tri�s
								Console.WriteLine("R�pertoires dont le nom est un chiffre et <= " + tp_version + ", tri�s dans l'ordre d�croissant:");
								foreach (var directory in numericDirectories)
								{
									if (File.Exists(Path.Combine(directory, originalConfigFileNameWithoutExt + ".gamepad.txt")) || File.Exists(Path.Combine(directory, originalConfigFileNameWithoutExt + ".lightgun.txt")))
									{
										potentialAltConfigDir = directory;
										break;
									}
								}
							}
							Utils.LogMessage($"Alt Config dir = {potentialAltConfigDir}");

							string gameInfoContent = "";
							JObject gameInfoParsedJson = null;
							JObject gameInfoGOSection = null;
							JObject gameInfoTpSection = null;
							JObject gameInfoGlobalSection = null;
							GameInfo = new Dictionary<string, string>();
							string gameInfoFile = Path.Combine(basePath, "config", originalConfigFileNameWithoutExt + "." + "info" + ".json");
							if (potentialAltConfigDir != "")
							{
								var gameInfoFileAlt = Path.Combine(potentialAltConfigDir, originalConfigFileNameWithoutExt + "." + "info" + ".json");
								if (File.Exists(gameInfoFileAlt)) gameInfoFile = gameInfoFileAlt;
							}
							if (File.Exists(gameInfoFile) && !fullpassthrough)
							{
								Utils.LogMessage($"info file found : " + gameInfoFile);
								gameInfoContent = Utils.ReadAllText(gameInfoFile);
								gameInfoParsedJson = JObject.Parse(gameInfoContent);
								gameInfoGlobalSection = (JObject)gameInfoParsedJson["global"];
								GameInfo = gameInfoGlobalSection.ToObject<Dictionary<string, string>>();
								gameInfoGOSection = (JObject)gameInfoParsedJson["gameoptions"];
								//gameOptions.Overwrite(gameInfoGOSection, TpSettingsManager.tags);
								gameInfoTpSection = (JObject)gameInfoParsedJson["tpoptions"];
							}

							if (performanceProfile == 1 && GameInfo.ContainsKey("disableCrtForLowPerf") && GameInfo["disableCrtForLowPerf"].ToLower() == "true") useCrt = false;


							bool refreshrate_done = false;
							_dispositionToSwitch = ConfigurationManager.MainConfig.Disposition;
							if (gameOptions.UseGlobalDisposition == false) _dispositionToSwitch = gameOptions.Disposition;

							if (!string.IsNullOrEmpty(_dispositionToSwitch))
							{
								if (_dispositionToSwitch == "MainMonitor:720p 60hz")
								{
									usePresetDisposition = true;
									presetDispositionWith = 1280;
									presetDispositionHeight = 720;
									presetDispositionFrequency = 60;
								}
								if (_dispositionToSwitch == "MainMonitor:1080p 60hz")
								{
									usePresetDisposition = true;
									presetDispositionWith = 1920;
									presetDispositionHeight = 1080;
									presetDispositionFrequency = 60;
								}
								if (_dispositionToSwitch == "MainMonitor:2k 60hz")
								{
									usePresetDisposition = true;
									presetDispositionWith = 2560;
									presetDispositionHeight = 1440;
									presetDispositionFrequency = 60;
								}
								if (_dispositionToSwitch == "MainMonitor:4k 60hz")
								{
									usePresetDisposition = true;
									presetDispositionWith = 3840;
									presetDispositionHeight = 2160;
									presetDispositionFrequency = 60;
								}
								if (_dispositionToSwitch == "MainMonitor:720p 120hz")
								{
									usePresetDisposition = true;
									presetDispositionWith = 1280;
									presetDispositionHeight = 720;
									presetDispositionFrequency = 120;
								}
								if (_dispositionToSwitch == "MainMonitor:1080p 120hz")
								{
									usePresetDisposition = true;
									presetDispositionWith = 1920;
									presetDispositionHeight = 1080;
									presetDispositionFrequency = 120;
								}
								if (_dispositionToSwitch == "MainMonitor:2k 120hz")
								{
									usePresetDisposition = true;
									presetDispositionWith = 2560;
									presetDispositionHeight = 1440;
									presetDispositionFrequency = 120;
								}
								if (_dispositionToSwitch == "MainMonitor:4k 120hz")
								{
									usePresetDisposition = true;
									presetDispositionWith = 3840;
									presetDispositionHeight = 2160;
									presetDispositionFrequency = 120;
								}
								if (usePresetDisposition)
								{
									var devices = Westwind.SetResolution.DisplayManager.GetAllDisplayDevices();
									var monitor = devices.FirstOrDefault(d => d.IsSelected);  // main monitor
									var list = Westwind.SetResolution.DisplayManager.GetAllDisplaySettings(monitor.DriverDeviceName);

									var resolutionAvailiable = list
										.Where(d => d.BitCount == 32 && d.Orientation == 0)
										.OrderBy(d => d.Width)
										.ThenBy(d => d.Height)
										.ThenBy(d => d.Frequency)
										.FirstOrDefault(d => d.Width == presetDispositionWith &&
															 d.Height == presetDispositionHeight &&
															 d.Frequency == presetDispositionFrequency);


									if (resolutionAvailiable == null)
									{
										//Resolution juste en dessous mais sans decendre en dessous de 1, par exemple 59Hz au lieu de 60hz
										resolutionAvailiable = list
										.Where(d => d.Width == presetDispositionWith &&
													d.Height == presetDispositionHeight &&
													d.BitCount == 32 &&
														d.Orientation == 0 &&
														d.Frequency < presetDispositionFrequency &&
														d.Frequency >= presetDispositionFrequency - 1)
										.OrderByDescending(d => d.Frequency)
										.FirstOrDefault();
									}
									if (resolutionAvailiable == null)
									{
										// Chercher la r�solution imm�diatement au-dessus de 60 Hz
										resolutionAvailiable = list
											.Where(d => d.Width == presetDispositionWith &&
														d.Height == presetDispositionHeight &&
														d.BitCount == 32 &&
														d.Orientation == 0 &&
														d.Frequency > presetDispositionFrequency)
											.OrderBy(d => d.Frequency)
											.FirstOrDefault();
									}
									// Si aucune r�solution n'est trouv�e au-dessus, chercher la r�solution imm�diatement en dessous de 60 Hz
									if (resolutionAvailiable == null)
									{
										resolutionAvailiable = list
											.Where(d => d.Width == presetDispositionWith &&
														d.Height == presetDispositionHeight &&
														d.BitCount == 32 &&
														d.Orientation == 0 &&
														d.Frequency < presetDispositionFrequency)
											.OrderByDescending(d => d.Frequency)
											.FirstOrDefault();
									}
									if (resolutionAvailiable != null)
									{
										presetDispositionFrequency = resolutionAvailiable.Frequency;
									}
									else presetDispositionFrequency = 60;


									Program.refreshRate = presetDispositionFrequency;
									refreshrate_done = true;
								}
							}

							if (!string.IsNullOrEmpty(_dispositionToSwitch) && _dispositionToSwitch != "<none>" && usePresetDisposition == false)
							{
								var cfg = Path.Combine(Program.DispositionFolder, "disposition_" + _dispositionToSwitch + ".xml");
								if (File.Exists(cfg))
								{
									try
									{
										Program.refreshRate = Utils.GetPrimaryMonitorRefreshRateFromXml(Utils.ReadAllText(cfg));
										refreshrate_done = true;
										if (gameOptions.moveBackWindowToOriginalMonitor)
										{
											Program.moveWindowToOriginalMonitor = true;
											originalMonitorDeviceName = Screen.PrimaryScreen.DeviceName;
										}
									}
									catch { }
								}
							}
							if (!refreshrate_done)
							{
								try
								{
									CCDWrapper.DisplayConfigPathInfo[] pathInfoArray = new CCDWrapper.DisplayConfigPathInfo[0];
									CCDWrapper.DisplayConfigModeInfo[] modeInfoArray = new CCDWrapper.DisplayConfigModeInfo[0];
									CCDWrapper.MonitorAdditionalInfo[] additionalInfo = new CCDWrapper.MonitorAdditionalInfo[0];
									CCDWrapper.DpiInfo[] dpiInfoArray = new CCDWrapper.DpiInfo[0];
									Boolean status = MonitorSwitcher.GetDisplaySettings(ref pathInfoArray, ref modeInfoArray, ref additionalInfo, ref dpiInfoArray, true);
									if (status)
									{
										var monitorData = MonitorSwitcher.PrintDisplaySettings2ForFrequency(pathInfoArray, modeInfoArray, dpiInfoArray);
										Program.refreshRate = Utils.GetPrimaryMonitorRefreshRateFromXml(monitorData);
									}
								}
								catch { }
							}

							//Tag Define Part1
							TpSettingsManager.tags = new List<string>();
							if (Program.patchGpuTP)
							{
								if (Program.gpuType == 0) TpSettingsManager.tags.Add("nvidia");
								if (Program.gpuType == 1) TpSettingsManager.tags.Add("intel");
								if (Program.gpuType == 2) TpSettingsManager.tags.Add("amdold");
								if (Program.gpuType == 3) TpSettingsManager.tags.Add("amdnew");
								if (Program.gpuType == 4) TpSettingsManager.tags.Add("amdrid");
								if (Program.gpuType >= 2) TpSettingsManager.tags.Add("amd");

								if (Program.gpuType != 0) TpSettingsManager.tags.Add("no_nvidia");
								if (Program.gpuType != 1) TpSettingsManager.tags.Add("no_intel");
								if (Program.gpuType <= 1) TpSettingsManager.tags.Add("no_amd");

							}

							if (Program.performanceProfile == 0)
							{
								TpSettingsManager.tags.Add("normal_perfprofile");
								TpSettingsManager.tags.Add("not_low_perfprofile");
								TpSettingsManager.tags.Add("not_high_perfprofile");
							}
							if (Program.performanceProfile == 1)
							{
								TpSettingsManager.tags.Add("not_normal_perfprofile");
								TpSettingsManager.tags.Add("low_perfprofile");
								TpSettingsManager.tags.Add("not_high_perfprofile");
							}
							if (Program.performanceProfile == 2)
							{
								TpSettingsManager.tags.Add("not_normal_perfprofile");
								TpSettingsManager.tags.Add("not_low_perfprofile");
								TpSettingsManager.tags.Add("high_perfprofile");
							}


							if (Program.forceVsync) TpSettingsManager.tags.Add("vsync");
							else TpSettingsManager.tags.Add("no_vsync");

							if (Program.refreshRate >= 120) TpSettingsManager.tags.Add("120hz");
							else TpSettingsManager.tags.Add("60hz");

							if (Program.gpuResolution == 0) TpSettingsManager.tags.Add("720p");
							if (Program.gpuResolution == 1) TpSettingsManager.tags.Add("1080p");
							if (Program.gpuResolution == 2) TpSettingsManager.tags.Add("2k");
							if (Program.gpuResolution == 3) TpSettingsManager.tags.Add("4k");
							if (Program.gpuResolution == 4) TpSettingsManager.tags.Add("game_native_res");

							if (Program.gpuResolution != 4 && Program.gpuResolution >= 0) TpSettingsManager.tags.Add("720p+");
							if (Program.gpuResolution != 4 && Program.gpuResolution >= 1) TpSettingsManager.tags.Add("1080p+");
							if (Program.gpuResolution != 4 && Program.gpuResolution >= 2) TpSettingsManager.tags.Add("2k+");
							if (Program.gpuResolution != 4 && Program.gpuResolution >= 3) TpSettingsManager.tags.Add("4k+");

							if (Program.gpuResolution != 4 && Program.gpuResolution <= 0) TpSettingsManager.tags.Add("720p-");
							if (Program.gpuResolution != 4 && Program.gpuResolution <= 1) TpSettingsManager.tags.Add("1080p-");
							if (Program.gpuResolution != 4 && Program.gpuResolution <= 2) TpSettingsManager.tags.Add("2k-");
							if (Program.gpuResolution != 4 && Program.gpuResolution <= 3) TpSettingsManager.tags.Add("4k-");

							if (Program.gpuResolution != 0) TpSettingsManager.tags.Add("no_720p");
							if (Program.gpuResolution != 1) TpSettingsManager.tags.Add("no_1080p");
							if (Program.gpuResolution != 2) TpSettingsManager.tags.Add("no_2k");
							if (Program.gpuResolution != 3) TpSettingsManager.tags.Add("no_4k");
							if (Program.gpuResolution != 4) TpSettingsManager.tags.Add("no_game_native_res");

							if (Program.patchResolutionTP) TpSettingsManager.tags.Add("set_resolution");
							else TpSettingsManager.tags.Add("dont_set_resolution");



							if (Program.patchDisplayModeTP && Program.displayMode == 0) TpSettingsManager.tags.Add("set_displaymode_recommanded"); //Apply Res & Fullscreen in TP
							if (Program.patchDisplayModeTP && Program.displayMode == 1) TpSettingsManager.tags.Add("set_fullscreen"); //Apply Res & Fullscreen in TP
							if (Program.patchDisplayModeTP && Program.displayMode == 2) TpSettingsManager.tags.Add("set_windowed"); //Apply Res & Fullscreen in TP



							if (Program.patchReshade) TpSettingsManager.tags.Add("optional_reshade");
							else TpSettingsManager.tags.Add("no_reshade");



							if (Program.patchFFB) TpSettingsManager.tags.Add("ffb");

							if (Program.useBezel) TpSettingsManager.tags.Add("bezel");
							else TpSettingsManager.tags.Add("no_bezel");

							if (Program.useCrt) TpSettingsManager.tags.Add("crtfilter");
							else TpSettingsManager.tags.Add("no_crtfilter");

							if (Program.useKeepAspectRatio) TpSettingsManager.tags.Add("keepaspectratio");
							else TpSettingsManager.tags.Add("no_keepaspectratio");

							if (Program.patchLang == 0) TpSettingsManager.tags.Add("no_translation");
							else TpSettingsManager.tags.Add("translation");
							if (Program.patchLang == 1) TpSettingsManager.tags.Add("english");
							if (Program.patchLang == 2) TpSettingsManager.tags.Add("french");


							//Tags that does not have equivalents on the file system
							if (Program.patchGameID) TpSettingsManager.tags.Add("replace_gameid");
							if (Program.patchNetwork) TpSettingsManager.tags.Add("replace_network");
							if (Program.patchOtherTPSettings) TpSettingsManager.tags.Add("recommanded_tp_settings");
							if (Program.patchOthersGameOptions) TpSettingsManager.tags.Add("recommanded_gameoptions");
							if (Program.patchDisplayModeFix) TpSettingsManager.tags.Add("set_displaymode_in_patches");
							else TpSettingsManager.tags.Add("dont_set_displaymode_in_patches");
							if (Program.patchResolutionFix) TpSettingsManager.tags.Add("set_resolution_patches");
							else TpSettingsManager.tags.Add("dont_set_resolution_patches");

							if (GameInfo.Count > 0)
							{
								TpSettingsManager.SetSettings(gameInfoTpSection);
								if (GameInfo.ContainsKey("windowed"))
								{
									TpSettingsManager.windowed_search = GameInfo["windowed"];
								}
								//GameInfo = (Dictionary<string, string>)JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(gameInfoFile));
							}

							if (GameInfo.ContainsKey("canForceRegAsWindowed") && (GameInfo["canForceRegAsWindowed"].ToLower() == "true"))
							{
								if (TpSettingsManager.tags.Contains("set_windowed"))
								{
									TpSettingsManager.ForceRegAsWindowed = true;
								}
								if (TpSettingsManager.tags.Contains("set_displaymode_recommanded"))
								{
									var allSettings = gameInfoTpSection.ToObject<Dictionary<string, Dictionary<string, string>>>();
									if (allSettings.ContainsKey("set_displaymode_recommanded") && allSettings["set_displaymode_recommanded"].ContainsKey("General||RegAsWindowed") && allSettings["set_displaymode_recommanded"]["General||RegAsWindowed"] == "1")
									{
										TpSettingsManager.ForceRegAsWindowed = true;
									}
								}
							}

							if (GameInfo.ContainsKey("canForceRegAsFullscreen") && (GameInfo["canForceRegAsFullscreen"].ToLower() == "true"))
							{
								if (TpSettingsManager.tags.Contains("set_fullscreen"))
								{
									TpSettingsManager.ForceRegAsFullscreen = true;
								}
								if (TpSettingsManager.tags.Contains("set_displaymode_recommanded"))
								{
									var allSettings = gameInfoTpSection.ToObject<Dictionary<string, Dictionary<string, string>>>();
									if (allSettings.ContainsKey("set_displaymode_recommanded") && allSettings["set_displaymode_recommanded"].ContainsKey("General||RegAsWindowed") && allSettings["set_displaymode_recommanded"]["General||RegAsWindowed"] == "0")
									{
										TpSettingsManager.ForceRegAsFullscreen = true;
									}
								}
							}

							//Rustine pour avoir le link source folder avant d'update le xml
							TpSettingsManager.tempLinkSourceFolderExe = "";
							if (!string.IsNullOrEmpty(ConfigurationManager.MainConfig.perGameLinkFolderExe))
							{
								TpSettingsManager.tempLinkSourceFolderExe = Path.Combine(ConfigurationManager.MainConfig.perGameLinkFolderExe, originalConfigFileNameWithoutExt);
								if (gameOptions.CustomPerGameLinkFolder != null && gameOptions.CustomPerGameLinkFolder != "")
								{
									string lastFolder = Path.GetFileName(gameOptions.CustomPerGameLinkFolder);
									if (lastFolder == originalConfigFileNameWithoutExt)
									{
										TpSettingsManager.tempLinkSourceFolderExe = gameOptions.CustomPerGameLinkFolder;
									}
								}
							}

							TpSettingsManager.UpdateXML();
							if (TpSettingsManager.IsWindowed) TpSettingsManager.tags.Add("windowed");
							else TpSettingsManager.tags.Add("fullscreen");
							TpSettingsManager.UpdateXML();

							if (Program.DebugMode)
							{
								string taglist = "TagList = ";
								foreach (var tag in TpSettingsManager.tags)
								{
									taglist = taglist + tag + " ,";
								}
								Utils.LogMessage(taglist);
							}


							if (GameInfo.ContainsKey("upscaleFullscreen") && (GameInfo["upscaleFullscreen"].ToLower() == "true" || (GameInfo["upscaleFullscreen"].Trim() != "" && TpSettingsManager.tags.Contains(GameInfo["upscaleFullscreen"].Trim())))) Program.upscaleFullscreen = true;
							if (GameInfo.ContainsKey("upscaleWindowed") && (GameInfo["upscaleWindowed"].ToLower() == "true" || (GameInfo["upscaleWindowed"].Trim() != "" && TpSettingsManager.tags.Contains(GameInfo["upscaleWindowed"].Trim())))) Program.upscaleWindowed = true;
							if (GameInfo.ContainsKey("need60hz") && (GameInfo["need60hz"].ToLower() == "true" || (GameInfo["need60hz"].Trim() != "" && TpSettingsManager.tags.Contains(GameInfo["need60hz"].Trim())))) Program.need60hz = true;
							if (performanceProfile == 1 && GameInfo.ContainsKey("need60hzForLowPerf") && GameInfo["need60hzForLowPerf"].ToLower() == "true") Program.need60hz = true;
							if (GameInfo.ContainsKey("need60hzFullscreen") && (GameInfo["need60hzFullscreen"].ToLower() == "true" || (GameInfo["need60hzFullscreen"].Trim() != "" && TpSettingsManager.tags.Contains(GameInfo["need60hzFullscreen"].Trim())))) Program.need60hzFullscreen = true;
							if (GameInfo.ContainsKey("need60hzWindowed") && (GameInfo["need60hzWindowed"].ToLower() == "true" || (GameInfo["need60hzWindowed"].Trim() != "" && TpSettingsManager.tags.Contains(GameInfo["need60hzWindowed"].Trim())))) Program.need60hzWindowed = true;
							if (GameInfo.ContainsKey("forceResSwitchFullscreen") && (GameInfo["forceResSwitchFullscreen"].ToLower() == "true" || (GameInfo["forceResSwitchFullscreen"].Trim() != "" && TpSettingsManager.tags.Contains(GameInfo["forceResSwitchFullscreen"].Trim())))) Program.forceResSwitchFullscreen = true;
							if (GameInfo.ContainsKey("forceResSwitchWindowed") && (GameInfo["forceResSwitchWindowed"].ToLower() == "true" || (GameInfo["forceResSwitchWindowed"].Trim() != "" && TpSettingsManager.tags.Contains(GameInfo["forceResSwitchWindowed"].Trim())))) Program.forceResSwitchWindowed = true;

							if (GameInfo.ContainsKey("canUseJoyInsteadOfDpad") && GameInfo["canUseJoyInsteadOfDpad"].ToLower() == "true")
							{
								Program.favorJoystick = gameOptions.favorJoystick > 0 ? (gameOptions.favorJoystick == 1 ? true : false) : ConfigurationManager.MainConfig.favorJoystick;
							}
							if (Program.forceResSwitchFullscreen && Program.gpuResolution == 4) Program.forceResSwitchFullscreen = false;
							if (Program.forceResSwitchWindowed && Program.gpuResolution == 4) Program.forceResSwitchWindowed = false;



							if (gameInfoGOSection != null) gameOptions.Overwrite(gameInfoGOSection, TpSettingsManager.tags);

							string forceResizeCommand = "";
							if (GameInfo.ContainsKey("forceResize") && GameInfo["forceResize"] != "") forceResizeCommand = GameInfo["forceResize"];
							if (GameInfo.ContainsKey("forceResize720p") && GameInfo["forceResize720p"] != "" && Program.gpuResolution == 0) forceResizeCommand = GameInfo["forceResize720p"];
							if (GameInfo.ContainsKey("forceResize1080p") && GameInfo["forceResize1080p"] != "" && Program.gpuResolution == 1) forceResizeCommand = GameInfo["forceResize1080p"];
							if (GameInfo.ContainsKey("forceResize2k") && GameInfo["forceResize2k"] != "" && Program.gpuResolution == 2) forceResizeCommand = GameInfo["forceResize2k"];
							if (GameInfo.ContainsKey("forceResize4k") && GameInfo["forceResize4k"] != "" && Program.gpuResolution == 3) forceResizeCommand = GameInfo["forceResize4k"];
							if (GameInfo.ContainsKey("forceResizeNative") && GameInfo["forceResizeNative"] != "" && Program.gpuResolution == 4) forceResizeCommand = GameInfo["forceResizeNative"];
							var forceResizeCommands = forceResizeCommand.Split("||");
							foreach (var command in forceResizeCommands)
							{
								var matchForceResize = Regex.Match(command, @"^([0-9]+)x([0-9]+)(\|(.+))?$");
								if (matchForceResize.Success)
								{
									int forceResizeWidth = int.Parse(matchForceResize.Groups[1].Value);
									int forceResizeHeight = int.Parse(matchForceResize.Groups[2].Value);
									string forceResizeAfterBar = matchForceResize.Groups[4].Success ? matchForceResize.Groups[4].Value : string.Empty;

									if (forceResizeAfterBar != "")
									{
										bool validForceResize = true;
										foreach (var afterBarData in forceResizeAfterBar.Split("&"))
										{
											if (afterBarData.Trim() != "" && !TpSettingsManager.tags.Contains(afterBarData.Trim()))
											{
												validForceResize = false;
												break;
											}
										}

										if (!validForceResize)
										{
											forceResizeWidth = 0;
											forceResizeHeight = 0;
										}
									}

									// Appliquer la premi�re r�solution valide et sortir
									if (forceResizeWidth > 0 && forceResizeHeight > 0)
									{
										Program.forceResizeWidth = forceResizeWidth;
										Program.forceResizeHeight = forceResizeHeight;
										break;
									}
								}
							}
							/*
							var matchForceResize = Regex.Match(forceResizeCommand, @"^([0-9]+)x([0-9]+)(\|(.+))?$");
							if (matchForceResize.Success)
							{
								int forceResizeWidth = int.Parse(matchForceResize.Groups[1].Value);
								int forceResizeHeight = int.Parse(matchForceResize.Groups[2].Value);
								string forceResizeAfterBar = matchForceResize.Groups[4].Success ? matchForceResize.Groups[4].Value : string.Empty;
								if(forceResizeAfterBar != "")
								{
									bool validForceResize = true;
									foreach(var afterBarData in forceResizeAfterBar.Split("&"))
									{
										if(afterBarData.Trim() != "" && !TpSettingsManager.tags.Contains(afterBarData.Trim()))
										{
											validForceResize = false;
											break;
										}
									}
									if (!validForceResize)
									{
										forceResizeWidth = 0;
										forceResizeHeight = 0;
									}
								}
								Program.forceResizeWidth = forceResizeWidth;
								Program.forceResizeHeight = forceResizeHeight;
							}
							*/

							if (usePresetDisposition || string.IsNullOrEmpty(_dispositionToSwitch) || _dispositionToSwitch == "<none>")
							{

								if (!TpSettingsManager.IsWindowed && GameInfo.ContainsKey("forceResSwitchFullscreen") && GameInfo["forceResSwitchFullscreen"].ToLower().Split("x").Count() == 2)
								{
									var newResSplit = GameInfo["forceResSwitchFullscreen"].ToLower().Split("x");
									usePresetDisposition = true;
									presetDispositionWith = int.Parse(newResSplit[0].Trim());
									presetDispositionHeight = int.Parse(newResSplit[1].Trim());
									_dispositionToSwitch = "custom";
								}

								if (!TpSettingsManager.IsWindowed && Program.gpuResolution == 4 && GameInfo.ContainsKey("forceResSwitchNative") && GameInfo["forceResSwitchNative"].ToLower().Split("x").Count() == 2)
								{
									var newResSplit = GameInfo["forceResSwitchNative"].ToLower().Split("x");
									usePresetDisposition = true;
									presetDispositionWith = int.Parse(newResSplit[0].Trim());
									presetDispositionHeight = int.Parse(newResSplit[1].Trim());
									_dispositionToSwitch = "custom";
								}

								if (TpSettingsManager.IsWindowed && GameInfo.ContainsKey("forceResSwitchWindowed") && GameInfo["forceResSwitchWindowed"].ToLower().Split("x").Count() == 2)
								{
									var newResSplit = GameInfo["forceResSwitchWindowed"].ToLower().Split("x");
									usePresetDisposition = true;
									presetDispositionWith = int.Parse(newResSplit[0].Trim());
									presetDispositionHeight = int.Parse(newResSplit[1].Trim());
									_dispositionToSwitch = "custom";
								}

								if (TpSettingsManager.tags.Contains("windowed") && Program.need60hzWindowed) Program.need60hz = true;
								if (TpSettingsManager.tags.Contains("fullscreen") && Program.need60hzFullscreen) Program.need60hz = true;

								if (gameOptions.UseGlobalDisposition && Program.need60hz && Program.refreshRate != 60 && Program.currentResX > 0)
								{
									int target_refreshrate = 60;
									usePresetDisposition = true;
									presetDispositionWith = presetDispositionWith > 0 ? presetDispositionWith : Program.currentResX;
									presetDispositionHeight = presetDispositionHeight > 0 ? presetDispositionHeight : Program.currentResY;
									presetDispositionFrequency = target_refreshrate;

									var devices = Westwind.SetResolution.DisplayManager.GetAllDisplayDevices();
									var monitor = devices.FirstOrDefault(d => d.IsSelected);  // main monitor
									var list = Westwind.SetResolution.DisplayManager.GetAllDisplaySettings(monitor.DriverDeviceName);

									var resolutionAvailiable = list
										.Where(d => d.BitCount == 32 && d.Orientation == 0)
										.OrderBy(d => d.Width)
										.ThenBy(d => d.Height)
										.ThenBy(d => d.Frequency)
										.FirstOrDefault(d => d.Width == presetDispositionWith &&
															 d.Height == presetDispositionHeight &&
															 d.Frequency == presetDispositionFrequency);

									if (resolutionAvailiable == null)
									{
										//Resolution juste en dessous mais sans decendre en dessous de 1, par exemple 59Hz au lieu de 60hz
										resolutionAvailiable = list
										.Where(d => d.Width == presetDispositionWith &&
													d.Height == presetDispositionHeight &&
													d.BitCount == 32 &&
														d.Orientation == 0 &&
														d.Frequency < presetDispositionFrequency &&
														d.Frequency >= presetDispositionFrequency - 1)
										.OrderByDescending(d => d.Frequency)
										.FirstOrDefault();
									}
									if (resolutionAvailiable == null)
									{
										// Chercher la r�solution imm�diatement au-dessus de 60 Hz
										resolutionAvailiable = list
											.Where(d => d.Width == presetDispositionWith &&
														d.Height == presetDispositionHeight &&
														d.BitCount == 32 &&
														d.Orientation == 0 &&
														d.Frequency > presetDispositionFrequency)
											.OrderBy(d => d.Frequency)
											.FirstOrDefault();
									}
									// Si aucune r�solution n'est trouv�e au-dessus, chercher la r�solution imm�diatement en dessous de 60 Hz
									if (resolutionAvailiable == null)
									{
										resolutionAvailiable = list
											.Where(d => d.Width == presetDispositionWith &&
														d.Height == presetDispositionHeight &&
														d.BitCount == 32 &&
														d.Orientation == 0 &&
														d.Frequency < presetDispositionFrequency)
											.OrderByDescending(d => d.Frequency)
											.FirstOrDefault();
									}
									if (resolutionAvailiable != null)
									{
										_dispositionToSwitch = "custom";
										presetDispositionFrequency = resolutionAvailiable.Frequency;
									}
									else presetDispositionFrequency = 60;

									Utils.LogMessage($"Change mainRes to {presetDispositionWith}x{presetDispositionHeight}:{presetDispositionFrequency} (need60hz)");

									Program.refreshRate = presetDispositionFrequency;
									refreshrate_done = true;

								}

								if (TpSettingsManager.tags.Contains("fullscreen") && gameOptions.UseGlobalDisposition && Program.forceResSwitchFullscreen && Program.currentResX > 0)
								{
									int target_refreshrate = Program.refreshRate;
									if (need60hz) target_refreshrate = 60;
									int screenWidth = Program.currentResX;
									int screenHeight = Program.currentResY;
									bool changeres = false;
									if (Program.gpuResolution == 0 && (screenWidth != 1280 || screenHeight != 720))
									{
										usePresetDisposition = true;
										presetDispositionWith = 1280;
										presetDispositionHeight = 720;
										presetDispositionFrequency = target_refreshrate;
										changeres = true;
									}
									if (Program.gpuResolution == 1 && (screenWidth != 1920 || screenHeight != 1080))
									{
										usePresetDisposition = true;
										presetDispositionWith = 1920;
										presetDispositionHeight = 1080;
										presetDispositionFrequency = target_refreshrate;
										changeres = true;
									}
									if (Program.gpuResolution == 2 && (screenWidth != 2560 || screenHeight != 1440))
									{
										usePresetDisposition = true;
										presetDispositionWith = 2560;
										presetDispositionHeight = 1440;
										presetDispositionFrequency = target_refreshrate;
										changeres = true;
									}
									if (Program.gpuResolution == 3 && (screenWidth != 3840 || screenHeight != 2160))
									{
										usePresetDisposition = true;
										presetDispositionWith = 3840;
										presetDispositionHeight = 2160;
										presetDispositionFrequency = target_refreshrate;
										changeres = true;
									}
									if (changeres)
									{

										var devices = Westwind.SetResolution.DisplayManager.GetAllDisplayDevices();
										var monitor = devices.FirstOrDefault(d => d.IsSelected);  // main monitor
										var list = Westwind.SetResolution.DisplayManager.GetAllDisplaySettings(monitor.DriverDeviceName);

										var resolutionAvailiable = list
											.Where(d => d.BitCount == 32 && d.Orientation == 0)
											.OrderBy(d => d.Width)
											.ThenBy(d => d.Height)
											.ThenBy(d => d.Frequency)
											.FirstOrDefault(d => d.Width == presetDispositionWith &&
																 d.Height == presetDispositionHeight &&
																 d.Frequency == presetDispositionFrequency);

										if (resolutionAvailiable == null)
										{
											//Resolution juste en dessous mais sans decendre en dessous de 1, par exemple 59Hz au lieu de 60hz
											resolutionAvailiable = list
											.Where(d => d.Width == presetDispositionWith &&
														d.Height == presetDispositionHeight &&
														d.BitCount == 32 &&
															d.Orientation == 0 &&
															d.Frequency < presetDispositionFrequency &&
															d.Frequency >= presetDispositionFrequency - 1)
											.OrderByDescending(d => d.Frequency)
											.FirstOrDefault();
										}
										if (resolutionAvailiable == null)
										{
											// Chercher la r�solution imm�diatement au-dessus de 60 Hz
											resolutionAvailiable = list
												.Where(d => d.Width == presetDispositionWith &&
															d.Height == presetDispositionHeight &&
															d.BitCount == 32 &&
															d.Orientation == 0 &&
															d.Frequency > presetDispositionFrequency)
												.OrderBy(d => d.Frequency)
												.FirstOrDefault();
										}
										// Si aucune r�solution n'est trouv�e au-dessus, chercher la r�solution imm�diatement en dessous de 60 Hz
										if (resolutionAvailiable == null)
										{
											resolutionAvailiable = list
												.Where(d => d.Width == presetDispositionWith &&
															d.Height == presetDispositionHeight &&
															d.BitCount == 32 &&
															d.Orientation == 0 &&
															d.Frequency < presetDispositionFrequency)
												.OrderByDescending(d => d.Frequency)
												.FirstOrDefault();
										}
										if (resolutionAvailiable != null)
										{
											_dispositionToSwitch = "custom";
											presetDispositionFrequency = resolutionAvailiable.Frequency;
										}
										else presetDispositionFrequency = 60;

										Utils.LogMessage($"Change mainRes to {presetDispositionWith}x{presetDispositionHeight}:{presetDispositionFrequency} (forceResSwitchFullscreen)");

										Program.refreshRate = presetDispositionFrequency;
										refreshrate_done = true;
									}

								}

								if (TpSettingsManager.tags.Contains("windowed") && gameOptions.UseGlobalDisposition && Program.forceResSwitchWindowed && Program.currentResX > 0)
								{
									int target_refreshrate = Program.refreshRate;
									if (need60hz) target_refreshrate = 60;
									int screenWidth = Program.currentResX;
									int screenHeight = Program.currentResY;
									bool changeres = false;
									if (Program.gpuResolution == 0 && (screenWidth != 1280 || screenHeight != 720))
									{
										usePresetDisposition = true;
										presetDispositionWith = 1280;
										presetDispositionHeight = 720;
										presetDispositionFrequency = target_refreshrate;
										changeres = true;
									}
									if (Program.gpuResolution == 1 && (screenWidth != 1920 || screenHeight != 1080))
									{
										usePresetDisposition = true;
										presetDispositionWith = 1920;
										presetDispositionHeight = 1080;
										presetDispositionFrequency = target_refreshrate;
										changeres = true;
									}
									if (Program.gpuResolution == 2 && (screenWidth != 2560 || screenHeight != 1440))
									{
										usePresetDisposition = true;
										presetDispositionWith = 2560;
										presetDispositionHeight = 1440;
										presetDispositionFrequency = target_refreshrate;
										changeres = true;
									}
									if (Program.gpuResolution == 3 && (screenWidth != 3840 || screenHeight != 2160))
									{
										usePresetDisposition = true;
										presetDispositionWith = 3840;
										presetDispositionHeight = 2160;
										presetDispositionFrequency = target_refreshrate;
										changeres = true;
									}
									if (changeres)
									{

										var devices = Westwind.SetResolution.DisplayManager.GetAllDisplayDevices();
										var monitor = devices.FirstOrDefault(d => d.IsSelected);  // main monitor
										var list = Westwind.SetResolution.DisplayManager.GetAllDisplaySettings(monitor.DriverDeviceName);

										var resolutionAvailiable = list
											.Where(d => d.BitCount == 32 && d.Orientation == 0)
											.OrderBy(d => d.Width)
											.ThenBy(d => d.Height)
											.ThenBy(d => d.Frequency)
											.FirstOrDefault(d => d.Width == presetDispositionWith &&
																 d.Height == presetDispositionHeight &&
																 d.Frequency == presetDispositionFrequency);

										if (resolutionAvailiable == null)
										{
											//Resolution juste en dessous mais sans decendre en dessous de 1, par exemple 59Hz au lieu de 60hz
											resolutionAvailiable = list
											.Where(d => d.Width == presetDispositionWith &&
														d.Height == presetDispositionHeight &&
														d.BitCount == 32 &&
															d.Orientation == 0 &&
															d.Frequency < presetDispositionFrequency &&
															d.Frequency >= presetDispositionFrequency - 1)
											.OrderByDescending(d => d.Frequency)
											.FirstOrDefault();
										}
										if (resolutionAvailiable == null)
										{
											// Chercher la r�solution imm�diatement au-dessus de 60 Hz
											resolutionAvailiable = list
												.Where(d => d.Width == presetDispositionWith &&
															d.Height == presetDispositionHeight &&
															d.BitCount == 32 &&
															d.Orientation == 0 &&
															d.Frequency > presetDispositionFrequency)
												.OrderBy(d => d.Frequency)
												.FirstOrDefault();
										}
										// Si aucune r�solution n'est trouv�e au-dessus, chercher la r�solution imm�diatement en dessous de 60 Hz
										if (resolutionAvailiable == null)
										{
											resolutionAvailiable = list
												.Where(d => d.Width == presetDispositionWith &&
															d.Height == presetDispositionHeight &&
															d.BitCount == 32 &&
															d.Orientation == 0 &&
															d.Frequency < presetDispositionFrequency)
												.OrderByDescending(d => d.Frequency)
												.FirstOrDefault();
										}
										if (resolutionAvailiable != null)
										{
											_dispositionToSwitch = "custom";
											presetDispositionFrequency = resolutionAvailiable.Frequency;
										}
										else presetDispositionFrequency = 60;

										Utils.LogMessage($"Change mainRes to {presetDispositionWith}x{presetDispositionHeight}:{presetDispositionFrequency} (forceResSwitchFullscreen)");

										Program.refreshRate = presetDispositionFrequency;
										refreshrate_done = true;
									}

								}

								if (TpSettingsManager.tags.Contains("windowed") && gameOptions.UseGlobalDisposition && Program.upscaleWindowed && Program.currentResX > 0)
								{
									int target_refreshrate = Program.refreshRate;
									if (need60hz) target_refreshrate = 60;
									int screenWidth = Program.currentResX;
									int screenHeight = Program.currentResY;
									//MessageBox.Show($"screenWidth = {screenWidth} screenHeight= {screenHeight}");
									//if (Program.gpuResolution != 0) TpSettingsManager.tags.Add("!720p");
									//if (Program.gpuResolution != 1) TpSettingsManager.tags.Add("!1080p");
									//if (Program.gpuResolution != 2) TpSettingsManager.tags.Add("!2k");
									//if (Program.gpuResolution != 3) TpSettingsManager.tags.Add("!4k");
									bool changeres = false;
									if (Program.gpuResolution == 0 && (screenWidth < 1280 || screenHeight < 720))
									{
										usePresetDisposition = true;
										presetDispositionWith = 1280;
										presetDispositionHeight = 720;
										presetDispositionFrequency = target_refreshrate;
										changeres = true;
									}
									if (Program.gpuResolution == 1 && (screenWidth < 1920 || screenHeight < 1080))
									{
										usePresetDisposition = true;
										presetDispositionWith = 1920;
										presetDispositionHeight = 1080;
										presetDispositionFrequency = target_refreshrate;
										changeres = true;
									}
									if (Program.gpuResolution == 2 && (screenWidth < 2560 || screenHeight < 1440))
									{
										usePresetDisposition = true;
										presetDispositionWith = 2560;
										presetDispositionHeight = 1440;
										presetDispositionFrequency = target_refreshrate;
										changeres = true;
									}
									if (Program.gpuResolution == 3 && (screenWidth < 3840 || screenHeight < 2160))
									{
										usePresetDisposition = true;
										presetDispositionWith = 3840;
										presetDispositionHeight = 2160;
										presetDispositionFrequency = target_refreshrate;
										changeres = true;
									}
									if (changeres)
									{

										var devices = Westwind.SetResolution.DisplayManager.GetAllDisplayDevices();
										var monitor = devices.FirstOrDefault(d => d.IsSelected);  // main monitor
										var list = Westwind.SetResolution.DisplayManager.GetAllDisplaySettings(monitor.DriverDeviceName);

										var resolutionAvailiable = list
											.Where(d => d.BitCount == 32 && d.Orientation == 0)
											.OrderBy(d => d.Width)
											.ThenBy(d => d.Height)
											.ThenBy(d => d.Frequency)
											.FirstOrDefault(d => d.Width == presetDispositionWith &&
																 d.Height == presetDispositionHeight &&
																 d.Frequency == presetDispositionFrequency);

										if (resolutionAvailiable == null)
										{
											//Resolution juste en dessous mais sans decendre en dessous de 1, par exemple 59Hz au lieu de 60hz
											resolutionAvailiable = list
											.Where(d => d.Width == presetDispositionWith &&
														d.Height == presetDispositionHeight &&
														d.BitCount == 32 &&
															d.Orientation == 0 &&
															d.Frequency < presetDispositionFrequency &&
															d.Frequency >= presetDispositionFrequency - 1)
											.OrderByDescending(d => d.Frequency)
											.FirstOrDefault();
										}
										if (resolutionAvailiable == null)
										{
											// Chercher la r�solution imm�diatement au-dessus de 60 Hz
											resolutionAvailiable = list
												.Where(d => d.Width == presetDispositionWith &&
															d.Height == presetDispositionHeight &&
															d.BitCount == 32 &&
															d.Orientation == 0 &&
															d.Frequency > presetDispositionFrequency)
												.OrderBy(d => d.Frequency)
												.FirstOrDefault();
										}
										// Si aucune r�solution n'est trouv�e au-dessus, chercher la r�solution imm�diatement en dessous de 60 Hz
										if (resolutionAvailiable == null)
										{
											resolutionAvailiable = list
												.Where(d => d.Width == presetDispositionWith &&
															d.Height == presetDispositionHeight &&
															d.BitCount == 32 &&
															d.Orientation == 0 &&
															d.Frequency < presetDispositionFrequency)
												.OrderByDescending(d => d.Frequency)
												.FirstOrDefault();
										}
										if (resolutionAvailiable != null)
										{
											_dispositionToSwitch = "custom";
											presetDispositionFrequency = resolutionAvailiable.Frequency;
										}
										else presetDispositionFrequency = 60;

										Utils.LogMessage($"Change mainRes to {presetDispositionWith}x{presetDispositionHeight}:{presetDispositionFrequency} (upscaleWindowed)");

										Program.refreshRate = presetDispositionFrequency;
										refreshrate_done = true;
									}


								}
							}


							/*

							string TPAdvisedSettingsFile = Path.Combine(basePath, "config", originalConfigFileNameWithoutExt + "." + "tp_patch_settings" + ".json");
							if (potentialAltConfigDir != "")
							{
								var TPAdvisedSettingsFileAlt = Path.Combine(potentialAltConfigDir, originalConfigFileNameWithoutExt + "." + "tp_patch_settings" + ".json");
								if (File.Exists(TPAdvisedSettingsFileAlt)) TPAdvisedSettingsFile = TPAdvisedSettingsFileAlt;
							}
							if (File.Exists(TPAdvisedSettingsFile))
							{
								//TPSettings Manager
							}

							string gameOptionsAdvisedFile = Path.Combine(basePath, "config", originalConfigFileNameWithoutExt + "." + "gameoptions_patch_settings" + ".json");
							if (potentialAltConfigDir != "")
							{
								var gameOptionsAdvisedFileAlt = Path.Combine(potentialAltConfigDir, originalConfigFileNameWithoutExt + "." + "gameoptions_patch_settings" + ".json");
								if (File.Exists(gameOptionsAdvisedFileAlt)) gameOptionsAdvisedFile = gameOptionsAdvisedFileAlt;
							}
							if (File.Exists(gameOptionsAdvisedFile))
							{

								//TPSettings Manager
							}
							*/


							string perGameLinkFolder = ConfigurationManager.MainConfig.perGameLinkFolder;
							if (perGameLinkFolder == @"Default (<YourTeknoparrotFolder>\AutoXinputLinks)")
							{
								perGameLinkFolder = Path.Combine(baseTpDirOriginal, "AutoXinputLinks");
							}

							string linkSourceFolder = Path.Combine(perGameLinkFolder, originalConfigFileNameWithoutExt);
							string linkTargetFolder = "";


							string linkTargetFolderExe = "";

							string executableGame = "";
							string executableGameFile = "";
							string executableGameDir = "";
							string gameDir = "";
							bool gameNeedAdmin = false;
							string DirFFBPlugin = "";

							Utils.LogMessage($"baseTpDir : {baseTpDir}");
							Utils.LogMessage($"originalConfigFileName : {originalConfigFileName}");
							Utils.LogMessage($"originalConfigFileNameWithoutExt : {originalConfigFileNameWithoutExt}");
							Utils.LogMessage($"teknoparrotExe : {teknoparrotExe}");
							Utils.LogMessage($"linkSourceFolder Elf : {linkSourceFolder}");

							if (!File.Exists(teknoparrotExe))
							{
								Utils.LogMessage($"Can't find {teknoparrotExe}");
								MessageBox.Show($"Can't find {teknoparrotExe}");
								return;
							}

							if (Directory.Exists(perGameLinkFolder))
							{
								string targetTest = Path.Combine(baseTpDir, "TeknoParrot");
								string linktest1 = Path.Combine(targetTest, "test.link.write");
								string linktest2 = Path.Combine(targetTest, "test.link.hardlink");
								string linktest3 = Path.Combine(targetTest, "test.link.softlink");
								try
								{
									if (File.Exists(linktest1)) File.Delete(linktest1);
									if (File.Exists(linktest2)) File.Delete(linktest2);
									if (File.Exists(linktest3)) File.Delete(linktest3);
								}
								catch { }
								string sourceLinkTest = "";
								if (!string.IsNullOrEmpty(perGameLinkFolder) && Directory.Exists(perGameLinkFolder))
								{
									sourceLinkTest = Utils.FindAnyFile(perGameLinkFolder);
								}

								try
								{
									File.WriteAllText(linktest1, "todelete");
								}
								catch { }
								if (sourceLinkTest != "")
								{
									try
									{
										Utils.MakeLink(sourceLinkTest, linktest2);
									}
									catch { }
									try
									{
										Utils.CreateSoftlink(sourceLinkTest, linktest3);
									}
									catch { }
								}
								try
								{
									if (File.Exists(linktest1)) { linkAutoWritable = true; File.Delete(linktest1); }
									if (File.Exists(linktest2)) { linkAutoHardLink = true; File.Delete(linktest2); }
									if (File.Exists(linktest3)) { linkAutoSoftLink = true; File.Delete(linktest3); }
								}
								catch { }
								Utils.LogMessage($"linkAutoWritable={linkAutoWritable} linkAutoHardLink={linkAutoHardLink} linkAutoSoftLink={linkAutoSoftLink}");
							}

							Utils.LogMessage($"check if eligigible to HardLink");
							if (linkAutoWritable && ((Utils.IsEligibleHardLink(baseTpDir) && linkAutoHardLink) || linkAutoSoftLink))
							{
								Utils.LogMessage($"Starting Clean HardlinkFiles");
								//Utils.LogMessage($"DEBUG Baggio 1: CleanHardLinksFilesOriginal({Path.Combine(baseTpDir, "TeknoParrot")}, {perGameLinkFolder})");

								Utils.CleanHardLinksFilesOriginal(Path.Combine(baseTpDir, "TeknoParrot"), perGameLinkFolder);
								Utils.CleanHardLinksFilesOriginal(Path.Combine(baseTpDir, "ElfLdr2"), perGameLinkFolder);
								Utils.LogMessage($"End Clean HardlinkFiles");
							}
							else
							{
								Utils.LogMessage($"Not eligible for Hardlink");
							}


							Utils.LogMessage($"Load XML to get emulatorType and requiresAdmin");
							try
							{
								XmlNode gamePathNode = TpSettingsManager.xmlDoc.SelectSingleNode("/GameProfile/GamePath");
								if (gamePathNode != null)
								{
									string gamePathContent = gamePathNode.InnerText;
									if (gamePathContent != "" && File.Exists(gamePathContent)) executableGameFile = gamePathContent;

									if (gamePathContent.ToLower().EndsWith(".exe")) executableGame = gamePathContent;
									if (!string.IsNullOrEmpty(gamePathContent) && File.Exists(gamePathContent))
									{
										gameDir = Path.GetFullPath(Directory.GetParent(gamePathContent).ToString());
									}
								}

								XmlNode emulatorTypeNode = TpSettingsManager.xmlDoc.SelectSingleNode("/GameProfile/EmulatorType");
								if (emulatorTypeNode != null)
								{
									string emulatorTypeValue = emulatorTypeNode.InnerText.ToLower().Trim();
									if (emulatorTypeValue == "elfldr2" || emulatorTypeValue == "lindbergh")
									{

										if (emulatorTypeValue == "elfldr2")
										{
											linkTargetFolder = Path.Combine(baseTpDir, "ElfLdr2");
											executableGame = Path.Combine(linkTargetFolder, "BudgieLoader.exe");
										}
										if (emulatorTypeValue == "lindbergh")
										{
											linkTargetFolder = Path.Combine(baseTpDir, "TeknoParrot");
											executableGame = Path.Combine(linkTargetFolder, "BudgieLoader.exe");
										}
										if (File.Exists(Path.Combine(linkSourceFolder, "FFBPlugin.ini")))
										{
											DirFFBPlugin = linkSourceFolder;
										}

									}
									Utils.LogMessage($"emulatorTypeValue = {emulatorTypeValue}");
									Utils.LogMessage($"linkTargetFolder = {linkTargetFolder}");
								}

								XmlNode requiresAdminNode = TpSettingsManager.xmlDoc.SelectSingleNode("/GameProfile/RequiresAdmin");
								if (requiresAdminNode != null)
								{
									string requiresAdminValue = requiresAdminNode.InnerText.ToLower().Trim();
									if (requiresAdminValue == "true")
									{
										gameNeedAdmin = true;
									}
									Utils.LogMessage($"requiresAdminValue = {requiresAdminValue}");
								}
							}
							catch
							{
								Utils.LogMessage($"Error while parsing xml file");
							}


							//Utils.LogMessage($"Checking gameOverride options");
							//GameOptionOverrite

							if (gameOptions.UseGlobalStoozZoneGamepad == false)
							{
								gamepadStooz = gameOptions.gamepadStooz;
								enableStoozZone_Gamepad = gameOptions.enableStoozZone_Gamepad;
								valueStooz_Gamepad = gameOptions.valueStooz_Gamepad;
							}
							if (gameOptions.UseGlobalStoozZoneWheel == false)
							{
								wheelStooz = gameOptions.wheelStooz;
								enableStoozZone_Wheel = gameOptions.enableStoozZone_Wheel;
								valueStooz_Wheel = gameOptions.valueStooz_Wheel;
							}
							if (gameOptions.UseGlobalStoozZoneHotas == false)
							{
								hotasStooz = gameOptions.hotasStooz;
								enableStoozZone_Hotas = gameOptions.enableStoozZone_Hotas;
								valueStooz_Hotas = gameOptions.valueStooz_Hotas;
							}

							Utils.LogMessage($"gameOptions Values = {gameOptions.Serialize()}");

							if (executableGame != "" && File.Exists(executableGame))
							{
								executableGameDir = Path.GetFullPath(Directory.GetParent(executableGame).ToString());
							}
							Utils.LogMessage($"executableGame = {executableGame}");
							Utils.LogMessage($"executableGameDir = {executableGameDir}");
							if (gameDir != "" && !string.IsNullOrEmpty(ConfigurationManager.MainConfig.perGameLinkFolderExe))
							{
								linkSourceFolderExe = Path.Combine(ConfigurationManager.MainConfig.perGameLinkFolderExe, originalConfigFileNameWithoutExt);
								linkTargetFolderExe = gameDir;
								if (gameOptions.CustomPerGameLinkFolder != null && gameOptions.CustomPerGameLinkFolder != "")
								{
									string lastFolder = Path.GetFileName(gameOptions.CustomPerGameLinkFolder);
									if (lastFolder == originalConfigFileNameWithoutExt)
									{
										linkSourceFolderExe = gameOptions.CustomPerGameLinkFolder;
									}
								}
								if (File.Exists(Path.Combine(linkSourceFolderExe, "FFBPlugin.ini")))
								{
									DirFFBPlugin = linkSourceFolderExe;
								}
								else if (File.Exists(Path.Combine(linkSourceFolderExe, "[!ffb!]", "FFBPlugin.ini")))
								{
									DirFFBPlugin = Path.Combine(linkSourceFolderExe, "[!ffb!]");
								}
								Utils.LogMessage($"linkSourceFolderExe = {linkSourceFolderExe}");
								Utils.LogMessage($"linkTargetFolderExe = {linkTargetFolderExe}");

								{
									string linktest1 = Path.Combine(linkTargetFolderExe, "test.link.write");
									string linktest2 = Path.Combine(linkTargetFolderExe, "test.link.hardlink");
									string linktest3 = Path.Combine(linkTargetFolderExe, "test.link.softlink");
									try
									{
										if (File.Exists(linktest1)) File.Delete(linktest1);
										if (File.Exists(linktest2)) File.Delete(linktest2);
										if (File.Exists(linktest3)) File.Delete(linktest3);
									}
									catch { }
									string sourceLinkTest = "";
									if (!string.IsNullOrEmpty(linkSourceFolderExe) && Directory.Exists(linkSourceFolderExe))
									{
										sourceLinkTest = Utils.FindAnyFile(linkSourceFolderExe);
									}

									try
									{
										File.WriteAllText(linktest1, "todelete");
									}
									catch { }
									if (sourceLinkTest != "")
									{
										try
										{
											Utils.MakeLink(sourceLinkTest, linktest2);
										}
										catch { }
										try
										{
											Utils.CreateSoftlink(sourceLinkTest, linktest3);
										}
										catch { }
									}
									try
									{
										if (File.Exists(linktest1)) { linkExeWritable = true; File.Delete(linktest1); }
										if (File.Exists(linktest2)) { linkExeHardLink = true; File.Delete(linktest2); }
										if (File.Exists(linktest3)) { linkExeSoftLink = true; File.Delete(linktest3); }
									}
									catch { }
									Utils.LogMessage($"linkExeWritable={linkExeWritable} linkExeHardLink={linkExeHardLink} linkExeSoftLink={linkExeSoftLink}");
								}

								if (!nolink && ((Utils.IsEligibleHardLink(linkTargetFolderExe) && linkExeHardLink) || linkExeSoftLink) && linkExeWritable)
								{
									if (Utils.PrecheckLinksFile(linkTargetFolderExe, linkSourceFolderExe, executableGameFile))
									{
										linkExePrecheck = true;
										Utils.LogMessage("Pre clean linkTargetFolderExe");
										Utils.CleanHardLinksFiles(linkTargetFolderExe, linkSourceFolderExe, executableGameFile);
									}
								}
							}

							ParrotDataOriginal = Path.Combine(Directory.GetParent(Path.GetDirectoryName(Path.GetFullPath(xmlFile))).FullName, "ParrotData.xml");
							ParrotDataBackup = Path.Combine(Directory.GetParent(Path.GetDirectoryName(Path.GetFullPath(xmlFile))).FullName, "ParrotData.xml.AutoXinputBackup");
							Utils.LogMessage($"ParrotDataOriginal = {ParrotDataOriginal}");
							Utils.LogMessage($"ParrotDataBackup = {ParrotDataBackup}");

							if (File.Exists(ParrotDataBackup))
							{
								Utils.LogMessage($"Restore ParrotData");
								try
								{
									File.Copy(ParrotDataBackup, ParrotDataOriginal, true);
									File.Delete(ParrotDataBackup);
								}
								catch
								{
									Utils.LogMessage($"Error while restoring ParrotData");
								}
							}

							FFBPluginIniFile = "";
							FFBPluginIniBackup = "";

							if (DirFFBPlugin == "") DirFFBPlugin = Path.GetDirectoryName(executableGameDir);
							Utils.LogMessage($"DirFFBPlugin = {DirFFBPlugin}");

							if (!string.IsNullOrEmpty(executableGame) && !string.IsNullOrEmpty(DirFFBPlugin))
							{
								FFBPluginIniFile = Path.Combine(DirFFBPlugin, "FFBPlugin.ini");
								FFBPluginIniBackup = Path.Combine(DirFFBPlugin, "FFBPlugin.ini.AutoXinputBackup");
								Utils.LogMessage($"FFBPluginIniFile = {FFBPluginIniFile}");
								Utils.LogMessage($"FFBPluginIniBackup = {FFBPluginIniBackup}");
								if (File.Exists(FFBPluginIniBackup))
								{
									Utils.LogMessage($"FFBPluginIniBackup exist, restoring original");
									try
									{
										File.Copy(FFBPluginIniBackup, FFBPluginIniFile, true);
										File.Delete(FFBPluginIniBackup);
									}
									catch
									{
										Utils.LogMessage($"Error while restoring FFBPluginIniBackup");
									}
								}
							}


							/*
							if (!string.IsNullOrEmpty(executableGame))
							{
								string dirCrosshair = Path.GetDirectoryName(executableGameDir);
								string p1Crosshair = Path.Combine(dirCrosshair, "p1.png");
								string p2Crosshair = Path.Combine(dirCrosshair, "p2.png");
								string p1CrosshairBackup = Path.Combine(dirCrosshair, "p1.png.AutoXinputBackup");
								string p2CrosshairBackup = Path.Combine(dirCrosshair, "p2.png.AutoXinputBackup");
								if (File.Exists(p1CrosshairBackup))
								{
									try
									{
										Utils.LogMessage($"Restore Crosshair p1.png.AutoXinputBackup");
										File.Move(p1CrosshairBackup, p1Crosshair, true);
									}
									catch
									{}
								}
								if (File.Exists(p2CrosshairBackup))
								{
									try
									{
										Utils.LogMessage($"Restore Crosshair p2.png.AutoXinputBackup");
										File.Move(p2CrosshairBackup, p2Crosshair, true);
									}
									catch
									{ }
								}
							}
							*/

							var shifterPath = Path.Combine(basePath, "config", originalConfigFileNameWithoutExt + "." + "shifter" + ".json");
							if (potentialAltConfigDir != "")
							{
								var shifterPathAlt = Path.Combine(potentialAltConfigDir, originalConfigFileNameWithoutExt + "." + "shifter" + ".json");
								if (File.Exists(shifterPathAlt)) shifterPath = shifterPathAlt;
							}
							if (File.Exists(shifterPath))
							{
								shifterData = (Dictionary<string, string>)JsonConvert.DeserializeObject<Dictionary<string, string>>(Utils.ReadAllText(shifterPath));
							}


							var throttlePath = Path.Combine(basePath, "config", originalConfigFileNameWithoutExt + "." + "throttle" + ".json");
							if (potentialAltConfigDir != "")
							{
								var throttlePathAlt = Path.Combine(potentialAltConfigDir, originalConfigFileNameWithoutExt + "." + "throttle" + ".json");
								if (File.Exists(throttlePathAlt)) throttlePath = throttlePathAlt;
							}
							if (File.Exists(throttlePath))
							{
								throttleData = (Dictionary<string, string>)JsonConvert.DeserializeObject<Dictionary<string, string>>(Utils.ReadAllText(throttlePath));
							}


							string emptyConfigPath = Path.Combine(Directory.GetParent(Path.GetDirectoryName(Path.GetFullPath(xmlFile))).FullName, "GameProfiles", originalConfigFileName);
							Utils.LogMessage($"Base TP GameProfile : {emptyConfigPath}");
							if (!File.Exists(emptyConfigPath))
							{
								Utils.LogMessage($"GameProfile Does not exist");
								finalConfig = xmlFile;
							}

							Utils.LogMessage($"Load Availiable config :");
							foreach (var type in typeConfig)
							{
								var configPath = Path.Combine(basePath, "config", originalConfigFileNameWithoutExt + "." + type + ".txt");
								if (potentialAltConfigDir != "")
								{
									var configPathAlt = Path.Combine(potentialAltConfigDir, originalConfigFileNameWithoutExt + "." + type + ".txt");
									if (File.Exists(configPathAlt)) configPath = configPathAlt;
								}
								if (File.Exists(configPath))
								{
									Utils.LogMessage($"Found {configPath}");
									existingConfig.Add(type, configPath);
								}
							}
							if (existingConfig.Count() == 0 || passthrough)
							{
								Utils.LogMessage($"passthrough Mode = ON");
								finalConfig = xmlFile;
								showStartup = false;
							}

							bool usealtgamepad = false;
							if (favorAB && existingConfig.ContainsKey("gamepadalt") && existingConfig.ContainsKey("wheel"))
							{
								usealtgamepad = true;
								Utils.LogMessage($"usealtgamepad = ON");
							}

							Utils.LogMessage($"finalConfig = {finalConfig}");
							ShifterHack shifterHack = null;
							bool useShifterHack = false;
							if (gameOptions.EnableGearChange) useShifterHack = true;
							Utils.LogMessage($"useShifterHack Option = " + (useShifterHack ? "ON" : "OFF"));

							if (finalConfig == "")
							{
								var connectedGamePad = new Dictionary<int, XinputGamepad>();
								var gamepad = X.Gamepad_1;
								if (gamepad.Capabilities.Type != 0) connectedGamePad.Add(0, new XinputGamepad(gamepad, 0));
								gamepad = X.Gamepad_2;
								if (gamepad.Capabilities.Type != 0) connectedGamePad.Add(1, new XinputGamepad(gamepad, 1));
								gamepad = X.Gamepad_3;
								if (gamepad.Capabilities.Type != 0) connectedGamePad.Add(2, new XinputGamepad(gamepad, 2));
								gamepad = X.Gamepad_4;
								if (gamepad.Capabilities.Type != 0) connectedGamePad.Add(3, new XinputGamepad(gamepad, 3));
								int availableSlot = -1;
								if (availableSlot == -1 && !connectedGamePad.ContainsKey(0)) availableSlot = 0;
								if (availableSlot == -1 && !connectedGamePad.ContainsKey(1)) availableSlot = 1;
								if (availableSlot == -1 && !connectedGamePad.ContainsKey(2)) availableSlot = 2;
								if (availableSlot == -1 && !connectedGamePad.ContainsKey(3)) availableSlot = 3;
								bool haveArcade = false;
								bool haveWheel = false;
								bool haveGamepad = false;
								bool haveHotas = false;
								bool haveLightgun = false;

								Utils.LogMessage($"availableSlot = {availableSlot}");

								bool checkDinputWheel = ConfigurationManager.MainConfig.useDinputWheel;
								Dictionary<string, JoystickButtonData> bindingDinputWheel = null;
								string bindingDinputWheelJson = ConfigurationManager.MainConfig.bindingDinputWheel;

								bool checkDinputShifter = ConfigurationManager.MainConfig.useDinputShifter;
								Dictionary<string, JoystickButtonData> bindingDinputShifter = null;
								string bindingDinputShifterJson = ConfigurationManager.MainConfig.bindingDinputShifter;

								bool checkDinputHotas = ConfigurationManager.MainConfig.useDinputHotas;
								Dictionary<string, JoystickButtonData> bindingDinputHotas = null;
								string bindingDinputHotasJson = ConfigurationManager.MainConfig.bindingDinputHotas;


								//Start LightGunCheck
								bool checkDinputLightgun = false;
								string LightgunA_Type = ConfigurationManager.MainConfig.gunAType;
								string LightgunB_Type = ConfigurationManager.MainConfig.gunBType;
								if (!string.IsNullOrEmpty(LightgunA_Type) && LightgunA_Type != "<none>") checkDinputLightgun = true;
								if (!string.IsNullOrEmpty(LightgunB_Type) && LightgunB_Type != "<none>") checkDinputLightgun = true;


								string bindingDinputLightgunAJson = "";
								string bindingDinputLightgunBJson = "";
								//bool dinputLightgunAFound = false;
								//bool dinputLightgunBFound = false;
								Dictionary<string, JoystickButtonData> bindingDinputLightGunA = null;
								Dictionary<string, JoystickButtonData> bindingDinputLightGunB = null;
								Dictionary<string, JoystickButtonData> bindingDinputLightGun = new Dictionary<string, JoystickButtonData>();
								Dictionary<int, (JoystickButtonData, JoystickButtonData)> originalJoystickPerGun = new Dictionary<int, (JoystickButtonData, JoystickButtonData)>();
								originalJoystickPerGun.Add(1, (null, null));
								originalJoystickPerGun.Add(2, (null, null));
								bool GunCoinOverwriteA = false;
								bool GunCoinOverwriteB = false;
								bool GunCoinOverwrite = false;


								if (checkDinputLightgun)
								{
									int nb_wiimote = 0;
									int current_wiimote = 0;
									if (LightgunA_Type == "wiimote" || LightgunB_Type == "wiimote")
									{
										WiimoteCollection mWC = new WiimoteCollection();
										try
										{
											mWC.FindAllWiimotes();
										}
										catch (Exception ex)
										{

										}
										foreach (Wiimote wm in mWC)
										{
											nb_wiimote++;
										}
									}


									if (LightgunA_Type == "sinden" || LightgunA_Type == "guncon1" || LightgunA_Type == "guncon2" || LightgunA_Type == "wiimote" || LightgunA_Type == "gamepad")
									{
										if (LightgunA_Type == "gamepad") bindingDinputLightgunAJson = ConfigurationManager.MainConfig.bindingDinputGunAXbox;
										if (LightgunA_Type == "sinden") bindingDinputLightgunAJson = ConfigurationManager.MainConfig.bindingDinputGunASinden;
										if (LightgunA_Type == "guncon1") bindingDinputLightgunAJson = ConfigurationManager.MainConfig.bindingDinputGunAGuncon1;
										if (LightgunA_Type == "guncon2") bindingDinputLightgunAJson = ConfigurationManager.MainConfig.bindingDinputGunAGuncon2;
										if (LightgunA_Type == "wiimote") bindingDinputLightgunAJson = ConfigurationManager.MainConfig.bindingDinputGunAWiimote;
										bindingDinputLightGunA = (Dictionary<string, JoystickButtonData>)JsonConvert.DeserializeObject<Dictionary<string, JoystickButtonData>>(bindingDinputLightgunAJson);

										if (LightgunA_Type == "wiimote")
										{
											if (bindingDinputLightGunA != null && bindingDinputLightGunA.ContainsKey("LightgunX"))
											{
												string tempGuid = bindingDinputLightGunA["LightgunX"].JoystickGuid.ToString();
												string tempDeviceName = bindingDinputLightGunA["LightgunX"].DeviceName;
												var match = Regex.Match(tempDeviceName, @"^vjoy(\d+)$");
												if (match.Success)
												{
													int tempvjoynumber = int.Parse(match.Groups[1].Value);
													if (vjoyData.ContainsKey(tempvjoynumber))
													{
														string newGuid = vjoyData[tempvjoynumber];
														Guid newGuid2;
														if (Guid.TryParse(newGuid, out newGuid2))
														{
															foreach (var key in bindingDinputLightGunA.Keys.ToList())
															{
																if (bindingDinputLightGunA[key].JoystickGuid.ToString() == tempGuid)
																{
																	bindingDinputLightGunA[key].JoystickGuid = newGuid2;
																}
															}
														}
													}
												}
											}
										}

										if (bindingDinputLightGunA != null && bindingDinputLightGunA.ContainsKey("LightgunX"))
										{
											GunAGuid = bindingDinputLightGunA["LightgunX"].JoystickGuid.ToString();
											Utils.LogMessage($"bindingDinputLightGunA to Search = {GunAGuid}");
										}
										if (LightgunA_Type == "wiimote")
										{
											if (bindingDinputLightGunA != null && bindingDinputLightGunA.ContainsKey("LightgunX"))
											{
												if (bindingDinputLightGunA["LightgunX"].DeviceName.ToLower().Contains("vjoy"))
												{
													current_wiimote++;
													if (nb_wiimote < current_wiimote)
													{
														Utils.LogMessage($"bindingDinputLightGunA = No Wiimote connected");
														GunAGuid = "";
													}
												}
											}

										}
									}
									if (GunAGuid != "") GunAType = LightgunA_Type;

									if (LightgunB_Type == "sinden" || LightgunB_Type == "guncon1" || LightgunB_Type == "guncon2" || LightgunB_Type == "wiimote" || LightgunB_Type == "gamepad")
									{
										if (LightgunB_Type == "gamepad") bindingDinputLightgunBJson = ConfigurationManager.MainConfig.bindingDinputGunBXbox;
										if (LightgunB_Type == "sinden") bindingDinputLightgunBJson = ConfigurationManager.MainConfig.bindingDinputGunBSinden;
										if (LightgunB_Type == "guncon1") bindingDinputLightgunBJson = ConfigurationManager.MainConfig.bindingDinputGunBGuncon1;
										if (LightgunB_Type == "guncon2") bindingDinputLightgunBJson = ConfigurationManager.MainConfig.bindingDinputGunBGuncon2;
										if (LightgunB_Type == "wiimote") bindingDinputLightgunBJson = ConfigurationManager.MainConfig.bindingDinputGunBWiimote;
										bindingDinputLightGunB = (Dictionary<string, JoystickButtonData>)JsonConvert.DeserializeObject<Dictionary<string, JoystickButtonData>>(bindingDinputLightgunBJson);


										if (LightgunB_Type == "wiimote")
										{
											if (bindingDinputLightGunB != null && bindingDinputLightGunB.ContainsKey("LightgunX"))
											{
												string tempGuid = bindingDinputLightGunB["LightgunX"].JoystickGuid.ToString();
												string tempDeviceName = bindingDinputLightGunB["LightgunX"].DeviceName;
												var match = Regex.Match(tempDeviceName, @"^vjoy(\d+)$");
												if (match.Success)
												{
													int tempvjoynumber = int.Parse(match.Groups[1].Value);
													if (vjoyData.ContainsKey(tempvjoynumber))
													{
														string newGuid = vjoyData[tempvjoynumber];
														Guid newGuid2;
														if (Guid.TryParse(newGuid, out newGuid2))
														{
															foreach (var key in bindingDinputLightGunB.Keys.ToList())
															{
																if (bindingDinputLightGunB[key].JoystickGuid.ToString() == tempGuid)
																{
																	bindingDinputLightGunB[key].JoystickGuid = newGuid2;
																}
															}
														}
													}
												}
											}
										}

										if (bindingDinputLightGunB != null && bindingDinputLightGunB.ContainsKey("LightgunX"))
										{
											GunBGuid = bindingDinputLightGunB["LightgunX"].JoystickGuid.ToString();
											Utils.LogMessage($"bindingDinputLightGunB to Search = {GunBGuid}");
										}
										if (LightgunB_Type == "wiimote")
										{
											if (bindingDinputLightGunB != null && bindingDinputLightGunB.ContainsKey("LightgunX"))
											{
												if (bindingDinputLightGunB["LightgunX"].DeviceName.ToLower().Contains("vjoy"))
												{
													current_wiimote++;
													if (nb_wiimote < current_wiimote)
													{
														Utils.LogMessage($"bindingDinputLightGunB = No Wiimote connected");
														GunBGuid = "";
													}
												}
											}
										}
									}
									if (GunBGuid != "") GunBType = LightgunB_Type;

									if (!string.IsNullOrEmpty(GunAGuid) || !string.IsNullOrEmpty(GunBGuid))
									{
										DirectInput directInput = new DirectInput();
										List<DeviceInstance> devices = new List<DeviceInstance>();
										devices.AddRange(directInput.GetDevices().Where(x => x.Type != SharpDX.DirectInput.DeviceType.Mouse && x.UsagePage != UsagePage.VendorDefinedBegin && x.Usage != UsageId.AlphanumericBitmapSizeX && x.Usage != UsageId.AlphanumericAlphanumericDisplay && x.UsagePage != unchecked((UsagePage)0xffffff43) && x.UsagePage != UsagePage.Vr).ToList());
										foreach (var device in devices)
										{
											if (device.Type == SharpDX.DirectInput.DeviceType.Keyboard && FirstKeyboardGuid == null)
											{
												FirstKeyboardGuid = device.InstanceGuid;
											}
											if (device.InstanceGuid.ToString() == GunAGuid)
											{
												Utils.LogMessage($"GunAGuid Found");
												dinputLightgunAFound = true;
												haveLightgun = true;
												if (GunAGuid != "" && GunAType == "sinden")
												{
													var joystick = new Joystick(directInput, device.InstanceGuid);
													var sindentype = 0;
													if (joystick.Properties.InterfacePath.ToUpper().Contains("VID_16C0&PID_0F38")) sindentype = 1; //Black
													if (joystick.Properties.InterfacePath.ToUpper().Contains("VID_16C0&PID_0F01")) sindentype = 2; //Blue
													if (joystick.Properties.InterfacePath.ToUpper().Contains("VID_16C0&PID_0F02")) sindentype = 3; //Red
													if (joystick.Properties.InterfacePath.ToUpper().Contains("VID_16C0&PID_0F39")) sindentype = 4; //Player2
													GunASindenType = sindentype;
													joystick.Dispose();
												}
											}
											if (device.InstanceGuid.ToString() == GunBGuid)
											{
												Utils.LogMessage($"GunBGuid Found");
												dinputLightgunBFound = true;
												haveLightgun = true;
												if (GunBGuid != "" && GunBType == "sinden")
												{
													var joystick = new Joystick(directInput, device.InstanceGuid);
													var sindentype = 0;
													if (joystick.Properties.InterfacePath.ToUpper().Contains("VID_16C0&PID_0F38")) sindentype = 1; //Black
													if (joystick.Properties.InterfacePath.ToUpper().Contains("VID_16C0&PID_0F01")) sindentype = 2; //Blue
													if (joystick.Properties.InterfacePath.ToUpper().Contains("VID_16C0&PID_0F02")) sindentype = 3; //Red
													if (joystick.Properties.InterfacePath.ToUpper().Contains("VID_16C0&PID_0F39")) sindentype = 4; //Player2
													GunBSindenType = sindentype;
													joystick.Dispose();
												}
											}
										}
									}
									if (!dinputLightgunAFound) GunAGuid = "";
									if (!dinputLightgunBFound) GunBGuid = "";


									if (haveLightgun)
									{
										hideCrosshair = true;
										crosshairA = false;
										crosshairB = false;

										crosshairA = ConfigurationManager.MainConfig.gunACrosshair;
										if (gameOptions.gunA_crosshair > 0)
										{
											if (gameOptions.gunA_crosshair == 1) crosshairA = true;
											if (gameOptions.gunA_crosshair == 2) crosshairA = false;
										}
										crosshairB = ConfigurationManager.MainConfig.gunBCrosshair;
										if (gameOptions.gunB_crosshair > 0)
										{
											if (gameOptions.gunB_crosshair == 1) crosshairB = true;
											if (gameOptions.gunB_crosshair == 2) crosshairB = false;
										}
										if (!dinputLightgunAFound) crosshairA = false;
										if (!dinputLightgunBFound) crosshairB = false;
										//hideCrosshair = (!crosshairA || !crosshairB);

										if (dinputLightgunAFound && crosshairA) hideCrosshair = false;
										if (dinputLightgunBFound && crosshairB) hideCrosshair = false;


									}
								}
								Utils.LogMessage($"gunA_crosshair = {(crosshairA ? "True" : "False")}");
								Utils.LogMessage($"gunB_crosshair = {(crosshairB ? "True" : "False")}");
								Utils.LogMessage($"hideCrosshair = {(hideCrosshair ? "True" : "False")}");


								if (dinputLightgunAFound)
								{
									bool gunAoffscreenReload = ConfigurationManager.MainConfig.gunAOffscreenReload;
									if (gameOptions.gunA_OffscreenReload > 0)
									{
										if (gameOptions.gunA_OffscreenReload == 1) gunAoffscreenReload = false;
										if (gameOptions.gunA_OffscreenReload == 2) gunAoffscreenReload = true;
									}
									Utils.LogMessage($"gunA_OffscreenReload = {(gunAoffscreenReload ? "True" : "False")}");
									ButtonToKeyManager.buttonToKey.EnableGunAOffscreenReload = gunAoffscreenReload;
								}
								if (dinputLightgunBFound)
								{
									bool gunBoffscreenReload = ConfigurationManager.MainConfig.gunAOffscreenReload;
									if (gameOptions.gunB_OffscreenReload > 0)
									{
										if (gameOptions.gunB_OffscreenReload == 1) gunBoffscreenReload = false;
										if (gameOptions.gunB_OffscreenReload == 2) gunBoffscreenReload = true;
									}
									Utils.LogMessage($"gunB_OffscreenReload = {(gunBoffscreenReload ? "True" : "False")}");
									ButtonToKeyManager.buttonToKey.EnableGunBOffscreenReload = gunBoffscreenReload;
								}

								if (dinputLightgunAFound && bindingDinputLightGunA.ContainsKey("LightgunStart") && !bindingDinputLightGunA.ContainsKey("LightgunCoin"))
								{
									if (dinputLightgunAFound && bindingDinputLightGunA.ContainsKey("LightgunX") && bindingDinputLightGunA.ContainsKey("LightgunY"))
									{
										originalJoystickPerGun[1] = (bindingDinputLightGunA["LightgunX"], bindingDinputLightGunA["LightgunY"]);
										GunCoinOverwriteA = true;
										GunCoinOverwrite = true;
									}
								}
								if (dinputLightgunBFound && bindingDinputLightGunB.ContainsKey("LightgunStart") && !bindingDinputLightGunB.ContainsKey("LightgunCoin"))
								{
									if (dinputLightgunBFound && bindingDinputLightGunB.ContainsKey("LightgunX") && bindingDinputLightGunB.ContainsKey("LightgunY"))
									{
										originalJoystickPerGun[2] = (bindingDinputLightGunB["LightgunX"], bindingDinputLightGunB["LightgunY"]);
										GunCoinOverwriteB = true;
										GunCoinOverwrite = true;
									}
								}


								if (dinputLightgunAFound && ButtonToKeyManager.buttonToKey.EnableGunAOffscreenReload)
								{
									if (dinputLightgunAFound && bindingDinputLightGunA.ContainsKey("LightgunX") && bindingDinputLightGunA.ContainsKey("LightgunY"))
									{
										originalJoystickPerGun[1] = (bindingDinputLightGunA["LightgunX"], bindingDinputLightGunA["LightgunY"]);
									}
								}
								if (dinputLightgunBFound && ButtonToKeyManager.buttonToKey.EnableGunBOffscreenReload)
								{
									if (dinputLightgunBFound && bindingDinputLightGunB.ContainsKey("LightgunX") && bindingDinputLightGunB.ContainsKey("LightgunY"))
									{
										originalJoystickPerGun[2] = (bindingDinputLightGunB["LightgunX"], bindingDinputLightGunB["LightgunY"]);
									}
								}

								ButtonToKeyManager.buttonToKey.originalJoystickPerGun = originalJoystickPerGun;

								Utils.LogMessage($"dinputLightgunAFound = {dinputLightgunAFound}");
								Utils.LogMessage($"dinputLightgunBFound = {dinputLightgunBFound}");

								bool replaceLightgunWithVjoy = ConfigurationManager.MainConfig.indexvjoy > 0 ? true : false;
								vjoyIndex = ConfigurationManager.MainConfig.indexvjoy;
								bool useVjoy = false;

								string LightgunConfigAFile = "";
								string LightgunConfigBFile = "";
								Dictionary<string, string> LightgunConfigA = new Dictionary<string, string>();
								Dictionary<string, string> LightgunConfigB = new Dictionary<string, string>();
								Dictionary<string, string> LightgunConfigFinal = new Dictionary<string, string>();


								if (haveLightgun)
								{

									bool vjoy_recommanded_gunA = ConfigurationManager.MainConfig.gunAvjoy;
									bool vjoy_recommanded_gunB = ConfigurationManager.MainConfig.gunBvjoy;
									vjoy_gunA = false;
									vjoy_gunB = false;
									bool sinden_and_notsinden = false;
									bool all_sinden = false;
									if (dinputLightgunAFound || dinputLightgunBFound) all_sinden = true;
									if (dinputLightgunAFound && GunAType != "sinden") all_sinden = false;
									if (dinputLightgunBFound && GunBType != "sinden") all_sinden = false;

									if (dinputLightgunAFound && dinputLightgunBFound)
									{
										if (GunAType == "sinden" && GunBType != "sinden")
										{
											sinden_and_notsinden = true;
										}
										if (GunBType == "sinden" && GunAType != "sinden")
										{
											sinden_and_notsinden = true;
										}
									}

									string key_vjoy_info = "recommanded_vjoy_fullscreen";
									if (TpSettingsManager.IsWindowed) key_vjoy_info = "recommanded_vjoy_windowed";
									if (useKeepAspectRatio && GameInfo.ContainsKey(key_vjoy_info + "_keepaspectratio") && GameInfo[key_vjoy_info + "_keepaspectratio"].Trim() != "") key_vjoy_info = key_vjoy_info + "_keepaspectratio";
									if (!useKeepAspectRatio && GameInfo.ContainsKey(key_vjoy_info + "_dontkeepaspectratio") && GameInfo[key_vjoy_info + "_dontkeepaspectratio"].Trim() != "") key_vjoy_info = key_vjoy_info + "_dontkeepaspectratio";

									if (GameInfo.ContainsKey(key_vjoy_info) && GameInfo[key_vjoy_info].Trim() != "")
									{
										if (dinputLightgunAFound)
										{
											if (vjoy_recommanded_gunA)
											{
												foreach (var vtag in GameInfo[key_vjoy_info].Trim().Split(','))
												{
													if (vtag == "all")
													{
														vjoy_gunA = true;
														break;
													}
													if (vtag == GunAType)
													{
														vjoy_gunA = true;
														break;
													}
													if (vtag == "sinden_and_notsinden" && sinden_and_notsinden)
													{
														vjoy_gunA = true;
														break;
													}
												}
											}
										}
										if (dinputLightgunBFound)
										{
											if (vjoy_recommanded_gunB)
											{
												foreach (var vtag in GameInfo[key_vjoy_info].Trim().Split(','))
												{
													if (vtag == "all")
													{
														vjoy_gunB = true;
														break;
													}
													if (vtag == GunBType)
													{
														vjoy_gunB = true;
														break;
													}
													if (vtag == "sinden_and_notsinden" && sinden_and_notsinden)
													{
														vjoy_gunB = true;
														break;
													}
												}
											}
										}
										if (GameInfo[key_vjoy_info].Trim() == "not_all_sinden")
										{
											if (all_sinden)
											{
												vjoy_gunA = false;
												vjoy_gunB = false;
											}
											else
											{
												if (dinputLightgunAFound && vjoy_recommanded_gunA) vjoy_gunA = true;
												if (dinputLightgunBFound && vjoy_recommanded_gunB) vjoy_gunB = true;

											}
										}
									}
									if (vjoy_gunA && ConfigurationManager.MainConfig.gunBvjoy == true)
									{
										vjoy_gunB = true;
									}
									if (vjoy_gunB && ConfigurationManager.MainConfig.gunAvjoy == true)
									{
										vjoy_gunA = true;
									}

									useVjoy = true;
									if (gameOptions.gunA_useVjoy > 0)
									{
										if (gameOptions.gunA_useVjoy == 1) vjoy_gunA = false;
										if (gameOptions.gunA_useVjoy == 2) vjoy_gunA = true;
									}
									if (gameOptions.gunB_useVjoy > 0)
									{
										if (gameOptions.gunB_useVjoy == 1) vjoy_gunB = false;
										if (gameOptions.gunB_useVjoy == 2) vjoy_gunB = true;
									}
									if (!vjoy_gunA && !vjoy_gunB) useVjoy = false;

									bool vjoy_installed = false;
									string vjoyPath = Utils.checkInstalled("vJoy");
									if (!string.IsNullOrEmpty(vjoyPath)) vjoy_installed = true;


									//string vjoyGuid = "";
									if (gameOptions.indexvjoy != -1) vjoyIndex = gameOptions.indexvjoy;

									int true_vjoy_index = -1;
									bool vjoy_enabled = false;
									if (haveLightgun && useVjoy && vjoy_installed)
									{
										try
										{
											var vJoyObj = new vJoyManager();
											if (vJoyObj.vJoyEnabled())
											{
												if (vjoyIndex == 0)
												{
													for (uint i = 16; i >= 0; i--)
													{
														VjdStat status = vJoyObj.m_joystick.GetVJDStatus(i);
														if (status == VjdStat.VJD_STAT_FREE)
														{
															true_vjoy_index = (int)i;
															break;
														}
													}
												}
												else if (vjoyIndex > 0)
												{
													VjdStat status = vJoyObj.m_joystick.GetVJDStatus((uint)vjoyIndex);
													if (status == VjdStat.VJD_STAT_FREE)
													{
														true_vjoy_index = vjoyIndex;
													}
												}
												if (true_vjoy_index > 0)
												{
													DirectInput directInput = new DirectInput();
													List<DeviceInstance> devices = new List<DeviceInstance>();
													devices.AddRange(directInput.GetDevices().Where(x => x.Type != SharpDX.DirectInput.DeviceType.Mouse && x.UsagePage != UsagePage.VendorDefinedBegin && x.Usage != UsageId.AlphanumericBitmapSizeX && x.Usage != UsageId.AlphanumericAlphanumericDisplay && x.UsagePage != unchecked((UsagePage)0xffffff43) && x.UsagePage != UsagePage.Vr).ToList());
													Dictionary<string, Joystick> joyList = new Dictionary<string, Joystick>();
													var joystickState = new JoystickState();
													foreach (var device in devices)
													{
														if (device.ProductName.ToLower().Contains("vjoy"))
														{
															Joystick joystick = new Joystick(directInput, device.InstanceGuid);
															joystick.Properties.BufferSize = 512;
															joystick.Acquire();
															joyList.Add(device.InstanceGuid.ToString(), joystick);
														}
													}


													vJoyObj.InitDevice((uint)(true_vjoy_index));
													vJoyObj.SetButton(1, true);
													vJoyObj.SetButton(2, false);
													vJoyObj.SetButton(3, true);
													Thread.Sleep(100);

													foreach (var joy in joyList)
													{
														var joystick = joy.Value;
														joystick.GetCurrentState(ref joystickState);
														if (joystickState != null && joystickState.Buttons.Count() >= 3 && joystickState.Buttons[0] && !joystickState.Buttons[1] && joystickState.Buttons[2])
														{
															VjoyGuid = joy.Key;
														}
														joystick.Dispose();
													}

													vJoyObj.SetButton(1, false);
													vJoyObj.SetButton(2, false);
													vJoyObj.SetButton(3, false);
													vJoyObj.ReleaseDevice();
												}
											}
										}
										catch (Exception ex) { }
									}
									vjoyIndex = true_vjoy_index;

									Utils.LogMessage($"vjoy Index = {vjoyIndex}");
									Utils.LogMessage($"vjoy gunA = {vjoy_gunA}");
									Utils.LogMessage($"vjoy gunB = {vjoy_gunB}");
									Utils.LogMessage($"vjoy use = {useVjoy}");

									if (VjoyGuid != "")
									{
										Utils.LogMessage($"vjoy found ! = {VjoyGuid}");
										if (dinputLightgunAFound && vjoy_gunA)
										{
											if (bindingDinputLightGunA.ContainsKey("LightgunX"))
											{
												bindingDinputLightGunA["LightgunX"].Button = (int)JoystickOffset.X;
												bindingDinputLightGunA["LightgunX"].IsAxis = true;
												bindingDinputLightGunA["LightgunX"].IsAxisMinus = false;
												bindingDinputLightGunA["LightgunX"].IsFullAxis = false;
												bindingDinputLightGunA["LightgunX"].PovDirection = 0;
												bindingDinputLightGunA["LightgunX"].IsReverseAxis = false;
												bindingDinputLightGunA["LightgunX"].JoystickGuid = new Guid(VjoyGuid);
											}
											if (bindingDinputLightGunA.ContainsKey("LightgunY"))
											{
												bindingDinputLightGunA["LightgunY"].Button = (int)JoystickOffset.Y;
												bindingDinputLightGunA["LightgunY"].IsAxis = true;
												bindingDinputLightGunA["LightgunY"].IsAxisMinus = false;
												bindingDinputLightGunA["LightgunY"].IsFullAxis = false;
												bindingDinputLightGunA["LightgunY"].PovDirection = 0;
												bindingDinputLightGunA["LightgunY"].IsReverseAxis = false;
												bindingDinputLightGunA["LightgunY"].JoystickGuid = new Guid(VjoyGuid);
											}
										}

										if (dinputLightgunBFound && vjoy_gunB)
										{
											if (bindingDinputLightGunB.ContainsKey("LightgunX"))
											{
												bindingDinputLightGunB["LightgunX"].Button = (int)JoystickOffset.RotationX;
												bindingDinputLightGunB["LightgunX"].IsAxis = true;
												bindingDinputLightGunB["LightgunX"].IsAxisMinus = false;
												bindingDinputLightGunB["LightgunX"].IsFullAxis = false;
												bindingDinputLightGunB["LightgunX"].PovDirection = 0;
												bindingDinputLightGunB["LightgunX"].IsReverseAxis = false;
												bindingDinputLightGunB["LightgunX"].JoystickGuid = new Guid(VjoyGuid);
											}
											if (bindingDinputLightGunB.ContainsKey("LightgunY"))
											{
												bindingDinputLightGunB["LightgunY"].Button = (int)JoystickOffset.RotationY;
												bindingDinputLightGunB["LightgunY"].IsAxis = true;
												bindingDinputLightGunB["LightgunY"].IsAxisMinus = false;
												bindingDinputLightGunB["LightgunY"].IsFullAxis = false;
												bindingDinputLightGunB["LightgunY"].PovDirection = 0;
												bindingDinputLightGunB["LightgunY"].IsReverseAxis = false;
												bindingDinputLightGunB["LightgunY"].JoystickGuid = new Guid(VjoyGuid);
											}
										}
									}
									else
									{
										Utils.LogMessage($"vjoy not found !");
									}

									if (dinputLightgunAFound)
									{
										string variante = "";
										if (LightgunA_Type == "sinden")
										{
											int sindenPump = ConfigurationManager.MainConfig.gunASidenPump;
											if (gameOptions.gunA_pump > 0) sindenPump = gameOptions.gunA_pump;
											variante = sindenPump.ToString();
										}

										LightgunConfigAFile = Path.Combine(basePath, "config", originalConfigFileNameWithoutExt + ".lightgun-" + LightgunA_Type + variante + ".json");
										if (potentialAltConfigDir != "")
										{
											var LightgunConfigAFileAlt = Path.Combine(potentialAltConfigDir, originalConfigFileNameWithoutExt + ".lightgun-" + LightgunA_Type + variante + ".json");
											if (File.Exists(LightgunConfigAFileAlt)) LightgunConfigAFile = LightgunConfigAFileAlt;
										}

										Utils.LogMessage($"LightGunConfigA = {LightgunConfigAFile}");
										if (File.Exists(LightgunConfigAFile))
										{
											LightgunConfigA = (Dictionary<string, string>)JsonConvert.DeserializeObject<Dictionary<string, string>>(Utils.ReadAllText(LightgunConfigAFile));
										}
										else
										{
											Utils.LogMessage($"LightGunConfigA does not exist");
											dinputLightgunAFound = false;
										}
									}
									if (dinputLightgunBFound)
									{
										string variante = "";
										if (LightgunB_Type == "sinden")
										{
											int sindenPump = ConfigurationManager.MainConfig.gunBSidenPump;
											if (gameOptions.gunB_pump > 0) sindenPump = gameOptions.gunB_pump;
											variante = sindenPump.ToString();
										}

										LightgunConfigBFile = Path.Combine(basePath, "config", originalConfigFileNameWithoutExt + ".lightgun-" + LightgunB_Type + variante + ".json");
										if (potentialAltConfigDir != "")
										{
											var LightgunConfigBFileAlt = Path.Combine(potentialAltConfigDir, originalConfigFileNameWithoutExt + ".lightgun-" + LightgunB_Type + variante + ".json");
											if (File.Exists(LightgunConfigBFileAlt)) LightgunConfigBFile = LightgunConfigBFileAlt;
										}

										Utils.LogMessage($"LightGunConfigB = {LightgunConfigBFile}");
										if (File.Exists(LightgunConfigBFile))
										{
											LightgunConfigB = (Dictionary<string, string>)JsonConvert.DeserializeObject<Dictionary<string, string>>(Utils.ReadAllText(LightgunConfigBFile));
										}
										else
										{
											Utils.LogMessage($"LightGunConfigB does not exist");
											dinputLightgunBFound = false;
										}
									}
									//if(!dinputLightgunAFound && !dinputLightgunBFound)
									if (!dinputLightgunAFound)
									{
										haveLightgun = false;
										dinputLightgunBFound = false;
									}
									else
									{
										Dictionary<string, string> AlreadyAddedGunValue = new Dictionary<string, string>();
										foreach (var lightgunConfig in LightgunConfigA)
										{
											string arcadeKey = lightgunConfig.Key;
											string[] gunValues = lightgunConfig.Value.Split(',');
											string newvalues = "";
											foreach (var gunValue in gunValues)
											{
												if (gunValue.StartsWith("GunA_") || gunValue == "Test" || gunValue == "Service1" || gunValue == "Service2")
												{
													if (!AlreadyAddedGunValue.ContainsKey(gunValue))
													{
														AlreadyAddedGunValue.Add(gunValue, arcadeKey);
													}
												}
											}
										}


										foreach (var lightgunConfig in LightgunConfigB)
										{
											string arcadeKey = lightgunConfig.Key;
											string[] gunValues = lightgunConfig.Value.Split(',');
											string newvalues = "";
											foreach (var gunValue in gunValues)
											{
												if (gunValue.StartsWith("GunB_") || gunValue == "Test" || gunValue == "Service1" || gunValue == "Service2")
												{
													if (!AlreadyAddedGunValue.ContainsKey(gunValue))
													{
														AlreadyAddedGunValue.Add(gunValue, arcadeKey);
													}
												}
											}
										}

										bool reversePedal = false;
										if (ConfigurationManager.MainConfig.reasignPedals)
										{
											string emptyPedal = "";
											string usedPedal = "";
											if (AlreadyAddedGunValue.ContainsKey("GunA_LightgunPedalRight") && !AlreadyAddedGunValue.ContainsKey("GunA_LightgunPedalLeft"))
											{
												emptyPedal = "LightgunPedalLeft";
												usedPedal = "LightgunPedalRight";
											}
											if (!AlreadyAddedGunValue.ContainsKey("GunA_LightgunPedalRight") && AlreadyAddedGunValue.ContainsKey("GunA_LightgunPedalLeft"))
											{
												emptyPedal = "LightgunPedalRight";
												usedPedal = "LightgunPedalLeft";
											}

											if (ConfigurationManager.MainConfig.reversePedals && emptyPedal != "" && AlreadyAddedGunValue.ContainsKey("GunA_LightgunPedalRight") && !AlreadyAddedGunValue.ContainsKey("GunA_LightgunPedalLeft") && AlreadyAddedGunValue.ContainsKey("GunB_LightgunPedalRight") && !AlreadyAddedGunValue.ContainsKey("GunB_LightgunPedalLeft"))
											{
												reversePedal = true;
												emptyPedal = "LightgunPedalRight";
												usedPedal = "LightgunPedalLeft";

												var usedValue = AlreadyAddedGunValue["GunA_LightgunPedalRight"];
												AlreadyAddedGunValue.Add("GunA_LightgunPedalLeft", usedValue);
												AlreadyAddedGunValue["GunA_LightgunPedalRight"] = AlreadyAddedGunValue["GunB_LightgunPedalRight"];

											}
											else
											{
												if (emptyPedal != "" && !AlreadyAddedGunValue.ContainsKey("GunB_" + emptyPedal) && AlreadyAddedGunValue.ContainsKey("GunB_" + usedPedal))
												{
													string valueUsed = AlreadyAddedGunValue["GunB_" + usedPedal];
													AlreadyAddedGunValue.Add("GunA_" + emptyPedal, valueUsed);
												}

											}
										}


										foreach (var alreadyAddedData in AlreadyAddedGunValue)
										{
											string arcadeKey = alreadyAddedData.Value;
											string gunValue = alreadyAddedData.Key;
											if (LightgunConfigFinal.ContainsKey(arcadeKey))
											{
												LightgunConfigFinal[arcadeKey] = LightgunConfigFinal[arcadeKey] + "," + gunValue;
											}
											else
											{
												LightgunConfigFinal.Add(arcadeKey, gunValue);
											}
										}

										//AssignedButtons register buttons used to gunA, to be sure to not reuse the same joyguid + button to gunB
										List<string> AssignedButtons = new List<string>();

										//int gunindex = 0;
										if (dinputLightgunAFound)
										{
											string gunprefix = "GunA_";
											//if(gunindex == 1) gunprefix = "GunB_";

											foreach (var bind in bindingDinputLightGunA)
											{
												if (bind.Key.StartsWith("Lightgun"))
												{
													bindingDinputLightGun.Add(gunprefix + bind.Key, bind.Value);
													string assignedButton = bind.Value.JoystickGuid.ToString() + "===" + bind.Value.Button.ToString() + "===" + bind.Value.PovDirection + "===" + (bind.Value.IsAxis ? "true" : "false") + "===" + (bind.Value.IsAxisMinus ? "true" : "false");
													AssignedButtons.Add(assignedButton);
												}

												if (bind.Key.StartsWith("LightgunTrigger"))
												{
													dinputTriggerGunA = bind.Value;
												}
											}
											//gunindex++;
											if (GunCoinOverwriteA && bindingDinputLightGunA.ContainsKey("LightgunStart") && !bindingDinputLightGunA.ContainsKey("LightgunCoin"))
											{
												JoystickButtonData newCoinBtn = new JoystickButtonData();
												newCoinBtn.Button = (int)int.MinValue;
												newCoinBtn.PovDirection = bindingDinputLightGunA["LightgunStart"].PovDirection;
												newCoinBtn.IsAxis = bindingDinputLightGunA["LightgunStart"].IsAxis;
												newCoinBtn.IsFullAxis = bindingDinputLightGunA["LightgunStart"].IsFullAxis;
												newCoinBtn.IsAxisMinus = bindingDinputLightGunA["LightgunStart"].IsAxisMinus;
												newCoinBtn.IsReverseAxis = bindingDinputLightGunA["LightgunStart"].IsReverseAxis;
												newCoinBtn.JoystickGuid = bindingDinputLightGunA["LightgunStart"].JoystickGuid;
												newCoinBtn.Title = "NEW COIN GUN 1";
												newCoinBtn.XinputTitle = "NEW COIN GUN 1";
												bindingDinputLightGun.Add(gunprefix + "LightgunCoin", newCoinBtn);
												string assignedButton = newCoinBtn.JoystickGuid.ToString() + "===" + newCoinBtn.Button.ToString() + "===" + newCoinBtn.PovDirection + "===" + (newCoinBtn.IsAxis ? "true" : "false") + "===" + (newCoinBtn.IsAxisMinus ? "true" : "false");
												AssignedButtons.Add(assignedButton);
											}
										}
										if (dinputLightgunBFound)
										{
											string gunprefix = "GunB_";
											//if (gunindex == 1) gunprefix = "GunB_";

											foreach (var bind in bindingDinputLightGunB)
											{
												if (bind.Key.StartsWith("Lightgun"))
												{
													string assignedButton = bind.Value.JoystickGuid.ToString() + "===" + bind.Value.Button.ToString() + "===" + bind.Value.PovDirection + "===" + (bind.Value.IsAxis ? "true" : "false") + "===" + (bind.Value.IsAxisMinus ? "true" : "false");
													if (!AssignedButtons.Contains(assignedButton)) bindingDinputLightGun.Add(gunprefix + bind.Key, bind.Value);
												}
												if (bind.Key.StartsWith("LightgunTrigger"))
												{
													dinputTriggerGunB = bind.Value;
												}
											}
											//gunindex++;

											if (GunCoinOverwriteB && bindingDinputLightGunB.ContainsKey("LightgunStart") && !bindingDinputLightGunB.ContainsKey("LightgunCoin"))
											{
												JoystickButtonData newCoinBtn = new JoystickButtonData();
												newCoinBtn.Button = (int)int.MinValue;
												newCoinBtn.PovDirection = bindingDinputLightGunB["LightgunStart"].PovDirection;
												newCoinBtn.IsAxis = bindingDinputLightGunB["LightgunStart"].IsAxis;
												newCoinBtn.IsFullAxis = bindingDinputLightGunB["LightgunStart"].IsFullAxis;
												newCoinBtn.IsAxisMinus = bindingDinputLightGunB["LightgunStart"].IsAxisMinus;
												newCoinBtn.IsReverseAxis = bindingDinputLightGunB["LightgunStart"].IsReverseAxis;
												newCoinBtn.JoystickGuid = bindingDinputLightGunB["LightgunStart"].JoystickGuid;
												newCoinBtn.Title = "NEW COIN GUN 2";
												newCoinBtn.XinputTitle = "NEW COIN GUN 2";

												string assignedButton = newCoinBtn.JoystickGuid.ToString() + "===" + newCoinBtn.Button.ToString() + "===" + newCoinBtn.PovDirection + "===" + (newCoinBtn.IsAxis ? "true" : "false") + "===" + (newCoinBtn.IsAxisMinus ? "true" : "false");
												if (!AssignedButtons.Contains(assignedButton)) bindingDinputLightGun.Add(gunprefix + "LightgunCoin", newCoinBtn);
											}
										}
									}
								}


								//End LightGun Check


								bool checkDinputThrottle = (ConfigurationManager.MainConfig.useDinputHotas && ConfigurationManager.MainConfig.useDinputWheel && ConfigurationManager.MainConfig.useHotasWithWheel);

								Guid throttleGuid = new Guid();
								//bool throttleGuidFound = false;

								Guid shifterGuid = new Guid();
								//bool shifterGuidFound = false;

								bool dinputHotasFound = false;
								if (checkDinputHotas)
								{
									Utils.LogMessage($"check Dinput Hotas");
									if (!string.IsNullOrEmpty(bindingDinputHotasJson))
									{
										bindingDinputHotas = (Dictionary<string, JoystickButtonData>)JsonConvert.DeserializeObject<Dictionary<string, JoystickButtonData>>(bindingDinputHotasJson);
										if (bindingDinputHotas.ContainsKey("InputDevice0LeftThumbInputDevice0X+"))
										{
											HotasGuid = bindingDinputHotas["InputDevice0LeftThumbInputDevice0X+"].JoystickGuid.ToString();
											Utils.LogMessage($"bindingDinputHotasGuid to Search = {HotasGuid}");
										}
									}
									if (!string.IsNullOrEmpty(HotasGuid))
									{
										DirectInput directInput = new DirectInput();
										List<DeviceInstance> devices = new List<DeviceInstance>();
										devices.AddRange(directInput.GetDevices().Where(x => x.Type != SharpDX.DirectInput.DeviceType.Mouse && x.UsagePage != UsagePage.VendorDefinedBegin && x.Usage != UsageId.AlphanumericBitmapSizeX && x.Usage != UsageId.AlphanumericAlphanumericDisplay && x.UsagePage != unchecked((UsagePage)0xffffff43) && x.UsagePage != UsagePage.Vr).ToList());
										foreach (var device in devices)
										{
											if (device.Type == SharpDX.DirectInput.DeviceType.Keyboard && FirstKeyboardGuid == null)
											{
												FirstKeyboardGuid = device.InstanceGuid;
											}
											if (device.InstanceGuid.ToString() == HotasGuid)
											{
												Utils.LogMessage($"HotasGuid Found");
												dinputHotasFound = true;
												haveHotas = true;
											}
										}
									}
								}

								if (checkDinputThrottle)
								{
									Utils.LogMessage($"check Dinput Throttle");
									if (bindingDinputHotas != null && bindingDinputHotas.ContainsKey("InputDevice0RightThumbInputDevice0Y+"))
									{
										throttleGuid = bindingDinputHotas["InputDevice0RightThumbInputDevice0Y+"].JoystickGuid;
										Utils.LogMessage($"bindingDinputThrottleGuid to Search = {throttleGuid}");
									}
									if (!string.IsNullOrEmpty(throttleGuid.ToString()))
									{
										DirectInput directInput = new DirectInput();
										List<DeviceInstance> devices = new List<DeviceInstance>();
										devices.AddRange(directInput.GetDevices().Where(x => x.Type != SharpDX.DirectInput.DeviceType.Mouse && x.UsagePage != UsagePage.VendorDefinedBegin && x.Usage != UsageId.AlphanumericBitmapSizeX && x.Usage != UsageId.AlphanumericAlphanumericDisplay && x.UsagePage != unchecked((UsagePage)0xffffff43) && x.UsagePage != UsagePage.Vr).ToList());
										foreach (var device in devices)
										{
											if (device.InstanceGuid.ToString() == HotasGuid)
											{
												Utils.LogMessage($"ThrottleGuid Found");
												throttleGuidFound = true;
											}
										}
									}
								}

								bool dinputWheelFound = false;
								if (checkDinputWheel)
								{
									Utils.LogMessage($"check Dinput Wheel");
									if (!string.IsNullOrEmpty(bindingDinputWheelJson))
									{
										bindingDinputWheel = (Dictionary<string, JoystickButtonData>)JsonConvert.DeserializeObject<Dictionary<string, JoystickButtonData>>(bindingDinputWheelJson);
										if (bindingDinputWheel.ContainsKey("InputDevice0LeftThumbInputDevice0X+"))
										{
											WheelGuid = bindingDinputWheel["InputDevice0LeftThumbInputDevice0X+"].JoystickGuid.ToString();
											Utils.LogMessage($"WheelGuid to Search = {WheelGuid}");
										}
									}
									if (!string.IsNullOrEmpty(WheelGuid))
									{
										DirectInput directInput = new DirectInput();
										List<DeviceInstance> devices = new List<DeviceInstance>();
										devices.AddRange(directInput.GetDevices().Where(x => x.Type != SharpDX.DirectInput.DeviceType.Mouse && x.UsagePage != UsagePage.VendorDefinedBegin && x.Usage != UsageId.AlphanumericBitmapSizeX && x.Usage != UsageId.AlphanumericAlphanumericDisplay && x.UsagePage != unchecked((UsagePage)0xffffff43) && x.UsagePage != UsagePage.Vr).ToList());
										foreach (var device in devices)
										{
											if (device.Type == SharpDX.DirectInput.DeviceType.Keyboard && FirstKeyboardGuid == null)
											{
												FirstKeyboardGuid = device.InstanceGuid;
											}
											if (device.InstanceGuid.ToString() == WheelGuid)
											{
												Utils.LogMessage($"WheelGuid Found");
												dinputWheelFound = true;
												haveWheel = true;
											}
										}
									}

									if (dinputWheelFound && checkDinputShifter)
									{
										bindingDinputShifter = (Dictionary<string, JoystickButtonData>)JsonConvert.DeserializeObject<Dictionary<string, JoystickButtonData>>(bindingDinputShifterJson);
										if (bindingDinputShifter.ContainsKey("InputDeviceGear1"))
										{
											if (bindingDinputShifter["InputDeviceGear1"].JoystickGuid.ToString() != "")
											{
												shifterGuidFound = true;
												shifterGuid = bindingDinputShifter["InputDeviceGear1"].JoystickGuid;
												Utils.LogMessage($"shifterGuid = {shifterGuid}");
											}

										}
									}
								}




								if (haveWheel && existingConfig.ContainsKey("wheel") && dinputWheelFound)
								{
									useXinput = false;
									useDinputWheel = true;
								}
								if (haveHotas && existingConfig.ContainsKey("hotas") && dinputHotasFound)
								{
									useXinput = false;
									useDinputWheel = false;
									useDinputHotas = true;
								}
								if (haveLightgun && existingConfig.ContainsKey("lightgun"))
								{
									useXinput = false;
									useDinputWheel = false;
									useDinputHotas = false;
									useDinputLightGun = true;
								}

								Utils.LogMessage($"useXinput : {useXinput}");
								Utils.LogMessage($"useDinputWheel : {useDinputWheel}");
								Utils.LogMessage($"useDinputHotas : {useDinputHotas}");
								Utils.LogMessage($"useLightgun : {useDinputLightGun}");

								foreach (var gp in connectedGamePad.Values)
								{
									if (gp.Type == "arcade")
									{
										haveArcade = true;
										break;
									}
								}
								foreach (var gp in connectedGamePad.Values)
								{
									if (gp.Type == "wheel")
									{
										haveWheel = true;
										break;
									}
								}
								foreach (var gp in connectedGamePad.Values)
								{
									if (gp.Type == "gamepad")
									{
										haveGamepad = true;
										break;
									}
								}

								Utils.LogMessage($"haveArcade : {haveArcade}");
								Utils.LogMessage($"haveWheel : {haveWheel}");
								Utils.LogMessage($"haveGamepad : {haveGamepad}");
								if (DebugMode)
								{
									Utils.LogMessage("Connected gamepad List : ");
									foreach (var gp in connectedGamePad.Values)
									{
										Utils.LogMessage($"{gp.ToString()}");
									}
								}



								Dictionary<string, JoystickButton> finalJoystickButtonDictionary = ParseConfig(TpSettingsManager.ModifiedXML, false, true);
								Dictionary<string, JoystickButton> emptyJoystickButtonDictionary = ParseConfig(emptyConfigPath, false);
								Dictionary<int, (string, XinputGamepad)> ConfigPerPlayer = new Dictionary<int, (string, XinputGamepad)>();
								Dictionary<string, JoystickButton> joystickButtonWheel = new Dictionary<string, JoystickButton>();
								Dictionary<string, JoystickButton> joystickButtonArcade = new Dictionary<string, JoystickButton>();
								Dictionary<string, JoystickButton> joystickButtonGamepad = new Dictionary<string, JoystickButton>();
								Dictionary<string, JoystickButton> joystickButtonHotas = new Dictionary<string, JoystickButton>();
								//Dictionary<string, JoystickButton> joystickButtonLightgun = new Dictionary<string, JoystickButton>();

								//Tag Define Part 2
								if (useXinput) TpSettingsManager.tags.Add("xinput");
								else TpSettingsManager.tags.Add("no_xinput");

								if (useDinputWheel) TpSettingsManager.tags.Add("dwheel");
								else TpSettingsManager.tags.Add("no_dwheel");

								if (useDinputHotas) TpSettingsManager.tags.Add("dhotas");
								else TpSettingsManager.tags.Add("no_dhotas");

								if (useDinputLightGun) TpSettingsManager.tags.Add("dlightgun");
								else TpSettingsManager.tags.Add("no_dlightgun");

								if (shifterGuidFound) TpSettingsManager.tags.Add("shifter_found");
								else TpSettingsManager.tags.Add("no_shifter_found");

								if (throttleGuidFound) TpSettingsManager.tags.Add("throttle_found");
								else TpSettingsManager.tags.Add("no_throttle_found");

								if (dinputLightgunAFound) TpSettingsManager.tags.Add("guna_found");
								else TpSettingsManager.tags.Add("no_guna_found");

								if (dinputLightgunBFound) TpSettingsManager.tags.Add("gunb_found");
								else TpSettingsManager.tags.Add("no_gunb_found");

								if (!hideCrosshair) TpSettingsManager.tags.Add("show_crosshair");
								if (hideCrosshair) TpSettingsManager.tags.Add("hide_crosshair");
								if (crosshairA && crosshairB) TpSettingsManager.tags.Add("crosshair_gun1_and_gun2");
								if (crosshairA && !crosshairB) TpSettingsManager.tags.Add("crosshair_gun1_only");
								if (!crosshairA && crosshairB) TpSettingsManager.tags.Add("crosshair_gun2_only");

								if (useDinputLightGun && ((GunAGuid != "" && GunAType == "sinden") || (GunBGuid != "" && GunBType == "sinden")))
								{
									TpSettingsManager.tags.Add("at_least_one_sinden");
									atLeastOneSinden = true;
								}
								else
								{
									atLeastOneSinden = false;
									TpSettingsManager.tags.Add("!at_least_one_sinden");
									if (useDinputLightGun) TpSettingsManager.tags.Add("no_sinden");
								}

								//We only set sinden soft calibration if all sinden are not using vjoy
								if (useDinputLightGun && dinputLightgunAFound)
								{
									if (GunAType == "sinden" && vjoy_gunA) allSindenWithoutVjoy = false;
								}
								if (useDinputLightGun && dinputLightgunBFound)
								{
									if (GunBType == "sinden" && vjoy_gunB) allSindenWithoutVjoy = false;
								}
								if (allSindenWithoutVjoy) TpSettingsManager.tags.Add("no_sinden_using_vjoy");


								//Usefull to set 4/3 mode to gun4ir if vjoy is not enabled on them
								if (useDinputLightGun)
								{
									if (dinputLightgunAFound && ConfigurationManager.MainConfig.gunARecoil == "gun4ir" && vjoy_gunA == false) TpSettingsManager.tags.Add("gun1_is_gun4ir_without_vjoy");
									if (dinputLightgunBFound && ConfigurationManager.MainConfig.gunBRecoil == "gun4ir" && vjoy_gunB == false) TpSettingsManager.tags.Add("gun2_is_gun4ir_without_vjoy");
								}



								if (gameInfoGOSection != null) gameOptions.Overwrite(gameInfoGOSection, TpSettingsManager.tags);
								TpSettingsManager.UpdateXML();





								if (useXinput)
								{
									if (availableSlot != -1 && useVirtualKeyboard)
									{
										bool VigemInstalled = false;
										try
										{
											client = new ViGEmClient();
											VigemInstalled = true;
										}
										catch (VigemBusNotFoundException e) { }

										Utils.LogMessage($"VigemInstalled : {VigemInstalled}");
										if (VigemInstalled)
										{
											if (availableSlot == 0) gamepad = X.Gamepad_1;
											if (availableSlot == 1) gamepad = X.Gamepad_2;
											if (availableSlot == 2) gamepad = X.Gamepad_3;
											if (availableSlot == 3) gamepad = X.Gamepad_4;

											controller = client.CreateXbox360Controller();
											Thread.Sleep(500);
											controller.AutoSubmitReport = false;
											try
											{
												controller.Connect();
											}
											catch (Exception ex)
											{
												Utils.LogMessage(ex.Message);
											}

											Thread.Sleep(1000);
											try
											{
												virtualKeyboardXinputSlot = controller.UserIndex;
											}
											catch (Exception e) { }
											if (virtualKeyboardXinputSlot < 0 || virtualKeyboardXinputSlot > 3)
											{
												int maxloop = 10;
												for (int i = 0; i < maxloop; i++)
												{
													Thread.Sleep(500);
													if (gamepad.Capabilities.Type != 0)
													{
														virtualKeyboardXinputSlot = availableSlot;
														break;
													}
												}
											}

											if (virtualKeyboardXinputSlot < 0 || virtualKeyboardXinputSlot > 3)
											{
												try { controller.Disconnect(); } catch { }
												try { client.Dispose(); } catch { }
											}

											if (virtualKeyboardXinputSlot > -1)
											{
												var assignment = new Dictionary<Combination, Action>();

												if (!string.IsNullOrEmpty(keyTest))
												{
													try
													{
														var keycombi = Combination.FromString(keyTest);
														Action actionPauseMenu = () =>
														{
															ControllerAction(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Button.LeftThumb);

														};
														assignment.Add(keycombi, actionPauseMenu);

													}
													catch (Exception ex) { }
												}
												if (!string.IsNullOrEmpty(keyService1))
												{
													try
													{
														var keycombi = Combination.FromString(keyService1);
														Action actionPauseMenu = () =>
														{
															ControllerAction(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Button.RightThumb);

														};
														assignment.Add(keycombi, actionPauseMenu);

													}
													catch (Exception ex) { }
												}
												if (!string.IsNullOrEmpty(keyService2))
												{
													try
													{
														var keycombi = Combination.FromString(keyService2);
														Action actionPauseMenu = () =>
														{
															ControllerAction(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Button.LeftShoulder);

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

										}
									}



									if (haveWheel && existingConfig.ContainsKey("wheel"))
									{
										Utils.LogMessage($"Assign Wheel");
										joystickButtonWheel = ParseConfig(existingConfig["wheel"]);
										var PlayerList = GetPlayersList(joystickButtonWheel);
										int nb_wheel = connectedGamePad.Values.Where(c => c.Type == "wheel").Count();
										int currentlyAttributed = 0;
										List<XinputGamepad> gamepadList = new List<XinputGamepad>();
										foreach (var cgp in connectedGamePad.Values)
										{
											if (cgp.Type == "wheel")
											{
												gamepadList.Add(cgp);
											}
										}

										foreach (var PlayerXinputSlot in PlayerList)
										{
											if (currentlyAttributed < nb_wheel)
											{
												if (!ConfigPerPlayer.ContainsKey(PlayerXinputSlot))
												{
													ConfigPerPlayer.Add(PlayerXinputSlot, ("wheel", gamepadList[currentlyAttributed]));
													currentlyAttributed++;
												}
											}
										}
									}
									if (haveArcade && existingConfig.ContainsKey("arcade") && (!GameInfo.ContainsKey("priorityGamepadOverArcade") || GameInfo["priorityGamepadOverArcade"].Trim().ToLower() != "true"))
									{
										Utils.LogMessage($"Assign Arcade");
										joystickButtonArcade = ParseConfig(existingConfig["arcade"]);
										var PlayerList = GetPlayersList(joystickButtonArcade);
										int nb_arcade = connectedGamePad.Values.Where(c => c.Type == "arcade").Count();
										int currentlyAttributed = 0;
										List<XinputGamepad> gamepadList = new List<XinputGamepad>();
										foreach (var cgp in connectedGamePad.Values)
										{
											if (cgp.Type == "arcade")
											{
												gamepadList.Add(cgp);
											}
										}

										foreach (var PlayerXinputSlot in PlayerList)
										{
											if (currentlyAttributed < nb_arcade)
											{
												if (!ConfigPerPlayer.ContainsKey(PlayerXinputSlot))
												{
													ConfigPerPlayer.Add(PlayerXinputSlot, ("arcade", gamepadList[currentlyAttributed]));
													currentlyAttributed++;
												}
											}
										}
									}
									if (haveGamepad)
									{
										Utils.LogMessage($"Assign Gamepad");
										string configname = "gamepad";
										if (usealtgamepad) configname = "gamepadalt";
										if (existingConfig.ContainsKey("lightgun")) configname = "lightgun";


										joystickButtonGamepad = ParseConfig(existingConfig[configname]);
										var PlayerList = GetPlayersList(joystickButtonGamepad);
										int nb_gamepad = connectedGamePad.Values.Where(c => c.Type == "gamepad").Count();
										int currentlyAttributed = 0;
										List<XinputGamepad> gamepadList = new List<XinputGamepad>();
										foreach (var cgp in connectedGamePad.Values)
										{
											if (cgp.Type == "gamepad")
											{
												gamepadList.Add(cgp);
											}
										}

										foreach (var PlayerXinputSlot in PlayerList)
										{
											if (currentlyAttributed < nb_gamepad)
											{
												if (!ConfigPerPlayer.ContainsKey(PlayerXinputSlot))
												{
													if (configname == "lightgun") ConfigPerPlayer.Add(PlayerXinputSlot, ("lightgun", gamepadList[currentlyAttributed]));
													else ConfigPerPlayer.Add(PlayerXinputSlot, ("gamepad", gamepadList[currentlyAttributed]));
													currentlyAttributed++;
												}
											}
										}
									}
									if (haveArcade && existingConfig.ContainsKey("arcade") && GameInfo.ContainsKey("priorityGamepadOverArcade") && GameInfo["priorityGamepadOverArcade"].Trim().ToLower() == "true")
									{
										Utils.LogMessage($"Assign Arcade");
										joystickButtonArcade = ParseConfig(existingConfig["arcade"]);
										var PlayerList = GetPlayersList(joystickButtonArcade);
										int nb_arcade = connectedGamePad.Values.Where(c => c.Type == "arcade").Count();
										int currentlyAttributed = 0;
										List<XinputGamepad> gamepadList = new List<XinputGamepad>();
										foreach (var cgp in connectedGamePad.Values)
										{
											if (cgp.Type == "arcade")
											{
												gamepadList.Add(cgp);
											}
										}

										foreach (var PlayerXinputSlot in PlayerList)
										{
											if (currentlyAttributed < nb_arcade)
											{
												if (!ConfigPerPlayer.ContainsKey(PlayerXinputSlot))
												{
													ConfigPerPlayer.Add(PlayerXinputSlot, ("arcade", gamepadList[currentlyAttributed]));
													currentlyAttributed++;
												}
											}
										}
									}

								}
								else
								{
									if (useDinputWheel)
									{
										Utils.LogMessage($"Assign Dinput Wheel");
										if (useVirtualKeyboard && FirstKeyboardGuid != null)
										{

											if (!string.IsNullOrEmpty(keyTest))
											{
												if (Enum.TryParse<SharpDX.DirectInput.Key>(keyTest, out SharpDX.DirectInput.Key resultat))
												{
													var keyData = new JoystickButtonData();
													keyData.Button = (int)resultat + 47;
													keyData.IsAxis = false;
													keyData.IsAxisMinus = false;
													keyData.IsFullAxis = false;
													keyData.PovDirection = 0;
													keyData.IsReverseAxis = false;
													keyData.XinputTitle = "InputDevice10LeftThumb";
													keyData.Title = "Keyboard Button " + keyTest;
													keyData.JoystickGuid = (Guid)FirstKeyboardGuid;
													bindingDinputWheel.Add(keyData.XinputTitle, keyData);
													virtualKeyboardXinputSlot = 10;
												}
											}
											if (!string.IsNullOrEmpty(keyService1))
											{
												if (Enum.TryParse<SharpDX.DirectInput.Key>(keyService1, out SharpDX.DirectInput.Key resultat))
												{
													var keyData = new JoystickButtonData();
													keyData.Button = (int)resultat + 47;
													keyData.IsAxis = false;
													keyData.IsAxisMinus = false;
													keyData.IsFullAxis = false;
													keyData.PovDirection = 0;
													keyData.IsReverseAxis = false;
													keyData.XinputTitle = "InputDevice10RightThumb";
													keyData.Title = "Keyboard Button " + keyTest;
													keyData.JoystickGuid = (Guid)FirstKeyboardGuid;
													bindingDinputWheel.Add(keyData.XinputTitle, keyData);
													virtualKeyboardXinputSlot = 10;
												}
											}
											if (!string.IsNullOrEmpty(keyService2))
											{
												if (Enum.TryParse<SharpDX.DirectInput.Key>(keyService2, out SharpDX.DirectInput.Key resultat))
												{
													var keyData = new JoystickButtonData();
													keyData.Button = (int)resultat + 47;
													keyData.IsAxis = false;
													keyData.IsAxisMinus = false;
													keyData.IsFullAxis = false;
													keyData.PovDirection = 0;
													keyData.IsReverseAxis = false;
													keyData.XinputTitle = "InputDevice10LeftShoulder";
													keyData.Title = "Keyboard Button " + keyTest;
													keyData.JoystickGuid = (Guid)FirstKeyboardGuid;
													bindingDinputWheel.Add(keyData.XinputTitle, keyData);
													virtualKeyboardXinputSlot = 10;
												}
											}
										}
										joystickButtonWheel = ParseConfig(existingConfig["wheel"]);
										XinputGamepad xinputGamepad = new XinputGamepad(0);
										xinputGamepad.Type = "wheel";
										ConfigPerPlayer.Add(0, ("wheel", xinputGamepad));
									}
									if (useDinputHotas)
									{
										Utils.LogMessage($"Assign Dinput Hotas");
										if (useVirtualKeyboard && FirstKeyboardGuid != null)
										{

											if (!string.IsNullOrEmpty(keyTest))
											{
												if (Enum.TryParse<SharpDX.DirectInput.Key>(keyTest, out SharpDX.DirectInput.Key resultat))
												{
													var keyData = new JoystickButtonData();
													keyData.Button = (int)resultat + 47;
													keyData.IsAxis = false;
													keyData.IsAxisMinus = false;
													keyData.IsFullAxis = false;
													keyData.PovDirection = 0;
													keyData.IsReverseAxis = false;
													keyData.XinputTitle = "InputDevice10LeftThumb";
													keyData.Title = "Keyboard Button " + keyTest;
													keyData.JoystickGuid = (Guid)FirstKeyboardGuid;
													bindingDinputHotas.Add(keyData.XinputTitle, keyData);
													virtualKeyboardXinputSlot = 10;
												}
											}
											if (!string.IsNullOrEmpty(keyService1))
											{
												if (Enum.TryParse<SharpDX.DirectInput.Key>(keyService1, out SharpDX.DirectInput.Key resultat))
												{
													var keyData = new JoystickButtonData();
													keyData.Button = (int)resultat + 47;
													keyData.IsAxis = false;
													keyData.IsAxisMinus = false;
													keyData.IsFullAxis = false;
													keyData.PovDirection = 0;
													keyData.IsReverseAxis = false;
													keyData.XinputTitle = "InputDevice10RightThumb";
													keyData.Title = "Keyboard Button " + keyTest;
													keyData.JoystickGuid = (Guid)FirstKeyboardGuid;
													bindingDinputHotas.Add(keyData.XinputTitle, keyData);
													virtualKeyboardXinputSlot = 10;
												}
											}
											if (!string.IsNullOrEmpty(keyService2))
											{
												if (Enum.TryParse<SharpDX.DirectInput.Key>(keyService2, out SharpDX.DirectInput.Key resultat))
												{
													var keyData = new JoystickButtonData();
													keyData.Button = (int)resultat + 47;
													keyData.IsAxis = false;
													keyData.IsAxisMinus = false;
													keyData.IsFullAxis = false;
													keyData.PovDirection = 0;
													keyData.IsReverseAxis = false;
													keyData.XinputTitle = "InputDevice10LeftShoulder";
													keyData.Title = "Keyboard Button " + keyTest;
													keyData.JoystickGuid = (Guid)FirstKeyboardGuid;
													bindingDinputHotas.Add(keyData.XinputTitle, keyData);
													virtualKeyboardXinputSlot = 10;
												}
											}
										}
										joystickButtonHotas = ParseConfig(existingConfig["hotas"]);
										XinputGamepad xinputGamepad = new XinputGamepad(0);
										xinputGamepad.Type = "hotas";
										ConfigPerPlayer.Add(0, ("hotas", xinputGamepad));
									}
									if (useDinputLightGun)
									{
										if (useVirtualKeyboard && FirstKeyboardGuid != null)
										{

											if (!string.IsNullOrEmpty(keyTest))
											{
												if (Enum.TryParse<SharpDX.DirectInput.Key>(keyTest, out SharpDX.DirectInput.Key resultat))
												{
													var keyData = new JoystickButtonData();
													keyData.Button = (int)resultat + 47;
													keyData.IsAxis = false;
													keyData.IsAxisMinus = false;
													keyData.IsFullAxis = false;
													keyData.PovDirection = 0;
													keyData.IsReverseAxis = false;
													keyData.XinputTitle = "Test";
													keyData.Title = "Keyboard Button " + keyTest;
													keyData.JoystickGuid = (Guid)FirstKeyboardGuid;
													bindingDinputLightGun.Add(keyData.XinputTitle, keyData);
													virtualKeyboardXinputSlot = 10;
												}
											}
											if (!string.IsNullOrEmpty(keyService1))
											{
												if (Enum.TryParse<SharpDX.DirectInput.Key>(keyService1, out SharpDX.DirectInput.Key resultat))
												{
													var keyData = new JoystickButtonData();
													keyData.Button = (int)resultat + 47;
													keyData.IsAxis = false;
													keyData.IsAxisMinus = false;
													keyData.IsFullAxis = false;
													keyData.PovDirection = 0;
													keyData.IsReverseAxis = false;
													keyData.XinputTitle = "Service1";
													keyData.Title = "Keyboard Button " + keyTest;
													keyData.JoystickGuid = (Guid)FirstKeyboardGuid;
													bindingDinputLightGun.Add(keyData.XinputTitle, keyData);
													virtualKeyboardXinputSlot = 10;
												}
											}
											if (!string.IsNullOrEmpty(keyService2))
											{
												if (Enum.TryParse<SharpDX.DirectInput.Key>(keyService2, out SharpDX.DirectInput.Key resultat))
												{
													var keyData = new JoystickButtonData();
													keyData.Button = (int)resultat + 47;
													keyData.IsAxis = false;
													keyData.IsAxisMinus = false;
													keyData.IsFullAxis = false;
													keyData.PovDirection = 0;
													keyData.IsReverseAxis = false;
													keyData.XinputTitle = "Service2";
													keyData.Title = "Keyboard Button " + keyTest;
													keyData.JoystickGuid = (Guid)FirstKeyboardGuid;
													bindingDinputLightGun.Add(keyData.XinputTitle, keyData);
													virtualKeyboardXinputSlot = 10;
												}
											}
										}
										if (dinputLightgunAFound)
										{
											XinputGamepad xinputGamepadA = new XinputGamepad(0);
											xinputGamepadA.Type = "lightgun";
											ConfigPerPlayer.Add(0, ("lightgun", xinputGamepadA));
										}

										if (dinputLightgunBFound)
										{
											XinputGamepad xinputGamepadB = new XinputGamepad(0);
											xinputGamepadB.Type = "lightgun";
											ConfigPerPlayer.Add(1, ("lightgun", xinputGamepadB));
										}
										//joystickButtonLightgun = ParseConfig(existingConfig["lightgun"]);

									}
								}


								if (ConfigPerPlayer.Count > 0 && (gamepadStooz || wheelStooz || hotasStooz))
								{
									bool doChangeStooz = false;
									bool enableStooz = false;
									int valueStooz = 0;
									var firstPlayer = ConfigPerPlayer.First();
									if (firstPlayer.Value.Item2.Type == "gamepad" && gamepadStooz)
									{
										doChangeStooz = true;
										enableStooz = enableStoozZone_Gamepad;
										valueStooz = valueStooz_Gamepad;

									}
									if (firstPlayer.Value.Item2.Type == "wheel" && wheelStooz)
									{
										doChangeStooz = true;
										enableStooz = enableStoozZone_Wheel;
										valueStooz = valueStooz_Wheel;
									}
									if (firstPlayer.Value.Item2.Type == "hotas" && hotasStooz)
									{
										doChangeStooz = true;
										enableStooz = enableStoozZone_Hotas;
										valueStooz = valueStooz_Hotas;
									}
									if (firstPlayer.Value.Item2.Type == "lightgun")
									{
										doChangeStooz = true;
										enableStooz = false;
										valueStooz = 0;
									}
									if (doChangeStooz)
									{
										Utils.LogMessage($"Change Stooz");
										if (!String.IsNullOrEmpty(ParrotDataBackup))
										{
											try
											{
												File.Copy(ParrotDataOriginal, ParrotDataBackup, true);
											}
											catch { }
										}
										XmlDocument xmlDocParrotData = new XmlDocument();
										xmlDocParrotData.Load(ParrotDataOriginal);
										XmlNode useSto0ZDrivingHackNode = xmlDocParrotData.SelectSingleNode("/ParrotData/UseSto0ZDrivingHack");
										XmlNode stoozPercentNode = xmlDocParrotData.SelectSingleNode("/ParrotData/StoozPercent");
										useSto0ZDrivingHackNode.InnerText = enableStooz.ToString().ToLower();
										stoozPercentNode.InnerText = valueStooz.ToString();
										xmlDocParrotData.Save(ParrotDataOriginal);
									}

								}

								if (changeFFBConfig && ConfigPerPlayer.Count > 0)
								{
									var firstPlayer = ConfigPerPlayer.First();

									if (!String.IsNullOrEmpty(FFBPluginIniFile) && File.Exists(FFBPluginIniFile))
									{
										Utils.LogMessage($"FFB Ini Exist");
										if (!String.IsNullOrEmpty(FFBPluginIniBackup))
										{
											try
											{
												File.Copy(FFBPluginIniFile, FFBPluginIniBackup, true);
											}
											catch { }
										}
										var ConfigFFB = new IniFile(FFBPluginIniFile);


										if (useXinput)
										{
											SDL2.SDL.SDL_Quit();
											SDL2.SDL.SDL_SetHint(SDL2.SDL.SDL_HINT_JOYSTICK_RAWINPUT, "0");
											SDL2.SDL.SDL_Init(SDL2.SDL.SDL_INIT_JOYSTICK | SDL2.SDL.SDL_INIT_GAMECONTROLLER);
											SDL2.SDL.SDL_JoystickUpdate();
											for (int i = 0; i < SDL2.SDL.SDL_NumJoysticks(); i++)
											{
												var currentJoy = SDL.SDL_JoystickOpen(i);
												string nameController = SDL2.SDL.SDL_JoystickNameForIndex(i).Trim('\0');
												if (nameController.ToLower().StartsWith("xinput") && nameController.ToLower().EndsWith("#" + (firstPlayer.Value.Item2.XinputSlot + 1).ToString()))
												{

													const int bufferSize = 256; // La taille doit �tre au moins 33 pour stocker le GUID sous forme de cha�ne (32 caract�res + le caract�re nul)
													byte[] guidBuffer = new byte[bufferSize];
													SDL.SDL_JoystickGetGUIDString(SDL.SDL_JoystickGetGUID(currentJoy), guidBuffer, bufferSize);
													string guidString = System.Text.Encoding.UTF8.GetString(guidBuffer).Trim('\0');
													ConfigFFB.Write("DeviceGUID", guidString, "Settings");
													Utils.LogMessage($"Change FFB DeviceGUID To {guidString}");
													SDL.SDL_JoystickClose(currentJoy);
													SDL.SDL_JoystickClose(currentJoy);
													break;
												}
												SDL.SDL_JoystickClose(currentJoy);
											}
											SDL2.SDL.SDL_Quit();
										}
										if (useDinputWheel)
										{
											Utils.LogMessage($"Change FFB DeviceGUID To DINPUT {WheelFFBGuid}");
											ConfigFFB.Write("DeviceGUID", WheelFFBGuid, "Settings");
										}
										if (useDinputHotas)
										{
											Utils.LogMessage($"Change FFB DeviceGUID To DINPUT {HotasFFBGuid}");
											ConfigFFB.Write("DeviceGUID", HotasFFBGuid, "Settings");
										}
									}

								}

								if (DebugMode)
								{
									Utils.LogMessage($"ConfigPerPlayer Data :");
									foreach (var ConfigPlayer in ConfigPerPlayer)
									{
										int TargetXinput = ConfigPlayer.Key;
										string ConfigType = ConfigPlayer.Value.Item1;
										XinputGamepad ConfigGamePad = ConfigPlayer.Value.Item2;
										int newXinputSlot = ConfigGamePad.XinputSlot;
										Utils.LogMessage($"TargetXinput = {TargetXinput}, ConfigType = {ConfigType}, newXinputSlot = {newXinputSlot}");
									}
								}
								foreach (var ConfigPlayer in ConfigPerPlayer)
								{
									int TargetXinput = ConfigPlayer.Key;
									string ConfigType = ConfigPlayer.Value.Item1;
									XinputGamepad ConfigGamePad = ConfigPlayer.Value.Item2;
									int newXinputSlot = ConfigGamePad.XinputSlot;

									Dictionary<string, JoystickButton> joystickButtonData = new Dictionary<string, JoystickButton>();
									if (ConfigType == "hotas") joystickButtonData = joystickButtonHotas;
									if (ConfigType == "wheel") joystickButtonData = joystickButtonWheel;
									if (ConfigType == "arcade") joystickButtonData = joystickButtonArcade;
									if (ConfigType == "gamepad") joystickButtonData = joystickButtonGamepad;
									if (ConfigType == "lightgun") joystickButtonData = joystickButtonGamepad;

									foreach (var buttonData in joystickButtonData)
									{
										if (buttonData.Value.XinputSlot == TargetXinput)
										{
											if (emptyJoystickButtonDictionary.ContainsKey(buttonData.Key))
											{
												if (ConfigType == "gamepad") emptyJoystickButtonDictionary[buttonData.Key] = buttonData.Value.RemapButtonData(newXinputSlot, Program.favorJoystick);
												else emptyJoystickButtonDictionary[buttonData.Key] = buttonData.Value.RemapButtonData(newXinputSlot);

											}
										}
									}
								}
								if (useVirtualKeyboard && ConfigPerPlayer.Count() > 0 && virtualKeyboardXinputSlot >= 0)
								{
									string ConfigType = ConfigPerPlayer.First().Value.Item1;
									Dictionary<string, JoystickButton> joystickButtonData = new Dictionary<string, JoystickButton>();
									if (ConfigType == "hotas") joystickButtonData = joystickButtonHotas;
									if (ConfigType == "wheel") joystickButtonData = joystickButtonWheel;
									if (ConfigType == "arcade") joystickButtonData = joystickButtonArcade;
									if (ConfigType == "gamepad") joystickButtonData = joystickButtonGamepad;
									if (ConfigType == "lightgun") joystickButtonData = joystickButtonGamepad;

									foreach (var buttonData in joystickButtonData)
									{
										if (buttonData.Value.XinputSlot == 10)
										{
											emptyJoystickButtonDictionary[buttonData.Key] = buttonData.Value.RemapButtonData(virtualKeyboardXinputSlot);
										}
									}
								}

								/*
								string debutFichier = xmlFileContent.Split("<JoystickButtons>")[0];
								string finFichier = xmlFileContent.Split("</JoystickButtons>").Last();
								string xmlFinalContent = debutFichier + "\n" + "\t<JoystickButtons>";
								foreach (var button in emptyJoystickButtonDictionary)
								{
									xmlFinalContent += button.Value.Xml + "\n";
								}

								xmlFinalContent += "\t</JoystickButtons>" + "\n" + finFichier;

								XmlDocument xmlDoc = new XmlDocument();
								xmlDoc.LoadXml(xmlFinalContent);
								*/

								TpSettingsManager.emptyJoystickButtons(emptyJoystickButtonDictionary);

								// Cr�ez les param�tres pour l'indentation
								XmlWriterSettings settings = new XmlWriterSettings
								{
									Indent = true,
									IndentChars = "    ", // Utilisez la cha�ne que vous pr�f�rez pour l'indentation (par exemple, des espaces ou des tabulations)
									NewLineChars = "\n",
									NewLineHandling = NewLineHandling.Replace
								};

								string xpathExpression = $"/GameProfile/ConfigValues/FieldInformation[FieldName='Input API']/FieldValue";
								XmlNode fieldValueNode = TpSettingsManager.xmlDoc.SelectSingleNode(xpathExpression);

								bool use_dinput = false;
								if (fieldValueNode != null)
								{
									if (useXinput)
									{
										fieldValueNode.InnerText = "XInput";
									}
									if (useDinputWheel || useDinputHotas || useDinputLightGun)
									{
										fieldValueNode.InnerText = "DirectInput";
										use_dinput = true;
									}
								}

								string shiftUpKey = "";
								string shiftDownKey = "";
								string shiftUpKeyBind = "";
								string shiftDownKeyBind = "";
								if (shifterGuidFound && ShifterHack.supportedGames.ContainsKey(originalConfigFileNameWithoutExt))
								{

									shiftUpKey = ShifterHack.getShiftUp(originalConfigFileNameWithoutExt);
									shiftDownKey = ShifterHack.getShifDown(originalConfigFileNameWithoutExt);
								}

								if (useDinputWheel)
								{
									XmlNodeList joystickButtonsNodes = TpSettingsManager.xmlDoc.SelectNodes("/GameProfile/JoystickButtons/JoystickButtons");

									foreach (XmlNode node in joystickButtonsNodes)
									{
										XmlNode existingDirectInputButtonNode = node.SelectSingleNode("DirectInputButton");
										if (existingDirectInputButtonNode != null)
										{
											node.RemoveChild(existingDirectInputButtonNode);
										}
										XmlNode existingBindNameDiNode = node.SelectSingleNode("BindNameDi");
										if (existingBindNameDiNode != null)
										{
											node.RemoveChild(existingBindNameDiNode);
										}

										XmlNode buttonNameNode = node.SelectSingleNode("ButtonName");
										string buttonName = "";
										if (buttonNameNode != null && !string.IsNullOrEmpty(buttonNameNode.InnerText)) buttonName = buttonNameNode.InnerText;

										XmlNode bindNameXiNode = node.SelectSingleNode("BindNameXi");
										if (bindNameXiNode != null && !string.IsNullOrEmpty(bindNameXiNode.InnerText))
										{
											string bindkey = bindNameXiNode.InnerText.Trim().Replace(" ", "");
											if (bindingDinputWheel.ContainsKey(bindkey))
											{
												var bindData = bindingDinputWheel[bindkey];

												XmlNode newDirectInputButtonNode = TpSettingsManager.xmlDoc.CreateElement("DirectInputButton");

												XmlNode buttonNode = TpSettingsManager.xmlDoc.CreateElement("Button");
												buttonNode.InnerText = bindData.Button.ToString();
												newDirectInputButtonNode.AppendChild(buttonNode);

												XmlNode isAxisNode = TpSettingsManager.xmlDoc.CreateElement("IsAxis");
												isAxisNode.InnerText = bindData.IsAxis ? "true" : "false";
												newDirectInputButtonNode.AppendChild(isAxisNode);

												XmlNode IsAxisMinusNode = TpSettingsManager.xmlDoc.CreateElement("IsAxisMinus");
												IsAxisMinusNode.InnerText = bindData.IsAxisMinus ? "true" : "false";
												newDirectInputButtonNode.AppendChild(IsAxisMinusNode);

												XmlNode IsFullAxisNode = TpSettingsManager.xmlDoc.CreateElement("IsFullAxis");
												IsFullAxisNode.InnerText = bindData.IsFullAxis ? "true" : "false";
												newDirectInputButtonNode.AppendChild(IsFullAxisNode);

												XmlNode PovDirectionNode = TpSettingsManager.xmlDoc.CreateElement("PovDirection");
												PovDirectionNode.InnerText = bindData.PovDirection.ToString();
												newDirectInputButtonNode.AppendChild(PovDirectionNode);

												XmlNode IsReverseAxisNode = TpSettingsManager.xmlDoc.CreateElement("IsReverseAxis");
												IsReverseAxisNode.InnerText = bindData.IsReverseAxis ? "true" : "false";
												newDirectInputButtonNode.AppendChild(IsReverseAxisNode);

												XmlNode JoystickGuidNode = TpSettingsManager.xmlDoc.CreateElement("JoystickGuid");
												JoystickGuidNode.InnerText = bindData.JoystickGuid.ToString();
												newDirectInputButtonNode.AppendChild(JoystickGuidNode);

												node.AppendChild(newDirectInputButtonNode);

												XmlNode BindNameDiNode = TpSettingsManager.xmlDoc.CreateElement("BindNameDi");
												BindNameDiNode.InnerText = bindData.Title;
												node.AppendChild(BindNameDiNode);


												if (buttonName == shiftUpKey)
												{
													shiftUpKeyBind = bindData.Title;
												}
												if (buttonName == shiftDownKey)
												{
													shiftDownKeyBind = bindData.Title;
												}
											}
										}


										if (shifterGuidFound && buttonNameNode != null && !string.IsNullOrEmpty(buttonNameNode.InnerText))
										{
											if (shifterData != null && shifterData.ContainsKey(buttonNameNode.InnerText) && bindingDinputShifter != null)
											{
												string deviceKey = shifterData[buttonNameNode.InnerText];
												if (bindingDinputShifter.ContainsKey(deviceKey))
												{
													var bindData = bindingDinputShifter[deviceKey];

													if (bindData.Title != "")
													{

														if (DebugMode)
														{
															Utils.LogMessage($"Shifter Overwrite {buttonNameNode.InnerText} with {bindData.Title}");
														}

														XmlNode existingBindNameDiNode2 = node.SelectSingleNode("BindNameDi");
														if (existingBindNameDiNode2 != null)
														{
															node.RemoveChild(existingBindNameDiNode2);
															if (DebugMode)
															{
																Utils.LogMessage($"Delete existing {buttonNameNode.InnerText}");
															}
														}
														XmlNode existingDirectInputButtonNode2 = node.SelectSingleNode("DirectInputButton");
														if (existingDirectInputButtonNode2 != null)
														{
															node.RemoveChild(existingDirectInputButtonNode2);
														}

														XmlNode newDirectInputButtonNode = TpSettingsManager.xmlDoc.CreateElement("DirectInputButton");

														XmlNode buttonNode = TpSettingsManager.xmlDoc.CreateElement("Button");
														buttonNode.InnerText = bindData.Button.ToString();
														newDirectInputButtonNode.AppendChild(buttonNode);

														XmlNode isAxisNode = TpSettingsManager.xmlDoc.CreateElement("IsAxis");
														isAxisNode.InnerText = bindData.IsAxis ? "true" : "false";
														newDirectInputButtonNode.AppendChild(isAxisNode);

														XmlNode IsAxisMinusNode = TpSettingsManager.xmlDoc.CreateElement("IsAxisMinus");
														IsAxisMinusNode.InnerText = bindData.IsAxisMinus ? "true" : "false";
														newDirectInputButtonNode.AppendChild(IsAxisMinusNode);

														XmlNode IsFullAxisNode = TpSettingsManager.xmlDoc.CreateElement("IsFullAxis");
														IsFullAxisNode.InnerText = bindData.IsFullAxis ? "true" : "false";
														newDirectInputButtonNode.AppendChild(IsFullAxisNode);

														XmlNode PovDirectionNode = TpSettingsManager.xmlDoc.CreateElement("PovDirection");
														PovDirectionNode.InnerText = bindData.PovDirection.ToString();
														newDirectInputButtonNode.AppendChild(PovDirectionNode);

														XmlNode IsReverseAxisNode = TpSettingsManager.xmlDoc.CreateElement("IsReverseAxis");
														IsReverseAxisNode.InnerText = bindData.IsReverseAxis ? "true" : "false";
														newDirectInputButtonNode.AppendChild(IsReverseAxisNode);

														XmlNode JoystickGuidNode = TpSettingsManager.xmlDoc.CreateElement("JoystickGuid");
														JoystickGuidNode.InnerText = bindData.JoystickGuid.ToString();
														newDirectInputButtonNode.AppendChild(JoystickGuidNode);

														node.AppendChild(newDirectInputButtonNode);

														XmlNode BindNameDiNode = TpSettingsManager.xmlDoc.CreateElement("BindNameDi");
														BindNameDiNode.InnerText = bindData.Title;
														node.AppendChild(BindNameDiNode);
													}
												}

											}
										}

										if (throttleGuidFound && buttonNameNode != null && !string.IsNullOrEmpty(buttonNameNode.InnerText))
										{
											if (throttleData != null && throttleData.ContainsKey(buttonNameNode.InnerText) && bindingDinputHotas != null)
											{
												string deviceKey = throttleData[buttonNameNode.InnerText];
												if (bindingDinputHotas.ContainsKey(deviceKey))
												{
													var bindData = bindingDinputHotas[deviceKey];

													if (bindData.Title != "")
													{

														if (DebugMode)
														{
															Utils.LogMessage($"Throttle Overwrite {buttonNameNode.InnerText} with {bindData.Title}");
														}

														XmlNode existingBindNameDiNode2 = node.SelectSingleNode("BindNameDi");
														if (existingBindNameDiNode2 != null)
														{
															node.RemoveChild(existingBindNameDiNode2);
															if (DebugMode)
															{
																Utils.LogMessage($"Delete existing {buttonNameNode.InnerText}");
															}
														}
														XmlNode existingDirectInputButtonNode2 = node.SelectSingleNode("DirectInputButton");
														if (existingDirectInputButtonNode2 != null)
														{
															node.RemoveChild(existingDirectInputButtonNode2);
														}

														XmlNode newDirectInputButtonNode = TpSettingsManager.xmlDoc.CreateElement("DirectInputButton");

														XmlNode buttonNode = TpSettingsManager.xmlDoc.CreateElement("Button");
														buttonNode.InnerText = bindData.Button.ToString();
														newDirectInputButtonNode.AppendChild(buttonNode);

														XmlNode isAxisNode = TpSettingsManager.xmlDoc.CreateElement("IsAxis");
														isAxisNode.InnerText = bindData.IsAxis ? "true" : "false";
														newDirectInputButtonNode.AppendChild(isAxisNode);

														XmlNode IsAxisMinusNode = TpSettingsManager.xmlDoc.CreateElement("IsAxisMinus");
														IsAxisMinusNode.InnerText = bindData.IsAxisMinus ? "true" : "false";
														newDirectInputButtonNode.AppendChild(IsAxisMinusNode);

														XmlNode IsFullAxisNode = TpSettingsManager.xmlDoc.CreateElement("IsFullAxis");
														IsFullAxisNode.InnerText = bindData.IsFullAxis ? "true" : "false";
														newDirectInputButtonNode.AppendChild(IsFullAxisNode);

														XmlNode PovDirectionNode = TpSettingsManager.xmlDoc.CreateElement("PovDirection");
														PovDirectionNode.InnerText = bindData.PovDirection.ToString();
														newDirectInputButtonNode.AppendChild(PovDirectionNode);

														XmlNode IsReverseAxisNode = TpSettingsManager.xmlDoc.CreateElement("IsReverseAxis");
														IsReverseAxisNode.InnerText = bindData.IsReverseAxis ? "true" : "false";
														newDirectInputButtonNode.AppendChild(IsReverseAxisNode);

														XmlNode JoystickGuidNode = TpSettingsManager.xmlDoc.CreateElement("JoystickGuid");
														JoystickGuidNode.InnerText = bindData.JoystickGuid.ToString();
														newDirectInputButtonNode.AppendChild(JoystickGuidNode);

														node.AppendChild(newDirectInputButtonNode);

														XmlNode BindNameDiNode = TpSettingsManager.xmlDoc.CreateElement("BindNameDi");
														BindNameDiNode.InnerText = bindData.Title;
														node.AppendChild(BindNameDiNode);
													}
												}

											}
										}

									}

									if (useShifterHack && shifterGuidFound && shiftDownKeyBind != "" && shiftUpKeyBind != "" && ShifterHack.supportedGames.ContainsKey(originalConfigFileNameWithoutExt))
									{
										Utils.LogMessage($"ShifterHack Start");
										foreach (XmlNode node in joystickButtonsNodes)
										{
											XmlNode bindNameDiNode = node.SelectSingleNode("BindNameDi");
											if (bindNameDiNode != null && !string.IsNullOrEmpty(bindNameDiNode.InnerText))
											{
												string bindkey = bindNameDiNode.InnerText.Trim();
												if (bindkey == shiftUpKeyBind)
												{

													XmlNode existingBindNameDiNode2 = node.SelectSingleNode("BindNameDi");
													if (existingBindNameDiNode2 != null)
													{
														node.RemoveChild(existingBindNameDiNode2);
														if (DebugMode)
														{
															Utils.LogMessage($"Delete existing {existingBindNameDiNode2.InnerText}");
														}
													}
													XmlNode existingDirectInputButtonNode2 = node.SelectSingleNode("DirectInputButton");
													if (existingDirectInputButtonNode2 != null)
													{
														node.RemoveChild(existingDirectInputButtonNode2);
													}


													XmlNode newDirectInputButtonNode = TpSettingsManager.xmlDoc.CreateElement("DirectInputButton");
													XmlNode buttonNode = TpSettingsManager.xmlDoc.CreateElement("Button");
													buttonNode.InnerText = "153";
													newDirectInputButtonNode.AppendChild(buttonNode);
													XmlNode isAxisNode = TpSettingsManager.xmlDoc.CreateElement("IsAxis");
													isAxisNode.InnerText = "false";
													newDirectInputButtonNode.AppendChild(isAxisNode);
													XmlNode IsAxisMinusNode = TpSettingsManager.xmlDoc.CreateElement("IsAxisMinus");
													IsAxisMinusNode.InnerText = "false";
													newDirectInputButtonNode.AppendChild(IsAxisMinusNode);
													XmlNode IsFullAxisNode = TpSettingsManager.xmlDoc.CreateElement("IsFullAxis");
													IsFullAxisNode.InnerText = "false";
													newDirectInputButtonNode.AppendChild(IsFullAxisNode);
													XmlNode PovDirectionNode = TpSettingsManager.xmlDoc.CreateElement("PovDirection");
													PovDirectionNode.InnerText = "0";
													newDirectInputButtonNode.AppendChild(PovDirectionNode);
													XmlNode IsReverseAxisNode = TpSettingsManager.xmlDoc.CreateElement("IsReverseAxis");
													IsReverseAxisNode.InnerText = "false";
													newDirectInputButtonNode.AppendChild(IsReverseAxisNode);
													XmlNode JoystickGuidNode = TpSettingsManager.xmlDoc.CreateElement("JoystickGuid");
													JoystickGuidNode.InnerText = "6f1d2b61-d5a0-11cf-bfc7-444553540000";
													newDirectInputButtonNode.AppendChild(JoystickGuidNode);
													node.AppendChild(newDirectInputButtonNode);
													XmlNode BindNameDiNode = TpSettingsManager.xmlDoc.CreateElement("BindNameDi");
													BindNameDiNode.InnerText = "Keyboard Button 106";
													node.AppendChild(BindNameDiNode);


												}
												if (bindkey == shiftDownKeyBind)
												{
													XmlNode existingBindNameDiNode2 = node.SelectSingleNode("BindNameDi");
													if (existingBindNameDiNode2 != null)
													{
														node.RemoveChild(existingBindNameDiNode2);
														if (DebugMode)
														{
															Utils.LogMessage($"Delete existing {existingBindNameDiNode2.InnerText}");
														}
													}
													XmlNode existingDirectInputButtonNode2 = node.SelectSingleNode("DirectInputButton");
													if (existingDirectInputButtonNode2 != null)
													{
														node.RemoveChild(existingDirectInputButtonNode2);
													}


													XmlNode newDirectInputButtonNode = TpSettingsManager.xmlDoc.CreateElement("DirectInputButton");
													XmlNode buttonNode = TpSettingsManager.xmlDoc.CreateElement("Button");
													buttonNode.InnerText = "158";
													newDirectInputButtonNode.AppendChild(buttonNode);
													XmlNode isAxisNode = TpSettingsManager.xmlDoc.CreateElement("IsAxis");
													isAxisNode.InnerText = "false";
													newDirectInputButtonNode.AppendChild(isAxisNode);
													XmlNode IsAxisMinusNode = TpSettingsManager.xmlDoc.CreateElement("IsAxisMinus");
													IsAxisMinusNode.InnerText = "false";
													newDirectInputButtonNode.AppendChild(IsAxisMinusNode);
													XmlNode IsFullAxisNode = TpSettingsManager.xmlDoc.CreateElement("IsFullAxis");
													IsFullAxisNode.InnerText = "false";
													newDirectInputButtonNode.AppendChild(IsFullAxisNode);
													XmlNode PovDirectionNode = TpSettingsManager.xmlDoc.CreateElement("PovDirection");
													PovDirectionNode.InnerText = "0";
													newDirectInputButtonNode.AppendChild(PovDirectionNode);
													XmlNode IsReverseAxisNode = TpSettingsManager.xmlDoc.CreateElement("IsReverseAxis");
													IsReverseAxisNode.InnerText = "false";
													newDirectInputButtonNode.AppendChild(IsReverseAxisNode);
													XmlNode JoystickGuidNode = TpSettingsManager.xmlDoc.CreateElement("JoystickGuid");
													JoystickGuidNode.InnerText = "6f1d2b61-d5a0-11cf-bfc7-444553540000";
													newDirectInputButtonNode.AppendChild(JoystickGuidNode);
													node.AppendChild(newDirectInputButtonNode);
													XmlNode BindNameDiNode = TpSettingsManager.xmlDoc.CreateElement("BindNameDi");
													BindNameDiNode.InnerText = "Keyboard Button 111";
													node.AppendChild(BindNameDiNode);

												}
											}
										}
										shifterHack = new ShifterHack();
										shifterHack.Start(originalConfigFileNameWithoutExt, shifterGuid.ToString(), WheelGuid, shiftUpKeyBind, shiftDownKeyBind, executableGame, bindingDinputShifter);

									}



								}

								if (useDinputHotas)
								{
									XmlNodeList joystickButtonsNodes = TpSettingsManager.xmlDoc.SelectNodes("/GameProfile/JoystickButtons/JoystickButtons");

									foreach (XmlNode node in joystickButtonsNodes)
									{
										XmlNode existingDirectInputButtonNode = node.SelectSingleNode("DirectInputButton");
										if (existingDirectInputButtonNode != null)
										{
											node.RemoveChild(existingDirectInputButtonNode);
										}
										XmlNode existingBindNameDiNode = node.SelectSingleNode("BindNameDi");
										if (existingBindNameDiNode != null)
										{
											node.RemoveChild(existingBindNameDiNode);
										}

										XmlNode buttonNameNode = node.SelectSingleNode("ButtonName");
										string buttonName = "";
										if (buttonNameNode != null && !string.IsNullOrEmpty(buttonNameNode.InnerText)) buttonName = buttonNameNode.InnerText;

										XmlNode bindNameXiNode = node.SelectSingleNode("BindNameXi");
										if (bindNameXiNode != null && !string.IsNullOrEmpty(bindNameXiNode.InnerText))
										{
											string bindkey = bindNameXiNode.InnerText.Trim().Replace(" ", "");
											if (bindingDinputHotas.ContainsKey(bindkey))
											{
												var bindData = bindingDinputHotas[bindkey];

												XmlNode newDirectInputButtonNode = TpSettingsManager.xmlDoc.CreateElement("DirectInputButton");

												XmlNode buttonNode = TpSettingsManager.xmlDoc.CreateElement("Button");
												buttonNode.InnerText = bindData.Button.ToString();
												newDirectInputButtonNode.AppendChild(buttonNode);

												XmlNode isAxisNode = TpSettingsManager.xmlDoc.CreateElement("IsAxis");
												isAxisNode.InnerText = bindData.IsAxis ? "true" : "false";
												newDirectInputButtonNode.AppendChild(isAxisNode);

												XmlNode IsAxisMinusNode = TpSettingsManager.xmlDoc.CreateElement("IsAxisMinus");
												IsAxisMinusNode.InnerText = bindData.IsAxisMinus ? "true" : "false";
												newDirectInputButtonNode.AppendChild(IsAxisMinusNode);

												XmlNode IsFullAxisNode = TpSettingsManager.xmlDoc.CreateElement("IsFullAxis");
												IsFullAxisNode.InnerText = bindData.IsFullAxis ? "true" : "false";
												newDirectInputButtonNode.AppendChild(IsFullAxisNode);

												XmlNode PovDirectionNode = TpSettingsManager.xmlDoc.CreateElement("PovDirection");
												PovDirectionNode.InnerText = bindData.PovDirection.ToString();
												newDirectInputButtonNode.AppendChild(PovDirectionNode);

												XmlNode IsReverseAxisNode = TpSettingsManager.xmlDoc.CreateElement("IsReverseAxis");
												IsReverseAxisNode.InnerText = bindData.IsReverseAxis ? "true" : "false";
												newDirectInputButtonNode.AppendChild(IsReverseAxisNode);

												XmlNode JoystickGuidNode = TpSettingsManager.xmlDoc.CreateElement("JoystickGuid");
												JoystickGuidNode.InnerText = bindData.JoystickGuid.ToString();
												newDirectInputButtonNode.AppendChild(JoystickGuidNode);

												node.AppendChild(newDirectInputButtonNode);

												XmlNode BindNameDiNode = TpSettingsManager.xmlDoc.CreateElement("BindNameDi");
												BindNameDiNode.InnerText = bindData.Title;
												node.AppendChild(BindNameDiNode);
											}
										}
									}
								}

								if (useDinputLightGun)
								{
									/*
									Dictionary<string, List<string>> FieldsToEnable = new Dictionary<string, List<string>>();
									Dictionary<string, List<string>> FieldsToDisable = new Dictionary<string, List<string>>();
									if(!hideCrosshair && GameInfo.ContainsKey("crosshairON") && GameInfo["crosshairON"] != "")
									{
										foreach (var cFields in GameInfo["crosshairON"].Split('|'))
										{
											if (cFields.Trim() == "") continue;
											var cTuple = cFields.Split(',');
											if (cTuple.Count() == 2)
											{
												string CategoryName = cTuple[0].Trim();
												string FieldName = cTuple[1].Trim();
												if (FieldName.StartsWith("!"))
												{
													FieldName = FieldName.Substring(1).Trim();
													if (!FieldsToDisable.ContainsKey(CategoryName)) FieldsToDisable.Add(CategoryName, new List<string>());
													FieldsToDisable[CategoryName].Add(FieldName);
												}
												else
												{
													if (!FieldsToEnable.ContainsKey(CategoryName)) FieldsToEnable.Add(CategoryName, new List<string>());
													FieldsToEnable[CategoryName].Add(FieldName);
												}
											}
										}
									}
									if (hideCrosshair && GameInfo.ContainsKey("crosshairOFF") && GameInfo["crosshairOFF"] != "")
									{
										foreach (var cFields in GameInfo["crosshairOFF"].Split('|'))
										{
											if (cFields.Trim() == "") continue;
											var cTuple = cFields.Split(',');
											if (cTuple.Count() == 2)
											{
												string CategoryName = cTuple[0].Trim();
												string FieldName = cTuple[1].Trim();
												if (FieldName.StartsWith("!"))
												{
													FieldName = FieldName.Substring(1).Trim();
													if (!FieldsToDisable.ContainsKey(CategoryName)) FieldsToDisable.Add(CategoryName, new List<string>());
													FieldsToDisable[CategoryName].Add(FieldName);
												}
												else
												{
													if (!FieldsToEnable.ContainsKey(CategoryName)) FieldsToEnable.Add(CategoryName, new List<string>());
													FieldsToEnable[CategoryName].Add(FieldName);
												}
											}
										}
									}
									if (crosshairA && GameInfo.ContainsKey("crosshairA") && GameInfo["crosshairA"] != "")
									{
										foreach (var cFields in GameInfo["crosshairA"].Split('|'))
										{
											if (cFields.Trim() == "") continue;
											var cTuple = cFields.Split(',');
											if (cTuple.Count() == 2)
											{
												string CategoryName = cTuple[0].Trim();
												string FieldName = cTuple[1].Trim();
												if (FieldName.StartsWith("!"))
												{
													FieldName = FieldName.Substring(1).Trim();
													if (!FieldsToDisable.ContainsKey(CategoryName)) FieldsToDisable.Add(CategoryName, new List<string>());
													FieldsToDisable[CategoryName].Add(FieldName);
												}
												else
												{
													if (!FieldsToEnable.ContainsKey(CategoryName)) FieldsToEnable.Add(CategoryName, new List<string>());
													FieldsToEnable[CategoryName].Add(FieldName);
												}
											}
										}
									}
									if (!crosshairA && GameInfo.ContainsKey("crosshairA") && GameInfo["crosshairA"] != "")
									{
										foreach (var cFields in GameInfo["crosshairA"].Split('|'))
										{
											if (cFields.Trim() == "") continue;
											var cTuple = cFields.Split(',');
											if (cTuple.Count() == 2)
											{
												string CategoryName = cTuple[0].Trim();
												string FieldName = cTuple[1].Trim();
												if (!FieldName.StartsWith("!"))
												{
													FieldName = FieldName.Substring(1).Trim();
													if (!FieldsToDisable.ContainsKey(CategoryName)) FieldsToDisable.Add(CategoryName, new List<string>());
													FieldsToDisable[CategoryName].Add(FieldName);
												}
												else
												{
													if (!FieldsToEnable.ContainsKey(CategoryName)) FieldsToEnable.Add(CategoryName, new List<string>());
													FieldsToEnable[CategoryName].Add(FieldName);
												}
											}
										}
									}

									if (crosshairB && GameInfo.ContainsKey("crosshairB") && GameInfo["crosshairB"] != "")
									{
										foreach (var cFields in GameInfo["crosshairB"].Split('|'))
										{
											if (cFields.Trim() == "") continue;
											var cTuple = cFields.Split(',');
											if (cTuple.Count() == 2)
											{
												string CategoryName = cTuple[0].Trim();
												string FieldName = cTuple[1].Trim();
												if (FieldName.StartsWith("!"))
												{
													FieldName = FieldName.Substring(1).Trim();
													if (!FieldsToDisable.ContainsKey(CategoryName)) FieldsToDisable.Add(CategoryName, new List<string>());
													FieldsToDisable[CategoryName].Add(FieldName);
												}
												else
												{
													if (!FieldsToEnable.ContainsKey(CategoryName)) FieldsToEnable.Add(CategoryName, new List<string>());
													FieldsToEnable[CategoryName].Add(FieldName);
												}
											}
										}
									}
									if (!crosshairB && GameInfo.ContainsKey("crosshairB") && GameInfo["crosshairB"] != "")
									{
										foreach (var cFields in GameInfo["crosshairB"].Split('|'))
										{
											if (cFields.Trim() == "") continue;
											var cTuple = cFields.Split(',');
											if (cTuple.Count() == 2)
											{
												string CategoryName = cTuple[0].Trim();
												string FieldName = cTuple[1].Trim();
												if (!FieldName.StartsWith("!"))
												{
													FieldName = FieldName.Substring(1).Trim();
													if (!FieldsToDisable.ContainsKey(CategoryName)) FieldsToDisable.Add(CategoryName, new List<string>());
													FieldsToDisable[CategoryName].Add(FieldName);
												}
												else
												{
													if (!FieldsToEnable.ContainsKey(CategoryName)) FieldsToEnable.Add(CategoryName, new List<string>());
													FieldsToEnable[CategoryName].Add(FieldName);
												}
											}
										}
									}
									XmlNodeList fieldNodes = TpSettingsManager.xmlDoc.SelectNodes("//FieldInformation");
									foreach (XmlNode fieldNode in fieldNodes)
									{
										XmlNode categoryNameNode = fieldNode.SelectSingleNode("CategoryName");
										XmlNode fieldNameNode = fieldNode.SelectSingleNode("FieldName");
										XmlNode fieldValueToChangeNode = fieldNode.SelectSingleNode("FieldValue");

										if (categoryNameNode != null && fieldNameNode != null && fieldValueNode != null)
										{
											string categoryNameNodeValue = categoryNameNode.InnerText;
											string fieldNameNodeValue = fieldNameNode.InnerText;


											if (FieldsToEnable.ContainsKey(categoryNameNodeValue))
											{
												if (FieldsToEnable[categoryNameNodeValue].Contains(fieldNameNodeValue))
												{
													fieldValueToChangeNode.InnerText = "1";
												}
											}

											if (FieldsToDisable.ContainsKey(categoryNameNodeValue))
											{
												if (FieldsToDisable[categoryNameNodeValue].Contains(fieldNameNodeValue))
												{
													fieldValueToChangeNode.InnerText = "0";
												}
											}

										}
									}
									*/

									XmlNodeList joystickButtonsNodes = TpSettingsManager.xmlDoc.SelectNodes("/GameProfile/JoystickButtons/JoystickButtons");

									foreach (XmlNode node in joystickButtonsNodes)
									{
										XmlNode existingDirectInputButtonNode = node.SelectSingleNode("DirectInputButton");
										if (existingDirectInputButtonNode != null)
										{
											node.RemoveChild(existingDirectInputButtonNode);
										}
										XmlNode existingBindNameDiNode = node.SelectSingleNode("BindNameDi");
										if (existingBindNameDiNode != null)
										{
											node.RemoveChild(existingBindNameDiNode);
										}

										XmlNode buttonNameNode = node.SelectSingleNode("ButtonName");
										string buttonName = "";
										if (buttonNameNode != null && !string.IsNullOrEmpty(buttonNameNode.InnerText))
										{
											buttonName = buttonNameNode.InnerText;
											if (LightgunConfigFinal.ContainsKey(buttonName))
											{
												var bindkey_list = LightgunConfigFinal[buttonName].Split(',');
												if (bindkey_list.Count() == 1)
												{
													var bindkey = bindkey_list.First();
													if (!string.IsNullOrEmpty(bindkey) && bindingDinputLightGun.ContainsKey(bindkey))
													{
														var bindData = bindingDinputLightGun[bindkey];
														if (((GunCoinOverwriteA && bindkey.StartsWith("GunA_")) || (GunCoinOverwriteB && bindkey.StartsWith("GunB_"))) && (bindkey.EndsWith("_LightgunCoin") || bindkey.EndsWith("_LightgunStart")))
														{
															int coinOrStart = 0;
															if (GunCoinOverwriteA && bindkey.StartsWith("GunA_"))
															{
																coinOrStart = 10;
																if (bindkey.EndsWith("_LightgunCoin")) coinOrStart += 1;
																if (bindkey.EndsWith("_LightgunStart")) coinOrStart += 2;
															}
															if (GunCoinOverwriteB && bindkey.StartsWith("GunB_"))
															{
																coinOrStart = 20;
																if (bindkey.EndsWith("_LightgunCoin")) coinOrStart += 1;
																if (bindkey.EndsWith("_LightgunStart")) coinOrStart += 2;
															}
															string key = ButtonToKeyManager.buttonToKey.GetFreeKey();
															node.AppendChild(NodeFromKey(ButtonToKeyManager.buttonToKey.keyToAssign[key].Item2, TpSettingsManager.xmlDoc));
															ButtonToKeyManager.buttonToKey.Assign(key, bindData.JoystickGuid.ToString(), bindData.Title, coinOrStart);
														}
														else if (bindkey.EndsWith("_LightgunReload"))
														{
															int coinOrStart = 0;
															if (bindkey.StartsWith("GunA_"))
															{
																coinOrStart = 13;
															}
															if (bindkey.StartsWith("GunB_"))
															{
																coinOrStart = 23;
															}
															string key = ButtonToKeyManager.buttonToKey.GetFreeKey();
															node.AppendChild(NodeFromKey(ButtonToKeyManager.buttonToKey.keyToAssign[key].Item2, TpSettingsManager.xmlDoc));
															ButtonToKeyManager.buttonToKey.Assign(key, bindData.JoystickGuid.ToString(), bindData.Title, coinOrStart);
														}
														else if (!bindkey.EndsWith("_LightgunX") && !bindkey.EndsWith("_LightgunY") && bindData.IsAxis)
														{
															string key = ButtonToKeyManager.buttonToKey.GetFreeKey();
															node.AppendChild(NodeFromKey(ButtonToKeyManager.buttonToKey.keyToAssign[key].Item2, TpSettingsManager.xmlDoc));
															ButtonToKeyManager.buttonToKey.Assign(key, bindData.JoystickGuid.ToString(), bindData.Title);
														}
														else
														{
															XmlNode newDirectInputButtonNode = TpSettingsManager.xmlDoc.CreateElement("DirectInputButton");

															XmlNode buttonNode = TpSettingsManager.xmlDoc.CreateElement("Button");
															buttonNode.InnerText = bindData.Button.ToString();
															newDirectInputButtonNode.AppendChild(buttonNode);

															XmlNode isAxisNode = TpSettingsManager.xmlDoc.CreateElement("IsAxis");
															isAxisNode.InnerText = bindData.IsAxis ? "true" : "false";
															newDirectInputButtonNode.AppendChild(isAxisNode);

															XmlNode IsAxisMinusNode = TpSettingsManager.xmlDoc.CreateElement("IsAxisMinus");
															IsAxisMinusNode.InnerText = bindData.IsAxisMinus ? "true" : "false";
															newDirectInputButtonNode.AppendChild(IsAxisMinusNode);

															XmlNode IsFullAxisNode = TpSettingsManager.xmlDoc.CreateElement("IsFullAxis");
															IsFullAxisNode.InnerText = bindData.IsFullAxis ? "true" : "false";
															newDirectInputButtonNode.AppendChild(IsFullAxisNode);

															XmlNode PovDirectionNode = TpSettingsManager.xmlDoc.CreateElement("PovDirection");
															PovDirectionNode.InnerText = bindData.PovDirection.ToString();
															newDirectInputButtonNode.AppendChild(PovDirectionNode);

															XmlNode IsReverseAxisNode = TpSettingsManager.xmlDoc.CreateElement("IsReverseAxis");
															IsReverseAxisNode.InnerText = bindData.IsReverseAxis ? "true" : "false";
															newDirectInputButtonNode.AppendChild(IsReverseAxisNode);

															XmlNode JoystickGuidNode = TpSettingsManager.xmlDoc.CreateElement("JoystickGuid");
															JoystickGuidNode.InnerText = bindData.JoystickGuid.ToString();
															newDirectInputButtonNode.AppendChild(JoystickGuidNode);

															XmlNode JoystickDiNameNode = TpSettingsManager.xmlDoc.CreateElement("DiName");
															JoystickDiNameNode.InnerText = bindData.Title;
															newDirectInputButtonNode.AppendChild(JoystickDiNameNode);

															node.AppendChild(newDirectInputButtonNode);

															XmlNode BindNameDiNode = TpSettingsManager.xmlDoc.CreateElement("BindNameDi");
															BindNameDiNode.InnerText = bindData.Title;
															node.AppendChild(BindNameDiNode);


														}




													}
												}
												else if (bindkey_list.Count() > 1)
												{

													string key = ButtonToKeyManager.buttonToKey.GetFreeKey();
													node.AppendChild(NodeFromKey(ButtonToKeyManager.buttonToKey.keyToAssign[key].Item2, TpSettingsManager.xmlDoc));

													foreach (var bindkey in bindkey_list)
													{
														if (!string.IsNullOrEmpty(bindkey) && bindingDinputLightGun.ContainsKey(bindkey))
														{
															int coinOrStart = 0;
															if (GunCoinOverwriteA && bindkey.StartsWith("GunA_"))
															{
																coinOrStart = 10;
																if (bindkey.EndsWith("_LightgunCoin")) coinOrStart += 1;
																if (bindkey.EndsWith("_LightgunStart")) coinOrStart += 2;
																if (bindkey.EndsWith("_LightgunReload")) coinOrStart += 3;
															}
															if (GunCoinOverwriteB && bindkey.StartsWith("GunB_"))
															{
																coinOrStart = 20;
																if (bindkey.EndsWith("_LightgunCoin")) coinOrStart += 1;
																if (bindkey.EndsWith("_LightgunStart")) coinOrStart += 2;
																if (bindkey.EndsWith("_LightgunReload")) coinOrStart += 3;
															}

															if (bindkey.StartsWith("GunA_") && bindkey.EndsWith("_LightgunReload"))
															{
																coinOrStart = 13;
															}
															if (bindkey.StartsWith("GunB_") && bindkey.EndsWith("_LightgunReload"))
															{
																coinOrStart = 23;
															}
															var bindData = bindingDinputLightGun[bindkey];
															ButtonToKeyManager.buttonToKey.Assign(key, bindData.JoystickGuid.ToString(), bindData.Title, coinOrStart);
														}


													}
												}

												/*
												foreach(var bindkey in bindkey_list)
												{
													if (string.IsNullOrEmpty(bindkey)) continue;
													if (bindingDinputLightGun.ContainsKey(bindkey))
													{
														var bindData = bindingDinputLightGun[bindkey];
														XmlNode newDirectInputButtonNode = xmlDoc.CreateElement("DirectInputButton");

														XmlNode buttonNode = xmlDoc.CreateElement("Button");
														buttonNode.InnerText = bindData.Button.ToString();
														newDirectInputButtonNode.AppendChild(buttonNode);

														XmlNode isAxisNode = xmlDoc.CreateElement("IsAxis");
														isAxisNode.InnerText = bindData.IsAxis ? "true" : "false";
														newDirectInputButtonNode.AppendChild(isAxisNode);

														XmlNode IsAxisMinusNode = xmlDoc.CreateElement("IsAxisMinus");
														IsAxisMinusNode.InnerText = bindData.IsAxisMinus ? "true" : "false";
														newDirectInputButtonNode.AppendChild(IsAxisMinusNode);

														XmlNode IsFullAxisNode = xmlDoc.CreateElement("IsFullAxis");
														IsFullAxisNode.InnerText = bindData.IsFullAxis ? "true" : "false";
														newDirectInputButtonNode.AppendChild(IsFullAxisNode);

														XmlNode PovDirectionNode = xmlDoc.CreateElement("PovDirection");
														PovDirectionNode.InnerText = bindData.PovDirection.ToString();
														newDirectInputButtonNode.AppendChild(PovDirectionNode);

														XmlNode IsReverseAxisNode = xmlDoc.CreateElement("IsReverseAxis");
														IsReverseAxisNode.InnerText = bindData.IsReverseAxis ? "true" : "false";
														newDirectInputButtonNode.AppendChild(IsReverseAxisNode);

														XmlNode JoystickGuidNode = xmlDoc.CreateElement("JoystickGuid");
														JoystickGuidNode.InnerText = bindData.JoystickGuid.ToString();
														newDirectInputButtonNode.AppendChild(JoystickGuidNode);

														XmlNode JoystickDiNameNode = xmlDoc.CreateElement("DiName");
														JoystickDiNameNode.InnerText = bindData.Title;
														newDirectInputButtonNode.AppendChild(JoystickDiNameNode);

														node.AppendChild(newDirectInputButtonNode);

														XmlNode BindNameDiNode = xmlDoc.CreateElement("BindNameDi");
														BindNameDiNode.InnerText = bindData.Title;
														node.AppendChild(BindNameDiNode);
													}
												}
												*/
											}

											/*
											string bindkey = buttonNameNode.InnerText.Trim().Replace(" ", "");
											if (bindingDinputLightGun.ContainsKey(bindkey))
											{
												var bindData = bindingDinputHotas[bindkey];

												XmlNode newDirectInputButtonNode = xmlDoc.CreateElement("DirectInputButton");

												XmlNode buttonNode = xmlDoc.CreateElement("Button");
												buttonNode.InnerText = bindData.Button.ToString();
												newDirectInputButtonNode.AppendChild(buttonNode);

												XmlNode isAxisNode = xmlDoc.CreateElement("IsAxis");
												isAxisNode.InnerText = bindData.IsAxis ? "true" : "false";
												newDirectInputButtonNode.AppendChild(isAxisNode);

												XmlNode IsAxisMinusNode = xmlDoc.CreateElement("IsAxisMinus");
												IsAxisMinusNode.InnerText = bindData.IsAxisMinus ? "true" : "false";
												newDirectInputButtonNode.AppendChild(IsAxisMinusNode);

												XmlNode IsFullAxisNode = xmlDoc.CreateElement("IsFullAxis");
												IsFullAxisNode.InnerText = bindData.IsFullAxis ? "true" : "false";
												newDirectInputButtonNode.AppendChild(IsFullAxisNode);

												XmlNode PovDirectionNode = xmlDoc.CreateElement("PovDirection");
												PovDirectionNode.InnerText = bindData.PovDirection.ToString();
												newDirectInputButtonNode.AppendChild(PovDirectionNode);

												XmlNode IsReverseAxisNode = xmlDoc.CreateElement("IsReverseAxis");
												IsReverseAxisNode.InnerText = bindData.IsReverseAxis ? "true" : "false";
												newDirectInputButtonNode.AppendChild(IsReverseAxisNode);

												XmlNode JoystickGuidNode = xmlDoc.CreateElement("JoystickGuid");
												JoystickGuidNode.InnerText = bindData.JoystickGuid.ToString();
												newDirectInputButtonNode.AppendChild(JoystickGuidNode);

												node.AppendChild(newDirectInputButtonNode);

												XmlNode BindNameDiNode = xmlDoc.CreateElement("BindNameDi");
												BindNameDiNode.InnerText = bindData.Title;
												node.AppendChild(BindNameDiNode);
											}
											*/
										}

									}
								}


								/*
								//Fix Axis on button
								if (use_dinput)
								{
									XmlNodeList joystickButtonsNodes = xmlDoc.SelectNodes("/GameProfile/JoystickButtons/JoystickButtons");

									foreach (XmlNode node in joystickButtonsNodes)
									{
										List<(XmlNode,string,string)> nodesToRemove = new List<(XmlNode, string, string)>();
										XmlNode buttonNameNode = node.SelectSingleNode("ButtonName");
										string buttonName = "";
										if (buttonNameNode != null && !string.IsNullOrEmpty(buttonNameNode.InnerText))
										{
											buttonName = buttonNameNode.InnerText;
										}

										XmlNode AnalogTypeNode = node.SelectSingleNode("AnalogType");
										string analogType = "";
										if (AnalogTypeNode != null && !string.IsNullOrEmpty(AnalogTypeNode.InnerText))
										{
											analogType = AnalogTypeNode.InnerText;
											if (analogType.ToLower() == "none")
											{
												XmlNodeList DirectInputButtonNodes = node.SelectNodes("DirectInputButton");
												foreach (XmlNode directInputButtonNode in DirectInputButtonNodes)
												{

													XmlNode DiNameNode = directInputButtonNode.SelectSingleNode("DiName");
													string diName = "";
													if (DiNameNode != null && !string.IsNullOrEmpty(DiNameNode.InnerText))
													{
														diName = DiNameNode.InnerText;
													}

													XmlNode JoystickGuidNode = directInputButtonNode.SelectSingleNode("JoystickGuid");
													string joystickGuid = "";
													if (JoystickGuidNode != null && !string.IsNullOrEmpty(JoystickGuidNode.InnerText))
													{
														joystickGuid = JoystickGuidNode.InnerText;
													}



													if (diName != "" && joystickGuid != "")
													{
														XmlNode IsAxisNode = directInputButtonNode.SelectSingleNode("IsAxis");
														string isAxis = "";
														if (IsAxisNode != null && !string.IsNullOrEmpty(IsAxisNode.InnerText))
														{
															isAxis = IsAxisNode.InnerText;
															if (isAxis.ToLower() == "true")
															{
																Utils.LogMessage("ERROR FOR " + buttonName);
																nodesToRemove.Add((directInputButtonNode, joystickGuid, diName));
															}
														}
													}

												}
											}
										}

										foreach(var nodeToRemove in nodesToRemove)
										{
											string key = ButtonToKeyManager.buttonToKey.GetFreeKey(nodeToRemove.Item2, nodeToRemove.Item3);
											node.RemoveChild(nodeToRemove.Item1);
											node.AppendChild(NodeFromKey(ButtonToKeyManager.buttonToKey.keyToAssign[key].Item2, xmlDoc));
										}

									}
								}
								ButtonToKeyManager.buttonToKey.StartMonitor();
								*/
								ButtonToKeyManager.buttonToKey.StartMonitor();

								using (XmlWriter xmlWriter = XmlWriter.Create(xmlFile + ".custom.xml", settings))
								{
									// Enregistrez le XmlDocument avec l'indentation dans le XmlWriter
									TpSettingsManager.xmlDoc.Save(xmlWriter);
									finalConfig = xmlFile + ".custom.xml";
								}
								var startupMetadata = MainDPIcs.DeSerializeMetadata(xmlFile);

								Startup.tpBasePath = Directory.GetParent(Path.GetDirectoryName(Path.GetFullPath(xmlFile))).FullName;
								Startup.gameTitle = startupMetadata.game_name;
								Startup.xmlFile = xmlFile;
								Startup.logoPath = Path.Combine(Directory.GetParent(Path.GetDirectoryName(Path.GetFullPath(xmlFile))).FullName, "Icons", startupMetadata.icon_name);
								Startup.playerAttributionDesc = "";
								Startup.ExecutableDir = executableGameDir;
								Startup.ExecutableGame = executableGame;
								/*
								int playernum = 0;
								Startup.imagePaths = new List<string>();
								foreach (var ConfigPlayer in ConfigPerPlayer)
								{
									playernum++;
									string variante = "";
									string upperType = char.ToUpper(ConfigPlayer.Value.Item1[0]) + ConfigPlayer.Value.Item1.Substring(1);

									Startup.playerAttributionDesc += $"P{playernum}={upperType}\n";
									string configname = ConfigPlayer.Value.Item1;
									if (configname == "gamepad" && usealtgamepad) configname = "gamepadalt";

									if (configname == "lightgun" && dinputLightgunAFound && playernum == 1)
									{
										configname = "lightgun-" + LightgunA_Type;

										if (LightgunA_Type == "sinden")
										{
											int sindenPump = ConfigurationManager.MainConfig.gunASidenPump;
											if (gameOptions.gunA_pump > 0) sindenPump = gameOptions.gunA_pump;
											variante = sindenPump.ToString();
										}

									}
									if (configname == "lightgun" && dinputLightgunBFound && playernum == 2)
									{
										configname = "lightgun-" + LightgunB_Type;
										if (LightgunB_Type == "sinden")
										{
											int sindenPump = ConfigurationManager.MainConfig.gunBSidenPump;
											if (gameOptions.gunB_pump > 0) sindenPump = gameOptions.gunB_pump;
											variante = sindenPump.ToString();
										}
									}

									var imgPath = Path.Combine(basePath, "img", originalConfigFileNameWithoutExt + "." + configname + variante + ".jpg");

									Utils.LogMessage($"AddToImgPath : {imgPath}");

									Startup.imagePaths.Add(imgPath);
								}
								Startup.playerAttributionDesc.TrimEnd('\n');
								*/
								List<string> playersDevices = new List<string>();

								List<Image> startupImageLogo = new List<Image>();
								int playernum = 0;
								Startup.imagePaths = new List<string>();
								foreach (var ConfigPlayer in ConfigPerPlayer)
								{
									playernum++;
									string variante = "";
									string upperType = char.ToUpper(ConfigPlayer.Value.Item1[0]) + ConfigPlayer.Value.Item1.Substring(1);
									Startup.playersDevices.Add(ConfigPlayer.Value.Item1.ToLower());
									if(playernum == 1 && ConfigPlayer.Value.Item1.ToLower() == "gamepad")
									{
										firstPlayerIsXinputGamepad = ConfigPlayer.Value.Item2.XinputSlot;
									}
									if (playernum == 1 && ConfigPlayer.Value.Item1.ToLower() == "arcade")
									{
										firstPlayerIsXinputArcade = ConfigPlayer.Value.Item2.XinputSlot;
									}

									Startup.playerAttributionDesc += $"P{playernum}={upperType}\n";
									string configname = ConfigPlayer.Value.Item1;
									if (configname == "gamepad" && usealtgamepad) configname = "gamepadalt";

									if (configname == "lightgun" && dinputLightgunAFound && playernum == 1)
									{
										configname = "lightgun-" + LightgunA_Type;

										if (LightgunA_Type == "sinden")
										{
											int sindenPump = ConfigurationManager.MainConfig.gunASidenPump;
											if (gameOptions.gunA_pump > 0) sindenPump = gameOptions.gunA_pump;
											variante = sindenPump.ToString();
										}

									}
									if (configname == "lightgun" && dinputLightgunBFound && playernum == 2)
									{
										configname = "lightgun-" + LightgunB_Type;
										if (LightgunB_Type == "sinden")
										{
											int sindenPump = ConfigurationManager.MainConfig.gunBSidenPump;
											if (gameOptions.gunB_pump > 0) sindenPump = gameOptions.gunB_pump;
											variante = sindenPump.ToString();
										}
									}

									var imgPath = Path.Combine(basePath, "img", originalConfigFileNameWithoutExt + "." + configname + variante + ".jpg");

									Utils.LogMessage($"AddToImgPath : {imgPath}");

									Startup.imagePaths.Add(imgPath);
								}
								Startup.playerAttributionDesc.TrimEnd('\n');

							}
							else
							{
								if (!fullpassthrough)
								{
									// Cr�ez les param�tres pour l'indentation
									XmlWriterSettings settings = new XmlWriterSettings
									{
										Indent = true,
										IndentChars = "    ", // Utilisez la cha�ne que vous pr�f�rez pour l'indentation (par exemple, des espaces ou des tabulations)
										NewLineChars = "\n",
										NewLineHandling = NewLineHandling.Replace
									};

									using (XmlWriter xmlWriter = XmlWriter.Create(xmlFile + ".custom.xml", settings))
									{
										// Enregistrez le XmlDocument avec l'indentation dans le XmlWriter
										TpSettingsManager.xmlDoc.Save(xmlWriter);
										finalConfig = xmlFile + ".custom.xml";
									}
								}

							}
							//Fin if(FinalConfig=="")
							if (finalConfig != "")
							{
								if (!nolink)
								{
									if (gameOptions.EnableLink && !String.IsNullOrEmpty(linkTargetFolder) && !String.IsNullOrEmpty(linkSourceFolder) && Directory.Exists(linkSourceFolder))
									{
										Utils.LogMessage($"HardLinkFiles {linkSourceFolder}, {linkTargetFolder}");
										if (Utils.IsEligibleHardLink(linkSourceFolder, linkTargetFolder) && linkAutoHardLink)
										{
											Utils.HardLinkFiles(linkSourceFolder, linkTargetFolder, executableGameFile);
										}
										else if (linkAutoSoftLink)
										{
											Utils.HardLinkFiles(linkSourceFolder, linkTargetFolder, executableGameFile, true);
										}
									}
									if (gameOptions.EnableLinkExe && !String.IsNullOrEmpty(linkTargetFolderExe) && !String.IsNullOrEmpty(linkSourceFolderExe) && Directory.Exists(linkSourceFolderExe) && linkExeWritable && linkExePrecheck)
									{
										Utils.LogMessage($"HardLinkFiles {linkSourceFolderExe}, {linkTargetFolderExe}");
										if (Utils.IsEligibleHardLink(linkSourceFolderExe, linkTargetFolderExe) && linkExeHardLink)
										{
											Utils.HardLinkFiles(linkSourceFolderExe, linkTargetFolderExe, executableGameFile);
										}
										else if (linkExeSoftLink)
										{
											Utils.HardLinkFiles(linkSourceFolderExe, linkTargetFolderExe, executableGameFile, true);
										}
									}
								}


								if (!string.IsNullOrEmpty(_dispositionToSwitch) && _dispositionToSwitch != "<none>")
								{
									if (usePresetDisposition)
									{
										if (MonitorSwitcher.SaveDisplaySettings(Path.Combine(Path.GetFullPath(Program.DispositionFolder), "dispositionrestore_app.xml")))
										{
											bool resolutionSwitchOk = true;
											var devices = Westwind.SetResolution.DisplayManager.GetAllDisplayDevices();
											var monitor = devices.FirstOrDefault(d => d.IsSelected);  // main monitor
											var list = Westwind.SetResolution.DisplayManager.GetAllDisplaySettings(monitor.DriverDeviceName);

											var set = list.FirstOrDefault(d => d.Width == presetDispositionWith &&
																	d.Height == presetDispositionHeight &&
																	d.Frequency == presetDispositionFrequency
																	&& d.BitCount == 32 && d.Orientation == 0);

											if (set != null)
											{
												// Initialisation du processus
												Process process = new Process();
												process.StartInfo.FileName = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "QRes", "QRes.exe");
												process.StartInfo.Arguments = $"/x:{presetDispositionWith} /y:{presetDispositionHeight} /r:{presetDispositionFrequency}";

												// Configuration de la sortie standard pour capturer l'output
												process.StartInfo.RedirectStandardOutput = true;
												process.StartInfo.UseShellExecute = false;
												process.StartInfo.CreateNoWindow = true;

												// D�marrage du processus
												process.Start();

												// Lecture de l'output
												string output = process.StandardOutput.ReadToEnd();

												// Attente de la fin du processus
												process.WaitForExit();

												Utils.LogMessage(output);

												if (output.ToLower().Contains("error:")) resolutionSwitchOk = false;
												/*
												try
												{
													Westwind.SetResolution.DisplayManager.SetDisplaySettings(set, monitor.DriverDeviceName);
												}
												catch (Exception ex)
												{
													resolutionSwitchOk = false;
												}
												*/
											}
											else resolutionSwitchOk = false;


											if (resolutionSwitchOk)
											{
												_restoreSwitch = true;
												Thread.Sleep(1000);
											}



										}
									}
									else
									{
										var cfg = Path.Combine(Program.DispositionFolder, "disposition_" + _dispositionToSwitch + ".xml");
										if (File.Exists(cfg))
										{
											Utils.LogMessage($"Disposition {cfg} Exist, save Current Disposition");
											if (MonitorSwitcher.SaveDisplaySettings(Path.Combine(Path.GetFullPath(Program.DispositionFolder), "dispositionrestore_app.xml")))
											{
												if (UseMonitorDisposition(_dispositionToSwitch))
												{
													_restoreSwitch = true;
													Thread.Sleep(1000);
												}
											}
										}
										//------------------

										if (gameOptions.UseGlobalDisposition && Program.need60hz && Program.refreshRate != 60 && Program.currentResX > 0)
										{
											int target_refreshrate = 60;
											usePresetDisposition = true;
											presetDispositionWith = presetDispositionWith > 0 ? presetDispositionWith : Program.currentResX;
											presetDispositionHeight = presetDispositionHeight > 0 ? presetDispositionHeight : Program.currentResY;
											presetDispositionFrequency = target_refreshrate;

											var devices = Westwind.SetResolution.DisplayManager.GetAllDisplayDevices();
											var monitor = devices.FirstOrDefault(d => d.IsSelected);  // main monitor
											var list = Westwind.SetResolution.DisplayManager.GetAllDisplaySettings(monitor.DriverDeviceName);

											var resolutionAvailiable = list
												.Where(d => d.BitCount == 32 && d.Orientation == 0)
												.OrderBy(d => d.Width)
												.ThenBy(d => d.Height)
												.ThenBy(d => d.Frequency)
												.FirstOrDefault(d => d.Width == presetDispositionWith &&
																	 d.Height == presetDispositionHeight &&
																	 d.Frequency == presetDispositionFrequency);

											if (resolutionAvailiable == null)
											{
												//Resolution juste en dessous mais sans decendre en dessous de 1, par exemple 59Hz au lieu de 60hz
												resolutionAvailiable = list
												.Where(d => d.Width == presetDispositionWith &&
															d.Height == presetDispositionHeight &&
															d.BitCount == 32 &&
																d.Orientation == 0 &&
																d.Frequency < presetDispositionFrequency &&
																d.Frequency >= presetDispositionFrequency - 1)
												.OrderByDescending(d => d.Frequency)
												.FirstOrDefault();
											}
											if (resolutionAvailiable == null)
											{
												// Chercher la r�solution imm�diatement au-dessus de 60 Hz
												resolutionAvailiable = list
													.Where(d => d.Width == presetDispositionWith &&
																d.Height == presetDispositionHeight &&
																d.BitCount == 32 &&
																d.Orientation == 0 &&
																d.Frequency > presetDispositionFrequency)
													.OrderBy(d => d.Frequency)
													.FirstOrDefault();
											}
											// Si aucune r�solution n'est trouv�e au-dessus, chercher la r�solution imm�diatement en dessous de 60 Hz
											if (resolutionAvailiable == null)
											{
												resolutionAvailiable = list
													.Where(d => d.Width == presetDispositionWith &&
																d.Height == presetDispositionHeight &&
																d.BitCount == 32 &&
																d.Orientation == 0 &&
																d.Frequency < presetDispositionFrequency)
													.OrderByDescending(d => d.Frequency)
													.FirstOrDefault();
											}

											if (resolutionAvailiable != null)
											{
												_dispositionToSwitch = "custom";
												presetDispositionFrequency = resolutionAvailiable.Frequency;
											}
											else presetDispositionFrequency = 60;

											Utils.LogMessage($"Change mainRes to {presetDispositionWith}x{presetDispositionHeight}:{presetDispositionFrequency} (need60hz)");

											Program.refreshRate = presetDispositionFrequency;
											refreshrate_done = true;

										}

										if (TpSettingsManager.tags.Contains("fullscreen") && gameOptions.UseGlobalDisposition && Program.forceResSwitchFullscreen && Program.currentResX > 0)
										{
											int target_refreshrate = Program.refreshRate;
											if (need60hz) target_refreshrate = 60;
											int screenWidth = Program.currentResX;
											int screenHeight = Program.currentResY;
											bool changeres = false;
											if (Program.gpuResolution == 0 && (screenWidth != 1280 || screenHeight != 720))
											{
												usePresetDisposition = true;
												presetDispositionWith = 1280;
												presetDispositionHeight = 720;
												presetDispositionFrequency = target_refreshrate;
												changeres = true;
											}
											if (Program.gpuResolution == 1 && (screenWidth != 1920 || screenHeight != 1080))
											{
												usePresetDisposition = true;
												presetDispositionWith = 1920;
												presetDispositionHeight = 1080;
												presetDispositionFrequency = target_refreshrate;
												changeres = true;
											}
											if (Program.gpuResolution == 2 && (screenWidth != 2560 || screenHeight != 1440))
											{
												usePresetDisposition = true;
												presetDispositionWith = 2560;
												presetDispositionHeight = 1440;
												presetDispositionFrequency = target_refreshrate;
												changeres = true;
											}
											if (Program.gpuResolution == 3 && (screenWidth != 3840 || screenHeight != 2160))
											{
												usePresetDisposition = true;
												presetDispositionWith = 3840;
												presetDispositionHeight = 2160;
												presetDispositionFrequency = target_refreshrate;
												changeres = true;
											}
											if (changeres)
											{

												var devices = Westwind.SetResolution.DisplayManager.GetAllDisplayDevices();
												var monitor = devices.FirstOrDefault(d => d.IsSelected);  // main monitor
												var list = Westwind.SetResolution.DisplayManager.GetAllDisplaySettings(monitor.DriverDeviceName);

												var resolutionAvailiable = list
													.Where(d => d.BitCount == 32 && d.Orientation == 0)
													.OrderBy(d => d.Width)
													.ThenBy(d => d.Height)
													.ThenBy(d => d.Frequency)
													.FirstOrDefault(d => d.Width == presetDispositionWith &&
																		 d.Height == presetDispositionHeight &&
																		 d.Frequency == presetDispositionFrequency);

												if (resolutionAvailiable == null)
												{
													//Resolution juste en dessous mais sans decendre en dessous de 1, par exemple 59Hz au lieu de 60hz
													resolutionAvailiable = list
													.Where(d => d.Width == presetDispositionWith &&
																d.Height == presetDispositionHeight &&
																d.BitCount == 32 &&
																	d.Orientation == 0 &&
																	d.Frequency < presetDispositionFrequency &&
																	d.Frequency >= presetDispositionFrequency - 1)
													.OrderByDescending(d => d.Frequency)
													.FirstOrDefault();
												}
												if (resolutionAvailiable == null)
												{
													// Chercher la r�solution imm�diatement au-dessus de 60 Hz
													resolutionAvailiable = list
														.Where(d => d.Width == presetDispositionWith &&
																	d.Height == presetDispositionHeight &&
																	d.BitCount == 32 &&
																	d.Orientation == 0 &&
																	d.Frequency > presetDispositionFrequency)
														.OrderBy(d => d.Frequency)
														.FirstOrDefault();
												}
												// Si aucune r�solution n'est trouv�e au-dessus, chercher la r�solution imm�diatement en dessous de 60 Hz
												if (resolutionAvailiable == null)
												{
													resolutionAvailiable = list
														.Where(d => d.Width == presetDispositionWith &&
																	d.Height == presetDispositionHeight &&
																	d.BitCount == 32 &&
																	d.Orientation == 0 &&
																	d.Frequency < presetDispositionFrequency)
														.OrderByDescending(d => d.Frequency)
														.FirstOrDefault();
												}
												if (resolutionAvailiable != null)
												{
													_dispositionToSwitch = "custom";
													presetDispositionFrequency = resolutionAvailiable.Frequency;
												}
												else presetDispositionFrequency = 60;

												Utils.LogMessage($"Change mainRes to {presetDispositionWith}x{presetDispositionHeight}:{presetDispositionFrequency} (forceResSwitchFullscreen)");

												Program.refreshRate = presetDispositionFrequency;
												refreshrate_done = true;
											}

										}

										if (TpSettingsManager.tags.Contains("windowed") && gameOptions.UseGlobalDisposition && Program.forceResSwitchWindowed && Program.currentResX > 0)
										{
											int target_refreshrate = Program.refreshRate;
											if (need60hz) target_refreshrate = 60;
											int screenWidth = Program.currentResX;
											int screenHeight = Program.currentResY;
											bool changeres = false;
											if (Program.gpuResolution == 0 && (screenWidth != 1280 || screenHeight != 720))
											{
												usePresetDisposition = true;
												presetDispositionWith = 1280;
												presetDispositionHeight = 720;
												presetDispositionFrequency = target_refreshrate;
												changeres = true;
											}
											if (Program.gpuResolution == 1 && (screenWidth != 1920 || screenHeight != 1080))
											{
												usePresetDisposition = true;
												presetDispositionWith = 1920;
												presetDispositionHeight = 1080;
												presetDispositionFrequency = target_refreshrate;
												changeres = true;
											}
											if (Program.gpuResolution == 2 && (screenWidth != 2560 || screenHeight != 1440))
											{
												usePresetDisposition = true;
												presetDispositionWith = 2560;
												presetDispositionHeight = 1440;
												presetDispositionFrequency = target_refreshrate;
												changeres = true;
											}
											if (Program.gpuResolution == 3 && (screenWidth != 3840 || screenHeight != 2160))
											{
												usePresetDisposition = true;
												presetDispositionWith = 3840;
												presetDispositionHeight = 2160;
												presetDispositionFrequency = target_refreshrate;
												changeres = true;
											}
											if (changeres)
											{

												var devices = Westwind.SetResolution.DisplayManager.GetAllDisplayDevices();
												var monitor = devices.FirstOrDefault(d => d.IsSelected);  // main monitor
												var list = Westwind.SetResolution.DisplayManager.GetAllDisplaySettings(monitor.DriverDeviceName);

												var resolutionAvailiable = list
													.Where(d => d.BitCount == 32 && d.Orientation == 0)
													.OrderBy(d => d.Width)
													.ThenBy(d => d.Height)
													.ThenBy(d => d.Frequency)
													.FirstOrDefault(d => d.Width == presetDispositionWith &&
																		 d.Height == presetDispositionHeight &&
																		 d.Frequency == presetDispositionFrequency);

												if (resolutionAvailiable == null)
												{
													//Resolution juste en dessous mais sans decendre en dessous de 1, par exemple 59Hz au lieu de 60hz
													resolutionAvailiable = list
													.Where(d => d.Width == presetDispositionWith &&
																d.Height == presetDispositionHeight &&
																d.BitCount == 32 &&
																	d.Orientation == 0 &&
																	d.Frequency < presetDispositionFrequency &&
																	d.Frequency >= presetDispositionFrequency - 1)
													.OrderByDescending(d => d.Frequency)
													.FirstOrDefault();
												}
												if (resolutionAvailiable == null)
												{
													// Chercher la r�solution imm�diatement au-dessus de 60 Hz
													resolutionAvailiable = list
														.Where(d => d.Width == presetDispositionWith &&
																	d.Height == presetDispositionHeight &&
																	d.BitCount == 32 &&
																	d.Orientation == 0 &&
																	d.Frequency > presetDispositionFrequency)
														.OrderBy(d => d.Frequency)
														.FirstOrDefault();
												}
												// Si aucune r�solution n'est trouv�e au-dessus, chercher la r�solution imm�diatement en dessous de 60 Hz
												if (resolutionAvailiable == null)
												{
													resolutionAvailiable = list
														.Where(d => d.Width == presetDispositionWith &&
																	d.Height == presetDispositionHeight &&
																	d.BitCount == 32 &&
																	d.Orientation == 0 &&
																	d.Frequency < presetDispositionFrequency)
														.OrderByDescending(d => d.Frequency)
														.FirstOrDefault();
												}
												if (resolutionAvailiable != null)
												{
													_dispositionToSwitch = "custom";
													presetDispositionFrequency = resolutionAvailiable.Frequency;
												}
												else presetDispositionFrequency = 60;

												Utils.LogMessage($"Change mainRes to {presetDispositionWith}x{presetDispositionHeight}:{presetDispositionFrequency} (forceResSwitchFullscreen)");

												Program.refreshRate = presetDispositionFrequency;
												refreshrate_done = true;
											}

										}

										if (TpSettingsManager.tags.Contains("windowed") && gameOptions.UseGlobalDisposition && Program.upscaleWindowed && Program.currentResX > 0)
										{
											int target_refreshrate = Program.refreshRate;
											if (need60hz) target_refreshrate = 60;
											int screenWidth = Program.currentResX;
											int screenHeight = Program.currentResY;
											//MessageBox.Show($"screenWidth = {screenWidth} screenHeight= {screenHeight}");
											//if (Program.gpuResolution != 0) TpSettingsManager.tags.Add("!720p");
											//if (Program.gpuResolution != 1) TpSettingsManager.tags.Add("!1080p");
											//if (Program.gpuResolution != 2) TpSettingsManager.tags.Add("!2k");
											//if (Program.gpuResolution != 3) TpSettingsManager.tags.Add("!4k");
											bool changeres = false;
											if (Program.gpuResolution == 0 && (screenWidth < 1280 || screenHeight < 720))
											{
												usePresetDisposition = true;
												presetDispositionWith = 1280;
												presetDispositionHeight = 720;
												presetDispositionFrequency = target_refreshrate;
												changeres = true;
											}
											if (Program.gpuResolution == 1 && (screenWidth < 1920 || screenHeight < 1080))
											{
												usePresetDisposition = true;
												presetDispositionWith = 1920;
												presetDispositionHeight = 1080;
												presetDispositionFrequency = target_refreshrate;
												changeres = true;
											}
											if (Program.gpuResolution == 2 && (screenWidth < 2560 || screenHeight < 1440))
											{
												usePresetDisposition = true;
												presetDispositionWith = 2560;
												presetDispositionHeight = 1440;
												presetDispositionFrequency = target_refreshrate;
												changeres = true;
											}
											if (Program.gpuResolution == 3 && (screenWidth < 3840 || screenHeight < 2160))
											{
												usePresetDisposition = true;
												presetDispositionWith = 3840;
												presetDispositionHeight = 2160;
												presetDispositionFrequency = target_refreshrate;
												changeres = true;
											}
											if (changeres)
											{

												var devices = Westwind.SetResolution.DisplayManager.GetAllDisplayDevices();
												var monitor = devices.FirstOrDefault(d => d.IsSelected);  // main monitor
												var list = Westwind.SetResolution.DisplayManager.GetAllDisplaySettings(monitor.DriverDeviceName);

												var resolutionAvailiable = list
													.Where(d => d.BitCount == 32 && d.Orientation == 0)
													.OrderBy(d => d.Width)
													.ThenBy(d => d.Height)
													.ThenBy(d => d.Frequency)
													.FirstOrDefault(d => d.Width == presetDispositionWith &&
																		 d.Height == presetDispositionHeight &&
																		 d.Frequency == presetDispositionFrequency);

												if (resolutionAvailiable == null)
												{
													//Resolution juste en dessous mais sans decendre en dessous de 1, par exemple 59Hz au lieu de 60hz
													resolutionAvailiable = list
													.Where(d => d.Width == presetDispositionWith &&
																d.Height == presetDispositionHeight &&
																d.BitCount == 32 &&
																	d.Orientation == 0 &&
																	d.Frequency < presetDispositionFrequency &&
																	d.Frequency >= presetDispositionFrequency - 1)
													.OrderByDescending(d => d.Frequency)
													.FirstOrDefault();
												}
												if (resolutionAvailiable == null)
												{
													// Chercher la r�solution imm�diatement au-dessus de 60 Hz
													resolutionAvailiable = list
														.Where(d => d.Width == presetDispositionWith &&
																	d.Height == presetDispositionHeight &&
																	d.BitCount == 32 &&
																	d.Orientation == 0 &&
																	d.Frequency > presetDispositionFrequency)
														.OrderBy(d => d.Frequency)
														.FirstOrDefault();
												}
												// Si aucune r�solution n'est trouv�e au-dessus, chercher la r�solution imm�diatement en dessous de 60 Hz
												if (resolutionAvailiable == null)
												{
													resolutionAvailiable = list
														.Where(d => d.Width == presetDispositionWith &&
																	d.Height == presetDispositionHeight &&
																	d.BitCount == 32 &&
																	d.Orientation == 0 &&
																	d.Frequency < presetDispositionFrequency)
														.OrderByDescending(d => d.Frequency)
														.FirstOrDefault();
												}
												if (resolutionAvailiable != null)
												{
													_dispositionToSwitch = "custom";
													presetDispositionFrequency = resolutionAvailiable.Frequency;
												}
												else presetDispositionFrequency = 60;

												Utils.LogMessage($"Change mainRes to {presetDispositionWith}x{presetDispositionHeight}:{presetDispositionFrequency} (upscaleWindowed)");

												Program.refreshRate = presetDispositionFrequency;
												refreshrate_done = true;
											}


										}



										if (usePresetDisposition)
										{
											{
												bool resolutionSwitchOk = true;
												var devices = Westwind.SetResolution.DisplayManager.GetAllDisplayDevices();
												var monitor = devices.FirstOrDefault(d => d.IsSelected);  // main monitor
												var list = Westwind.SetResolution.DisplayManager.GetAllDisplaySettings(monitor.DriverDeviceName);

												var set = list.FirstOrDefault(d => d.Width == presetDispositionWith &&
																		d.Height == presetDispositionHeight &&
																		d.Frequency == presetDispositionFrequency
																		&& d.BitCount == 32 && d.Orientation == 0);

												if (set != null)
												{
													// Initialisation du processus
													Process process = new Process();
													process.StartInfo.FileName = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "QRes", "QRes.exe");
													process.StartInfo.Arguments = $"/x:{presetDispositionWith} /y:{presetDispositionHeight} /r:{presetDispositionFrequency}";
													// Configuration de la sortie standard pour capturer l'output
													process.StartInfo.RedirectStandardOutput = true;
													process.StartInfo.UseShellExecute = false;
													process.StartInfo.CreateNoWindow = true;

													// D�marrage du processus
													process.Start();

													// Lecture de l'output
													string output = process.StandardOutput.ReadToEnd();

													// Attente de la fin du processus
													process.WaitForExit();

													Utils.LogMessage(output);

													if (output.ToLower().Contains("error:")) resolutionSwitchOk = false;
													/*
													try
													{
														Westwind.SetResolution.DisplayManager.SetDisplaySettings(set, monitor.DriverDeviceName);
													}
													catch (Exception ex)
													{
														resolutionSwitchOk = false;
													}
													*/
												}
												else resolutionSwitchOk = false;


												if (resolutionSwitchOk)
												{
													Thread.Sleep(1000);
												}



											}
										}



										//------------------



									}

								}
								if (gameOptions.disableAllMonitorExceptPrimary)
								{
									if (!_restoreSwitch)
									{
										if (MonitorSwitcher.SaveDisplaySettings(Path.Combine(Path.GetFullPath(Program.DispositionFolder), "dispositionrestore_app.xml")))
										{
											_restoreSwitch = true;
										}
										var displays = Display.GetDisplays().ToArray();
										string MainMonitorName = Screen.PrimaryScreen.DeviceName;
										foreach (Display display in displays.Where(display => !display.IsGDIPrimary))
										{
											display.Disable(false);
										}

										DisplaySetting.ApplySavedSettings();
									}
								}

								if (useDinputLightGun)
								{
									string RumbleTypeA = "<none>";
									string RumbleParameterA = "";
									string RumbleTypeB = "<none>";
									string RumbleParameterB = "";

									bool gunAAutoJoy = false;
									bool gunBAutoJoy = false;
									bool gunADamageRumble = false;
									bool gunBDamageRumble = false;
									bool gunA4tiers = false;
									bool gunB4tiers = false;

									int sindenRumbleGunASet = 0;
									if (GunAGuid != "")
									{
										bool AutoJoy = false;
										bool DamageRumble = false;
										bool Fourtiers = false;

										string RumbleType = ConfigurationManager.MainConfig.gunARecoil;
										if (gameOptions.gunA_recoil == 1) RumbleType = "<none>";
										if (gameOptions.gunA_recoil == 2) RumbleType = "mamehooker";

										string RumbleParameter = "";

										if (RumbleType == "gun4ir")
										{


											int comPort = ConfigurationManager.MainConfig.gunAComPort;
											if (comPort > 0)
											{
												if (gun_preconfigDPI.isValidGun4irPort(comPort))
												{
													RumbleParameter = "COM" + comPort.ToString();
													if (ConfigurationManager.MainConfig.gunAAutoJoy) AutoJoy = true;
													if (ConfigurationManager.MainConfig.gunAdomagerumble) DamageRumble = true;
													if (ConfigurationManager.MainConfig.gunA4tiers) Fourtiers = true;
													if (gameOptions.gunA_4tiers > 0)
													{
														if (gameOptions.gunA_4tiers == 1) Fourtiers = false;
														if (gameOptions.gunA_4tiers == 2) Fourtiers = true;
													}
												}
												else
												{
													Utils.LogMessage($"Invalid com port {comPort}");
													RumbleType = "<none>";
												}

												//if (gameOptions.gun > 0) sindenPump = gameOptions.gunA_pump;
											}
											else RumbleType = "<none>";
										}
										if (RumbleType == "sinden-gun1" || RumbleType == "sinden-gun2")
										{
											if (RumbleType == "sinden-gun1")
											{
												RumbleParameter = "RecoilSindenGunA";
												sindenRumbleGunASet = 1;
											}
											if (RumbleType == "sinden-gun2")
											{
												RumbleParameter = "RecoilSindenGunB";
												sindenRumbleGunASet = 2;
											}
											RumbleType = "sinden";
										}
										if (RumbleType == "rumble" || RumbleType == "lichtknarre")
										{
											string SDLGuid = GunAGuid.ToString();
											/*
											try
											{
												SDLGuid = ButtonToKey.DSharpGuidToSDLGuid(GunAGuid);
											}
											catch { }
											*/
											if (SDLGuid != "")
											{
												RumbleParameter = SDLGuid;
											}
											else
											{
												RumbleType = "<none>";
											}
										}
										RumbleTypeA = RumbleType;
										RumbleParameterA = RumbleParameter;

										gunAAutoJoy = AutoJoy;
										gunADamageRumble = DamageRumble;
										gunA4tiers = Fourtiers;

										if (RumbleTypeA.ToLower() == "mamehooker" || RumbleTypeB.ToLower() == "mamehooker")
										{
											DemulshooterManager.UseMamehooker = true;
										}

									}
									if (GunBGuid != "")
									{
										bool AutoJoy = false;
										bool DamageRumble = false;
										bool Fourtiers = false;

										string RumbleType = ConfigurationManager.MainConfig.gunBRecoil;
										if (gameOptions.gunB_recoil == 1) RumbleType = "<none>";
										if (gameOptions.gunB_recoil == 2) RumbleType = "mamehooker";

										string RumbleParameter = "";
										if (RumbleType == "gun4ir")
										{
											int comPort = ConfigurationManager.MainConfig.gunBComPort;
											if (comPort > 0)
											{
												if (gun_preconfigDPI.isValidGun4irPort(comPort))
												{
													RumbleParameter = "COM" + comPort.ToString();
													if (ConfigurationManager.MainConfig.gunBAutoJoy) AutoJoy = true;
													if (ConfigurationManager.MainConfig.gunBdomagerumble) DamageRumble = true;
													if (ConfigurationManager.MainConfig.gunB4tiers) Fourtiers = true;
													if (gameOptions.gunB_4tiers > 0)
													{
														if (gameOptions.gunB_4tiers == 1) Fourtiers = false;
														if (gameOptions.gunB_4tiers == 2) Fourtiers = true;
													}
												}
												else
												{
													Utils.LogMessage($"Invalid com port {comPort}");
													RumbleType = "<none>";
												}
											}
											else RumbleType = "<none>";
										}
										if (RumbleType == "sinden-gun1" || RumbleType == "sinden-gun2")
										{

											if (RumbleType == "sinden-gun1") RumbleParameter = "RecoilSindenGunA";
											if (RumbleType == "sinden-gun2") RumbleParameter = "RecoilSindenGunB";
											RumbleType = "sinden";

											if (RumbleParameter == "RecoilSindenGunA" && sindenRumbleGunASet == 1) RumbleParameter = "RecoilSindenGunB";
											if (RumbleParameter == "RecoilSindenGunB" && sindenRumbleGunASet == 2) RumbleParameter = "RecoilSindenGunA";

										}
										if (RumbleType == "rumble" || RumbleType == "lichtknarre")
										{
											string SDLGuid = GunBGuid.ToString();
											/*
											try
											{
												SDLGuid = ButtonToKey.DSharpGuidToSDLGuid(GunBGuid);
											}
											catch { }
											*/
											if (SDLGuid != "")
											{
												RumbleParameter = SDLGuid;
											}
											else
											{
												RumbleType = "<none>";
											}
										}

										if (RumbleParameter == RumbleParameterA && RumbleParameter != "")
										{
											RumbleType = "<none>";
											RumbleParameter = "";
											gunAAutoJoy = false;
											gunADamageRumble = false;
											gunA4tiers = false;
										}
										RumbleTypeB = RumbleType;
										RumbleParameterB = RumbleParameter;
										gunBAutoJoy = AutoJoy;
										gunBDamageRumble = DamageRumble;
										gunB4tiers = Fourtiers;
									}

									if (GameInfo.ContainsKey("target") && GameInfo.ContainsKey("rom") && GameInfo["rom"] != "")
									{
										string processtarget = "";
										if (GameInfo.ContainsKey("usedemulprocesstarget") && GameInfo["usedemulprocesstarget"].ToLower() == "false") processtarget = "";
										else
										{
											string targetExecutableGame = executableGame;
											if (GameInfo.ContainsKey("magpieExecutable") && GameInfo["magpieExecutable"].Trim() != "")
											{
												targetExecutableGame = Path.GetFullPath(Path.Combine(executableGameDir, GameInfo["magpieExecutable"]));
											}
											processtarget = Path.GetFileNameWithoutExtension(targetExecutableGame);
											if (!targetExecutableGame.ToLower().EndsWith(".exe")) processtarget = "";
										}
										/*
										string forcemd5 = "";
										if (GameInfo.ContainsKey("usedemulforcemd5") && GameInfo["usedemulforcemd5"].ToLower() == "false") forcemd5 = "";
										else
										{
											string targetExecutableGame = executableGame;
											if (GameInfo.ContainsKey("magpieExecutable") && GameInfo["magpieExecutable"].Trim() != "")
											{
												targetExecutableGame = Path.GetFullPath(Path.Combine(executableGameDir, GameInfo["magpieExecutable"]));
											}
											if(!string.IsNullOrEmpty(targetExecutableGame) && File.Exists(targetExecutableGame))
											{
												if (File.Exists(targetExecutableGame + ".filetorestore")) targetExecutableGame = targetExecutableGame + ".filetorestore";
												using (var md5 = MD5.Create())
												{
													using (var stream = File.OpenRead(targetExecutableGame))
													{
														var hash = md5.ComputeHash(stream);
														forcemd5 = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
													}
												}
											}
										}
										if (forcemd5 == "715fe71de740e248b428b2b1d35af3d9") forcemd5 = "7e11f7e78ed566a277edba1a8aab0749"; //Transformers Human Alliance
										*/

										DemulshooterManager.InitGuns(RumbleTypeA, RumbleParameterA, RumbleTypeB, RumbleParameterB, gunAAutoJoy, gunADamageRumble, gunA4tiers, gunBAutoJoy, gunBDamageRumble, gunB4tiers);
										if (GameInfo.ContainsKey("64bits") && GameInfo["64bits"].ToLower() == "true") DemulshooterManager.Is64bits = true;
										else DemulshooterManager.Is64bits = false;

										if (ConfigurationManager.MainConfig.alwaysRunMamehooker) DemulshooterManager.UseMamehooker = true;
										DemulshooterManager.UseTcp = true;
										DemulshooterManager.Rom = GameInfo["rom"];
										DemulshooterManager.Target = GameInfo["target"];
										DemulshooterManager.HideCrosshair = hideCrosshair;
										//DemulshooterManager.Start(processtarget, forcemd5);
										DemulshooterManager.Start(processtarget);
									}
									else
									{
										DemulshooterManager.InitGuns(RumbleTypeA, RumbleParameterA, RumbleTypeB, RumbleParameterB, gunAAutoJoy, gunADamageRumble, gunA4tiers, gunBAutoJoy, gunBDamageRumble, gunB4tiers, false);
										if (GameInfo.ContainsKey("recoil") && GameInfo["recoil"].Trim() != "")
										{
											DemulshooterManager.StartSelfManaged(GameInfo["recoil"].Trim().ToLower());
										}


									}
								}

								/*
								string p1CrosshairToRestore = "";
								string p2CrosshairToRestore = "";
								if (GameInfo.ContainsKey("crosshairPNG") && GameInfo["crosshairPNG"] == "True")
								{
									string sourcePngDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
									string targetPngDir = executableGameDir;
									string p1Crosshair = Path.Combine(targetPngDir, "p1.png");
									string p2Crosshair = Path.Combine(targetPngDir, "p2.png");
									if (executableGameDir != "" && Directory.Exists(targetPngDir) && File.Exists(p1Crosshair) && File.Exists(p2Crosshair))
									{
										string p1CrosshairBackup = Path.Combine(targetPngDir, "p1.png.AutoXinputBackup");
										string p2CrosshairBackup = Path.Combine(targetPngDir, "p2.png.AutoXinputBackup");
										string sourcePngA = Path.Combine(sourcePngDir, "p1.png");
										string sourcePngB = Path.Combine(sourcePngDir, "p2.png");

										if (File.Exists(p1Crosshair))
										{
											try
											{
												File.Move(p1Crosshair, p1CrosshairBackup, true);
												p1CrosshairToRestore = p1CrosshairBackup;
											}
											catch
											{ }
										}
										if (File.Exists(p2Crosshair))
										{
											try
											{
												File.Move(p2Crosshair, p2CrosshairBackup, true);
												p2CrosshairToRestore = p2CrosshairBackup;
											}
											catch
											{ }
										}

										if (!crosshairA) sourcePngA = Path.Combine(sourcePngDir, "p1-empty.png");
										if (!crosshairB) sourcePngB = Path.Combine(sourcePngDir, "p2-empty.png");
										if (File.Exists(sourcePngA))
										{
											try
											{
												File.Move(sourcePngA, p1Crosshair, true);
											}
											catch
											{ }
										}
										if (File.Exists(p2Crosshair))
										{
											try
											{
												File.Move(p2Crosshair, p2CrosshairBackup, true);
											}
											catch
											{ }
										}
									}
								}
								*/

								if (TpSettingsManager.IsPatreon && ConfigurationManager.MainConfig.tpLicenceRegOnLaunch)
								{
									bool unregister = false;
									using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\TeknoGods\TeknoParrot"))
									{
										var isPatronRegistery = key != null && key.GetValue("PatreonSerialKey") != null;

										if (isPatronRegistery)
										{
											ProcessStartInfo tpUnRegStartInfo = new ProcessStartInfo();
											Process TpUnRegProcess = new Process();
											tpUnRegStartInfo.FileName = Path.Combine(baseTpDir, "TeknoParrot", "BudgieLoader.exe");
											tpUnRegStartInfo.UseShellExecute = false;
											//tpUnRegStartInfo.CreateNoWindow = true;
											tpUnRegStartInfo.Arguments = "-deactivate";
											TpUnRegProcess.StartInfo = tpUnRegStartInfo;
											TpUnRegProcess.Start();
											TpUnRegProcess.WaitForExit();
										}
										unregister = true;
									}
									if (unregister)
									{
										using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\TeknoGods\TeknoParrot", true))
										{
											if (key != null)
											{
												Registry.CurrentUser.DeleteSubKey(@"SOFTWARE\TeknoGods\TeknoParrot");
												Console.WriteLine("La cl� de registre a �t� supprim�e avec succ�s.");
											}
										}
									}

									ProcessStartInfo tpRegStartInfo = new ProcessStartInfo();
									Process TpRegProcess = new Process();
									tpRegStartInfo.FileName = Path.Combine(baseTpDir, "TeknoParrot", "BudgieLoader.exe");
									tpRegStartInfo.UseShellExecute = false;
									//tpRegStartInfo.CreateNoWindow = true;
									tpRegStartInfo.Arguments = "-register " + Utils.Decrypt(ConfigurationManager.MainConfig.tpLicence);
									TpRegProcess.StartInfo = tpRegStartInfo;
									TpRegProcess.Start();
									TpRegProcess.WaitForExit();
								}

								if (gameOptions.AhkBefore.Trim() != "")
								{
									Utils.LogMessage($"Execute AHK Before");
									Utils.ExecuteAHK(gameOptions.AhkBefore, gameOptions.WaitForExitAhkBefore, gameDir, linkSourceFolderExe);
								}

								Thread NoStartupThread = null;
								if (showStartup)
								{
									Utils.LogMessage($"showStartup");
									cancellationTokenSource = new CancellationTokenSource();
									Task.Run(() => ShowFormAsync(cancellationTokenSource.Token));
								}
								else
								{
									NoStartupThread = new Thread(() =>
										{
											bool NoStartupThreadDone = false;
											while (!NoStartupThreadDone)
											{
												Process[] processes = Process.GetProcessesByName("TeknoParrotUi"); // Remplacez par le nom de l'ex�cutable de l'application cible
												if (processes.Length > 0)
												{
													Process targetProcess = processes[0];
													if (!targetProcess.HasExited) //&& Startup.IsWinVisible(targetProcess.MainWindowHandle))
													{
														//ShowWindow(targetProcess.MainWindowHandle, SW_MINIMIZE);
														Startup.MoveBottom(targetProcess.MainWindowHandle);
														//NoStartupThreadDone = true;
													}
												}
												else
												{
													Thread.Sleep(30);
												}
											}


											return;
										});
									//NoStartupThread.Start();

								}







								int sinden_process_pid = -1;
								if (useDinputLightGun && ((GunAGuid != "" && GunAType == "sinden") || (GunBGuid != "" && GunBType == "sinden")))
								{

									int firstSindenGun = 0;
									int secondSindenGun = 0;

									string RumbleTypeA = ConfigurationManager.MainConfig.gunARecoil;
									if (gameOptions.gunA_recoil == 1) RumbleTypeA = "";

									string RumbleTypeB = ConfigurationManager.MainConfig.gunBRecoil;
									if (gameOptions.gunB_recoil == 1) RumbleTypeB = "";

									if (RumbleTypeA == RumbleTypeB) RumbleTypeB = "";

									//MessageBox.Show($"GunAGuid={GunAGuid}, GunAType={GunAType}, RumbleTypeA={RumbleTypeA}, GunASindenType={GunASindenType}");
									//MessageBox.Show($"GunBGuid={GunBGuid}, GunAType={GunBType}, RumbleTypeA={RumbleTypeB}, GunASindenType={GunBSindenType}");


									if (GunAGuid != "" && GunAType == "sinden" && GunASindenType > 0)
									{

										if (RumbleTypeA == "sinden-gun1" && firstSindenGun == 0) firstSindenGun = 1;
										if (RumbleTypeA == "sinden-gun2" && secondSindenGun == 0) secondSindenGun = 1;
									}
									if (GunBGuid != "" && GunBType == "sinden" && GunBSindenType > 0)
									{

										if (RumbleTypeB == "sinden-gun1" && firstSindenGun == 0) firstSindenGun = 2;
										if (RumbleTypeB == "sinden-gun2" && secondSindenGun == 0) secondSindenGun = 2;
									}
									if (GunAGuid != "" && GunAType == "sinden" && GunASindenType > 0 && firstSindenGun != 1 && secondSindenGun != 1)
									{
										if (firstSindenGun == 0) firstSindenGun = 1;
										else if (secondSindenGun == 0) secondSindenGun = 1;
									}
									if (GunBGuid != "" && GunBType == "sinden" && GunBSindenType > 0 && firstSindenGun != 2 && secondSindenGun != 2)
									{
										if (firstSindenGun == 0) firstSindenGun = 2;
										else if (secondSindenGun == 0) secondSindenGun = 2;
									}

									if (firstSindenGun == secondSindenGun) secondSindenGun = 0; //shouldn't happen

									//MessageBox.Show($"firstSindenGun={firstSindenGun} secondSindenGun={secondSindenGun}");

									int firstSindenType = 0;
									int firstSindenRecoil1 = 0;
									int firstSindenRecoil2 = 0;
									int firstSindenRecoil3 = 0;
									int secondSindenType = 0;
									int secondSindenRecoil1 = 0;
									int secondSindenRecoil2 = 0;
									int secondSindenRecoil3 = 0;

									if (firstSindenGun > 0)
									{
										if (firstSindenGun == 1)
										{
											firstSindenType = GunASindenType;
											firstSindenRecoil1 = gameOptions.gunA_sindenRecoil1 > 0 ? gameOptions.gunA_sindenRecoil1 - 1 : ConfigurationManager.MainConfig.gunA_sindenRecoil1;
											firstSindenRecoil2 = gameOptions.gunA_sindenRecoil2 > 0 ? gameOptions.gunA_sindenRecoil2 - 1 : ConfigurationManager.MainConfig.gunA_sindenRecoil2;
											firstSindenRecoil3 = gameOptions.gunA_sindenRecoil3 > 0 ? gameOptions.gunA_sindenRecoil3 - 1 : ConfigurationManager.MainConfig.gunA_sindenRecoil3;
										}
										if (firstSindenGun == 2)
										{
											firstSindenType = GunBSindenType;
											firstSindenRecoil1 = gameOptions.gunB_sindenRecoil1 > 0 ? gameOptions.gunB_sindenRecoil1 - 1 : ConfigurationManager.MainConfig.gunB_sindenRecoil1;
											firstSindenRecoil2 = gameOptions.gunB_sindenRecoil2 > 0 ? gameOptions.gunB_sindenRecoil2 - 1 : ConfigurationManager.MainConfig.gunB_sindenRecoil2;
											firstSindenRecoil3 = gameOptions.gunB_sindenRecoil3 > 0 ? gameOptions.gunB_sindenRecoil3 - 1 : ConfigurationManager.MainConfig.gunB_sindenRecoil3;
										}
									}
									if (secondSindenGun > 0)
									{
										if (secondSindenGun == 1)
										{
											secondSindenType = GunASindenType;
											secondSindenRecoil1 = gameOptions.gunA_sindenRecoil1 > 0 ? gameOptions.gunA_sindenRecoil1 - 1 : ConfigurationManager.MainConfig.gunA_sindenRecoil1;
											secondSindenRecoil2 = gameOptions.gunA_sindenRecoil2 > 0 ? gameOptions.gunA_sindenRecoil2 - 1 : ConfigurationManager.MainConfig.gunA_sindenRecoil2;
											secondSindenRecoil3 = gameOptions.gunA_sindenRecoil3 > 0 ? gameOptions.gunA_sindenRecoil3 - 1 : ConfigurationManager.MainConfig.gunA_sindenRecoil3;
										}
										if (secondSindenGun == 2)
										{
											secondSindenType = GunBSindenType;
											secondSindenRecoil1 = gameOptions.gunB_sindenRecoil1 > 0 ? gameOptions.gunB_sindenRecoil1 - 1 : ConfigurationManager.MainConfig.gunB_sindenRecoil1;
											secondSindenRecoil2 = gameOptions.gunB_sindenRecoil2 > 0 ? gameOptions.gunB_sindenRecoil2 - 1 : ConfigurationManager.MainConfig.gunB_sindenRecoil2;
											secondSindenRecoil3 = gameOptions.gunB_sindenRecoil3 > 0 ? gameOptions.gunB_sindenRecoil3 - 1 : ConfigurationManager.MainConfig.gunB_sindenRecoil3;
										}
									}

									if ((firstSindenType > 0 || secondSindenType > 0) && File.Exists(ConfigurationManager.MainConfig.sindenExe))
									{

										ProcessStartInfo psi = new ProcessStartInfo
										{
											FileName = "taskkill",
											Arguments = $"/F /IM Lightgun.exe",
											CreateNoWindow = true,
											UseShellExecute = false
										};
										Process.Start(psi);
										Thread.Sleep(1500);

										string argument_sinden = ConfigurationManager.MainConfig.sindenExtraCmd;
										if (gameOptions.gun_useExtraSinden) argument_sinden = gameOptions.gun_ExtraSinden;
										if (argument_sinden != "") argument_sinden += " ";
										argument_sinden += $"--p1-{firstSindenType}-{firstSindenRecoil1}-{firstSindenRecoil2}-{firstSindenRecoil3} --p2-{secondSindenType}-{secondSindenRecoil1}-{secondSindenRecoil2}-{secondSindenRecoil3}";

										Process sinden_process = new Process();
										sinden_process.StartInfo.FileName = ConfigurationManager.MainConfig.sindenExe;
										sinden_process.StartInfo.WorkingDirectory = Path.GetDirectoryName(ConfigurationManager.MainConfig.sindenExe);
										sinden_process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized; // Ajout de cette ligne pour minimiser la fen�tre
										sinden_process.StartInfo.Arguments = argument_sinden;
										sinden_process.StartInfo.UseShellExecute = true;
										sinden_process.Start();
										sinden_process_pid = sinden_process.Id;
									}

								}

								if (useDinputLightGun && VjoyGuid != "")
								{
									Utils.LogMessage("Launch VjoyControl");
									string gunOptions = "";
									if (vjoy_gunA) gunOptions = "gunA";
									if (vjoy_gunB) gunOptions = "gunB";
									if (vjoy_gunA && vjoy_gunB) gunOptions = "all";

									if (gameOptions.tmpGunXFormula != "" || gameOptions.tmpGunYFormula != "" || gameOptions.tmpGunAMinMax != "" || gameOptions.tmpGunBMinMax != ""
										|| gameOptions.tmpGunAXFormulaBefore != "" || gameOptions.tmpGunAYFormulaBefore != "" || gameOptions.tmpGunBXFormulaBefore != "" || gameOptions.tmpGunBYFormulaBefore != ""

										)
									{
										gunOptions += $" \"{gameOptions.tmpGunXFormula}\"";
										gunOptions += $" \"{gameOptions.tmpGunYFormula}\"";
										gunOptions += $" \"{gameOptions.tmpGunAMinMax}\"";
										gunOptions += $" \"{gameOptions.tmpGunBMinMax}\"";
										gunOptions += $" \"{gameOptions.tmpGunAXFormulaBefore}\"";
										gunOptions += $" \"{gameOptions.tmpGunAYFormulaBefore}\"";
										gunOptions += $" \"{gameOptions.tmpGunBXFormulaBefore}\"";
										gunOptions += $" \"{gameOptions.tmpGunBYFormulaBefore}\"";
									}

									Process vjoy_process = new Process();
									vjoy_process.StartInfo.FileName = Process.GetCurrentProcess().MainModule.FileName;
									vjoy_process.StartInfo.WorkingDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
									vjoy_process.StartInfo.Arguments = $"--runvjoy {vjoyIndex} " + gunOptions + $" \"{xmlFile}\"";
									vjoy_process.StartInfo.UseShellExecute = true;
									vjoy_process.Start();
									Thread.Sleep(1000);
								}

								Thread WaitForWindowed = null;
								bool forceMagpie = false;
								bool useMagpie = ConfigurationManager.MainConfig.useMagpie;
								if (gameOptions.useMagpie > 0)
								{
									if (gameOptions.useMagpie == 1) useMagpie = true;
									if (gameOptions.useMagpie == 2) useMagpie = false;
									if (gameOptions.useMagpie == 3)
									{
										forceMagpie = true;
										useMagpie = true;
									}
								}

								string magpieClass = "";
								if (GameInfo.ContainsKey("magpieClass") && GameInfo["magpieClass"].Trim() != "") magpieClass = GameInfo["magpieClass"].Trim();

								string magpieTitle = "";
								if (GameInfo.ContainsKey("magpieTitle") && GameInfo["magpieTitle"].Trim() != "") magpieTitle = GameInfo["magpieTitle"].Trim();

								string magpieExecutableGame = executableGame;
								if (GameInfo.ContainsKey("magpieExecutable") && GameInfo["magpieExecutable"].Trim() != "")
								{
									magpieExecutableGame = Path.GetFullPath(Path.Combine(executableGameDir, GameInfo["magpieExecutable"]));
								}

								string forceSindenCalibration = "";
								string forcevjoyXformula = "";
								string forcevjoyYformula = "";

								if ((TpSettingsManager.IsWindowed || forceMagpie) && useMagpie)
								{
									string magpieExe = ConfigurationManager.MainConfig.magpieExe;
									string magpieConfig = Path.Combine(Path.GetDirectoryName(magpieExe), "config", "config.json");

									ProcessStartInfo psi = new ProcessStartInfo
									{
										FileName = "taskkill",
										Arguments = $"/F /IM Magpie.exe",
										CreateNoWindow = true,
										UseShellExecute = false
									};
									Process.Start(psi);
									Thread.Sleep(1000);

									int magpieDelay = ConfigurationManager.MainConfig.magpieDelay;
									if (gameOptions.magpieDelay > 0)
									{
										if (gameOptions.magpieDelay == 1) magpieDelay = 0;
										if (gameOptions.magpieDelay == 2) magpieDelay = 3;
										if (gameOptions.magpieDelay == 3) magpieDelay = 5;
										if (gameOptions.magpieDelay == 4) magpieDelay = 10;
										if (gameOptions.magpieDelay == 5) magpieDelay = 20;
									}

									int magpieScaling = ConfigurationManager.MainConfig.magpieScaling;
									if (gameOptions.magpieScaling > 0) magpieScaling = gameOptions.magpieScaling - 1;

									int magpieCapture = ConfigurationManager.MainConfig.magpieCapture;
									if (gameOptions.magpieCapture > 0) magpieScaling = gameOptions.magpieCapture - 1;


									bool magpieShowFps = ConfigurationManager.MainConfig.magpieShowFps;
									if (gameOptions.magpieShowFps > 0) magpieShowFps = gameOptions.magpieShowFps == 1 ? true : false;

									bool magpieTripleBuffering = ConfigurationManager.MainConfig.magpieTripleBuffering;
									if (gameOptions.magpieTripleBuffering > 0) magpieTripleBuffering = gameOptions.magpieTripleBuffering == 1 ? true : false;

									bool magpieVsync = ConfigurationManager.MainConfig.magpieVsync;
									if (gameOptions.magpieVsync > 0) magpieVsync = gameOptions.magpieVsync == 1 ? true : false;

									int magpieFsrSharp = ConfigurationManager.MainConfig.magpieFsrSharp;
									if (gameOptions.magpieFsrSharp > 0)
									{
										magpieFsrSharp = 55 + (gameOptions.magpieFsrSharp * 5);
									}

									bool magpieExclusiveFullscreen = ConfigurationManager.MainConfig.magpieExclusiveFullscreen;
									if (gameOptions.magpieExclusiveFullscreen > 0) magpieExclusiveFullscreen = gameOptions.magpieExclusiveFullscreen == 1 ? true : false;

									bool magpieDisableDirectFlip = false;
									if (GameInfo.ContainsKey("magpieDisableDirectFlip") && (GameInfo["magpieDisableDirectFlip"].ToLower() == "true" || (GameInfo["magpieDisableDirectFlip"].Trim() != "" && TpSettingsManager.tags.Contains(GameInfo["magpieDisableDirectFlip"].Trim())))) magpieDisableDirectFlip = true;

									bool magpie3DGameMode = false;
									if (GameInfo.ContainsKey("magpie3DGameMode") && (GameInfo["magpie3DGameMode"].ToLower() == "true" || (GameInfo["magpie3DGameMode"].Trim() != "" && TpSettingsManager.tags.Contains(GameInfo["magpie3DGameMode"].Trim())))) magpie3DGameMode = true;

									bool magpieLaunchBefore = false;
									if (GameInfo.ContainsKey("magpieLaunchBefore") && (GameInfo["magpieLaunchBefore"].ToLower() == "true" || (GameInfo["magpieLaunchBefore"].Trim() != "" && TpSettingsManager.tags.Contains(GameInfo["magpieLaunchBefore"].Trim())))) magpieLaunchBefore = true;

									bool magpieAllowScalingMaximized = false;
									if (GameInfo.ContainsKey("magpieAllowScalingMaximized") && (GameInfo["magpieAllowScalingMaximized"].ToLower() == "true" || (GameInfo["magpieAllowScalingMaximized"].Trim() != "" && TpSettingsManager.tags.Contains(GameInfo["magpieAllowScalingMaximized"].Trim())))) magpieAllowScalingMaximized = true;

									bool magpieNoMoveWindow = false;
									if (GameInfo.ContainsKey("magpieNoMoveWindow") && (GameInfo["magpieNoMoveWindow"].ToLower() == "true" || (GameInfo["magpieNoMoveWindow"].Trim() != "" && TpSettingsManager.tags.Contains(GameInfo["magpieNoMoveWindow"].Trim())))) magpieNoMoveWindow = true;

									bool magpieNoLateFocus = false;
									if (GameInfo.ContainsKey("magpieNoLateFocus") && (GameInfo["magpieNoLateFocus"].ToLower() == "true" || (GameInfo["magpieNoLateFocus"].Trim() != "" && TpSettingsManager.tags.Contains(GameInfo["magpieNoLateFocus"].Trim())))) magpieNoLateFocus = true;

									bool magpieNoClick = false;
									if (GameInfo.ContainsKey("magpieNoClick") && (GameInfo["magpieNoClick"].ToLower() == "true" || (GameInfo["magpieNoClick"].Trim() != "" && TpSettingsManager.tags.Contains(GameInfo["magpieNoClick"].Trim())))) magpieNoClick = true;

									bool magpieShowCursor = false;
									if (GameInfo.ContainsKey("magpieShowCursor") && (GameInfo["magpieShowCursor"].ToLower() == "true" || (GameInfo["magpieShowCursor"].Trim() != "" && TpSettingsManager.tags.Contains(GameInfo["magpieShowCursor"].Trim())))) magpieShowCursor = true;

									bool magpieNoLaunchIfSameRes = false;
									if (GameInfo.ContainsKey("magpieNoLaunchIfSameRes") && ((GameInfo["magpieNoLaunchIfSameRes"].ToLower() == "true" || GameInfo["magpieNoLaunchIfSameRes"].ToLower() == "force") || (GameInfo["magpieNoLaunchIfSameRes"].Trim() != "" && TpSettingsManager.tags.Contains(GameInfo["magpieNoLaunchIfSameRes"].Trim())))) magpieNoLaunchIfSameRes = true;

									bool forceWindowResize = false;
									if (GameInfo.ContainsKey("forceWindowResize") && (GameInfo["forceWindowResize"].ToLower() == "true" || (GameInfo["forceWindowResize"].Trim() != "" && TpSettingsManager.tags.Contains(GameInfo["forceWindowResize"].Trim())))) forceWindowResize = true;

									bool magpieCrop = false;
									int[] magpieCropValue = { 0, 0, 0, 0 };
									if (GameInfo.ContainsKey("magpieCrop") && GameInfo["magpieCrop"].Trim().ToLower() != "")
									{
										string value_magpieCrop_tag = GameInfo["magpieCrop"].Trim().ToLower();
										foreach (var magpieCropTag in value_magpieCrop_tag.Split('|'))
										{

											if (magpieCropTag.Split(',').Count() == 2 && magpieCropTag.Split(',')[0].Split('x').Count() == 4)
											{
												var splited_magpieCropTag = magpieCropTag.Split(',');
												var magpieCropTag_tailleReg = splited_magpieCropTag[0];
												var magpieCropTag_tag = splited_magpieCropTag[1].Trim().ToLower();
												if (magpieCropTag_tag == "others" || magpieCropTag_tag.Replace(" ", "").Split('&').All(tag => TpSettingsManager.tags.Contains(tag)))
												{
													try
													{
														magpieCropValue[0] = int.Parse(magpieCropTag_tailleReg.Split("x")[0]);
														magpieCropValue[1] = int.Parse(magpieCropTag_tailleReg.Split("x")[1]);
														magpieCropValue[2] = int.Parse(magpieCropTag_tailleReg.Split("x")[2]);
														magpieCropValue[3] = int.Parse(magpieCropTag_tailleReg.Split("x")[3]);
													}
													catch { }
													break;
												}
											}
										}
									}
									if (magpieCropValue[0] == 0 && magpieCropValue[1] == 0 && magpieCropValue[2] == 0 && magpieCropValue[3] == 0) magpieCrop = false;
									else magpieCrop = true;


									if (forceMagpie)
									{
										magpieNoMoveWindow = true;
										magpieAllowScalingMaximized = true;
										magpieNoLaunchIfSameRes = false;
										if (GameInfo.ContainsKey("magpieNoLaunchIfSameRes") && GameInfo["magpieNoLaunchIfSameRes"].ToLower() == "force") magpieNoLaunchIfSameRes = true;
									}

									if (!useDinputLightGun)
									{

										bool magpieContentWritten = false;

										WaitForWindowed = new Thread(() =>
										{
											Utils.LogMessage("Start Magpie Thread");
											string trueClassName = "";
											IntPtr windowHandle = Utils.FindWindowByMultipleCriteria(magpieClass, Path.GetFileNameWithoutExtension(magpieExecutableGame), magpieTitle, out trueClassName);
											while (!isExiting)
											{
												while (windowHandle == IntPtr.Zero && !isExiting)
												{
													windowHandle = Utils.FindWindowByMultipleCriteria(magpieClass, Path.GetFileNameWithoutExtension(magpieExecutableGame), magpieTitle, out trueClassName);
													Thread.Sleep(500);
													Utils.LogMessage("Search Window ...");
													if (windowHandle != IntPtr.Zero)
													{
														Utils.RECT clientRectTest;
														Utils.GetClientRect(windowHandle, out clientRectTest);
														int clientWidthTest = clientRectTest.Right - clientRectTest.Left;
														int clientHeightTest = clientRectTest.Bottom - clientRectTest.Top;
														if (clientWidthTest < 150 || clientHeightTest < 100) windowHandle = IntPtr.Zero;
													}
												}

												Utils.LogMessage("Window Found");

												Thread.Sleep(1000 + (magpieDelay * 1000));

												int screenWidth = 1920;
												int screenHeight = 1080;

												Screen monitorThatWillReceiveGameWindow = Screen.PrimaryScreen;
												if (Program.moveWindowToOriginalMonitor)
												{
													foreach (var s in Screen.AllScreens)
													{
														if (s.DeviceName == Program.originalMonitorDeviceName)
														{
															monitorThatWillReceiveGameWindow = s;
														}
													}
												}
												screenWidth = monitorThatWillReceiveGameWindow.Bounds.Width;
												screenHeight = monitorThatWillReceiveGameWindow.Bounds.Height;
												/*
												Screen[] screens = Screen.AllScreens;
												for (int i = 0; i < screens.Length; i++)
												{
													Screen screen = screens[i];
													string DeviceName = screen.DeviceName.Trim('\\').Trim('.').Trim('\\');
													if (screen.Primary)
													{
														screenWidth = screen.Bounds.Width;
														screenHeight = screen.Bounds.Height;
													}
												}
												*/

												Utils.RECT clientRect;
												Utils.GetClientRect(windowHandle, out clientRect);
												int clientWidth = clientRect.Right - clientRect.Left;
												int clientHeight = clientRect.Bottom - clientRect.Top;

												/*
												Utils.RECT WindowRect;
												Utils.GetClientRect(windowHandle, out WindowRect);
												int WindowWidth = WindowRect.Right - WindowRect.Left;
												int WindowHeight = clientRect.Bottom - clientRect.Top;
												*/

												string key_RegisterAsSize = "magpieRegisterAsSize";
												if (useKeepAspectRatio && GameInfo.ContainsKey(key_RegisterAsSize + "_keepaspectratio") && GameInfo[key_RegisterAsSize + "_keepaspectratio"].Trim() != "") key_RegisterAsSize = key_RegisterAsSize + "_keepaspectratio";
												if (!useKeepAspectRatio && GameInfo.ContainsKey(key_RegisterAsSize + "_dontkeepaspectratio") && GameInfo[key_RegisterAsSize + "_dontkeepaspectratio"].Trim() != "") key_RegisterAsSize = key_RegisterAsSize + "_dontkeepaspectratio";
												if (GameInfo.ContainsKey(key_RegisterAsSize) && GameInfo[key_RegisterAsSize].ToLower().Contains("x"))
												{
													string tailleReg = GameInfo[key_RegisterAsSize];
													try
													{
														clientWidth = int.Parse(tailleReg.Split("x")[0]);
														clientHeight = int.Parse(tailleReg.Split("x")[1]);
													}
													catch { }
												}
												if (GameInfo.ContainsKey("magpieRegisterAsSize_tag") && GameInfo["magpieRegisterAsSize_tag"].Trim().ToLower() != "")
												{
													string value_magpieRegisterAsSize_tag = GameInfo["magpieRegisterAsSize_tag"].Trim().ToLower();
													foreach (var magpieRegisterSizeTag in value_magpieRegisterAsSize_tag.Split('|'))
													{

														if (magpieRegisterSizeTag.Split(',').Count() == 2 && magpieRegisterSizeTag.Split(',')[0].Split('x').Count() == 2)
														{
															var splited_magpieRegisterSizeTag = magpieRegisterSizeTag.Split(',');
															var magpieRegisterSizeTag_tailleReg = splited_magpieRegisterSizeTag[0];
															var magpieRegisterSizeTag_tag = splited_magpieRegisterSizeTag[1].Trim().ToLower();
															//if(magpieRegisterSizeTag_tag == "others" || TpSettingsManager.tags.Contains(magpieRegisterSizeTag_tag))
															if (magpieRegisterSizeTag_tag == "others" || magpieRegisterSizeTag_tag.Replace(" ", "").Split('&').All(tag => TpSettingsManager.tags.Contains(tag)))
															{
																try
																{
																	clientWidth = int.Parse(magpieRegisterSizeTag_tailleReg.Split("x")[0]);
																	clientHeight = int.Parse(magpieRegisterSizeTag_tailleReg.Split("x")[1]);
																}
																catch { }
																break;
															}
														}
													}
												}


												double originalRatio = (double)clientWidth / clientHeight;
												int maxWindowWidth = Math.Min(screenWidth, (int)(screenHeight * originalRatio));
												int maxWindowHeight = (int)(maxWindowWidth / originalRatio);

												Utils.LogMessage("Informations sur la fen�tre :");
												Utils.LogMessage("Taille actuelle de la fen�tre : " + clientWidth + "x" + clientHeight);
												Utils.LogMessage("Taille maximis�e de la fen�tre sur l'�cran : " + maxWindowWidth + "x" + maxWindowHeight);

												if (forceResizeWidth > 0 && forceResizeHeight > 0)
												{
													WindowResizer.ResizeWindowToClientRect(windowHandle, forceResizeWidth, forceResizeHeight, true);
													magpieNoMoveWindow = true;
													if (magpieNoLaunchIfSameRes)
													{
														if (forceResizeWidth != monitorThatWillReceiveGameWindow.Bounds.Width) magpieNoLaunchIfSameRes = false;
														if (forceResizeHeight != monitorThatWillReceiveGameWindow.Bounds.Height) magpieNoLaunchIfSameRes = false;
													}
												}
												else
												{
													if (magpieNoLaunchIfSameRes)
													{
														if (clientWidth != monitorThatWillReceiveGameWindow.Bounds.Width) magpieNoLaunchIfSameRes = false;
														if (clientHeight != monitorThatWillReceiveGameWindow.Bounds.Height) magpieNoLaunchIfSameRes = false;
													}
													if (forceWindowResize) WindowResizer.ResizeWindowToClientRect(windowHandle, maxWindowWidth, maxWindowHeight);
												}

												if (File.Exists(magpieExe) && File.Exists(magpieConfig))
												{

													string magpieReshadeDll = Path.Combine(Path.GetDirectoryName(magpieExe), "d2d1.dll");
													if (string.IsNullOrEmpty(Program.magpieIni) && magpieContentWritten == false)
													{
														if (File.Exists(magpieReshadeDll))
														{
															File.Move(magpieReshadeDll, magpieReshadeDll + ".disabled");
														}
													}
													else
													{
														magpieNoLaunchIfSameRes = false;
														if (File.Exists(magpieReshadeDll + ".disabled"))
														{
															File.Move(magpieReshadeDll + ".disabled", magpieReshadeDll);
														}

														string magpieReshadeIni = Path.Combine(Path.GetDirectoryName(magpieExe), "ReShadePreset.ini");
														if (!magpieContentWritten)
														{
															File.Copy(Program.magpieIni, magpieReshadeIni, true);
														}

														string cacheShaderDirMagpie = Path.Combine(Path.GetDirectoryName(magpieExe), "reshade-shaders", "Cache");
														if (Directory.Exists(cacheShaderDirMagpie))
														{
															try
															{
																string[] files = Directory.GetFiles(cacheShaderDirMagpie, "reshade-BezelMagpie*");
																foreach (string file in files) File.Delete(file);
																string[] files2 = Directory.GetFiles(cacheShaderDirMagpie, "reshade-UIMask*");
																foreach (string file2 in files) File.Delete(file2);

															}
															catch (Exception ex) { }
														}
														else
														{
															try
															{
																Directory.CreateDirectory(cacheShaderDirMagpie);
															}
															catch { }
														}

														string magpieReshadeMainIni = Path.Combine(Path.GetDirectoryName(magpieExe), "ReShadePreset.ini");
														if (File.Exists(magpieReshadeMainIni))
														{
															IniFile reshadeIniFile = new IniFile(magpieReshadeMainIni);
															reshadeIniFile.Write("IntermediateCachePath", Path.GetFullPath(cacheShaderDirMagpie), "GENERAL");
														}

													}

													string jsonText = Utils.ReadAllText(magpieConfig);
													JObject jsonObject = JObject.Parse(jsonText);
													jsonObject["allowScalingMaximized"] = magpieAllowScalingMaximized;
													jsonObject["simulateExclusiveFullscreen"] = magpieExclusiveFullscreen;
													try
													{
														double sharpnessValue = (double)magpieFsrSharp / 100.0;
														jsonObject["scalingModes"][1]["effects"][1]["parameters"]["sharpness"] = sharpnessValue;
													}
													catch { }
													JArray profilesArray = (JArray)jsonObject["profiles"];
													foreach (JObject profile in profilesArray)
													{
														if (profile["name"] != null && profile["name"].ToString() == "Teknoparrot")
														{
															profile["pathRule"] = magpieExecutableGame;
															profile["classNameRule"] = trueClassName;
															profile["scalingMode"] = magpieScaling;
															profile["captureMethod"] = magpieCapture;
															profile["VSync"] = magpieVsync;
															profile["tripleBuffering"] = magpieTripleBuffering;
															profile["showFPS"] = magpieShowFps;
															profile["drawCursor"] = magpieShowCursor;
															profile["croppingEnabled"] = magpieCrop;
															profile["cropping"]["left"] = magpieCropValue[0];
															profile["cropping"]["top"] = magpieCropValue[1];
															profile["cropping"]["right"] = magpieCropValue[2];
															profile["cropping"]["bottom"] = magpieCropValue[3];


														}
													}


													string modifiedJsonText = JsonConvert.SerializeObject(jsonObject, Newtonsoft.Json.Formatting.Indented);
													File.WriteAllText(magpieConfig, modifiedJsonText);
													try
													{
														if (!magpieNoMoveWindow) Utils.MoveWindowsToZero(windowHandle);
														Thread.Sleep(100);
														Utils.SetForegroundWindow(windowHandle);
														Thread.Sleep(100);

														//Utils.ClickWindow(windowHandle);
													}
													catch { }

													if (!magpieNoLaunchIfSameRes)
													{
														Process magpie_process = new Process();
														magpie_process.StartInfo.FileName = magpieExe;
														magpie_process.StartInfo.Arguments = "-t";
														magpie_process.StartInfo.WorkingDirectory = Path.GetDirectoryName(magpieExe);
														magpie_process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized; // Ajout de cette ligne pour minimiser la fen�tre
														magpie_process.StartInfo.UseShellExecute = true;
														magpie_process.Start();
														magpie_process_pid = magpie_process.Id;
													}
													else
													{
														int screenLeft = Screen.PrimaryScreen.Bounds.Left;
														int screenBottom = Screen.PrimaryScreen.Bounds.Bottom;

														// D�placer le curseur en bas � gauche du deuxi�me �cran
														Utils.SetCursorPos(screenLeft, screenBottom - 1);
													}


													if (!magpieNoClick)
													{
														var ClickOnPrimaryScreen = new Thread(() =>
														{
															for (int i = 0; i < 100; i++)
															{
																if (isExiting) break;
																string foregroundClassName = Utils.GetForegroundClassName();
																Thread.Sleep(100);
																if (foregroundClassName == "Magpie_Main")
																{
																	Thread.Sleep(2000);
																	Utils.ClickOnPrimaryScreen(30, 30);
																	break;
																}
															}
														});
														ClickOnPrimaryScreen.Start();

													}




												}


												return;

											}
										});
										WaitForWindowed.Start();


									}
									else
									{

										bool magpieContentWritten = false;

										/*
										//0=No, 1=Yes, 2=Yes-Sinden
										int magpieLightgun = ConfigurationManager.MainConfig.magpieLightgun;
										if (gameOptions.magpieLightgun > 0) magpieLightgun = gameOptions.magpieLightgun - 1;

										//0=Nothing, 1=vjoy, 2=sinden
										int magpieLightgunCalibration = ConfigurationManager.MainConfig.magpieLightgunCalibration;
										if (gameOptions.magpieLightgunCalibration > 0) magpieLightgunCalibration = gameOptions.magpieLightgunCalibration - 1;

										*/

										bool magpieSindenBorder = ConfigurationManager.MainConfig.magpieSinden;
										if (gameOptions.magpieSinden == 1) magpieSindenBorder = true;
										if (gameOptions.magpieSinden == 2) magpieSindenBorder = false;
										if (!atLeastOneSinden) magpieSindenBorder = false;


										double magpieBorderSize = ConfigurationManager.MainConfig.magpieBorderSize;

										bool magpieGunCalibration = ConfigurationManager.MainConfig.magpieGunCalibration;
										if (gameOptions.magpieGunCalibration == 1) magpieGunCalibration = true;
										if (gameOptions.magpieGunCalibration == 2) magpieGunCalibration = false;

										bool magpieCalibrateSindenSoft = false;
										bool magpieCalibrateVjoy = false;
										if (magpieGunCalibration)
										{
											if (atLeastOneSinden && allSindenWithoutVjoy)
											{
												magpieCalibrateSindenSoft = true;
											}
											magpieCalibrateVjoy = true;
										}

										WaitForWindowed = new Thread(() =>
										{
											Utils.LogMessage("Start Magpie Thread");
											string trueClassName = "";
											IntPtr windowHandle = Utils.FindWindowByMultipleCriteria(magpieClass, Path.GetFileNameWithoutExtension(magpieExecutableGame), magpieTitle, out trueClassName);
											while (!isExiting)
											{
												while (windowHandle == IntPtr.Zero && !isExiting)
												{
													windowHandle = Utils.FindWindowByMultipleCriteria(magpieClass, Path.GetFileNameWithoutExtension(magpieExecutableGame), magpieTitle, out trueClassName);
													Thread.Sleep(500);
													Utils.LogMessage("Search Window ...");
													if (windowHandle != IntPtr.Zero)
													{
														Utils.RECT clientRectTest;
														Utils.GetClientRect(windowHandle, out clientRectTest);
														int clientWidthTest = clientRectTest.Right - clientRectTest.Left;
														if (clientWidthTest < 150) windowHandle = IntPtr.Zero;
													}
												}

												Utils.LogMessage("Window Found");

												Thread.Sleep(1000 + (magpieDelay * 1000));


												int screenWidth = 1920;
												int screenHeight = 1080;

												Screen monitorThatWillReceiveGameWindow = Screen.PrimaryScreen;
												if (Program.moveWindowToOriginalMonitor)
												{
													foreach (var s in Screen.AllScreens)
													{
														if (s.DeviceName == Program.originalMonitorDeviceName)
														{
															monitorThatWillReceiveGameWindow = s;
														}
													}
												}
												screenWidth = monitorThatWillReceiveGameWindow.Bounds.Width;
												screenHeight = monitorThatWillReceiveGameWindow.Bounds.Height;


												Utils.RECT clientRect;
												Utils.GetClientRect(windowHandle, out clientRect);
												int clientWidth = clientRect.Right - clientRect.Left;
												int clientHeight = clientRect.Bottom - clientRect.Top;

												string key_RegisterAsSize = "magpieRegisterAsSize";
												if (useKeepAspectRatio && GameInfo.ContainsKey(key_RegisterAsSize + "_keepaspectratio") && GameInfo[key_RegisterAsSize + "_keepaspectratio"].Trim() != "") key_RegisterAsSize = key_RegisterAsSize + "_keepaspectratio";
												if (!useKeepAspectRatio && GameInfo.ContainsKey(key_RegisterAsSize + "_dontkeepaspectratio") && GameInfo[key_RegisterAsSize + "_dontkeepaspectratio"].Trim() != "") key_RegisterAsSize = key_RegisterAsSize + "_dontkeepaspectratio";
												if (GameInfo.ContainsKey(key_RegisterAsSize) && GameInfo[key_RegisterAsSize].ToLower().Contains("x"))
												{
													string tailleReg = GameInfo[key_RegisterAsSize];
													try
													{
														clientWidth = int.Parse(tailleReg.Split("x")[0]);
														clientHeight = int.Parse(tailleReg.Split("x")[1]);
													}
													catch { }
												}

												if (GameInfo.ContainsKey("magpieRegisterAsSize_tag") && GameInfo["magpieRegisterAsSize_tag"].Trim().ToLower() != "")
												{
													string value_magpieRegisterAsSize_tag = GameInfo["magpieRegisterAsSize_tag"].Trim().ToLower();
													foreach (var magpieRegisterSizeTag in value_magpieRegisterAsSize_tag.Split('|'))
													{

														if (magpieRegisterSizeTag.Split(',').Count() == 2 && magpieRegisterSizeTag.Split(',')[0].Split('x').Count() == 2)
														{
															var splited_magpieRegisterSizeTag = magpieRegisterSizeTag.Split(',');
															var magpieRegisterSizeTag_tailleReg = splited_magpieRegisterSizeTag[0];
															var magpieRegisterSizeTag_tag = splited_magpieRegisterSizeTag[1].Trim().ToLower();
															//if (magpieRegisterSizeTag_tag == "others" || TpSettingsManager.tags.Contains(magpieRegisterSizeTag_tag))
															if (magpieRegisterSizeTag_tag == "others" || magpieRegisterSizeTag_tag.Replace(" ", "").Split('&').All(tag => TpSettingsManager.tags.Contains(tag)))
															{
																try
																{
																	clientWidth = int.Parse(magpieRegisterSizeTag_tailleReg.Split("x")[0]);
																	clientHeight = int.Parse(magpieRegisterSizeTag_tailleReg.Split("x")[1]);
																}
																catch { }
																break;
															}
														}
													}
												}


												double originalRatio = (double)clientWidth / clientHeight;
												int maxWindowWidth = Math.Min(screenWidth, (int)(screenHeight * originalRatio));
												int maxWindowHeight = (int)(maxWindowWidth / originalRatio);

												Utils.LogMessage("Informations sur la fen�tre :");
												Utils.LogMessage("Taille actuelle de la fen�tre : " + clientWidth + "x" + clientHeight);
												Utils.LogMessage("Taille maximis�e de la fen�tre sur l'�cran : " + maxWindowWidth + "x" + maxWindowHeight);

												if (forceResizeWidth > 0 && forceResizeHeight > 0)
												{
													WindowResizer.ResizeWindowToClientRect(windowHandle, forceResizeWidth, forceResizeHeight, true);
													magpieNoMoveWindow = true;
													if (magpieNoLaunchIfSameRes)
													{
														if (forceResizeWidth != Screen.PrimaryScreen.Bounds.Width) magpieNoLaunchIfSameRes = false;
														if (forceResizeHeight != Screen.PrimaryScreen.Bounds.Height) magpieNoLaunchIfSameRes = false;
													}
												}
												else
												{
													if (magpieNoLaunchIfSameRes)
													{
														if (clientWidth != Screen.PrimaryScreen.Bounds.Width) magpieNoLaunchIfSameRes = false;
														if (clientHeight != Screen.PrimaryScreen.Bounds.Height) magpieNoLaunchIfSameRes = false;
													}
													if (forceWindowResize) WindowResizer.ResizeWindowToClientRect(windowHandle, maxWindowWidth, maxWindowHeight);
												}

												double borderSize = 0;
												if (magpieSindenBorder) borderSize = magpieBorderSize;

												double widthWindowWithoutBorder = (maxWindowWidth / 100.0) * (100.0 - (borderSize * 2));
												double heightWindowWithoutBorder = (maxWindowHeight / 100.0) * (100.0 - (borderSize * 2));

												Utils.LogMessage("Taille ajust� sans bordure : " + widthWindowWithoutBorder + "x" + heightWindowWithoutBorder);
												double ratioReshadeBorderWidth = maxWindowWidth / widthWindowWithoutBorder;
												double ratioReshadeBorderHeight = maxWindowHeight / heightWindowWithoutBorder;

												Utils.LogMessage("Ratio Reshade : " + ratioReshadeBorderWidth + "x" + ratioReshadeBorderHeight);

												double ratioVjoyWidth = screenWidth / widthWindowWithoutBorder;
												double ratioVjoyHeight = screenHeight / heightWindowWithoutBorder;

												Utils.LogMessage("Ratio vjoy : " + ratioVjoyWidth + "x" + ratioVjoyHeight);

												double pourcentageWidth = (((screenWidth - widthWindowWithoutBorder) / 2.0) / screenWidth) * 100;
												double pourcentageHeight = (((screenHeight - heightWindowWithoutBorder) / 2.0) / screenHeight) * 100;
												Utils.LogMessage("Start Percent : " + pourcentageWidth + "x" + pourcentageHeight);

												Utils.LogMessage("Sinden Config :");
												Utils.LogMessage("X Offset = " + pourcentageWidth * -1);
												Utils.LogMessage("X RatioFactor = " + (1 + (pourcentageWidth / 50.0)));
												Utils.LogMessage("Y Offset = " + pourcentageHeight * -1);
												Utils.LogMessage("Y RatioFactor = " + (1 + (pourcentageHeight / 50.0)));

												if (magpieCalibrateSindenSoft)
												{
													double XOffsetValue = pourcentageWidth * -1;
													double XRatioFactorValue = (1 + (pourcentageWidth / 50.0));
													double YOffsetValue = pourcentageHeight * -1;
													double YRatioFactorValue = (1 + (pourcentageHeight / 50.0));

													if (XOffsetValue <= 0.0 && XOffsetValue > -50.0
													&& XRatioFactorValue >= 1.0 && XRatioFactorValue <= 2.0
													&& YRatioFactorValue >= 1.0 && YRatioFactorValue <= 2.0
													&& YOffsetValue <= 0.0 && YOffsetValue > -50.0)
													{
														string XOffsetString = Math.Round(XOffsetValue, 5).ToString("0.#####", System.Globalization.CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
														string XRatioFactorString = Math.Round(XRatioFactorValue, 5).ToString("0.#####", System.Globalization.CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
														string YOffsetString = Math.Round(YOffsetValue, 5).ToString("0.#####", System.Globalization.CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
														string YRatioFactorString = Math.Round(YRatioFactorValue, 5).ToString("0.#####", System.Globalization.CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
														forceSindenCalibration = $"{XOffsetString},{XRatioFactorString},{YOffsetString},{YRatioFactorString}";
														Utils.LogMessage("Sinden Calibration Config : " + forceSindenCalibration);
														string addedArg = @$"-action ""set-offsets {forceSindenCalibration}""";
														Process[] processes = Process.GetProcessesByName("Lightgun");
														if (processes.Length > 0)
														{
															// Si le processus est en cours d'ex�cution, en d�marrer un nouveau avec des arguments suppl�mentaires

															string processPath = processes[0].MainModule.FileName;
															Process sinden_process2 = new Process();
															sinden_process2.StartInfo.FileName = processPath;
															sinden_process2.StartInfo.WorkingDirectory = Path.GetDirectoryName(processPath);
															sinden_process2.StartInfo.Arguments = addedArg;
															sinden_process2.StartInfo.UseShellExecute = true;
															sinden_process2.Start();
														}

													}
												}
												if (magpieCalibrateVjoy)
												{
													double ratioVjoyFinalWidth = ratioVjoyWidth - 1;
													double ratioVjoyFinalHeight = ratioVjoyHeight - 1;

													if (ratioVjoyFinalWidth >= 0 && ratioVjoyFinalWidth < 3 && ratioVjoyFinalHeight >= 0 && ratioVjoyFinalHeight < 3)
													{
														string ratioVjoyFinalWidthString = Math.Round(ratioVjoyFinalWidth, 5).ToString("0.#####", System.Globalization.CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
														string ratioVjoyFinalHeightString = Math.Round(ratioVjoyFinalHeight, 5).ToString("0.#####", System.Globalization.CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
														if (ratioVjoyFinalWidthString == "") ratioVjoyFinalWidthString = "0";
														if (ratioVjoyFinalHeightString == "") ratioVjoyFinalHeightString = "0";



														forcevjoyXformula = $"[X]+(([X]-(32767/2))*{ratioVjoyFinalWidthString})";
														forcevjoyYformula = $"[Y]+(([Y]-(32767/2))*{ratioVjoyFinalHeightString})";

														if (gameOptions.tmpGunXFormula != "") forcevjoyXformula = gameOptions.tmpGunXFormula;
														if (gameOptions.tmpGunYFormula != "") forcevjoyYformula = gameOptions.tmpGunYFormula;

														Utils.LogMessage("Vjoy forced formula X : " + forcevjoyXformula);
														Utils.LogMessage("Vjoy forced formula Y : " + forcevjoyYformula);

														try
														{
															NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "VjoyControlCommand", PipeDirection.Out);
															pipeClient.Connect(5000);

															if (pipeClient.IsConnected)
															{

																using (StreamWriter writer = new StreamWriter(pipeClient, Encoding.UTF8))
																{
																	writer.Write(@$"formula={forcevjoyXformula}<-->{forcevjoyYformula}");
																	writer.Flush();
																	if (gameOptions.tmpGunAXFormulaBefore != "" || gameOptions.tmpGunAYFormulaBefore != "")
																	{

																		writer.Write(@$"beforeformula1={gameOptions.tmpGunAXFormulaBefore}<-->{gameOptions.tmpGunAYFormulaBefore}");
																		writer.Flush();
																	}
																	if (gameOptions.tmpGunBXFormulaBefore != "" || gameOptions.tmpGunBYFormulaBefore != "")
																	{
																		writer.Write(@$"beforeformula2={gameOptions.tmpGunBXFormulaBefore}<-->{gameOptions.tmpGunBYFormulaBefore}");
																		writer.Flush();
																	}

																}
																pipeClient.Close();
															}
														}
														catch (Exception ex)
														{
															Utils.LogMessage("Exception :  " + ex.Message);
														}
													}
												}
												//string reshadeString = "";
												if (magpieSindenBorder)
												{

													string magpieIniContent = "";
													if (Program.magpieIni != "")
													{
														magpieIniContent = Utils.ReadAllText(Program.magpieIni);
													}

													if (magpieIniContent != "")
													{
														string existingTechniques = Regex.Match(magpieIniContent, @"^Techniques=(.*)", RegexOptions.Multiline).Groups[1].Value.Trim('\n').Trim('\r').Trim('\n');
														string newTechniques = "";
														bool addGridLast = false;
														bool addBezelAfterTransform = false;
														foreach (var technique in existingTechniques.Split(','))
														{
															if (technique.ToLower().Trim() == "CShade_TransformSinden@cTransformSinden.fx".ToLower()) continue;
															if (technique.ToLower().Trim() == "BorderSinden@BorderSinden.fx".ToLower()) continue;
															if (technique.ToLower().Trim() == "GridLast@GridLast.fx".ToLower())
															{
																addGridLast = true;
																continue;
															}
															if (technique.ToLower().Trim() == "BezelMagpie@BezelMagpie.fx".ToLower())
															{
																addBezelAfterTransform = true;
																continue;
															}
															newTechniques += technique + ",";
														}
														if (addBezelAfterTransform) newTechniques += "CShade_TransformSinden@cTransformSinden.fx,BezelMagpie@BezelMagpie.fx,BorderSinden@BorderSinden.fx";
														else newTechniques += "CShade_TransformSinden@cTransformSinden.fx,BorderSinden@BorderSinden.fx";
														if (addGridLast) newTechniques += ",GridLast@GridLast.fx";
														magpieIniContent = Regex.Replace(magpieIniContent, @"^Techniques=.*", "Techniques=" + newTechniques, RegexOptions.Multiline);
													}
													else
													{
														magpieIniContent = @"
Techniques=CShade_TransformSinden@cTransformSinden.fx,BorderSinden@BorderSinden.fx
TechniqueSorting=CShade_TransformSinden@cTransformSinden.fx,BorderSinden@BorderSinden.fx

[BorderSinden.fx]
border_color=1.000000,1.000000,1.000000
border_width=1.5,1.5
outerblack=0
use4tiers=0

[cTransformSinden.fx]
_Angle=0.000000
_Scale=1.030928,1.030928
_Translate=0.000000,0.000000

";
													}


													//magpieExe = ConfigurationManager.MainConfig.magpieSindenExe;
													//magpieConfig = Path.Combine(Path.GetDirectoryName(magpieExe), "config", "config.json");
													/*
													reshadeString = "";
													if (magpieReshadeAdaptiveSharpen) reshadeString += "AdaptiveSharpen@AdaptiveSharpen.fx,";
													if (magpieReshadeClarity) reshadeString += "Clarity@Clarity.fx,";
													if (magpieReshadeAdaptiveSharpen) reshadeString += "Colourfulness@Colourfulness.fx,";
													//if (magpieReshade43) reshadeString += "AspectRatioPS@AspectRatio.fx,";
													*/
													//reshadeString = "CShade_Transform@cTransform.fx,BorderSinden@BorderSinden.fx";



													string magpieReshadeIni = Path.Combine(Path.GetDirectoryName(magpieExe), "ReShadePreset.ini");
													File.WriteAllText(magpieReshadeIni, magpieIniContent);
													magpieContentWritten = true;
													if (File.Exists(magpieReshadeIni))
													{
														string ratioReshadeBorderWidthString = Math.Round(ratioReshadeBorderWidth, 6).ToString("0.######", System.Globalization.CultureInfo.InvariantCulture);
														string ratioReshadeBorderHeightString = Math.Round(ratioReshadeBorderHeight, 6).ToString("0.######", System.Globalization.CultureInfo.InvariantCulture);
														string bordersizeString = Math.Round(borderSize, 6).ToString("0.######", System.Globalization.CultureInfo.InvariantCulture);
														IniFile reshadeIni = new IniFile(magpieReshadeIni);
														reshadeIni.Write("border_width", $"{bordersizeString},{bordersizeString}", "BorderSinden.fx");
														reshadeIni.Write("border_color", $"1.000000,1.000000,1.000000", "BorderSinden.fx");

														//_Scale=1.030928,1.03093
														reshadeIni.Write("_Scale", $"{ratioReshadeBorderWidthString},{ratioReshadeBorderHeightString}", "cTransformSinden.fx");

													}
												}


												if (magpieNoLaunchIfSameRes)
												{
													if (clientWidth != Screen.PrimaryScreen.Bounds.Width) magpieNoLaunchIfSameRes = false;
													if (clientHeight != Screen.PrimaryScreen.Bounds.Height) magpieNoLaunchIfSameRes = false;
												}

												if (File.Exists(magpieExe) && File.Exists(magpieConfig))
												{

													string magpieReshadeDll = Path.Combine(Path.GetDirectoryName(magpieExe), "d2d1.dll");
													if (string.IsNullOrEmpty(Program.magpieIni) && magpieContentWritten == false)
													{
														if (File.Exists(magpieReshadeDll))
														{
															File.Move(magpieReshadeDll, magpieReshadeDll + ".disabled");
														}
													}
													else
													{
														magpieNoLaunchIfSameRes = false;
														if (File.Exists(magpieReshadeDll + ".disabled"))
														{
															File.Move(magpieReshadeDll + ".disabled", magpieReshadeDll);
														}

														string magpieReshadeIni = Path.Combine(Path.GetDirectoryName(magpieExe), "ReShadePreset.ini");
														if (!magpieContentWritten)
														{
															File.Copy(Program.magpieIni, magpieReshadeIni, true);
														}

														string cacheShaderDirMagpie = Path.Combine(Path.GetDirectoryName(magpieExe), "reshade-shaders", "Cache");
														if (Directory.Exists(cacheShaderDirMagpie))
														{
															try
															{
																string[] files = Directory.GetFiles(cacheShaderDirMagpie, "reshade-BezelMagpie*");
																foreach (string file in files) File.Delete(file);
																string[] files2 = Directory.GetFiles(cacheShaderDirMagpie, "reshade-UIMask*");
																foreach (string file2 in files) File.Delete(file2);

															}
															catch (Exception ex) { }
														}
														else
														{
															try
															{
																Directory.CreateDirectory(cacheShaderDirMagpie);
															}
															catch { }
														}

														string magpieReshadeMainIni = Path.Combine(Path.GetDirectoryName(magpieExe), "ReShadePreset.ini");
														if (File.Exists(magpieReshadeMainIni))
														{
															IniFile reshadeIniFile = new IniFile(magpieReshadeMainIni);
															reshadeIniFile.Write("IntermediateCachePath", Path.GetFullPath(cacheShaderDirMagpie), "GENERAL");
														}

													}

													string jsonText = Utils.ReadAllText(magpieConfig);
													JObject jsonObject = JObject.Parse(jsonText);
													jsonObject["allowScalingMaximized"] = magpieAllowScalingMaximized;
													jsonObject["simulateExclusiveFullscreen"] = magpieExclusiveFullscreen;
													try
													{
														double sharpnessValue = (double)magpieFsrSharp / 100.0;
														jsonObject["scalingModes"][1]["effects"][1]["parameters"]["sharpness"] = sharpnessValue;
													}
													catch { }
													JArray profilesArray = (JArray)jsonObject["profiles"];
													foreach (JObject profile in profilesArray)
													{
														if (profile["name"] != null && profile["name"].ToString() == "Teknoparrot")
														{
															profile["pathRule"] = magpieExecutableGame;
															profile["classNameRule"] = trueClassName;
															profile["scalingMode"] = magpieScaling;
															profile["captureMethod"] = magpieCapture;
															profile["VSync"] = magpieVsync;
															profile["tripleBuffering"] = magpieTripleBuffering;
															profile["showFPS"] = magpieShowFps;
															profile["drawCursor"] = magpieShowCursor;
															profile["croppingEnabled"] = magpieCrop;
															profile["cropping"]["left"] = magpieCropValue[0];
															profile["cropping"]["top"] = magpieCropValue[1];
															profile["cropping"]["right"] = magpieCropValue[2];
															profile["cropping"]["bottom"] = magpieCropValue[3];
														}
													}
													string modifiedJsonText = JsonConvert.SerializeObject(jsonObject, Newtonsoft.Json.Formatting.Indented);
													File.WriteAllText(magpieConfig, modifiedJsonText);
													try
													{
														if (!magpieNoMoveWindow) Utils.MoveWindowsToZero(windowHandle);
														Thread.Sleep(100);
														Utils.SetForegroundWindow(windowHandle);
														Thread.Sleep(100);

														//Utils.ClickWindow(windowHandle);
													}
													catch { }


													if (!magpieNoLaunchIfSameRes)
													{
														Process magpie_process = new Process();
														magpie_process.StartInfo.FileName = magpieExe;
														magpie_process.StartInfo.Arguments = "-t";
														magpie_process.StartInfo.WorkingDirectory = Path.GetDirectoryName(magpieExe);
														magpie_process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized; // Ajout de cette ligne pour minimiser la fen�tre
														magpie_process.StartInfo.UseShellExecute = true;
														magpie_process.Start();
														magpie_process_pid = magpie_process.Id;
													}
													else
													{
														int screenLeft = Screen.PrimaryScreen.Bounds.Left;
														int screenBottom = Screen.PrimaryScreen.Bounds.Bottom;

														// D�placer le curseur en bas � gauche du deuxi�me �cran
														Utils.SetCursorPos(screenLeft, screenBottom - 1);
													}

													if (!magpieNoClick)
													{
														var ClickOnPrimaryScreen = new Thread(() =>
														{
															for (int i = 0; i < 100; i++)
															{
																if (isExiting) break;
																string foregroundClassName = Utils.GetForegroundClassName();
																Thread.Sleep(100);
																if (foregroundClassName == "Magpie_Main")
																{
																	Thread.Sleep(2000);
																	Utils.ClickOnPrimaryScreen(30, 30);
																	break;
																}
															}
														});
														ClickOnPrimaryScreen.Start();

													}




												}


												return;

											}
										});
										WaitForWindowed.Start();


									}

									/*
									var WaitForMagpie = new Thread(() =>
									{
										IntPtr windowHandle = Utils.FindWindowByMultipleCriteria("Magpie_Main", "", "");
										while (!isExiting)
										{
											while (windowHandle == IntPtr.Zero && !isExiting)
											{
												windowHandle = Utils.FindWindowByMultipleCriteria("Magpie_Main", "", "");
												Thread.Sleep(500);
											}
											Utils.ClickWindow(windowHandle);

										}
									});
									WaitForMagpie.Start();
									*/


								}

								bool useRivaTuner = gameOptions.runRivaTuner;
								/*
								if(!useRivaTuner && (need60hz || need60hzFullscreen || need60hzWindowed))
								{
									bool upscaleFullscreen = false;
									if (GameInfo.ContainsKey("upscaleFullscreen") && GameInfo["upscaleFullscreen"].ToLower() == "true") upscaleFullscreen = true;
									bool upscaleWindowed = false;
									if (GameInfo.ContainsKey("upscaleWindowed") && GameInfo["upscaleWindowed"].ToLower() == "true") upscaleWindowed = true;


									if (!TpSettingsManager.IsWindowed && !forceResSwitchFullscreen && upscaleFullscreen)
									{
										int target_refreshrate = 60;
										int target_width = 0;
										int target_height = 0;
										if (Program.gpuResolution == 0)
										{
											target_width = 1280;
											target_height = 720;
										}
										if (Program.gpuResolution == 1)
										{
											target_width = 1920;
											target_height = 1080;
										}
										if (Program.gpuResolution == 2)
										{
											target_width = 2560;
											target_height = 1440;
										}
										if (Program.gpuResolution == 3)
										{
											target_width = 3840;
											target_height = 2160;
										}

										var devices = Westwind.SetResolution.DisplayManager.GetAllDisplayDevices();
										var monitor = devices.FirstOrDefault(d => d.IsSelected);  // main monitor
										var list = Westwind.SetResolution.DisplayManager.GetAllDisplaySettings(monitor.DriverDeviceName);

										var resolutionAvailiable = list
											.Where(d => d.BitCount == 32 && d.Orientation == 0)
											.OrderBy(d => d.Width)
											.ThenBy(d => d.Height)
											.ThenBy(d => d.Frequency)
											.FirstOrDefault(d => d.Width == target_width &&
																 d.Height == target_height &&
																 d.Frequency == target_refreshrate);

										if (resolutionAvailiable == null)
										{
											Utils.LogMessage("Future 60hz frequency not found, force rivatuner");
											useRivaTuner = true;
										}




									}
									else
									{
										try
										{
											CCDWrapper.DisplayConfigPathInfo[] pathInfoArray = new CCDWrapper.DisplayConfigPathInfo[0];
											CCDWrapper.DisplayConfigModeInfo[] modeInfoArray = new CCDWrapper.DisplayConfigModeInfo[0];
											CCDWrapper.MonitorAdditionalInfo[] additionalInfo = new CCDWrapper.MonitorAdditionalInfo[0];
											CCDWrapper.DpiInfo[] dpiInfoArray = new CCDWrapper.DpiInfo[0];
											Boolean status = MonitorSwitcher.GetDisplaySettings(ref pathInfoArray, ref modeInfoArray, ref additionalInfo, ref dpiInfoArray, true);
											if (status)
											{
												var monitorData = MonitorSwitcher.PrintDisplaySettings2ForFrequency(pathInfoArray, modeInfoArray, dpiInfoArray);
												int freq = Utils.GetPrimaryMonitorRefreshRateFromXml(monitorData);
												if(freq > 60)
												{
													Utils.LogMessage($"frequency = {freq}, force rivatuner");
													useRivaTuner = true;
												}
											}
										}
										catch { }

									}


								}
								*/

								string rivaTunerExe = ConfigurationManager.MainConfig.rivatunerExe;
								if (useRivaTuner && rivaTunerExe != "" && File.Exists(rivaTunerExe))
								{
									Utils.LogMessage($"Use Riva Tuner = {rivaTunerExe}");
									string rivaTunerIniFile = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "rivatuner.ini");
									rivaTunerIniFile = Path.GetFullPath(rivaTunerIniFile);
									IniFile ini = new IniFile(rivaTunerIniFile);
									ini.Write("ParentProcess", Process.GetCurrentProcess().Id.ToString());
									string selfExe = Process.GetCurrentProcess().MainModule.FileName;
									if (!Utils.CheckTaskExist(selfExe, "--rivatuner"))
									{
										string exePath = selfExe;
										string exeDir = Path.GetDirectoryName(exePath);
										Process process = new Process();
										process.StartInfo.FileName = selfExe;
										process.StartInfo.Arguments = "--registerTask " + $"\"{selfExe}\" " + "--rivatuner";
										process.StartInfo.WorkingDirectory = exeDir;
										process.StartInfo.UseShellExecute = true;
										process.StartInfo.Verb = "runas";
										process.Start();
										process.WaitForExit();
									}
									Utils.ExecuteTask(Utils.ExeToTaskName(selfExe, "--rivatuner"), -1);

								}

								Thread XenosThread = null;

								if (useXenos && (!gameOptions.useInjector || gameOptions.injectorDllList == "")) useXenos = false;
								if (useXenos)
								{
									var dllCheckedList = new List<string>();
									foreach (var dll in gameOptions.injectorDllList.Split(','))
									{
										if (File.Exists(Path.Combine(executableGameDir, dll))) dllCheckedList.Add(Path.GetFullPath(Path.Combine(executableGameDir, dll)));
									}

									string Xenos32 = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "xenos", "Xenos.exe");
									string Xenos64 = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "xenos", "Xenos64.exe");
									string xenosEmptyConf = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "xenos", "empty.xpr");

									string xenosConf32 = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "xenos", "teknoparrot32.xpr");
									string xenosConf64 = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "xenos", "teknoparrot64.xpr");


									if (File.Exists(xenosConf32)) File.Delete(xenosConf32);
									if (File.Exists(xenosConf64)) File.Delete(xenosConf64);

									bool validXenos = true;
									if (!File.Exists(Xenos32) || !File.Exists(Xenos64))
									{
										MessageBox.Show("Missing Xenos exe");
										validXenos = false;
									}
									if (!File.Exists(xenosEmptyConf))
									{
										MessageBox.Show("Missing " + xenosEmptyConf);
										validXenos = false;
									}

									if (validXenos)
									{


										XenosThread = new Thread(() =>
										{
											string processExist = Utils.ProcessExist(Path.GetFileNameWithoutExtension(executableGame));
											while (processExist == "" && !isExiting)
											{
												processExist = Utils.ProcessExist(Path.GetFileNameWithoutExtension(executableGame));
												Thread.Sleep(500);
											}

											string xenosOutConf = xenosConf32;
											if (processExist == "64")
											{
												xenosOutConf = xenosConf64;
											}
											int xenosDelay = (gameOptions.injectorDelay * 1000);

											if (gameOptions.injectorWaitForGameWindow)
											{
												string trueClassName = "";
												IntPtr windowHandle = Utils.FindWindowByMultipleCriteria(magpieClass, Path.GetFileNameWithoutExtension(magpieExecutableGame), magpieTitle, out trueClassName);
												while (windowHandle == IntPtr.Zero && !isExiting)
												{
													windowHandle = Utils.FindWindowByMultipleCriteria(magpieClass, Path.GetFileNameWithoutExtension(magpieExecutableGame), magpieTitle, out trueClassName);
													Thread.Sleep(500);
													Utils.LogMessage("Search Window for injector ...");
													if (windowHandle != IntPtr.Zero)
													{
														Utils.RECT clientRectTest;
														Utils.GetClientRect(windowHandle, out clientRectTest);
														int clientWidthTest = clientRectTest.Right - clientRectTest.Left;
														if (clientWidthTest < 150) windowHandle = IntPtr.Zero;
													}
												}
												Utils.LogMessage("Found Window for injector ...");
											}

											Thread.Sleep(xenosDelay);

											XmlDocument xmlDocumentXenos = new XmlDocument();
											xmlDocumentXenos.Load(xenosEmptyConf);

											XmlNode procNameNode = xmlDocumentXenos.SelectSingleNode("//procName");
											procNameNode.InnerText = Path.GetFileName(executableGame);

											XmlNode delayNode = xmlDocumentXenos.SelectSingleNode("//delay");
											//delayNode.InnerText = xenosDelay.ToString();
											delayNode.InnerText = "500";

											// Ajouter un n�ud <imagePath> pour chaque �l�ment dans la liste
											foreach (string imagePath in dllCheckedList)
											{
												XmlElement imagePathNode = xmlDocumentXenos.CreateElement("imagePath");
												imagePathNode.InnerText = imagePath;
												XmlNode xenosConfigNode = xmlDocumentXenos.SelectSingleNode("XenosConfig");
												xenosConfigNode.AppendChild(imagePathNode);
											}
											xmlDocumentXenos.Save(xenosOutConf);

											string selfExe = Process.GetCurrentProcess().MainModule.FileName;
											if (!Utils.CheckTaskExist(selfExe, "--xenos"))
											{
												string exePath = selfExe;
												string exeDir = Path.GetDirectoryName(exePath);
												Process process = new Process();
												process.StartInfo.FileName = selfExe;
												process.StartInfo.Arguments = "--registerTask " + $"\"{selfExe}\" " + "--xenos";
												process.StartInfo.WorkingDirectory = exeDir;
												process.StartInfo.UseShellExecute = true;
												process.StartInfo.Verb = "runas";
												process.Start();
												process.WaitForExit();
											}

											Utils.ExecuteTask(Utils.ExeToTaskName(selfExe, "--xenos"), -1);
											return;
										});
										XenosThread.Start();
									}
								}

								InputLoop inputLoop = null;
								//MessageBox.Show($"firstPlayerIsXinputGamepad={firstPlayerIsXinputGamepad}, firstPlayerIsXinputArcade={firstPlayerIsXinputArcade}");
								if (firstPlayerIsXinputGamepad >= 0)
								{
									if (GameInfo.ContainsKey("mouseKeyboardToFirstXinputGamepad") && GameInfo["mouseKeyboardToFirstXinputGamepad"].ToLower() == "true")
									{
										XGamepad gamepad = new((XInputUserIndex)firstPlayerIsXinputGamepad);
										XInputDeviceManager deviceManager = new();  // We will use this to monitor all physical XInput devices.
										deviceManager.DeviceStateChanged += (_, e) =>
										{
											if (e.Device.IsConnected)
												gamepad.Device = e.Device;
										};

										if(GameInfo.ContainsKey("mouseDpadGamepad") && GameInfo["mouseDpadGamepad"].ToLower() == "true")
										{
											TimeSpan buttonRepeatDelayX = TimeSpan.FromMilliseconds(25);  // When a button is held, how long until it starts repeating?
											TimeSpan buttonRepeatIntervalX = TimeSpan.FromMilliseconds(10);  // While a held button is repeating, how long is the interval?
											gamepad.RegisterButtonRepeatEvent(XButtons.DPadRight, buttonRepeatDelayX, buttonRepeatIntervalX, (_, _) =>
											{
												TimeSpan duration = gamepad.Buttons.DPadRight.Duration;
												double accelerationFactor = 1.0;
												if (duration.TotalSeconds > 3) accelerationFactor = 3.0;
												else accelerationFactor = 1.0 + (duration.TotalSeconds / 2.0);
												ControllerToMouseMapper.Input.Mouse.MoveBy(0.0002 * accelerationFactor, 0);
											});
											gamepad.RegisterButtonRepeatEvent(XButtons.DPadLeft, buttonRepeatDelayX, buttonRepeatIntervalX, (_, _) =>
											{
												TimeSpan duration = gamepad.Buttons.DPadLeft.Duration;
												double accelerationFactor = 1.0;
												if (duration.TotalSeconds > 3) accelerationFactor = 3.0;
												else accelerationFactor = 1.0 + (duration.TotalSeconds / 2.0);
												ControllerToMouseMapper.Input.Mouse.MoveBy(-0.0002 * accelerationFactor, 0);
											});
											gamepad.RegisterButtonRepeatEvent(XButtons.DPadDown, buttonRepeatDelayX, buttonRepeatIntervalX, (_, _) =>
											{
												TimeSpan duration = gamepad.Buttons.DPadDown.Duration;
												double accelerationFactor = 1.0;
												if (duration.TotalSeconds > 3) accelerationFactor = 3.0;
												else accelerationFactor = 1.0 + (duration.TotalSeconds / 2.0);
												ControllerToMouseMapper.Input.Mouse.MoveBy(0, 0.0002 * accelerationFactor);
											});
											gamepad.RegisterButtonRepeatEvent(XButtons.DPadUp, buttonRepeatDelayX, buttonRepeatIntervalX, (_, _) =>
											{
												TimeSpan duration = gamepad.Buttons.DPadUp.Duration;
												double accelerationFactor = 1.0;
												if (duration.TotalSeconds > 3) accelerationFactor = 3.0;
												else accelerationFactor = 1.0 + (duration.TotalSeconds / 2.0);
												ControllerToMouseMapper.Input.Mouse.MoveBy(0, -0.0002 * accelerationFactor);
											});
										}

										if (GameInfo.ContainsKey("mouseLeftJoySpeed") && GameInfo["mouseLeftJoySpeed"].Trim() != "")
										{
											double mouseLeftJoySpeed = 0.015;
											double.TryParse(GameInfo["mouseLeftJoySpeed"].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out mouseLeftJoySpeed);

											var mouseJoystickLeft = gamepad.LeftJoystick;
											float mouseLeftJoyDeadZone = 0.2f;
											if (GameInfo.ContainsKey("mouseLeftJoyDeadZone") && GameInfo["mouseLeftJoyDeadZone"].Trim() != "") float.TryParse(GameInfo["mouseLeftJoyDeadZone"].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out mouseLeftJoyDeadZone);
											mouseJoystickLeft.InnerDeadZone = mouseLeftJoyDeadZone;  // Let's use a circular dead zone.
											mouseJoystickLeft.RadiusModifierFunction = NonLinearFunctions.QuadraticEaseIn;  // Make the joystick more precise near the center.
											mouseJoystickLeft.Updated += (_, _) =>
											{
												if (mouseJoystickLeft.IsPushed)
												{
													ControllerToMouseMapper.Input.Mouse.MoveBy(
														mouseJoystickLeft.X * mouseLeftJoySpeed * mouseJoystickLeft.FrameTime.TotalSeconds,
														-mouseJoystickLeft.Y * mouseLeftJoySpeed * mouseJoystickLeft.FrameTime.TotalSeconds);
												}
											};
										}
										if (GameInfo.ContainsKey("mouseRightJoySpeed") && GameInfo["mouseRightJoySpeed"].Trim() != "")
										{
											double mouseRightJoySpeed = 0.015;
											double.TryParse(GameInfo["mouseRightJoySpeed"].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out mouseRightJoySpeed);

											var mouseJoystickRight = gamepad.RightJoystick;
											float mouseRightJoyDeadZone = 0.2f;
											if (GameInfo.ContainsKey("mouseRightJoyDeadZone") && GameInfo["mouseRightJoyDeadZone"].Trim() != "") float.TryParse(GameInfo["mouseRightJoyDeadZone"].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out mouseRightJoyDeadZone);
											mouseJoystickRight.InnerDeadZone = mouseRightJoyDeadZone;
											mouseJoystickRight.RadiusModifierFunction = NonLinearFunctions.QuadraticEaseIn;
											mouseJoystickRight.Updated += (_, _) =>
											{
												if (mouseJoystickRight.IsPushed)
												{
													ControllerToMouseMapper.Input.Mouse.MoveBy(
														mouseJoystickRight.X * mouseRightJoySpeed * mouseJoystickRight.FrameTime.TotalSeconds,
														-mouseJoystickRight.Y * mouseRightJoySpeed * mouseJoystickRight.FrameTime.TotalSeconds);
												}
											};
										}

										if (GameInfo.ContainsKey("mouseBindGamepad") && GameInfo["mouseBindGamepad"].Trim() != "")
										{
											string mouseBind_txt = GameInfo["mouseBindGamepad"].Trim();
											foreach (var mouseBind_elem in mouseBind_txt.Split(","))
											{
												var mouseBind_items = mouseBind_elem.Split("=");
												if(mouseBind_items.Count() == 2)
												{
													string mouseBind_trigger = mouseBind_items[0].Trim().ToLower();
													string mouseBind_action = mouseBind_items[1].Trim();
													XInputButton mouseTrigger;
													ControllerToMouseMapper.Input.MouseButton mouseAction;

													if (mouseBind_trigger == "dpadup") mouseTrigger = gamepad.Buttons.DPadUp;
													else if (mouseBind_trigger == "dpaddown") mouseTrigger = gamepad.Buttons.DPadDown;
													else if (mouseBind_trigger == "dpadleft") mouseTrigger = gamepad.Buttons.DPadLeft;
													else if (mouseBind_trigger == "dpadright") mouseTrigger = gamepad.Buttons.DPadRight;
													else if (mouseBind_trigger == "start") mouseTrigger = gamepad.Buttons.Start;
													else if (mouseBind_trigger == "back") mouseTrigger = gamepad.Buttons.Back;
													else if (mouseBind_trigger == "a") mouseTrigger = gamepad.Buttons.A;
													else if (mouseBind_trigger == "b") mouseTrigger = gamepad.Buttons.B;
													else if (mouseBind_trigger == "x") mouseTrigger = gamepad.Buttons.X;
													else if (mouseBind_trigger == "y") mouseTrigger = gamepad.Buttons.Y;
													else if (mouseBind_trigger == "lb") mouseTrigger = gamepad.Buttons.LB;
													else if (mouseBind_trigger == "rb") mouseTrigger = gamepad.Buttons.RB;
													else if (mouseBind_trigger == "ls") mouseTrigger = gamepad.Buttons.LS;
													else if (mouseBind_trigger == "rs") mouseTrigger = gamepad.Buttons.RS;
													else
													{
														continue;
													}

													try
													{
														mouseAction = (ControllerToMouseMapper.Input.MouseButton)Enum.Parse(typeof(ControllerToMouseMapper.Input.MouseButton), mouseBind_action);
													}
													catch
													{
														continue;
													}
													mouseTrigger.Pressed += (_, _) => ControllerToMouseMapper.Input.Mouse.PressButton(mouseAction);
													mouseTrigger.Released += (_, _) => ControllerToMouseMapper.Input.Mouse.ReleaseButton(mouseAction);
												}
											}
										}
										if (GameInfo.ContainsKey("keyboardBindGamepad") && GameInfo["keyboardBindGamepad"].Trim() != "")
										{
											string mouseBind_txt = GameInfo["keyboardBindGamepad"].Trim();
											foreach (var mouseBind_elem in mouseBind_txt.Split(","))
											{
												var mouseBind_items = mouseBind_elem.Split("=");
												if (mouseBind_items.Count() == 2)
												{
													string mouseBind_trigger = mouseBind_items[0].Trim().ToLower();
													string mouseBind_action = mouseBind_items[1].Trim();
													XInputButton mouseTrigger;
													ControllerToMouseMapper.Input.KeyboardVirtualKey mouseAction;

													if (mouseBind_trigger == "dpadup") mouseTrigger = gamepad.Buttons.DPadUp;
													else if (mouseBind_trigger == "dpaddown") mouseTrigger = gamepad.Buttons.DPadDown;
													else if (mouseBind_trigger == "dpadleft") mouseTrigger = gamepad.Buttons.DPadLeft;
													else if (mouseBind_trigger == "dpadright") mouseTrigger = gamepad.Buttons.DPadRight;
													else if (mouseBind_trigger == "start") mouseTrigger = gamepad.Buttons.Start;
													else if (mouseBind_trigger == "back") mouseTrigger = gamepad.Buttons.Back;
													else if (mouseBind_trigger == "a") mouseTrigger = gamepad.Buttons.A;
													else if (mouseBind_trigger == "b") mouseTrigger = gamepad.Buttons.B;
													else if (mouseBind_trigger == "x") mouseTrigger = gamepad.Buttons.X;
													else if (mouseBind_trigger == "y") mouseTrigger = gamepad.Buttons.Y;
													else if (mouseBind_trigger == "lb") mouseTrigger = gamepad.Buttons.LB;
													else if (mouseBind_trigger == "rb") mouseTrigger = gamepad.Buttons.RB;
													else if (mouseBind_trigger == "ls") mouseTrigger = gamepad.Buttons.LS;
													else if (mouseBind_trigger == "rs") mouseTrigger = gamepad.Buttons.RS;
													else
													{
														continue;
													}

													try
													{
														mouseAction = (ControllerToMouseMapper.Input.KeyboardVirtualKey)Enum.Parse(typeof(ControllerToMouseMapper.Input.KeyboardVirtualKey), mouseBind_action);
													}
													catch
													{
														continue;
													}
													mouseTrigger.Pressed += (_, _) => ControllerToMouseMapper.Input.Keyboard.TapKey(mouseAction);
												}
											}
										}

										inputLoop = new(time =>
										{
											deviceManager.Update();  // All input events will fire here.
										});
										inputLoop.Start();  // Start our input loop, to listen for input events.

									}


								}
								if (firstPlayerIsXinputArcade >= 0)
								{
									if (GameInfo.ContainsKey("mouseKeyboardToFirstXinputArcade") && GameInfo["mouseKeyboardToFirstXinputArcade"].ToLower() == "true")
									{
										XGamepad gamepad = new((XInputUserIndex)firstPlayerIsXinputArcade);
										XInputDeviceManager deviceManager = new();  // We will use this to monitor all physical XInput devices.
										deviceManager.DeviceStateChanged += (_, e) =>
										{
											if (e.Device.IsConnected)
												gamepad.Device = e.Device;
										};

										if (GameInfo.ContainsKey("mouseDpadArcade") && GameInfo["mouseDpadArcade"].ToLower() == "true")
										{
											TimeSpan buttonRepeatDelayX = TimeSpan.FromMilliseconds(25);  // When a button is held, how long until it starts repeating?
											TimeSpan buttonRepeatIntervalX = TimeSpan.FromMilliseconds(10);  // While a held button is repeating, how long is the interval?
											gamepad.RegisterButtonRepeatEvent(XButtons.DPadRight, buttonRepeatDelayX, buttonRepeatIntervalX, (_, _) =>
											{
												TimeSpan duration = gamepad.Buttons.DPadRight.Duration;
												double accelerationFactor = 1.0;
												if (duration.TotalSeconds > 3) accelerationFactor = 3.0;
												else accelerationFactor = 1.0 + (duration.TotalSeconds / 2.0);
												ControllerToMouseMapper.Input.Mouse.MoveBy(0.0002 * accelerationFactor, 0);
											});
											gamepad.RegisterButtonRepeatEvent(XButtons.DPadLeft, buttonRepeatDelayX, buttonRepeatIntervalX, (_, _) =>
											{
												TimeSpan duration = gamepad.Buttons.DPadLeft.Duration;
												double accelerationFactor = 1.0;
												if (duration.TotalSeconds > 3) accelerationFactor = 3.0;
												else accelerationFactor = 1.0 + (duration.TotalSeconds / 2.0);
												ControllerToMouseMapper.Input.Mouse.MoveBy(-0.0002 * accelerationFactor, 0);
											});
											gamepad.RegisterButtonRepeatEvent(XButtons.DPadDown, buttonRepeatDelayX, buttonRepeatIntervalX, (_, _) =>
											{
												TimeSpan duration = gamepad.Buttons.DPadDown.Duration;
												double accelerationFactor = 1.0;
												if (duration.TotalSeconds > 3) accelerationFactor = 3.0;
												else accelerationFactor = 1.0 + (duration.TotalSeconds / 2.0);
												ControllerToMouseMapper.Input.Mouse.MoveBy(0, 0.0002 * accelerationFactor);
											});
											gamepad.RegisterButtonRepeatEvent(XButtons.DPadUp, buttonRepeatDelayX, buttonRepeatIntervalX, (_, _) =>
											{
												TimeSpan duration = gamepad.Buttons.DPadUp.Duration;
												double accelerationFactor = 1.0;
												if (duration.TotalSeconds > 3) accelerationFactor = 3.0;
												else accelerationFactor = 1.0 + (duration.TotalSeconds / 2.0);
												ControllerToMouseMapper.Input.Mouse.MoveBy(0, -0.0002 * accelerationFactor);
											});

										}

										if (GameInfo.ContainsKey("mouseBindArcade") && GameInfo["mouseBindArcade"].Trim() != "")
										{
											string mouseBind_txt = GameInfo["mouseBindArcade"].Trim();
											foreach (var mouseBind_elem in mouseBind_txt.Split(","))
											{
												var mouseBind_items = mouseBind_elem.Split("=");
												if (mouseBind_items.Count() == 2)
												{
													string mouseBind_trigger = mouseBind_items[0].Trim().ToLower();
													string mouseBind_action = mouseBind_items[1].Trim();
													XInputButton mouseTrigger;
													ControllerToMouseMapper.Input.MouseButton mouseAction;

													if (mouseBind_trigger == "dpadup") mouseTrigger = gamepad.Buttons.DPadUp;
													else if (mouseBind_trigger == "dpaddown") mouseTrigger = gamepad.Buttons.DPadDown;
													else if (mouseBind_trigger == "dpadleft") mouseTrigger = gamepad.Buttons.DPadLeft;
													else if (mouseBind_trigger == "dpadright") mouseTrigger = gamepad.Buttons.DPadRight;
													else if (mouseBind_trigger == "start") mouseTrigger = gamepad.Buttons.Start;
													else if (mouseBind_trigger == "back") mouseTrigger = gamepad.Buttons.Back;
													else if (mouseBind_trigger == "a") mouseTrigger = gamepad.Buttons.A;
													else if (mouseBind_trigger == "b") mouseTrigger = gamepad.Buttons.B;
													else if (mouseBind_trigger == "x") mouseTrigger = gamepad.Buttons.X;
													else if (mouseBind_trigger == "y") mouseTrigger = gamepad.Buttons.Y;
													else if (mouseBind_trigger == "lb") mouseTrigger = gamepad.Buttons.LB;
													else if (mouseBind_trigger == "rb") mouseTrigger = gamepad.Buttons.RB;
													else if (mouseBind_trigger == "ls") mouseTrigger = gamepad.Buttons.LS;
													else if (mouseBind_trigger == "rs") mouseTrigger = gamepad.Buttons.RS;
													else
													{
														continue;
													}

													try
													{
														mouseAction = (ControllerToMouseMapper.Input.MouseButton)Enum.Parse(typeof(ControllerToMouseMapper.Input.MouseButton), mouseBind_action);
													}
													catch
													{
														continue;
													}
													mouseTrigger.Pressed += (_, _) => ControllerToMouseMapper.Input.Mouse.PressButton(mouseAction);
													mouseTrigger.Released += (_, _) => ControllerToMouseMapper.Input.Mouse.ReleaseButton(mouseAction);
												}
											}
										}

										if (GameInfo.ContainsKey("keyboardBindArcade") && GameInfo["keyboardBindArcade"].Trim() != "")
										{
											string mouseBind_txt = GameInfo["keyboardBindArcade"].Trim();
											foreach (var mouseBind_elem in mouseBind_txt.Split(","))
											{
												var mouseBind_items = mouseBind_elem.Split("=");
												if (mouseBind_items.Count() == 2)
												{
													string mouseBind_trigger = mouseBind_items[0].Trim().ToLower();
													string mouseBind_action = mouseBind_items[1].Trim();
													XInputButton mouseTrigger;
													ControllerToMouseMapper.Input.KeyboardVirtualKey mouseAction;

													if (mouseBind_trigger == "dpadup") mouseTrigger = gamepad.Buttons.DPadUp;
													else if (mouseBind_trigger == "dpaddown") mouseTrigger = gamepad.Buttons.DPadDown;
													else if (mouseBind_trigger == "dpadleft") mouseTrigger = gamepad.Buttons.DPadLeft;
													else if (mouseBind_trigger == "dpadright") mouseTrigger = gamepad.Buttons.DPadRight;
													else if (mouseBind_trigger == "start") mouseTrigger = gamepad.Buttons.Start;
													else if (mouseBind_trigger == "back") mouseTrigger = gamepad.Buttons.Back;
													else if (mouseBind_trigger == "a") mouseTrigger = gamepad.Buttons.A;
													else if (mouseBind_trigger == "b") mouseTrigger = gamepad.Buttons.B;
													else if (mouseBind_trigger == "x") mouseTrigger = gamepad.Buttons.X;
													else if (mouseBind_trigger == "y") mouseTrigger = gamepad.Buttons.Y;
													else if (mouseBind_trigger == "lb") mouseTrigger = gamepad.Buttons.LB;
													else if (mouseBind_trigger == "rb") mouseTrigger = gamepad.Buttons.RB;
													else if (mouseBind_trigger == "ls") mouseTrigger = gamepad.Buttons.LS;
													else if (mouseBind_trigger == "rs") mouseTrigger = gamepad.Buttons.RS;
													else
													{
														continue;
													}

													try
													{
														mouseAction = (ControllerToMouseMapper.Input.KeyboardVirtualKey)Enum.Parse(typeof(ControllerToMouseMapper.Input.KeyboardVirtualKey), mouseBind_action);
													}
													catch
													{
														continue;
													}
													mouseTrigger.Pressed += (_, _) => ControllerToMouseMapper.Input.Keyboard.TapKey(mouseAction);
												}
											}
										}

										inputLoop = new(time =>
										{
											deviceManager.Update();  // All input events will fire here.
										});
										inputLoop.Start();  // Start our input loop, to listen for input events.

									}


								}

								string argumentTpExe = "--profile=\"" + finalConfig + "\"";
								if (testMode) argumentTpExe += " --test";

								if (gameOptions.RunAsRoot)
								{
									Utils.LogMessage($"Force RunAsRoot");
									if (!Utils.CheckTaskExist(teknoparrotExe, argumentTpExe))
									{

										string exePath = Process.GetCurrentProcess().MainModule.FileName;
										string exeDir = Path.GetDirectoryName(exePath);
										Process process = new Process();
										process.StartInfo.FileName = exePath;
										process.StartInfo.Arguments = "--registerTask " + $"\"{teknoparrotExe}\" " + argumentTpExe;
										process.StartInfo.WorkingDirectory = exeDir;
										process.StartInfo.UseShellExecute = true;
										process.StartInfo.Verb = "runas";
										process.Start();
										process.WaitForExit();
									}
									Utils.ExecuteTask(Utils.ExeToTaskName(teknoparrotExe, argumentTpExe));
								}
								else
								{
									/*

									*/



									Utils.LogMessage($"Starting {teknoparrotExe} {argumentTpExe}");
									Process process = new Process();
									process.StartInfo.FileName = teknoparrotExe;
									process.StartInfo.Arguments = "--startMinimized " + argumentTpExe;

									bool UseShellExecute = true;
									if (GameInfo.ContainsKey("environmentVariables") && GameInfo["environmentVariables"].Trim() != "")
									{
										string value_environmentVariables = GameInfo["environmentVariables"].Trim().ToLower();
										foreach (var environmentVariable in value_environmentVariables.Split("##"))
										{

											if (environmentVariable.Split(",,").Count() == 2)
											{
												var splited_environmentVariable = environmentVariable.Split(",,");
												var environmentVariable_content = splited_environmentVariable[0];
												var environmentVariable_tag = splited_environmentVariable[1].Trim().ToLower();
												//if(magpieRegisterSizeTag_tag == "others" || TpSettingsManager.tags.Contains(magpieRegisterSizeTag_tag))
												if (environmentVariable_tag == "others" || environmentVariable_tag.Replace(" ", "").Split('&').All(tag => TpSettingsManager.tags.Contains(tag)))
												{

													var listEnvVariable = environmentVariable_content.Split("||");
													foreach (var envVariable in listEnvVariable)
													{
														var envVariableData = envVariable.Split("==");
														if (envVariableData.Count() == 2)
														{
															process.StartInfo.EnvironmentVariables[envVariableData[0]] = envVariableData[1];
															UseShellExecute = false;
														}
													}

												}
											}
										}


										/*
										var listEnvVariable = GameInfo["environmentVariables"].Split("||");
										foreach (var envVariable in listEnvVariable)
										{
											var envVariableData = envVariable.Split("==");
											if (envVariableData.Count() == 2)
											{
												process.StartInfo.EnvironmentVariables[envVariableData[0]] = envVariableData[1];
												UseShellExecute = false;
											}
										}
										*/
									}
									/*
									process.StartInfo.EnvironmentVariables["NGLIDE_RESOLUTION"] = "1";
									process.StartInfo.EnvironmentVariables["NGLIDE_ASPECT"] = "2";
									process.StartInfo.EnvironmentVariables["NGLIDE_SPLASH"] = "1";
									*/

									process.StartInfo.WorkingDirectory = Path.GetDirectoryName(teknoparrotExe);
									process.StartInfo.UseShellExecute = UseShellExecute;
									process.Start();
									process.WaitForExit();


									//
								}


								Thread.Sleep(500);
								Utils.LogMessage($"End Execution");
								isExiting = true;

								if(inputLoop != null) inputLoop.Stop();

								ButtonToKeyManager.buttonToKey.StopMonitor();
								DemulshooterManager.Stop();

								if (WaitForWindowed != null && WaitForWindowed.IsAlive)
								{
									WaitForWindowed.Join();
								}

								if (XenosThread != null && XenosThread.IsAlive)
								{
									XenosThread.Join();
								}

								if (sinden_process_pid > 0)
								{
									Utils.KillProcessById(sinden_process_pid);
								}

								/*
								if(p1CrosshairToRestore != "" || p2CrosshairToRestore != "")
								{
									string targetPngDir = executableGameDir;
									string p1Crosshair = Path.Combine(targetPngDir, "p1.png");
									string p2Crosshair = Path.Combine(targetPngDir, "p2.png");
									if(p1CrosshairToRestore != "" && File.Exists(p1CrosshairToRestore))
									{
										try
										{
											File.Move(p1CrosshairToRestore, p1Crosshair, true);
										}
										catch
										{ }
									}
									if (p2CrosshairToRestore != "" && File.Exists(p2CrosshairToRestore))
									{
										try
										{
											File.Move(p2CrosshairToRestore, p2Crosshair, true);
										}
										catch
										{ }
									}
								}
								*/

								if (gameOptions.AhkAfter.Trim() != "")
								{
									Utils.LogMessage($"Execute AhkAfter");
									Utils.ExecuteAHK(gameOptions.AhkAfter, true, gameDir, linkSourceFolderExe);
								}

								if (!nolink)
								{
									if (gameOptions.EnableLink && !String.IsNullOrEmpty(linkTargetFolder) && !String.IsNullOrEmpty(linkSourceFolder) && Directory.Exists(linkSourceFolder) && linkAutoWritable)
									{
										Utils.LogMessage($"CleanHardLinksFiles Elf");
										if ((Utils.IsEligibleHardLink(linkTargetFolder) && linkAutoHardLink) || linkAutoSoftLink)
										{
											Utils.CleanHardLinksFilesOriginal(linkTargetFolder, linkSourceFolder);
										}
									}

									if (gameOptions.EnableLinkExe && !String.IsNullOrEmpty(linkTargetFolderExe) && !String.IsNullOrEmpty(linkSourceFolderExe) && Directory.Exists(linkSourceFolderExe) && linkExeWritable && linkExePrecheck)
									{
										Utils.LogMessage($"CleanHardLinksFiles GameDir");
										if ((Utils.IsEligibleHardLink(linkTargetFolderExe) && linkExeHardLink) || linkExeSoftLink)
										{
											Utils.CleanHardLinksFiles(linkTargetFolderExe, linkSourceFolderExe, executableGameFile);
										}
									}

								}




								Utils.LogMessage($"CleanAndKillAhk");
								Utils.CleanAndKillAhk();

								if (TpSettingsManager.IsPatreon && ConfigurationManager.MainConfig.tpLicenceRegOnLaunch && !ConfigurationManager.MainConfig.tpLicenceUnRegAfterStart)
								{
									using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\TeknoGods\TeknoParrot"))
									{
										var isPatronRegistery = key != null && key.GetValue("PatreonSerialKey") != null;

										if (isPatronRegistery)
										{
											ProcessStartInfo tpUnRegStartInfo = new ProcessStartInfo();
											Process TpUnRegProcess = new Process();
											tpUnRegStartInfo.FileName = Path.Combine(baseTpDir, "TeknoParrot", "BudgieLoader.exe");
											tpUnRegStartInfo.UseShellExecute = false;
											//tpUnRegStartInfo.CreateNoWindow = true;
											tpUnRegStartInfo.Arguments = "-deactivate";
											TpUnRegProcess.StartInfo = tpUnRegStartInfo;
											TpUnRegProcess.Start();
											TpUnRegProcess.WaitForExit();
										}
									}
									using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\TeknoGods\TeknoParrot", true))
									{
										if (key != null)
										{
											Registry.CurrentUser.DeleteSubKey(@"SOFTWARE\TeknoGods\TeknoParrot");
											Console.WriteLine("La cl� de registre a �t� supprim�e avec succ�s.");
										}
									}


								}




								if (!String.IsNullOrEmpty(ParrotDataOriginal) && !String.IsNullOrEmpty(ParrotDataBackup))
								{
									if (File.Exists(ParrotDataBackup))
									{
										Utils.LogMessage($"Restore ParrotDataOriginal");
										try
										{
											File.Copy(ParrotDataBackup, ParrotDataOriginal, true);
											Thread.Sleep(50);
											File.Delete(ParrotDataBackup);
										}
										catch { }
									}
								}

								if (!String.IsNullOrEmpty(FFBPluginIniFile) && !String.IsNullOrEmpty(FFBPluginIniBackup))
								{
									if (File.Exists(FFBPluginIniBackup))
									{
										Utils.LogMessage($"Restore FFBPluginIniFile");
										try
										{
											File.Copy(FFBPluginIniBackup, FFBPluginIniFile, true);
											Thread.Sleep(50);
											File.Delete(FFBPluginIniBackup);
										}
										catch { }
									}
								}

								if (virtualKeyboardXinputSlot > -1)
								{
									Utils.LogMessage($"Dispose virtualKeyboard");
									try { controller.Disconnect(); } catch { }
									try { client.Dispose(); } catch { }
								}


								if (startupForm != null && !startupForm.IsDisposed)
								{
									Utils.LogMessage($"Close startupForm");
									// Utilisez Invoke pour fermer le formulaire depuis un thread diff�rent
									startupForm.Invoke(new Action(() => startupForm.Close()));
								}

								if (_restoreSwitch)
								{
									Utils.LogMessage($"Restore Disposition");
									bool resultSwitch = MonitorSwitcher.LoadDisplaySettings(Path.Combine(Program.DispositionFolder, "dispositionrestore_app.xml"));
									if (!resultSwitch)
									{
										Thread.Sleep(3000);
										MonitorSwitcher.LoadDisplaySettings(Path.Combine(Program.DispositionFolder, "dispositionrestore_app.xml"));
									}
								}

								if (magpie_process_pid > 0)
								{
									Thread.Sleep(1000);
									Utils.KillProcessById(magpie_process_pid);
								}

								if (DebugMode)
								{
									Console.WriteLine("Press Enter Key to quit");
									Console.ReadLine();
								}
								if (shifterHack != null) shifterHack.Stop();

								if (createdNew)
								{
									try
									{
										mutex.ReleaseMutex();
										createdNew = false;
									}
									catch { }
								}
								//removed self kill
								//Utils.KillProcessById(Process.GetCurrentProcess().Id);
							}

						}
						finally
						{
							if (createdNew)
							{
								try
								{
									mutex.ReleaseMutex();
								}
								catch { }
							}
						}
					}
				}
			}


			/*
			if(args.Length == 0)
			{
				ApplicationConfiguration.Initialize();
				Application.Run(new Form1());
			}
			if(args.Length > 0)
			{

			}

			*/
		}


		private static XmlNode NodeFromKey(Key value, XmlDocument xmlDoc)
		{
			var keyData = new JoystickButtonData();
			keyData.Button = (int)value + 47;
			keyData.IsAxis = false;
			keyData.IsAxisMinus = false;
			keyData.IsFullAxis = false;
			keyData.PovDirection = 0;
			keyData.IsReverseAxis = false;
			keyData.XinputTitle = "Test";
			keyData.Title = "Keyboard Button " + value.ToString();
			keyData.JoystickGuid = new Guid("6f1d2b61-d5a0-11cf-bfc7-444553540000");

			XmlNode newDirectInputButtonNode = xmlDoc.CreateElement("DirectInputButton");

			XmlNode buttonNode = xmlDoc.CreateElement("Button");
			buttonNode.InnerText = keyData.Button.ToString();
			newDirectInputButtonNode.AppendChild(buttonNode);

			XmlNode isAxisNode = xmlDoc.CreateElement("IsAxis");
			isAxisNode.InnerText = keyData.IsAxis ? "true" : "false";
			newDirectInputButtonNode.AppendChild(isAxisNode);

			XmlNode IsAxisMinusNode = xmlDoc.CreateElement("IsAxisMinus");
			IsAxisMinusNode.InnerText = keyData.IsAxisMinus ? "true" : "false";
			newDirectInputButtonNode.AppendChild(IsAxisMinusNode);

			XmlNode IsFullAxisNode = xmlDoc.CreateElement("IsFullAxis");
			IsFullAxisNode.InnerText = keyData.IsFullAxis ? "true" : "false";
			newDirectInputButtonNode.AppendChild(IsFullAxisNode);

			XmlNode PovDirectionNode = xmlDoc.CreateElement("PovDirection");
			PovDirectionNode.InnerText = keyData.PovDirection.ToString();
			newDirectInputButtonNode.AppendChild(PovDirectionNode);

			XmlNode IsReverseAxisNode = xmlDoc.CreateElement("IsReverseAxis");
			IsReverseAxisNode.InnerText = keyData.IsReverseAxis ? "true" : "false";
			newDirectInputButtonNode.AppendChild(IsReverseAxisNode);

			XmlNode JoystickGuidNode = xmlDoc.CreateElement("JoystickGuid");
			JoystickGuidNode.InnerText = keyData.JoystickGuid.ToString();
			newDirectInputButtonNode.AppendChild(JoystickGuidNode);

			XmlNode JoystickDiNameNode = xmlDoc.CreateElement("DiName");
			JoystickDiNameNode.InnerText = keyData.Title;
			newDirectInputButtonNode.AppendChild(JoystickDiNameNode);

			return newDirectInputButtonNode;
		}

		private static async Task ShowFormAsync(CancellationToken cancellationToken)
		{
			startupForm = new Startup();
			Application.Run(startupForm);

			// Attendez la demande d'annulation avant de fermer d�finitivement le formulaire
			await Task.Delay(Timeout.Infinite, cancellationToken);
		}

		public static List<int> GetPlayersList(Dictionary<string, JoystickButton> joystickButtonDictionary)
		{
			List<int> playerList = new List<int>();
			foreach (var joystickButton in joystickButtonDictionary)
			{
				if (joystickButton.Value.XinputSlot >= 0 && joystickButton.Value.XinputSlot <= 3)
				{
					if (!playerList.Contains(joystickButton.Value.XinputSlot)) playerList.Add(joystickButton.Value.XinputSlot);
				}
			}
			playerList.Sort();
			return playerList;
		}


		public static Dictionary<string, JoystickButton> ParseConfig(string configFilePath, bool skipMissingXinput = true, bool loadDirectXml = false)
		{
			//string configFilePath = @"E:\TODO\teknoparrot\TeknoparrotXinputSetup\config\Batman.gamepad.txt";

			XmlDocument xmlDoc = new XmlDocument();
			if (loadDirectXml) xmlDoc.LoadXml(configFilePath);
			else xmlDoc.Load(configFilePath);

			Dictionary<string, JoystickButton> joystickButtonDictionary = new Dictionary<string, JoystickButton>();
			XmlNodeList joystickButtonsNodes = xmlDoc.SelectNodes("//JoystickButtons/JoystickButtons");
			foreach (XmlNode joystickButtonNode in joystickButtonsNodes)
			{
				JoystickButton joystickButton = new JoystickButton();
				joystickButton.ButtonName = GetValueOfChildNode(joystickButtonNode, "ButtonName");
				joystickButton.Xml = joystickButtonNode.OuterXml;
				joystickButton.BindNameXi = GetValueOfChildNode(joystickButtonNode, "BindNameXi");

				if (!string.IsNullOrEmpty(joystickButton.BindNameXi))
				{
					joystickButton.XinputSlot = DetermineXinputSlot(joystickButton.BindNameXi);
				}
				else
				{
					joystickButton.XinputSlot = -1;
				}

				if (skipMissingXinput)
				{
					if (!string.IsNullOrEmpty(joystickButton.BindNameXi))
					{
						joystickButtonDictionary[joystickButton.ButtonName] = joystickButton;
					}
				}
				else
				{
					joystickButtonDictionary[joystickButton.ButtonName] = joystickButton;
				}


			}
			return joystickButtonDictionary;

		}

		static string GetValueOfChildNode(XmlNode parentNode, string childNodeName)
		{
			XmlNode childNode = parentNode.SelectSingleNode(childNodeName);
			return childNode != null ? childNode.InnerText : null;
		}

		static int DetermineXinputSlot(string bindNameXi)
		{
			// Utilisation d'une expression r�guli�re pour extraire le num�ro du slot
			Match match = Regex.Match(bindNameXi, @"Input Device (\d+)");
			if (match.Success)
			{
				// Conversion du match en entier
				if (int.TryParse(match.Groups[1].Value, out int slotNumber))
				{
					return slotNumber;
				}
			}

			// Retourner -1 si aucune correspondance ou conversion �chou�e
			return -1;
		}

		private static void ControllerAction(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Button button)
		{
			Mutex.WaitOne();
			controller.SetButtonState(button, true);
			controller.SubmitReport();
			Thread.Sleep(100);
			controller.SetButtonState(button, false);
			controller.SubmitReport();
			Mutex.ReleaseMutex();

		}

		private static void OnApplicationExit(object sender, EventArgs e)
		{

			if (virtualKeyboardXinputSlot > -1)
			{
				try { controller.Disconnect(); } catch { }
				try { client.Dispose(); } catch { }
			}
		}

		public static void InitJoyList()
		{
			AddJoyId(0x0079, 0x18d4, "GPD Win 2 X-Box Controller");
			AddJoyId(0x044f, 0xb326, "Thrustmaster Gamepad GP XID");
			AddJoyId(0x045e, 0x028e, "Microsoft X-Box 360 pad");
			AddJoyId(0x045e, 0x028f, "Microsoft X-Box 360 pad v2");
			AddJoyId(0x045e, 0x0291, "Xbox 360 Wireless Receiver (XBOX)");
			AddJoyId(0x045e, 0x02a0, "Microsoft X-Box 360 Big Button IR");
			AddJoyId(0x045e, 0x02a1, "Microsoft X-Box 360 pad");
			AddJoyId(0x045e, 0x02d1, "Microsoft X-Box One pad");
			AddJoyId(0x045e, 0x02dd, "Microsoft X-Box One pad (Firmware 2015)");
			AddJoyId(0x045e, 0x02e0, "Microsoft X-Box One S pad (Bluetooth)");
			AddJoyId(0x045e, 0x02e3, "Microsoft X-Box One Elite pad");
			AddJoyId(0x045e, 0x02ea, "Microsoft X-Box One S pad");
			AddJoyId(0x045e, 0x02fd, "Microsoft X-Box One S pad (Bluetooth)");
			AddJoyId(0x045e, 0x02ff, "Microsoft X-Box One Elite pad");
			AddJoyId(0x045e, 0x0719, "Xbox 360 Wireless Receiver");
			AddJoyId(0x046d, 0xc21d, "Logitech Gamepad F310");
			AddJoyId(0x046d, 0xc21e, "Logitech Gamepad F510");
			AddJoyId(0x046d, 0xc21f, "Logitech Gamepad F710");
			AddJoyId(0x046d, 0xc242, "Logitech Chillstream Controller");
			AddJoyId(0x056e, 0x2004, "Elecom JC-U3613M");
			AddJoyId(0x06a3, 0xf51a, "Saitek P3600");
			AddJoyId(0x0738, 0x4716, "Mad Catz Wired Xbox 360 Controller");
			AddJoyId(0x0738, 0x4718, "Mad Catz Street Fighter IV FightStick SE");
			AddJoyId(0x0738, 0x4726, "Mad Catz Xbox 360 Controller");
			AddJoyId(0x0738, 0x4728, "Mad Catz Street Fighter IV FightPad");
			AddJoyId(0x0738, 0x4736, "Mad Catz MicroCon Gamepad");
			AddJoyId(0x0738, 0x4738, "Mad Catz Wired Xbox 360 Controller (SFIV)");
			AddJoyId(0x0738, 0x4740, "Mad Catz Beat Pad");
			AddJoyId(0x0738, 0x4a01, "Mad Catz FightStick TE 2");
			AddJoyId(0x0738, 0xb726, "Mad Catz Xbox controller - MW2");
			AddJoyId(0x0738, 0xbeef, "Mad Catz JOYTECH NEO SE Advanced GamePad");
			AddJoyId(0x0738, 0xcb02, "Saitek Cyborg Rumble Pad - PC/Xbox 360");
			AddJoyId(0x0738, 0xcb03, "Saitek P3200 Rumble Pad - PC/Xbox 360");
			AddJoyId(0x0738, 0xf738, "Super SFIV FightStick TE S");
			AddJoyId(0x0955, 0xb400, "NVIDIA Shield streaming controller");
			AddJoyId(0x0e6f, 0x0105, "HSM3 Xbox360 dancepad");
			AddJoyId(0x0e6f, 0x0113, "Afterglow AX.1 Gamepad for Xbox 360");
			AddJoyId(0x0e6f, 0x011f, "Rock Candy Gamepad Wired Controller");
			AddJoyId(0x0e6f, 0x0131, "PDP EA Sports Controller");
			AddJoyId(0x0e6f, 0x0133, "Xbox 360 Wired Controller");
			AddJoyId(0x0e6f, 0x0139, "Afterglow Prismatic Wired Controller");
			AddJoyId(0x0e6f, 0x013a, "PDP Xbox One Controller");
			AddJoyId(0x0e6f, 0x0146, "Rock Candy Wired Controller for Xbox One");
			AddJoyId(0x0e6f, 0x0147, "PDP Marvel Xbox One Controller");
			AddJoyId(0x0e6f, 0x015c, "PDP Xbox One Arcade Stick");
			AddJoyId(0x0e6f, 0x0161, "PDP Xbox One Controller");
			AddJoyId(0x0e6f, 0x0162, "PDP Xbox One Controller");
			AddJoyId(0x0e6f, 0x0163, "PDP Xbox One Controller");
			AddJoyId(0x0e6f, 0x0164, "PDP Battlefield One");
			AddJoyId(0x0e6f, 0x0165, "PDP Titanfall 2");
			AddJoyId(0x0e6f, 0x0201, "Pelican PL-3601 TSZ Wired Xbox 360 Controller");
			AddJoyId(0x0e6f, 0x0213, "Afterglow Gamepad for Xbox 360");
			AddJoyId(0x0e6f, 0x021f, "Rock Candy Gamepad for Xbox 360");
			AddJoyId(0x0e6f, 0x0246, "Rock Candy Gamepad for Xbox One 2015");
			AddJoyId(0x0e6f, 0x02a0, "Counterfeit 360Controller");
			AddJoyId(0x0e6f, 0x0301, "Logic3 Controller");
			AddJoyId(0x0e6f, 0x0346, "Rock Candy Gamepad for Xbox One 2016");
			AddJoyId(0x0e6f, 0x0401, "Logic3 Controller");
			AddJoyId(0x0e6f, 0x0413, "Afterglow AX.1 Gamepad for Xbox 360");
			AddJoyId(0x0e6f, 0x0501, "PDP Xbox 360 Controller");
			AddJoyId(0x0e6f, 0xf501, "Counterfeit 360 Controller");
			AddJoyId(0x0e6f, 0xf900, "PDP Afterglow AX.1");
			AddJoyId(0x0f0d, 0x000a, "Hori Co. DOA4 FightStick");
			AddJoyId(0x0f0d, 0x000c, "Hori PadEX Turbo");
			AddJoyId(0x0f0d, 0x000d, "Hori Fighting Stick EX2");
			AddJoyId(0x0f0d, 0x0016, "Hori Real Arcade Pro.EX");
			AddJoyId(0x0f0d, 0x001b, "Hori Real Arcade Pro VX");
			AddJoyId(0x0f0d, 0x0063, "Hori Real Arcade Pro Hayabusa (USA) Xbox One");
			AddJoyId(0x0f0d, 0x0067, "HORIPAD ONE");
			AddJoyId(0x0f0d, 0x0078, "Hori Real Arcade Pro V Kai Xbox One");
			AddJoyId(0x0f0d, 0x008c, "Hori Real Arcade Pro 4");
			AddJoyId(0x11c9, 0x55f0, "Nacon GC-100XF");
			AddJoyId(0x12ab, 0x0004, "Honey Bee Xbox360 dancepad");
			AddJoyId(0x12ab, 0x0301, "PDP AFTERGLOW AX.1");
			AddJoyId(0x12ab, 0x0303, "Mortal Kombat Klassic FightStick");
			AddJoyId(0x1430, 0x02a0, "RedOctane Controller Adapter");
			AddJoyId(0x1430, 0x4748, "RedOctane Guitar Hero X-plorer");
			AddJoyId(0x1430, 0xf801, "RedOctane Controller");
			AddJoyId(0x146b, 0x0601, "BigBen Interactive XBOX 360 Controller");
			AddJoyId(0x1532, 0x0037, "Razer Sabertooth");
			AddJoyId(0x1532, 0x0a00, "Razer Atrox Arcade Stick");
			AddJoyId(0x1532, 0x0a03, "Razer Wildcat");
			AddJoyId(0x15e4, 0x3f00, "Power A Mini Pro Elite");
			AddJoyId(0x15e4, 0x3f0a, "Xbox Airflo wired controller");
			AddJoyId(0x15e4, 0x3f10, "Batarang Xbox 360 controller");
			AddJoyId(0x162e, 0xbeef, "Joytech Neo-Se Take2");
			AddJoyId(0x1689, 0xfd00, "Razer Onza Tournament Edition");
			AddJoyId(0x1689, 0xfd01, "Razer Onza Classic Edition");
			AddJoyId(0x1689, 0xfe00, "Razer Sabertooth");
			AddJoyId(0x1bad, 0x0002, "Harmonix Rock Band Guitar");
			AddJoyId(0x1bad, 0x0003, "Harmonix Rock Band Drumkit");
			AddJoyId(0x1bad, 0xf016, "Mad Catz Xbox 360 Controller");
			AddJoyId(0x1bad, 0xf018, "Mad Catz Street Fighter IV SE Fighting Stick");
			AddJoyId(0x1bad, 0xf019, "Mad Catz Brawlstick for Xbox 360");
			AddJoyId(0x1bad, 0xf021, "Mad Cats Ghost Recon FS GamePad");
			AddJoyId(0x1bad, 0xf023, "MLG Pro Circuit Controller (Xbox)");
			AddJoyId(0x1bad, 0xf025, "Mad Catz Call Of Duty");
			AddJoyId(0x1bad, 0xf027, "Mad Catz FPS Pro");
			AddJoyId(0x1bad, 0xf028, "Street Fighter IV FightPad");
			AddJoyId(0x1bad, 0xf02e, "Mad Catz Fightpad");
			AddJoyId(0x1bad, 0xf036, "Mad Catz MicroCon GamePad Pro");
			AddJoyId(0x1bad, 0xf038, "Street Fighter IV FightStick TE");
			AddJoyId(0x1bad, 0xf039, "Mad Catz MvC2 TE");
			AddJoyId(0x1bad, 0xf03a, "Mad Catz SFxT Fightstick Pro");
			AddJoyId(0x1bad, 0xf03d, "Street Fighter IV Arcade Stick TE - Chun Li");
			AddJoyId(0x1bad, 0xf03e, "Mad Catz MLG FightStick TE");
			AddJoyId(0x1bad, 0xf03f, "Mad Catz FightStick SoulCaliber");
			AddJoyId(0x1bad, 0xf042, "Mad Catz FightStick TES+");
			AddJoyId(0x1bad, 0xf080, "Mad Catz FightStick TE2");
			AddJoyId(0x1bad, 0xf501, "HoriPad EX2 Turbo");
			AddJoyId(0x1bad, 0xf502, "Hori Real Arcade Pro.VX SA");
			AddJoyId(0x1bad, 0xf503, "Hori Fighting Stick VX");
			AddJoyId(0x1bad, 0xf504, "Hori Real Arcade Pro. EX");
			AddJoyId(0x1bad, 0xf505, "Hori Fighting Stick EX2B");
			AddJoyId(0x1bad, 0xf506, "Hori Real Arcade Pro.EX Premium VLX");
			AddJoyId(0x1bad, 0xf900, "Harmonix Xbox 360 Controller");
			AddJoyId(0x1bad, 0xf901, "Gamestop Xbox 360 Controller");
			AddJoyId(0x1bad, 0xf902, "Mad Catz Gamepad2");
			AddJoyId(0x1bad, 0xf903, "Tron Xbox 360 controller");
			AddJoyId(0x1bad, 0xf904, "PDP Versus Fighting Pad");
			AddJoyId(0x1bad, 0xf906, "MortalKombat FightStick");
			AddJoyId(0x1bad, 0xfa01, "MadCatz GamePad");
			AddJoyId(0x1bad, 0xfd00, "Razer Onza TE");
			AddJoyId(0x1bad, 0xfd01, "Razer Onza");
			AddJoyId(0x24c6, 0x5000, "Razer Atrox Arcade Stick");
			AddJoyId(0x24c6, 0x5300, "PowerA MINI PROEX Controller");
			AddJoyId(0x24c6, 0x5303, "Xbox Airflo wired controller");
			AddJoyId(0x24c6, 0x530a, "Xbox 360 Pro EX Controller");
			AddJoyId(0x24c6, 0x531a, "PowerA Pro Ex");
			AddJoyId(0x24c6, 0x5397, "FUS1ON Tournament Controller");
			AddJoyId(0x24c6, 0x541a, "PowerA Xbox One Mini Wired Controller");
			AddJoyId(0x24c6, 0x542a, "Xbox ONE spectra");
			AddJoyId(0x24c6, 0x543a, "PowerA Xbox One wired controller");
			AddJoyId(0x24c6, 0x5500, "Hori XBOX 360 EX 2 with Turbo");
			AddJoyId(0x24c6, 0x5501, "Hori Real Arcade Pro VX-SA");
			AddJoyId(0x24c6, 0x5502, "Hori Fighting Stick VX Alt");
			AddJoyId(0x24c6, 0x5503, "Hori Fighting Edge");
			AddJoyId(0x24c6, 0x5506, "Hori SOULCALIBUR V Stick");
			AddJoyId(0x24c6, 0x5510, "Hori Fighting Commander ONE");
			AddJoyId(0x24c6, 0x550d, "Hori GEM Xbox controller");
			AddJoyId(0x24c6, 0x550e, "Hori Real Arcade Pro V Kai 360");
			AddJoyId(0x24c6, 0x551a, "PowerA FUSION Pro Controller");
			AddJoyId(0x24c6, 0x561a, "PowerA FUSION Controller");
			AddJoyId(0x24c6, 0x5b00, "ThrustMaster Ferrari Italia 458 Racing Wheel");
			AddJoyId(0x24c6, 0x5b02, "Thrustmaster, Inc. GPX Controller");
			AddJoyId(0x24c6, 0x5b03, "Thrustmaster Ferrari 458 Racing Wheel");
			AddJoyId(0x24c6, 0x5d04, "Razer Sabertooth");
			AddJoyId(0x24c6, 0xfafa, "Aplay Controller");
			AddJoyId(0x24c6, 0xfafb, "Aplay Controller");
			AddJoyId(0x24c6, 0xfafc, "Afterglow Gamepad 1");
			AddJoyId(0x24c6, 0xfafe, "Rock Candy Gamepad for Xbox 360");
			AddJoyId(0x24c6, 0xfafd, "Afterglow Gamepad 3");
			AddJoyId(0x0955, 0x7210, "Nvidia Shield local controller");
			AddJoyId(0x0e6f, 0x0205, "Victrix Pro FS Xbox One Edition");
			AddJoyId(0x0c12, 0x0ef8, "Homemade fightstick");
			AddJoyId(0x046d, 0x0401, "logitech xinput");
			AddJoyId(0x046d, 0x0301, "logitech xinput");
			AddJoyId(0x046d, 0xcaa3, "logitech xinput");
			AddJoyId(0x046d, 0xc261, "logitech xinput");
			AddJoyId(0x046d, 0x0291, "logitech xinput");
			AddJoyId(0x1038, 0xb360, "SteelSeries Nimbus/Stratus XL");



		}

		// M�thode pour ajouter une valeur au dictionnaire � deux dimensions
		static void AddJoyId(int cle1, int cle2, string valeur)
		{
			if (!joysticksIds.ContainsKey(cle1))
			{
				joysticksIds[cle1] = new Dictionary<int, string>();
			}

			joysticksIds[cle1][cle2] = valeur;
		}

		// M�thode pour obtenir une valeur du dictionnaire � deux dimensions
		public static string GetJoyId(int cle1, int cle2)
		{
			if (joysticksIds.ContainsKey(cle1) && joysticksIds[cle1].ContainsKey(cle2))
			{
				return joysticksIds[cle1][cle2];
			}

			return "Unknown";
		}

		public static bool UseMonitorDisposition(string key)
		{
			var cfg = Path.Combine(Program.DispositionFolder, "disposition_" + key + ".xml");
			if (File.Exists(cfg))
			{
				bool result1 = MonitorSwitcher.LoadDisplaySettings(cfg);
				bool result2 = true;
				if (!result1)
				{
					Thread.Sleep(3000);
					result2 = MonitorSwitcher.LoadDisplaySettings(cfg);
				}
				return result2;
			}
			return false;
		}

		static void RedirectConsoleOutput()
		{
			// Rediriger la sortie standard vers la nouvelle console
			IntPtr stdHandle = GetStdHandle(-11); // -11 correspond � STD_OUTPUT_HANDLE
			var fileStream = new FileStream(new SafeFileHandle(stdHandle, true), FileAccess.Write);
			var streamWriter = new StreamWriter(fileStream, Console.OutputEncoding) { AutoFlush = true };
			Console.SetOut(streamWriter);
			Console.SetError(streamWriter);
		}
	}


	public class JoystickButton
	{
		public string ButtonName { get; set; }
		public string Xml { get; set; }
		public string BindNameXi { get; set; }

		public int XinputSlot { get; set; }

		public JoystickButton RemapButtonData(int newXinputSlot, bool changeDpadToJoy = false)
		{
			var button = new JoystickButton();
			button.ButtonName = this.ButtonName;
			button.Xml = this.Xml;
			button.BindNameXi = this.BindNameXi;
			button.XinputSlot = this.XinputSlot;

			if (changeDpadToJoy)
			{
				string newBindNameXi = "";
				if (this.BindNameXi.EndsWith("DPadUp"))
				{
					newBindNameXi = newBindNameXi.Replace("DPadUp", $"LeftThumbInput Device {newXinputSlot} Y+");
					XDocument doc = XDocument.Parse(this.Xml);
					XElement xInputButton = doc.Root.Element("XInputButton");
					xInputButton.Element("IsLeftThumbY").Value = "true";
					xInputButton.Element("ButtonCode").Value = "0";
					xInputButton.Element("IsButton").Value = "false";
					doc.Root.Element("BindNameXi").Value = newBindNameXi;
					button.Xml = doc.ToString();
					button.BindNameXi = newBindNameXi;
				}
				if (this.BindNameXi.EndsWith("DPadDown"))
				{
					newBindNameXi = newBindNameXi.Replace("DPadDown", $"LeftThumbInput Device {newXinputSlot} Y-");
					XDocument doc = XDocument.Parse(this.Xml);
					XElement xInputButton = doc.Root.Element("XInputButton");
					xInputButton.Element("IsLeftThumbY").Value = "true";
					xInputButton.Element("IsAxisMinus").Value = "true";
					xInputButton.Element("ButtonCode").Value = "0";
					xInputButton.Element("IsButton").Value = "false";
					doc.Root.Element("BindNameXi").Value = newBindNameXi;
					button.Xml = doc.ToString();
					button.BindNameXi = newBindNameXi;
				}
				if (this.BindNameXi.EndsWith("DPadRight"))
				{
					newBindNameXi = newBindNameXi.Replace("DPadRight", $"LeftThumbInput Device {newXinputSlot} X+");
					XDocument doc = XDocument.Parse(this.Xml);
					XElement xInputButton = doc.Root.Element("XInputButton");
					xInputButton.Element("IsLeftThumbX").Value = "true";
					xInputButton.Element("ButtonCode").Value = "0";
					xInputButton.Element("IsButton").Value = "false";
					doc.Root.Element("BindNameXi").Value = newBindNameXi;
					button.Xml = doc.ToString();
					button.BindNameXi = newBindNameXi;
				}
				if (this.BindNameXi.EndsWith("DPadLeft"))
				{
					newBindNameXi = newBindNameXi.Replace("DPadLeft", $"LeftThumbInput Device {newXinputSlot} X-");
					XDocument doc = XDocument.Parse(this.Xml);
					XElement xInputButton = doc.Root.Element("XInputButton");
					xInputButton.Element("IsLeftThumbX").Value = "true";
					xInputButton.Element("IsAxisMinus").Value = "true";
					xInputButton.Element("ButtonCode").Value = "0";
					xInputButton.Element("IsButton").Value = "false";
					doc.Root.Element("BindNameXi").Value = newBindNameXi;
					button.Xml = doc.ToString();
					button.BindNameXi = newBindNameXi;
				}
			}

			button.Xml = button.Xml.Replace($"Input Device {XinputSlot}", $"Input Device {newXinputSlot}");
			button.Xml = button.Xml.Replace($"<XInputIndex>{XinputSlot}</XInputIndex>", $"<XInputIndex>{newXinputSlot}</XInputIndex>");
			button.BindNameXi = button.BindNameXi.Replace($"Input Device {XinputSlot}", $"Input Device {newXinputSlot}");


			button.XinputSlot = newXinputSlot;
			return button;
		}

	}

	public class XinputGamepad
	{
		public X.Gamepad Gamepad { get; set; }
		public int XinputSlot { get; set; }
		public string Type { get; set; }

		public string SubType { get; set; }

		public string Signature { get; set; }

		public ushort VendorId { get; set; } = 0;
		public ushort ProductId { get; set; } = 0;
		public ushort RevisionID { get; set; } = 0;

		public string ControllerName { get; set; } = "";

		public XinputGamepad(int xinputSlot)
		{
			XinputSlot = xinputSlot;
			Gamepad = null;
		}

		public XinputGamepad(X.Gamepad gamepad, int xinputSlot, bool useForceType = true, string MatchListWheel = null, string MatchListArcade = null, string MatchListGamepad = null)
		{
			Gamepad = gamepad;
			XinputSlot = xinputSlot;
			var caps = Gamepad.Capabilities;
			string json = JsonConvert.SerializeObject(caps, Newtonsoft.Json.Formatting.None);
			Signature = GetMD5Short(json);
			SubType = caps.SubType.ToString().Trim();


			var capsEx = XExt.GetExtraData(XinputSlot);
			VendorId = capsEx.vendorId;
			ProductId = capsEx.productId;
			RevisionID = capsEx.revisionId;

			ControllerName = Program.GetJoyId(VendorId, ProductId);

			Type = "";
			string dataTxt = ToString();

			if (useForceType)
			{
				if (Program.forceTypeController.ContainsKey(xinputSlot))
				{
					Type = Program.forceTypeController[xinputSlot];
				}
			}


			if (Type == "")
			{
				string MatchListWheelTxt = MatchListWheel == null ? Program.wheelXinputData : MatchListWheel;
				var MatchList = MatchListWheelTxt.ToLower().Trim().Split(',');
				foreach (var m in MatchList)
				{
					if (m.Trim() != "" && dataTxt.ToLower().Trim().Contains(m.Trim()))
					{
						Type = "wheel";
						break;
					}
				}
			}
			if (Type == "")
			{
				string MatchListArcadeTxt = MatchListArcade == null ? Program.arcadeXinputData : MatchListArcade;
				var MatchList = MatchListArcadeTxt.ToLower().Trim().Split(',');
				foreach (var m in MatchList)
				{
					if (m.Trim() != "" && dataTxt.ToLower().Trim().Contains(m.Trim()))
					{
						Type = "arcade";
						break;
					}
				}
			}
			if (Type == "")
			{
				string MatchListGamepadTxt = MatchListGamepad == null ? Program.gamepadXinputData : MatchListGamepad;
				var MatchList = MatchListGamepadTxt.ToLower().Trim().Split(',');
				foreach (var m in MatchList)
				{
					if (m.Trim() != "" && dataTxt.ToLower().Trim().Contains(m.Trim()))
					{
						Type = "gamepad";
						break;
					}
				}
			}

		}

		public static string GetMD5Short(string input)
		{

			using (MD5 md5 = MD5.Create())
			{
				byte[] inputBytes = Encoding.UTF8.GetBytes(input);
				byte[] hashBytes = md5.ComputeHash(inputBytes);

				// Convertir les octets du hash en une cha�ne hexad�cimale
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < hashBytes.Length; i++)
				{
					sb.Append(hashBytes[i].ToString("X2")); // X2 pour obtenir en majuscules
				}

				return sb.ToString().Substring(0, 6);
			}
		}



		public override string ToString()
		{
			return $"XINPUT{XinputSlot}<>Type={SubType}<>Signature={Signature}<>VendorID=0x{VendorId:X04}<>ProductID=0x{ProductId:X04}<>RevisionID=0x{RevisionID:X04}<>{ControllerName}";
		}




	}
}