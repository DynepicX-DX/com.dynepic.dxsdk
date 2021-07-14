using System;
using System.Threading.Tasks;
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



public class MOTARSetupWindow : EditorWindow
{

    private static DXConfiguration dxConfiguration;
    public static bool WasActive = false;
    public static bool IsActive = false;
    public static EditorWindow myWindow;
    public static GUIContent focusedTitle;
    public static GUIContent unfocusedTitle;
    public static GUIContent currentTitle;
    public static MOTARSetupWindow instance;
    public static VisualElement root;

    public static DateTime StartedShowingClientID;
    public static DateTime StartedShowingClientSecret;
    private static TextField clientID;
    private static TextField clientSecret;
    private static string appId;
    public static DXAppLessonData thisAppsTest;

    private string userId;
    // [MenuItem("MOTAR Prefabs/Open _%#T")]

    public static void TestWindow()
    {
        // Opens the window, otherwise focuses it if it�s already open.
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
        instance = this;
        if (myWindow == null)
            myWindow = GetWindow<MOTARSetupWindow>();

        thisAppsTest = CreateInstance<DXAppLessonData>();
        
        //if (DXConfiguration == null)
        //    DXConfiguration = Resources.Load("DX Configuration") as DXConfiguration;
        //DXConfiguration dxConfig = Resources.Load("DX Configuration") as DXConfiguration;

        //Reference to the root of the window.
        root = rootVisualElement;
        //StyleSheet stl = Resources.Load<StyleSheet>("MotarPlugin_Style");

        //root.styleSheets.Add(stl);

        //var quickToolVisualTree = Resources.Load<VisualTreeAsset>("MOTARPrefabsPane");
        var quickToolVisualTree = Resources.Load<VisualTreeAsset>("MOTARSetupPane");
        quickToolVisualTree.CloneTree(root);

        //// Queries all the buttons (via type) in our root and passes them
        //// in the SetupButton method.
        var toolButtons = root.Query<Button>();
        var textFields = root.Query<TextField>();
        var enumFields = root.Query<EnumField>();
       // var dropDownFields = root.Query<DropdownField>().ToList();
        textFields.ForEach(SetupTextField);
        toolButtons.ForEach(SetupButton);
        enumFields.ForEach(SetupEnum);


        
        
        
        
    }

    private void SetupSecretButton(Button secretButton)
    {
        
    }
    public void PopulateSandboxUserList()
    {
        List<string> DropDownSandboxUserChoices = new List<string>();
        
        if (DXCommunicationLayerEditor.endUserList != null && DXCommunicationLayerEditor.endUserList.Count > 0)
        {
            foreach (var x in DXCommunicationLayerEditor.endUserList)
            {
                string SandboxUserName = (string)x["firstName"] + " " + (string)x["lastName"];

                DropDownSandboxUserChoices.Add(SandboxUserName);
            }
            if (DropDownSandboxUserChoices.Count > 0)
            {
                //var myBoxList = root.Query<Box>().ToList();
                var myBox = root.Query<Box>().ToList().Find(x => x.name == "SandboxUserSelection");
                var xy = root.Query<VisualElement>().ToList().Find(z => z.name == "USERNAMES");
                if (xy != null)
                    myBox.Remove(xy);
                DropdownField dtf = new DropdownField(DropDownSandboxUserChoices, 0);
                
                myBox.Add((VisualElement)dtf);
                // dtf.BindProperty()
                dtf.name = "USERNAMES";

                dtf.style.top = 33;
                dtf.style.width = 170;
                dtf.style.left = 95;

                dtf.RegisterCallback<ChangeEvent<string>>(SandboxUserSelectionChangeListener);
                
                
                var myUserHandle = root.Query<Label>().ToList().Find(y => y.name == "SANDBOXUSERHANDLE");
                myUserHandle.text = (string)DXCommunicationLayerEditor.endUserList[0]["handle"];


                ////APPCLIENTID
                //var myClientIdField = root.Query<TextField>().ToList().Find(y => y.name == "APPCLIENTID");
                //myClientIdField.value = (string)DXCommunicationLayerEditor.appList[0]["keys"][0]["clientId"];
                ////APPCLIENTSECRET
                //var myClientSecretField = root.Query<TextField>().ToList().Find(y => y.name == "APPCLIENTSECRET");
                //myClientSecretField.value = (string)DXCommunicationLayerEditor.appList[0]["keys"][0]["clientSecret"];

                var myUserImage = root.Query<Image>().ToList().Find(y => y.name == "SANDBOXUSERIMAGE");
                userId = (string)DXCommunicationLayerEditor.endUserList[0]["userId"];
                if (string.IsNullOrEmpty(userId))
                    Debug.LogWarning("why?");
                if (DXCommunicationLayerEditor.SandboxUserProfileImages.ContainsKey(userId))
                    myUserImage.image = DXCommunicationLayerEditor.SandboxUserProfileImages[userId];
                else
                    myUserImage.image = null;
               
            }
            //APPCLIENTID
        }
    }

