using Krypton.Toolkit;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml;

namespace TeknoparrotAutoXinput
{
	public partial class GameOptionsSimple : KryptonForm
	{
		public Game GameData;
		private string _tpBaseFolder;
		private string _elfldr2Folder = "";
		private string _lindberghFolder = "";
		private string _linkTargetFolder = "";
		private string _linkSourceFolder = "";

		private string _linkTargetFolderExe = "";
		private string _linkSourceFolderExe = "";

		public string PerGameConfigFile = "";

		private List<string> dllPathList = new List<string>();
		private Dictionary<string, string> GameInfo = new Dictionary<string, string>();

		GameSettings gameSettings = new GameSettings();

		public GameOptionsSimple(Game gameData)
		{
			GameData = gameData;
			InitializeComponent();

			lbl_Titre.Text = GameData.Name;
			string TpFolder = Path.GetDirectoryName(Path.GetFullPath(GameData.UserConfigFile));
			TpFolder = Path.GetDirectoryName(TpFolder);
			_tpBaseFolder = TpFolder;
			_lindberghFolder = Path.Combine(_tpBaseFolder, "TeknoParrot");
			_elfldr2Folder = Path.Combine(_tpBaseFolder, "ElfLdr2");
			if (!Directory.Exists(Program.GameOptionsFolder))
			{
				Directory.CreateDirectory(Program.GameOptionsFolder);
			}
			PerGameConfigFile = Path.Combine(Program.GameOptionsFolder, Path.GetFileNameWithoutExtension(GameData.UserConfigFile) + ".json");
			if (File.Exists(PerGameConfigFile))
			{
				gameSettings = new GameSettings(File.ReadAllText(PerGameConfigFile));
			}

			if (ShifterHack.supportedGames.ContainsKey(Path.GetFileNameWithoutExtension(GameData.UserConfigFile))) chk_enableGearChange.Enabled = true;

			List<string> typeConfig = new List<string>();
			typeConfig.Add("gamepad");
			typeConfig.Add("gamepadalt");
			typeConfig.Add("arcade");
			typeConfig.Add("wheel");
			typeConfig.Add("hotas");
			typeConfig.Add("lightgun");
			string basePath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
			bool hideWheelTab = true;
			bool hideHotasTab = true;
			bool hideGunTab = true;
			foreach (var type in typeConfig)
			{
				var configPath = Path.Combine(basePath, "config", Path.GetFileNameWithoutExtension(GameData.UserConfigFile) + "." + type + ".txt");
				if (File.Exists(configPath))
				{
					if (type == "wheel")
					{
						hideWheelTab = false;
					}
					if (type == "hotas")
					{
						hideHotasTab= false;
					}
					if (type == "lightgun")
					{
						hideGunTab = false;
					}
				}
			}
			if(hideWheelTab) tabInfo.TabPages.RemoveByKey("tabWheel");
			if(hideHotasTab) tabInfo.TabPages.RemoveByKey("tabHotas");
			if(hideGunTab) tabInfo.TabPages.RemoveByKey("tabLightgun");


			/*
			foreach (var type in typeConfig)
			{
				var configPath = Path.Combine(basePath, "config", Path.GetFileNameWithoutExtension(GameData.UserConfigFile) + "." + type + ".txt");
				if (File.Exists(configPath))
				{
					if (type == "wheel")
					{
						tabInfo.SelectedIndex = 2;
					}
					if (type == "hotas")
					{
						tabInfo.SelectedIndex = 3;
					}
					if (type == "lightgun")
					{
						tabInfo.SelectedIndex = 4;
					}
				}
			}
			*/

			var infoFile = Path.Combine(basePath, "config", Path.GetFileNameWithoutExtension(GameData.UserConfigFile) + ".info.json");
			if (File.Exists(infoFile))
			{
				txt_info.Text = File.ReadAllText(infoFile);
				try
				{
					var gameInfoParsedJson = JObject.Parse(txt_info.Text);
					var gameInfoGlobalSection = (JObject)gameInfoParsedJson["global"];
					GameInfo = gameInfoGlobalSection.ToObject<Dictionary<string, string>>();
					if (GameInfo.ContainsKey("showGameOptionBezel") && GameInfo["showGameOptionBezel"].ToLower() == "false") kryptonLabel6.Visible = cmb_useBezel.Visible = false;
					if (GameInfo.ContainsKey("showGameOptionCrt") && GameInfo["showGameOptionCrt"].ToLower() == "false") kryptonLabel7.Visible = cmb_useCrt.Visible = false;
					if (GameInfo.ContainsKey("showGameOptionVsync") && GameInfo["showGameOptionVsync"].ToLower() == "false") kryptonLabel18.Visible = cmb_forcevsync.Visible = false;
					if (GameInfo.ContainsKey("showGameOptionFFB") && GameInfo["showGameOptionFFB"].ToLower() == "false") kryptonLabel39.Visible = cmb_patchFFB.Visible = false;
					if (GameInfo.ContainsKey("showGameOptionKeepAspectRatio") && GameInfo["showGameOptionKeepAspectRatio"].ToLower() == "false") kryptonLabel21.Visible = cmb_keepAspectRatio.Visible = false;

				}
				catch (Exception ex) { MessageBox.Show("Invalid game.info json, please report the issue"); }
			}

		}

