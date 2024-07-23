using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NovaLauncher.Core.Utilities;

namespace NovaLauncher.Core.Nova;

public class AuthFileManager
{
	private static AuthFileManager _authFileManager = new AuthFileManager();

	private AuthFile _authFile;

	public AuthFileManager()
	{
		string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NovaLauncher");
		string path = Path.Combine(text, "auth.json");
		string path2 = Path.Combine(text, "entropy.bin");
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		if (!File.Exists(path2))
		{
			byte[] bytes = Security.GenerateEntropy();
			File.WriteAllBytes(path2, bytes);
		}
		if (File.Exists(path))
		{
			byte[] entropy = File.ReadAllBytes(path2);
			AuthFile authFile = JsonExtension.TryDeserialize<AuthFile>(Security.Unprotect(File.ReadAllBytes(path), entropy));
			_authFile = authFile ?? new AuthFile();
		}
		else
		{
			_authFile = new AuthFile();
		}
	}

	public async Task Save()
	{
		string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NovaLauncher");
		string path = Path.Combine(text, "auth.json");
		string path2 = Path.Combine(text, "entropy.bin");
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		byte[] entropy = File.ReadAllBytes(path2);
		byte[] bytes = Security.Protect(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(_authFile)), entropy);
		await File.WriteAllBytesAsync(path, bytes);
	}

	public AuthFile GetAuthFile()
	{
		return _authFile;
	}

	public static AuthFileManager Get()
	{
		return _authFileManager;
	}
}
