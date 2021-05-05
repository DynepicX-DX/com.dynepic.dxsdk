using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Reflection;

public class MOTARShowStartModuleButtonBehavior : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(MOTARUnityObjectSettingsHandler.instance.MOTARUserInfoCanvasHandler != null)
        {
            if (MOTARUserInfoCanvasHandler.instance == null)
            {

                GameObject goNew = Instantiate((GameObject)(MOTARUnityObjectSettingsHandler.instance.MOTARUserInfoCanvasHandler));
            }
            else
            {
                MOTARUserInfoCanvasHandler.instance.ShowLogoutButton();
            }
        }

        if (MOTARUnityObjectSettingsHandler.instance.MOTARStartModuleButtonHandler != null)
        {
            if (MOTARStartModuleButtonHandler.instance == null)
            {

                GameObject goNew = Instantiate((GameObject)(MOTARUnityObjectSettingsHandler.instance.MOTARStartModuleButtonHandler));
            }
            else
            {
                MOTARStartModuleButtonHandler.instance.GetComponent<Canvas>().enabled = true;
            }
        }

        //if(MOTARStartModuleButtonHandler.instance != null)
        //foreach (GameObject go in MOTARUnityObjectSettingsHandler.instance.DisplayOnSuccessfulLogin)
        {
            //todo -- check instances
          //  GameObject goNew = (GameObject)Instantiate(go);


                
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
