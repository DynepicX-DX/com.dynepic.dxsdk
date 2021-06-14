using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using DXCommunications;
using Newtonsoft.Json;

public class MOTARStateMachineHandler : MonoBehaviour
{
    
    public static MOTARStateMachineHandler instance;


    public List<GameObject> MOTARUIObjects;

    public Animator MOTARRuntimeStateMachine;
    public DXProfile profile;

    private bool IsInternetReachable = true;
    private bool WasInternetReachable = true;

    public Transform MOTARParent;

    public bool TestInternet;
    public bool NewInternetStatus = true;
    DXAppLessonData dXAppLessonData;

    private void Awake()
    {
        instance = this;
        MOTARParent = transform.parent;
        MOTARRuntimeStateMachine = GetComponent<Animator>();

        dXAppLessonData = Resources.Load<DXAppLessonData>("DxAppData");
    }
    // Start is called before the first frame update
    void Start()
    {
       
        if(MOTARLoginStateButtonHandler.instance != null)
        {
            MOTARRuntimeStateMachine.SetBool("ShowLoginStateButton", true);

            string[] args = System.Environment.GetCommandLineArgs();

            //Debug.LogError("arg 1 " + args[1]);
            if (args.Length > 1)
            {
                char[] separators = new char[2];
                separators[0] = '&';
                separators[1] = '=';
                string[] MOTARParameters = args[1].Split(separators);
                for (int i = 0; i < MOTARParameters.Length; i += 2)
                    if (MOTARParameters[i] == "code")
                    {
                        //Debug.LogError("CODE WAS:" + MOTARParameters[i + 1]);
                       
                        
                        StartCoroutine(DXCommunicationLayer.DXGetAuthTokenFromMOTARLaunch(MOTARParameters[i+1],LoginComplete));
                        break;
                    }
                //StartCoroutine(DXCommunicationLayer.DXGetAuthTokenFromMOTARLaunch(code));
                

            }
        }
        //MOTARRuntimeStateMachine.SetTrigger("StartProcessingUI");
    }

    // Update is called once per frame
    void Update()
    {
        IsInternetReachable = Application.internetReachability != NetworkReachability.NotReachable;
        if (TestInternet)
            if (IsInternetReachable != NewInternetStatus)
                IsInternetReachable = NewInternetStatus;
        if (IsInternetReachable != WasInternetReachable)
        {
            WasInternetReachable = IsInternetReachable;
            MOTARRuntimeStateMachine.SetBool("InternetConnectionLost", !IsInternetReachable);

            UpdateOfflineMessagesIfAny();

            string attemptedClassId = dXAppLessonData.dxClass.groupId;
            string attemptedLessonId = dXAppLessonData.dxLesson.lessonId;

            Dictionary<string, string> o = new Dictionary<string, string>();
            o["lessonId"] = attemptedLessonId;
            o["classId"] = attemptedClassId;
            //o["studentId"] = profile.userId;

            string verb = IsInternetReachable ? "Internet Reconnected" : "Internet Disconnected";

            string sBody = JsonConvert.SerializeObject(o);
            DXCommunicationLayer.AddXApiStatement(verb, profile.userId, sBody);

           
          
            //MOTARRuntimeStateMachine.SetTrigger("InternetStatusChanged");
            //IternetDownStatus.SetActive(!IsInternetReachable);

        }
    }
    public static void UpdateOfflineMessagesIfAny()
    {
        bool bActivate = Application.internetReachability == NetworkReachability.NotReachable;
        Text[] texts = instance.MOTARParent.GetComponentsInChildren<Text>(true);
        if (texts != null)
        {
            foreach(Text tx in texts)
                if(tx.name == "Offline_Text")
                    tx.gameObject.SetActive(bActivate);
        }
        
        if(!bActivate && DXCommunicationLayer.DXOfflineUpdates != null)
        {
            
            foreach(DXOfflineActivityQueueEntry daqe in DXCommunicationLayer.DXOfflineUpdates)
            {
                string api = daqe.apiEndpoint;
                string sBody = daqe.apiParametersJson;
                instance.StartCoroutine(DXCommunicationLayer.DXPostAPIRequest<DXLessonProgress>(api, sBody, null));
            }
            
        }
    }
    public void LoginComplete(DXProfile userProfile)
    {
        if (userProfile == null)
        {
            Debug.Log("Error logging in.");

        }
        else
        {
            if (MOTARStateMachineHandler.instance.profile == null || MOTARStateMachineHandler.instance.profile.userId== "")
                MOTARStateMachineHandler.instance.profile = userProfile;
            PersistentDataManager.Instance.Save(userProfile, "userProfile");
            //Debug.LogError("User Profile:" + userProfile);
            MOTARStateMachineHandler.instance.LoginSucceeded();

         
        }
    }
    public void LoginSucceeded()
    {
        if (MOTARLoginStateButtonHandler.instance != null)
            MOTARLoginStateButtonHandler.instance.ShowLogoutPrompt();
        MOTARRuntimeStateMachine.SetTrigger("LoginSuccess");
        MOTARRuntimeStateMachine.SetBool("LoggedIn",true);
    }
    public static void Show<T>()
    {
        Type type = typeof(T);

        //GameObject go = (GameObject)type.GetMethod(UnityEditorInternal)
        if (MOTARTestQuestionsCanvasHandler.instance != null)
            Destroy(MOTARTestQuestionsCanvasHandler.instance);
        //{
            GameObject go = (GameObject)Instantiate(MOTARUnityObjectSettingsHandler.instance.MOTARTestQuestionsCanvas,MOTARStateMachineHandler.instance.MOTARParent);
        //}
        //else
          //  MOTARTestQuestionsCanvasHandler.instance.GetComponent<Canvas>().enabled = true;
        //Debug.LogWarning("it'll crash!");
    }
            
}
