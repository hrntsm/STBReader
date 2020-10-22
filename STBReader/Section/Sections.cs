﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;
using static STBReader.StbData;

namespace STBReader.Section
{
    /// <summary>
    /// RC柱断面
    /// </summary>
    public class StbSecColumnRc : StbRcSections
    {
        public override string Tag { get; } = "StbSecColumn_RC";
        /// <summary>
        /// 部材が主柱か間柱かの区別
        /// </summary>
        public List<KindsColumn> KindColumn { get; } = new List<KindsColumn>();
        /// <summary>
        /// 部材幅
        /// </summary>
        public List<double> Width { get; } = new List<double>();
        /// <summary>
        /// 部材高さ
        /// </summary>
        public List<double> Height { get; } = new List<double>();
        /// <summary>
        /// 部材が矩形であるかどうか
        /// </summary>
        public List<bool> IsRect { get; } = new List<bool>();

        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
            if (stbElem.Attribute("kind_column") != null)
            {
                KindColumn.Add((string) stbElem.Attribute("kind_column") == "COLUMN"
                    ? KindsColumn.Column
                    : KindsColumn.Post);
            }
            else
            {
                KindColumn.Add(KindsColumn.Column);
            }

            // 子要素 StbSecFigure
            var stbColSecFigure = new StbColSecFigure();
            stbColSecFigure.Load(stbElem, version, xmlns);
            Width.Add(stbColSecFigure.Width);
            Height.Add(stbColSecFigure.Height);
            IsRect.Add(stbColSecFigure.IsRect);
        }
    }

    /// <summary>
    /// RCとSRCの柱断面形状
    /// </summary>
    class StbColSecFigure
    {
        public double Width { get; private set; }
        public double Height { get; private set; }
        public bool IsRect { get; private set; }

        /// <summary>
        /// 与えられたstbデータからRC柱断面の形状を取得する。
        /// </summary>
        /// <param name="stbColumn"></param>
        /// <param name="version"></param>
        /// <param name="xmlns"></param>
        public void Load(XElement stbColumn, StbVersion version, string xmlns)
        {
            XElement stbFigure;
            switch (version)
            {
                case StbVersion.Ver1:
                    stbFigure = stbColumn.Element("StbSecFigure");
                    if (stbFigure != null && stbFigure.Element("StbSecRect") != null)
                    {
                        Width = (double)stbFigure.Element("StbSecRect")?.Attribute("DX");
                        Height = (double)stbFigure.Element("StbSecRect")?.Attribute("DY");
                        IsRect = true;
                    }
                    else if (stbFigure != null && stbFigure.Element("StbSecCircle") != null)
                    {
                        Width = (double)stbFigure.Element("StbSecCircle")?.Attribute("D");
                        Height = 0;
                        IsRect = false;
                    }
                    else
                    {
                        Width = 0;
                        Height = 0;
                        IsRect = false;
                    }
                    break;
                case StbVersion.Ver2:
                    string tag;
                    stbFigure = stbColumn.Element(xmlns + "StbSecFigureColumn_RC");
                    if (stbFigure != null && stbFigure.Element(xmlns + "StbSecColumn_RC_Rect") != null)
                    {
                        tag = xmlns + "StbSecColumn_RC_Rect";
                        Width = (double)stbFigure.Element(tag)?.Attribute("width_X");
                        Height = (double)stbFigure.Element(tag)?.Attribute("width_Y");
                        IsRect = true;
                    }
                    else if (stbFigure != null && stbFigure.Element(xmlns + "StbSecColumn_RC_Circle") != null)
                    {
                        tag = xmlns + "StbSecColumn_RC_Circle";
                        Width = (double)stbFigure.Element(tag)?.Attribute("D");
                        Height = 0;
                        IsRect = false;
                    }
                    else
                    {
                        Width = 0;
                        Height = 0;
                        IsRect = false;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, "The STB version is not set");
            }
        }
    }

    /// <summary>
    /// RC柱の配筋情報
    /// </summary>
    class StbColSecBarArrangement
    {
        public List<double> BarList { get; } = new List<double>();

        public void Load(XElement stbColumn, StbVersion version, string xmlns)
        {
            switch (version)
            {
                case StbVersion.Ver1:
                    string elementName;
                    XElement stbBar = stbColumn.Element("StbSecBar_Arrangement");
                    if (stbBar == null)
                        break;

                    if (stbBar.Element("StbSecRect_Column_Same") != null)
                    {
                        elementName = "StbSecRect_Column_Same";
                    }
                    else if (stbBar.Element("StbSecRect_Column_Not_Same") != null)
                    {
                        elementName = "StbSecRect_Column_Not_Same";
                    }
                    else if (stbBar.Element("StbSecCircle_Column_Same") != null)
                    {
                        elementName = "StbSecCircle_Column_Same";
                    }
                    else if (stbBar.Element("StbSecCircle_Column_Not_Same") != null)
                    {
                        elementName = "StbSecCircle_Column_Not_Same";
                    }
                    else
                    {
                        BarList.AddRange(new List<double> { 2, 2, 0, 0, 4, 200, 2, 2 });
                        return;
                    }

                    XElement stbBarElem = stbBar.Element(elementName);
                    if (stbBarElem == null)
                    {
                        break;
                    }

                    // Main 1
                    if (stbBarElem.Attribute("count_main_X_1st") != null)
                    {
                        BarList.Add((double)stbBarElem.Attribute("count_main_X_1st"));
                    }
                    else
                    {
                        BarList.Add(0);
                    }

                    if (stbBarElem.Attribute("count_main_Y_1st") != null)
                    {
                        BarList.Add((double)stbBarElem.Attribute("count_main_Y_1st"));
                    }
                    else
                    {
                        BarList.Add(0);
                    }

                    // Main2
                    if (stbBarElem.Attribute("count_main_X_2nd") != null)
                    {
                        BarList.Add((double)stbBarElem.Attribute("count_main_X_2nd"));
                    }
                    else
                    {
                        BarList.Add(0);
                    }

                    if (stbBarElem.Attribute("count_main_Y_2nd") != null)
                    {
                        BarList.Add((double)stbBarElem.Attribute("count_main_Y_2nd"));
                    }
                    else
                    {
                        BarList.Add(0);
                    }

                    // Main total
                    if (stbBarElem.Attribute("count_main_total") != null)
                    {
                        BarList.Add((double)stbBarElem.Attribute("count_main_total"));
                    }
                    else
                    {
                        BarList.Add(0);
                    }

                    // Band
                    if (stbBarElem.Attribute("pitch_band") != null)
                    {
                        BarList.Add((double)stbBarElem.Attribute("pitch_band"));
                    }
                    else
                    {
                        BarList.Add(0);
                    }

                    if (stbBarElem.Attribute("count_band_dir_X") != null)
                    {
                        BarList.Add((double)stbBarElem.Attribute("count_band_dir_X"));
                    }
                    else
                    {
                        BarList.Add(0);
                    }

                    if (stbBarElem.Attribute("count_band_dir_Y") != null)
                    {
                        BarList.Add((double)stbBarElem.Attribute("count_band_dir_Y"));
                    }
                    else
                    {
                        BarList.Add(0);
                    }
                    break;
                case StbVersion.Ver2:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, "The STB version is not set");
            }
        }
    }

    /// <summary>
    /// S柱断面
    /// </summary>
    public class StbSecColumnS : StbSteelSections
    {
        public override string Tag { get; } = "StbSecColumn_S";
        /// <summary>
        /// 部材が主柱か間柱かの区別
        /// </summary>
        public List<KindsColumn> KindColumn { get; } = new List<KindsColumn>();
        /// <summary>
        /// StbSecSteelの基準方向が部材座標系Xかどうか
        /// </summary>
        public List<bool> Direction { get; } = new List<bool>();
        /// <summary>
        /// 柱脚の形式
        /// </summary>
        public List<BaseTypes> BaseType { get; } = new List<BaseTypes>();
        /// <summary>
        /// 柱頭の継手のID
        /// </summary>
        public List<int> JointIdTop { get; } = new List<int>();
        /// <summary>
        /// 柱脚の継手のID
        /// </summary>
        public List<int> JointIdBottom { get; } = new List<int>();

        /// <summary>
        /// 与えられたstbデータからS柱断面を取得する。
        /// </summary>
        /// <param name="stbData"></param>
        /// <param name="version"></param>
        /// <param name="xmlns"></param>
        public override void Load(XDocument stbData, StbVersion version, string xmlns)
        {
            if (stbData.Root == null)
            {
                return;
            }

            IEnumerable<XElement> stbStCols = stbData.Root.Descendants(xmlns + Tag);
            foreach (XElement stbStCol in stbStCols)
            {
                // 必須コード
                Id.Add((int)stbStCol.Attribute("id"));
                Name.Add((string)stbStCol.Attribute("name"));

                // 必須ではないコード
                if (stbStCol.Attribute("floor") != null)
                {
                    Floor.Add((string)stbStCol.Attribute("floor"));
                }
                else
                {
                    Floor.Add(string.Empty);
                }
                if (stbStCol.Attribute("kind_column") != null)
                {
                    KindColumn.Add((string) stbStCol.Attribute("kind_column") == "COLUMN"
                        ? KindsColumn.Column
                        : KindsColumn.Post);
                }
                else
                {
                    KindColumn.Add(KindsColumn.Column);
                }
                if (stbStCol.Attribute("base_type") != null)
                {
                    switch ((string)stbStCol.Attribute("base_type"))
                    {
                        case "EXPOSE":
                            BaseType.Add(BaseTypes.Expose);
                            break;
                        case "EMBEDDED":
                            BaseType.Add(BaseTypes.Embedded);
                            break;
                        case "WRAP":
                            BaseType.Add(BaseTypes.Wrap);
                            break;
                        default:
                            BaseType.Add(BaseTypes.Any);
                            break;
                    }
                }
                else
                {
                    BaseType.Add(BaseTypes.Expose);
                }
                if (stbStCol.Attribute("direction") != null)
                {
                    Direction.Add((bool)stbStCol.Attribute("direction"));
                }
                else
                {
                    Direction.Add(true);
                }
                if (stbStCol.Attribute("joint_id_top") != null)
                {
                    JointIdTop.Add((int)stbStCol.Attribute("joint_id_top"));
                }
                else
                {
                    JointIdTop.Add(-1);
                }
                if (stbStCol.Attribute("joint_id_bottom") != null)
                {
                    JointIdBottom.Add((int)stbStCol.Attribute("joint_id_bottom"));
                }
                else
                {
                    JointIdBottom.Add(-1);
                }

                // 子要素 StbSecSteelColumn
                var stbSecSteelColumn = new StbSecSteelColumn();
                stbSecSteelColumn.Load(stbStCol, version, xmlns);
                Shape.Add(stbSecSteelColumn.Shape);
            }
        }
    }

    /// <summary>
    /// 柱断面形状の名称
    /// </summary>
    class StbSecSteelColumn
    {
        public string Pos { get; private set; }
        public string Shape { get; private set; }
        public string StrengthMain { get; private set; }
        public string StrengthWeb { get; private set; }

        public void Load(XElement stbStCol, StbVersion version, string xmlns)
        {
            XElement stbFigure;
            switch (version)
            {
                case StbVersion.Ver1:
                    stbFigure = stbStCol.Element("StbSecSteelColumn");
                    // 必須コード
                    if (stbFigure != null)
                    {
                        Pos = (string) stbFigure.Attribute("pos");
                        Shape = (string) stbFigure.Attribute("shape");
                        StrengthMain = (string) stbFigure.Attribute("strength_main");

                        // 必須ではないコード
                        if (stbFigure.Attribute("strength_web") != null)
                        {
                            StrengthWeb = (string) stbFigure.Attribute("strength_web");
                        }
                        else
                        {
                            StrengthWeb = string.Empty;
                        }
                    }
                    break;
                case StbVersion.Ver2:
                    string tag;
                    stbFigure = stbStCol.Element(xmlns + "StbSecSteelFigureColumn_S");
                    if (stbFigure == null)
                    {
                        break;
                    }

                    if (stbFigure.Element(xmlns + "StbSecSteelColumn_S_Same") != null)
                    {
                        tag = xmlns + "StbSecSteelColumn_S_Same";
                        Pos = "ALL";
                        Shape = (string)stbFigure.Element(tag).Attribute("shape");
                        StrengthMain = (string)stbFigure.Element(tag).Attribute("strength_main");
                    }
                    else if (stbFigure.Element(xmlns + "StbSecSteelColumn_S_NotSame") != null)
                    {
                        tag = xmlns + "StbSecSteelColumn_S_NotSame";
                        foreach (XElement elem in stbFigure.Elements(tag))
                        {
                            if ((string) elem.Attribute("pos") == "BOTTOM")
                            {
                                Pos = "CENTER";
                                Shape = (string) stbFigure.Element(tag).Attribute("shape");
                                StrengthMain = (string) stbFigure.Element(tag).Attribute("strength_main");
                            }
                        }
                    }
                    else if (stbFigure.Element(xmlns + "StbSecSteelColumn_S_ThreeTypes") != null)
                    {
                        tag = xmlns + "StbSecSteelColumn_S_ThreeTypes";
                        foreach (XElement elem in stbFigure.Elements(tag))
                        {
                            if ((string)elem.Attribute("pos") == "CENTER")
                            {
                                Pos = "CENTER";
                                Shape = (string)stbFigure.Element(tag).Attribute("shape");
                                StrengthMain = (string)stbFigure.Element(tag).Attribute("strength_main");
                            }
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, "The STB version is not set");
            }
        }
    }

    /// <summary>
    /// SRC柱断面
    /// </summary>
    public class StbSecColumnSrc : StbSrcSections
    {
    }

    /// <summary>
    /// CFT柱断面
    /// </summary>
    public class StbSecColumnCft : StbSrcSections
    {
    }

    /// <summary>
    /// RC梁断面
    /// </summary>
    public class StbSecBeamRc : StbRcSections
    {
        public override string Tag { get; } = "StbSecBeam_RC";
        /// <summary>
        /// 部材が大梁か小梁かの区別
        /// </summary>
        public List<KindsBeam> KindBeam { get; } = new List<KindsBeam>();
        /// <summary>
        /// 部材が基礎梁であるかどうか
        /// </summary>
        public List<bool> IsFoundation { get; } = new List<bool>();
        /// <summary>
        /// 部材が片持ちであるかどうか
        /// </summary>
        public List<bool> IsCanti { get; } = new List<bool>();
        /// <summary>
        /// 部材が外端内端であるかどうか
        /// </summary>
        public List<bool> IsOutIn { get; } = new List<bool>();
        /// <summary>
        /// 部材幅
        /// </summary>
        public List<double> Width { get; } = new List<double>();
        /// <summary>
        /// 部材高さ
        /// </summary>
        public List<double> Depth { get; } = new List<double>();

        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
            if (stbElem.Attribute("kind_beam") != null)
            {
                KindBeam.Add((string) stbElem.Attribute("kind_beam") == "GIRDER"
                    ? KindsBeam.Girder
                    : KindsBeam.Beam);
            }
            else
            {
                KindBeam.Add(KindsBeam.Girder);
            }

            if (stbElem.Attribute("isFoundation") != null)
            {
                IsFoundation.Add((bool)stbElem.Attribute("isFoundation"));
            }
            else
            {
                IsFoundation.Add(false);
            }

            var stbBeamSecFigure = new StbBeamSecFigure();
            stbBeamSecFigure.Load(stbElem, version, xmlns);
            Width.Add(stbBeamSecFigure.Width);
            Depth.Add(stbBeamSecFigure.Depth);
        }
    }

    /// <summary>
    /// RC梁断面の形状
    /// </summary>
    class StbBeamSecFigure
    {
        public double Width { get; private set; }
        public double Depth { get; private set; }

        /// <summary>
        /// 与えられたstbデータからRC梁断面の形状を取得する。
        /// </summary>
        /// <param name="stbBeam"></param>
        /// <param name="version"></param>
        /// <param name="xmlns"></param>
        public void Load(XElement stbBeam, StbVersion version, string xmlns)
        {
            string tag;
            XElement stbFigure;
            switch (version)
            {
                case StbVersion.Ver1:
                    stbFigure = stbBeam.Element("StbSecFigure");

                    if (stbFigure?.Element("StbSecHaunch") != null)
                    {
                        tag = "StbSecHaunch";
                        Width = (double)stbFigure.Element(tag)?.Attribute("width_center");
                        Depth = (double)stbFigure.Element(tag)?.Attribute("depth_center");
                    }
                    else if (stbFigure?.Element("StbSecStraight") != null)
                    {
                        tag = "StbSecStraight";
                        Width = (double)stbFigure.Element(tag)?.Attribute("width");
                        Depth = (double)stbFigure.Element(tag)?.Attribute("depth");
                    }
                    else if (stbFigure?.Element("StbSecTaper") != null)
                    {
                        tag = "StbSecTaper";
                        Width = (double)stbFigure.Element(tag)?.Attribute("width_end");
                        Depth = (double)stbFigure.Element(tag)?.Attribute("depth_end");
                    }
                    else
                    {
                        Width = 0;
                        Depth = 0;
                    }
                    
                    break;
                case StbVersion.Ver2:
                    stbFigure = stbBeam.Element(xmlns + "StbSecFigureBeam_RC");
                    if (stbFigure == null)
                    {
                        break;
                    }

                    if (stbFigure.Element(xmlns + "StbSecBeam_RC_Straight") != null)
                    {
                        tag = xmlns + "StbSecBeam_RC_Straight";
                        Width = (double)stbFigure.Element(tag)?.Attribute("width");
                        Depth = (double)stbFigure.Element(tag)?.Attribute("depth");
                    }
                    else if (stbFigure.Element(xmlns + "StbSecBeam_RC_Taper") != null)
                    {
                        tag = xmlns + "StbSecBeam_RC_Taper";
                        foreach (XElement elem in stbFigure.Elements(tag))
                        {
                            if ((string)elem.Attribute("pos") == "END")
                            {
                                Width = (double)stbFigure.Element(tag)?.Attribute("width");
                                Depth = (double)stbFigure.Element(tag)?.Attribute("depth");
                            }
                        }
                    }
                    else if (stbFigure.Element(xmlns + "StbSecBeam_RC_Haunch") != null)
                    {
                        tag = xmlns + "StbSecBeam_RC_Haunch";
                        foreach (XElement elem in stbFigure.Elements(tag))
                        {
                            if ((string)elem.Attribute("pos") == "CENTER")
                            {
                                Width = (double)stbFigure.Element(tag)?.Attribute("width");
                                Depth = (double)stbFigure.Element(tag)?.Attribute("depth");
                            }
                        }
                    }
                    else
                    {
                        Width = 0;
                        Depth = 0;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, "The STB version is not set");
            }
        }
    }

    /// <summary>
    /// RC梁の配筋情報
    /// </summary>
    class StbBeamSecBarArrangement
    {
        public List<double> BarList { get; } = new List<double>();

        public void Load(XElement stbBeam, StbVersion version, string xmlns)
        {
            switch (version)
            {
                case StbVersion.Ver1:
                    string elementName;
                    XElement stbBar = stbBeam.Element("StbSecBar_Arrangement");
                    if (stbBar == null)
                    {
                        break;
                    }

                    if (stbBar.Element("StbSecBeam_Start_Center_End_Section") != null)
                    {
                        elementName = "StbSecBeam_Start_Center_End_Section";
                    }
                    else if (stbBar.Element("StbSecBeam_Start_End_Section") != null)
                    {
                        elementName = "StbSecBeam_Start_End_Section";
                    }
                    else if (stbBar.Element("StbSecBeam_Same_Section") != null)
                    {
                        elementName = "StbSecBeam_Same_Section";
                    }
                    else
                    {
                        BarList.AddRange(new List<double> { 2, 2, 0, 0, 0, 0, 200, 2 });
                        return;
                    }

                    XElement stbBarElem = stbBar.Element(elementName);
                    if (stbBarElem == null)
                        break;

                    // Main 1
                    if (stbBarElem.Attribute("count_main_top_1st") != null)
                    {
                        BarList.Add((double)stbBarElem.Attribute("count_main_top_1st"));
                    }
                    else
                    {
                        BarList.Add(0);
                    }
                    if (stbBarElem.Attribute("count_main_bottom_1st") != null)
                    {
                        BarList.Add((double)stbBarElem.Attribute("count_main_bottom_1st"));
                    }
                    else
                    {
                        BarList.Add(0);
                    }

                    // Main2
                    if (stbBarElem.Attribute("count_main_top_2nd") != null)
                    {
                        BarList.Add((double)stbBarElem.Attribute("count_main_top_2nd"));
                    }
                    else
                    {
                        BarList.Add(0);
                    }
                    if (stbBarElem.Attribute("count_main_bottom_2nd") != null)
                    {
                        BarList.Add((double)stbBarElem.Attribute("count_main_bottom_2nd"));
                    }
                    else
                    {
                        BarList.Add(0);
                    }

                    // Main3
                    if (stbBarElem.Attribute("count_main_top_3rd") != null)
                    {
                        BarList.Add((double)stbBarElem.Attribute("count_main_top_3rd"));
                    }
                    else
                    {
                        BarList.Add(0);
                    }
                    if (stbBarElem.Attribute("count_main_bottom_3rd") != null)
                    {
                        BarList.Add((double)stbBarElem.Attribute("count_main_bottom_3rd"));
                    }
                    else
                    {
                        BarList.Add(0);
                    }

                    // Band
                    if (stbBarElem.Attribute("pitch_stirrup") != null)
                    {
                        BarList.Add((double)stbBarElem.Attribute("pitch_stirrup"));
                    }
                    else
                    {
                        BarList.Add(0);
                    }
                    if (stbBarElem.Attribute("count_stirrup") != null)
                    {
                        BarList.Add((double)stbBarElem.Attribute("count_stirrup"));
                    }
                    else
                    {
                        BarList.Add(0);
                    }
                    break;
                case StbVersion.Ver2:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, "The STB version is not set");
            }
        }
    }

    /// <summary>
    /// S梁断面
    /// </summary>
    public class StbSecBeamS : StbSteelSections
    {
        public override string Tag { get; } = "StbSecBeam_S";
        /// <summary>
        /// 部材が大梁か小梁かの区別
        /// </summary>
        public List<KindsBeam> KindBeam { get; } = new List<KindsBeam>();
        /// <summary>
        /// 部材が片持ちであるかどうか
        /// </summary>
        public List<bool> IsCanti { get; } = new List<bool>();
        /// <summary>
        /// 部材が外端内端であるかどうか
        /// </summary>
        public List<bool> IsOutIn { get; } = new List<bool>();
        /// <summary>
        /// 始端の継手のID
        /// </summary>
        public List<int> JointIdStart { get; } = new List<int>();
        /// <summary>
        /// 終端の継手のID
        /// </summary>
        public List<int> JointIdEnd { get; } = new List<int>();


        /// <summary>
        /// 与えられたstbデータからS梁断面を取得する。
        /// </summary>
        /// <param name="stbData"></param>
        /// <param name="version"></param>
        /// <param name="xmlns"></param>
        public override void Load(XDocument stbData, StbVersion version, string xmlns)
        {
            if (stbData.Root == null)
            {
                return;
            }

            IEnumerable<XElement> stbStBeams = stbData.Root.Descendants(xmlns + Tag);
            foreach (XElement stbStBeam in stbStBeams)
            {
                // 必須コード
                Id.Add((int)stbStBeam.Attribute("id"));
                Name.Add((string)stbStBeam.Attribute("name"));

                // 必須ではないコード
                if (stbStBeam.Attribute("floor") != null)
                {
                    Floor.Add((string)stbStBeam.Attribute("floor"));
                }
                else
                {
                    Floor.Add(string.Empty);
                }
                if (stbStBeam.Attribute("kind_beam") != null)
                {
                    KindBeam.Add((string) stbStBeam.Attribute("kind_beam") == "GIRDER"
                        ? KindsBeam.Girder
                        : KindsBeam.Beam);
                }
                else
                {
                    KindBeam.Add(KindsBeam.Girder);
                }
                if (stbStBeam.Attribute("isCanti") != null)
                {
                    IsCanti.Add((bool)stbStBeam.Attribute("isCanti"));
                }
                else
                {
                    IsCanti.Add(false);
                }
                if (stbStBeam.Attribute("isOutIn") != null)
                {
                    IsCanti.Add((bool)stbStBeam.Attribute("isOutIn"));
                }
                else
                {
                    IsCanti.Add(false);
                }
                if (stbStBeam.Attribute("joint_id_start") != null)
                {
                    JointIdStart.Add((int)stbStBeam.Attribute("joint_id_start"));
                }
                else
                {
                    JointIdStart.Add(-1);
                }
                if (stbStBeam.Attribute("joint_id_end") != null)
                {
                    JointIdEnd.Add((int)stbStBeam.Attribute("joint_id_end"));
                }
                else
                {
                    JointIdEnd.Add(-1);
                }

                // 子要素 StbSecSteelBeam
                var stbSecSteelBeam = new StbSecSteelBeam();
                stbSecSteelBeam.Load(stbStBeam, version, xmlns);
                Shape.Add(stbSecSteelBeam.Shape);
            }
        }
    }

    /// <summary>
    /// S梁断面形状の名称
    /// </summary>
    class StbSecSteelBeam : StbSteelShapes
    {
        public void Load(XElement stbStBeam, StbVersion version, string xmlns)
        {
            XElement stbFigure;
            switch (version)
            {
                case StbVersion.Ver1:
                    stbFigure = stbStBeam.Element("StbSecSteelBeam");
                    if (stbFigure != null)
                    {
                        Pos = (string) stbFigure.Attribute("pos");
                        Shape = (string) stbFigure.Attribute("shape");
                        StrengthMain = (string) stbFigure.Attribute("strength_main");

                        // 必須ではないコード
                        StrengthWeb = stbFigure.Attribute("strength_web") != null
                            ? (string) stbFigure.Attribute("strength_web")
                            : string.Empty;
                    }
                    break;
                case StbVersion.Ver2:
                    stbFigure = stbStBeam.Element(xmlns + "StbSecSteelFigureBeam_S");
                    if (stbFigure != null)
                    {
                        string tag;
                        if (stbFigure.Element(xmlns + "StbSecSteelBeam_S_Straight") != null)
                        {
                            tag = xmlns + "StbSecSteelBeam_S_Straight";
                            Shape = (string) stbFigure.Element(tag)?.Attribute("shape");
                            StrengthMain = (string) stbFigure.Element(tag)?.Attribute("strength_main");
                        }
                        else if (stbFigure.Element(xmlns + "StbSecSteelBeam_S_Taper") != null)
                        {
                            tag = xmlns + "StbSecSteelBeam_S_Taper";
                            foreach (XElement elem in stbFigure.Elements(tag))
                            {
                                if ((string) elem.Attribute("pos") == "END")
                                {
                                    Shape = (string) stbFigure.Element(tag)?.Attribute("shape");
                                    StrengthMain = (string) stbFigure.Element(tag)?.Attribute("strength_main");
                                }
                            }
                        }
                        else if (stbFigure.Element(xmlns + "StbSecSteelBeam_S_Joint") != null)
                        {
                            tag = xmlns + "StbSecSteelBeam_S_Joint";
                            foreach (XElement elem in stbFigure.Elements(tag))
                            {
                                if ((string) elem.Attribute("pos") == "CENTER")
                                {
                                    Shape = (string) stbFigure.Element(tag)?.Attribute("shape");
                                    StrengthMain = (string) stbFigure.Element(tag)?.Attribute("strength_main");
                                }
                            }
                        }
                        else if (stbFigure.Element(xmlns + "StbSecSteelBeam_S_Haunch") != null)
                        {
                            tag = xmlns + "StbSecSteelBeam_S_Haunch";
                            foreach (XElement elem in stbFigure.Elements(tag))
                            {
                                if ((string) elem.Attribute("pos") == "CENTER")
                                {
                                    Shape = (string) stbFigure.Element(tag)?.Attribute("shape");
                                    StrengthMain = (string) stbFigure.Element(tag)?.Attribute("strength_main");
                                }
                            }
                        }
                        else if (stbFigure.Element(xmlns + "StbSecSteelBeam_S_FiveTypes") != null)
                        {
                            tag = xmlns + "StbSecSteelBeam_S_FiveTypes";
                            foreach (XElement elem in stbFigure.Elements(tag))
                            {
                                if ((string) elem.Attribute("pos") == "CENTER")
                                {
                                    Shape = (string) stbFigure.Element(tag)?.Attribute("shape");
                                    StrengthMain = (string) stbFigure.Element(tag)?.Attribute("strength_main");
                                }
                            }
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, "The STB version is not set");
            }
        }
    }

    /// <summary>
    /// SRC梁断面
    /// </summary>
    public class StbSecBeamSRC
    {
    }

    /// <summary>
    /// Sブレース断面
    /// </summary>
    public class StbSecBraceS : StbSteelSections
    {
        public override string Tag { get; } = "StbSecBrace_S";
        /// <summary>
        /// 部材が水平か鉛直かの区別
        /// </summary>
        public List<KindsBrace> KindBrace { get; } = new List<KindsBrace>();

        /// <summary>
        /// 与えられたstbデータからSブレース断面を取得する。
        /// </summary>
        /// <param name="stbData"></param>
        /// <param name="version"></param>
        /// <param name="xmlns"></param>
        public override void Load(XDocument stbData, StbVersion version, string xmlns)
        {
            if (stbData.Root == null)
            {
                return;
            }

            IEnumerable<XElement> stbStBraces = stbData.Root.Descendants(xmlns + Tag);
            foreach (XElement stbStBrace in stbStBraces)
            {
                // 必須コード
                Id.Add((int)stbStBrace.Attribute("id"));
                Name.Add((string)stbStBrace.Attribute("name"));

                // 必須ではないコード
                if (stbStBrace.Attribute("floor") != null)
                {
                    Floor.Add((string)stbStBrace.Attribute("floor"));
                }
                else
                {
                    Floor.Add(string.Empty);
                }
                if (stbStBrace.Attribute("kind_brace") != null)
                {
                    KindBrace.Add((string) stbStBrace.Attribute("kind_brace") == "HORIZONTAL"
                        ? KindsBrace.Horizontal
                        : KindsBrace.Vertical);
                }
                else
                {
                    KindBrace.Add(KindsBrace.Vertical);
                }

                // 子要素 StbSecSteelBeam
                var stbSecSteelBrace = new StbSecSteelBrace();
                stbSecSteelBrace.Load(stbStBrace, version, xmlns);
                Shape.Add(stbSecSteelBrace.Shape);
            }
        }
    }

    /// <summary>
    /// Sブレース断面形状の名称
    /// </summary>
    public class StbSecSteelBrace : StbSteelShapes
    {
        /// <summary>
        /// 属性の読み込み
        /// </summary>
        /// <param name="stbStBrace"></param>
        /// <param name="version"></param>
        /// <param name="xmlns"></param>
        public void Load(XElement stbStBrace, StbVersion version, string xmlns)
        {
            XElement stbFigure;
            switch (version)
            {
                case StbVersion.Ver1:
                    stbFigure = stbStBrace.Element("StbSecSteelBrace");
                    if (stbFigure == null)
                    {
                        break;
                    }

                    // 必須コード
                    Pos = (string)stbFigure.Attribute("pos");
                    Shape = (string)stbFigure.Attribute("shape");
                    StrengthMain = (string)stbFigure.Attribute("strength_main");

                    // 必須ではないコード
                    if (stbFigure.Attribute("strength_web") != null)
                    {
                        StrengthWeb = (string)stbFigure.Attribute("strength_web");
                    }
                    else
                    {
                        StrengthWeb = string.Empty;
                    }

                    break;
                case StbVersion.Ver2:
                    string tag;
                    stbFigure = stbStBrace.Element(xmlns + "StbSecSteelFigureBrace_S");
                    if (stbFigure == null)
                    {
                        break;
                    }

                    if (stbFigure.Element(xmlns + "StbSecSteelBrace_S_Same") != null)
                    {
                        tag = xmlns + "StbSecSteelBrace_S_Same";
                        Shape = (string)stbFigure.Element(tag)?.Attribute("shape");
                        StrengthMain = (string)stbFigure.Element(tag)?.Attribute("strength_main");
                    }
                    else if (stbFigure.Element("StbSecSteelBrace_S_NotSame") != null)
                    {
                        tag = xmlns + "StbSecSteelBrace_S_NotSame";
                        if ((string)stbFigure.Element(tag)?.Attribute("pos") == "TOP")
                        {
                            Shape = (string)stbFigure.Element(tag)?.Attribute("shape");
                            StrengthMain = (string)stbFigure.Element(tag)?.Attribute("strength_main");
                        }
                    }
                    else if (stbFigure.Element(xmlns + "StbSecSteelBrace_S_ThreeTypes") != null)
                    {
                        tag = xmlns + "StbSecSteelBrace_S_ThreeTypes";
                        if ((string)stbFigure.Element(tag)?.Attribute("pos") == "CENTER")
                        {
                            Shape = (string)stbFigure.Element(tag)?.Attribute("shape");
                            StrengthMain = (string)stbFigure.Element(tag)?.Attribute("strength_main");
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, "The STB version is not set");
            }
        }
    }

    /// <summary>
    /// RC壁断面
    /// </summary>
    public class StbSecWallRC
    {
    }

    /// <summary>
    /// RCスラブ断面
    /// </summary>
    public class StbSecSlabRC
    {
    }

    /// <summary>
    /// RC基礎断面
    /// </summary>
    public class StbSecFoundationRC
    {
    }

    /// <summary>
    /// 鉄骨断面
    /// </summary>
    public class StbSecSteel : StbSteelParameters
    {
        private StbSecRollH RollH { get; } = new StbSecRollH();
        private StbSecBuildH BuildH { get; } = new StbSecBuildH();
        private StbSecRollBox RollBox { get; } = new StbSecRollBox();
        private StbSecBuildBox BuildBox { get; } = new StbSecBuildBox();
        private StbSecPipe Pipe { get; } = new StbSecPipe();
        private StbSecRollT RollT { get; } = new StbSecRollT();
        private StbSecRollC RollC { get; } = new StbSecRollC();
        private StbSecRollL RollL { get; } = new StbSecRollL();
        private StbSecRollLipC RollLipC { get; } = new StbSecRollLipC();
        private StbSecFlatBar FlatBar { get; } = new StbSecFlatBar();
        private StbSecRoundBar RoundBar { get; } = new StbSecRoundBar();

        /// <summary>
        /// 属性情報の読み込み
        /// </summary>
        /// <param name="stbData"></param>
        /// <param name="version"></param>
        /// <param name="xmlns"></param>
        public override void Load(XDocument stbData, StbVersion version, string xmlns)
        {
            var sections = new List<StbSteelParameters>
            {
                RollH, BuildH, RollBox, BuildBox, Pipe, RollT, RollC, RollL, RollLipC, FlatBar, RoundBar
            };

            foreach (StbSteelParameters section in sections)
            {
                section.Load(stbData, version, xmlns);
                Name.AddRange(section.Name);
                P1.AddRange(section.P1);
                P2.AddRange(section.P2);
                P3.AddRange(section.P3);
                P4.AddRange(section.P4);
                P5.AddRange(section.P5);
                P6.AddRange(section.P6);
                ShapeType.AddRange(section.ShapeType);
            }
        }
    }

    /// <summary>
    /// ロールH形断面
    /// </summary>
    public class StbSecRollH : StbSteelParameters
    {
        public override string ElementTag { get; } = "StbSecRoll-H";

        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
            P1.Add((double)stbElem.Attribute("A"));
            P2.Add((double)stbElem.Attribute("B"));
            P3.Add((double)stbElem.Attribute("t1"));
            P4.Add((double)stbElem.Attribute("t2"));
            P5.Add((double)stbElem.Attribute("r"));
            P6.Add(-1d);
            ShapeType.Add(ShapeTypes.H);
        }
    }

    /// <summary>
    /// ビルトH形断面
    /// </summary>
    public class StbSecBuildH : StbSteelParameters
    {
        public override string ElementTag { get; } = "StbSecBuild-H";
        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
            P1.Add((double)stbElem.Attribute("A"));
            P2.Add((double)stbElem.Attribute("B"));
            P3.Add((double)stbElem.Attribute("t1"));
            P4.Add((double)stbElem.Attribute("t2"));
            P5.Add(-1d);
            P6.Add(-1d);
            ShapeType.Add(ShapeTypes.H);
        }
    }

    /// <summary>
    /// ロール箱形断面
    /// </summary>
    public class StbSecRollBox : StbSteelParameters
    {
        public override string ElementTag { get; } = "StbSecRoll-BOX";
        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
            P1.Add((double)stbElem.Attribute("A"));
            P2.Add((double)stbElem.Attribute("B"));
            P3.Add((double)stbElem.Attribute("t"));
            switch (version)
            {
                case StbVersion.Ver1:
                    P4.Add((float) stbElem.Attribute("R"));
                    break;
                case StbVersion.Ver2:
                    P4.Add((float) stbElem.Attribute("r"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, "The STB version is not set");
            }
            P5.Add(-1d);
            P6.Add(-1d);
            ShapeType.Add(ShapeTypes.RollBOX);
        }
    }

    /// <summary>
    /// ビルト箱形断面
    /// </summary>
    public class StbSecBuildBox : StbSteelParameters
    {
        public override string ElementTag { get; } = "StbSecBuild-BOX";
        
        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
            P1.Add((double)stbElem.Attribute("A"));
            P2.Add((double)stbElem.Attribute("B"));
            P3.Add((double)stbElem.Attribute("t1"));
            P4.Add((double)stbElem.Attribute("t2"));
            P5.Add(-1d);
            P6.Add(-1d);
            ShapeType.Add(ShapeTypes.BuildBOX);
        }
    }

    /// <summary>
    /// 円形断面
    /// </summary>
    public class StbSecPipe : StbSteelParameters
    {
        public override string ElementTag { get; } = "StbSecPipe";
        
        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
            P1.Add((double)stbElem.Attribute("t"));
            P2.Add((double)stbElem.Attribute("D"));
            P3.Add(-1d);
            P4.Add(-1d);
            P5.Add(-1d);
            P6.Add(-1d);
            ShapeType.Add(ShapeTypes.Pipe);
        }
    }

    /// <summary>
    /// T形断面
    /// </summary>
    public class StbSecRollT : StbSteelParameters
    {
        public override string ElementTag { get; } = "StbSecRoll-T";
        
        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
            P1.Add((double)stbElem.Attribute("A"));
            P2.Add((double)stbElem.Attribute("B"));
            P3.Add((double)stbElem.Attribute("t1"));
            P4.Add((double)stbElem.Attribute("t2"));
            P5.Add((double)stbElem.Attribute("r"));
            P6.Add(-1d);
            ShapeType.Add(ShapeTypes.T);
        }
    }

    /// <summary>
    /// 溝形断面
    /// </summary>
    public class StbSecRollC : StbSteelParameters
    {
        public override string ElementTag { get; } = "StbSecRoll-C";
        
        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
            P1.Add((double)stbElem.Attribute("A"));
            P2.Add((double)stbElem.Attribute("B"));
            P3.Add((double)stbElem.Attribute("t1"));
            P4.Add((double)stbElem.Attribute("t2"));
            P5.Add((double)stbElem.Attribute("r1"));
            P6.Add((double)stbElem.Attribute("r2"));
            ShapeType.Add(ShapeTypes.C);
        }
    }

    /// <summary>
    /// 山形断面
    /// </summary>
    public class StbSecRollL : StbSteelParameters
    {
        public override string ElementTag { get; } = "StbSecRoll-L";
        
        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
            P1.Add((double)stbElem.Attribute("A"));
            P2.Add((double)stbElem.Attribute("B"));
            P3.Add((double)stbElem.Attribute("t1"));
            P4.Add((double)stbElem.Attribute("t2"));
            P5.Add((double)stbElem.Attribute("r1"));
            P6.Add((double)stbElem.Attribute("r2"));
            ShapeType.Add(ShapeTypes.L);
        }
    }

    /// <summary>
    /// リップ溝形断面
    /// </summary>
    public class StbSecRollLipC : StbSteelParameters
    {
        public override string ElementTag { get; } = "StbSecRoll-LipC";
        
        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
            P1.Add((double)stbElem.Attribute("H"));
            P2.Add((double)stbElem.Attribute("A"));
            P3.Add((double)stbElem.Attribute("C"));
            P4.Add((double)stbElem.Attribute("t"));
            P5.Add(-1d);
            P6.Add(-1d);
            ShapeType.Add(ShapeTypes.C);
        }
    }

    /// <summary>
    /// フラットバー断面
    /// </summary>
    public class StbSecFlatBar : StbSteelParameters
    {
        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
            P1.Add((double)stbElem.Attribute("B"));
            P2.Add((double)stbElem.Attribute("t"));
            P3.Add(-1d);
            P4.Add(-1d);
            P5.Add(-1d);
            P6.Add(-1d);
            ShapeType.Add(ShapeTypes.FB);
        }
        
        public override void Load(XDocument stbData, StbVersion version, string xmlns)
        {
            if (stbData.Root == null)
            {
                return;
            }

            IEnumerable<XElement> stSections  = null;
            IEnumerable<XElement> stSecSteel = stbData.Root.Descendants(xmlns + Tag);
            switch (version)
            {
                case StbVersion.Ver1:
                    stSections = stSecSteel.Elements(xmlns + "StbSecRoll-FB");
                    break;
                case StbVersion.Ver2:
                    stSections = stSecSteel.Elements(xmlns + "StbSecFlatBar");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, "The STB version is not set");
            }
            
            foreach (XElement stSection in stSections)
            {
                ElementLoader(stSection, version, xmlns);
            }
        }
    }

    /// <summary>
    /// 丸鋼断面
    /// </summary>
    public class StbSecRoundBar : StbSteelParameters
    {
        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
            P1.Add((double)stbElem.Attribute("R"));
            P2.Add((double)stbElem.Attribute("R"));
            P3.Add(-1d);
            P4.Add(-1d);
            P5.Add(-1d);
            P6.Add(-1d);
            ShapeType.Add(ShapeTypes.Bar);
        }

        public override void Load(XDocument stbData, StbVersion version, string xmlns)
        {
            if (stbData.Root == null)
            {
                return;
            }

            IEnumerable<XElement> stSections  = null;
            IEnumerable<XElement> stSecSteel = stbData.Root.Descendants(xmlns + Tag);

            switch (version)
            {
                case StbVersion.Ver1:
                    stSections = stSecSteel.Elements(xmlns + "StbSecRoll-Bar");
                    break;
                case StbVersion.Ver2:
                    stSections = stSecSteel.Elements(xmlns + "StbSecRoundBar");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, "The STB version is not set");
            }

            foreach (XElement stSection in stSections)
            {
                ElementLoader(stSection, version, xmlns);
            }
        }
    }
}
