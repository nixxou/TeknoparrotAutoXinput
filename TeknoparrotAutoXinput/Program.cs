using Gma.System.MouseKeyHook;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Exceptions;
using Nefarius.ViGEm.Client.Targets;
using Newtonsoft.Json;
using SDL2;
using SharpDX.DirectInput;
using SharpDX.Multimedia;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using XInput.Wrapper;
using static SDL2.SDL;
using static XInput.Wrapper.X;

namespace TeknoparrotAutoXinput
{
	internal static class Program
	{

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


		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
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
			wheelXinputData = Properties.Settings.Default["wheelXinputData"].ToString();
			arcadeXinputData = Properties.Settings.Default["arcadeXinputData"].ToString();
			gamepadXinputData = Properties.Settings.Default["gamepadXinputData"].ToString();

			if (args.Length == 0)
			{
				ApplicationConfiguration.Initialize();
				Application.Run(new Main());
			}
			if (args.Length > 0)
			{
				
				bool changeFFBConfig = (bool)Properties.Settings.Default["FFB"];
				bool showStartup = (bool)Properties.Settings.Default["showStartup"];

				bool useVirtualKeyboard = (bool)Properties.Settings.Default["virtualKeyboard"];
				string keyTest = Properties.Settings.Default["keyTest"].ToString();
				string keyService1 = Properties.Settings.Default["keyService1"].ToString();
				string keyService2 = Properties.Settings.Default["keyService2"].ToString();



				bool favorAB = (bool)Properties.Settings.Default["favorAB"];

				bool gamepadStooz = (bool)Properties.Settings.Default["gamepadStooz"];
				bool wheelStooz = (bool)Properties.Settings.Default["wheelStooz"];
				bool enableStoozZone_Gamepad = (bool)Properties.Settings.Default["enableStoozZone_Gamepad"];
				int valueStooz_Gamepad = (int)Properties.Settings.Default["valueStooz_Gamepad"];
				bool enableStoozZone_Wheel = (bool)Properties.Settings.Default["enableStoozZone_Wheel"];
				int valueStooz_Wheel = (int)Properties.Settings.Default["valueStooz_Wheel"];


				WheelFFBGuid = Properties.Settings.Default["ffbDinputWheel"].ToString();

				bool passthrough = false;


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
						}

						if(arg.ToLower().Trim() == "--passthrough")
						{
							passthrough = true;
						}
					}


					string finalConfig = "";
					string basePath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
					string customConfigPath = "";

					string xmlFile = args.Last();
					if (!xmlFile.ToLower().EndsWith(".xml") || !File.Exists(xmlFile))
					{
						MessageBox.Show("Invalid parameters");
						return;
					}

					string originalConfigFileName = Path.GetFileName(xmlFile);
					string originalConfigFileNameWithoutExt = Path.GetFileNameWithoutExtension(xmlFile);
					string teknoparrotExe = Path.Combine(Directory.GetParent(Path.GetDirectoryName(Path.GetFullPath(xmlFile))).FullName, "TeknoParrotUi.exe");
					if (!File.Exists(teknoparrotExe))
					{
						MessageBox.Show($"Can't find {teknoparrotExe}");
						return;
					}

					ParrotDataOriginal = Path.Combine(Directory.GetParent(Path.GetDirectoryName(Path.GetFullPath(xmlFile))).FullName, "ParrotData.xml");
					ParrotDataBackup = Path.Combine(Directory.GetParent(Path.GetDirectoryName(Path.GetFullPath(xmlFile))).FullName, "ParrotData.xml.AutoXinputBackup");
					if (File.Exists(ParrotDataBackup))
					{
						try
						{
							File.Copy(ParrotDataBackup, ParrotDataOriginal, true);
							File.Delete(ParrotDataBackup);
						}
						catch { }
					}

					FFBPluginIniFile = "";
					FFBPluginIniBackup = "";
					XmlDocument xmlDocOri = new XmlDocument();
					xmlDocOri.Load(xmlFile);
					XmlNode gamePathNode = xmlDocOri.SelectSingleNode("/GameProfile/GamePath");
					if (gamePathNode != null)
					{
						string gamePathContent = gamePathNode.InnerText;
						FFBPluginIniFile = Path.Combine(Path.GetDirectoryName(gamePathContent), "FFBPlugin.ini");
						FFBPluginIniBackup = Path.Combine(Path.GetDirectoryName(gamePathContent), "FFBPlugin.ini.AutoXinputBackup");
						if (File.Exists(FFBPluginIniBackup))
						{
							try
							{
								File.Copy(FFBPluginIniBackup, FFBPluginIniFile, true);
								File.Delete(FFBPluginIniBackup);
							}
							catch { }
						}
					}

