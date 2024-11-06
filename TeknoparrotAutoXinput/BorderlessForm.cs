using System.Runtime.InteropServices;

namespace TeknoparrotAutoXinput
{
	public static class WindowResizer
	{
		// Déclarations des constantes pour les styles de fenêtres
		private const int GWL_STYLE = -16;
		private const int WS_BORDER = 0x00800000;
		private const int WS_DLGFRAME = 0x00400000;
		private const int WS_CAPTION = 0x00C00000;
		private const int WS_THICKFRAME = 0x00040000;

		private const uint SWP_NOMOVE = 0x0002;
		private const uint SWP_NOZORDER = 0x0004;
		private const uint SWP_NOACTIVATE = 0x0010;
		private const uint SWP_FRAMECHANGED = 0x0020;

		// P/Invoke pour manipuler les fenêtres
		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("user32.dll")]
		private static extern IntPtr GetDesktopWindow();

		[StructLayout(LayoutKind.Sequential)]
		private struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		// Méthode pour redimensionner la fenêtre en fonction de la taille client souhaitée
		public static void ResizeWindowToClientRect(IntPtr hWnd, int clientWidth, int clientHeight, bool borderless = false)
		{

			if (borderless)
			{
				Thread.Sleep(100);
				IntPtr style = GetWindowLong(hWnd, GWL_STYLE);
				SetWindowLong(hWnd, GWL_STYLE, (IntPtr)(style.ToInt64() & ~WS_CAPTION & ~WS_THICKFRAME));
				SetWindowPos(hWnd, IntPtr.Zero, 0, 0, clientWidth, clientHeight, SWP_NOZORDER | SWP_NOACTIVATE | SWP_FRAMECHANGED);
				return;
			}

			// Obtenir la taille de la fenêtre et le style
			if (!GetWindowRect(hWnd, out RECT windowRect) ||
				!GetClientRect(hWnd, out RECT clientRect))
			{
				throw new Exception("Impossible de récupérer les dimensions de la fenêtre ou de la zone client.");
			}

			// Calculer les dimensions de la fenêtre pour obtenir les dimensions client souhaitées
			int borderWidth = (windowRect.Right - windowRect.Left) - (clientRect.Right - clientRect.Left);
			int borderHeight = (windowRect.Bottom - windowRect.Top) - (clientRect.Bottom - clientRect.Top);

			// Appliquer la nouvelle taille
			int newWidth = clientWidth + borderWidth;
			int newHeight = clientHeight + borderHeight;

			// Définir la nouvelle taille de la fenêtre
			SetWindowPos(hWnd, IntPtr.Zero, 0, 0, newWidth, newHeight, SWP_NOMOVE | SWP_NOZORDER | SWP_NOACTIVATE | SWP_FRAMECHANGED);



		}
	}
}
