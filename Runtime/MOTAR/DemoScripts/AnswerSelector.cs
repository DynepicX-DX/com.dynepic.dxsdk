using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerSelector : MonoBehaviour
{
    public Toggle me;

    public List<Toggle> them;

    // Start is called before the first frame update
    void Start()
    {
        me.onValueChanged.AddListener(delegate
        {
            ToggleValueChanged(me);
        });
        foreach (Toggle tg in them)
            me.onValueChanged.AddListener(delegate {
                ToggleValueChanged(tg);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Output the new state of the Toggle into Text
    void ToggleValueChanged(Toggle change)
    {
        //Debug.LogError("toggle:" + change + " is on:" + change.isOn);

            if (change == me)
                if (change.isOn)
                    foreach (Toggle tg in them)
                        tg.isOn = false;
      //  if (change.isOn && change != me)
      //      me.isOn = false;
    }
}
