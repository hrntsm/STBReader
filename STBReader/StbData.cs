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
        private readonly double _toleLength;
        private readonly double _toleAngle;

        private readonly string _xmlns;
        private readonly StbVersion _version;
        private StbNodes _nodes;
        private StbColumns _columns;
        private StbPosts _posts;
        private StbGirders _girders;
        private StbBeams _beams;
        private StbBraces _braces;
        private StbSlabs _slabs;
        private StbWalls _walls;

        private StbSecColumnRc _secColumnRc;
        private StbSecBeamRc _secBeamRc;
        private StbSecColumnS _secColumnS;
        private StbSecBeamS _secBeamS;
        private StbSecBraceS _secBraceS;
        private StbSecSteel _secSteel;

        public StbData(string path, double toleLength, double toleAngle)
        {
            _toleLength = toleLength;
            _toleAngle = toleAngle;
            
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
                        throw new Exception("The STB version is not set");
                }
            }
            Init();
            Load(xDocument);
        }
        
        private void Init()
        {
            _nodes = new StbNodes();
            _columns = new StbColumns();
            _posts = new StbPosts();
            _girders = new StbGirders();
            _beams = new StbBeams();
            _braces = new StbBraces();
            _slabs = new StbSlabs();
            _walls = new StbWalls();
            _secColumnRc = new StbSecColumnRc();
            _secBeamRc = new StbSecBeamRc();
            _secColumnS = new StbSecColumnS();
            _secBeamS = new StbSecBeamS();
            _secBraceS = new StbSecBraceS();
            _secSteel = new StbSecSteel();
        }
        
        private void Load(XDocument xDoc)
        {
            var members = new List<IStbLoader>
            {
                _nodes, _slabs, _walls,
                _columns, _posts, _girders, _beams, _braces,
                _secColumnRc, _secColumnS, _secBeamRc, _secBeamS, _secBraceS, _secSteel
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