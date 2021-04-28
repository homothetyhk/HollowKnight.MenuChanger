using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MenuChanger.Demos.RandomizerData
{
    public static class StartLocationData
    {
        public static StartLocationSettings KingsPass;
        public static StartLocationSettings RandomNoKP;
        public static StartLocationSettings RandomWithKP;
        public static StartLocationSettings MoreRandom;
        public static StartLocationSettings MostRandom;

        public static Dictionary<string, StartLocationSettings> StartLocationPresets;

        static StartLocationData()
        {
            KingsPass = new StartLocationSettings
            {
                StartLocationType = StartLocationSettings.RandomizeStartLocationType.Fixed,
                StartLocationIndex = (byte)StartLocationSettings.StartLocationIndices.KingsPass,
            };

            RandomNoKP = new StartLocationSettings
            {
                StartLocationType = StartLocationSettings.RandomizeStartLocationType.Random,
                StartLocationIndex = (byte)StartLocationSettings.StartLocationIndices.KingsPass,
            };

            RandomWithKP = new StartLocationSettings
            {
                StartLocationType = StartLocationSettings.RandomizeStartLocationType.RandomExcludingKP,
                StartLocationIndex = (byte)StartLocationSettings.StartLocationIndices.KingsPass,
            };

            MoreRandom = new StartLocationSettings
            {
                StartLocationType = StartLocationSettings.RandomizeStartLocationType.RandomWithMinorForcedStartItems,
                StartLocationIndex = (byte)StartLocationSettings.StartLocationIndices.KingsPass,
            };

            MostRandom = new StartLocationSettings
            {
                StartLocationType = StartLocationSettings.RandomizeStartLocationType.RandomWithAnyForcedStartItems,
                StartLocationIndex = (byte)StartLocationSettings.StartLocationIndices.KingsPass,
            };

            StartLocationPresets = new Dictionary<string, StartLocationSettings>
            {
                { "King's Pass", KingsPass },
                { "Random (excluding King's Pass)", RandomNoKP },
                { "Random (including King's Pass", RandomWithKP },
                { "Random (More Starts)", MoreRandom },
                { "Random (All Starts)", MostRandom },
            };
        }
    }
}
