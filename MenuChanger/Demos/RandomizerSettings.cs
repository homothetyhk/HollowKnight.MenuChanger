using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modding;

namespace MenuChanger.Demos
{
    public class RandomizerSettings : ModSettings
    {
        public GameSettings GameSettings = new GameSettings();
        public GenerationSettings GenerationSettings = new GenerationSettings();
        public SaveData SaveData = new SaveData();
    }

    public class SaveData
    {
        // TODO
    }

    public class GameSettings
    {
        public bool NPCItemDialogue;
        public bool JinnAppearsWithJiji;
        public bool RealGeoRocks;
        public bool RealGrubJars;

        public bool PreloadJinn;
        public bool PreloadGeoRocks;
    }

    public class GenerationSettings : ICloneable
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

        private object[] serializationFields => new object[]
        {
            SkipSettings,
            PoolSettings,
            CursedSettings,
            GrubCostRandomizerSettings,
            EssenceCostRandomizerSettings,
            LongLocationSettings,
            StartLocationSettings,
            StartItemSettings,
            MiscSettings
        };

        public string Serialize()
        {
            return string.Join(":", serializationFields.Select(o => BinaryFormatting.Serialize(o)).ToArray());
        }

        public static GenerationSettings Deserialize(string code)
        {
            GenerationSettings gs = new GenerationSettings();
            string[] pieces = code.Split(':');
            object[] fields = gs.serializationFields;

            if (pieces.Length != fields.Length)
            {
                MenuChanger.instance.LogWarn("Invalid settings code: not enough pieces.");
                return null;
            }

            for (int i = 0; i < fields.Length; i++)
            {
                BinaryFormatting.Deserialize(pieces[i], fields[i]);
            }

            return gs;
        }

        public object Clone()
        {
            return new GenerationSettings
            {
                Seed = Seed,
                TransitionMode = TransitionMode,
                SkipSettings = SkipSettings.Clone() as SkipSettings,
                PoolSettings = PoolSettings.Clone() as PoolSettings,
                GrubCostRandomizerSettings = GrubCostRandomizerSettings.Clone() as GrubCostRandomizerSettings,
                EssenceCostRandomizerSettings = EssenceCostRandomizerSettings.Clone() as EssenceCostRandomizerSettings,
                LongLocationSettings = LongLocationSettings.Clone() as LongLocationSettings,
                CursedSettings = CursedSettings.Clone() as CursedSettings,
                StartLocationSettings = StartLocationSettings.Clone() as StartLocationSettings,
                StartItemSettings = StartItemSettings.Clone() as StartItemSettings,
                MiscSettings = MiscSettings.Clone() as MiscSettings,
            };
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
    public class SkipSettings : ICloneable
    {
        public bool MildSkips;
        public bool ShadeSkips;
        public bool FireballSkips;
        public bool AcidSkips;
        public bool SpikeTunnels;
        public bool DarkRooms;
        public bool SpicySkips;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    [Serializable]
    public class PoolSettings : ICloneable
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

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    [Serializable]
    public class CursedSettings : ICloneable
    {
        public bool RandomCurses;
        public bool RandomizeFocus;
        public bool RandomizeNail;
        public bool LongerProgressionChains;
        public bool ReplaceJunkWithOneGeo;
        public bool RemoveSpellUpgrades;
        public bool SplitClaw;
        public bool SplitCloak;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class GrubCostRandomizerSettings : ICloneable
    {
        public bool RandomizeGrubItemCosts;
        public byte MinimumGrubCost;
        public byte MaximumGrubCost;
        public byte GrubTolerance;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class EssenceCostRandomizerSettings : ICloneable
    {
        public bool RandomizeEssenceItemCosts;
        public short MinimumEssenceCost;
        public short MaximumEssenceCost;
        public short EssenceTolerance;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    [Serializable]
    public class LongLocationSettings : ICloneable
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

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    [Serializable]
    public class StartLocationSettings : ICloneable
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

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    [Serializable]
    public class StartItemSettings : ICloneable
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

        public object Clone()
        {
            return MemberwiseClone();
        }
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
    public class MiscSettings : ICloneable
    {
        public bool AddDuplicateItems;
        //public bool RandomizeNotchCosts;
        //public bool RandomizeShopLocations;
        //shhh

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

}