    private void SandboxUserSelectionChangeListener(ChangeEvent<string> evt)
    {
        foreach (var x in DXCommunicationLayerEditor.endUserList)
        {
            string SandboxUserName = (string)x["firstName"] + " " + (string)x["lastName"];
            if (SandboxUserName == evt.newValue)
            {
                var myUserHandle = root.Query<Label>().ToList().Find(y => y.name == "SANDBOXUSERHANDLE");
                myUserHandle.text = (string)x["handle"];

              

                var myUserImage = root.Query<Image>().ToList().Find(y => y.name == "SANDBOXUSERIMAGE");
                userId = (string)x["userId"];
                if (string.IsNullOrEmpty(userId))
                    Debug.LogWarning("why?");
                if (DXCommunicationLayerEditor.SandboxUserProfileImages.ContainsKey(userId))
                    myUserImage.image = DXCommunicationLayerEditor.SandboxUserProfileImages[userId];
                else
                    myUserImage.image = null;
                //userId = (string)x["userId"];
                Debug.LogWarning("Userid changed to:" + userId);
            }
        }
    }
    public void PopulateClassList(bool bSuccess)
    {
        if (bSuccess)
        {
            List<string> DropDownClassChoices = new List<string>();

            if (DXCommunicationLayerEditor.thisAppClasses != null && DXCommunicationLayerEditor.thisAppClasses.docs != null && DXCommunicationLayerEditor.thisAppClasses.docs.Count > 0)
            {
                foreach (var x in DXCommunicationLayerEditor.thisAppClasses.docs)
                {
                    DXClass dxClass = x;

                    DropDownClassChoices.Add(x.name);
                }
                //< !--< ui:Label name = "CLASSNAME" style = "position: absolute; top: 75px; left: 811px;" /> !-->
                if (DropDownClassChoices.Count > 0)
                {
                    var myBox = root.Query<Box>().ToList().Find(x => x.name == "CLASSINFOBOX");
                    var xy = root.Query<DropdownField>().ToList().Find(z => z.name == "CLASSNAMES");
                    if (xy != null)
                        myBox.Remove(xy);
                    DropdownField dtf = new DropdownField(DropDownClassChoices, 0);
                    myBox.Add(dtf);
                    // dtf.BindProperty()
                    dtf.name = "CLASSNAMES";
                    dtf.style.position = Position.Absolute;
                    dtf.style.top = 38;
                    dtf.style.width = 200;
                    dtf.style.left = 116;

                    dtf.RegisterCallback<ChangeEvent<string>>(ClassSelectionDropDownListener);

                    var labels = root.Query<Label>().ToList();
                    var textFields = root.Query<TextField>().ToList();

                    var GroupID = textFields.Find(x => x.name == "GROUPID");
                    var JoinCode = textFields.Find(x => x.name == "JOINCODE");


                    GroupID.value = DXCommunicationLayerEditor.thisAppClasses.docs[0].groupId;
                    JoinCode.value = DXCommunicationLayerEditor.thisAppClasses.docs[0].joinCode;

                    var warningLabel = labels.Find(x => x.name == "WarningLabel");
                    if (DXCommunicationLayerEditor.thisAppClasses.docs[0].course != null)
                    {
                        DXClass dxClassforCourse = DXCommunicationLayerEditor.thisAppClasses.docs[0];
                        thisAppsTest.dxClass = dxClassforCourse;
                        warningLabel.visible = false;
                        labels.Find(x => x.name == "COURSENAME").text = dxClassforCourse.course.name;
                        labels.Find(x => x.name == "COURSEDESCRIPTION").tooltip = dxClassforCourse.course.description;
                        string checkLength = dxClassforCourse.course.description.Substring(0, Math.Min(35, dxClassforCourse.course.description.Length));
                        if (checkLength.Length == 35)
                            checkLength += "...";
                        labels.Find(x => x.name == "COURSEDESCRIPTION").text = checkLength;
                        textFields.Find(x => x.name == "COURSEID").value = dxClassforCourse.course.courseId;
                        EditorCoroutineUtility.StartCoroutine(DXCommunicationLayerEditor.MOTARDeveloperLessonInfoFromCourseFromSandboxUser(dxClassforCourse, PopulateLessonList), this);
                    }


                }
            }
        }
        else
        {
            // try to refresh token

        }
    }

