using System;

namespace NovaLauncher.EasyInstaller;

public class DownloadProgressEventArgs : EventArgs
{
	public double ProgressPercentage { get; }

	public DownloadProgressEventArgs(double progressPercentage)
	{
		ProgressPercentage = progressPercentage;
	}
}
