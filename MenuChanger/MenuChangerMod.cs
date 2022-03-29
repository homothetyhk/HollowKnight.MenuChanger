using GlobalEnums;
using Modding;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MenuChanger
{
    public class MenuChangerMod : Mod, ILocalSettings<Settings>
    {
        internal static MenuChangerMod instance { get; private set; }
        internal static readonly List<MenuPage> displayedPages = new();

        public static event Action OnExitMainMenu;

        public static void HideAllMenuPages()
        {
            while (displayedPages.FirstOrDefault() is MenuPage page) page.Hide();
        }

        private void OnSceneChange(Scene from, Scene to)
        {
            if (from.name == "Menu_Title" && to.name != "Menu_Title")
            {
                ModeMenu.OnExitMainMenu();
                PrefabMenuObjects.Dispose();
                try
                {
                    OnExitMainMenu?.Invoke();
                }
                catch (Exception e)
                {
                    LogError($"Error invoking OnExitMainMenu:\n{e}");
                }
            }
        }

        private void EditUI()
        {
            displayedPages.Clear();
            ResumeMenu.Reset();
            ModeMenu.OnEnterMainMenu();
        }

        public override int LoadPriority() => 100000;

        public MenuChangerMod() : base("MenuChanger")
        {
            instance = this;
            LogHelper.OnLog += Log;
            LogHelper.OnLogError += LogError;
            UIManager.EditMenus += PrefabMenuObjects.Setup;
        }

        public override void Initialize()
        {
            ThreadSupport.Setup();
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChange;
            UIManager.EditMenus += EditUI;
            On.UIManager.UIGoToProfileMenu += (o, s) =>
            {
                HideAllMenuPages();
                o(s);
            };
            On.UIManager.UIGoToPlayModeMenu += (o, s) => 
            {
                s.StartCoroutine(GoToModeMenu(s));
            };
            ResumeMenu.Hook();
        }

        private IEnumerator GoToModeMenu(UIManager s)
        {
            InputHandler.Instance.StopUIInput();
            yield return s.HideSaveProfileMenu();
            ReflectionHelper.CallMethod(s, "SetMenuState", MainMenuState.PLAY_MODE_MENU);
            InputHandler.Instance.StartUIInput();
            ModeMenu.Show();
        }


        public Settings Settings = new();

        public override string GetVersion()
        {
            return _version;
        }

        private static readonly string _sha1;
        private static readonly string _version;
        static MenuChangerMod()
        {
            System.Reflection.Assembly a = typeof(MenuChangerMod).Assembly;

            using var sha1 = System.Security.Cryptography.SHA1.Create();
            using var sr = File.OpenRead(a.Location);
            _sha1 = Convert.ToBase64String(sha1.ComputeHash(sr));

            int buildHash;
            unchecked // stable string hash code
            {
                int hash1 = 5381;
                int hash2 = hash1;
                string str = _sha1;

                for (int i = 0; i < str.Length && str[i] != '\0'; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1 || str[i + 1] == '\0')
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                buildHash = hash1 + (hash2 * 1566083941);
                buildHash = Math.Abs(buildHash) % 997;
            }

            Version v = a.GetName().Version;
            _version = $"{v.Major}.{v.Minor}.{v.Build}+{buildHash.ToString().PadLeft(3, '0')}";
        }


        public void OnLoadLocal(Settings s)
        {
            Settings = s;
        }

        public Settings OnSaveLocal()
        {
            return Settings;
        }
    }
}
