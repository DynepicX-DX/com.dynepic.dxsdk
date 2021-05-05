using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOTARLoginCanvasHandler : MonoBehaviour
{
    public static MOTARLoginCanvasHandler instance;

    private void Awake()
    {
        if (instance == null)
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

    private void OnDestroy()
    {
        instance = null;
    }
}
