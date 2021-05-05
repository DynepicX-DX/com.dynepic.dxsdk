using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

public class MOTARModelDetailWindow : EditorWindow
{
    Texture2D previewBackgroundTexture;
    private GameObject gameObject;
    private GameObject actualPlane;
    Editor gameObjectEditor;

    public static Vector2 vMyPos;
    public static Vector2 vPreviousPos;
    public static VisualElement root;
    public static Box myBox;
    public static MOTARModelDetailWindow myWindow;
    public static bool PreviewModel = false;
    [MenuItem("MOTAR ModelDetail/Open _%#T")]
   
    public static void OpenPreviewWindow()
    {
        myWindow = GetWindow<MOTARModelDetailWindow>();
    }
    private void OnGUI()
    {
        if(myBox == null)
            myBox = root.Query<Box>("THUMBPLAY").ToList()[0];

        //Debug.LogWarning("ROOT RECT:" + root.worldBound.yMin);
        //Debug.LogWarning("RECT:" + myBox.worldBound.yMin);

        Vector2 v2 = GUIUtility.GUIToScreenPoint(new Vector2(myBox.worldBound.xMin - root.worldBound.xMin, myBox.worldBound.yMin-root.worldBound.yMin));

        vMyPos = v2;
        Debug.LogWarning("VPOS:" + v2);
        if(vPreviousPos != v2)
        {
            vPreviousPos = v2;
            if (PreviewModel)
                MOTARA10DemoPreviewWindow.ShowA10Animation();
            else
                MOTARA10DemoPreviewWindow.CloseWindow();

        }
    }
    private void Update()
    {
        //if (myBox != null)
        //{
        //    Vector2 v2 = GUIUtility.GUIToScreenPoint(new Vector2(myBox.worldBound.xMin - root.worldBound.xMin, myBox.worldBound.yMin - root.worldBound.yMin));

        //    vMyPos = v2;
        //    Debug.LogWarning("VPOS:" + v2);
        //}
    }
    //void OnGUI()
    //{
    //    EditorGUI.BeginChangeCheck();

    //    //gameObject = (GameObject)EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

    //    if (EditorGUI.EndChangeCheck())
    //    {
    //        if (gameObjectEditor != null) DestroyImmediate(gameObjectEditor);
    //    }

    //    GUIStyle bgColor = new GUIStyle();

    //    bgColor.normal.background = previewBackgroundTexture;

    //    if (gameObject != null)
    //    {
    //        if (gameObjectEditor == null)

    //            gameObjectEditor = Editor.CreateEditor(gameObject);
    //        gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(502, 337), bgColor);
    //    }
    //}
    private void OnDisable()
    {
        MOTARA10DemoPreviewWindow.CloseWindow();
    }
    private void OnDestroy()
    {
        MOTARA10DemoPreviewWindow.CloseWindow();
    }
    private void OnEnable()
    {
        myWindow = this;
        root = rootVisualElement;

        //StyleSheet stl = Resources.Load<StyleSheet>("MotarPlugin_Style");

        //root.styleSheets.Add(stl);

        //var quickToolVisualTree = Resources.Load<VisualTreeAsset>("MOTARPrefabsPane");
        var quickToolVisualTree = Resources.Load<VisualTreeAsset>("MOTARModelHubPreviewPane");
        quickToolVisualTree.CloneTree(root);

        gameObject = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/A-10/A10preview.prefab");
        actualPlane = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/A-10/A10placement.prefab");


        var myButtons = root.Query<Button>().ToList();

        myButtons.ForEach(SetupButton);


    }

    private void SetupButton(Button obj)
    {
        obj.clickable.clicked += () => LaunchAction(obj);
    }

    private void LaunchAction(Button obj)
    {
        switch(obj.name)
        {
            case "PreviewButton":
                if (!PreviewModel)
                {
                    
                    PreviewModel = true;
                    obj.text = "Hide Model";
                    MOTARA10DemoPreviewWindow.ShowA10Animation();
                }
                else
                {
                    PreviewModel = false;
                    obj.text = "Preview Model";
                    MOTARA10DemoPreviewWindow.CloseWindow();
                }
                break;

            case "DownloadButton":
                GameObject go = GameObject.Instantiate(actualPlane);
                go.transform.position = Vector3.zero;
                break;

        }
    }
}
