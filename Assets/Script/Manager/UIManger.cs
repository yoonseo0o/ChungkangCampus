using TMPro;
using Unity.VisualScripting;
using UnityEngine;
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
}
