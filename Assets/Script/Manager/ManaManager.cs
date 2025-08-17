using System.Collections;
using UnityEngine;

public class ManaManager : MonoBehaviour
{
    [SerializeField] private float maxMana;
    private float mana;

    [Header("Fever")]
    [SerializeField] private float feverTimeDuration;
    [SerializeField] private float decreaseRate; // ���л� ���ô� �ð� ���Һ���
    public bool IsFeverTime;
    //[SerializeField] private float decreaseMana; // �Ѹ� ���Ƕ�?
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        mana = maxMana;
        IsFeverTime = false;
        StartCoroutine(FeverTime());
    }
    public bool ManaIncrease(float amount)
    {
        Debug.Log(amount);
        if (IsFeverTime) return true; 
        if (amount < 0 && amount > mana)
        { 
            return false; 
        }
        else
        {
            if (mana + amount > maxMana)
                mana = maxMana;
            else
                mana += amount;
        }
        //mana = Mathf.Ceil(mana * 100f) / 100f;
        GameManager.Instance.UIManger.SetMana(mana);
        if(mana < 0)
        {
            // ���� ������ �̱���
            Debug.Log("���� ����");
        }
        else if ( mana >= maxMana)
        {
            StartCoroutine(FeverTime());
        } 
        return true;
    }  
    private IEnumerator FeverTime()
    {
        IsFeverTime = true;
        GameManager.Instance.UIManger.SetActiveFeverGauge(true);
        float curTime = feverTimeDuration;

        MaleStudent.timeToBreakGauge *= decreaseRate;  
        while (curTime>0)
        {
            curTime -= Time.deltaTime;
            GameManager.Instance.UIManger.SetValueFeverGauge(curTime/feverTimeDuration); 
            yield return null;
        }
        MaleStudent.timeToBreakGauge /= decreaseRate; 
        GameManager.Instance.UIManger.SetActiveFeverGauge(false);
        IsFeverTime = false; 
    }
}
