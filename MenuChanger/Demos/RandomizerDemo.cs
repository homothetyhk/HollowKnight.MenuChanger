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

        MenuPage StartPage;
        MenuPage ResumePage;
        MenuPage AdvancedSettingsPage;

        GenerationSettings Settings = new GenerationSettings();

        BigButton StartButton;
        NumericEntryField SeedEntryField;
        MenuPreset<PoolSettings> PoolPreset;
        MenuPreset<SkipSettings> SkipPreset;
        VerticalItemPanel StartVIP;
        MenuLabel SettingsCode;
        SmallButton CodeUpdater;

        SmallButton advancedSettingsButton;
        OrderedItemViewer advancedSettingsViewer;

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
        MenuElementFactory<CostRandomizerSettings> costMEF;
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
        


        public RandomizerDemo()
        {
            MakeMenuPages();
            MakeMenuElements();
            MakePanels();
            AddEvents();
            Arrange();


            //StartPage.Add(startButton.GameObject);
            //PrefabMenuObjects.RescaleModeButton(startButton.GameObject);
            ResumeMenu.AddResumePage("Randomizer", ResumePage);
            ModeMenu.AddButtonToModeSelect(entryButton);
            SeedEntryField.InputValue = (new System.Random()).Next(0, 999999999);
        }

        private void MakeMenuPages()
        {
            StartPage = MenuPage.Create("Randomizer Setting Select Page", ModeMenu.ModePage);
            AdvancedSettingsPage = MenuPage.Create("Randomizer Advanced Settings Page", StartPage);
            ResumePage = MenuPage.Create("Randomizer Resume Page");

            
        }

        private void MakeMenuElements()
        {
            entryButton = new BigButton(ModeMenu.ModePage, MenuChanger.Sprites["logo"], "Randomizer 3");
            
            advancedSettingsButton = new SmallButton(StartPage, "Advanced Settings");

            StartButton = new BigButton(StartPage, Mode.Classic); // TODO: constructor for default sprite, custom text
            SeedEntryField = new NumericEntryField(StartPage, "Seed");
            

            CodeUpdater = new SmallButton(StartPage, "Compute Settings Code");
            SettingsCode = new MenuLabel(StartPage, "", MenuLabel.Style.Body);

            PoolPreset = new MenuPreset<PoolSettings>(StartPage, "Randomized Items", PoolData.PoolPresets, Settings.PoolSettings, true);
            SkipPreset = new MenuPreset<SkipSettings>(StartPage, "Required Skips", SkipData.SkipPresets, Settings.SkipSettings, true);

            poolMEF = new MenuElementFactory<PoolSettings>(AdvancedSettingsPage, Settings.PoolSettings);
            skipMEF = new MenuElementFactory<SkipSettings>(AdvancedSettingsPage, Settings.SkipSettings);

            PoolPreset.Pair(poolMEF);
            SkipPreset.Pair(skipMEF);

            cursedMEF = new MenuElementFactory<CursedSettings>(AdvancedSettingsPage, Settings.CursedSettings);
            costMEF = new MenuElementFactory<CostRandomizerSettings>(AdvancedSettingsPage, Settings.CostRandomizerSettings);
            longLocationMEF = new MenuElementFactory<LongLocationSettings>(AdvancedSettingsPage, Settings.LongLocationSettings);
            startLocMEF = new MenuElementFactory<StartLocationSettings>(AdvancedSettingsPage, Settings.StartLocationSettings);

            Dictionary<string, StartDef> startLocations = StartDef.GetStartLocations();
            startLocationSwitch = new RadioSwitch(AdvancedSettingsPage, startLocations.Keys.ToArray());
            startLocationSwitch.Changed += Settings.StartLocationSettings.SetStartLocation;
            Settings.StartLocationSettings.StartLocation = startLocationSwitch.CurrentSelection;

            //startItemMEF = new MenuElementFactory<StartItemSettings>(AdvancedSettingsPage, Settings.StartItemSettings);
            //startItemClassMEFs = Settings.StartItemSettings.Items.Select(i => new MenuElementFactory<StartItemClass>(AdvancedSettingsPage, i)).ToArray();
            
            miscMEF = new MenuElementFactory<MiscSettings>(AdvancedSettingsPage, Settings.MiscSettings);
        }

        private void MakePanels()
        {
            StartVIP = new VerticalItemPanel(StartPage, new Vector2(0, 300), 80f,
                PoolPreset,
                SkipPreset,
                SeedEntryField,
                StartButton);

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
            costGIP = new GridItemPanel(AdvancedSettingsPage, new Vector2(0, 300), 4, 200f, 400f, costMEF.Elements);
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

            advancedSettingsViewer = new OrderedItemViewer(AdvancedSettingsPage, AdvancedSettingsSubpages);
        }

        private void AddEvents()
        {
            StartButton.Button.AddHideAllMenuPagesEvent();
            //startButton.Button.AddEvent(() => MenuChanger.instance.Settings.resumeKey = "Randomizer");
            StartButton.Button.AddSetResumeKeyEvent("Randomizer");

            entryButton.Button.AddHideAndShowEvent(ModeMenu.ModePage, StartPage);
            advancedSettingsButton.Button.AddHideAndShowEvent(StartPage, AdvancedSettingsPage);
            CodeUpdater.Button.AddEvent(() => SettingsCode.Text.text = BinaryFormatting.Serialize(Settings));
        }

        private void Arrange()
        {
            //StartButton.MoveTo(new Vector2(0, 0));
            advancedSettingsButton.MoveTo(new Vector2(-650, -400));
            //SeedEntryField.MoveTo(new Vector2(0, 100));
            CodeUpdater.MoveTo(new Vector2(0, -100));
            SettingsCode.MoveTo(new Vector2(0, -150));
        }

    }
}
