
namespace TeknoparrotAutoXinput
{
	partial class DllSelectInjector
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
			kryptonCheckedListBox1 = new Krypton.Toolkit.KryptonCheckedListBox();
			btn_Save = new Krypton.Toolkit.KryptonButton();
			btn_Cancel = new Krypton.Toolkit.KryptonButton();
			SuspendLayout();
			// 
			// kryptonCheckedListBox1
			// 
			kryptonCheckedListBox1.Location = new Point(12, 12);
			kryptonCheckedListBox1.Name = "kryptonCheckedListBox1";
			kryptonCheckedListBox1.Size = new Size(328, 554);
			kryptonCheckedListBox1.TabIndex = 0;
			// 
			// btn_Save
			// 
			btn_Save.Location = new Point(250, 572);
			btn_Save.Name = "btn_Save";
			btn_Save.Size = new Size(90, 25);
			btn_Save.TabIndex = 30;
			btn_Save.Values.Text = "Save";
			btn_Save.Click += btn_Save_Click;
			// 
			// btn_Cancel
			// 
			btn_Cancel.Location = new Point(154, 572);
			btn_Cancel.Name = "btn_Cancel";
			btn_Cancel.Size = new Size(90, 25);
			btn_Cancel.TabIndex = 29;
			btn_Cancel.Values.Text = "Cancel";
			btn_Cancel.Click += btn_Cancel_Click;
			// 
			// DllSelectInjector
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(351, 606);
			Controls.Add(btn_Save);
			Controls.Add(btn_Cancel);
			Controls.Add(kryptonCheckedListBox1);
			Name = "DllSelectInjector";
			Text = "Select Dlls to Inject";
			Load += Form2_Load;
			ResumeLayout(false);
		}

		#endregion

		private Krypton.Toolkit.KryptonCheckedListBox kryptonCheckedListBox1;
		private Krypton.Toolkit.KryptonButton btn_Save;
		private Krypton.Toolkit.KryptonButton btn_Cancel;
	}
}