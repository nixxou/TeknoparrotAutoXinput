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

			if(ShifterHack.supportedGames.ContainsKey(Path.GetFileNameWithoutExtension(GameData.UserConfigFile))) chk_enableGearChange.Enabled = true;

		}



		private void GameOptions_Load(object sender, EventArgs e)
		{
			chk_runAsAdmin.Enabled = false;
			chk_group_StoozZone_Wheel.Location = new Point(chk_group_StoozZone_Wheel.Location.X, chk_group_StoozZone_Wheel.Location.Y + 15);
			chk_group_StoozZone_Gamepad.Location = new Point(chk_group_StoozZone_Gamepad.Location.X, chk_group_StoozZone_Gamepad.Location.Y + 15);
			chk_group_monitorDisposition.Location = new Point(chk_group_monitorDisposition.Location.X, chk_group_monitorDisposition.Location.Y + 15);
			grp_link.Enabled = false;
			try
			{
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(GameData.UserConfigFile);
				XmlNode emulatorTypeNode = xmlDoc.SelectSingleNode("/GameProfile/EmulatorType");
				if (emulatorTypeNode != null)
				{
					string emulatorTypeValue = emulatorTypeNode.InnerText.ToLower().Trim();
					if (emulatorTypeValue == "elfldr2" || emulatorTypeValue == "lindbergh")
					{
						string perGameLinkFolder = ConfigurationManager.MainConfig.perGameLinkFolder;
						if (perGameLinkFolder == @"Default (<YourTeknoparrotFolder>\AutoXinputLinks)")
						{
							_linkSourceFolder = Path.Combine(_tpBaseFolder, "AutoXinputLinks", Path.GetFileNameWithoutExtension(GameData.FileName));
						}
						else
						{
							_linkSourceFolder = Path.Combine(perGameLinkFolder, Path.GetFileNameWithoutExtension(GameData.FileName));
						}
						

						lbl_linkFrom.Text = "Link from : " + _linkSourceFolder;
						grp_link.Enabled = true;
						if (emulatorTypeValue == "elfldr2")
						{
							lbl_LinkTo.Text = "To : " + _elfldr2Folder;
							_linkTargetFolder = _elfldr2Folder;
						}
						if (emulatorTypeValue == "lindbergh")
						{
							lbl_LinkTo.Text = "To : " + _lindberghFolder;
							_linkTargetFolder = _lindberghFolder;
						}
					}
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
					grp_link.Enabled = false;
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
	}
}
