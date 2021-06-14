using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DXCommunications;
public class MOTARQuestionsInfoHandler : MonoBehaviour
{
    public static MOTARQuestionsInfoHandler instance;

    public List<DXAssessmentQuestion> questions;

    private void Awake()
    {
        instance = this;
        MOTARStateMachineHandler.UpdateOfflineMessagesIfAny();
        //  dip.

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