		private void LinkLoad()
		{
			grp_link.Enabled = false;
			btn_link_open.Enabled = true;
			txt_linkFrom.Text = "";


			grp_linkExe.Enabled = false;
			txt_linkFromExe.Text = "";
			txt_linkToExe.Text = "";

			if (gameSettings.CustomTpExe != "" && File.Exists(gameSettings.CustomTpExe))
			{
				string TpFolder = Path.GetDirectoryName(gameSettings.CustomTpExe);
				_lindberghFolder = Path.Combine(TpFolder, "TeknoParrot");
				_elfldr2Folder = Path.Combine(TpFolder, "ElfLdr2");
			}
			else
			{
				_lindberghFolder = Path.Combine(_tpBaseFolder, "TeknoParrot");
				_elfldr2Folder = Path.Combine(_tpBaseFolder, "ElfLdr2");
			}


			string executableGame = "";
			string executableGameDir = "";
			bool linkTypeExe = true;
			try
			{
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(GameData.UserConfigFile);

				XmlNode gamePathNode = xmlDoc.SelectSingleNode("/GameProfile/GamePath");
				if (gamePathNode != null)
				{
					string gamePathContent = gamePathNode.InnerText;
					executableGame = gamePathContent;
					if (executableGame != "" && File.Exists(executableGame))
					{
						executableGameDir = Path.GetFullPath(Directory.GetParent(executableGame).ToString());
					}
				}


				XmlNode emulatorTypeNode = xmlDoc.SelectSingleNode("/GameProfile/EmulatorType");
				if (emulatorTypeNode != null)
				{
					string emulatorTypeValue = emulatorTypeNode.InnerText.ToLower().Trim();
					if (emulatorTypeValue == "elfldr2" || emulatorTypeValue == "lindbergh")
					{
						linkTypeExe = false;
						string perGameLinkFolder = ConfigurationManager.MainConfig.perGameLinkFolder;
						if (perGameLinkFolder == @"Default (<YourTeknoparrotFolder>\AutoXinputLinks)")
						{
							_linkSourceFolder = Path.Combine(_tpBaseFolder, "AutoXinputLinks", Path.GetFileNameWithoutExtension(GameData.FileName));
						}
						else
						{
							_linkSourceFolder = Path.Combine(perGameLinkFolder, Path.GetFileNameWithoutExtension(GameData.FileName));
						}


						//lbl_linkFrom.Text = "Link from : " + _linkSourceFolder;
						txt_linkFrom.Text = _linkSourceFolder;

						grp_link.Enabled = true;
						if (emulatorTypeValue == "elfldr2")
						{
							//lbl_LinkTo.Text = "To : " + _elfldr2Folder;
							//txt_linkTo.Text = _elfldr2Folder;
							_linkTargetFolder = _elfldr2Folder;
						}
						if (emulatorTypeValue == "lindbergh")
						{
							//lbl_LinkTo.Text = "To : " + _lindberghFolder;
							//txt_linkTo.Text = _lindberghFolder;
							_linkTargetFolder = _lindberghFolder;
						}
					}
				}

				if (Directory.Exists(executableGameDir))
				{
					_linkTargetFolderExe = executableGameDir;

					if (String.IsNullOrEmpty(ConfigurationManager.MainConfig.perGameLinkFolderExe))
					{
						_linkSourceFolderExe = "";
						//lbl_linkNumberExe.Text = "You must define a default link folder in the general config first";
						grp_linkExe.Enabled = false;
					}
					else
					{
						grp_linkExe.Enabled = true;
						_linkSourceFolderExe = "";

						if (gameSettings.CustomPerGameLinkFolder != null && gameSettings.CustomPerGameLinkFolder != "")
						{
							string lastFolder = Path.GetFileName(gameSettings.CustomPerGameLinkFolder);
							if (lastFolder == Path.GetFileNameWithoutExtension(GameData.FileName))
							{
								_linkSourceFolderExe = gameSettings.CustomPerGameLinkFolder;
							}
						}
						else
						{
							_linkSourceFolderExe = Path.Combine(ConfigurationManager.MainConfig.perGameLinkFolderExe, Path.GetFileNameWithoutExtension(GameData.FileName));
						}
						txt_linkFromExe.Text = _linkSourceFolderExe;

					}
					txt_linkToExe.Text = _linkTargetFolderExe;


				}

				if (grp_link.Enabled)
				{
					dllPathList.Add(_linkSourceFolder);
					dllPathList.Add(_linkTargetFolder);
				}
				else
				{
					dllPathList.Add(_linkSourceFolderExe);
					dllPathList.Add(_linkTargetFolderExe);
				}


				//Crosshair
				string crosshairDir = "";
				if (Directory.Exists(Path.Combine(_linkSourceFolderExe, "[!!crosshair!!]", "[!crosshair_gun1_and_gun2!]"))) crosshairDir = Path.Combine(_linkSourceFolderExe, "[!!crosshair!!]", "[!crosshair_gun1_and_gun2!]");
				if (Directory.Exists(Path.Combine(_linkSourceFolderExe, "[!crosshair_gun1_and_gun2!]"))) crosshairDir = Path.Combine(_linkSourceFolderExe, "[!!crosshair!!]", "[!crosshair_gun1_and_gun2!]");
				if (crosshairDir != "")
				{
					if (File.Exists(Path.Combine(crosshairDir, "p1.png")))
					{
						pic_crosshairp1.Image = LoadImage(Path.Combine(crosshairDir, "p1.png"));
						lbl_crosshairp1Size.Text = $"{pic_crosshairp1.Image.Width}x{pic_crosshairp1.Image.Height}";
						int tailleImg = pic_crosshairp1.Image.Width > pic_crosshairp1.Image.Height ? pic_crosshairp1.Image.Width : pic_crosshairp1.Image.Height;
						if (tailleImg >= 10)
						{
							trk_crosshairp1Size.Minimum = 10;
							trk_crosshairp1Size.Visible = true;
							btn_crosshairp1Size.Visible = true;
							trk_crosshairp1Size.Maximum = tailleImg;
							trk_crosshairp1Size.Value = trk_crosshairp1Size.Maximum;
							lbl_crosshairp1ReSize.Visible = true;
							lbl_crosshairp1ReSize.Text = tailleImg.ToString();
						}
						else
						{
							lbl_crosshairp1ReSize.Visible = trk_crosshairp1Size.Visible = btn_crosshairp1Size.Visible = false;
						}


					}
					if (File.Exists(Path.Combine(crosshairDir, "p2.png")))
					{
						pic_crosshairp2.Image = LoadImage(Path.Combine(crosshairDir, "p2.png"));
						lbl_crosshairp2Size.Text = $"{pic_crosshairp2.Image.Width}x{pic_crosshairp2.Image.Height}";
						int tailleImg = pic_crosshairp2.Image.Width > pic_crosshairp2.Image.Height ? pic_crosshairp2.Image.Width : pic_crosshairp2.Image.Height;
						if (tailleImg >= 10)
						{
							trk_crosshairp2Size.Minimum = 10;
							trk_crosshairp2Size.Visible = true;
							btn_crosshairp2Size.Visible = true;
							trk_crosshairp2Size.Maximum = tailleImg;
							trk_crosshairp2Size.Value = trk_crosshairp2Size.Maximum;
							lbl_crosshairp2ReSize.Visible = true;
							lbl_crosshairp2ReSize.Text = tailleImg.ToString();
						}
						else
						{
							lbl_crosshairp2ReSize.Visible = trk_crosshairp2Size.Visible = btn_crosshairp2Size.Visible = false;
						}
					}
				}
				else
				{
					groupCrosshair.Visible = false;
				}


			}
			catch { }

			/*
			if (grp_link.Enabled)
			{
				if (!Utils.IsEligibleHardLink(_linkTargetFolder))
				{
					grp_link.Enabled = false;
					//chk_linkfiles.Visible = false;
					//lbl_linkNumber.Text = "Target folder is not eligible for HardLink";
				}
				else
				{
					if (Directory.Exists(_linkSourceFolder))
					{
						if (!Utils.IsEligibleHardLink(_linkSourceFolder, _linkTargetFolder))
						{
							//lbl_linkNumber.Text = "Source folder is not eligible for HardLink";
						}
						else
						{
							try
							{
								int count = Directory.EnumerateFiles(_linkSourceFolder, "*", SearchOption.AllDirectories).Count();
								//lbl_linkNumber.Text = "Number of files = " + count;
							}
							catch
							{
								//lbl_linkNumber.Text = "Number of files = ??? (error reading directory)";
							}
						}
					}
					else
					{
						if (!Utils.IsEligibleHardLink(_linkSourceFolder, _linkTargetFolder, false))
						{
							///lbl_linkNumber.Text = "Target folder does not exist and is not eligible for HardLink";
						}
						else
						{
							//lbl_linkNumber.Text = "Number of files = 0 (Source dir does not exist)";
						}
					}

				}
			}
			else
			{
				if (linkTypeExe)
				{
					//lbl_linkNumber.Text = "This game does not use BudgieLoader";
				}
			}

			if (grp_linkExe.Enabled)
			{
				if (!Utils.IsEligibleHardLink(_linkTargetFolderExe))
				{
					//if (btn_selectLinkFolderExe.Enabled) btn_selectLinkFolderExe.Enabled = false;
					grp_linkExe.Enabled = false;
					//chk_linkfilesExe.Visible = false;
					//lbl_linkNumberExe.Text = "Target folder is not eligible for HardLink";
					btn_link_openExe.Enabled = false;
				}
				else
				{
					if (Directory.Exists(_linkSourceFolderExe))
					{
						if (!Utils.IsEligibleHardLink(_linkSourceFolderExe, _linkTargetFolderExe))
						{
							//lbl_linkNumberExe.Text = "Source folder is not eligible for HardLink";
							btn_link_openExe.Enabled = false;
						}
						else
						{
							try
							{
								int count = Directory.EnumerateFiles(_linkSourceFolderExe, "*", SearchOption.AllDirectories).Count();
								//lbl_linkNumberExe.Text = "Number of files = " + count;
							}
							catch
							{
								//lbl_linkNumberExe.Text = "Number of files = ??? (error reading directory)";
							}
						}
					}
					else
					{
						if (!Utils.IsEligibleHardLink(_linkSourceFolderExe, _linkTargetFolderExe, false))
						{
							//lbl_linkNumberExe.Text = "Target folder does not exist and is not eligible for HardLink";
							btn_link_openExe.Enabled = false;
						}
						else
						{
							//lbl_linkNumberExe.Text = "Number of files = 0 (Source dir does not exist)";
						}
					}

				}
			}
			*/

		}

