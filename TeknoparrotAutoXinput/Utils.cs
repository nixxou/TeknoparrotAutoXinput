﻿using BsDiff;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using Microsoft.Win32.TaskScheduler;
using Newtonsoft.Json;
using SharpDX.DirectInput;
using SharpDX.Multimedia;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using XJoy;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;


namespace TeknoparrotAutoXinput
{

	public static class Utils
	{
		public static Dictionary<string, string> md5Cache = new Dictionary<string, string>();


		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool GetFinalPathNameByHandle(SafeFileHandle hFile, StringBuilder lpszFilePath, uint cchFilePath, uint dwFlags);


		private const uint FILE_SHARE_READ = 1;
		private const uint OPEN_EXISTING = 3;
		private const uint FILE_FLAG_OPEN_REPARSE_POINT = 0x00200000;
		private const uint FILE_NAME_NORMALIZED = 0x0;
		private const uint FILE_ATTRIBUTE_REPARSE_POINT = 0x0400;


		[StructLayout(LayoutKind.Sequential)]
		public struct BY_HANDLE_FILE_INFORMATION
		{
			public uint FileAttributes;
			public FILETIME CreationTime;
			public FILETIME LastAccessTime;
			public FILETIME LastWriteTime;
			public uint VolumeSerialNumber;
			public uint FileSizeHigh;
			public uint FileSizeLow;
			public uint NumberOfLinks;
			public uint FileIndexHigh;
			public uint FileIndexLow;
		}

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		static extern SafeFileHandle CreateFile(
			string lpFileName,
			[MarshalAs(UnmanagedType.U4)] FileAccess dwDesiredAccess,
			[MarshalAs(UnmanagedType.U4)] FileShare dwShareMode,
			IntPtr lpSecurityAttributes,
			[MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition,
			[MarshalAs(UnmanagedType.U4)] FileAttributes dwFlagsAndAttributes,
			IntPtr hTemplateFile);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool GetFileInformationByHandle(SafeFileHandle handle, out BY_HANDLE_FILE_INFORMATION lpFileInformation);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool CloseHandle(SafeHandle hObject);

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		static extern IntPtr FindFirstFileNameW(
			string lpFileName,
			uint dwFlags,
			ref uint stringLength,
			StringBuilder fileName);

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		static extern bool FindNextFileNameW(
			IntPtr hFindStream,
			ref uint stringLength,
			StringBuilder fileName);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool FindClose(IntPtr fFindHandle);

		[DllImport("kernel32.dll")]
		static extern bool GetVolumePathName(string lpszFileName,
			[Out] StringBuilder lpszVolumePathName, uint cchBufferLength);

		[DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
		static extern bool PathAppend([In, Out] StringBuilder pszPath, string pszMore);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll")]
		public static extern int GetClientRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("user32.dll")]
		public static extern int GetWindowRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);


		[DllImport("user32.dll")]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		static extern bool ClientToScreen(IntPtr hWnd, ref POINTCLICK lpPoint);

		[DllImport("user32.dll")]
		private static extern IntPtr GetDesktopWindow();

		[DllImport("user32.dll")]
		static extern IntPtr GetForegroundWindow();

		// Importation des fonctions de la Windows API
		[DllImport("user32.dll")]
		public static extern bool SetCursorPos(int X, int Y);

		[DllImport("user32.dll")]
		static extern bool SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		static int MakeLParam(int LoWord, int HiWord)
		{
			return ((HiWord << 16) | (LoWord & 0xFFFF));
		}


