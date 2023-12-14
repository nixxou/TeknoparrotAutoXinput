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
			chk_showStartup.Location = new Point(12, 160);
			chk_showStartup.Name = "chk_showStartup";
			chk_showStartup.Size = new Size(231, 20);
			chk_showStartup.TabIndex = 14;
			chk_showStartup.Values.Text = "Show Startup Screen on Game Launch";
			chk_showStartup.CheckedChanged += chk_showStartup_CheckedChanged;
			// 
			// chk_FFB
			// 
			chk_FFB.Location = new Point(12, 186);
			chk_FFB.Name = "chk_FFB";
			chk_FFB.Size = new Size(282, 20);
			chk_FFB.TabIndex = 16;
			chk_FFB.Values.Text = "Update FFBPlugin Device in the game config ini";
			chk_FFB.CheckedChanged += chk_FFB_CheckedChanged;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(800, 450);
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
			Load += Form1_Load;
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
	}
}