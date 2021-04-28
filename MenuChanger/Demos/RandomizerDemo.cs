using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MenuChanger.Demos.RandomizerData;
using MenuChanger.MenuPanels;
using MenuChanger.MenuElements;


namespace MenuChanger.Demos
{
    public class RandomizerDemo
    {
        public BigButton entryButton;
        GenerationSettings Settings = new GenerationSettings();

        MenuPage ResumePage;
        MenuPage GameSettingsPage;

        //MenuLabel SettingsCode;
        //SmallButton CodeUpdater;


        #region Start

        MenuPage StartPage;

        BigButton StartButton;
        IntEntryField SeedEntryField;


        MenuPreset<PoolSettings> PoolPreset;
        MenuPreset<SkipSettings> SkipPreset;
        MenuPreset<GrubCostRandomizerSettings> GrubCostPreset;
        MenuPreset<EssenceCostRandomizerSettings> EssenceCostPreset;
        MenuPreset<CursedSettings> CursedPreset;
        MenuPreset<LongLocationSettings> LongLocationPreset;

        SmallButton[] PresetButtons => new SmallButton[]
        {
            PoolPreset,
            SkipPreset,
            GrubCostPreset,
            EssenceCostPreset,
            LongLocationPreset,
            CursedPreset,
        };
        GridItemPanel StartGIP;


        SmallButton JumpToJumpPageButton;
        SmallButton JumpToGameSettingsButton;
        SmallButton ToggleCaptionsButton;

        SmallButton[] StartCornerButtons => new SmallButton[]
        {
            JumpToJumpPageButton,
            JumpToGameSettingsButton,
            ToggleCaptionsButton,
        };
        VerticalItemPanel StartCornerVIP;

        #endregion

        #region Jump

        MenuPage JumpToSettingsPage;
        SmallButton[] JumpButtons;
        GridItemPanel JumpPanel;

        #endregion

        #region Advanced Settings

        MenuPage AdvancedSettingsPage;

        OrderedItemViewer AdvancedSettingsViewer;

        Subpage[] AdvancedSettingsSubpages => new Subpage[]
        {
            PoolSubpage,
            SkipSubpage,
            CursedSubpage,
            CostSubpage,
            LongLocationSubpage,
            StartLocationSubpage,
            StartItemSubpage,
            MiscSubpage
        };

        Subpage PoolSubpage;
        MenuElementFactory<PoolSettings> poolMEF;
        GridItemPanel poolGIP;

        Subpage SkipSubpage;
        MenuElementFactory<SkipSettings> skipMEF;
        VerticalItemPanel skipVIP;

        Subpage CursedSubpage;
        MenuElementFactory<CursedSettings> cursedMEF;
        VerticalItemPanel cursedVIP;

        Subpage CostSubpage;
        MenuElementFactory<GrubCostRandomizerSettings> grubCostMEF;
        MenuElementFactory<EssenceCostRandomizerSettings> essenceCostMEF;
        GridItemPanel costGIP;

        Subpage LongLocationSubpage;
        MenuElementFactory<LongLocationSettings> longLocationMEF;
        VerticalItemPanel longVIP;

        Subpage StartLocationSubpage;
        MenuElementFactory<StartLocationSettings> startLocMEF;
        RadioSwitch startLocationSwitch;
        GridItemPanel startLocationGIP;
        VerticalItemPanel startLocVIP;

        Subpage StartItemSubpage;
        MenuElementFactory<StartItemSettings> startItemMEF;
        MenuElementFactory<StartItemClass>[] startItemClassMEFs;

        Subpage MiscSubpage;
        MenuElementFactory<MiscSettings> miscMEF;
        VerticalItemPanel miscVIP;

        #endregion


        public RandomizerDemo()
        {
            MakeMenuPages();
            MakeMenuElements();
            MakePanels();
            AddEvents();
            Arrange();

            ResumeMenu.AddResumePage("Randomizer", ResumePage);
            ModeMenu.AddButtonToModeSelect(entryButton);
            SeedEntryField.InputValue = new System.Random().Next(0, 999999999);
        }

        private void MakeMenuPages()
        {
            StartPage = new MenuPage("Randomizer Setting Select Page", ModeMenu.ModePage);
            JumpToSettingsPage = new MenuPage("Randomizer Jump To Advanced Settings Page", StartPage);
            AdvancedSettingsPage = new MenuPage("Randomizer Advanced Settings Page", JumpToSettingsPage);
            GameSettingsPage = new MenuPage("Randomizer Game Settings Page", StartPage);

            ResumePage = new MenuPage("Randomizer Resume Page");
        }

