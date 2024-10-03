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
	public partial class FormTest : Form
	{
		public FormTest()
		{
			InitializeComponent();

		}

		private void FormTest_Shown(object sender, EventArgs e)
		{
			this.gameManagementUserControl1.TpFolder = ConfigurationManager.MainConfig.TpFolder;
			this.gameManagementUserControl1.ManualLoad();
		}
	}
}