    private void ClassSelectionDropDownListener(ChangeEvent<string> evt)
    {
        DXClass dxClass = DXCommunicationLayerEditor.thisAppClasses.docs.Find(x => x.name == evt.newValue);

        var labels = root.Query<Label>().ToList();
        var textFields = root.Query<TextField>().ToList();

        var GroupID = textFields.Find(x => x.name == "GROUPID");
        var JoinCode = textFields.Find(x => x.name == "JOINCODE");

        labels.Find(x => x.name == "COURSENAME").text = "";
        labels.Find(x => x.name == "COURSEDESCRIPTION").text = "";
        textFields.Find(x => x.name == "COURSEID").value = "";

        GroupID.value = dxClass.groupId;
        JoinCode.value = dxClass.joinCode;

        var warningLabel = labels.Find(x => x.name == "WarningLabel");
        warningLabel.visible = dxClass.course == null;
        if(dxClass.course != null)
        {
            thisAppsTest.dxClass = dxClass;
            labels.Find(x => x.name == "COURSENAME").text = dxClass.course.name;
            labels.Find(x => x.name == "COURSEDESCRIPTION").text = dxClass.course.description;
            textFields.Find(x => x.name == "COURSEID").value = dxClass.course.courseId;
            EditorCoroutineUtility.StartCoroutine(DXCommunicationLayerEditor.MOTARDeveloperLessonInfoFromCourseFromSandboxUser(dxClass, PopulateLessonList), this);
        }
        
    }
    private void LessonSelectionDropdownListener(ChangeEvent<string> evt)
    {
        DXLesson dXLesson = DXCommunicationLayerEditor.thisAppLessons.docs.Find(x => x.name == evt.newValue);

        var labels = root.Query<Label>().ToList();
        var textFields = root.Query<TextField>().ToList();

        var LessonDescription = textFields.Find(x => x.name == "LESSONDESCRIPTION");
        var LessonID = textFields.Find(x => x.name == "LESSONID");


        LessonDescription.value = dXLesson.description;
        LessonID.value = dXLesson.lessonId;

        thisAppsTest.dxLesson = dXLesson;

    }

