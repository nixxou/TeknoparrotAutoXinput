﻿using Krypton.Toolkit;
using Newtonsoft.Json;
using SharpDX.DirectInput;
using SharpDX.Multimedia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TeknoParrotUi.Common;
using XInput.Wrapper;

namespace TeknoparrotAutoXinput
{
	public partial class Main : KryptonForm
	{
		private Dictionary<string, Game> _gameList = new Dictionary<string, Game>();
		private string _tpFolder = "";
		private string _userProfileFolder = "";
		private string _basePath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);

		private Dictionary<int, XinputGamepad> _connectedGamePad = new Dictionary<int, XinputGamepad>();
		private bool _dinputWheelFound = false;
		private string _dinputWheelName = "";
		private bool _haveWheel = false;
		private bool _haveGamepad = false;
		private bool _haveArcade = false;

		private bool _isPlaying = false;
		public bool isPlaying
		{
			get
			{
				return _isPlaying;
			}
			set
			{

				_isPlaying = value;
				if (_isPlaying)
				{
					btn_playgame.Enabled = false;
					btn_playgamedirect.Enabled = false;
				}
				else
				{
					if (_playAutoEnabled) btn_playgame.Enabled = true;
					if (_playDirectEnabled) btn_playgamedirect.Enabled = true;
				}

			}
		}


		private bool _playDirectEnabled = false;
		private bool _playAutoEnabled = false;

		List<string> typeConfig = new List<string>();

		int testval = 0;

		public Main()
		{
			InitializeComponent();
			this.Activated += VotreForm_Activated;
			this.Deactivate += VotreForm_Deactivate;

			chk_showAll.Checked = (bool)Properties.Settings.Default["ShowAllGames"];

			typeConfig.Add("gamepad");
			typeConfig.Add("gamepadalt");
			typeConfig.Add("arcade");
			typeConfig.Add("wheel");

			lbl_player1.Text = "";
			lbl_player2.Text = "";
			lbl_player3.Text = "";
			lbl_player4.Text = "";
			lbl_GameTitle.Text = "";

			Reload();
			UpdateGamePadList();

		}


		private void VotreForm_Activated(object sender, EventArgs e)
		{
			timer_controllerUpdate.Enabled = true;
		}

		private void VotreForm_Deactivate(object sender, EventArgs e)
		{
			timer_controllerUpdate.Enabled = false;
		}



		private void Reload()
		{
			list_games.Items.Clear();
			_gameList = new Dictionary<string, Game>();
			_tpFolder = Properties.Settings.Default["TpFolder"].ToString();
			if (Directory.Exists(_tpFolder))
			{
				string UserProfileDir = Path.Combine(Path.GetFullPath(_tpFolder), "UserProfiles");
				if (Directory.Exists(UserProfileDir))
				{
					list_games.Enabled = true;
					_userProfileFolder = UserProfileDir;
					var profileList = Directory.GetFiles(_userProfileFolder, "*.xml");
					foreach (var profile in profileList)
					{
						if (profile.ToLower().EndsWith("custom.xml")) continue;
						string gameName = ExtractGameNameInternal(profile);
						if (!_gameList.ContainsKey(gameName))
						{
							var newGame = new Game();
							newGame.Name = gameName;
							newGame.UserConfigFile = profile;
							newGame.FileName = Path.GetFileName(profile);
							newGame.Metadata = DeSerializeMetadata(profile);
							if (newGame.Metadata != null)
							{
								newGame.Name = newGame.Metadata.game_name;
							}

							foreach (var type in typeConfig)
							{
								var configPath = Path.Combine(_basePath, "config", Path.GetFileNameWithoutExtension(profile) + "." + type + ".txt");
								if (File.Exists(configPath))
								{
									newGame.existingConfig.Add(type, configPath);
								}
							}

							_gameList.Add(newGame.Name, newGame);
						}
					}
					List<string> sortedGameList = _gameList.Keys.OrderBy(key => key).ToList();
					foreach (var gameName in sortedGameList)
					{
						if (_gameList[gameName].existingConfig.Count > 0)
						{
							list_games.Items.Add(gameName);
						}
						else
						{
							if (chk_showAll.Checked)
							{
								list_games.Items.Add(gameName + " [NOT SUPPORTED]");
							}

						}



					}




				}

			}

		}

