using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MOTARTestQuestionsCanvasHandler : MonoBehaviour
{
    public static MOTARTestQuestionsCanvasHandler instance;
    //private static MOTARTestQuestionsCanvasHandler m_instance;

    //public static MOTARTestQuestionsCanvasHandler Instance
    //{
    //    get
    //    {
    //        return null;


    //    }
    //}

    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CheckTestCompletion()
    {
        MOTARStateMachineHandler.instance.MOTARRuntimeStateMachine.SetTrigger("TestCompleted");
    }

    
}
