using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;
using SharpDX.Multimedia;


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

		public static void MakeLink(string source, string target)
		{
			if (!File.Exists(source)) return;
			if (File.Exists(target)) return;

			CreateHardLink(target, source, IntPtr.Zero);
		}

		public static bool IsHardLink(string fileToTestPath, string fromDirPath)
		{
			if (!File.Exists(fileToTestPath)) throw new Exception("file does not exist");
			if (!Directory.Exists(fromDirPath)) throw new Exception("directory does not exist");

			fileToTestPath = Path.GetFullPath(fileToTestPath).ToLower();
			fromDirPath = Path.GetFullPath(fromDirPath).ToLower();
			var sibling = GetFileSiblingHardLinks(fileToTestPath);
			foreach(var siblingLink in sibling)
			{
				string siblingDir = Path.GetDirectoryName(siblingLink);

				if (siblingDir.StartsWith(fromDirPath)) return true;
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
			if (!Directory.Exists(directorySource)) throw new Exception("directory does not exist");
			if (!Directory.Exists(directoryDest)) throw new Exception("directory does not exist");
			directorySource = Path.GetFullPath(directorySource);
			directoryDest = Path.GetFullPath(directoryDest);

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
				MakeLink(file, newfile);
			}

        }

		public static void CleanHardLinksFiles(string directoryToClean, string originalLinkDir)
		{
			directoryToClean = Path.GetFullPath(directoryToClean);
			originalLinkDir = Path.GetFullPath(originalLinkDir);
			if (!Directory.Exists(directoryToClean)) throw new Exception("directory does not exist");
			if (!Directory.Exists(originalLinkDir)) throw new Exception("directory does not exist");
			var filePaths = Directory.EnumerateFiles(directoryToClean, "*", new EnumerationOptions
			{
				IgnoreInaccessible = true,
				RecurseSubdirectories = true
			});
			foreach (var file in filePaths)
			{
				if (IsHardLink(file, originalLinkDir))
				{
					File.Delete(file);
				}
			}
		}
	}
}