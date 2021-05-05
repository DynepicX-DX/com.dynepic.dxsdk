using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.EditorCoroutines.Editor;
using System;

public class MOTARA10DemoPreviewWindow : EditorWindow
{
    Texture2D previewBackgroundTexture;
    private GameObject gameObject;
    Editor gameObjectEditor;
    private static MOTARA10DemoPreviewWindow myWindow;
    private float showmeTime;
    private Animation m_animation = null;

    private AnimationClip m_playingAnim = null;
    private float m_time = 0.0f;

    public static void CloseWindow()
    {
        if (myWindow != null)
            myWindow.Close();
        //Destroy(myWindow);
    }
    [MenuItem("MOTAR ShowA10/Open _%#T")]

    public static void ShowA10Animation()
    {
        if (myWindow == null)
        {
            MOTARA10DemoPreviewWindow window = ScriptableObject.CreateInstance<MOTARA10DemoPreviewWindow>();
            window.position = new Rect(MOTARModelDetailWindow.vMyPos.x, MOTARModelDetailWindow.vMyPos.y, 502, 337);
            window.ShowPopup();
            myWindow = window;
        }
        else
        {
            myWindow.position = new Rect(MOTARModelDetailWindow.vMyPos.x, MOTARModelDetailWindow.vMyPos.y, 502, 337);
            myWindow.ShowPopup();
            myWindow.Focus();
           // Debug.LogError("gui.depth"+ GUI.depth);
            
        }
        
    }
    private void Update()
    {
        if(myWindow != null)
        if(!myWindow.hasFocus || MOTARModelDetailWindow.PreviewModel == false)
        {
            MOTARModelDetailWindow.PreviewModel = false;
            myWindow.Close();
            Destroy(myWindow);
            myWindow = null;
            
        }
        //if(m_playingAnim)
        //{
        //    //gameObject.SampleAnimation(m_playingAnim, m_time);
        //    gameObject.GetComponent<Animation>().clip.SampleAnimation(myWindow.gameObject, m_time);
        //    m_time += 0.01f;    //Update() is reportedly called 100 times per second

        //    if (m_time > m_playingAnim.length)
        //    {
        //        m_playingAnim = null;

        //        Debug.Log("Done playing " + m_playingAnim.name);
        //    }
        //}
    }
    void OnGUI()
    {
        
        EditorGUI.BeginChangeCheck();

        //gameObject = (GameObject)EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

        if (EditorGUI.EndChangeCheck())
        {
            if (gameObjectEditor != null) DestroyImmediate(gameObjectEditor);
        }

        GUIStyle bgColor = new GUIStyle();

        bgColor.normal.background = previewBackgroundTexture;

        if (gameObject != null)
        {
            if (gameObjectEditor == null)

                gameObjectEditor = Editor.CreateEditor(gameObject);
            
            gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(502, 337), bgColor);

            //GUILayout.BeginVertical();
            //{
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    m_animation = myWindow.gameObject.GetComponent<Animation>();
            //    var m_clips = AnimationUtility.GetAnimationClips(gameObject);
            //    foreach (AnimationClip anim in m_clips)
            //    {

            //        //if (GUILayout.Button("Play " + anim.name, GUILayout.ExpandWidth(false)))
            //        {
            //            m_animation[anim.name].normalizedTime = 0;

            //            m_playingAnim = anim;
            //            m_time = 0.0f;

            //            Debug.Log("Playing " + anim.name);
            //        }
            //    }
            //}
            //}
            //GUILayout.EndVertical();


        }

    }
    //private static IEnumerator ShowAnimationOfModel()
    //{
    //    Debug.LogError("STARTING>>>");
    //    float fTime = 1f/60f;
    //    Animation anim = myWindow.gameObject.GetComponent<Animation>();
    //    //anim["MyClip"].time = 2.0F;
    //    //anim["MyClip"].enabled = true;
    //    //anim.Sample();
    //    //anim["MyClip"].enabled = false;

    //    DateTime start = System.DateTime.Now;
    //    //for (int i = 0; i < 60; i++)
    //    {
    //        while (true)
    //        {
    //            TimeSpan tsp = DateTime.Now - start;
    //            float SampleLocation = (float)tsp.TotalSeconds;
    //            Debug.Log("Sampling:" + SampleLocation + " at " + System.DateTime.Now);
    //            anim["A10anim"].time = SampleLocation;
    //            anim["A10anim"].enabled = true;
    //            anim.Sample();
    //            anim["A10anim"].enabled = false;
    //            if (SampleLocation > 1)
    //            {
    //                //anim["A10anim"].enabled = false;
    //                break;
    //            }
    //        }
    //    }
    //    //myWindow.gameObject.GetComponent<Animation>().clip.SampleAnimation(myWindow.gameObject, 0f);
    //    yield return null;
    //}
    private void OnEnable()
    {
        gameObject = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/A-10/A10preview.prefab");
    }
    
}

