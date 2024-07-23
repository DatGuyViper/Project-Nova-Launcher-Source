using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using NovaLauncher.Core.Utilities;

namespace NovaLauncher.Core.Nova
{
    public class InstallationManager
    {
        private static InstallationManager _installationManager = new InstallationManager();

        private List<Installation> _installations;

        public InstallationManager()
        {
            string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NovaLauncher");
            string path = Path.Combine(text, "installations.json");
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            int num = 0;
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                try
                {
                    List<Installation> list = JsonExtension.TryDeserialize<List<Installation>>(json);
                    _installations = list ?? new List<Installation>();
                    num = _installations.Count;
                }
                catch (Exception)
                {
                    _installations = new List<Installation>();
                }
            }
            else
            {
                _installations = new List<Installation>();
            }
            if (_installations.Where((Installation installation) => Directory.Exists(installation.Path)).ToList().Count != num)
            {
                _installations = _installations
                    .Where(installation => Directory.Exists(installation.Path) && File.Exists(Path.Combine(installation.Path, "FortniteGame", "Binaries", "Win64", "FortniteClient-Win64-Shipping.exe")))
                    .ToList();
                Save();
            }
        }

        public void Refresh()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NovaLauncher", "installations.json");
            if (File.Exists(path))
            {
                List<Installation> list = JsonSerializer.Deserialize<List<Installation>>(File.ReadAllText(path));
                _installations = list ?? new List<Installation>();
            }
        }

        public List<Installation> GetInstallations()
        {
            return _installations;
        }

        public Installation? Find(string version)
        {
            return _installations.Find(x => x.Version == version);
        }

        public bool Add(Installation installation)
        {
            if (_installations.Find(x => x.Version == installation.Version) != null)
            {
                return false;
            }
            _installations.Add(installation);
            return true;
        }

        public async Task<bool> Select(string version)
        {
            int num = _installations.FindIndex(inst => inst.Selected);
            if (num != -1)
            {
                _installations[num].Selected = false;
            }
            int num2 = _installations.FindIndex(inst => inst.Version == version);
            if (num2 == -1)
            {
                return false;
            }
            _installations[num2].Selected = true;
            await Save();
            return true;
        }

        public bool Remove(Installation installation)
        {
            return _installations.Remove(installation);
        }

        public async Task Save()
        {
            string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NovaLauncher");
            string path = Path.Combine(text, "installations.json");
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            await File.WriteAllTextAsync(path, JsonSerializer.Serialize(_installations));
        }

        public static InstallationManager Get()
        {
            return _installationManager;
        }
    }
}
