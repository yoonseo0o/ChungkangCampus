using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MaleStudent : AI
{
    [Header("HeartGuard Status")]
    [SerializeField] private Slider heartGuardGauge;
    [SerializeField] private float timeToBreakGauge;  
    private bool beAttacked; // 공격당하고 있는지 ing
    private bool IsAttacked; // 공격 당했는지 
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
        Debug.Log("눈빛 공격 시작 --------------");
        IsMove = false;
        beAttacked =true;
        StartCoroutine(EyeLaserAttacked());
    }
    public void ReceiveInterruptedEyeLaser()
    {
        if(beAttacked)
        {
            Debug.Log("눈빛 공격 중단 --------------");
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
                Debug.Log($"꼬심 성공");
                ReceiveInterruptedEyeLaser();
                IsMove = false;
                IsAttacked = true;
            }

            yield return new WaitForSeconds(callTime);
        }
    }
}
