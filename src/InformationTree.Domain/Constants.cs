using System.Collections.Generic;
using System.Drawing;

namespace InformationTree.Domain
{
    public static class Constants
    {
        public static class Colors
        {
            public static readonly Color DefaultBackGroundColor = Color.White;
            public static readonly Color DefaultForeGroundColor = Color.Black;
            public static readonly Color BackGroundColorSearch = Color.SlateGray;
            public static readonly Color ForeGroundColorSearch = Color.LightBlue;
            public static readonly Color LinkBackGroundColor = Color.DarkCyan;
            public static readonly Color DataBackGroundColor = Color.FromArgb(200, 200, 200);
            public static readonly Color ExceptionColor = Color.Red;
        }

        public static class Parsing
        {
            public const char SpaceSeparator = ' ';
            public const char UnderscoreSeparator = '_';
        }

        public static class XmlAttributes
        {
            public const string XmlAttrText = "text";

            public const string XmlAttrName = "name";
            public const string XmlAttrBold = "bold";
            public const string XmlAttrItalic = "italic";
            public const string XmlAttrUnderline = "underline";
            public const string XmlAttrStrikeout = "strikeout";
            public const string XmlAttrForegroundColor = "foreground";
            public const string XmlAttrBackgroundColor = "background";
            public const string XmlAttrFontFamily = "fontFamily";
            public const string XmlAttrFontSize = "fontSize";
            public const string XmlAttrAddedNumber = "addedNumber";
            public const string XmlAttrAddedDate = "addedDate";
            public const string XmlAttrLastChangeDate = "lastChangeDate";
            public const string XmlAttrUrgency = "urgency";
            public const string XmlAttrLink = "link";
            public const string XmlAttrCategory = "category";
            public const string XmlAttrIsStartupAlert = "isStartupAlert";

            public const string XmlAttrData = "data";

            public const string XmlAttrPercentCompleted = "percentCompleted";

            #region Old XML Attribute Names

            public static List<string> XmlAttrForegroundColorAcceptedList
            {
                get
                {
                    return new List<string> { XmlAttrForegroundColor, "foreColor", "color" };
                }
            }

            public static List<string> XmlAttrBackgroundColorAcceptedList
            {
                get
                {
                    return new List<string> { XmlAttrBackgroundColor, "backColor" };
                }
            }

            public static List<string> XmlAttrUrgencyAcceptedList
            {
                get
                {
                    return new List<string> { XmlAttrUrgency, "attrUrgency" };
                }
            }

            public static List<string> XmlAttrLinkAcceptedList
            {
                get
                {
                    return new List<string> { XmlAttrLink, "attrLink" };
                }
            }

            #endregion Old XML Attribute Names
        }

        public static class DateTimeFormats
        {
            public const string DateTimeFormatSeparatedWithDot = "dd.MM.yyyy HH:mm:ss";
            public const string DateTimeFormatSeparatedWithSlash = "dd/MM/yyyy HH:mm:ss";
        }

        public const string DefaultFileName = "Data.xml";

        public static class CacheKeys
        {
            public const string NodesList = "Nodes";
            public const string TreeState = "TreeState";
            public const string IsControlKeyPressed = "IsControlKeyPressed";
            public const string StyleCheckedListBox_ItemCheckEntered = "StyleCheckedListBox_ItemCheckEntered";
            public const string CanvasForm = "CanvasForm";
            public const string SoundNumber = "SoundNumber";
            public const string TreeViewOldX = "TreeViewOldX";
            public const string TreeViewOldY = "TreeViewOldY";
        }
    }
}