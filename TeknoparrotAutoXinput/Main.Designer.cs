﻿namespace TeknoparrotAutoXinput
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
			lbl_hotaslist = new Krypton.Toolkit.KryptonLabel();
			kryptonLabel5 = new Krypton.Toolkit.KryptonLabel();
			lbl_wheellist = new Krypton.Toolkit.KryptonLabel();
			lbl_arcadelist = new Krypton.Toolkit.KryptonLabel();
			lbl_gamepadlist = new Krypton.Toolkit.KryptonLabel();
			kryptonLabel3 = new Krypton.Toolkit.KryptonLabel();
			kryptonLabel2 = new Krypton.Toolkit.KryptonLabel();
			kryptonLabel1 = new Krypton.Toolkit.KryptonLabel();
			button8 = new Button();
			button7 = new Button();
			button6 = new Button();
			button5 = new Button();
			button4 = new Button();
			button3 = new Button();
			button2 = new Button();
			button1 = new Button();
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
			button9 = new Button();
			kryptonLabel8 = new Krypton.Toolkit.KryptonLabel();
			cmb_patchReshade = new Krypton.Toolkit.KryptonComboBox();
			cmb_displayMode = new Krypton.Toolkit.KryptonComboBox();
			cmb_resolution = new Krypton.Toolkit.KryptonComboBox();
			groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)pictureBox_gameControls).BeginInit();
			flowLayoutPanelThumbs.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)kryptonPictureBox1).BeginInit();
			((System.ComponentModel.ISupportInitialize)cmb_patchReshade).BeginInit();
			((System.ComponentModel.ISupportInitialize)cmb_displayMode).BeginInit();
			((System.ComponentModel.ISupportInitialize)cmb_resolution).BeginInit();
			SuspendLayout();
			// 
			// list_games
			// 
			list_games.Enabled = false;
			list_games.Location = new Point(12, 91);
			list_games.Name = "list_games";
			list_games.Size = new Size(482, 613);
			list_games.TabIndex = 0;
			list_games.SelectedIndexChanged += list_games_SelectedIndexChanged;
			// 
			// btn_globalconfig
			// 
			btn_globalconfig.Location = new Point(12, 12);
			btn_globalconfig.Name = "btn_globalconfig";
			btn_globalconfig.Size = new Size(482, 47);
			btn_globalconfig.TabIndex = 1;
			btn_globalconfig.Values.Text = "Edit TeknoparrotAutoXinput Global Configuration";
			btn_globalconfig.Click += btn_globalconfig_Click;
			// 
			// btn_playgame
			// 
			btn_playgame.Enabled = false;
			btn_playgame.Location = new Point(511, 551);
			btn_playgame.Name = "btn_playgame";
			btn_playgame.Size = new Size(476, 47);
			btn_playgame.TabIndex = 2;
			btn_playgame.Values.Text = "Play game with Auto-Set Bindings";
			btn_playgame.Click += btn_playgame_Click;
			// 
			// btn_playgamedirect
			// 
			btn_playgamedirect.Enabled = false;
			btn_playgamedirect.Location = new Point(511, 604);
			btn_playgamedirect.Name = "btn_playgamedirect";
			btn_playgamedirect.Size = new Size(476, 47);
			btn_playgamedirect.TabIndex = 3;
			btn_playgamedirect.Values.Text = "Play with TP Directly";
			btn_playgamedirect.Click += btn_playgamedirect_Click;
			// 
			// btn_gameoptions
			// 
			btn_gameoptions.Enabled = false;
			btn_gameoptions.Location = new Point(511, 657);
			btn_gameoptions.Name = "btn_gameoptions";
			btn_gameoptions.Size = new Size(235, 47);
			btn_gameoptions.TabIndex = 4;
			btn_gameoptions.Values.Text = "Game Options";
			btn_gameoptions.Click += btn_gameoptions_Click;
			// 
			// chk_showAll
			// 
			chk_showAll.Location = new Point(12, 65);
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
			groupBox1.Controls.Add(lbl_hotaslist);
			groupBox1.Controls.Add(kryptonLabel5);
			groupBox1.Controls.Add(lbl_wheellist);
			groupBox1.Controls.Add(lbl_arcadelist);
			groupBox1.Controls.Add(lbl_gamepadlist);
			groupBox1.Controls.Add(kryptonLabel3);
			groupBox1.Controls.Add(kryptonLabel2);
			groupBox1.Controls.Add(kryptonLabel1);
			groupBox1.Location = new Point(12, 710);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new Size(734, 145);
			groupBox1.TabIndex = 7;
			groupBox1.TabStop = false;
			groupBox1.Text = "Controller Status";
			// 
			// lbl_gunslist
			// 
			lbl_gunslist.Location = new Point(129, 119);
			lbl_gunslist.Name = "lbl_gunslist";
			lbl_gunslist.Size = new Size(19, 20);
			lbl_gunslist.TabIndex = 13;
			lbl_gunslist.Values.Text = "...";
			// 
			// kryptonLabel6
			// 
			kryptonLabel6.Location = new Point(6, 119);
			kryptonLabel6.Name = "kryptonLabel6";
			kryptonLabel6.Size = new Size(61, 20);
			kryptonLabel6.TabIndex = 12;
			kryptonLabel6.Values.Text = "Gun List :";
			// 
			// lbl_hotaslist
			// 
			lbl_hotaslist.Location = new Point(129, 97);
			lbl_hotaslist.Name = "lbl_hotaslist";
			lbl_hotaslist.Size = new Size(19, 20);
			lbl_hotaslist.TabIndex = 7;
			lbl_hotaslist.Values.Text = "...";
			// 
			// kryptonLabel5
			// 
			kryptonLabel5.Location = new Point(6, 97);
			kryptonLabel5.Name = "kryptonLabel5";
			kryptonLabel5.Size = new Size(70, 20);
			kryptonLabel5.TabIndex = 6;
			kryptonLabel5.Values.Text = "Hotas List :";
			// 
			// lbl_wheellist
			// 
			lbl_wheellist.Location = new Point(129, 74);
			lbl_wheellist.Name = "lbl_wheellist";
			lbl_wheellist.Size = new Size(19, 20);
			lbl_wheellist.TabIndex = 5;
			lbl_wheellist.Values.Text = "...";
			lbl_wheellist.Click += kryptonLabel6_Click;
			// 
			// lbl_arcadelist
			// 
			lbl_arcadelist.Location = new Point(129, 48);
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
			// kryptonLabel3
			// 
			kryptonLabel3.Location = new Point(6, 74);
			kryptonLabel3.Name = "kryptonLabel3";
			kryptonLabel3.Size = new Size(73, 20);
			kryptonLabel3.TabIndex = 2;
			kryptonLabel3.Values.Text = "Wheel List :";
			// 
			// kryptonLabel2
			// 
			kryptonLabel2.Location = new Point(6, 48);
			kryptonLabel2.Name = "kryptonLabel2";
			kryptonLabel2.Size = new Size(105, 20);
			kryptonLabel2.TabIndex = 1;
			kryptonLabel2.Values.Text = "Arcade Stick List :";
			// 
			// kryptonLabel1
			// 
			kryptonLabel1.Location = new Point(6, 22);
			kryptonLabel1.Name = "kryptonLabel1";
			kryptonLabel1.Size = new Size(91, 20);
			kryptonLabel1.TabIndex = 0;
			kryptonLabel1.Values.Text = "GamePad List :";
			// 
			// button8
			// 
			button8.Location = new Point(752, 855);
			button8.Name = "button8";
			button8.Size = new Size(75, 23);
			button8.TabIndex = 17;
			button8.Text = "button8";
			button8.UseVisualStyleBackColor = true;
			button8.Click += button8_Click;
			// 
			// button7
			// 
			button7.Location = new Point(752, 826);
			button7.Name = "button7";
			button7.Size = new Size(75, 23);
			button7.TabIndex = 16;
			button7.Text = "button7";
			button7.UseVisualStyleBackColor = true;
			button7.Click += button7_Click;
			// 
			// button6
			// 
			button6.Location = new Point(833, 855);
			button6.Name = "button6";
			button6.Size = new Size(75, 23);
			button6.TabIndex = 15;
			button6.Text = "button6";
			button6.UseVisualStyleBackColor = true;
			button6.Click += button6_Click;
			// 
			// button5
			// 
			button5.Location = new Point(833, 826);
			button5.Name = "button5";
			button5.Size = new Size(75, 23);
			button5.TabIndex = 14;
			button5.Text = "button5";
			button5.UseVisualStyleBackColor = true;
			button5.Click += button5_Click;
			// 
			// button4
			// 
			button4.Location = new Point(833, 798);
			button4.Name = "button4";
			button4.Size = new Size(75, 23);
			button4.TabIndex = 11;
			button4.Text = "button4";
			button4.UseVisualStyleBackColor = true;
			button4.Click += button4_Click;
			// 
			// button3
			// 
			button3.Location = new Point(912, 855);
			button3.Name = "button3";
			button3.Size = new Size(75, 23);
			button3.TabIndex = 10;
			button3.Text = "button3";
			button3.UseVisualStyleBackColor = true;
			button3.Click += button3_Click;
			// 
			// button2
			// 
			button2.Location = new Point(912, 826);
			button2.Name = "button2";
			button2.Size = new Size(75, 23);
			button2.TabIndex = 9;
			button2.Text = "button2";
			button2.UseVisualStyleBackColor = true;
			button2.Click += button2_Click;
			// 
			// button1
			// 
			button1.Location = new Point(912, 798);
			button1.Name = "button1";
			button1.Size = new Size(75, 23);
			button1.TabIndex = 8;
			button1.Text = "button1";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click_1;
			// 
			// timer_controllerUpdate
			// 
			timer_controllerUpdate.Interval = 2000;
			timer_controllerUpdate.Tick += timer_controllerUpdate_Tick;
			// 
			// pictureBox_gameControls
			// 
			pictureBox_gameControls.Location = new Point(521, 91);
			pictureBox_gameControls.Name = "pictureBox_gameControls";
			pictureBox_gameControls.Size = new Size(466, 295);
			pictureBox_gameControls.TabIndex = 8;
			pictureBox_gameControls.TabStop = false;
			pictureBox_gameControls.Click += pictureBox1_Click;
			// 
			// lbl_GameTitle
			// 
			lbl_GameTitle.LabelStyle = Krypton.Toolkit.LabelStyle.TitleControl;
			lbl_GameTitle.Location = new Point(521, 12);
			lbl_GameTitle.Name = "lbl_GameTitle";
			lbl_GameTitle.Size = new Size(53, 29);
			lbl_GameTitle.TabIndex = 9;
			lbl_GameTitle.Values.Text = "Titre";
			// 
			// flowLayoutPanelThumbs
			// 
			flowLayoutPanelThumbs.Controls.Add(kryptonPictureBox1);
			flowLayoutPanelThumbs.Location = new Point(521, 387);
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
			lbl_player1.Location = new Point(524, 453);
			lbl_player1.Name = "lbl_player1";
			lbl_player1.Size = new Size(88, 20);
			lbl_player1.TabIndex = 11;
			lbl_player1.Values.Text = "kryptonLabel4";
			// 
			// lbl_player2
			// 
			lbl_player2.Location = new Point(524, 477);
			lbl_player2.Name = "lbl_player2";
			lbl_player2.Size = new Size(88, 20);
			lbl_player2.TabIndex = 12;
			lbl_player2.Values.Text = "kryptonLabel5";
			// 
			// lbl_player3
			// 
			lbl_player3.Location = new Point(524, 499);
			lbl_player3.Name = "lbl_player3";
			lbl_player3.Size = new Size(88, 20);
			lbl_player3.TabIndex = 13;
			lbl_player3.Values.Text = "kryptonLabel6";
			lbl_player3.Click += kryptonLabel6_Click_1;
			// 
			// lbl_player4
			// 
			lbl_player4.Location = new Point(524, 525);
			lbl_player4.Name = "lbl_player4";
			lbl_player4.Size = new Size(88, 20);
			lbl_player4.TabIndex = 14;
			lbl_player4.Values.Text = "kryptonLabel7";
			// 
			// btn_tpsettings
			// 
			btn_tpsettings.Enabled = false;
			btn_tpsettings.Location = new Point(752, 657);
			btn_tpsettings.Name = "btn_tpsettings";
			btn_tpsettings.Size = new Size(235, 47);
			btn_tpsettings.TabIndex = 15;
			btn_tpsettings.Values.Text = "TP Game Settings";
			btn_tpsettings.Click += btn_tpsettings_Click;
			// 
			// kryptonLabel4
			// 
			kryptonLabel4.Location = new Point(752, 716);
			kryptonLabel4.Name = "kryptonLabel4";
			kryptonLabel4.Size = new Size(91, 20);
			kryptonLabel4.TabIndex = 17;
			kryptonLabel4.Values.Text = "Display Mode :";
			kryptonLabel4.Click += kryptonLabel4_Click;
			// 
			// kryptonLabel7
			// 
			kryptonLabel7.Location = new Point(752, 742);
			kryptonLabel7.Name = "kryptonLabel7";
			kryptonLabel7.Size = new Size(74, 20);
			kryptonLabel7.TabIndex = 18;
			kryptonLabel7.Values.Text = "Resolution :";
			// 
			// button9
			// 
			button9.Location = new Point(752, 798);
			button9.Name = "button9";
			button9.Size = new Size(75, 23);
			button9.TabIndex = 18;
			button9.Text = "button9";
			button9.UseVisualStyleBackColor = true;
			button9.Click += button9_Click;
			// 
			// kryptonLabel8
			// 
			kryptonLabel8.Location = new Point(752, 768);
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
			cmb_patchReshade.Location = new Point(838, 768);
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
			cmb_displayMode.Location = new Point(838, 716);
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
			cmb_resolution.Items.AddRange(new object[] { "Global Settings", "720p", "1080p", "1440p (2K)", "2160p (4K)" });
			cmb_resolution.Location = new Point(838, 741);
			cmb_resolution.Name = "cmb_resolution";
			cmb_resolution.Size = new Size(150, 21);
			cmb_resolution.StateCommon.ComboBox.Content.TextH = Krypton.Toolkit.PaletteRelativeAlign.Near;
			cmb_resolution.TabIndex = 202;
			cmb_resolution.SelectedIndexChanged += cmb_resolution_SelectedIndexChanged;
			// 
			// Main
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(999, 881);
			Controls.Add(button8);
			Controls.Add(cmb_patchReshade);
			Controls.Add(button7);
			Controls.Add(cmb_displayMode);
			Controls.Add(button6);
			Controls.Add(cmb_resolution);
			Controls.Add(button5);
			Controls.Add(kryptonLabel8);
			Controls.Add(button9);
			Controls.Add(kryptonLabel7);
			Controls.Add(button4);
			Controls.Add(kryptonLabel4);
			Controls.Add(button3);
			Controls.Add(button2);
			Controls.Add(button1);
			Controls.Add(btn_tpsettings);
			Controls.Add(lbl_player4);
			Controls.Add(lbl_player3);
			Controls.Add(lbl_player2);
			Controls.Add(lbl_player1);
			Controls.Add(flowLayoutPanelThumbs);
			Controls.Add(lbl_GameTitle);
			Controls.Add(pictureBox_gameControls);
			Controls.Add(groupBox1);
			Controls.Add(chk_showAll);
			Controls.Add(btn_gameoptions);
			Controls.Add(btn_playgamedirect);
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
	}
}