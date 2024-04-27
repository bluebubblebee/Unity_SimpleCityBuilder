using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CityBuilder
{
#if UNITY_EDITOR
    public class CityBuilderTool : EditorWindow
    {
        private int currentToolState = 0;

        private string woodResTextField = "";
        private string stoneResTextField = "";


        [MenuItem("Bsgg/CityBuilder/StatsTool")]
        public static void OpenCityBuilderTool()
        {
            GetWindow<CityBuilderTool>("Stats Tool");
        }

        private void OnGUI()
        {
            DisplayTitle(" Player Stats tool");

            if (currentToolState == 1)
            {
                DisplayPlayerResources();
            }

            GUILayout.Space(10.0f);
            if (GUILayout.Button(" Refresh"))
            {
                currentToolState = 1;
            }
        }

        private void DisplayTitle(string title)
        {
            DrawUILine(Color.cyan, 10, 2, 5);
            
            GUIStyle titleLabelStyle = new GUIStyle();
            titleLabelStyle.alignment = TextAnchor.MiddleCenter;
            titleLabelStyle.fontStyle = FontStyle.Bold;
            titleLabelStyle.fontSize = 18;
            titleLabelStyle.fixedWidth = 500;
            titleLabelStyle.fixedHeight = 20;
            titleLabelStyle.normal.textColor = Color.white;
            GUILayout.Label(title, titleLabelStyle);

            DrawUILine(Color.cyan, 10, 2, 5);
        }

        private void DrawUILine(Color color, int spacing = 10, int thickness = 2, int padding = 10)
        {
            GUILayout.Space(spacing);

            Rect contentRect = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            contentRect.height = thickness;            
            contentRect.y = padding / 2;
            contentRect.x -= 2;
            contentRect.width += 6;
            EditorGUI.DrawRect(contentRect, color);

            GUILayout.Space(spacing);            
        }               

        private void DisplayPlayerResources()
        {
            if ((GameManager.instance == null) || (GameManager.instance.PlayerController == null)) return;
            {
                int currentWoodRes = GameManager.instance.PlayerController.GetResource(ResourceType.Wood);
                int currentStoneRes = GameManager.instance.PlayerController.GetResource(ResourceType.Stone);

                GUILayout.Space(5.0f);

                GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();
                GUILayout.Label(" Current Wood: ", GetLabelTitleStyle(150));
                GUILayout.Label(currentWoodRes.ToString(), GetValueTitleStyle(120));

                GUILayout.Space(2.0f);
                GUILayout.Label(" - Update Wood: ", GetLabelTitleStyle(150));
                woodResTextField = GUILayout.TextField(woodResTextField, 100);
                GUILayout.EndHorizontal();

                GUILayout.Space(5.0f);

                GUILayout.BeginHorizontal();
                GUILayout.Label(" Current Stone: ", GetLabelTitleStyle(150));
                GUILayout.Label(currentStoneRes.ToString(), GetValueTitleStyle(120));

                GUILayout.Space(2.0f);
                GUILayout.Label(" - Update Stone: ", GetLabelTitleStyle(150));
                stoneResTextField = GUILayout.TextField(stoneResTextField, 100);
                GUILayout.EndHorizontal();

                GUILayout.Space(5.0f);
                //DrawUILine(Color.cyan, 3);

                if (GUILayout.Button(" Update Player Resources"))
                {
                    int newWoodResource = 0;
                    if (int.TryParse(woodResTextField, out newWoodResource))
                    {
                        GameManager.instance.PlayerController.SetResource(ResourceType.Wood, newWoodResource);
                    }

                    int newStoneResource = 0;
                    if (int.TryParse(stoneResTextField, out newStoneResource))
                    {
                        GameManager.instance.PlayerController.SetResource(ResourceType.Stone, newStoneResource);
                    }

                }

                GUILayout.EndVertical();
            }
        }

        private GUIStyle GetLabelTitleStyle(int width = 300)
        {
            GUIStyle labelTitleStyle = new GUIStyle();
            labelTitleStyle.fontStyle = FontStyle.Bold;
            labelTitleStyle.fontSize = 12;
            labelTitleStyle.fixedWidth = width;
            labelTitleStyle.normal.textColor = Color.cyan;

            return labelTitleStyle;
        }

        private GUIStyle GetValueTitleStyle(int width = 300)
        {
            GUIStyle labelTitleStyle = new GUIStyle();
            labelTitleStyle.fontStyle = FontStyle.Normal;
            labelTitleStyle.fontSize = 12;
            labelTitleStyle.fixedWidth = width;
            labelTitleStyle.fixedWidth = 20;
            labelTitleStyle.normal.textColor = Color.white;

            return labelTitleStyle;

        }

    }
#endif
}
