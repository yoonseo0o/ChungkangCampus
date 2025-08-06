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
        // D8 : Decimal ����(D)���� 8�ڸ� - ������ �ڸ����� ���ʿ� 0�� ä��
        score.text = value.ToString("D8");

    }
}
