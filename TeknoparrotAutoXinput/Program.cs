using Gma.System.MouseKeyHook;
using Microsoft.Win32.SafeHandles;
using MonitorSwitcherGUI;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Exceptions;
using Nefarius.ViGEm.Client.Targets;
using Newtonsoft.Json;
using SDL2;
using SharpDX.DirectInput;
using SharpDX.Multimedia;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using XInput.Wrapper;
using static SDL2.SDL;
using static XInput.Wrapper.X;
using static XInput.Wrapper.X.Gamepad;

namespace TeknoparrotAutoXinput
{
	internal static class Program
	{
		// Importer la fonction AllocConsole depuis Kernel32.dll
		[DllImport("kernel32.dll")]
		public static extern bool AllocConsole();

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetStdHandle(int nStdHandle);

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
		public static Guid? FirstKeyboardGuid = null;

		public static string ParrotDataOriginal="";
		public static string ParrotDataBackup="";
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

		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{

			//RegisterTask
			if (args.Length > 2 && args.First() == "--registerTask")
			{
				var filteredArgs = Utils.ArgsWithoutFirstElement(args);
				var taskExe = filteredArgs[0];
				var taskArguments = Utils.ArgsToCommandLine(Utils.ArgsWithoutFirstElement(filteredArgs));
				Utils.RegisterTask(taskExe, taskArguments);
				return;
			}

			joysticksIds.Clear();
			InitJoyList();

			string runtimedir = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "runtimes");
			if (Directory.Exists(runtimedir))
			{
				Directory.Delete(runtimedir, true);
			}


			Application.ApplicationExit += new EventHandler(OnApplicationExit);
#if DEBUG
			//List<string> fakeArgs = new List<string>();
			//fakeArgs.Add(@"C:\teknoparrot\UserProfiles\Daytona3.xml");
			//args = fakeArgs.ToArray();
#endif

			ConfigurationManager.LoadConfig();

			wheelXinputData = ConfigurationManager.MainConfig.wheelXinputData;
			arcadeXinputData = ConfigurationManager.MainConfig.arcadeXinputData;
			gamepadXinputData = ConfigurationManager.MainConfig.gamepadXinputData;

			if (args.Length == 0)
			{
				ApplicationConfiguration.Initialize();
				Application.Run(new Main());
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
				bool enableStoozZone_Gamepad = ConfigurationManager.MainConfig.enableStoozZone_Gamepad;
				int valueStooz_Gamepad = ConfigurationManager.MainConfig.valueStooz_Gamepad;
				bool enableStoozZone_Wheel = ConfigurationManager.MainConfig.enableStoozZone_Wheel;
				int valueStooz_Wheel = ConfigurationManager.MainConfig.valueStooz_Wheel;

				WheelFFBGuid = ConfigurationManager.MainConfig.ffbDinputWheel;

				bool passthrough = false;


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
				Utils.LogMessage($"valueStooz_Gamepad = {valueStooz_Gamepad}");
				Utils.LogMessage($"enableStoozZone_Wheel = {enableStoozZone_Wheel}");
				Utils.LogMessage($"valueStooz_Wheel = {valueStooz_Wheel}");
				Utils.LogMessage($"WheelFFBGuid = {WheelFFBGuid}");
				

				List<string> typeConfig = new List<string>();
				typeConfig.Add("gamepad");
				typeConfig.Add("gamepadalt");
				typeConfig.Add("arcade");
				typeConfig.Add("wheel");

				Dictionary<string, string> existingConfig = new Dictionary<string, string>();
				if (args.Length > 0)
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

						if(arg.ToLower().Trim() == "--passthrough")
						{
							passthrough = true;
							Utils.LogMessage($"CMDLINE Option : passthrough = {passthrough}");
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
					}


					string finalConfig = "";
					string basePath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
					string customConfigPath = "";
					Dictionary<string, string> shifterData = new Dictionary<string, string>();

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

					string perGameLinkFolder = ConfigurationManager.MainConfig.perGameLinkFolder;
					if (perGameLinkFolder == @"Default (<YourTeknoparrotFolder>\AutoXinputLinks)")
					{
						perGameLinkFolder = Path.Combine(baseTpDir, "AutoXinputLinks");
					}
					string linkSourceFolder = Path.Combine(perGameLinkFolder, originalConfigFileNameWithoutExt);

					string linkTargetFolder = "";
					string executableGame = "";
					bool gameNeedAdmin = false;

					Utils.LogMessage($"baseTpDir : {baseTpDir}");
					Utils.LogMessage($"originalConfigFileName : {originalConfigFileName}");
					Utils.LogMessage($"originalConfigFileNameWithoutExt : {originalConfigFileNameWithoutExt}");
					Utils.LogMessage($"teknoparrotExe : {teknoparrotExe}");
					Utils.LogMessage($"linkSourceFolder : {linkSourceFolder}");

					if (!File.Exists(teknoparrotExe))
					{
						Utils.LogMessage($"Can't find {teknoparrotExe}");
						MessageBox.Show($"Can't find {teknoparrotExe}");
						return;
					}

					Utils.LogMessage($"check if eligigible to HardLink");
					if (Utils.IsEligibleHardLink(baseTpDir))
					{
						Utils.LogMessage($"Starting Clean HardlinkFiles");
						Utils.CleanHardLinksFiles(Path.Combine(baseTpDir, "TeknoParrot"), perGameLinkFolder);
						Utils.CleanHardLinksFiles(Path.Combine(baseTpDir, "ElfLdr2"), perGameLinkFolder);
						Utils.LogMessage($"End Clean HardlinkFiles");
					}
					else
					{
						Utils.LogMessage($"Not eligible for Hardlink");
					}


					Utils.LogMessage($"Load XML to get emulatorType and requiresAdmin");
					try
					{
						XmlDocument xmlDoc = new XmlDocument();
						xmlDoc.Load(xmlFile);
						XmlNode emulatorTypeNode = xmlDoc.SelectSingleNode("/GameProfile/EmulatorType");
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
							}
							Utils.LogMessage($"emulatorTypeValue = {emulatorTypeValue}");
							Utils.LogMessage($"linkTargetFolder = {linkTargetFolder}");
						}

