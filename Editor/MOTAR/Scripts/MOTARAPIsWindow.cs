using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.UIElements.Experimental;

using UnityEngine.Networking;
using System.Collections;
using Unity.EditorCoroutines.Editor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Globalization;
using System.Xml;
using DXCommunications;


public class MOTARAPIsWindow : EditorWindow
{
    //public const string GitbookAPIToken = "VDNYT3Jjc2ZrZU9PcG0zWlg1MmhSRWZ6RnNuMjotTVdSQnhPa1VRZ3JiMGpJU3gyeC0tTVdSQnhPbEFPSE5wb2puSV94aw";
    //public const string GitbookSpaceID = "-LfL7IpQLlHbntS_nANV"; //API doc
    //public const string GitbookAPIBaseURL = "https://api.gitbook.com/v1/";
    //public const string MOTARGitbookSpaceID = "-LfL7IpQLlHbntS_nANV";
    //public const string MOTARGitbookAPIPageID = "-LkUR2avuiCm1dtZA5zz";
    //public static string GitbookAPIPageURL = GitbookAPIBaseURL + "spaces/" + MOTARGitbookSpaceID + "/content/v/master/id/" + MOTARGitbookAPIPageID;
    //public static string GitbookAPIPageURLBase = GitbookAPIBaseURL + "spaces/" + MOTARGitbookSpaceID + "/content/v/master/id/";

    public const string GitbookOldAPIToken = "VDNYT3Jjc2ZrZU9PcG0zWlg1MmhSRWZ6RnNuMjotTVdSQnhPa1VRZ3JiMGpJU3gyeC0tTVdSQnhPbEFPSE5wb2puSV94aw";
    public const string GitbookAPIToken = "-MWRBxOkUQgrb0jISx2x--MWRBxOlAOHNpojnI_xk";
    public const string GitbookSpaceID = "-LfL7IpQLlHbntS_nANV"; //API doc
    public const string GitbookAPIBaseURL = "https://api.gitbook.com/v1/";
    public const string MOTARGitbookSpaceID = "-LfL7IpQLlHbntS_nANV";
    public const string MOTARGitbookAPIPageID = "-LkUR2avuiCm1dtZA5zz";


    public static string GitbookAPIPageURL = GitbookAPIBaseURL + "spaces/" + MOTARGitbookSpaceID + "/content";// + "/content/url/develop/api/training/course"; ///id/" + MOTARGitbookAPIPageID;
///id/" + MOTARGitbookAPIPageID;
    public static string GitbookAPIPageURLBase = GitbookAPIBaseURL + "spaces/" + MOTARGitbookSpaceID + "/content/v/master/id/";
    private static List<string> ExcludedCategories = new List<string>();
    public static List<string> GitbookAPIPageURLExceptions;
    public static List<string> apiEntries;
    public static APIDetails details;
    public static Dictionary<string, List<string>> GitbookAPIsByCategory;

    Dictionary<string, JToken> MOTARAPIToDetailsMap = new Dictionary<string, JToken>();
    [SerializeField] TreeViewState m_GitbookTopicTreeState;
    TreeViewState m_GitbookAPIParamsTreeState;
    private static List<EditorCoroutine> crts = new List<EditorCoroutine>();
    Dictionary<string, APIDetails> MOTARAPIDetails = new Dictionary<string, APIDetails>();
    //The TreeView is not serializable, so it should be reconstructed from the tree data.
    //public GitbookTopicTree m_SimpleTreeView;
    //public static GitbookTopicTree ms_instance;
    //public GitbookAPIParamsTree m_GitbookAPIParamsView;
    //public static GitbookAPIParamsTree ms_GitbookAPIParamsViewInstance;
    public static MOTARAPIsWindow myWindow;
    public static ListView myListView;
    public static bool ChangingAPICategory = false;
    public static string highlightedAPITitle = "";
    // [MenuItem("MOTAR Prefabs/Open _%#T")]

    public static VisualElement root;
    public static JObject ourGitbookDocs;

    public static string CurrentAPIMethod;
    public static string CurrentAPIEndpoint;
    public static Dictionary<string, object> CurrentQueryParametersDictionary;
    public static string CurrentAPIDocs = "";
    public static Dictionary<string, object> CurrentBodyParametersDictionary;
    public static JToken CurrentSampleResponse;

    [MenuItem("MOTAR API/Open _%#T")]
    public static void OpenMOTARAPIsWindow()
    {
        myWindow = GetWindow<MOTARAPIsWindow>();
    }
    //public class GitbookTopicTree : TreeView
    //{
    //    Dictionary<string, string> GitbookPageKeys = new Dictionary<string, string>();
        
    //    public GitbookTopicTree(TreeViewState treeViewState)
    //        : base(treeViewState)
    //    {
    //        Reload();
    //    }
    //    protected override void SelectionChanged(IList<int> selectedIds)
    //    {
    //        ChangingAPICategory = true;
    //        //Debug.LogWarning("Found:" + selectedIds[selectedIds.Count - 1]);
    //        TreeViewItem tItem = ms_instance.FindItem(selectedIds[selectedIds.Count - 1],rootItem);
    //        // Debug.LogWarning("selecteD:" + GitbookPageKeys[tItem.displayName]);
    //        if (myWindow != null)
    //        {
    //            myWindow.GetTreeItemDataFromGitbook(GitbookPageKeys[tItem.displayName]);
    //            highlightedAPITitle = tItem.displayName;
    //        }
    //        ms_GitbookAPIParamsViewInstance.Reload();
            
    //    }
    //    protected override TreeViewItem BuildRoot()
    //    {
    //        // BuildRoot is called every time Reload is called to ensure that TreeViewItems 
    //        // are created from data. Here we create a fixed set of items. In a real world example,
    //        // a data model should be passed into the TreeView and the items created from the model.

