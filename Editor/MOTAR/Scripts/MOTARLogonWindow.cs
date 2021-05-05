using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity;
using Unity.EditorCoroutines.Editor;
using System.Collections;
using System.Text;

using System.Collections.Generic;

using UnityEngine.Networking;

using DXCommunications;



public class MOTARLogonWindow : EditorWindow
{

    private static DXConfiguration dxConfiguration;
    public static bool WasActive = false;
    public static bool IsActive = false;
    public static EditorWindow myWindow;
    public static GUIContent focusedTitle;
    public static GUIContent unfocusedTitle;
    public static GUIContent currentTitle;
    public static MOTARLogonWindow instance;
    public static VisualElement root;
    // [MenuItem("MOTAR Prefabs/Open _%#T")]

  

    public void OnGUI()
    {
    }
   
    
    private void OnEnable()
    {
        instance = this;
        if (myWindow == null)
            myWindow = GetWindow<MOTARLogonWindow>();
      
        
        //if (DXConfiguration == null)
        //    DXConfiguration = Resources.Load("DX Configuration") as DXConfiguration;
        //DXConfiguration dxConfig = Resources.Load("DX Configuration") as DXConfiguration;

        //Reference to the root of the window.
        root = rootVisualElement;
        //StyleSheet stl = Resources.Load<StyleSheet>("MotarPlugin_Style");

        //root.styleSheets.Add(stl);

        //var quickToolVisualTree = Resources.Load<VisualTreeAsset>("MOTARPrefabsPane");
        var quickToolVisualTree = Resources.Load<VisualTreeAsset>("MotarDevLogonPane");
        quickToolVisualTree.CloneTree(root);

        //// Queries all the buttons (via type) in our root and passes them
        //// in the SetupButton method.
        

        
        
        var image = root.Query<Image>().ToList().Find(x => x.name == "MOTARLOGO");
        image.image = Resources.Load<Texture2D>("MOTAR_SDK_LOGO");

        

        myWindow.maxSize = new Vector2((512+256), myWindow.position.height);
        myWindow.minSize = maxSize;
        image.style.left = 50;

        var LogonBox = root.Query<Box>().ToList().Find(x => x.name == "LOGONBOX");
        LogonBox.style.left = 50 + ((image.image.width / 2) - 165);

        var toolButtons = root.Query<Button>();
        toolButtons.ForEach(SetupButton);
    }
    public void PopulateAppList()
    {
        List<string> DropDownAppChoices = new List<string>();
        //Dictionary<string, List<string>> DropDownApps = new Dictionary<string, List<string>>();
        if (DXCommunicationLayerEditor.appList != null && DXCommunicationLayerEditor.appList.Count > 0)
        {
            foreach (var x in DXCommunicationLayerEditor.appList)
            {
                string AppName = (string)x["name"];
                string AppDescription = (string)x["description"];

                
                var theirKeys = x["keys"];
                if (theirKeys != null && theirKeys.HasValues)
                {
                    var theirKey = theirKeys[0];
                    string client_id = (string)theirKey["clientId"];
                    string clinet_secret = (string)theirKey["clientSecret"];
                    DropDownAppChoices.Add(AppName);

                }
            }
            if (DropDownAppChoices.Count > 0)
            {
                var myBox = root.Query<Box>().ToList().Find(x => x.name == "App Selection");
                var xy = root.Query<DropdownField>().ToList().Find(z => z.name == "APPNAMES");
                if (xy != null)
                    myBox.Remove(xy);
                DropdownField dtf = new DropdownField(DropDownAppChoices, 0);
                myBox.Add(dtf);
               // dtf.BindProperty()
                dtf.name = "APPNAMES";
                
                dtf.style.top = 33;
                dtf.style.width = 170;
                dtf.style.left = 95;
                
                dtf.RegisterCallback<ChangeEvent<string>>(AppSelectionDropDownListener);
                var myDescriptionField = root.Query<Label>().ToList().Find(y => y.name == "APPDESCRIPTION");
                myDescriptionField.text = (string)DXCommunicationLayerEditor.appList[0]["description"];
                //APPCLIENTID
                var myClientIdField = root.Query<TextField>().ToList().Find(y => y.name == "APPCLIENTID");
                myClientIdField.value = (string)DXCommunicationLayerEditor.appList[0]["keys"][0]["clientId"];
                //APPCLIENTSECRET
                var myClientSecretField = root.Query<TextField>().ToList().Find(y => y.name == "APPCLIENTSECRET");
                myClientSecretField.value = (string)DXCommunicationLayerEditor.appList[0]["keys"][0]["clientSecret"];

                var myAppImage = root.Query<Image>().ToList().Find(y => y.name == "APPIMAGE");
                string sName = (string)DXCommunicationLayerEditor.appList[0]["name"];
                if (string.IsNullOrEmpty(sName))
                    Debug.LogWarning("why?");
                if (DXCommunicationLayerEditor.appIcons.ContainsKey(sName))
                    myAppImage.image = DXCommunicationLayerEditor.appIcons[sName];
                else
                    myAppImage.image = null;
            }
            //APPCLIENTID
        }
    }