						XmlNode requiresAdminNode = xmlDoc.SelectSingleNode("/GameProfile/RequiresAdmin");
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

					Utils.LogMessage($"Checking gameOverride options");
					//GameOptionOverrite
					_dispositionToSwitch = ConfigurationManager.MainConfig.Disposition;
					GameSettings gameOptions = new GameSettings();
					string optionFile = Path.Combine(GameOptionsFolder, originalConfigFileNameWithoutExt + ".json");
					if (File.Exists(optionFile))
					{
						Utils.LogMessage($"gameoveride file found, loading it");
						gameOptions = new GameSettings(File.ReadAllText(optionFile)); 
					}
					if (gameOptions.UseGlobalDisposition == false) _dispositionToSwitch = gameOptions.Disposition;
					if (gameOptions.UseGlobalStoozZoneGamepad == false)
					{
						gamepadStooz = gameOptions.gamepadStooz;
						enableStoozZone_Gamepad = gameOptions.enableStoozZone_Gamepad;
						valueStooz_Gamepad = gameOptions.valueStooz_Gamepad;
					}
					if(gameOptions.UseGlobalStoozZoneWheel == false)
					{
						wheelStooz = gameOptions.wheelStooz;
						enableStoozZone_Wheel = gameOptions.enableStoozZone_Wheel;
						valueStooz_Wheel = gameOptions.valueStooz_Wheel;
					}
					Utils.LogMessage($"gameOptions Values = {gameOptions.Serialize()}");


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

