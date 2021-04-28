using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using System.IO;
using GlobalEnums;
using SereCore;

namespace MenuChanger.Demos.RandomizerData
{
    public struct StartDef
    {
        public static StartDef[] StartDefs = GetStartLocations();

        public static StartDef[] GetStartLocations()
        {
            Assembly a = typeof(MenuChanger).Assembly;
            Stream startLocationStream = a.GetManifestResourceStream("MenuChanger.Resources.startlocations.xml");
            XmlDocument startLocationXml = new XmlDocument();
            startLocationXml.Load(startLocationStream);
            startLocationStream.Dispose();

            return ParseStartLocationXML(startLocationXml.SelectNodes("randomizer/start"));
        }

        public string name;

        // respawn marker properties
        public string sceneName;
        public float x;
        public float y;
        public MapZone zone;

        // logic info
        public string waypoint;
        public string areaTransition;
        public string roomTransition;

        // control for menu select
        public bool itemSafe; // safe := no items required to get to Dirtmouth
        public bool areaSafe; // safe := no items required to get to an area transition
        public bool roomSafe; // safe := no items required to get to a room transition


        private static StartDef[] ParseStartLocationXML(XmlNodeList nodes)
        {
            List<StartDef> defs = new List<StartDef>();

            Dictionary<string, FieldInfo> startLocationFields = new Dictionary<string, FieldInfo>();
            typeof(StartDef).GetFields().ToList().ForEach(f => startLocationFields.Add(f.Name, f));

            foreach (XmlNode startNode in nodes)
            {
                XmlAttribute nameAttr = startNode.Attributes?["name"];

                // Setting as object prevents boxing in FieldInfo.SetValue calls
                object def = new StartDef { name = nameAttr?.InnerText };

                foreach (XmlNode fieldNode in startNode.ChildNodes)
                {
                    if (!startLocationFields.TryGetValue(fieldNode.Name, out FieldInfo field))
                    {
                        continue;
                    }

                    else if (field.FieldType == typeof(bool))
                    {
                        if (bool.TryParse(fieldNode.InnerText, out bool xmlBool))
                        {
                            field.SetValue(def, xmlBool);
                        }
                    }

                    else if (field.FieldType == typeof(float))
                    {
                        if (float.TryParse(fieldNode.InnerText, out float xmlFloat))
                        {
                            field.SetValue(def, xmlFloat);
                        }
                    }

                    else if (field.FieldType == typeof(string))
                    {
                        field.SetValue(def, fieldNode.InnerText);
                    }

                    else if (field.FieldType == typeof(MapZone))
                    {
                        if (fieldNode.InnerText.TryToEnum(out MapZone xmlZone))
                        {
                            field.SetValue(def, xmlZone);
                        }
                    }
                }

                defs.Add((StartDef)def);
            }

            return defs.ToArray();
        }

    }
}