    //        // This section illustrates that IDs should be unique. The root item is required to 
    //        // have a depth of -1, and the rest of the items increment from that.

    //        ExcludedCategories.Add("hub");
    //        ExcludedCategories.Add("plugins");
    //        ExcludedCategories.Add("sdk");
    //        var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
    //        var allItems = new List<TreeViewItem>();

    //        int iDepthLevel = 0;
    //        int id = 1;
    //        foreach(JToken jt in ourGitbookDocs["pages"])
    //        {
    //            JArray deeperPages = (JArray) jt["pages"];
    //            string nodeText = (string)jt["title"];

    //            if (!ExcludedCategories.Contains(nodeText.ToLower()))
    //            {
    //                GitbookPageKeys[nodeText] = (string)jt["uid"];
    //                allItems.Add(new TreeViewItem(id++, iDepthLevel, nodeText));
    //            }
    //            if (deeperPages == null || deeperPages.Count == 0)
    //            {
                    
    //            }
    //            else
    //            {
    //                foreach(JToken jt2 in deeperPages)
    //                {
    //                    string innernodeText = (string)jt2["title"];
    //                    if (!ExcludedCategories.Contains(innernodeText.ToLower()))
    //                    {
    //                        allItems.Add(new TreeViewItem(id++, iDepthLevel + 1, innernodeText));
    //                        GitbookPageKeys[innernodeText] = (string)jt2["uid"];
    //                    }
    //                    }
    //            }
    //        }

      
    //        // Utility method that initializes the TreeViewItem.children and .parent for all items.
    //        SetupParentsAndChildrenFromDepths(root, allItems);

            

    //        return root;
    //    }
    //}
    //public class GitbookAPIParamsTree : TreeView
    //{
    //    Dictionary<string, string> GitbookPageKeys = new Dictionary<string, string>();

    //    public GitbookAPIParamsTree(TreeViewState treeViewState)
    //        : base(treeViewState)
    //    {
    //        Reload();
    //    }
    //    protected override void SelectionChanged(IList<int> selectedIds)
    //    {
    //        //Debug.LogWarning("Found:" + selectedIds[selectedIds.Count - 1]);
    //        TreeViewItem tItem = ms_GitbookAPIParamsViewInstance.FindItem(selectedIds[selectedIds.Count - 1], rootItem);
           
    //        //Debug.LogWarning("selecteD:" + GitbookPageKeys[tItem.displayName]);
    //        //if (myWindow != null)
    //        //    myWindow.GetTreeItemDataFromGitbook(GitbookPageKeys[tItem.displayName]);
    //    }
      
    //    protected override void ExpandedStateChanged()
    //    {
             
    //        ms_GitbookAPIParamsViewInstance.GetFirstAndLastVisibleRows(out int firstVisible, out int lastVisible);

    //        //Debug.LogWarning("first:" + firstVisible + " last:" + lastVisible);
    //        bool showCodeGeneration = lastVisible <= 8;

    //        var CodeGenBox = root.Query("CODEGENERATION").ToList()[0];

    //        if (showCodeGeneration)
    //            CodeGenBox.style.opacity = 1;
    //        else
    //            CodeGenBox.style.opacity = 0;
    //        base.ExpandedStateChanged();
    //    }
    //    protected override TreeViewItem BuildRoot()
    //    {
    //        // BuildRoot is called every time Reload is called to ensure that TreeViewItems 
    //        // are created from data. Here we create a fixed set of items. In a real world example,
    //        // a data model should be passed into the TreeView and the items created from the model.

    //        // This section illustrates that IDs should be unique. The root item is required to 
    //        // have a depth of -1, and the rest of the items increment from that.

            
    //        var treeRoot = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
    //        var allItems = new List<TreeViewItem>();
            
    //        int iDepthLevel = 0;
    //        int id = 1;

    //        // git current api selected

    //        List<ListView> llview = root.Query<ListView>().ToList();
    //        object iListItem = llview[0].selectedItem;
    //        string MOTARApiEndpointKey = (string)iListItem;


    //        int RequestResponseLevel = 0;
    //        int HeaderParamLevel = 1;
    //        int HeaderParamDetail = 2;

    //        int ResponseJavascriptLevel = 1;
    //        int ResponseHeaderLevel = 1;
    //        int ResponseDesriptionLevel = 2;

    //        if (ChangingAPICategory)
    //        {
    //            allItems.Clear();
    //            Repaint();
    //            if (CurrentBodyParametersDictionary != null)
    //                CurrentBodyParametersDictionary.Clear();
    //            if (CurrentQueryParametersDictionary != null)
    //                CurrentQueryParametersDictionary.Clear();
    //            CurrentSampleResponse = null;
    //        }
    //        else
    //            try
    //            {
    //                JToken thisEntry = myWindow.MOTARAPIToDetailsMap[MOTARApiEndpointKey];
    //                if (CurrentQueryParametersDictionary == null)
    //                    CurrentQueryParametersDictionary = new Dictionary<string, object>();
    //                else
    //                    CurrentQueryParametersDictionary.Clear();

    //                if (CurrentBodyParametersDictionary == null)
    //                    CurrentBodyParametersDictionary = new Dictionary<string, object>();
    //                else
    //                    CurrentBodyParametersDictionary.Clear();
    //                foreach (JToken jtNested in thisEntry)
    //                {
    //                    string gitbookSubTokenType = (string)jtNested["type"];
    //                    switch (gitbookSubTokenType)
    //                    {
    //                        case "api-method-summary":
    //                            string mySummary = (string)jtNested["nodes"][0]["ranges"][0]["text"];
    //                            //root.Query<Label>().ToList().Find(x => x.name == "SUMMARY").text = mySummary;
    //                            break;