        private void MakeMenuElements()
        {
            entryButton = new BigButton(ModeMenu.ModePage, MenuChanger.Sprites["logo"], "Randomizer 3");
            
            JumpToJumpPageButton = new SmallButton(StartPage, "Advanced Settings");
            JumpToGameSettingsButton = new SmallButton(StartPage, "Game Settings");
            ToggleCaptionsButton = new SmallButton(StartPage, "Toggle Captions");


            StartButton = new BigButton(StartPage, Mode.Classic); // TODO: constructor for default sprite, custom text
            SeedEntryField = new IntEntryField(StartPage, "Seed");

            //CodeUpdater = new SmallButton(StartPage, "Compute Settings Code");
            //SettingsCode = new MenuLabel(StartPage, "", MenuLabel.Style.Body);

            poolMEF = new MenuElementFactory<PoolSettings>(AdvancedSettingsPage, Settings.PoolSettings);
            skipMEF = new MenuElementFactory<SkipSettings>(AdvancedSettingsPage, Settings.SkipSettings);
            cursedMEF = new MenuElementFactory<CursedSettings>(AdvancedSettingsPage, Settings.CursedSettings);
            grubCostMEF = new MenuElementFactory<GrubCostRandomizerSettings>(AdvancedSettingsPage, Settings.GrubCostRandomizerSettings);
            essenceCostMEF = new MenuElementFactory<EssenceCostRandomizerSettings>(AdvancedSettingsPage, Settings.EssenceCostRandomizerSettings);
            longLocationMEF = new MenuElementFactory<LongLocationSettings>(AdvancedSettingsPage, Settings.LongLocationSettings);
            startLocMEF = new MenuElementFactory<StartLocationSettings>(AdvancedSettingsPage, Settings.StartLocationSettings);

            PoolPreset = new MenuPreset<PoolSettings>(StartPage, "Randomized Items", 
                PoolData.PoolPresets, Settings.PoolSettings, poolMEF,
                (ps) => string.Join(", ", typeof(PoolSettings).GetFields().Where(f => (bool)f.GetValue(ps)).Select(f => f.Name.FromCamelCase()).ToArray()));
            SkipPreset = new MenuPreset<SkipSettings>(StartPage, "Required Skips", 
                SkipData.SkipPresets, Settings.SkipSettings, skipMEF,
                (sk) => string.Join(", ", typeof(SkipSettings).GetFields().Where(f => (bool)f.GetValue(sk)).Select(f => f.Name.FromCamelCase()).ToArray()));
            GrubCostPreset = new MenuPreset<GrubCostRandomizerSettings>(StartPage, "Grub Cost Randomization",
                GrubCostData.GrubCostPresets, Settings.GrubCostRandomizerSettings, grubCostMEF,
                gc => gc.Caption());
            LongLocationPreset = new MenuPreset<LongLocationSettings>(StartPage, "Long Locations",
                LongLocationHintData.LongLocationPresets, Settings.LongLocationSettings, longLocationMEF,
                ll => ll.Caption(Settings));
            EssenceCostPreset = new MenuPreset<EssenceCostRandomizerSettings>(StartPage, "Essence Cost Randomization",
                EssenceCostData.EssencePresets, Settings.EssenceCostRandomizerSettings, essenceCostMEF,
                ec => ec.Caption());
            CursedPreset = new MenuPreset<CursedSettings>(StartPage, "Curses",
                CurseData.CursedPresets, Settings.CursedSettings, cursedMEF,
                cs => cs.Caption());


            startLocationSwitch = new RadioSwitch(AdvancedSettingsPage, StartDef.StartDefs.Select(def => def.name).ToArray());
            startLocationSwitch.Changed += Settings.StartLocationSettings.SetStartLocation;

            //startItemMEF = new MenuElementFactory<StartItemSettings>(AdvancedSettingsPage, Settings.StartItemSettings);
            //startItemClassMEFs = Settings.StartItemSettings.Items.Select(i => new MenuElementFactory<StartItemClass>(AdvancedSettingsPage, i)).ToArray();
            
            miscMEF = new MenuElementFactory<MiscSettings>(AdvancedSettingsPage, Settings.MiscSettings);
        }

