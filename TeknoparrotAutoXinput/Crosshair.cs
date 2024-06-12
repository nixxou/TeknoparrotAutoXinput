using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TeknoparrotAutoXinput
{
	public class Crosshair : Form
	{
		private float percentX;
		private float percentY;
		private int crosshairSize = 20; // Taille de la croix
		private Color crosshairColor;   // Couleur de la croix
		private IContainer components;
		private Bitmap CrosshairBitmap;

		public Crosshair(float percentX, float percentY, Color crosshairColor)
		{
			this.percentX = percentX;
			this.percentY = percentY;
			this.crosshairColor = crosshairColor;
			this.InitializeComponent();
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				createParams.ExStyle |= 32; // WS_EX_TRANSPARENT
				return createParams;
			}
		}

		private void DrawCrosshair()
		{
			Screen primaryScreen = Screen.PrimaryScreen;
			int S_width = primaryScreen.Bounds.Width;
			int S_height = primaryScreen.Bounds.Height;
			CrosshairBitmap = new Bitmap(S_width, S_height);

			using (Graphics graphics = Graphics.FromImage(CrosshairBitmap))
			{
				Pen pen = new Pen(crosshairColor, 2f);

				int centerX = (int)(S_width * percentX);
				int centerY = (int)(S_height * percentY);

				// Draw horizontal line
				graphics.DrawLine(pen, centerX - crosshairSize / 2, centerY, centerX + crosshairSize / 2, centerY);
				// Draw vertical line
				graphics.DrawLine(pen, centerX, centerY - crosshairSize / 2, centerX, centerY + crosshairSize / 2);

				pen.Dispose();
			}

			this.BackgroundImage = CrosshairBitmap;
			this.FormBorderStyle = FormBorderStyle.None;
			this.BackColor = Color.LimeGreen;
			this.TransparencyKey = Color.LimeGreen;
			this.TopMost = true;
			this.Location = new Point(0, 0);
			this.Size = new Size(S_width, S_height);
		}

		private void Crosshair_Load(object sender, EventArgs e)
		{
			this.DrawCrosshair();
			this.StartPosition = FormStartPosition.Manual;
			Screen primaryScreen = Screen.PrimaryScreen;
			Rectangle bounds = primaryScreen.Bounds;
			this.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);
		}

		public new void Show()
		{
			base.Show();
		}

		public new void Hide()
		{
			base.Hide();
		}

		public void UpdateCrosshair(float newPercentX, float newPercentY, Color? newColor = null)
		{
			this.percentX = newPercentX;
			this.percentY = newPercentY;
			if (newColor.HasValue)
			{
				this.crosshairColor = newColor.Value;
			}
			this.DrawCrosshair();
			this.Refresh();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
				this.components.Dispose();
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.SuspendLayout();
			this.AutoScaleDimensions = new SizeF(8f, 16f);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.BackgroundImageLayout = ImageLayout.Stretch;
			this.ClientSize = new Size(800, 450);
			this.Name = nameof(Crosshair);
			this.ShowInTaskbar = false;
			this.Text = nameof(Crosshair);
			this.Load += new EventHandler(this.Crosshair_Load);
			this.ResumeLayout(false);
		}
	}
}
