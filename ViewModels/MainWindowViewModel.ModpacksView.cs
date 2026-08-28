using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StellarModManager.Models;
using StellarModManager.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StellarModManager.ViewModels;

public partial class MainWindowViewModel
{
    public ObservableCollection<Modpack> InstalledModpacks { get; } = new();

    private readonly InstalledModpacksService installedModpacksService = new();

    private readonly string modpackLibraryPath = Path.Combine(AppContext.BaseDirectory, "ModpackLibrary");

    private void LoadInstalledModpacks()
    {
        InstalledModpacks.Clear();

        var modpacks = installedModpacksService.GetInstalledModpacks(modpackLibraryPath);

        
        foreach (Modpack modpack in modpacks)
        {
            InstalledModpacks.Add(modpack);
            FillModpackModsCollection(modpack);
        }
    }

    private async Task FillModpackModsCollection(Modpack modpack)
    {
        modpack.Mods.Clear();

        List<OnlineModInfo>? onlineMods = null;

        foreach (string modID in modpack.ModIDs)
        {
            bool modAdded = false;
            foreach (ModInfo modInfo in InstalledMods)
            {
                if (modInfo.Id == modID)
                {
                    modpack.Mods.Add(new ModpackModInfo(modInfo));
                    modAdded = true;
                    break;
                }
            }

            if (modAdded) continue; //Dont check online mods if already found in InstalledMods

            //If onlineMods null -> GetModsAsync from Online Repository
            onlineMods ??= await new ModRepositoryService().GetModsAsync(ModRepositoryUrl);

            foreach (ModInfo modInfo in onlineMods)
            {
                if (modInfo.Id == modID)
                {
                    modpack.Mods.Add(new ModpackModInfo(modInfo));
                    modAdded = true;
                    break;
                }
            }

            //TODO: Add a status text or some feedback if a mod from the modIds was not found at all
        }
    }

    [RelayCommand]
    private void ViewModpack(Modpack modpack)
    {
        OpenModpackContenView(modpack);
    }

    [RelayCommand]
    private void OpenModpackDirectory()
    {
        try
        {
            Process.Start("explorer.exe",modpackLibraryPath);
        }
        catch
        {

        }
    }

    [RelayCommand]
    private async Task DeplayCurrentlyViewedModpack()
    {
        if (CurrentlyViewedModpack == null) return;
        await DeployModpack(CurrentlyViewedModpack);
    }

    [RelayCommand]
    private async Task DeployModpack(Modpack modpack)
    {
        if (GamePath == "No game selected")
        {
            MelonLoaderStatusText = "Select a game first";
            return;
        }

        modpack.IsDeploying = true;
        modpack.DeployProgress = 0;

        try //TODO: Status Texts for removing previously deployed mods, deploying modpack, .. in Modpack Content Window
        {
            List<Task> tasks = new();

            //Download missing mods
            string downloadsDir = Path.Combine(AppContext.BaseDirectory, "Downloads");

            foreach (ModpackModInfo modInfo in modpack.Mods)
            {
                Debug.WriteLine("Downloading " + modInfo.Name);
                if(!modInfo.IsInstalled)
                {
                    string zipFile = Path.Combine(downloadsDir, $"{modInfo.Id}.zip");
                    string libraryPath = Path.Combine(AppContext.BaseDirectory, "Library", modInfo.Id);

                    //TODO: Add Progress bars to everything in this method
#pragma warning disable CS8604 // modInfo.DownloadUrl is not null because !modInfo.IsInstalled
                    tasks.Add(repositoryService.DownloadModAsync(modInfo.DownloadUrl, zipFile));
#pragma warning restore CS8604
                    tasks.Add(installerService.InstallAsync(zipFile, libraryPath));
                }
            }

            await Task.WhenAll(tasks); //Wait for Downloads to finish

            //Remove already deployed Mods
            string[] deployedModLibraryPaths = FindAllDeployedMods();
            foreach(string path in deployedModLibraryPaths)
            {
                Debug.WriteLine("Removing Deployed " + path);
                deploymentService.RemoveDeployedFiles(path, GamePath);
            }

            //Deploy all Mods
            foreach (ModpackModInfo modInfo in modpack.Mods)
            {
                Debug.WriteLine("Deploying " + modInfo.Name);
                string libraryPath = Path.Combine(AppContext.BaseDirectory, "Library", modInfo.Id);
                var progress = new Progress<double>(pct => modpack.DeployProgress = pct);

                tasks.Add(Task.Run(() => deploymentService.DeployMod(libraryPath, GamePath, progress)));
            }

            Debug.WriteLine("Deployed Modpack " + modpack.Name);
            MelonLoaderStatusText = $"{modpack.Name} copied to game";
        }
        catch (Exception ex)
        {
            MelonLoaderStatusText = $"Deploy failed: {ex.Message}";
        }
        finally
        {
            modpack.IsDeploying = false;
        }
    }

    /// <summary>
    /// Returns an array of paths to the mod folders in the library of the mods that are deployed.
    /// </summary>
    private string[] FindAllDeployedMods()
    {
        //Find all folders with manifest files
        string library = Path.Combine(AppContext.BaseDirectory, "Library");

        string[] manifestPaths = Directory.GetFiles(library,$"*{ModDeploymentService.ManifestFileName}");

        string[] folderPaths = new string[manifestPaths.Length];

        for (int i = 0; i < manifestPaths.Length; i++)
        {
            folderPaths[i] = Path.GetDirectoryName(manifestPaths[i]) ?? "";
        }

        return folderPaths;
    }

    [RelayCommand]
    private async Task UninstallModpack(InstalledModInfo mod)
    {
        
    }

    [RelayCommand]
    private void RequestRemoveModpack(Modpack modpack)
    {
        //Refactor Confirm / Cancel before remove dialogue so its usable for anything

        //if (ConfirmBeforeRemove)
        //{
        //    modPendingRemoval = mod;
        //    ConfirmRemoveMessage = LocalizationService.Instance.Format("ConfirmRemoveMessage", mod.Name);
        //    IsConfirmRemoveOpen = true;
        //}
        //else
        //{
        //    _ = RemoveModpack(mod);
        //}
    }

    [RelayCommand]
    private async Task RemoveModpack(Modpack modpack)
    {

    }
}