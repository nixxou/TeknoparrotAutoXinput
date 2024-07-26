class GameExecutableData
{
	public string gameId = "";
	public Dictionary<string, GameExecutableHash> mainExecutable = new Dictionary<string, GameExecutableHash>();
	public string mainExecutableAdjacentFile = "";
	public GameExecutableHash mainExecutableAdjacentFileHash = new GameExecutableHash();

	public Dictionary<string, GameExecutableHash> secondaryExecutable = new Dictionary<string, GameExecutableHash>();
	public string secondaryExecutableAdjacentFile = "";
	public GameExecutableHash secondaryExecutableAdjacentFileHash = new GameExecutableHash();

	public string relativePathSecondary = "";

}

class GameExecutableHash
{
	public long size = -1;
	public string md5 = "";
	public string crc = "";
}