		private void GameOptionsSimple_Load(object sender, EventArgs e)
		{
			cmb_favorJoystick.SelectedIndex = gameSettings.favorJoystick;
			cmb_resolution.SelectedIndex = gameSettings.gpuResolution;
			cmb_displayMode.SelectedIndex = gameSettings.displayMode;

			cmb_patchReshade.SelectedIndex = gameSettings.patchReshade;
			cmb_patchFFB.SelectedIndex = gameSettings.patchFFB;

			chk_group_StoozZone_Wheel.Location = new Point(chk_group_StoozZone_Wheel.Location.X, chk_group_StoozZone_Wheel.Location.Y + 15);
			chk_group_StoozZone_Gamepad.Location = new Point(chk_group_StoozZone_Gamepad.Location.X, chk_group_StoozZone_Gamepad.Location.Y + 15);
			chk_group_StoozZone_Hotas.Location = new Point(chk_group_StoozZone_Hotas.Location.X, chk_group_StoozZone_Hotas.Location.Y + 15);
			chk_group_monitorDisposition.Location = new Point(chk_group_monitorDisposition.Location.X, chk_group_monitorDisposition.Location.Y + 15);

			LinkLoad();

			radio_useCustomStooz_Gamepad.Checked = gameSettings.gamepadStooz;
			radio_useCustomStooz_Wheel.Checked = gameSettings.wheelStooz;
			radio_useCustomStooz_Hotas.Checked = gameSettings.hotasStooz;
			radio_useDefaultStooze_Gamepad.Checked = !radio_useCustomStooz_Gamepad.Checked;
			radio_useDefaultStooze_Wheel.Checked = !radio_useCustomStooz_Wheel.Checked;
			radio_useDefaultStooze_Hotas.Checked = !radio_useCustomStooz_Hotas.Checked;

			chk_group_monitorDisposition.Checked = gameSettings.UseGlobalDisposition;
			chk_group_StoozZone_Gamepad.Checked = gameSettings.UseGlobalStoozZoneGamepad;
			chk_group_StoozZone_Wheel.Checked = gameSettings.UseGlobalStoozZoneWheel;
			chk_group_StoozZone_Hotas.Checked = gameSettings.UseGlobalStoozZoneHotas;
			chk_enableStoozZone_Gamepad.Checked = gameSettings.enableStoozZone_Gamepad;
			chk_enableStoozZone_Wheel.Checked = gameSettings.enableStoozZone_Wheel;
			chk_enableStoozZone_Hotas.Checked = gameSettings.enableStoozZone_Hotas;
			trk_useCustomStooz_Gamepad.Value = gameSettings.valueStooz_Gamepad;
			trk_useCustomStooz_Wheel.Value = gameSettings.valueStooz_Wheel;
			trk_useCustomStooz_Hotas.Value = gameSettings.valueStooz_Hotas;
			chk_enableGearChange.Checked = gameSettings.EnableGearChange;
			txt_monitorswitch.Text = gameSettings.Disposition == "" ? "<none>" : gameSettings.Disposition;

			Reload();

			cmb_gunA_recoil.SelectedIndex = gameSettings.gunA_recoil;
			cmb_gunA_sindenPump.SelectedIndex = gameSettings.gunA_pump;
			cmb_gunA_Crosshair.SelectedIndex = gameSettings.gunA_crosshair;
			cmb_gunA_UseVjoy.SelectedIndex = gameSettings.gunA_useVjoy;
			cmb_gunA_OffscreenReload.SelectedIndex = gameSettings.gunA_OffscreenReload;

			cmb_gunB_recoil.SelectedIndex = gameSettings.gunB_recoil;
			cmb_gunB_sindenPump.SelectedIndex = gameSettings.gunB_pump;
			cmb_gunB_Crosshair.SelectedIndex = gameSettings.gunB_crosshair;
			cmb_gunB_UseVjoy.SelectedIndex = gameSettings.gunB_useVjoy;
			cmb_gunB_OffscreenReload.SelectedIndex = gameSettings.gunB_OffscreenReload;

			cmb_gunA_sindenRecoil1.SelectedIndex = gameSettings.gunA_sindenRecoil1;
			cmb_gunA_sindenRecoil2.SelectedIndex = gameSettings.gunA_sindenRecoil2;
			cmb_gunA_sindenRecoil3.SelectedIndex = gameSettings.gunA_sindenRecoil3;
			cmb_gunB_sindenRecoil1.SelectedIndex = gameSettings.gunB_sindenRecoil1;
			cmb_gunB_sindenRecoil2.SelectedIndex = gameSettings.gunB_sindenRecoil2;
			cmb_gunB_sindenRecoil3.SelectedIndex = gameSettings.gunB_sindenRecoil3;

			chk_runRivaTuner.Checked = gameSettings.runRivaTuner;

			cmb_forcevsync.SelectedIndex = gameSettings.forceVsync;
			cmb_performance.SelectedIndex = gameSettings.performanceProfile;
			cmb_useCrt.SelectedIndex = gameSettings.useCrt;
			cmb_useBezel.SelectedIndex = gameSettings.useBezel;
			cmb_keepAspectRatio.SelectedIndex = gameSettings.keepAspectRatio;
			SetBezelCmb();
			SetCrtCmb();
			SetKeepRatioCmb();

		}

