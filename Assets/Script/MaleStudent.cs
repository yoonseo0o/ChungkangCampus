using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MaleStudent : AI
{
    [Header("HeartGuard Status")]
    [SerializeField] private Slider heartGuardGauge;
    [SerializeField] private float timeToBreakGauge;  
    private bool beAttacked; // ���ݴ��ϰ� �ִ��� ing
    private bool IsAttacked; // ���� ���ߴ��� 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    { 
        base.debugLineColor = GetComponent<SpriteRenderer>().color;
        IsAttacked =false;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(!IsAttacked)
            base.Update();
    }
    public void ReceiveEyeLaser()
    {
        Debug.Log("���� ���� ���� --------------");
        IsMove = false;
        beAttacked =true;
        StartCoroutine(EyeLaserAttacked());
    }
    public void ReceiveInterruptedEyeLaser()
    {
        if(beAttacked)
        {
            Debug.Log("���� ���� �ߴ� --------------");
            beAttacked =false;
            IsMove = true;
        }
    }
    private IEnumerator EyeLaserAttacked()
    {
        float callTime = 0.2f;
        while(beAttacked)
        {
            heartGuardGauge.value += heartGuardGauge.maxValue / (timeToBreakGauge / callTime);
            if( heartGuardGauge.value == 1 )
            {
                Debug.Log($"���� ����");
                ReceiveInterruptedEyeLaser();
                IsMove = false;
                IsAttacked = true;
            }

            yield return new WaitForSeconds(callTime);
        }
    }
}
