using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using KSP.IO;

//using MuMech;

namespace DevHelper
{
    public partial class DevHelper : MonoBehaviour
    {
        public bool autoLoadSave = true;
        public string autoLoadSaveName = "default";

        public bool autoLoadScene = true;
        public string autoLoadSceneName = "VAB";

        private void log(string msg)
        {
            Debug.Log("[DevHelper] " + msg);
        }

        private List<string> saveNames;
        private void FindSaves()
        {
            log("FindSaves");
            var dirs = Directory.GetDirectories(KSPUtil.ApplicationRootPath + "saves\\");
            saveNames = dirs.Where(x => System.IO.File.Exists(x + "\\persistent.sfs")).Select(x => x.Split(new[] { '\\' })[1]).ToList();
        }

        //IButton DHReloadDatabase;
        private void Awake()
        {
            log("Injector awake");
            DontDestroyOnLoad(this);
        }

        void OnEnable()
        {
            log("Injector enabled");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Start()
        {
            log("DevHelper Starting");

            if (ToolbarManager.ToolbarAvailable)
            {
                DHButtons();
                log("buttons loaded");
            }

            loadConfigXML();
            FindSaves();

            InitComboBox();
            InitComboBoxScenes();
        }

        public void loadConfigXML()
        {
            PluginConfiguration config = PluginConfiguration.CreateForType<DevHelper>();
            config.load();

            autoLoadSave = config.GetValue<bool>("autoLoadSave");
            autoLoadSaveName = config.GetValue<string>("autoLoadSaveName");
            autoLoadScene = config.GetValue<bool>("autoLoadScene");
            autoLoadSceneName = config.GetValue<string>("autoLoadSceneName");
        }

        public void saveConfigXML()
        {
            PluginConfiguration config = PluginConfiguration.CreateForType<DevHelper>();
            config.SetValue("autoLoadSave", autoLoadSave);
            config.SetValue("autoLoadSaveName", autoLoadSaveName);
            config.SetValue("autoLoadScene", autoLoadScene);
            config.SetValue("autoLoadSceneName", autoLoadSceneName);
            config.save();
        }

        private bool bDoOnce = true;
        private void Update()
        {
            var menu = GameObject.Find("MainMenu");
            if (menu != null && bDoOnce)
            {
                bDoOnce = false;

                if (autoLoadSave)
                {
                    HighLogic.CurrentGame = GamePersistence.LoadGame("persistent", autoLoadSaveName, true, false);
                    if (HighLogic.CurrentGame != null)
                    {
                        HighLogic.SaveFolder = autoLoadSaveName;
                        //load to scene if needed
                        if (autoLoadScene)
                        {
                            switch (autoLoadSceneName)
                            {
                                case "VAB":
                                    HighLogic.CurrentGame.startScene = GameScenes.EDITOR;
                                    HighLogic.CurrentGame.editorFacility = EditorFacility.VAB;
                                    break;
                                case "SPH":
                                    HighLogic.CurrentGame.startScene = GameScenes.EDITOR;
                                    HighLogic.CurrentGame.editorFacility = EditorFacility.SPH;
                                    break;
                                case "Tracking Station":
                                    HighLogic.CurrentGame.startScene = GameScenes.TRACKSTATION;
                                    break;
                                case "Space Center":
                                    HighLogic.CurrentGame.startScene = GameScenes.SPACECENTER;
                                    break;
                                case "Flight":
                                    HighLogic.CurrentGame.startScene = GameScenes.FLIGHT;
                                    break;
                                default:
                                    HighLogic.CurrentGame.startScene = GameScenes.SPACECENTER;
                                    break;
                            }

                        }
                        HighLogic.CurrentGame.Start();
                    }
                }
                else
                {
                    //pop up load game dialog.
                    var mc = menu.GetComponent<MainMenu>();
                    mc.continueBtn.onPressed.Invoke();
                }
            }
        }
        private IButton DHReloadDatabase;

        //private BoxDrawable boxDrawable;
        internal void DHButtons()
        {
            // button that toggles its icon when clicked
            DHReloadDatabase = ToolbarManager.Instance.add("DevHelper", "DHReloadGD");
            DHReloadDatabase.TexturePath = "DevHelper/Textures/icon_buttonReload";
            DHReloadDatabase.ToolTip = "Reload Game Database";
            DHReloadDatabase.Visibility = new GameScenesVisibility(GameScenes.EDITOR);
            DHReloadDatabase.OnClick += (e) =>
            {
                GameDatabase.Instance.Recompile = true;
                GameDatabase.Instance.StartLoad();
                PartLoader.Instance.Recompile = true;
                PartLoader.Instance.StartLoad();
            };
        }

        void OnDestroy()
        {
            if (ToolbarManager.ToolbarAvailable)
            {
                DHReloadDatabase.Destroy();
            }
        }

        private bool isTooLateToLoad = false;
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            log("OnSceneLoaded for Scene " + scene.name);
            if (PSystemManager.Instance != null && ScaledSpace.Instance == null)
            {
                isTooLateToLoad = true;
                log("It's now too late to load");
                // we no longer need this callback
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }
    }
}

public class DevHelperPartlessLoader : KSP.Testing.UnitTest
{
    public DevHelperPartlessLoader()
    {
        DevHelperPluginWrapper.Initialize();
    }
}

public static class DevHelperPluginWrapper
{
    public static GameObject DevHelper;

    public static void Initialize()
    {
        if (GameObject.Find("DevHelper") == null)
        {
            DevHelper = new GameObject(
                "DevHelper",
                new [] {typeof (DevHelper.DevHelper)});
            UnityEngine.Object.DontDestroyOnLoad(DevHelper);
        }
    }
}


