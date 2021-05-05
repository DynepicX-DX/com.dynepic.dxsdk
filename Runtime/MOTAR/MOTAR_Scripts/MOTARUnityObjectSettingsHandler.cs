using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MOTARUnityObjectSettingsHandler : MonoBehaviour
{
    public static MOTARUnityObjectSettingsHandler instance;

    public bool LoginStateButtonWasClicked { get; set; }
   

    // Start is called before the first frame update
    public Object MOTARLoginPromptLocation;
    public Object MOTARUserInfoCanvasHandler;
    public Object MOTARStartModuleButtonHandler;
    public Object MOTARTestResultsCanvasHandler;

    public GameObject MOTARTestQuestionsCanvas;
    



    private void Awake()
    {
        instance = this;
     
    }
    void Start()
    {
        //if (MOTARLoginStateButtonHandler.instance != null) // there's a button on the scene
        //{
        //    MOTARLoginStateButtonHandler.instance.ShowLoginPrompt.GetComponent<Button>().onClick.AddListener(LoginStateButtonClicked);
        //}
        //Debug.LogError("Object was " + LoginPromptLocation.GetType());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   


}
