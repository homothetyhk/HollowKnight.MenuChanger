using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modding;

namespace MenuChanger.Demos
{
    public class RandomizerSettings : ModSettings
    {
        public GenerationSettings GenerationSettings;
        public SaveData SaveData;
    }

    public class SaveData
    {
        // TODO
    }

    public class GenerationSettings
    {
        public int Seed;
        public TransitionMode TransitionMode;
        public SkipSettings SkipSettings = new SkipSettings();
        public PoolSettings PoolSettings = new PoolSettings();
        public CursedSettings CursedSettings = new CursedSettings();
        public CostRandomizerSettings CostRandomizerSettings = new CostRandomizerSettings();
        public LongLocationSettings LongLocationSettings = new LongLocationSettings();
        public StartLocationSettings StartLocationSettings = new StartLocationSettings();
        public StartItemSettings StartItemSettings = new StartItemSettings();
        public MiscSettings MiscSettings = new MiscSettings();
    }

    public enum TransitionMode
    {
        None,
        AreaRandomizer,
        ConnectedAreaRoomRandomizer,
        RoomRandomizer
    }

    [Serializable]
    public class SkipSettings
    {
        public bool MildSkips;
        public bool ShadeSkips;
        public bool FireballSkips;
        public bool AcidSkips;
        public bool SpikeTunnels;
        public bool DarkRooms;
        public bool SpicySkips;
    }

    [Serializable]
    public class PoolSettings
    {
        public bool Dreamers;
        public bool Skills;
        public bool Charms;
        public bool Keys;
        public bool MaskShards;
        public bool VesselFragments;
        public bool PaleOre;
        public bool CharmNotches;
        public bool GeoChests;
        public bool Relics;
        public bool RancidEggs;
        public bool Stags;
        public bool Maps;
        public bool WhisperingRoots;
        public bool Grubs;
        public bool LifebloodCocoons;
        public bool SoulTotems;
        public bool GrimmkinFlames;
        public bool GeoRocks;
        public bool BossEssence;
        public bool BossGeo;
        public bool LoreTablets;
    }

    [Serializable]
    public class CursedSettings
    {
        public bool RandomizeFocus;
        public bool RandomizeNail;
        public bool LongerProgressionChains;
        public bool ReplaceJunkWithOneGeo;
        public bool SplitClaw;
        public bool SplitCloak;
    }

    [Serializable]
    public class CostRandomizerSettings
    {
        public bool RandomizeGrubItemCosts;
        public int MinimumGrubCost;
        public int MaximumGrubCost;
        public int GrubTolerance;

        public bool RandomizeEssenceItemCosts;
        public int MinimumEssenceCost;
        public int MaximumEssenceCost;
        public int EssenceTolerance;
    }

    [Serializable]
    public class LongLocationSettings
    {
        public enum WPSetting
        {
            Allowed,
            ExcludePathOfPain,
            ExcludeWhitePalace
        }

        public enum BossEssenceSetting
        {
            All,
            ExcludeGreyPrinceZoteAndWhiteDefender,
            ExcludeAllDreamBosses,
            ExcludeAllDreamWarriors
        }

        public WPSetting RandomizationInWhitePalace;
        public BossEssenceSetting BossEssenceRandomization;
        public bool LongLocationHints;
    }

    [Serializable]
    public class StartLocationSettings
    {
        public bool RandomizeStartLocation;

        [NonSerialized]
        public string StartLocation; // can't be done by reflection

        public void SetStartLocation(string loc) => StartLocation = loc;
    }

    [Serializable]
    public class StartItemSettings
    {
        public bool RandomizeStartGeo;
        public int MinimumStartGeo;
        public int MaximumStartGeo;

        public bool RandomizeStartItems;
        /*
        public StartItemClass Item1 = new StartItemClass();
        public StartItemClass Item2 = new StartItemClass();
        public StartItemClass Item3 = new StartItemClass();
        public StartItemClass Item4 = new StartItemClass();
        public StartItemClass Item5 = new StartItemClass();
        public StartItemClass Item6 = new StartItemClass();
        public StartItemClass Item7 = new StartItemClass();
        public StartItemClass Item8 = new StartItemClass();
        public StartItemClass Item9 = new StartItemClass();

        public StartItemClass[] Items => new StartItemClass[]
        {
            Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9,
        };
        */
    }

    [Serializable]
    public class StartItemClass
    {
        public ItemType Type;
        public ItemCount Count;

        public enum ItemType
        {
            None,
            VerticalMovement,
            HorizontalMovement,
            Movement,
            Key,
            SpecialItem,
            Stag,
            Charm
        }

        public enum ItemCount
        {
            Zero,
            ZeroOrMore,
            ExactlyOne,
            OneOrMore
        }
    }

    [Serializable]
    public class MiscSettings
    {
        public bool AddDuplicateItems;
        //public bool RandomizeNotchCosts;
        //public bool RandomizeShopLocations;
        //shhh
    }

}