    //                        case "api-method-description":
    //                            string myDescription = (string)jtNested["nodes"][0]["ranges"][0]["text"];
    //                            //root.Query<Label>().ToList().Find(x => x.name == "DESCRIPTION").text = myDescription;
    //                            break;

    //                        case "api-method-spec":
    //                            foreach (JToken jtMethodSpecToken in jtNested["nodes"])
    //                            {
    //                                string subTokeType = (string)jtMethodSpecToken["type"];
    //                                if (subTokeType == "api-method-request")
    //                                {
    //                                    allItems.Add(new TreeViewItem(id++, RequestResponseLevel, "Request"));
    //                                    foreach (JToken jtParamType in jtMethodSpecToken["nodes"])
    //                                    {
    //                                        string paramTokeType = (string)jtParamType["type"];
    //                                        if (paramTokeType == "api-method-body-parameters")
    //                                        {
    //                                            allItems.Add(new TreeViewItem(id++, HeaderParamLevel, "Body Parameters"));
    //                                            allItems.Add(new TreeViewItem(id++, HeaderParamDetail, "Parameter\tType"));
    //                                            foreach (JToken jtParamDetail in jtParamType["nodes"])
    //                                            {
    //                                                string paramDetailTokeType = (string)jtParamDetail["type"];
    //                                                if (paramDetailTokeType == "api-method-parameter")
    //                                                {

    //                                                    string paramName = (string)jtParamDetail["data"]["name"];
    //                                                    string paramType = (string)jtParamDetail["data"]["type"];
    //                                                    string paramRequired = (string)jtParamDetail["data"]["required"];

    //                                                    if (paramName.Length < 10)
    //                                                        paramName += "\t";


    //                                                    string lineToAdd = paramName + "\t" + paramType;
    //                                                    if (paramRequired.ToLower() == "true")
    //                                                        lineToAdd += "\t\tRequired";
    //                                                    allItems.Add(new TreeViewItem(id++, HeaderParamDetail, lineToAdd));
    //                                                    string detail = (string)jtParamDetail["nodes"][0]["ranges"][0]["text"];
    //                                                    allItems.Add(new TreeViewItem(id++, HeaderParamDetail + 1, detail));
    //                                                }

    //                                            }
    //                                        }
    //                                        else if (paramTokeType == "api-method-headers")
    //                                        {
    //                                            allItems.Add(new TreeViewItem(id++, HeaderParamLevel, "Header Parameters"));
    //                                            allItems.Add(new TreeViewItem(id++, HeaderParamDetail, "Parameter\tType"));
    //                                            foreach (JToken jtParamDetail in jtParamType["nodes"])
    //                                            {
    //                                                string paramDetailTokeType = (string)jtParamDetail["type"];
    //                                                if (paramDetailTokeType == "api-method-parameter")
    //                                                {
    //                                                    string paramName = (string)jtParamDetail["data"]["name"];
    //                                                    string originalBodyParamName = paramName;
    //                                                    if (paramName.Length < 10)
    //                                                        paramName += "\t";
    //                                                    string paramType = (string)jtParamDetail["data"]["type"];
    //                                                    string paramRequired = (string)jtParamDetail["data"]["required"];

    //                                                    CurrentBodyParametersDictionary[originalBodyParamName] = paramType + "|||" + paramRequired;

    //                                                    string lineToAdd = paramName + "\t" + paramType;
    //                                                    if (paramRequired.ToLower() == "true")
    //                                                        lineToAdd += "\t\tRequired";
    //                                                    allItems.Add(new TreeViewItem(id++, HeaderParamDetail, lineToAdd));
    //                                                    string detail = (string)jtParamDetail["nodes"][0]["ranges"][0]["text"];
    //                                                    allItems.Add(new TreeViewItem(id++, HeaderParamDetail + 1, detail));
    //                                                }

    //                                            }
    //                                        }
    //                                        else if(paramTokeType == "api-method-query-parameters")
    //                                        {
                                              

    //                                            allItems.Add(new TreeViewItem(id++, HeaderParamLevel, "Query Parameters"));
    //                                            allItems.Add(new TreeViewItem(id++, HeaderParamDetail, "Parameter\tType"));
    //                                            foreach (JToken jtParamDetail in jtParamType["nodes"])
    //                                            {
    //                                                string paramDetailTokeType = (string)jtParamDetail["type"];
    //                                                if (paramDetailTokeType == "api-method-parameter")
    //                                                {
    //                                                    string paramName = (string)jtParamDetail["data"]["name"];
    //                                                    string originalParamName = paramName;
                                                        
    //                                                    if (paramName.Length < 10)
    //                                                        paramName += "\t";
    //                                                    string paramType = (string)jtParamDetail["data"]["type"];
    //                                                    string paramRequired = (string)jtParamDetail["data"]["required"];
    //                                                    CurrentQueryParametersDictionary[originalParamName] = paramType + "|||" + paramRequired;


    //                                                    string lineToAdd = paramName + "\t" + paramType;
    //                                                    if (paramRequired.ToLower() == "true")
    //                                                        lineToAdd += "\t\tRequired";
    //                                                    allItems.Add(new TreeViewItem(id++, HeaderParamDetail, lineToAdd));
    //                                                    string detail = (string)jtParamDetail["nodes"][0]["ranges"][0]["text"];
    //                                                    allItems.Add(new TreeViewItem(id++, HeaderParamDetail + 1, detail));
    //                                                }

