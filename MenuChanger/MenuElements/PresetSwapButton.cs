using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


namespace MenuChanger.MenuElements
{
    [Obsolete]
    public class PresetSwapButton<T> : MenuItem<string>
    {
        Dictionary<string, PresetDef<T>> Presets;
        Dictionary<string, MenuItem<T>> subButtons;

        public PresetSwapButton(MenuPage page, string name, 
            Dictionary<string, PresetDef<T>> Presets, Dictionary<string, MenuItem<T>> subButtons) 
            : base(page, name, Presets.Keys.ToArray())
        {
            this.Presets = Presets;
            this.subButtons = subButtons;

            Changed += UpdatePreset;
            foreach (var kvp in subButtons) kvp.Value.Changed += UpdateSelection;

            UpdatePreset(null);
        }

        public void UpdatePreset(MenuItem<string> m)
        {
            if (Presets.TryGetValue(CurrentSelection, out PresetDef<T> preset))
            {
                preset.ApplyPreset(subButtons);
            }
        }

        public void UpdateSelection(MenuItem<T> m)
        {
            string p = Presets.FirstOrDefault(kvp => kvp.Value.CheckPreset(subButtons)).Key;
            if (!string.IsNullOrEmpty(p)) SetSelection(p, true);
            else SetSelection("Custom", true);
        }

    }

    [Obsolete]
    public class PresetDef<T>
    {
        public Dictionary<string, T> Presets;
        public bool SetUnusedButtonsToValue;
        public T UnusedButtonValue;

        public void ApplyPreset(Dictionary<string, MenuItem<T>> buttons)
        {
            foreach (KeyValuePair<string, MenuItem<T>> kvp in buttons)
            {
                if (Presets.TryGetValue(kvp.Key, out T t))
                {
                    kvp.Value.SetSelection(t, true);
                }
                else if (SetUnusedButtonsToValue)
                {
                    kvp.Value.SetSelection(UnusedButtonValue, true);
                }
            }
        }

        public bool CheckPreset(Dictionary<string, MenuItem<T>> buttons)
        {
            return buttons.All(pair => Presets.TryGetValue(pair.Key, out T t) ? pair.Value.CurrentSelection.Equals(t) :
            (SetUnusedButtonsToValue && pair.Value.CurrentSelection.Equals(UnusedButtonValue)));
        }
    }
}
