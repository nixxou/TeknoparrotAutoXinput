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

namespace TeknoparrotAutoXinput
{
	public partial class Wizard : KryptonForm
	{
		Region tabOriginalRegion;
		string currentDir = Path.GetFullPath(Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
		List<string> gameExecutableList = new List<string>();
		bool allRequiredSoftInstalled = false;
		public Wizard()
		{
			InitializeComponent();
		}

		private void Wizard_Load(object sender, EventArgs e)
		{
			tabOriginalRegion = tabControl1.Region;
			tabControl1.Region = new Region(tabControl1.DisplayRectangle);
			btn_next.Enabled = false;
			btn_previous.Enabled = false;
			CheckInstalledSoftware();




		}

		private void btn_next_Click(object sender, EventArgs e)
		{
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

		private bool verifDataTab1()
		{
			btn_next.Enabled = false;
			btn_previous.Enabled = true;
			return true;
		}

		private bool verifDataTab2()
		{
			MessageBox.Show("test2");
			// Ajoutez ici la logique de vérification pour le Tab 2
			return true; // Remplacez par la logique réelle
		}

		private void kryptonGroup1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void tabPage1_Click(object sender, EventArgs e)
		{

		}

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
					string tpui = Path.Combine(fbd.SelectedPath, "TeknoParrotUi.exe");
					if (File.Exists(tpui))
					{
						txt_tpfolder.Text = fbd.SelectedPath;
					}
					else
					{
						MessageBox.Show($"Can't find {tpui}");
					}

				}
			}
		}

		private void txt_tpfolder_TextChanged(object sender, EventArgs e)
		{
			if (txt_tpfolder.Text != "")
			{
				groupBox2.Enabled = true;
				string userDir = Path.Combine(txt_tpfolder.Text, "UserProfiles");
				if (Directory.Exists(userDir))
				{
					var cfgList = Directory.GetFiles(userDir, "*.xml");
					foreach (var cfg in cfgList)
					{
						if (gameExecutableList.Count > 30) continue;
						string OriginalXML = File.ReadAllText(cfg);
						var xmlDoc = new XmlDocument();
						xmlDoc.LoadXml(OriginalXML);
						try
						{
							XmlNode gamePathNode = xmlDoc.SelectSingleNode("/GameProfile/GamePath");
							if (gamePathNode != null)
							{
								string gamePathContent = gamePathNode.InnerText;
								if (gamePathContent != "" && File.Exists(gamePathContent))
								{
									if (!gameExecutableList.Contains(gamePathContent))
									{
										gameExecutableList.Add(gamePathContent);
									}
								}
							}
						}
						catch (Exception ex) { }
					}
				}
				if (gameExecutableList.Count > 10)
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
							txt_linksourcefolderexe.Text = Path.Combine(mostCommonPath, "Teknoparrot-patchs");
						}
					}


				}

			}
		}

		private void btn_selectLinkFolderExe_Click(object sender, EventArgs e)
		{
			MessageBox.Show("You must select a directory that use the same Drive as your game folder.");
			using (var fbd = new FolderBrowserDialog())
			{
				DialogResult result = fbd.ShowDialog();

				if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
				{
					txt_linksourcefolderexe.Text = Path.Combine(fbd.SelectedPath, "Teknoparrot-patchs");
					if (!Utils.IsEligibleHardLink(txt_linksourcefolderexe.Text))
					{
						MessageBox.Show("The drive does not seems eligible for hardlink (Must be NTFS and Windows 10 or newer)");
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

		private void groupBox2_Enter(object sender, EventArgs e)
		{

		}

		private bool CheckInstalledSoftware()
		{
			bool vigem = false;
			try
			{
				var client = new ViGEmClient();
				client.Dispose();
				vigem = true;
			}
			catch (VigemBusNotFoundException ex)
			{
				vigem = false;
			}

			bool vjoy = false;
			string vjoyPath = Utils.checkInstalled("vJoy");
			if(!string.IsNullOrEmpty(vjoyPath)) vjoy = true;

			/*

			try
			{
				var m_joystick = new vJoy();
				try
				{
					vjoy = (m_joystick != null && m_joystick.vJoyEnabled());
				}
				catch (DllNotFoundException)
				{
					vjoy = false;
				}
			}
			catch (Exception ex) { }
			*/

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

				btn_next.Enabled = true;
				return true;
			}
			return false;

		}

		private void btn_install_riva_Click(object sender, EventArgs e)
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

			Thread.Sleep(1000);
			CheckInstalledSoftware();


		}

		private void btn_install_vjoy_Click(object sender, EventArgs e)
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

			Thread.Sleep(1000);
			CheckInstalledSoftware();
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
			CheckInstalledSoftware();
		}

		private void groupBox4_Enter(object sender, EventArgs e)
		{

		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			string currentTabName = tabControl1.SelectedTab.Text;
			if(currentTabName == "tabPage1")
			{
				btn_previous.Enabled = false;
				btn_next.Enabled = true;
			}
			else
			{
				btn_previous.Enabled = true;
			}


			MessageBox.Show(currentTabName);
		}
	}
}