        private void MakePanels()
        {
            //StartVIP = new VerticalItemPanel(StartPage, new Vector2(-480, 450), 125f, PresetButtons);
            StartGIP = new GridItemPanel(StartPage, new Vector2(0, 450), 2, 125, 960, PresetButtons);

            PoolSubpage = new Subpage(AdvancedSettingsPage, "Randomized Items");
            poolGIP = new GridItemPanel(AdvancedSettingsPage, new Vector2(0, 300), 4, 50f, 300f, poolMEF.Elements);
            PoolSubpage.Add(poolGIP);

            SkipSubpage = new Subpage(AdvancedSettingsPage, "Required Skips");
            skipVIP = new VerticalItemPanel(AdvancedSettingsPage, new Vector2(0, 300), 50f, skipMEF.Elements);
            SkipSubpage.Add(skipVIP);

            CursedSubpage = new Subpage(AdvancedSettingsPage, "Curse Options");
            cursedVIP = new VerticalItemPanel(AdvancedSettingsPage, new Vector2(0, 300), 50f, cursedMEF.Elements);
            CursedSubpage.Add(cursedVIP);

            CostSubpage = new Subpage(AdvancedSettingsPage, "Cost Randomization");
            costGIP = new GridItemPanel(AdvancedSettingsPage, new Vector2(0, 300), 4, 200f, 400f, grubCostMEF.Elements.Concat(essenceCostMEF.Elements).ToArray());
            CostSubpage.Add(costGIP);

            LongLocationSubpage = new Subpage(AdvancedSettingsPage, "Long Location Options");
            longVIP = new VerticalItemPanel(AdvancedSettingsPage, new Vector2(0, 300), 50f, longLocationMEF.Elements);
            LongLocationSubpage.Add(longVIP);

            StartLocationSubpage = new Subpage(AdvancedSettingsPage, "Start Location");
            startLocVIP = new VerticalItemPanel(AdvancedSettingsPage, new Vector2(0, 300), 50f, startLocMEF.Elements);
            StartLocationSubpage.Add(startLocVIP);
            startLocationGIP = new GridItemPanel(AdvancedSettingsPage, new Vector2(0, 100), 5, 50f, 350f, startLocationSwitch.Elements);
            StartLocationSubpage.Add(startLocationGIP);

            StartItemSubpage = new Subpage(AdvancedSettingsPage, "Start Items");
            // TODO


            MiscSubpage = new Subpage(AdvancedSettingsPage, "Miscellaneous");
            miscVIP = new VerticalItemPanel(AdvancedSettingsPage, new Vector2(0, 300), 50f, miscMEF.Elements);
            MiscSubpage.Add(miscVIP);

            AdvancedSettingsViewer = new OrderedItemViewer(AdvancedSettingsPage, AdvancedSettingsSubpages);

            JumpButtons = AdvancedSettingsSubpages.Select(p =>
            {
                SmallButton b = new SmallButton(JumpToSettingsPage, p.TitleLabel.Text.text);
                b.Button.AddHideAndShowEvent(JumpToSettingsPage, AdvancedSettingsPage);
                b.Button.AddEvent(() => AdvancedSettingsViewer.JumpTo(p));
                return b;
            }).ToArray();
            JumpPanel = new GridItemPanel(JumpToSettingsPage, new Vector2(0, 300), 2, 60f, 800f, JumpButtons);

            StartCornerVIP = new VerticalItemPanel(StartPage, new Vector2(-650, -350), 50f, StartCornerButtons);
        }

        private void AddEvents()
        {
            StartButton.Button.AddHideAllMenuPagesEvent();
            StartButton.Button.AddSetResumeKeyEvent("Randomizer");

            entryButton.Button.AddHideAndShowEvent(ModeMenu.ModePage, StartPage);
            JumpToJumpPageButton.Button.AddHideAndShowEvent(StartPage, JumpToSettingsPage);
            JumpToGameSettingsButton.Button.AddHideAndShowEvent(StartPage, GameSettingsPage);

            //CodeUpdater.Button.AddEvent(() => SettingsCode.Text.text = Settings.ToString());
        }

        private void Arrange()
        {
            StartButton.MoveTo(new Vector2(0, -350));
            SeedEntryField.MoveTo(new Vector2(650, -350));
            //CodeUpdater.MoveTo(new Vector2(0, -100));
            //SettingsCode.MoveTo(new Vector2(0, -150));
        }
    }
}
