using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using DXCommunications;

public class MOTARStateMachineHandler : MonoBehaviour
{
    
    public static MOTARStateMachineHandler instance;


    public List<GameObject> MOTARUIObjects;

    public Animator MOTARRuntimeStateMachine;
    public DXProfile profile;
     
    private void Awake()
    {
        instance = this;
        MOTARRuntimeStateMachine = GetComponent<Animator>();
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
            GameObject go = (GameObject)Instantiate(MOTARUnityObjectSettingsHandler.instance.MOTARTestQuestionsCanvas);
        //}
        //else
          //  MOTARTestQuestionsCanvasHandler.instance.GetComponent<Canvas>().enabled = true;
        //Debug.LogWarning("it'll crash!");
    }
            
}
