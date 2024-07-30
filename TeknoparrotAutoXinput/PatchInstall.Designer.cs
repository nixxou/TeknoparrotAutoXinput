namespace TeknoparrotAutoXinput
{
	partial class PatchInstall
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
			progress_extract = new Krypton.Toolkit.KryptonProgressBar();
			SuspendLayout();
			// 
			// progress_extract
			// 
			progress_extract.Location = new Point(12, 13);
			progress_extract.Name = "progress_extract";
			progress_extract.Size = new Size(686, 26);
			progress_extract.StateCommon.Back.Color1 = Color.Green;
			progress_extract.StateDisabled.Back.ColorStyle = Krypton.Toolkit.PaletteColorStyle.OneNote;
			progress_extract.StateNormal.Back.ColorStyle = Krypton.Toolkit.PaletteColorStyle.OneNote;
			progress_extract.TabIndex = 0;
			progress_extract.Text = "progress_extract";
			progress_extract.Values.Text = "progress_extract";
			// 
			// PatchInstall
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(710, 51);
			CloseBox = false;
			Controls.Add(progress_extract);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "PatchInstall";
			Text = "PatchInstall";
			Load += PatchInstall_Load;
			Shown += PatchInstall_Shown;
			ResumeLayout(false);
		}

		#endregion

		private Krypton.Toolkit.KryptonProgressBar progress_extract;
	}
}