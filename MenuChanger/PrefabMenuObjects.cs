using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MenuChanger.MenuElements;
using UnityEngine.UI.Extensions;
using System.Reflection;

namespace MenuChanger
{
    public static class PrefabMenuObjects
    {
        internal static GameObject classicModeButtonObjectPrefab;
        internal static MenuButton classicModeButtonPrefab;

        internal static GameObject steelModeButtonObjectPrefab;
        internal static MenuButton steelModeButtonPrefab;

        internal static GameObject ggModeButtonObjectPrefab;
        internal static MenuButton ggModeButtonPrefab;

        internal static GameObject backButtonObjectPrefab;
        internal static MenuButton backButtonPrefab;

        internal static GameObject descText;

        internal static void Setup()
        {
            classicModeButtonObjectPrefab = GameObject.Instantiate(UIManager.instance.playModeMenuScreen.defaultHighlight.gameObject);
            GameObject.DontDestroyOnLoad(classicModeButtonObjectPrefab);
            classicModeButtonPrefab = classicModeButtonObjectPrefab.GetComponent<MenuButton>();

            steelModeButtonObjectPrefab = GameObject.Instantiate(UIManager.instance.playModeMenuScreen.defaultHighlight.FindSelectableOnDown().gameObject);
            GameObject.DontDestroyOnLoad(steelModeButtonObjectPrefab);
            steelModeButtonPrefab = steelModeButtonObjectPrefab.GetComponent<MenuButton>();

            ggModeButtonObjectPrefab = GameObject.Instantiate(UIManager.instance.playModeMenuScreen.content.transform.Find("GGButton").gameObject);
            GameObject.DontDestroyOnLoad(ggModeButtonObjectPrefab);
            ggModeButtonPrefab = ggModeButtonObjectPrefab.GetComponent<MenuButton>();

            backButtonObjectPrefab = GameObject.Instantiate(UIManager.instance.playModeMenuScreen.defaultHighlight
                .FindSelectableOnDown().FindSelectableOnDown().gameObject);
            GameObject.DontDestroyOnLoad(backButtonObjectPrefab);
            backButtonPrefab = backButtonObjectPrefab.GetComponent<MenuButton>();

            descText = GameObject.Instantiate(
                UIManager.instance.extrasContentMenuScreen.content.gameObject.transform.Find("ScrollRect").Find("DescriptionText").gameObject);
            GameObject.Destroy(descText.GetComponent<SoftMaskScript>());
            GameObject.DontDestroyOnLoad(descText);
        }

        public static void Normalize(MenuPage page, params GameObject[] gs)
        {
            foreach (GameObject g in gs)
            {
                page.Add(g);
                g.transform.localPosition = Vector2.zero;
                g.transform.localScale = Vector2.one;
            }
        }

        public static (GameObject, Text, CanvasGroup) BuildDescText(MenuPage page, string text)
        {
            GameObject obj = GameObject.Instantiate(descText);
            
            // prevent null ref logs
            SoftMaskScript sms = obj.GetComponent<SoftMaskScript>();
            if (sms is SoftMaskScript)
            {
                typeof(Graphic).GetField("m_Canvas", BindingFlags.NonPublic | BindingFlags.Instance)
                    .SetValue(sms.GetComponent<Graphic>(), UIManager.instance.UICanvas);
                GameObject.DestroyImmediate(sms, true);
            }

            // set text and remove scrollbar mask
            Text t = obj.GetComponent<Text>();
            t.text = text;
            t.material = backButtonObjectPrefab.transform.Find("Text").GetComponent<Text>().material;

            // add to page and fix scale issues
            page.Add(obj);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            obj.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);

            // allow clicking things placed behind the text
            CanvasGroup cg = obj.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;

            return (obj, t, cg);
        }


        public static GameObject CloneModeButton(string name)
        {
            GameObject button = GameObject.Instantiate(classicModeButtonObjectPrefab);
            button.name = name;

            return button;
        }

        public static MenuButton CloneBackButton(string name)
        {
            GameObject button = GameObject.Instantiate(backButtonObjectPrefab);
            button.name = name;

            return button.GetComponent<MenuButton>();
        }

        public static void RescaleModeButton(GameObject button)
        {
            button.transform.localScale = new Vector2(0.6f, 0.6f);
            button.transform.Find("Selector").localScale = new Vector2(0.5f, 0.5f);
        }

        public static void RescaleBackButton(GameObject button)
        {
            button.transform.localScale = new Vector2(0.7f, 0.7f);
        }