    //                                            }
    //                                        }
    //                                    }
    //                                }
    //                                else if (subTokeType == "api-method-response")
    //                                {
    //                                    int iJSLevelAdder = 0;
    //                                    allItems.Add(new TreeViewItem(id++, RequestResponseLevel, "Sample Response"));
    //                                    string whatItReturns = "";
    //                                    foreach (JToken jtResponseType in jtMethodSpecToken["nodes"])
    //                                    {
    //                                        string responseType = (string)jtResponseType["type"];
    //                                        if (responseType == "api-method-response-example")
    //                                        {
    //                                            //string httpCode = jtResponseType["data"]
    //                                            CurrentSampleResponse = jtResponseType["nodes"];
    //                                            foreach (JToken jResponsePart in jtResponseType["nodes"])
    //                                            {
    //                                                string responsePartType = (string)jResponsePart["type"];
    //                                                if (responsePartType == "api-method-response-example-description")
    //                                                {
    //                                                    whatItReturns = (string)jResponsePart["nodes"][0]["ranges"][0]["text"];
    //                                                }
    //                                                else if (responsePartType == "code")
    //                                                {
    //                                                    foreach (JToken jtJavascriptTextToken in jResponsePart["nodes"])
    //                                                    {
    //                                                        string jsLine = (string)jtJavascriptTextToken["nodes"][0]["ranges"][0]["text"];
    //                                                        //if (jsLine.StartsWith("["))
    //                                                        //    iJSLevelAdder=1;



    //                                                        allItems.Add(new TreeViewItem(id++, ResponseJavascriptLevel + iJSLevelAdder, jsLine));
    //                                                        if (jsLine.Trim().StartsWith("{") || jsLine.Trim().StartsWith("["))
    //                                                            iJSLevelAdder++;
    //                                                        else if (jsLine.Trim().EndsWith("}") || jsLine.Trim().EndsWith("]"))
    //                                                            iJSLevelAdder--;
    //                                                        //else
    //                                                        //   iJSLevelAdder++;
    //                                                        if (iJSLevelAdder < 0)
    //                                                            iJSLevelAdder = 0;
    //                                                    }
    //                                                }

    //                                            }
    //                                        }

    //                                    }
    //                                    if (whatItReturns != "")
    //                                    {
    //                                        allItems.Add(new TreeViewItem(id++, ResponseJavascriptLevel, ""));
    //                                        allItems.Add(new TreeViewItem(id++, ResponseJavascriptLevel, whatItReturns));
    //                                    }
    //                                }


    //                            }
    //                            break;
    //                    }
    //                }
    //            }
    //            catch
    //            {

    //            }




    //        // Utility method that initializes the TreeViewItem.children and .parent for all items.
    //        SetupParentsAndChildrenFromDepths(treeRoot, allItems);

            
    //        // Return root of the tree
            

    //        // Return root of the tree
    //        return treeRoot;
    //    }
    //}
    //void OnGUI()
    //{
    //    if(m_SimpleTreeView != null)
    //        m_SimpleTreeView.OnGUI(new Rect(10, 10, 200, position.height));
    //    if (m_GitbookAPIParamsView != null)
    //        m_GitbookAPIParamsView.OnGUI(new Rect(675, 10, 400, position.height));
    //}
    private void OnEnable()
    {
        myWindow = this;
        root = rootVisualElement;
      
        //StyleSheet stl = Resources.Load<StyleSheet>("MotarPlugin_Style");

        //root.styleSheets.Add(stl);

        //var quickToolVisualTree = Resources.Load<VisualTreeAsset>("MOTARPrefabsPane");
        var quickToolVisualTree = Resources.Load<VisualTreeAsset>("MOTARAPIsPane");
        quickToolVisualTree.CloneTree(root);

        var llView = root.Query<UnityEngine.UIElements.ListView>().ToList();
        myListView = llView[0];
        llView[0].selectionType = SelectionType.Single;
        llView[0].onSelectionChange += MOTARAPIsWindow_onSelectionChange;

        Label loadingText = root.Q<Label>("LOADINGLABEL");
        crts.Add(EditorCoroutineUtility.StartCoroutine(HandleLoadingText(loadingText, 80f), this));

        var toolButtons = root.Query<Button>();

        toolButtons.ForEach(SetupButton);

        GitbookAPIPageURLExceptions = new List<string>();
        GitbookAPIPageURLExceptions.Add("getting-started");
        GitbookAPIPageURLExceptions.Add("error-codes");
        GitbookAPIPageURLExceptions.Add("authentication-tokens");

        GetTreeDataFromGitbook();
    }
    public class APIDetails
    {
        public string categoryURL;
        public string summary;
        public string description;
        public Dictionary<string, APIParameter> parameters;

        public APIDetails(string _categoryURL, string _summary, string _description, Dictionary<string, APIParameter> _parameters)
        {
            categoryURL = _categoryURL;
            summary = _summary;
            description = _description;
            parameters = _parameters;
        }
    }
    public class APIParameter
    {
        public string name;
        public string type;
        public bool required;
        public string description;

        public APIParameter(string _name, string _type, bool _required, string _description)
        {
            name = _name;
            type = _type;
            required = _required;
            description = _description;
        }
    }

    private void SetupButton(Button obj)
    {
        obj.clickable.clicked += () => LaunchAction(obj.name);

       
    }

    private void OnDestroy()
    {
        foreach (EditorCoroutine crt in crts)
        {
            if (crt != null) EditorCoroutineUtility.StopCoroutine(crt);
        }
    }

