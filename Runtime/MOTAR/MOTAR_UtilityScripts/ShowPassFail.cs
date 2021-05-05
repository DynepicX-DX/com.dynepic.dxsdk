using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowPassFail : MonoBehaviour
{
    public GameObject Pass;
    public GameObject Fail;
    public static ShowPassFail instance;
    public bool bDone = false;
    public Image ScoreAnimationImage;
    public bool Test = false;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
       
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Test)
        {
            Pass.SetActive(false);
            Fail.SetActive(false);
            MOTARTestResultsCanvasHandler.iScore = Random.Range(20, 101);
            StartCoroutine(ShowScoreCalculationAnimation());
            Test = false;
        }    
        
    }
    IEnumerator ShowScoreCalculationAnimation()
    {
        ScoreAnimationImage.fillAmount = 0;
        yield return new WaitForSeconds(1f);
        
        for(int i=0;i<360;i++)
        {
            yield return 0;

            float fPercentage = ((float)i / 6f) / 100f;

            ScoreAnimationImage.fillAmount += fPercentage;
            yield return new WaitForSeconds(0.05f);
            if (ScoreAnimationImage.fillAmount > (float)MOTARTestResultsCanvasHandler.iScore / 100f)
                break;
            MOTARTestResultsCanvasHandler.instance.TextScore.text = MOTARTestResultsCanvasHandler.iScore.ToString();
        }
        bool bPass = MOTARTestResultsCanvasHandler.iScore >= 80;
        //StartCoroutine(ShowScoreCalculationAnimation());
        Pass.SetActive(bPass);
        Fail.SetActive(!bPass);
    }
    public void ShowStats()
    {
        bDone = true;
        //bool bPass = (Michsky.UI.ModernUIPack.ProgressBar.CurrentScore >= 80);
        
        StartCoroutine(ShowScoreCalculationAnimation());
       
    }
}
