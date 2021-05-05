using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class MOTARShowLoginUIBehavior : StateMachineBehaviour
{
    public static void ShowLoginPrompt()
    {
        if (MOTARUnityObjectSettingsHandler.instance == null)
        {
            Debug.LogWarning("MOTAR: Specify a Canvas or Scene as the LoginPromptLocation object to proceed...");
        }
        else if (MOTARUnityObjectSettingsHandler.instance.MOTARLoginPromptLocation == null)
        {

            Debug.LogError("MOTAR: Specify a Canvas or Scene as the LoginPromptLocation object to proceed...");
        }
        else if (MOTARUnityObjectSettingsHandler.instance.MOTARLoginPromptLocation != null)
        {
            if (MOTARUnityObjectSettingsHandler.instance.MOTARLoginPromptLocation.GetType() == typeof(GameObject)) // prefab
            {
                if (MOTARLoginCanvasHandler.instance == null)
                {
                    Debug.Log("MOTAR: Instantiating Login Form prefab...");
                    GameObject goLoginCanvas = (GameObject)Instantiate(MOTARUnityObjectSettingsHandler.instance.MOTARLoginPromptLocation);
                }
                else
                {

                    Debug.Log("MOTAR: Showing Login Form...");
                    MOTARLoginCanvasHandler.instance.gameObject.SetActive(true);
                    if (MOTARLoginCanvasHandler.instance.gameObject.GetComponent<Canvas>() != null)
                        MOTARLoginCanvasHandler.instance.gameObject.GetComponent<Canvas>().enabled = true;
                }
            }
            //else if (MOTARUnityObjectSettingsHandler.instance.MOTARLoginPromptLocation.GetType() == typeof(SceneAsset)) // scene
            //{
            //    //TODO
            //    Debug.Log("MOTAR: Launching Login scene...");
            //}
        }

    }
    void HideLoginPrompt()
    {
        if (MOTARLoginCanvasHandler.instance.gameObject.GetComponent<Canvas>() != null)
            MOTARLoginCanvasHandler.instance.gameObject.GetComponent<Canvas>().enabled = false;
    }
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
        {
            //MOTARLoginPromptDetails.instance.ShowLoginPrompt();
            ShowLoginPrompt();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        HideLoginPrompt();
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}

