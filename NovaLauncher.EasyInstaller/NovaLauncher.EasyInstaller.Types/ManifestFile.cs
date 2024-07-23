using System.Collections.Generic;

namespace NovaLauncher.EasyInstaller.Types;

public class ManifestFile
{
	public required string Name { get; set; }

	public required long TotalSize { get; set; }

	public required List<ManifestFileInfo> Files { get; set; }
}
