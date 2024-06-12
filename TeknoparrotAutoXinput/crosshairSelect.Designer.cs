namespace TeknoparrotAutoXinput
{
	partial class crosshairSelect
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
			flowLayoutPanelImages = new FlowLayoutPanel();
			btn_Save = new Krypton.Toolkit.KryptonButton();
			btn_Cancel = new Krypton.Toolkit.KryptonButton();
			SuspendLayout();
			// 
			// flowLayoutPanelImages
			// 
			flowLayoutPanelImages.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			flowLayoutPanelImages.AutoScroll = true;
			flowLayoutPanelImages.Location = new Point(26, 12);
			flowLayoutPanelImages.Name = "flowLayoutPanelImages";
			flowLayoutPanelImages.Size = new Size(567, 438);
			flowLayoutPanelImages.TabIndex = 0;
			flowLayoutPanelImages.Click += flowLayoutPanelImages_Click;
			flowLayoutPanelImages.Paint += flowLayoutPanelImages_Paint;
			// 
			// btn_Save
			// 
			btn_Save.Location = new Point(503, 456);
			btn_Save.Name = "btn_Save";
			btn_Save.Size = new Size(90, 25);
			btn_Save.TabIndex = 75;
			btn_Save.Values.Text = "Save";
			btn_Save.Click += btn_Save_Click;
			// 
			// btn_Cancel
			// 
			btn_Cancel.Location = new Point(407, 456);
			btn_Cancel.Name = "btn_Cancel";
			btn_Cancel.Size = new Size(90, 25);
			btn_Cancel.TabIndex = 74;
			btn_Cancel.Values.Text = "Cancel";
			btn_Cancel.Click += btn_Cancel_Click;
			// 
			// crosshairSelect
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(622, 490);
			Controls.Add(btn_Save);
			Controls.Add(btn_Cancel);
			Controls.Add(flowLayoutPanelImages);
			FormTitleAlign = Krypton.Toolkit.PaletteRelativeAlign.Inherit;
			Name = "crosshairSelect";
			Text = "Double-Click to select crosshair";
			Load += crosshairSelect_Load;
			ResumeLayout(false);
		}

		#endregion

		private FlowLayoutPanel flowLayoutPanelImages;
		private Krypton.Toolkit.KryptonButton btn_Save;
		private Krypton.Toolkit.KryptonButton btn_Cancel;
	}
}