using CliWrap.EventStream;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using XInput.Wrapper;

namespace TeknoparrotAutoXinput
{
	public partial class Startup : Form
	{

		public static string gameTitle = "";
		public static string logoPath = "";
		public static string xmlFile = "";
		public static string ExecutableGame = "";
		public static string ExecutableDir = "";
		public static string playerAttributionDesc = "";
		public static List<string> imagePaths = new List<string>();
		public static string tpBasePath = "";

		public static List<string> playersDevices = new List<string>();

		private Screen _selectedScreen = null;
		private System.Windows.Forms.Timer checkWindowTimer;

		private System.Windows.Forms.Timer grabLoaderTimer;
		private System.Windows.Forms.Timer updateAeroOverlay;

		private PictureBox pictureBox;
		private Label titleLabel;
		private Label descLabel;

		private string BackgroundImagePath = "";

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

		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		const int GWL_STYLE = -16;
		const uint WS_CAPTION = 0x00C00000;
		const uint WS_THICKFRAME = 0x00040000;
		const int WS_SYSMENU = 0x00080000;

		static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
		const UInt32 SWP_NOSIZE = 0x0001;
		const UInt32 SWP_NOMOVE = 0x0002;
		const UInt32 SWP_NOACTIVATE = 0x0010;
		private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
		private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

		public static int OptionFenetreTP = ConfigurationManager.MainConfig.TPConsoleAction;

		private int originalExStyle;

		private PictureBox pictureBoxCapture;
		private IntPtr externalWindowHandle;
		private PictureBox overlayPictureBox;


		string grabMethod = "";
		bool addBezel = true;

		int formWidth = 0;
		int formHeight = 0;
		int newWidth = 0;
		int newHeight = 0;
		bool taskbarHidden = false;
		bool loaded = false;
		
		ColoredProgressBar progressBar = null;
		OverlayForm overlay = null;
		long startTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();



		public Startup()
		{
			/*
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
			//Temp
			MainScreenDpi = 100;
			//FinTemp
			int SizeWidth = MainScreen.Bounds.Width;
			int SizeHeight = MainScreen.Bounds.Height;
			SizeWidth = (int)Math.Floor((double)SizeWidth * ((double)MainScreenDpi / 100.0));
			SizeHeight = (int)Math.Floor((double)SizeHeight * ((double)MainScreenDpi / 100.0));

			*/


			InitializeComponent();
			var MainScreen = Screen.PrimaryScreen;
			int SizeWidth = MainScreen.Bounds.Width;
			int SizeHeight = MainScreen.Bounds.Height;

			this.FormBorderStyle = FormBorderStyle.None;
			this.Location = new Point(MainScreen.Bounds.Left, MainScreen.Bounds.Top);
			this.Width = SizeWidth;
			this.Height = SizeHeight;

			if (OptionFenetreTP == 0) grabMethod = "grab_invasive";
			if (OptionFenetreTP == 1) grabMethod = "move_on_top";
			if (OptionFenetreTP == 2) grabMethod = "move_on_top_notaskbar";
			if (OptionFenetreTP == 3) grabMethod = "hide";
			if (OptionFenetreTP == 4) grabMethod = "Do Nothing";


			if (grabMethod == "grab_invasive" || grabMethod == "move_on_top" || grabMethod == "move_on_top_notaskbar")
			{
				SetForegroundWindow(this.Handle);
				SetWindowPos2(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0040);
			}

			if (grabMethod == "move_on_top_notaskbar") HideTaskbar();


			Thread.Sleep(30);



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

			string possibleBackgroundPath = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "img", "art", Path.GetFileNameWithoutExtension(xmlFile) + ".jpg");
			if (File.Exists(possibleBackgroundPath))
			{
				BackgroundImagePath = possibleBackgroundPath;
				this.BackgroundImage = CreateTransparentBackgroundImage(Image.FromFile(BackgroundImagePath), this.ClientSize, 0.15f);
				this.BackgroundImageLayout = ImageLayout.Stretch;

			}

			// Appliquer l'image avec une opacité de 0.4

			//this.BackColor = Color.Transparent;

			LoadImages();
			AddImageToForm();
			LoadPlayersInfo();

			AddProgressBar();
			//pictureBoxTransparent.BringToFront();
		}