    private void LaunchAction(string name)
    {
        switch (name)
        {
            case "LAUNCHWEB":
                string url = "https://docs.motar.io/";
                if (highlightedAPITitle != "")
                    url += "studio/api?q=" + highlightedAPITitle;
                Application.OpenURL(url);
                
                break;

            //case "GENERATECODE":
            //    m_GitbookAPIParamsView.CollapseAll();

            //    break;

            case "CLIPBOARD":
                var SampleCode = root.Query<TextField>().ToList().Find(x => x.name == "SampleCode");
                //SampleCode.SelectAll();

                EditorGUIUtility.systemCopyBuffer = SampleCode.value;
                break;
            default:
                break;
        }
    }
    private void PopulateSampleCode(string endPoint)
    {
        string[] apiParts = endPoint.Split(new char[] { ' ', '\t' });

        CurrentAPIMethod = apiParts[0];
        CurrentAPIEndpoint = apiParts[apiParts.Length - 1].Replace("https://api.motar.io/", "");

        var SampleCode = root.Query<TextField>().ToList().Find(x => x.name == "SampleCode");

      
        string sampleCodeTemplateLine1 = "// Don't forget to use the DXCommunications namespace\n\n//Note HEADER parameters are used for authentication and are set automatically by the API\n\n";
        string sampleCodeTemplateLine4 = "//declare a Dictionary<string,object> to pass any necessary body parameters, or pass null\n";
        string sampleCodeTemplateLine5 = "Dictionary<string,object> body = new Dictionary<string,object>();\n";

        string sampleCodeTemplateLine2 = "//declare a Dictionary<string,object> to pass any necessary query parameters, or pass null\n";
        string sampleCodeTemplateLine3 = "Dictionary<string,object> queryParams = new Dictionary<string,object>();\n";

        

        List<string> queryParamLines = new List<string>();
        if (CurrentQueryParametersDictionary == null || CurrentQueryParametersDictionary.Count == 0)
        {
            sampleCodeTemplateLine2 = "";
            sampleCodeTemplateLine3 = "\n";
        }
        else
        {
            foreach (string key in CurrentQueryParametersDictionary.Keys)
            {
                string lineDeclare = "//queryParams[VARNAME] = //VALUE;\n";
                lineDeclare = lineDeclare.Replace("VARNAME", "\"" + key + "\"");
                string sValue = (string)CurrentQueryParametersDictionary[key];
                sValue = sValue.Replace("|||True", " note this is a required query parameter");
                sValue = sValue.Replace("|||False", " note this query paramter is optional");
                lineDeclare = lineDeclare.Replace("VALUE", sValue);
                queryParamLines.Add(lineDeclare);
            }
        }

        List<string> bodyParamLines = new List<string>();
        if (CurrentBodyParametersDictionary == null || CurrentBodyParametersDictionary.Count == 0)
        {
            sampleCodeTemplateLine4 = "";
            sampleCodeTemplateLine5 = "\n";
        }
        else
        {
            foreach (string key in CurrentBodyParametersDictionary.Keys)
            {
                string lineDeclare = "//body[VARNAME] = //VALUE;\n";
                lineDeclare = lineDeclare.Replace("VARNAME", "\"" + key + "\"");
                string sValue = (string)CurrentBodyParametersDictionary[key];
                sValue = sValue.Replace("|||True", " note this is a required body parameter");
                sValue = sValue.Replace("|||False", " note this body paramter is optional");
                lineDeclare = lineDeclare.Replace("VALUE", sValue);
                bodyParamLines.Add(lineDeclare);
            }
        }
        //public static IEnumerator DXAPIRequest(string api, string method,Action<object> completion, Dictionary<string, object> queryParameters = null, Dictionary<string, object> body = null,Texture2D txImage=null )

        string sampleCompletionFunc = "\n(outputObject) => {\nDebug.Log(\"Output type:\"+ outputObject.GetType());}\n";
                

        string sampleCoroutineLineWithParams = "StartCoroutine(DXCommunicationLayer.DXAPIRequest(\"APIENDPOINT\", \"METHOD\","+sampleCompletionFunc+",queryParams,body,null));";

        sampleCoroutineLineWithParams = sampleCoroutineLineWithParams.Replace("METHOD", CurrentAPIMethod);
        if (CurrentQueryParametersDictionary == null || CurrentQueryParametersDictionary.Count == 0)
            sampleCoroutineLineWithParams = sampleCoroutineLineWithParams.Replace("queryParams", "null");
        if (CurrentBodyParametersDictionary == null || CurrentBodyParametersDictionary.Count == 0)
            sampleCoroutineLineWithParams = sampleCoroutineLineWithParams.Replace("body", "null");
        // string sampleCodeTemplate = queryParamLines.Count == 0 ? sampleCoroutineLineWithoutParams : sampleCoroutineLineWithParams;
        string sampleCodeTemplate = sampleCoroutineLineWithParams;
        sampleCodeTemplate = sampleCodeTemplate.Replace("APIENDPOINT", CurrentAPIEndpoint);


        /*
         * foreach (JToken jResponsePart in jtResponseType["nodes"])
                                                {
                                                    string responsePartType = (string)jResponsePart["type"];
                                                    if (responsePartType == "api-method-response-example-description")
                                                    {
                                                        whatItReturns = (string)jResponsePart["nodes"][0]["ranges"][0]["text"];
                                                    }
                                                    else if (responsePartType == "code")
                                                    {
                                                        foreach (JToken jtJavascriptTextToken in jResponsePart["nodes"])
                                                        {
                                                            string jsLine = (string)jtJavascriptTextToken["nodes"][0]["ranges"][0]["text"];
                                                            //if (jsLine.StartsWith("["))
                                                            //    iJSLevelAdder=1;



                                                            allItems.Add(new TreeViewItem(id++, ResponseJavascriptLevel + iJSLevelAdder, jsLine));
                                                            if (jsLine.Trim().StartsWith("{") || jsLine.Trim().StartsWith("["))
                                                                iJSLevelAdder++;
                                                            else if (jsLine.Trim().EndsWith("}") || jsLine.Trim().EndsWith("]"))
                                                                iJSLevelAdder--;
                                                            //else
                                                            //   iJSLevelAdder++;
                                                            if (iJSLevelAdder < 0)
                                                                iJSLevelAdder = 0;
                                                        }
                                                    }

                                                }*/
        string totalText = sampleCodeTemplateLine1 + sampleCodeTemplateLine2 + sampleCodeTemplateLine3;
        foreach (string sLine in queryParamLines)
            totalText += sLine;
        totalText += sampleCodeTemplateLine4;
        totalText += sampleCodeTemplateLine5;
        foreach (string sLine in bodyParamLines)
            totalText += sLine;
        totalText += sampleCodeTemplate;
        SampleCode.value = totalText;
    }

