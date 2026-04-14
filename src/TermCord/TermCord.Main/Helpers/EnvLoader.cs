namespace TermCord.Main.Helpers;

public class EnvLoader
{
	private const string FileName = ".env";
	private readonly Dictionary<string, string> _variables;

	public IReadOnlyDictionary<string, string> Variables => _variables;

	public EnvLoader()
	{
		_variables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		EnsureFileExists();
		Load();
	}

	private static void EnsureFileExists()
	{
		if (File.Exists(FileName)) 
			return;

		var defaultContent = "BOT_TOKEN=your_token_here" + Environment.NewLine;
		File.WriteAllText(FileName, defaultContent);
	}

	private void Load()
	{
		foreach (var line in File.ReadAllLines(FileName))
		{
			if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
				continue;

			var parts = line.Split('=', 2);
			if (parts.Length != 2)
				continue;

			var key = parts[0].Trim();
			var value = parts[1].Trim();

			_variables[key] = value;
		}
	}

	public string? Get(string key)
		=> _variables.GetValueOrDefault(key);
}