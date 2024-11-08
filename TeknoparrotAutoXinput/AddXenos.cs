﻿using Krypton.Toolkit;
using System.Diagnostics;

namespace TeknoparrotAutoXinput
{
	public partial class AddXenos : KryptonForm
	{
		public AddXenos()
		{
			InitializeComponent();

		}

		private void kryptonButton1_Click(object sender, EventArgs e)
		{
			string selfExe = Process.GetCurrentProcess().MainModule.FileName;
			string exePath = selfExe;
			string exeDir = Path.GetDirectoryName(exePath);
			Process process = new Process();
			process.StartInfo.FileName = selfExe;
			process.StartInfo.Arguments = "--xenos register";
			process.StartInfo.WorkingDirectory = exeDir;
			process.StartInfo.UseShellExecute = true;
			process.StartInfo.Verb = "runas";
			process.Start();
			process.WaitForExit();
		}

		private void kryptonButton2_Click(object sender, EventArgs e)
		{
			string XenosDir = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "xenos");
			string Xenos7z = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "xenos", "Xenos.7z");
			string XenosPath = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "xenos", "Xenos.exe");
			string SevenZipExe = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "7zip", "7z.exe");
			if (File.Exists(SevenZipExe) && File.Exists(Xenos7z))
			{
				Process process = new Process();
				process.StartInfo.FileName = SevenZipExe;
				process.StartInfo.Arguments = "x -y -pxenos Xenos.7z";
				process.StartInfo.WorkingDirectory = Path.GetDirectoryName(Xenos7z);
				process.StartInfo.UseShellExecute = true;
				process.Start();
				process.WaitForExit();
				if (File.Exists(XenosPath))
				{
					btn_Save.Enabled = true;
				}
			}



		}

		private void btn_Save_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void AddXenos_Load(object sender, EventArgs e)
		{
			label6.Text = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "xenos");
		}
	}
}
