
using UnityEngine.Serialization;

using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEditor;
//using DX.Assessment;
//using DX.Class;
//using DX.Lesson;

namespace DXCommunications
{
    public class DXAppLessonData : ScriptableObject
    {


        [SerializeField]
        public DXClass dxClass;

        [SerializeField]
        public DXLesson dxLesson;
        [SerializeField]
        public List<DXAssessmentQuestion> dxQuestionsList;

    }
}
