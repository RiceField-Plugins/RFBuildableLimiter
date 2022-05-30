using System.Linq;
using RFBuildableLimiter.Enums;
using RFBuildableLimiter.Models;
using RFBuildableLimiter.Utils;
using RFRocketLibrary.Helpers;
using SDG.Unturned;
using UnityEngine;

namespace RFBuildableLimiter.EventListeners
{
    internal static class ServerEvent
    {
        internal static void OnPreBuildbaleDeployed(Barricade barricade, ItemBarricadeAsset asset, Transform hit,
            ref Vector3 point, ref float angle_x, ref float angle_y, ref float angle_z, ref ulong owner,
            ref ulong group, ref bool shouldallow)
        {
            LimiterUtil.CheckBuildableLimit(asset, point, owner, group, ref shouldallow);
        }

        public static void OnPreBuildbaleDeployed(Structure structure, ItemStructureAsset asset, ref Vector3 point,
            ref float angle_x, ref float angle_y, ref float angle_z, ref ulong owner, ref ulong group,
            ref bool shouldallow)
        {
            LimiterUtil.CheckBuildableLimit(asset, point, owner, group, ref shouldallow);
        }
    }
}