		public void SetKeepRatioCmb()
		{
			bool enabled = true;
			if (cmb_patchReshade.SelectedIndex == 0)
			{
				if (!ConfigurationManager.MainConfig.patchReshade) enabled = false;
			}
			if (cmb_patchReshade.SelectedIndex == 2) enabled = false;

			cmb_keepAspectRatio.Enabled = enabled;
		}
		public void SetBezelCmb()
		{
			bool enabled = true;
			if (cmb_patchReshade.SelectedIndex == 0)
			{
				if (!ConfigurationManager.MainConfig.patchReshade) enabled = false;
			}
			if (cmb_patchReshade.SelectedIndex == 2) enabled = false;

			cmb_useBezel.Enabled = enabled;
		}

		public void SetCrtCmb()
		{
			bool enabled = true;
			if (cmb_patchReshade.SelectedIndex == 0)
			{
				if (!ConfigurationManager.MainConfig.patchReshade) enabled = false;
			}
			if (cmb_patchReshade.SelectedIndex == 2) enabled = false;

			if (cmb_performance.SelectedIndex == 0)
			{
				if (ConfigurationManager.MainConfig.performanceProfile == 1) enabled = false;
			}
			if (cmb_performance.SelectedIndex == 2) enabled = false;
			cmb_useCrt.Enabled = enabled;
		}

		private void Reload()
		{
			grp_monitorDisposition.Enabled = !chk_group_monitorDisposition.Checked;
			grp_StoozZone_Gamepad.Enabled = !chk_group_StoozZone_Gamepad.Checked;
			grp_StoozZone_Wheel.Enabled = !chk_group_StoozZone_Wheel.Checked;
			grp_StoozZone_Hotas.Enabled = !chk_group_StoozZone_Hotas.Checked;
		}


