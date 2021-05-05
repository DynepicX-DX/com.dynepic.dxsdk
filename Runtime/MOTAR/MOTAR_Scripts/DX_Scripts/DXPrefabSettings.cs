
using UnityEngine.Serialization;

using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEditor;
//using DXDataTypes;

namespace DXCommunications
{
   
    public class DXPrefabSettings : ScriptableObject
    {


        [SerializeField]
        public Object LoginStateButton;

        [SerializeField]
        public Object LoginUICanvas;

        [SerializeField]
        public Object UserProfileCanvas;

        [SerializeField]
        public Object StartModuleButton;

        [SerializeField]
        public Object QuestionsCanvas;

        [SerializeField]
        public Object TestResultsCanvas;

    }
}
