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
			lbl_wheellist = new Krypton.Toolkit.KryptonLabel();
			lbl_arcadelist = new Krypton.Toolkit.KryptonLabel();
			lbl_gamepadlist = new Krypton.Toolkit.KryptonLabel();
			kryptonLabel3 = new Krypton.Toolkit.KryptonLabel();
			kryptonLabel2 = new Krypton.Toolkit.KryptonLabel();
			kryptonLabel1 = new Krypton.Toolkit.KryptonLabel();
			timer_controllerUpdate = new System.Windows.Forms.Timer(components);
			pictureBox_gameControls = new PictureBox();
			lbl_GameTitle = new Krypton.Toolkit.KryptonLabel();
			flowLayoutPanelThumbs = new FlowLayoutPanel();
			kryptonPictureBox1 = new Krypton.Toolkit.KryptonPictureBox();
			lbl_player1 = new Krypton.Toolkit.KryptonLabel();
			lbl_player2 = new Krypton.Toolkit.KryptonLabel();
			lbl_player3 = new Krypton.Toolkit.KryptonLabel();
			lbl_player4 = new Krypton.Toolkit.KryptonLabel();
			groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)pictureBox_gameControls).BeginInit();
			flowLayoutPanelThumbs.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)kryptonPictureBox1).BeginInit();
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
			btn_gameoptions.Size = new Size(476, 47);
			btn_gameoptions.TabIndex = 4;
			btn_gameoptions.Values.Text = "Options";
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
			groupBox1.Controls.Add(lbl_wheellist);
			groupBox1.Controls.Add(lbl_arcadelist);
			groupBox1.Controls.Add(lbl_gamepadlist);
			groupBox1.Controls.Add(kryptonLabel3);
			groupBox1.Controls.Add(kryptonLabel2);
			groupBox1.Controls.Add(kryptonLabel1);
			groupBox1.Location = new Point(12, 710);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new Size(975, 100);
			groupBox1.TabIndex = 7;
			groupBox1.TabStop = false;
			groupBox1.Text = "Controller Status";
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
			// Main
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1011, 822);
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
	}
}