    private void AppSelectionDropDownListener(ChangeEvent<string> evt)
    {
        foreach (var x in DXCommunicationLayerEditor.appList)
        {
            string AppName = (string)x["name"];
            if(AppName == evt.newValue)
            {
                var myDescriptionField = root.Query<Label>().ToList().Find(y => y.name == "APPDESCRIPTION");
                myDescriptionField.text = (string)x["description"];

                //APPCLIENTID
                var myClientIdField = root.Query<TextField>().ToList().Find(y => y.name == "APPCLIENTID");
                myClientIdField.value = (string)x["keys"][0]["clientId"];
                //APPCLIENTSECRET
                var myClientSecretField = root.Query<TextField>().ToList().Find(y => y.name == "APPCLIENTSECRET");
                myClientSecretField.value = (string)x["keys"][0]["clientSecret"];

                var myAppImage = root.Query<Image>().ToList().Find(y => y.name == "APPIMAGE");
                
                if (string.IsNullOrEmpty(AppName))
                    Debug.LogWarning("why?");
                if (DXCommunicationLayerEditor.appIcons.ContainsKey(AppName))
                    myAppImage.image = DXCommunicationLayerEditor.appIcons[AppName];
                else
                    myAppImage.image = null;
            }
        }
    }

    private void SetupDropdownField(DropdownField obj)
    {
        if (dxConfiguration == null)
            dxConfiguration = ScriptableObject.CreateInstance<DXConfiguration>();

        switch (obj.name)
        {
            case "APPCLIENTID":
                obj.value = dxConfiguration.clientID;
                break;

            case "APPCLIENTSECRET":
                obj.value = dxConfiguration.clientSecret;
                break;
        }
    }

    private void SetupEnum(EnumField obj)
    {
        switch (obj.name)
        {
            case "ENVIRONMENT":
                if (dxConfiguration.environment == DXEnvironment.Production)
                {
                   
                    obj.value = DXEnvironment.Production;
                }
                else
                    obj.value = DXEnvironment.Sandbox;
                break;

            
        }
    }

    private void SetupTextField(TextField obj)
    {
        if (dxConfiguration == null)
            dxConfiguration = ScriptableObject.CreateInstance<DXConfiguration>();
        
        switch(obj.name)
        {
            case "APPCLIENTID":
                obj.value =  dxConfiguration.clientID;
                break;

            case "APPCLIENTSECRET":
                obj.value = dxConfiguration.clientSecret;
                break;
        }
    }

    public void OnDestroy()
    {
        
    }
    private void SetupButton(Button button)
    {
        // Reference to the VisualElement inside the button that serves
        // as the button’s icon.
        var buttonIcon = button.Q(className: "motar-tab");

      
       
        // Instantiates our primitive object on a left click.
        button.clickable.clicked += () => ButtonAction(button.name);

        // Sets a basic tooltip to the button itself.
        button.tooltip = button.parent.name;
    }
    private void ButtonAction(string primitiveTypeName)
    {
        //    var pt = (PrimitiveType)Enum.Parse
        //                 (typeof(PrimitiveType), primitiveTypeName, true);
        //    var go = ObjectFactory.CreatePrimitive(pt);
        root = rootVisualElement;
        var textFields = root.Query<TextField>().ToList();
        GameObject go = null;
        
        switch (primitiveTypeName)
        {
            case "MOTARLOGIN":
                Debug.Log("Sending login request at " + System.DateTime.Now);
                
                string handle = textFields.Find(x => x.name== "USERID" ).text;
                string password = textFields.Find(x => x.name == "PASSWORD").text;
                //DXTrainingLogin.AuthenticateMotarTrainingUser(handle,password);
                //EditorCoroutineUtility.StartCoroutine(DXCommunicationLayerEditor.MOTARDeveloperAuthenticationAndSetupFromTestUser(handle, password,MOTARAuthenticationCompletion),this);
                EditorCoroutineUtility.StartCoroutine(DXCommunicationLayerEditor.MOTARDeveloperAuthenticationAndSetupFromMOTARDeveloperID(handle, password, MOTARDevAuthenticationCompletion), this);

                break;

            case "USERINFO":
                //go = (GameObject)Instantiate(Resources.Load("Prefabs/Motar_User_Prefab"));
                
                break;

            case "GENERATE":
                DXAppLessonData appData = DXCommunicationLayerEditor.thisApp;
                
                AssetDatabase.CreateAsset(appData, "Packages/com.dynepic.dxsdk/Editor/Motar/Resources/DXAppData.asset");
                break;
                
        }
        //go.transform.position = Vector3.zero;
    }
    private void MOTARDevAuthenticationCompletion(bool bSuccess)
    {
        if(bSuccess)
        {
            MOTARSdk.ShowAuthenticatedWindows();
            myWindow.Close();
            
        }
    }
    
   
    // Note that this function is only meant to be called from OnGUI() functions.
    

   
   
}
