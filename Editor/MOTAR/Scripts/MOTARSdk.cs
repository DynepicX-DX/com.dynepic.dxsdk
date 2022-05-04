using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Animations;
using UnityEditor.Experimental;
using System.Reflection;

public class MOTARSdk : EditorWindow
{
    public static Texture motarLabel;
    public static Texture motarGreyLabel;

    public static int MOTARSDKWindowWidth = 850;

    public string TestValue;

    public static MOTARSdk instance;

    public static void ShowAuthenticatedWindows()
    {
        Texture tx = (Texture)Resources.Load<Texture>("motar_editor_label");
        motarLabel = tx;

        Texture txFaded = (Texture)Resources.Load<Texture>("motar_editor_label");

        var windowAuth = GetWindow<MOTARSetupWindow>();
        windowAuth.titleContent = new GUIContent("Setup", txFaded, "MOTAR Setup");
        windowAuth.minSize = new Vector2(MOTARSDKWindowWidth, 300);

        Type type = typeof(MOTARSetupWindow);

        var window = GetWindow<MOTARPrefabsWindow>(new Type[] { type });
        //var window = GetWindow<MOTARPrefabsWindow>();
        window.titleContent = new GUIContent("Prefabs", txFaded, "MOTAR Prefabs");
        window.minSize = new Vector2(MOTARSDKWindowWidth, 300);

        //GUIStyle gstyle = new GUIStyle(GUI)
        //  gstyle.font.material.color = Color.grey;

        type = typeof(MOTARPrefabsWindow);
        //var window2 = GetWindow<MOTARAPIsWindow>(new Type[] { type });
        ////var window2 = GetWindow<MOTARAPIsWindow>();
        //window2.titleContent = new GUIContent("APIs", txFaded, "MOTAR APIs");
        //window2.minSize = new Vector2(MOTARSDKWindowWidth, 300);

        //type = typeof(MOTARAPIsWindow);
        //var window3 = GetWindow<MOTARDiscoverWindow>();
        var window3 = GetWindow<MOTARDiscoverWindow>(new Type[] { type });

        window3.titleContent = new GUIContent("Discover", txFaded, "MOTAR Discover");
        window3.minSize = new Vector2(MOTARSDKWindowWidth, 300);

        type = typeof(MOTARDiscoverWindow);
        


        MOTARSetupWindow.instance.PopulateAppList();
        MOTARSetupWindow.instance.PopulateSandboxUserList();
        windowAuth.Focus();
    }
    [MenuItem("MOTAR SDK/Open _%#T")]
    public static void ShowSDKWindows()
    {


       
        Texture tx = (Texture)Resources.Load<Texture>("motar_editor_label");
        motarLabel = tx;

        Texture txFaded = (Texture)Resources.Load<Texture>("motar_editor_label");
        motarGreyLabel = txFaded;
        // Opens the window, otherwise focuses it if it’s already open.

        var AuthenticationWindow = GetWindow<MOTARLogonWindow>();
        AuthenticationWindow.maxSize = new Vector2(612, 412);
        AuthenticationWindow.minSize = AuthenticationWindow.maxSize;
        AuthenticationWindow.titleContent = new GUIContent("Authentication", txFaded, "Welcome to MOTAR");
        Vector2 vPosition = new Vector2(Screen.width / 2 + 306, Screen.height / 2 - 206);
        Vector2 vSize = new Vector2(612, 412);
        AuthenticationWindow.position = new Rect(vPosition, vSize);
        AuthenticationWindow.ShowPopup();

        

       


    }

    public static void ShowOrginizationWindow() {
        Texture tx = (Texture)Resources.Load<Texture>("motar_editor_label");
        motarLabel = tx;

        Texture txFaded = (Texture)Resources.Load<Texture>("motar_editor_label");

        var windowOrgSelect = GetWindow<MOTAROrgWindow>();
        windowOrgSelect.titleContent = new GUIContent("Org Selection", txFaded, "MOTAR Setup");

        Type type = typeof(MOTARSetupWindow);

        windowOrgSelect.Focus();
    }

    public static System.Type[] GetAllEditorWindowTypes()
    {
        var result = new System.Collections.Generic.List<System.Type>();
        System.Reflection.Assembly[] AS = System.AppDomain.CurrentDomain.GetAssemblies();
        System.Type editorWindow = typeof(EditorWindow);
        foreach (var A in AS)
        {
            System.Type[] types = A.GetTypes();
            foreach (var T in types)
            {
                if (T.IsSubclassOf(editorWindow))
                    result.Add(T);
            }
        }
        return result.ToArray();
    }
    private void OnEnable()
    {
        // Reference to the root of the window.
        //var root = rootVisualElement;
        //StyleSheet stl = Resources.Load<StyleSheet>("MotarPlugin_Style");

        //root.styleSheets.Add(stl);

        //var quickToolVisualTree = Resources.Load<VisualTreeAsset>("MotarPlugin_Main");
        //quickToolVisualTree.CloneTree(root);

        //// Queries all the buttons (via type) in our root and passes them
        //// in the SetupButton method.
        //var toolButtons = root.Query<Button>();
        //toolButtons.ForEach(SetupButton);

    }
    private void SetupButton(Button button)
    {
        // Reference to the VisualElement inside the button that serves
        // as the button’s icon.
        var buttonIcon = button.Q(className: "motar-tab");

        // Icon’s path in our project.
        var iconPath = "MOTARTabs/" + button.parent.name + "-tab";

        // Loads the actual asset from the above path.
        var iconAsset = Resources.Load<Texture2D>(iconPath);

        // Applies the above asset as a background image for the icon.
        buttonIcon.style.backgroundImage = iconAsset;


        // Instantiates our primitive object on a left click.
        button.clickable.clicked += () => CreateObject(button.parent.name);

        // Sets a basic tooltip to the button itself.
        button.tooltip = button.parent.name;
    }
    private void CreateObject(string primitiveTypeName)
    {
        var pt = (PrimitiveType)Enum.Parse
                     (typeof(PrimitiveType), primitiveTypeName, true);
        var go = ObjectFactory.CreatePrimitive(pt);
        go.transform.position = Vector3.zero;
    }
}