using SharpDX;

namespace TeknoparrotAutoXinput
{
	partial class Main
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
			components = new System.ComponentModel.Container();
			list_games = new Krypton.Toolkit.KryptonListBox();
			btn_globalconfig = new Krypton.Toolkit.KryptonButton();
			btn_playgame = new Krypton.Toolkit.KryptonButton();
			btn_playgamedirect = new Krypton.Toolkit.KryptonButton();
			btn_gameoptions = new Krypton.Toolkit.KryptonButton();
			chk_showAll = new Krypton.Toolkit.KryptonCheckBox();
			groupBox1 = new GroupBox();
			lbl_gunslist = new Krypton.Toolkit.KryptonLabel();
			kryptonLabel6 = new Krypton.Toolkit.KryptonLabel();
			button8 = new Button();
			button7 = new Button();
			lbl_hotaslist = new Krypton.Toolkit.KryptonLabel();
			kryptonLabel5 = new Krypton.Toolkit.KryptonLabel();
			button5 = new Button();
			lbl_wheellist = new Krypton.Toolkit.KryptonLabel();
			button6 = new Button();
			button9 = new Button();
			lbl_arcadelist = new Krypton.Toolkit.KryptonLabel();
			lbl_gamepadlist = new Krypton.Toolkit.KryptonLabel();
			button4 = new Button();
			kryptonLabel3 = new Krypton.Toolkit.KryptonLabel();
			kryptonLabel2 = new Krypton.Toolkit.KryptonLabel();
			button2 = new Button();
			kryptonLabel1 = new Krypton.Toolkit.KryptonLabel();
			button1 = new Button();
			button3 = new Button();
			timer_controllerUpdate = new System.Windows.Forms.Timer(components);
			pictureBox_gameControls = new PictureBox();
			lbl_GameTitle = new Krypton.Toolkit.KryptonLabel();
			flowLayoutPanelThumbs = new FlowLayoutPanel();
			kryptonPictureBox1 = new Krypton.Toolkit.KryptonPictureBox();
			lbl_player1 = new Krypton.Toolkit.KryptonLabel();
			lbl_player2 = new Krypton.Toolkit.KryptonLabel();
			lbl_player3 = new Krypton.Toolkit.KryptonLabel();
			lbl_player4 = new Krypton.Toolkit.KryptonLabel();
			btn_tpsettings = new Krypton.Toolkit.KryptonButton();
			kryptonLabel4 = new Krypton.Toolkit.KryptonLabel();
			kryptonLabel7 = new Krypton.Toolkit.KryptonLabel();
			kryptonLabel8 = new Krypton.Toolkit.KryptonLabel();
			cmb_patchReshade = new Krypton.Toolkit.KryptonComboBox();
			cmb_displayMode = new Krypton.Toolkit.KryptonComboBox();
			cmb_resolution = new Krypton.Toolkit.KryptonComboBox();
			groupBox2 = new GroupBox();
			cmb_patchlink = new Krypton.Toolkit.KryptonComboBox();
			kryptonLabel9 = new Krypton.Toolkit.KryptonLabel();
			btn_playgamedirect2 = new Krypton.Toolkit.KryptonButton();
			kryptonButton1 = new Krypton.Toolkit.KryptonButton();
			panel_ffb = new Panel();
			label2 = new Label();
			lbl_ffb = new Label();
			panel_crt = new Panel();
			label1 = new Label();
			lbl_crt = new Label();
			panel_bezel = new Panel();
			label3 = new Label();
			lbl_bezel = new Label();
			panel_vsync = new Panel();
			label6 = new Label();
			lbl_vsync = new Label();
			panel_aspectratio = new Panel();
			label8 = new Label();
			lbl_aspectratio = new Label();
			flowLayoutPanel1 = new FlowLayoutPanel();
			label10 = new Label();
			lbl_translation = new Label();
			groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)pictureBox_gameControls).BeginInit();
			flowLayoutPanelThumbs.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)kryptonPictureBox1).BeginInit();
			((System.ComponentModel.ISupportInitialize)cmb_patchReshade).BeginInit();
			((System.ComponentModel.ISupportInitialize)cmb_displayMode).BeginInit();
			((System.ComponentModel.ISupportInitialize)cmb_resolution).BeginInit();
			groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)cmb_patchlink).BeginInit();
			panel_ffb.SuspendLayout();
			panel_crt.SuspendLayout();
			panel_bezel.SuspendLayout();
			panel_vsync.SuspendLayout();
			panel_aspectratio.SuspendLayout();
			flowLayoutPanel1.SuspendLayout();
			SuspendLayout();
			// 
			// list_games
			// 
			list_games.Enabled = false;
			list_games.Location = new Point(12, 42);
			list_games.Name = "list_games";
			list_games.Size = new Size(482, 534);
			list_games.TabIndex = 0;
			list_games.SelectedIndexChanged += list_games_SelectedIndexChanged;
			// 
			// btn_globalconfig
			// 
			btn_globalconfig.Location = new Point(12, 6);
			btn_globalconfig.Name = "btn_globalconfig";
			btn_globalconfig.Size = new Size(284, 29);
			btn_globalconfig.TabIndex = 1;
			btn_globalconfig.Values.Text = "Edit TeknoparrotAutoXinput Global Configuration";
			btn_globalconfig.Click += btn_globalconfig_Click;
			// 
			// btn_playgame
			// 
			btn_playgame.Enabled = false;
			btn_playgame.Location = new Point(511, 492);
			btn_playgame.Name = "btn_playgame";
			btn_playgame.Size = new Size(476, 36);
			btn_playgame.TabIndex = 2;
			btn_playgame.Values.Text = "PLAY GAME ! (With Auto-set Bindings)";
			btn_playgame.Click += btn_playgame_Click;
			// 
			// btn_playgamedirect
			// 
			btn_playgamedirect.Enabled = false;
			btn_playgamedirect.Location = new Point(752, 534);
			btn_playgamedirect.Name = "btn_playgamedirect";
			btn_playgamedirect.Size = new Size(235, 29);
			btn_playgamedirect.TabIndex = 3;
			btn_playgamedirect.Values.Text = "Play without altering controller bindings";
			btn_playgamedirect.Click += btn_playgamedirect_Click;
			// 
			// btn_gameoptions
			// 
			btn_gameoptions.Enabled = false;
			btn_gameoptions.Location = new Point(511, 569);
			btn_gameoptions.Name = "btn_gameoptions";
			btn_gameoptions.Size = new Size(235, 29);
			btn_gameoptions.TabIndex = 4;
			btn_gameoptions.Values.Text = "Game Options";
			btn_gameoptions.Click += btn_gameoptions_Click;
			// 
			// chk_showAll
			// 
			chk_showAll.Location = new Point(12, 582);
			chk_showAll.Name = "chk_showAll";
			chk_showAll.Size = new Size(326, 20);
			chk_showAll.TabIndex = 6;
			chk_showAll.Values.Text = "Show All games even if not supported by TPAutoXinput";
			chk_showAll.CheckedChanged += chk_showAll_CheckedChanged;
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(lbl_gunslist);
			groupBox1.Controls.Add(kryptonLabel6);
			groupBox1.Controls.Add(button8);
			groupBox1.Controls.Add(button7);
			groupBox1.Controls.Add(lbl_hotaslist);
			groupBox1.Controls.Add(kryptonLabel5);
			groupBox1.Controls.Add(button5);
			groupBox1.Controls.Add(lbl_wheellist);
			groupBox1.Controls.Add(button6);
			groupBox1.Controls.Add(button9);
			groupBox1.Controls.Add(lbl_arcadelist);
			groupBox1.Controls.Add(lbl_gamepadlist);
			groupBox1.Controls.Add(button4);
			groupBox1.Controls.Add(kryptonLabel3);
			groupBox1.Controls.Add(kryptonLabel2);
			groupBox1.Controls.Add(button2);
			groupBox1.Controls.Add(kryptonLabel1);
			groupBox1.Controls.Add(button1);
			groupBox1.Controls.Add(button3);
			groupBox1.Location = new Point(12, 608);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new Size(715, 132);
			groupBox1.TabIndex = 7;
			groupBox1.TabStop = false;
			groupBox1.Text = "Controller Status";
			// 
			// lbl_gunslist
			// 
			lbl_gunslist.Location = new Point(129, 108);
			lbl_gunslist.Name = "lbl_gunslist";
			lbl_gunslist.Size = new Size(19, 20);
			lbl_gunslist.TabIndex = 13;
			lbl_gunslist.Values.Text = "...";
			// 
			// kryptonLabel6
			// 
			kryptonLabel6.Location = new Point(6, 108);
			kryptonLabel6.Name = "kryptonLabel6";
			kryptonLabel6.Size = new Size(61, 20);
			kryptonLabel6.TabIndex = 12;
			kryptonLabel6.Values.Text = "Gun List :";
			// 
			// button8
			// 
			button8.Location = new Point(487, 107);
			button8.Name = "button8";
			button8.Size = new Size(75, 23);
			button8.TabIndex = 17;
			button8.Text = "button8";
			button8.UseVisualStyleBackColor = true;
			button8.Visible = false;
			button8.Click += button8_Click;
			// 
			// button7
			// 
			button7.Location = new Point(499, 78);
			button7.Name = "button7";
			button7.Size = new Size(75, 23);
			button7.TabIndex = 16;
			button7.Text = "button7";
			button7.UseVisualStyleBackColor = true;
			button7.Visible = false;
			button7.Click += button7_Click;
			// 
			// lbl_hotaslist
			// 
			lbl_hotaslist.Location = new Point(129, 86);
			lbl_hotaslist.Name = "lbl_hotaslist";
			lbl_hotaslist.Size = new Size(19, 20);
			lbl_hotaslist.TabIndex = 7;
			lbl_hotaslist.Values.Text = "...";
			// 
			// kryptonLabel5
			// 
			kryptonLabel5.Location = new Point(6, 86);
			kryptonLabel5.Name = "kryptonLabel5";
			kryptonLabel5.Size = new Size(70, 20);
			kryptonLabel5.TabIndex = 6;
			kryptonLabel5.Values.Text = "Hotas List :";
			// 
			// button5
			// 
			button5.Location = new Point(580, 78);
			button5.Name = "button5";
			button5.Size = new Size(75, 23);
			button5.TabIndex = 14;
			button5.Text = "button5";
			button5.UseVisualStyleBackColor = true;
			button5.Visible = false;
			button5.Click += button5_Click;
			// 
			// lbl_wheellist
			// 
			lbl_wheellist.Location = new Point(129, 65);
			lbl_wheellist.Name = "lbl_wheellist";
			lbl_wheellist.Size = new Size(19, 20);
			lbl_wheellist.TabIndex = 5;
			lbl_wheellist.Values.Text = "...";
			lbl_wheellist.Click += kryptonLabel6_Click;
			// 
			// button6
			// 
			button6.Location = new Point(568, 107);
			button6.Name = "button6";
			button6.Size = new Size(75, 23);
			button6.TabIndex = 15;
			button6.Text = "button6";
			button6.UseVisualStyleBackColor = true;
			button6.Visible = false;
			button6.Click += button6_Click;
			// 
			// button9
			// 
			button9.Location = new Point(499, 50);
			button9.Name = "button9";
			button9.Size = new Size(75, 23);
			button9.TabIndex = 18;
			button9.Text = "button9";
			button9.UseVisualStyleBackColor = true;
			button9.Visible = false;
			button9.Click += button9_Click;
			// 
			// lbl_arcadelist
			// 
			lbl_arcadelist.Location = new Point(129, 45);
			lbl_arcadelist.Name = "lbl_arcadelist";
			lbl_arcadelist.Size = new Size(19, 20);
			lbl_arcadelist.TabIndex = 4;
			lbl_arcadelist.Values.Text = "...";
			// 
			// lbl_gamepadlist
			// 
			lbl_gamepadlist.Location = new Point(129, 22);
			lbl_gamepadlist.Name = "lbl_gamepadlist";
			lbl_gamepadlist.Size = new Size(19, 20);
			lbl_gamepadlist.TabIndex = 3;
			lbl_gamepadlist.Values.Text = "...";
			// 
			// button4
			// 
			button4.Location = new Point(580, 50);
			button4.Name = "button4";
			button4.Size = new Size(75, 23);
			button4.TabIndex = 11;
			button4.Text = "button4";
			button4.UseVisualStyleBackColor = true;
			button4.Visible = false;
			button4.Click += button4_Click;
			// 
			// kryptonLabel3
			// 
			kryptonLabel3.Location = new Point(6, 65);
			kryptonLabel3.Name = "kryptonLabel3";
			kryptonLabel3.Size = new Size(73, 20);
			kryptonLabel3.TabIndex = 2;
			kryptonLabel3.Values.Text = "Wheel List :";
			// 
			// kryptonLabel2
			// 
			kryptonLabel2.Location = new Point(6, 45);
			kryptonLabel2.Name = "kryptonLabel2";
			kryptonLabel2.Size = new Size(105, 20);
			kryptonLabel2.TabIndex = 1;
			kryptonLabel2.Values.Text = "Arcade Stick List :";
			// 
			// button2
			// 
			button2.Location = new Point(659, 78);
			button2.Name = "button2";
			button2.Size = new Size(75, 23);
			button2.TabIndex = 9;
			button2.Text = "button2";
			button2.UseVisualStyleBackColor = true;
			button2.Click += button2_Click;
			// 
			// kryptonLabel1
			// 
			kryptonLabel1.Location = new Point(6, 22);
			kryptonLabel1.Name = "kryptonLabel1";
			kryptonLabel1.Size = new Size(91, 20);
			kryptonLabel1.TabIndex = 0;
			kryptonLabel1.Values.Text = "GamePad List :";
			// 
			// button1
			// 
			button1.Location = new Point(659, 50);
			button1.Name = "button1";
			button1.Size = new Size(75, 23);
			button1.TabIndex = 8;
			button1.Text = "button1";
			button1.UseVisualStyleBackColor = true;
			button1.Visible = false;
			button1.Click += button1_Click_1;
			// 
			// button3
			// 
			button3.Location = new Point(647, 107);
			button3.Name = "button3";
			button3.Size = new Size(75, 23);
			button3.TabIndex = 10;
			button3.Text = "button3";
			button3.UseVisualStyleBackColor = true;
			button3.Click += button3_Click;
			// 
			// timer_controllerUpdate
			// 
			timer_controllerUpdate.Interval = 2000;
			timer_controllerUpdate.Tick += timer_controllerUpdate_Tick;
			// 
			// pictureBox_gameControls
			// 
			pictureBox_gameControls.Location = new Point(521, 42);
			pictureBox_gameControls.Name = "pictureBox_gameControls";
			pictureBox_gameControls.Size = new Size(466, 295);
			pictureBox_gameControls.SizeMode = PictureBoxSizeMode.Zoom;
			pictureBox_gameControls.TabIndex = 8;
			pictureBox_gameControls.TabStop = false;
			pictureBox_gameControls.Click += pictureBox1_Click;
			// 
			// lbl_GameTitle
			// 
			lbl_GameTitle.LabelStyle = Krypton.Toolkit.LabelStyle.TitleControl;
			lbl_GameTitle.Location = new Point(521, 4);
			lbl_GameTitle.Name = "lbl_GameTitle";
			lbl_GameTitle.Size = new Size(53, 29);
			lbl_GameTitle.TabIndex = 9;
			lbl_GameTitle.Values.Text = "Titre";
			// 
			// flowLayoutPanelThumbs
			// 
			flowLayoutPanelThumbs.Controls.Add(kryptonPictureBox1);
			flowLayoutPanelThumbs.Location = new Point(521, 338);
			flowLayoutPanelThumbs.Name = "flowLayoutPanelThumbs";
			flowLayoutPanelThumbs.Size = new Size(466, 60);
			flowLayoutPanelThumbs.TabIndex = 10;
			flowLayoutPanelThumbs.WrapContents = false;
			flowLayoutPanelThumbs.Paint += flowLayoutPanel1_Paint;
			// 
			// kryptonPictureBox1
			// 
			kryptonPictureBox1.Location = new Point(3, 3);
			kryptonPictureBox1.Name = "kryptonPictureBox1";
			kryptonPictureBox1.Size = new Size(77, 50);
			kryptonPictureBox1.TabIndex = 0;
			kryptonPictureBox1.TabStop = false;
			// 
			// lbl_player1
			// 
			lbl_player1.Location = new Point(524, 404);
			lbl_player1.Name = "lbl_player1";
			lbl_player1.Size = new Size(88, 20);
			lbl_player1.TabIndex = 11;
			lbl_player1.Values.Text = "kryptonLabel4";
			// 
			// lbl_player2
			// 
			lbl_player2.Location = new Point(524, 425);
			lbl_player2.Name = "lbl_player2";
			lbl_player2.Size = new Size(88, 20);
			lbl_player2.TabIndex = 12;
			lbl_player2.Values.Text = "kryptonLabel5";
			// 
			// lbl_player3
			// 
			lbl_player3.Location = new Point(524, 446);
			lbl_player3.Name = "lbl_player3";
			lbl_player3.Size = new Size(88, 20);
			lbl_player3.TabIndex = 13;
			lbl_player3.Values.Text = "kryptonLabel6";
			lbl_player3.Click += kryptonLabel6_Click_1;
			// 
			// lbl_player4
			// 
			lbl_player4.Location = new Point(524, 466);
			lbl_player4.Name = "lbl_player4";
			lbl_player4.Size = new Size(88, 20);
			lbl_player4.TabIndex = 14;
			lbl_player4.Values.Text = "kryptonLabel7";
			// 
			// btn_tpsettings
			// 
			btn_tpsettings.Enabled = false;
			btn_tpsettings.Location = new Point(752, 569);
			btn_tpsettings.Name = "btn_tpsettings";
			btn_tpsettings.Size = new Size(235, 29);
			btn_tpsettings.TabIndex = 15;
			btn_tpsettings.Values.Text = "TP Game Settings";
			btn_tpsettings.Click += btn_tpsettings_Click;
			// 
			// kryptonLabel4
			// 
			kryptonLabel4.Location = new Point(10, 22);
			kryptonLabel4.Name = "kryptonLabel4";
			kryptonLabel4.Size = new Size(91, 20);
			kryptonLabel4.TabIndex = 17;
			kryptonLabel4.Values.Text = "Display Mode :";
			kryptonLabel4.Click += kryptonLabel4_Click;
			// 
			// kryptonLabel7
			// 
			kryptonLabel7.Location = new Point(9, 50);
			kryptonLabel7.Name = "kryptonLabel7";
			kryptonLabel7.Size = new Size(74, 20);
			kryptonLabel7.TabIndex = 18;
			kryptonLabel7.Values.Text = "Resolution :";
			// 
			// kryptonLabel8
			// 
			kryptonLabel8.Location = new Point(9, 75);
			kryptonLabel8.Name = "kryptonLabel8";
			kryptonLabel8.Size = new Size(63, 20);
			kryptonLabel8.TabIndex = 19;
			kryptonLabel8.Values.Text = "Reshade :";
			// 
			// cmb_patchReshade
			// 
			cmb_patchReshade.DropDownStyle = ComboBoxStyle.DropDownList;
			cmb_patchReshade.DropDownWidth = 242;
			cmb_patchReshade.IntegralHeight = false;
			cmb_patchReshade.Items.AddRange(new object[] { "Global Settings", "Yes", "No" });
			cmb_patchReshade.Location = new Point(95, 75);
			cmb_patchReshade.Name = "cmb_patchReshade";
			cmb_patchReshade.Size = new Size(149, 21);
			cmb_patchReshade.StateCommon.ComboBox.Content.TextH = Krypton.Toolkit.PaletteRelativeAlign.Near;
			cmb_patchReshade.TabIndex = 200;
			// 
			// cmb_displayMode
			// 
			cmb_displayMode.DropDownStyle = ComboBoxStyle.DropDownList;
			cmb_displayMode.DropDownWidth = 242;
			cmb_displayMode.IntegralHeight = false;
			cmb_displayMode.Items.AddRange(new object[] { "Global Settings", "Recommanded Settings", "Fullscreen", "Windowed" });
			cmb_displayMode.Location = new Point(96, 22);
			cmb_displayMode.Name = "cmb_displayMode";
			cmb_displayMode.Size = new Size(150, 21);
			cmb_displayMode.StateCommon.ComboBox.Content.TextH = Krypton.Toolkit.PaletteRelativeAlign.Near;
			cmb_displayMode.TabIndex = 201;
			// 
			// cmb_resolution
			// 
			cmb_resolution.DropDownStyle = ComboBoxStyle.DropDownList;
			cmb_resolution.DropDownWidth = 242;
			cmb_resolution.IntegralHeight = false;
			cmb_resolution.Items.AddRange(new object[] { "Global Settings", "720p", "1080p", "1440p (2K)", "2160p (4K)", "Native" });
			cmb_resolution.Location = new Point(95, 49);
			cmb_resolution.Name = "cmb_resolution";
			cmb_resolution.Size = new Size(150, 21);
			cmb_resolution.StateCommon.ComboBox.Content.TextH = Krypton.Toolkit.PaletteRelativeAlign.Near;
			cmb_resolution.TabIndex = 202;
			cmb_resolution.SelectedIndexChanged += cmb_resolution_SelectedIndexChanged;
			// 
			// groupBox2
			// 
			groupBox2.Controls.Add(cmb_patchlink);
			groupBox2.Controls.Add(kryptonLabel9);
			groupBox2.Controls.Add(cmb_displayMode);
			groupBox2.Controls.Add(kryptonLabel4);
			groupBox2.Controls.Add(cmb_resolution);
			groupBox2.Controls.Add(cmb_patchReshade);
			groupBox2.Controls.Add(kryptonLabel8);
			groupBox2.Controls.Add(kryptonLabel7);
			groupBox2.Location = new Point(733, 608);
			groupBox2.Name = "groupBox2";
			groupBox2.Size = new Size(254, 132);
			groupBox2.TabIndex = 205;
			groupBox2.TabStop = false;
			groupBox2.Text = "Quick Options (Not saved)";
			// 
			// cmb_patchlink
			// 
			cmb_patchlink.DropDownStyle = ComboBoxStyle.DropDownList;
			cmb_patchlink.DropDownWidth = 242;
			cmb_patchlink.IntegralHeight = false;
			cmb_patchlink.Items.AddRange(new object[] { "Global Settings", "Don't link anything" });
			cmb_patchlink.Location = new Point(96, 102);
			cmb_patchlink.Name = "cmb_patchlink";
			cmb_patchlink.Size = new Size(148, 21);
			cmb_patchlink.StateCommon.ComboBox.Content.TextH = Krypton.Toolkit.PaletteRelativeAlign.Near;
			cmb_patchlink.TabIndex = 204;
			cmb_patchlink.SelectedIndexChanged += kryptonComboBox1_SelectedIndexChanged;
			// 
			// kryptonLabel9
			// 
			kryptonLabel9.Location = new Point(9, 102);
			kryptonLabel9.Name = "kryptonLabel9";
			kryptonLabel9.Size = new Size(63, 20);
			kryptonLabel9.TabIndex = 203;
			kryptonLabel9.Values.Text = "Link files :";
			// 
			// btn_playgamedirect2
			// 
			btn_playgamedirect2.Enabled = false;
			btn_playgamedirect2.Location = new Point(511, 534);
			btn_playgamedirect2.Name = "btn_playgamedirect2";
			btn_playgamedirect2.Size = new Size(235, 29);
			btn_playgamedirect2.TabIndex = 206;
			btn_playgamedirect2.Values.Text = "Play without controller bind and options";
			btn_playgamedirect2.Click += btn_playgamedirect2_Click;
			// 
			// kryptonButton1
			// 
			kryptonButton1.Location = new Point(302, 6);
			kryptonButton1.Name = "kryptonButton1";
			kryptonButton1.Size = new Size(192, 29);
			kryptonButton1.TabIndex = 207;
			kryptonButton1.Values.Text = "Setup Wizard";
			kryptonButton1.Click += kryptonButton1_Click;
			// 
			// panel_ffb
			// 
			panel_ffb.BorderStyle = BorderStyle.FixedSingle;
			panel_ffb.Controls.Add(label2);
			panel_ffb.Controls.Add(lbl_ffb);
			panel_ffb.Location = new Point(343, 416);
			panel_ffb.Name = "panel_ffb";
			panel_ffb.Size = new Size(67, 18);
			panel_ffb.TabIndex = 218;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Dock = DockStyle.Left;
			label2.Font = new Font("Calibri", 8.1F, FontStyle.Bold, GraphicsUnit.Point);
			label2.ForeColor = Color.MidnightBlue;
			label2.Location = new Point(0, 0);
			label2.Name = "label2";
			label2.Size = new Size(28, 13);
			label2.TabIndex = 23;
			label2.Text = "FFB :";
			label2.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// lbl_ffb
			// 
			lbl_ffb.AutoSize = true;
			lbl_ffb.Dock = DockStyle.Right;
			lbl_ffb.Font = new Font("Calibri", 8.1F, FontStyle.Bold, GraphicsUnit.Point);
			lbl_ffb.ForeColor = Color.Red;
			lbl_ffb.Location = new Point(41, 0);
			lbl_ffb.Name = "lbl_ffb";
			lbl_ffb.Size = new Size(24, 13);
			lbl_ffb.TabIndex = 22;
			lbl_ffb.Text = "OFF";
			lbl_ffb.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// panel_crt
			// 
			panel_crt.BorderStyle = BorderStyle.FixedSingle;
			panel_crt.Controls.Add(label1);
			panel_crt.Controls.Add(lbl_crt);
			panel_crt.Location = new Point(416, 416);
			panel_crt.Name = "panel_crt";
			panel_crt.Size = new Size(114, 18);
			panel_crt.TabIndex = 219;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Dock = DockStyle.Left;
			label1.Font = new Font("Calibri", 8.1F, FontStyle.Bold, GraphicsUnit.Point);
			label1.ForeColor = Color.MidnightBlue;
			label1.Location = new Point(0, 0);
			label1.Name = "label1";
			label1.Size = new Size(62, 13);
			label1.TabIndex = 23;
			label1.Text = "CRT EFFECT :";
			label1.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// lbl_crt
			// 
			lbl_crt.AutoSize = true;
			lbl_crt.Dock = DockStyle.Right;
			lbl_crt.Font = new Font("Calibri", 8.1F, FontStyle.Bold, GraphicsUnit.Point);
			lbl_crt.ForeColor = Color.Red;
			lbl_crt.Location = new Point(88, 0);
			lbl_crt.Name = "lbl_crt";
			lbl_crt.Size = new Size(24, 13);
			lbl_crt.TabIndex = 22;
			lbl_crt.Text = "OFF";
			lbl_crt.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// panel_bezel
			// 
			panel_bezel.BorderStyle = BorderStyle.FixedSingle;
			panel_bezel.Controls.Add(label3);
			panel_bezel.Controls.Add(lbl_bezel);
			panel_bezel.Location = new Point(536, 416);
			panel_bezel.Name = "panel_bezel";
			panel_bezel.Size = new Size(75, 18);
			panel_bezel.TabIndex = 220;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Dock = DockStyle.Left;
			label3.Font = new Font("Calibri", 8.1F, FontStyle.Bold, GraphicsUnit.Point);
			label3.ForeColor = Color.MidnightBlue;
			label3.Location = new Point(0, 0);
			label3.Name = "label3";
			label3.Size = new Size(38, 13);
			label3.TabIndex = 23;
			label3.Text = "BEZEL :";
			label3.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// lbl_bezel
			// 
			lbl_bezel.AutoSize = true;
			lbl_bezel.Dock = DockStyle.Right;
			lbl_bezel.Font = new Font("Calibri", 8.1F, FontStyle.Bold, GraphicsUnit.Point);
			lbl_bezel.ForeColor = Color.Red;
			lbl_bezel.Location = new Point(49, 0);
			lbl_bezel.Name = "lbl_bezel";
			lbl_bezel.Size = new Size(24, 13);
			lbl_bezel.TabIndex = 22;
			lbl_bezel.Text = "OFF";
			lbl_bezel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// panel_vsync
			// 
			panel_vsync.BorderStyle = BorderStyle.FixedSingle;
			panel_vsync.Controls.Add(label6);
			panel_vsync.Controls.Add(lbl_vsync);
			panel_vsync.Location = new Point(617, 416);
			panel_vsync.Name = "panel_vsync";
			panel_vsync.Size = new Size(80, 18);
			panel_vsync.TabIndex = 221;
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Dock = DockStyle.Left;
			label6.Font = new Font("Calibri", 8.1F, FontStyle.Bold, GraphicsUnit.Point);
			label6.ForeColor = Color.MidnightBlue;
			label6.Location = new Point(0, 0);
			label6.Name = "label6";
			label6.Size = new Size(43, 13);
			label6.TabIndex = 23;
			label6.Text = "VSYNC :";
			label6.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// lbl_vsync
			// 
			lbl_vsync.AutoSize = true;
			lbl_vsync.Dock = DockStyle.Right;
			lbl_vsync.Font = new Font("Calibri", 8.1F, FontStyle.Bold, GraphicsUnit.Point);
			lbl_vsync.ForeColor = Color.Red;
			lbl_vsync.Location = new Point(54, 0);
			lbl_vsync.Name = "lbl_vsync";
			lbl_vsync.Size = new Size(24, 13);
			lbl_vsync.TabIndex = 22;
			lbl_vsync.Text = "OFF";
			lbl_vsync.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// panel_aspectratio
			// 
			panel_aspectratio.BorderStyle = BorderStyle.FixedSingle;
			panel_aspectratio.Controls.Add(label8);
			panel_aspectratio.Controls.Add(lbl_aspectratio);
			panel_aspectratio.Location = new Point(703, 416);
			panel_aspectratio.Name = "panel_aspectratio";
			panel_aspectratio.Size = new Size(150, 18);
			panel_aspectratio.TabIndex = 222;
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Dock = DockStyle.Left;
			label8.Font = new Font("Calibri", 8.1F, FontStyle.Bold, GraphicsUnit.Point);
			label8.ForeColor = Color.MidnightBlue;
			label8.Location = new Point(0, 0);
			label8.Name = "label8";
			label8.Size = new Size(99, 13);
			label8.TabIndex = 23;
			label8.Text = "KEEP ASPECT RATIO :";
			label8.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// lbl_aspectratio
			// 
			lbl_aspectratio.AutoSize = true;
			lbl_aspectratio.Dock = DockStyle.Right;
			lbl_aspectratio.Font = new Font("Calibri", 8.1F, FontStyle.Bold, GraphicsUnit.Point);
			lbl_aspectratio.ForeColor = Color.Red;
			lbl_aspectratio.Location = new Point(124, 0);
			lbl_aspectratio.Name = "lbl_aspectratio";
			lbl_aspectratio.Size = new Size(24, 13);
			lbl_aspectratio.TabIndex = 22;
			lbl_aspectratio.Text = "OFF";
			lbl_aspectratio.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// flowLayoutPanel1
			// 
			flowLayoutPanel1.AutoSize = true;
			flowLayoutPanel1.BorderStyle = BorderStyle.FixedSingle;
			flowLayoutPanel1.Controls.Add(label10);
			flowLayoutPanel1.Controls.Add(lbl_translation);
			flowLayoutPanel1.Location = new Point(718, 448);
			flowLayoutPanel1.Name = "flowLayoutPanel1";
			flowLayoutPanel1.Padding = new Padding(0, 2, 0, 0);
			flowLayoutPanel1.Size = new Size(127, 18);
			flowLayoutPanel1.TabIndex = 223;
			// 
			// label10
			// 
			label10.AutoSize = true;
			label10.Font = new Font("Calibri", 8.1F, FontStyle.Bold, GraphicsUnit.Point);
			label10.ForeColor = Color.MidnightBlue;
			label10.Location = new Point(3, 2);
			label10.Name = "label10";
			label10.Size = new Size(75, 13);
			label10.TabIndex = 23;
			label10.Text = "TRANSLATION :";
			label10.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// lbl_translation
			// 
			lbl_translation.AutoSize = true;
			lbl_translation.Font = new Font("Calibri", 8.1F, FontStyle.Bold, GraphicsUnit.Point);
			lbl_translation.ForeColor = Color.Red;
			lbl_translation.Location = new Point(84, 2);
			lbl_translation.Name = "lbl_translation";
			lbl_translation.Size = new Size(33, 13);
			lbl_translation.TabIndex = 22;
			lbl_translation.Text = "NONE";
			lbl_translation.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// Main
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(999, 744);
			Controls.Add(flowLayoutPanel1);
			Controls.Add(panel_ffb);
			Controls.Add(panel_crt);
			Controls.Add(panel_bezel);
			Controls.Add(panel_vsync);
			Controls.Add(panel_aspectratio);
			Controls.Add(kryptonButton1);
			Controls.Add(btn_playgamedirect2);
			Controls.Add(groupBox2);
			Controls.Add(btn_tpsettings);
			Controls.Add(lbl_player4);
			Controls.Add(lbl_player3);
			Controls.Add(btn_playgamedirect);
			Controls.Add(lbl_player2);
			Controls.Add(lbl_player1);
			Controls.Add(flowLayoutPanelThumbs);
			Controls.Add(lbl_GameTitle);
			Controls.Add(pictureBox_gameControls);
			Controls.Add(groupBox1);
			Controls.Add(chk_showAll);
			Controls.Add(btn_gameoptions);
			Controls.Add(btn_playgame);
			Controls.Add(btn_globalconfig);
			Controls.Add(list_games);
			Name = "Main";
			Text = "Main";
			Load += Main_Load;
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)pictureBox_gameControls).EndInit();
			flowLayoutPanelThumbs.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)kryptonPictureBox1).EndInit();
			((System.ComponentModel.ISupportInitialize)cmb_patchReshade).EndInit();
			((System.ComponentModel.ISupportInitialize)cmb_displayMode).EndInit();
			((System.ComponentModel.ISupportInitialize)cmb_resolution).EndInit();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)cmb_patchlink).EndInit();
			panel_ffb.ResumeLayout(false);
			panel_ffb.PerformLayout();
			panel_crt.ResumeLayout(false);
			panel_crt.PerformLayout();
			panel_bezel.ResumeLayout(false);
			panel_bezel.PerformLayout();
			panel_vsync.ResumeLayout(false);
			panel_vsync.PerformLayout();
			panel_aspectratio.ResumeLayout(false);
			panel_aspectratio.PerformLayout();
			flowLayoutPanel1.ResumeLayout(false);
			flowLayoutPanel1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Krypton.Toolkit.KryptonListBox list_games;
		private Krypton.Toolkit.KryptonButton btn_globalconfig;
		private Krypton.Toolkit.KryptonButton btn_playgame;
		private Krypton.Toolkit.KryptonButton btn_playgamedirect;
		private Krypton.Toolkit.KryptonButton btn_gameoptions;
		private Krypton.Toolkit.KryptonCheckBox chk_showAll;
		private GroupBox groupBox1;
		private Krypton.Toolkit.KryptonLabel kryptonLabel1;
		private Krypton.Toolkit.KryptonLabel kryptonLabel3;
		private Krypton.Toolkit.KryptonLabel kryptonLabel2;
		private Krypton.Toolkit.KryptonLabel lbl_wheellist;
		private Krypton.Toolkit.KryptonLabel lbl_arcadelist;
		private Krypton.Toolkit.KryptonLabel lbl_gamepadlist;
		private System.Windows.Forms.Timer timer_controllerUpdate;
		private PictureBox pictureBox_gameControls;
		private Krypton.Toolkit.KryptonLabel lbl_GameTitle;
		private FlowLayoutPanel flowLayoutPanelThumbs;
		private Krypton.Toolkit.KryptonPictureBox kryptonPictureBox1;
		private Krypton.Toolkit.KryptonLabel lbl_player1;
		private Krypton.Toolkit.KryptonLabel lbl_player2;
		private Krypton.Toolkit.KryptonLabel lbl_player3;
		private Krypton.Toolkit.KryptonLabel lbl_player4;
		private Krypton.Toolkit.KryptonLabel lbl_hotaslist;
		private Krypton.Toolkit.KryptonLabel kryptonLabel5;
		private Krypton.Toolkit.KryptonButton btn_tpsettings;
		private Button button1;
		private Button button2;
		private Button button3;
		private Button button4;
		private Krypton.Toolkit.KryptonLabel lbl_gunslist;
		private Krypton.Toolkit.KryptonLabel kryptonLabel6;
		private Button button5;
		private Button button6;
		private Button button7;
		private Button button8;
		private Krypton.Toolkit.KryptonLabel kryptonLabel4;
		private Krypton.Toolkit.KryptonLabel kryptonLabel7;
		private Button button9;
		private Krypton.Toolkit.KryptonLabel kryptonLabel8;
		private Krypton.Toolkit.KryptonComboBox cmb_patchReshade;
		private Krypton.Toolkit.KryptonComboBox cmb_displayMode;
		private Krypton.Toolkit.KryptonComboBox cmb_resolution;
		private GroupBox groupBox2;
		private Krypton.Toolkit.KryptonButton btn_playgamedirect2;
		private Krypton.Toolkit.KryptonComboBox cmb_patchlink;
		private Krypton.Toolkit.KryptonLabel kryptonLabel9;
		private Krypton.Toolkit.KryptonButton kryptonButton1;
		private Panel panel_ffb;
		private Label label2;
		private Label lbl_ffb;
		private Panel panel_crt;
		private Label label1;
		private Label lbl_crt;
		private Panel panel_bezel;
		private Label label3;
		private Label lbl_bezel;
		private Panel panel_vsync;
		private Label label6;
		private Label lbl_vsync;
		private Panel panel_aspectratio;
		private Label label8;
		private Label lbl_aspectratio;
		private FlowLayoutPanel flowLayoutPanel1;
		private Label label10;
		private Label lbl_translation;
	}
}