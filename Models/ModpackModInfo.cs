using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StellarModManager.Models
{
    public class ModpackModInfo : ModInfo
    {
        [JsonIgnore]
        public bool IsInstalled { get; set; }

        [JsonIgnore]
        ///<summary>Only set if <c>IsInstalled</c> is false</summary>
        public string? DownloadUrl { get; set; }

        public ModpackModInfo(ModInfo modInfo)
        {
            this.Id = modInfo.Id;
            this.Name = modInfo.Name;
            this.Version = modInfo.Version;
            this.Description = modInfo.Description;
            this.Author = modInfo.Author;
            this.IsInstalled = modInfo is InstalledModInfo;

            //If it isnt installed, it has a DownloadUrl, because InstalledModInfo is ruled out
            if (!this.IsInstalled) this.DownloadUrl = ((OnlineModInfo)modInfo).DownloadUrl;

        }
    }
}
