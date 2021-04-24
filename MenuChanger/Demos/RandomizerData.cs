using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlobalEnums;
using MenuChanger.MenuElements;
using System.Xml;
using System.Reflection;
using System.IO;
using SereCore;

namespace MenuChanger
{
    public static class RandomizerData
    {



        public static readonly string[] PoolInfo =
        {
            "Dreamers: Lurien, Monomon, Herrah, World Sense" +
                "\n\t\u2022 \"World Sense\" is the ability found in Black Egg Temple to view the completion percentage" +
                "\nCharms: All 40 Charms" +
                "\n\t\u2022 Grimmchild is randomized, but its upgrades (including Carefree Melody) are not randomized" +
                "\n\t\u2022 In randomizer, collecting Grimmchild automatically activates the Nightmare Lantern" +
                "\n\t\t\u2022 Visiting Grimm is in logic after collecting Grimmchild" +
                "\n\t\u2022 If Grimmkin Flames are not randomized, then Grimmchild comes with 3 flames and its first upgrade" +
                "\n\t\t\u2022 In this case, the first 6 Grimmkin are deactivated" +
                "\n\t\u2022 The 2 Kingsoul fragments and Void Heart are randomized collectively as progressive items" +
                "\n\t\u2022 A hint for the item at King Fragment is available by interacting with the Kingsmould in Palace Grounds" +
                "\nSkills: Vengeful Spirit, Shade Soul, Howling Wraiths, Abyss Shriek, Desolate Dive, Descending Dark, " +
                "Mothwing Cloak, Shade Cloak, Mantis Claw, Crystal Heart, Isma's Tear, Monarch Wings, " +
                "Cyclone Slash, Great Slash, Dash Slash, Dream Nail, Dream Gate, Awoken Dream Nail" +
                "\n\t\u2022 Each spell and its upgrade are randomized as progressive items" +
                "\n\t\u2022 Mothwing Cloak and Shade Cloak are randomized as progressive items" +
                "\n\t\u2022 Dream Nail, Dream Gate, and Awoken Dream Nail are randomized as progressive items" +
                "\nKeys: City Crest, Lumafly Lantern, Tram Pass, Shopkeeper's Key, Elegant Key, Love Key, King's Brand, " +
                "Collector's Map, Godtuner, and the Simple Keys at Sly, City of Tears, Ancient Basin, and Pale Lurker" +
                "\n\t\u2022 The randomizer expects in logic that the first simple key is used to open Royal Waterways" +
                "\n\t\u2022 Simple Keys can be spent in other locations only once either Royal Waterways has been opened " +
                "or the player has collected two or more Simple Keys"
        };

        public static readonly string[] Settings =
        {
            "Mild Skips",
            "Shade Skips",
            "Fireball Skips",
            "Acid Skips",
            "Spike Tunnels",
            "Dark Rooms",
            "Spicy Skips"
        };

        public static readonly string[] Pools =
        {
            "Dreamers", "Skills",
            "Charms", "Keys",
            "Duplicate Major Items",
            "Mask Shards", "Vessel Fragments",
            "Pale Ore", "Charm Notches",
            "Geo Chests", "Relics",
            "Rancid Eggs", "Stags",
            "Maps", "Whispering Roots",
            "Grubs", "Lifeblood Cocoons",
            "Soul Totems", "Palace Totems",
            "Grimmkin Flames", "Geo Rocks",
            "Boss Essence"
        };

        public static readonly int[] PoolShape =
        {
            2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1
        };

