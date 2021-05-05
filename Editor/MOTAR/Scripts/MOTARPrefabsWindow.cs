using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity;
using DXCommunications;
using UnityEngine.EventSystems;


public class MOTARPrefabsWindow : EditorWindow
{
    public static bool WasActive = false;
    public static bool IsActive = false;
    public static EditorWindow myWindow;
    public static GUIContent focusedTitle;
    public static GUIContent unfocusedTitle;
    public static GUIContent currentTitle;

    public static DXPrefabSettings dps;
    // [MenuItem("MOTAR Prefabs/Open _%#T")]

    public static void TestWindow()
    {
        // Opens the window, otherwise focuses it if it’s already open.
        var window = GetWindow<MOTARPrefabsWindow>();
       
        Texture tx = (Texture)Resources.Load<Texture>("MOTARTabs/MOTARPrefabs-tab");

        GUIContent guic = new GUIContent(tx);
      
        // Adds a title to the window.
        window.titleContent = new GUIContent(guic);
       // window.titleContent = new GUIContent("MOTAR Prefabs");
        // Sets a minimum size to the window.
        window.minSize = new Vector2(250, 400);




    }

   
   
    private void OnEnable()
    {
        //if (myWindow == null)
        //    myWindow = GetWindow<MOTARPrefabsWindow>();

        myWindow = this;
        //Reference to the root of the window.
        var root = rootVisualElement;
        //StyleSheet stl = Resources.Load<StyleSheet>("MotarPlugin_Style");

        //root.styleSheets.Add(stl);

        var quickToolVisualTree = Resources.Load<VisualTreeAsset>("MOTARPrefabsPane");
        //var quickToolVisualTree = Resources.Load<VisualTreeAsset>("MOTARAuthenticationPane");
        quickToolVisualTree.CloneTree(root);

        //// Queries all the buttons (via type) in our root and passes them
        //// in the SetupButton method.
        var toolButtons = root.Query<Button>();
        
        toolButtons.ForEach(SetupButton);

        var objectFields = root.Query<ObjectField>();
        

        dps = Resources.Load<DXPrefabSettings>("DXPrefabSettings");
        objectFields.ForEach(SetupObjectField);

    }

    private void SetupObjectField(ObjectField obj)
    {

       if(dps != null)
        {
            switch(obj.name)
            {
                case "LOGINSTATEBUTTON_PREFAB":
                    obj.value = dps.LoginStateButton;
                    break;

                case "LOGINCANVAS_PREFAB":
                    obj.value = dps.LoginUICanvas;
                    break;

                case "USERPROFILE_PREFAB":
                    obj.value = dps.UserProfileCanvas;
                    break;
                case "STARTMODULE_PREFAB":
                    obj.value = dps.StartModuleButton;
                    break;
                case "QUESTIONS_PREFAB":
                    obj.value = dps.QuestionsCanvas;
                    break;
                case "TESTRESULTS_PREFAB":
                    obj.value = dps.TestResultsCanvas;
                    break;
            }
        }    
        
       // obj.RegisterCallback<DragEnterEvent>(DragEntered);
        //obj.RegisterCallback<DragPerformEvent>(TestTest);


    }

    

    public void OnDestroy()
    {
        
    }
    private void SetupButton(Button button)
    {
        
        button.clickable.clicked += () => CreateObject(button.name);

        // Sets a basic tooltip to the button itself.
        button.tooltip = button.parent.name;
    }
    private void CreateObject(string primitiveTypeName)
    {
        //    var pt = (PrimitiveType)Enum.Parse
        //                 (typeof(PrimitiveType), primitiveTypeName, true);
        //    var go = ObjectFactory.CreatePrimitive(pt);

        GameObject go = null;
        var objectFields = rootVisualElement.Query<ObjectField>().ToList();
        switch (primitiveTypeName)
        {
            case "SAVEPREFABSETTINGS":

                DXPrefabSettings dps = new DXPrefabSettings();
                
                dps.LoginStateButton = objectFields.Find(x => x.name == "LOGINSTATEBUTTON_PREFAB").value;
                dps.LoginUICanvas = objectFields.Find(x => x.name == "LOGINCANVAS_PREFAB").value;
                dps.UserProfileCanvas = objectFields.Find(x => x.name == "USERPROFILE_PREFAB").value;
                dps.StartModuleButton = objectFields.Find(x => x.name == "STARTMODULE_PREFAB").value;
                dps.QuestionsCanvas = objectFields.Find(x => x.name == "QUESTIONS_PREFAB").value;
                dps.TestResultsCanvas = objectFields.Find(x => x.name == "TESTRESULTS_PREFAB").value;
                AssetDatabase.CreateAsset(dps, "Packages/com.dynepic.dxsdk/Editor/Motar/Resources/DXPrefabSettings.asset");
                break;

            case "GENERATEPREFABS":
                if (objectFields.Find(x => x.name == "LOGINSTATEBUTTON_PREFAB").value == null)
                {
                    go = (GameObject)Instantiate(Resources.Load("MOTAR_Prefabs/MOTAR-PrefabsNoLoginState"));
                    
                }
                else
                    go = (GameObject)Instantiate(Resources.Load("MOTAR_Prefabs/MOTAR-Prefabs"));
                MOTARUnityObjectSettingsHandler mosh = go.GetComponentInChildren<MOTARUnityObjectSettingsHandler>();
                mosh.MOTARLoginPromptLocation = objectFields.Find(x => x.name == "LOGINCANVAS_PREFAB").value;
                mosh.MOTARUserInfoCanvasHandler = objectFields.Find(x => x.name == "USERPROFILE_PREFAB").value;
                mosh.MOTARStartModuleButtonHandler = objectFields.Find(x => x.name == "STARTMODULE_PREFAB").value;
                mosh.MOTARTestResultsCanvasHandler = objectFields.Find(x => x.name == "TESTRESULTS_PREFAB").value;
                mosh.MOTARTestQuestionsCanvas = (GameObject) objectFields.Find(x => x.name == "QUESTIONS_PREFAB").value;


                break;
        }
        if(go != null)
            go.transform.position = Vector3.zero;
        if (GameObject.Find("EventSystem") == null)
        {
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

            //TMPro.TMP_PackageUtilities.ImportProjectResourcesMenu();
        }

    }
private void TestTest(DragAndDrop evt)
    {
        throw new NotImplementedException();
    }

    private static Texture2D _staticRectTexture;
    private static GUIStyle _staticRectStyle;

    // Note that this function is only meant to be called from OnGUI() functions.
    public static void GUIDrawRect(Rect position, Color color)
    {
        if (_staticRectTexture == null)
        {
            _staticRectTexture = new Texture2D(1, 1);
        }

        if (_staticRectStyle == null)
        {
            _staticRectStyle = new GUIStyle();
        }

        _staticRectTexture.SetPixel(0, 0, color);
        _staticRectTexture.Apply();

        _staticRectStyle.normal.background = _staticRectTexture;
        //GUI.BeginGroup(.
        GUI.Box(position, GUIContent.none, _staticRectStyle);


    }
}
