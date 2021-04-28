using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MenuChanger.Demos.RandomizerData
{
    public static class EssenceCostData
    {
        public static EssenceCostRandomizerSettings Standard;
        public static EssenceCostRandomizerSettings More;
        public static EssenceCostRandomizerSettings Less;
        public static EssenceCostRandomizerSettings Expert;
        public static EssenceCostRandomizerSettings Vanilla;
        public static Dictionary<string, EssenceCostRandomizerSettings> EssencePresets;

        static EssenceCostData()
        {
            Standard = new EssenceCostRandomizerSettings
            {
                RandomizeEssenceItemCosts = true,
                EssenceTolerance = 150,
                MinimumEssenceCost = 1,
                MaximumEssenceCost = 900,
            };
            More = new EssenceCostRandomizerSettings
            {
                RandomizeEssenceItemCosts = true,
                EssenceTolerance = 200,
                MinimumEssenceCost = 1,
                MaximumEssenceCost = 1800,
            };
            Less = new EssenceCostRandomizerSettings
            {
                RandomizeEssenceItemCosts = true,
                EssenceTolerance = 150,
                MinimumEssenceCost = 1,
                MaximumEssenceCost = 600,
            };
            Expert = new EssenceCostRandomizerSettings
            {
                RandomizeEssenceItemCosts = true,
                EssenceTolerance = 20,
                MinimumEssenceCost = 1,
                MaximumEssenceCost = 1800,
            };
            Vanilla = new EssenceCostRandomizerSettings
            {
                RandomizeEssenceItemCosts = false,
                EssenceTolerance = 0,
                MinimumEssenceCost = 1,
                MaximumEssenceCost = 1,
            };
            EssencePresets = new Dictionary<string, EssenceCostRandomizerSettings>
            {
                { "Standard", Standard },
                { "More", More },
                { "Less", Less },
                { "Expert", Expert },
                { "Vanilla", Vanilla },
            };
        }
    }
}
