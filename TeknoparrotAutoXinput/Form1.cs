using Krypton.Toolkit;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Exceptions;
using TestVgme;

namespace TeknoparrotAutoXinput
{
	public partial class Form1 : KryptonForm
	{
		private PickKeyCombo KeyPicker { get; set; }
		public Keys[] Keys { get; set; }

		public Form1()
		{
			InitializeComponent();
		}


		private void Form1_Load(object sender, EventArgs e)
		{
			this.KeyPicker = new PickKeyCombo(this);
			System.Diagnostics.Debugger.Break();

			chk_FFB.Checked = (bool)Properties.Settings.Default["FFB"];
			chk_showStartup.Checked = (bool)Properties.Settings.Default["showStartup"];
			chk_enableVirtualKeyboard.Checked = (bool)Properties.Settings.Default["virtualKeyboard"];
			txt_KeyTest.Text = Properties.Settings.Default["keyTest"].ToString();
			txt_KeyService1.Text = Properties.Settings.Default["keyService1"].ToString();
			txt_KeyService2.Text = Properties.Settings.Default["keyService2"].ToString();

			txt_wheelXinputData.Text = Properties.Settings.Default["wheelXinputData"].ToString();
			txt_arcadeXinputData.Text = Properties.Settings.Default["arcadeXinputData"].ToString();
			txt_gamepadXinputData.Text = Properties.Settings.Default["gamepadXinputData"].ToString();


			radio_useCustomStooz_Gamepad.Checked = (bool)Properties.Settings.Default["gamepadStooz"];
			radio_useCustomStooz_Wheel.Checked = (bool)Properties.Settings.Default["wheelStooz"];
			radio_useDefaultStooze_Gamepad.Checked = !radio_useCustomStooz_Gamepad.Checked;
			radio_useDefaultStooze_Wheel.Checked = !radio_useCustomStooz_Wheel.Checked;

			chk_enableStoozZone_Gamepad.Checked = (bool)Properties.Settings.Default["enableStoozZone_Gamepad"];
			trk_useCustomStooz_Gamepad.Value = (int)Properties.Settings.Default["valueStooz_Gamepad"];
			chk_enableStoozZone_Wheel.Checked = (bool)Properties.Settings.Default["enableStoozZone_Wheel"];
			trk_useCustomStooz_Wheel.Value = (int)Properties.Settings.Default["valueStooz_Wheel"];

			updateStooz();

			if (!chk_enableVirtualKeyboard.Checked)
			{
				btn_ClearService1.Enabled = false;
				btn_ClearService2.Enabled = false;
				btn_ClearTest.Enabled = false;
				btn_setService1.Enabled = false;
				btn_setService2.Enabled = false;
				btn_setTest.Enabled = false;
			}


		}

		private void chk_enableVirtualKeyboard_CheckedChanged(object sender, EventArgs e)
		{
			if (chk_enableVirtualKeyboard.Checked)
			{
				try
				{
					var client = new ViGEmClient();
					client.Dispose();
				}
				catch (VigemBusNotFoundException ex)
				{
					chk_enableVirtualKeyboard.Checked = false;
					MessageBox.Show("ViGEm bus not found, please make sure ViGEm is correctly installed.");
				}
			}


			btn_ClearService1.Enabled = chk_enableVirtualKeyboard.Checked;
			btn_ClearService2.Enabled = chk_enableVirtualKeyboard.Checked;
			btn_ClearTest.Enabled = chk_enableVirtualKeyboard.Checked;
			btn_setService1.Enabled = chk_enableVirtualKeyboard.Checked;
			btn_setService2.Enabled = chk_enableVirtualKeyboard.Checked;
			btn_setTest.Enabled = chk_enableVirtualKeyboard.Checked;
			Properties.Settings.Default["virtualKeyboard"] = chk_enableVirtualKeyboard.Checked;
			Properties.Settings.Default.Save();
		}
		public void DrawKeyDisplay(string TextBoxName)
		{

			if (TextBoxName == "txt_KeyTest") txt_KeyTest.Text = PickKeyCombo.GetKeyCombo(this.Keys, true);
			if (TextBoxName == "txt_KeyService1") txt_KeyService1.Text = PickKeyCombo.GetKeyCombo(this.Keys, true);
			if (TextBoxName == "txt_KeyService2") txt_KeyService2.Text = PickKeyCombo.GetKeyCombo(this.Keys, true);

			Properties.Settings.Default["keyTest"] = txt_KeyTest.Text;
			Properties.Settings.Default["keyService1"] = txt_KeyService1.Text;
			Properties.Settings.Default["keyService2"] = txt_KeyService2.Text;
			Properties.Settings.Default.Save();

		}

