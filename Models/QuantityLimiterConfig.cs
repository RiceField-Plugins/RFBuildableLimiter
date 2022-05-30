using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RFBuildableLimiter.Models
{
    [Serializable]
    public class QuantityLimiterConfig
    {
        [XmlAttribute]
        public bool Enabled;
        [XmlAttribute]
        public bool IgnoreAdmins;
        [XmlAttribute]
        public uint DefaultBarricadeLimit;
        [XmlAttribute]
        public uint DefaultStructureLimit;
        // public bool EnableGroupBuildableLimiter;
        // public uint DefaultGroupBarricadeLimit;
        // public uint DefaultGroupStructureLimit;
        public HashSet<BuildableItem> IgnoredIDs;
        // public HashSet<BuildableItem> GroupIgnoredIDs;
        public byte WarningPercentage;
    }
}