using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;



//using DX.Profile;
//using DXDataTypes;

using UnityEngine.Events;

using DXCommunications;

public class MOTARApplicationLoginHandler : MonoBehaviour
{
    [Serializable]
    public class DXLoginCompleteEvent : UnityEvent<Exception, DXProfile>
    {
    }

    public static MOTARApplicationLoginHandler instance;

    
    public int MOTARMinUseridLength = 2;
    public int MOTARMinPasswordLength = 4;// 10;
    public TMP_InputField UserID;
    //public TMP_Text Password;
    public TMP_InputField Password;

    [SerializeField]
    public DXLoginCompleteEvent loginCompleteEvent;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
      


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleLogonAttempt()
    {
        if (UserID.text.Length < MOTARMinUseridLength)
            Debug.LogError("MOTAR user id must be at least 2 characters...");
        else if(Password.text.Length < MOTARMinPasswordLength)
            Debug.LogError("MOTAR passwords must be at least 2 characters...");
        else
        {
            Debug.Log("Calling MOTAR Login API...");
            string handle = UserID.text;
            string password = Password.text;

            DXCommunicationLayer.DXLoginRequest(handle, password, LoginComplete);
            
        }
    }
    
    public void LoginComplete( DXProfile userProfile)
    {
        if (userProfile== null)
        {
            Debug.Log("Error logging in.");
            
        }
        else
        {
            if (MOTARStateMachineHandler.instance.profile == null)
                MOTARStateMachineHandler.instance.profile = userProfile;

            MOTARStateMachineHandler.instance.LoginSucceeded();
            
            //DXImageClient.Instance.GetImage(profile.profilePic, (error, data) =>
            //{
            //    if (error != null)
            //    {
            //        Debug.LogError("Error requesting image." + error.Message);
            //    }
            //    else
            //    {
            //        Debug.Log("Got image successfully.");

            //        this.userImageLoader.LoadImage(data);
            //        this.userImage.texture = this.userImageLoader;
            //    }
            //});
            //userName.text = "Welcome " + profile.firstName + " " + profile.lastName;
            // MOTARStateMachineHandler.instance.profile = userProfile;
            PersistentDataManager.Instance.Save(userProfile, "userProfile");
            //SceneManager.LoadSceneAsync("Quiz");
            //SceneManager.LoadSceneAsync("USAF_Gameplay");
        }
    }
}