					Utils.LogMessage($"Get GamePath");
					FFBPluginIniFile = "";
					FFBPluginIniBackup = "";
					XmlDocument xmlDocOri = new XmlDocument();
					xmlDocOri.Load(xmlFile);
					XmlNode gamePathNode = xmlDocOri.SelectSingleNode("/GameProfile/GamePath");
					if (gamePathNode != null)
					{
						string gamePathContent = gamePathNode.InnerText;
						if (gamePathContent.ToLower().EndsWith(".exe")) executableGame = gamePathContent;

						Utils.LogMessage($"gamePathContent = {gamePathContent}");
						FFBPluginIniFile = Path.Combine(Path.GetDirectoryName(gamePathContent), "FFBPlugin.ini");
						FFBPluginIniBackup = Path.Combine(Path.GetDirectoryName(gamePathContent), "FFBPlugin.ini.AutoXinputBackup");
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

					var shifterPath = Path.Combine(basePath, "config", originalConfigFileNameWithoutExt + "." + "shifter" + ".json");
					if (File.Exists(shifterPath))
					{
						shifterData = (Dictionary<string, string>)JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(shifterPath));
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

						Utils.LogMessage($"availableSlot = {availableSlot}");

						bool checkDinputWheel = ConfigurationManager.MainConfig.useDinputWheel;
						Dictionary<string, JoystickButtonData> bindingDinputWheel = null;
						string bindingDinputWheelJson = ConfigurationManager.MainConfig.bindingDinputWheel;

						bool checkDinputShifter = ConfigurationManager.MainConfig.useDinputShifter;
						Dictionary<string, JoystickButtonData> bindingDinputShifter = null;
						string bindingDinputShifterJson = ConfigurationManager.MainConfig.bindingDinputShifter;

						Guid shifterGuid = new Guid();
						bool shifterGuidFound = false;

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
									if(device.Type == SharpDX.DirectInput.DeviceType.Keyboard && FirstKeyboardGuid == null)
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

							if(dinputWheelFound && checkDinputShifter)
							{
								bindingDinputShifter = (Dictionary<string, JoystickButtonData>)JsonConvert.DeserializeObject<Dictionary<string, JoystickButtonData>>(bindingDinputShifterJson);
								if (bindingDinputShifter.ContainsKey("InputDeviceGear1"))
								{
									if(bindingDinputShifter["InputDeviceGear1"].JoystickGuid.ToString() != "")
									{
										shifterGuidFound = true;
										shifterGuid = bindingDinputShifter["InputDeviceGear1"].JoystickGuid;
										Utils.LogMessage($"shifterGuid = {shifterGuid}");
									}

								}
							}
						}
						bool useXinput = true;
						bool useDinputWheel = false;
						if (haveWheel && existingConfig.ContainsKey("wheel") && dinputWheelFound)
						{
							useXinput = false;
							useDinputWheel = true;
						}

						Utils.LogMessage($"useXinput : {useXinput}");
						Utils.LogMessage($"useDinputWheel : {useDinputWheel}");

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
							foreach (var  gp in connectedGamePad.Values)
							{
								Utils.LogMessage($"{gp.ToString()}");
							}
						}



