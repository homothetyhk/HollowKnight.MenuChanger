using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MenuChanger.Demos.RandomizerData
{
    public static class Captions
    {


        public static string Caption(this GrubCostRandomizerSettings gc)
        {
            return !gc.RandomizeGrubItemCosts ?
            "Grub reward items will have vanilla costs."
            : $"Grub reward items will be randomized to costs between " +
            $"{gc.MinimumGrubCost} and {gc.MaximumGrubCost}. For each item, the randomizer will guarantee " +
            $"{gc.GrubTolerance} grub(s) beyond the listed cost are accessible before " +
            $"the item is expected in logic.";
        }

        public static string Caption(this EssenceCostRandomizerSettings ec)
        {
            return !ec.RandomizeEssenceItemCosts ?
            "Seer reward items will have vanilla costs."
            : $"Seer reward items will be randomized to costs between " +
            $"{ec.MinimumEssenceCost} and {ec.MaximumEssenceCost}. For each item, the randomizer will guarantee " +
            $"{ec.EssenceTolerance} essence beyond the listed cost is accessible before " +
            $"the item is expected in logic.";
        }

        public static string Caption(this LongLocationSettings ll, GenerationSettings Settings)
        {
            StringBuilder sb = new StringBuilder();
            switch (ll.RandomizationInWhitePalace)
            {
                case LongLocationSettings.WPSetting.ExcludePathOfPain:
                    sb.Append("Locations (such as soul totems) in Path of Pain will not be randomized. ");
                    break;
                case LongLocationSettings.WPSetting.ExcludeWhitePalace:
                    sb.Append("Locations (such as King Fragment and soul totems) in White Palace will not be randomized. ");
                    break;
            }
            switch (ll.BossEssenceRandomization)
            {
                case LongLocationSettings.BossEssenceSetting.ExcludeAllDreamBosses when Settings.PoolSettings.BossEssence:
                    sb.Append("Dream Boss essence rewards will not be randomized. ");
                    break;
                case LongLocationSettings.BossEssenceSetting.ExcludeAllDreamWarriors when Settings.PoolSettings.BossEssence:
                    sb.Append("Dream Warrior essence rewards will not be randomized. ");
                    break;
                case LongLocationSettings.BossEssenceSetting.ExcludeGreyPrinceZoteAndWhiteDefender when Settings.PoolSettings.BossEssence:
                    sb.Append("Grey Prince Zote and White Defender essence rewards will not be randomized. ");
                    break;
            }
            switch (ll.CostItemHints)
            {
                case LongLocationSettings.CostItemHintSettings.CostOnly:
                    sb.Append("Item dialogue boxes will not display the item name, and will show only the cost of the item. ");
                    break;
                case LongLocationSettings.CostItemHintSettings.NameOnly:
                    sb.Append("Item dialogue boxes will not display the cost, and will show only the name of the item. ");
                    break;
                case LongLocationSettings.CostItemHintSettings.None:
                    sb.Append("Item dialogue boxes will show neither the cost nor the name of the item. ");
                    break;
            }
            switch (ll.LongLocationHints)
            {
                case LongLocationSettings.LongLocationHintSetting.Standard:

                    sb.Append("Hints are provided for the items (if randomized) " +
                        (ll.RandomizationInWhitePalace != LongLocationSettings.WPSetting.ExcludeWhitePalace ?
                        "at King Fragment, " : string.Empty) +
                        (ll.BossEssenceRandomization != LongLocationSettings.BossEssenceSetting.ExcludeAllDreamBosses &&
                        ll.BossEssenceRandomization != LongLocationSettings.BossEssenceSetting.ExcludeGreyPrinceZoteAndWhiteDefender ?
                        "at Grey Prince Zote, " : string.Empty) +
                        "in the colosseum, " +
                        "and behind Flower Quest. ");
                    break;
            }

            return sb.ToString();
        }

        public static string Caption(this CursedSettings cs)
        {
            StringBuilder sb = new StringBuilder();
            if (cs.RandomCurses) sb.Append("A random assortment of curses. ");
            if (cs.RandomizeFocus) sb.Append("The ability to heal is randomized. ");
            if (cs.ReplaceJunkWithOneGeo) sb.Append("Luxury items like mask shards and pale ore and the like are replaced with 1 geo pickups. ");
            if (cs.RemoveSpellUpgrades) sb.Append("Spell upgrades are completely removed. ");
            if (cs.LongerProgressionChains) sb.Append("Progression items are harder to find on average. ");
            if (cs.SplitClaw) sb.Append("The abilities to walljump from left and right slopes are separated. ");
            if (cs.SplitCloak) sb.Append("The abilities to dash left and right are separated. ");
            if (cs.RandomizeNail) sb.Append("The abilities to swing the nail in each direction are randomized. ");

            return sb.ToString();
        }

        public static string Caption(this StartLocationSettings sl)
        {
            switch (sl.StartLocationType)
            {
                default:
                case StartLocationSettings.RandomizeStartLocationType.Fixed:
                    return $"The randomizer will start at {sl.StartLocation}";
                case StartLocationSettings.RandomizeStartLocationType.RandomExcludingKP:
                    return $"The randomizer will start at a random location. " +
                        $"It will not start at King's Pass or any location that requires additional items.";
                case StartLocationSettings.RandomizeStartLocationType.Random:
                    return $"The randomizer will start at a random location. " +
                        $"It will not start at any location that requires additional items.";
                case StartLocationSettings.RandomizeStartLocationType.RandomWithMinorForcedStartItems:
                    return $"The randomizer will start at a random location. " +
                        $"Some starting items may be given to allow progress from the location. " +
                        $"These items may include keys, stags, or skills, but not vertical movement. ";
                case StartLocationSettings.RandomizeStartLocationType.RandomWithAnyForcedStartItems:
                    return $"The randomizer will start at a random location. " +
                        $"Many starting items may be given to allow progress from the location. ";
            }
        }



    }
}
