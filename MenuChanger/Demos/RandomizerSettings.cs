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
        public GrubCostRandomizerSettings GrubCostRandomizerSettings = new GrubCostRandomizerSettings();
        public EssenceCostRandomizerSettings EssenceCostRandomizerSettings = new EssenceCostRandomizerSettings();
        public LongLocationSettings LongLocationSettings = new LongLocationSettings();
        public StartLocationSettings StartLocationSettings = new StartLocationSettings();
        public StartItemSettings StartItemSettings = new StartItemSettings();
        public MiscSettings MiscSettings = new MiscSettings();

        public override string ToString()
        {
            string[] comps = new string[]
            {
                BinaryFormatting.Serialize(SkipSettings),
                BinaryFormatting.Serialize(PoolSettings),
                BinaryFormatting.Serialize(CursedSettings),
                BinaryFormatting.Serialize(GrubCostRandomizerSettings),
                BinaryFormatting.Serialize(EssenceCostRandomizerSettings),
                BinaryFormatting.Serialize(LongLocationSettings),
                BinaryFormatting.Serialize(StartLocationSettings),
                BinaryFormatting.Serialize(StartItemSettings),
                BinaryFormatting.Serialize(MiscSettings),
            };

            return string.Join(":", comps);
        }

        public void FromString(string code)
        {

        }
    }

    public enum TransitionMode : byte
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
        public bool RandomCurses;
        public bool RandomizeFocus;
        public bool RandomizeNail;
        public bool LongerProgressionChains;
        public bool ReplaceJunkWithOneGeo;
        public bool RemoveSpellUpgrades;
        public bool SplitClaw;
        public bool SplitCloak;
    }

    public class GrubCostRandomizerSettings
    {
        public bool RandomizeGrubItemCosts;
        public byte MinimumGrubCost;
        public byte MaximumGrubCost;
        public byte GrubTolerance;
    }

    public class EssenceCostRandomizerSettings
    {
        public bool RandomizeEssenceItemCosts;
        public short MinimumEssenceCost;
        public short MaximumEssenceCost;
        public short EssenceTolerance;
    }

    [Serializable]
    public class LongLocationSettings
    {
        public enum WPSetting : byte
        {
            Allowed,
            ExcludePathOfPain,
            ExcludeWhitePalace
        }

        public enum BossEssenceSetting : byte
        {
            All,
            ExcludeGreyPrinceZoteAndWhiteDefender,
            ExcludeAllDreamBosses,
            ExcludeAllDreamWarriors
        }

        public enum CostItemHintSettings : byte
        {
            CostAndName,
            CostOnly,
            NameOnly,
            None
        }

        public enum LongLocationHintSetting : byte
        {
            Standard,
            MoreHints,
            None
        }

        public WPSetting RandomizationInWhitePalace;
        public BossEssenceSetting BossEssenceRandomization;
        public CostItemHintSettings CostItemHints;
        public LongLocationHintSetting LongLocationHints;
    }

    [Serializable]
    public class StartLocationSettings
    {
        public enum RandomizeStartLocationType : byte
        {
            Fixed,
            RandomExcludingKP,
            Random,
            RandomWithMinorForcedStartItems,
            RandomWithAnyForcedStartItems
        }

        public enum StartLocationIndices : byte
        {
            // All Mode Starts
            KingsPass,
            StagNest,
            WestCrossroads,
            EastCrossroads,
            AncestralMound,
            WestFogCanyon,
            EastFogCanyon,
            QueensStation,
            FungalWastes,
            Greenpath,
            CityStorerooms,
            KingsStation,
            OutsideColosseum,
            CrystallizedMound,
            // Restricted starts
            QueensGardens,
            FarGreenpath,
            RoyalWaterways,
            DistantVillage,
            KingdomsEdge,
            HallownestsCrown,
            // Max restricted starts
            FungalCore,
            Abyss,
            Hive,
            CityOfTears
        }

        public RandomizeStartLocationType StartLocationType;

        internal byte StartLocationIndex;
        public string StartLocation => RandomizerData.StartDef.StartDefs[StartLocationIndex].name;

        public void SetStartLocation(string start) => StartLocationIndex = (byte)RandomizerData.StartDef.StartDefs
            .Select((def, i) => (def, i))
            .First(p => p.def.name == start).i;
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

        public enum ItemType : byte
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

        public enum ItemCount : byte
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
