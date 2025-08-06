using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManger : MonoBehaviour
{
    [SerializeField] private Slider timer;
    [SerializeField] private TMP_Text score;
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
}
