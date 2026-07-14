using StellarModManager.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace StellarModManager.Services;

public class InstalledModpacksService
{
    public List<Modpack> GetInstalledModpacks(string libraryPath)
    {
        List<Modpack> modpacks = new();

        if (!Directory.Exists(libraryPath))
            return modpacks;

        foreach (string modpackFile in Directory.GetFiles(libraryPath))
        {

            string json = File.ReadAllText(modpackFile);
            Modpack? modpack = JsonSerializer.Deserialize<Modpack>(json);

            if (modpack != null)
                modpacks.Add(modpack);
        }

        return modpacks;
    }
}
