using BrightIdeasSoftware;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using TeknoParrotUi.Common;

namespace TeknoparrotAutoXinput
{


	public partial class GameManagementUserControl : UserControl
	{

		public string TpFolder = "";

		private List<string> gameExecutableList = new List<string>();
		private Dictionary<string, string> cacheMD5 = new Dictionary<string, string>();
		private List<string> MissingGameProfile = new List<string>();
		private List<string> ExistingGameProfile = new List<string>();
		private List<string> ExistingUserProfile = new List<string>();
		private Dictionary<string, Metadata> GamesMetaData = new Dictionary<string, Metadata>();

		private Dictionary<string, UserProfileGame> UserProfiles = new Dictionary<string, UserProfileGame>();

		Dictionary<string, GameExecutableData2> GameExecutableDatas = (Dictionary<string, GameExecutableData2>)JsonConvert.DeserializeObject<Dictionary<string, GameExecutableData2>>(File.ReadAllText(Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "gameList4.json")));


		private class UserProfileGame
		{
			public UserProfileGame() { }
			public string GameID { get; set; } = "";
			public string GameName { get; set; } = "";
			public string GameLabel { get; set; } = "";
			public string XmlProfilePath { get; set; } = "";
			public Metadata Metadata { get; set; } = null;

			public string MainExecutable { get; set; } = "";
			public string SecondaryExecutable { get; set; } = "";
			public bool HasTwoExecutable = false;
			public bool MissingFirstExecutable = false;
			public bool MissingSecondExecutable = false;
			public string MainExecutableMD5 { get; set; } = "";
			public string SecondaryExecutableMD5 { get; set; } = "";

			public int Status = 0;
			public string Description = "";


