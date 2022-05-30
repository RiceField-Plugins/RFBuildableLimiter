using System.Linq;
using RFBuildableLimiter.Models;
using RFBuildableLimiter.Enums;
using RFBuildableLimiter.EventListeners;
using RFRocketLibrary.Helpers;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace RFBuildableLimiter
{
    public class Plugin : RocketPlugin<Configuration>
    {
        private static int Major = 1;
        private static int Minor = 0;
        private static int Patch = 0;

        public static Plugin Inst;
        public static Configuration Conf;
        internal static Color MsgColor;

        protected override void Load()
        {
            Inst = this;
            Conf = Configuration.Instance;
            if (Conf.Enabled)
            {
                MsgColor = UnturnedChat.GetColorFromName(Conf.MessageColor, Color.green);

                BarricadeManager.onDeployBarricadeRequested += ServerEvent.OnPreBuildbaleDeployed;
                StructureManager.onDeployStructureRequested += ServerEvent.OnPreBuildbaleDeployed;
            }
            else
                Logger.LogWarning($"[{Name}] Plugin: DISABLED");

            Logger.LogWarning($"[{Name}] Plugin loaded successfully!");
            Logger.LogWarning($"[{Name}] {Name} v{Major}.{Minor}.{Patch}");
            Logger.LogWarning($"[{Name}] Made with 'rice' by RiceField Plugins!");
        }

        protected override void Unload()
        {
            if (Conf.Enabled)
            {
                BarricadeManager.onDeployBarricadeRequested -= ServerEvent.OnPreBuildbaleDeployed;
                StructureManager.onDeployStructureRequested -= ServerEvent.OnPreBuildbaleDeployed;
            }

            Conf = null;
            Inst = null;

            Logger.LogWarning($"[{Name}] Plugin unloaded successfully!");
        }

        public override TranslationList DefaultTranslations => new()
        {
            { $"{EResponse.BARRICADE_HEIGHT_LIMIT}", "You are not allowed to build barricade at this height! Current height: {0} ● Max height: {1}" },
            { $"{EResponse.BARRICADE_HEIGHT_WARNING}", "You are about to reach barricade height limit! Current height: {0} ● Max height: {1}" },
            { $"{EResponse.BARRICADE_QUANTITY_LIMIT}", "You have reached barricade build limit! Current barricades: {0} ● Max barricades: {1}" },
            { $"{EResponse.BARRICADE_QUANTITY_WARNING}", "You are about to reach barricade build limit! Current barricades: {0} ● Max barricades: {1}" },
            { $"{EResponse.STRUCTURE_HEIGHT_LIMIT}", "You are not allowed to build structure at this height! Current height: {0} ● Max height: {1}" },
            { $"{EResponse.STRUCTURE_HEIGHT_WARNING}", "You are about to reach structure height limit! Current height: {0} ● Max height: {1}" },
            { $"{EResponse.STRUCTURE_QUANTITY_LIMIT}", "You have reached structure build limit! Current structures: {0} ● Max structures: {1}" },
            { $"{EResponse.STRUCTURE_QUANTITY_WARNING}", "You are about to reach structure build limit! Current structures: {0} ● Max barricades: {1}" },
        };

        internal static string TranslateRich(object s, params object[] objects) =>
            Inst.Translate(s.ToString(), objects).Replace("-=", "<").Replace("=-", ">");
    }
}