		[DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsWow64Process([In] IntPtr process, [Out] out bool wow64Process);

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct POINTCLICK
		{
			public int X;
			public int Y;
		}

		// Constantes pour les messages de la souris
		const int WM_RBUTTONDOWN = 0x0204;
		const int WM_RBUTTONUP = 0x0205;
		const int WM_LBUTTONDOWN = 0x0201;
		const int WM_LBUTTONUP = 0x0202;

		// Constantes pour les événements de souris
		private const int MOUSEEVENTF_MOVE = 0x0001;
		private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
		private const int MOUSEEVENTF_LEFTUP = 0x0004;
		private const int MOUSEEVENTF_ABSOLUTE = 0x8000;

		[DllImport("user32.dll")]
		private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		const int GWL_STYLE = -16;
		const int WS_CAPTION = 0x00C00000;
		const int WS_SYSMENU = 0x00080000;

		public static ConcurrentDictionary<string, string> CacheAllText = new ConcurrentDictionary<string, string>();
		public static string ReadAllText(string fileName)
		{
			FileInfo fileInfo = new FileInfo(fileName);

			string fileIdentifier = $"{fileInfo.LastWriteTimeUtc:yyyy-MM-ddTHH:mm:ssZ}{fileInfo.Length}{fileInfo.FullName}";

			if (CacheAllText.ContainsKey(fileIdentifier)) return CacheAllText[fileIdentifier];
			else
			{
				string content = File.ReadAllText(fileName);
				CacheAllText.TryAdd(fileIdentifier, content);
				return content;
			}
		}

		public static int GetFileLinkCount(string filepath)
		{
			int result = 0;
			SafeFileHandle handle = CreateFile(filepath, FileAccess.Read, FileShare.Read, IntPtr.Zero, FileMode.Open, FileAttributes.Archive, IntPtr.Zero);
			BY_HANDLE_FILE_INFORMATION fileInfo = new BY_HANDLE_FILE_INFORMATION();
			if (GetFileInformationByHandle(handle, out fileInfo))
				result = (int)fileInfo.NumberOfLinks;
			CloseHandle(handle);
			return result;
		}


		[DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
		//static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, int dwFlags);
		static extern bool CreateHardLink(
  string lpFileName,
  string lpExistingFileName,
  IntPtr lpSecurityAttributes
);

		[DllImport("shell32.dll", SetLastError = true)]
		static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);
		public static void MakeLink(string source, string target)
		{
			if (!File.Exists(source)) return;
			if (File.Exists(target)) return;
			if (!AreFoldersOnSameDrive(source, target)) return;

			CreateHardLink(target, source, IntPtr.Zero);
		}

		public static bool MakeLinkBool(string source, string target)
		{
			if (!File.Exists(source)) return false;
			if (File.Exists(target)) return false;
			if (!AreFoldersOnSameDrive(source, target)) return false;

			bool result = false;
			try
			{
				result = CreateHardLink(target, source, IntPtr.Zero);
			}
			catch
			{
				return false;
			}
			return result;
		}

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWindowVisible(IntPtr hWnd);


		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		private static extern bool CreateSymbolicLink(string symlinkFileName, string targetFileName, int flags);
		// Constante pour le type de lien symbolique
		private const int SYMBOLIC_LINK_FLAG_FILE = 0x0;
		private const int SYMBOLIC_LINK_FLAG_DIRECTORY = 0x1;

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		static extern uint GetShortPathName(
			[MarshalAs(UnmanagedType.LPTStr)]
	string lpszLongPath,
			[MarshalAs(UnmanagedType.LPTStr)]
	StringBuilder lpszShortPath,
			uint cchBuffer);

		[StructLayout(LayoutKind.Sequential)]
		public struct FILE_ID_INFO
		{
			public ulong VolumeSerialNumber;
			public ulong FileIdLow;
			public ulong FileIdHigh;
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool GetFileInformationByHandleEx(
			IntPtr hFile, int FileInformationClass, out FILE_ID_INFO lpFileInformation, uint dwBufferSize);

		private const int FileIdInfo = 18;

		public static string ShortFilenameFor(string longFilename)
		{
			// Add to the long filename a prefix to cause the API to handle filenames longer than 260 characters.

			const string PREFIX = @"\\?\";
			//longFilename = PREFIX + longFilename;

			// First check how much space is required for the short filename.

			uint length = GetShortPathName(longFilename, null, 0);

			if (length == 0)
				throw new Win32Exception(Marshal.GetLastWin32Error());

			// Now allocate buffer of the correct length and fill it.

			StringBuilder buffer = new StringBuilder((int)length);
			uint result = GetShortPathName(longFilename, buffer, length);

			if (result == 0)
				throw new Win32Exception(Marshal.GetLastWin32Error());

			buffer.Remove(0, PREFIX.Length);
			return buffer.ToString();
		}

		public static bool CreateSoftlink(string sourceFilePath, string targetFilePath)
		{
			// Vérifier si le fichier source existe
			if (!File.Exists(sourceFilePath))
			{
				return false;
			}

			// Créer le lien symbolique
			try
			{
				if (File.Exists(targetFilePath))
				{
					// Supprimer le fichier existant s'il existe déjà
					File.Delete(targetFilePath);
				}

				// Appeler la fonction CreateSymbolicLink pour créer le lien symbolique
				bool success = CreateSymbolicLink(targetFilePath, sourceFilePath, SYMBOLIC_LINK_FLAG_FILE);

				return success;
			}
			catch (Exception)
			{
				// Gérer les erreurs éventuelles lors de la création du lien symbolique
				return false;
			}
		}

		public static bool IsSoftLink(string filePath)
		{
			// Vérifier si le fichier existe
			if (!File.Exists(filePath))
			{
				return false;
			}

			try
			{

				// Obtenir les attributs du fichier
				FileAttributes attributes = File.GetAttributes(filePath);

				// Vérifier si le fichier est un lien symbolique
				return (attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint;
			}
			catch { return false; }
		}

		public static string GetSoftLinkTarget(string linkPath)
		{
			if (!IsSoftLink(linkPath))
				throw new InvalidOperationException("Not a soft link");

			using (SafeFileHandle handle = CreateFile(
				linkPath,
				FileAccess.Read,
				FileShare.Read,
				IntPtr.Zero,
				FileMode.Open,
				(FileAttributes)FILE_ATTRIBUTE_REPARSE_POINT, // Utilisation des attributs appropriés
				IntPtr.Zero))
			{
				if (handle.IsInvalid)
					throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());

				StringBuilder targetPath = new StringBuilder(1024);
				if (GetFinalPathNameByHandle(handle, targetPath, (uint)targetPath.Capacity, FILE_NAME_NORMALIZED))
				{
					return targetPath.ToString().TrimEnd('\0');
				}
				throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
			}
		}
		private static string NormalizePath(string path)
		{
			// Enlever le préfixe \\?\ s'il est présent
			if (path.StartsWith(@"\\?\"))
			{
				path = path.Substring(4);
			}

			// Convertir les barres obliques inverses doubles en simples pour la compatibilité
			return path.Replace(@"\\", @"\");
		}
		public static bool IsSoftLink(string filePath, string directoryPath)
		{
			if (!IsSoftLink(filePath))
				return false;

			try
			{
				string targetPath = GetSoftLinkTarget(filePath);
				// Normaliser les chemins pour les comparer
				string normalizedFilePath = NormalizePath(targetPath);
				string normalizedDirectoryPath = NormalizePath(directoryPath);

				return normalizedFilePath.StartsWith(normalizedDirectoryPath, StringComparison.OrdinalIgnoreCase);
			}
			catch { return false; }

		}
		public static bool IsHardLink(string fileToTestPath, string fromDirPath)
		{
			if (!File.Exists(fileToTestPath)) throw new Exception("file does not exist");
			if (!Directory.Exists(fromDirPath)) throw new Exception("directory does not exist");

			fileToTestPath = Path.GetFullPath(fileToTestPath).ToLower();
			fromDirPath = Path.GetFullPath(fromDirPath).ToLower();

			//Utils.LogMessage($"DEBUG Baggio 3 : fileToTestPath = {fileToTestPath} -> {fromDirPath}");

			var sibling = GetFileSiblingHardLinks(fileToTestPath);
			if (sibling == null) return false;

			foreach (var siblingLink in sibling)
			{
				if (siblingLink == null)
				{
					//Utils.LogMessage("DEBUG Baggio 4: siblingLink is null");
					continue;
				}
				string siblingDir = Path.GetDirectoryName(siblingLink).ToLower();

				if (siblingDir == null)
				{
					//Utils.LogMessage("DEBUG Baggio 5: siblingDir is null");
					continue;
				}


				if (siblingDir.StartsWith(fromDirPath) || siblingDir == fromDirPath) return true;
			}
			return false;
		}

		public static string[] GetFileSiblingHardLinks(string filepath)
		{
			List<string> result = new List<string>();
			uint stringLength = 256;
			StringBuilder sb = new StringBuilder(256);
			GetVolumePathName(filepath, sb, stringLength);
			string volume = sb.ToString();
			sb.Length = 0; stringLength = 256;
			IntPtr findHandle = FindFirstFileNameW(filepath, 0, ref stringLength, sb);
			if ((long)findHandle != -1)
			{
				do
				{
					StringBuilder pathSb = new StringBuilder(volume, 256);
					PathAppend(pathSb, sb.ToString());
					result.Add(pathSb.ToString());
					sb.Length = 0; stringLength = 256;
				} while (FindNextFileNameW(findHandle, ref stringLength, sb));
				FindClose(findHandle);
				return result.ToArray();
			}
			return null;
		}


		public static void HardLinkFiles(string directorySource, string directoryDest, string executableGame = "", bool useSoftLink = false)
		{
			/*
			if (!Directory.Exists(directorySource)) return;
			if (!Directory.Exists(directoryDest)) return;
			directorySource = Path.GetFullPath(directorySource);
			directoryDest = Path.GetFullPath(directoryDest);

			if (!AreFoldersOnSameDrive(directorySource, directoryDest)) return;

			var filePaths = Directory.EnumerateFiles(directorySource, "*", new EnumerationOptions
			{
				IgnoreInaccessible = true,
				RecurseSubdirectories = true
			});

            foreach (var file in filePaths)
            {
				string newfile = directoryDest + file.Remove(0, directorySource.Length);
				string newfiledir = Directory.GetParent(newfile).FullName;
				if (!Directory.Exists(newfiledir))
				{
					Directory.CreateDirectory(newfiledir);
				}
				if(File.Exists(newfile))
				{
					File.Move(newfile, newfile + ".filetorestore");
				}
				MakeLink(file, newfile);
			}
			*/

			bool main_executable_linked = false;
			string moveToDest = "";
			bool moveNeedAdmin = false;
			string moveAhkCode = "";
			List<string> DirectoryList = new List<string>();

			if (!Directory.Exists(directorySource)) return;
			if (!Directory.Exists(directoryDest)) return;
			directorySource = Path.GetFullPath(directorySource);
			directoryDest = Path.GetFullPath(directoryDest);

			if (!useSoftLink && !AreFoldersOnSameDrive(directorySource, directoryDest)) return;

			var filePaths = Directory.EnumerateFiles(directorySource, "*", new EnumerationOptions
			{
				IgnoreInaccessible = true,
				RecurseSubdirectories = true
			});

			//Generate Patch
			bool need_reload = false;
			string patchJsonFile = Path.Combine(directorySource, "[!patch_generation!]", "patchdata.json");
			string patchDir = Path.Combine(directorySource, "[!patch_generation!]");
			if (File.Exists(patchJsonFile))
			{
				Utils.LogMessage("Checking patches");
				bool missing_patch = false;
				string jsonData = Utils.ReadAllText(patchJsonFile);
				List<PatchInfoJsonElement> patchInfoJson = JsonConvert.DeserializeObject<List<PatchInfoJsonElement>>(jsonData);
				foreach (var patch in patchInfoJson)
				{
					string expectedPatchFile = Path.Combine(directorySource, patch.destination);
					if (!File.Exists(expectedPatchFile))
					{
						Utils.LogMessage($"Missing patch : {expectedPatchFile}");
						missing_patch = true;
					}
				}
				if (!missing_patch) Utils.LogMessage("All patch are OK !");


				if (missing_patch && executableGame != "" && File.Exists(executableGame))
				{
					string dirGameExec = Path.GetDirectoryName(executableGame);
					Dictionary<string, (string, bool)> mainExecutableDict = new Dictionary<string, (string, bool)>();
					foreach (var patch in patchInfoJson)
					{
						string expectedPatchFile = Path.Combine(directorySource, patch.destination);
						string originalFile = "";
						string bdfPatchFile = "";
						if (File.Exists(expectedPatchFile)) continue;
						if (patch.source_path == "mainexecutable")
						{

							long executableSize = new FileInfo(executableGame).Length;
							foreach (var sourceData in patch.source_data)
							{
								if (patch.bdf)
								{
									bdfPatchFile = Path.Combine(patchDir, sourceData.patch_name);
									if (!File.Exists(bdfPatchFile)) continue;
								}
								//Start looking at the current game executable
								if (sourceData.source_size == executableSize && sourceData.source_md5 == GetMd5HashAsString(executableGame, true))
								{
									originalFile = executableGame;
								}

								if (originalFile == "") //Uf the game executable dont match
								{
									//Verify all files in the game dir until we found a match

									var potentialFiles = Directory.GetFiles(dirGameExec, "*");
									foreach (var potentialFile in potentialFiles)
									{
										long potentialFileSize = new FileInfo(potentialFile).Length;
										if (sourceData.source_size == potentialFileSize && sourceData.source_md5 == GetMd5HashAsString(potentialFile, true))
										{
											originalFile = potentialFile;
											bdfPatchFile = Path.Combine(patchDir, sourceData.patch_name);
											break;
										}
									}
								}

								if (originalFile != "") break;

							}
						}
						if (patch.source_path == "secondaryexecutable")
						{

						}
						if (patch.source_path != "mainexecutable" && patch.source_path != "secondaryexecutable" && patch.source_path != "")
						{
							string mainPotentialFile = Path.Combine(dirGameExec, patch.source_path);
							string mainPotentialFileDir = Path.GetDirectoryName(mainPotentialFile);

							long mainPotentialFileSize = -1;
							if (File.Exists(mainPotentialFile)) mainPotentialFileSize = new FileInfo(mainPotentialFile).Length;
							foreach (var sourceData in patch.source_data)
							{
								if (patch.bdf)
								{
									bdfPatchFile = Path.Combine(patchDir, sourceData.patch_name);
									if (!File.Exists(bdfPatchFile)) continue;
								}
								//Start looking at the current game executable
								if (sourceData.source_size == mainPotentialFileSize && sourceData.source_md5 == GetMd5HashAsString(mainPotentialFile, true))
								{
									originalFile = mainPotentialFile;
								}

								if (originalFile == "") //Uf the game executable dont match
								{
									//Verify all files in the game dir until we found a match
									var potentialFiles = Directory.GetFiles(mainPotentialFileDir, "*");
									foreach (var potentialFile in potentialFiles)
									{
										long potentialFileSize = new FileInfo(potentialFile).Length;
										if (sourceData.source_size == potentialFileSize && sourceData.source_md5 == GetMd5HashAsString(potentialFile, true))
										{
											originalFile = potentialFile;
											bdfPatchFile = Path.Combine(patchDir, sourceData.patch_name);
											break;
										}
									}
								}

								if (originalFile != "") break;
							}



						}

						//Now we apply patch
						if (originalFile != "" && expectedPatchFile != "" && File.Exists(originalFile) && !File.Exists(expectedPatchFile))
						{
							Utils.LogMessage($"{originalFile} => {expectedPatchFile} ({bdfPatchFile})");

							string expectedPatchFileDir = Path.GetDirectoryName(expectedPatchFile);
							if (!Directory.Exists(expectedPatchFileDir)) Directory.CreateDirectory(expectedPatchFileDir);

							if (patch.bdf && bdfPatchFile != "" && File.Exists(bdfPatchFile))
							{
								var oldFile = originalFile;
								var newFile = expectedPatchFile;
								try
								{
									using (var input = new FileStream(oldFile, FileMode.Open, FileAccess.Read, FileShare.Read))
									using (var output = new FileStream(newFile, FileMode.Create))
										BinaryPatch.Apply(input, () => new FileStream(bdfPatchFile, FileMode.Open, FileAccess.Read, FileShare.Read), output);
								}
								catch (Exception ex) { }
							}
							if (!patch.bdf)
							{
								File.Copy(originalFile, expectedPatchFile, true);
							}
						}
					}
				}
			}
			//End Apply Patch

			string reshadeiniFile = "";
			string reshadeiniFileSource = "";
			Dictionary<string, string> reshadeShaderPossibleDir = new Dictionary<string, string>();

			foreach (var file in filePaths)
			{
				string originalFileName = file;
				string newfile = directoryDest + file.Remove(0, directorySource.Length);
				newfile = newfile.Replace("[..]", "..");




				if (Path.GetDirectoryName(newfile).Contains(@"\[!windowed!]") && newfile.Contains(@"\[!windowed!]\"))
				{
					newfile = newfile.Replace(@"\[!windowed!]\", @"\");
					if (!TpSettingsManager.IsWindowed) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!fullscreen!]") && newfile.Contains(@"\[!fullscreen!]\"))
				{
					newfile = newfile.Replace(@"\[!fullscreen!]\", @"\");
					if (TpSettingsManager.IsWindowed) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!amd!]") && newfile.Contains(@"\[!amd!]\"))
				{
					newfile = newfile.Replace(@"\[!amd!]\", @"\");
					if (!Program.patchGpuFix || Program.gpuType <= 1) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!amdold!]") && newfile.Contains(@"\[!amdold!]\"))
				{
					newfile = newfile.Replace(@"\[!amdold!]\", @"\");
					if (!Program.patchGpuFix || Program.gpuType != 2) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!amdnew!]") && newfile.Contains(@"\[!amdnew!]\"))
				{
					newfile = newfile.Replace(@"\[!amdnew!]\", @"\");
					if (!Program.patchGpuFix || Program.gpuType != 3) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!amdrid!]") && newfile.Contains(@"\[!amdrid!]\"))
				{
					newfile = newfile.Replace(@"\[!amdrid!]\", @"\");
					if (!Program.patchGpuFix || Program.gpuType != 4) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!nvidia!]") && newfile.Contains(@"\[!nvidia!]\"))
				{
					newfile = newfile.Replace(@"\[!nvidia!]\", @"\");
					if (!Program.patchGpuFix || Program.gpuType != 0) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!intel!]") && newfile.Contains(@"\[!intel!]\"))
				{
					newfile = newfile.Replace(@"\[!intel!]\", @"\");
					if (!Program.patchGpuFix || Program.gpuType != 1) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_amd!]") && newfile.Contains(@"\[!no_amd!]\"))
				{
					newfile = newfile.Replace(@"\[!no_amd!]\", @"\");
					if (!Program.patchGpuFix || Program.gpuType >= 2) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_nvidia!]") && newfile.Contains(@"\[!no_nvidia!]\"))
				{
					newfile = newfile.Replace(@"\[!no_nvidia!]\", @"\");
					if (!Program.patchGpuFix || Program.gpuType == 0) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_intel!]") && newfile.Contains(@"\[!no_intel!]\"))
				{
					newfile = newfile.Replace(@"\[!no_intel!]\", @"\");
					if (!Program.patchGpuFix || Program.gpuType == 1) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!dwheel!]") && newfile.Contains(@"\[!dwheel!]\"))
				{
					newfile = newfile.Replace(@"\[!dwheel!]\", @"\");
					if (!Program.useDinputWheel) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!dhotas!]") && newfile.Contains(@"\[!dhotas!]\"))
				{
					newfile = newfile.Replace(@"\[!dhotas!]\", @"\");
					if (!Program.useDinputHotas) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!dlightgun!]") && newfile.Contains(@"\[!dlightgun!]\"))
				{
					newfile = newfile.Replace(@"\[!dlightgun!]\", @"\");
					if (!Program.useDinputLightGun) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!xinput!]") && newfile.Contains(@"\[!xinput!]\"))
				{
					newfile = newfile.Replace(@"\[!xinput!]\", @"\");
					if (!Program.useXinput) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_dwheel!]") && newfile.Contains(@"\[!no_dwheel!]\"))
				{
					newfile = newfile.Replace(@"\[!no_dwheel!]\", @"\");
					if (Program.useDinputWheel) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_dhotas!]") && newfile.Contains(@"\[!no_dhotas!]\"))
				{
					newfile = newfile.Replace(@"\[!no_dhotas!]\", @"\");
					if (Program.useDinputHotas) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_dlightgun!]") && newfile.Contains(@"\[!no_dlightgun!]\"))
				{
					newfile = newfile.Replace(@"\[!no_dlightgun!]\", @"\");
					if (Program.useDinputLightGun) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_xinput!]") && newfile.Contains(@"\[!no_xinput!]\"))
				{
					newfile = newfile.Replace(@"\[!no_xinput!]\", @"\");
					if (Program.useXinput) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!ffb!]") && newfile.Contains(@"\[!ffb!]\"))
				{
					newfile = newfile.Replace(@"\[!ffb!]\", @"\");
					if (!Program.patchFFB) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!set_resolution!]") && newfile.Contains(@"\[!set_resolution!]\"))
				{
					newfile = newfile.Replace(@"\[!set_resolution!]\", @"\");
					if (!Program.patchResolutionFix) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!dont_set_resolution!]") && newfile.Contains(@"\[!dont_set_resolution!]\"))
				{
					newfile = newfile.Replace(@"\[!dont_set_resolution!]\", @"\");
					if (Program.patchResolutionFix) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!set_displaymode_recommanded!]") && newfile.Contains(@"\[!set_displaymode_recommanded!]\"))
				{
					newfile = newfile.Replace(@"\[!set_displaymode_recommanded!]\", @"\");
					if (!Program.patchDisplayModeFix || Program.displayMode != 0) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!set_fullscreen!]") && newfile.Contains(@"\[!set_fullscreen!]\"))
				{
					newfile = newfile.Replace(@"\[!set_fullscreen!]\", @"\");
					if (!Program.patchDisplayModeFix || Program.displayMode != 1) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!set_windowed!]") && newfile.Contains(@"\[!set_windowed!]\"))
				{
					newfile = newfile.Replace(@"\[!set_windowed!]\", @"\");
					if (!Program.patchDisplayModeFix || Program.displayMode != 2) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!game_native_res!]") && newfile.Contains(@"\[!game_native_res!]\"))
				{
					newfile = newfile.Replace(@"\[!game_native_res!]\", @"\");
					if (Program.gpuResolution != 4) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!720p!]") && newfile.Contains(@"\[!720p!]\"))
				{
					newfile = newfile.Replace(@"\[!720p!]\", @"\");
					if (Program.gpuResolution != 0) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!1080p!]") && newfile.Contains(@"\[!1080p!]\"))
				{
					newfile = newfile.Replace(@"\[!1080p!]\", @"\");
					if (Program.gpuResolution != 1) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!2k!]") && newfile.Contains(@"\[!2k!]\"))
				{
					newfile = newfile.Replace(@"\[!2k!]\", @"\");
					if (Program.gpuResolution != 2) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!4k!]") && newfile.Contains(@"\[!4k!]\"))
				{
					newfile = newfile.Replace(@"\[!4k!]\", @"\");
					if (Program.gpuResolution != 3) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_game_native_res!]") && newfile.Contains(@"\[!no_game_native_res!]\"))
				{
					newfile = newfile.Replace(@"\[!no_game_native_res!]\", @"\");
					if (Program.gpuResolution == 4) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_720p!]") && newfile.Contains(@"\[!no_720p!]\"))
				{
					newfile = newfile.Replace(@"\[!no_720p!]\", @"\");
					if (Program.gpuResolution == 0) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_1080p!]") && newfile.Contains(@"\[!no_1080p!]\"))
				{
					newfile = newfile.Replace(@"\[!no_1080p!]\", @"\");
					if (Program.gpuResolution == 1) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_2k!]") && newfile.Contains(@"\[!no_2k!]\"))
				{
					newfile = newfile.Replace(@"\[!no_2k!]\", @"\");
					if (Program.gpuResolution == 2) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_4k!]") && newfile.Contains(@"\[!no_4k!]\"))
				{
					newfile = newfile.Replace(@"\[!no_4k!]\", @"\");
					if (Program.gpuResolution == 3) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!720p+!]") && newfile.Contains(@"\[!720p+!]\"))
				{
					newfile = newfile.Replace(@"\[!720p+!]\", @"\");
					if (Program.gpuResolution < 0 || Program.gpuResolution == 4) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!1080p+!]") && newfile.Contains(@"\[!1080p+!]\"))
				{
					newfile = newfile.Replace(@"\[!1080p+!]\", @"\");
					if (Program.gpuResolution < 1 || Program.gpuResolution == 4) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!2k+!]") && newfile.Contains(@"\[!2k+!]\"))
				{
					newfile = newfile.Replace(@"\[!2k+!]\", @"\");
					if (Program.gpuResolution < 2 || Program.gpuResolution == 4) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!4k+!]") && newfile.Contains(@"\[!4k+!]\"))
				{
					newfile = newfile.Replace(@"\[!4k+!]\", @"\");
					if (Program.gpuResolution < 3 || Program.gpuResolution == 4) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!720p-!]") && newfile.Contains(@"\[!720p-!]\"))
				{
					newfile = newfile.Replace(@"\[!720p-!]\", @"\");
					if (Program.gpuResolution > 0 || Program.gpuResolution == 4) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!1080p-!]") && newfile.Contains(@"\[!1080p-!]\"))
				{
					newfile = newfile.Replace(@"\[!1080p-!]\", @"\");
					if (Program.gpuResolution > 1 || Program.gpuResolution == 4) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!2k-!]") && newfile.Contains(@"\[!2k-!]\"))
				{
					newfile = newfile.Replace(@"\[!2k-!]\", @"\");
					if (Program.gpuResolution > 2 || Program.gpuResolution == 4) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!4k-!]") && newfile.Contains(@"\[!4k-!]\"))
				{
					newfile = newfile.Replace(@"\[!4k-!]\", @"\");
					if (Program.gpuResolution > 3 || Program.gpuResolution == 4) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!optional_reshade!]") && newfile.Contains(@"\[!optional_reshade!]\"))
				{
					newfile = newfile.Replace(@"\[!optional_reshade!]\", @"\");
					if (!Program.patchReshade) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_reshade!]") && newfile.Contains(@"\[!no_reshade!]\"))
				{
					newfile = newfile.Replace(@"\[!no_reshade!]\", @"\");
					if (Program.patchReshade) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!bezel!]") && newfile.Contains(@"\[!bezel!]\"))
				{
					newfile = newfile.Replace(@"\[!bezel!]\", @"\");
					if (!Program.useBezel) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_bezel!]") && newfile.Contains(@"\[!no_bezel!]\"))
				{
					newfile = newfile.Replace(@"\[!no_bezel!]\", @"\");
					if (Program.useBezel) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!crtfilter!]") && newfile.Contains(@"\[!crtfilter!]\"))
				{
					newfile = newfile.Replace(@"\[!crtfilter!]\", @"\");
					if (!Program.useCrt) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_crtfilter!]") && newfile.Contains(@"\[!no_crtfilter!]\"))
				{
					newfile = newfile.Replace(@"\[!no_crtfilter!]\", @"\");
					if (Program.useCrt) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!keepaspectratio!]") && newfile.Contains(@"\[!keepaspectratio!]\"))
				{
					newfile = newfile.Replace(@"\[!keepaspectratio!]\", @"\");
					if (!Program.useKeepAspectRatio) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_keepaspectratio!]") && newfile.Contains(@"\[!no_keepaspectratio!]\"))
				{
					newfile = newfile.Replace(@"\[!no_keepaspectratio!]\", @"\");
					if (Program.useKeepAspectRatio) continue;
				}

				if (Path.GetDirectoryName(newfile).Contains(@"\[!at_least_one_sinden!]") && newfile.Contains(@"\[!at_least_one_sinden!]\"))
				{
					newfile = newfile.Replace(@"\[!at_least_one_sinden!]\", @"\");
					if (!Program.useDinputLightGun || ((Program.GunAGuid == "" || ConfigurationManager.MainConfig.gunAType != "sinden") && (Program.GunBGuid == "" || ConfigurationManager.MainConfig.gunBType != "sinden"))) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!magpie_sinden_bezel!]") && newfile.Contains(@"\[!magpie_sinden_bezel!]\"))
				{
					newfile = newfile.Replace(@"\[!magpie_sinden_bezel!]\", @"\");
					if (!Program.useDinputLightGun || (ConfigurationManager.MainConfig.gunAType != "sinden" && ConfigurationManager.MainConfig.gunBType != "sinden")) continue;
					if (ConfigurationManager.MainConfig.magpieBorderSize != 1.5) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_sinden!]") && newfile.Contains(@"\[!no_sinden!]\"))
				{
					newfile = newfile.Replace(@"\[!no_sinden!]\", @"\");
					if ((Program.GunAGuid != "" && ConfigurationManager.MainConfig.gunAType == "sinden") || (Program.GunAGuid != "" && ConfigurationManager.MainConfig.gunBType == "sinden")) continue;
				}

				if (Path.GetDirectoryName(newfile).Contains(@"\[!crosshair_gun1_and_gun2!]") && newfile.Contains(@"\[!crosshair_gun1_and_gun2!]\"))
				{
					newfile = newfile.Replace(@"\[!crosshair_gun1_and_gun2!]\", @"\");
					if (!Program.useDinputLightGun || !Program.crosshairA || !Program.crosshairB) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!crosshair_gun1_only!]") && newfile.Contains(@"\[!crosshair_gun1_only!]\"))
				{
					newfile = newfile.Replace(@"\[!crosshair_gun1_only!]\", @"\");
					if (!Program.useDinputLightGun || !Program.crosshairA || Program.crosshairB) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!crosshair_gun2_only!]") && newfile.Contains(@"\[!crosshair_gun2_only!]\"))
				{
					newfile = newfile.Replace(@"\[!crosshair_gun2_only!]\", @"\");
					if (!Program.useDinputLightGun || Program.crosshairA || !Program.crosshairB) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!hide_crosshair!]") && newfile.Contains(@"\[!hide_crosshair!]\"))
				{
					newfile = newfile.Replace(@"\[!hide_crosshair!]\", @"\");
					if (!Program.hideCrosshair) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!show_crosshair!]") && newfile.Contains(@"\[!show_crosshair!]\"))
				{
					newfile = newfile.Replace(@"\[!show_crosshair!]\", @"\");
					if (Program.hideCrosshair) continue;
				}

				if (Path.GetDirectoryName(newfile).Contains(@"\[!guna_found!]") && newfile.Contains(@"\[!guna_found!]\"))
				{
					newfile = newfile.Replace(@"\[!guna_found!]\", @"\");
					if (Program.GunAGuid == "") continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!gunb_found!]") && newfile.Contains(@"\[!gunb_found!]\"))
				{
					newfile = newfile.Replace(@"\[!gunb_found!]\", @"\");
					if (Program.GunBGuid == "") continue;
				}

				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_guna_found!]") && newfile.Contains(@"\[!no_guna_found!]\"))
				{
					newfile = newfile.Replace(@"\[!no_guna_found!]\", @"\");
					if (Program.GunAGuid != "") continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_gunb_found!]") && newfile.Contains(@"\[!no_gunb_found!]\"))
				{
					newfile = newfile.Replace(@"\[!no_gunb_found!]\", @"\");
					if (Program.GunBGuid != "") continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!shifter_found!]") && newfile.Contains(@"\[!shifter_found!]\"))
				{
					newfile = newfile.Replace(@"\[!shifter_found!]\", @"\");
					if (!Program.shifterGuidFound) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_shifter_found!]") && newfile.Contains(@"\[!no_shifter_found!]\"))
				{
					newfile = newfile.Replace(@"\[!no_shifter_found!]\", @"\");
					if (Program.shifterGuidFound) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!throttle_found!]") && newfile.Contains(@"\[!throttle_found!]\"))
				{
					newfile = newfile.Replace(@"\[!throttle_found!]\", @"\");
					if (!Program.throttleGuidFound) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_throttle_found!]") && newfile.Contains(@"\[!no_throttle_found!]\"))
				{
					newfile = newfile.Replace(@"\[!no_throttle_found!]\", @"\");
					if (Program.throttleGuidFound) continue;
				}

				if (Path.GetDirectoryName(newfile).Contains(@"\[!vsync!]") && newfile.Contains(@"\[!vsync!]\"))
				{
					newfile = newfile.Replace(@"\[!vsync!]\", @"\");
					if (!Program.forceVsync) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_vsync!]") && newfile.Contains(@"\[!no_vsync!]\"))
				{
					newfile = newfile.Replace(@"\[!no_vsync!]\", @"\");
					if (Program.forceVsync) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!normal_perfprofile!]") && newfile.Contains(@"\[!normal_perfprofile!]\"))
				{
					newfile = newfile.Replace(@"\[!normal_perfprofile!]\", @"\");
					if (Program.performanceProfile != 0) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!low_perfprofile!]") && newfile.Contains(@"\[!low_perfprofile!]\"))
				{
					newfile = newfile.Replace(@"\[!low_perfprofile!]\", @"\");
					if (Program.performanceProfile != 1) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!high_perfprofile!]") && newfile.Contains(@"\[!high_perfprofile!]\"))
				{
					newfile = newfile.Replace(@"\[!high_perfprofile!]\", @"\");
					if (Program.performanceProfile != 2) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!not_normal_perfprofile!]") && newfile.Contains(@"\[!not_normal_perfprofile!]\"))
				{
					newfile = newfile.Replace(@"\[!not_normal_perfprofile!]\", @"\");
					if (Program.performanceProfile == 0) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!not_low_perfprofile!]") && newfile.Contains(@"\[!not_low_perfprofile!]\"))
				{
					newfile = newfile.Replace(@"\[!not_low_perfprofile!]\", @"\");
					if (Program.performanceProfile == 1) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!not_high_perfprofile!]") && newfile.Contains(@"\[!not_high_perfprofile!]\"))
				{
					newfile = newfile.Replace(@"\[!not_high_perfprofile!]\", @"\");
					if (Program.performanceProfile == 2) continue;
				}

				if (Path.GetDirectoryName(newfile).Contains(@"\[!translation!]") && newfile.Contains(@"\[!translation!]\"))
				{
					newfile = newfile.Replace(@"\[!translation!]\", @"\");
					if (Program.patchLang == 0) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!english!]") && newfile.Contains(@"\[!english!]\"))
				{
					newfile = newfile.Replace(@"\[!english!]\", @"\");
					if (Program.patchLang != 1) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!french!]") && newfile.Contains(@"\[!french!]\"))
				{
					newfile = newfile.Replace(@"\[!french!]\", @"\");
					if (Program.patchLang != 2) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_translation!]") && newfile.Contains(@"\[!no_translation!]\"))
				{
					newfile = newfile.Replace(@"\[!no_translation!]\", @"\");
					if (Program.patchLang != 0) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_english!]") && newfile.Contains(@"\[!no_english!]\"))
				{
					newfile = newfile.Replace(@"\[!no_english!]\", @"\");
					if (Program.patchLang == 1) continue;
				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!no_french!]") && newfile.Contains(@"\[!no_french!]\"))
				{
					newfile = newfile.Replace(@"\[!no_french!]\", @"\");
					if (Program.patchLang == 2) continue;
				}

				if (Path.GetDirectoryName(newfile).Contains(@"\[!120hz!]") && newfile.Contains(@"\[!120hz!]\"))
				{
					//MessageBox.Show("ok");
					newfile = newfile.Replace(@"\[!120hz!]\", @"\");
					if (Program.refreshRate < 120) continue;
					if (file.ToLower().EndsWith(".remplacerefreshrate"))
					{
						string filedata = File.ReadAllText(file);
						string newContent = filedata.Replace("{refreshRate}", Program.refreshRate.ToString());
						File.WriteAllText(Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file)), newContent);
						originalFileName = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
						newfile = Path.Combine(Path.GetDirectoryName(newfile), Path.GetFileNameWithoutExtension(newfile));
					}
					if (File.Exists(file + ".remplacerefreshrate"))
					{
						continue;
					}


				}
				if (Path.GetDirectoryName(newfile).Contains(@"\[!60hz!]") && newfile.Contains(@"\[!60hz!]\"))
				{
					newfile = newfile.Replace(@"\[!60hz!]\", @"\");
					if (Program.refreshRate >= 120) continue;
				}

				var anyTagMatch = Regex.Match(newfile, @"\\\[!tag-any\{[a-zA-Z0-9+\-_]+\}!\]\\", RegexOptions.IgnoreCase);
				if (anyTagMatch.Success)
				{
					string anyTagDir = anyTagMatch.Groups[0].Value;
					string anyTagFlags = anyTagMatch.Groups[0].Value.Substring(11, anyTagMatch.Groups[0].Value.Length - 15);
					bool anyFlagsValid = false;
					foreach (var flag in anyTagFlags.Split(","))
					{
						if (flag.Trim() == "") continue;
						if (TpSettingsManager.tags.Contains(flag.Trim().ToLower()))
						{
							anyFlagsValid = true;
							break;
						}
					}
					if (anyFlagsValid)
					{
						newfile = newfile.Replace(anyTagDir, @"\");
					}
					else continue;
				}

				var allTagMatch = Regex.Match(newfile, @"\\\[!tag-all\{[a-zA-Z0-9+\-_]+\}!\]\\", RegexOptions.IgnoreCase);
				if (allTagMatch.Success)
				{
					string allTagDir = allTagMatch.Groups[0].Value;
					string allTagFlags = allTagMatch.Groups[0].Value.Substring(11, allTagMatch.Groups[0].Value.Length - 15);
					bool allFlagsValid = true;
					foreach (var flag in allTagFlags.Split(","))
					{
						if (flag.Trim() == "") continue;
						if (!TpSettingsManager.tags.Contains(flag.Trim().ToLower()))
						{
							allFlagsValid = false;
							break;
						}
					}
					if (allFlagsValid)
					{
						newfile = newfile.Replace(allTagDir, @"\");
					}
					else continue;
				}

				var tagMd5Match = Regex.Match(newfile, @"\\\[!md5-[a-zA-Z0-9]+!\]\\", RegexOptions.IgnoreCase);
				if (tagMd5Match.Success)
				{
					bool isSameMD5 = false;
					string tagMd5MatchDir = tagMd5Match.Groups[0].Value;
					string md5val = tagMd5Match.Groups[0].Value.Substring(7, tagMd5Match.Groups[0].Value.Length - 10);
					if (executableGame != "" && File.Exists(executableGame))
					{
						var md5exec = Utils.GetMd5HashAsString(executableGame, true);
						if (md5exec.ToUpper() == md5val.ToUpper())
						{
							isSameMD5 = true;
						}
					}
					if (isSameMD5)
					{
						newfile = newfile.Replace(tagMd5MatchDir, @"\");
					}
					else continue;
				}
				tagMd5Match = Regex.Match(newfile, @"\\\[!md5-([a-zA-Z0-9]+)-([a-zA-Z0-9 #-_\.|]+)!\]\\", RegexOptions.IgnoreCase);
				if (tagMd5Match.Success)
				{
					bool isSameMD5 = false;
					string tagMd5MatchDir = tagMd5Match.Groups[0].Value;
					string md5val = tagMd5Match.Groups[1].Value;
					string filePathToVerify = executableGame; // Par défaut, utiliser executableGame
					string relativeFileName = tagMd5Match.Groups[2].Value;
					relativeFileName = relativeFileName.Replace('#', Path.DirectorySeparatorChar);
					filePathToVerify = Path.Combine(Path.GetDirectoryName(executableGame), relativeFileName);
					filePathToVerify = Path.GetFullPath(filePathToVerify);

					if (File.Exists(filePathToVerify))
					{
						var md5exec = Utils.GetMd5HashAsString(filePathToVerify, true);
						if (md5exec.ToUpper() == md5val.ToUpper())
						{
							isSameMD5 = true;
						}
					}
					if (isSameMD5)
					{
						newfile = newfile.Replace(tagMd5MatchDir, @"\");
					}
					else continue;
				}

				var tagSizeMatch = Regex.Match(newfile, @"\\\[!size-[0-9]+!\]\\", RegexOptions.IgnoreCase);
				if (tagSizeMatch.Success)
				{
					bool isSameSize = false;
					string tagSizeMatchDir = tagSizeMatch.Groups[0].Value;
					string Sizeval = tagSizeMatch.Groups[0].Value.Substring(8, tagSizeMatch.Groups[0].Value.Length - 11);
					if (executableGame != "" && File.Exists(executableGame))
					{
						var sizeExec = new FileInfo(executableGame).Length.ToString();

						if (Sizeval.ToUpper() == sizeExec.ToUpper())
						{
							isSameSize = true;
						}
					}
					if (isSameSize)
					{
						newfile = newfile.Replace(tagSizeMatchDir, @"\");
					}
					else continue;
				}
				tagSizeMatch = Regex.Match(newfile, @"\\\[!size-([0-9]+)-([a-zA-Z0-9 #-_\.|]+)!\]\\", RegexOptions.IgnoreCase);
				if (tagSizeMatch.Success)
				{
					bool isSameSize = false;
					string tagSizeMatchDir = tagSizeMatch.Groups[0].Value;
					string Sizeval = tagSizeMatch.Groups[1].Value;
					string filePathToVerify = executableGame; // Par défaut, utiliser executableGame
					string relativeFileName = tagSizeMatch.Groups[2].Value;
					// Remplace les | par des / pour reconstruire le chemin
					relativeFileName = relativeFileName.Replace('#', Path.DirectorySeparatorChar);
					filePathToVerify = Path.Combine(Path.GetDirectoryName(executableGame), relativeFileName);
					filePathToVerify = Path.GetFullPath(filePathToVerify);

					if (File.Exists(filePathToVerify))
					{
						var sizeExec = new FileInfo(filePathToVerify).Length.ToString();

						if (Sizeval.ToUpper() == sizeExec.ToUpper())
						{
							isSameSize = true;
						}
					}
					if (isSameSize)
					{
						newfile = newfile.Replace(tagSizeMatchDir, @"\");
					}
					else continue;
				}


				if (Path.GetDirectoryName(newfile).Contains(@"\[!cachereshade!]") && newfile.Contains(@"\[!cachereshade!]\"))
				{
					continue;
				}

				if (Path.GetDirectoryName(newfile).Contains(@"\[!patch_generation!]") && newfile.Contains(@"\[!patch_generation!]\"))
				{
					continue;
				}

				if (Path.GetDirectoryName(newfile).Contains(@"\[!dontlinkthis!]") && newfile.Contains(@"\[!dontlinkthis!]\"))
				{
					continue;
				}

				if (Path.GetFileName(newfile).ToLower() == "dgvoodoo.conf.custom")
				{
					continue;
				}

				if (Path.GetDirectoryName(newfile) != null && Regex.IsMatch(Path.GetDirectoryName(newfile), @"\\\[!!([A-Za-z0-9 ]+)!!\]") && Regex.IsMatch(newfile, @"\\\[!!([A-Za-z0-9 ]+)!!\]\\"))
				{
					newfile = Regex.Replace(newfile, @"\\\[!!([A-Za-z0-9 ]+)!!\]\\", @"\");
				}

				newfile = Path.GetFullPath(newfile);



				if (Path.GetFileName(newfile).ToLower() == "dgvoodoo.conf")
				{
					try
					{
						File.Copy(file, file + ".custom", true);
						if (File.Exists(file + ".custom"))
						{
							IniFile dgvodooIni = new IniFile(file + ".custom");
							string dgvAntialiasing = dgvodooIni.Read("Antialiasing", "DirectX");
							string dgvFiltering = dgvodooIni.Read("Filtering", "DirectX");
							int dgvFilteringValue = 0;
							int.TryParse(dgvFiltering, out dgvFilteringValue);
							int dgvAntialiasingValue = 0;
							int.TryParse(dgvAntialiasing.Trim('x'), out dgvAntialiasingValue);
							string dgvForceVerticalSync = dgvodooIni.Read("ForceVerticalSync", "DirectX");
							if (Program.performanceProfile == 1)
							{
								dgvodooIni.Write("Antialiasing", "appdriven", "DirectX");
								dgvodooIni.Write("Filtering", "appdriven", "DirectX");
							}
							if (Program.performanceProfile == 2)
							{
								dgvodooIni.Write("Antialiasing", "8x", "DirectX");
								dgvodooIni.Write("Filtering", "16", "DirectX");
							}

							if (Program.forceVsync) dgvodooIni.Write("ForceVerticalSync", "true", "DirectX");
							else dgvodooIni.Write("ForceVerticalSync", "false", "DirectX");
							originalFileName = file + ".custom";
						}
					}
					catch { }






				}

				if (Path.GetFileName(newfile).ToLower() == "reshade.ini")
				{
					reshadeiniFile = newfile;
					reshadeiniFileSource = originalFileName;
				}

				if (Path.GetDirectoryName(newfile).ToLower().Contains(@"\reshade-shaders\"))
				{

					var reshadeShaderDir = newfile.ToLower().Split(@"\reshade-shaders\")[0] + @"\reshade-shaders";
					var reshadeShaderDirSource = originalFileName.ToLower().Split(@"\reshade-shaders\")[0] + @"\reshade-shaders";
					if (!reshadeShaderPossibleDir.ContainsKey(reshadeShaderDir)) reshadeShaderPossibleDir.Add(reshadeShaderDir, reshadeShaderDirSource);

				}

				/*
				if (Path.GetFileName(newfile).ToLower() == "reshade.ini")
				{
					MessageBox.Show("debug rewrite reshade");
					reshadeiniFile = newfile;
					string currentDir = Path.GetDirectoryName(reshadeiniFile);
					string reshadePath = Path.Combine(currentDir, "reshade-shaders");
					if (Directory.Exists(reshadePath))
					{
						string reshadeTemp = Path.Combine(reshadePath, "[!cachereshade!]");
						if (!Directory.Exists(reshadeTemp))
						{
							Directory.CreateDirectory(reshadeTemp);
							IniFile reshadeIniFile = new IniFile(file);
							reshadeIniFile.Write("IntermediateCachePath", reshadeTemp, "GENERAL");
						}
					}
				}
				*/


				if (Path.GetDirectoryName(newfile).Contains(@"\[!moveto!]") && newfile.Contains(@"\[!moveto!]\"))
				{
					string fileName = Path.GetFileName(file);
					if (moveToDest == "")
					{
						string dir = Path.GetDirectoryName(file);
						string destFile = Path.Combine(dir, "destination.txt");
						if (File.Exists(destFile))
						{
							string pathWithEnvVar = File.ReadAllText(destFile);
							string expandedPath = Environment.ExpandEnvironmentVariables(pathWithEnvVar);
							if (!Path.IsPathRooted(expandedPath))
							{
								expandedPath = Path.Combine(directoryDest, expandedPath);
							}
							string moveToDestContent = Path.GetFullPath(expandedPath);
							moveToDestContent = moveToDestContent.Trim().TrimEnd('\r').TrimEnd('\n').TrimEnd('\r').Trim();
							if (Directory.Exists(moveToDestContent))
							{
								moveToDest = moveToDestContent;
								moveNeedAdmin = !Utils.HasWritePermissionOnDir(moveToDest);
							}
						}
					}
					if (fileName != "destination.txt" && moveToDest != "")
					{
						string dest = Path.Combine(moveToDest, fileName);
						if (moveNeedAdmin)
						{
							moveAhkCode += @$"FileCopy, {file}, {dest}, 1" + "\n";
						}
						else
						{
							try
							{
								File.Copy(file, dest, true);
								if (!Program.movetoFiles.ContainsKey(dest))
								{
									Program.movetoFiles[dest] = file;
								}
								else
								{
									Program.movetoFiles.Add(dest, file);
								}
							}
							catch { }
						}

					}

					continue;
				}

				if (Path.GetDirectoryName(newfile).Contains(@"\[!moveto_nocopyback!]") && newfile.Contains(@"\[!moveto_nocopyback!]\"))
				{
					string fileName = Path.GetFileName(file);
					if (moveToDest == "")
					{
						string dir = Path.GetDirectoryName(file);
						string destFile = Path.Combine(dir, "destination.txt");
						if (File.Exists(destFile))
						{
							string pathWithEnvVar = File.ReadAllText(destFile);
							string expandedPath = Environment.ExpandEnvironmentVariables(pathWithEnvVar);

							if (!Path.IsPathRooted(expandedPath))
							{
								expandedPath = Path.Combine(directoryDest, expandedPath);
							}

							string moveToDestContent = Path.GetFullPath(expandedPath);
							moveToDestContent = moveToDestContent.Trim().TrimEnd('\r').TrimEnd('\n').TrimEnd('\r').Trim();
							if (Directory.Exists(moveToDestContent))
							{
								moveToDest = moveToDestContent;
								moveNeedAdmin = !Utils.HasWritePermissionOnDir(moveToDest);
							}
						}
					}
					if (fileName != "destination.txt" && moveToDest != "")
					{
						string dest = Path.Combine(moveToDest, fileName);
						if (moveNeedAdmin)
						{
							moveAhkCode += @$"FileCopy, {file}, {dest}, 1" + "\n";
						}
						else
						{
							try
							{
								File.Copy(file, dest, true);
							}
							catch { }
						}

					}

					continue;
				}

				if (Path.GetFileName(newfile).ToLower() == "[!magpiereshade!].ini")
				{
					Utils.LogMessage("Magpie Reshade : " + file);
					Program.magpieIni = file;
					continue;
				}

				if (Path.GetFileNameWithoutExtension(newfile).StartsWith(@"[!main_executable!") && Path.GetFileNameWithoutExtension(newfile).EndsWith(@"]"))
				{
					if (main_executable_linked)
					{
						continue;
					}
					else
					{
						if (executableGame != "" && File.Exists(executableGame))
						{
							string executableGameExt = Path.GetExtension(executableGame).ToLower();
							string newFileExt = Path.GetExtension(newfile).ToLower();
							string executableGameDir = Path.GetFullPath(Path.GetDirectoryName(executableGame));
							string newFileDir = Path.GetFullPath(Path.GetDirectoryName(newfile));

							if ((executableGameExt == newFileExt || newFileExt == string.Empty) && executableGameDir == newFileDir)
							{
								main_executable_linked = true;
								newfile = executableGame;
							}
						}
					}
				}

				string newfiledir = Directory.GetParent(newfile).FullName;
				if (!Directory.Exists(newfiledir))
				{
					List<string> listDir = new List<string>();
					string subdir = newfiledir;
					while (!Directory.Exists(subdir))
					{
						if (!Directory.Exists(subdir)) listDir.Add(subdir);
						subdir = Directory.GetParent(subdir).FullName;
					}
					listDir.Reverse();
					foreach (var subdirV in listDir)
					{
						DirectoryList.Add(subdirV);
					}

					Directory.CreateDirectory(newfiledir);
				}
				if (File.Exists(newfile))
				{
					if (File.Exists(newfile + ".filetorestore")) File.Delete(newfile + ".filetorestore");
					File.Move(newfile, newfile + ".filetorestore", true);
				}
				if (useSoftLink) CreateSoftlink(originalFileName, newfile);
				else MakeLink(originalFileName, newfile);

				Utils.LogMessage($"{originalFileName}");
			}

			if (reshadeiniFile != "" && File.Exists(reshadeiniFile))
			{
				Utils.LogMessage($"found reshade ici : {reshadeiniFile}");
				string currentDir = Path.GetDirectoryName(reshadeiniFile);
				IniFile reshadeIniFile = new IniFile(reshadeiniFile);
				string reshadePath = Path.Combine(currentDir, "reshade-shaders");

				if (reshadeShaderPossibleDir.ContainsKey(reshadePath.ToLower()))
				{
					string reshadeTemp = Path.GetFullPath(GetExactPathName(reshadeShaderPossibleDir[reshadePath.ToLower()]));
					reshadeTemp = Path.Combine(reshadeTemp, "[!cachereshade!]");
					if (!Directory.Exists(reshadeTemp))
					{
						Directory.CreateDirectory(reshadeTemp);
					}
					reshadeIniFile.Write("IntermediateCachePath", reshadeTemp, "GENERAL");
				}

				string presetStr = "PresetPath";
				string reshadePreset = reshadeIniFile.Read("PresetPath", "GENERAL").Trim();
				if (reshadePreset == "")
				{
					reshadePreset = reshadeIniFile.Read("CurrentPresetPath", "GENERAL").Trim();
					if (reshadePreset != "") presetStr = "CurrentPresetPath";
				}
				Utils.LogMessage($"reshadePreset : {reshadePreset}");

				if (reshadePreset.ToLower().EndsWith(".ini"))
				{

					string reshadePresetFile = reshadePreset;
					string reshadePresetBase = reshadePreset.Replace("-low.ini", ".ini");
					reshadePresetBase = reshadePresetBase.Replace("-high.ini", ".ini");
					reshadePresetBase = reshadePresetBase.Substring(0, reshadePresetBase.Length - 4);

					string reshadePresetFileBase = reshadePresetBase + ".ini";
					string reshadePresetFileLow = reshadePresetBase + "-low.ini";
					string reshadePresetFileHigh = reshadePresetBase + "-high.ini";

					if (!Path.IsPathRooted(reshadePreset))
					{
						reshadePresetFile = Path.Combine(currentDir, reshadePreset);
						reshadePresetFileBase = Path.GetFullPath(Path.Combine(currentDir, reshadePresetBase + ".ini"));
						reshadePresetFileLow = Path.GetFullPath(Path.Combine(currentDir, reshadePresetBase + "-low.ini"));
						reshadePresetFileHigh = Path.GetFullPath(Path.Combine(currentDir, reshadePresetBase + "-high.ini"));
					}

					Utils.LogMessage($"check for : {reshadePresetFileBase} {reshadePresetFileLow} {reshadePresetFileHigh}");


					if (Program.performanceProfile == 0 && reshadePresetFile.ToLower() != reshadePresetFileBase.ToLower())
					{
						reshadeIniFile.Write(presetStr, reshadePresetBase + ".ini", "GENERAL");
						Utils.LogMessage($"write : " + reshadePresetBase + ".ini");
					}

					if (Program.performanceProfile == 1 && reshadePresetFile.ToLower() != reshadePresetFileLow.ToLower())
					{
						if (File.Exists(reshadePresetFileLow))
						{
							reshadeIniFile.Write(presetStr, reshadePresetBase + "-low.ini", "GENERAL");
							Utils.LogMessage($"write2 : " + reshadePresetBase + "-low.ini");
						}
						else if (reshadePresetFile.ToLower() != reshadePresetFileBase.ToLower())
						{
							reshadeIniFile.Write(presetStr, reshadePresetBase + ".ini", "GENERAL");
							Utils.LogMessage($"write2 : " + reshadePresetBase + ".ini");
						}
					}

					if (Program.performanceProfile == 2 && reshadePresetFile.ToLower() != reshadePresetFileHigh.ToLower())
					{
						if (File.Exists(reshadePresetFileHigh))
						{
							reshadeIniFile.Write(presetStr, reshadePresetBase + "-high.ini", "GENERAL");
							Utils.LogMessage($"write3 : " + reshadePresetBase + "-high.ini");
						}
						else if (reshadePresetFile.ToLower() != reshadePresetFileBase.ToLower())
						{
							reshadeIniFile.Write(presetStr, reshadePresetBase + ".ini", "GENERAL");
							Utils.LogMessage($"write3 : " + reshadePresetBase + ".ini");
						}
					}

					if (reshadePresetFile.ToLower() != reshadePresetFileBase.ToLower() && !File.Exists(reshadePresetFile))
					{
						reshadeIniFile.Write(presetStr, reshadePresetBase + ".ini", "GENERAL");
						Utils.LogMessage($"write5 : " + reshadePresetBase + ".ini");
					}



				}


			}

			if (DirectoryList.Count() > 0)
			{
				string json = JsonConvert.SerializeObject(DirectoryList, Formatting.Indented);
				File.WriteAllText(Path.Combine(directoryDest, "tempDirList.json"), json);
			}

			if (moveAhkCode != "")
			{
				File.WriteAllText(Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "moveadmin.tmp.ahk"), moveAhkCode);
				string selfExe = Process.GetCurrentProcess().MainModule.FileName;
				if (!Utils.CheckTaskExist(selfExe, "--moveadmin"))
				{
					string exePath = selfExe;
					string exeDir = Path.GetDirectoryName(exePath);
					Process process = new Process();
					process.StartInfo.FileName = selfExe;
					process.StartInfo.Arguments = "--registerTask " + $"\"{selfExe}\" " + "--moveadmin";
					process.StartInfo.WorkingDirectory = exeDir;
					process.StartInfo.UseShellExecute = true;
					process.StartInfo.Verb = "runas";
					process.Start();
					process.WaitForExit();
				}
				Utils.ExecuteTask(Utils.ExeToTaskName(selfExe, "--moveadmin"), -1);

			}
		}

		public static bool RealTestHardLinkEligible(string target, string source = "", bool checkOnlyHardlink = false)
		{
			bool linkAutoHardLink = false;
			bool linkAutoSoftLink = false;

			if (Directory.Exists(target))
			{
				string targetTest = target;
				string linktest1 = Path.Combine(targetTest, "test.link.write");
				string linktest2 = Path.Combine(targetTest, "test.link.hardlink");
				string linktest3 = Path.Combine(targetTest, "test.link.softlink");
				try
				{
					if (File.Exists(linktest1)) File.Delete(linktest1);
					if (File.Exists(linktest2)) File.Delete(linktest2);
					if (File.Exists(linktest3)) File.Delete(linktest3);
				}
				catch { }


				try
				{
					File.WriteAllText(linktest1, "todelete");
				}
				catch
				{
					if (File.Exists(linktest1)) { File.Delete(linktest1); }
					return false;

				}

				string sourceLinkTest = linktest1;
				if (!string.IsNullOrEmpty(source))
				{
					if (File.Exists(source)) sourceLinkTest = source;
					else if (Directory.Exists(source))
					{
						try
						{
							sourceLinkTest = Path.Combine(source, "test.link.write");
							File.WriteAllText(sourceLinkTest, "todelete");
						}
						catch
						{
							if (File.Exists(linktest1)) { File.Delete(linktest1); }
							if (File.Exists(sourceLinkTest)) { File.Delete(sourceLinkTest); }
							return false;
						}
					}
				}

				if (!File.Exists(linktest1)) return false;

				if (sourceLinkTest != "")
				{
					try
					{
						Utils.MakeLink(sourceLinkTest, linktest2);
					}
					catch { }
					try
					{
						Utils.CreateSoftlink(sourceLinkTest, linktest3);
					}
					catch { }
				}
				try
				{
					if (File.Exists(linktest1)) { File.Delete(linktest1); }
					if (File.Exists(linktest2)) { linkAutoHardLink = true; File.Delete(linktest2); }
					if (File.Exists(linktest3)) { linkAutoSoftLink = true; File.Delete(linktest3); }
				}
				catch { }
				if (checkOnlyHardlink)
				{
					return linkAutoHardLink;
				}
				if (linkAutoHardLink || linkAutoSoftLink) return true;
			}
			return false;
		}

		public static void CleanHardLinksFilesOriginal(string directoryToClean, string originalLinkDir)
		{
			originalLinkDir = Path.GetFullPath(originalLinkDir);
			if (!Directory.Exists(directoryToClean)) return;
			if (!Directory.Exists(originalLinkDir)) return;

			bool CanUseHardLink = true;
			if (!AreFoldersOnSameDrive(directoryToClean, originalLinkDir)) CanUseHardLink = false;
			if (!Program.linkAutoHardLink) CanUseHardLink = false;

			bool CanUseSoftLink = Program.linkAutoSoftLink;

			var filePaths = Directory.EnumerateFiles(directoryToClean, "*", new EnumerationOptions
			{
				IgnoreInaccessible = true,
				RecurseSubdirectories = true
			});
			foreach (var file in filePaths)
			{
				//Utils.LogMessage($"DEBUG Baggio 1: IsHardLink({file}, {originalLinkDir})");
				//if (Program.DebugMode) Utils.LogMessage($"Check Link for {file}");
				if (CanUseHardLink && IsHardLink(file, originalLinkDir))
				{
					if (Program.DebugMode) Utils.LogMessage($"{file} is Hardlink, delete it");
					try
					{
						File.Delete(file);
					}
					catch (Exception ex)
					{
						if (File.Exists(file))
						{
							try
							{
								FileInfo finfo = new FileInfo(file);
								if (finfo.IsReadOnly)
								{
									finfo.IsReadOnly = false;
									finfo.Delete();
								}
							}
							catch (Exception ex2) { }
						}

					}
				}
				else if (CanUseSoftLink && IsSoftLink(file, originalLinkDir))
				{
					if (Program.DebugMode) Utils.LogMessage($"{file} is Softlink, delete it");
					try
					{
						File.Delete(file);
					}
					catch (Exception ex)
					{
						if (File.Exists(file))
						{
							try
							{
								FileInfo finfo = new FileInfo(file);
								if (finfo.IsReadOnly)
								{
									finfo.IsReadOnly = false;
									finfo.Delete();
								}
							}
							catch (Exception ex2) { }
						}

					}
				}
			}
			foreach (var file in filePaths)
			{
				if (file.EndsWith(".filetorestore") && File.Exists(file))
				{
					string originalName = file.Substring(0, file.Length - 14);
					if (!File.Exists(originalName))
					{
						File.Move(file, originalName);
					}
				}
			}

			string tempDirFile = Path.Combine(directoryToClean, "tempDirList.json");
			if (File.Exists(tempDirFile))
			{
				try
				{
					List<string> DirList = (List<string>)JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(tempDirFile));
					foreach (var dir in DirList)
					{
						CleanTemporaryDirectories(dir);
					}
				}
				catch { }
				File.Delete(tempDirFile);
			}

		}

		public static bool PrecheckLinksFile(string directoryToClean, string originalLinkDir, string executableGameFile)
		{

			directoryToClean = Path.GetFullPath(directoryToClean);
			originalLinkDir = Path.GetFullPath(originalLinkDir);
			if (!Directory.Exists(directoryToClean)) return false;
			if (!Directory.Exists(originalLinkDir)) return false;

			List<string> filesToCheck = new List<string>();
			var filePaths = Directory.EnumerateFiles(originalLinkDir, "*", new EnumerationOptions
			{
				IgnoreInaccessible = true,
				RecurseSubdirectories = true
			});
			foreach (var fileInLinkFolder in filePaths)
			{
				string file = directoryToClean + fileInLinkFolder.Remove(0, originalLinkDir.Length).TrimStart();
				file = file.Replace("[..]", "..");


				if (Path.GetDirectoryName(file).Contains(@"\[!windowed!]") && file.Contains(@"\[!windowed!]\"))
				{
					file = file.Replace(@"\[!windowed!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!fullscreen!]") && file.Contains(@"\[!fullscreen!]\"))
				{
					file = file.Replace(@"\[!fullscreen!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!amd!]") && file.Contains(@"\[!amd!]\"))
				{
					file = file.Replace(@"\[!amd!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!amdold!]") && file.Contains(@"\[!amdold!]\"))
				{
					file = file.Replace(@"\[!amdold!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!amdnew!]") && file.Contains(@"\[!amdnew!]\"))
				{
					file = file.Replace(@"\[!amdnew!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!amdrid!]") && file.Contains(@"\[!amdrid!]\"))
				{
					file = file.Replace(@"\[!amdrid!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!nvidia!]") && file.Contains(@"\[!nvidia!]\"))
				{
					file = file.Replace(@"\[!nvidia!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!intel!]") && file.Contains(@"\[!intel!]\"))
				{
					file = file.Replace(@"\[!intel!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_nvidia!]") && file.Contains(@"\[!no_nvidia!]\"))
				{
					file = file.Replace(@"\[!no_nvidia!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_intel!]") && file.Contains(@"\[!no_intel!]\"))
				{
					file = file.Replace(@"\[!no_intel!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_amd!]") && file.Contains(@"\[!no_amd!]\"))
				{
					file = file.Replace(@"\[!no_amd!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!dwheel!]") && file.Contains(@"\[!dwheel!]\"))
				{
					file = file.Replace(@"\[!dwheel!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!dhotas!]") && file.Contains(@"\[!dhotas!]\"))
				{
					file = file.Replace(@"\[!dhotas!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!dlightgun!]") && file.Contains(@"\[!dlightgun!]\"))
				{
					file = file.Replace(@"\[!dlightgun!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!xinput!]") && file.Contains(@"\[!xinput!]\"))
				{
					file = file.Replace(@"\[!xinput!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_dwheel!]") && file.Contains(@"\[!no_dwheel!]\"))
				{
					file = file.Replace(@"\[!no_dwheel!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_dhotas!]") && file.Contains(@"\[!no_dhotas!]\"))
				{
					file = file.Replace(@"\[!no_dhotas!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_dlightgun!]") && file.Contains(@"\[!no_dlightgun!]\"))
				{
					file = file.Replace(@"\[!no_dlightgun!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_xinput!]") && file.Contains(@"\[!no_xinput!]\"))
				{
					file = file.Replace(@"\[!no_xinput!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!ffb!]") && file.Contains(@"\[!ffb!]\"))
				{
					file = file.Replace(@"\[!ffb!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!set_resolution!]") && file.Contains(@"\[!set_resolution!]\"))
				{
					file = file.Replace(@"\[!set_resolution!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!dont_set_resolution!]") && file.Contains(@"\[!dont_set_resolution!]\"))
				{
					file = file.Replace(@"\[!dont_set_resolution!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!set_displaymode_recommanded!]") && file.Contains(@"\[!set_displaymode_recommanded!]\"))
				{
					file = file.Replace(@"\[!set_displaymode_recommanded!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!set_fullscreen!]") && file.Contains(@"\[!set_fullscreen!]\"))
				{
					file = file.Replace(@"\[!set_fullscreen!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!set_windowed!]") && file.Contains(@"\[!set_windowed!]\"))
				{
					file = file.Replace(@"\[!set_windowed!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!game_native_res!]") && file.Contains(@"\[!game_native_res!]\"))
				{
					file = file.Replace(@"\[!game_native_res!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!720p!]") && file.Contains(@"\[!720p!]\"))
				{
					file = file.Replace(@"\[!720p!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!1080p!]") && file.Contains(@"\[!1080p!]\"))
				{
					file = file.Replace(@"\[!1080p!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!2k!]") && file.Contains(@"\[!2k!]\"))
				{
					file = file.Replace(@"\[!2k!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!4k!]") && file.Contains(@"\[!4k!]\"))
				{
					file = file.Replace(@"\[!4k!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_game_native_res!]") && file.Contains(@"\[!no_game_native_res!]\"))
				{
					file = file.Replace(@"\[!no_game_native_res!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_720p!]") && file.Contains(@"\[!no_720p!]\"))
				{
					file = file.Replace(@"\[!no_720p!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_1080p!]") && file.Contains(@"\[!no_1080p!]\"))
				{
					file = file.Replace(@"\[!no_1080p!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_2k!]") && file.Contains(@"\[!no_2k!]\"))
				{
					file = file.Replace(@"\[!no_2k!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_4k!]") && file.Contains(@"\[!no_4k!]\"))
				{
					file = file.Replace(@"\[!no_4k!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!720p+!]") && file.Contains(@"\[!720p+!]\"))
				{
					file = file.Replace(@"\[!720p+!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!1080p+!]") && file.Contains(@"\[!1080p+!]\"))
				{
					file = file.Replace(@"\[!1080p+!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!2k+!]") && file.Contains(@"\[!2k+!]\"))
				{
					file = file.Replace(@"\[!2k+!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!4k+!]") && file.Contains(@"\[!4k+!]\"))
				{
					file = file.Replace(@"\[!4k+!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!720p-!]") && file.Contains(@"\[!720p-!]\"))
				{
					file = file.Replace(@"\[!720p-!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!1080p-!]") && file.Contains(@"\[!1080p-!]\"))
				{
					file = file.Replace(@"\[!1080p-!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!2k-!]") && file.Contains(@"\[!2k-!]\"))
				{
					file = file.Replace(@"\[!2k-!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!4k-!]") && file.Contains(@"\[!4k-!]\"))
				{
					file = file.Replace(@"\[!4k-!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!optional_reshade!]") && file.Contains(@"\[!optional_reshade!]\"))
				{
					file = file.Replace(@"\[!optional_reshade!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_reshade!]") && file.Contains(@"\[!no_reshade!]\"))
				{
					file = file.Replace(@"\[!no_reshade!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!bezel!]") && file.Contains(@"\[!bezel!]\"))
				{
					file = file.Replace(@"\[!bezel!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_bezel!]") && file.Contains(@"\[!no_bezel!]\"))
				{
					file = file.Replace(@"\[!no_bezel!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!crtfilter!]") && file.Contains(@"\[!crtfilter!]\"))
				{
					file = file.Replace(@"\[!crtfilter!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_crtfilter!]") && file.Contains(@"\[!no_crtfilter!]\"))
				{
					file = file.Replace(@"\[!no_crtfilter!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!keepaspectratio!]") && file.Contains(@"\[!keepaspectratio!]\"))
				{
					file = file.Replace(@"\[!keepaspectratio!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_keepaspectratio!]") && file.Contains(@"\[!no_keepaspectratio!]\"))
				{
					file = file.Replace(@"\[!no_keepaspectratio!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!at_least_one_sinden!]") && file.Contains(@"\[!at_least_one_sinden!]\"))
				{
					file = file.Replace(@"\[!at_least_one_sinden!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!magpie_sinden_bezel!]") && file.Contains(@"\[!magpie_sinden_bezel!]\"))
				{
					file = file.Replace(@"\[!magpie_sinden_bezel!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_sinden!]") && file.Contains(@"\[!no_sinden!]\"))
				{
					file = file.Replace(@"\[!no_sinden!]\", @"\");
				}

				if (Path.GetDirectoryName(file).Contains(@"\[!crosshair_gun1_and_gun2!]") && file.Contains(@"\[!crosshair_gun1_and_gun2!]\"))
				{
					file = file.Replace(@"\[!crosshair_gun1_and_gun2!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!crosshair_gun1_only!]") && file.Contains(@"\[!crosshair_gun1_only!]\"))
				{
					file = file.Replace(@"\[!crosshair_gun1_only!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!crosshair_gun2_only!]") && file.Contains(@"\[!crosshair_gun2_only!]\"))
				{
					file = file.Replace(@"\[!crosshair_gun2_only!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!hide_crosshair!]") && file.Contains(@"\[!hide_crosshair!]\"))
				{
					file = file.Replace(@"\[!hide_crosshair!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!show_crosshair!]") && file.Contains(@"\[!show_crosshair!]\"))
				{
					file = file.Replace(@"\[!show_crosshair!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!guna_found!]") && file.Contains(@"\[!guna_found!]\"))
				{
					file = file.Replace(@"\[!guna_found!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!gunb_found!]") && file.Contains(@"\[!gunb_found!]\"))
				{
					file = file.Replace(@"\[!gunb_found!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_guna_found!]") && file.Contains(@"\[!no_guna_found!]\"))
				{
					file = file.Replace(@"\[!no_guna_found!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_gunb_found!]") && file.Contains(@"\[!no_gunb_found!]\"))
				{
					file = file.Replace(@"\[!no_gunb_found!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!shifter_found!]") && file.Contains(@"\[!shifter_found!]\"))
				{
					file = file.Replace(@"\[!shifter_found!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_shifter_found!]") && file.Contains(@"\[!no_shifter_found!]\"))
				{
					file = file.Replace(@"\[!no_shifter_found!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!throttle_found!]") && file.Contains(@"\[!throttle_found!]\"))
				{
					file = file.Replace(@"\[!throttle_found!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_throttle_found!]") && file.Contains(@"\[!no_throttle_found!]\"))
				{
					file = file.Replace(@"\[!no_throttle_found!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!vsync!]") && file.Contains(@"\[!vsync!]\"))
				{
					file = file.Replace(@"\[!vsync!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_vsync!]") && file.Contains(@"\[!no_vsync!]\"))
				{
					file = file.Replace(@"\[!no_vsync!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!normal_perfprofile!]") && file.Contains(@"\[!normal_perfprofile!]\"))
				{
					file = file.Replace(@"\[!normal_perfprofile!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!low_perfprofile!]") && file.Contains(@"\[!low_perfprofile!]\"))
				{
					file = file.Replace(@"\[!low_perfprofile!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!high_perfprofile!]") && file.Contains(@"\[!high_perfprofile!]\"))
				{
					file = file.Replace(@"\[!high_perfprofile!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!not_normal_perfprofile!]") && file.Contains(@"\[!not_normal_perfprofile!]\"))
				{
					file = file.Replace(@"\[!not_normal_perfprofile!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!not_low_perfprofile!]") && file.Contains(@"\[!not_low_perfprofile!]\"))
				{
					file = file.Replace(@"\[!not_low_perfprofile!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!not_high_perfprofile!]") && file.Contains(@"\[!not_high_perfprofile!]\"))
				{
					file = file.Replace(@"\[!not_high_perfprofile!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!translation!]") && file.Contains(@"\[!translation!]\"))
				{
					file = file.Replace(@"\[!translation!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!english!]") && file.Contains(@"\[!english!]\"))
				{
					file = file.Replace(@"\[!english!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!french!]") && file.Contains(@"\[!french!]\"))
				{
					file = file.Replace(@"\[!french!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_translation!]") && file.Contains(@"\[!no_translation!]\"))
				{
					file = file.Replace(@"\[!no_translation!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_english!]") && file.Contains(@"\[!no_english!]\"))
				{
					file = file.Replace(@"\[!no_english!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_french!]") && file.Contains(@"\[!no_french!]\"))
				{
					file = file.Replace(@"\[!no_french!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!120hz!]") && file.Contains(@"\[!120hz!]\"))
				{
					file = file.Replace(@"\[!120hz!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!60hz!]") && file.Contains(@"\[!60hz!]\"))
				{
					file = file.Replace(@"\[!60hz!]\", @"\");
				}
				var anyTagMatch = Regex.Match(file, @"\\\[!tag-any\{[a-zA-Z0-9+\-_]+\}!\]\\", RegexOptions.IgnoreCase);
				if (anyTagMatch.Success)
				{
					file = file.Replace(anyTagMatch.Groups[0].Value, @"\");
				}
				var allTagMatch = Regex.Match(file, @"\\\[!tag-all\{[a-zA-Z0-9+\-_]+\}!\]\\", RegexOptions.IgnoreCase);
				if (allTagMatch.Success)
				{
					file = file.Replace(allTagMatch.Groups[0].Value, @"\");
				}
				var tagMd5Match = Regex.Match(file, @"\\\[!md5-[a-zA-Z0-9]+!\]\\", RegexOptions.IgnoreCase);
				if (tagMd5Match.Success)
				{
					file = file.Replace(tagMd5Match.Groups[0].Value, @"\");
				}
				tagMd5Match = Regex.Match(file, @"\\\[!md5-([a-zA-Z0-9]+)-([a-zA-Z0-9 #-_\.|]+)!\]\\", RegexOptions.IgnoreCase);
				if (tagMd5Match.Success)
				{
					file = file.Replace(tagMd5Match.Groups[0].Value, @"\");
				}

				var tagSizeMatch = Regex.Match(file, @"\\\[!size-[0-9+]+!\]\\", RegexOptions.IgnoreCase);
				if (tagSizeMatch.Success)
				{
					file = file.Replace(tagSizeMatch.Groups[0].Value, @"\");
				}
				tagSizeMatch = Regex.Match(file, @"\\\[!size-([0-9]+)-([a-zA-Z0-9 #-_\.|]+)!\]\\", RegexOptions.IgnoreCase);
				if (tagSizeMatch.Success)
				{
					file = file.Replace(tagSizeMatch.Groups[0].Value, @"\");
				}


				if (Path.GetDirectoryName(file) != null && Regex.IsMatch(Path.GetDirectoryName(file), @"\\\[!!([A-Za-z0-9 ]+)!!\]") && Regex.IsMatch(file, @"\\\[!!([A-Za-z0-9 ]+)!!\]\\"))
				{
					file = Regex.Replace(file, @"\\\[!!([A-Za-z0-9 ]+)!!\]\\", @"\");
				}


				if (Path.GetFileNameWithoutExtension(file).StartsWith(@"[!main_executable!") && Path.GetFileNameWithoutExtension(file).EndsWith(@"]"))
				{
					if (executableGameFile != "" && File.Exists(executableGameFile))
					{
						file = executableGameFile;
					}
				}

				file = Path.GetFullPath(file);
				if (!File.Exists(file)) continue;
				if (!filesToCheck.Contains(file)) filesToCheck.Add(file);

				//Utils.LogMessage("check rights for " + file);

			}
			List<string> errorList = new List<string>();
			bool valid = true;
			foreach (var file in filesToCheck)
			{
				if (!verifyRightFile(file))
				{
					if (RemoveReadOnly(file))
					{
						if (!verifyRightFile(file))
						{
							valid = false;
							errorList.Add(file);
						}
					}
					else
					{
						valid = false;
						errorList.Add(file);
					}

				}

			}
			if (valid)
			{
				Utils.LogMessage($"Precheck {filesToCheck.Count} files, all ok");
			}
			else
			{
				Utils.LogMessage($"Precheck {filesToCheck.Count} files, error");
				MessageBox.Show($"No read/write access to {errorList.Count()} files like {errorList.First()}, patch linking will be disabled");

			}
			return valid;

		}

		public static bool verifyRightFile(string filePath)
		{
			if (File.Exists(filePath))
			{

				try
				{
					bool valid1 = false;
					bool valid2 = false;
					// Vérifier les permissions en lecture
					using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
					{
						valid1 = true;
					}

					// Vérifier les permissions en écriture
					using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Write))
					{
						valid2 = true;
					}

					if (valid1 && valid2) { return true; }
				}
				catch (UnauthorizedAccessException)
				{
					Utils.LogMessage("No rights for " + filePath);
				}
				catch (Exception ex)
				{
				}
			}
			return false;
		}

		public static bool RemoveReadOnly(string filePath)
		{
			if (!File.Exists(filePath)) return false;

			try
			{
				var attributes = File.GetAttributes(filePath);

				if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				{
					File.SetAttributes(filePath, attributes & ~FileAttributes.ReadOnly);
				}

				return true;
			}
			catch
			{
				return false;
			}
		}


		public static void CleanHardLinksFiles(string directoryToClean, string originalLinkDir, string executableGameFile)
		{
			List<string> restauredFiles = new List<string>();
			string moveToDest = "";
			bool moveNeedAdmin = false;
			string moveAhkCode = "";

			directoryToClean = Path.GetFullPath(directoryToClean);
			originalLinkDir = Path.GetFullPath(originalLinkDir);
			if (!Directory.Exists(directoryToClean)) return;
			if (!Directory.Exists(originalLinkDir)) return;

			bool CanUseHardLink = true;
			if (!AreFoldersOnSameDrive(directoryToClean, originalLinkDir)) CanUseHardLink = false;
			if (!Program.linkExeHardLink) CanUseHardLink = false;
			bool CanUseSoftLink = Program.linkExeSoftLink;
			bool CanWrite = Program.linkExeWritable;

			var filePaths = Directory.EnumerateFiles(originalLinkDir, "*", new EnumerationOptions
			{
				IgnoreInaccessible = true,
				RecurseSubdirectories = true
			});
			foreach (var fileInLinkFolder in filePaths)
			{
				string file = directoryToClean + fileInLinkFolder.Remove(0, originalLinkDir.Length).TrimStart();
				file = file.Replace("[..]", "..");


				if (Path.GetDirectoryName(file).Contains(@"\[!windowed!]") && file.Contains(@"\[!windowed!]\"))
				{
					file = file.Replace(@"\[!windowed!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!fullscreen!]") && file.Contains(@"\[!fullscreen!]\"))
				{
					file = file.Replace(@"\[!fullscreen!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!amd!]") && file.Contains(@"\[!amd!]\"))
				{
					file = file.Replace(@"\[!amd!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!amdold!]") && file.Contains(@"\[!amdold!]\"))
				{
					file = file.Replace(@"\[!amdold!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!amdnew!]") && file.Contains(@"\[!amdnew!]\"))
				{
					file = file.Replace(@"\[!amdnew!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!amdrid!]") && file.Contains(@"\[!amdrid!]\"))
				{
					file = file.Replace(@"\[!amdrid!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!nvidia!]") && file.Contains(@"\[!nvidia!]\"))
				{
					file = file.Replace(@"\[!nvidia!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!intel!]") && file.Contains(@"\[!intel!]\"))
				{
					file = file.Replace(@"\[!intel!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_nvidia!]") && file.Contains(@"\[!no_nvidia!]\"))
				{
					file = file.Replace(@"\[!no_nvidia!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_intel!]") && file.Contains(@"\[!no_intel!]\"))
				{
					file = file.Replace(@"\[!no_intel!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_amd!]") && file.Contains(@"\[!no_amd!]\"))
				{
					file = file.Replace(@"\[!no_amd!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!dwheel!]") && file.Contains(@"\[!dwheel!]\"))
				{
					file = file.Replace(@"\[!dwheel!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!dhotas!]") && file.Contains(@"\[!dhotas!]\"))
				{
					file = file.Replace(@"\[!dhotas!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!dlightgun!]") && file.Contains(@"\[!dlightgun!]\"))
				{
					file = file.Replace(@"\[!dlightgun!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!xinput!]") && file.Contains(@"\[!xinput!]\"))
				{
					file = file.Replace(@"\[!xinput!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_dwheel!]") && file.Contains(@"\[!no_dwheel!]\"))
				{
					file = file.Replace(@"\[!no_dwheel!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_dhotas!]") && file.Contains(@"\[!no_dhotas!]\"))
				{
					file = file.Replace(@"\[!no_dhotas!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_dlightgun!]") && file.Contains(@"\[!no_dlightgun!]\"))
				{
					file = file.Replace(@"\[!no_dlightgun!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_xinput!]") && file.Contains(@"\[!no_xinput!]\"))
				{
					file = file.Replace(@"\[!no_xinput!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!ffb!]") && file.Contains(@"\[!ffb!]\"))
				{
					file = file.Replace(@"\[!ffb!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!set_resolution!]") && file.Contains(@"\[!set_resolution!]\"))
				{
					file = file.Replace(@"\[!set_resolution!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!dont_set_resolution!]") && file.Contains(@"\[!dont_set_resolution!]\"))
				{
					file = file.Replace(@"\[!dont_set_resolution!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!set_displaymode_recommanded!]") && file.Contains(@"\[!set_displaymode_recommanded!]\"))
				{
					file = file.Replace(@"\[!set_displaymode_recommanded!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!set_fullscreen!]") && file.Contains(@"\[!set_fullscreen!]\"))
				{
					file = file.Replace(@"\[!set_fullscreen!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!set_windowed!]") && file.Contains(@"\[!set_windowed!]\"))
				{
					file = file.Replace(@"\[!set_windowed!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!game_native_res!]") && file.Contains(@"\[!game_native_res!]\"))
				{
					file = file.Replace(@"\[!game_native_res!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!720p!]") && file.Contains(@"\[!720p!]\"))
				{
					file = file.Replace(@"\[!720p!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!1080p!]") && file.Contains(@"\[!1080p!]\"))
				{
					file = file.Replace(@"\[!1080p!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!2k!]") && file.Contains(@"\[!2k!]\"))
				{
					file = file.Replace(@"\[!2k!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!4k!]") && file.Contains(@"\[!4k!]\"))
				{
					file = file.Replace(@"\[!4k!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_game_native_res!]") && file.Contains(@"\[!no_game_native_res!]\"))
				{
					file = file.Replace(@"\[!no_game_native_res!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_720p!]") && file.Contains(@"\[!no_720p!]\"))
				{
					file = file.Replace(@"\[!no_720p!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_1080p!]") && file.Contains(@"\[!no_1080p!]\"))
				{
					file = file.Replace(@"\[!no_1080p!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_2k!]") && file.Contains(@"\[!no_2k!]\"))
				{
					file = file.Replace(@"\[!no_2k!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_4k!]") && file.Contains(@"\[!no_4k!]\"))
				{
					file = file.Replace(@"\[!no_4k!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!720p+!]") && file.Contains(@"\[!720p+!]\"))
				{
					file = file.Replace(@"\[!720p+!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!1080p+!]") && file.Contains(@"\[!1080p+!]\"))
				{
					file = file.Replace(@"\[!1080p+!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!2k+!]") && file.Contains(@"\[!2k+!]\"))
				{
					file = file.Replace(@"\[!2k+!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!4k+!]") && file.Contains(@"\[!4k+!]\"))
				{
					file = file.Replace(@"\[!4k+!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!720p-!]") && file.Contains(@"\[!720p-!]\"))
				{
					file = file.Replace(@"\[!720p-!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!1080p-!]") && file.Contains(@"\[!1080p-!]\"))
				{
					file = file.Replace(@"\[!1080p-!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!2k-!]") && file.Contains(@"\[!2k-!]\"))
				{
					file = file.Replace(@"\[!2k-!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!4k-!]") && file.Contains(@"\[!4k-!]\"))
				{
					file = file.Replace(@"\[!4k-!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!optional_reshade!]") && file.Contains(@"\[!optional_reshade!]\"))
				{
					file = file.Replace(@"\[!optional_reshade!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_reshade!]") && file.Contains(@"\[!no_reshade!]\"))
				{
					file = file.Replace(@"\[!no_reshade!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!bezel!]") && file.Contains(@"\[!bezel!]\"))
				{
					file = file.Replace(@"\[!bezel!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_bezel!]") && file.Contains(@"\[!no_bezel!]\"))
				{
					file = file.Replace(@"\[!no_bezel!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!crtfilter!]") && file.Contains(@"\[!crtfilter!]\"))
				{
					file = file.Replace(@"\[!crtfilter!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_crtfilter!]") && file.Contains(@"\[!no_crtfilter!]\"))
				{
					file = file.Replace(@"\[!no_crtfilter!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!keepaspectratio!]") && file.Contains(@"\[!keepaspectratio!]\"))
				{
					file = file.Replace(@"\[!keepaspectratio!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_keepaspectratio!]") && file.Contains(@"\[!no_keepaspectratio!]\"))
				{
					file = file.Replace(@"\[!no_keepaspectratio!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!at_least_one_sinden!]") && file.Contains(@"\[!at_least_one_sinden!]\"))
				{
					file = file.Replace(@"\[!at_least_one_sinden!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!magpie_sinden_bezel!]") && file.Contains(@"\[!magpie_sinden_bezel!]\"))
				{
					file = file.Replace(@"\[!magpie_sinden_bezel!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_sinden!]") && file.Contains(@"\[!no_sinden!]\"))
				{
					file = file.Replace(@"\[!no_sinden!]\", @"\");
				}

				if (Path.GetDirectoryName(file).Contains(@"\[!crosshair_gun1_and_gun2!]") && file.Contains(@"\[!crosshair_gun1_and_gun2!]\"))
				{
					file = file.Replace(@"\[!crosshair_gun1_and_gun2!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!crosshair_gun1_only!]") && file.Contains(@"\[!crosshair_gun1_only!]\"))
				{
					file = file.Replace(@"\[!crosshair_gun1_only!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!crosshair_gun2_only!]") && file.Contains(@"\[!crosshair_gun2_only!]\"))
				{
					file = file.Replace(@"\[!crosshair_gun2_only!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!hide_crosshair!]") && file.Contains(@"\[!hide_crosshair!]\"))
				{
					file = file.Replace(@"\[!hide_crosshair!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!show_crosshair!]") && file.Contains(@"\[!show_crosshair!]\"))
				{
					file = file.Replace(@"\[!show_crosshair!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!guna_found!]") && file.Contains(@"\[!guna_found!]\"))
				{
					file = file.Replace(@"\[!guna_found!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!gunb_found!]") && file.Contains(@"\[!gunb_found!]\"))
				{
					file = file.Replace(@"\[!gunb_found!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_guna_found!]") && file.Contains(@"\[!no_guna_found!]\"))
				{
					file = file.Replace(@"\[!no_guna_found!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_gunb_found!]") && file.Contains(@"\[!no_gunb_found!]\"))
				{
					file = file.Replace(@"\[!no_gunb_found!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!shifter_found!]") && file.Contains(@"\[!shifter_found!]\"))
				{
					file = file.Replace(@"\[!shifter_found!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_shifter_found!]") && file.Contains(@"\[!no_shifter_found!]\"))
				{
					file = file.Replace(@"\[!no_shifter_found!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!throttle_found!]") && file.Contains(@"\[!throttle_found!]\"))
				{
					file = file.Replace(@"\[!throttle_found!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_throttle_found!]") && file.Contains(@"\[!no_throttle_found!]\"))
				{
					file = file.Replace(@"\[!no_throttle_found!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!vsync!]") && file.Contains(@"\[!vsync!]\"))
				{
					file = file.Replace(@"\[!vsync!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_vsync!]") && file.Contains(@"\[!no_vsync!]\"))
				{
					file = file.Replace(@"\[!no_vsync!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!normal_perfprofile!]") && file.Contains(@"\[!normal_perfprofile!]\"))
				{
					file = file.Replace(@"\[!normal_perfprofile!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!low_perfprofile!]") && file.Contains(@"\[!low_perfprofile!]\"))
				{
					file = file.Replace(@"\[!low_perfprofile!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!high_perfprofile!]") && file.Contains(@"\[!high_perfprofile!]\"))
				{
					file = file.Replace(@"\[!high_perfprofile!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!not_normal_perfprofile!]") && file.Contains(@"\[!not_normal_perfprofile!]\"))
				{
					file = file.Replace(@"\[!not_normal_perfprofile!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!not_low_perfprofile!]") && file.Contains(@"\[!not_low_perfprofile!]\"))
				{
					file = file.Replace(@"\[!not_low_perfprofile!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!not_high_perfprofile!]") && file.Contains(@"\[!not_high_perfprofile!]\"))
				{
					file = file.Replace(@"\[!not_high_perfprofile!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!translation!]") && file.Contains(@"\[!translation!]\"))
				{
					file = file.Replace(@"\[!translation!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!english!]") && file.Contains(@"\[!english!]\"))
				{
					file = file.Replace(@"\[!english!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!french!]") && file.Contains(@"\[!french!]\"))
				{
					file = file.Replace(@"\[!french!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_translation!]") && file.Contains(@"\[!no_translation!]\"))
				{
					file = file.Replace(@"\[!no_translation!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_english!]") && file.Contains(@"\[!no_english!]\"))
				{
					file = file.Replace(@"\[!no_english!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!no_french!]") && file.Contains(@"\[!no_french!]\"))
				{
					file = file.Replace(@"\[!no_french!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!120hz!]") && file.Contains(@"\[!120hz!]\"))
				{
					file = file.Replace(@"\[!120hz!]\", @"\");
				}
				if (Path.GetDirectoryName(file).Contains(@"\[!60hz!]") && file.Contains(@"\[!60hz!]\"))
				{
					file = file.Replace(@"\[!60hz!]\", @"\");
				}
				var anyTagMatch = Regex.Match(file, @"\\\[!tag-any\{[a-zA-Z0-9+\-_]+\}!\]\\", RegexOptions.IgnoreCase);
				if (anyTagMatch.Success)
				{
					file = file.Replace(anyTagMatch.Groups[0].Value, @"\");
				}
				var tagMd5Match = Regex.Match(file, @"\\\[!md5-[a-zA-Z0-9]+!\]\\", RegexOptions.IgnoreCase);
				if (tagMd5Match.Success)
				{
					file = file.Replace(tagMd5Match.Groups[0].Value, @"\");
				}
				tagMd5Match = Regex.Match(file, @"\\\[!md5-([a-zA-Z0-9]+)-([a-zA-Z0-9 #-_\.|]+)!\]\\", RegexOptions.IgnoreCase);
				if (tagMd5Match.Success)
				{
					file = file.Replace(tagMd5Match.Groups[0].Value, @"\");
				}
				var tagSizeMatch = Regex.Match(file, @"\\\[!size-[0-9+]+!\]\\", RegexOptions.IgnoreCase);
				if (tagSizeMatch.Success)
				{
					file = file.Replace(tagSizeMatch.Groups[0].Value, @"\");
				}
				tagSizeMatch = Regex.Match(file, @"\\\[!size-([0-9]+)-([a-zA-Z0-9 #-_\.|]+)!\]\\", RegexOptions.IgnoreCase);
				if (tagSizeMatch.Success)
				{
					file = file.Replace(tagSizeMatch.Groups[0].Value, @"\");
				}

				if (Path.GetDirectoryName(file) != null && Regex.IsMatch(Path.GetDirectoryName(file), @"\\\[!!([A-Za-z0-9 ]+)!!\]") && Regex.IsMatch(file, @"\\\[!!([A-Za-z0-9 ]+)!!\]\\"))
				{
					file = Regex.Replace(file, @"\\\[!!([A-Za-z0-9 ]+)!!\]\\", @"\");
				}



				/*
				file = Path.GetFullPath(file);
				if (Path.GetDirectoryName(file).Contains(@"\[!moveto!]") && file.Contains(@"\[!moveto!]\"))
				{
					string fileName = Path.GetFileName(fileInLinkFolder);
					if (moveToDest == "")
					{
						string dir = Path.GetDirectoryName(fileInLinkFolder);
						string destFile = Path.Combine(dir, "destination.txt");
						if (File.Exists(destFile))
						{
							string moveToDestContent = File.ReadAllText(destFile);
							moveToDestContent = moveToDestContent.Trim().TrimEnd('\r').TrimEnd('\n').TrimEnd('\r').Trim();

							if (Directory.Exists(moveToDestContent))
							{
								moveToDest = moveToDestContent;
								moveNeedAdmin = !Utils.HasWritePermissionOnDir(moveToDest);
							}
						}
					}
					if (fileName != "destination.txt" && moveToDest != "")
					{
						string dest = Path.Combine(moveToDest, fileName);
						if (File.Exists(dest))
						{
							try
							{
								File.Copy(dest, fileInLinkFolder, true);
							}
							catch { }
							if (moveNeedAdmin)
							{
								moveAhkCode += @$"FileDelete, {dest}" + "\n";
							}
							else
							{
								try
								{
									File.Delete(dest);
								}
								catch { }
							}

						}
					}
					continue;
					//file = file.Replace(@"\[!intel!]\", @"\");
				}
				*/

				if (!File.Exists(file)) continue;

				if (CanUseHardLink && IsHardLink(file, originalLinkDir) && !restauredFiles.Contains(file))
				{
					if (Program.DebugMode) Utils.LogMessage($"{file} is Hardlink, delete it");
					try
					{
						File.Delete(file);
					}
					catch (Exception ex)
					{
						if (File.Exists(file))
						{
							try
							{
								FileInfo finfo = new FileInfo(file);
								if (finfo.IsReadOnly)
								{
									finfo.IsReadOnly = false;
									finfo.Delete();
								}
							}
							catch (Exception ex2) { }
						}

					}
				}

				if (CanUseSoftLink && IsSoftLink(file, originalLinkDir) && !restauredFiles.Contains(file))
				{
					if (Program.DebugMode) Utils.LogMessage($"{file} is SoftLink, delete it");
					try
					{
						File.Delete(file);
					}
					catch (Exception ex)
					{
						if (File.Exists(file))
						{
							try
							{
								FileInfo finfo = new FileInfo(file);
								if (finfo.IsReadOnly)
								{
									finfo.IsReadOnly = false;
									finfo.Delete();
								}
							}
							catch (Exception ex2) { }
						}

					}
				}

				string fileToRestore = file + ".filetorestore";
				if (File.Exists(fileToRestore))
				{
					if (Program.DebugMode) Utils.LogMessage($"{file} must be restored");
					string newFilePath = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(fileToRestore));
					if (File.Exists(newFilePath)) File.Delete(newFilePath);
					File.Move(fileToRestore, newFilePath, true);
					restauredFiles.Add(newFilePath);
				}
			}

			if (executableGameFile != "" && File.Exists(executableGameFile))
			{
				string file = executableGameFile;
				if (CanUseHardLink && IsHardLink(file, originalLinkDir))
				{
					if (Program.DebugMode) Utils.LogMessage($"{file} is Hardlink, delete it");
					try
					{
						File.Delete(file);
					}
					catch (Exception ex)
					{
						if (File.Exists(file))
						{
							try
							{
								FileInfo finfo = new FileInfo(file);
								if (finfo.IsReadOnly)
								{
									finfo.IsReadOnly = false;
									finfo.Delete();
								}
							}
							catch (Exception ex2) { }
						}

					}
				}
				if (CanUseSoftLink && IsSoftLink(file, originalLinkDir))
				{
					if (Program.DebugMode) Utils.LogMessage($"{file} is SoftLink, delete it");
					try
					{
						File.Delete(file);
					}
					catch (Exception ex)
					{
						if (File.Exists(file))
						{
							try
							{
								FileInfo finfo = new FileInfo(file);
								if (finfo.IsReadOnly)
								{
									finfo.IsReadOnly = false;
									finfo.Delete();
								}
							}
							catch (Exception ex2) { }
						}

					}
				}
				string fileToRestore = file + ".filetorestore";
				if (File.Exists(fileToRestore))
				{
					if (Program.DebugMode) Utils.LogMessage($"{file} must be restored");
					string newFilePath = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(fileToRestore));
					if (File.Exists(newFilePath)) File.Delete(newFilePath);
					File.Move(fileToRestore, newFilePath, true);
				}
			}

			string tempDirFile = Path.Combine(directoryToClean, "tempDirList.json");
			if (File.Exists(tempDirFile))
			{
				try
				{
					List<string> DirList = (List<string>)JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(tempDirFile));
					//DirList.Reverse();
					foreach (var dir in DirList)
					{
						CleanTemporaryDirectories(dir);
					}
				}
				catch { }
				File.Delete(tempDirFile);
			}

			foreach (var fileToMoveBack in Program.movetoFiles)
			{
				if (File.Exists(fileToMoveBack.Key))
				{
					try
					{
						File.Copy(fileToMoveBack.Key, fileToMoveBack.Value, true);
						Utils.LogMessage($"CopyBack [!moveto!] {fileToMoveBack.Key} => {fileToMoveBack.Value}");
					}
					catch { }
				}
			}

			/*
			if (moveAhkCode != "")
			{
				File.WriteAllText(Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "moveadmin.tmp.ahk"), moveAhkCode);
				string selfExe = Process.GetCurrentProcess().MainModule.FileName;
				if (!Utils.CheckTaskExist(selfExe, "--moveadmin"))
				{
					string exePath = selfExe;
					string exeDir = Path.GetDirectoryName(exePath);
					Process process = new Process();
					process.StartInfo.FileName = selfExe;
					process.StartInfo.Arguments = "--registerTask " + $"\"{selfExe}\" " + "--moveadmin";
					process.StartInfo.WorkingDirectory = exeDir;
					process.StartInfo.UseShellExecute = true;
					process.StartInfo.Verb = "runas";
					process.Start();
					process.WaitForExit();
				}
				Utils.ExecuteTask(Utils.ExeToTaskName(selfExe, "--moveadmin"), -1);

			}
			*/

		}

		public static void CleanTemporaryDirectories(string rootDirectory)
		{
			try
			{
				// Vérifie si le répertoire racine existe
				if (!Directory.Exists(rootDirectory))
				{
					Console.WriteLine($"Le répertoire {rootDirectory} n'existe pas.");
					return;
				}

				try
				{
					bool isEmpty = Directory.GetFiles(rootDirectory, "*", SearchOption.AllDirectories).Length == 0;
					if (isEmpty)
					{
						Console.WriteLine($"Delete {rootDirectory}");
						Directory.Delete(rootDirectory, true);
					}
				}
				catch (Exception ex)
				{
				}

			}
			catch (Exception ex)
			{
			}
		}

		public static string FindAnyFile(string rootDirectory)
		{
			if (!Directory.Exists(rootDirectory))
				return "";

			// Utiliser une queue pour explorer les répertoires de manière itérative
			var directoriesToExplore = new Queue<string>();
			directoriesToExplore.Enqueue(rootDirectory);

			while (directoriesToExplore.Count > 0)
			{
				string currentDirectory = directoriesToExplore.Dequeue();

				try
				{
					// Recherche des fichiers dans le répertoire actuel
					foreach (var file in Directory.EnumerateFiles(currentDirectory, "*"))
					{
						// Retourner dès qu'un fichier est trouvé
						return file;
					}

					// Ajouter les sous-répertoires à la queue pour exploration
					foreach (var directory in Directory.EnumerateDirectories(currentDirectory))
					{
						directoriesToExplore.Enqueue(directory);
					}
				}
				catch (UnauthorizedAccessException)
				{
					// Gérer les exceptions d'accès refusé en continuant à explorer les autres répertoires
					Console.WriteLine($"Access denied to directory: {currentDirectory}");
				}
				catch (IOException ex)
				{
					// Gérer les exceptions d'entrée/sortie en continuant à explorer les autres répertoires
					Console.WriteLine($"IO Exception in directory {currentDirectory}: {ex.Message}");
				}
			}

			// Aucun fichier trouvé
			return "";
		}

		public static bool HasWritePermissionOnDir(string path)
		{
			try
			{
				// Get the current user's identity
				WindowsIdentity currentUser = WindowsIdentity.GetCurrent();
				WindowsPrincipal principal = new WindowsPrincipal(currentUser);

				// Get the directory security info
				DirectoryInfo directoryInfo = new DirectoryInfo(path);
				DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();

				// Get access rules
				AuthorizationRuleCollection rules = directorySecurity.GetAccessRules(true, true, typeof(SecurityIdentifier));

				foreach (FileSystemAccessRule rule in rules)
				{
					// Check if the current user is in the rule identity
					if (principal.IsInRole(rule.IdentityReference as SecurityIdentifier))
					{
						// Check for write permissions
						if ((rule.FileSystemRights & FileSystemRights.WriteData) == FileSystemRights.WriteData)
						{
							if (rule.AccessControlType == AccessControlType.Allow)
							{
								return true;
							}
							else if (rule.AccessControlType == AccessControlType.Deny)
							{
								return false;
							}
						}
					}
				}

				// If no explicit rules found, assume no write permissions
				return false;
			}
			catch (UnauthorizedAccessException)
			{
				return false;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred: {ex.Message}");
				return false;
			}
		}


		public static bool AHKSyntaxCheck(string ahkCode, out string errorTxt, string[] args = null)
		{
			errorTxt = "";

			string currentDir = Path.GetFullPath(Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
			string ahkExe = Path.Combine(currentDir, "AutoHotkeyU32.exe");
			if (!File.Exists(ahkExe)) return true;

			// Obtenez le chemin du répertoire temporaire
			string tempDirectory = Path.GetTempPath();
			string tempFileName = Path.GetRandomFileName() + ".ahk";
			string tempFilePath = Path.Combine(tempDirectory, tempFileName);

			if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
			try
			{
				// Create a temporary file to store the AHK code

				File.WriteAllText(tempFilePath, ahkCode);

				// Create a process to run AutoHotkey and capture standard error
				Process process = new Process();
				process.StartInfo.FileName = ahkExe;
				process.StartInfo.Arguments = $"/iLib nul /ErrorStdOut \"{tempFilePath}\"";
				process.StartInfo.RedirectStandardError = true;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

				process.Start();

				// Read and print the standard error output
				errorTxt = process.StandardError.ReadToEnd();

				int index = errorTxt.IndexOf("\n");
				if (index != -1) errorTxt = errorTxt.Substring(index);

				process.WaitForExit();

				// Delete the temporary file
				File.Delete(tempFilePath);

				// Check if the exit code is 0 (indicating successful syntax check)
				if (process.ExitCode == 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public static void ExecuteAHK(string ahkCode, bool waitForExit, string workingdir = "", string repertoireLinkExe = "")
		{
			try
			{
				string currentDir = Path.GetFullPath(Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
				string ahkExe = Path.Combine(currentDir, "AutoHotkeyU32.exe");
				string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName() + ".ahk");
				File.WriteAllText(tempFilePath, ahkCode);
				Thread.Sleep(100);
				if (File.Exists(tempFilePath))
				{
					Process process = new Process();
					process.StartInfo.FileName = ahkExe;
					if (repertoireLinkExe != "") process.StartInfo.Arguments = $"{tempFilePath} \"{repertoireLinkExe}\"";
					else process.StartInfo.Arguments = tempFilePath;
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.CreateNoWindow = true;
					if (workingdir != "") process.StartInfo.WorkingDirectory = workingdir;

					// Démarrer le processus
					process.Start();
					if (waitForExit)
					{
						process.WaitForExit();
						Thread.Sleep(100);
						File.Delete(tempFilePath);
					}
					else
					{
						Thread.Sleep(100);
						Program.ProcessToKill.Add(process.Id);
						Program.FilesToDelete.Add(tempFilePath);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public static void KillProcessById(int processId)
		{
			ProcessStartInfo psi = new ProcessStartInfo
			{
				FileName = "taskkill",
				Arguments = $"/F /PID {processId}",
				CreateNoWindow = true,
				UseShellExecute = false
			};

			Process.Start(psi);
		}

		public static void CleanAndKillAhk()
		{
			foreach (var ahkpid in Program.ProcessToKill)
			{
				KillProcessById(ahkpid);
			}
			Thread.Sleep(100);
			foreach (var ahkfile in Program.FilesToDelete)
			{
				if (File.Exists(ahkfile))
				{
					File.Delete(ahkfile);
				}
			}
		}

		public static string GetExactPathName(string path)
		{
			if (!File.Exists(path) && !Directory.Exists(path))
			{
				throw new FileNotFoundException("The specified path does not exist.");
			}

			DirectoryInfo dirInfo = new DirectoryInfo(path);
			if (dirInfo.Parent == null)
			{
				return dirInfo.Name;
			}

			return Path.Combine(GetExactPathName(dirInfo.Parent.FullName), dirInfo.Parent.GetFileSystemInfos(dirInfo.Name)[0].Name);
		}

		public static bool CheckTaskExist(string taskName)
		{
			using (TaskService taskService = new TaskService())
			{
				var task = taskService.GetTask(taskName);
				if (task == null)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}

		public static bool CheckTaskExist(string taskExe, string taskArgument)
		{
			string taskName = ExeToTaskName(taskExe, taskArgument);
			return CheckTaskExist(taskName);
		}

		public static void RegisterTask(string taskExe, string taskArgument)
		{
			string taskName = ExeToTaskName(taskExe, taskArgument);
			using (TaskService ts = new TaskService())
			{
				if (ts.GetTask(taskName) == null)
				{
					var UsersRights = TaskLogonType.InteractiveToken;
					TaskDefinition td = ts.NewTask();
					td.RegistrationInfo.Description = "Task as admin";
					td.Principal.RunLevel = TaskRunLevel.Highest;
					td.Principal.LogonType = UsersRights;
					// Create an action that will launch Notepad whenever the trigger fires
					td.Actions.Add(taskExe, taskArgument, null);
					// Register the task in the root folder
					ts.RootFolder.RegisterTaskDefinition(taskName, td, TaskCreation.CreateOrUpdate, Environment.GetEnvironmentVariable("USERNAME"), null, UsersRights, null);
				}
			}
		}

		public static void RegisterUserTaskAtLogon(string taskExe, string taskArgument)
		{
			string taskName = ExeToTaskName(taskExe, taskArgument);
			using (TaskService ts = new TaskService())
			{
				if (ts.GetTask(taskName) == null)
				{
					var UsersRights = TaskLogonType.InteractiveToken;
					TaskDefinition td = ts.NewTask();
					td.RegistrationInfo.Description = "User Task at Logon";
					td.Principal.LogonType = UsersRights;
					td.Principal.UserId = Environment.UserName;

					// Crée un déclencheur qui démarre la tâche à l'ouverture de session
					td.Triggers.Add(new LogonTrigger());

					// Crée une action qui lancera le programme souhaité
					td.Actions.Add(taskExe, taskArgument, null);

					// Enregistre la tâche dans le dossier de l'utilisateur
					ts.RootFolder.RegisterTaskDefinition(taskName, td, TaskCreation.CreateOrUpdate, Environment.UserName, null, UsersRights, null);
				}
			}
		}

		public static string ExeToTaskName(string exeFile, string arguments)
		{
			var argsReformated = CommandLineToArgs(arguments);
			arguments = ArgsToCommandLine(argsReformated);
			using (MD5 md5 = MD5.Create())
			{
				byte[] inputBytes = Encoding.UTF8.GetBytes(exeFile.ToLower() + " " + arguments.ToLower());
				byte[] hashBytes = md5.ComputeHash(inputBytes);
				string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
				return "TPAutoXinput_" + hashString;
			}
		}

		public static string ArgsToCommandLine(string[] arguments)
		{
			var sb = new StringBuilder();
			foreach (string argument in arguments)
			{
				bool needsQuoting = argument.Any(c => char.IsWhiteSpace(c) || c == '\"');
				if (!needsQuoting)
				{
					sb.Append(argument);
				}
				else
				{
					sb.Append('\"');
					foreach (char c in argument)
					{
						int backslashes = 0;
						while (backslashes < argument.Length && argument[backslashes] == '\\')
						{
							backslashes++;
						}
						if (c == '\"')
						{
							sb.Append('\\', backslashes * 2 + 1);
							sb.Append(c);
						}
						else if (c == '\0')
						{
							sb.Append('\\', backslashes * 2);
							break;
						}
						else
						{
							sb.Append('\\', backslashes);
							sb.Append(c);
						}
					}
					sb.Append('\"');
				}
				sb.Append(' ');
			}
			return sb.ToString().TrimEnd();
		}

		public static string[] ArgsWithoutFirstElement(string[] args)
		{
			string[] filteredArgs;
			if (args.Length > 1)
			{
				filteredArgs = new string[args.Length - 1];

				for (int i = 1; i < args.Length; i++)
				{
					filteredArgs[i - 1] = args[i];
				}
			}
			else
			{
				filteredArgs = new string[0];
			}
			return filteredArgs;
		}

		public static bool IsUserAdministrator()
		{
			bool isAdmin;
			try
			{
				WindowsIdentity user = WindowsIdentity.GetCurrent();
				WindowsPrincipal principal = new WindowsPrincipal(user);
				isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
			}
			catch (UnauthorizedAccessException ex)
			{
				isAdmin = false;
			}
			catch (Exception ex)
			{
				isAdmin = false;
			}
			return isAdmin;
		}

		public static string[] CommandLineToArgs(string commandLine, bool addfakeexe = true)
		{
			string executableName;
			return CommandLineToArgs(commandLine, out executableName, addfakeexe);
		}

		public static string[] CommandLineToArgs(string commandLine, out string executableName, bool addfakeexe = true)
		{
			if (addfakeexe) commandLine = "test.exe " + commandLine;
			int argCount;
			IntPtr result;
			string arg;
			IntPtr pStr;
			result = CommandLineToArgvW(commandLine, out argCount);
			if (result == IntPtr.Zero)
			{
				throw new System.ComponentModel.Win32Exception();
			}
			else
			{
				try
				{
					// Jump to location 0*IntPtr.Size (in other words 0).  
					pStr = Marshal.ReadIntPtr(result, 0 * IntPtr.Size);
					executableName = Marshal.PtrToStringUni(pStr);
					// Ignore the first parameter because it is the application   
					// name which is not usually part of args in Managed code.   
					string[] args = new string[argCount - 1];
					for (int i = 0; i < args.Length; i++)
					{
						pStr = Marshal.ReadIntPtr(result, (i + 1) * IntPtr.Size);
						arg = Marshal.PtrToStringUni(pStr);
						args[i] = arg;
					}
					return args;
				}
				finally
				{
					Marshal.FreeHGlobal(result);
				}
			}
		}

		public static void ExecuteTask(string taskName, int delay = 2000)
		{
			string argTask = $@"/I /run /tn ""{taskName}""";



			Process process = new Process();
			process.StartInfo.FileName = "schtasks";
			process.StartInfo.Arguments = argTask;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			process.Start();

			if (delay == -1) return;
			TaskService ts = new TaskService();
			Microsoft.Win32.TaskScheduler.Task task = ts.GetTask(taskName);
			Microsoft.Win32.TaskScheduler.RunningTaskCollection instances = task.GetInstances();

			//Code a enlever si execution sans attente
			int nbrun = delay / 100;
			if (instances.Count == 0)
			{
				instances = task.GetInstances();
				Thread.Sleep(100);
				int i = 0;
				while (instances.Count == 0)
				{
					i++;
					instances = task.GetInstances();
					Thread.Sleep(100);
					if (i > nbrun) break;
				}
			}
			while (instances.Count == 1)
			{
				instances = task.GetInstances();
				Thread.Sleep(100);
			}

		}

		public static bool IsEligibleHardLink(string path)
		{
			if (!Directory.Exists(path)) return false;

			bool isWindows10OrNewer = (Environment.OSVersion.Version.Major >= 10);
			if (!isWindows10OrNewer) return false;

			try
			{
				DriveInfo driveInfo = new DriveInfo(Path.GetPathRoot(path));
				return driveInfo.DriveFormat.Equals("NTFS", StringComparison.OrdinalIgnoreCase);
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public static bool IsEligibleHardLink(string source, string dest, bool checkExist = true)
		{
			if (string.IsNullOrEmpty(source)) return false;
			if (string.IsNullOrEmpty(dest)) return false;

			source = Path.GetFullPath(source);
			dest = Path.GetFullPath(dest);

			if (checkExist)
			{
				if (!Directory.Exists(source)) return false;
				if (!Directory.Exists(dest)) return false;
			}


			string drive1 = Path.GetPathRoot(source);
			string drive2 = Path.GetPathRoot(dest);

			bool sameDrive = string.Equals(drive1, drive2, StringComparison.OrdinalIgnoreCase);
			if (!sameDrive) return false;

			bool isWindows10OrNewer = (Environment.OSVersion.Version.Major >= 10);
			if (!isWindows10OrNewer) return false;

			try
			{
				DriveInfo driveInfo = new DriveInfo(Path.GetPathRoot(source));
				if (!driveInfo.DriveFormat.Equals("NTFS", StringComparison.OrdinalIgnoreCase)) return false;
			}
			catch (Exception ex)
			{
				return false;
			}

			try
			{
				DriveInfo driveInfo = new DriveInfo(Path.GetPathRoot(dest));
				if (!driveInfo.DriveFormat.Equals("NTFS", StringComparison.OrdinalIgnoreCase)) return false;
			}
			catch (Exception ex)
			{
				return false;
			}

			return true;
		}

		public static void LogMessage(string message)
		{
			if (Program.DebugMode)
			{
				// Format [Date-Heure] Texte
				string logEntry = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {message}";

				Console.WriteLine(logEntry);
				// Écrire dans le fichier de journal
				// File.AppendAllText("debug.log.txt", logEntry + Environment.NewLine);
			}

		}


		public static bool AreFoldersOnSameDrive(string path1, string path2)
		{
			string drive1 = Path.GetPathRoot(path1);
			string drive2 = Path.GetPathRoot(path2);

			return string.Equals(drive1, drive2, StringComparison.OrdinalIgnoreCase);
		}

		public static string ProcessExist(string exeName)
		{
			foreach (var process in Process.GetProcesses())
			{
				if (process.ProcessName.Contains(exeName))
				{
					bool is64bit = false;
					try
					{
						IsWow64Process(process.Handle, out is64bit);
					}
					catch { }
					if (is64bit) return "64";
					return "32";
				}
			}
			return "";
		}

		public static bool Is64Bit(Process process)
		{
			if (!Environment.Is64BitOperatingSystem)
				return false;
			// if this method is not available in your version of .NET, use GetNativeSystemInfo via P/Invoke instead

			bool isWow64;
			if (!IsWow64Process(process.Handle, out isWow64))
				throw new Win32Exception();
			return !isWow64;
		}

		public static IntPtr FindWindowByMultipleCriteria(string windowClass, string exeName, string windowTitle)
		{
			foreach (var process in Process.GetProcesses())
			{
				if (!string.IsNullOrEmpty(exeName) && !process.ProcessName.Contains(exeName))
					continue;

				IntPtr windowHandle = process.MainWindowHandle;
				if (windowHandle == IntPtr.Zero)
					continue;

				StringBuilder className = new StringBuilder(256);
				GetClassName(windowHandle, className, className.Capacity);

				if (!string.IsNullOrEmpty(windowClass) && !className.ToString().Contains(windowClass))
					continue;

				StringBuilder title = new StringBuilder(256);
				GetWindowText(windowHandle, title, title.Capacity);

				if (!string.IsNullOrEmpty(windowTitle) && !title.ToString().Contains(windowTitle))
					continue;

				return windowHandle;
			}

			return IntPtr.Zero;
		}

		public static IntPtr FindWindowByMultipleCriteria(string windowClass, string exeName, string windowTitle, out string trueClassTitle)
		{
			foreach (var process in Process.GetProcesses())
			{
				if (!string.IsNullOrEmpty(exeName) && !process.ProcessName.Contains(exeName))
					continue;

				IntPtr windowHandle = process.MainWindowHandle;
				if (windowHandle == IntPtr.Zero)
					continue;

				StringBuilder className = new StringBuilder(256);
				GetClassName(windowHandle, className, className.Capacity);

				if (!string.IsNullOrEmpty(windowClass) && !className.ToString().Contains(windowClass))
					continue;

				StringBuilder title = new StringBuilder(256);
				GetWindowText(windowHandle, title, title.Capacity);

				if (!string.IsNullOrEmpty(windowTitle) && !title.ToString().Contains(windowTitle))
					continue;

				trueClassTitle = className.ToString();
				return windowHandle;
			}
			trueClassTitle = "";
			return IntPtr.Zero;
		}


		public static void MoveWindowsToZero(IntPtr hWnd)
		{
			int compteur = 0;

			RECT windowRect;
			RECT clientRect;
			Utils.GetWindowRect(hWnd, out windowRect);
			Utils.GetClientRect(hWnd, out clientRect);

			int winWidth = (windowRect.Right - windowRect.Left);
			int clientWidth = (clientRect.Right - clientRect.Left);

			int winHeight = (windowRect.Bottom - windowRect.Top);
			int clientHeight = (clientRect.Bottom - clientRect.Top);
			Utils.LogMessage($"{clientWidth}x{clientHeight}");
			while ((clientWidth < 150 || clientHeight < 100) && compteur < 50)
			{
				compteur++;
				Thread.Sleep(200);
				Utils.GetWindowRect(hWnd, out windowRect);
				Utils.GetClientRect(hWnd, out clientRect);
				winWidth = (windowRect.Right - windowRect.Left);
				clientWidth = (clientRect.Right - clientRect.Left);
				winHeight = (windowRect.Bottom - windowRect.Top);
				clientHeight = (clientRect.Bottom - clientRect.Top);
				Utils.LogMessage($"{clientWidth}x{clientHeight}");
			}


			int borderSizeWidth = (int)Math.Floor(((double)winWidth - (double)clientWidth) / 2.0);
			int diffHeight = (winHeight - clientHeight);

			//Screen screen = Screen.FromHandle(hWnd);
			Screen screen = Screen.PrimaryScreen;
			if (Program.moveWindowToOriginalMonitor)
			{
				foreach (var s in Screen.AllScreens)
				{
					if (s.DeviceName == Program.originalMonitorDeviceName)
					{
						screen = s;
						Utils.LogMessage($"Move back to {Program.originalMonitorDeviceName}");
					}
				}
			}

			//int style = GetWindowLong(hWnd, GWL_STYLE);
			//SetWindowLong(hWnd, GWL_STYLE, style & ~WS_CAPTION & ~WS_SYSMENU);


			if (winWidth <= screen.Bounds.Width && winHeight <= screen.Bounds.Height)
			{
				Utils.LogMessage("Try move window1");
				SetWindowPos(hWnd, IntPtr.Zero, screen.Bounds.X, screen.Bounds.Y, 0, 0, 0x0001 | 0x0004 | 0x0010);
			}
			else
			{
				Utils.LogMessage("Try move window2");
				SetWindowPos(hWnd, IntPtr.Zero, screen.Bounds.X - (borderSizeWidth), screen.Bounds.Y - (diffHeight - borderSizeWidth), 0, 0, 0x0001 | 0x0004 | 0x0010);
			}

		}

		public static void ClickWindow(IntPtr windowHandle)
		{
			RECT clientRect;
			// Obtenir les dimensions du client (zone de rendu) de la fenêtre
			GetClientRect(windowHandle, out clientRect);

			// Convertir les coordonnées client en coordonnées écran
			POINTCLICK upperRightCorner = new POINTCLICK();
			upperRightCorner.X = (int)(clientRect.Right * 0.95); // 5% du bord droit
			upperRightCorner.Y = (int)(clientRect.Top * 0.95);

			ClientToScreen(windowHandle, ref upperRightCorner);

			// Déplacer la souris au coin supérieur droit
			SetCursorPos(upperRightCorner.X, upperRightCorner.Y);

			// Envoyer un message de clic de souris (clic droit) à la fenêtre cible
			SendMessage(windowHandle, WM_LBUTTONDOWN, 0, MakeLParam(upperRightCorner.X, upperRightCorner.Y));
			SendMessage(windowHandle, WM_LBUTTONUP, 0, MakeLParam(upperRightCorner.X, upperRightCorner.Y));
		}

		public static void ClickOnPrimaryScreen(int x, int y)
		{

			// Obtenir l'écran principal
			Screen primaryScreen = Screen.PrimaryScreen;

			// Calculer les coordonnées absolues en fonction de l'écran principal
			int absX = primaryScreen.Bounds.Left + x;
			int absY = primaryScreen.Bounds.Top + y;

			// Déplacer la souris aux coordonnées spécifiées
			SetCursorPos(absX, absY);

			// Simuler un clic gauche
			mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, absX, absY, 0, 0);
		}

		public static string GetForegroundClassName()
		{
			// Récupérer le handle de la fenêtre avec le focus
			IntPtr foregroundWindowHandle = GetForegroundWindow();

			// Vérifier si le handle est valide
			if (foregroundWindowHandle != IntPtr.Zero)
			{
				// Obtenir le nom de la classe de la fenêtre avec le focus
				StringBuilder className = new StringBuilder(256);
				GetClassName(foregroundWindowHandle, className, className.Capacity);

				// Retourner le nom de la classe de la fenêtre
				return className.ToString();
			}
			else
			{
				return "Aucune fenêtre avec le focus n'a été trouvée.";
			}
		}



		public static System.Net.IPAddress GetBroadcastAddress(System.Net.IPAddress address, System.Net.IPAddress subnetMask)
		{
			byte[] ipAdressBytes = address.GetAddressBytes();
			byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

			if (ipAdressBytes.Length != subnetMaskBytes.Length)
				throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

			byte[] broadcastAddress = new byte[ipAdressBytes.Length];
			for (int i = 0; i < broadcastAddress.Length; i++)
			{
				broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
			}
			return new System.Net.IPAddress(broadcastAddress);
		}

		public static async Task<NetworkInterface> GetPrimaryNetworkAdapterAsync()
		{
			try
			{
				NetworkInterface[] networks = NetworkInterface.GetAllNetworkInterfaces();

				foreach (var adapter in networks)
				{
					if (adapter.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
						adapter.NetworkInterfaceType != NetworkInterfaceType.Tunnel &&
						adapter.OperationalStatus == OperationalStatus.Up &&
						!adapter.Name.StartsWith("vEthernet"))
					{
						var ipProperties = adapter.GetIPProperties();
						var ipv4Address = ipProperties.UnicastAddresses
							.FirstOrDefault(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

						if (ipv4Address != null && await IsConnectedToInternet(adapter))
						{
							return adapter;
						}
					}
				}
			}
			catch (Exception ex)
			{ }
			return null;
		}

		public static async Task<bool> IsConnectedToInternet(NetworkInterface adapter)
		{
			try
			{
				using (Ping ping = new Ping())
				{
					// Ping Google's DNS server
					PingReply reply = await ping.SendPingAsync("8.8.8.8", 1000); // 5-second timeout

					return reply.Status == IPStatus.Success;
				}
			}
			catch
			{
				return false;
			}
		}

		public static Dictionary<string, string> GetFirstNetworkAdapterInfo()
		{
			Dictionary<string, string> results = new Dictionary<string, string>();
			NetworkInterface activeAdapter = null;
			System.Threading.Tasks.Task.Run(async () =>
			{
				NetworkInterface primaryAdapter = await GetPrimaryNetworkAdapterAsync();
				if (primaryAdapter != null)
				{
					activeAdapter = primaryAdapter;
				}
			}).GetAwaiter().GetResult();

			if (activeAdapter != null)
			{
				var ipProperties = activeAdapter.GetIPProperties();
				var unicastAddress = ipProperties.UnicastAddresses
					.FirstOrDefault(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

				if (unicastAddress != null)
				{
					var ipAddress = unicastAddress.Address;
					var subnetMask = unicastAddress.IPv4Mask;
					var gatewayAddress = ipProperties.GatewayAddresses
						.FirstOrDefault(g => g.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.Address;
					var dnsAddresses = ipProperties.DnsAddresses
						.Where(dns => dns.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
						.ToArray();

					var broadcastAddress = GetBroadcastAddress(ipAddress, subnetMask);

					results.Add("networkGateway", gatewayAddress?.ToString());
					results.Add("networkIP", ipAddress.ToString());
					results.Add("networkMask", subnetMask.ToString());
					results.Add("BroadcastAddress", broadcastAddress.ToString());

					if (dnsAddresses.Length > 0)
					{
						results.Add("networkDns1", dnsAddresses[0].ToString());
					}
					if (dnsAddresses.Length > 1)
					{
						results.Add("networkDns2", dnsAddresses[1].ToString());
					}
				}
			}

			return results;
		}

		public static string Encrypt(string plainText)
		{
			string base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
			return ApplyRot13(base64Encoded);
		}

		public static string Decrypt(string encryptedText)
		{
			string rot13Decoded = ApplyRot13(encryptedText);
			return Encoding.UTF8.GetString(Convert.FromBase64String(rot13Decoded));
		}

		public static string ApplyRot13(string input)
		{
			char[] array = input.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				int number = (int)array[i];
				if (number >= 'a' && number <= 'z')
				{
					if (number > 'm')
						number -= 13;
					else
						number += 13;
				}
				else if (number >= 'A' && number <= 'Z')
				{
					if (number > 'M')
						number -= 13;
					else
						number += 13;
				}
				array[i] = (char)number;
			}
			return new string(array);
		}

		public static string GetMd5HashAsString(string FileName)
		{
			FileName = Path.GetFullPath(FileName);

			FileInfo fileInfo = new FileInfo(FileName);
			string fileIdentifier = $"{fileInfo.LastWriteTimeUtc:yyyy-MM-ddTHH:mm:ssZ}{fileInfo.Length}{fileInfo.FullName}";

			if (md5Cache.ContainsKey(fileIdentifier)) return md5Cache[fileIdentifier];

			if (File.Exists(FileName))
			{
				using (var md5 = MD5.Create())
				{
					using (var stream = File.OpenRead(FileName))
					{
						var hash = md5.ComputeHash(stream);
						string md5val = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant().ToUpper();
						md5Cache[fileIdentifier] = md5val;
						return md5val;
					}
				}
			}
			return "";
		}

		public static string GetMd5HashAsString(string FileName, bool useCache)
		{
			if (!useCache) return GetMd5HashAsString(FileName);
			else
			{
				string CacheDir = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "cachemd5");
				string fileId = GetFileIdentifier(FileName);
				if (File.Exists(Path.Combine(CacheDir, fileId)))
				{
					return File.ReadAllText(Path.Combine(Path.Combine(CacheDir, fileId)));
				}
				else
				{
					string md5 = GetMd5HashAsString(FileName);
					File.WriteAllText(Path.Combine(CacheDir, fileId), md5);
					return md5;
				}
			}

		}

		public static string getVjoyData()
		{
			bool vjoy_installed = false;
			string vjoyPath = Utils.checkInstalled("vJoy");
			if (!string.IsNullOrEmpty(vjoyPath)) vjoy_installed = true;

			Dictionary<int, string> vjoyGuidData = new Dictionary<int, string>();
			string markerFilePath = Path.Combine(Path.GetTempPath(), "list_vjoy.json");

			if (vjoy_installed)
			{

				DirectInput directInput = new DirectInput();
				List<DeviceInstance> devices = new List<DeviceInstance>();
				devices.AddRange(directInput.GetDevices().Where(x => x.Type != SharpDX.DirectInput.DeviceType.Mouse && x.UsagePage != UsagePage.VendorDefinedBegin && x.Usage != UsageId.AlphanumericBitmapSizeX && x.Usage != UsageId.AlphanumericAlphanumericDisplay && x.UsagePage != unchecked((UsagePage)0xffffff43) && x.UsagePage != UsagePage.Vr).ToList());
				Dictionary<string, Joystick> joyList = new Dictionary<string, Joystick>();
				var joystickState = new JoystickState();
				foreach (var device in devices)
				{
					if (device.ProductName.ToLower().Contains("vjoy"))
					{
						Joystick joystick = new Joystick(directInput, device.InstanceGuid);
						joystick.Properties.BufferSize = 512;
						joystick.Acquire();
						joyList.Add(device.InstanceGuid.ToString(), joystick);
					}
				}
				var vJoyObj = new vJoyManager();
				if (vJoyObj.vJoyEnabled())
				{

					for (uint i = 0; i <= 16; i++)
					{
						VjdStat status = vJoyObj.m_joystick.GetVJDStatus(i);
						if (status == VjdStat.VJD_STAT_FREE)
						{

							vJoyObj.InitDevice((uint)(i));
							vJoyObj.SetButton(1, true);
							vJoyObj.SetButton(2, false);
							vJoyObj.SetButton(3, true);
							Thread.Sleep(100);

							foreach (var joy in joyList)
							{
								var joystick = joy.Value;
								joystick.GetCurrentState(ref joystickState);
								if (joystickState != null && joystickState.Buttons.Count() >= 3 && joystickState.Buttons[0] && !joystickState.Buttons[1] && joystickState.Buttons[2])
								{
									vjoyGuidData.Add((int)i, joy.Key);
									break;
								}

							}

							vJoyObj.SetButton(1, false);
							vJoyObj.SetButton(2, false);
							vJoyObj.SetButton(3, false);
							vJoyObj.ReleaseDevice();

						}
					}

					foreach (var joy in joyList)
					{
						joy.Value.Dispose();

					}

				}

			}
			string json = JsonConvert.SerializeObject(vjoyGuidData, Newtonsoft.Json.Formatting.Indented);
			return json;
		}

		public static string checkInstalled(string findByName)
		{
			string displayName = "";
			string InstallPath = "";
			string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

			//64 bits computer
			RegistryKey key64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
			RegistryKey key = key64.OpenSubKey(registryKey);

			if (key != null)
			{
				foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
				{
					displayName = subkey.GetValue("DisplayName") as string;
					if (displayName != null && displayName.Contains(findByName))
					{
						try
						{
							InstallPath = subkey.GetValue("InstallLocation", ".").ToString();

							return InstallPath; //or displayName
						}
						catch { }

					}
				}
				key.Close();
			}
			RegistryKey key32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
			key = key32.OpenSubKey(registryKey);
			if (key != null)
			{
				foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
				{
					displayName = subkey.GetValue("DisplayName") as string;
					if (displayName != null && displayName.Contains(findByName))
					{

						try
						{
							InstallPath = subkey.GetValue("InstallLocation").ToString();

							return InstallPath; //or displayName

						}
						catch { }

					}
				}
				key.Close();
			}

			if (string.IsNullOrEmpty(InstallPath))
			{
				//Ordinateur\HKEY_LOCAL_MACHINE\

				registryKey = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

				//64 bits computer
				key64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
				key = key64.OpenSubKey(registryKey);

				if (key != null)
				{
					foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
					{
						displayName = subkey.GetValue("DisplayName") as string;
						if (displayName != null && displayName.Contains(findByName))
						{
							try
							{
								InstallPath = subkey.GetValue("UninstallString", ".").ToString();
								InstallPath = InstallPath.Trim('"');
								if (!string.IsNullOrEmpty(InstallPath) && File.Exists(InstallPath)) return InstallPath; //or displayName
							}
							catch { }

						}
					}
					key.Close();
				}

			}
			return null;
		}
		/*
				public static int GetPrimaryMonitorRefreshRateFromXml(string xmlData)
				{
					try
					{
						// Charger le document XML
						XDocument xmlDoc = XDocument.Parse(xmlData);

						// Trouver l'écran principal (position x = 0, y = 0)
						var mainScreen = xmlDoc.Descendants("modeInfo")
							.FirstOrDefault(e =>
							{
								var positionElement = e.Element("DisplayConfigSourceMode")?.Element("position");
								if (positionElement != null)
								{
									var x = positionElement.Element("x")?.Value;
									var y = positionElement.Element("y")?.Value;
									return x == "0" && y == "0";
								}
								return false;
							});

						if (mainScreen != null)
						{
							// Trouver la configuration de mode correspondant à l'ID du modeInfo
							var id = (int?)mainScreen.Element("id");
							try
							{
								var positionElement = mainScreen.Element("DisplayConfigSourceMode");
								if (positionElement != null)
								{
									var widthScreen = (int?)positionElement.Element("width");
									var heightScreen = (int?)positionElement.Element("height");
									if (widthScreen.HasValue && heightScreen.HasValue)
									{
										Program.currentResX = widthScreen.Value;
										Program.currentResY = heightScreen.Value;
									}
								}
							}
							catch { }


							if (id.HasValue)
							{
								var targetId = id.Value.ToString();
								if (string.IsNullOrEmpty(targetId))
								{
									return 60; // Identifiant du moniteur principal non trouvé
								}

								// Trouver le chemin d'information avec le targetId correspondant dans sourceInfo
								var pathInfoElement = xmlDoc.Descendants("DisplayConfigPathInfo")
									.FirstOrDefault(pi =>
									{
										var sourceInfoId = pi.Element("sourceInfo")?.Element("id")?.Value;
										return sourceInfoId == targetId;
									});

								if (pathInfoElement == null)
								{
									return 60;
								}

								var targetInfoElement = pathInfoElement.Element("targetInfo");

								if (targetInfoElement == null)
								{
									return 60;
								}

								var refreshRateElement = targetInfoElement.Element("refreshRate");

								if (refreshRateElement == null)
								{
									return 60;
								}

								// Extraire le numérateur et le dénominateur de la fréquence de rafraîchissement
								var numeratorElement = refreshRateElement.Element("numerator")?.Value;
								var denominatorElement = refreshRateElement.Element("denominator")?.Value;

								if (int.TryParse(numeratorElement, out int numerator) &&
									int.TryParse(denominatorElement, out int denominator) &&
									denominator != 0)
								{
									// Calculer la fréquence de rafraîchissement en Hz
									return (int)Math.Round((double)numerator / denominator);
								}
								else
								{
									return 60; // Erreur de conversion ou dénominateur nul
								}

							}
						}

						// Si l'écran principal n'est pas trouvé, retourner la valeur par défaut de 60 Hz
						return 60;

					}
					catch (Exception ex)
					{
						return 60;
					}
				}
		*/

		public static int GetPrimaryMonitorRefreshRateFromXml(string xmlData)
		{
			try
			{
				// Charger le document XML
				XDocument xmlDoc = XDocument.Parse(xmlData);

				// Trouver l'écran principal (position x = 0, y = 0)
				var mainScreen = xmlDoc.Descendants("modeInfo")
					.FirstOrDefault(e =>
					{
						var positionElement = e.Element("DisplayConfigSourceMode")?.Element("position");
						if (positionElement != null)
						{
							var x = positionElement.Element("x")?.Value;
							var y = positionElement.Element("y")?.Value;
							return x == "0" && y == "0";
						}
						return false;
					});


				if (mainScreen != null)
				{
					var previousModeInfo = mainScreen.ElementsBeforeSelf("modeInfo").LastOrDefault();
					try
					{
						var positionElement = mainScreen.Element("DisplayConfigSourceMode");
						if (positionElement != null)
						{
							var widthScreen = (int?)positionElement.Element("width");
							var heightScreen = (int?)positionElement.Element("height");
							if (widthScreen.HasValue && heightScreen.HasValue)
							{
								Program.currentResX = widthScreen.Value;
								Program.currentResY = heightScreen.Value;
							}
						}
					}
					catch { }


					if (previousModeInfo != null)
					{


						var refreshRateElement = previousModeInfo.Descendants("vSyncFreq").FirstOrDefault();

						if (refreshRateElement == null)
						{
							return 60;
						}

						// Extraire le numérateur et le dénominateur de la fréquence de rafraîchissement
						var numeratorElement = refreshRateElement.Element("numerator")?.Value;
						var denominatorElement = refreshRateElement.Element("denominator")?.Value;

						if (int.TryParse(numeratorElement, out int numerator) &&
							int.TryParse(denominatorElement, out int denominator) &&
							denominator != 0)
						{
							// Calculer la fréquence de rafraîchissement en Hz
							return (int)Math.Round((double)numerator / denominator);
						}
						else
						{
							return 60; // Erreur de conversion ou dénominateur nul
						}

					}
				}

				// Si l'écran principal n'est pas trouvé, retourner la valeur par défaut de 60 Hz
				return 60;

			}
			catch (Exception ex)
			{
				return 60;
			}
		}


		public static string GetFileIdentifier(string filePath)
		{
			FileInfo fileInfo = new FileInfo(filePath);
			string fileIdentifier = "";
			try
			{
				// Tentative de récupération du FILE_ID_INFO
				using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					if (GetFileInformationByHandleEx(fs.SafeFileHandle.DangerousGetHandle(), FileIdInfo, out FILE_ID_INFO fileIdInfo, (uint)Marshal.SizeOf<FILE_ID_INFO>()))
					{
						// Construction de l'identifiant unique avec FILE_ID_INFO
						fileIdentifier = $"{fileIdInfo.VolumeSerialNumber}-{fileIdInfo.FileIdHigh}-{fileIdInfo.FileIdLow}";
					}
					else
					{
						throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
					}
				}
			}
			catch (Exception)
			{
			}
			// Si FILE_ID_INFO n'est pas disponible, utiliser une concaténation classique
			string lastModified = fileInfo.LastWriteTimeUtc.ToString("o"); // Format ISO 8601
			long fileSize = fileInfo.Length;
			string fullPath = fileInfo.FullName;
			fileIdentifier += $"{lastModified}{fileSize}{fullPath}";
			using (var md5 = MD5.Create())
			{
				byte[] inputBytes = Encoding.UTF8.GetBytes(fileIdentifier);
				byte[] hashBytes = md5.ComputeHash(inputBytes);
				fileIdentifier = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
			}

			return fileIdentifier;
		}


	}


}

public class PatchArchive
{
	public string version = "";
	public bool needExtra = false;

	public Dictionary<string, PatchInfoJsonSourceData> FixesArchive = new Dictionary<string, PatchInfoJsonSourceData>();
	public Dictionary<string, List<string>> FixesFiles = new Dictionary<string, List<string>>();

	public Dictionary<string, List<string>> allFiles = new Dictionary<string, List<string>>();
	public Dictionary<string, List<string>> autoXinputLinksFiles = new Dictionary<string, List<string>>();
	public Dictionary<string, List<string>> teknoparrotPatchs = new Dictionary<string, List<string>>();
}

public class PatchInfoJsonElement
{
	public string destination = "";
	public bool bdf = false;
	public List<string> alt_destination = new List<string>();
	public string source_path = "";
	public List<PatchInfoJsonSourceData> source_data = new List<PatchInfoJsonSourceData>();
}

public class PatchInfoJsonSourceData
{
	public long source_size;
	public string source_md5 = "";
	public string patch_name = "";
}

