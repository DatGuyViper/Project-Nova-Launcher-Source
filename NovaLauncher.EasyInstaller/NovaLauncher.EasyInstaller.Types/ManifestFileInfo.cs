namespace NovaLauncher.EasyInstaller.Types;

public class ManifestFileInfo
{
	public required string FileName { get; set; }

	public required string FileHash { get; set; }

	public required long FileSize { get; set; }
}