		private void btn_setTest_Click(object sender, EventArgs e)
		{
			foreach (Control c in this.Controls)
			{
				c.Enabled = false;
			}
			txt_KeyTest.Text = "Press up to three keys...";
			KeyPicker.StartPicking("txt_KeyTest");
			Focus();
			txt_KeyTest.Focus();
			txt_KeyTest.Select(txt_KeyTest.Text.Length, 0);
		}

		private void btn_setService1_Click(object sender, EventArgs e)
		{
			foreach (Control c in this.Controls)
			{
				c.Enabled = false;
			}
			txt_KeyService1.Text = "Press up to three keys...";
			KeyPicker.StartPicking("txt_KeyService1");
			Focus();
			txt_KeyService1.Focus();
			txt_KeyService1.Select(txt_KeyService1.Text.Length, 0);
		}

		private void btn_setService2_Click(object sender, EventArgs e)
		{
			foreach (Control c in this.Controls)
			{
				c.Enabled = false;
			}
			txt_KeyService2.Text = "Press up to three keys...";
			KeyPicker.StartPicking("txt_KeyService2");
			Focus();
			txt_KeyService2.Focus();
			txt_KeyService2.Select(txt_KeyService2.Text.Length, 0);
		}

		private void btn_ClearTest_Click(object sender, EventArgs e)
		{
			txt_KeyTest.Text = "";
			Properties.Settings.Default["keyTest"] = txt_KeyTest.Text;
			Properties.Settings.Default.Save();
		}

		private void btn_ClearService1_Click(object sender, EventArgs e)
		{
			txt_KeyService1.Text = "";
			Properties.Settings.Default["keyService1"] = txt_KeyService1.Text;
			Properties.Settings.Default.Save();
		}

		private void btn_ClearService2_Click(object sender, EventArgs e)
		{
			txt_KeyService2.Text = "";
			Properties.Settings.Default["keyService2"] = txt_KeyService2.Text;
			Properties.Settings.Default.Save();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var frm = new Startup();
			var result = frm.ShowDialog();

		}

		private void chk_showStartup_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default["showStartup"] = chk_showStartup.Checked;
			Properties.Settings.Default.Save();
		}

		private void chk_FFB_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default["FFB"] = chk_FFB.Checked;
			Properties.Settings.Default.Save();
		}

		private void btn_testxinput_Click(object sender, EventArgs e)
		{
			txt_xinputdata.Text = HIDInfo.GetXINPUT(true);
		}

		private void kryptonLabel4_Click(object sender, EventArgs e)
		{

		}

		private void kryptonButton1_Click(object sender, EventArgs e)
		{
			txt_wheelXinputData.Text = "Type=Wheel";
			txt_arcadeXinputData.Text = "Type=ArcadeStick,Type=ArcadePad";
			txt_gamepadXinputData.Text = "Type=Gamepad";
			Properties.Settings.Default["wheelXinputData"] = txt_wheelXinputData.Text;
			Properties.Settings.Default["arcadeXinputData"] = txt_arcadeXinputData.Text;
			Properties.Settings.Default["gamepadXinputData"] = txt_gamepadXinputData.Text;


		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			Properties.Settings.Default["wheelXinputData"] = txt_wheelXinputData.Text;
			Properties.Settings.Default["arcadeXinputData"] = txt_arcadeXinputData.Text;
			Properties.Settings.Default["gamepadXinputData"] = txt_gamepadXinputData.Text;

			Properties.Settings.Default["gamepadStooz"] = radio_useCustomStooz_Gamepad.Checked;
			Properties.Settings.Default["wheelStooz"] = radio_useCustomStooz_Wheel.Checked;

			Properties.Settings.Default["enableStoozZone_Gamepad"] = chk_enableStoozZone_Gamepad.Checked;
			Properties.Settings.Default["valueStooz_Gamepad"] = trk_useCustomStooz_Gamepad.Value;
			Properties.Settings.Default["enableStoozZone_Wheel"] = chk_enableStoozZone_Wheel.Checked;
			Properties.Settings.Default["valueStooz_Wheel"] = trk_useCustomStooz_Wheel.Value;


			Properties.Settings.Default.Save();
		}

		private void trk_useCustomStooz_Gamepad_Scroll(object sender, EventArgs e)
		{
			lbl_useCustomStooz_Gamepad.Text = trk_useCustomStooz_Gamepad.Value.ToString() + "%";
		}

		private void trk_useCustomStooz_Wheel_Scroll(object sender, EventArgs e)
		{
			lbl_useCustomStooz_Wheel.Text = trk_useCustomStooz_Wheel.Value.ToString() + "%";
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



	}
}