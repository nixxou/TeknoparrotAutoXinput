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
using TeknoParrotUi.Common;

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

		}



		private void GameOptions_Load(object sender, EventArgs e)
		{
			chk_runAsAdmin.Enabled = false;
			chk_group_StoozZone_Wheel.Location = new Point(chk_group_StoozZone_Wheel.Location.X, chk_group_StoozZone_Wheel.Location.Y + 15);
			chk_group_StoozZone_Gamepad.Location = new Point(chk_group_StoozZone_Gamepad.Location.X, chk_group_StoozZone_Gamepad.Location.Y + 15);
			chk_group_monitorDisposition.Location = new Point(chk_group_monitorDisposition.Location.X, chk_group_monitorDisposition.Location.Y + 15);
			grp_link.Enabled = false;

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
					if (gamePathContent.ToLower().EndsWith(".exe")) executableGame = gamePathContent;
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
						btn_selectLinkFolder.Enabled = false;
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
					if (btn_selectLinkFolder.Enabled) btn_selectLinkFolder.Enabled = false;
					grp_link.Enabled = false;
					chk_linkfiles.Visible = false;
					lbl_linkNumber.Text = "Target folder is not eligible for HardLink";
					btn_link_open.Enabled = false;
				}
				else
				{
					if (Directory.Exists(_linkSourceFolder))
					{
						if (!Utils.IsEligibleHardLink(_linkSourceFolder, _linkTargetFolder))
						{
							lbl_linkNumber.Text = "Source folder is not eligible for HardLink";
							btn_link_open.Enabled = false;
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
							btn_link_open.Enabled = false;
						}
						else
						{
							lbl_linkNumber.Text = "Number of files = 0 (Source dir does not exist)";
						}
					}

				}
			}

			chk_group_monitorDisposition.Checked = gameSettings.UseGlobalDisposition;
			chk_group_StoozZone_Gamepad.Checked = gameSettings.UseGlobalStoozZoneGamepad;
			chk_group_StoozZone_Wheel.Checked = gameSettings.UseGlobalStoozZoneWheel;
			chk_runAsAdmin.Checked = gameSettings.RunAsRoot;
			chk_enableStoozZone_Gamepad.Checked = gameSettings.enableStoozZone_Gamepad;
			chk_enableStoozZone_Wheel.Checked = gameSettings.enableStoozZone_Wheel;
			trk_useCustomStooz_Gamepad.Value = gameSettings.valueStooz_Gamepad;
			trk_useCustomStooz_Wheel.Value = gameSettings.valueStooz_Wheel;
			txt_ahkafter.Text = gameSettings.AhkAfter;
			txt_ahkbefore.Text = gameSettings.AhkBefore;
			chk_linkfiles.Checked = gameSettings.EnableLink;
			chk_WaitForExitBefore.Checked = gameSettings.WaitForExitAhkBefore;
			chk_enableGearChange.Checked = gameSettings.EnableGearChange;
			txt_monitorswitch.Text = gameSettings.Disposition == "" ? "<none>" : gameSettings.Disposition;
			txt_customTp.Text = gameSettings.CustomTpExe;
			Reload();

		}

		private void Reload()
		{
			grp_monitorDisposition.Enabled = !chk_group_monitorDisposition.Checked;
			grp_StoozZone_Gamepad.Enabled = !chk_group_StoozZone_Gamepad.Checked;
			grp_StoozZone_Wheel.Enabled = !chk_group_StoozZone_Wheel.Checked;
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
				if (!Directory.Exists(_linkSourceFolder))
				{
					Directory.CreateDirectory(_linkSourceFolder);
					Thread.Sleep(100);
				}
				ProcessStartInfo startInfo = new ProcessStartInfo
				{
					Arguments = _linkSourceFolder,
					FileName = "explorer.exe"
				};
				Process.Start(startInfo);
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



			gameSettings.UseGlobalDisposition = chk_group_monitorDisposition.Checked;
			gameSettings.UseGlobalStoozZoneGamepad = chk_group_StoozZone_Gamepad.Checked;
			gameSettings.UseGlobalStoozZoneWheel = chk_group_StoozZone_Wheel.Checked;
			gameSettings.RunAsRoot = chk_runAsAdmin.Checked;
			gameSettings.enableStoozZone_Gamepad = chk_enableStoozZone_Gamepad.Checked;
			gameSettings.enableStoozZone_Wheel = chk_enableStoozZone_Wheel.Checked;
			gameSettings.valueStooz_Gamepad = trk_useCustomStooz_Gamepad.Value;
			gameSettings.valueStooz_Wheel = trk_useCustomStooz_Wheel.Value;
			gameSettings.AhkAfter = txt_ahkafter.Text.Trim();
			gameSettings.AhkBefore = txt_ahkbefore.Text.Trim();
			gameSettings.EnableLink = chk_linkfiles.Checked;
			gameSettings.WaitForExitAhkBefore = chk_WaitForExitBefore.Checked;
			gameSettings.EnableGearChange = chk_enableGearChange.Checked;
			gameSettings.Disposition = txt_monitorswitch.Text.Trim();
			gameSettings.CustomTpExe = txt_customTp.Text.Trim();

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
			}

		}

		private void txt_customTp_TextChanged(object sender, EventArgs e)
		{

		}
	}
}
