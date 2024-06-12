namespace TeknoparrotAutoXinput
{
	partial class Wizard
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
			tabControl1 = new TabControl();
			tabPage1 = new TabPage();
			kryptonLabel1 = new Krypton.Toolkit.KryptonLabel();
			tabPage2 = new TabPage();
			kryptonLabel2 = new Krypton.Toolkit.KryptonLabel();
			btn_previous = new Krypton.Toolkit.KryptonButton();
			btn_next = new Krypton.Toolkit.KryptonButton();
			tabControl1.SuspendLayout();
			tabPage1.SuspendLayout();
			tabPage2.SuspendLayout();
			SuspendLayout();
			// 
			// tabControl1
			// 
			tabControl1.Controls.Add(tabPage1);
			tabControl1.Controls.Add(tabPage2);
			tabControl1.Location = new Point(12, 12);
			tabControl1.Name = "tabControl1";
			tabControl1.SelectedIndex = 0;
			tabControl1.Size = new Size(858, 539);
			tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			tabPage1.Controls.Add(kryptonLabel1);
			tabPage1.Location = new Point(4, 24);
			tabPage1.Name = "tabPage1";
			tabPage1.Padding = new Padding(3);
			tabPage1.Size = new Size(850, 511);
			tabPage1.TabIndex = 0;
			tabPage1.Text = "tabPage1";
			tabPage1.UseVisualStyleBackColor = true;
			// 
			// kryptonLabel1
			// 
			kryptonLabel1.Location = new Point(290, 60);
			kryptonLabel1.Name = "kryptonLabel1";
			kryptonLabel1.Size = new Size(88, 20);
			kryptonLabel1.TabIndex = 0;
			kryptonLabel1.Values.Text = "kryptonLabel1";
			// 
			// tabPage2
			// 
			tabPage2.Controls.Add(kryptonLabel2);
			tabPage2.Location = new Point(4, 24);
			tabPage2.Name = "tabPage2";
			tabPage2.Padding = new Padding(3);
			tabPage2.Size = new Size(850, 511);
			tabPage2.TabIndex = 1;
			tabPage2.Text = "tabPage2";
			tabPage2.UseVisualStyleBackColor = true;
			// 
			// kryptonLabel2
			// 
			kryptonLabel2.Location = new Point(22, 92);
			kryptonLabel2.Name = "kryptonLabel2";
			kryptonLabel2.Size = new Size(88, 20);
			kryptonLabel2.TabIndex = 0;
			kryptonLabel2.Values.Text = "kryptonLabel2";
			// 
			// btn_previous
			// 
			btn_previous.Location = new Point(651, 557);
			btn_previous.Name = "btn_previous";
			btn_previous.Size = new Size(90, 25);
			btn_previous.TabIndex = 1;
			btn_previous.Values.Text = "kryptonButton1";
			btn_previous.Click += btn_previous_Click;
			// 
			// btn_next
			// 
			btn_next.Location = new Point(767, 557);
			btn_next.Name = "btn_next";
			btn_next.Size = new Size(90, 25);
			btn_next.TabIndex = 2;
			btn_next.Values.Text = "kryptonButton2";
			btn_next.Click += btn_next_Click;
			// 
			// Wizard
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(869, 601);
			Controls.Add(btn_next);
			Controls.Add(btn_previous);
			Controls.Add(tabControl1);
			Name = "Wizard";
			Text = "Wizard";
			Load += Wizard_Load;
			tabControl1.ResumeLayout(false);
			tabPage1.ResumeLayout(false);
			tabPage1.PerformLayout();
			tabPage2.ResumeLayout(false);
			tabPage2.PerformLayout();
			ResumeLayout(false);
		}

		#endregion

		private TabControl tabControl1;
		private TabPage tabPage1;
		private TabPage tabPage2;
		private Krypton.Toolkit.KryptonButton btn_previous;
		private Krypton.Toolkit.KryptonButton btn_next;
		private Krypton.Toolkit.KryptonLabel kryptonLabel1;
		private Krypton.Toolkit.KryptonLabel kryptonLabel2;
	}
}