    public void PopulateLessonList(GetAllLessonsResponse response)
    {
        List<string> DropDownLessonList = new List<string>();

        if (response != null && response.docs != null && response.docs.Count > 0)
        {
            foreach (var x in response.docs)
            {
                DXLesson dxLesson = x;
                if(x.isAssessment)
                    DropDownLessonList.Add(x.name);
            }
            //< !--< ui:Label name = "CLASSNAME" style = "position: absolute; top: 75px; left: 811px;" /> !-->
            if (DropDownLessonList.Count > 0)
            {
                var myBox = root.Query<Box>().ToList().Find(x => x.name == "LESSONINFOBOX");
                var xy = root.Query<DropdownField>().ToList().Find(z => z.name == "LESSONNAMES");
                if (xy != null)
                    myBox.Remove(xy);
                DropdownField dtf = new DropdownField(DropDownLessonList, 0);
                myBox.Add(dtf);
                // dtf.BindProperty()
                dtf.name = "LESSONNAMES";
                dtf.style.position = Position.Absolute;
                dtf.style.top = 38;
                dtf.style.width = 200;
                dtf.style.left = 105;

                dtf.RegisterCallback<ChangeEvent<string>>(LessonSelectionDropdownListener);

                var labels = root.Query<Label>().ToList();
                var textFields = root.Query<TextField>().ToList();

                var LessonDescription = textFields.Find(x => x.name == "LESSONDESCRIPTION");
                var LessonID = textFields.Find(x => x.name == "LESSONID");



                foreach (DXLesson dxl in response.docs)
                    if (dxl.isAssessment)
                    {

                        LessonDescription.value = dxl.description;
                        LessonID.value = dxl.lessonId;


                        EditorCoroutineUtility.StartCoroutine(DXCommunicationLayerEditor.MOTARDeveloperQuestionsFromLessonFromSandboxUser(dxl, (myQuestions) =>
                        {

                            /* [SerializeField]
                public DXClass dxClass;

                [SerializeField]
                public DXLesson dxLesson;
                [SerializeField]
                public List<DXAssessmentQuestion> dxQuestionsList;*/





                            PopulateQuestionsListFromSandboxUser(myQuestions);

                        }), this);
                        //var warningLabel = labels.Find(x => x.name == "WarningLAbel");
                        //if (DXCommunicationLayerEditor.thisAppClasses.docs[0].course != null)
                        //{
                        //    warningLabel.visible = false;
                        //}

                        //DXCourse dxCourse = DXCommunicationLayerEditor.thisAppClasses.docs.Find(x => x.course == DXCommunicationLayerEditor.thisAppClasses.docs.Find(x => x.course.courseId == response.docs[0].courseId).course).course;

                        break;
                    }
            }
        }
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

                appId = (string)DXCommunicationLayerEditor.appList[0]["id"];
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

                appId = (string)x["id"];


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
                clientID = obj;
                break;

            case "APPCLIENTSECRET":
                obj.value = dxConfiguration.clientSecret;
                clientSecret = obj;
                break;
        }
    }

    public void OnDestroy()
    {
        
    }
    private void SetupButton(Button button)
    {
        // Reference to the VisualElement inside the button that serves
        // as the button�s icon.
        var buttonIcon = button.Q(className: "motar-tab");

      
       
        // Instantiates our primitive object on a left click.
        button.clickable.clicked += () => ButtonActionAsync(button.name);

        // Sets a basic tooltip to the button itself.
        button.tooltip = button.parent.name;
    }
    private async Task ButtonActionAsync(string primitiveTypeName)
    {
        //    var pt = (PrimitiveType)Enum.Parse
        //                 (typeof(PrimitiveType), primitiveTypeName, true);
        //    var go = ObjectFactory.CreatePrimitive(pt);
        root = rootVisualElement;
        var textFields = root.Query<TextField>().ToList();
        GameObject go = null;
        
        switch (primitiveTypeName)
        {
            case "IMPERSONATE":
                Debug.Log("Sending login request at " + System.DateTime.Now);
                
                //string handle = textFields.Find(x => x.name== "USERID" ).text;
                //string password = textFields.Find(x => x.name == "PASSWORD").text;
                //DXTrainingLogin.AuthenticateMotarTrainingUser(handle,password);
                EditorCoroutineUtility.StartCoroutine(DXCommunicationLayerEditor.MOTARDeveloperSandboxUserImpersonation(userId, clientID.value, appId,PopulateClassList),this);
                //EditorCoroutineUtility.StartCoroutine(DXCommunicationLayerEditor.MOTARDeveloperAuthenticationAndSetupFromMOTARDeveloperID(handle, password, MOTARAuthenticationCompletion), this);

                break;

            case "USERINFO":
                //go = (GameObject)Instantiate(Resources.Load("Prefabs/Motar_User_Prefab"));
                
                break;

            case "GENERATE":
                //DXAppLessonData appData = DXCommunicationLayerEditor.thisApp;
                dxConfiguration.clientID = clientID.value;
                dxConfiguration.clientSecret = clientSecret.value;
                dxConfiguration.environment = DXEnvironment.Sandbox;
                dxConfiguration.Name = root.Query<DropdownField>("APPNAMES").ToList()[0].value;
                dxConfiguration.Description = root.Query<Label>("APPDESCRIPTION").ToList()[0].text;
                AssetDatabase.CreateAsset(dxConfiguration, "Packages/com.dynepic.dxsdk/Editor/Motar/Resources/DX Configuration.asset");
                AssetDatabase.CreateAsset(thisAppsTest, "Packages/com.dynepic.dxsdk/Editor/Motar/Resources/DXAppData.asset");

                EditorCoroutineUtility.StartCoroutine(DXCommunicationLayerEditor.MOTARDeveloperClearImpersonatedStudentClassData(userId),this);
                break;

            case "SHOWCLIENTID":
                if (clientID.isPasswordField)
                {
                    var taskClientID = Task.Run(async () =>
                  {
                      clientID.isPasswordField = false;
                      await Task.Delay(10000);
                      clientID.isPasswordField = true;

                  });
                }

               
                break;

            case "SHOWCLIENTSECRET":
                if (clientSecret.isPasswordField)
                {
                    var taskClientSecret = Task.Run(async () =>
                    {
                        clientSecret.isPasswordField = false;
                        await Task.Delay(10000);
                        clientSecret.isPasswordField = true;

                    });
                }
                break;

                
        }
        //go.transform.position = Vector3.zero;
    }

    //private IEnumerator TemporarilyShowMaskedField(TextField clientID)
    //{
    //    clientID.isPasswordField = false;

    //    yield return new WaitForSeconds(10);

    //    clientID.isPasswordField = true;

    //}

    private void MOTARAuthenticationCompletion(bool bSuccess)
    {
        if(bSuccess)
        {
            
            var labels = root.Query<Label>().ToList();
            var textFields = root.Query<TextField>().ToList();
            var ClassName = labels.Find(x => x.name == "CLASSNAME");
            var GroupID = textFields.Find(x => x.name == "GROUPID");
            var JoinCode = textFields.Find(x => x.name == "JOINCODE");

            ClassName.text = DXCommunicationLayerEditor.thisApp.dxClass.name;
            GroupID.value = DXCommunicationLayerEditor.thisApp.dxClass.groupId;
            JoinCode.value = DXCommunicationLayerEditor.thisApp.dxClass.joinCode;

            var CourseName = labels.Find(x => x.name == "COURSENAME");
            var CourseDescription = labels.Find(x => x.name == "COURSEDESCRIPTION");
            var CourseID = textFields.Find(x => x.name == "COURSEID");
            CourseName.text = DXCommunicationLayerEditor.thisApp.dxClass.course.name;
            CourseDescription.text = DXCommunicationLayerEditor.thisApp.dxClass.course.description;
            CourseID.value = DXCommunicationLayerEditor.thisApp.dxClass.course.courseId;

            var LessonName = labels.Find(x => x.name == "LESSONNAME");
            LessonName.text = DXCommunicationLayerEditor.thisApp.dxLesson.name;

            var LessonDescription = textFields.Find(x => x.name == "LESSONDESCRIPTION");
            LessonDescription.value = DXCommunicationLayerEditor.thisApp.dxLesson.description;

            var LessonID = textFields.Find(x => x.name == "LESSONID");
            LessonID.value = DXCommunicationLayerEditor.thisApp.dxLesson.lessonId;

            
            PopulateQuestionsList();
        }
    }
    private void OldMOTARAuthenticationCompletion(bool bSuccess)
    {
        if (bSuccess)
        {

            var labels = root.Query<Label>().ToList();
            var textFields = root.Query<TextField>().ToList();
            var ClassName = labels.Find(x => x.name == "CLASSNAME");
            var GroupID = textFields.Find(x => x.name == "GROUPID");
            var JoinCode = textFields.Find(x => x.name == "JOINCODE");

            ClassName.text = DXCommunicationLayerEditor.thisApp.dxClass.name;
            GroupID.value = DXCommunicationLayerEditor.thisApp.dxClass.groupId;
            JoinCode.value = DXCommunicationLayerEditor.thisApp.dxClass.joinCode;

            var CourseName = labels.Find(x => x.name == "COURSENAME");
            var CourseDescription = labels.Find(x => x.name == "COURSEDESCRIPTION");
            var CourseID = textFields.Find(x => x.name == "COURSEID");
            CourseName.text = DXCommunicationLayerEditor.thisApp.dxClass.course.name;
            CourseDescription.text = DXCommunicationLayerEditor.thisApp.dxClass.course.description;
            CourseID.value = DXCommunicationLayerEditor.thisApp.dxClass.course.courseId;

            var LessonName = labels.Find(x => x.name == "LESSONNAME");
            LessonName.text = DXCommunicationLayerEditor.thisApp.dxLesson.name;

            var LessonDescription = textFields.Find(x => x.name == "LESSONDESCRIPTION");
            LessonDescription.value = DXCommunicationLayerEditor.thisApp.dxLesson.description;

            var LessonID = textFields.Find(x => x.name == "LESSONID");
            LessonID.value = DXCommunicationLayerEditor.thisApp.dxLesson.lessonId;


            PopulateQuestionsList();
        }
    }
    public static void PopulateQuestionsListFromSandboxUser(List<DXAssessmentQuestion> dxQuestionsList)
    {
        int itemCount = dxQuestionsList.Count;
        var items = new List<string>(itemCount);
        for (int i = 1; i <= itemCount; i++)
            items.Add(dxQuestionsList[i-1].question);

        // The "makeItem" function will be called as needed
        // when the ListView needs more items to render
        Func<VisualElement> makeItem = () => new Label();

        // As the user scrolls through the list, the ListView object
        // will recycle elements created by the "makeItem"
        // and invoke the "bindItem" callback to associate
        // the element with the matching data item (specified as an index in the list)
        Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = items[i];

        // Provide the list view with an explict height for every row
        // so it can calculate how many items to actually display
        const int itemHeight = 16;
        var listView = root.Query<ListView>().ToList().Find(x => x.name == "QUESTIONS");
        listView.itemHeight = itemHeight;
        listView.bindItem = bindItem;
        listView.makeItem = makeItem;
        listView.itemsSource = items;
        //var listView = new ListView(items, itemHeight, makeItem, bindItem);

        listView.selectionType = SelectionType.Single;

        listView.onItemsChosen += obj => Debug.Log(obj);
        listView.onSelectionChange += objects => Debug.Log(objects);

        listView.style.flexGrow = 1.0f;

        //listView.Refresh();
    }

    public static void PopulateQuestionsList()
    {
        int itemCount = DXCommunicationLayerEditor.thisApp.dxQuestionsList.Count;
        var items = new List<string>(itemCount);
        for (int i = 1; i <= itemCount; i++)
            items.Add(DXCommunicationLayerEditor.thisApp.dxQuestionsList[i - 1].question);

        // The "makeItem" function will be called as needed
        // when the ListView needs more items to render
        Func<VisualElement> makeItem = () => new Label();

        // As the user scrolls through the list, the ListView object
        // will recycle elements created by the "makeItem"
        // and invoke the "bindItem" callback to associate
        // the element with the matching data item (specified as an index in the list)
        Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = items[i];

        // Provide the list view with an explict height for every row
        // so it can calculate how many items to actually display
        const int itemHeight = 16;
        var listView = root.Query<ListView>().ToList().Find(x => x.name == "QUESTIONS");
        listView.itemHeight = itemHeight;
        listView.bindItem = bindItem;
        listView.makeItem = makeItem;
        listView.itemsSource = items;
        //var listView = new ListView(items, itemHeight, makeItem, bindItem);

        listView.selectionType = SelectionType.Single;

        listView.onItemsChosen += obj => Debug.Log(obj);
        listView.onSelectionChange += objects => Debug.Log(objects);

        listView.style.flexGrow = 1.0f;

        //listView.Refresh();
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
