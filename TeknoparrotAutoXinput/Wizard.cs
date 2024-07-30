using Krypton.Toolkit;
using Nefarius.ViGEm.Client.Exceptions;
using Nefarius.ViGEm.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using XJoy;
using System.Diagnostics;
using SharpDX.DirectInput;
using vJoyInterfaceWrap;
using Newtonsoft.Json;
using System.Security.Cryptography;
using TeknoParrotUi.Common;
using SevenZip;
using System.Text.RegularExpressions;
using CliWrap;
using SharpDX.Multimedia;
using TeknoparrotAutoXinput;
using WiimoteLib;
using XInput.Wrapper;
using System.Reflection;
using Microsoft.Win32;
using SerialPortLib2;
using SDL2;

namespace TeknoparrotAutoXinput
{
	public partial class Wizard : KryptonForm
	{
		Region tabOriginalRegion;
		string currentDir = Path.GetFullPath(Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
		string SevenZipExe = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "7zip", "7z.exe");
		string wizardSettingsJson = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)), "wizard.json");
		string fixesDir = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)), "fixes");

		WizardSettings SavedWizardSettings;

		private bool haveRequiredSoftware = false;
		private Action<string> handleStdOut;
		private int progress = 0; // Variable pour stocker la progression
		private int totalCount = 0; // Total des éléments à traiter
		private bool isProcessing = false; // Indicateur de traitement
		private Dictionary<string, CacheMetadata> cacheCheckFix = new Dictionary<string, CacheMetadata>();
		Dictionary<string, string> ExpectedArchivesFix = new Dictionary<string, string>();


		//Tab 2
		List<string> gameExecutableList = new List<string>();
		Dictionary<string, string> cacheMD5 = new Dictionary<string, string>();
		List<string> MissingGameProfile = new List<string>();
		List<string> ExistingGameProfile = new List<string>();
		List<string> ExistingUserProfile = new List<string>();
		Dictionary<string, Metadata> GamesMetaData = new Dictionary<string, Metadata>();


		//Tab3


		private Dictionary<int, XinputGamepad> _connectedGamePad = new Dictionary<int, XinputGamepad>();
		string dwheel_config = "";
		string dshifter_config = "";
		string dhotas_config = "";

		int gunA_comport = 0;
		int gunA_type = -1;
		string gunA_json = "";


		int gunB_comport = 0;
		int gunB_type = -1;
		string gunB_json = "";

		int selectController_xinput = 0;
		int selectController_wheel = 0;
		int selectController_hotas = 0;
		int selectController_lightgun = 0;
		int selectController_guntypeA = -1;
		int selectController_guntypeB = -1;

		public bool IsWheelConfigured = false;
		public bool IsShifterConfigured = false;
		public bool IsHotasConfigured = false;
		public bool IsGunAConfigured = false;
		public bool IsGunBConfigured = false;

		FFBDevice ffbWheel = new FFBDevice { Name = "<none", Guid = "" };
		FFBDevice ffbHotas = new FFBDevice { Name = "<none", Guid = "" };

		public Wizard()
		{
			SevenZipExtractor.SetLibraryPath(Path.Combine(currentDir, "thirdparty", "7zip", "7z.dll"));
			InitializeComponent();
			InitializeHandleStdOut();


		}

		public bool checkControllerStatus()
		{
			bool wheel = false;
			if (selectController_wheel == 0) wheel = true;
			else if (selectController_wheel == 1 && IsWheelConfigured) wheel = true;
			else if (selectController_wheel == 2 && IsWheelConfigured && IsShifterConfigured) wheel = true;

			bool hotas = false;
			if (selectController_hotas == 0) hotas = true;
			else if (selectController_wheel == 1 && IsHotasConfigured) hotas = true;

			bool gun = false;
			if (selectController_lightgun == 0) gun = true;
			if (selectController_lightgun == 1 && IsGunAConfigured) gun = true;
			if (selectController_lightgun == 2 && IsGunAConfigured && IsGunBConfigured) gun = true;

			lbl_wheel_status.Text = (wheel) ? "Ok" : "Not Ok";
			lbl_wheel_status.ForeColor = (wheel) ? Color.DarkGreen : Color.Red;
			lbl_hotas_status.Text = (hotas) ? "Ok" : "Not Ok";
			lbl_hotas_status.ForeColor = (hotas) ? Color.DarkGreen : Color.Red;
			lbl_gun_status.Text = (gun) ? "Ok" : "Not Ok";
			lbl_gun_status.ForeColor = (gun) ? Color.DarkGreen : Color.Red;

			if (tabControl1.SelectedIndex == 2)
			{
				if (wheel && hotas && gun) btn_next.Enabled = true;
				else btn_next.Enabled = false;
			}

			if (wheel && hotas && gun) return true;
			return false;

		}

		public void UpdateFFBGuidList()
		{
			SDL2.SDL.SDL_Quit();
			SDL2.SDL.SDL_SetHint(SDL2.SDL.SDL_HINT_JOYSTICK_RAWINPUT, "0");
			SDL2.SDL.SDL_Init(SDL2.SDL.SDL_INIT_JOYSTICK | SDL2.SDL.SDL_INIT_GAMECONTROLLER);
			SDL2.SDL.SDL_JoystickUpdate();
			for (int i = 0; i < SDL2.SDL.SDL_NumJoysticks(); i++)
			{
				var currentJoy = SDL.SDL_JoystickOpen(i);
				string nameController = SDL2.SDL.SDL_JoystickNameForIndex(i).Trim('\0');
				{

					const int bufferSize = 256; // La taille doit être au moins 33 pour stocker le GUID sous forme de chaîne (32 caractères + le caractère nul)
					byte[] guidBuffer = new byte[bufferSize];
					SDL.SDL_JoystickGetGUIDString(SDL.SDL_JoystickGetGUID(currentJoy), guidBuffer, bufferSize);
					string guidString = System.Text.Encoding.UTF8.GetString(guidBuffer).Trim('\0');

					if (!string.IsNullOrEmpty(guidString))
					{
						FFBDevice fFBDevice = new FFBDevice { Name = nameController, Guid = guidString };
						if (!cmb_ffbguid_hotas.Items.Contains(fFBDevice)) cmb_ffbguid_hotas.Items.Add(fFBDevice);
						if (!cmb_ffbguid_wheel.Items.Contains(fFBDevice)) cmb_ffbguid_wheel.Items.Add(fFBDevice);
					}
					SDL.SDL_JoystickClose(currentJoy);
				}
			}
			SDL2.SDL.SDL_Quit();

		}

		#region GlobalLoadAndTabManagement

		private void Wizard_Load(object sender, EventArgs e)
		{
			SavedWizardSettings = new WizardSettings(wizardSettingsJson);
			SavedWizardSettings.disableSave = true;
			txt_patchArchive.Text = SavedWizardSettings.patchArchive;

			tabOriginalRegion = tabControl1.Region;
			tabControl1.Region = new Region(tabControl1.DisplayRectangle);

			haveRequiredSoftware = CheckInstalledSoftware();
			CheckRequiredFixes();

			cmb_ffbguid_hotas.Items.Add(new FFBDevice { Name = "<none>", Guid = "" });
			cmb_ffbguid_wheel.Items.Add(new FFBDevice { Name = "<none>", Guid = "" });
			UpdateFFBGuidList();


			tabPage1_Load();

			if (SavedWizardSettings.tpfolder != "" && Directory.Exists(SavedWizardSettings.tpfolder) && File.Exists(Path.Combine(SavedWizardSettings.tpfolder, "TeknoParrotUi.exe")))
			{
				txt_tpfolder.Text = SavedWizardSettings.tpfolder;
			}
			if (SavedWizardSettings.linksourcefolderexe != "")
			{
				txt_linksourcefolderexe.Text = SavedWizardSettings.linksourcefolderexe;
			}

			if (SavedWizardSettings.arcadeXinputData != "") txt_arcadeXinputData.Text = SavedWizardSettings.arcadeXinputData;
			dwheel_config = SavedWizardSettings.dwheel_config;
			dhotas_config = SavedWizardSettings.dhotas_config;
			dshifter_config = SavedWizardSettings.dhotas_config;
			radio_selectcontroller_arcadestick_yes.Checked = (SavedWizardSettings.selectController_xinput == 1);
			radio_selectcontroller_dwheel_shifter.Checked = (SavedWizardSettings.selectController_wheel == 2);
			radio_selectcontroller_dwheel_yes.Checked = (SavedWizardSettings.selectController_wheel == 1);
			radio_selectcontroller_dhotas_yes.Checked = (SavedWizardSettings.selectController_hotas == 1);
			radio_selectcontroller_gun_1.Checked = (SavedWizardSettings.selectController_lightgun == 1);
			radio_selectcontroller_gun_2.Checked = (SavedWizardSettings.selectController_lightgun == 2);

			if (SavedWizardSettings.selectController_guntypeA >= 0) cmb_gunA_type.SelectedIndex = SavedWizardSettings.selectController_guntypeA;
			if (SavedWizardSettings.selectController_guntypeB >= 0) cmb_gunB_type.SelectedIndex = SavedWizardSettings.selectController_guntypeB;


			this.gunA_type = SavedWizardSettings.gunA_type;
			this.gunA_json = SavedWizardSettings.gunA_json;
			this.gunA_comport = SavedWizardSettings.gunA_comport;

			this.gunB_comport = SavedWizardSettings.gunB_comport;
			this.gunB_type = SavedWizardSettings.gunB_type;
			this.gunB_json = SavedWizardSettings.gunB_json;

			this.IsGunAConfigured = SavedWizardSettings.IsGunAConfigured;
			this.IsGunBConfigured = SavedWizardSettings.IsGunBConfigured;
			this.IsHotasConfigured = SavedWizardSettings.IsHotasConfigured;
			this.IsWheelConfigured = SavedWizardSettings.IsWheelConfigured;

			var savedFFBWheel = new FFBDevice { Name = SavedWizardSettings.ffbWheel_name, Guid = SavedWizardSettings.ffbWheel_guid };
			var savedFFBHotas = new FFBDevice { Name = SavedWizardSettings.ffbHotas_name, Guid = SavedWizardSettings.ffbHotas_guid };

			if (!cmb_ffbguid_wheel.Items.Contains(savedFFBWheel)) cmb_ffbguid_wheel.Items.Add(savedFFBWheel);
			if (!cmb_ffbguid_hotas.Items.Contains(savedFFBHotas)) cmb_ffbguid_hotas.Items.Add(savedFFBHotas);
			foreach (FFBDevice fFBDevice in cmb_ffbguid_hotas.Items)
			{
				if (fFBDevice.Guid == SavedWizardSettings.ffbHotas_guid) ffbHotas = fFBDevice;
			}
			foreach (FFBDevice fFBDevice in cmb_ffbguid_wheel.Items)
			{
				if (fFBDevice.Guid == SavedWizardSettings.ffbWheel_guid) ffbWheel = fFBDevice;
			}


			cmb_ffbguid_hotas.SelectedIndex = 0;
			if (cmb_ffbguid_hotas.Items.Contains(ffbHotas)) cmb_ffbguid_hotas.SelectedIndex = cmb_ffbguid_hotas.Items.IndexOf(ffbHotas);
			cmb_ffbguid_wheel.SelectedIndex = 0;
			if (cmb_ffbguid_wheel.Items.Contains(ffbWheel)) cmb_ffbguid_wheel.SelectedIndex = cmb_ffbguid_hotas.Items.IndexOf((ffbWheel));
			timer_ffb_update.Enabled = true;

			checkControllerStatus();

			cmb_gpu.SelectedIndex = SavedWizardSettings.gpu;
			cmb_resolution.SelectedIndex = SavedWizardSettings.resolution;
			txt_mariokartId.Text = SavedWizardSettings.mariokartId;
			txt_apm3id.Text = SavedWizardSettings.apm3id;
			txt_customName.Text = SavedWizardSettings.customName;

			int max_tab = 0;
			for (int i = 0; i < 4; i++)
			{
				max_tab = i;
				if (i == 0 && !verifDataTab1()) break;
				if (i == 1 && !verifDataTab2()) break;
				if (i == 2 && !verifDataTab3()) break;
			}
			int start_tab = SavedWizardSettings.start_tab;
			if (start_tab > max_tab) start_tab = max_tab;
			tabControl1.SelectedIndex = start_tab;
			if (start_tab == 0)
			{
				btn_next.Enabled = verifDataTab1();
				btn_previous.Enabled = false;
			}
			SavedWizardSettings.disableSave = false;
		}

		private void btn_next_Click(object sender, EventArgs e)
		{
			bool canMoveNext = true;

			if (tabControl1.SelectedIndex == 0 && haveRequiredSoftware == false)
			{
				DialogResult result = MessageBox.Show(
					"All required software are not installed, are you sure to continue?",
					"Warning",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Warning
				);

				// Obtenir la réponse dans une variable booléenne
				bool userWantsToContinue = result == DialogResult.Yes;
				if (!userWantsToContinue)
				{
					canMoveNext = false;
				}
			}


			/*
			bool canMoveNext = false;
			switch (tabControl1.SelectedIndex)
			{
				case 0:
					canMoveNext = verifDataTab1();
					break;
				case 1:
					canMoveNext = verifDataTab2();
					break;
				// Ajoutez des cases pour les autres onglets si nécessaire
				default:
					canMoveNext = true;
					break;
			}
			*/

			// Vérifier si l'index actuel est inférieur à l'index maximal des onglets
			if (canMoveNext && tabControl1.SelectedIndex < tabControl1.TabCount - 1)
			{
				// Passer à l'onglet suivant
				tabControl1.SelectedIndex++;
			}
		}

		private void btn_previous_Click(object sender, EventArgs e)
		{
			if (tabControl1.SelectedIndex > 0)
			{
				// Revenir à l'onglet précédent
				tabControl1.SelectedIndex--;
			}
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			SavedWizardSettings.start_tab = tabControl1.SelectedIndex;
			SavedWizardSettings.Save(wizardSettingsJson);

			string currentTabName = tabControl1.SelectedTab.Text;
			//MessageBox.Show(currentTabName);
			if (currentTabName == "tabPage1")
			{
				haveRequiredSoftware = CheckInstalledSoftware();
				btn_previous.Enabled = false;
				btn_next.Enabled = verifDataTab1();
			}
			else if (currentTabName == "tabPage2")
			{
				btn_previous.Enabled = true;
				btn_next.Enabled = verifDataTab2();

			}
			else if (currentTabName == "tabPage3")
			{
				btn_previous.Enabled = true;
				btn_next.Enabled = verifDataTab3();
			}
			else if (currentTabName == "tabPage4")
			{
				btn_previous.Enabled = true;
				btn_next.Enabled = false;
				chk_installpatch.Text = $"Install and overwrite patches in {Path.Combine(txt_tpfolder.Text, "pathAutoXinputLinks")}\n and {txt_linksourcefolderexe.Text}";

			}
			else
			{
				btn_previous.Enabled = true;
			}
			//MessageBox.Show(currentTabName);
		}


		private bool verifDataTab1()
		{
			bool valid = true;
			if (String.IsNullOrEmpty(txt_patchArchive.Text) || !File.Exists(txt_patchArchive.Text))
			{
				valid = false;
			}
			if (grp_fixes.Enabled && lbl_fixes_status.ForeColor == Color.Red)
			{
				valid = false;
			}
			return valid;
		}

		private bool verifDataTab2()
		{
			bool valid = true;
			if (String.IsNullOrEmpty(txt_tpfolder.Text) || !File.Exists(Path.Combine(Path.Combine(txt_tpfolder.Text, "TeknoParrotUi.exe"))))
			{
				valid = false;
			}
			if (String.IsNullOrEmpty(txt_linksourcefolderexe.Text) || !Directory.Exists(Directory.GetParent(txt_linksourcefolderexe.Text).FullName))
			{
				valid = false;
			}
			return valid;
		}

		private bool verifDataTab3()
		{
			return checkControllerStatus();
		}

		private bool verifDataTab4()
		{
			if (cmb_gpu.SelectedIndex < 0) return false;
			if (cmb_resolution.SelectedIndex < 0) return false;
			if (txt_apm3id.Text == "") return false;
			if (txt_mariokartId.Text == "") return false;
			if (txt_customName.Text == "") return false;
			return true;
		}

		#endregion

		//Tab 1

		#region Tab1
		private void tabPage1_Load()
		{
			string parentDir = Path.GetDirectoryName(currentDir);
			if (File.Exists(Path.Combine(parentDir, "TeknoParrotUi.exe")))
			{
				txt_tpfolder.Text = parentDir;
			}
		}



		private void btn_selectTP_Click(object sender, EventArgs e)
		{
			using (var fbd = new FolderBrowserDialog())
			{
				DialogResult result = fbd.ShowDialog();

				if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
				{
					
					if (!Utils.IsEligibleHardLink(fbd.SelectedPath))
					{
						MessageBox.Show("The drive does not seems eligible for hardlink (Must be NTFS and Windows 10 or newer)");
					}
					else
					{
						string tpui = Path.Combine(fbd.SelectedPath, "TeknoParrotUi.exe");
						if (File.Exists(tpui))
						{
							string previous = txt_tpfolder.Text;
							txt_tpfolder.Text = fbd.SelectedPath;
							if (txt_tpfolder.Text == previous) txt_tpfolder_TextChanged(null, null);
						}
						else
						{
							MessageBox.Show($"Can't find {tpui}");
						}
					}
				}
			}
		}

		private void txt_tpfolder_TextChanged(object sender, EventArgs e)
		{
			UpdateTPData();
			if (txt_tpfolder.Text != "")
			{
				if (!File.Exists(Path.Combine(txt_tpfolder.Text, "TeknoParrotUi.exe")))
				{
					txt_tpfolder.Text = "";
					lbl_tpfolder_status.ForeColor = Color.Red;
					lbl_tpfolder_status.Text = "Not Ok";
				}
				else
				{
					lbl_tpfolder_status.ForeColor = Color.DarkGreen;
					lbl_tpfolder_status.Text = "Ok";
				}
				groupBox2.Enabled = true;
				SavedWizardSettings.tpfolder = txt_tpfolder.Text;
				SavedWizardSettings.Save(wizardSettingsJson);
			}
			btn_next.Enabled = verifDataTab2();
		}

		private void UpdateTPData()
		{
			if (txt_tpfolder.Text != "")
			{
				MissingGameProfile = new List<string>();
				ExistingGameProfile = new List<string>();
				ExistingUserProfile = new List<string>();
				GamesMetaData = new Dictionary<string, Metadata>();

				string gameProfileDir = Path.Combine(txt_tpfolder.Text, "GameProfiles");
				if (Directory.Exists(gameProfileDir))
				{
					var cfgList = Directory.GetFiles(gameProfileDir, "*.xml");
					Parallel.ForEach(cfgList, new ParallelOptions { MaxDegreeOfParallelism = 6 }, (cfg) =>
					{
						lock (ExistingGameProfile)
						{
							ExistingGameProfile.Add(Path.GetFileNameWithoutExtension(cfg));
						}

						string ParentPath = Path.GetDirectoryName(cfg);
						ParentPath = Path.GetDirectoryName(ParentPath);
						var metadataPath = Path.Combine(ParentPath, "Metadata", Path.GetFileNameWithoutExtension(cfg) + ".json");
						if (File.Exists(metadataPath))
						{
							try
							{
								var metadata = JsonConvert.DeserializeObject<Metadata>(File.ReadAllText(metadataPath));
								if (metadata != null)
								{
									lock (GamesMetaData)
									{
										GamesMetaData.Add(Path.GetFileNameWithoutExtension(cfg), metadata);
									}
								}
							}
							catch { }
						}
					});
				}





				groupBox2.Enabled = true;
				string userDir = Path.Combine(txt_tpfolder.Text, "UserProfiles");
				if (Directory.Exists(userDir))
				{
					var cfgList = Directory.GetFiles(userDir, "*.xml");

					Parallel.ForEach(cfgList, new ParallelOptions { MaxDegreeOfParallelism = 6 }, (cfg) =>
					{
						if (Path.GetFileName(cfg).Contains(".custom.") == false)
						{
							//if (gameExecutableList.Count > 30) return;
							string OriginalXML = File.ReadAllText(cfg);
							var xmlDoc = new XmlDocument();
							xmlDoc.LoadXml(OriginalXML);
							bool missing_second_executable = false;
							try
							{
								bool hasTwoExecutable = false;
								XmlNode hasTwoExecutablesNode = xmlDoc.SelectSingleNode("/GameProfile/HasTwoExecutables");
								if (hasTwoExecutablesNode != null)
								{
									if (hasTwoExecutablesNode.InnerText.ToLower() == "true")
									{
										hasTwoExecutable = true;
									}
								}
								if (hasTwoExecutable)
								{
									missing_second_executable = true;
									XmlNode gamePath2Node = xmlDoc.SelectSingleNode("/GameProfile/GamePath2");
									if (gamePath2Node != null)
									{
										string gamePath2Content = gamePath2Node.InnerText;
										if (gamePath2Content != "" && File.Exists(gamePath2Content))
										{
											missing_second_executable = false;
										}
									}
								}

							}
							catch { }

							bool missing_first_executable = true;
							try
							{
								XmlNode gamePathNode = xmlDoc.SelectSingleNode("/GameProfile/GamePath");
								if (gamePathNode != null)
								{
									string gamePathContent = gamePathNode.InnerText;
									if (gamePathContent != "" && File.Exists(gamePathContent))
									{
										missing_first_executable = false;
										lock (gameExecutableList)
										{
											if (!gameExecutableList.Contains(gamePathContent))
											{
												gameExecutableList.Add(gamePathContent);
											}
										}
									}
								}
							}
							catch (Exception ex) { }

							if (!missing_first_executable && !missing_second_executable)
							{
								lock (ExistingUserProfile)
								{
									ExistingUserProfile.Add(Path.GetFileNameWithoutExtension(cfg));
								}
							}
						}
					});
				}

				foreach (var gameProfile in ExistingGameProfile)
				{
					if (!ExistingUserProfile.Contains(gameProfile))
					{
						MissingGameProfile.Add(gameProfile);
					}
				}

				if (txt_scangame.Text == "" && gameExecutableList.Count > 10)
				{
					Dictionary<string, int> commonPathsCount = new Dictionary<string, int>();
					Random random = new Random();

					for (int i = 0; i < 100; i++)
					{
						var selectedDirectories = new List<string>();
						while (selectedDirectories.Count < 3)
						{
							string directory = gameExecutableList[random.Next(gameExecutableList.Count)];
							if (!selectedDirectories.Contains(directory))
							{
								selectedDirectories.Add(directory);
							}
						}

						string commonPath = new Func<List<string>, string>(paths =>
						{
							if (paths == null || paths.Count == 0)
							{
								return string.Empty;
							}

							string[] splitPath = paths[0].Split('\\');
							for (int j = 1; j < paths.Count; j++)
							{
								string[] splitCurrent = paths[j].Split('\\');
								for (int k = 0; k < splitPath.Length; k++)
								{
									if (k >= splitCurrent.Length || !splitPath[k].Equals(splitCurrent[k], StringComparison.OrdinalIgnoreCase))
									{
										splitPath = splitPath.Take(k).ToArray();
										break;
									}
								}
							}

							return string.Join("\\", splitPath);
						})(selectedDirectories);

						if (!string.IsNullOrEmpty(commonPath))
						{
							if (commonPathsCount.ContainsKey(commonPath))
							{
								commonPathsCount[commonPath]++;
							}
							else
							{
								commonPathsCount[commonPath] = 1;
							}
						}
					}
					if (commonPathsCount.Count > 0)
					{
						string mostCommonPath = commonPathsCount.OrderByDescending(kvp => kvp.Value).FirstOrDefault().Key;
						if (Utils.IsEligibleHardLink(mostCommonPath))
						{
							txt_scangame.Text = mostCommonPath;

							if (Utils.IsEligibleHardLink(mostCommonPath))
							{
								txt_linksourcefolderexe.Text = Path.Combine(mostCommonPath, "Teknoparrot-patchs");
							}

						}
					}

				}
				grp_scangame.Enabled = true;

			}
		}

		private void btn_tpDiscord_Click(object sender, EventArgs e)
		{
			string discordServerUrl = "https://discord.gg/bntkyXZ";

			// Ouvrir l'URL dans le navigateur par défaut, ce qui devrait lancer Discord si configuré
			Process.Start(new ProcessStartInfo
			{
				FileName = discordServerUrl,
				UseShellExecute = true
			});

		}
		private void btn_openTPfixes_Click(object sender, EventArgs e)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				Arguments = fixesDir,
				FileName = "explorer.exe"
			};
			Process.Start(startInfo);
		}

		private void btn_install_riva_Click(object sender, EventArgs e)
		{
			try
			{
				string exePath = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "RTSSSetup736.exe");
				string exeDir = Path.GetDirectoryName(exePath);
				Process process = new Process();
				process.StartInfo.FileName = exePath;
				process.StartInfo.WorkingDirectory = exeDir;
				process.StartInfo.UseShellExecute = true;
				process.StartInfo.Verb = "runas";
				process.Start();
				process.WaitForExit();
			}
			catch (Exception ex) { }

			Thread.Sleep(1000);
			haveRequiredSoftware = CheckInstalledSoftware();
		}

		private void btn_install_vjoy_Click(object sender, EventArgs e)
		{
			try
			{
				string exePath = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "vJoySetup.exe");
				string exeDir = Path.GetDirectoryName(exePath);
				Process process = new Process();
				process.StartInfo.FileName = exePath;
				process.StartInfo.WorkingDirectory = exeDir;
				process.StartInfo.UseShellExecute = true;
				process.StartInfo.Verb = "runas";
				process.Start();
				process.WaitForExit();
			}
			catch (Exception ex) { }


			Thread.Sleep(1000);
			haveRequiredSoftware = CheckInstalledSoftware();
		}

		private void btn_install_vigem_Click(object sender, EventArgs e)
		{
			try
			{
				string exePath = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "ViGEmBusSetup_x64.msi");
				string exeDir = Path.GetDirectoryName(exePath);

				Process process = new Process();
				process.StartInfo.FileName = "msiexec";
				process.StartInfo.Arguments = $"/i \"{exePath}\""; // /i for install
				process.StartInfo.WorkingDirectory = exeDir;
				process.StartInfo.UseShellExecute = true;
				process.StartInfo.Verb = "runas";
				process.Start();
				process.WaitForExit();
			}
			catch (Exception ex) { }

			Thread.Sleep(1000);
			haveRequiredSoftware = CheckInstalledSoftware();
		}

		private void btn_install_xenos_Click(object sender, EventArgs e)
		{
			string Xenos7z = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "xenos", "Xenos.7z");
			string XenosPath = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "xenos", "Xenos.exe");
			if (!File.Exists(XenosPath))
			{
				bool valid = false;
				if (File.Exists(Xenos7z))
				{
					AddXenos addXenos = new AddXenos();
					DialogResult result = addXenos.ShowDialog();
					if (result == DialogResult.OK)
					{
						MessageBox.Show("Xenos Installed");
						valid = true;
					}
				}
			}
			else MessageBox.Show("Already installed");
			haveRequiredSoftware = CheckInstalledSoftware();
		}

		private void groupBox5_Enter(object sender, EventArgs e)
		{

		}

		private void btn_selectPatchArchive_Click(object sender, EventArgs e)
		{
			using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
			{
				openFileDialog.Filter = "Fichiers 7z (TAXPatches.*.7z)|TAXPatches.*.7z|Tous les fichiers (*.*)|*.*";
				openFileDialog.Title = "Select TAXPatches.*.7z";

				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					txt_patchArchive.Text = openFileDialog.FileName;
				}
			}
			btn_next.Enabled = verifDataTab1();
		}

		private void txt_patchArchive_TextChanged(object sender, EventArgs e)
		{
			cacheCheckFix.Clear();
			grp_fixes.Enabled = false;
			if (txt_patchArchive.Text != "" && File.Exists(txt_patchArchive.Text) && txt_patchArchive.Text.ToLower().EndsWith(".7z"))
			{
				string fileContent = "";
				try
				{
					using (SevenZipExtractor extractor = new SevenZipExtractor(txt_patchArchive.Text))
					{
						string fileToExtract = "patch.json";

						using (MemoryStream ms = new MemoryStream())
						{
							extractor.ExtractFile(fileToExtract, ms);
							ms.Seek(0, SeekOrigin.Begin);
							using (StreamReader reader = new StreamReader(ms))
							{
								fileContent = reader.ReadToEnd();
							}
						}
					}
				}
				catch
				{
					fileContent = "";
				}
				if (fileContent == "")
				{
					txt_patchArchive.Text = "";
					SavedWizardSettings.patchArchive = "";
					SavedWizardSettings.Save(wizardSettingsJson);
					MessageBox.Show("Invalid Patch File");

					return;
				}

				SavedWizardSettings.patchArchive = txt_patchArchive.Text;
				SavedWizardSettings.Save(wizardSettingsJson);

				PatchArchive patchInfoJson = JsonConvert.DeserializeObject<PatchArchive>(fileContent);
				if (patchInfoJson.FixesArchive.Count() > 0)
				{
					grp_fixes.Enabled = true;

					foreach (var fix in patchInfoJson.FixesArchive)
					{
						ExpectedArchivesFix.Add(fix.Key, fix.Value.source_md5);
					}
				}

				//MessageBox.Show(fileContent);


			}
			else
			{
				txt_patchArchive.Text = "";
				SavedWizardSettings.patchArchive = "";
				SavedWizardSettings.Save(wizardSettingsJson);
				return;
			}
		}



		private void btn_verifyfix_Click(object sender, EventArgs e)
		{
			var missingFiles = VerifyFiles(fixesDir, ExpectedArchivesFix);

			if (missingFiles.Count > 0)
			{
				MessageBox.Show("Missing files:\n" + string.Join("\n", missingFiles), "File verification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				lbl_fixes_status.Text = missingFiles.Count() + " Files Missing";
				lbl_fixes_status.ForeColor = Color.Red;
			}
			else
			{
				MessageBox.Show("All needed files are here !", "File verification", MessageBoxButtons.OK, MessageBoxIcon.Information);
				lbl_fixes_status.Text = "All OK !";
				lbl_fixes_status.ForeColor = Color.DarkGreen;
			}
			btn_next.Enabled = verifDataTab1();
		}

		private void grp_fixes_EnabledChanged(object sender, EventArgs e)
		{
			lbl_fixes_status.Visible = grp_fixes.Enabled;
			if (grp_fixes.Enabled)
			{
				var missingFiles = VerifyFiles(fixesDir, ExpectedArchivesFix);
				if (missingFiles.Count() == 0)
				{
					lbl_fixes_status.Text = "All OK !";
					lbl_fixes_status.ForeColor = Color.DarkGreen;
				}
				else
				{
					lbl_fixes_status.Text = missingFiles.Count() + " Files Missing";
					lbl_fixes_status.ForeColor = Color.Red;
				}
			}
		}
		#endregion

		#region Tab2


		private void btn_scangame_Click(object sender, EventArgs e)
		{
			string dirGame = txt_scangame.Text;
			if (dirGame == "" || !Directory.Exists(dirGame)) { return; }
			kryptonCheckedListBox1.Items.Clear();

			string jsonDataFile = @"gameList.json";

			Dictionary<string, GameExecutableData> gameExecutableDatasOriginal = (Dictionary<string, GameExecutableData>)JsonConvert.DeserializeObject<Dictionary<string, GameExecutableData>>(File.ReadAllText(jsonDataFile));
			Dictionary<string, GameExecutableData> gameExecutableDatas = new Dictionary<string, GameExecutableData>();

			foreach (var gameExecutableData in gameExecutableDatasOriginal)
			{
				if (MissingGameProfile.Contains(gameExecutableData.Key, StringComparer.OrdinalIgnoreCase))
				{
					gameExecutableDatas.Add(gameExecutableData.Key, gameExecutableData.Value);
				}
			}

			List<long> gameSizeList = new List<long>();
			List<long> gameSecondarySizeList = new List<long>();

			foreach (var gameExecutableData in gameExecutableDatas)
			{
				foreach (var mainExec in gameExecutableData.Value.mainExecutable.Values)
				{
					if (!gameSizeList.Contains(mainExec.size)) gameSizeList.Add(mainExec.size);
				}
				foreach (var secondExec in gameExecutableData.Value.secondaryExecutable.Values)
				{
					if (!gameSecondarySizeList.Contains(secondExec.size)) gameSecondarySizeList.Add(secondExec.size);
				}
			}

			Dictionary<string, List<string>> suspectedGameFile = new Dictionary<string, List<string>>();
			Dictionary<string, List<string>> suspectedSecondaryGameFile = new Dictionary<string, List<string>>();

			var subDirs = Directory.GetDirectories(dirGame, "*");
			progress_scan.Minimum = 0;
			progress_scan.Maximum = subDirs.Count();
			progress_scan.Value = 0;
			progress_scan.Visible = true;
			progress_scan.Text = "Initial Scan";
			progress_scan.Show();

			int progress = 0;
			int totalCount = progress_scan.Maximum;
			bool isProcessing = true;

			Task.Run(() => MonitorProgress());
			Parallel.ForEach(subDirs, new ParallelOptions { MaxDegreeOfParallelism = 3 }, (subDir) =>
			{
				var filePaths = Directory.EnumerateFiles(subDir, "*", new EnumerationOptions
				{
					IgnoreInaccessible = true,
					RecurseSubdirectories = true
				});
				foreach (var file in filePaths)
				{
					long sizeFile = new FileInfo(file).Length;
					if (sizeFile > 0 && gameSizeList.Contains(sizeFile))
					{
						string suspectFile = file;
						foreach (KeyValuePair<string, GameExecutableData> gameExecutableData in gameExecutableDatas)
						{

							bool foundAdjascent = false;
							foreach (var mainExec in gameExecutableData.Value.mainExecutable.Values)
							{

								if (mainExec.size == sizeFile)
								{
									string potentialAdjacentFile = Path.Combine(Path.GetDirectoryName(file), gameExecutableData.Value.mainExecutableAdjacentFile);

									if (File.Exists(potentialAdjacentFile) && new FileInfo(potentialAdjacentFile).Length == gameExecutableData.Value.mainExecutableAdjacentFileHash.size)
									{
										foundAdjascent = true;
									}
									break;
								}

							}
							if (foundAdjascent)
							{
								lock (suspectedGameFile)
								{
									if (!suspectedGameFile.ContainsKey(gameExecutableData.Key))
									{
										suspectedGameFile.Add(gameExecutableData.Key, new List<string>());
									}
									suspectedGameFile[gameExecutableData.Key].Add(suspectFile);
								}
								continue;
							}
						}
					}

					if (sizeFile > 0 && gameSecondarySizeList.Contains(sizeFile))
					{
						string suspectFile = file;
						foreach (KeyValuePair<string, GameExecutableData> gameExecutableData in gameExecutableDatas)
						{
							bool foundAdjascent = false;
							foreach (var secondExec in gameExecutableData.Value.secondaryExecutable.Values)
							{

								if (secondExec.size == sizeFile)
								{
									string potentialAdjacentFile = Path.Combine(Path.GetDirectoryName(file), gameExecutableData.Value.secondaryExecutableAdjacentFile);

									if (File.Exists(potentialAdjacentFile) && new FileInfo(potentialAdjacentFile).Length == gameExecutableData.Value.secondaryExecutableAdjacentFileHash.size)
									{
										foundAdjascent = true;
									}
									break;
								}

							}
							if (foundAdjascent)
							{
								lock (suspectedSecondaryGameFile)
								{
									if (!suspectedSecondaryGameFile.ContainsKey(gameExecutableData.Key))
									{
										suspectedSecondaryGameFile.Add(gameExecutableData.Key, new List<string>());
									}
									suspectedSecondaryGameFile[gameExecutableData.Key].Add(suspectFile);
								}
								continue;
							}
						}
					}
				}
				Interlocked.Increment(ref progress);
			});

			Task.Delay(100).Wait(); // Temps pour s'assurer que la mise à jour finale est visible
			isProcessing = false; // Arrêter le thread de surveillance

			progress_scan.Minimum = 0;
			progress_scan.Maximum = suspectedGameFile.Count();
			progress_scan.Value = 0;
			progress_scan.Text = "Verification";
			progress_scan.Update();
			foreach (KeyValuePair<string, GameExecutableData> gameExecutableData in gameExecutableDatas)
			{
				string foundGame = "";
				string foundSecondGame = "";
				if (suspectedGameFile.ContainsKey(gameExecutableData.Key))
				{
					foreach (var mainExec in gameExecutableData.Value.mainExecutable)
					{
						foreach (string suspected in suspectedGameFile[gameExecutableData.Key])
						{
							if (new FileInfo(suspected).Length == mainExec.Value.size && GetMd5HashAsString(suspected) == mainExec.Value.md5)
							{
								if (gameExecutableData.Value.secondaryExecutable.Count() > 0)
								{
									bool found_second_exec = false;
									if (suspectedSecondaryGameFile.ContainsKey(gameExecutableData.Key))
									{
										foreach (var secondExec in gameExecutableData.Value.secondaryExecutable)
										{
											foreach (string suspectedSecond in suspectedSecondaryGameFile[gameExecutableData.Key])
											{
												string relative = GetRelativeDirectoryPath(suspected, suspectedSecond);
												if (relative != gameExecutableData.Value.relativePathSecondary) continue;
												if (new FileInfo(suspectedSecond).Length == secondExec.Value.size && GetMd5HashAsString(suspectedSecond) == secondExec.Value.md5)
												{
													foundSecondGame = suspectedSecond;
													found_second_exec = true;
													break;
												}
											}
											if (found_second_exec) break;
										}
									}
									if (found_second_exec)
									{
										foundGame = suspected;
										break;
									}
								}
								else
								{
									foundGame = suspected;
									break;
								}
							}
						}
						if (foundGame != "") break;
					}
					progress_scan.Value++;
					progress_scan.Update();
				}
				if (foundGame != "")
				{
					GameInstall gameInstall = new GameInstall();
					gameInstall.gameId = gameExecutableData.Key;
					gameInstall.mainExecutable = foundGame;
					gameInstall.secondaryExecutable = foundSecondGame;
					gameInstall.source = foundGame;
					gameInstall.isArchive = false;
					gameInstall.gameName = GamesMetaData.ContainsKey(gameInstall.gameId) ? GamesMetaData[gameInstall.gameId].game_name : gameInstall.gameId;


					bool alreadyAdded = false;
					foreach (GameInstall addedGameInstall in kryptonCheckedListBox1.Items)
					{
						if (addedGameInstall.gameId == gameInstall.gameId) alreadyAdded = true;
					}
					if (!alreadyAdded) kryptonCheckedListBox1.Items.Add(gameInstall);
				}
			}
			progress_scan.Visible = false;
			btn_select_scanarchive.Enabled = true;
			btn_scangame.Enabled = false;
		}

		private void btn_scanarchive_Click(object sender, EventArgs e)
		{
			string dirArchive = txt_scanarchive.Text;
			if (dirArchive == "" || !Directory.Exists(dirArchive)) { return; }
			//kryptonCheckedListBox1.Items.Clear();

			string jsonDataFile = @"gameList.json";

			Dictionary<string, GameExecutableData> gameExecutableDatasOriginal = (Dictionary<string, GameExecutableData>)JsonConvert.DeserializeObject<Dictionary<string, GameExecutableData>>(File.ReadAllText(jsonDataFile));
			Dictionary<string, GameExecutableData> gameExecutableDatas = new Dictionary<string, GameExecutableData>();

			foreach (var gameExecutableData in gameExecutableDatasOriginal)
			{
				if (MissingGameProfile.Contains(gameExecutableData.Key, StringComparer.OrdinalIgnoreCase))
				{
					bool alreadyAddedAsRegularFolder = false;
					foreach (GameInstall gameInstall in kryptonCheckedListBox1.Items)
					{
						if (gameInstall.gameId == gameExecutableData.Key)
						{
							alreadyAddedAsRegularFolder = true;
							break;
						}
					}
					if (!alreadyAddedAsRegularFolder) gameExecutableDatas.Add(gameExecutableData.Key, gameExecutableData.Value);
				}
			}

			Dictionary<string, long> GameCrcList = new Dictionary<string, long>();
			Dictionary<string, long> GameSecondaryCrcList = new Dictionary<string, long>();
			foreach (var gameExecutableData in gameExecutableDatas)
			{
				foreach (var mainExec in gameExecutableData.Value.mainExecutable.Values)
				{
					if (!GameCrcList.ContainsKey(mainExec.crc))
					{
						GameCrcList.Add(mainExec.crc, mainExec.size);
					}
				}
				foreach (var secondExec in gameExecutableData.Value.secondaryExecutable.Values)
				{
					if (!GameSecondaryCrcList.ContainsKey(secondExec.crc))
					{
						GameSecondaryCrcList.Add(secondExec.crc, secondExec.size);
					}
				}
			}

			List<long> gameSizeList = new List<long>();
			List<long> gameSecondarySizeList = new List<long>();

			foreach (var gameExecutableData in gameExecutableDatas)
			{
				foreach (var mainExec in gameExecutableData.Value.mainExecutable.Values)
				{
					if (!gameSizeList.Contains(mainExec.size)) gameSizeList.Add(mainExec.size);
				}
				foreach (var secondExec in gameExecutableData.Value.secondaryExecutable.Values)
				{
					if (!gameSecondarySizeList.Contains(secondExec.size)) gameSecondarySizeList.Add(secondExec.size);
				}
			}

			string[] extensions = new[] { ".7z", ".rar" };
			var allFiles = Directory.GetFiles(dirArchive);
			var archivesFiles = allFiles
				.Where(file => extensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
				.ToArray();

			progress_scan.Minimum = 0;
			progress_scan.Maximum = archivesFiles.Count();
			progress_scan.Value = 0;
			progress_scan.Visible = true;
			progress_scan.Text = "Scan archives";
			progress_scan.Show();
			foreach (var archiveFile in archivesFiles)
			{
				progress_scan.Text = "Scan archive " + Path.GetFileName(archiveFile);
				progress_scan.Update();

				List<string> suspectedGameFileFirstPass = new List<string>();
				List<string> suspectedSecondaryGameFileFirstPass = new List<string>();

				Dictionary<string, List<string>> suspectedGameFile = new Dictionary<string, List<string>>();
				Dictionary<string, List<string>> suspectedSecondaryGameFile = new Dictionary<string, List<string>>();

				string destination = "";
				string minimalDir = null;
				Dictionary<string, (string, long)> contentArchive = new Dictionary<string, (string, long)>();
				using (var sevenZipExtractor = new SevenZipExtractor(archiveFile))
				{
					foreach (var s in sevenZipExtractor.ArchiveFileData)
					{
						if (!s.IsDirectory)
						{
							if (minimalDir == null)
							{
								minimalDir = Path.GetDirectoryName(s.FileName);
							}
							else
							{
								minimalDir = GetCommonPath(minimalDir, Path.GetDirectoryName(s.FileName));
							}

							string crc = s.Crc.ToString("X8");
							long size = (long)s.Size;
							contentArchive.Add(s.FileName, (crc, size));

							if (GameCrcList.ContainsKey(crc))
							{
								suspectedGameFileFirstPass.Add(s.FileName);
							}

							if (GameSecondaryCrcList.ContainsKey(crc))
							{
								suspectedSecondaryGameFileFirstPass.Add(s.FileName);
							}

						}
					}
				}

				foreach (var file in suspectedGameFileFirstPass)
				{
					long sizeFile = contentArchive[file].Item2;
					if (sizeFile > 0 && gameSizeList.Contains(sizeFile))
					{
						string suspectFile = file;
						foreach (KeyValuePair<string, GameExecutableData> gameExecutableData in gameExecutableDatas)
						{
							bool foundAdjascent = false;
							foreach (var mainExec in gameExecutableData.Value.mainExecutable.Values)
							{

								if (mainExec.size == sizeFile)
								{

									string potentialAdjacentFile = Path.Combine("Z:/", Path.GetDirectoryName(file), gameExecutableData.Value.mainExecutableAdjacentFile);
									potentialAdjacentFile = Path.GetFullPath(potentialAdjacentFile).Substring(3);

									if (contentArchive.ContainsKey(potentialAdjacentFile) && contentArchive[potentialAdjacentFile].Item2 == gameExecutableData.Value.mainExecutableAdjacentFileHash.size)
									{
										foundAdjascent = true;
									}
									break;
								}

							}
							if (foundAdjascent)
							{

								if (!suspectedGameFile.ContainsKey(gameExecutableData.Key))
								{
									suspectedGameFile.Add(gameExecutableData.Key, new List<string>());
								}
								suspectedGameFile[gameExecutableData.Key].Add(suspectFile);
								continue;
							}
						}
					}
				}

				foreach (var file in suspectedSecondaryGameFileFirstPass)
				{
					long sizeFile = contentArchive[file].Item2;
					if (sizeFile > 0 && gameSecondarySizeList.Contains(sizeFile))
					{
						string suspectFile = file;
						foreach (KeyValuePair<string, GameExecutableData> gameExecutableData in gameExecutableDatas)
						{
							bool foundAdjascent = false;
							foreach (var secondExec in gameExecutableData.Value.secondaryExecutable.Values)
							{

								if (secondExec.size == sizeFile)
								{
									string potentialAdjacentFile = Path.Combine("Z:/", Path.GetDirectoryName(file), gameExecutableData.Value.secondaryExecutableAdjacentFile);
									potentialAdjacentFile = Path.GetFullPath(potentialAdjacentFile).Substring(3);

									if (contentArchive.ContainsKey(potentialAdjacentFile) && contentArchive[potentialAdjacentFile].Item2 == gameExecutableData.Value.secondaryExecutableAdjacentFileHash.size)
									{
										foundAdjascent = true;
									}
									break;
								}

							}
							if (foundAdjascent)
							{

								if (!suspectedSecondaryGameFile.ContainsKey(gameExecutableData.Key))
								{
									suspectedSecondaryGameFile.Add(gameExecutableData.Key, new List<string>());
								}
								suspectedSecondaryGameFile[gameExecutableData.Key].Add(suspectFile);
								continue;
							}
						}
					}
				}


				foreach (KeyValuePair<string, GameExecutableData> gameExecutableData in gameExecutableDatas)
				{
					string foundGame = "";
					string foundSecondGame = "";
					if (suspectedGameFile.ContainsKey(gameExecutableData.Key))
					{
						foreach (var mainExec in gameExecutableData.Value.mainExecutable)
						{
							foreach (string suspected in suspectedGameFile[gameExecutableData.Key])
							{
								if (contentArchive[suspected].Item2 == mainExec.Value.size && contentArchive[suspected].Item1 == mainExec.Value.crc)
								{
									if (gameExecutableData.Value.secondaryExecutable.Count() > 0)
									{
										bool found_second_exec = false;
										if (suspectedSecondaryGameFile.ContainsKey(gameExecutableData.Key))
										{
											foreach (var secondExec in gameExecutableData.Value.secondaryExecutable)
											{
												foreach (string suspectedSecond in suspectedSecondaryGameFile[gameExecutableData.Key])
												{
													string relative = GetRelativeDirectoryPath(Path.Combine("Z:/", suspected), Path.Combine("Z:/", suspectedSecond));
													if (relative != gameExecutableData.Value.relativePathSecondary) continue;
													if (contentArchive[suspectedSecond].Item2 == secondExec.Value.size && contentArchive[suspectedSecond].Item1 == secondExec.Value.crc)
													{
														foundSecondGame = suspectedSecond;
														found_second_exec = true;
														break;
													}
												}
												if (found_second_exec) break;
											}
										}
										if (found_second_exec)
										{
											foundGame = suspected;
											break;
										}
									}
									else
									{
										foundGame = suspected;
										break;
									}
								}
							}
							if (foundGame != "") break;
						}
					}
					if (foundGame != "" && gameExecutableData.Key != "")
					{

						GameInstall gameInstall = new GameInstall();
						gameInstall.gameId = gameExecutableData.Key;
						gameInstall.mainExecutable = foundGame;
						gameInstall.secondaryExecutable = foundSecondGame;
						gameInstall.source = archiveFile;
						gameInstall.isArchive = true;
						gameInstall.gameName = GamesMetaData.ContainsKey(gameInstall.gameId) ? GamesMetaData[gameInstall.gameId].game_name : gameInstall.gameId;
						gameInstall.minimalDirArchive = minimalDir;

						if (destination == "")
						{
							destination = Path.Combine(txt_scangame.Text, gameInstall.gameId);
							string new_destination = destination;
							int i = 1;
							while (Directory.Exists(new_destination))
							{
								new_destination = destination + "_" + i.ToString();
								i++;
							}
							destination = new_destination;
						}
						gameInstall.destinationArchive = destination;

						if (!string.IsNullOrEmpty(gameInstall.minimalDirArchive))
						{
							if (gameInstall.mainExecutable.StartsWith(gameInstall.minimalDirArchive))
							{
								string relativePath = gameInstall.mainExecutable.Substring(gameInstall.minimalDirArchive.Length);
								if (relativePath.StartsWith("\\") || relativePath.StartsWith("/")) relativePath = relativePath.Substring(1);
								gameInstall.mainExecutable = relativePath;
							}
							if (gameInstall.secondaryExecutable.StartsWith(gameInstall.minimalDirArchive))
							{
								string relativePath = gameInstall.secondaryExecutable.Substring(gameInstall.minimalDirArchive.Length);
								if (relativePath.StartsWith("\\") || relativePath.StartsWith("/")) relativePath = relativePath.Substring(1);
								gameInstall.secondaryExecutable = relativePath;
							}
						}
						if (!string.IsNullOrEmpty(gameInstall.mainExecutable)) gameInstall.mainExecutable = Path.Combine(gameInstall.destinationArchive, gameInstall.mainExecutable);
						if (!string.IsNullOrEmpty(gameInstall.secondaryExecutable)) gameInstall.secondaryExecutable = Path.Combine(gameInstall.destinationArchive, gameInstall.secondaryExecutable);



						bool alreadyAdded = false;
						foreach (GameInstall addedGameInstall in kryptonCheckedListBox1.Items)
						{
							if (addedGameInstall.gameId == gameInstall.gameId) alreadyAdded = true;
						}
						if (!alreadyAdded) kryptonCheckedListBox1.Items.Add(gameInstall);
					}
				}
				progress_scan.Value++;
				progress_scan.Update();
			}
			progress_scan.Visible = false;

		}

		private void txt_scangame_TextChanged(object sender, EventArgs e)
		{
			btn_scangame.Enabled = false;
			if (txt_scangame.Text != "" && Directory.Exists(txt_scangame.Text))
			{
				btn_scangame.Enabled = true;

				if (txt_linksourcefolderexe.Text == "" && Utils.IsEligibleHardLink(txt_scangame.Text))
				{
					txt_linksourcefolderexe.Text = Path.Combine(txt_scangame.Text, "Teknoparrot-patchs");
				}
			}
			kryptonCheckedListBox1.Items.Clear();
		}

		private void btn_select_scangame_Click(object sender, EventArgs e)
		{
			using (var fbd = new FolderBrowserDialog())
			{
				DialogResult result = fbd.ShowDialog();

				if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
				{
					txt_scangame.Text = fbd.SelectedPath;

				}
			}
		}

		private void btn_select_scanarchive_Click(object sender, EventArgs e)
		{
			using (var fbd = new FolderBrowserDialog())
			{
				DialogResult result = fbd.ShowDialog();

				if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
				{
					txt_scanarchive.Text = fbd.SelectedPath;
					btn_scanarchive.Enabled = true;

				}
			}
		}

		private void txt_scanarchive_TextChanged(object sender, EventArgs e)
		{

		}

		//Tab 3
		private void btn_selectLinkFolderExe_Click(object sender, EventArgs e)
		{
			MessageBox.Show("You must select a directory that use the same Drive as your game folder.");
			using (var fbd = new FolderBrowserDialog())
			{
				DialogResult result = fbd.ShowDialog();

				if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
				{

					if (!Utils.IsEligibleHardLink(fbd.SelectedPath))
					{
						MessageBox.Show("The drive does not seems eligible for hardlink (Must be NTFS and Windows 10 or newer)");
					}
					else
					{
						txt_linksourcefolderexe.Text = Path.Combine(fbd.SelectedPath, "Teknoparrot-patchs");
					}
				}
			}
		}

		private void txt_linksourcefolderexe_TextChanged(object sender, EventArgs e)
		{
			if (txt_linksourcefolderexe.Text != "")
			{
				groupBox3.Enabled = true;
			}
		}

		private async void btn_installgames_Click(object sender, EventArgs e)
		{

			// Chemin de l'application à lancer
			string teknoparrotExe = Path.Combine(txt_tpfolder.Text, "TeknoParrotUi.exe");
			if (!File.Exists(teknoparrotExe))
			{
				MessageBox.Show($"Can't find {teknoparrotExe}");
				return;
			}

			string userDir = Path.Combine(txt_tpfolder.Text, "UserProfiles");
			string gameProfileDir = Path.Combine(txt_tpfolder.Text, "GameProfiles");
			if (!Directory.Exists(gameProfileDir)) return;
			if (!Directory.Exists(userDir)) Directory.CreateDirectory(userDir);

			Process[] existingProcesses = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(teknoparrotExe));
			if (existingProcesses.Length > 0)
			{
				foreach (var existingProcess in existingProcesses)
				{
					existingProcess.Kill();
					Thread.Sleep(1000);
				}
			}

			int nbArchiveToExtract = 0;
			foreach (var item in kryptonCheckedListBox1.Items)
			{
				if (kryptonCheckedListBox1.GetItemChecked(kryptonCheckedListBox1.Items.IndexOf(item)))
				{
					GameInstall gameInstall = item as GameInstall;
					if (gameInstall != null && gameInstall.isArchive && !Directory.Exists(gameInstall.destinationArchive))
					{
						nbArchiveToExtract++;
					}
				}
			}

			if (nbArchiveToExtract > 0)
			{
				progress_scan.Minimum = 0;
				progress_scan.Maximum = nbArchiveToExtract;
				progress_scan.Value = 0;
				progress_scan.Visible = true;
				progress_scan.Text = "Extract Archives";
				progress_scan.Show();

				foreach (var item in kryptonCheckedListBox1.Items)
				{
					if (kryptonCheckedListBox1.GetItemChecked(kryptonCheckedListBox1.Items.IndexOf(item)))
					{
						GameInstall gameInstall = item as GameInstall;
						if (gameInstall != null)
						{
							if (gameInstall.isArchive)
							{
								if (!Directory.Exists(gameInstall.destinationArchive))
								{
									string temp_dest = Path.Combine(gameInstall.destinationArchive, "_tmpextract");
									if (Directory.Exists(temp_dest)) Directory.Delete(temp_dest, true);
									Directory.CreateDirectory(temp_dest);

									_ = await SimpleExtractArchiveWithProgressAsync(gameInstall.source, temp_dest);
									//Task extractTask = SimpleExtractArchiveWithProgressAsync(gameInstall.source, temp_dest);

									try
									{
										string minimalDir = Path.Combine(temp_dest, gameInstall.minimalDirArchive);
										foreach (var dirToMove in Directory.GetDirectories(minimalDir))
										{
											Directory.Move(dirToMove, Path.Combine(gameInstall.destinationArchive, Path.GetFileName(dirToMove)));
										}
										foreach (var fileToMove in Directory.GetFiles(minimalDir))
										{
											File.Move(fileToMove, Path.Combine(gameInstall.destinationArchive, Path.GetFileName(fileToMove)));
										}
										if (Directory.Exists(temp_dest)) Directory.Delete(temp_dest, true);
									}
									catch (Exception ex)
									{
										if (Directory.Exists(gameInstall.destinationArchive))
										{
											Directory.Delete(gameInstall.destinationArchive, true);
										}
									}
									progress_scan.Value++;
								}
							}
						}
					}
				}
				progress_scan.Visible = false;
				progress_extract.Visible = false;
			}

			int nbGameToInstall = 0;
			foreach (var item in kryptonCheckedListBox1.Items)
			{
				if (kryptonCheckedListBox1.GetItemChecked(kryptonCheckedListBox1.Items.IndexOf(item)))
				{
					GameInstall gameInstall = item as GameInstall;
					if (gameInstall != null && File.Exists(gameInstall.mainExecutable))
					{
						if (!string.IsNullOrEmpty(gameInstall.secondaryExecutable) && !File.Exists(gameInstall.secondaryExecutable)) continue;
						nbGameToInstall++;
					}
				}
			}
			progress_scan.Minimum = 0;
			progress_scan.Maximum = nbGameToInstall;
			progress_scan.Value = 0;
			progress_scan.Visible = true;
			progress_scan.Text = "Adding games";
			progress_scan.Show();
			List<int> indexToRemove = new List<int>();

			foreach (var item in kryptonCheckedListBox1.Items)
			{
				int index = kryptonCheckedListBox1.Items.IndexOf(item);
				if (kryptonCheckedListBox1.GetItemChecked(index))
				{
					GameInstall gameInstall = item as GameInstall;
					if (gameInstall != null && File.Exists(gameInstall.mainExecutable))
					{
						if (!string.IsNullOrEmpty(gameInstall.secondaryExecutable) && !File.Exists(gameInstall.secondaryExecutable)) continue;
						string gameProfileXml = Path.Combine(gameProfileDir, gameInstall.gameId + ".xml");
						string userProfileXml = Path.Combine(userDir, gameInstall.gameId + ".xml");

						if (!File.Exists(userProfileXml))
						{
							if (File.Exists(gameProfileXml))
							{
								File.Copy(gameProfileXml, userProfileXml);
							}
							else
							{
								continue;
							}
						}

						try
						{
							XmlDocument xmlDoc = new XmlDocument();
							xmlDoc.Load(userProfileXml);
							XmlNode gamePathNode = xmlDoc.SelectSingleNode("/GameProfile/GamePath");
							if (gamePathNode == null)
							{
								gamePathNode = xmlDoc.CreateElement("GamePath");
								xmlDoc.DocumentElement.AppendChild(gamePathNode);
							}
							gamePathNode.InnerText = gameInstall.mainExecutable;

							if (!string.IsNullOrEmpty(gameInstall.secondaryExecutable))
							{
								XmlNode gamePath2Node = xmlDoc.SelectSingleNode("/GameProfile/GamePath2");
								if (gamePath2Node == null)
								{
									gamePath2Node = xmlDoc.CreateElement("GamePath2");
									xmlDoc.DocumentElement.AppendChild(gamePath2Node);
								}
								gamePath2Node.InnerText = gameInstall.secondaryExecutable;
							}

							// Sauvegarder les modifications dans le fichier XML
							xmlDoc.Save(userProfileXml);
						}
						catch { }

						indexToRemove.Add(index);
						progress_scan.Value++;


					}
				}
			}
			progress_scan.Visible = false;
			progress_extract.Visible = false;
			for (int i = indexToRemove.Count - 1; i >= 0; i--)
			{
				kryptonCheckedListBox1.Items.RemoveAt(indexToRemove[i]);
			}
			UpdateTPData();

			Process process = Process.Start(teknoparrotExe);
			process.WaitForInputIdle();
			int tries = 0;
			while (!Utils.IsWindowVisible(process.MainWindowHandle))
			{
				if (tries >= 50) // 50 * 100ms = 5 secondes
				{
					Console.WriteLine("La fenêtre de l'application n'est pas visible après 5 secondes. Sortie du programme.");
					return;
				}

				tries++; // Incrémenter le compteur de tentatives
						 // Attendre un court instant avant de vérifier à nouveau
				System.Threading.Thread.Sleep(100);
			}
		}

		private void btn_selectAll_Click(object sender, EventArgs e)
		{
			// Cocher tous les éléments
			for (int i = 0; i < kryptonCheckedListBox1.Items.Count; i++)
			{
				kryptonCheckedListBox1.SetItemChecked(i, true);
			}
		}

		private void btn_selectNone_Click(object sender, EventArgs e)
		{
			// Décocher tous les éléments
			for (int i = 0; i < kryptonCheckedListBox1.Items.Count; i++)
			{
				kryptonCheckedListBox1.SetItemChecked(i, false);
			}
		}

		private void txt_linksourcefolderexe_TextChanged_1(object sender, EventArgs e)
		{
			if (txt_linksourcefolderexe.Text != "")
			{
				if (!Directory.Exists(Directory.GetParent(txt_linksourcefolderexe.Text).FullName))
				{
					txt_linksourcefolderexe.Text = "";
					lbl_linksourcefolderexe_status.ForeColor = Color.Red;
					lbl_linksourcefolderexe_status.Text = "Not Ok";
				}
				else
				{
					lbl_linksourcefolderexe_status.ForeColor = Color.DarkGreen;
					lbl_linksourcefolderexe_status.Text = "Ok";
				}
				if (tabControl1.SelectedTab.Text == "tabPage1") btn_next.Enabled = true;
				SavedWizardSettings.linksourcefolderexe = txt_linksourcefolderexe.Text;
				SavedWizardSettings.Save(wizardSettingsJson);
			}
			btn_next.Enabled = verifDataTab2();
		}

		#endregion

		#region Tab3



		private void timer_controllerUpdate_Tick(object sender, EventArgs e)
		{
			if (this == Form.ActiveForm && tabControl1.SelectedIndex == 2)
			{
				if (radio_selectcontroller_arcadestick_yes.Checked)
				{

					groupBox_xinputarcade.Enabled = true;

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
					lbl_arcadelist.Text = lbl_arcadelist.Text.TrimEnd().TrimEnd(',');
					lbl_gamepadlist.Text = lbl_gamepadlist.Text.TrimEnd().TrimEnd(',');
					lbl_wheellist.Text = lbl_wheellist.Text.TrimEnd().TrimEnd(',');
				}
			}
			else
			{
				groupBox_xinputarcade.Enabled = false;
			}

		}



		private void radio_selectcontroller_arcadestick_yes_CheckedChanged(object sender, EventArgs e)
		{
			if (radio_selectcontroller_arcadestick_yes.Checked)
			{
				groupBox_xinputarcade.Enabled = true;
				timer_controllerUpdate.Enabled = true;
				SavedWizardSettings.selectController_xinput = 1;
				SavedWizardSettings.Save(wizardSettingsJson);
			}
		}

		private void groupBox_xinputarcade_Enter(object sender, EventArgs e)
		{

		}

		private void radio_selectcontroller_dwheel_yes_CheckedChanged(object sender, EventArgs e)
		{
			if (radio_selectcontroller_dwheel_yes.Checked)
			{
				selectController_wheel = 1;
				btn_configure_wheel.Visible = true;
				btn_configure_shifter.Visible = false;
				SavedWizardSettings.selectController_wheel = 1;
				SavedWizardSettings.Save(wizardSettingsJson);
				checkControllerStatus();
				lbl_ffb_wheel.Visible = true;
				cmb_ffbguid_wheel.Visible = true;
			}
		}

		private void radio_selectcontroller_dwheel_shifter_CheckedChanged(object sender, EventArgs e)
		{
			if (radio_selectcontroller_dwheel_shifter.Checked)
			{
				selectController_wheel = 2;
				btn_configure_wheel.Visible = true;
				btn_configure_shifter.Visible = true;
				SavedWizardSettings.selectController_wheel = 2;
				SavedWizardSettings.Save(wizardSettingsJson);
				checkControllerStatus();
				lbl_ffb_wheel.Visible = true;
				cmb_ffbguid_wheel.Visible = true;
			}
		}

		private void radio_selectcontroller_dwheel_no_CheckedChanged(object sender, EventArgs e)
		{
			if (radio_selectcontroller_dwheel_no.Checked)
			{
				selectController_wheel = 0;
				btn_configure_wheel.Visible = false;
				btn_configure_shifter.Visible = false;
				SavedWizardSettings.selectController_wheel = 0;
				SavedWizardSettings.Save(wizardSettingsJson);
				checkControllerStatus();
				lbl_ffb_wheel.Visible = false;
				cmb_ffbguid_wheel.Visible = false;
			}
		}

		private void radio_selectcontroller_dhotas_yes_CheckedChanged(object sender, EventArgs e)
		{
			if (radio_selectcontroller_dhotas_yes.Checked)
			{
				selectController_hotas = 1;
				btn_configure_hotas.Visible = true;
				SavedWizardSettings.selectController_hotas = 1;
				SavedWizardSettings.Save(wizardSettingsJson);
				checkControllerStatus();
				lbl_ffb_hotas.Visible = true;
				cmb_ffbguid_hotas.Visible = true;
			}
		}

		private void radio_selectcontroller_dhotas_no_CheckedChanged(object sender, EventArgs e)
		{
			if (radio_selectcontroller_dhotas_no.Checked)
			{
				selectController_hotas = 0;
				btn_configure_hotas.Visible = false;
				SavedWizardSettings.selectController_hotas = 0;
				SavedWizardSettings.Save(wizardSettingsJson);
				checkControllerStatus();
				lbl_ffb_hotas.Visible = false;
				cmb_ffbguid_hotas.Visible = false;
			}
		}

		private void radio_selectcontroller_gun_no_CheckedChanged(object sender, EventArgs e)
		{
			btn_configure_gunA.Visible = btn_configure_gunB.Visible = false;
			if (cmb_gunA_type.SelectedIndex >= 0 && !radio_selectcontroller_gun_no.Checked) btn_configure_gunA.Visible = true;
			if (cmb_gunB_type.SelectedIndex >= 0 && radio_selectcontroller_gun_2.Checked) btn_configure_gunB.Visible = true;

			if (radio_selectcontroller_gun_no.Checked)
			{
				selectController_lightgun = 0;
				kryptonLabel23.Visible = cmb_gunA_type.Visible = false;
				kryptonLabel24.Visible = cmb_gunB_type.Visible = false;
				btn_configure_gunA.Visible = false;
				btn_configure_gunB.Visible = false;
				SavedWizardSettings.selectController_lightgun = 0;
				SavedWizardSettings.Save(wizardSettingsJson);
				checkControllerStatus();
			}
		}

		private void radio_selectcontroller_gun_1_CheckedChanged(object sender, EventArgs e)
		{
			btn_configure_gunA.Visible = btn_configure_gunB.Visible = false;
			if (cmb_gunA_type.SelectedIndex >= 0 && !radio_selectcontroller_gun_no.Checked) btn_configure_gunA.Visible = true;
			if (cmb_gunB_type.SelectedIndex >= 0 && radio_selectcontroller_gun_2.Checked) btn_configure_gunB.Visible = true;

			if (radio_selectcontroller_gun_1.Checked)
			{
				selectController_lightgun = 1;
				kryptonLabel23.Visible = cmb_gunA_type.Visible = true;
				kryptonLabel24.Visible = cmb_gunB_type.Visible = false;
				SavedWizardSettings.selectController_lightgun = 1;
				SavedWizardSettings.Save(wizardSettingsJson);
				checkControllerStatus();
			}
		}

		private void radio_selectcontroller_gun_2_CheckedChanged(object sender, EventArgs e)
		{
			btn_configure_gunA.Visible = btn_configure_gunB.Visible = false;
			if (cmb_gunA_type.SelectedIndex >= 0 && !radio_selectcontroller_gun_no.Checked) btn_configure_gunA.Visible = true;
			if (cmb_gunB_type.SelectedIndex >= 0 && radio_selectcontroller_gun_2.Checked) btn_configure_gunB.Visible = true;

			if (radio_selectcontroller_gun_2.Checked)
			{
				selectController_lightgun = 2;
				kryptonLabel23.Visible = cmb_gunA_type.Visible = true;
				kryptonLabel24.Visible = cmb_gunB_type.Visible = true;
				SavedWizardSettings.selectController_lightgun = 2;
				SavedWizardSettings.Save(wizardSettingsJson);
				checkControllerStatus();
			}
		}

		private void radio_selectcontroller_arcadestick_no_CheckedChanged(object sender, EventArgs e)
		{
			if (radio_selectcontroller_arcadestick_no.Checked)
			{
				groupBox_xinputarcade.Enabled = false;
				timer_controllerUpdate.Enabled = false;
				SavedWizardSettings.selectController_xinput = 0;
				SavedWizardSettings.Save(wizardSettingsJson);
				checkControllerStatus();
			}
		}

		private void btn_configure_wheel_Click(object sender, EventArgs e)
		{
			var frm = new dinputwheel(dwheel_config);
			var result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{
				dwheel_config = frm.Dialogconfig;
				IsWheelConfigured = frm.IsConfigured;
				SavedWizardSettings.IsWheelConfigured = IsWheelConfigured;
				SavedWizardSettings.dwheel_config = dwheel_config;
				SavedWizardSettings.Save(wizardSettingsJson);
				checkControllerStatus();
			}
		}

		private void txt_arcadeXinputData_TextChanged(object sender, EventArgs e)
		{
			SavedWizardSettings.arcadeXinputData = txt_arcadeXinputData.Text;
			SavedWizardSettings.Save(wizardSettingsJson);
		}

		private void btn_configure_shifter_Click(object sender, EventArgs e)
		{
			var frm = new dinputshifter(dshifter_config);
			var result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{
				dshifter_config = frm.Dialogconfig;
				IsShifterConfigured = frm.IsConfigured;
				SavedWizardSettings.IsShifterConfigured = IsShifterConfigured;
				SavedWizardSettings.dshifter_config = dshifter_config;
				SavedWizardSettings.Save(wizardSettingsJson);
				checkControllerStatus();
			}
		}

		private void btn_configure_hotas_Click(object sender, EventArgs e)
		{
			var frm = new dinputhotas(dhotas_config);
			var result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{
				dhotas_config = frm.Dialogconfig;
				IsHotasConfigured = frm.IsConfigured;
				SavedWizardSettings.IsHotasConfigured = IsHotasConfigured;
				SavedWizardSettings.dhotas_config = dhotas_config;
				SavedWizardSettings.Save(wizardSettingsJson);
				checkControllerStatus();
			}
		}


		private void cmb_gunA_type_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!SavedWizardSettings.disableSave)
			{
				gunA_json = "";
				gunA_comport = -1;
				IsGunAConfigured = false;
				SavedWizardSettings.IsGunAConfigured = false;
				SavedWizardSettings.gunA_json = "";
				SavedWizardSettings.gunA_comport = -1;
				checkControllerStatus();
			}
			SavedWizardSettings.selectController_guntypeA = cmb_gunA_type.SelectedIndex;
			SavedWizardSettings.Save(wizardSettingsJson);
			if (cmb_gunA_type.SelectedIndex >= 0 && !radio_selectcontroller_gun_no.Checked) btn_configure_gunA.Visible = true;
			else btn_configure_gunA.Visible = false;
		}

		private void cmb_gunB_type_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!SavedWizardSettings.disableSave)
			{
				gunB_json = "";
				gunB_comport = -1;
				IsGunBConfigured = false;
				SavedWizardSettings.IsGunBConfigured = false;
				SavedWizardSettings.gunB_json = "";
				SavedWizardSettings.gunB_comport = -1;
				checkControllerStatus();
			}
			SavedWizardSettings.selectController_guntypeB = cmb_gunB_type.SelectedIndex;
			SavedWizardSettings.Save(wizardSettingsJson);
			if (cmb_gunB_type.SelectedIndex >= 0 && radio_selectcontroller_gun_2.Checked) btn_configure_gunB.Visible = true;
			else btn_configure_gunB.Visible = false;

		}


		private void btn_configure_gunA_Click(object sender, EventArgs e)
		{
			string config = gunA_json;
			int selected_index = cmb_gunA_type.SelectedIndex;
			if (selected_index <= 3)
			{
				var frm = new gun_preconfig(selected_index, 1, config);
				var result = frm.ShowDialog();
				if (result == DialogResult.OK)
				{
					gunA_json = frm.Dialogconfig;
					IsGunAConfigured = true;

					SavedWizardSettings.gunA_json = gunA_json;
					SavedWizardSettings.IsGunAConfigured = IsGunAConfigured;
					SavedWizardSettings.Save(wizardSettingsJson);
					checkControllerStatus();
				}
			}
			else
			{
				string typeGunTxt = "guncon1";
				if (selected_index == 4) { typeGunTxt = "guncon1"; }
				if (selected_index == 5) { typeGunTxt = "guncon2"; }
				if (selected_index == 6) { typeGunTxt = "wiimote"; }

				var frm = new dinputgun(1, typeGunTxt, config);
				var result = frm.ShowDialog();
				if (result == DialogResult.OK)
				{
					var json = frm.Dialogconfig;
					gunA_json = json;
					IsGunAConfigured = frm.IsConfigured;
					SavedWizardSettings.gunA_json = gunA_json;
					SavedWizardSettings.IsGunAConfigured = IsGunAConfigured;
					SavedWizardSettings.Save(wizardSettingsJson);
					checkControllerStatus();
				}
			}
		}


		private void btn_configure_gunB_Click(object sender, EventArgs e)
		{
			string config = gunB_json;
			int selected_index = cmb_gunB_type.SelectedIndex;
			if (selected_index <= 3)
			{
				var frm = new gun_preconfig(selected_index, 2, config);
				var result = frm.ShowDialog();
				if (result == DialogResult.OK)
				{
					gunB_json = frm.Dialogconfig;
					IsGunBConfigured = true;

					SavedWizardSettings.gunB_json = gunB_json;
					SavedWizardSettings.IsGunBConfigured = IsGunBConfigured;
					SavedWizardSettings.Save(wizardSettingsJson);
					checkControllerStatus();
				}
			}
			else
			{
				string typeGunTxt = "guncon1";
				if (selected_index == 4) { typeGunTxt = "guncon1"; }
				if (selected_index == 5) { typeGunTxt = "guncon2"; }
				if (selected_index == 6) { typeGunTxt = "wiimote"; }

				var frm = new dinputgun(2, typeGunTxt, config);
				var result = frm.ShowDialog();
				if (result == DialogResult.OK)
				{
					var json = frm.Dialogconfig;
					gunB_json = json;
					IsGunBConfigured = frm.IsConfigured;
					SavedWizardSettings.gunB_json = gunB_json;
					SavedWizardSettings.IsGunBConfigured = IsGunBConfigured;
					SavedWizardSettings.Save(wizardSettingsJson);
					checkControllerStatus();
				}
			}

		}
		#endregion




		#region Utils

		//Utils

		private List<string> VerifyFiles(string directoryPath, Dictionary<string, string> expectedFiles)
		{
			var missingFiles = new List<string>();
			var currentFiles = Directory.GetFiles(directoryPath);
			var currentFilesSet = new HashSet<string>(currentFiles);

			foreach (var filePath in currentFiles)
			{
				var fileInfo = new FileInfo(filePath);
				var fileName = Path.GetFileName(filePath);

				if (expectedFiles.ContainsKey(fileName))
				{
					if (!cacheCheckFix.TryGetValue(fileName, out var metadata) || metadata.LastModified != fileInfo.LastWriteTime || metadata.Size != fileInfo.Length)
					{
						metadata = new CacheMetadata
						{
							LastModified = fileInfo.LastWriteTime,
							Size = fileInfo.Length,
							Md5 = GetMd5HashAsString(filePath)
						};
						cacheCheckFix[fileName] = metadata;
					}

					var isOk = metadata.Md5 == expectedFiles[fileName];
					if (!isOk)
					{
						missingFiles.Add($"{fileName} (Mismatch)");
					}
				}
			}

			foreach (var expectedFile in expectedFiles.Keys)
			{
				if (!currentFilesSet.Contains(Path.Combine(directoryPath, expectedFile)))
				{
					missingFiles.Add($"{expectedFile} (Missing)");
				}
			}

			return missingFiles;
		}

		private void UpdateGamePadList()
		{


			_connectedGamePad.Clear();
			var gamepad = X.Gamepad_1;
			if (gamepad.Capabilities.Type != 0) _connectedGamePad.Add(0, new XinputGamepad(gamepad, 0, false, "Type=Wheel", txt_arcadeXinputData.Text, "Type=Gamepad"));
			gamepad = X.Gamepad_2;
			if (gamepad.Capabilities.Type != 0) _connectedGamePad.Add(1, new XinputGamepad(gamepad, 1, false, "Type=Wheel", txt_arcadeXinputData.Text, "Type=Gamepad"));
			gamepad = X.Gamepad_3;
			if (gamepad.Capabilities.Type != 0) _connectedGamePad.Add(2, new XinputGamepad(gamepad, 2, false, "Type=Wheel", txt_arcadeXinputData.Text, "Type=Gamepad"));
			gamepad = X.Gamepad_4;
			if (gamepad.Capabilities.Type != 0) _connectedGamePad.Add(3, new XinputGamepad(gamepad, 3, false, "Type=Wheel", txt_arcadeXinputData.Text, "Type=Gamepad"));

			X.StartPolling(X.Gamepad_1, X.Gamepad_2, X.Gamepad_3, X.Gamepad_4);
			bool isPolling = true;
			if (isPolling && X.Gamepad_1.Start_down)
			{
				int slot = 0;
				isPolling = false;
				X.StopPolling();
				if (_connectedGamePad.ContainsKey(slot))
				{
					var pad = _connectedGamePad[slot];
					if (pad.Type != "arcade")
					{
						string ControllerName = pad.ControllerName;
						if (!ControllerName.StartsWith("Microsoft X-Box 360"))
						{
							txt_arcadeXinputData.Text += $",VendorID=0x{pad.VendorId:X04}<>ProductID=0x{pad.ProductId:X04}";
						}
					}
				}
			}
			if (isPolling && X.Gamepad_2.Start_down)
			{
				int slot = 1;
				isPolling = false;
				X.StopPolling();
				if (_connectedGamePad.ContainsKey(slot))
				{
					var pad = _connectedGamePad[slot];
					if (pad.Type != "arcade")
					{
						string ControllerName = pad.ControllerName;
						if (!ControllerName.StartsWith("Microsoft X-Box 360"))
						{
							txt_arcadeXinputData.Text += $",VendorID=0x{pad.VendorId:X04}<>ProductID=0x{pad.ProductId:X04}";
						}
					}
				}
			}
			if (isPolling && X.Gamepad_3.Start_down)
			{
				int slot = 2;
				isPolling = false;
				X.StopPolling();
				if (_connectedGamePad.ContainsKey(slot))
				{
					var pad = _connectedGamePad[slot];
					if (pad.Type != "arcade")
					{
						string ControllerName = pad.ControllerName;
						if (!ControllerName.StartsWith("Microsoft X-Box 360"))
						{
							txt_arcadeXinputData.Text += $",VendorID=0x{pad.VendorId:X04}<>ProductID=0x{pad.ProductId:X04}";
						}
					}
				}
			}
			if (isPolling && X.Gamepad_4.Start_down)
			{
				int slot = 3;
				isPolling = false;
				X.StopPolling();
				if (_connectedGamePad.ContainsKey(slot))
				{
					var pad = _connectedGamePad[slot];
					if (pad.Type != "arcade")
					{
						string ControllerName = pad.ControllerName;
						if (!ControllerName.StartsWith("Microsoft X-Box 360"))
						{
							txt_arcadeXinputData.Text += $",VendorID=0x{pad.VendorId:X04}<>ProductID=0x{pad.ProductId:X04}";
						}
					}
				}
			}

			if (isPolling) X.StopPolling();

		}

		public static string GetRelativeDirectoryPath(string fromPath, string toPath)
		{
			Uri fromUri = new Uri(fromPath);
			Uri toUri = new Uri(toPath);

			// Make sure toUri is absolute
			if (!toUri.IsAbsoluteUri)
			{
				toUri = new Uri(Path.GetFullPath(toUri.OriginalString));
			}

			// Make sure fromUri is absolute
			if (!fromUri.IsAbsoluteUri)
			{
				fromUri = new Uri(Path.GetFullPath(fromUri.OriginalString));
			}

			// Get relative URI
			Uri relativeUri = fromUri.MakeRelativeUri(toUri);

			// Convert the relative URI to a string and return
			string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

			// Replace forward slashes with backslashes if needed
			relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar);

			// Get the directory part of the relative path
			string directoryPath = Path.GetDirectoryName(relativePath);

			// If the directory path is null or empty, return "./"
			if (string.IsNullOrEmpty(directoryPath))
			{
				return ".";
			}

			return directoryPath;
		}

		private string GetMd5HashAsString(string FileName)
		{
			if (cacheMD5.ContainsKey(FileName))
			{
				return cacheMD5[FileName];
			}
			if (File.Exists(FileName))
			{
				using (var md5 = MD5.Create())
				{
					using (var stream = File.OpenRead(FileName))
					{
						var hash = md5.ComputeHash(stream);
						string md5val = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant().ToUpper();
						cacheMD5.Add(FileName, md5val);
						return md5val;
					}
				}
			}
			return "";
		}

		static string GetCommonPath(string path1, string path2)
		{
			string[] parts1 = path1.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			string[] parts2 = path2.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			int minLength = Math.Min(parts1.Length, parts2.Length);
			int commonLength = 0;

			for (int i = 0; i < minLength; i++)
			{
				if (parts1[i].Equals(parts2[i], StringComparison.OrdinalIgnoreCase))
				{
					commonLength++;
				}
				else
				{
					break;
				}
			}

			return string.Join(Path.DirectorySeparatorChar.ToString(), parts1.Take(commonLength));
		}

		public async Task<string> SimpleExtractArchiveWithProgressAsync(string fileToExtract, string dirOut)
		{


			progress_extract.Minimum = 0;
			progress_extract.Maximum = 100;
			progress_extract.Value = 0;
			progress_extract.Visible = true;
			progress_extract.Text = Path.GetFileName(fileToExtract);
			progress_extract.Show();


			//RomExtractor_ArchiveFile.progress = progress;
			try
			{
				var result = await Cli.Wrap(SevenZipExe)
					.WithArguments(new[] { "x", fileToExtract, @"-o" + dirOut, "-bsp1", "-y" })
					.WithStandardOutputPipe(PipeTarget.ToDelegate(handleStdOut))
					.ExecuteAsync();

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			return dirOut;
		}

		private void MonitorProgress()
		{
			while (isProcessing || progress < totalCount)
			{
				this.Invoke(new Action(() =>
				{
					// Calculer le pourcentage de progression
					int percentage = (int)((double)progress / totalCount * 100);
					progress_scan.Value = Math.Min(percentage, 100);
				}));

				// Attendre 100ms avant la prochaine mise à jour
				Thread.Sleep(100);
			}
		}

		private bool CheckRequiredFixes()
		{
			if (!Directory.Exists(fixesDir)) Directory.CreateDirectory(fixesDir);
			lbl_foldernameTPfixes.Text = fixesDir;
			return true;
		}

		private bool CheckInstalledSoftware()
		{

			btn_next.Enabled = verifDataTab1();

			bool vigem = false;
			string vigemPath = Utils.checkInstalled("ViGEm");
			if (!string.IsNullOrEmpty(vigemPath)) vigem = true;

			bool vjoy = false;
			string vjoyPath = Utils.checkInstalled("vJoy");
			if (!string.IsNullOrEmpty(vjoyPath)) vjoy = true;

			bool riva = false;
			string rivaPath = Utils.checkInstalled("RivaTuner Statistics");
			if (!string.IsNullOrEmpty(rivaPath))
			{
				riva = true;
			}

			bool xenos = false;
			string XenosPath = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "xenos", "Xenos.exe");
			if (File.Exists(XenosPath)) xenos = true;

			if (vigem)
			{
				lbl_installed_vigem.Text = "Installed";
				lbl_installed_vigem.ForeColor = Color.DarkGreen;
			}
			if (vjoy)
			{
				lbl_installed_vjoy.Text = "Installed";
				lbl_installed_vjoy.ForeColor = Color.DarkGreen;
			}
			if (riva)
			{
				lbl_installed_rivatuner.Text = "Installed";
				lbl_installed_rivatuner.ForeColor = Color.DarkGreen;
			}
			if (xenos)
			{
				lbl_installed_xenos.Text = "Installed";
				lbl_installed_xenos.ForeColor = Color.DarkGreen;
			}
			if (vigem && vjoy && riva && xenos)
			{

				if (tabControl1.SelectedTab.Text == "tabPage0") btn_next.Enabled = true;
				return true;
			}
			return false;


		}


		private void InitializeHandleStdOut()
		{
			handleStdOut = delegate (string msg)
			{
				string pattern = @"([0-9]*)%";
				string input = msg;

				Match match = Regex.Match(input, pattern);

				if (match.Success)
				{
					string percentage = match.Groups[1].Value;

					int progress = 0;
					if (int.TryParse(percentage, out progress))
					{
						progress_extract.Invoke(new MethodInvoker(delegate
						{

							progress_extract.Value = progress;
							progress_extract.Update();

						}));
					}
				}
				if (msg.Contains("Everything is Ok"))
				{
					int progress = 100;
					progress_extract.Invoke(new MethodInvoker(delegate
					{

						progress_extract.Visible = false;

					}));
				}
			};
		}
		#endregion

		private void btn_configure_wheel_VisibleChanged(object sender, EventArgs e)
		{

		}

		private void btn_configure_shifter_VisibleChanged(object sender, EventArgs e)
		{

		}

		private void btn_configure_hotas_VisibleChanged(object sender, EventArgs e)
		{

		}

		private void btn_configure_gunA_VisibleChanged(object sender, EventArgs e)
		{
		}

		private void btn_configure_gunB_VisibleChanged(object sender, EventArgs e)
		{
		}

		private void kryptonLabel31_Click(object sender, EventArgs e)
		{

		}

		private void timer_ffb_update_Tick(object sender, EventArgs e)
		{
			if (this == Form.ActiveForm && tabControl1.SelectedIndex == 2)
			{
				UpdateFFBGuidList();
			}
		}

		private void cmb_ffbguid_wheel_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!SavedWizardSettings.disableSave)
			{
				ffbWheel = (FFBDevice)cmb_ffbguid_wheel.SelectedItem;
				SavedWizardSettings.ffbWheel_guid = ffbWheel.Guid;
				SavedWizardSettings.ffbWheel_name = ffbWheel.Name;
				SavedWizardSettings.Save(wizardSettingsJson);
			}
		}

		private void cmb_ffbguid_hotas_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!SavedWizardSettings.disableSave)
			{
				ffbHotas = (FFBDevice)cmb_ffbguid_hotas.SelectedItem;
				SavedWizardSettings.ffbHotas_guid = ffbHotas.Guid;
				SavedWizardSettings.ffbHotas_name = ffbHotas.Name;
				SavedWizardSettings.Save(wizardSettingsJson);
			}
		}

		private void Wizard_FormClosing(object sender, FormClosingEventArgs e)
		{
			SavedWizardSettings.mariokartId = txt_mariokartId.Text;
			SavedWizardSettings.apm3id = txt_apm3id.Text;
			SavedWizardSettings.customName = txt_customName.Text;
			SavedWizardSettings.Save(wizardSettingsJson);

			DialogResult result = MessageBox.Show("Save the Wizard settings to resume later ?",
				"Warning",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Warning
			);

			if (result == DialogResult.No)
			{
				if (File.Exists(wizardSettingsJson))
				{
					File.Delete(wizardSettingsJson);
				}

			}
		}

		private void cmb_gpu_SelectedIndexChanged(object sender, EventArgs e)
		{
			kryptonLinkLabel1.Visible = false;
			lbl_note_gpu.Text = "";
			if (cmb_gpu.SelectedIndex == 2) lbl_note_gpu.Text = "Version < 22.7.1";
			if (cmb_gpu.SelectedIndex == 3) lbl_note_gpu.Text = "Version > 22.7.1, due to change in amd opengl, some games wont work";
			if (cmb_gpu.SelectedIndex == 4)
			{
				kryptonLinkLabel1.Visible = true;
				lbl_note_gpu.Text = "Custom version of AMD drivers for Polaris/Vega/Navi based on 23.12.1.\nThose driver made possible to sideload the old opengl version.";
			}
			if (cmb_gpu.SelectedIndex >= 0)
			{
				lbl_gpu_status.ForeColor = Color.DarkGreen;
				lbl_gpu_status.Text = "Ok";
			}
			if (!SavedWizardSettings.disableSave)
			{
				SavedWizardSettings.gpu = cmb_gpu.SelectedIndex;
				SavedWizardSettings.Save(wizardSettingsJson);
			}
			grp_finalize.Enabled = verifDataTab4();

		}

		private void cmb_resolution_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cmb_resolution.SelectedIndex >= 0)
			{
				lbl_resolution_status.ForeColor = Color.DarkGreen;
				lbl_resolution_status.Text = "Ok";
			}
			if (!SavedWizardSettings.disableSave)
			{
				SavedWizardSettings.resolution = cmb_resolution.SelectedIndex;
				SavedWizardSettings.Save(wizardSettingsJson);
			}
			grp_finalize.Enabled = verifDataTab4();
		}

		private void btn_apm3id_show_Click(object sender, EventArgs e)
		{
			if (txt_apm3id.PasswordChar == '*') txt_apm3id.PasswordChar = '\0';
			else txt_apm3id.PasswordChar = '*';
		}

		private void btn_mariokartId_show_Click(object sender, EventArgs e)
		{
			if (txt_mariokartId.PasswordChar == '*') txt_mariokartId.PasswordChar = '\0';
			else txt_mariokartId.PasswordChar = '*';
		}

		private void kryptonButton1_Click(object sender, EventArgs e)
		{
			// Ouvrir l'URL dans le navigateur par défaut, ce qui devrait lancer Discord si configuré
			Process.Start(new ProcessStartInfo
			{
				FileName = "https://teknoparrot.com/OnlineProfile/MKDX",
				UseShellExecute = true
			});
		}

		private void kryptonButton2_Click(object sender, EventArgs e)
		{
			Process.Start(new ProcessStartInfo
			{
				FileName = "https://teknoparrot.com/OnlineProfile/APM3",
				UseShellExecute = true
			});

		}

		private void txt_apm3id_TextChanged(object sender, EventArgs e)
		{
			if (txt_apm3id.Text != "" && txt_mariokartId.Text != "" && txt_customName.Text != "")
			{
				lbl_onlineprofil_status.ForeColor = Color.DarkGreen;
				lbl_onlineprofil_status.Text = "Ok";
			}
			else
			{
				lbl_onlineprofil_status.ForeColor = Color.Red;
				lbl_onlineprofil_status.Text = "Not Ok";
			}
			grp_finalize.Enabled = verifDataTab4();
		}

		private void txt_mariokartId_TextChanged(object sender, EventArgs e)
		{
			if (txt_apm3id.Text != "" && txt_mariokartId.Text != "" && txt_customName.Text != "")
			{
				lbl_onlineprofil_status.ForeColor = Color.DarkGreen;
				lbl_onlineprofil_status.Text = "Ok";
			}
			else
			{
				lbl_onlineprofil_status.ForeColor = Color.Red;
				lbl_onlineprofil_status.Text = "Not Ok";
			}
			grp_finalize.Enabled = verifDataTab4();
		}

		private void txt_customName_TextChanged(object sender, EventArgs e)
		{
			if (txt_apm3id.Text != "" && txt_mariokartId.Text != "" && txt_customName.Text != "")
			{
				lbl_onlineprofil_status.ForeColor = Color.DarkGreen;
				lbl_onlineprofil_status.Text = "Ok";
			}
			else
			{
				lbl_onlineprofil_status.ForeColor = Color.Red;
				lbl_onlineprofil_status.Text = "Not Ok";
			}
			grp_finalize.Enabled = verifDataTab4();
		}

		private void btn_finalize_Click(object sender, EventArgs e)
		{
			if(chk_installpatch.Checked)
			{
				var frm = new PatchInstall(txt_patchArchive.Text, fixesDir, Path.Combine(txt_tpfolder.Text, "AutoXinputLinks"), txt_linksourcefolderexe.Text);
				var result = frm.ShowDialog();
				if (result == DialogResult.OK)
				{
					MessageBox.Show("Ok !");
				}
				else
				{
					MessageBox.Show("Exaction Not Ok, Abording");
					return;
				}
			}

			var NewConfig = new Configuration();
			NewConfig.TpFolder = txt_tpfolder.Text;
			NewConfig.perGameLinkFolderExe = txt_linksourcefolderexe.Text;
			NewConfig.perGameLinkFolder = Path.Combine(txt_tpfolder.Text, "AutoXinputLinks");
			NewConfig.virtualKeyboard = true;
			NewConfig.keyTest = "F1";
			NewConfig.keyService1 = "F2";
			NewConfig.keyService2 = "F3";
			NewConfig.ShowAllGames = true;
			NewConfig.FFB = true;
			NewConfig.arcadeXinputData = txt_arcadeXinputData.Text;
			if (selectController_wheel > 0)
			{
				NewConfig.bindingDinputWheel = dwheel_config;
				NewConfig.useDinputWheel = true;
				NewConfig.ffbDinputWheel = ffbWheel.Guid;
			}
			if(selectController_wheel == 2)
			{
				NewConfig.bindingDinputShifter = dshifter_config;
				NewConfig.useDinputShifter= true;
			}
			if(selectController_hotas == 1)
			{
				NewConfig.useDinputHotas = true;
				NewConfig.bindingDinputHotas = dhotas_config;
				NewConfig.ffbDinputHotas = ffbHotas.Guid;
				if(selectController_wheel > 0)
				{
					NewConfig.useHotasWithWheel = true;
				}
			}
			if(selectController_lightgun > 0)
			{
				string typeGunTxt = "";
				int gunType = cmb_gunA_type.SelectedIndex;

				if (gunType == 0) { typeGunTxt = "guncon2"; }
				if (gunType == 1) { typeGunTxt = "guncon1"; }
				if (gunType == 2) { typeGunTxt = "sinden"; }
				if (gunType == 3) { typeGunTxt = "wiimote"; }
				if (gunType == 4) { typeGunTxt = "guncon1"; }
				if (gunType == 5) { typeGunTxt = "guncon2"; }
				if (gunType == 6) { typeGunTxt = "wiimote"; }
				NewConfig.gunAType = typeGunTxt;

				if (typeGunTxt == "guncon1") NewConfig.bindingDinputGunAGuncon1 = gunA_json;
				if (typeGunTxt == "guncon2") NewConfig.bindingDinputGunAGuncon2 = gunA_json;
				if (typeGunTxt == "sinden") NewConfig.bindingDinputGunASinden = gunA_json;
				if (typeGunTxt == "wiimote") NewConfig.bindingDinputGunAWiimote = gunA_json;

				if(gunType == 0 || gunType == 1)
				{
					NewConfig.gunARecoil = "gun4ir";
					NewConfig.gunAComPort = gunA_comport;
					NewConfig.gunAdomagerumble = true;
					NewConfig.gunAAutoJoy = true;
				}
				if(gunType == 2)
				{
					NewConfig.gunARecoil = "sinden-gun1";
				}
			}
			if (selectController_lightgun == 2)
			{
				string typeGunTxt = "";
				int gunType = cmb_gunB_type.SelectedIndex;

				if (gunType == 0) { typeGunTxt = "guncon2"; }
				if (gunType == 1) { typeGunTxt = "guncon1"; }
				if (gunType == 2) { typeGunTxt = "sinden"; }
				if (gunType == 3) { typeGunTxt = "wiimote"; }
				if (gunType == 4) { typeGunTxt = "guncon1"; }
				if (gunType == 5) { typeGunTxt = "guncon2"; }
				if (gunType == 6) { typeGunTxt = "wiimote"; }
				NewConfig.gunBType = typeGunTxt;

				if (typeGunTxt == "guncon1") NewConfig.bindingDinputGunAGuncon1 = gunB_json;
				if (typeGunTxt == "guncon2") NewConfig.bindingDinputGunAGuncon2 = gunB_json;
				if (typeGunTxt == "sinden") NewConfig.bindingDinputGunASinden = gunB_json;
				if (typeGunTxt == "wiimote") NewConfig.bindingDinputGunAWiimote = gunB_json;

				if (gunType == 0 || gunType == 1)
				{
					NewConfig.gunBRecoil = "gun4ir";
					NewConfig.gunBComPort = gunB_comport;
					NewConfig.gunBdomagerumble = true;
					NewConfig.gunBAutoJoy = true;
				}
				if (gunType == 2)
				{
					NewConfig.gunBRecoil = "sinden-gun2";
				}
			}
			string rivaPath = Utils.checkInstalled("RivaTuner Statistics");
			if (string.IsNullOrEmpty(rivaPath))
			{
				NewConfig.rivatunerExe = Path.Combine(Path.GetDirectoryName(rivaPath), "RTTS.exe");
			}

			string XenosPath = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "xenos", "Xenos.exe");
			if (File.Exists(XenosPath))
			{
				NewConfig.useXenos = true;
			}

			NewConfig.gpuType = cmb_gpu.SelectedIndex;
			NewConfig.gpuResolution = cmb_resolution.SelectedIndex;
			NewConfig.patch_apm3id = txt_apm3id.Text;
			NewConfig.patch_mariokartId = txt_mariokartId.Text;
			NewConfig.patch_customName = txt_customName.Text;
			ConfigurationManager.MainConfig = NewConfig;
			ConfigurationManager.SaveConfig();



		}
	}
}