    private void MOTARAPIsWindow_onSelectionChange(IEnumerable<object> obj)
    {
        
        root.Query<Label>().ToList().Find(x => x.name == "SUMMARY").text = "";
        root.Query<Label>().ToList().Find(x => x.name == "DESCRIPTION").text = "";

        if (obj != null)
        {
            string MOTARApiEndpointKey = (string) obj.ToList()[0];
            PopulateSampleCode(MOTARApiEndpointKey);

            //foreach()

            details = MOTARAPIDetails[MOTARApiEndpointKey];

            root.Query<Label>().ToList().Find(x => x.name == "SUMMARY").text = details.summary;
            root.Query<Label>().ToList().Find(x => x.name == "DESCRIPTION").text = details.description;

            Button openAPIDoc = root.Query<Button>("OPENAPIDOC");
            openAPIDoc.visible = true;

            openAPIDoc.clickable = new Clickable((Action) null);
            openAPIDoc.clickable.clicked += () => { System.Diagnostics.Process.Start(details.categoryURL); };
    
            PopulateParametersFromAPISelection();
        }
    }

    private static IEnumerator HandleLoadingText(Label label, float timer) {
        timer -= Time.deltaTime;

        if (timer <= 0 && label.visible == true) {
            if (label.text == "Loading...") {
                label.text = "Loading";
            } else {
                label.text += ".";
            }
            timer = 80f;
        }

        yield return HandleLoadingText(label, timer);
    }

    public void GetTreeDataFromGitbook()
    {
        //EditorCoroutineUtility.StartCoroutine(GitbookURLRequest(GitbookAPIPageURL,LoadTreeDataIntoTreeView),this);
        crts.Add(EditorCoroutineUtility.StartCoroutine(GitbookURLRequest(GitbookAPIPageURL, LoadTreeItemDataInfoPane), this));
    }
    public void GetTreeItemDataFromGitbook(string id)
    {
        string URL = GitbookAPIPageURLBase + id;
        crts.Add(EditorCoroutineUtility.StartCoroutine(GitbookURLRequest(URL,LoadTreeItemDataInfoPane), this));
    }
   
    private string safeLookup(List<string> ls, int i)
    {
        if (ls == null || ls.Count == 0)
            return "";
        if (i < 0)
            i = 0;
        if (i > ls.Count - 1)
            i = ls.Count - 1;
        return ls[i];

    }

    public void CreateTreeItemDataInfoPane()
    {
        Label loadingLabel = root.Q<Label>("LOADINGLABEL");
        loadingLabel.visible = false;

        var listView = root.Query<ListView>().ToList().Find(x => x.name == "APIs");
        //listView.Clear();

        foreach (string entry in apiEntries)
        {
            Func<VisualElement> makeItem = () => new Label();

            // As the user scrolls through the list, the ListView object
            // will recycle elements created by the "makeItem"
            // and invoke the "bindItem" callback to associate
            // the element with the matching data item (specified as an index in the list)
            Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = safeLookup(apiEntries, i);

            // Provide the list view with an explict height for every row
            // so it can calculate how many items to actually display
            const int itemHeight = 25;

            listView.itemHeight = itemHeight;
            listView.bindItem = bindItem;
            listView.makeItem = makeItem;
            listView.itemsSource = apiEntries;
            //var listView = new ListView(items, itemHeight, makeItem, bindItem);

            listView.selectionType = SelectionType.Single;

            //listView.onItemsChosen += obj => Debug.Log(obj);
            //listView.onSelectionChange += objects => Debug.Log(objects);

            listView.style.flexGrow = 1.0f;
            //if (gitbookTokenType == "api-method")
            //  Debug.LogWarning("API:" + (string)jt["data"]["host"] + (string)jt["data"]["path"]);
            // else if (gitbookTokenType == "api-method )
            //   Debug.LogWarning("description:" + (string)jt["nodes"][0]["ranges"][0]["text"]);
        }
        //Debug.Log("collection");
        ChangingAPICategory = false;
    }
    private void LoadTreeItemDataInfoPane(string json)
    {
        JObject ourGitbookPageDetail = JObject.Parse(json);

        var jNodeCollection = ourGitbookPageDetail["pages"];
        apiEntries = new List<string>();
        
        //Debug.Log(jNodeCollection);
        foreach (JToken jt in jNodeCollection)
        {
            string gitbookTokenType = (string)jt["title"];
            switch(gitbookTokenType)
            {
                case "Develop":
                    string baseLevelURL = GitbookAPIPageURL + "/url/" + jt["path"];

                    foreach (var page in jt["pages"])
                    {
                        if (page["title"].ToString() == "API")
                        {
                            //Debug.Log(page);
                            GitbookAPIsByCategory = new Dictionary<string, List<string>>();
                            DeriveAllAPIURLS(baseLevelURL, page);
                            CreateURLCategoryDropdown();
                        }
                    }

                    break;
            }
        }
        //CreateTreeItemDataInfoPane(apiEntries);
    }

