using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MOTARPositionToSampleCamera : MonoBehaviour
{
    private void Awake()
    {
        if(Camera.main != null)
        {
            Vector3 vCamPosition = Camera.main.transform.position;
            transform.position = vCamPosition -= new Vector3(0, 0, -2.5f);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //string[] args = System.Environment.GetCommandLineArgs();
        

        //for (int i = 0; i < args.Length; i++)
        //{
        //    Debug.LogError("ARG " + i + ": " + args[i]);
            
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