        /*
        public static readonly Dictionary<string, PresetDef<bool>> DifficultyPresets = new Dictionary<string, PresetDef<bool>>
        {
            { 
                "Easy", 
                new MenuItemPreset<bool>
                {
                    SetUnusedButtonsToValue = true,
                    UnusedButtonValue = false,
                    Presets = new Dictionary<string, bool>()
                }
            },

            {
                "Medium",
                new MenuItemPreset<bool>
                {
                    SetUnusedButtonsToValue = true,
                    UnusedButtonValue = false,
                    Presets = new Dictionary<string, bool>
                    {
                        { "Mild Skips", true },
                        { "Shade Skips", true }
                    }
                }
            },

            {
                "Hard",
                new MenuItemPreset<bool>
                {
                    SetUnusedButtonsToValue = true,
                    UnusedButtonValue = true,
                    Presets = new Dictionary<string, bool>()
                }
            },
        };

        public static readonly Dictionary<string, PresetDef<bool>> PoolPresets = new Dictionary<string, PresetDef<bool>>
        {
            {
                "Standard",
                new MenuItemPreset<bool>
                {
                    SetUnusedButtonsToValue = true,
                    UnusedButtonValue = false,
                    Presets = new Dictionary<string, bool>
                    {
                        { "Dreamers", true },
                        { "Skills", true },
                        { "Charms", true },
                        { "Keys", true },
                        { "Duplicate Major Items", true },
                        { "Mask Shards", true },
                        { "Vessel Fragments", true },
                        { "Pale Ore", true },
                        { "Charm Notches", true },
                        { "Geo Chests", true },
                        { "Relics", true },
                        { "Rancid Eggs", true },
                        { "Stags", true },
                    }
                }
            },

            {
                "Super",
                new MenuItemPreset<bool>
                {
                    SetUnusedButtonsToValue = true,
                    UnusedButtonValue = false,
                    Presets = new Dictionary<string, bool>
                    {
                        { "Dreamers", true },
                        { "Skills", true },
                        { "Charms", true },
                        { "Keys", true },
                        { "Duplicate Major Items", true },
                        { "Mask Shards", true },
                        { "Vessel Fragments", true },
                        { "Pale Ore", true },
                        { "Charm Notches", true },
                        { "Geo Chests", true },
                        { "Relics", true },
                        { "Rancid Eggs", true },
                        { "Stags", true },
                        { "Maps", true },
                        { "Grubs", true },
                        { "Whispering Roots", true },
                    }
                }
            },

            {
                "LifeTotems",
                new MenuItemPreset<bool>
                {
                    SetUnusedButtonsToValue = true,
                    UnusedButtonValue = false,
                    Presets = new Dictionary<string, bool>
                    {
                        { "Dreamers", true },
                        { "Skills", true },
                        { "Charms", true },
                        { "Keys", true },
                        { "Duplicate Major Items", true },
                        { "Mask Shards", true },
                        { "Vessel Fragments", true },
                        { "Pale Ore", true },
                        { "Charm Notches", true },
                        { "Geo Chests", true },
                        { "Relics", true },
                        { "Rancid Eggs", true },
                        { "Stags", true },
                        { "Lifeblood Cocoons", true },
                        { "Soul Totems", true },
                        { "Palace Totems", true },
                    }
                }
            },

            {
                "Spoiler DAB",
                new MenuItemPreset<bool>
                {
                    SetUnusedButtonsToValue = true,
                    UnusedButtonValue = false,
                    Presets = new Dictionary<string, bool>
                    {
                        { "Dreamers", true },
                        { "Skills", true },
                        { "Charms", true },
                        { "Keys", true },
                        { "Duplicate Major Items", true },
                        { "Mask Shards", true },
                        { "Vessel Fragments", true },
                        { "Pale Ore", true },
                        { "Charm Notches", true },
                        { "Geo Chests", true },
                        { "Relics", true },
                        { "Rancid Eggs", true },
                        { "Stags", true },
                        { "Maps", true },
                        { "Whispering Roots", true },
                        { "Lifeblood Cocoons", true },
                        { "Soul Totems", true },
                    }
                }
            },

            {
                "EVERYTHING",
                new MenuItemPreset<bool>
                {
                    SetUnusedButtonsToValue = true,
                    UnusedButtonValue = true,
                    Presets = new Dictionary<string, bool>()
                }
            },

            {
                "Vanilla",
                new MenuItemPreset<bool>
                {
                    SetUnusedButtonsToValue = true,
                    UnusedButtonValue = false,
                    Presets = new Dictionary<string, bool>()
                }
            },
        };
        */
    }
}