			public void CalculateHash()
			{
				string CacheDir = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)), "cachemd5");
				if (!Directory.Exists(CacheDir))
				{
					Directory.CreateDirectory(CacheDir);
				}

				if (File.Exists(MainExecutable))
				{

					FileInfo fileInfo = new FileInfo(MainExecutable);
					string lastModified = fileInfo.LastWriteTimeUtc.ToString("o"); // Format ISO 8601
					long fileSize = fileInfo.Length;
					string fullPath = fileInfo.FullName;
					string concatenatedInfo = $"{lastModified}{fileSize}{fullPath}";
					string fileIdentifier = "";
					using (var md5 = MD5.Create())
					{
						byte[] inputBytes = Encoding.UTF8.GetBytes(concatenatedInfo);
						byte[] hashBytes = md5.ComputeHash(inputBytes);
						fileIdentifier = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
					}
					if (fileIdentifier != "" && File.Exists(Path.Combine(CacheDir, fileIdentifier)))
					{
						MainExecutableMD5 = File.ReadAllText(Path.Combine(CacheDir, fileIdentifier));
					}
					else
					{
						MainExecutableMD5 = CalculateMD5(MainExecutable);
						File.WriteAllText(Path.Combine(CacheDir, fileIdentifier), MainExecutableMD5);
					}
				}

				if (HasTwoExecutable && File.Exists(SecondaryExecutable))
				{
					FileInfo fileInfo = new FileInfo(SecondaryExecutable);
					string lastModified = fileInfo.LastWriteTimeUtc.ToString("o"); // Format ISO 8601
					long fileSize = fileInfo.Length;
					string fullPath = fileInfo.FullName;
					string concatenatedInfo = $"{lastModified}{fileSize}{fullPath}";
					string fileIdentifier = "";
					using (var md5 = MD5.Create())
					{
						byte[] inputBytes = Encoding.UTF8.GetBytes(concatenatedInfo);
						byte[] hashBytes = md5.ComputeHash(inputBytes);
						fileIdentifier = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
					}
					if (fileIdentifier != "" && File.Exists(Path.Combine(CacheDir, fileIdentifier)))
					{
						SecondaryExecutableMD5 = File.ReadAllText(Path.Combine(CacheDir, fileIdentifier));
					}
					else
					{
						SecondaryExecutableMD5 = CalculateMD5(SecondaryExecutable);
						File.WriteAllText(Path.Combine(CacheDir, fileIdentifier), SecondaryExecutableMD5);
					}
				}
			}

			private string CalculateMD5(string filePath)
			{

				using (var md5 = MD5.Create())
				{
					using (var stream = new BufferedStream(File.OpenRead(filePath), 120000)) // Buffer de 120 Ko
					{
						var hash = md5.ComputeHash(stream);
						return BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
					}
				}

			}
		}

		class GameExecutableData2
		{
			public string gameId = "";
			public Dictionary<string, GameExecutableTarget> mainExecutable = new Dictionary<string, GameExecutableTarget>();
			public Dictionary<string, GameExecutableTarget> secondaryExecutable = new Dictionary<string, GameExecutableTarget>();
		}

		class GameExecutableTarget
		{
			public GameExecutableHash hash;
			public List<GameExecutableAdjacent> adjacents = new List<GameExecutableAdjacent>();
			public List<string> relativePathSecondary = new List<string>();
			public string label = "";
			public string preferedName = "";
			public bool isOriginal = false;
			public string patchedFrom = "";
		}

		class GameExecutableAdjacent
		{
			public GameExecutableHash hash;
			public string file;
		}
		class GameExecutableHash
		{
			public long size = -1;
			public string md5 = "";
			public string crc = "";
		}

		public GameManagementUserControl()
		{
			InitializeComponent();
			this.objectListViewGameList.RowHeight = 35;
			//this.objectListViewGameList.SmallImageList = imageListSmall;
			this.objectListViewGameList.EmptyListMsg = "No tasks match the filter";
			this.objectListViewGameList.UseAlternatingBackColors = false;
			this.objectListViewGameList.UseHotItem = false;

			DescribedTaskRenderer renderer = new DescribedTaskRenderer();
			renderer.DescriptionAspectName = "Description";
			renderer.TitleFont = new Font("Tahoma", 9, FontStyle.Bold);
			renderer.DescriptionFont = new Font("Tahoma", 8);
			renderer.ImageTextSpace = 8;
			renderer.TitleDescriptionSpace = 1;

			renderer.UseGdiTextRendering = true;
			clm_name.Renderer = renderer;
			clm_name.CellPadding = new Rectangle(4, 2, 4, 2);
		}

		public void ManualLoad()
		{
			MissingGameProfile = new List<string>();
			ExistingGameProfile = new List<string>();
			ExistingUserProfile = new List<string>();
			GamesMetaData = new Dictionary<string, Metadata>();

			string gameProfileDir = Path.Combine(TpFolder, "GameProfiles");
			if (Directory.Exists(gameProfileDir))
			{
				var cfgList = Directory.GetFiles(gameProfileDir, "*.xml");
				Parallel.ForEach(cfgList, new ParallelOptions { MaxDegreeOfParallelism = 6 }, (cfg) =>
				{
					lock (ExistingGameProfile)
					{
						ExistingGameProfile.Add(Path.GetFileNameWithoutExtension(cfg));
					}

					string ParentPath = Path.GetDirectoryName(cfg);
					ParentPath = Path.GetDirectoryName(ParentPath);
					var metadataPath = Path.Combine(ParentPath, "Metadata", Path.GetFileNameWithoutExtension(cfg) + ".json");
					if (File.Exists(metadataPath))
					{
						try
						{
							var metadata = JsonConvert.DeserializeObject<Metadata>(File.ReadAllText(metadataPath));
							if (metadata != null)
							{
								lock (GamesMetaData)
								{
									GamesMetaData.Add(Path.GetFileNameWithoutExtension(cfg), metadata);
								}
							}
						}
						catch { }
					}
				});
			}


			string userDir = Path.Combine(TpFolder, "UserProfiles");
			if (Directory.Exists(userDir))
			{
				var cfgList = Directory.GetFiles(userDir, "*.xml");

				Parallel.ForEach(cfgList, new ParallelOptions { MaxDegreeOfParallelism = 6 }, (cfg) =>
				{
					if (Path.GetFileName(cfg).Contains(".custom.") == false)
					{


						string OriginalXML = File.ReadAllText(cfg);
						var xmlDoc = new XmlDocument();
						xmlDoc.LoadXml(OriginalXML);

						UserProfileGame userProfileGame = new UserProfileGame();
						userProfileGame.GameID = Path.GetFileNameWithoutExtension(cfg);
						userProfileGame.XmlProfilePath = cfg;

						try
						{
							XmlNode hasTwoExecutablesNode = xmlDoc.SelectSingleNode("/GameProfile/HasTwoExecutables");
							if (hasTwoExecutablesNode != null)
							{
								if (hasTwoExecutablesNode.InnerText.ToLower() == "true")
								{
									userProfileGame.HasTwoExecutable = true;
								}
							}
							if (userProfileGame.HasTwoExecutable)
							{
								userProfileGame.MissingSecondExecutable = true;
								XmlNode gamePath2Node = xmlDoc.SelectSingleNode("/GameProfile/GamePath2");
								if (gamePath2Node != null)
								{
									string gamePath2Content = gamePath2Node.InnerText;
									if (gamePath2Content != "" && File.Exists(gamePath2Content))
									{
										userProfileGame.SecondaryExecutable = gamePath2Content;
										userProfileGame.MissingSecondExecutable = false;
									}
								}
							}

						}
						catch { }

						userProfileGame.MissingFirstExecutable = true;
						try
						{
							XmlNode gamePathNode = xmlDoc.SelectSingleNode("/GameProfile/GamePath");
							if (gamePathNode != null)
							{
								string gamePathContent = gamePathNode.InnerText;
								if (gamePathContent != "" && File.Exists(gamePathContent))
								{
									userProfileGame.MainExecutable = gamePathContent;
									userProfileGame.MissingFirstExecutable = false;
									lock (gameExecutableList)
									{
										if (!gameExecutableList.Contains(gamePathContent))
										{
											gameExecutableList.Add(gamePathContent);
										}
									}
								}
							}
						}
						catch (Exception ex) { }

						if (!userProfileGame.MissingFirstExecutable && !userProfileGame.MissingSecondExecutable)
						{
							lock (ExistingUserProfile)
							{
								ExistingUserProfile.Add(Path.GetFileNameWithoutExtension(cfg));
							}
						}



						userProfileGame.Metadata = GamesMetaData.ContainsKey(userProfileGame.GameID) ? GamesMetaData[userProfileGame.GameID] : null;
						userProfileGame.GameName = userProfileGame.Metadata != null ? userProfileGame.Metadata.game_name : null;

						if (userProfileGame.MissingFirstExecutable || userProfileGame.MissingSecondExecutable)
						{
							userProfileGame.Description = "Missing executable !";
							userProfileGame.Status = 2;
						}



						lock (UserProfiles)
						{
							UserProfiles.Add(userProfileGame.GameID, userProfileGame);
						}
					}
				});
				this.objectListViewGameList.SetObjects(UserProfiles.Values);
			}

			foreach (var gameProfile in ExistingGameProfile)
			{
				if (!ExistingUserProfile.Contains(gameProfile))
				{
					MissingGameProfile.Add(gameProfile);
				}
			}

			if (txt_scangame.Text == "" && gameExecutableList.Count > 5)
			{
				Dictionary<string, int> commonPathsCount = new Dictionary<string, int>();
				Random random = new Random();

				for (int i = 0; i < 100; i++)
				{
					var selectedDirectories = new List<string>();
					while (selectedDirectories.Count < 3)
					{
						string directory = gameExecutableList[random.Next(gameExecutableList.Count)];
						if (!selectedDirectories.Contains(directory))
						{
							selectedDirectories.Add(directory);
						}
					}

					string commonPath = new Func<List<string>, string>(paths =>
					{
						if (paths == null || paths.Count == 0)
						{
							return string.Empty;
						}

						string[] splitPath = paths[0].Split('\\');
						for (int j = 1; j < paths.Count; j++)
						{
							string[] splitCurrent = paths[j].Split('\\');
							for (int k = 0; k < splitPath.Length; k++)
							{
								if (k >= splitCurrent.Length || !splitPath[k].Equals(splitCurrent[k], StringComparison.OrdinalIgnoreCase))
								{
									splitPath = splitPath.Take(k).ToArray();
									break;
								}
							}
						}

						return string.Join("\\", splitPath);
					})(selectedDirectories);

					if (!string.IsNullOrEmpty(commonPath))
					{
						if (commonPathsCount.ContainsKey(commonPath))
						{
							commonPathsCount[commonPath]++;
						}
						else
						{
							commonPathsCount[commonPath] = 1;
						}
					}
				}
				if (commonPathsCount.Count > 0)
				{
					string mostCommonPath = commonPathsCount.OrderByDescending(kvp => kvp.Value).FirstOrDefault().Key;
					txt_scangame.Text = mostCommonPath;
				}

			}
			Task.Run(() =>
			{
				Parallel.ForEach(UserProfiles.Values, new ParallelOptions { MaxDegreeOfParallelism = 4 }, (u) =>
				{
					if (u.Status == 0)
					{
						u.CalculateHash();
						u.Status = 1;
						if (GameExecutableDatas.ContainsKey(u.GameID) && GameExecutableDatas[u.GameID].mainExecutable.ContainsKey(u.MainExecutableMD5))
						{
							if (GameExecutableDatas[u.GameID].mainExecutable[u.MainExecutableMD5].isOriginal)
							{
								u.Status = 0;
							}
							u.Description = GameExecutableDatas[u.GameID].mainExecutable[u.MainExecutableMD5].label;
						}
						else
						{
							u.Description = $"Unknow Executable ({u.MainExecutableMD5})";
						}

						// Utiliser Invoke pour mettre à jour le contrôle sur le thread principal
						objectListViewGameList.Invoke(new Action(() =>
						{
							// Assure-toi de gérer les mises à jour de l'UI correctement
							objectListViewGameList.RefreshObject(u);
						}));

					}

				});
			});

		}


		private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void objectListViewGameList_FormatCell(object sender, FormatCellEventArgs e)
		{

		}

		private void objectListViewGameList_FormatRow(object sender, FormatRowEventArgs e)
		{
			UserProfileGame upg = (UserProfileGame)e.Model;
			if (upg.Status == 2)
			{
				e.Item.BackColor = ColorTranslator.FromHtml("#ffe6e6");
			}
			if (upg.Status == 1)
			{
				e.Item.BackColor = Color.LightYellow;
			}
		}
	}
}