		private void UpdateGamePadList()
		{
			string wheelGuid = string.Empty;
			_dinputWheelName = string.Empty;
			_dinputWheelFound = false;
			_haveWheel = _haveArcade = _haveGamepad = false;

			_connectedGamePad.Clear();
			var gamepad = X.Gamepad_1;
			if (gamepad.Capabilities.Type != 0) _connectedGamePad.Add(0, new XinputGamepad(gamepad, 0, false));
			gamepad = X.Gamepad_2;
			if (gamepad.Capabilities.Type != 0) _connectedGamePad.Add(1, new XinputGamepad(gamepad, 1, false));
			gamepad = X.Gamepad_3;
			if (gamepad.Capabilities.Type != 0) _connectedGamePad.Add(2, new XinputGamepad(gamepad, 2, false));
			gamepad = X.Gamepad_4;
			if (gamepad.Capabilities.Type != 0) _connectedGamePad.Add(3, new XinputGamepad(gamepad, 3, false));
			bool checkDinputWheel = (bool)Properties.Settings.Default["useDinputWheel"];
			Dictionary<string, JoystickButtonData> bindingDinputWheel = null;
			string bindingDinputWheelJson = Properties.Settings.Default["bindingDinputWheel"].ToString();

			foreach (var cg in _connectedGamePad)
			{
				if (cg.Value.Type == "arcade") _haveArcade = true;
				if (cg.Value.Type == "gamepad") _haveGamepad = true;
				if (cg.Value.Type == "wheel") _haveWheel = true;
			}

			if (checkDinputWheel)
			{
				if (!string.IsNullOrEmpty(bindingDinputWheelJson))
				{
					bindingDinputWheel = (Dictionary<string, JoystickButtonData>)JsonConvert.DeserializeObject<Dictionary<string, JoystickButtonData>>(bindingDinputWheelJson);
					if (bindingDinputWheel.ContainsKey("InputDevice0LeftThumbInputDevice0X+"))
					{
						wheelGuid = bindingDinputWheel["InputDevice0LeftThumbInputDevice0X+"].JoystickGuid.ToString();
					}
				}
				if (!string.IsNullOrEmpty(wheelGuid))
				{
					DirectInput directInput = new DirectInput();
					List<DeviceInstance> devices = new List<DeviceInstance>();
					devices.AddRange(directInput.GetDevices().Where(x => x.Type != DeviceType.Mouse && x.UsagePage != UsagePage.VendorDefinedBegin && x.Usage != UsageId.AlphanumericBitmapSizeX && x.Usage != UsageId.AlphanumericAlphanumericDisplay && x.UsagePage != unchecked((UsagePage)0xffffff43) && x.UsagePage != UsagePage.Vr).ToList());
					foreach (var device in devices)
					{
						if (device.InstanceGuid.ToString() == wheelGuid)
						{
							_dinputWheelFound = true;
							_dinputWheelName = device.ProductName;
							_haveWheel = true;
						}
					}
				}
			}

		}

		static string ExtractGameNameInternal(string cheminFichierXml)
		{
			string DefaultName = Path.GetFileName(cheminFichierXml);
			DefaultName = DefaultName.Substring(0, DefaultName.Length - 4);

			try
			{
				using (FileStream fs = new FileStream(cheminFichierXml, FileMode.Open, FileAccess.Read))
				using (StreamReader reader = new StreamReader(fs))
				{
					int bufferSize = 4096;
					char[] buffer = new char[bufferSize];
					string pattern = "<GameNameInternal>(.*?)</GameNameInternal>";
					Regex regex = new Regex(pattern);
					while (!reader.EndOfStream)
					{
						int bytesRead = reader.Read(buffer, 0, bufferSize);
						string bufferContent = new string(buffer, 0, bytesRead);
						Match match = regex.Match(bufferContent);
						if (match.Success)
						{
							return match.Groups[1].Value;
						}
					}
					return DefaultName;
				}
			}
			catch { return DefaultName; }
		}