		private void btn_link_open_Click(object sender, EventArgs e)
		{

			if (!string.IsNullOrEmpty(_linkSourceFolder))
			{
				bool needUpdate = false;
				if (!Directory.Exists(_linkSourceFolder) && Directory.Exists(Directory.GetParent(_linkSourceFolder).FullName))
				{
					DialogResult dialogResult = MessageBox.Show($"Do you want to create {_linkSourceFolder} ?", "Create Directory ?", MessageBoxButtons.YesNo);
					if (dialogResult == DialogResult.Yes)
					{
						Directory.CreateDirectory(_linkSourceFolder);
						Thread.Sleep(100);
						needUpdate = true;
					}
				}
				if (Directory.Exists(_linkSourceFolder))
				{
					ProcessStartInfo startInfo = new ProcessStartInfo
					{
						Arguments = _linkSourceFolder,
						FileName = "explorer.exe"
					};
					Process.Start(startInfo);
				}
				else
				{
					MessageBox.Show($"Directory {_linkSourceFolder} does not exist");
				}
				if (needUpdate) LinkLoad();
			}
		}

		private void chk_group_monitorDisposition_CheckedChanged(object sender, EventArgs e)
		{
			Reload();
		}

		private void chk_group_StoozZone_Wheel_CheckedChanged(object sender, EventArgs e)
		{
			Reload();
		}

		private void chk_group_StoozZone_Gamepad_CheckedChanged(object sender, EventArgs e)
		{
			Reload();
		}

		private void chk_group_StoozZone_Hotas_CheckedChanged(object sender, EventArgs e)
		{
			Reload();
		}

		private void btn_Save_Click(object sender, EventArgs e)
		{
			gameSettings.favorJoystick = cmb_favorJoystick.SelectedIndex;
			gameSettings.gpuResolution = cmb_resolution.SelectedIndex;
			gameSettings.displayMode = cmb_displayMode.SelectedIndex;

			gameSettings.patchReshade = cmb_patchReshade.SelectedIndex;
			gameSettings.patchFFB = cmb_patchFFB.SelectedIndex;

			gameSettings.gamepadStooz = radio_useCustomStooz_Gamepad.Checked;
			gameSettings.wheelStooz = radio_useCustomStooz_Wheel.Checked;
			gameSettings.hotasStooz = radio_useCustomStooz_Hotas.Checked;

			gameSettings.UseGlobalDisposition = chk_group_monitorDisposition.Checked;
			gameSettings.UseGlobalStoozZoneGamepad = chk_group_StoozZone_Gamepad.Checked;
			gameSettings.UseGlobalStoozZoneWheel = chk_group_StoozZone_Wheel.Checked;
			gameSettings.UseGlobalStoozZoneHotas = chk_group_StoozZone_Hotas.Checked;

			gameSettings.enableStoozZone_Gamepad = chk_enableStoozZone_Gamepad.Checked;
			gameSettings.enableStoozZone_Wheel = chk_enableStoozZone_Wheel.Checked;
			gameSettings.enableStoozZone_Hotas = chk_enableStoozZone_Hotas.Checked;
			gameSettings.valueStooz_Gamepad = trk_useCustomStooz_Gamepad.Value;
			gameSettings.valueStooz_Wheel = trk_useCustomStooz_Wheel.Value;
			gameSettings.valueStooz_Hotas = trk_useCustomStooz_Hotas.Value;

			gameSettings.EnableGearChange = chk_enableGearChange.Checked;
			gameSettings.Disposition = txt_monitorswitch.Text.Trim();

			//gameSettings.CustomTpExe = txt_customTp.Text.Trim();

			gameSettings.gunA_recoil = cmb_gunA_recoil.SelectedIndex;
			gameSettings.gunA_pump = cmb_gunA_sindenPump.SelectedIndex;
			gameSettings.gunA_crosshair = cmb_gunA_Crosshair.SelectedIndex;
			gameSettings.gunA_useVjoy = cmb_gunA_UseVjoy.SelectedIndex;
			gameSettings.gunA_OffscreenReload = cmb_gunA_OffscreenReload.SelectedIndex;

			gameSettings.gunB_recoil = cmb_gunB_recoil.SelectedIndex;
			gameSettings.gunB_pump = cmb_gunB_sindenPump.SelectedIndex;
			gameSettings.gunB_crosshair = cmb_gunB_Crosshair.SelectedIndex;
			gameSettings.gunB_useVjoy = cmb_gunB_UseVjoy.SelectedIndex;
			gameSettings.gunB_OffscreenReload = cmb_gunB_OffscreenReload.SelectedIndex;

			gameSettings.gunA_sindenRecoil1 = cmb_gunA_sindenRecoil1.SelectedIndex;
			gameSettings.gunA_sindenRecoil2 = cmb_gunA_sindenRecoil2.SelectedIndex;
			gameSettings.gunA_sindenRecoil3 = cmb_gunA_sindenRecoil3.SelectedIndex;
			gameSettings.gunB_sindenRecoil1 = cmb_gunB_sindenRecoil1.SelectedIndex;
			gameSettings.gunB_sindenRecoil2 = cmb_gunB_sindenRecoil2.SelectedIndex;
			gameSettings.gunB_sindenRecoil3 = cmb_gunB_sindenRecoil3.SelectedIndex;

			gameSettings.runRivaTuner = chk_runRivaTuner.Checked;

			gameSettings.forceVsync = cmb_forcevsync.SelectedIndex;
			gameSettings.performanceProfile = cmb_performance.SelectedIndex;
			if (cmb_useCrt.Enabled) gameSettings.useCrt = cmb_useCrt.SelectedIndex;
			if (cmb_useBezel.Enabled) gameSettings.useBezel = cmb_useBezel.SelectedIndex;
			if (cmb_keepAspectRatio.Enabled) gameSettings.keepAspectRatio = cmb_keepAspectRatio.SelectedIndex;

			gameSettings.Save(PerGameConfigFile);
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void btn_editMonitorSwitch_Click(object sender, EventArgs e)
		{
			var frm = new MonitorDispositionConfig();
			var result = frm.ShowDialog();

			if (result == DialogResult.OK)
			{
				txt_monitorswitch.Text = frm.result;
			}
		}

		private void trk_useCustomStooz_Gamepad_Scroll(object sender, EventArgs e)
		{
			lbl_useCustomStooz_Gamepad.Text = trk_useCustomStooz_Gamepad.Value.ToString() + "%";
		}

		private void trk_useCustomStooz_Wheel_Scroll(object sender, EventArgs e)
		{
			lbl_useCustomStooz_Wheel.Text = trk_useCustomStooz_Wheel.Value.ToString() + "%";
		}

		private void trk_useCustomStooz_Hotas_Scroll(object sender, EventArgs e)
		{
			lbl_useCustomStooz_Hotas.Text = trk_useCustomStooz_Hotas.Value.ToString() + "%";
		}

		private void radio_useDefaultStooze_Gamepad_CheckedChanged(object sender, EventArgs e)
		{
			updateStooz();
		}

		private void radio_useCustomStooz_Gamepad_CheckedChanged(object sender, EventArgs e)
		{
			updateStooz();
		}

		private void radio_useDefaultStooze_Wheel_CheckedChanged(object sender, EventArgs e)
		{
			updateStooz();
		}

		private void radio_useCustomStooz_Wheel_CheckedChanged(object sender, EventArgs e)
		{
			updateStooz();
		}

		private void radio_useDefaultStooze_Hotas_CheckedChanged(object sender, EventArgs e)
		{
			updateStooz();
		}

		private void radio_useCustomStooz_Hotas_CheckedChanged(object sender, EventArgs e)
		{
			updateStooz();
		}

		private void updateStooz()
		{
			chk_enableStoozZone_Gamepad.Enabled = radio_useCustomStooz_Gamepad.Checked;
			trk_useCustomStooz_Gamepad.Enabled = radio_useCustomStooz_Gamepad.Checked;
			chk_enableStoozZone_Gamepad.Visible = radio_useCustomStooz_Gamepad.Checked;
			trk_useCustomStooz_Gamepad.Visible = radio_useCustomStooz_Gamepad.Checked;
			lbl_useCustomStooz_Gamepad.Text = trk_useCustomStooz_Gamepad.Value.ToString() + "%";
			lbl_useCustomStooz_Gamepad.Visible = radio_useCustomStooz_Gamepad.Checked;

			chk_enableStoozZone_Wheel.Enabled = radio_useCustomStooz_Wheel.Checked;
			trk_useCustomStooz_Wheel.Enabled = radio_useCustomStooz_Wheel.Checked;
			chk_enableStoozZone_Wheel.Visible = radio_useCustomStooz_Wheel.Checked;
			trk_useCustomStooz_Wheel.Visible = radio_useCustomStooz_Wheel.Checked;
			lbl_useCustomStooz_Wheel.Text = trk_useCustomStooz_Wheel.Value.ToString() + "%";
			lbl_useCustomStooz_Wheel.Visible = radio_useCustomStooz_Wheel.Checked;

			chk_enableStoozZone_Hotas.Enabled = radio_useCustomStooz_Hotas.Checked;
			trk_useCustomStooz_Hotas.Enabled = radio_useCustomStooz_Hotas.Checked;
			chk_enableStoozZone_Hotas.Visible = radio_useCustomStooz_Hotas.Checked;
			trk_useCustomStooz_Hotas.Visible = radio_useCustomStooz_Hotas.Checked;
			lbl_useCustomStooz_Hotas.Text = trk_useCustomStooz_Hotas.Value.ToString() + "%";
			lbl_useCustomStooz_Hotas.Visible = radio_useCustomStooz_Hotas.Checked;
		}

		private void btn_link_openExe_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(_linkSourceFolderExe))
			{
				bool needupdate = false;
				if (!Directory.Exists(_linkSourceFolderExe) && Directory.Exists(Directory.GetParent(_linkSourceFolderExe).FullName))
				{
					DialogResult dialogResult = MessageBox.Show($"Do you want to create {_linkSourceFolderExe} ?", "Create Directory ?", MessageBoxButtons.YesNo);
					if (dialogResult == DialogResult.Yes)
					{
						Directory.CreateDirectory(_linkSourceFolderExe);
						Thread.Sleep(100);
						needupdate = true;
					}
				}
				if (Directory.Exists(_linkSourceFolderExe))
				{
					ProcessStartInfo startInfo = new ProcessStartInfo
					{
						Arguments = _linkSourceFolderExe,
						FileName = "explorer.exe"
					};
					Process.Start(startInfo);
				}
				else
				{
					MessageBox.Show($"Directory {_linkSourceFolderExe} does not exist");
				}
				if (needupdate) LinkLoad();
			}
		}

