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

    [Header("DropItem")]
    [SerializeField] private GameObject manaPrefab;
    //[SerializeField] private float decreaseMana; // �Ѹ� ���Ƕ�?
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //mana = maxMana;
        IsFeverTime = false;
    }
    public void SetMana(float m)
    {
        ManaIncrease(m - mana); // 4-9 -5
    }
    public bool ManaIncrease(float amount) 
    {
        if (IsFeverTime) return true;

        if (mana + amount > maxMana)
            mana = maxMana;
        else if (mana + amount < 0)
            mana = 0;
        else
            mana += amount;
        Debug.Log($"manaIncrease | amount : {amount}, mana : {mana}");
        //mana = Mathf.Ceil(mana * 100f) / 100f;
        GameManager.Instance.UIManger.SetManaUI(mana);
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
    public void DropMana(Vector3 pos, float value)
    {
        GameObject obj = Instantiate(manaPrefab, pos, Quaternion.identity, transform);
        obj.GetComponent<Mana>().Init(value);
        obj.transform.localScale = Vector3.one*value;
    }
}
