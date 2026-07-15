using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace StellarModManager.Models;

public partial class Modpack : ObservableObject
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("version")]
    public string Version { get; set; } = "";

    [JsonPropertyName("modIDs")]
    public ObservableCollection<string> ModIDs { get; set; } = new();

    [JsonIgnore] //Filled at Runtime
    public ObservableCollection<ModpackModInfo> Mods { get; set; } = new();

    [JsonIgnore]
    public string ModpackFilePath { get; set; } = "";

    // UI 
    [JsonIgnore]
    [ObservableProperty]
    private bool isDeploying;

    [JsonIgnore]
    [ObservableProperty]
    private double deployProgress;

    [JsonIgnore]
    [ObservableProperty]
    private bool isRemoving;

    [JsonIgnore]
    [ObservableProperty]
    private bool isUpdating;

    [JsonIgnore]
    [ObservableProperty]
    private double updateProgress;

    [JsonIgnore]
    [ObservableProperty]
    private bool isUpdateAvailable;

    [JsonIgnore]
    [ObservableProperty]
    private string? latestVersion;
}