					string emptyConfigPath = Path.Combine(Directory.GetParent(Path.GetDirectoryName(Path.GetFullPath(xmlFile))).FullName, "GameProfiles", originalConfigFileName); ;
					if (!File.Exists(emptyConfigPath))
					{
						finalConfig = xmlFile;
					}

					foreach (var type in typeConfig)
					{
						var configPath = Path.Combine(basePath, "config", originalConfigFileNameWithoutExt + "." + type + ".txt");
						if (File.Exists(configPath))
						{
							existingConfig.Add(type, configPath);
						}
					}
					if (existingConfig.Count() == 0 || passthrough)
					{
						finalConfig = xmlFile;
					}

					bool usealtgamepad = false;
					if (favorAB && existingConfig.ContainsKey("gamepadalt") && existingConfig.ContainsKey("wheel")) usealtgamepad = true;

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

						bool checkDinputWheel = (bool)Properties.Settings.Default["useDinputWheel"];
						Dictionary<string, JoystickButtonData> bindingDinputWheel = null;
						string bindingDinputWheelJson = Properties.Settings.Default["bindingDinputWheel"].ToString();
						
						bool dinputWheelFound = false;
						if (checkDinputWheel)
						{
							if (!string.IsNullOrEmpty(bindingDinputWheelJson))
							{
								bindingDinputWheel = (Dictionary<string, JoystickButtonData>)JsonConvert.DeserializeObject<Dictionary<string, JoystickButtonData>>(bindingDinputWheelJson);
								if (bindingDinputWheel.ContainsKey("InputDevice0LeftThumbInputDevice0X+"))
								{
									WheelGuid = bindingDinputWheel["InputDevice0LeftThumbInputDevice0X+"].JoystickGuid.ToString();
								}
							}
							if (!string.IsNullOrEmpty(WheelGuid))
							{
								DirectInput directInput = new DirectInput();
								List<DeviceInstance> devices = new List<DeviceInstance>();
								devices.AddRange(directInput.GetDevices().Where(x => x.Type != DeviceType.Mouse && x.UsagePage != UsagePage.VendorDefinedBegin && x.Usage != UsageId.AlphanumericBitmapSizeX && x.Usage != UsageId.AlphanumericAlphanumericDisplay && x.UsagePage != unchecked((UsagePage)0xffffff43) && x.UsagePage != UsagePage.Vr).ToList());
								foreach (var device in devices)
								{
									if(device.Type == DeviceType.Keyboard && FirstKeyboardGuid == null)
									{
										FirstKeyboardGuid = device.InstanceGuid;
									}
									if (device.InstanceGuid.ToString() == WheelGuid)
									{
										dinputWheelFound = true;
										haveWheel = true;
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
											ConfigFFB.Write("DeviceGUID", WheelFFBGuid, "Settings");
										}
									//}

								}

							//}

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
									}
								}
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
						
						if (showStartup)
						{


							cancellationTokenSource = new CancellationTokenSource();
							Task.Run(() => ShowFormAsync(cancellationTokenSource.Token));

						}
						
						Process process = new Process();

						process.StartInfo.FileName = teknoparrotExe;
						process.StartInfo.Arguments = "--profile=\"" + finalConfig + "\"";
						process.StartInfo.WorkingDirectory = Path.GetDirectoryName(teknoparrotExe);

						process.StartInfo.UseShellExecute = true;
						//process.StartInfo.RedirectStandardOutput = true;
						//process.StartInfo.RedirectStandardError = true;
						//process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;

						process.Start();

						process.WaitForExit();

						if (!String.IsNullOrEmpty(ParrotDataOriginal) && !String.IsNullOrEmpty(ParrotDataBackup))
						{
							if (File.Exists(ParrotDataBackup))
							{
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
							try
							{
								controller.Disconnect();
								client.Dispose();
							}
							catch { }
						}


						if (startupForm != null && !startupForm.IsDisposed)
						{
							// Utilisez Invoke pour fermer le formulaire depuis un thread différent
							startupForm.Invoke(new Action(() => startupForm.Close()));
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