        public static (MenuButton button, Text titleText, Text descText) CloneBigButton(Mode mode = Mode.Classic)
        {
            MenuButton button;
            switch (mode)
            {
                default:
                case Mode.Classic:
                    button = GameObject.Instantiate(classicModeButtonObjectPrefab).GetComponent<MenuButton>();
                    break;
                case Mode.Steel:
                    button = GameObject.Instantiate(steelModeButtonObjectPrefab).GetComponent<MenuButton>();
                    break;
                case Mode.Godmaster:
                    button = GameObject.Instantiate(ggModeButtonObjectPrefab).GetComponent<MenuButton>();
                    break;
            }
            button.buttonType = MenuButton.MenuButtonType.Proceed;
            GameObject.Destroy(button.gameObject.GetComponent<StartGameEventTrigger>());

            Transform textTrans = button.transform.Find("Text");
            GameObject.Destroy(textTrans.GetComponent<AutoLocalizeTextUI>());
            Text titleText = textTrans.GetComponent<Text>();
            textTrans.GetComponent<RectTransform>().sizeDelta = new Vector2(784f, 63f);

            Transform descTrans = button.transform.Find("DescriptionText");
            GameObject.Destroy(descTrans.GetComponent<AutoLocalizeTextUI>());
            Text descText = descTrans.GetComponent<Text>();

            return (button, titleText, descText);
        }

        public static MenuButton BuildBigButtonOneTextNoSprite(string title)
        {
            var obj = CloneBigButton();
            GameObject.Destroy(obj.button.transform.Find("Image").GetComponent<Image>());
            obj.titleText.text = title;
            obj.titleText.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -5f);
            obj.descText.text = string.Empty;

