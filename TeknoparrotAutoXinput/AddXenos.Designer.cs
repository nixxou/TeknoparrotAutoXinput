namespace TeknoparrotAutoXinput
{
	partial class AddXenos
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
			kryptonLabel2 = new Krypton.Toolkit.KryptonLabel();
			label1 = new Label();
			label2 = new Label();
			label3 = new Label();
			kryptonButton1 = new Krypton.Toolkit.KryptonButton();
			kryptonButton2 = new Krypton.Toolkit.KryptonButton();
			btn_Save = new Krypton.Toolkit.KryptonButton();
			btn_Cancel = new Krypton.Toolkit.KryptonButton();
			SuspendLayout();
			// 
			// kryptonLabel1
			// 
			kryptonLabel1.Location = new Point(12, 12);
			kryptonLabel1.Name = "kryptonLabel1";
			kryptonLabel1.Size = new Size(215, 20);
			kryptonLabel1.TabIndex = 0;
			kryptonLabel1.Values.Text = "Xenos is a tool used for DLL injection.";
			// 
			// kryptonLabel2
			// 
			kryptonLabel2.Location = new Point(12, 38);
			kryptonLabel2.Name = "kryptonLabel2";
			kryptonLabel2.Size = new Size(764, 20);
			kryptonLabel2.TabIndex = 1;
			kryptonLabel2.Values.Text = "Can be usefull in some minor case, for exemple i use it to load the ffb dll for a game that crash if i just nammed it opengl32.dll or wimm.dll";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(12, 61);
			label1.Name = "label1";
			label1.Size = new Size(490, 15);
			label1.TabIndex = 2;
			label1.Text = "I include an archive with Xenos in my program subfolder, but i do not extract this by default";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(12, 86);
			label2.Name = "label2";
			label2.Size = new Size(341, 15);
			label2.TabIndex = 3;
			label2.Text = "Because due to the nature of the injector, it will trigger antivirus";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new Point(12, 110);
			label3.Name = "label3";
			label3.Size = new Size(164, 15);
			label3.TabIndex = 4;
			label3.Text = "So if you really want to use it :";
			// 
			// kryptonButton1
			// 
			kryptonButton1.Location = new Point(12, 140);
			kryptonButton1.Name = "kryptonButton1";
			kryptonButton1.Size = new Size(764, 64);
			kryptonButton1.TabIndex = 6;
			kryptonButton1.Values.Text = "Step 1 : Add the folder XXX To the Antivirus Exclusion List";
			kryptonButton1.Click += kryptonButton1_Click;
			// 
			// kryptonButton2
			// 
			kryptonButton2.Location = new Point(12, 210);
			kryptonButton2.Name = "kryptonButton2";
			kryptonButton2.Size = new Size(764, 64);
			kryptonButton2.TabIndex = 7;
			kryptonButton2.Values.Text = "Step 2 : Extract the archive";
			kryptonButton2.Click += kryptonButton2_Click;
			// 
			// btn_Save
			// 
			btn_Save.Enabled = false;
			btn_Save.Location = new Point(686, 280);
			btn_Save.Name = "btn_Save";
			btn_Save.Size = new Size(90, 25);
			btn_Save.TabIndex = 54;
			btn_Save.Values.Text = "Save";
			btn_Save.Click += btn_Save_Click;
			// 
			// btn_Cancel
			// 
			btn_Cancel.Location = new Point(590, 280);
			btn_Cancel.Name = "btn_Cancel";
			btn_Cancel.Size = new Size(90, 25);
			btn_Cancel.TabIndex = 53;
			btn_Cancel.Values.Text = "Cancel";
			btn_Cancel.Click += btn_Cancel_Click;
			// 
			// AddXenos
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(785, 320);
			Controls.Add(btn_Save);
			Controls.Add(btn_Cancel);
			Controls.Add(kryptonButton2);
			Controls.Add(kryptonButton1);
			Controls.Add(label3);
			Controls.Add(label2);
			Controls.Add(label1);
			Controls.Add(kryptonLabel2);
			Controls.Add(kryptonLabel1);
			Name = "AddXenos";
			Text = "AddXenos";
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Krypton.Toolkit.KryptonLabel kryptonLabel1;
		private Krypton.Toolkit.KryptonLabel kryptonLabel2;
		private Label label1;
		private Label label2;
		private Label label3;
		private Krypton.Toolkit.KryptonButton kryptonButton1;
		private Krypton.Toolkit.KryptonButton kryptonButton2;
		private Krypton.Toolkit.KryptonButton btn_Save;
		private Krypton.Toolkit.KryptonButton btn_Cancel;
	}
}