public class FFBDevice
{
	public string Name = "";
	public string Guid = "";
	public override string ToString()
	{
		return Name;
	}

	public override bool Equals(object obj)
	{
		if (obj == null || GetType() != obj.GetType())
			return false;

		FFBDevice other = (FFBDevice)obj;
		return Guid == other.Guid;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Guid);
	}
}

class GameInstall
{
	public string gameId = "";
	public string gameName = "";
	public bool isArchive = false;
	public string source = "";
	public string mainExecutable = "";
	public string secondaryExecutable = "";
	public string minimalDirArchive;
	public string destinationArchive = "";
	public override string ToString()
	{
		return $"{gameName} => {source}";
	}
}

public class CacheMetadata
{
	public DateTime LastModified { get; set; }
	public long Size { get; set; }
	public string Md5 { get; set; }
}
public class WizardSettings
{
	public bool disableSave = false;
	public int start_tab = 0;
	public string tpfolder = "";
	public string linksourcefolderexe = "";
	public string dwheel_config = "";
	public string dshifter_config = "";
	public string dhotas_config = "";
	public string arcadeXinputData = "";

	public int selectController_xinput = 0;
	public int selectController_wheel = 0;
	public int selectController_hotas = 0;
	public int selectController_lightgun = 0;
	public int selectController_guntypeA = -1;
	public int selectController_guntypeB = -1;

