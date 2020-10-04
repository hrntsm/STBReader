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
                return;
            
            var stbElems = stbFile.Root.Descendants(xmlns + Tag);
            foreach (var stbElem in stbElems)
            {
                ElementLoader(stbElem, version, xmlns);
            }
        }

        protected virtual void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            Name.Add((string) stbElem.Attribute("name"));
        }
    }

    public struct Point
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
