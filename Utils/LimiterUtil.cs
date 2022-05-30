using System;
using System.Linq;
using RFBuildableLimiter.Enums;
using RFBuildableLimiter.Models;
using RFRocketLibrary.Helpers;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace RFBuildableLimiter.Utils
{
    internal static class LimiterUtil
    {
        internal static uint GetBestPermValue(IRocketPlayer player, ELimitType type)
        {
            var prefix = string.Empty;
            uint defaultValue = 0;
            var valueStr = string.Empty;
            switch (type)
            {
                case ELimitType.BARRICADE_QUANTITY:
                    prefix = Permissions.BarricadeQuantity;
                    defaultValue = Plugin.Conf.QuantityOptions.DefaultBarricadeLimit;
                    valueStr = "quantity";
                    break;
                case ELimitType.STRUCTURE_QUANTITY:
                    prefix = Permissions.StructureQuantity;
                    defaultValue = Plugin.Conf.QuantityOptions.DefaultStructureLimit;
                    valueStr = "quantity";
                    break;
                case ELimitType.BARRICADE_HEIGHT:
                    prefix = Permissions.BarricadeHeight;
                    defaultValue = Plugin.Conf.HeightOptions.DefaultBarricadeLimit;
                    valueStr = "height";
                    break;
                case ELimitType.STRUCTURE_HEIGHT:
                    prefix = Permissions.StructureHeight;
                    defaultValue = Plugin.Conf.HeightOptions.DefaultBarricadeLimit;
                    valueStr = "height";
                    break;
            }

            var permissions = player.GetPermissions().Select(a => a.Name).Where(p =>
                p.ToLower().StartsWith(prefix) && !p.Equals(prefix, StringComparison.OrdinalIgnoreCase)).ToList();
            if (permissions.Count == 0)
                return defaultValue;

            uint bestValue = 0;
            foreach (var perm in permissions)
            {
                var pocketSplit = perm.Split('.');
                if (pocketSplit.Length != 3)
                {
                    Logger.LogError($"[{Plugin.Inst.Name}] Invalid permission format: {perm}");
                    Logger.LogError($"[{Plugin.Inst.Name}] Correct format: {prefix}'{valueStr}'");
                    continue;
                }

                uint.TryParse(pocketSplit.ElementAtOrDefault(2), out var result);
                if (result > bestValue)
                    bestValue = result;
            }

            return bestValue;
        }

        internal static uint GetOwnerBuildableCount(ulong owner, ELimitType type)
        {
            uint result = 0;
            switch (type)
            {
                case ELimitType.BARRICADE_QUANTITY:
                    foreach (var region in BarricadeManager.regions)
                    {
                        foreach (var drop in region.drops.ToList())
                        {
                            if (drop.GetServersideData().owner != owner)
                                continue;

                            result++;
                        }
                    }
                    
                    foreach (var region in BarricadeManager.vehicleRegions)
                    {
                        foreach (var drop in region.drops.ToList())
                        {
                            if (drop.GetServersideData().owner != owner)
                                continue;

                            result++;
                        }
                    }
                    break;
                case ELimitType.STRUCTURE_QUANTITY:
                    foreach (var region in StructureManager.regions)
                    {
                        foreach (var drop in region.drops.ToList())
                        {
                            if (drop.GetServersideData().owner != owner)
                                continue;

                            result++;
                        }
                    }
                    break;
            }

            return result;
        }

        internal static void CheckBuildableLimit(ItemAsset asset, Vector3 point, ulong owner, ulong group,
            ref bool shouldAllow)
        {
            if (!shouldAllow)
                return;

            var isBarricade = asset is ItemBarricadeAsset;
            if (Plugin.Conf.HeightOptions.Enabled)
            {
                if (!Plugin.Conf.HeightOptions.IgnoredIDs.Contains(new BuildableItem { Id = asset.id }))
                {
                    var bOwner = owner;
                    var steamPlayer = Provider.clients.FirstOrDefault(x => x.playerID.steamID.m_SteamID == bOwner);
                    if (steamPlayer != null)
                    {
                        if (!Plugin.Conf.HeightOptions.IgnoreAdmins || !steamPlayer.isAdmin)
                        {
                            Physics.Raycast(new Ray(point + new Vector3(0, 2, 0), -Vector3.up),
                                out var raycastHit,
                                float.MaxValue, RayMasks.GROUND | RayMasks.GROUND2);
                            if (raycastHit.transform != null)
                            {
                                var currentValue = Vector3.Distance(point, raycastHit.point);
                                switch (isBarricade)
                                {
                                    case true:
                                        var limitValue = GetBestPermValue(UnturnedPlayer.FromPlayer(steamPlayer.player),
                                            ELimitType.BARRICADE_HEIGHT);
                                        if (currentValue > limitValue)
                                        {
                                            shouldAllow = false;
                                            ChatHelper.Say(steamPlayer,
                                                Plugin.TranslateRich(EResponse.BARRICADE_HEIGHT_LIMIT, currentValue,
                                                    limitValue), Plugin.MsgColor,
                                                Plugin.Conf.MessageIconUrl);
                                            return;
                                        }

                                        if (currentValue >= Plugin.Conf.HeightOptions.WarningPercentage / 100f * limitValue)
                                        {
                                            ChatHelper.Say(steamPlayer,
                                                Plugin.TranslateRich(EResponse.BARRICADE_HEIGHT_WARNING, currentValue,
                                                    limitValue), Plugin.MsgColor,
                                                Plugin.Conf.MessageIconUrl);
                                        }

                                        break;
                                    case false:
                                        limitValue = GetBestPermValue(UnturnedPlayer.FromPlayer(steamPlayer.player),
                                            ELimitType.STRUCTURE_HEIGHT);
                                        if (currentValue > limitValue)
                                        {
                                            shouldAllow = false;
                                            ChatHelper.Say(steamPlayer,
                                                Plugin.TranslateRich(EResponse.STRUCTURE_HEIGHT_LIMIT, currentValue,
                                                    limitValue), Plugin.MsgColor,
                                                Plugin.Conf.MessageIconUrl);
                                            return;
                                        }

                                        if (currentValue >= Plugin.Conf.HeightOptions.WarningPercentage / 100f * limitValue)
                                        {
                                            ChatHelper.Say(steamPlayer,
                                                Plugin.TranslateRich(EResponse.STRUCTURE_HEIGHT_WARNING, currentValue,
                                                    limitValue), Plugin.MsgColor,
                                                Plugin.Conf.MessageIconUrl);
                                        }

                                        break;
                                }
                            }
                            else
                            {
                                Logger.LogWarning($"[{Plugin.Inst.Name}] [DEBUG] {owner}'s Ground is null");
                            }
                        }
                    }
                }
            }

            if (Plugin.Conf.QuantityOptions.Enabled)
            {
                if (!Plugin.Conf.QuantityOptions.IgnoredIDs.Contains(new BuildableItem { Id = asset.id }))
                {
                    var bOwner = owner;
                    var steamPlayer = Provider.clients.FirstOrDefault(x => x.playerID.steamID.m_SteamID == bOwner);
                    if (steamPlayer != null)
                    {
                        if (!Plugin.Conf.QuantityOptions.IgnoreAdmins || !steamPlayer.isAdmin)
                        {
                            switch (isBarricade)
                            {
                                case true:
                                    var currentValue = GetOwnerBuildableCount(bOwner, ELimitType.BARRICADE_QUANTITY);
                                    var limitValue = GetBestPermValue(UnturnedPlayer.FromPlayer(steamPlayer.player),
                                        ELimitType.BARRICADE_QUANTITY);
                                    if (currentValue >= limitValue)
                                    {
                                        shouldAllow = false;
                                        ChatHelper.Say(steamPlayer,
                                            Plugin.TranslateRich(EResponse.BARRICADE_QUANTITY_LIMIT, currentValue, limitValue), Plugin.MsgColor,
                                            Plugin.Conf.MessageIconUrl);
                                        return;
                                    }

                                    if (currentValue >= Plugin.Conf.HeightOptions.WarningPercentage / 100f * limitValue)
                                    {
                                        ChatHelper.Say(steamPlayer,
                                            Plugin.TranslateRich(EResponse.BARRICADE_QUANTITY_LIMIT, currentValue,
                                                limitValue), Plugin.MsgColor,
                                            Plugin.Conf.MessageIconUrl);
                                    }

                                    break;
                                case false:
                                    currentValue = GetOwnerBuildableCount(bOwner, ELimitType.STRUCTURE_QUANTITY);
                                    limitValue = GetBestPermValue(UnturnedPlayer.FromPlayer(steamPlayer.player),
                                        ELimitType.STRUCTURE_QUANTITY);
                                    if (currentValue >= limitValue)
                                    {
                                        shouldAllow = false;
                                        ChatHelper.Say(steamPlayer,
                                            Plugin.TranslateRich(EResponse.STRUCTURE_QUANTITY_LIMIT, currentValue, limitValue), Plugin.MsgColor,
                                            Plugin.Conf.MessageIconUrl);
                                        return;
                                    }

                                    if (currentValue >= Plugin.Conf.HeightOptions.WarningPercentage / 100f * limitValue)
                                    {
                                        ChatHelper.Say(steamPlayer,
                                            Plugin.TranslateRich(EResponse.STRUCTURE_QUANTITY_WARNING, currentValue,
                                                limitValue), Plugin.MsgColor,
                                            Plugin.Conf.MessageIconUrl);
                                    }

                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}