	public int gunA_type = -1;
	public string gunA_json = "";
	public int gunA_comport = 0;

	public int gunB_comport = 0;
	public int gunB_type = -1;
	public string gunB_json = "";

	public string patchArchive = "";

	public bool IsWheelConfigured = false;
	public bool IsShifterConfigured = false;
	public bool IsHotasConfigured = false;
	public bool IsGunAConfigured = false;
	public bool IsGunBConfigured = false;

	public string ffbWheel_guid = "";
	public string ffbWheel_name = "<none>";

	public string ffbHotas_guid = "";
	public string ffbHotas_name = "<none>";

	public int gpu = -1;
	public int resolution = -1;

	public string apm3id = "";
	public string mariokartId = "";
	public string customName = "";


	public WizardSettings()
	{

	}
	public WizardSettings(string filename) 
	{
		if (File.Exists(filename))
		{
			try
			{
				WizardSettings DeserializeData = JsonConvert.DeserializeObject<WizardSettings>(File.ReadAllText(filename));
				this.start_tab = DeserializeData.start_tab;
				this.tpfolder = DeserializeData.tpfolder;
				this.linksourcefolderexe = DeserializeData.linksourcefolderexe;
				this.dwheel_config = DeserializeData.dwheel_config;
				this.dshifter_config= DeserializeData.dshifter_config;
				this.dhotas_config = DeserializeData.dhotas_config;
				this.arcadeXinputData = DeserializeData.arcadeXinputData;

				this.selectController_xinput = DeserializeData.selectController_xinput;
				this.selectController_wheel = DeserializeData.selectController_wheel;
				this.selectController_hotas = DeserializeData.selectController_hotas;
				this.selectController_lightgun = DeserializeData.selectController_lightgun;
				this.selectController_guntypeA = DeserializeData.selectController_guntypeA;
				this.selectController_guntypeB = DeserializeData.selectController_guntypeB;

				this.gunA_type = DeserializeData.gunA_type;
				this.gunA_json = DeserializeData.gunA_json;
				this.gunA_comport = DeserializeData.gunA_comport;


				this.gunB_type = DeserializeData.gunB_type;
				this.gunB_json = DeserializeData.gunB_json;
				this.gunB_comport = DeserializeData.gunB_comport;

				this.patchArchive = DeserializeData.patchArchive;
				this.IsWheelConfigured = DeserializeData.IsWheelConfigured;
				this.IsShifterConfigured = DeserializeData.IsShifterConfigured;
				this.IsHotasConfigured = DeserializeData.IsHotasConfigured;
				this.IsGunAConfigured = DeserializeData.IsGunAConfigured;
				this.IsGunBConfigured = DeserializeData.IsGunBConfigured;

				this.ffbWheel_guid = DeserializeData.ffbWheel_guid;
				this.ffbWheel_name = DeserializeData.ffbWheel_name;
				this.ffbHotas_guid = DeserializeData.ffbHotas_guid;
				this.ffbHotas_name = DeserializeData.ffbHotas_name;

				this.gpu = DeserializeData.gpu;
				this.resolution = DeserializeData.resolution;
				this.apm3id = DeserializeData.apm3id;
				this.mariokartId = DeserializeData.mariokartId;
				this.customName = DeserializeData.customName;
			

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}

	public string Serialize()
	{
		string json = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
		return json;
	}

	public void Save(string filename)
	{
		
		if(!disableSave) File.WriteAllText(filename, this.Serialize());
	}


}