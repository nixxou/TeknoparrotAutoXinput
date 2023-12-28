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

			if (args.Length == 0)
			{
				ApplicationConfiguration.Initialize();
				Application.Run(new Form1());
			}
			if (args.Length > 0)
			{
				
				bool changeFFBConfig = (bool)Properties.Settings.Default["FFB"];
				bool showStartup = (bool)Properties.Settings.Default["showStartup"];

				bool useVirtualKeyboard = (bool)Properties.Settings.Default["virtualKeyboard"];
				string keyTest = Properties.Settings.Default["keyTest"].ToString();
				string keyService1 = Properties.Settings.Default["keyService1"].ToString();
				string keyService2 = Properties.Settings.Default["keyService2"].ToString();

				wheelXinputData = Properties.Settings.Default["wheelXinputData"].ToString();
				arcadeXinputData = Properties.Settings.Default["arcadeXinputData"].ToString();
				gamepadXinputData = Properties.Settings.Default["gamepadXinputData"].ToString();

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

		static List<int> GetPlayersList(Dictionary<string, JoystickButton> joystickButtonDictionary)
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

		static Dictionary<string, JoystickButton> ParseConfig(string configFilePath, bool skipMissingXinput = true)
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

		public XinputGamepad(int xinputSlot)
		{
			XinputSlot = xinputSlot;
			Gamepad = null;
		}

		public XinputGamepad(X.Gamepad gamepad, int xinputSlot)
		{
			Gamepad = gamepad;
			XinputSlot = xinputSlot;
			var caps = Gamepad.Capabilities;
			string json = JsonConvert.SerializeObject(caps, Newtonsoft.Json.Formatting.None);
			Signature = GetMD5Short(json);
			SubType = caps.SubType.ToString().Trim();


			ushort vendorId, productId, revisionId;
			bool result = XInputWrapper.GetControllerInfo((uint)xinputSlot, out vendorId, out productId, out revisionId);
			if (result)
			{
				VendorId = vendorId;
				ProductId = productId;
				RevisionID = revisionId;
			}

			Type = "";
			string dataTxt = ToString();

			if (Program.forceTypeController.ContainsKey(xinputSlot))
			{
				Type = Program.forceTypeController[xinputSlot];
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
			return $"XINPUT{XinputSlot}<>Type={SubType}<>Signature={Signature}<>VendorID=0x{VendorId:X04}<>ProductID=0x{ProductId:X04}<>RevisionID=0x{RevisionID:X04}";
		}




	}
}