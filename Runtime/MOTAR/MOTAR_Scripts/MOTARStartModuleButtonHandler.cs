using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOTARStartModuleButtonHandler : MonoBehaviour
{
    public static MOTARStartModuleButtonHandler instance;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //RectTransform trect = GetComponent<RectTransform>();

        //trect.position -= new Vector3(0, 0.5f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowTest()
    {
        MOTARStateMachineHandler.instance.MOTARRuntimeStateMachine.SetTrigger("ShowTestButtonPressed");
        if (MOTARUserInfoCanvasHandler.instance != null)
            MOTARUserInfoCanvasHandler.instance.GetComponent<Canvas>().enabled = false;
        GetComponent<Canvas>().enabled = false;
    }
    
}
