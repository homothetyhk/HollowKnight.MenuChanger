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
        GenerationSettings Settings => MenuChanger.gs.DefaultMenuSettings;
        GameSettings GameSettings;

        MenuPage ResumePage;
        MenuPage GameSettingsPage;

        //MenuLabel SettingsCode;
        //SmallButton CodeUpdater;


        #region Start

        MenuPage StartPage;

        BigButton StartButton;
        IntEntryField SeedEntryField;
        SmallButton RandomSeedButton;


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
        ToggleButton ToggleCaptionsButton;

        SmallButton[] StartCornerButtons => new SmallButton[]
        {
            JumpToJumpPageButton,
            JumpToGameSettingsButton,
            ToggleCaptionsButton,
        };
        VerticalItemPanel StartCornerVIP;

        #endregion

        #region Jump

        MenuPage JumpPage;
        SmallButton[] JumpButtons;
        GridItemPanel JumpPanel;

        SmallButton DefaultSettingsButton;
        SmallButton ToManageSettingsPageButton;

        #endregion

        #region Manage Settings

        MenuPage ManageSettingsPage;

        TextEntryField SettingsCodeField;
        SmallButton GenerateCodeButton;
        SmallButton ApplyCodeButton;
        IMenuElement[] CodeElements => new IMenuElement[]
        {
            GenerateCodeButton,
            ApplyCodeButton,
            SettingsCodeField,
        };
        VerticalItemPanel CodeVIP;

        TextEntryField ProfileName;
        MenuItem<MenuProfile> ProfileSwitch;
        SmallButton OverwriteProfileButton;
        SmallButton DeleteProfileButton;
        SmallButton SaveAsNewProfileButton;
        SmallButton ApplyProfileButton;
        IMenuElement[] ProfileElements => new IMenuElement[]
        {
            ProfileSwitch,
            OverwriteProfileButton,
            DeleteProfileButton,
            SaveAsNewProfileButton,
            ApplyProfileButton,
        };
        VerticalItemPanel ProfileVIP;

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

        #region Game Settings

        MenuElementFactory<GameSettings> gameSettingsMEF;
        MenuLabel preloadExplanationLabel;

        VerticalItemPanel gameSettingsVIP;

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
            ApplySettingsToMenu(Settings);
        }

        private void MakeMenuPages()
        {
            StartPage = new MenuPage("Randomizer Setting Select Page", ModeMenu.ModePage);
            JumpPage = new MenuPage("Randomizer Jump To Advanced Settings Page", StartPage);
            AdvancedSettingsPage = new MenuPage("Randomizer Advanced Settings Page", JumpPage);
            ManageSettingsPage = new MenuPage("Randomizer Manage Settings Page", JumpPage);
            GameSettingsPage = new MenuPage("Randomizer Game Settings Page", StartPage);

            ResumePage = new MenuPage("Randomizer Resume Page");
        }

        private void MakeMenuElements()
        {
            entryButton = new BigButton(ModeMenu.ModePage, MenuChanger.Sprites["logo"], "Randomizer 3");

            JumpToJumpPageButton = new SmallButton(StartPage, "Advanced Settings");
            JumpToGameSettingsButton = new SmallButton(StartPage, "Game Settings");
            ToggleCaptionsButton = new ToggleButton(StartPage, "Toggle Menu Captions");

            StartButton = new BigButton(StartPage, "Begin Randomization"); // TODO: constructor for default sprite, custom text
            SeedEntryField = new IntEntryField(StartPage, "Seed");
            RandomSeedButton = new SmallButton(StartPage, "Random");

            // The AdvancedSettingsPage Elements must be constructed before the StartPage preset buttons.
            poolMEF = new MenuElementFactory<PoolSettings>(AdvancedSettingsPage, Settings.PoolSettings);
            skipMEF = new MenuElementFactory<SkipSettings>(AdvancedSettingsPage, Settings.SkipSettings);
            cursedMEF = new MenuElementFactory<CursedSettings>(AdvancedSettingsPage, Settings.CursedSettings);
            grubCostMEF = new MenuElementFactory<GrubCostRandomizerSettings>(AdvancedSettingsPage, Settings.GrubCostRandomizerSettings);
            essenceCostMEF = new MenuElementFactory<EssenceCostRandomizerSettings>(AdvancedSettingsPage, Settings.EssenceCostRandomizerSettings);
            longLocationMEF = new MenuElementFactory<LongLocationSettings>(AdvancedSettingsPage, Settings.LongLocationSettings);
            startLocMEF = new MenuElementFactory<StartLocationSettings>(AdvancedSettingsPage, Settings.StartLocationSettings);
            startLocationSwitch = new RadioSwitch(AdvancedSettingsPage, StartDef.StartDefs.Select(def => def.name).ToArray());
            startLocationSwitch.Changed += Settings.StartLocationSettings.SetStartLocation;

            miscMEF = new MenuElementFactory<MiscSettings>(AdvancedSettingsPage, Settings.MiscSettings);

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

            DefaultSettingsButton = new SmallButton(JumpPage, "Restore Default Settings");
            ToManageSettingsPageButton = new SmallButton(JumpPage, "Manage Settings Profiles");

            GenerateCodeButton = new SmallButton(ManageSettingsPage, "Refresh Code");
            ApplyCodeButton = new SmallButton(ManageSettingsPage, "Apply Code To Menu");
            SettingsCodeField = new TextEntryField(ManageSettingsPage, "Shareable Settings Code");

            ProfileSwitch = new MenuItem<MenuProfile>(ManageSettingsPage, "Profile", MenuChanger.gs.Profiles);
            OverwriteProfileButton = new SmallButton(ManageSettingsPage, "Overwrite Selected Profile");
            DeleteProfileButton = new SmallButton(ManageSettingsPage, "Delete Selected Profile");
            SaveAsNewProfileButton = new SmallButton(ManageSettingsPage, "Save As New Profile");
            ApplyProfileButton = new SmallButton(ManageSettingsPage, "Apply Profile");

            gameSettingsMEF = new MenuElementFactory<GameSettings>(GameSettingsPage, GameSettings);
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
                SmallButton b = new SmallButton(JumpPage, p.TitleLabel.Text.text);
                b.Button.AddHideAndShowEvent(JumpPage, AdvancedSettingsPage);
                b.Button.AddEvent(() => AdvancedSettingsViewer.JumpTo(p));
                return b;
            }).ToArray();
            JumpPanel = new GridItemPanel(JumpPage, new Vector2(0, 300), 2, 60f, 800f, JumpButtons);

            StartCornerVIP = new VerticalItemPanel(StartPage, new Vector2(-650, -350), 50f, StartCornerButtons);

            preloadExplanationLabel = new MenuLabel(GameSettingsPage,
                "Disabling preloads allows the game to use less memory, and may help to prevent glitches such as " +
                "infinite loads or invisible bosses. Changes to this setting apply to all files, " +
                "but only take effect after restarting the game.",
                MenuLabel.Style.Body);
            gameSettingsVIP = new VerticalItemPanel(GameSettingsPage, new Vector2(0f, 300f), 100f, gameSettingsMEF.Elements);
            gameSettingsVIP.Insert(1, preloadExplanationLabel);


            CodeVIP = new VerticalItemPanel(ManageSettingsPage, new Vector2(-400, 300), 100, CodeElements);
            ProfileVIP = new VerticalItemPanel(ManageSettingsPage, new Vector2(400, 300), 100, ProfileElements);

        }

        private void AddEvents()
        {
            entryButton.Button.AddHideAndShowEvent(ModeMenu.ModePage, StartPage);

            StartButton.Button.AddHideAllMenuPagesEvent();
            StartButton.Button.AddSetResumeKeyEvent("Randomizer");

            RandomSeedButton.Button.AddEvent(() =>
            {
                SeedEntryField.InputValue = new System.Random().Next(0, 1000000000);
            });
            
            JumpToJumpPageButton.Button.AddHideAndShowEvent(StartPage, JumpPage);
            JumpToGameSettingsButton.Button.AddHideAndShowEvent(StartPage, GameSettingsPage);

            ToggleCaptionsButton.SetSelection(true, false);
            ToggleCaptionsButton.Changed += (self) =>
            {
                foreach (SmallButton button in PresetButtons)
                {
                    if (button is IMenuPreset preset) preset.Label?.SetVisibleByAlpha(self.CurrentSelection);
                }
            };

            grubCostMEF.ByteFields[nameof(GrubCostRandomizerSettings.MaximumGrubCost)]
                .ChangedSelf += (e) =>
                {
                    if (e.InputValue > 46) e.InputValue = 46;
                };

            ToManageSettingsPageButton.Button.AddHideAndShowEvent(JumpPage, ManageSettingsPage);
            DefaultSettingsButton.Button.AddEvent(() => ApplySettingsToMenu(new GenerationSettings())); // Proper defaults please!


            GenerateCodeButton.Button.AddEvent(() =>
            {
                SettingsCodeField.InputValue = Settings.Serialize();
            });
            ApplyCodeButton.Button.AddEvent(() =>
            {
                if (GenerationSettings.Deserialize(SettingsCodeField.InputValue) is GenerationSettings gs)
                {
                    ApplySettingsToMenu(gs);
                }
            });

            OverwriteProfileButton.Button.AddEvent(() =>
            {
                MenuProfile mp = new MenuProfile
                {
                    name = "Test",
                    settings = Settings.Clone() as GenerationSettings
                };
                ProfileSwitch.OverwriteCurrent(mp);
            });
            SaveAsNewProfileButton.Button.AddEvent(() =>
            {
                MenuProfile mp = new MenuProfile
                {
                    name = "Test",
                    settings = Settings.Clone() as GenerationSettings
                };
                ProfileSwitch.AddItem(mp);
            });
            DeleteProfileButton.Button.AddEvent(() =>
            {
                ProfileSwitch.RemoveCurrent();
            });
            ApplyProfileButton.Button.AddEvent(() =>
            {
                if (ProfileSwitch.CurrentSelection is MenuProfile mp)
                {
                    ApplySettingsToMenu(mp.settings);
                }
            });
        }

        private void Arrange()
        {
            StartButton.MoveTo(new Vector2(0, -350));
            SeedEntryField.MoveTo(new Vector2(650, -350));
            RandomSeedButton.MoveTo(new Vector2(650, -400));
            //CodeUpdater.MoveTo(new Vector2(0, -100));
            //SettingsCode.MoveTo(new Vector2(0, -150));

            DefaultSettingsButton.MoveTo(new Vector2(-400, -300));
            ToManageSettingsPageButton.MoveTo(new Vector2(400, -300));

        }

        private void ApplySettingsToMenu(GenerationSettings settings)
        {
            poolMEF.SetMenuValues(settings.PoolSettings, Settings.PoolSettings);
            skipMEF.SetMenuValues(settings.SkipSettings, Settings.SkipSettings);
            grubCostMEF.SetMenuValues(settings.GrubCostRandomizerSettings, Settings.GrubCostRandomizerSettings);
            essenceCostMEF.SetMenuValues(settings.EssenceCostRandomizerSettings, Settings.EssenceCostRandomizerSettings);
            longLocationMEF.SetMenuValues(settings.LongLocationSettings, Settings.LongLocationSettings);
            
            cursedMEF.SetMenuValues(settings.CursedSettings, Settings.CursedSettings);

            miscMEF.SetMenuValues(settings.MiscSettings, Settings.MiscSettings);

            foreach (IMenuPreset preset in PresetButtons) preset.UpdatePreset();
        }

    }
}