		public static Metadata DeSerializeMetadata(string fileName)
		{
			string ParentPath = Path.GetDirectoryName(fileName);
			ParentPath = Path.GetDirectoryName(ParentPath);

			var metadataPath = Path.Combine(ParentPath, "Metadata", Path.GetFileNameWithoutExtension(fileName) + ".json");
			if (File.Exists(metadataPath))
			{
				try
				{
					return JsonConvert.DeserializeObject<Metadata>(File.ReadAllText(metadataPath));
				}
				catch
				{
					Debug.WriteLine($"Error loading Metadata file {metadataPath}!");
				}
			}
			else
			{
				Debug.WriteLine($"Metadata file {metadataPath} missing!");
			}
			return null;
		}

		private void Main_Load(object sender, EventArgs e)
		{

		}

		private void btn_globalconfig_Click(object sender, EventArgs e)
		{
			var frm = new Form1();
			var result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{
				Reload();
			}
		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void chk_showAll_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default["ShowAllGames"] = chk_showAll.Checked;
			Properties.Settings.Default.Save();
			Reload();
		}

		private void hg_controllerState_Paint(object sender, PaintEventArgs e)
		{

		}

		private void kryptonLabel6_Click(object sender, EventArgs e)
		{

		}

		private void timer_controllerUpdate_Tick(object sender, EventArgs e)
		{
			lbl_gamepadlist.Text = "";
			lbl_arcadelist.Text = "";
			lbl_wheellist.Text = "";
			UpdateGamePadList();
			foreach (var gp in _connectedGamePad)
			{
				string displayControllerName = "XINPUT" + (gp.Value.XinputSlot + 1).ToString() + " " + gp.Value.ControllerName;

				if (gp.Value.Type == "gamepad") lbl_gamepadlist.Text += $"{displayControllerName}, ";
				if (gp.Value.Type == "arcade") lbl_arcadelist.Text += $"{displayControllerName}, ";
				if (gp.Value.Type == "wheel") lbl_wheellist.Text += $"{displayControllerName}, ";
			}
			if (_dinputWheelFound) lbl_wheellist.Text = _dinputWheelName;
			lbl_arcadelist.Text = lbl_arcadelist.Text.TrimEnd().TrimEnd(',');
			lbl_gamepadlist.Text = lbl_gamepadlist.Text.TrimEnd().TrimEnd(',');
			lbl_wheellist.Text = lbl_wheellist.Text.TrimEnd().TrimEnd(',');

		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{

		}

		private void list_games_SelectedIndexChanged(object sender, EventArgs e)
		{
			btn_playgame.Enabled = false;
			btn_playgamedirect.Enabled = false;
			btn_gameoptions.Enabled = false;
			_playAutoEnabled = false;
			_playDirectEnabled = false;
			lbl_player1.Text = "";
			lbl_player2.Text = "";
			lbl_player3.Text = "";
			lbl_player4.Text = "";
			flowLayoutPanelThumbs.Controls.Clear();

			lbl_GameTitle.Text = string.Empty;
			pictureBox_gameControls.Image = null;
			if (list_games.SelectedItems.Count > 0)
			{
				btn_playgamedirect.Enabled = true;
				string GameSelected = list_games.SelectedItems[0].ToString();
				GameSelected = GameSelected.Replace(" [NOT SUPPORTED]", "");
				if (_gameList.ContainsKey(GameSelected))
				{


					var DataGame = _gameList[GameSelected];
					lbl_GameTitle.Text = DataGame.Name;


					bool usealtgamepad = false;
					bool favorAB = (bool)Properties.Settings.Default["favorAB"];
					if (favorAB && DataGame.existingConfig.ContainsKey("gamepadalt") && DataGame.existingConfig.ContainsKey("wheel")) usealtgamepad = true;

					bool useXinput = true;
					bool useDinputWheel = false;
					if (_haveWheel && DataGame.existingConfig.ContainsKey("wheel") && _dinputWheelFound)
					{
						useXinput = false;
						useDinputWheel = true;
					}
					Dictionary<int, (string, XinputGamepad)> ConfigPerPlayer = new Dictionary<int, (string, XinputGamepad)>();
					if (useXinput)
					{
						if (_haveWheel && DataGame.existingConfig.ContainsKey("wheel"))
						{
							var joystickButtonWheel = Program.ParseConfig(DataGame.existingConfig["wheel"]);
							var PlayerList = Program.GetPlayersList(joystickButtonWheel);
							int nb_wheel = _connectedGamePad.Values.Where(c => c.Type == "wheel").Count();
							int currentlyAttributed = 0;
							List<XinputGamepad> gamepadList = new List<XinputGamepad>();
							foreach (var cgp in _connectedGamePad.Values)
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
						if (_haveArcade && DataGame.existingConfig.ContainsKey("arcade"))
						{
							var joystickButtonArcade = Program.ParseConfig(DataGame.existingConfig["arcade"]);
							var PlayerList = Program.GetPlayersList(joystickButtonArcade);
							int nb_arcade = _connectedGamePad.Values.Where(c => c.Type == "arcade").Count();
							int currentlyAttributed = 0;
							List<XinputGamepad> gamepadList = new List<XinputGamepad>();
							foreach (var cgp in _connectedGamePad.Values)
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
						if (_haveGamepad && DataGame.existingConfig.ContainsKey("gamepad"))
						{
							string configname = "gamepad";
							if (usealtgamepad) configname = "gamepadalt";

							var joystickButtonGamepad = Program.ParseConfig(DataGame.existingConfig[configname]);
							var PlayerList = Program.GetPlayersList(joystickButtonGamepad);
							int nb_gamepad = _connectedGamePad.Values.Where(c => c.Type == "gamepad").Count();
							int currentlyAttributed = 0;
							List<XinputGamepad> gamepadList = new List<XinputGamepad>();
							foreach (var cgp in _connectedGamePad.Values)
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
							var joystickButtonWheel = Program.ParseConfig(DataGame.existingConfig["wheel"]);
							XinputGamepad xinputGamepad = new XinputGamepad(0);
							xinputGamepad.Type = "wheel";
							xinputGamepad.ControllerName = _dinputWheelName;
							ConfigPerPlayer.Add(0, ("wheel", xinputGamepad));
						}
					}

					int currentcpp = 0;
					foreach (var cpp in ConfigPerPlayer)
					{
						currentcpp++;
						if (useXinput)
						{
							if (currentcpp == 1) lbl_player1.Text = "Player 1 : " + cpp.Value.Item1 + " -> " + "XINPUT " + (cpp.Value.Item2.XinputSlot + 1).ToString() + $" ({cpp.Value.Item2.ControllerName})";
							if (currentcpp == 2) lbl_player2.Text = "Player 2 : " + cpp.Value.Item1 + " -> " + "XINPUT " + (cpp.Value.Item2.XinputSlot + 1).ToString() + $" ({cpp.Value.Item2.ControllerName})";
							if (currentcpp == 3) lbl_player3.Text = "Player 3 : " + cpp.Value.Item1 + " -> " + "XINPUT " + (cpp.Value.Item2.XinputSlot + 1).ToString() + $" ({cpp.Value.Item2.ControllerName})";
							if (currentcpp == 4) lbl_player4.Text = "Player 4 : " + cpp.Value.Item1 + " -> " + "XINPUT " + (cpp.Value.Item2.XinputSlot + 1).ToString() + $" ({cpp.Value.Item2.ControllerName})";
						}
						else
						{
							if (useDinputWheel)
							{
								lbl_player1.Text = "Player 1 : " + cpp.Value.Item1 + " -> " + "DINPUT " + $" ({cpp.Value.Item2.ControllerName})";
							}
						}

						if (currentcpp == 4) break;
					}

					string FirstConfig = string.Empty;
					if (DataGame.existingConfig.ContainsKey("gamepad"))
					{
						FirstConfig = DataGame.existingConfig["gamepad"];
					}
					if (ConfigPerPlayer.Count() > 0)
					{
						var FirstPlayer = ConfigPerPlayer[0];
						string FirstConfigLabel = FirstPlayer.Item1;
						if (FirstConfigLabel == "gamepad" && usealtgamepad) FirstConfigLabel = "gamepadalt";
						FirstConfig = DataGame.existingConfig[FirstConfigLabel];
					}

					if (FirstConfig != "" && File.Exists(FirstConfig))
					{
						btn_gameoptions.Enabled = true;
						_playAutoEnabled = true;
						_playDirectEnabled = true;
						isPlaying = isPlaying;
						{
							string fileName = FirstConfig;
							string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
							string fileDirectory = Path.GetDirectoryName(fileName);
							fileDirectory = Path.GetDirectoryName(fileDirectory);
							string imgFile = Path.Combine(fileDirectory, "img", fileNameWithoutExt + ".jpg");
							if (File.Exists(imgFile))
							{
								Image originalImage = System.Drawing.Image.FromFile(imgFile);
								pictureBox_gameControls.Image = ResizeImageBest(originalImage, pictureBox_gameControls.Size);
							}
						}

						List<string> AllImages = new List<string>();
						foreach (var configFile in DataGame.existingConfig)
						{
							if (configFile.Key == "gamepad" && usealtgamepad) continue;
							if (configFile.Key == "gamepadalt" && !usealtgamepad) continue;

							string fileName = configFile.Value;
							string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
							string fileDirectory = Path.GetDirectoryName(fileName);
							fileDirectory = Path.GetDirectoryName(fileDirectory);
							string imgFile = Path.Combine(fileDirectory, "img", fileNameWithoutExt + ".jpg");
							if (File.Exists(imgFile))
							{
								AllImages.Add(imgFile);
							}
						}
						AddPictureBoxesToFlowLayoutPanel(AllImages.ToArray());

					}
					else
					{
						_playAutoEnabled = false;
						_playDirectEnabled = true;
						isPlaying = isPlaying;
						btn_gameoptions.Enabled = true;

						string fileDirectory = Path.GetDirectoryName(DataGame.UserConfigFile);
						fileDirectory = Path.GetDirectoryName(fileDirectory);
						string iconFile = string.Empty;
						if (DataGame.Metadata != null)
						{
							iconFile = DataGame.Metadata.icon_name;
						}
						else
						{
							iconFile = Path.GetFileNameWithoutExtension(DataGame.UserConfigFile) + ".png";
						}
						iconFile = Path.Combine(fileDirectory, "Icons", iconFile);
						if (File.Exists(iconFile))
						{
							Image originalImage = System.Drawing.Image.FromFile(iconFile);
							pictureBox_gameControls.Image = ResizeImageBest(originalImage, pictureBox_gameControls.Size);

						}



					}
				}
				else
				{

				}

			}
		}

		public static Image ResizeImageBest(Image image, Size newSize)
		{
			if (image == null)
				throw new ArgumentNullException(nameof(image));

			if (newSize.Width <= 0 || newSize.Height <= 0)
				throw new ArgumentException("La taille doit être supérieure à zéro.", nameof(newSize));

			Bitmap newImage = new Bitmap(newSize.Width, newSize.Height);

			using (Graphics graphics = Graphics.FromImage(newImage))
			{

				float aspectRatio = (float)image.Width / image.Height;
				int newWidth = image.Width;
				int newHeight = image.Height;
				/*
				int newWidth = newSize.Width;
				int newHeight = newSize.Height;

				if (aspectRatio > 1)
				{
					// L'image est plus large que haute, ajuster en fonction de la largeur
					newHeight = (int)(newWidth / aspectRatio);
				}
				else
				{
					// L'image est plus haute que large, ajuster en fonction de la hauteur
					newWidth = (int)(newHeight * aspectRatio);
				}
				*/
				if (newWidth > newSize.Width || newHeight > newSize.Height)
				{
					if ((newWidth / newSize.Width) > (newHeight / newSize.Height))
					{
						newHeight = (int)Math.Round(((double)newSize.Width / (double)newWidth) * (double)newHeight);
						newWidth = newSize.Width;
					}
					else
					{
						newWidth = (int)Math.Round(((double)newSize.Height / (double)newHeight) * (double)newWidth);
						newHeight = newSize.Height;
					}
				}

				int x = (int)Math.Round((double)(newSize.Width - newWidth) / 2.0);
				int y = (int)Math.Round((double)(newSize.Height - newHeight) / 2.0);

				graphics.DrawImage(image, x, y, newWidth, newHeight);
			}
			image.Dispose();
			return newImage;
		}

		private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void AddPictureBoxesToFlowLayoutPanel(String[] selectedImgList)
		{
			if (flowLayoutPanelThumbs.Visible == false) flowLayoutPanelThumbs.Visible = true;
			// Supprimez tous les contrôles existants dans le FlowLayoutPanel
			flowLayoutPanelThumbs.Controls.Clear();

			// La marge entre chaque PictureBox
			int spacing = 10;

			// Pour chaque image dans votre tableau selectedImgList
			foreach (var imgDetails in selectedImgList)
			{


				if (string.IsNullOrEmpty(imgDetails) || !File.Exists(imgDetails)) continue;

				Image ImageThumb = null;
				try
				{
					Image originalImage = System.Drawing.Image.FromFile(imgDetails);
					ImageThumb = ResizeImageBest(originalImage, new Size(77, 50));
				}
				catch
				{
					continue;
				}
				if (ImageThumb == null) continue;


				// Créez une nouvelle instance de PictureBox
				PictureBox pictureBox = new PictureBox();

				pictureBox.Anchor = AnchorStyles.Left;

				// Définissez la taille de la PictureBox à 77x77 pixels
				pictureBox.Size = new Size(77, 50);

				// Assurez-vous que la taille de l'image est ajustée à la taille de la PictureBox
				pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

				// Définissez l'image à afficher dans la PictureBox (imgDetails.Image représente votre image)
				pictureBox.Image = ImageThumb;

				// Ajoutez une fonction anonyme pour gérer le clic sur la PictureBox
				pictureBox.Click += (sender, e) =>
				{
					try
					{

						Image originalImage = System.Drawing.Image.FromFile(imgDetails);
						pictureBox_gameControls.Image = ResizeImageBest(originalImage, pictureBox_gameControls.Size);
						pictureBox_gameControls.Visible = true;
					}
					catch
					{
						pictureBox_gameControls.Image = null;
					}


				};

				// Ajoutez la PictureBox au FlowLayoutPanel
				flowLayoutPanelThumbs.Controls.Add(pictureBox);

				/*
				// Définissez la marge entre les PictureBox
				flowLayoutPanelThumbs.SetFlowBreak(pictureBox, true);

				// Ajoutez un espacement horizontal entre les PictureBox
				if (flowLayoutPanelThumbs.Controls.Count > 1)
				{
					pictureBox.Margin = new Padding(spacing, 0, 0, 0);
				}
				*/
			}
		}

		private void kryptonLabel6_Click_1(object sender, EventArgs e)
		{

		}

		private async void btn_playgame_Click(object sender, EventArgs e)
		{

			string finalConfig = "";
			if (list_games.SelectedItems.Count > 0)
			{
				btn_playgamedirect.Enabled = true;
				string GameSelected = list_games.SelectedItems[0].ToString();
				GameSelected = GameSelected.Replace(" [NOT SUPPORTED]", "");
				if (_gameList.ContainsKey(GameSelected))
				{
					finalConfig = _gameList[GameSelected].UserConfigFile;
				}
			}
			if (string.IsNullOrEmpty(finalConfig)) return;


			isPlaying = true;

			await Task.Run(() =>
			{
				ProcessStartInfo startInfo = new ProcessStartInfo
				{
					FileName = Process.GetCurrentProcess().MainModule.FileName,
					Arguments = $"\"{finalConfig}\"",
					WorkingDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
					UseShellExecute = true
				};

				using (Process process = new Process { StartInfo = startInfo })
				{
					try
					{
						process.Start();
						process.WaitForExit();
					}
					catch (Exception ex)
					{
						MessageBox.Show("Erreur lors du lancement du programme : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			});

			isPlaying = false;
		}

		private async void btn_playgamedirect_Click(object sender, EventArgs e)
		{
			string finalConfig = "";
			if (list_games.SelectedItems.Count > 0)
			{
				btn_playgamedirect.Enabled = true;
				string GameSelected = list_games.SelectedItems[0].ToString();
				GameSelected = GameSelected.Replace(" [NOT SUPPORTED]", "");
				if (_gameList.ContainsKey(GameSelected))
				{
					finalConfig = _gameList[GameSelected].UserConfigFile;
				}
			}
			if (string.IsNullOrEmpty(finalConfig)) return;


			isPlaying = true;

			await Task.Run(() =>
			{
				ProcessStartInfo startInfo = new ProcessStartInfo
				{
					FileName = Process.GetCurrentProcess().MainModule.FileName,
					Arguments = $"--passthrough \"{finalConfig}\"",
					WorkingDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
					UseShellExecute = true
				};

				using (Process process = new Process { StartInfo = startInfo })
				{
					try
					{
						process.Start();
						process.WaitForExit();
					}
					catch (Exception ex)
					{
						MessageBox.Show("Erreur lors du lancement du programme : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			});

			isPlaying = false;
			/*
			string finalConfig = "";
			if (list_games.SelectedItems.Count > 0)
			{
				btn_playgamedirect.Enabled = true;
				string GameSelected = list_games.SelectedItems[0].ToString();
				GameSelected = GameSelected.Replace(" [NOT SUPPORTED]", "");
				if (_gameList.ContainsKey(GameSelected))
				{
					finalConfig = _gameList[GameSelected].UserConfigFile;
				}
			}
			if (string.IsNullOrEmpty(finalConfig)) return;


			string teknoparrotExe = Path.Combine(_tpFolder, "TeknoParrotUi.exe");
			if (!File.Exists(teknoparrotExe)) return;

			isPlaying = true;

			await Task.Run(() =>
			{
				ProcessStartInfo startInfo = new ProcessStartInfo
				{
					FileName = teknoparrotExe,
					Arguments = "--profile=\"" + finalConfig + "\"",
					WorkingDirectory = Path.GetDirectoryName(teknoparrotExe),
					UseShellExecute = true
				};

				using (Process process = new Process { StartInfo = startInfo })
				{
					try
					{
						process.Start();
						process.WaitForExit();
					}
					catch (Exception ex)
					{
						MessageBox.Show("Erreur lors du lancement du programme : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			});

			isPlaying = false;
			*/
		}

		private void btn_gameoptions_Click(object sender, EventArgs e)
		{
			if (list_games.SelectedItems.Count == 0)
			{
				return;
			}

			string GameSelected = list_games.SelectedItems[0].ToString();
			GameSelected = GameSelected.Replace(" [NOT SUPPORTED]", "");
			if (_gameList.ContainsKey(GameSelected))
			{
				var frm = new GameOptions(_gameList[GameSelected]);
				var result = frm.ShowDialog();
				if (result == DialogResult.OK)
				{

				}
			}



		}
	}

	public class Game
	{
		public string Name;
		public string FileName;
		public string UserConfigFile;
		public Metadata Metadata;
		public Dictionary<string, string> existingConfig = new Dictionary<string, string>();
	}
}
