using System.ComponentModel;

namespace TeknoparrotAutoXinput
{
	public class Grid : Form
	{
		private int S_width;
		private int S_height;
		private Bitmap GridBitmap;
		private Color gridColor;
		private int numLines;
		private IContainer components;

		public Grid(Color gridColor, int numLines = 10)
		{
			this.gridColor = Color.FromArgb(128, gridColor); // 50% transparence
			this.numLines = numLines;
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

		private void DrawGrid()
		{
			Screen primaryScreen = Screen.PrimaryScreen;
			this.S_width = primaryScreen.Bounds.Width;
			this.S_height = primaryScreen.Bounds.Height;
			GridBitmap = new Bitmap(S_width, S_height);

			using (Graphics graphics = Graphics.FromImage(this.GridBitmap))
			{
				Pen pen = new Pen(gridColor, 2f);
				float lineHeight = (float)this.S_height / numLines;
				for (int i = 0; i < numLines; i++)
				{
					float y = i * lineHeight;
					graphics.DrawLine(pen, 0, y, this.S_width, y);
				}
				float lineWidth = (float)this.S_width / numLines;
				for (int i = 0; i < numLines; i++)
				{
					float x = i * lineWidth;
					graphics.DrawLine(pen, x, 0, x, this.S_height);
				}
				pen.Dispose();
			}

			this.BackgroundImage = GridBitmap;
			this.FormBorderStyle = FormBorderStyle.None;
			this.BackColor = Color.LimeGreen;
			this.TransparencyKey = Color.LimeGreen;
			this.TopMost = true;
			this.Location = new Point(0, 0);
			this.Size = new Size(S_width, S_height);
		}

		private void Grid_Load(object sender, EventArgs e)
		{
			this.DrawGrid();
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

		public void UpdateGrid(Color? newGridColor = null, int? newNumLines = null)
		{
			if (newGridColor.HasValue)
			{
				this.gridColor = Color.FromArgb(128, newGridColor.Value); // 50% transparence
			}
			if (newNumLines.HasValue)
			{
				this.numLines = newNumLines.Value;
			}
			this.DrawGrid();
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
			this.Name = nameof(Grid);
			this.ShowInTaskbar = false;
			this.Text = nameof(Grid);
			this.Load += new EventHandler(this.Grid_Load);
			this.ResumeLayout(false);
		}
	}
}