						Dictionary<string, JoystickButton> finalJoystickButtonDictionary = ParseConfig(xmlFile, false);
						Dictionary<string, JoystickButton> emptyJoystickButtonDictionary = ParseConfig(emptyConfigPath, false);
						Dictionary<int, (string, XinputGamepad)> ConfigPerPlayer = new Dictionary<int, (string, XinputGamepad)>();
						Dictionary<string, JoystickButton> joystickButtonWheel = new Dictionary<string, JoystickButton>();
						Dictionary<string, JoystickButton> joystickButtonArcade = new Dictionary<string, JoystickButton>();
						Dictionary<string, JoystickButton> joystickButtonGamepad = new Dictionary<string, JoystickButton>();


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
									controller.AutoSubmitReport = false;
									controller.Connect();
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
										controller.Disconnect();
										client.Dispose();
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
							if (haveArcade && existingConfig.ContainsKey("arcade"))
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
											ConfigPerPlayer.Add(PlayerXinputSlot, ("gamepad", gamepadList[currentlyAttributed]));
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
										if (Enum.TryParse<SharpDX.DirectInput.Key> (keyTest, out SharpDX.DirectInput.Key resultat))
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
						}


						if(ConfigPerPlayer.Count > 0 && (gamepadStooz || wheelStooz))
						{
							bool doChangeStooz = false;
							bool enableStooz = false;
							int valueStooz = 0;
							var firstPlayer = ConfigPerPlayer.First();
							if(firstPlayer.Value.Item2.Type == "gamepad" && gamepadStooz)
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
							/*
							XmlDocument xmlDocOri = new XmlDocument();
							xmlDocOri.Load(xmlFile);
							XmlNode gamePathNode = xmlDocOri.SelectSingleNode("/GameProfile/GamePath");
							
							if (gamePathNode != null)
							{
								string gamePathContent = gamePathNode.InnerText;
								string FFBPluginIniFile = Path.Combine(Path.GetDirectoryName(gamePathContent), "FFBPlugin.ini");
								*/
								if (!String.IsNullOrEmpty(FFBPluginIniFile) && File.Exists(FFBPluginIniFile))
								{
									Utils.LogMessage($"FFB Ini Exist");
									if(!String.IsNullOrEmpty(FFBPluginIniBackup))
									{
										try
										{
											File.Copy(FFBPluginIniFile, FFBPluginIniBackup, true);
										}
										catch { }
									}
									var ConfigFFB = new IniFile(FFBPluginIniFile);
									//if (ConfigFFB.KeyExists("DeviceGUID", "Settings"))
									//{

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

													const int bufferSize = 256; // La taille doit être au moins 33 pour stocker le GUID sous forme de chaîne (32 caractères + le caractère nul)
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
									//}

								}

							//}

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
							if (ConfigType == "wheel") joystickButtonData = joystickButtonWheel;
							if (ConfigType == "arcade") joystickButtonData = joystickButtonArcade;
							if (ConfigType == "gamepad") joystickButtonData = joystickButtonGamepad;

							foreach (var buttonData in joystickButtonData)
							{
								if (buttonData.Value.XinputSlot == TargetXinput)
								{
									if (emptyJoystickButtonDictionary.ContainsKey(buttonData.Key))
									{
										emptyJoystickButtonDictionary[buttonData.Key] = buttonData.Value.RemapButtonData(newXinputSlot);

									}
								}
							}
						}
						if (useVirtualKeyboard && ConfigPerPlayer.Count() > 0 && virtualKeyboardXinputSlot >= 0)
						{
							string ConfigType = ConfigPerPlayer.First().Value.Item1;
							Dictionary<string, JoystickButton> joystickButtonData = new Dictionary<string, JoystickButton>();
							if (ConfigType == "wheel") joystickButtonData = joystickButtonWheel;
							if (ConfigType == "arcade") joystickButtonData = joystickButtonArcade;
							if (ConfigType == "gamepad") joystickButtonData = joystickButtonGamepad;

							foreach (var buttonData in joystickButtonData)
							{
								if (buttonData.Value.XinputSlot == 10)
								{
									emptyJoystickButtonDictionary[buttonData.Key] = buttonData.Value.RemapButtonData(virtualKeyboardXinputSlot);
								}
							}
						}

						string xmlFileContent = File.ReadAllText(xmlFile);
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

						// Créez les paramètres pour l'indentation
						XmlWriterSettings settings = new XmlWriterSettings
						{
							Indent = true,
							IndentChars = "    ", // Utilisez la chaîne que vous préférez pour l'indentation (par exemple, des espaces ou des tabulations)
							NewLineChars = "\n",
							NewLineHandling = NewLineHandling.Replace
						};

						string xpathExpression = $"/GameProfile/ConfigValues/FieldInformation[FieldName='Input API']/FieldValue";
						XmlNode fieldValueNode = xmlDoc.SelectSingleNode(xpathExpression);

						if (fieldValueNode != null)
						{
							if (useXinput)
							{
								fieldValueNode.InnerText = "XInput";
							}
							if (useDinputWheel)
							{
								fieldValueNode.InnerText = "DirectInput";
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
							XmlNodeList joystickButtonsNodes = xmlDoc.SelectNodes("/GameProfile/JoystickButtons/JoystickButtons");

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
								if(bindNameXiNode != null && !string.IsNullOrEmpty(bindNameXiNode.InnerText))
								{
									string bindkey = bindNameXiNode.InnerText.Trim().Replace(" ", "");
									if (bindingDinputWheel.ContainsKey(bindkey))
									{
										var bindData = bindingDinputWheel[bindkey];

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


										if(buttonName == shiftUpKey)
										{
											shiftUpKeyBind = bindData.Title;
										}
										if (buttonName == shiftDownKey)
										{
											shiftDownKeyBind = bindData.Title;
										}
									}
								}

								
								if (buttonNameNode != null && !string.IsNullOrEmpty(buttonNameNode.InnerText))
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
										if(bindkey == shiftUpKeyBind)
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


											XmlNode newDirectInputButtonNode = xmlDoc.CreateElement("DirectInputButton");
											XmlNode buttonNode = xmlDoc.CreateElement("Button");
											buttonNode.InnerText = "153";
											newDirectInputButtonNode.AppendChild(buttonNode);
											XmlNode isAxisNode = xmlDoc.CreateElement("IsAxis");
											isAxisNode.InnerText = "false";
											newDirectInputButtonNode.AppendChild(isAxisNode);
											XmlNode IsAxisMinusNode = xmlDoc.CreateElement("IsAxisMinus");
											IsAxisMinusNode.InnerText = "false";
											newDirectInputButtonNode.AppendChild(IsAxisMinusNode);
											XmlNode IsFullAxisNode = xmlDoc.CreateElement("IsFullAxis");
											IsFullAxisNode.InnerText = "false";
											newDirectInputButtonNode.AppendChild(IsFullAxisNode);
											XmlNode PovDirectionNode = xmlDoc.CreateElement("PovDirection");
											PovDirectionNode.InnerText = "0";
											newDirectInputButtonNode.AppendChild(PovDirectionNode);
											XmlNode IsReverseAxisNode = xmlDoc.CreateElement("IsReverseAxis");
											IsReverseAxisNode.InnerText = "false";
											newDirectInputButtonNode.AppendChild(IsReverseAxisNode);
											XmlNode JoystickGuidNode = xmlDoc.CreateElement("JoystickGuid");
											JoystickGuidNode.InnerText = "6f1d2b61-d5a0-11cf-bfc7-444553540000";
											newDirectInputButtonNode.AppendChild(JoystickGuidNode);
											node.AppendChild(newDirectInputButtonNode);
											XmlNode BindNameDiNode = xmlDoc.CreateElement("BindNameDi");
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


											XmlNode newDirectInputButtonNode = xmlDoc.CreateElement("DirectInputButton");
											XmlNode buttonNode = xmlDoc.CreateElement("Button");
											buttonNode.InnerText = "158";
											newDirectInputButtonNode.AppendChild(buttonNode);
											XmlNode isAxisNode = xmlDoc.CreateElement("IsAxis");
											isAxisNode.InnerText = "false";
											newDirectInputButtonNode.AppendChild(isAxisNode);
											XmlNode IsAxisMinusNode = xmlDoc.CreateElement("IsAxisMinus");
											IsAxisMinusNode.InnerText = "false";
											newDirectInputButtonNode.AppendChild(IsAxisMinusNode);
											XmlNode IsFullAxisNode = xmlDoc.CreateElement("IsFullAxis");
											IsFullAxisNode.InnerText = "false";
											newDirectInputButtonNode.AppendChild(IsFullAxisNode);
											XmlNode PovDirectionNode = xmlDoc.CreateElement("PovDirection");
											PovDirectionNode.InnerText = "0";
											newDirectInputButtonNode.AppendChild(PovDirectionNode);
											XmlNode IsReverseAxisNode = xmlDoc.CreateElement("IsReverseAxis");
											IsReverseAxisNode.InnerText = "false";
											newDirectInputButtonNode.AppendChild(IsReverseAxisNode);
											XmlNode JoystickGuidNode = xmlDoc.CreateElement("JoystickGuid");
											JoystickGuidNode.InnerText = "6f1d2b61-d5a0-11cf-bfc7-444553540000";
											newDirectInputButtonNode.AppendChild(JoystickGuidNode);
											node.AppendChild(newDirectInputButtonNode);
											XmlNode BindNameDiNode = xmlDoc.CreateElement("BindNameDi");
											BindNameDiNode.InnerText = "Keyboard Button 111";
											node.AppendChild(BindNameDiNode);

										}
									}
								}
								shifterHack = new ShifterHack();
								shifterHack.Start(originalConfigFileNameWithoutExt, shifterGuid.ToString(), WheelGuid, shiftUpKeyBind, shiftDownKeyBind, executableGame, bindingDinputShifter);

							}



						}

						using (XmlWriter xmlWriter = XmlWriter.Create(xmlFile + ".custom.xml", settings))
						{
							// Enregistrez le XmlDocument avec l'indentation dans le XmlWriter
							xmlDoc.Save(xmlWriter);
							finalConfig = xmlFile + ".custom.xml";
						}

						Startup.tpBasePath = Directory.GetParent(Path.GetDirectoryName(Path.GetFullPath(xmlFile))).FullName;
						Startup.gameTitle = originalConfigFileNameWithoutExt;
						Startup.logoPath = Path.Combine(Directory.GetParent(Path.GetDirectoryName(Path.GetFullPath(xmlFile))).FullName, "Icons", originalConfigFileNameWithoutExt + ".png");
						Startup.playerAttributionDesc = "";
						int playernum = 0;
						Startup.imagePaths = new List<string>();
						foreach (var ConfigPlayer in ConfigPerPlayer)
						{
							playernum++;
							
							string upperType = char.ToUpper(ConfigPlayer.Value.Item1[0]) + ConfigPlayer.Value.Item1.Substring(1);

							Startup.playerAttributionDesc += $"P{playernum}={upperType}\n";
							string configname = ConfigPlayer.Value.Item1;
							if (configname == "gamepad" && usealtgamepad) configname = "gamepadalt";

							var imgPath = Path.Combine(basePath, "img", originalConfigFileNameWithoutExt + "." + configname + ".jpg");
							Startup.imagePaths.Add(imgPath);
						}
						Startup.playerAttributionDesc.TrimEnd('\n');
					}

					if (finalConfig != "")
					{

						if(gameOptions.EnableLink && !String.IsNullOrEmpty(linkTargetFolder) && !String.IsNullOrEmpty(linkSourceFolder) && Directory.Exists(linkSourceFolder))
						{
							Utils.LogMessage($"HardLinkFiles {linkSourceFolder}, {linkTargetFolder}");
							if (Utils.IsEligibleHardLink(linkTargetFolder))
							{
								Utils.HardLinkFiles(linkSourceFolder, linkTargetFolder);
							}
							
						}

						if (!string.IsNullOrEmpty(_dispositionToSwitch) && _dispositionToSwitch != "<none>")
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
						}

						if(gameOptions.AhkBefore.Trim() != "")
						{
							Utils.LogMessage($"Execute AHK Before");
							Utils.ExecuteAHK(gameOptions.AhkBefore,gameOptions.WaitForExitAhkBefore);
						}

						if (showStartup)
						{
							Utils.LogMessage($"showStartup");
							cancellationTokenSource = new CancellationTokenSource();
							Task.Run(() => ShowFormAsync(cancellationTokenSource.Token));
						}

						string argumentTpExe = "--profile=\"" + finalConfig + "\"";
						if (gameOptions.RunAsRoot && gameNeedAdmin)
						{
							Utils.LogMessage($"Force RunAsRoot");
							if (!Utils.CheckTaskExist(teknoparrotExe, argumentTpExe))
							{
								
								string exePath = Process.GetCurrentProcess().MainModule.FileName;
								string exeDir = Path.GetDirectoryName(exePath);
								Process process = new Process();
								process.StartInfo.FileName = exePath;
								process.StartInfo.Arguments = "--registerTask " + $"\"{teknoparrotExe}\" " +  argumentTpExe;
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
							//Thread.Sleep(5000);	
							
							Utils.LogMessage($"Starting {teknoparrotExe} {argumentTpExe}");
							Process process = new Process();
							process.StartInfo.FileName = teknoparrotExe;
							process.StartInfo.Arguments = argumentTpExe;
							process.StartInfo.WorkingDirectory = Path.GetDirectoryName(teknoparrotExe);
							process.StartInfo.UseShellExecute = true;
							process.Start();
							process.WaitForExit();
							
						}

						Thread.Sleep(500);
						Utils.LogMessage($"End Execution");

						if (gameOptions.EnableLink && !String.IsNullOrEmpty(linkTargetFolder) && !String.IsNullOrEmpty(linkSourceFolder) && Directory.Exists(linkSourceFolder))
						{
							Utils.LogMessage($"CleanHardLinksFiles");
							if (Utils.IsEligibleHardLink(linkTargetFolder))
							{
								Utils.CleanHardLinksFiles(linkTargetFolder, linkSourceFolder);
							}
						}

						if (gameOptions.AhkAfter.Trim() != "")
						{
							Utils.LogMessage($"Execute AhkAfter");
							Utils.ExecuteAHK(gameOptions.AhkAfter,true);
						}

						Utils.LogMessage($"CleanAndKillAhk");
						Utils.CleanAndKillAhk();

						
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
							try
							{
								controller.Disconnect();
								client.Dispose();
							}
							catch { }
						}

						
						if (startupForm != null && !startupForm.IsDisposed)
						{
							Utils.LogMessage($"Close startupForm");
							// Utilisez Invoke pour fermer le formulaire depuis un thread différent
							startupForm.Invoke(new Action(() => startupForm.Close()));
						}

						if (_restoreSwitch)
						{
							Utils.LogMessage($"Restore Disposition");
							MonitorSwitcher.LoadDisplaySettings(Path.Combine(Program.DispositionFolder, "dispositionrestore_app.xml"));
						}

						if (DebugMode)
						{
							Console.WriteLine("Press any key to quit");
							Console.ReadKey();
						}
						if(shifterHack != null) shifterHack.Stop();
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

		private static async Task ShowFormAsync(CancellationToken cancellationToken)
		{
			startupForm = new Startup();
			Application.Run(startupForm);

			// Attendez la demande d'annulation avant de fermer définitivement le formulaire
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

		public static Dictionary<string, JoystickButton> ParseConfig(string configFilePath, bool skipMissingXinput = true)
		{
			//string configFilePath = @"E:\TODO\teknoparrot\TeknoparrotXinputSetup\config\Batman.gamepad.txt";

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(configFilePath);
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
			// Utilisation d'une expression régulière pour extraire le numéro du slot
			Match match = Regex.Match(bindNameXi, @"Input Device (\d+)");
			if (match.Success)
			{
				// Conversion du match en entier
				if (int.TryParse(match.Groups[1].Value, out int slotNumber))
				{
					return slotNumber;
				}
			}

			// Retourner -1 si aucune correspondance ou conversion échouée
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
				try
				{
					controller.Disconnect();
					client.Dispose();
				}
				catch { }
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

		// Méthode pour ajouter une valeur au dictionnaire à deux dimensions
		static void AddJoyId(int cle1, int cle2, string valeur)
		{
			if (!joysticksIds.ContainsKey(cle1))
			{
				joysticksIds[cle1] = new Dictionary<int, string>();
			}

			joysticksIds[cle1][cle2] = valeur;
		}

		// Méthode pour obtenir une valeur du dictionnaire à deux dimensions
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
				return MonitorSwitcher.LoadDisplaySettings(cfg);
			}
			return false;
		}

		static void RedirectConsoleOutput()
		{
			// Rediriger la sortie standard vers la nouvelle console
			IntPtr stdHandle = GetStdHandle(-11); // -11 correspond à STD_OUTPUT_HANDLE
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

		public JoystickButton RemapButtonData(int newXinputSlot)
		{
			var button = new JoystickButton();
			button.ButtonName = this.ButtonName;
			button.Xml = this.Xml;
			button.BindNameXi = this.BindNameXi;
			button.XinputSlot = this.XinputSlot;

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

		public XinputGamepad(X.Gamepad gamepad, int xinputSlot, bool useForceType=true)
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

			
			if(Type == "")
			{
				var MatchList = Program.wheelXinputData.ToLower().Trim().Split(',');
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
				var MatchList = Program.arcadeXinputData.ToLower().Trim().Split(',');
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
				var MatchList = Program.gamepadXinputData.ToLower().Trim().Split(',');
				foreach (var m in MatchList)
				{
					if (m.Trim() != "" && dataTxt.ToLower().Trim().Contains(m.Trim()))
					{
						Type = "gamepad";
						break;
					}
				}
			}

			/*
			Type = "gamepad";
			if (Type == "gamepad")
			{
				if (Gamepad.Capabilities.SubType == X.Gamepad.DeviceSubType.ArcadePad || Gamepad.Capabilities.SubType == X.Gamepad.DeviceSubType.ArcadeStick)
				{
					Type = "arcade";
				}
			}
			if (Type == "gamepad")
			{
				if (Gamepad.Capabilities.SubType == X.Gamepad.DeviceSubType.Wheel)
				{
					Type = "wheel";
				}
			}
			*/


		}

		public static string GetMD5Short(string input)
		{

			using (MD5 md5 = MD5.Create())
			{
				byte[] inputBytes = Encoding.UTF8.GetBytes(input);
				byte[] hashBytes = md5.ComputeHash(inputBytes);

				// Convertir les octets du hash en une chaîne hexadécimale
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