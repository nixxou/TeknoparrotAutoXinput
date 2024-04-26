using Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using TeknoParrotUi.Common;
using XJoy;

namespace TeknoparrotAutoXinput
{
	public partial class GameOptions : KryptonForm
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

		GameSettings gameSettings = new GameSettings();
		public GameOptions(Game gameData)
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
			foreach (var type in typeConfig)
			{
				var configPath = Path.Combine(basePath, "config", Path.GetFileNameWithoutExtension(GameData.UserConfigFile) + "." + type + ".txt");
				if (File.Exists(configPath))
				{
					if (type == "wheel")
					{
						tabControl1.SelectedIndex = 1;
					}
					if (type == "hotas")
					{
						tabControl1.SelectedIndex = 2;
					}
					if (type == "lightgun")
					{
						tabControl1.SelectedIndex = 3;
					}
				}
			}
			

		}

		private void LinkLoad()
		{
			grp_link.Enabled = false;
			btn_link_open.Enabled = true;
			txt_linkFrom.Text = "";
			txt_linkTo.Text = "";
			lbl_linkNumber.Text = "";

			grp_linkExe.Enabled = false;
			txt_linkFromExe.Text = "";
			txt_linkToExe.Text = "";
			lbl_linkNumberExe.Text = "";

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
							txt_linkTo.Text = _elfldr2Folder;
							_linkTargetFolder = _elfldr2Folder;
						}
						if (emulatorTypeValue == "lindbergh")
						{
							//lbl_LinkTo.Text = "To : " + _lindberghFolder;
							txt_linkTo.Text = _lindberghFolder;
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
						lbl_linkNumberExe.Text = "You must define a default link folder in the general config first";
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

				/*
				if (linkTypeExe && executableGame != "" && Directory.Exists(executableGameDir))
				{
					_linkSourceFolder = "";
					_linkTargetFolder = executableGameDir;
					grp_link.Enabled = true;
					grp_link.Text = "Links Files before Execute (exe version)";
					_linkTargetFolder = executableGameDir;
					if (gameSettings.CustomPerGameLinkFolder != null && gameSettings.CustomPerGameLinkFolder != "")
					{
						string lastFolder = Path.GetFileName(gameSettings.CustomPerGameLinkFolder);
						if (lastFolder == Path.GetFileNameWithoutExtension(GameData.FileName))
						{
							_linkSourceFolder = gameSettings.CustomPerGameLinkFolder;
						}
					}
					else
					{
						if (!String.IsNullOrEmpty(ConfigurationManager.MainConfig.perGameLinkFolderExe))
						{
							_linkSourceFolder = Path.Combine(ConfigurationManager.MainConfig.perGameLinkFolderExe, Path.GetFileNameWithoutExtension(GameData.FileName));
						}

					}
					txt_linkFrom.Text = _linkSourceFolder;
					txt_linkTo.Text = _linkTargetFolder;
				}
				*/

				XmlNode requiresAdminNode = xmlDoc.SelectSingleNode("/GameProfile/RequiresAdmin");
				if (requiresAdminNode != null)
				{
					string requiresAdminValue = requiresAdminNode.InnerText.ToLower().Trim();
					if (requiresAdminValue == "true")
					{
						chk_runAsAdmin.Enabled = true;

					}
				}

			}
			catch { }

			if (grp_link.Enabled)
			{
				if (!Utils.IsEligibleHardLink(_linkTargetFolder))
				{
					grp_link.Enabled = false;
					chk_linkfiles.Visible = false;
					lbl_linkNumber.Text = "Target folder is not eligible for HardLink";
				}
				else
				{
					if (Directory.Exists(_linkSourceFolder))
					{
						if (!Utils.IsEligibleHardLink(_linkSourceFolder, _linkTargetFolder))
						{
							lbl_linkNumber.Text = "Source folder is not eligible for HardLink";
						}
						else
						{
							try
							{
								int count = Directory.EnumerateFiles(_linkSourceFolder, "*", SearchOption.AllDirectories).Count();
								lbl_linkNumber.Text = "Number of files = " + count;
							}
							catch
							{
								lbl_linkNumber.Text = "Number of files = ??? (error reading directory)";
							}
						}
					}
					else
					{
						if (!Utils.IsEligibleHardLink(_linkSourceFolder, _linkTargetFolder, false))
						{
							lbl_linkNumber.Text = "Target folder does not exist and is not eligible for HardLink";
						}
						else
						{
							lbl_linkNumber.Text = "Number of files = 0 (Source dir does not exist)";
						}
					}

				}
			}
			else
			{
				if (linkTypeExe)
				{
					lbl_linkNumber.Text = "This game does not use BudgieLoader";
				}
			}

			if (grp_linkExe.Enabled)
			{
				if (!Utils.IsEligibleHardLink(_linkTargetFolderExe))
				{
					if (btn_selectLinkFolderExe.Enabled) btn_selectLinkFolderExe.Enabled = false;
					grp_linkExe.Enabled = false;
					chk_linkfilesExe.Visible = false;
					lbl_linkNumberExe.Text = "Target folder is not eligible for HardLink";
					btn_link_openExe.Enabled = false;
				}
				else
				{
					if (Directory.Exists(_linkSourceFolderExe))
					{
						if (!Utils.IsEligibleHardLink(_linkSourceFolderExe, _linkTargetFolderExe))
						{
							lbl_linkNumberExe.Text = "Source folder is not eligible for HardLink";
							btn_link_openExe.Enabled = false;
						}
						else
						{
							try
							{
								int count = Directory.EnumerateFiles(_linkSourceFolderExe, "*", SearchOption.AllDirectories).Count();
								lbl_linkNumberExe.Text = "Number of files = " + count;
							}
							catch
							{
								lbl_linkNumberExe.Text = "Number of files = ??? (error reading directory)";
							}
						}
					}
					else
					{
						if (!Utils.IsEligibleHardLink(_linkSourceFolderExe, _linkTargetFolderExe, false))
						{
							lbl_linkNumberExe.Text = "Target folder does not exist and is not eligible for HardLink";
							btn_link_openExe.Enabled = false;
						}
						else
						{
							lbl_linkNumberExe.Text = "Number of files = 0 (Source dir does not exist)";
						}
					}

				}
			}

		}

		private void GameOptions_Load(object sender, EventArgs e)
		{
			chk_runAsAdmin.Enabled = false;
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
			chk_runAsAdmin.Checked = gameSettings.RunAsRoot;
			chk_enableStoozZone_Gamepad.Checked = gameSettings.enableStoozZone_Gamepad;
			chk_enableStoozZone_Wheel.Checked = gameSettings.enableStoozZone_Wheel;
			chk_enableStoozZone_Hotas.Checked = gameSettings.enableStoozZone_Hotas;
			trk_useCustomStooz_Gamepad.Value = gameSettings.valueStooz_Gamepad;
			trk_useCustomStooz_Wheel.Value = gameSettings.valueStooz_Wheel;
			trk_useCustomStooz_Hotas.Value = gameSettings.valueStooz_Hotas;
			txt_ahkafter.Text = gameSettings.AhkAfter;
			txt_ahkbefore.Text = gameSettings.AhkBefore;
			chk_linkfiles.Checked = gameSettings.EnableLink;
			chk_linkfilesExe.Checked = gameSettings.EnableLinkExe;
			chk_WaitForExitBefore.Checked = gameSettings.WaitForExitAhkBefore;
			chk_enableGearChange.Checked = gameSettings.EnableGearChange;
			txt_monitorswitch.Text = gameSettings.Disposition == "" ? "<none>" : gameSettings.Disposition;
			txt_customTp.Text = gameSettings.CustomTpExe;
			chk_reverseYAxis_Hotas.Checked = gameSettings.reverseYAxis_Hotas;
			Reload();

			cmb_vjoy.SelectedIndex = gameSettings.indexvjoy + 1;
			vJoyManager vJoyObj;
			vJoyObj = new vJoyManager();
			if (!vJoyObj.vJoyEnabled())
			{
				btn_vjoyconfig.Enabled = false;
				cmb_vjoy.Enabled = false;
			}

			cmb_gunA_recoil.SelectedIndex = gameSettings.gunA_recoil;
			cmb_gunA_sindenPump.SelectedIndex = gameSettings.gunA_pump;
			cmb_gunA_Crosshair.SelectedIndex = gameSettings.gunA_crosshair;
			cmb_gunA_4tiers.SelectedIndex = gameSettings.gunA_4tiers;
			cmb_gunA_UseVjoy.SelectedIndex = gameSettings.gunA_useVjoy;

			cmb_gunB_recoil.SelectedIndex = gameSettings.gunB_recoil;
			cmb_gunB_sindenPump.SelectedIndex = gameSettings.gunB_pump;
			cmb_gunB_Crosshair.SelectedIndex = gameSettings.gunB_crosshair;
			cmb_gunB_4tiers.SelectedIndex = gameSettings.gunB_4tiers;
			cmb_gunB_UseVjoy.SelectedIndex = gameSettings.gunB_useVjoy;

			chk_sindenextra.Checked = !gameSettings.gun_useExtraSinden;
			txt_sindenextra.Text = gameSettings.gun_ExtraSinden;

			chk_runRivaTuner.Checked = gameSettings.runRivaTuner;
		}

		private void Reload()
		{
			grp_monitorDisposition.Enabled = !chk_group_monitorDisposition.Checked;
			grp_StoozZone_Gamepad.Enabled = !chk_group_StoozZone_Gamepad.Checked;
			grp_StoozZone_Wheel.Enabled = !chk_group_StoozZone_Wheel.Checked;
			grp_StoozZone_Hotas.Enabled = !chk_group_StoozZone_Hotas.Checked;
		}

		private void kryptonRichTextBox2_TextChanged(object sender, EventArgs e)
		{

		}

		private void kryptonLabel2_Click(object sender, EventArgs e)
		{

		}

		private void kryptonButton1_Click(object sender, EventArgs e)
		{


		}

		private void groupBox1_Enter(object sender, EventArgs e)
		{

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
			string errorTxt = "";
			if (!string.IsNullOrEmpty(txt_ahkbefore.Text.Trim()))
			{
				if (!Utils.AHKSyntaxCheck(txt_ahkbefore.Text.Trim(), out errorTxt))
				{
					DialogResult dialogResult = MessageBox.Show($"AHK Syntax error : {errorTxt} \n Are you sure you want to save ?", "AHK Syntax error", MessageBoxButtons.YesNo);
					if (dialogResult == DialogResult.No)
					{
						return;
					}
				}
			}
			errorTxt = "";
			if (!string.IsNullOrEmpty(txt_ahkafter.Text.Trim()))
			{
				if (!Utils.AHKSyntaxCheck(txt_ahkafter.Text.Trim(), out errorTxt))
				{
					DialogResult dialogResult = MessageBox.Show($"AHK Syntax error : {errorTxt} \n Are you sure you want to save ?", "AHK Syntax error", MessageBoxButtons.YesNo);
					if (dialogResult == DialogResult.No)
					{
						return;
					}
				}
			}


			gameSettings.gamepadStooz = radio_useCustomStooz_Gamepad.Checked;
			gameSettings.wheelStooz = radio_useCustomStooz_Wheel.Checked;
			gameSettings.hotasStooz = radio_useCustomStooz_Hotas.Checked;

			gameSettings.UseGlobalDisposition = chk_group_monitorDisposition.Checked;
			gameSettings.UseGlobalStoozZoneGamepad = chk_group_StoozZone_Gamepad.Checked;
			gameSettings.UseGlobalStoozZoneWheel = chk_group_StoozZone_Wheel.Checked;
			gameSettings.UseGlobalStoozZoneHotas = chk_group_StoozZone_Hotas.Checked;
			gameSettings.RunAsRoot = chk_runAsAdmin.Checked;
			gameSettings.enableStoozZone_Gamepad = chk_enableStoozZone_Gamepad.Checked;
			gameSettings.enableStoozZone_Wheel = chk_enableStoozZone_Wheel.Checked;
			gameSettings.enableStoozZone_Hotas = chk_enableStoozZone_Hotas.Checked;
			gameSettings.valueStooz_Gamepad = trk_useCustomStooz_Gamepad.Value;
			gameSettings.valueStooz_Wheel = trk_useCustomStooz_Wheel.Value;
			gameSettings.valueStooz_Hotas = trk_useCustomStooz_Hotas.Value;
			gameSettings.AhkAfter = txt_ahkafter.Text.Trim();
			gameSettings.AhkBefore = txt_ahkbefore.Text.Trim();
			gameSettings.EnableLink = chk_linkfiles.Checked;
			gameSettings.EnableLinkExe = chk_linkfilesExe.Checked;
			gameSettings.WaitForExitAhkBefore = chk_WaitForExitBefore.Checked;
			gameSettings.EnableGearChange = chk_enableGearChange.Checked;
			gameSettings.Disposition = txt_monitorswitch.Text.Trim();

			gameSettings.reverseYAxis_Hotas = chk_reverseYAxis_Hotas.Checked;
			//gameSettings.CustomTpExe = txt_customTp.Text.Trim();

			gameSettings.gunA_recoil = cmb_gunA_recoil.SelectedIndex;
			gameSettings.gunA_pump = cmb_gunA_sindenPump.SelectedIndex;
			gameSettings.gunA_crosshair = cmb_gunA_Crosshair.SelectedIndex;
			gameSettings.gunA_4tiers = cmb_gunA_4tiers.SelectedIndex;
			gameSettings.gunA_useVjoy = cmb_gunA_UseVjoy.SelectedIndex;

			gameSettings.gunB_recoil = cmb_gunB_recoil.SelectedIndex;
			gameSettings.gunB_pump = cmb_gunB_sindenPump.SelectedIndex;
			gameSettings.gunB_crosshair = cmb_gunB_Crosshair.SelectedIndex;
			gameSettings.gunB_4tiers = cmb_gunB_4tiers.SelectedIndex;
			gameSettings.gunB_useVjoy = cmb_gunB_UseVjoy.SelectedIndex;

			gameSettings.gun_useExtraSinden = !chk_sindenextra.Checked;
			gameSettings.gun_ExtraSinden = txt_sindenextra.Text;



			gameSettings.runRivaTuner = chk_runRivaTuner.Checked;



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

		private void groupBox3_Enter(object sender, EventArgs e)
		{

		}

		private void btn_selectLinkFolder_Click(object sender, EventArgs e)
		{
			MessageBox.Show("You must select a directory that use the same Drive as your game folder.");
			using (var fbd = new FolderBrowserDialog())
			{
				DialogResult result = fbd.ShowDialog();

				if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
				{
					string selectedPath = fbd.SelectedPath;
					selectedPath = Path.GetFullPath(selectedPath);

					if (!Utils.IsEligibleHardLink(selectedPath, _linkTargetFolder))
					{
						MessageBox.Show("This folder is not eligible for HardLink");
						return;
					}

					string gameName = Path.GetFileNameWithoutExtension(GameData.FileName);
					string lastFolder = Path.GetFileName(selectedPath);
					if (lastFolder != gameName)
					{
						DialogResult dr = MessageBox.Show($"The last folder must be {gameName} do you want to create {Path.Combine(selectedPath, gameName)} ?",
					  "Create Link folder", MessageBoxButtons.YesNo);
						if (dr == DialogResult.Yes)
						{
							selectedPath = Path.Combine(selectedPath, gameName);
							if (!Directory.Exists(selectedPath)) Directory.CreateDirectory(selectedPath);
						}
						else return;

						//_linkSourceFolder = gameSettings.CustomPerGameLinkFolder;
					}
					txt_linkFrom.Text = selectedPath;
					gameSettings.CustomPerGameLinkFolder = selectedPath;
					gameSettings.Save(PerGameConfigFile);
					btn_link_open.Enabled = true;

					try
					{
						int count = Directory.EnumerateFiles(_linkSourceFolder, "*", SearchOption.AllDirectories).Count();
						lbl_linkNumber.Text = "Number of files = " + count;
					}
					catch
					{
						lbl_linkNumber.Text = "Number of files = ??? (error reading directory)";
					}
					//txt_linksourcefolderexe.Text = fbd.SelectedPath;
					//ConfigurationManager.MainConfig.perGameLinkFolderExe = fbd.SelectedPath;
					//ConfigurationManager.SaveConfig();
				}
			}
		}

		private void btn_customTp_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog1 = new OpenFileDialog();
			openFileDialog1.Filter = "Fichiers TeknoParrotUi.exe|TeknoParrotUi.exe";
			openFileDialog1.Title = "Select TeknoParrotUi.exe";
			DialogResult result = openFileDialog1.ShowDialog();
			if (result == DialogResult.OK)
			{
				string fichierSelectionne = openFileDialog1.FileName;
				txt_customTp.Text = fichierSelectionne;
				gameSettings.CustomTpExe = txt_customTp.Text.Trim();
				gameSettings.Save(PerGameConfigFile);
				LinkLoad();
			}

		}

		private void btn_customTpClear_Click(object sender, EventArgs e)
		{
			txt_customTp.Text = "";
			gameSettings.CustomTpExe = txt_customTp.Text.Trim();
			gameSettings.Save(PerGameConfigFile);
			LinkLoad();
		}

		private void txt_customTp_TextChanged(object sender, EventArgs e)
		{

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

		private void chk_enableStoozZone_Hotas_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void chk_invertYAxis_Hotas_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void btn_selectLinkFolderExe_Click(object sender, EventArgs e)
		{
			MessageBox.Show("You must select a directory that use the same Drive as your game folder.");
			using (var fbd = new FolderBrowserDialog())
			{
				DialogResult result = fbd.ShowDialog();

				if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
				{
					string selectedPath = fbd.SelectedPath;
					selectedPath = Path.GetFullPath(selectedPath);

					if (!Utils.IsEligibleHardLink(selectedPath, _linkTargetFolderExe))
					{
						MessageBox.Show("This folder is not eligible for HardLink");
						return;
					}

					string gameName = Path.GetFileNameWithoutExtension(GameData.FileName);
					string lastFolder = Path.GetFileName(selectedPath);
					if (lastFolder != gameName)
					{
						DialogResult dr = MessageBox.Show($"The last folder must be {gameName} do you want to create {Path.Combine(selectedPath, gameName)} ?",
					  "Create Link folder", MessageBoxButtons.YesNo);
						if (dr == DialogResult.Yes)
						{
							selectedPath = Path.Combine(selectedPath, gameName);
							if (!Directory.Exists(selectedPath)) Directory.CreateDirectory(selectedPath);
						}
						else return;

						//_linkSourceFolder = gameSettings.CustomPerGameLinkFolder;
					}
					txt_linkFromExe.Text = selectedPath;
					gameSettings.CustomPerGameLinkFolder = selectedPath;
					gameSettings.Save(PerGameConfigFile);
					btn_link_openExe.Enabled = true;

					try
					{
						int count = Directory.EnumerateFiles(_linkSourceFolderExe, "*", SearchOption.AllDirectories).Count();
						lbl_linkNumberExe.Text = "Number of files = " + count;
					}
					catch
					{
						lbl_linkNumberExe.Text = "Number of files = ??? (error reading directory)";
					}
					//txt_linksourcefolderexe.Text = fbd.SelectedPath;
					//ConfigurationManager.MainConfig.perGameLinkFolderExe = fbd.SelectedPath;
					//ConfigurationManager.SaveConfig();
					LinkLoad();
				}
			}
		}

		private void btn_linkTarget_open_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(_linkTargetFolder))
			{
				if (Directory.Exists(_linkTargetFolder))
				{
					ProcessStartInfo startInfo = new ProcessStartInfo
					{
						Arguments = _linkTargetFolder,
						FileName = "explorer.exe"
					};
					Process.Start(startInfo);
				}
				else
				{
					MessageBox.Show($"Directory {_linkTargetFolder} does not exist");
				}
			}
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

		private void btn_vjoyconfig_Click(object sender, EventArgs e)
		{
			// Recherche de la fenêtre à fermer parmi les fenêtres ouvertes
			foreach (Form form in Application.OpenForms)
			{
				if (form.GetType() == typeof(VjoyControl))
				{
					// La fenêtre Fenetre1 est ouverte, fermez-la
					form.Close();
					Thread.Sleep(100);
					break; // Sortir de la boucle une fois que la fenêtre est trouvée et fermée
				}
			}

			
			bool vjoy_gunA = ConfigurationManager.MainConfig.gunAvjoy;
			if (cmb_gunA_UseVjoy.SelectedIndex > 0)
			{
				if (cmb_gunA_UseVjoy.SelectedIndex == 1) vjoy_gunA = false;
				if (cmb_gunA_UseVjoy.SelectedIndex == 2) vjoy_gunA = true;
			}
			bool vjoy_gunB = ConfigurationManager.MainConfig.gunBvjoy;
			if (cmb_gunB_UseVjoy.SelectedIndex > 0)
			{
				if (cmb_gunB_UseVjoy.SelectedIndex == 1) vjoy_gunB = false;
				if (cmb_gunB_UseVjoy.SelectedIndex == 2) vjoy_gunB = true;
			}

			var frm = new VjoyControl(true, Path.GetFileNameWithoutExtension(GameData.UserConfigFile), gameSettings, vjoy_gunA, vjoy_gunB);
			var result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{
				if (!string.IsNullOrEmpty(frm.GunA_json)) gameSettings.vjoySettingsGunA = frm.GunA_json;
				if (!string.IsNullOrEmpty(frm.GunB_json)) gameSettings.vjoySettingsGunB = frm.GunB_json;
				gameSettings.Save(PerGameConfigFile);
			}
		}

		private void cmb_vjoy_SelectedIndexChanged(object sender, EventArgs e)
		{
			gameSettings.indexvjoy = cmb_vjoy.SelectedIndex - 1;
			gameSettings.Save(PerGameConfigFile);
		}

		private void groupBox5_Enter(object sender, EventArgs e)
		{

		}

		private void kryptonLabel10_Click(object sender, EventArgs e)
		{

		}
	}
}
