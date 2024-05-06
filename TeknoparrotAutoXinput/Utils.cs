using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;
using SharpDX.Multimedia;
using System.Diagnostics;
using Microsoft.Win32.TaskScheduler;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Xml.Linq;
using System.ComponentModel;


namespace TeknoparrotAutoXinput
{

	public static class Utils
	{

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
		public static extern bool SetForegroundWindow(IntPtr hWnd);


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

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWindowVisible(IntPtr hWnd);


		public static bool IsHardLink(string fileToTestPath, string fromDirPath)
		{
			if (!File.Exists(fileToTestPath)) throw new Exception("file does not exist");
			if (!Directory.Exists(fromDirPath)) throw new Exception("directory does not exist");

			fileToTestPath = Path.GetFullPath(fileToTestPath).ToLower();
			fromDirPath = Path.GetFullPath(fromDirPath).ToLower();
			var sibling = GetFileSiblingHardLinks(fileToTestPath);
			foreach(var siblingLink in sibling)
			{
				string siblingDir = Path.GetDirectoryName(siblingLink).ToLower();

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


		public static void HardLinkFiles(string directorySource, string directoryDest)
		{
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

        }

		public static void CleanHardLinksFiles(string directoryToClean, string originalLinkDir)
		{
			directoryToClean = Path.GetFullPath(directoryToClean);
			originalLinkDir = Path.GetFullPath(originalLinkDir);
			if (!Directory.Exists(directoryToClean)) return;
			if (!Directory.Exists(originalLinkDir)) return;

			if (!AreFoldersOnSameDrive(directoryToClean, originalLinkDir)) return;

			var filePaths = Directory.EnumerateFiles(directoryToClean, "*", new EnumerationOptions
			{
				IgnoreInaccessible = true,
				RecurseSubdirectories = true
			});
			foreach (var file in filePaths)
			{
				//if (Program.DebugMode) Utils.LogMessage($"Check Link for {file}");
				if (IsHardLink(file, originalLinkDir))
				{
					if (Program.DebugMode) Utils.LogMessage($"{file} is Hardlink, delete it");
					try
					{
						File.Delete(file);
					}
					catch(Exception ex)
					{
						if (File.Exists(file))
						{
							try
							{
								FileInfo finfo = new FileInfo(file);
								if(finfo.IsReadOnly)
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
				if (file.EndsWith(".filetorestore"))
				{
					if (Program.DebugMode) Utils.LogMessage($"{file} must be restored");
					string newFilePath = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
					File.Move(file, newFilePath);
				}
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

		public static void ExecuteAHK(string ahkCode, bool waitForExit)
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
					process.StartInfo.Arguments = tempFilePath;
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.CreateNoWindow = true;

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
			foreach(var ahkpid in Program.ProcessToKill) 
			{
				KillProcessById(ahkpid);
			}
			Thread.Sleep(100);
			foreach(var ahkfile in Program.FilesToDelete)
			{
				if (File.Exists(ahkfile))
				{
					File.Delete(ahkfile);
				}
			}
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
			string taskName = ExeToTaskName(taskExe,taskArgument);
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
			if(!Directory.Exists(path)) return false;

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
				if(!driveInfo.DriveFormat.Equals("NTFS", StringComparison.OrdinalIgnoreCase)) return false;
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
					catch{}
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

	}
}