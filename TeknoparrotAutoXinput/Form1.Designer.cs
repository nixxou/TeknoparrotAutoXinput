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
			chk_favorAB = new Krypton.Toolkit.KryptonCheckBox();
			kryptonLabel8 = new Krypton.Toolkit.KryptonLabel();
			lbl_useCustomStooz_Gamepad = new Krypton.Toolkit.KryptonLabel();
			trk_useCustomStooz_Gamepad = new TrackBar();
			chk_enableStoozZone_Gamepad = new Krypton.Toolkit.KryptonCheckBox();
			radio_useCustomStooz_Gamepad = new Krypton.Toolkit.KryptonRadioButton();
			radio_useDefaultStooze_Gamepad = new Krypton.Toolkit.KryptonRadioButton();
			groupBox2 = new GroupBox();
			btn_configureDinputShifter = new Krypton.Toolkit.KryptonButton();
			chk_useDinputShifter = new Krypton.Toolkit.KryptonCheckBox();
			btn_setffbguid = new Krypton.Toolkit.KryptonButton();
			cmb_ffbguid = new Krypton.Toolkit.KryptonComboBox();
			kryptonLabel10 = new Krypton.Toolkit.KryptonLabel();
			txt_ffbguid = new Krypton.Toolkit.KryptonTextBox();
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
			btn_Save = new Krypton.Toolkit.KryptonButton();
			txt_tpfolder = new Krypton.Toolkit.KryptonTextBox();
			groupBox4 = new GroupBox();
			btn_selectTP = new Krypton.Toolkit.KryptonButton();
			kryptonLabel11 = new Krypton.Toolkit.KryptonLabel();
			txt_monitorswitch = new Krypton.Toolkit.KryptonTextBox();
			kryptonLabel12 = new Krypton.Toolkit.KryptonLabel();
			btn_editMonitorSwitch = new Krypton.Toolkit.KryptonButton();
			chk_enableDebug = new Krypton.Toolkit.KryptonCheckBox();
			btn_checkConfig = new Krypton.Toolkit.KryptonButton();
			groupBox5 = new GroupBox();
			btn_resetdefaultlinksource = new Krypton.Toolkit.KryptonButton();
			btn_selectLinkFolder = new Krypton.Toolkit.KryptonButton();
			kryptonLabel13 = new Krypton.Toolkit.KryptonLabel();
			txt_linksourcefolder = new Krypton.Toolkit.KryptonTextBox();
			groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)trk_useCustomStooz_Gamepad).BeginInit();
			groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)cmb_ffbguid).BeginInit();
			((System.ComponentModel.ISupportInitialize)trk_useCustomStooz_Wheel).BeginInit();
			groupBox3.SuspendLayout();
			groupBox4.SuspendLayout();
			groupBox5.SuspendLayout();
			SuspendLayout();
			// 
			// chk_enableVirtualKeyboard
			// 
			chk_enableVirtualKeyboard.Location = new Point(12, 180);
			chk_enableVirtualKeyboard.Name = "chk_enableVirtualKeyboard";
			chk_enableVirtualKeyboard.Size = new Size(295, 20);
			chk_enableVirtualKeyboard.TabIndex = 0;
			chk_enableVirtualKeyboard.Values.Text = "Bind Test, Service1 and Service2 to Keyboard keys";
			chk_enableVirtualKeyboard.CheckedChanged += chk_enableVirtualKeyboard_CheckedChanged;
			// 
			// btn_setTest
			// 
			btn_setTest.Location = new Point(267, 206);
			btn_setTest.Name = "btn_setTest";
			btn_setTest.Size = new Size(70, 23);
			btn_setTest.TabIndex = 2;
			btn_setTest.Values.Text = "Bind Key";
			btn_setTest.Click += btn_setTest_Click;
			// 
			// btn_ClearTest
			// 
			btn_ClearTest.Location = new Point(343, 206);
			btn_ClearTest.Name = "btn_ClearTest";
			btn_ClearTest.Size = new Size(58, 23);
			btn_ClearTest.TabIndex = 3;
			btn_ClearTest.Values.Text = "Clear";
			btn_ClearTest.Click += btn_ClearTest_Click;
			// 
			// txt_KeyTest
			// 
			txt_KeyTest.Location = new Point(78, 206);
			txt_KeyTest.Name = "txt_KeyTest";
			txt_KeyTest.ReadOnly = true;
			txt_KeyTest.Size = new Size(183, 23);
			txt_KeyTest.TabIndex = 4;
			// 
			// kryptonLabel1
			// 
			kryptonLabel1.Location = new Point(12, 209);
			kryptonLabel1.Name = "kryptonLabel1";
			kryptonLabel1.Size = new Size(39, 20);
			kryptonLabel1.TabIndex = 5;
			kryptonLabel1.Values.Text = "Test :";
			// 
			// kryptonLabel2
			// 
			kryptonLabel2.Location = new Point(12, 238);
			kryptonLabel2.Name = "kryptonLabel2";
			kryptonLabel2.Size = new Size(65, 20);
			kryptonLabel2.TabIndex = 9;
			kryptonLabel2.Values.Text = "Service 1 :";
			// 
			// txt_KeyService1
			// 
			txt_KeyService1.Location = new Point(78, 235);
			txt_KeyService1.Name = "txt_KeyService1";
			txt_KeyService1.ReadOnly = true;
			txt_KeyService1.Size = new Size(183, 23);
			txt_KeyService1.TabIndex = 8;
			// 
			// btn_ClearService1
			// 
			btn_ClearService1.Location = new Point(343, 235);
			btn_ClearService1.Name = "btn_ClearService1";
			btn_ClearService1.Size = new Size(58, 23);
			btn_ClearService1.TabIndex = 7;
			btn_ClearService1.Values.Text = "Clear";
			btn_ClearService1.Click += btn_ClearService1_Click;
			// 
			// btn_setService1
			// 
			btn_setService1.Location = new Point(267, 235);
			btn_setService1.Name = "btn_setService1";
			btn_setService1.Size = new Size(70, 23);
			btn_setService1.TabIndex = 6;
			btn_setService1.Values.Text = "Bind Key";
			btn_setService1.Click += btn_setService1_Click;
			// 
			// kryptonLabel3
			// 
			kryptonLabel3.Location = new Point(12, 267);
			kryptonLabel3.Name = "kryptonLabel3";
			kryptonLabel3.Size = new Size(65, 20);
			kryptonLabel3.TabIndex = 13;
			kryptonLabel3.Values.Text = "Service 2 :";
			// 
			// txt_KeyService2
			// 
			txt_KeyService2.Location = new Point(78, 264);
			txt_KeyService2.Name = "txt_KeyService2";
			txt_KeyService2.ReadOnly = true;
			txt_KeyService2.Size = new Size(183, 23);
			txt_KeyService2.TabIndex = 12;
			// 
			// btn_ClearService2
			// 
			btn_ClearService2.Location = new Point(343, 264);
			btn_ClearService2.Name = "btn_ClearService2";
			btn_ClearService2.Size = new Size(58, 23);
			btn_ClearService2.TabIndex = 11;
			btn_ClearService2.Values.Text = "Clear";
			btn_ClearService2.Click += btn_ClearService2_Click;
			// 
			// btn_setService2
			// 
			btn_setService2.Location = new Point(267, 264);
			btn_setService2.Name = "btn_setService2";
			btn_setService2.Size = new Size(70, 23);
			btn_setService2.TabIndex = 10;
			btn_setService2.Values.Text = "Bind Key";
			btn_setService2.Click += btn_setService2_Click;
			// 
			// chk_showStartup
			// 
			chk_showStartup.Location = new Point(12, 302);
			chk_showStartup.Name = "chk_showStartup";
			chk_showStartup.Size = new Size(231, 20);
			chk_showStartup.TabIndex = 14;
			chk_showStartup.Values.Text = "Show Startup Screen on Game Launch";
			chk_showStartup.CheckedChanged += chk_showStartup_CheckedChanged;
			// 
			// chk_FFB
			// 
			chk_FFB.Location = new Point(12, 328);
			chk_FFB.Name = "chk_FFB";
			chk_FFB.Size = new Size(346, 20);
			chk_FFB.TabIndex = 16;
			chk_FFB.Values.Text = "Auto-Update FFBPlugin Device GUID in the game config ini";
			chk_FFB.CheckedChanged += chk_FFB_CheckedChanged;
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(chk_favorAB);
			groupBox1.Controls.Add(kryptonLabel8);
			groupBox1.Controls.Add(lbl_useCustomStooz_Gamepad);
			groupBox1.Controls.Add(trk_useCustomStooz_Gamepad);
			groupBox1.Controls.Add(chk_enableStoozZone_Gamepad);
			groupBox1.Controls.Add(radio_useCustomStooz_Gamepad);
			groupBox1.Controls.Add(radio_useDefaultStooze_Gamepad);
			groupBox1.Location = new Point(12, 613);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new Size(943, 101);
			groupBox1.TabIndex = 31;
			groupBox1.TabStop = false;
			groupBox1.Text = "GamePad Settings";
			// 
			// chk_favorAB
			// 
			chk_favorAB.Location = new Point(12, 49);
			chk_favorAB.Name = "chk_favorAB";
			chk_favorAB.Size = new Size(354, 20);
			chk_favorAB.TabIndex = 40;
			chk_favorAB.Values.Text = "On Drive game favor A/B for shift Up/Down Instead of LB/RB";
			chk_favorAB.CheckedChanged += chk_favorAB_CheckedChanged;
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
			groupBox2.Controls.Add(btn_configureDinputShifter);
			groupBox2.Controls.Add(chk_useDinputShifter);
			groupBox2.Controls.Add(btn_setffbguid);
			groupBox2.Controls.Add(cmb_ffbguid);
			groupBox2.Controls.Add(kryptonLabel10);
			groupBox2.Controls.Add(txt_ffbguid);
			groupBox2.Controls.Add(btn_configureDinputWheel);
			groupBox2.Controls.Add(chk_useDinputWheel);
			groupBox2.Controls.Add(kryptonLabel9);
			groupBox2.Controls.Add(lbl_useCustomStooz_Wheel);
			groupBox2.Controls.Add(trk_useCustomStooz_Wheel);
			groupBox2.Controls.Add(chk_enableStoozZone_Wheel);
			groupBox2.Controls.Add(radio_useCustomStooz_Wheel);
			groupBox2.Controls.Add(radio_useDefaultStooze_Wheel);
			groupBox2.Location = new Point(12, 720);
			groupBox2.Name = "groupBox2";
			groupBox2.Size = new Size(943, 169);
			groupBox2.TabIndex = 35;
			groupBox2.TabStop = false;
			groupBox2.Text = "Wheel Settings";
			// 
			// btn_configureDinputShifter
			// 
			btn_configureDinputShifter.Location = new Point(308, 132);
			btn_configureDinputShifter.Name = "btn_configureDinputShifter";
			btn_configureDinputShifter.Size = new Size(110, 23);
			btn_configureDinputShifter.TabIndex = 41;
			btn_configureDinputShifter.Values.Text = "Configure";
			btn_configureDinputShifter.Click += btn_configureDinputShifter_Click;
			// 
			// chk_useDinputShifter
			// 
			chk_useDinputShifter.Location = new Point(13, 135);
			chk_useDinputShifter.Name = "chk_useDinputShifter";
			chk_useDinputShifter.Size = new Size(305, 20);
			chk_useDinputShifter.TabIndex = 40;
			chk_useDinputShifter.Values.Text = "Use shifter and/or handbrake with the dinput wheel";
			chk_useDinputShifter.CheckedChanged += chk_useDinputShifter_CheckedChanged;
			// 
			// btn_setffbguid
			// 
			btn_setffbguid.Location = new Point(786, 77);
			btn_setffbguid.Name = "btn_setffbguid";
			btn_setffbguid.Size = new Size(110, 23);
			btn_setffbguid.TabIndex = 39;
			btn_setffbguid.Values.Text = "Set FFB GUID";
			btn_setffbguid.Click += btn_setffbguid_Click;
			// 
			// cmb_ffbguid
			// 
			cmb_ffbguid.DropDownStyle = ComboBoxStyle.DropDownList;
			cmb_ffbguid.DropDownWidth = 483;
			cmb_ffbguid.IntegralHeight = false;
			cmb_ffbguid.Location = new Point(308, 77);
			cmb_ffbguid.Name = "cmb_ffbguid";
			cmb_ffbguid.Size = new Size(467, 21);
			cmb_ffbguid.StateCommon.ComboBox.Content.TextH = Krypton.Toolkit.PaletteRelativeAlign.Near;
			cmb_ffbguid.TabIndex = 38;
			// 
			// kryptonLabel10
			// 
			kryptonLabel10.Location = new Point(13, 77);
			kryptonLabel10.Name = "kryptonLabel10";
			kryptonLabel10.Size = new Size(146, 20);
			kryptonLabel10.TabIndex = 37;
			kryptonLabel10.Values.Text = "Dinput Wheel FFB GUID :";
			// 
			// txt_ffbguid
			// 
			txt_ffbguid.Location = new Point(308, 103);
			txt_ffbguid.Name = "txt_ffbguid";
			txt_ffbguid.Size = new Size(467, 23);
			txt_ffbguid.TabIndex = 37;
			// 
			// btn_configureDinputWheel
			// 
			btn_configureDinputWheel.Location = new Point(308, 48);
			btn_configureDinputWheel.Name = "btn_configureDinputWheel";
			btn_configureDinputWheel.Size = new Size(110, 23);
			btn_configureDinputWheel.TabIndex = 36;
			btn_configureDinputWheel.Values.Text = "Configure";
			btn_configureDinputWheel.Click += btn_configureDinputWheel_Click;
			// 
			// chk_useDinputWheel
			// 
			chk_useDinputWheel.Location = new Point(13, 51);
			chk_useDinputWheel.Name = "chk_useDinputWheel";
			chk_useDinputWheel.Size = new Size(273, 20);
			chk_useDinputWheel.TabIndex = 35;
			chk_useDinputWheel.Values.Text = "Use Dinput Wheel instead of an Xinput Wheel";
			chk_useDinputWheel.CheckedChanged += chk_useDinputWheel_CheckedChanged;
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
			groupBox3.Location = new Point(12, 354);
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
			// btn_Save
			// 
			btn_Save.Location = new Point(859, 940);
			btn_Save.Name = "btn_Save";
			btn_Save.Size = new Size(90, 25);
			btn_Save.TabIndex = 37;
			btn_Save.Values.Text = "Save";
			btn_Save.Click += btn_Save_Click;
			// 
			// txt_tpfolder
			// 
			txt_tpfolder.Enabled = false;
			txt_tpfolder.Location = new Point(141, 19);
			txt_tpfolder.Name = "txt_tpfolder";
			txt_tpfolder.Size = new Size(553, 23);
			txt_tpfolder.TabIndex = 38;
			// 
			// groupBox4
			// 
			groupBox4.Controls.Add(btn_selectTP);
			groupBox4.Controls.Add(kryptonLabel11);
			groupBox4.Controls.Add(txt_tpfolder);
			groupBox4.Location = new Point(8, 6);
			groupBox4.Name = "groupBox4";
			groupBox4.Size = new Size(941, 59);
			groupBox4.TabIndex = 39;
			groupBox4.TabStop = false;
			// 
			// btn_selectTP
			// 
			btn_selectTP.Location = new Point(700, 19);
			btn_selectTP.Name = "btn_selectTP";
			btn_selectTP.Size = new Size(70, 23);
			btn_selectTP.TabIndex = 40;
			btn_selectTP.Values.Text = "...";
			btn_selectTP.Click += btn_selectTP_Click;
			// 
			// kryptonLabel11
			// 
			kryptonLabel11.Location = new Point(6, 22);
			kryptonLabel11.Name = "kryptonLabel11";
			kryptonLabel11.Size = new Size(121, 20);
			kryptonLabel11.TabIndex = 37;
			kryptonLabel11.Values.Text = "Teknoparrot Folder :";
			// 
			// txt_monitorswitch
			// 
			txt_monitorswitch.Location = new Point(264, 146);
			txt_monitorswitch.Name = "txt_monitorswitch";
			txt_monitorswitch.ReadOnly = true;
			txt_monitorswitch.Size = new Size(251, 23);
			txt_monitorswitch.TabIndex = 37;
			// 
			// kryptonLabel12
			// 
			kryptonLabel12.Location = new Point(8, 149);
			kryptonLabel12.Name = "kryptonLabel12";
			kryptonLabel12.Size = new Size(250, 20);
			kryptonLabel12.TabIndex = 37;
			kryptonLabel12.Values.Text = "Change Monitor Resolution/Disposition To : ";
			// 
			// btn_editMonitorSwitch
			// 
			btn_editMonitorSwitch.Location = new Point(521, 146);
			btn_editMonitorSwitch.Name = "btn_editMonitorSwitch";
			btn_editMonitorSwitch.Size = new Size(58, 23);
			btn_editMonitorSwitch.TabIndex = 40;
			btn_editMonitorSwitch.Values.Text = "Edit";
			btn_editMonitorSwitch.Click += btn_editMonitorSwitch_Click;
			// 
			// chk_enableDebug
			// 
			chk_enableDebug.Location = new Point(8, 945);
			chk_enableDebug.Name = "chk_enableDebug";
			chk_enableDebug.Size = new Size(135, 20);
			chk_enableDebug.TabIndex = 41;
			chk_enableDebug.Values.Text = "Enable Debug Mode";
			chk_enableDebug.CheckedChanged += chk_enableDebug_CheckedChanged;
			// 
			// btn_checkConfig
			// 
			btn_checkConfig.Location = new Point(149, 940);
			btn_checkConfig.Name = "btn_checkConfig";
			btn_checkConfig.Size = new Size(223, 25);
			btn_checkConfig.TabIndex = 42;
			btn_checkConfig.Values.Text = "Check if Tp Update broke config files";
			btn_checkConfig.Click += btn_checkConfig_Click;
			// 
			// groupBox5
			// 
			groupBox5.Controls.Add(btn_resetdefaultlinksource);
			groupBox5.Controls.Add(btn_selectLinkFolder);
			groupBox5.Controls.Add(kryptonLabel13);
			groupBox5.Controls.Add(txt_linksourcefolder);
			groupBox5.Location = new Point(8, 71);
			groupBox5.Name = "groupBox5";
			groupBox5.Size = new Size(941, 59);
			groupBox5.TabIndex = 41;
			groupBox5.TabStop = false;
			groupBox5.Text = "Per Game Link Folder";
			// 
			// btn_resetdefaultlinksource
			// 
			btn_resetdefaultlinksource.Location = new Point(776, 19);
			btn_resetdefaultlinksource.Name = "btn_resetdefaultlinksource";
			btn_resetdefaultlinksource.Size = new Size(121, 23);
			btn_resetdefaultlinksource.TabIndex = 37;
			btn_resetdefaultlinksource.Values.Text = "Reset To Default";
			btn_resetdefaultlinksource.Click += btn_resetdefaultlinksource_Click;
			// 
			// btn_selectLinkFolder
			// 
			btn_selectLinkFolder.Location = new Point(700, 19);
			btn_selectLinkFolder.Name = "btn_selectLinkFolder";
			btn_selectLinkFolder.Size = new Size(70, 23);
			btn_selectLinkFolder.TabIndex = 40;
			btn_selectLinkFolder.Values.Text = "...";
			btn_selectLinkFolder.Click += btn_selectLinkFolder_Click;
			// 
			// kryptonLabel13
			// 
			kryptonLabel13.Location = new Point(6, 22);
			kryptonLabel13.Name = "kryptonLabel13";
			kryptonLabel13.Size = new Size(134, 20);
			kryptonLabel13.TabIndex = 37;
			kryptonLabel13.Values.Text = "Per-Game Link Folder :";
			// 
			// txt_linksourcefolder
			// 
			txt_linksourcefolder.Enabled = false;
			txt_linksourcefolder.Location = new Point(141, 19);
			txt_linksourcefolder.Name = "txt_linksourcefolder";
			txt_linksourcefolder.Size = new Size(553, 23);
			txt_linksourcefolder.TabIndex = 38;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(965, 977);
			Controls.Add(groupBox5);
			Controls.Add(btn_checkConfig);
			Controls.Add(chk_enableDebug);
			Controls.Add(btn_editMonitorSwitch);
			Controls.Add(kryptonLabel12);
			Controls.Add(txt_monitorswitch);
			Controls.Add(groupBox4);
			Controls.Add(btn_Save);
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
			Text = "Teknoparrot Auto Xinput Configuration";
			FormClosing += Form1_FormClosing;
			Load += Form1_Load;
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)trk_useCustomStooz_Gamepad).EndInit();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)cmb_ffbguid).EndInit();
			((System.ComponentModel.ISupportInitialize)trk_useCustomStooz_Wheel).EndInit();
			groupBox3.ResumeLayout(false);
			groupBox3.PerformLayout();
			groupBox4.ResumeLayout(false);
			groupBox4.PerformLayout();
			groupBox5.ResumeLayout(false);
			groupBox5.PerformLayout();
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
		private Krypton.Toolkit.KryptonLabel kryptonLabel10;
		private Krypton.Toolkit.KryptonTextBox txt_ffbguid;
		private Krypton.Toolkit.KryptonButton btn_setffbguid;
		private Krypton.Toolkit.KryptonComboBox cmb_ffbguid;
		private Krypton.Toolkit.KryptonCheckBox chk_favorAB;
		private Krypton.Toolkit.KryptonButton btn_Save;
		private Krypton.Toolkit.KryptonTextBox txt_tpfolder;
		private GroupBox groupBox4;
		private Krypton.Toolkit.KryptonButton btn_selectTP;
		private Krypton.Toolkit.KryptonLabel kryptonLabel11;
		private Krypton.Toolkit.KryptonTextBox txt_monitorswitch;
		private Krypton.Toolkit.KryptonLabel kryptonLabel12;
		private Krypton.Toolkit.KryptonButton btn_editMonitorSwitch;
		private Krypton.Toolkit.KryptonCheckBox chk_enableDebug;
		private Krypton.Toolkit.KryptonButton btn_checkConfig;
		private GroupBox groupBox5;
		private Krypton.Toolkit.KryptonButton btn_selectLinkFolder;
		private Krypton.Toolkit.KryptonLabel kryptonLabel13;
		private Krypton.Toolkit.KryptonTextBox txt_linksourcefolder;
		private Krypton.Toolkit.KryptonButton btn_resetdefaultlinksource;
		private Krypton.Toolkit.KryptonButton btn_configureDinputShifter;
		private Krypton.Toolkit.KryptonCheckBox chk_useDinputShifter;
	}
}