using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TeknoparrotAutoXinput
{
	public partial class Startup : Form
	{

		public static string gameTitle = "";
		public static string logoPath = "";
		public static string playerAttributionDesc = "";
		public static List<string> imagePaths = new List<string>();
		public static string tpBasePath = "";



		private Screen _selectedScreen = null;
		private System.Windows.Forms.Timer checkWindowTimer;

		private System.Windows.Forms.Timer grabLoaderTimer;

		private PictureBox pictureBox;
		private Label titleLabel;
		private Label descLabel;

		// Déclarations des fonctions Windows API nécessaires
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		private const int SW_MINIMIZE = 6;

		[DllImport("user32.dll", SetLastError = true)]
		static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll")]
		static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);


		[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
		public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

		[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
		static extern bool SetWindowPos2(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		[DllImport("user32.dll", SetLastError = true)]
		static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		const int GWL_STYLE = -16;
		const uint WS_CAPTION = 0x00C00000;
		const uint WS_THICKFRAME = 0x00040000;

		static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
		const UInt32 SWP_NOSIZE = 0x0001;
		const UInt32 SWP_NOMOVE = 0x0002;
		const UInt32 SWP_NOACTIVATE = 0x0010;

		public Startup()
		{

			Screen MainScreen = null;
			int MainScreenIndex = -1;
			int MainScreenDpi = -1;
			Screen[] screens = Screen.AllScreens;
			for (int i = 0; i < screens.Length; i++)
			{
				Screen screen = screens[i];
				string DeviceName = screen.DeviceName.Trim('\\').Trim('.').Trim('\\');
				if (screen.Primary)
				{
					MainScreenIndex = i;
					MainScreenDpi = DPIUtils.GetMonitorDPI(i);
					MainScreen = screen;
				}
			}

			int SizeWidth = MainScreen.Bounds.Width;
			int SizeHeight = MainScreen.Bounds.Height;
			SizeWidth = (int)Math.Floor((double)SizeWidth * ((double)MainScreenDpi / 100.0));
			SizeHeight = (int)Math.Floor((double)SizeHeight * ((double)MainScreenDpi / 100.0));


			InitializeComponent();

			this.FormBorderStyle = FormBorderStyle.None;
			this.Location = new Point(MainScreen.Bounds.Left, MainScreen.Bounds.Top);
			this.Width = SizeWidth;
			this.Height = SizeHeight;

			
			checkWindowTimer = new System.Windows.Forms.Timer();
			checkWindowTimer.Interval = 30; // Intervalle d'attente en millisecondes (1 seconde dans cet exemple)
			checkWindowTimer.Tick += CheckWindowTimer_Tick;
			checkWindowTimer.Enabled = true;
			checkWindowTimer.Start();

			
			grabLoaderTimer = new System.Windows.Forms.Timer();
			grabLoaderTimer.Interval = 30; // Intervalle d'attente en millisecondes (1 seconde dans cet exemple)
			grabLoaderTimer.Tick += GrabLoaderTimer_Tick;
			grabLoaderTimer.Enabled = true;
			grabLoaderTimer.Start();
			
			LoadImages();
			AddImageToForm();
		}

		private void CheckWindowTimer_Tick(object sender, EventArgs e)
		{

			Process[] processes = Process.GetProcessesByName("TeknoParrotUi"); // Remplacez par le nom de l'exécutable de l'application cible
			if (processes.Length > 0)
			{
				Process targetProcess = processes[0];
				if (!targetProcess.HasExited && IsWindowVisible(targetProcess.MainWindowHandle))
				{
					//ShowWindow(targetProcess.MainWindowHandle, SW_MINIMIZE);
					SetWindowPos2(targetProcess.MainWindowHandle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
					checkWindowTimer.Stop();
				}
			}
		}

		private void GrabLoaderTimer_Tick(object sender, EventArgs e)
		{
			IntPtr externalWindowHandle = FindWindow(null, Path.Combine(tpBasePath, "OpenParrotWin32", "OpenParrotLoader.exe"));
			if (externalWindowHandle == IntPtr.Zero) externalWindowHandle = FindWindow(null, Path.Combine(tpBasePath, "OpenParrotWin32", "OpenParrotKonamiLoader.exe"));
			if (externalWindowHandle == IntPtr.Zero) externalWindowHandle = FindWindow(null, Path.Combine(tpBasePath, "OpenParrotx64", "OpenParrotLoader64.exe"));
			if (externalWindowHandle == IntPtr.Zero) externalWindowHandle = FindWindow(null, Path.Combine(tpBasePath, "TeknoParrot", "BudgieLoader.exe"));
			if (externalWindowHandle == IntPtr.Zero) externalWindowHandle = FindWindow(null, Path.Combine(tpBasePath, "ElfLdr2", "BudgieLoader.exe"));
			if (externalWindowHandle != IntPtr.Zero)
			{
				//int style = GetWindowLong(externalWindowHandle, GWL_STYLE);
				//SetWindowLong(externalWindowHandle, GWL_STYLE, style & ~(int)(WS_CAPTION | WS_THICKFRAME));

				// Récupérer la taille du formulaire
				int formWidth = this.Width;
				int formHeight = this.Height;

				// Calculer la nouvelle taille de la fenêtre externe (1/4 du formulaire)
				int newWidth = formWidth / 2;  // Vous pouvez ajuster ce facteur selon vos besoins
				int newHeight = formHeight / 2;  // Vous pouvez ajuster ce facteur selon vos besoins

				const short SWP_NOMOVE = 0X2;
				const short SWP_NOSIZE = 1;
				const short SWP_NOZORDER = 0X4;
				const int SWP_SHOWWINDOW = 0x0040;

				SetWindowPos(externalWindowHandle, 0, 0, 0, 0, 0, 0x0001 | 0x0040);       // Place the window in the top left of the parent window without resizing it
				SetWindowPos(externalWindowHandle, 0, 0, 0, newWidth, newHeight, SWP_NOZORDER | SWP_SHOWWINDOW);
				SetParent(externalWindowHandle, this.Handle);
				SetWindowPos(externalWindowHandle, 0, 0, formHeight - newHeight, newWidth, newHeight, 0x0001 | 0x0040);

				checkWindowTimer.Stop();

			}
		}

		private void LoadImages()
		{

			// Calculer la taille des images (1/4 de la taille du formulaire)
			int imageWidth = this.Width / 4;
			int imageHeight = this.Height / 4;
			if (imagePaths.Count() <= 2)
			{
				imageWidth = this.Width / 2;
				imageHeight = this.Height / 2;
			}

			for (int i = 0; i < imagePaths.Count; i++)
			{
				// Créer un nouveau PictureBox pour chaque image
				PictureBox pictureBox = new PictureBox();
				pictureBox.Size = new Size(imageWidth, imageHeight);
				pictureBox.Location = new Point(this.Width - imageWidth, i * imageHeight); // Aligner les images verticalement

				// Charger l'image depuis le fichier
				if (System.IO.File.Exists(imagePaths[i]))
				{
					pictureBox.Image = Image.FromFile(imagePaths[i]);
					pictureBox.SizeMode = PictureBoxSizeMode.StretchImage; // Ajuster l'image à la taille du PictureBox
					this.Controls.Add(pictureBox);
				}
				else
				{
					MessageBox.Show($"L'image {imagePaths[i]} n'a pas pu être chargée.", "Erreur de chargement d'image", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void AddImageToForm()
		{
			if (string.IsNullOrEmpty(logoPath) || !File.Exists(logoPath)) return;

			pictureBox = new PictureBox();
			pictureBox.Image = Image.FromFile(logoPath);

			// Ajuster la taille de l'image à 1/4 de la taille de la fenêtre
			int imageWidth = this.Width / 4;
			int imageHeight = this.Height / 4;
			pictureBox.Size = new Size(imageWidth, imageHeight);

			// Placer l'image en haut à droite (1/4 de la hauteur)
			//.Location = new Point(this.Width - imageWidth, this.Height / 4);
			pictureBox.Location = new Point(0, this.Height / 8);
			pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

			// Ajouter la PictureBox au formulaire
			Controls.Add(pictureBox);


			// Ajouter le titre centré en haut à gauche
			titleLabel = new Label();
			titleLabel.Text = gameTitle;
			titleLabel.ForeColor = Color.White;
			titleLabel.BackColor = Color.Transparent;
			titleLabel.AutoSize = true;

			Font font = new Font("Arial", 60.0f, FontStyle.Regular);
			float fontSize = 60.0f; // Taille de police initiale
			SizeF stringSize;
			do
			{
				font = new Font(font.FontFamily, fontSize);
				stringSize = TextRenderer.MeasureText(titleLabel.Text, font);
				fontSize -= 1.0f;
			} while (stringSize.Width > this.Width / 2 || stringSize.Height > this.Height / 2);

			titleLabel.Font = font;
			// Placer le label centré dans le quart supérieur gauche
			//int labelX = SystemInformation.Border3DSize.Width + this.Width - ((int)titleLabel.Width / 4);
			stringSize = TextRenderer.MeasureText(titleLabel.Text, font);
			int posX = ((this.Width / 2) - (int)stringSize.Width) / 2;

			titleLabel.Location = new Point(posX, 0);

			// Ajouter le label au formulaire
			Controls.Add(titleLabel);

			// Ajouter le titre centré en haut à gauche
			descLabel = new Label();
			descLabel.Text = playerAttributionDesc;
			descLabel.ForeColor = Color.White;
			descLabel.BackColor = Color.Transparent;
			descLabel.AutoSize = true;

			font = new Font("Arial", 60.0f, FontStyle.Regular);
			fontSize = 60.0f; // Taille de police initiale
			do
			{
				font = new Font(font.FontFamily, fontSize);
				stringSize = TextRenderer.MeasureText(titleLabel.Text, font);
				fontSize -= 1.0f;
			} while (stringSize.Width > this.Width / 4 || stringSize.Height > this.Height / 4);

			descLabel.Font = font;
			// Placer le label centré dans le quart supérieur gauche
			//int labelX = SystemInformation.Border3DSize.Width + this.Width - ((int)titleLabel.Width / 4);
			stringSize = TextRenderer.MeasureText(titleLabel.Text, font);
			posX = (this.Width / 4);

			descLabel.Location = new Point(posX, (this.Height / 4) - ((int)stringSize.Height / 2));

			// Ajouter le label au formulaire
			Controls.Add(descLabel);


		}
	}

}
