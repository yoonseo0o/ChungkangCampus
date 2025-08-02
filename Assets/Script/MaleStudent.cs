using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class MaleStudent : AI
{
    public enum State { None, BeingAttacked, RivalMatch, OwnedByPlayer, OwnedByRival }
    public State currentState;
    [Header("HeartGuard Status")]
    [SerializeField] private Slider heartGuardGauge;
    [SerializeField] private float timeToBreakGauge;
    [SerializeField] private float callTime = 0.2f; // break while

    [Header("RivalMatch")]
    [SerializeField] private float rivalDelayTime;
    [SerializeField] private float rivalTimeToBreakGauge;
    public static event Action<MaleStudent> rivalMatch;
    public static event Action<bool> rivalMatchEnd;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    { 
        base.debugLineColor = GetComponent<SpriteRenderer>().color;
        base.Start();
        currentState = State.None;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(currentState != State.OwnedByPlayer)
            base.Update();
    }
    public void ReceiveEyeLaser()
    {
        switch(currentState)
        {
            case State.OwnedByPlayer:
            case State.BeingAttacked:
                break;
            case State.None:
            Debug.Log("눈빛 공격 시작 --------------");
            currentState = State.BeingAttacked;
            IsMove = false;
            StartCoroutine(EyeLaserAttacked());
                break;
            case State.RivalMatch:
                Debug.Log("라이벌 공격");
                heartGuardGauge.value += heartGuardGauge.maxValue / (timeToBreakGauge / callTime);
                BreakHeartGuard(); 
                break;
        } 
    }
    public void ReceiveInterruptedEyeLaser()
    {
        if(currentState == State.BeingAttacked)
        {
            Debug.Log("눈빛 공격 중단 --------------");
            currentState = State.None;
            IsMove = true;
        }
    }
    private bool BreakHeartGuard()
    {
        if(heartGuardGauge.value <1&& heartGuardGauge.value >0) 
        {
            return false;
        }
        if (heartGuardGauge.value == 1)
        {
            Debug.Log($"꼬심 성공");
            if( currentState == State.RivalMatch)
            {
                Debug.Log("남학생 상태 확인");

                rivalMatchEnd?.Invoke(false);
            }
            currentState = State.OwnedByPlayer;
            ReceiveInterruptedEyeLaser();
        }
        else if (heartGuardGauge.value == 0)
        {
            Debug.Log($"꼬심 실패");
            currentState = State.OwnedByRival;
            rivalMatchEnd?.Invoke(true);
        }
        Destroy(this.gameObject);
        return true;
    }
    private IEnumerator EyeLaserAttacked()
    {
        float currentTime = 0;
        while (currentState==State.BeingAttacked)
        {
            heartGuardGauge.value += heartGuardGauge.maxValue / (timeToBreakGauge / callTime);
            if (BreakHeartGuard())
            {
                break;
            }
            // 라이벌 매치 
            if(GetRivalCount() > 0)
            {
                currentTime += callTime;
                if (currentTime >= rivalDelayTime)
                {
                    StartCoroutine(RivalMatch());
                    break;
                }
            }
            yield return new WaitForSeconds(callTime);
        }
    }
    private IEnumerator RivalMatch()
    {
        rivalMatch?.Invoke(this);
        currentState = State.RivalMatch;
        // 플레이어 움직임 멈춤
        while (currentState == State.RivalMatch)
        {
            //Debug.Log($"{rivalMatchCount}명의 라이벌 매치 중..");

            heartGuardGauge.value -= GetRivalCount() * heartGuardGauge.maxValue / (rivalTimeToBreakGauge / callTime);
            if (BreakHeartGuard())
                break;
            yield return new WaitForSeconds(callTime);
        }
        // 플레이어 움직이기 시작
    }
    public int GetRivalCount()
    {
        return rivalMatch?.GetInvocationList().Length-1 ?? 0;
    }
}
