using System;
using Unity;
using UnityEditor;
using UnityEngine;

using UnityEngine.Serialization;

using System.Collections;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Generic;



using UnityEditor.UIElements;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;

namespace DXCommunications 
{

    [Serializable]
    public class DXAssessmentQuestion
    {
        public List<string> answers;
        public string questionType;
        public string question;
        public int correctAnswerIndex;
    }

    [Serializable]
    public class DXAssessmentAnswer
    {
        public string answer;
        public string question;
        public bool correct;
    }


    [Serializable]
    public class DXCourse
    {
        public string courseId;
        public string name;
        public string description;
        public string profilePic;
        public string coverPhoto;
    }

    [Serializable]
    public class DXClass
    {
        public string groupId;
        public string name;
        public string joinCode;
       
        public DXCourse course;
    }

    [Serializable]
    public class GetAllClassesResponse
    {
        public List<DXClass> docs;
    }
    [Serializable]
    public class GetAllLessonsResponse
    {
        public List<DXLesson> docs;
    }
    
    [Serializable]
    internal class UpdateProgressBody
    {
        public bool correct;
        public string answer;
        public int questionIndex;
        public string classId;
        public string lessonId;
        public string studentId;
    }

    [Serializable]
    internal class UpdateStudentsProgressBody
    {
        public string classId;
        public string lessonId;
        public bool pass;
        public string studentId;
        public bool complete;
        public string answers;
        public int score;
        public string startTime;
        public string endTime;
    }
    [Serializable]
    public class DXLesson
    {
        public string lessonId;
        public string courseId;
        public string name;
        public string description;
        public string profilePic;
        public string coverPhoto;
        public string media;
        public bool isAssessment;
        public List<DXLesson> childLessons;
        public int expectedCompletionTime;
        public bool instructorGraded;
        public bool @public;
        public bool locked;
    }

    [Serializable]
    public class DXLessonProgress
    {
        public string studentId;
        public string lessonId;
        public bool pass;
        public int score;
        public List<DXAssessmentAnswer> answers;
        public string started;
        public string completed;
        public List<Interval> progressIntervals;
        public List<Interval> idleIntervals;

        [Serializable]
        public class Interval
        {
            public string start;
            public string end;
        }
    }
    

    [Serializable]
    public class DXProfile
    {
        public string userId;
        public UserType userType;
        public AccountType accountType;
        public string handle;
        public string firstName;
        public string lastName;
        public string profilePic;
        public string coverPhoto;
        public string country;
        public bool anonymous;
        public string position;
        public string role;
       // public Dictionary<string,string> organizationRoles;
        public string email;
        public bool trainer;
        public bool trainee;

        public enum UserType
        {
            [EnumMember(Value = "adult")]
            Adult,

            [EnumMember(Value = "child")]
            Child,

            [EnumMember(Value = "teen-minor")]
            TeenMinor
        }

        public enum AccountType
        {
            [EnumMember(Value = "Parent")]
            Parent,

            [EnumMember(Value = "Kid")]
            Kid,

            [EnumMember(Value = "Adult")]
            Adult,

            [EnumMember(Value = "Character")]
            Character,

            [EnumMember(Value = "Community")]
            Community
        }
    }
}
