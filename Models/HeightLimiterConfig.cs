using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RFBuildableLimiter.Models
{
    [Serializable]
    public class HeightLimiterConfig
    {
        [XmlAttribute]
        public bool Enabled;
        [XmlAttribute]
        public bool IgnoreAdmins;
        [XmlAttribute]
        public uint DefaultBarricadeLimit;
        [XmlAttribute]
        public uint DefaultStructureLimit;
        public HashSet<BuildableItem> IgnoredIDs;
        public byte WarningPercentage;
    }
}