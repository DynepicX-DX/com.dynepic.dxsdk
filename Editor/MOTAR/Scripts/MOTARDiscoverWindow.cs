using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Experimental;
using Newtonsoft.Json;

using SketchfabAPI;
using UnityEngine.Networking;
using UnityEditor.UIElements;
using DXCommunications;

public class MOTARDiscoverWindow : EditorWindow
{
    
    //public static Camera cam;
    public static Texture fakeHub;
    public static bool bCreatedPlane = false;
    public static VisualElement root;
    public static List<VisualElement> listEntries = new List<VisualElement>();
    public static List<Texture2D> testModelThumbnails;
    public static VisualTreeAsset hubEntryTemplate;
    public static MOTARDiscoverWindow myWindow;
    // [MenuItem("MOTAR Prefabs/Open _%#T")]

    [MenuItem("MOTAR Discover/Open _%#T")]
    public static void OpenDiscoveryWindow()
    {
        var window = GetWindow<MOTARDiscoverWindow>();
        myWindow = window;
    }
    IEnumerator GetSketfabSampleAssets(string search)
    {
        string url = "https://api.sketchfab.com/v3/search?q=" + search;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {


            //webRequest.SetRequestHeader("Authorization", "Bearer " + DXCommunicationLayerEditorAccessToken);
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();


            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("received login RESPONSE at " + System.DateTime.Now);
                    Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                    string json = webRequest.downloadHandler.text;

                    //JObject test = (JObject)JsonConvert.DeserializeObject(json);

                    //foreach (JToken jt in test.Children())
                    //{
                    //    Debug.LogWarning("jt:Type" + jt.Type);
                    //}
                    //var testDetail = test["docs"];

                    //var result = JsonUtility.FromJson<GetAllClassesResponse>(json);
                    var result = JsonConvert.DeserializeObject<Root>(json);
                   

                    break;

                default:
                    

                    break;
            }
        }

    }


   
    private void OnEnable()
    {
        root = rootVisualElement;
        //StyleSheet stl = Resources.Load<StyleSheet>("MotarPlugin_Style");

        //root.styleSheets.Add(stl);

        //var quickToolVisualTree = Resources.Load<VisualTreeAsset>("MOTARPrefabsPane");


        hubEntryTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.dynepic.dxsdk/Editor/Motar/Resources/MOTARHubThumbnailRow.uxml");
       

        var quickToolVisualTree = Resources.Load<VisualTreeAsset>("MOTARDiscoveryPane");
        quickToolVisualTree.CloneTree(root);

        Texture2D[] myPlanes = Resources.LoadAll<Texture2D>("DemoImages/planes");
        Texture2D[] myTools = Resources.LoadAll<Texture2D>("DemoImages/tools");
        testModelThumbnails = new List<Texture2D>(myPlanes);
        testModelThumbnails.AddRange(myTools);

        testModelThumbnails = testModelThumbnails.OrderBy(x => UnityEngine.Random.value).ToList();
      
        var myScrollView = root.Query<ScrollView>().First();
        myScrollView.StretchToParentWidth();
        //myScrollView.StretchToParentSize();
        float fPctPadding = myScrollView.contentRect.height * 0.056f;

        int iThumbnailRows = testModelThumbnails.Count / 4;
        int iOverflow = testModelThumbnails.Count % 4;

        if (iOverflow > 0)
            iThumbnailRows++;
        for(int i=0;i<iThumbnailRows;i++)
        {
            var x = hubEntryTemplate.CloneTree();
            var imagesToLoad = x.Query<UnityEngine.UIElements.Image>().ToList();
            int j = 0;
            foreach (var img in imagesToLoad)
            {
                if (i * 4 + j < testModelThumbnails.Count)
                {
                    int iIndex = i * 4 + j++;
                    img.style.backgroundImage = testModelThumbnails[iIndex];
                    

                    string[] nameParts = testModelThumbnails[iIndex].name.Split('.');
                    var VendorName = img.parent.Query<Label>("VENDORNAME").First();
                    VendorName.text = nameParts[1];

                    var AssetName = img.parent.Query<Label>("MODELNAME").First();
                    AssetName.text = nameParts[0].Replace("_", " ");
                    if (AssetName.text.StartsWith("A10"))
                        AssetName.parent.RegisterCallback<MouseDownEvent>(ProcessMouseClick);
                }
                else
                    img.parent.style.opacity = 0;

            }
            myScrollView.Add(x);
            listEntries.Add(x);
        }

        //for (int i = 0; i < 3; i++)
        //{
        //    var x = hubEntrytemplate.CloneTree();

        //    var imagesToLoad = x.Query<UnityEngine.UIElements.Image>().ToList();
        //    int j = 0;
        //    foreach (var img in imagesToLoad)
        //    {
        //        img.style.backgroundImage = testModelThumbnails[j++];

        //    }

        //    myScrollView.Add(x);
        //    listEntries.Add(x);
        //}

        var FillAreaBox = root.Query<Box>().ToList().Find(x => x.name == "FilterArea");

        var pops = Enum.GetValues(typeof(SortOrder));

        var popIt = new PopupField<SortOrder>();

        var SearchField = root.Query<TextField>("SearchField").First();
        SearchField.RegisterCallback<KeyDownEvent>(DemoSearchProcessor);
        // var sortType = new EnumField(SortOrder.Name);
        // sortType.style.width = 100;
        //   sortType.style.position = new StyleEnum<Position>(Position.Absolute)(new Position())
        //   FillAreaBox.Add(sortType);


        //EditorCoroutineUtility.StartCoroutine(GetSketfabSampleAssets("airplane"),this);
        //EditorCoroutineUtility.StartCoroutine(DXCommunicationLayerEditor.MOTARDeveloperHubListings(), this);


    }

    private void ProcessMouseClick(MouseDownEvent evt)
    {
        var windowModelPreview = GetWindow<MOTARModelDetailWindow>();
    }

    private void DemoSearchProcessor(KeyDownEvent evt)
    {
        if (evt.keyCode != KeyCode.None)
        {
            
            var myScrollView = root.Query<ScrollView>().First();
            myScrollView.Clear();

            string search = root.Query<TextField>("SearchField").First().value;
            if(evt.keyCode.ToString().Length == 1)
                search += evt.keyCode.ToString().ToLower();
            else switch(evt.keyCode)
                {
                    case KeyCode.Backspace:
                        if(!string.IsNullOrEmpty(search))
                            search = search.Substring(0, search.Length - 1);
                        break;

                    
                    default:
                        if(evt.keyCode.ToString().StartsWith("Alpha"))
                        {
                            search += evt.keyCode.ToString().Substring(evt.keyCode.ToString().Length - 1);
                        }
                        break;
                }

            List<Texture2D> texturesToUse = testModelThumbnails.FindAll(x => x.name.ToLower().Contains(search));
            int iThumbnailRows = texturesToUse.Count / 4;
            int iOverflow = texturesToUse.Count % 4;

            if (iOverflow > 0)
                iThumbnailRows++;
            for (int i = 0; i < iThumbnailRows; i++)
            {
                var x = hubEntryTemplate.CloneTree();
                var imagesToLoad = x.Query<UnityEngine.UIElements.Image>().ToList();
                int j = 0;
                foreach (var img in imagesToLoad)
                {
                    if (i * 4 + j < texturesToUse.Count)
                    {
                        int iIndex = i * 4 + j++;
                        img.style.backgroundImage = texturesToUse[iIndex];


                        string[] nameParts = texturesToUse[iIndex].name.Split('.');
                        var VendorName = img.parent.Query<Label>("VENDORNAME").First();
                        VendorName.text = nameParts[1];

                        var AssetName = img.parent.Query<Label>("MODELNAME").First();
                        AssetName.text = nameParts[0].Replace("_", " ");
                        img.parent.style.opacity = 1;
                        Debug.LogWarning("added " + AssetName.text + " and should be visible");
                        if (AssetName.text.StartsWith("A10"))
                            AssetName.parent.RegisterCallback<MouseDownEvent>(ProcessMouseClick);
                    }
                    else
                        img.parent.style.opacity = 0;

                }
                myScrollView.Add(x);
                listEntries.Add(x);
            }
        }
    }

    public void OnGUI()
    {
        ////if (fakeHub != null)
        ////    GUI.DrawTextureWithTexCoords(new Rect(10, 10, 640, 300), fakeHub, new Rect(0, 0, fakeHub.width, fakeHub.height));
        //if (fakeHub != null)
        //    GUI.DrawTexture(new Rect(10, 10, 640, 300), fakeHub, ScaleMode.StretchToFill, true, 10.0F);
        //if(Event.current != null)
        //    if(Event.current.type == EventType.MouseDown && !bCreatedPlane)
        //    {
        //        bCreatedPlane = true;
        //        Debug.LogWarning("mouse clicked");
        //        GameObject goPlane = (GameObject)Instantiate(Resources.Load("Plane"));
        //        goPlane.transform.eulerAngles = new Vector3(0, 270, 0);
        //    }
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
