using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;

namespace MenuChanger.Demos
{
    public class PlandoDemo
    {
        public BigButton EntryButton;
        
        MenuPage PlandoSelectPage;
        PlandoDef[] Plandos;
        BigButton[] PlandoButtons;

        MenuPage StartPage;
        public PlandoDef SelectedPlando;
        MenuLabel StartTitle;
        MenuLabel StartDesc;
        BigButton StartButton;

        static string FolderPath => Path.GetFullPath(Application.dataPath + "/Managed/Mods/Plando/");

        public PlandoDemo()
        {
            PlandoSelectPage = new MenuPage("Plando Select Page", ModeMenu.ModePage);
            StartPage = new MenuPage("Plando Start Page", PlandoSelectPage);

            EntryButton = new BigButton(ModeMenu.ModePage, MenuChanger.Sprites["plando"]);
            EntryButton.Button.AddHideMenuPageEvent(ModeMenu.ModePage);
            EntryButton.Button.AddShowMenuPageEvent(PlandoSelectPage);
            ModeMenu.AddButtonToModeSelect(EntryButton);

            Plandos = GetPlandoDefs();
            PlandoButtons = Plandos.Select(p => MakePlandoButton(p)).ToArray();
            new MultiGridItemPanel(PlandoSelectPage,
                5, 3, 150, 650, new Vector2(0, 300), PlandoButtons);

            StartTitle = new MenuLabel(StartPage, "???");
            StartTitle.MoveTo(new Vector2(0, 450));

            StartDesc = new MenuLabel(StartPage, "???", MenuLabel.Style.Body);
            StartDesc.MoveTo(new Vector2(0, 380));
            
            StartButton = new BigButton(StartPage, Mode.Classic);
            StartButton.MoveTo(new Vector2(0, -350));
        }

        private BigButton MakePlandoButton(PlandoDef def)
        {
            BigButton bb = new BigButton(PlandoSelectPage, def.Title, $"by {def.Author}");
            bb.Button.AddEvent(() => PlandoButtonEvent(def));

            return bb;
        }

        private void PlandoButtonEvent(PlandoDef def)
        {
            SelectedPlando = def;
            StartTitle.Text.text = def.Title;
            StartDesc.Text.text = def.Desc;
            PlandoSelectPage.Hide();
            StartPage.Show();
        }

        public static PlandoDef[] GetPlandoDefs()
        {
            if (!Directory.Exists(FolderPath)) return new PlandoDef[0];

            List<PlandoDef> defs = new List<PlandoDef>();
            foreach (string path in Directory.GetFiles(FolderPath, "*.xml"))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                PlandoDef def = new PlandoDef();
                def.FileName = Path.GetFileNameWithoutExtension(path);
                def.Title = def.FileName.Replace('_', ' ');
                def.Xml = doc;

                if (doc.SelectSingleNode("randomizer/title") is XmlNode titleNode && !string.IsNullOrEmpty(titleNode.InnerText))
                {
                    def.Title = titleNode.InnerText;
                }
                if (doc.SelectSingleNode("randomizer/author") is XmlNode authorNode && !string.IsNullOrEmpty(authorNode.InnerText))
                {
                    def.Author = authorNode.InnerText;
                }
                if (doc.SelectSingleNode("randomizer/description") is XmlNode descNode && !string.IsNullOrEmpty(descNode.InnerText))
                {
                    def.Desc = descNode.InnerText;
                }

                defs.Add(def);
            }

            return defs.ToArray();
        }
    }
}
