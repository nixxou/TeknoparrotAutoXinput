namespace TeknoparrotAutoXinput
{
	partial class gun_preconfig
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			kryptonLabel1 = new Krypton.Toolkit.KryptonLabel();
			lbl_gunindex = new Krypton.Toolkit.KryptonLabel();
			lbl_guntype = new Krypton.Toolkit.KryptonLabel();
			kryptonLabel4 = new Krypton.Toolkit.KryptonLabel();
			grp_gun4ir = new GroupBox();
			label_warning_guncon1 = new Label();
			btn_gunir_done = new Krypton.Toolkit.KryptonButton();
			btn_configure_key_gun4ir = new Krypton.Toolkit.KryptonButton();
			lbl_gunguid = new Krypton.Toolkit.KryptonLabel();
			kryptonLabel3 = new Krypton.Toolkit.KryptonLabel();
			btn_savecomport = new Krypton.Toolkit.KryptonButton();
			kryptonLabel2 = new Krypton.Toolkit.KryptonLabel();
			cmb_comport = new Krypton.Toolkit.KryptonComboBox();
			lbl_presstrigger_gun4ir = new Krypton.Toolkit.KryptonLabel();
			kryptonButton1 = new Krypton.Toolkit.KryptonButton();
			grp_sinden = new GroupBox();
			label1 = new Label();
			btn_siden_enable_joystick = new Krypton.Toolkit.KryptonButton();
			btn_sinden_done = new Krypton.Toolkit.KryptonButton();
			btn_sinden_configurekeys = new Krypton.Toolkit.KryptonButton();
			lbl_sinden_nojoystick = new Label();
			lbl_sinden_wrongversion = new Label();
			lbl_gunguidsinden = new Krypton.Toolkit.KryptonLabel();
			kryptonLabel7 = new Krypton.Toolkit.KryptonLabel();
			kryptonLabel5 = new Krypton.Toolkit.KryptonLabel();
			cmb_selectSinden = new Krypton.Toolkit.KryptonComboBox();
			backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			grp_wiimote = new GroupBox();
			label3 = new Label();
			btn_wiimote_done = new Krypton.Toolkit.KryptonButton();
			btn_wiimote_configurekeys = new Krypton.Toolkit.KryptonButton();
			lbl_gunguidwiimote = new Krypton.Toolkit.KryptonLabel();
			kryptonLabel8 = new Krypton.Toolkit.KryptonLabel();
			lbl_presstrigger_wiimote = new Krypton.Toolkit.KryptonLabel();
			grp_gun4ir.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)cmb_comport).BeginInit();
			grp_sinden.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)cmb_selectSinden).BeginInit();
			grp_wiimote.SuspendLayout();
			SuspendLayout();
			// 
			// kryptonLabel1
			// 
			kryptonLabel1.LabelStyle = Krypton.Toolkit.LabelStyle.BoldControl;
			kryptonLabel1.Location = new Point(12, 12);
			kryptonLabel1.Name = "kryptonLabel1";
			kryptonLabel1.Size = new Size(77, 20);
			kryptonLabel1.TabIndex = 0;
			kryptonLabel1.Values.Text = "Gun Index :";
			// 
			// lbl_gunindex
			// 
			lbl_gunindex.Location = new Point(95, 12);
			lbl_gunindex.Name = "lbl_gunindex";
			lbl_gunindex.Size = new Size(88, 20);
			lbl_gunindex.TabIndex = 1;
			lbl_gunindex.Values.Text = "kryptonLabel2";
			// 
			// lbl_guntype
			// 
			lbl_guntype.Location = new Point(95, 38);
			lbl_guntype.Name = "lbl_guntype";
			lbl_guntype.Size = new Size(88, 20);
			lbl_guntype.TabIndex = 3;
			lbl_guntype.Values.Text = "kryptonLabel3";
			// 
			// kryptonLabel4
			// 
			kryptonLabel4.LabelStyle = Krypton.Toolkit.LabelStyle.BoldControl;
			kryptonLabel4.Location = new Point(12, 38);
			kryptonLabel4.Name = "kryptonLabel4";
			kryptonLabel4.Size = new Size(73, 20);
			kryptonLabel4.TabIndex = 2;
			kryptonLabel4.Values.Text = "Gun Type :";
			// 
			// grp_gun4ir
			// 
			grp_gun4ir.Controls.Add(label_warning_guncon1);
			grp_gun4ir.Controls.Add(btn_gunir_done);
			grp_gun4ir.Controls.Add(btn_configure_key_gun4ir);
			grp_gun4ir.Controls.Add(lbl_gunguid);
			grp_gun4ir.Controls.Add(kryptonLabel3);
			grp_gun4ir.Controls.Add(btn_savecomport);
			grp_gun4ir.Controls.Add(kryptonLabel2);
			grp_gun4ir.Controls.Add(cmb_comport);
			grp_gun4ir.Controls.Add(lbl_presstrigger_gun4ir);
			grp_gun4ir.Location = new Point(814, 64);
			grp_gun4ir.Name = "grp_gun4ir";
			grp_gun4ir.Size = new Size(776, 261);
			grp_gun4ir.TabIndex = 4;
			grp_gun4ir.TabStop = false;
			// 
			// label_warning_guncon1
			// 
			label_warning_guncon1.AutoSize = true;
			label_warning_guncon1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
			label_warning_guncon1.ForeColor = Color.Brown;
			label_warning_guncon1.Location = new Point(44, 168);
			label_warning_guncon1.Name = "label_warning_guncon1";
			label_warning_guncon1.Size = new Size(677, 21);
			label_warning_guncon1.TabIndex = 63;
			label_warning_guncon1.Text = "For guncon1, you need to set Extra keys Coin / Start to some device other than your gun";
			label_warning_guncon1.Visible = false;
			// 
			// btn_gunir_done
			// 
			btn_gunir_done.Enabled = false;
			btn_gunir_done.Location = new Point(402, 206);
			btn_gunir_done.Name = "btn_gunir_done";
			btn_gunir_done.Size = new Size(368, 49);
			btn_gunir_done.TabIndex = 62;
			btn_gunir_done.Values.Text = "Done";
			btn_gunir_done.Click += btn_gunir_done_Click;
			// 
			// btn_configure_key_gun4ir
			// 
			btn_configure_key_gun4ir.Enabled = false;
			btn_configure_key_gun4ir.Location = new Point(6, 206);
			btn_configure_key_gun4ir.Name = "btn_configure_key_gun4ir";
			btn_configure_key_gun4ir.Size = new Size(368, 49);
			btn_configure_key_gun4ir.TabIndex = 61;
			btn_configure_key_gun4ir.Values.Text = "Configure Pedals and Extra Keys";
			btn_configure_key_gun4ir.Click += btn_configure_key_gun4ir_Click;
			// 
			// lbl_gunguid
			// 
			lbl_gunguid.Location = new Point(143, 50);
			lbl_gunguid.Name = "lbl_gunguid";
			lbl_gunguid.Size = new Size(55, 20);
			lbl_gunguid.TabIndex = 60;
			lbl_gunguid.Values.Text = "<none>";
			// 
			// kryptonLabel3
			// 
			kryptonLabel3.LabelStyle = Krypton.Toolkit.LabelStyle.BoldControl;
			kryptonLabel3.Location = new Point(16, 50);
			kryptonLabel3.Name = "kryptonLabel3";
			kryptonLabel3.Size = new Size(72, 20);
			kryptonLabel3.TabIndex = 59;
			kryptonLabel3.Values.Text = "Gun Guid :";
			// 
			// btn_savecomport
			// 
			btn_savecomport.Enabled = false;
			btn_savecomport.Location = new Point(270, 22);
			btn_savecomport.Name = "btn_savecomport";
			btn_savecomport.Size = new Size(55, 21);
			btn_savecomport.TabIndex = 58;
			btn_savecomport.Values.Text = "Done";
			btn_savecomport.Click += btn_savecomport_Click;
			// 
			// kryptonLabel2
			// 
			kryptonLabel2.LabelStyle = Krypton.Toolkit.LabelStyle.BoldControl;
			kryptonLabel2.Location = new Point(16, 23);
			kryptonLabel2.Name = "kryptonLabel2";
			kryptonLabel2.Size = new Size(110, 20);
			kryptonLabel2.TabIndex = 57;
			kryptonLabel2.Values.Text = "Select Com port :";
			// 
			// cmb_comport
			// 
			cmb_comport.DropDownStyle = ComboBoxStyle.DropDownList;
			cmb_comport.DropDownWidth = 121;
			cmb_comport.IntegralHeight = false;
			cmb_comport.Location = new Point(143, 23);
			cmb_comport.Name = "cmb_comport";
			cmb_comport.Size = new Size(121, 21);
			cmb_comport.StateCommon.ComboBox.Content.TextH = Krypton.Toolkit.PaletteRelativeAlign.Near;
			cmb_comport.TabIndex = 56;
			cmb_comport.SelectedIndexChanged += cmb_comport_SelectedIndexChanged;
			// 
			// lbl_presstrigger_gun4ir
			// 
			lbl_presstrigger_gun4ir.LabelStyle = Krypton.Toolkit.LabelStyle.TitlePanel;
			lbl_presstrigger_gun4ir.Location = new Point(210, 91);
			lbl_presstrigger_gun4ir.Name = "lbl_presstrigger_gun4ir";
			lbl_presstrigger_gun4ir.Size = new Size(294, 29);
			lbl_presstrigger_gun4ir.TabIndex = 55;
			lbl_presstrigger_gun4ir.Values.Text = "Press Trigger to detect Lightgun";
			lbl_presstrigger_gun4ir.Visible = false;
			// 
			// kryptonButton1
			// 
			kryptonButton1.Location = new Point(692, 12);
			kryptonButton1.Name = "kryptonButton1";
			kryptonButton1.Size = new Size(90, 25);
			kryptonButton1.TabIndex = 5;
			kryptonButton1.Values.Text = "kryptonButton1";
			kryptonButton1.Visible = false;
			kryptonButton1.Click += kryptonButton1_Click;
			// 
			// grp_sinden
			// 
			grp_sinden.Controls.Add(label1);
			grp_sinden.Controls.Add(btn_siden_enable_joystick);
			grp_sinden.Controls.Add(btn_sinden_done);
			grp_sinden.Controls.Add(btn_sinden_configurekeys);
			grp_sinden.Controls.Add(lbl_sinden_nojoystick);
			grp_sinden.Controls.Add(lbl_sinden_wrongversion);
			grp_sinden.Controls.Add(lbl_gunguidsinden);
			grp_sinden.Controls.Add(kryptonLabel7);
			grp_sinden.Controls.Add(kryptonLabel5);
			grp_sinden.Controls.Add(cmb_selectSinden);
			grp_sinden.Location = new Point(12, 64);
			grp_sinden.Name = "grp_sinden";
			grp_sinden.Size = new Size(776, 261);
			grp_sinden.TabIndex = 6;
			grp_sinden.TabStop = false;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
			label1.ForeColor = Color.Brown;
			label1.Location = new Point(357, 22);
			label1.Name = "label1";
			label1.Size = new Size(330, 40);
			label1.TabIndex = 69;
			label1.Text = "The wizard will close the sinden Lightgun app.\r\nThat's normal and not a bug";
			label1.Visible = false;
			// 
			// btn_siden_enable_joystick
			// 
			btn_siden_enable_joystick.Enabled = false;
			btn_siden_enable_joystick.Location = new Point(11, 162);
			btn_siden_enable_joystick.Name = "btn_siden_enable_joystick";
			btn_siden_enable_joystick.Size = new Size(753, 27);
			btn_siden_enable_joystick.TabIndex = 68;
			btn_siden_enable_joystick.Values.Text = "Enable Joystick Mode";
			btn_siden_enable_joystick.Visible = false;
			btn_siden_enable_joystick.Click += btn_siden_enable_joystick_Click;
			// 
			// btn_sinden_done
			// 
			btn_sinden_done.Enabled = false;
			btn_sinden_done.Location = new Point(396, 201);
			btn_sinden_done.Name = "btn_sinden_done";
			btn_sinden_done.Size = new Size(368, 49);
			btn_sinden_done.TabIndex = 67;
			btn_sinden_done.Values.Text = "Done";
			btn_sinden_done.Click += btn_sinden_done_Click;
			// 
			// btn_sinden_configurekeys
			// 
			btn_sinden_configurekeys.Enabled = false;
			btn_sinden_configurekeys.Location = new Point(11, 201);
			btn_sinden_configurekeys.Name = "btn_sinden_configurekeys";
			btn_sinden_configurekeys.Size = new Size(368, 49);
			btn_sinden_configurekeys.TabIndex = 66;
			btn_sinden_configurekeys.Values.Text = "Configure Pedals and Extra Keys";
			btn_sinden_configurekeys.Click += btn_sinden_configurekeys_Click;
			// 
			// lbl_sinden_nojoystick
			// 
			lbl_sinden_nojoystick.AutoSize = true;
			lbl_sinden_nojoystick.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
			lbl_sinden_nojoystick.ForeColor = Color.Brown;
			lbl_sinden_nojoystick.Location = new Point(11, 119);
			lbl_sinden_nojoystick.Name = "lbl_sinden_nojoystick";
			lbl_sinden_nojoystick.Size = new Size(700, 40);
			lbl_sinden_nojoystick.TabIndex = 65;
			lbl_sinden_nojoystick.Text = "No Joystick found, click to enable joystick mode, your gun will still be able to work in mouse mode \r\nbut you will have to reconfigure some emulators since it change your gun identifier";
			lbl_sinden_nojoystick.Visible = false;
			// 
			// lbl_sinden_wrongversion
			// 
			lbl_sinden_wrongversion.AutoSize = true;
			lbl_sinden_wrongversion.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
			lbl_sinden_wrongversion.ForeColor = Color.Brown;
			lbl_sinden_wrongversion.Location = new Point(10, 91);
			lbl_sinden_wrongversion.Name = "lbl_sinden_wrongversion";
			lbl_sinden_wrongversion.Size = new Size(743, 20);
			lbl_sinden_wrongversion.TabIndex = 64;
			lbl_sinden_wrongversion.Text = "No Joystick found, your gun firmware is XX, you must update firmware to v1.9 and enable joystick mode";
			lbl_sinden_wrongversion.Visible = false;
			// 
			// lbl_gunguidsinden
			// 
			lbl_gunguidsinden.Location = new Point(133, 50);
			lbl_gunguidsinden.Name = "lbl_gunguidsinden";
			lbl_gunguidsinden.Size = new Size(55, 20);
			lbl_gunguidsinden.TabIndex = 62;
			lbl_gunguidsinden.Values.Text = "<none>";
			// 
			// kryptonLabel7
			// 
			kryptonLabel7.LabelStyle = Krypton.Toolkit.LabelStyle.BoldControl;
			kryptonLabel7.Location = new Point(6, 50);
			kryptonLabel7.Name = "kryptonLabel7";
			kryptonLabel7.Size = new Size(72, 20);
			kryptonLabel7.TabIndex = 61;
			kryptonLabel7.Values.Text = "Gun Guid :";
			// 
			// kryptonLabel5
			// 
			kryptonLabel5.LabelStyle = Krypton.Toolkit.LabelStyle.BoldControl;
			kryptonLabel5.Location = new Point(6, 24);
			kryptonLabel5.Name = "kryptonLabel5";
			kryptonLabel5.Size = new Size(121, 20);
			kryptonLabel5.TabIndex = 58;
			kryptonLabel5.Values.Text = "Select Sinden gun :";
			// 
			// cmb_selectSinden
			// 
			cmb_selectSinden.DropDownStyle = ComboBoxStyle.DropDownList;
			cmb_selectSinden.DropDownWidth = 121;
			cmb_selectSinden.IntegralHeight = false;
			cmb_selectSinden.Location = new Point(133, 23);
			cmb_selectSinden.Name = "cmb_selectSinden";
			cmb_selectSinden.Size = new Size(218, 21);
			cmb_selectSinden.StateCommon.ComboBox.Content.TextH = Krypton.Toolkit.PaletteRelativeAlign.Near;
			cmb_selectSinden.TabIndex = 57;
			cmb_selectSinden.SelectedIndexChanged += cmb_selectSinden_SelectedIndexChanged;
			// 
			// grp_wiimote
			// 
			grp_wiimote.Controls.Add(label3);
			grp_wiimote.Controls.Add(btn_wiimote_done);
			grp_wiimote.Controls.Add(btn_wiimote_configurekeys);
			grp_wiimote.Controls.Add(lbl_gunguidwiimote);
			grp_wiimote.Controls.Add(kryptonLabel8);
			grp_wiimote.Controls.Add(lbl_presstrigger_wiimote);
			grp_wiimote.Location = new Point(12, 342);
			grp_wiimote.Name = "grp_wiimote";
			grp_wiimote.Size = new Size(776, 261);
			grp_wiimote.TabIndex = 64;
			grp_wiimote.TabStop = false;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
			label3.ForeColor = Color.Brown;
			label3.Location = new Point(271, 19);
			label3.Name = "label3";
			label3.Size = new Size(493, 20);
			label3.TabIndex = 70;
			label3.Text = "Lichtknarre app must be up and running and your wiimote connected";
			label3.Visible = false;
			// 
			// btn_wiimote_done
			// 
			btn_wiimote_done.Enabled = false;
			btn_wiimote_done.Location = new Point(402, 206);
			btn_wiimote_done.Name = "btn_wiimote_done";
			btn_wiimote_done.Size = new Size(368, 49);
			btn_wiimote_done.TabIndex = 62;
			btn_wiimote_done.Values.Text = "Done";
			btn_wiimote_done.Click += btn_wiimote_done_Click;
			// 
			// btn_wiimote_configurekeys
			// 
			btn_wiimote_configurekeys.Enabled = false;
			btn_wiimote_configurekeys.Location = new Point(6, 206);
			btn_wiimote_configurekeys.Name = "btn_wiimote_configurekeys";
			btn_wiimote_configurekeys.Size = new Size(368, 49);
			btn_wiimote_configurekeys.TabIndex = 61;
			btn_wiimote_configurekeys.Values.Text = "Configure Pedals and Extra Keys";
			btn_wiimote_configurekeys.Click += btn_wiimote_configurekeys_Click;
			// 
			// lbl_gunguidwiimote
			// 
			lbl_gunguidwiimote.Location = new Point(94, 22);
			lbl_gunguidwiimote.Name = "lbl_gunguidwiimote";
			lbl_gunguidwiimote.Size = new Size(55, 20);
			lbl_gunguidwiimote.TabIndex = 60;
			lbl_gunguidwiimote.Values.Text = "<none>";
			// 
			// kryptonLabel8
			// 
			kryptonLabel8.LabelStyle = Krypton.Toolkit.LabelStyle.BoldControl;
			kryptonLabel8.Location = new Point(16, 21);
			kryptonLabel8.Name = "kryptonLabel8";
			kryptonLabel8.Size = new Size(72, 20);
			kryptonLabel8.TabIndex = 59;
			kryptonLabel8.Values.Text = "Gun Guid :";
			// 
			// lbl_presstrigger_wiimote
			// 
			lbl_presstrigger_wiimote.LabelStyle = Krypton.Toolkit.LabelStyle.TitlePanel;
			lbl_presstrigger_wiimote.Location = new Point(210, 91);
			lbl_presstrigger_wiimote.Name = "lbl_presstrigger_wiimote";
			lbl_presstrigger_wiimote.Size = new Size(294, 29);
			lbl_presstrigger_wiimote.TabIndex = 55;
			lbl_presstrigger_wiimote.Values.Text = "Press Trigger to detect Lightgun";
			lbl_presstrigger_wiimote.Visible = false;
			// 
			// gun_preconfig
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1631, 704);
			Controls.Add(grp_wiimote);
			Controls.Add(grp_sinden);
			Controls.Add(grp_gun4ir);
			Controls.Add(kryptonButton1);
			Controls.Add(lbl_guntype);
			Controls.Add(kryptonLabel4);
			Controls.Add(lbl_gunindex);
			Controls.Add(kryptonLabel1);
			Name = "gun_preconfig";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "gun_preconfig";
			Load += gun_preconfig_Load;
			grp_gun4ir.ResumeLayout(false);
			grp_gun4ir.PerformLayout();
			((System.ComponentModel.ISupportInitialize)cmb_comport).EndInit();
			grp_sinden.ResumeLayout(false);
			grp_sinden.PerformLayout();
			((System.ComponentModel.ISupportInitialize)cmb_selectSinden).EndInit();
			grp_wiimote.ResumeLayout(false);
			grp_wiimote.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Krypton.Toolkit.KryptonLabel kryptonLabel1;
		private Krypton.Toolkit.KryptonLabel lbl_gunindex;
		private Krypton.Toolkit.KryptonLabel lbl_guntype;
		private Krypton.Toolkit.KryptonLabel kryptonLabel4;
		private GroupBox grp_gun4ir;
		private Krypton.Toolkit.KryptonLabel lbl_presstrigger_gun4ir;
		private Krypton.Toolkit.KryptonLabel kryptonLabel2;
		private Krypton.Toolkit.KryptonComboBox cmb_comport;
		private Krypton.Toolkit.KryptonButton btn_savecomport;
		private Krypton.Toolkit.KryptonLabel lbl_gunguid;
		private Krypton.Toolkit.KryptonLabel kryptonLabel3;
		private Krypton.Toolkit.KryptonButton btn_configure_key_gun4ir;
		private Krypton.Toolkit.KryptonButton btn_gunir_done;
		private Krypton.Toolkit.KryptonButton kryptonButton1;
		private Label label_warning_guncon1;
		private GroupBox grp_sinden;
		private Krypton.Toolkit.KryptonComboBox cmb_selectSinden;
		private System.ComponentModel.BackgroundWorker backgroundWorker1;
		private Label lbl_sinden_wrongversion;
		private Krypton.Toolkit.KryptonLabel lbl_gunguidsinden;
		private Krypton.Toolkit.KryptonLabel kryptonLabel7;
		private Krypton.Toolkit.KryptonLabel kryptonLabel5;
		private Label lbl_sinden_nojoystick;
		private Krypton.Toolkit.KryptonButton btn_sinden_done;
		private Krypton.Toolkit.KryptonButton btn_sinden_configurekeys;
		private Krypton.Toolkit.KryptonButton btn_siden_enable_joystick;
		private Label label1;
		private GroupBox grp_wiimote;
		private Label label3;
		private Krypton.Toolkit.KryptonButton btn_wiimote_done;
		private Krypton.Toolkit.KryptonButton btn_wiimote_configurekeys;
		private Krypton.Toolkit.KryptonLabel lbl_gunguidwiimote;
		private Krypton.Toolkit.KryptonLabel kryptonLabel8;
		private Krypton.Toolkit.KryptonLabel lbl_presstrigger_wiimote;
	}
}