		private void btn_linkTarget_openExe_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(_linkTargetFolderExe))
			{
				if (Directory.Exists(_linkTargetFolderExe))
				{
					ProcessStartInfo startInfo = new ProcessStartInfo
					{
						Arguments = _linkTargetFolderExe,
						FileName = "explorer.exe"
					};
					Process.Start(startInfo);
				}
				else
				{
					MessageBox.Show($"Directory {_linkTargetFolderExe} does not exist");
				}
			}
		}

		private void btn_crosshairp1_Click(object sender, EventArgs e)
		{
			string crosshairDir = "";
			if (Directory.Exists(Path.Combine(_linkSourceFolderExe, "[!!crosshair!!]", "[!crosshair_gun1_and_gun2!]"))) crosshairDir = Directory.GetParent(Path.Combine(_linkSourceFolderExe, "[!!crosshair!!]", "[!crosshair_gun1_and_gun2!]")).FullName;
			if (Directory.Exists(Path.Combine(_linkSourceFolderExe, "[!crosshair_gun1_and_gun2!]"))) crosshairDir = Directory.GetParent(Path.Combine(_linkSourceFolderExe, "[!!crosshair!!]", "[!crosshair_gun1_and_gun2!]")).FullName;

			var frm = new crosshairSelect();
			var result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{

				if (Directory.Exists(Path.Combine(crosshairDir, "[!crosshair_gun1_and_gun2!]")))
				{
					try
					{
						File.Copy(frm.selectedImagePath, Path.Combine(crosshairDir, "[!crosshair_gun1_and_gun2!]", "p1.png"), true);
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message);
					}
				}
				if (Directory.Exists(Path.Combine(crosshairDir, "[!crosshair_gun1_only!]")))
				{
					try
					{
						File.Copy(frm.selectedImagePath, Path.Combine(crosshairDir, "[!crosshair_gun1_only!]", "p1.png"), true);
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message);
					}
				}
				pic_crosshairp1.Image = System.Drawing.Image.FromFile(frm.selectedImagePath);
				lbl_crosshairp1Size.Text = $"{pic_crosshairp1.Image.Width}x{pic_crosshairp1.Image.Height}";
				int tailleImg = pic_crosshairp1.Image.Width > pic_crosshairp1.Image.Height ? pic_crosshairp1.Image.Width : pic_crosshairp1.Image.Height;
				if (tailleImg >= 10)
				{
					trk_crosshairp1Size.Minimum = 10;
					trk_crosshairp1Size.Visible = true;
					btn_crosshairp1Size.Visible = true;
					trk_crosshairp1Size.Maximum = tailleImg;
					trk_crosshairp1Size.Value = trk_crosshairp1Size.Maximum;
					lbl_crosshairp1ReSize.Text = tailleImg.ToString();
					lbl_crosshairp1ReSize.Visible = true;
				}
				else
				{
					lbl_crosshairp1ReSize.Visible = trk_crosshairp1Size.Visible = btn_crosshairp1Size.Visible = false;
				}
			}
		}

		private System.Drawing.Image LoadImage(string path)
		{
			using (var stream = new MemoryStream(File.ReadAllBytes(path)))
			{
				return System.Drawing.Image.FromStream(stream);
			}
		}

		private void btn_crosshairp2_Click(object sender, EventArgs e)
		{
			string crosshairDir = "";
			if (Directory.Exists(Path.Combine(_linkSourceFolderExe, "[!!crosshair!!]", "[!crosshair_gun1_and_gun2!]"))) crosshairDir = Directory.GetParent(Path.Combine(_linkSourceFolderExe, "[!!crosshair!!]", "[!crosshair_gun1_and_gun2!]")).FullName;
			if (Directory.Exists(Path.Combine(_linkSourceFolderExe, "[!crosshair_gun1_and_gun2!]"))) crosshairDir = Directory.GetParent(Path.Combine(_linkSourceFolderExe, "[!!crosshair!!]", "[!crosshair_gun1_and_gun2!]")).FullName;

			var frm = new crosshairSelect();
			var result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{

				if (Directory.Exists(Path.Combine(crosshairDir, "[!crosshair_gun1_and_gun2!]")))
				{
					try
					{
						File.Copy(frm.selectedImagePath, Path.Combine(crosshairDir, "[!crosshair_gun1_and_gun2!]", "p2.png"), true);
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message);
					}
				}
				if (Directory.Exists(Path.Combine(crosshairDir, "[!crosshair_gun2_only!]")))
				{
					try
					{
						File.Copy(frm.selectedImagePath, Path.Combine(crosshairDir, "[!crosshair_gun2_only!]", "p2.png"), true);
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message);
					}
				}
				pic_crosshairp2.Image = System.Drawing.Image.FromFile(frm.selectedImagePath);
				lbl_crosshairp2Size.Text = $"{pic_crosshairp2.Image.Width}x{pic_crosshairp2.Image.Height}";
				int tailleImg = pic_crosshairp2.Image.Width > pic_crosshairp2.Image.Height ? pic_crosshairp2.Image.Width : pic_crosshairp2.Image.Height;
				if (tailleImg >= 10)
				{
					trk_crosshairp2Size.Minimum = 10;
					trk_crosshairp2Size.Visible = true;
					btn_crosshairp2Size.Visible = true;
					trk_crosshairp2Size.Maximum = tailleImg;
					trk_crosshairp2Size.Value = trk_crosshairp2Size.Maximum;
					lbl_crosshairp2ReSize.Visible = true;
					lbl_crosshairp2ReSize.Text = tailleImg.ToString();
				}
				else
				{
					lbl_crosshairp2ReSize.Visible = trk_crosshairp2Size.Visible = btn_crosshairp2Size.Visible = false;
				}
			}
		}

		private void trk_crosshairp1Size_Scroll(object sender, EventArgs e)
		{
			lbl_crosshairp1ReSize.Text = trk_crosshairp1Size.Value.ToString();
		}

		private void btn_crosshairp1Size_Click(object sender, EventArgs e)
		{
			System.Drawing.Image image = pic_crosshairp1.Image;
			int newSize = trk_crosshairp1Size.Value;
			float aspectRatio = (float)image.Width / image.Height;

			int newWidth;
			int newHeight;

			// Déterminer les nouvelles dimensions basées sur la taille la plus grande (largeur ou hauteur)
			if (image.Width > image.Height)
			{
				newWidth = newSize;
				newHeight = (int)(newSize / aspectRatio);
			}
			else
			{
				newHeight = newSize;
				newWidth = (int)(newSize * aspectRatio);
			}

			Bitmap newImage = new Bitmap(newWidth, newHeight);
			using (Graphics graphics = Graphics.FromImage(newImage))
			{
				graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
				graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

				graphics.DrawImage(image, 0, 0, newWidth, newHeight);
			}
			string crosshairDir = "";
			if (Directory.Exists(Path.Combine(_linkSourceFolderExe, "[!!crosshair!!]", "[!crosshair_gun1_and_gun2!]"))) crosshairDir = Directory.GetParent(Path.Combine(_linkSourceFolderExe, "[!!crosshair!!]", "[!crosshair_gun1_and_gun2!]")).FullName;
			if (Directory.Exists(Path.Combine(_linkSourceFolderExe, "[!crosshair_gun1_and_gun2!]"))) crosshairDir = Directory.GetParent(Path.Combine(_linkSourceFolderExe, "[!!crosshair!!]", "[!crosshair_gun1_and_gun2!]")).FullName;

			{

				if (Directory.Exists(Path.Combine(crosshairDir, "[!crosshair_gun1_and_gun2!]")))
				{
					newImage.Save(Path.Combine(crosshairDir, "[!crosshair_gun1_and_gun2!]", "p1.png"), System.Drawing.Imaging.ImageFormat.Png);
				}
				if (Directory.Exists(Path.Combine(crosshairDir, "[!crosshair_gun1_only!]")))
				{
					newImage.Save(Path.Combine(crosshairDir, "[!crosshair_gun1_only!]", "p1.png"), System.Drawing.Imaging.ImageFormat.Png);
				}
				pic_crosshairp1.Image = newImage;
				lbl_crosshairp1Size.Text = $"{pic_crosshairp1.Image.Width}x{pic_crosshairp1.Image.Height}";
				int tailleImg = pic_crosshairp1.Image.Width > pic_crosshairp1.Image.Height ? pic_crosshairp1.Image.Width : pic_crosshairp1.Image.Height;
				if (tailleImg >= 10)
				{
					trk_crosshairp1Size.Minimum = 10;
					trk_crosshairp1Size.Visible = true;
					btn_crosshairp1Size.Visible = true;
					trk_crosshairp1Size.Maximum = tailleImg;
					trk_crosshairp1Size.Value = trk_crosshairp1Size.Maximum;
					lbl_crosshairp1ReSize.Text = tailleImg.ToString();
					lbl_crosshairp1ReSize.Visible = true;
				}
				else
				{
					lbl_crosshairp1ReSize.Visible = trk_crosshairp1Size.Visible = btn_crosshairp1Size.Visible = false;
				}
			}
		}

		private void btn_crosshairp2Size_Click(object sender, EventArgs e)
		{
			System.Drawing.Image image = pic_crosshairp2.Image;
			int newSize = trk_crosshairp2Size.Value;
			float aspectRatio = (float)image.Width / image.Height;

			int newWidth;
			int newHeight;

			// Déterminer les nouvelles dimensions basées sur la taille la plus grande (largeur ou hauteur)
			if (image.Width > image.Height)
			{
				newWidth = newSize;
				newHeight = (int)(newSize / aspectRatio);
			}
			else
			{
				newHeight = newSize;
				newWidth = (int)(newSize * aspectRatio);
			}

			Bitmap newImage = new Bitmap(newWidth, newHeight);
			using (Graphics graphics = Graphics.FromImage(newImage))
			{
				graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
				graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

				graphics.DrawImage(image, 0, 0, newWidth, newHeight);
			}
			string crosshairDir = "";
			if (Directory.Exists(Path.Combine(_linkSourceFolderExe, "[!!crosshair!!]", "[!crosshair_gun1_and_gun2!]"))) crosshairDir = Directory.GetParent(Path.Combine(_linkSourceFolderExe, "[!!crosshair!!]", "[!crosshair_gun1_and_gun2!]")).FullName;
			if (Directory.Exists(Path.Combine(_linkSourceFolderExe, "[!crosshair_gun1_and_gun2!]"))) crosshairDir = Directory.GetParent(Path.Combine(_linkSourceFolderExe, "[!!crosshair!!]", "[!crosshair_gun1_and_gun2!]")).FullName;

			{

				if (Directory.Exists(Path.Combine(crosshairDir, "[!crosshair_gun1_and_gun2!]")))
				{
					newImage.Save(Path.Combine(crosshairDir, "[!crosshair_gun1_and_gun2!]", "p2.png"), System.Drawing.Imaging.ImageFormat.Png);
				}
				if (Directory.Exists(Path.Combine(crosshairDir, "[!crosshair_gun2_only!]")))
				{
					newImage.Save(Path.Combine(crosshairDir, "[!crosshair_gun2_only!]", "p2.png"), System.Drawing.Imaging.ImageFormat.Png);
				}
				pic_crosshairp2.Image = newImage;
				lbl_crosshairp2Size.Text = $"{pic_crosshairp2.Image.Width}x{pic_crosshairp2.Image.Height}";
				int tailleImg = pic_crosshairp2.Image.Width > pic_crosshairp2.Image.Height ? pic_crosshairp2.Image.Width : pic_crosshairp2.Image.Height;
				if (tailleImg >= 10)
				{
					trk_crosshairp2Size.Minimum = 10;
					trk_crosshairp2Size.Visible = true;
					btn_crosshairp2Size.Visible = true;
					trk_crosshairp2Size.Maximum = tailleImg;
					trk_crosshairp2Size.Value = trk_crosshairp2Size.Maximum;
					lbl_crosshairp2ReSize.Text = tailleImg.ToString();
					lbl_crosshairp2ReSize.Visible = true;
				}
				else
				{
					lbl_crosshairp2ReSize.Visible = trk_crosshairp2Size.Visible = btn_crosshairp2Size.Visible = false;
				}
			}
		}

		private void trk_crosshairp2Size_Scroll(object sender, EventArgs e)
		{
			lbl_crosshairp2ReSize.Text = trk_crosshairp2Size.Value.ToString();
		}

		private void txt_linkFrom_TextChanged(object sender, EventArgs e)
		{

		}

		private void grp_linkExe_Paint(object sender, PaintEventArgs e)
		{

		}

		private void kryptonLabel1_Click(object sender, EventArgs e)
		{

		}

		private void cmb_patchReshade_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetBezelCmb();
			SetCrtCmb();
			SetKeepRatioCmb();
		}

		private void cmb_performance_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetCrtCmb();
		}

		private void cmb_resolution_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
		{

		}
	}
}
