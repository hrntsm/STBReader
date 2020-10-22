using System;
using System.Collections.Generic;
using System.Xml.Linq;
using static STBReader.StbData;

namespace STBReader
{
    internal interface IStbLoader
    {
        void Load(XDocument stbFile, StbVersion version, string xmlns);
    }
    
    public class StbBase: IStbLoader
    {
        /// <summary>
        /// STBデータ内のタグ
        /// </summary>
        public virtual string Tag { get; } = "StbBase";
        /// <summary>
        /// GUID
        /// </summary>
        public List<string> Guid { get; } = new List<string>();
        /// <summary>
        /// 部材の名前
        /// </summary>
        public List<string> Name { get; } = new List<string>();

        public virtual void Load(XDocument stbFile, StbVersion version, string xmlns)
        {
            if (stbFile.Root == null)
            {
                return;
            }

            IEnumerable<XElement> stbElems = stbFile.Root.Descendants(xmlns + Tag);
            foreach (XElement stbElem in stbElems)
            {
                ElementLoader(stbElem, version, xmlns);
            }
        }

        protected virtual void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            Name.Add((string) stbElem.Attribute("name"));
        }
    }

    public readonly struct Point3 : IEquatable<Point3>
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public Point3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public bool Equals(Point3 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        public override bool Equals(object obj)
        {
            return obj is Point3 other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }
    }
}
