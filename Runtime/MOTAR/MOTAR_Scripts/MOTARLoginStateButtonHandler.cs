using DXCommunications;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

using UnityEngine;

public class MOTARLoginStateButtonHandler : MonoBehaviour
{
    public static MOTARLoginStateButtonHandler instance;
    public bool LoggedIn = false;

    public GameObject ShowLoginPrompt;
    public GameObject LogOut;

    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
           

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateButtonStateOnAnimatorChange();
    }
    void UpdateButtonStateOnAnimatorChange()
    {
        if (MOTARStateMachineHandler.instance != null)
            if (MOTARStateMachineHandler.instance.MOTARRuntimeStateMachine != null)
            {
                AnimatorStateInfo ainfo = MOTARStateMachineHandler.instance.MOTARRuntimeStateMachine.GetCurrentAnimatorStateInfo(0);
                if (ainfo.IsTag("loggedOut"))
                {
                    if (LogOut.activeSelf)
                        ResetLoginPrompt();
                }
                else if (ShowLoginPrompt.activeSelf)
                    ShowLogoutPrompt();
            }
    }
    public void LoginStateButtonPressed(string triggerName)
    {
        try
        {
            
            MOTARStateMachineHandler.instance.profile = PersistentDataManager.Instance.Load<DXProfile>("userProfile");
                       
            if (MOTARStateMachineHandler.instance.profile is object && DXCommunicationLayer.AccessToken != "")
            {
                
                MOTARStateMachineHandler.instance.MOTARRuntimeStateMachine.SetBool("LoggedIn", true);
                MOTARStateMachineHandler.instance.MOTARRuntimeStateMachine.SetTrigger("LoginSuccess");
            }
            else
            {
                MOTARStateMachineHandler.instance.MOTARRuntimeStateMachine.SetTrigger(triggerName);

            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        
        ShowLoginPrompt.SetActive(false);
        
    }

    public void LogoutStateButtonPressed(string triggerName)
    {
        MOTARStateMachineHandler.instance.profile = null;
        MOTARStateMachineHandler.instance.MOTARRuntimeStateMachine.SetTrigger(triggerName);
        
        ResetLoginPrompt();
    }

    public void ShowLogoutPrompt()
    {
        ShowLoginPrompt.SetActive(false);
        //LogOut.SetActive(true);
    }
    public void ResetLoginPrompt()
    {
        //LogOut.SetActive(false);
        ShowLoginPrompt.SetActive(true);
    }
}
