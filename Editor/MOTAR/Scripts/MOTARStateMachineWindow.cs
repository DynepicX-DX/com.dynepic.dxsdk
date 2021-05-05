using System;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;
public class MOTARStateMachineWindow : EditorWindow
{
    

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
        //// Reference to the root of the window.
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
