using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
public class UIManger : MonoBehaviour
{
    [SerializeField] private Slider timer;
    [SerializeField] private Transform timerHandle;

    [SerializeField] private TMP_Text score;
    [SerializeField] private Transform manaList;
    [SerializeField] private Slider feverTimeGauge;
    [SerializeField] private GameObject floorButton;
    private float manaValue=-1;
    [Header("Start")]
    [SerializeField] private GameObject start;
    [Header("Ending")]
    [SerializeField] private GameObject ending;
    [SerializeField] private TMP_Text endingScoreText;
    [SerializeField] private Image gradeSR;
    [SerializeField] private Sprite[] gradeImg;
    [SerializeField] private Image gayoSR;
    [SerializeField] private Sprite[] gayoImg;
    [SerializeField] private GameObject sticker_kanbi;
    [SerializeField] private GameObject sticker_anini;
    [SerializeField] private GameObject sticker_yungcon;
    [Header("Fade")]
    [SerializeField] private Image fadeImg;
    [SerializeField] private float fadeSpeed;
    public Action fadeComplete;
    private void OnDestroy()
    {
        fadeComplete = null;
    }
    public void SetTimerUI(float value)
    {
        timer.value = value;
        timerHandle.rotation = Quaternion.Euler(0, 0, value * -360);
    }
    public void SetScoreUI(int value)
    {
        // https://learn.microsoft.com/ko-kr/dotnet/standard/base-types/how-to-pad-a-number-with-leading-zeros
        // D8 : Decimal 형식(D)으로 8자리 - 부족한 자릿수는 왼쪽에 0을 채움
        score.text = value.ToString("D8");
    }
    public void SetManaUI(float value)
    {

        int childCount = manaList.childCount; // 9
        float usedMana = childCount - value; // 9 - 4.5 = 4.5
        int ceilUsedMana = Mathf.CeilToInt(usedMana); // 5
        for (int i = 0; i < childCount; i++) 
        {
            if (childCount - 1 - i < 0)
                return;
            if (usedMana - i < 1f)
            {
                manaList.GetChild(childCount - 1 - i).gameObject.SetActive(true); 
                manaList.GetChild(childCount - 1 - i).GetComponent<Image>().fillAmount = 1 - (usedMana - i);
            }
            else if(i < usedMana)
                manaList.GetChild(childCount - 1 - i).gameObject.SetActive(false);
            else
                manaList.GetChild(childCount - 1 - i).gameObject.SetActive(true);
        }
    }
    public void SetActiveFeverGauge(bool value)
    {
        feverTimeGauge.gameObject.SetActive(value);
        feverTimeGauge.value = 1;
    }
    public void SetValueFeverGauge(float value)
    {
        feverTimeGauge.value = value;
    }
    public void SetActiveFloorButton(bool value)
    { 
        floorButton.SetActive(value); 
    }
    public void EndingResource(GameManager.ScoreGrade scoreGrade)
    {
        ending.SetActive(true);
        endingScoreText.text = score.text;
        switch (scoreGrade)
        {
            case GameManager.ScoreGrade.S:
                gradeSR.sprite = gradeImg[0]? gradeImg[0]: gradeSR.sprite;
                gayoSR.sprite = gayoImg[0] ? gayoImg[0] : gayoSR.sprite;
                if (GameManager.Instance.Kanbi)
                    sticker_kanbi.SetActive(true);
                if (GameManager.Instance.Anini)
                    sticker_anini.SetActive(true);
                if (GameManager.Instance.Yungcon)
                    sticker_yungcon.SetActive(true);
                break;
            case GameManager.ScoreGrade.A:
                gradeSR.sprite = gradeImg[1] ? gradeImg[1] : gradeSR.sprite;
                gayoSR.sprite = gayoImg[1] ? gayoImg[1] : gayoSR.sprite;
                if (GameManager.Instance.Kanbi)
                    sticker_kanbi.SetActive(true);
                if (GameManager.Instance.Anini)
                    sticker_anini.SetActive(true);
                if (GameManager.Instance.Yungcon)
                    sticker_yungcon.SetActive(true);
                break;
            case GameManager.ScoreGrade.B:
                gradeSR.sprite = gradeImg[2] ? gradeImg[2] : gradeSR.sprite;
                gayoSR.sprite = gayoImg[2] ? gayoImg[2] : gayoSR.sprite;
                break;
            case GameManager.ScoreGrade.C:
                gradeSR.sprite = gradeImg[3] ? gradeImg[3] : gradeSR.sprite;
                gayoSR.sprite = gayoImg[3] ? gayoImg[3] : gayoSR.sprite;
                break;
        }
    }
    public void SetStartScene(bool value)
    {
        start.SetActive(value);
    }
    public IEnumerator FadeIn()
    {
        fadeImg.gameObject.SetActive(true);
        Color color = Color.black;
        color.a = 1;
        fadeImg.color = color;
        while (fadeImg.color.a >= 0.01f)
        { 
            color.a -= fadeSpeed * Time.deltaTime;
            fadeImg.color = color;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        fadeImg.gameObject.SetActive(false);
        fadeComplete?.Invoke();
        fadeComplete=null;
    }
    public IEnumerator FadeOut()
    {
        fadeImg.gameObject.SetActive(true);
        Color color = Color.black;
        color.a = 0;
        fadeImg.color = color;
        while (fadeImg.color.a <= 0.99f)
        {
            color.a += fadeSpeed * Time.deltaTime;
            fadeImg.color = color;
            yield return null;
        }
        fadeComplete?.Invoke();
    }
}
