
using UnityEngine.Serialization;

using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEditor;
//using DXDataTypes;
public class DXSetupSettings : ScriptableObject
{
    [SerializeField]
    public List<string> AppNames;

    [SerializeField]
    public string AppDescription;

    [SerializeField]
    public string clientID;

    [SerializeField]
    public string clientSecret;


}
