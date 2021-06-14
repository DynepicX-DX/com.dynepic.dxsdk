using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//using DXDataTypes;
using DXCommunications;

public class MOTARUserInfoCanvasHandler : MonoBehaviour
{
    public static MOTARUserInfoCanvasHandler instance;

    public GameObject LoginButton;
    public GameObject LogoutButton;

    public GameObject InternetDownStatus;

    public Texture2D defaultImage;
    public Text userName;
    public RawImage userImage;
    public Texture2D userImageLoader;
    public int size = 128;

   

    private void Awake()
    {
        userImageLoader = new Texture2D(size, size);
        instance = this;
        MOTARStateMachineHandler.UpdateOfflineMessagesIfAny();


    }
    public void OnEnable()
    {
        

        PopulateUserInfo();
    }
    //public void PopulateUserInfo()
    //{
    //    if (MOTARStateMachineHandler.instance != null && MOTARStateMachineHandler.instance.profile is object && MOTARStateMachineHandler.instance.profile != null)
    //        if (MOTARStateMachineHandler.instance.profile.profilePic != "" && MOTARStateMachineHandler.instance.profile.profilePic != null)
    //        {
    //            try
    //            {
    //                DXImageClient.Instance.GetImage(MOTARStateMachineHandler.instance.profile.profilePic, (error, data) =>
    //                {
    //                    if (error != null)
    //                    {
    //                        Debug.LogError("Error requesting image." + error.Message);
    //                    }
    //                    else
    //                    {
    //                        Debug.Log("Got image successfully.");

    //                        this.userImageLoader.LoadImage(data);
    //                        this.userImage.texture = this.userImageLoader;
    //                    }
    //                });
    //            }
    //            catch
    //            {

    //            }
    //            userName.text = "Welcome " + MOTARStateMachineHandler.instance.profile.firstName + " " + MOTARStateMachineHandler.instance.profile.lastName;
    //            PersistentDataManager.Instance.Save(MOTARStateMachineHandler.instance.profile, "userProfile");
    //        }
    //}
    public void PopulateUserInfo()
    {
        //Debug.LogError("CHECKING PROFILE, instance is " + MOTARStateMachineHandler.instance.profile.profilePic);
        if (MOTARStateMachineHandler.instance != null && MOTARStateMachineHandler.instance.profile is object && MOTARStateMachineHandler.instance.profile != null)
        {
            if (MOTARStateMachineHandler.instance.profile.profilePic != "" && MOTARStateMachineHandler.instance.profile.profilePic != null)
            {

                //if (!MOTARStateMachineHandler.instance.MOTARRuntimeStateMachine.GetBool("InternetConnectionLost"))
                {

                    //InternetDownStatus.SetActive(false);
                    string command = "image/v1/static/" + MOTARStateMachineHandler.instance.profile.profilePic;
                    StartCoroutine(DXCommunicationLayer.DXBinaryAPIRequest(command, LoadImageFromData));
                }
            //    else
            //    {
            //        //InternetDownStatus.SetActive(true);
            //    }
            }
            
            userName.text = "Welcome " + MOTARStateMachineHandler.instance.profile.firstName + " " + MOTARStateMachineHandler.instance.profile.lastName;
            PersistentDataManager.Instance.Save(MOTARStateMachineHandler.instance.profile, "userProfile");

            if(MOTARLoginStateButtonHandler.instance != null)
                MOTARLoginStateButtonHandler.instance.ShowLogoutPrompt();

        }
    }
    public void LoadImageFromData(byte[] data)
    {
        this.userImageLoader.LoadImage(data);
        this.userImage.texture = this.userImageLoader;
    }
    // Start is called before the first frame update



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
                    if (LogoutButton.activeSelf)
                        ShowLoginButton();
                }
                else if (LoginButton.activeSelf)
                {
                    PopulateUserInfo();
                    ShowLogoutButton();
                }
            }
    }
    public void ShowLoginButton()
    {
        if (!GetComponent<Canvas>().enabled)
            GetComponent<Canvas>().enabled = true;
        LogoutButton.SetActive(false);
        LoginButton.SetActive(true);
        this.userImage.texture = defaultImage;



    }
    public void ShowLoginForm()
    {
        MOTARStateMachineHandler.instance.MOTARRuntimeStateMachine.SetTrigger("LoginStateButtonPressed");
    }
    public void ShowLogoutButton()
    {
        PopulateUserInfo();
        if (!GetComponent<Canvas>().enabled)
            GetComponent<Canvas>().enabled = true;
        LoginButton.SetActive(false);
        LogoutButton.SetActive(true);
    }
    public void SendBackToLoginState()
    {
        userName.text = "Welcome, Name Last Name";
        PersistentDataManager.Instance.Clear("userprofile");
        MOTARStateMachineHandler.instance.profile = null;
        MOTARStateMachineHandler.instance.MOTARRuntimeStateMachine.SetBool("LoggedIn", false);
        MOTARStateMachineHandler.instance.MOTARRuntimeStateMachine.SetTrigger("CancelTest");
        MOTARStartModuleButtonHandler.instance.GetComponent<Canvas>().enabled = false;
    }
}
