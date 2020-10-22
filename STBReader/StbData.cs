using System;
using System.Collections.Generic;
using System.Xml.Linq;
using STBReader.Member;
using STBReader.Model;
using STBReader.Section;

namespace STBReader
{
    public class StbData
    {
        public readonly double ToleLength;
        public readonly double ToleAngle;

        private readonly string _xmlns;
        private readonly StbVersion _version;
        public StbNodes Nodes;
        public StbColumns Columns;
        public StbPosts Posts;
        public StbGirders Girders;
        public StbBeams Beams;
        public StbBraces Braces;
        public StbSlabs Slabs;
        public StbWalls Walls;

        public StbSecColumnRc SecColumnRc;
        public StbSecBeamRc SecBeamRc;
        public StbSecColumnS SecColumnS;
        public StbSecBeamS SecBeamS;
        public StbSecBraceS SecBraceS;
        public StbSecSteel SecSteel;

        public StbData(string path, double toleLength, double toleAngle)
        {
            ToleLength = toleLength;
            ToleAngle = toleAngle;
            
            try
            {
                XDocument xDocument = XDocument.Load(path);
                XElement root = xDocument.Root;

                if (root != null)
                {
                    if (root.Attribute("xmlns") != null)
                    {
                        _xmlns = "{" + (string)root.Attribute("xmlns") + "}";
                    }
                    else
                    {
                        _xmlns = string.Empty;
                    }

                    var tmp = (string) root.Attribute("version");
                    switch (tmp.Split('.')[0])
                    {
                        case "1":
                            _version = StbVersion.Ver1;
                            break;
                        case "2":
                            _version = StbVersion.Ver2;
                            break;
                        default:
                            throw new ArgumentException("The STB version is not set");
                    }
                }
                Init();
                Load(xDocument);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        private void Init()
        {
            Nodes = new StbNodes();
            Columns = new StbColumns();
            Posts = new StbPosts();
            Girders = new StbGirders();
            Beams = new StbBeams();
            Braces = new StbBraces();
            Slabs = new StbSlabs();
            Walls = new StbWalls();
            SecColumnRc = new StbSecColumnRc();
            SecBeamRc = new StbSecBeamRc();
            SecColumnS = new StbSecColumnS();
            SecBeamS = new StbSecBeamS();
            SecBraceS = new StbSecBraceS();
            SecSteel = new StbSecSteel();
        }
        
        private void Load(XDocument xDoc)
        {
            var members = new List<IStbLoader>
            {
                Nodes, Slabs, Walls,
                Columns, Posts, Girders, Beams, Braces,
                SecColumnRc, SecColumnS, SecBeamRc, SecBeamS, SecBraceS, SecSteel
            };

            foreach (IStbLoader member in members)
            {
                member.Load(xDoc, _version, _xmlns);
            }
        }
        
        public enum StbVersion
        {
            Ver1,
            Ver2
        }
    }
}