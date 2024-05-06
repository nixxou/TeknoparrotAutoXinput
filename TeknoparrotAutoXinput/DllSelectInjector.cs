using Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TeknoparrotAutoXinput
{
	public partial class DllSelectInjector : KryptonForm
	{
		public List<string> selectedDll = new List<string>();
		public DllSelectInjector(List<string> paths, List<string> dllchecked)
		{
			InitializeComponent();
			List<string> dlls = new List<string>();
			foreach(string path in paths) 
			{
				if(Directory.Exists(path))
				{
					var files = Directory.EnumerateFiles(path, "*.dll", new EnumerationOptions
					{
						IgnoreInaccessible = true,
						RecurseSubdirectories = false
					});

					var files2 = Directory.EnumerateFiles(path, "*.inject", new EnumerationOptions
					{
						IgnoreInaccessible = true,
						RecurseSubdirectories = false
					});

					foreach (var file in files)
					{
						string dllFile = Path.GetFileName(file).ToLower();
						if (dllFile.Contains(",")) continue;
						if (!dlls.Contains(dllFile))
						{
							dlls.Add(dllFile);
							kryptonCheckedListBox1.Items.Add(dllFile);
						}
					}
					foreach (var file in files2)
					{
						string dllFile = Path.GetFileName(file).ToLower();
						if (dllFile.Contains(",")) continue;
						if (!dlls.Contains(dllFile))
						{
							dlls.Add(dllFile);
							kryptonCheckedListBox1.Items.Add(dllFile);
						}
					}
				}
				for(int i= 0; i < kryptonCheckedListBox1.Items.Count; i++)
				{
					var dllName = kryptonCheckedListBox1.Items[i].ToString();
					if (dllchecked.Contains(dllName))
					{
						kryptonCheckedListBox1.SetItemChecked(i, true);
					}
				}
			}
		}

		private void Form2_Load(object sender, EventArgs e)
		{

		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void btn_Save_Click(object sender, EventArgs e)
		{
			selectedDll = new List<string>();
			for (int i = 0; i < kryptonCheckedListBox1.Items.Count; i++)
			{
				if(kryptonCheckedListBox1.GetItemChecked(i) == true)
				{
					string dllName = kryptonCheckedListBox1.Items[i].ToString();
					selectedDll.Add(dllName);
				}
			}

			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}
