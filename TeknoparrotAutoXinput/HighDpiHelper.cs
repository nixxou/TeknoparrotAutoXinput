using Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeknoparrotAutoXinput
{
	public static class HighDpiHelper
	{
		public static void AdjustControlImagesDpiScale(Control container)
		{
			var dpiScale = GetDpiScale(container).Value;
			if (CloseToOne(dpiScale))
				return;

			AdjustControlImagesDpiScale(container.Controls, dpiScale);
		}

		public static void AdjustFormBGDpiScale(Form container)
		{
			var dpiScale = GetDpiScale(container).Value;
			if (CloseToOne(dpiScale))
				return;

			AdjustFormDpiScale(container, dpiScale);
		}

		private static void AdjustButtonImageDpiScale(ButtonBase button, float dpiScale)
		{
			var image = button.Image;
			if (image == null)
				return;

			button.Image = ScaleImage(image, dpiScale);
		}

		private static void AdjustControlImagesDpiScale(Control.ControlCollection controls, float dpiScale)
		{
			foreach (Control control in controls)
			{
				var button = control as ButtonBase;
				if (button != null)
					AdjustButtonImageDpiScale(button, dpiScale);
				else
				{
					var pictureBox = control as PictureBox;
					if (pictureBox != null)
						AdjustPictureBoxDpiScale(pictureBox, dpiScale);
				}

				AdjustControlImagesDpiScale(control.Controls, dpiScale);
			}
		}

		private static void AdjustPictureBoxDpiScale(PictureBox pictureBox, float dpiScale)
		{
			var image = pictureBox.Image;
			if (image == null)
				return;

			if (pictureBox.SizeMode == PictureBoxSizeMode.CenterImage)
				pictureBox.Image = ScaleImage(pictureBox.Image, dpiScale);
		}


		public static void AdjustFormDpiScale(Form pictureBox, float dpiScale)
		{
			var image = pictureBox.BackgroundImage;
			if (image == null)
				return;

			pictureBox.BackgroundImage = ScaleImage(pictureBox.BackgroundImage, dpiScale);
		}

		private static bool CloseToOne(float dpiScale)
		{
			return Math.Abs(dpiScale - 1) < 0.001;
		}

		public static Lazy<float> GetDpiScale(Control control)
		{
			return new Lazy<float>(() =>
			{
				using (var graphics = control.CreateGraphics())
					return graphics.DpiX / 96.0f;
			});
		}

		private static Image ScaleImage(Image image, float dpiScale)
		{
			var newSize = ScaleSize(image.Size, dpiScale);
			var newBitmap = new Bitmap(newSize.Width, newSize.Height);

			using (var g = Graphics.FromImage(newBitmap))
			{
				// According to this blog post http://blogs.msdn.com/b/visualstudio/archive/2014/03/19/improving-high-dpi-support-for-visual-studio-2013.aspx
				// NearestNeighbor is more adapted for 200% and 200%+ DPI

				var interpolationMode = InterpolationMode.HighQualityBicubic;
				if (dpiScale >= 2.0f)
					interpolationMode = InterpolationMode.NearestNeighbor;

				g.InterpolationMode = interpolationMode;
				g.DrawImage(image, new Rectangle(new Point(), newSize));
			}

			return newBitmap;
		}

		private static Size ScaleSize(Size size, float scale)
		{
			return new Size((int)(size.Width * scale), (int)(size.Height * scale));
		}
	}


	public class CustomPalette : PaletteOffice2010Blue
	{

		private SizeF _dpi;
		private SizeF _scaleFactor;

		public SizeF Dpi
		{
			get
			{
				if (_dpi.IsEmpty)
				{
					using (var g = Graphics.FromHwnd(IntPtr.Zero))
					{
						_dpi = new SizeF(g.DpiX, g.DpiY);
					}
				}
				return _dpi;
			}
		}

		public SizeF ScaleFactor
		{
			get
			{
				if (_scaleFactor.IsEmpty)
				{
					_scaleFactor = new SizeF(Dpi.Width / 96.0F, Dpi.Height / 96.0F);
				}
				return _scaleFactor;
			}
		}

		public override Image GetButtonSpecImage(PaletteButtonSpecStyle style, PaletteState state)
		{
			return GetScaledImage(base.GetButtonSpecImage(style, state));
		}

		public override Image GetCheckBoxImage(bool enabled, System.Windows.Forms.CheckState checkState, bool tracking, bool pressed)
		{
			return GetScaledImage(base.GetCheckBoxImage(enabled, checkState, tracking, pressed));
		}

		public override Image GetContentShortTextImage(PaletteContentStyle style, PaletteState state)
		{
			return GetScaledImage(base.GetContentShortTextImage(style, state));
		}

		public override Image GetContentLongTextImage(PaletteContentStyle style, PaletteState state)
		{
			return GetScaledImage(base.GetContentLongTextImage(style, state));
		}

		public override Image GetContextMenuCheckedImage()
		{
			return GetScaledImage(base.GetContextMenuCheckedImage());
		}

		public override Image GetContextMenuIndeterminateImage()
		{
			return GetScaledImage(base.GetContextMenuIndeterminateImage());
		}

		public override Image GetContextMenuSubMenuImage()
		{
			return GetScaledImage(base.GetContextMenuSubMenuImage());
		}

		public override Image GetDropDownButtonImage(PaletteState state)
		{
			return GetScaledImage(base.GetDropDownButtonImage(state));
		}

		public override Image GetGalleryButtonImage(PaletteRibbonGalleryButton button, PaletteState state)
		{
			return GetScaledImage(base.GetGalleryButtonImage(button, state));
		}

		public override Image GetRadioButtonImage(bool enabled, bool checkState, bool tracking, bool pressed)
		{
			return GetScaledImage(base.GetRadioButtonImage(enabled, checkState, tracking, pressed));
		}

		public override Image GetTreeViewImage(bool expanded)
		{
			return GetScaledImage(base.GetTreeViewImage(expanded));
		}

		private Image GetScaledImage(Image img)
		{
			if ((img == null) || (ScaleFactor.Width == 1 && ScaleFactor.Height == 1))
				return img;

			Bitmap bmp = new Bitmap((int)(img.Width * ScaleFactor.Width), (int)(img.Height * ScaleFactor.Height), System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

			using (var tmpBmp = new Bitmap(img))
			{
				tmpBmp.MakeTransparent(Color.Magenta);
				using (var g = Graphics.FromImage(bmp))
				{
					g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
					g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
					g.DrawImage(tmpBmp, 0, 0, bmp.Width, bmp.Height);
				}
			}

			return bmp;
		}

	}

	static class PaletteImageScaler
	{
		// scales the custom KryptonPalette images using the current Dpi
		public static void ScalePalette(System.Windows.Forms.Form frm, KryptonCustomPaletteBase pal)
		{
			SizeF dpi = new SizeF();
			SizeF scaleFactor = new SizeF();

			// Get System Dpi setting. Note this does not handle per monitor Dpi
			// but should be the same Dpi as AutoScaleFont
			using (var g = frm.CreateGraphics())
			{
				dpi.Width = g.DpiX;
				dpi.Height = g.DpiY;
			}

			// set scale factor from current Dpi / the Dpi the images were created for (96)
			scaleFactor.Width = dpi.Width / 96.0F;
			scaleFactor.Height = dpi.Height / 96.0F;

			// if the scale is the same then no further processing needed (we are at 96 dpi).
			if (scaleFactor.Width == 1.0F && scaleFactor.Height == 1.0F)
				return;

			// suspend palette updates
			pal.SuspendUpdates();

			// scale buttonspec images
			var bs = pal.ButtonSpecs;
			bs.PopulateFromBase(); // populate images first so we can scale them
			ScaleButtonSpecImageType(bs.ArrowDown, scaleFactor);
			ScaleButtonSpecImageType(bs.ArrowLeft, scaleFactor);
			ScaleButtonSpecImageType(bs.ArrowRight, scaleFactor);
			ScaleButtonSpecImageType(bs.ArrowUp, scaleFactor);
			ScaleButtonSpecImageType(bs.Close, scaleFactor);
			ScaleButtonSpecImageType(bs.Common, scaleFactor);
			ScaleButtonSpecImageType(bs.Context, scaleFactor);
			ScaleButtonSpecImageType(bs.DropDown, scaleFactor);
			ScaleButtonSpecImageType(bs.FormClose, scaleFactor);
			ScaleButtonSpecImageType(bs.FormMax, scaleFactor);
			ScaleButtonSpecImageType(bs.FormMin, scaleFactor);
			ScaleButtonSpecImageType(bs.FormRestore, scaleFactor);
			ScaleButtonSpecImageType(bs.Generic, scaleFactor);
			ScaleButtonSpecImageType(bs.Next, scaleFactor);
			ScaleButtonSpecImageType(bs.PendantClose, scaleFactor);
			ScaleButtonSpecImageType(bs.PendantMin, scaleFactor);
			ScaleButtonSpecImageType(bs.PendantRestore, scaleFactor);
			ScaleButtonSpecImageType(bs.PinHorizontal, scaleFactor);
			ScaleButtonSpecImageType(bs.PinVertical, scaleFactor);
			ScaleButtonSpecImageType(bs.Previous, scaleFactor);
			ScaleButtonSpecImageType(bs.RibbonExpand, scaleFactor);
			ScaleButtonSpecImageType(bs.RibbonMinimize, scaleFactor);
			ScaleButtonSpecImageType(bs.WorkspaceMaximize, scaleFactor);
			ScaleButtonSpecImageType(bs.WorkspaceRestore, scaleFactor);

			// scale images
			pal.Images.PopulateFromBase(); //populate images first so we can scale them
										   // CheckBox
			var cb = pal.Images.CheckBox;
			cb.CheckedDisabled = GetScaledImage(cb.CheckedDisabled, scaleFactor);
			cb.CheckedNormal = GetScaledImage(cb.CheckedNormal, scaleFactor);
			cb.CheckedPressed = GetScaledImage(cb.CheckedPressed, scaleFactor);
			cb.CheckedTracking = GetScaledImage(cb.CheckedTracking, scaleFactor);
			cb.UncheckedDisabled = GetScaledImage(cb.UncheckedDisabled, scaleFactor);
			cb.UncheckedNormal = GetScaledImage(cb.UncheckedNormal, scaleFactor);
			cb.UncheckedPressed = GetScaledImage(cb.UncheckedPressed, scaleFactor);
			cb.UncheckedTracking = GetScaledImage(cb.UncheckedTracking, scaleFactor);
			// ContextMenu
			var cm = pal.Images.ContextMenu;
			cm.Checked = GetScaledImage(cm.Checked, scaleFactor);
			cm.Indeterminate = GetScaledImage(cm.Indeterminate, scaleFactor);
			cm.SubMenu = GetScaledImage(cm.SubMenu, scaleFactor);
			// DropDownButton
			var ddb = pal.Images.DropDownButton;
			ddb.Disabled = GetScaledImage(ddb.Disabled, scaleFactor);
			ddb.Normal = GetScaledImage(ddb.Normal, scaleFactor);
			ddb.Pressed = GetScaledImage(ddb.Pressed, scaleFactor);
			ddb.Tracking = GetScaledImage(ddb.Tracking, scaleFactor);
			// GalleryButtons
			// I'm not using these so I'm skipping it
			// Radio Buttons
			var rb = pal.Images.RadioButton;
			rb.CheckedDisabled = GetScaledImage(rb.CheckedDisabled, scaleFactor);
			rb.CheckedNormal = GetScaledImage(rb.CheckedNormal, scaleFactor);
			rb.CheckedPressed = GetScaledImage(rb.CheckedPressed, scaleFactor);
			rb.CheckedTracking = GetScaledImage(rb.CheckedTracking, scaleFactor);
			rb.UncheckedDisabled = GetScaledImage(rb.UncheckedDisabled, scaleFactor);
			rb.UncheckedNormal = GetScaledImage(rb.UncheckedNormal, scaleFactor);
			rb.UncheckedPressed = GetScaledImage(rb.UncheckedPressed, scaleFactor);
			rb.UncheckedTracking = GetScaledImage(rb.UncheckedTracking, scaleFactor);

			// resume palette updates
			pal.ResumeUpdates();
		}

		// helper method for scaling KyrptonPaletteButtonSpecTyped
		private static void ScaleButtonSpecImageType(KryptonPaletteButtonSpecTyped bst, SizeF scaleFactor)
		{
			bst.Image = GetScaledImage(bst.Image, scaleFactor);
			var imgState = bst.ImageStates;
			imgState.ImageCheckedNormal = GetScaledImage(imgState.ImageCheckedNormal, scaleFactor);
			imgState.ImageCheckedPressed = GetScaledImage(imgState.ImageCheckedPressed, scaleFactor);
			imgState.ImageCheckedTracking = GetScaledImage(imgState.ImageCheckedTracking, scaleFactor);
			imgState.ImageDisabled = GetScaledImage(imgState.ImageDisabled, scaleFactor);
			imgState.ImageNormal = GetScaledImage(imgState.ImageNormal, scaleFactor);
			imgState.ImagePressed = GetScaledImage(imgState.ImagePressed, scaleFactor);
			imgState.ImageTracking = GetScaledImage(imgState.ImageTracking, scaleFactor);
		}

		// scales an image and also makes magenta transparent
		private static Image GetScaledImage(Image img, SizeF scaleFactor)
		{
			if (img == null)
				return null;
			if (scaleFactor.Width == 1 && scaleFactor.Height == 1)
				return img;

			Bitmap bmp = new Bitmap((int)(img.Width * scaleFactor.Width), (int)(img.Height * scaleFactor.Height), System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

			using (var tmpBmp = new Bitmap(img))
			{
				tmpBmp.MakeTransparent(Color.Magenta);
				using (var g = Graphics.FromImage(bmp))
				{
					g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
					g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
					g.DrawImage(tmpBmp, 0, 0, bmp.Width, bmp.Height);
				}
			}

			return bmp;
		}
	}
}
