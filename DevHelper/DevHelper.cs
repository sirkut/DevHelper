using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using KSP.IO;

namespace DevHelper
{
    public partial class DevHelper : MonoBehaviour
    {
        public bool autoLoadSave = true;
        public string autoLoadSaveName = "default";

        public bool autoLoadScene = true;
        public string autoLoadSceneName = "VAB";

        private List<string> saveNames;
        private void FindSaves()
        {
            print("FindSaves");
            var dirs = Directory.GetDirectories(KSPUtil.ApplicationRootPath + "saves\\");
            saveNames = dirs.Where(x => System.IO.File.Exists(x + "\\persistent.sfs")).Select(x => x.Split(new[] { '\\' })[1]).ToList();
        }

        private void Awake()
        {
            print("Injector awake");
            DontDestroyOnLoad(this);
        }
        private void Start()
        {
            print("DevHelper Starting");

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
                                    break;
                                case "SPH":
                                    HighLogic.CurrentGame.startScene = GameScenes.SPH;
                                    break;
                                case "Tracking Station":
                                    HighLogic.CurrentGame.startScene = GameScenes.TRACKSTATION;
                                    break;
                                case "Space Center":
                                    HighLogic.CurrentGame.startScene = GameScenes.SPACECENTER;
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

        
        private bool isTooLateToLoad = false;
      

        public void OnLevelWasLoaded(int level)
        {
            print("OnLevelWasLoaded:" + level);

            if (PSystemManager.Instance != null && ScaledSpace.Instance == null)
            {
                isTooLateToLoad = true;
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


