using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManger : MonoBehaviour
{
    [SerializeField] private Slider timer;
    [SerializeField] private TMP_Text score;
    [SerializeField] private Transform manaList;
    [SerializeField] private Slider feverTimeGauge;
    [SerializeField] private GameObject upFloorButton;
    [SerializeField] private GameObject downFloorButton;
    public void SetTimerUI(float value)
    {
        timer.value = value;
    }
    public void SetScoreUI(int value)
    {
        // https://learn.microsoft.com/ko-kr/dotnet/standard/base-types/how-to-pad-a-number-with-leading-zeros
        // D8 : Decimal 형식(D)으로 8자리 - 부족한 자릿수는 왼쪽에 0을 채움
        score.text = value.ToString("D8");

    }
    public void SetMana(float value)
    {
        int childCount = manaList.childCount;
        float usedMana = childCount - value;
        int ceilUsedMana = Mathf.CeilToInt(usedMana);
        for (int i = 0; i < ceilUsedMana; i++)
        {
            if (childCount - 1 - i < 0)
                return;
            if (usedMana - i < 1f)
                manaList.GetChild(childCount - 1-i).GetComponent<Image>().fillAmount = 1 - (usedMana - i); 
            else
                manaList.GetChild(childCount - 1 - i).gameObject.SetActive(false);
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
        int curfloor = GameManager.Instance.FloorManager.curFloor;
        int maxfloor = GameManager.Instance.FloorManager.maxFloor;
        if (curfloor == 1)
        {
            Debug.Log("min");
            upFloorButton.SetActive(value);
            downFloorButton.SetActive(false);
        }
        else if (curfloor == maxfloor)
        {
            Debug.Log("max");
            upFloorButton.SetActive(false);
            downFloorButton.SetActive(value);
        }
        else
        {
            upFloorButton.SetActive(value);
            downFloorButton.SetActive(value);
        }
    }
}
