using CliWrap;
using Krypton.Toolkit;
using Newtonsoft.Json;
using SevenZip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace TeknoparrotAutoXinput
{
	public partial class PatchInstall : KryptonForm
	{

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);


		private Action<string> handleStdOut;
		string SevenZipExe = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "7zip", "7z.exe");


		private string PatchFile;
		private string FixDir;
		private string PathAutoXinputLinks;
		private string PathTeknoparrotPatchs;
		public bool ExtractSuccess = false;

		private string TmpDir;
		private string TmpDirFix;
		private string TmpDirFiles;

		private bool isBothPathDirInSameDrive;
		private bool valid = true;

		private PatchArchive patchInfoJson = null;
		public PatchInstall(string patchFile, string fixDir, string pathAutoXinputLinks, string pathTeknoparrotPatchs)
		{
			SevenZipExtractor.SetLibraryPath(Path.Combine(Path.GetFullPath(Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)), "thirdparty", "7zip", "7z.dll"));

			InitializeComponent();
			InitializeHandleStdOut();

			PatchFile = patchFile;
			FixDir = fixDir;
			PathAutoXinputLinks = pathAutoXinputLinks;
			PathTeknoparrotPatchs = pathTeknoparrotPatchs;

			TmpDir = Path.Combine(PathAutoXinputLinks, "_tmp");
			TmpDirFix = Path.Combine(TmpDir, "fix");
			TmpDirFiles = Path.Combine(TmpDir, "files");

			if (!Directory.Exists(Directory.GetParent(PathTeknoparrotPatchs).FullName)) valid = false;
			if (!Directory.Exists(Directory.GetParent(PathAutoXinputLinks).FullName)) valid = false;
			if (!Directory.Exists(FixDir)) valid = false;
			if (!File.Exists(PatchFile)) valid = false;

			Dictionary<string, long> fileSizeDic = new Dictionary<string, long>();
			if (valid)
			{
				long totalUncompressedSize = 0;
				string fileContent = "";
				try
				{
					using (SevenZipExtractor extractor = new SevenZipExtractor(PatchFile))
					{
						string fileToExtract = "patch.json";

						using (MemoryStream ms = new MemoryStream())
						{
							extractor.ExtractFile(fileToExtract, ms);
							ms.Seek(0, SeekOrigin.Begin);
							using (StreamReader reader = new StreamReader(ms))
							{
								fileContent = reader.ReadToEnd();
							}
						}

						foreach (var entry in extractor.ArchiveFileData)
						{
							if(!entry.IsDirectory && entry.FileName.StartsWith(@"files\"))
							{
								if (!fileSizeDic.ContainsKey(Path.GetFileName(entry.FileName))) fileSizeDic.Add(Path.GetFileName(entry.FileName), (long)entry.Size);
							}
							
						}
					}
				}
				catch
				{
					fileContent = "";
				}
				if (fileContent == "")
				{
					MessageBox.Show("Invalid Patch File");
					valid = false;
				}
				if (valid)
                {
					patchInfoJson = JsonConvert.DeserializeObject<PatchArchive>(fileContent);
					isBothPathDirInSameDrive = Utils.IsEligibleHardLink(PathAutoXinputLinks, PathTeknoparrotPatchs);

					long fixArchiveSize = 0;
					foreach(var fa in patchInfoJson.FixesArchive)
					{
						fixArchiveSize += fa.Value.source_size;
					}

					List<string> alreadyAdded = new List<string>();
					long sizeRequiredAutoXinput = 0;
					foreach(var entry in patchInfoJson.autoXinputLinksFiles)
					{
						if (fileSizeDic.ContainsKey(entry.Key))
						{
							long size = fileSizeDic[entry.Key];
							foreach(var link in entry.Value)
							{
								if (!alreadyAdded.Contains(entry.Key))
								{
									sizeRequiredAutoXinput += size;
									alreadyAdded.Add(entry.Key);
								}
								else
								{
									bool useHardlink = true;
									if (size < (300 * 1024) && !link.EndsWith(".fx") && !link.EndsWith(".fxh") && !link.EndsWith(".png") && !link.EndsWith(".jpg")) useHardlink = false;
									if (useHardlink && link.EndsWith(".cfg") || link.EndsWith(".json") || link.EndsWith(".xml") || link.EndsWith(".txt")) useHardlink = false;
									if(!useHardlink) sizeRequiredAutoXinput += size;
								}
							}
						}
					}
					if (!isBothPathDirInSameDrive) alreadyAdded.Clear();
					long sizeRequiredPatchLinkExe = 0;
					foreach (var entry in patchInfoJson.teknoparrotPatchs)
					{
						if (fileSizeDic.ContainsKey(entry.Key))
						{
							long size = fileSizeDic[entry.Key];
							foreach (var link in entry.Value)
							{
								if (!alreadyAdded.Contains(entry.Key))
								{
									sizeRequiredPatchLinkExe += size;
									alreadyAdded.Add(entry.Key);
								}
								else
								{
									bool useHardlink = true;
									if (size < (300 * 1024) && !link.EndsWith(".fx") && !link.EndsWith(".fxh") && !link.EndsWith(".png") && !link.EndsWith(".jpg")) useHardlink = false;
									if (useHardlink && link.EndsWith(".cfg") || link.EndsWith(".json") || link.EndsWith(".xml") || link.EndsWith(".txt")) useHardlink = false;
									if (!useHardlink) sizeRequiredPatchLinkExe += size;
								}
							}
						}
					}
					MessageBox.Show($"{sizeRequiredAutoXinput} and {sizeRequiredPatchLinkExe}");
					if (isBothPathDirInSameDrive)
					{
						long needed_space_AutoXinputLinks = sizeRequiredAutoXinput + sizeRequiredPatchLinkExe + (fixArchiveSize*3);
						//Check free space
						ulong FreeBytesAvailable;
						ulong TotalNumberOfBytes;
						ulong TotalNumberOfFreeBytes;
						bool success = GetDiskFreeSpaceEx(Directory.GetParent(PathAutoXinputLinks).FullName, out FreeBytesAvailable, out TotalNumberOfBytes, out TotalNumberOfFreeBytes);
						if (!success) throw new System.ComponentModel.Win32Exception();
						if (FreeBytesAvailable < (ulong)totalUncompressedSize)
						{
							MessageBox.Show("Not enought free space on " + PathAutoXinputLinks);
							valid = false;
						}
					}
					else
					{
						long needed_space = sizeRequiredAutoXinput + (fixArchiveSize * 3);

						//Check free space
						ulong FreeBytesAvailable;
						ulong TotalNumberOfBytes;
						ulong TotalNumberOfFreeBytes;
						bool success = GetDiskFreeSpaceEx(Directory.GetParent(PathAutoXinputLinks).FullName, out FreeBytesAvailable, out TotalNumberOfBytes, out TotalNumberOfFreeBytes);
						if (!success) throw new System.ComponentModel.Win32Exception();
						if (FreeBytesAvailable < (ulong)totalUncompressedSize)
						{
							MessageBox.Show("Not enought free space on " + PathAutoXinputLinks);
							valid = false;
						}

						needed_space = sizeRequiredPatchLinkExe + (fixArchiveSize * 3);
						success = GetDiskFreeSpaceEx(Directory.GetParent(PathTeknoparrotPatchs).FullName, out FreeBytesAvailable, out TotalNumberOfBytes, out TotalNumberOfFreeBytes);
						if (!success) throw new System.ComponentModel.Win32Exception();
						if (FreeBytesAvailable < (ulong)totalUncompressedSize)
						{
							MessageBox.Show("Not enought free space on " + PathTeknoparrotPatchs);
							valid = false;
						}

					}


					if (valid)
					{
						if (Directory.Exists(PathAutoXinputLinks)) Directory.Delete(PathAutoXinputLinks, true);
						if (Directory.Exists(PathTeknoparrotPatchs)) Directory.Delete(PathTeknoparrotPatchs, true);


						if (!Directory.Exists(TmpDirFix)) Directory.CreateDirectory(TmpDirFix);
						if (!Directory.Exists(TmpDirFiles)) Directory.CreateDirectory(TmpDirFiles);
						if (!Directory.Exists(PathAutoXinputLinks)) Directory.CreateDirectory(PathAutoXinputLinks);
						if (!Directory.Exists(PathTeknoparrotPatchs)) Directory.CreateDirectory(PathTeknoparrotPatchs);
					}
					
				}

			}
		}

		private void PatchInstall_Load(object sender, EventArgs e)
		{

		}

		private void InitializeHandleStdOut()
		{
			handleStdOut = delegate (string msg)
			{
				string pattern = @"([0-9]*)%";
				string input = msg;

				Match match = Regex.Match(input, pattern);

				if (match.Success)
				{
					string percentage = match.Groups[1].Value;

					int progress = 0;
					if (int.TryParse(percentage, out progress))
					{
						progress_extract.Invoke(new MethodInvoker(delegate
						{

							progress_extract.Value = progress;
							progress_extract.Refresh();

						}));
					}
				}
				if (msg.Contains("Everything is Ok"))
				{
					int progress = 100;
					progress_extract.Invoke(new MethodInvoker(delegate
					{

						progress_extract.Visible = false;

					}));
				}
			};
		}

		public async Task<string> SimpleExtractArchiveWithProgressAsync(string fileToExtract, string dirOut)
		{

			try
			{
				var result = await Cli.Wrap(SevenZipExe)
					.WithArguments(new[] { "x", fileToExtract, @"-o" + dirOut, "-bsp1", "-y" })
					.WithStandardOutputPipe(PipeTarget.ToDelegate(handleStdOut))
					.ExecuteAsync();

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			return dirOut;
		}
		public static string GetMd5HashAsString(string FileName)
		{
			if (File.Exists(FileName))
			{
				using (var md5 = MD5.Create())
				{
					using (var stream = File.OpenRead(FileName))
					{
						var hash = md5.ComputeHash(stream);
						return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant().ToUpper();
					}
				}
			}
			return "";
		}

		private async void PatchInstall_Shown(object sender, EventArgs e)
		{
			if (!valid)
			{
				this.DialogResult = DialogResult.Abort;
				this.Close();
			}
			else
			{
				if (patchInfoJson.FixesArchive.Count() > 0)
				{
					string[] archiveFiles = Directory.GetFiles(FixDir, "*.*", SearchOption.TopDirectoryOnly)
										  .Where(f => f.EndsWith(".7z") || f.EndsWith(".zip") || f.EndsWith(".rar"))
										  .ToArray();

					// Dictionnaire pour stocker les informations sur les archives et leurs répertoires de destination
					var archiveExtractionInfo = new Dictionary<string, string>();

					progress_extract.Minimum = 0;
					progress_extract.Maximum = archiveFiles.Count();
					progress_extract.Value = 0;
					progress_extract.Visible = true;
					progress_extract.Text = "Extracting fixes";
					progress_extract.Show();
					foreach (var archiveFile in archiveFiles)
					{
						// Nom de base du fichier avec l'extension
						string fileNameWithExtension = Path.GetFileName(archiveFile);
						// Chemin complet du sous-répertoire de destination
						string destinationDir = Path.Combine(TmpDirFix, fileNameWithExtension);

						ProcessStartInfo processStartInfo = new ProcessStartInfo
						{
							FileName = SevenZipExe, // Si 7z.exe n'est pas dans PATH, spécifiez le chemin complet ici
							Arguments = $"x \"{archiveFile}\" -o\"{destinationDir}\"",
							RedirectStandardOutput = true,
							UseShellExecute = false,
							CreateNoWindow = true
						};

						// Exécuter la commande
						using (Process process = new Process())
						{
							process.StartInfo = processStartInfo;
							process.Start();
							process.WaitForExit();

							// Lire la sortie de la commande si nécessaire
							string output = process.StandardOutput.ReadToEnd();
							Console.WriteLine(output);
						}

						progress_extract.Value++;
						progress_extract.Refresh();
					}
					progress_extract.Visible = false;

					List<string> md5fixes = new List<string>();
					progress_extract.Minimum = 0;
					progress_extract.Maximum = archiveFiles.Count();
					progress_extract.Value = 0;
					progress_extract.Visible = true;
					progress_extract.Text = "Check fixes files";
					progress_extract.Show();
					progress_extract.Visible = true;
					progress_extract.Refresh();
					foreach (var archiveFile in archiveFiles)
					{
						string fileNameWithExtension = Path.GetFileName(archiveFile);
						string destinationDir = Path.Combine(TmpDirFix, fileNameWithExtension);
						var files = Directory.GetFiles(destinationDir, "*", SearchOption.AllDirectories);
						foreach (var f in files)
						{
							string md5val = GetMd5HashAsString(f);
							if (!md5fixes.Contains(md5val))
							{
								md5fixes.Add(md5val);
								File.Move(f, Path.Combine(TmpDirFiles, md5val));
							}
						}
						Directory.Delete(destinationDir, true);

						progress_extract.Value++;
						progress_extract.Refresh();
					}
					progress_extract.Visible = false;
				}

				progress_extract.Minimum = 0;
				progress_extract.Maximum = 100;
				progress_extract.Value = 0;
				progress_extract.Visible = true;
				progress_extract.Text = Path.GetFileName(PatchFile);
				progress_extract.Show();
				progress_extract.Visible = true;
				await SimpleExtractArchiveWithProgressAsync(PatchFile, TmpDir);
				progress_extract.Visible = false;


				progress_extract.Minimum = 0;
				progress_extract.Maximum = patchInfoJson.autoXinputLinksFiles.Count();
				progress_extract.Value = 0;
				progress_extract.Visible = true;
				progress_extract.Text = "autoXinputLinksFiles";
				progress_extract.Show();
				progress_extract.Visible = true;
				Dictionary<string, string> filesSet = new Dictionary<string, string>();

				bool isTmpSameDrive = Utils.IsEligibleHardLink(TmpDirFiles, PathAutoXinputLinks);
				foreach (var fileToProcess in patchInfoJson.autoXinputLinksFiles)
				{
					string trueFile = Path.Combine(TmpDirFiles, fileToProcess.Key);
					if (File.Exists(trueFile))
					{
						long size = new FileInfo(trueFile).Length;
						foreach (var fileDest in fileToProcess.Value)
						{
							string trueDest = Path.Combine(PathAutoXinputLinks, fileDest);
							string parentDir = Path.GetDirectoryName(trueDest);
							if (!Directory.Exists(parentDir)) Directory.CreateDirectory(parentDir);

							bool useHardlink = false;
							string source = trueFile;
							if (filesSet.ContainsKey(fileToProcess.Key))
							{
								source = filesSet[fileToProcess.Key];
								useHardlink = true;
							}
							else
							{
								filesSet.Add(fileToProcess.Key, trueFile);
							}
							if (isTmpSameDrive) { useHardlink = true; }

							if (size < (300 * 1024) && !trueDest.EndsWith(".fx") && !trueDest.EndsWith(".fxh") && !trueDest.EndsWith(".png") && !trueDest.EndsWith(".jpg")) useHardlink = false;
							if (size == 0) useHardlink = false;
							if (useHardlink && trueDest.EndsWith(".cfg") || trueDest.EndsWith(".json") || trueDest.EndsWith(".xml") || trueDest.EndsWith(".txt")) useHardlink = false;

							if (useHardlink)
							{
								Utils.MakeLink(trueFile, trueDest);
							}
							else
							{
								File.Copy(trueFile, trueDest, true);
							}
						}
					}


					progress_extract.Value++;
					progress_extract.Refresh();
				}
				progress_extract.Visible = false;

				if (!isBothPathDirInSameDrive) filesSet.Clear();
				isTmpSameDrive = Utils.IsEligibleHardLink(TmpDirFiles, PathTeknoparrotPatchs);

				progress_extract.Minimum = 0;
				progress_extract.Maximum = patchInfoJson.teknoparrotPatchs.Count();
				progress_extract.Value = 0;
				progress_extract.Visible = true;
				progress_extract.Text = "Teknoparrot-Patches";
				progress_extract.Show();
				progress_extract.Visible = true;
				foreach (var fileToProcess in patchInfoJson.teknoparrotPatchs)
				{
					string trueFile = Path.Combine(TmpDirFiles, fileToProcess.Key);
					if (File.Exists(trueFile))
					{
						long size = new FileInfo(trueFile).Length;
						foreach (var fileDest in fileToProcess.Value)
						{
							string trueDest = Path.Combine(PathTeknoparrotPatchs, fileDest);
							string parentDir = Path.GetDirectoryName(trueDest);
							if (!Directory.Exists(parentDir)) Directory.CreateDirectory(parentDir);

							bool useHardlink = false;
							string source = trueFile;
							if (filesSet.ContainsKey(fileToProcess.Key))
							{
								source = filesSet[fileToProcess.Key];
								useHardlink = true;
							}
							else
							{
								filesSet.Add(fileToProcess.Key, trueFile);
							}

							if (isTmpSameDrive) { useHardlink = true; }

							if (size < (300 * 1024) && !trueDest.EndsWith(".fx") && !trueDest.EndsWith(".fxh") && !trueDest.EndsWith(".png") && !trueDest.EndsWith(".jpg")) useHardlink = false;
							if (size == 0) useHardlink = false;
							if (useHardlink && trueDest.EndsWith(".cfg") || trueDest.EndsWith(".json") || trueDest.EndsWith(".xml") || trueDest.EndsWith(".txt")) useHardlink = false;

							if (useHardlink)
							{
								Utils.MakeLink(trueFile, trueDest);
							}
							else
							{
								File.Copy(trueFile, trueDest, true);
							}
						}
					}


					progress_extract.Value++;
					progress_extract.Refresh();
				}
				progress_extract.Visible = false;

				Directory.Delete(TmpDir, true);

				ExtractSuccess = true;
				this.DialogResult = DialogResult.OK;
				this.Close();

			}

		}
	}
}
