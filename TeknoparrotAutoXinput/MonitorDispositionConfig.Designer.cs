namespace TeknoparrotAutoXinput
{
	partial class MonitorDispositionConfig
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
			cmb_DispositionList = new Krypton.Toolkit.KryptonComboBox();
			btn_cancel = new Krypton.Toolkit.KryptonButton();
			btn_ok = new Krypton.Toolkit.KryptonButton();
			button1 = new Krypton.Toolkit.KryptonButton();
			((System.ComponentModel.ISupportInitialize)cmb_DispositionList).BeginInit();
			SuspendLayout();
			// 
			// cmb_DispositionList
			// 
			cmb_DispositionList.DropDownStyle = ComboBoxStyle.DropDownList;
			cmb_DispositionList.DropDownWidth = 274;
			cmb_DispositionList.IntegralHeight = false;
			cmb_DispositionList.Location = new Point(12, 12);
			cmb_DispositionList.Name = "cmb_DispositionList";
			cmb_DispositionList.Size = new Size(274, 21);
			cmb_DispositionList.StateCommon.ComboBox.Content.TextH = Krypton.Toolkit.PaletteRelativeAlign.Near;
			cmb_DispositionList.TabIndex = 0;
			// 
			// btn_cancel
			// 
			btn_cancel.Location = new Point(292, 12);
			btn_cancel.Name = "btn_cancel";
			btn_cancel.Size = new Size(60, 21);
			btn_cancel.TabIndex = 1;
			btn_cancel.Values.Text = "Cancel";
			btn_cancel.Click += btn_cancel_Click;
			// 
			// btn_ok
			// 
			btn_ok.Location = new Point(358, 12);
			btn_ok.Name = "btn_ok";
			btn_ok.Size = new Size(65, 21);
			btn_ok.TabIndex = 2;
			btn_ok.Values.Text = "Save";
			btn_ok.Click += btn_ok_Click;
			// 
			// button1
			// 
			button1.Location = new Point(12, 39);
			button1.Name = "button1";
			button1.Size = new Size(411, 35);
			button1.TabIndex = 3;
			button1.Values.Text = "Create new Screen Disposition based on your current monitor Setup";
			button1.Click += button1_Click;
			// 
			// MonitorDispositionConfig
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(440, 88);
			Controls.Add(button1);
			Controls.Add(btn_ok);
			Controls.Add(btn_cancel);
			Controls.Add(cmb_DispositionList);
			FormBorderStyle = FormBorderStyle.FixedSingle;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "MonitorDispositionConfig";
			StartPosition = FormStartPosition.CenterParent;
			Text = "Monitor Disposition : Select or Register Monitor Layouts";
			Load += MonitorDispositionConfig_Load;
			((System.ComponentModel.ISupportInitialize)cmb_DispositionList).EndInit();
			ResumeLayout(false);
		}

		#endregion

		private Krypton.Toolkit.KryptonComboBox cmb_DispositionList;
		private Krypton.Toolkit.KryptonButton btn_cancel;
		private Krypton.Toolkit.KryptonButton btn_ok;
		private Krypton.Toolkit.KryptonButton button1;
	}
}