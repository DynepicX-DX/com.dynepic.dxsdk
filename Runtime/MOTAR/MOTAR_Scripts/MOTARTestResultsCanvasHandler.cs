using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DXCommunications;

public class MOTARTestResultsCanvasHandler : MonoBehaviour
{
    public static MOTARTestResultsCanvasHandler instance;
    public TextMeshProUGUI TextScore;
    public static int iScore = 0;
    private void Awake()
    {
        instance = this;
        MOTARStateMachineHandler.UpdateOfflineMessagesIfAny();
    }
    // Start is called before the first frame update
    void Start()
    {
        //TextScore.text = iScore.ToString();
        GetComponent<ShowPassFail>().ShowStats();
        if(DXCommunicationLayer.DXOfflineUpdates != null && DXCommunicationLayer.DXOfflineUpdates.Count > 0)
        {
            PersistentDataManager.Instance.Save(DXCommunicationLayer.DXOfflineUpdates, "OfflineAssessment");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToMainMenu()
    {
        MOTARStateMachineHandler.instance.MOTARRuntimeStateMachine.SetTrigger("GoToMainMenu");
        
    }

}