            return obj.button;
        }

        public static MenuButton BuildBigButtonTwoTextNoSprite(string title, string desc)
        {
            var obj = CloneBigButton();
            GameObject.Destroy(obj.button.transform.Find("Image").GetComponent<Image>());
            obj.titleText.text = title;
            obj.descText.text = desc;
            obj.titleText.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 28f);
            obj.descText.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -53f);

            return obj.button;
        }

        public static MenuButton BuildBigButtonTwoTextAndSprite(Sprite sprite, string title, string desc)
        {
            MenuButton button = GameObject.Instantiate(classicModeButtonObjectPrefab).GetComponent<MenuButton>();
            button.buttonType = MenuButton.MenuButtonType.Proceed;
            GameObject.Destroy(button.gameObject.GetComponent<StartGameEventTrigger>());

            Transform textTrans = button.transform.Find("Text");
            GameObject.Destroy(textTrans.GetComponent<AutoLocalizeTextUI>());
            textTrans.GetComponent<Text>().text = title ?? string.Empty;
            if (string.IsNullOrEmpty(desc))
            {
                textTrans.GetComponent<RectTransform>().anchoredPosition = new Vector2(130.5f, -5f);
            }
            // scaling issues with title text
            textTrans.GetComponent<RectTransform>().sizeDelta = new Vector2(784f, 63f);

            Transform descTrans = button.transform.Find("DescriptionText");
            GameObject.Destroy(descTrans.GetComponent<AutoLocalizeTextUI>());
            descTrans.GetComponent<Text>().text = desc ?? string.Empty;

            if (sprite != null)
            {
                button.transform.Find("Image").GetComponent<Image>().sprite = sprite;
            }

            return button;
        }

        public static MenuButton BuildBigButtonOneTextAndSprite(Sprite sprite, string title)
        {
            MenuButton button = GameObject.Instantiate(classicModeButtonObjectPrefab).GetComponent<MenuButton>();
            button.buttonType = MenuButton.MenuButtonType.Proceed;
            GameObject.Destroy(button.gameObject.GetComponent<StartGameEventTrigger>());

            Transform textTrans = button.transform.Find("Text");
            GameObject.Destroy(textTrans.GetComponent<AutoLocalizeTextUI>());
            textTrans.GetComponent<Text>().text = title ?? string.Empty;
            textTrans.GetComponent<RectTransform>().anchoredPosition = new Vector2(130.5f, -5f);

            // scaling issues with title text
            textTrans.GetComponent<RectTransform>().sizeDelta = new Vector2(784f, 63f);

            Transform descTrans = button.transform.Find("DescriptionText");
            GameObject.Destroy(descTrans.GetComponent<AutoLocalizeTextUI>());
            descTrans.GetComponent<Text>().text = string.Empty;

            if (sprite != null)
            {
                button.transform.Find("Image").GetComponent<Image>().sprite = sprite;
            }

            return button;
        }

        public static MenuButton BuildBigButtonSpriteOnly(Sprite sprite)
        {
            MenuButton button = GameObject.Instantiate(classicModeButtonObjectPrefab).GetComponent<MenuButton>();
            button.buttonType = MenuButton.MenuButtonType.Proceed;
            GameObject.Destroy(button.gameObject.GetComponent<StartGameEventTrigger>());

            GameObject.Destroy(button.transform.Find("Text").GetComponent<Text>());
            GameObject.Destroy(button.transform.Find("DescriptionText").GetComponent<Text>());
            Image i = button.transform.Find("Image").GetComponent<Image>();
            i.sprite = sprite;
            i.rectTransform.anchoredPosition = new Vector2(0f, 0f);
            i.rectTransform.localScale = new Vector2(2.8f, 2.8f);

            return button;
        }

        public static (GameObject, Text, CanvasGroup) BuildLabel(MenuPage page, string label)
        {
            GameObject obj = backButtonPrefab.Clone(label + " Label", MenuButton.MenuButtonType.Activate, Vector2.zero, label).gameObject;
            GameObject.Destroy(obj.GetComponent<EventTrigger>());
            GameObject.Destroy(obj.GetComponent<MenuButton>());

            page.Add(obj);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = new Vector3(0.7f, 0.7f, 1f);

            CanvasGroup cg = obj.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;

            return (obj, obj.transform.Find("Text").GetComponent<Text>(), cg);
        }

        public static MenuButton BuildNewButton(string text)
        {
            GameObject button = backButtonPrefab.Clone(text + " Button", MenuButton.MenuButtonType.Activate, Vector2.zero, text).gameObject;
            button.GetComponent<MenuButton>().ClearEvents();
            return button.GetComponent<MenuButton>();
        }

        public static (GameObject, InputField) BuildEntryField()
        {
            GameObject obj = backButtonPrefab.Clone("EntryField", MenuButton.MenuButtonType.Activate, Vector2.zero).gameObject;
            GameObject.DestroyImmediate(obj.GetComponent<MenuButton>());
            GameObject.DestroyImmediate(obj.GetComponent<EventTrigger>());
            GameObject.DestroyImmediate(obj.transform.Find("Text").GetComponent<AutoLocalizeTextUI>());
            GameObject.DestroyImmediate(obj.transform.Find("Text").GetComponent<FixVerticalAlign>());
            GameObject.DestroyImmediate(obj.transform.Find("Text").GetComponent<ContentSizeFitter>());

            RectTransform textRT = obj.transform.Find("Text").GetComponent<RectTransform>();
            textRT.anchorMin = textRT.anchorMax = new Vector2(0.5f, 0.5f);
            textRT.sizeDelta = new Vector2(337, 63.2f);

            InputField inputField = obj.AddComponent<InputField>();

            inputField.textComponent = obj.transform.Find("Text").GetComponent<Text>();

            inputField.caretColor = Color.white;
            inputField.contentType = InputField.ContentType.Standard;
            inputField.navigation = Navigation.defaultNavigation;
            inputField.caretWidth = 8;
            inputField.characterLimit = 9;
            inputField.text = string.Empty;

            return (obj, inputField);
        }

        public static (GameObject, InputField) BuildMultiLineEntryField(MenuPage page)
        {
            GameObject obj = GameObject.Instantiate(descText);

            // prevent null ref logs
            SoftMaskScript sms = obj.GetComponent<SoftMaskScript>();
            if (sms is SoftMaskScript)
            {
                typeof(Graphic).GetField("m_Canvas", BindingFlags.NonPublic | BindingFlags.Instance)
                    .SetValue(sms.GetComponent<Graphic>(), UIManager.instance.UICanvas);
                GameObject.DestroyImmediate(sms, true);
            }

            // remove scrollbar mask
            Text t = obj.GetComponent<Text>();
            t.material = backButtonObjectPrefab.transform.Find("Text").GetComponent<Text>().material;

            // add to page and fix scale issues
            page.Add(obj);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            obj.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
            try
            {
                GameObject.DestroyImmediate(obj.GetComponent<AutoLocalizeTextUI>());
                GameObject.DestroyImmediate(obj.GetComponent<FixVerticalAlign>());
                GameObject.DestroyImmediate(obj.GetComponent<ContentSizeFitter>());
            }
            catch (Exception e)
            {
                MenuChangerMod.instance.LogError(e);
            }

            // befuddling
            RectTransform textRT = obj.GetComponent<RectTransform>();
            textRT.anchorMin = textRT.anchorMax = new Vector2(0.5f, 0.5f);
            textRT.sizeDelta = new Vector2(450f, 800f);

            InputField inputField = obj.AddComponent<InputField>();

            inputField.textComponent = t;

            inputField.caretColor = Color.white;
            inputField.contentType = InputField.ContentType.Standard;
            inputField.navigation = Navigation.defaultNavigation;
            inputField.caretWidth = 8;
            inputField.characterLimit = 600;
            inputField.text = string.Empty;
            inputField.lineType = InputField.LineType.MultiLineSubmit;

            return (obj, inputField);
        }
    }
}
