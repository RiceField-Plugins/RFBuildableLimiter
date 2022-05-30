using System;
using System.Xml.Serialization;

namespace RFBuildableLimiter.Models
{
    [Serializable]
    public class BuildableItem
    {
        [XmlAttribute]
        public ushort Id { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is not BuildableItem other)
                return false;
            
            return Id == other.Id;
        }

        protected bool Equals(BuildableItem other)
        {
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}