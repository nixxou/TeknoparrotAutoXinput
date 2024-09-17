using Krypton.Toolkit;
using Microsoft.VisualBasic;
using MonitorSwitcherGUI;
using System.Text.RegularExpressions;

namespace TeknoparrotAutoXinput
{
	public partial class MonitorDispositionConfig : KryptonForm
	{
		public string result = ConfigurationManager.MainConfig.Disposition;
		public MonitorDispositionConfig()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			string name = Interaction.InputBox("Disposition Name :", "Name", "");
			string truename = FilterFileName(name);
			string filename = Path.Combine(Program.DispositionFolder, "disposition_" + truename + ".xml");
			if (File.Exists(filename))
			{
				MessageBox.Show("Name already exist");
			}
			else
			{
				if (!string.IsNullOrEmpty(truename.Trim()))
				{
					MonitorSwitcher.SaveDisplaySettings(filename);
					ReloadCmb();
				}
			}
		}
		public static string FilterFileName(string name)
		{
			string title = name;
			title = Regex.Replace(title, @"\p{S}", "");
			title = Regex.Replace(title, "[^A-Za-z0-9 -]", "");
			title = title.Trim().Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Trim().ToLower();
			return title;

		}

		private void MonitorDispositionConfig_Load(object sender, EventArgs e)
		{
			ReloadCmb();

		}

		private void ReloadCmb()
		{
			cmb_DispositionList.Items.Clear();
			cmb_DispositionList.Items.Add("<none>");
			cmb_DispositionList.Items.Add("MainMonitor:720p 60hz");
			cmb_DispositionList.Items.Add("MainMonitor:1080p 60hz");
			cmb_DispositionList.Items.Add("MainMonitor:2k 60hz");
			cmb_DispositionList.Items.Add("MainMonitor:4k 60hz");
			cmb_DispositionList.Items.Add("MainMonitor:720p 120hz");
			cmb_DispositionList.Items.Add("MainMonitor:1080p 120hz");
			cmb_DispositionList.Items.Add("MainMonitor:2k 120hz");
			cmb_DispositionList.Items.Add("MainMonitor:4k 120hz");
			foreach (var disposition in Directory.GetFiles(Program.DispositionFolder, "disposition_*.xml"))
			{
				string disposition_name = Path.GetFileNameWithoutExtension(disposition);
				disposition_name = disposition_name.Remove(0, 12);

				cmb_DispositionList.Items.Add($"{disposition_name}");
			}
			string selected = ConfigurationManager.MainConfig.Disposition;
			var index = cmb_DispositionList.Items.IndexOf(selected);
			if (index >= 0) cmb_DispositionList.SelectedIndex = index;
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void btn_ok_Click(object sender, EventArgs e)
		{
			result = cmb_DispositionList.SelectedItem.ToString();
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}