    public void CreateURLCategoryDropdown()
    {
        List<string> DropDownCategories = new List<string>();
        if (GitbookAPIsByCategory.Count > 0)
        {
            foreach(string category in GitbookAPIsByCategory.Keys)
            {
                DropDownCategories.Add(category);
            }

            var myBox = root.Query<VisualElement>().ToList().Find(x => x.name == "APICATEGORIES");
            var xy = root.Query<VisualElement>().ToList().Find(z => z.name == "CATEGORIES");
            if (xy != null)
                myBox.Remove(xy);
            DropdownField dtf = new DropdownField(DropDownCategories, 0);

            myBox.Add((VisualElement) dtf);
            // dtf.BindProperty()
            dtf.name = "CATEGORIES";

            dtf.style.top = 5;
            dtf.style.width = 170;
            dtf.style.left = 0;

            dtf.RegisterCallback<ChangeEvent<string>>(URLCategoryDropdownChangeListener);

            Debug.Log("Fetching APIs for category: " + DropDownCategories[0]);

            Label loadingLabel = root.Q<Label>("LOADINGLABEL");
            loadingLabel.visible = true;

            crts.Add(EditorCoroutineUtility.StartCoroutine(URLRequestHandlerWaitForFinish(GitbookAPIsByCategory[DropDownCategories[0]], CreateTreeItemDataInfo, CreateTreeItemDataInfoPane), this));
        }
    }

    public void URLCategoryDropdownChangeListener(ChangeEvent<string> evt)
    {
        apiEntries.Clear();

        var xy = root.Query<VisualElement>().ToList().Find(z => z.name == "PARAMETERS");
        if (xy != null) xy.Clear();

        Label paramType = root.Q<Label>("ParameterType");
        Label paramRequired = root.Q<Label>("ParameterRequired");
        Label paramDescription = root.Q<Label>("ParameterDescription");

        paramType.text = "Type: ";
        paramRequired.text = "Required: ";
        paramDescription.text = "Description: ";

        root.Query<Label>().ToList().Find(x => x.name == "SUMMARY").text = "";
        root.Query<Label>().ToList().Find(x => x.name == "DESCRIPTION").text = "";
        Button openAPIDoc = root.Query<Button>("OPENAPIDOC");
        openAPIDoc.visible = false;

        var SampleCode = root.Query<TextField>().ToList().Find(x => x.name == "SampleCode");
        SampleCode.value = "\n\n\n\n\n\n\n\n\n";

        var listView = root.Query<ListView>().ToList().Find(x => x.name == "APIs");
        listView.itemsSource = apiEntries;

        List<string> urls = GitbookAPIsByCategory[evt.newValue];

        Debug.Log("Fetching APIs for category: " + evt.newValue);

        Label loadingLabel = root.Q<Label>("LOADINGLABEL");
        loadingLabel.visible = true;

        crts.Add(EditorCoroutineUtility.StartCoroutine(URLRequestHandlerWaitForFinish(urls, CreateTreeItemDataInfo, CreateTreeItemDataInfoPane), this));
    }

    public void PopulateParametersFromAPISelection()
    {
        List<string> DropDownParameters = new List<string>();
        if (details.parameters.Count > 0)
        {
            foreach (KeyValuePair<string, APIParameter> parameter in details.parameters)
            {
                DropDownParameters.Add(parameter.Key);
            }

            var myBox = root.Query<Box>().ToList().Find(x => x.name == "ParameterBox");
            var xy = root.Query<VisualElement>().ToList().Find(z => z.name == "PARAMETERS");
            if (xy != null)
                myBox.Remove(xy);
            DropdownField dtf = new DropdownField(DropDownParameters, 0);

            myBox.Add((VisualElement) dtf);
            // dtf.BindProperty()
            dtf.name = "PARAMETERS";

            dtf.style.top = -88;
            dtf.style.width = 170;
            dtf.style.left = 80;

            dtf.RegisterCallback<ChangeEvent<string>>(ParameterDropdownChangeListener);

            Label paramType = root.Q<Label>("ParameterType");
            Label paramRequired = root.Q<Label>("ParameterRequired");
            Label paramDescription = root.Q<Label>("ParameterDescription");

            paramType.text = "Type: " + details.parameters[DropDownParameters[0]].type;
            paramRequired.text = "Required: " + details.parameters[DropDownParameters[0]].required.ToString();
            paramDescription.text = "Description: " + details.parameters[DropDownParameters[0]].description;
        }
    }

    public void ParameterDropdownChangeListener(ChangeEvent<string> evt)
    {
        Label paramType = root.Q<Label>("ParameterType");
        Label paramRequired = root.Q<Label>("ParameterRequired");
        Label paramDescription = root.Q<Label>("ParameterDescription");

        paramType.text = "Type: " + details.parameters[evt.newValue].type;
        paramRequired.text = "Required: " + details.parameters[evt.newValue].required.ToString();
        paramDescription.text = "Description: " + details.parameters[evt.newValue].description;
    }

    public static IEnumerator URLRequestHandlerWaitForFinish(List<string> urls, Action<string> eachCompletion, Action overallCompletion)
    {
        foreach (string url in urls)
        {
            //Debug.Log("Calling url request with: " + url);
            CurrentAPIDocs = "https://docs.motar.io" + url.Split(new string[] { "url" }, StringSplitOptions.None)[1];
            yield return GitbookURLRequest(url, eachCompletion);
        }
        //Debug.Log("Called from handler: " + apiEntries.Count);
        overallCompletion();
    }

