using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using KSP.IO;

namespace DevHelper
{

    public partial class DevHelper : MonoBehaviour
    {

        GUIContent[] comboBoxList;
        GUIContent[] comboBoxSelectedList;
        private ComboBox comboBoxControl = new ComboBox();
        private ComboBox comboBoxControlSpecificScene = new ComboBox();
        private GUIStyle listStyle = new GUIStyle();

        private void InitComboBox()
        {
            comboBoxList = saveNames.Select(x=>new GUIContent(x)).ToArray();
            comboBoxControl.SelectedItemIndex = saveNames.FindIndex(x=>x==autoLoadSaveName);

            listStyle.normal.textColor = Color.white;
            listStyle.onHover.background =
            listStyle.hover.background = new Texture2D(2, 2);
            listStyle.hover.textColor = Color.blue;
            listStyle.padding.left =
            listStyle.padding.right =
            listStyle.padding.top =
            listStyle.padding.bottom = 4;
        }

        private void InitComboBoxScenes()
        {
            comboBoxSelectedList = new GUIContent[4];
            comboBoxSelectedList[0] = new GUIContent("VAB");
            comboBoxSelectedList[1] = new GUIContent("SPH");
            comboBoxSelectedList[2] = new GUIContent("Tracking Station");
            comboBoxSelectedList[3] = new GUIContent("Space Center");
            comboBoxControlSpecificScene.SelectedItemIndex = comboBoxSelectedList.ToList().FindIndex(x => x.text == autoLoadSceneName);
            //Debug.Log("scene index: " + comboBoxControlSpecificScene.SelectedItemIndex);
            listStyle.normal.textColor = Color.white;
            listStyle.onHover.background =
            listStyle.hover.background = new Texture2D(2, 2);
            listStyle.hover.textColor = Color.yellow;
            listStyle.padding.left =
            listStyle.padding.right =
            listStyle.padding.top =
            listStyle.padding.bottom = 4;
        }

        private void OnGUI()
        {
            if (!isTooLateToLoad)
            {
                //Auto load feature
                //Debug.Log("combosave " + autoLoadSaveName);
                autoLoadSave = GUI.Toggle(new Rect(20, 20, 160, 20), autoLoadSave, "Auto Load Savegame-->");
                int selectedItemIndex = comboBoxControl.SelectedItemIndex;
                selectedItemIndex = comboBoxControl.List(
                    new Rect(180, 20, 160, 20), comboBoxList[selectedItemIndex].text, comboBoxList, listStyle);
                autoLoadSaveName = comboBoxList[selectedItemIndex].text;
               
                //Load specific scene
                //Debug.Log("comboscene " + autoLoadSceneName);
                autoLoadScene = GUI.Toggle(new Rect(400, 20, 100, 20), autoLoadScene, "Load Scene");
                int selectedSceneIndex = comboBoxControlSpecificScene.SelectedItemIndex;
                selectedSceneIndex = comboBoxControlSpecificScene.List(
                    new Rect(490, 20, 160, 20), comboBoxSelectedList[selectedSceneIndex].text, comboBoxSelectedList, listStyle);
                autoLoadSceneName = comboBoxSelectedList[selectedSceneIndex].text;

                if (GUI.changed)
                {
                    Debug.Log("saving config from OnGUI:");
                    saveConfigXML();
                }
            }
        }

    }

    
    }

    // Popup list created by Eric Haines
    // ComboBox Extended by Hyungseok Seo.(Jerry) sdragoon@nate.com
    // 

    public class ComboBox
    {
        private static bool forceToUnShow = false;
        private static int useControlID = -1;
        private bool isClickedComboButton = false;

        private int selectedItemIndex = 0;

        public int List(Rect rect, string buttonText, GUIContent[] listContent, GUIStyle listStyle)
        {
            return List(rect, new GUIContent(buttonText), listContent, "button", "box", listStyle);
        }

        public int List(Rect rect, GUIContent buttonContent, GUIContent[] listContent, GUIStyle listStyle)
        {
            return List(rect, buttonContent, listContent, "button", "box", listStyle);
        }

        public int List(Rect rect, string buttonText, GUIContent[] listContent, GUIStyle buttonStyle, GUIStyle boxStyle, GUIStyle listStyle)
        {
            return List(rect, new GUIContent(buttonText), listContent, buttonStyle, boxStyle, listStyle);
        }

        public int List(Rect rect, GUIContent buttonContent, GUIContent[] listContent,
                                        GUIStyle buttonStyle, GUIStyle boxStyle, GUIStyle listStyle)
        {
            if (forceToUnShow)
            {
                forceToUnShow = false;
                isClickedComboButton = false;
            }

            bool done = false;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.mouseUp:
                    {
                        if (isClickedComboButton)
                        {
                            done = true;
                        }
                    }
                    break;
            }

            if (GUI.Button(rect, buttonContent, buttonStyle))
            {
                if (useControlID == -1)
                {
                    useControlID = controlID;
                    isClickedComboButton = false;
                }

                if (useControlID != controlID)
                {
                    forceToUnShow = true;
                    useControlID = controlID;
                }
                isClickedComboButton = true;
            }

            if (isClickedComboButton)
            {
                var listRect = new Rect(rect.x, rect.y + listStyle.CalcHeight(listContent[0], 1.0f),
                          rect.width, listStyle.CalcHeight(listContent[0], 1.0f) * listContent.Length);

                GUI.Box(listRect, "", boxStyle);
                int newSelectedItemIndex = GUI.SelectionGrid(listRect, selectedItemIndex, listContent, 1, listStyle);
                if (newSelectedItemIndex != selectedItemIndex)
                {
                    selectedItemIndex = newSelectedItemIndex;
                    GUI.changed = true;
                }
            }

            if (done)
                isClickedComboButton = false;

            return SelectedItemIndex;
        }

        public int SelectedItemIndex
        {
            get
            {
                return selectedItemIndex;
            }
            set {
                selectedItemIndex = value < 0 ? 0 : value;
            }
        }
    }
    



