using Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeknoparrotAutoXinput
{
	public partial class crosshairSelect : KryptonForm
	{
		public string selectedImagePath = string.Empty;
		public crosshairSelect()
		{
			InitializeComponent();
		}

		private void crosshairSelect_Load(object sender, EventArgs e)
		{
			flowLayoutPanelImages.Controls.Clear();

			int maxImageSize = 50;

			var imageFiles = Directory.GetFiles(Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "crosshairs"), "*.png").ToList();

			foreach (var imagePath in imageFiles)
			{
				var pictureBox = new PictureBox
				{
					Image = Image.FromFile(imagePath),
					SizeMode = PictureBoxSizeMode.Zoom,
					Width = maxImageSize,
					Height = maxImageSize,
					Margin = new Padding(5),
					Tag = imagePath
				};

				pictureBox.Click += PictureBox_Click;
				flowLayoutPanelImages.Controls.Add(pictureBox);
			}

			flowLayoutPanelImages.AutoScroll = true;
		}

		private void PictureBox_Click(object? sender, EventArgs e)
		{
			foreach (PictureBox pictureBox in flowLayoutPanelImages.Controls)
			{
				pictureBox.BorderStyle = BorderStyle.None;
			}

			var selectedPictureBox = sender as PictureBox;
			selectedPictureBox.BorderStyle = BorderStyle.Fixed3D;
			selectedImagePath = selectedPictureBox.Tag as string;
		}

		private void flowLayoutPanelImages_Click(object sender, EventArgs e)
		{

		}

		private void flowLayoutPanelImages_Paint(object sender, PaintEventArgs e)
		{

		}

		private void btn_Save_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(selectedImagePath))
			{
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
			else
			{
				this.DialogResult = DialogResult.Cancel;
				this.Close();
			}

		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}
	}
}
