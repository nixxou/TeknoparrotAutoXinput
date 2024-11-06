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
