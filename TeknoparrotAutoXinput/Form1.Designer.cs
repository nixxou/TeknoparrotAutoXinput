namespace TeknoparrotAutoXinput
{
	partial class Form1
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			chk_enableVirtualKeyboard = new Krypton.Toolkit.KryptonCheckBox();
			btn_setTest = new Krypton.Toolkit.KryptonButton();
			btn_ClearTest = new Krypton.Toolkit.KryptonButton();
			txt_KeyTest = new TextBox();
			kryptonLabel1 = new Krypton.Toolkit.KryptonLabel();
			kryptonLabel2 = new Krypton.Toolkit.KryptonLabel();
			txt_KeyService1 = new TextBox();
			btn_ClearService1 = new Krypton.Toolkit.KryptonButton();
			btn_setService1 = new Krypton.Toolkit.KryptonButton();
			kryptonLabel3 = new Krypton.Toolkit.KryptonLabel();
			txt_KeyService2 = new TextBox();
			btn_ClearService2 = new Krypton.Toolkit.KryptonButton();
			btn_setService2 = new Krypton.Toolkit.KryptonButton();
			chk_showStartup = new Krypton.Toolkit.KryptonCheckBox();
			chk_FFB = new Krypton.Toolkit.KryptonCheckBox();
			groupBox1 = new GroupBox();
			kryptonLabel8 = new Krypton.Toolkit.KryptonLabel();
			lbl_useCustomStooz_Gamepad = new Krypton.Toolkit.KryptonLabel();
			trk_useCustomStooz_Gamepad = new TrackBar();
			chk_enableStoozZone_Gamepad = new Krypton.Toolkit.KryptonCheckBox();
			radio_useCustomStooz_Gamepad = new Krypton.Toolkit.KryptonRadioButton();
			radio_useDefaultStooze_Gamepad = new Krypton.Toolkit.KryptonRadioButton();
			groupBox2 = new GroupBox();
			btn_configureDinputWheel = new Krypton.Toolkit.KryptonButton();
			chk_useDinputWheel = new Krypton.Toolkit.KryptonCheckBox();
			kryptonLabel9 = new Krypton.Toolkit.KryptonLabel();
			lbl_useCustomStooz_Wheel = new Krypton.Toolkit.KryptonLabel();
			trk_useCustomStooz_Wheel = new TrackBar();
			chk_enableStoozZone_Wheel = new Krypton.Toolkit.KryptonCheckBox();
			radio_useCustomStooz_Wheel = new Krypton.Toolkit.KryptonRadioButton();
			radio_useDefaultStooze_Wheel = new Krypton.Toolkit.KryptonRadioButton();
			groupBox3 = new GroupBox();
			kryptonButton1 = new Krypton.Toolkit.KryptonButton();
			kryptonLabel7 = new Krypton.Toolkit.KryptonLabel();
			txt_gamepadXinputData = new Krypton.Toolkit.KryptonTextBox();
			kryptonLabel6 = new Krypton.Toolkit.KryptonLabel();
			txt_arcadeXinputData = new Krypton.Toolkit.KryptonTextBox();
			kryptonLabel5 = new Krypton.Toolkit.KryptonLabel();
			txt_wheelXinputData = new Krypton.Toolkit.KryptonTextBox();
			kryptonLabel4 = new Krypton.Toolkit.KryptonLabel();
			btn_testxinput = new Krypton.Toolkit.KryptonButton();
			txt_xinputdata = new Krypton.Toolkit.KryptonTextBox();
			groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)trk_useCustomStooz_Gamepad).BeginInit();
			groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)trk_useCustomStooz_Wheel).BeginInit();
			groupBox3.SuspendLayout();
			SuspendLayout();
			// 
			// chk_enableVirtualKeyboard
			// 
			chk_enableVirtualKeyboard.Location = new Point(12, 12);
			chk_enableVirtualKeyboard.Name = "chk_enableVirtualKeyboard";
			chk_enableVirtualKeyboard.Size = new Size(295, 20);
			chk_enableVirtualKeyboard.TabIndex = 0;
			chk_enableVirtualKeyboard.Values.Text = "Bind Test, Service1 and Service2 to Keyboard keys";
			chk_enableVirtualKeyboard.CheckedChanged += chk_enableVirtualKeyboard_CheckedChanged;
			// 
			// btn_setTest
			// 
			btn_setTest.Location = new Point(267, 38);
			btn_setTest.Name = "btn_setTest";
			btn_setTest.Size = new Size(70, 23);
			btn_setTest.TabIndex = 2;
			btn_setTest.Values.Text = "Bind Key";
			btn_setTest.Click += btn_setTest_Click;
			// 
			// btn_ClearTest
			// 
			btn_ClearTest.Location = new Point(343, 38);
			btn_ClearTest.Name = "btn_ClearTest";
			btn_ClearTest.Size = new Size(58, 23);
			btn_ClearTest.TabIndex = 3;
			btn_ClearTest.Values.Text = "Clear";
			btn_ClearTest.Click += btn_ClearTest_Click;
			// 
			// txt_KeyTest
			// 
			txt_KeyTest.Location = new Point(78, 38);
			txt_KeyTest.Name = "txt_KeyTest";
			txt_KeyTest.ReadOnly = true;
			txt_KeyTest.Size = new Size(183, 23);
			txt_KeyTest.TabIndex = 4;
			// 
			// kryptonLabel1
			// 
			kryptonLabel1.Location = new Point(12, 41);
			kryptonLabel1.Name = "kryptonLabel1";
			kryptonLabel1.Size = new Size(39, 20);
			kryptonLabel1.TabIndex = 5;
			kryptonLabel1.Values.Text = "Test :";
			// 
			// kryptonLabel2
			// 
			kryptonLabel2.Location = new Point(12, 70);
			kryptonLabel2.Name = "kryptonLabel2";
			kryptonLabel2.Size = new Size(65, 20);
			kryptonLabel2.TabIndex = 9;
			kryptonLabel2.Values.Text = "Service 1 :";
			// 
			// txt_KeyService1
			// 
			txt_KeyService1.Location = new Point(78, 67);
			txt_KeyService1.Name = "txt_KeyService1";
			txt_KeyService1.ReadOnly = true;
			txt_KeyService1.Size = new Size(183, 23);
			txt_KeyService1.TabIndex = 8;
			// 
			// btn_ClearService1
			// 
			btn_ClearService1.Location = new Point(343, 67);
			btn_ClearService1.Name = "btn_ClearService1";
			btn_ClearService1.Size = new Size(58, 23);
			btn_ClearService1.TabIndex = 7;
			btn_ClearService1.Values.Text = "Clear";
			btn_ClearService1.Click += btn_ClearService1_Click;
			// 
			// btn_setService1
			// 
			btn_setService1.Location = new Point(267, 67);
			btn_setService1.Name = "btn_setService1";
			btn_setService1.Size = new Size(70, 23);
			btn_setService1.TabIndex = 6;
			btn_setService1.Values.Text = "Bind Key";
			btn_setService1.Click += btn_setService1_Click;
			// 
			// kryptonLabel3
			// 
			kryptonLabel3.Location = new Point(12, 99);
			kryptonLabel3.Name = "kryptonLabel3";
			kryptonLabel3.Size = new Size(65, 20);
			kryptonLabel3.TabIndex = 13;
			kryptonLabel3.Values.Text = "Service 2 :";
			// 
			// txt_KeyService2
			// 
			txt_KeyService2.Location = new Point(78, 96);
			txt_KeyService2.Name = "txt_KeyService2";
			txt_KeyService2.ReadOnly = true;
			txt_KeyService2.Size = new Size(183, 23);
			txt_KeyService2.TabIndex = 12;
			// 
			// btn_ClearService2
			// 
			btn_ClearService2.Location = new Point(343, 96);
			btn_ClearService2.Name = "btn_ClearService2";
			btn_ClearService2.Size = new Size(58, 23);
			btn_ClearService2.TabIndex = 11;
			btn_ClearService2.Values.Text = "Clear";
			btn_ClearService2.Click += btn_ClearService2_Click;
			// 
			// btn_setService2
			// 
			btn_setService2.Location = new Point(267, 96);
			btn_setService2.Name = "btn_setService2";
			btn_setService2.Size = new Size(70, 23);
			btn_setService2.TabIndex = 10;
			btn_setService2.Values.Text = "Bind Key";
			btn_setService2.Click += btn_setService2_Click;
			// 
			// chk_showStartup
			// 
			chk_showStartup.Location = new Point(12, 134);
			chk_showStartup.Name = "chk_showStartup";
			chk_showStartup.Size = new Size(231, 20);
			chk_showStartup.TabIndex = 14;
			chk_showStartup.Values.Text = "Show Startup Screen on Game Launch";
			chk_showStartup.CheckedChanged += chk_showStartup_CheckedChanged;
			// 
			// chk_FFB
			// 
			chk_FFB.Location = new Point(12, 160);
			chk_FFB.Name = "chk_FFB";
			chk_FFB.Size = new Size(346, 20);
			chk_FFB.TabIndex = 16;
			chk_FFB.Values.Text = "Auto-Update FFBPlugin Device GUID in the game config ini";
			chk_FFB.CheckedChanged += chk_FFB_CheckedChanged;
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(kryptonLabel8);
			groupBox1.Controls.Add(lbl_useCustomStooz_Gamepad);
			groupBox1.Controls.Add(trk_useCustomStooz_Gamepad);
			groupBox1.Controls.Add(chk_enableStoozZone_Gamepad);
			groupBox1.Controls.Add(radio_useCustomStooz_Gamepad);
			groupBox1.Controls.Add(radio_useDefaultStooze_Gamepad);
			groupBox1.Location = new Point(12, 445);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new Size(943, 58);
			groupBox1.TabIndex = 31;
			groupBox1.TabStop = false;
			groupBox1.Text = "GamePad Settings";
			// 
			// kryptonLabel8
			// 
			kryptonLabel8.Location = new Point(8, 22);
			kryptonLabel8.Name = "kryptonLabel8";
			kryptonLabel8.Size = new Size(78, 20);
			kryptonLabel8.TabIndex = 32;
			kryptonLabel8.Values.Text = "Sto0z Zone :";
			// 
			// lbl_useCustomStooz_Gamepad
			// 
			lbl_useCustomStooz_Gamepad.Location = new Point(902, 22);
			lbl_useCustomStooz_Gamepad.Name = "lbl_useCustomStooz_Gamepad";
			lbl_useCustomStooz_Gamepad.Size = new Size(35, 20);
			lbl_useCustomStooz_Gamepad.TabIndex = 32;
			lbl_useCustomStooz_Gamepad.Values.Text = "XX%";
			// 
			// trk_useCustomStooz_Gamepad
			// 
			trk_useCustomStooz_Gamepad.Location = new Point(472, 22);
			trk_useCustomStooz_Gamepad.Maximum = 100;
			trk_useCustomStooz_Gamepad.Name = "trk_useCustomStooz_Gamepad";
			trk_useCustomStooz_Gamepad.Size = new Size(424, 45);
			trk_useCustomStooz_Gamepad.TabIndex = 34;
			trk_useCustomStooz_Gamepad.Scroll += trk_useCustomStooz_Gamepad_Scroll;
			// 
			// chk_enableStoozZone_Gamepad
			// 
			chk_enableStoozZone_Gamepad.Location = new Point(341, 22);
			chk_enableStoozZone_Gamepad.Name = "chk_enableStoozZone_Gamepad";
			chk_enableStoozZone_Gamepad.Size = new Size(125, 20);
			chk_enableStoozZone_Gamepad.TabIndex = 33;
			chk_enableStoozZone_Gamepad.Values.Text = "Enable Sto0z Zone";
			// 
			// radio_useCustomStooz_Gamepad
			// 
			radio_useCustomStooz_Gamepad.Location = new Point(200, 22);
			radio_useCustomStooz_Gamepad.Name = "radio_useCustomStooz_Gamepad";
			radio_useCustomStooz_Gamepad.Size = new Size(135, 20);
			radio_useCustomStooz_Gamepad.TabIndex = 32;
			radio_useCustomStooz_Gamepad.Values.Text = "Use Custom Settings";
			radio_useCustomStooz_Gamepad.CheckedChanged += radio_useCustomStooz_Gamepad_CheckedChanged;
			// 
			// radio_useDefaultStooze_Gamepad
			// 
			radio_useDefaultStooze_Gamepad.Location = new Point(92, 22);
			radio_useDefaultStooze_Gamepad.Name = "radio_useDefaultStooze_Gamepad";
			radio_useDefaultStooze_Gamepad.Size = new Size(102, 20);
			radio_useDefaultStooze_Gamepad.TabIndex = 31;
			radio_useDefaultStooze_Gamepad.Values.Text = "Use TP Setting";
			radio_useDefaultStooze_Gamepad.CheckedChanged += radio_useDefaultStooze_Gamepad_CheckedChanged;
			// 
			// groupBox2
			// 
			groupBox2.Controls.Add(btn_configureDinputWheel);
			groupBox2.Controls.Add(chk_useDinputWheel);
			groupBox2.Controls.Add(kryptonLabel9);
			groupBox2.Controls.Add(lbl_useCustomStooz_Wheel);
			groupBox2.Controls.Add(trk_useCustomStooz_Wheel);
			groupBox2.Controls.Add(chk_enableStoozZone_Wheel);
			groupBox2.Controls.Add(radio_useCustomStooz_Wheel);
			groupBox2.Controls.Add(radio_useDefaultStooze_Wheel);
			groupBox2.Location = new Point(12, 518);
			groupBox2.Name = "groupBox2";
			groupBox2.Size = new Size(943, 92);
			groupBox2.TabIndex = 35;
			groupBox2.TabStop = false;
			groupBox2.Text = "Wheel Settings";
			// 
			// btn_configureDinputWheel
			// 
			btn_configureDinputWheel.Enabled = false;
			btn_configureDinputWheel.Location = new Point(292, 48);
			btn_configureDinputWheel.Name = "btn_configureDinputWheel";
			btn_configureDinputWheel.Size = new Size(110, 23);
			btn_configureDinputWheel.TabIndex = 36;
			btn_configureDinputWheel.Values.Text = "Configure";
			// 
			// chk_useDinputWheel
			// 
			chk_useDinputWheel.Enabled = false;
			chk_useDinputWheel.Location = new Point(13, 51);
			chk_useDinputWheel.Name = "chk_useDinputWheel";
			chk_useDinputWheel.Size = new Size(273, 20);
			chk_useDinputWheel.TabIndex = 35;
			chk_useDinputWheel.Values.Text = "Use Dinput Wheel instead of an Xinput Wheel";
			// 
			// kryptonLabel9
			// 
			kryptonLabel9.Location = new Point(8, 22);
			kryptonLabel9.Name = "kryptonLabel9";
			kryptonLabel9.Size = new Size(78, 20);
			kryptonLabel9.TabIndex = 32;
			kryptonLabel9.Values.Text = "Sto0z Zone :";
			// 
			// lbl_useCustomStooz_Wheel
			// 
			lbl_useCustomStooz_Wheel.Location = new Point(902, 22);
			lbl_useCustomStooz_Wheel.Name = "lbl_useCustomStooz_Wheel";
			lbl_useCustomStooz_Wheel.Size = new Size(35, 20);
			lbl_useCustomStooz_Wheel.TabIndex = 32;
			lbl_useCustomStooz_Wheel.Values.Text = "XX%";
			// 
			// trk_useCustomStooz_Wheel
			// 
			trk_useCustomStooz_Wheel.Location = new Point(472, 22);
			trk_useCustomStooz_Wheel.Maximum = 100;
			trk_useCustomStooz_Wheel.Name = "trk_useCustomStooz_Wheel";
			trk_useCustomStooz_Wheel.Size = new Size(424, 45);
			trk_useCustomStooz_Wheel.TabIndex = 34;
			trk_useCustomStooz_Wheel.Scroll += trk_useCustomStooz_Wheel_Scroll;
			// 
			// chk_enableStoozZone_Wheel
			// 
			chk_enableStoozZone_Wheel.Location = new Point(341, 22);
			chk_enableStoozZone_Wheel.Name = "chk_enableStoozZone_Wheel";
			chk_enableStoozZone_Wheel.Size = new Size(125, 20);
			chk_enableStoozZone_Wheel.TabIndex = 33;
			chk_enableStoozZone_Wheel.Values.Text = "Enable Sto0z Zone";
			// 
			// radio_useCustomStooz_Wheel
			// 
			radio_useCustomStooz_Wheel.Location = new Point(200, 22);
			radio_useCustomStooz_Wheel.Name = "radio_useCustomStooz_Wheel";
			radio_useCustomStooz_Wheel.Size = new Size(135, 20);
			radio_useCustomStooz_Wheel.TabIndex = 32;
			radio_useCustomStooz_Wheel.Values.Text = "Use Custom Settings";
			radio_useCustomStooz_Wheel.CheckedChanged += radio_useCustomStooz_Wheel_CheckedChanged;
			// 
			// radio_useDefaultStooze_Wheel
			// 
			radio_useDefaultStooze_Wheel.Location = new Point(92, 22);
			radio_useDefaultStooze_Wheel.Name = "radio_useDefaultStooze_Wheel";
			radio_useDefaultStooze_Wheel.Size = new Size(102, 20);
			radio_useDefaultStooze_Wheel.TabIndex = 31;
			radio_useDefaultStooze_Wheel.Values.Text = "Use TP Setting";
			radio_useDefaultStooze_Wheel.CheckedChanged += radio_useDefaultStooze_Wheel_CheckedChanged;
			// 
			// groupBox3
			// 
			groupBox3.Controls.Add(kryptonButton1);
			groupBox3.Controls.Add(kryptonLabel7);
			groupBox3.Controls.Add(txt_gamepadXinputData);
			groupBox3.Controls.Add(kryptonLabel6);
			groupBox3.Controls.Add(txt_arcadeXinputData);
			groupBox3.Controls.Add(kryptonLabel5);
			groupBox3.Controls.Add(txt_wheelXinputData);
			groupBox3.Controls.Add(kryptonLabel4);
			groupBox3.Controls.Add(btn_testxinput);
			groupBox3.Controls.Add(txt_xinputdata);
			groupBox3.Location = new Point(12, 186);
			groupBox3.Name = "groupBox3";
			groupBox3.Size = new Size(943, 253);
			groupBox3.TabIndex = 36;
			groupBox3.TabStop = false;
			groupBox3.Text = "Configure Xinput AutoDetection of controller Type";
			// 
			// kryptonButton1
			// 
			kryptonButton1.Location = new Point(816, 103);
			kryptonButton1.Name = "kryptonButton1";
			kryptonButton1.Size = new Size(121, 23);
			kryptonButton1.TabIndex = 36;
			kryptonButton1.Values.Text = "Reset To Default";
			kryptonButton1.Click += kryptonButton1_Click;
			// 
			// kryptonLabel7
			// 
			kryptonLabel7.Location = new Point(268, 19);
			kryptonLabel7.Name = "kryptonLabel7";
			kryptonLabel7.Size = new Size(331, 20);
			kryptonLabel7.TabIndex = 35;
			kryptonLabel7.Values.Text = "You can add multiple entry using comma to separate them";
			// 
			// txt_gamepadXinputData
			// 
			txt_gamepadXinputData.Location = new Point(268, 103);
			txt_gamepadXinputData.Name = "txt_gamepadXinputData";
			txt_gamepadXinputData.Size = new Size(542, 23);
			txt_gamepadXinputData.TabIndex = 34;
			// 
			// kryptonLabel6
			// 
			kryptonLabel6.Location = new Point(9, 103);
			kryptonLabel6.Name = "kryptonLabel6";
			kryptonLabel6.Size = new Size(271, 20);
			kryptonLabel6.TabIndex = 33;
			kryptonLabel6.Values.Text = "Recognize Xinput as Gamepad if data Contains :";
			// 
			// txt_arcadeXinputData
			// 
			txt_arcadeXinputData.Location = new Point(268, 74);
			txt_arcadeXinputData.Name = "txt_arcadeXinputData";
			txt_arcadeXinputData.Size = new Size(542, 23);
			txt_arcadeXinputData.TabIndex = 32;
			// 
			// kryptonLabel5
			// 
			kryptonLabel5.Location = new Point(9, 74);
			kryptonLabel5.Name = "kryptonLabel5";
			kryptonLabel5.Size = new Size(260, 20);
			kryptonLabel5.TabIndex = 31;
			kryptonLabel5.Values.Text = "Recognize Xinput as  Arcade if data Contains :";
			// 
			// txt_wheelXinputData
			// 
			txt_wheelXinputData.Location = new Point(268, 45);
			txt_wheelXinputData.Name = "txt_wheelXinputData";
			txt_wheelXinputData.Size = new Size(542, 23);
			txt_wheelXinputData.TabIndex = 30;
			// 
			// kryptonLabel4
			// 
			kryptonLabel4.Location = new Point(9, 45);
			kryptonLabel4.Name = "kryptonLabel4";
			kryptonLabel4.Size = new Size(253, 20);
			kryptonLabel4.TabIndex = 29;
			kryptonLabel4.Values.Text = "Recognize Xinput as Wheel if data Contains :";
			// 
			// btn_testxinput
			// 
			btn_testxinput.Location = new Point(816, 135);
			btn_testxinput.Name = "btn_testxinput";
			btn_testxinput.Size = new Size(121, 108);
			btn_testxinput.TabIndex = 28;
			btn_testxinput.Values.Text = "Show Xinput \r\nDevices Data\r\n(Debug)";
			btn_testxinput.Click += btn_testxinput_Click;
			// 
			// txt_xinputdata
			// 
			txt_xinputdata.Location = new Point(9, 135);
			txt_xinputdata.Multiline = true;
			txt_xinputdata.Name = "txt_xinputdata";
			txt_xinputdata.Size = new Size(801, 108);
			txt_xinputdata.TabIndex = 27;
			txt_xinputdata.Text = "Click on the button to the right to show your connected Xinput Data\r\n";
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(965, 618);
			Controls.Add(groupBox3);
			Controls.Add(groupBox2);
			Controls.Add(groupBox1);
			Controls.Add(chk_FFB);
			Controls.Add(chk_showStartup);
			Controls.Add(kryptonLabel3);
			Controls.Add(txt_KeyService2);
			Controls.Add(btn_ClearService2);
			Controls.Add(btn_setService2);
			Controls.Add(kryptonLabel2);
			Controls.Add(txt_KeyService1);
			Controls.Add(btn_ClearService1);
			Controls.Add(btn_setService1);
			Controls.Add(kryptonLabel1);
			Controls.Add(txt_KeyTest);
			Controls.Add(btn_ClearTest);
			Controls.Add(btn_setTest);
			Controls.Add(chk_enableVirtualKeyboard);
			Name = "Form1";
			Text = "Form1";
			FormClosing += Form1_FormClosing;
			Load += Form1_Load;
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)trk_useCustomStooz_Gamepad).EndInit();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)trk_useCustomStooz_Wheel).EndInit();
			groupBox3.ResumeLayout(false);
			groupBox3.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Krypton.Toolkit.KryptonCheckBox chk_enableVirtualKeyboard;
		private Krypton.Toolkit.KryptonButton btn_setTest;
		private Krypton.Toolkit.KryptonButton btn_ClearTest;
		private TextBox txt_KeyTest;
		private Krypton.Toolkit.KryptonLabel kryptonLabel1;
		private Krypton.Toolkit.KryptonLabel kryptonLabel2;
		private TextBox txt_KeyService1;
		private Krypton.Toolkit.KryptonButton btn_ClearService1;
		private Krypton.Toolkit.KryptonButton btn_setService1;
		private Krypton.Toolkit.KryptonLabel kryptonLabel3;
		private TextBox txt_KeyService2;
		private Krypton.Toolkit.KryptonButton btn_ClearService2;
		private Krypton.Toolkit.KryptonButton btn_setService2;
		private Krypton.Toolkit.KryptonCheckBox chk_showStartup;
		private Krypton.Toolkit.KryptonCheckBox chk_FFB;
		private GroupBox groupBox1;
		private Krypton.Toolkit.KryptonRadioButton radio_useCustomStooz_Gamepad;
		private Krypton.Toolkit.KryptonRadioButton radio_useDefaultStooze_Gamepad;
		private Krypton.Toolkit.KryptonCheckBox chk_enableStoozZone_Gamepad;
		private Krypton.Toolkit.KryptonLabel lbl_useCustomStooz_Gamepad;
		private TrackBar trk_useCustomStooz_Gamepad;
		private Krypton.Toolkit.KryptonLabel kryptonLabel8;
		private GroupBox groupBox2;
		private Krypton.Toolkit.KryptonButton btn_configureDinputWheel;
		private Krypton.Toolkit.KryptonCheckBox chk_useDinputWheel;
		private Krypton.Toolkit.KryptonLabel kryptonLabel9;
		private Krypton.Toolkit.KryptonLabel lbl_useCustomStooz_Wheel;
		private TrackBar trk_useCustomStooz_Wheel;
		private Krypton.Toolkit.KryptonCheckBox chk_enableStoozZone_Wheel;
		private Krypton.Toolkit.KryptonRadioButton radio_useCustomStooz_Wheel;
		private Krypton.Toolkit.KryptonRadioButton radio_useDefaultStooze_Wheel;
		private GroupBox groupBox3;
		private Krypton.Toolkit.KryptonButton kryptonButton1;
		private Krypton.Toolkit.KryptonLabel kryptonLabel7;
		private Krypton.Toolkit.KryptonTextBox txt_gamepadXinputData;
		private Krypton.Toolkit.KryptonLabel kryptonLabel6;
		private Krypton.Toolkit.KryptonTextBox txt_arcadeXinputData;
		private Krypton.Toolkit.KryptonLabel kryptonLabel5;
		private Krypton.Toolkit.KryptonTextBox txt_wheelXinputData;
		private Krypton.Toolkit.KryptonLabel kryptonLabel4;
		private Krypton.Toolkit.KryptonButton btn_testxinput;
		private Krypton.Toolkit.KryptonTextBox txt_xinputdata;
	}
}