using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StellarModManager.Models;
using StellarModManager.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StellarModManager.ViewModels;

public partial class MainWindowViewModel
{
    [ObservableProperty]
    private Modpack? currentlyViewedModpack;

    [ObservableProperty]
    public bool isModpackContentPanelOpen = false;

    [RelayCommand]
    private void OpenModpackContenView(Modpack modpack)
    {
        CurrentlyViewedModpack = modpack;
        IsModpackContentPanelOpen = true;
    }

    [RelayCommand]
    private void CloseModpackContentView(Modpack modpack)
    {
        IsModpackContentPanelOpen = false;
    }

    [RelayCommand]
    private void RemoveModFromCurrentModpack(ModpackModInfo mod)
    {
        CurrentlyViewedModpack?.Mods.Remove(mod);
    }
}