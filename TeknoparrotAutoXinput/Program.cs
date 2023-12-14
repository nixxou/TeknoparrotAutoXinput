using Gma.System.MouseKeyHook;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Exceptions;
using Nefarius.ViGEm.Client.Targets;
using Newtonsoft.Json;
using SDL2;
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


		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Application.ApplicationExit += new EventHandler(OnApplicationExit);
#if DEBUG
			List<string> fakeArgs = new List<string>();
			fakeArgs.Add(@"C:\teknoparrot\UserProfiles\Daytona3.xml");
			//args = fakeArgs.ToArray();
			//MessageBox.Show("zog");
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
				//showStartup = false;
				bool useVirtualKeyboard = (bool)Properties.Settings.Default["virtualKeyboard"];
				string keyTest = Properties.Settings.Default["keyTest"].ToString();
				string keyService1 = Properties.Settings.Default["keyService1"].ToString();
				string keyService2 = Properties.Settings.Default["keyService2"].ToString();



				List<string> typeConfig = new List<string>();
				typeConfig.Add("gamepad");
				typeConfig.Add("arcade");
				typeConfig.Add("wheel");


				Dictionary<string, string> existingConfig = new Dictionary<string, string>();

				if (args.Length > 0)
				{
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
					if (existingConfig.Count() == 0)
					{
						finalConfig = xmlFile;
					}

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

						Dictionary<string, JoystickButton> finalJoystickButtonDictionary = ParseConfig(xmlFile, false);
						Dictionary<string, JoystickButton> emptyJoystickButtonDictionary = ParseConfig(emptyConfigPath, false);
						Dictionary<int, (string, XinputGamepad)> ConfigPerPlayer = new Dictionary<int, (string, XinputGamepad)>();
						Dictionary<string, JoystickButton> joystickButtonWheel = new Dictionary<string, JoystickButton>();
						Dictionary<string, JoystickButton> joystickButtonArcade = new Dictionary<string, JoystickButton>();
						Dictionary<string, JoystickButton> joystickButtonGamepad = new Dictionary<string, JoystickButton>();

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
						{
							joystickButtonGamepad = ParseConfig(existingConfig["gamepad"]);
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
						
						if (changeFFBConfig && ConfigPerPlayer.Count > 0)
						{
							var firstPlayer = ConfigPerPlayer.First();
							XmlDocument xmlDocOri = new XmlDocument();
							xmlDocOri.Load(xmlFile);
							XmlNode gamePathNode = xmlDocOri.SelectSingleNode("/GameProfile/GamePath");
							if (gamePathNode != null)
							{
								string gamePathContent = gamePathNode.InnerText;
								string FFBPluginIniFile = Path.Combine(Path.GetDirectoryName(gamePathContent), "FFBPlugin.ini");
								if (File.Exists(FFBPluginIniFile))
								{
									var ConfigFFB = new IniFile(FFBPluginIniFile);
									if (ConfigFFB.KeyExists("DeviceGUID", "Settings"))
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

								}

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
							var imgPath = Path.Combine(basePath, "img", originalConfigFileNameWithoutExt + "." + ConfigPlayer.Value.Item1 + ".jpg");
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