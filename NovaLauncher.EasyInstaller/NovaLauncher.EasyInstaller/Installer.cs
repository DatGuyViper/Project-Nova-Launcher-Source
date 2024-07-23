using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NovaLauncher.EasyInstaller
{
    public class Installer
    {
        private static string _baseUrl = "https://cdn.novafn.dev";

        private static HttpClient _httpClient = new HttpClient();

        public static List<ManifestFileInfo> InvalidFiles;

        public static int DownloadedFiles;

        public static int FilesToDownload;

        public static bool PauseDownload = false;

        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public static event EventHandler<DownloadProgressEventArgs>? DownloadProgressChanged;

        public static event EventHandler? DownloadCompletedEvent;

        private static async Task<long> CopyToAsync(Stream source, Stream destination, long? totalBytes, int bufferSize, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[bufferSize];
            long totalBytesRead = 0L;
            while (true)
            {
                int num;
                int bytesRead = (num = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken));
                if (num == 0)
                {
                    break;
                }
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }

        private static string ComputeHash(string file)
        {
            using BufferedStream inputStream = new BufferedStream(File.OpenRead(file), 67108864);
            using SHA256 sHA = SHA256.Create();
            return BitConverter.ToString(sHA.ComputeHash(inputStream)).Replace("-", string.Empty).ToLower();
        }

        public static async Task<ManifestFile?> GetManifest(string version)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(_baseUrl + "/Download/" + version + ".manifest");
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return null;
            }
            return JsonSerializer.Deserialize<ManifestFile>(await httpResponseMessage.Content.ReadAsStringAsync());
        }

        public static async Task<List<string>> GetVersions()
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(_baseUrl + "/versions.json");
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return new List<string>();
            }
            List<string> list = await httpResponseMessage.Content.ReadFromJsonAsync<List<string>>();
            if (list == null)
            {
                return new List<string>();
            }
            return list;
        }

        public static async Task DownloadFromList(List<ManifestFileInfo> files, string version, string path, CancellationTokenSource cancellationTokenSource)
        {
            SemaphoreSlim semaphore = new SemaphoreSlim(Environment.ProcessorCount / 2);
            _cancellationTokenSource = cancellationTokenSource;
            Installer.DownloadProgressChanged?.Invoke(null, new DownloadProgressEventArgs(0.0));
            PauseDownload = false;
            DownloadedFiles = 0;
            FilesToDownload = files.Count;
            await Task.WhenAll(files.Select((ManifestFileInfo file) => DownloadFile(file, semaphore, version, path, retryAttempt: false, cancellationTokenSource.Token)));
            if (!cancellationTokenSource.IsCancellationRequested && DownloadedFiles == FilesToDownload)
            {
                Installer.DownloadCompletedEvent?.Invoke(null, EventArgs.Empty);
            }
        }

        public static async Task Download(ManifestFile manifest, string version, string path, CancellationTokenSource cancellationTokenSource)
        {
            SemaphoreSlim semaphore = new SemaphoreSlim(Environment.ProcessorCount / 2);
            _cancellationTokenSource = cancellationTokenSource;
            Installer.DownloadProgressChanged?.Invoke(null, new DownloadProgressEventArgs(0.0));
            PauseDownload = false;
            DownloadedFiles = 0;
            FilesToDownload = manifest.Files.Count;
            await Task.WhenAll(manifest.Files.Select((ManifestFileInfo file) => DownloadFile(file, semaphore, version, path, retryAttempt: false, cancellationTokenSource.Token)));
            if (!cancellationTokenSource.IsCancellationRequested)
            {
                Installer.DownloadCompletedEvent?.Invoke(null, EventArgs.Empty);
            }
        }

        public static async Task<bool> Verify(ManifestFile manifest, string version, string path)
        {
            SemaphoreSlim semaphore = new SemaphoreSlim(Environment.ProcessorCount / 2);
            DownloadedFiles = 0;
            InvalidFiles = new List<ManifestFileInfo>();
            PauseDownload = false;
            FilesToDownload = manifest.Files.Count;
            await Task.WhenAll(manifest.Files.Select((ManifestFileInfo file) => Task.Run(() => VerifyFile(file, version, path))));
            return DownloadedFiles == FilesToDownload;
        }

        public static void VerifyFile(ManifestFileInfo manifestFile, string version, string path)
        {
            string filePath = Path.Combine(path, manifestFile.FileName);
            Console.WriteLine("Verifying " + manifestFile.FileName + ".");
            if (File.Exists(filePath))
            {
                if (new FileInfo(filePath).Length != manifestFile.FileSize)
                {
                    lock (InvalidFiles)
                    {
                        InvalidFiles.Add(manifestFile);
                        return;
                    }
                }
                Interlocked.Increment(ref DownloadedFiles);
                Console.WriteLine(manifestFile.FileName + " is OK.");
            }
            else
            {
                lock (InvalidFiles)
                {
                    InvalidFiles.Add(manifestFile);
                }
                Console.WriteLine(manifestFile.FileName + " is INCOMPLETE.");
            }
        }

        public static async Task DownloadFile(ManifestFileInfo manifestFile, SemaphoreSlim semaphore, string version, string path, bool retryAttempt, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            bool bAlreadyExists = false;
            string fileUrl = $"{_baseUrl}/Download/{version}/{manifestFile.FileName}";
            try
            {
                Console.WriteLine("Downloading " + manifestFile.FileName + ".");
                string filePath = Path.Combine(path, manifestFile.FileName);
                bAlreadyExists = File.Exists(filePath);
                if (bAlreadyExists)
                {
                    if (new FileInfo(filePath).Length == manifestFile.FileSize)
                    {
                        DownloadedFiles++;
                        Installer.DownloadProgressChanged?.Invoke(null, new DownloadProgressEventArgs((double)DownloadedFiles / (double)FilesToDownload * 100.0));
                        Console.WriteLine(manifestFile.FileName + " is OK.");
                        return;
                    }
                    Console.WriteLine(manifestFile.FileName + " is INCOMPLETE.");
                    bAlreadyExists = false;
                }
                if (!retryAttempt)
                {
                    await semaphore.WaitAsync(cancellationToken);
                }
                while (PauseDownload)
                {
                    await Task.Delay(250);
                }
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                Console.WriteLine("Downloading " + (retryAttempt ? "retrying" : "") + " " + filePath);
                using (Stream stream = await _httpClient.GetStreamAsync(fileUrl, cancellationToken))
                {
                    using FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 1048576, useAsync: true);
                    using GZipStream gZipStream = new GZipStream(stream, CompressionMode.Decompress);
                    await CopyToAsync(gZipStream, fileStream, manifestFile.FileSize, 1048576, cancellationToken);
                }
                if (new FileInfo(filePath).Length != manifestFile.FileSize)
                {
                    throw new Exception("Mismatching size after download");
                }
                DownloadedFiles++;
                Installer.DownloadProgressChanged?.Invoke(null, new DownloadProgressEventArgs((double)DownloadedFiles / (double)FilesToDownload * 100.0));
                Console.WriteLine("Terminated download of " + manifestFile.FileName + " " + (retryAttempt ? "after retry" : ""));
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Download of " + manifestFile.FileName + " was paused.");
                semaphore.Release();
            }
            catch (Exception ex2)
            {
                Console.WriteLine(ex2.Message);
                semaphore.Release();
                if (!cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Download of " + manifestFile.FileName + " retry.");
                    await DownloadFile(manifestFile, semaphore, version, path, retryAttempt: true, cancellationToken);
                }
            }
            finally
            {
                if (!bAlreadyExists && !cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Finally releasing semaphore");
                    semaphore.Release();
                }
            }
        }
    }
}