    public void CreateTreeItemDataInfo(string json)
    {
        JObject ourGitbookPageDetail = JObject.Parse(json);
        //Debug.Log(ourGitbookPageDetail["document"]["nodes"]);
        var jNodeCollection = ourGitbookPageDetail["document"]["nodes"];

        foreach (JToken jt in jNodeCollection)
        {
            string gitbookTokenType = (string) jt["type"];
            switch (gitbookTokenType)
            {   
                case "swagger":
                    string method = ((string) jt["data"]["method"]).ToUpper();
                    method = method.PadRight(6, ' ');

                    string apiEntry = method + "\t" + (string) jt["data"]["baseUrl"] + (string) jt["data"]["path"];
                    apiEntries.Add(apiEntry);

                    //Debug.Log(CurrentAPIDocs);
                    //Debug.Log(apiEntry);

                    string summary = (string) jt["data"]["summary"];
                    string description = GetTextFromFragment(jt["fragments"][0]);

                    Dictionary<string, APIParameter> parameters = GetParamsFromDocument(jt);

                    APIDetails d = new APIDetails(CurrentAPIDocs, summary, description, parameters);

                    MOTARAPIToDetailsMap[apiEntry] = jt["nodes"];
                    if (!MOTARAPIDetails.ContainsKey(apiEntry)) MOTARAPIDetails.Add(apiEntry, d);

                    break;
            }
        }
        //Debug.Log("API entries created: " + apiEntries.Count);
    }

    //public void LoadTreeDataIntoTreeView(string json)
    //{
    //    ourGitbookDocs = (JObject) JsonConvert.DeserializeObject(json);
    //    if (ourGitbookDocs != null)
    //    {
            
           
    //        if (m_GitbookTopicTreeState == null)
    //            m_GitbookTopicTreeState = new TreeViewState();

    //        //var treeView = root.Query<TreeView>().ToList().Find(x => x.name == "QUESTIONS");
    //        m_SimpleTreeView = new GitbookTopicTree(m_GitbookTopicTreeState);
    //        ms_instance = m_SimpleTreeView;

    //        //temp

    //        if (m_GitbookAPIParamsTreeState == null)
    //            m_GitbookAPIParamsTreeState = new TreeViewState();

    //        //var treeView = root.Query<TreeView>().ToList().Find(x => x.name == "QUESTIONS");
    //        m_GitbookAPIParamsView = new GitbookAPIParamsTree(m_GitbookAPIParamsTreeState);
    //        ms_GitbookAPIParamsViewInstance = m_GitbookAPIParamsView;
    //    }
    //}
    public static IEnumerator GitbookURLRequest(string url, Action<string> completion)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer " + GitbookAPIToken);
            //webRequest.downloadHandler = new DownloadHandlerBuffer();
            //webRequest.downloadHandler.contentType = "application/json";
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
                    //Debug.Log("received login RESPONSE at " + System.DateTime.Now);
                    //Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                    string json = webRequest.downloadHandler.text;
                    
                    //JObject test = (JObject)JsonConvert.DeserializeObject(json);

                    //foreach (JToken jt in test.Children())
                    //{
                    //    Debug.LogWarning("jt:Type" + jt.Type);
                    //}
                    //var testDetail = test["docs"];

                    //var result = JsonUtility.FromJson<GetAllClassesResponse>(json);
                    //var result = JsonConvert.DeserializeObject<DXProfile>(json);
                    //loggedOnUserProfile = result;
                    //Debug.Log("JSON:" + json);
                    completion(json);
                    break;

                default:
                    Exception error = null;
                    //error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
                    break;
            }
        }
            
   
    }

    public void DeriveAllAPIURLS(string baseLevelURL, JToken jt)
    {
        if (jt["pages"].Count() == 0)
        {
            if (!GitbookAPIPageURLExceptions.Contains(jt["path"].ToString()))
            {
                baseLevelURL += "/" + jt["path"];
                string[] parts = baseLevelURL.Split('/');
                string category = parts[parts.Length - 2].Replace('-', ' ');
                if (category == "api") category = parts[parts.Length - 1].Replace('-', ' ');
                TextInfo ti = new CultureInfo("en-US", false).TextInfo;
                category = ti.ToTitleCase(category);
                if (category.Length <= 3) category = category.ToUpper();
                
                
                if (GitbookAPIsByCategory.ContainsKey(category))
                    GitbookAPIsByCategory[category].Add(baseLevelURL);
                else
                    GitbookAPIsByCategory.Add(category, new List<string>{ baseLevelURL });
            }
            return;
        } else
        {
            baseLevelURL += "/" + jt["path"];
            foreach (JToken token in jt["pages"])
            {
                DeriveAllAPIURLS(baseLevelURL, token);
            }
        }
    }

    public string GetTextFromFragment(JToken fragment)
    {
        return (string) fragment["nodes"][0]["nodes"][0]["leaves"][0]["text"];
    }

    public Dictionary<string, APIParameter> GetParamsFromDocument(JToken doc)
    {
        Dictionary<string, APIParameter> parameters = new Dictionary<string, APIParameter>();
        //Debug.Log(doc);

        foreach (JToken fragment in doc["fragments"])
        {
            if ((string) fragment["type"] == "swagger-parameter-description")
            {
                string paramName = (string) doc["data"]["parameters"][(string) fragment["fragment"]]["name"];
                string paramType = (string) doc["data"]["parameters"][(string) fragment["fragment"]]["type"];
                bool paramRequired = doc["data"]["parameters"][(string) fragment["fragment"]]["required"] == null ? false : true;
                string paramDescription = GetTextFromFragment(fragment);

                APIParameter p = new APIParameter(paramName, paramType, paramRequired, paramDescription);
                parameters.Add((string) doc["data"]["parameters"][(string) fragment["fragment"]]["name"], p);
            }
        }

        return parameters;
    }
}