		public void AddProgressBar()
		{
			int estimedTime = -1;
			string estimedTimeFile = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "cachemd5", Path.GetFileNameWithoutExtension(xmlFile) + ".starttime.txt");
			if (File.Exists(estimedTimeFile))
			{
				estimedTime = int.Parse(File.ReadAllText(estimedTimeFile));
			}
			if(estimedTime > 0)
			{
				// Taille de la fenêtre cible
				int targetWidth = 2560;
				int targetHeight = 1440;

				// Calculer la taille de la barre de progression
				int progressBarWidth = (int)(500 * ((float)this.Width / targetWidth));
				int progressBarHeight = (int)(20 * ((float)this.Height / targetHeight));

				// Calculer la position en bas à droite
				int progressBarX = this.Width - progressBarWidth; // 50 pixels de marge à droite
				int progressBarY = this.Height - progressBarHeight; // 50 pixels de marge en bas

				// Créer une instance de la ProgressBar personnalisée
				progressBar = new ColoredProgressBar(estimedTime)
				{
					Size = new Size(progressBarWidth, progressBarHeight), // Taille de la barre
					Location = new Point(progressBarX, progressBarY), // Position de la barre
					Maximum = 100, // Valeur maximum
					Value = 0 // Valeur initiale (modifiée à 0 pour une animation de remplissage)
				};

				// Ajouter la ProgressBar à la Form
				this.Controls.Add(progressBar);
				this.Controls.SetChildIndex(progressBar, 0); // Assurer que la barre de progression est au-dessus des autres contrôles

			}

		}

		// Méthode pour créer une image avec la transparence
		private Bitmap CreateTransparentBackgroundImage(Image baseImage, Size size, float opacity)
		{
			// Créer un nouveau bitmap de la taille souhaitée
			Bitmap bmp = new Bitmap(size.Width, size.Height);

			using (Graphics g = Graphics.FromImage(bmp))
			{
				// Appliquer une interpolation pour une meilleure qualité
				g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

				// Créer un rectangle de la taille de l'image
				Rectangle rect = new Rectangle(0, 0, size.Width, size.Height);

				// Définir les attributs d'image pour gérer l'opacité
				ColorMatrix colorMatrix = new ColorMatrix
				{
					Matrix33 = opacity // Appliquer l'opacité
				};

				ImageAttributes attributes = new ImageAttributes();
				attributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

				// Dessiner l'image sur le bitmap avec l'opacité
				g.DrawImage(baseImage, rect, 0, 0, baseImage.Width, baseImage.Height, GraphicsUnit.Pixel, attributes);
			}

			return bmp;
		}


		private void LoadPlayersInfo()
		{
			if (playersDevices.Count() == 1)
			{
				string device = playersDevices.ElementAt(0);
				Image deviceLogo = null;
				if (device == "gamepad") deviceLogo = Properties.Resources.gamepad_p1;
				if (device == "arcade") deviceLogo = Properties.Resources.arcade_p1;
				if (device == "wheel") deviceLogo = Properties.Resources.wheel_p1;
				if (device == "lightgun") deviceLogo = Properties.Resources.lightgun_p1;
				if (device == "hotas") deviceLogo = Properties.Resources.hotas_p1;
				if (deviceLogo != null)
				{
					AddResizedImageFromResources(Properties.Resources.P1, 630, 255 - 60, 150, 150);
					AddResizedImageFromResources(deviceLogo, 790, 275 - 60, 470, 110);
				}
			}
			if (playersDevices.Count() == 2)
			{
				string device = playersDevices.ElementAt(0);
				Image deviceLogo = null;
				if (device == "gamepad") deviceLogo = Properties.Resources.gamepad_p1;
				if (device == "arcade") deviceLogo = Properties.Resources.arcade_p1;
				if (device == "wheel") deviceLogo = Properties.Resources.wheel_p1;
				if (device == "lightgun") deviceLogo = Properties.Resources.lightgun_p1;
				if (device == "hotas") deviceLogo = Properties.Resources.hotas_p1;
				if (deviceLogo != null)
				{
					AddResizedImageFromResources(Properties.Resources.P1, 630, 255 - 60, 150, 150);
					AddResizedImageFromResources(deviceLogo, 790, 275 - 60, 470, 110);
				}
				device = playersDevices.ElementAt(1);
				deviceLogo = null;
				if (device == "gamepad") deviceLogo = Properties.Resources.gamepad_p2;
				if (device == "arcade") deviceLogo = Properties.Resources.arcade_p2;
				if (device == "wheel") deviceLogo = Properties.Resources.wheel_p2;
				if (device == "lightgun") deviceLogo = Properties.Resources.lightgun_p2;
				if (device == "hotas") deviceLogo = Properties.Resources.hotas_p2;
				if (deviceLogo != null)
				{
					AddResizedImageFromResources(Properties.Resources.P2, 630, 410 - 60, 150, 150);
					AddResizedImageFromResources(deviceLogo, 790, 430 - 60, 470, 110);
				}
			}
			if (playersDevices.Count() == 3)
			{
				string device = playersDevices.ElementAt(0);
				Image deviceLogo = null;
				if (device == "gamepad") deviceLogo = Properties.Resources.gamepad_p1;
				if (device == "arcade") deviceLogo = Properties.Resources.arcade_p1;
				if (device == "wheel") deviceLogo = Properties.Resources.wheel_p1;
				if (device == "lightgun") deviceLogo = Properties.Resources.lightgun_p1;
				if (device == "hotas") deviceLogo = Properties.Resources.hotas_p1;
				if (deviceLogo != null)
				{
					AddResizedImageFromResources(Properties.Resources.P1, 630, 100 + 40, 150, 150);
					AddResizedImageFromResources(deviceLogo, 790, 120 + 40, 470, 110);
				}
				device = playersDevices.ElementAt(1);
				deviceLogo = null;
				if (device == "gamepad") deviceLogo = Properties.Resources.gamepad_p2;
				if (device == "arcade") deviceLogo = Properties.Resources.arcade_p2;
				if (device == "wheel") deviceLogo = Properties.Resources.wheel_p2;
				if (device == "lightgun") deviceLogo = Properties.Resources.lightgun_p2;
				if (device == "hotas") deviceLogo = Properties.Resources.hotas_p2;
				if (deviceLogo != null)
				{
					AddResizedImageFromResources(Properties.Resources.P2, 630, 255 + 40, 150, 150);
					AddResizedImageFromResources(deviceLogo, 790, 275 + 40, 470, 110);
				}
				device = playersDevices.ElementAt(2);
				deviceLogo = null;
				if (device == "gamepad") deviceLogo = Properties.Resources.gamepad_p3;
				if (device == "arcade") deviceLogo = Properties.Resources.arcade_p3;
				if (device == "wheel") deviceLogo = Properties.Resources.wheel_p3;
				if (device == "lightgun") deviceLogo = Properties.Resources.lightgun_p3;
				if (device == "hotas") deviceLogo = Properties.Resources.hotas_p3;
				if (deviceLogo != null)
				{
					AddResizedImageFromResources(Properties.Resources.P3, 630, 410 + 40, 150, 150);
					AddResizedImageFromResources(deviceLogo, 790, 430 + 40, 470, 110);
				}
			}
			if (playersDevices.Count() == 4)
			{
				string device = playersDevices.ElementAt(0);
				Image deviceLogo = null;
				if (device == "gamepad") deviceLogo = Properties.Resources.gamepad_p1;
				if (device == "arcade") deviceLogo = Properties.Resources.arcade_p1;
				if (device == "wheel") deviceLogo = Properties.Resources.wheel_p1;
				if (device == "lightgun") deviceLogo = Properties.Resources.lightgun_p1;
				if (device == "hotas") deviceLogo = Properties.Resources.hotas_p1;
				if (deviceLogo != null)
				{
					AddResizedImageFromResources(Properties.Resources.P1, 630, 100, 150, 150);
					AddResizedImageFromResources(deviceLogo, 790, 120, 470, 110);
				}
				device = playersDevices.ElementAt(1);
				deviceLogo = null;
				if (device == "gamepad") deviceLogo = Properties.Resources.gamepad_p2;
				if (device == "arcade") deviceLogo = Properties.Resources.arcade_p2;
				if (device == "wheel") deviceLogo = Properties.Resources.wheel_p2;
				if (device == "lightgun") deviceLogo = Properties.Resources.lightgun_p2;
				if (device == "hotas") deviceLogo = Properties.Resources.hotas_p2;
				if (deviceLogo != null)
				{
					AddResizedImageFromResources(Properties.Resources.P2, 630, 255, 150, 150);
					AddResizedImageFromResources(deviceLogo, 790, 275, 470, 110);
				}
				device = playersDevices.ElementAt(2);
				deviceLogo = null;
				if (device == "gamepad") deviceLogo = Properties.Resources.gamepad_p3;
				if (device == "arcade") deviceLogo = Properties.Resources.arcade_p3;
				if (device == "wheel") deviceLogo = Properties.Resources.wheel_p3;
				if (device == "lightgun") deviceLogo = Properties.Resources.lightgun_p3;
				if (device == "hotas") deviceLogo = Properties.Resources.hotas_p3;
				if (deviceLogo != null)
				{
					AddResizedImageFromResources(Properties.Resources.P3, 630, 410, 150, 150);
					AddResizedImageFromResources(deviceLogo, 790, 430, 470, 110);
				}
				device = playersDevices.ElementAt(3);
				deviceLogo = null;
				if (device == "gamepad") deviceLogo = Properties.Resources.gamepad_p4;
				if (device == "arcade") deviceLogo = Properties.Resources.arcade_p4;
				if (device == "wheel") deviceLogo = Properties.Resources.wheel_p4;
				if (device == "lightgun") deviceLogo = Properties.Resources.lightgun_p4;
				if (device == "hotas") deviceLogo = Properties.Resources.hotas_p4;
				if (deviceLogo != null)
				{
					AddResizedImageFromResources(Properties.Resources.P4, 630, 565, 150, 150);
					AddResizedImageFromResources(deviceLogo, 790, 585, 470, 110);
				}
			}
		}


		
		private void CheckWindowTimer_Tick(object sender, EventArgs e)
		{
			/*
			var MainScreen = Screen.PrimaryScreen;
			int SizeWidth = MainScreen.Bounds.Width;
			int SizeHeight = MainScreen.Bounds.Height;
			if(this.formHeight != SizeHeight || this.Width != SizeWidth)
			{
				this.FormBorderStyle = FormBorderStyle.None;
				this.Location = new Point(MainScreen.Bounds.Left, MainScreen.Bounds.Top);
				this.Width = SizeWidth;
				this.Height = SizeHeight;

				if (overlay != null)
				{
					try
					{
						overlay.Close();
					}
					catch { }
				}
				foreach(var Control  in this.Controls)
				{
					if(Control.GetType() == typeof(PictureBox))
					{
						this.Controls.Remove((PictureBox)Control);
					}
					else if (Control.GetType() == typeof(OutlinedLabel))
					{
						this.Controls.Remove((OutlinedLabel)Control);
					}
					else if (Control.GetType() == typeof(Label))
					{
						this.Controls.Remove((Label)Control);
					}
				}
				LoadImages();
				AddImageToForm();
				LoadPlayersInfo();
			}

			*/
			/*


			AddProgressBar();
			*/



			string magpieExecutableGame = ExecutableGame;
			if (Program.GameInfo.ContainsKey("magpieExecutable") && Program.GameInfo["magpieExecutable"].Trim() != "")
			{
				magpieExecutableGame = Path.GetFullPath(Path.Combine(ExecutableDir, Program.GameInfo["magpieExecutable"]));
			}
			string magpieClass = "";
			if (Program.GameInfo.ContainsKey("magpieClass") && Program.GameInfo["magpieClass"].Trim() != "") magpieClass = Program.GameInfo["magpieClass"].Trim();

			string magpieTitle = "";
			if (Program.GameInfo.ContainsKey("magpieTitle") && Program.GameInfo["magpieTitle"].Trim() != "") magpieTitle = Program.GameInfo["magpieTitle"].Trim();

			string trueClassName = "";
			IntPtr windowHandle = Utils.FindWindowByMultipleCriteria(magpieClass, Path.GetFileNameWithoutExtension(magpieExecutableGame), magpieTitle, out trueClassName);
			if (windowHandle == IntPtr.Zero) return;
			
			windowHandle = Utils.FindWindowByMultipleCriteria(magpieClass, Path.GetFileNameWithoutExtension(magpieExecutableGame), magpieTitle, out trueClassName);
			Thread.Sleep(500);
				Utils.LogMessage("Search Window ...");
				if (windowHandle != IntPtr.Zero)
				{
					Utils.RECT clientRectTest;
					Utils.GetClientRect(windowHandle, out clientRectTest);
					int clientWidthTest = clientRectTest.Right - clientRectTest.Left;
					int clientHeightTest = clientRectTest.Bottom - clientRectTest.Top;
					if (clientWidthTest < 150 || clientHeightTest < 100) windowHandle = IntPtr.Zero;
				}

			if (windowHandle == IntPtr.Zero) return;

			// Récupérer l'horodatage après la deuxième action (timestamp de fin)
			long endTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

			// Calculer le temps écoulé en secondes
			long elapsedSeconds = endTimestamp - startTimestamp;

			File.WriteAllText(Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "cachemd5", Path.GetFileNameWithoutExtension(xmlFile) + ".starttime.txt"), elapsedSeconds.ToString());


			Thread.Sleep(3000);
			if (overlay != null)
			{
				try
				{
					overlay.Close();
				}
				catch { }
			}


			checkWindowTimer.Stop();

			/*
			Process[] processes = Process.GetProcessesByName("TeknoParrotUi"); // Remplacez par le nom de l'exécutable de l'application cible
			if (processes.Length > 0)
			{
				Process targetProcess = processes[0];
				if (!targetProcess.HasExited && IsWindowVisible(targetProcess.MainWindowHandle))
				{
					//ShowWindow(targetProcess.MainWindowHandle, SW_MINIMIZE);
					SetWindowPos2(targetProcess.MainWindowHandle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
					//SetWindowPos2(this.Handle, HWND_NOTOPMOST, 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0040);
					checkWindowTimer.Stop();
				}
			}
			*/
		}
		

		public void HideTaskbar()
		{
			IntPtr taskbarHandle = FindWindow("Shell_TrayWnd", null);
			ShowWindow(taskbarHandle, 0);
			taskbarHidden = true;
		}

		public void ShowTaskbar()
		{
			IntPtr taskbarHandle = FindWindow("Shell_TrayWnd", null);
			ShowWindow(taskbarHandle, 5);
		}

		public static bool IsWinVisible(IntPtr hWnd)
		{
			return IsWindowVisible(hWnd);
		}

		public static void MoveBottom(IntPtr hWnd)
		{
			SetWindowPos2(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
		}


		private void GrabLoaderTimer_Tick(object sender, EventArgs e)
		{



			externalWindowHandle = FindWindow(null, Path.Combine(tpBasePath, "OpenParrotWin32", "OpenParrotLoader.exe"));
			if (externalWindowHandle == IntPtr.Zero) externalWindowHandle = FindWindow(null, Path.Combine(tpBasePath, "OpenParrotWin32", "OpenParrotKonamiLoader.exe"));
			if (externalWindowHandle == IntPtr.Zero) externalWindowHandle = FindWindow(null, Path.Combine(tpBasePath, "OpenParrotx64", "OpenParrotLoader64.exe"));
			if (externalWindowHandle == IntPtr.Zero) externalWindowHandle = FindWindow(null, Path.Combine(tpBasePath, "TeknoParrot", "BudgieLoader.exe"));
			if (externalWindowHandle == IntPtr.Zero) externalWindowHandle = FindWindow(null, Path.Combine(tpBasePath, "ElfLdr2", "BudgieLoader.exe"));
			if (externalWindowHandle != IntPtr.Zero)
			{

				formWidth = this.Width;
				formHeight = this.Height;
				newWidth = formWidth / 2;  // Vous pouvez ajuster ce facteur selon vos besoins
				newHeight = formHeight / 2;  // Vous pouvez ajuster ce facteur selon vos besoins
				const short SWP_NOMOVE = 0X2;
				const short SWP_NOSIZE = 1;
				const short SWP_NOZORDER = 0X4;
				const int SWP_SHOWWINDOW = 0x0040;


				if (grabMethod == "grab_invasive")
				{
					SetWindowPos2(this.Handle, HWND_NOTOPMOST, 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0040);
					int offset = (int)(formWidth * 0.005);
					SetWindowPos2(externalWindowHandle, IntPtr.Zero, offset, formHeight - newHeight, newWidth - (offset * 2), newHeight, 0);
					SetParent(externalWindowHandle, this.Handle);
					Rectangle consoleBounds = new Rectangle(0, formHeight - newHeight, newWidth, newHeight);

					if (addBezel)
					{
						// Créer l'overlay avec une image PNG
						overlay = new OverlayForm();
						overlay.UpdatePositionAndSize(consoleBounds);
						overlay.Show();
					}

				}
				if (grabMethod == "move_on_top" || grabMethod == "move_on_top_notaskbar")
				{

					if (addBezel)
					{
						int offset = (int)(formWidth * 0.005);
						SetWindowPos2(externalWindowHandle, IntPtr.Zero, offset, formHeight - newHeight, newWidth - (offset * 2), newHeight, 0);
					}
					else
					{
						SetWindowPos2(externalWindowHandle, IntPtr.Zero, 0, formHeight - newHeight, newWidth, newHeight, 0);
					}
					Rectangle consoleBounds = new Rectangle(0, formHeight - newHeight, newWidth, newHeight);
					Thread.Sleep(10);
					SetWindowPos2(this.Handle, HWND_NOTOPMOST, 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0040);
					Thread.Sleep(10);

					SetWindowPos2(externalWindowHandle, HWND_TOPMOST, 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0040);
					Thread.Sleep(10);
					SetWindowPos2(externalWindowHandle, HWND_NOTOPMOST, 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0040);
					Thread.Sleep(10);


					if (addBezel)
					{
						// Créer l'overlay avec une image PNG
						overlay = new OverlayForm();
						overlay.UpdatePositionAndSize(consoleBounds);
						overlay.Show();

						SetWindowPos2(overlay.Handle, HWND_TOPMOST, 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0040);
						Thread.Sleep(10);
						SetWindowPos2(overlay.Handle, HWND_NOTOPMOST, 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0040);
					}
				}
				if (grabMethod == "hide")
				{
					ShowWindow(externalWindowHandle, SW_MINIMIZE);
					SetWindowPos2(this.Handle, HWND_NOTOPMOST, 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0040);
				}

				//checkWindowTimer.Stop();
				grabLoaderTimer.Stop();
				loaded = true;
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
			if (imagePaths.Count() == 3)
			{
				imageWidth = this.Width / 3;
				imageHeight = this.Height / 3;
			}

			for (int i = 0; i < imagePaths.Count; i++)
			{
				// Créer un nouveau PictureBox pour chaque image
				PictureBox pictureBox = new PictureBox();
				pictureBox.Size = new Size(imageWidth, imageHeight);
				pictureBox.Location = new Point(this.Width - imageWidth, i * imageHeight); // Aligner les images verticalement
				pictureBox.BackColor = Color.Transparent;
				// Charger l'image depuis le fichier
				if (System.IO.File.Exists(imagePaths[i]))
				{
					//pictureBox.Image = Image.FromFile(imagePaths[i]);
					if(BackgroundImagePath != "")
					{
						Color targetColor = Color.FromArgb(54, 56, 55);
						float tolerance = 0.025f; // Tolérance de 10%
						Bitmap transparentImage = MakeColorTransparentWithTolerance(Image.FromFile(imagePaths[i]), targetColor, tolerance);
						pictureBox.Image = transparentImage;
					}
					else pictureBox.Image = Image.FromFile(imagePaths[i]);

					pictureBox.SizeMode = PictureBoxSizeMode.StretchImage; // Ajuster l'image à la taille du PictureBox
					this.Controls.Add(pictureBox);
				}
				else
				{
					MessageBox.Show($"L'image {imagePaths[i]} n'a pas pu être chargée.", "Erreur de chargement d'image", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private Bitmap MakeColorTransparentWithTolerance(Image image, Color targetColor, float tolerance)
		{
			Bitmap bmp = new Bitmap(image);


			Color pixelColorFix = bmp.GetPixel(925,583);
			if(IsColorSimilar(pixelColorFix, Color.FromArgb(47, 45, 46), 0.01f))
			{
				targetColor = pixelColorFix;
			}

			// Parcourir chaque pixel de l'image
			for (int y = 0; y < bmp.Height; y++)
			{
				for (int x = 0; x < bmp.Width; x++)
				{
					Color pixelColor = bmp.GetPixel(x, y);

					// Vérifier si le pixel est proche de la couleur cible
					if (IsColorSimilar(pixelColor, targetColor, tolerance))
					{
						// Rendre le pixel transparent
						bmp.SetPixel(x, y, Color.FromArgb(0, pixelColor.R, pixelColor.G, pixelColor.B));
					}
				}
			}

			return bmp;
		}

		// Méthode pour vérifier si une couleur est similaire à la couleur cible avec une tolérance donnée
		private bool IsColorSimilar(Color pixelColor, Color targetColor, float tolerance)
		{
			// Calculer les différences pour chaque canal (R, G, B)
			int diffR = Math.Abs(pixelColor.R - targetColor.R);
			int diffG = Math.Abs(pixelColor.G - targetColor.G);
			int diffB = Math.Abs(pixelColor.B - targetColor.B);

			// Calculer le pourcentage de différence
			float diffPercentage = (diffR + diffG + diffB) / (3f * 255f);

			// Retourner true si la différence est en dessous de la tolérance
			return diffPercentage <= tolerance;
		}

		private void AddResizedImageFromResources(Image resourceImage, int originalX, int originalY, int originalWidth, int originalHeight)
		{
			// Calculer les ratios de redimensionnement en fonction de la taille actuelle du form
			float widthRatio = (float)this.ClientSize.Width / 2560f;
			float heightRatio = (float)this.ClientSize.Height / 1440f;

			// Calculer la nouvelle position et taille de l'image
			int newX = (int)(originalX * widthRatio);
			int newY = (int)(originalY * heightRatio);
			int newWidth = (int)(originalWidth * widthRatio);
			int newHeight = (int)(originalHeight * heightRatio);

			// Créer et configurer le PictureBox
			PictureBox pictureBox = new PictureBox();
			pictureBox.Image = resourceImage; // Utiliser l'image de la ressource
			pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
			pictureBox.Location = new Point(newX, newY);
			pictureBox.Size = new Size(newWidth, newHeight);
			pictureBox.BackColor = Color.Transparent;

			// Ajouter le PictureBox au Form
			Controls.Add(pictureBox);
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
			pictureBox.BackColor = Color.Transparent;

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
				stringSize = TextRenderer.MeasureText(titleLabel.Text + " ", font);
				fontSize -= 1.0f;
			} while (stringSize.Width > this.Width / 2 || stringSize.Height > this.Height / 2);

			titleLabel.Font = font;
			// Placer le label centré dans le quart supérieur gauche
			//int labelX = SystemInformation.Border3DSize.Width + this.Width - ((int)titleLabel.Width / 4);
			stringSize = TextRenderer.MeasureText(titleLabel.Text, font);
			int posX = ((this.Width / 2) - (int)stringSize.Width) / 2;

			titleLabel.Location = new Point(posX, 0);
			//Controls.Add(titleLabel);
			// Ajouter le label au formulaire
			var titleLabel2 = new OutlinedLabel();
			titleLabel2.Font = titleLabel.Font;
			titleLabel2.ForeColor = titleLabel.ForeColor;
			titleLabel2.BackColor = titleLabel.BackColor;
			titleLabel2.AutoSize = true;
			titleLabel2.Text = titleLabel.Text;
			Controls.Add(titleLabel2);




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
			//Controls.Add(descLabel);


		}

		private void Startup_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (taskbarHidden)
			{
				IntPtr taskbarHandle = FindWindow("Shell_TrayWnd", null);
				ShowWindow(taskbarHandle, 5);
			}


		}

		private void Startup_Resize(object sender, EventArgs e)
		{
			if (loaded)
			{
				this.Close();
			}
		}
	}

	public class OverlayForm : Form
	{
		private Image _overlayImage;
		private Bitmap _resizedImage;

		public OverlayForm()
		{
			// Charger l'image PNG avec transparence
			_overlayImage = Properties.Resources.console_bezel;

			// Appliquer le style pour une fenêtre sans bordure et avec support de la transparence
			this.FormBorderStyle = FormBorderStyle.None;
			//this.TopMost = true;
			this.StartPosition = FormStartPosition.Manual;

			// Configurer la fenêtre comme une Layered Window
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.BackColor = Color.Transparent;
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			ApplyTransparency();
		}

		private void ApplyTransparency()
		{
			if (_resizedImage != null)
			{
				_resizedImage.Dispose(); // Libérer l'ancienne image redimensionnée si elle existe
			}

			// Redimensionner l'image pour correspondre à la taille de la fenêtre
			_resizedImage = new Bitmap(_overlayImage, this.ClientSize);

			IntPtr screenDc = GetDC(IntPtr.Zero);
			IntPtr memDc = CreateCompatibleDC(screenDc);
			IntPtr hBitmap = IntPtr.Zero;
			IntPtr oldBitmap = IntPtr.Zero;

			try
			{
				// Créer un Bitmap à partir de l'image PNG redimensionnée
				hBitmap = _resizedImage.GetHbitmap(Color.FromArgb(0)); // Crée un bitmap compatible avec la transparence
				oldBitmap = SelectObject(memDc, hBitmap);

				// Taille de l'image redimensionnée
				Size size = new Size(_resizedImage.Width, _resizedImage.Height);
				Point pointSource = new Point(0, 0);
				Point topPos = new Point(this.Left, this.Top);

				BLENDFUNCTION blend = new BLENDFUNCTION
				{
					BlendOp = AC_SRC_OVER,
					BlendFlags = 0,
					SourceConstantAlpha = 255, // Opacité
					AlphaFormat = AC_SRC_ALPHA // Utiliser la transparence alpha du PNG
				};

				// Appliquer la transparence et l'image à la fenêtre
				UpdateLayeredWindow(this.Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend, ULW_ALPHA);
			}
			finally
			{
				ReleaseDC(IntPtr.Zero, screenDc);
				if (hBitmap != IntPtr.Zero)
				{
					SelectObject(memDc, oldBitmap);
					DeleteObject(hBitmap);
				}
				DeleteDC(memDc);
			}
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x00080000; // WS_EX_LAYERED
				return cp;
			}
		}

		// Interop
		[DllImport("user32.dll", SetLastError = true)]
		static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc,
												ref Point pptSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern bool DeleteDC(IntPtr hdc);

		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern bool DeleteObject(IntPtr hObject);

		private const int ULW_ALPHA = 0x00000002;
		private const byte AC_SRC_OVER = 0x00;
		private const byte AC_SRC_ALPHA = 0x01;

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct BLENDFUNCTION
		{
			public byte BlendOp;
			public byte BlendFlags;
			public byte SourceConstantAlpha;
			public byte AlphaFormat;
		}

		// Méthode pour mettre à jour la position et la taille de l'overlay
		public void UpdatePositionAndSize(Rectangle consoleBounds)
		{
			this.Location = consoleBounds.Location;  // Positionner l'overlay au-dessus de la console
			this.Size = consoleBounds.Size;          // Redimensionner l'overlay pour correspondre
			this.Invalidate();                       // Redessiner l'overlay après redimensionnement
			ApplyTransparency();                     // Appliquer la transparence après le redimensionnement
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_overlayImage?.Dispose();
				_resizedImage?.Dispose();
			}
			base.Dispose(disposing);
		}
	}

public class ColoredProgressBar : ProgressBar
{
    // Déclaration explicite du type de Timer
    private System.Windows.Forms.Timer timer;
    private int totalMilliseconds; // Durée totale en millisecondes
    private int elapsedMilliseconds; // Temps écoulé en millisecondes

    // Constructeur qui prend la durée en secondes
    public ColoredProgressBar(int durationInSeconds)
    {
        totalMilliseconds = durationInSeconds * 1000; // Convertir en millisecondes
        elapsedMilliseconds = 0;

        // Désactiver le style Windows par défaut pour un rendu personnalisé
        this.SetStyle(ControlStyles.UserPaint, true);

        // Initialiser le timer
        timer = new System.Windows.Forms.Timer();
        timer.Interval = 100; // 100 millisecondes
        timer.Tick += Timer_Tick;
        timer.Start();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        elapsedMilliseconds += timer.Interval; // Ajouter l'intervalle écoulé
        if (elapsedMilliseconds > totalMilliseconds)
        {
            elapsedMilliseconds = totalMilliseconds; // Assurez-vous de ne pas dépasser la durée totale
            timer.Stop(); // Arrêter le timer une fois la durée atteinte
        }

        // Mettre à jour la valeur de la barre de progression
        this.Value = (int)((double)elapsedMilliseconds / totalMilliseconds * Maximum);
        this.Invalidate(); // Redessiner le contrôle
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        // Effacer l'arrière-plan
        e.Graphics.Clear(BackColor);

        // Créer un pinceau pour la barre de progression
        using (SolidBrush brush = new SolidBrush(Color.DarkBlue))
        {
            // Calculer la largeur de la barre de progression
            int width = (int)((double)Value / Maximum * Width);
            // Dessiner la barre de progression comme un rectangle plein
            e.Graphics.FillRectangle(brush, 0, 0, width, Height);
        }

        // Dessiner le texte centré
        string text = $"Estimated time left: {(totalMilliseconds - elapsedMilliseconds) / 1000} sec";
        using (Font font = new Font("Arial", 10, FontStyle.Bold))
        {
            SizeF textSize = e.Graphics.MeasureString(text, font);
            PointF textPosition = new PointF((Width - textSize.Width) / 2, (Height - textSize.Height) / 2);
            using (SolidBrush textBrush = new SolidBrush(Color.White))
            {
                e.Graphics.DrawString(text, font, textBrush, textPosition);
            }
        }

        // Dessiner le contour (optionnel)
        using (Pen pen = new Pen(Color.Black))
        {
            e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        }
    }
}

	public class OutlinedLabel : Label
	{
		protected override void OnPaint(PaintEventArgs e)
		{

			// Utiliser l'anticrénelage
			e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

			// Définir la couleur du texte
			Color textColor = ForeColor;
			Color outlineColor = Color.Gray;
			Font font = this.Font;

			int tailleContour = 2;
			// Dessiner le contour
			using (SolidBrush outlineBrush = new SolidBrush(outlineColor))
			{
				e.Graphics.DrawString(Text, font, outlineBrush, tailleContour, tailleContour); // Légère ombre
				e.Graphics.DrawString(Text, font, outlineBrush, tailleContour, tailleContour*-1);
				e.Graphics.DrawString(Text, font, outlineBrush, tailleContour*-1, tailleContour);
				e.Graphics.DrawString(Text, font, outlineBrush, tailleContour*-1, tailleContour*-1);
			}

			// Dessiner le texte principal
			using (SolidBrush textBrush = new SolidBrush(textColor))
			{
				e.Graphics.DrawString(Text, font, textBrush, 0, 0);
			}
		}

	}


}
