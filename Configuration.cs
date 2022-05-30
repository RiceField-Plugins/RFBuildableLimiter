using System.Collections.Generic;
using RFBuildableLimiter.Models;
using Rocket.API;

namespace RFBuildableLimiter
{
    public class Configuration : IRocketPluginConfiguration
    {
        public bool Enabled;
        public string MessageColor;
        public string MessageIconUrl;
        public QuantityLimiterConfig QuantityOptions;
        public HeightLimiterConfig HeightOptions;

        public void LoadDefaults()
        {
            Enabled = true;
            MessageColor = "green";
            MessageIconUrl = "https://cdn.jsdelivr.net/gh/RiceField-Plugins/UnturnedImages@images/plugin/Announcer.png";
            QuantityOptions = new QuantityLimiterConfig
            {
                Enabled = true,
                IgnoreAdmins = true,
                DefaultBarricadeLimit = 50,
                DefaultStructureLimit = 50,
                // EnableGroupBuildableLimiter = true,
                // DefaultGroupBarricadeLimit = 500,
                // DefaultGroupStructureLimit = 500,
                IgnoredIDs = new HashSet<BuildableItem>
                {
                    new() { Id = 1 },
                    new() { Id = 2 },
                },
                WarningPercentage = 90,
                // GroupIgnoredIDs = new HashSet<BuildableItem>
                // {
                    // new() { Id = 1 },
                    // new() { Id = 2 },
                // }
            };
            HeightOptions = new HeightLimiterConfig
            {
                Enabled = true,
                IgnoreAdmins = true,
                DefaultBarricadeLimit = 50,
                DefaultStructureLimit = 50,
                IgnoredIDs = new HashSet<BuildableItem>
                {
                    new() { Id = 1 },
                    new() { Id = 2 },
                },
                WarningPercentage = 90,
            };
        }
    }
}