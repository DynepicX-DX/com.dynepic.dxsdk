using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//using DX.Course;

using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using DXCommunications;

//using DXDataTypes;
//using DX.Assessment;

public class QuizManager : MonoBehaviour
{
    public float PassingCriteria = 0.80f;
    public TextMeshProUGUI questionLabel;
    public TextMeshProUGUI questionText;
    
    public List<TextMeshProUGUI> selectedQuestionEntries;
    public List<TextMeshProUGUI> unselectedQuestionEntries;
    public List<Button> buttons;

    public static string attemptedClassId;
    public static string attemptedLessonId;
    public static int recursionStopper = 0;

    public List<DXAssessmentQuestion> questions;
    public int questionIndex = 0;
    public int correctAnswers = 0;
    
    private DateTime TestStart;
    void Setup(DXAssessmentQuestion question)
    {
        //buttons.ForEach(button => button.onClick.RemoveAllListeners());

        questionText.text = question.question;
        //foreach (var (answer, index) in question.answers.WithIndex())
        //{
        //    buttons[index].GetComponentInChildren<TextMeshProUGUI>().text = answer;
        //}

        //foreach (var (button, index) in buttons.WithIndex())
        //{

        //    button.onClick.AddListener(() =>
        //    {
        //        var answer = question.answers[index];
        //        ButtonClicked(question, answer);
        //    });

        for (int i = 0; i < question.answers.Count; i++)
        {
           // unselectedQuestionEntries[i].text = question.answers[i];
            selectedQuestionEntries[i].text = question.answers[i];
            selectedQuestionEntries[i].GetComponentInParent<Toggle>().isOn = false;
        }
    }
    async void Start()
    {
        TestStart = System.DateTime.Now;
        if (recursionStopper > 1)
            return;

        try
        {
            DXAppLessonData dXAppLessonData = Resources.Load<DXAppLessonData>("DxAppData");

            attemptedClassId = dXAppLessonData.dxClass.groupId;
            attemptedLessonId = dXAppLessonData.dxLesson.lessonId;
            questions = dXAppLessonData.dxQuestionsList;


            //if (MOTARQuestionsInfoHandler.instance == null)
            //{
            //    GameObject goQuestions = (GameObject)Instantiate(MOTARUnityObjectSettingsHandler.instance.MOTARQuestionsInfoHandler);
            //    MOTARQuestionsInfoHandler mci = goQuestions.GetComponent<MOTARQuestionsInfoHandler>();

            //    mci.questions = questions;
               
            //}

            var correctAnswers = 0;

            
           
            Setup(questions[questionIndex]);

            

            //async void ButtonClicked(DXAssessmentQuestion question, string answer)
            //{
            //    if (answer == question.answers[question.correctAnswerIndex])
            //    {
            //        correctAnswers++;
            //    }

            //    try
            //    {
            //        await Taskify.FromAsync<DXLessonProgress>(completion =>
            //        {

            //            //need to add in required param "correct" of type boolean 

            //            // DXAssessmentClient.Instance.UpdateStudentAssessmentAnswers(
            //            //     answer: answer,
            //            //     classId: classId,
            //            //     lessonId: lessonId,
            //            //     completion: completion
            //            // );
            //        });

            //        questionIndex++;
            //        if (questionIndex >= questions.Count)
            //        {
            //            var score = (float) correctAnswers / questions.Count;
            //            PlayerPrefs.SetFloat("score", score);
            //            SceneManager.LoadSceneAsync("GameOver");
            //            DXLessonClient.Instance.UpdateStudentsProgress(
            //                classId: classId, 
            //                lessonId: lessonId, 
            //                pass: score >= 0.8,
            //                (exception, progress) =>
            //                {
                                
            //                },
            //                complete: true,
            //                score: (int) score
            //                );
            //        }
            //        else
            //        {
            //            Setup(questions[questionIndex]);
            //        }
            //    }
            //    catch (DXError e)
            //    {
            //        Debug.Log($"Description: {e.ErrorDescription}, code: {e.ErrorCode}");
            //    }
            //    catch (Exception e)
            //    {
            //        Debug.Log(e.Message);
            //        Debug.Log(e.StackTrace);
            //    }
            //}
        }
        catch (Exception e)
        {
            Debug.LogError("Exception:" + e.Message + " at " + System.DateTime.Now);
        }
        //catch (DXError e)
        //{
        //    Debug.Log($"Description: {e.ErrorDescription}, code: {e.ErrorCode}");
        //    if(e.ErrorCode == APIErrorCode.CONFLICTS_WITH_EXISTING_RESOURCE)
        //    {
        //        recursionStopper++;
        //        //await Taskify.FromAsync<DXLessonProgress>(completion =>
        //        //DXLessonClient.Instance.UpdateStudentsProgress(attemptedClassId, attemptedLessonId, false,completion));
        //        DXLessonClient.Instance.UpdateStudentsProgress(attemptedClassId, attemptedLessonId, false, completion: (error, lessonProgress) =>
        //        {
        //            if (error != null)
        //                Debug.LogError("error:" + error.Message);
        //            else
        //                Debug.Log("progress:" + lessonProgress.ToString());
        //        }, null, true, null, correctAnswers, TestStart.ToString(), System.DateTime.Now.ToString());
        //        Start();
        //    }
        //}
        //catch (Exception e)
        //{
        //    Debug.Log(e.Message);
        //    Debug.Log(e.StackTrace);
        //}
    }
    IEnumerator postAnswer(bool correct, string pquestion, int correctIndex, string cid, string lid)
    {
        yield return 0;
        Debug.Log("question:" + pquestion + " correcT:" + correct);

        DXCommunicationLayer.UpdateStudentAssessmentAnswers(correct, pquestion, correctIndex, cid, lid, (lip) =>
        {
            if (lip != null)
                Debug.Log("progress:" + lip.ToString());
            else
                Debug.LogError("Post Update Assesment error");
        });
        //try
        //{
        //    DXAssessmentClient.Instance.UpdateStudentAssessmentAnswers(correct, pquestion,
        //            correctIndex, cid, lid, (error, lessonProgress) =>
        //            {
        //                if (error != null)
        //                    Debug.LogError("error:" + error.Message);
        //                else
        //                    ;


        //                //        });
        //            });
        //}
        //catch
        //{

        //}
    }
    public void NextQuestion()
    {
        bool bCorrect = false;
        bool bPicked = false;

        int iAnswerIndex = 0;
        if (questionIndex < questions.Count)
        {
            for (int i = 0; i < selectedQuestionEntries.Count; i++)
            {
                Toggle toggle = selectedQuestionEntries[i].GetComponentInParent<Toggle>();
                if (toggle.isOn)
                    bPicked = true;
                iAnswerIndex = i;
                if (toggle.isOn && questions[questionIndex].correctAnswerIndex == i)
                {
                    bCorrect = true;
                    correctAnswers++;
                    toggle.isOn = false;
                    break;
                }

            }
        }

        if(questionIndex == questions.Count-1)
        {
            
            {
                int iScore = (int)(((float)correctAnswers / (float)questions.Count) * 100);
                bool pass = ((float)correctAnswers / (float)questions.Count) >= PassingCriteria;
                //Michsky.UI.ModernUIPack.ProgressBar.CurrentScore = iScore;
                //MOTARTestResultsCanvasHandler.instance.TextScore.text = iScore.ToString();
                MOTARTestResultsCanvasHandler.iScore = iScore;
                //DXLessonClient.Instance.UpdateStudentsProgress(attemptedClassId, attemptedLessonId, pass, completion: (error, lessonProgress) =>
                //{
                //    if (error != null)
                //        Debug.LogError("error:" + error.Message);
                //    else
                //        Debug.Log("progress:" + lessonProgress.ToString());
                //},null,true,null, correctAnswers, TestStart.ToString(),System.DateTime.Now.ToString());
                DXCommunicationLayer.UpdateStudentsProgress(attemptedClassId,attemptedLessonId,pass,completion:(lessonProgress)=>

                {
                    if (lessonProgress != null)
                        Debug.Log("progress:" + lessonProgress.ToString());
                    else
                        Debug.Log("Error in lesson update...");
                }, null, true, null, correctAnswers, TestStart.ToString(), System.DateTime.Now.ToString());


            }
            //SceneManager.LoadSceneAsync("USAF_Gameover");
            MOTARStateMachineHandler.instance.MOTARRuntimeStateMachine.SetTrigger("TestCompleted");

        }

        if (bPicked)
        {

            StartCoroutine(postAnswer(bCorrect, questions[questionIndex].answers[iAnswerIndex], questionIndex, attemptedClassId, attemptedLessonId));


            if (questionIndex < questions.Count-1)
            {
                Setup(questions[++questionIndex]);
                questionLabel.text = "Question " + (questionIndex + 1).ToString();

            }
        }

    }
}