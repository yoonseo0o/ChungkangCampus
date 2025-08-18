using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MaleStudent : AI
{
    public enum State { None, BeingAttacked, RivalMatch, OwnedByPlayer, OwnedByRival }
    public State currentState { private set; get; }

    public enum Type { none, mob, professor, named }
    [SerializeField] public Type type { get; protected set; }
    [Header("Gameplay Setting")]
    public int scoreValue;
    [SerializeField]
    private float manaCostNomal;
    [SerializeField]
    private float manaCostRival;

    [Header("HeartGuard Status")]
    [SerializeField] private Slider heartGuardGauge;
    public static float timeToBreakGauge = 2;
    [SerializeField] private float callTime = 0.2f; // break while
    public static event Action<MaleStudent> breakHeartGuard;

    [Header("RivalMatch")]
    [SerializeField] private float rivalDelayTime;
    [SerializeField] private float rivalTimeToBreakGauge;
    public static event Action<MaleStudent> rivalMatch;


    [Header("Following")]
    public Transform followTarget;
    [SerializeField] private float followDistance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        type = Type.mob;
        base.debugLineColor = GetComponent<SpriteRenderer>().color;
        base.Start();
        currentState = State.None;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (currentState != State.OwnedByPlayer)
            base.Update();
        else
            FollowingTarget();
    }
    public void ReceiveEyeLaser()
    {
        switch(currentState)
        {
            case State.OwnedByPlayer:
            case State.BeingAttacked:
                break;
            case State.None:
            Debug.Log("´«ºû °ø°Ý ½ÃÀÛ --------------");
            currentState = State.BeingAttacked;
            IsMove = false;
            StartCoroutine(EyeLaserAttacked());
                break;
            case State.RivalMatch:
                Debug.Log("¶óÀÌ¹ú °ø°Ý");
                if (!GameManager.Instance.ManaManager.ManaIncrease(-manaCostRival))
                    break;
                heartGuardGauge.value += heartGuardGauge.maxValue / (timeToBreakGauge / callTime);
                BreakHeartGuard(); 
                break;
        } 
    }
    public void ReceiveInterruptedEyeLaser()
    {
        if(currentState == State.BeingAttacked)
        {
            Debug.Log("´«ºû °ø°Ý Áß´Ü --------------");
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
            Debug.Log($"²¿½É ¼º°ø");
            currentState = State.OwnedByPlayer;
            breakHeartGuard?.Invoke(this);
            ReceiveInterruptedEyeLaser();
            transform.GetComponent<BoxCollider2D >().enabled = false;
        }
        else if (heartGuardGauge.value == 0)
        {
            Debug.Log($"²¿½É ½ÇÆÐ");
            currentState = State.OwnedByRival;
            breakHeartGuard?.Invoke(this);
            Destroy(this.gameObject);
        }
        return true;
    }
    private IEnumerator EyeLaserAttacked()
    {
        float currentTime = 0;
        float breakTime = timeToBreakGauge / callTime;
        Debug.Log($"{timeToBreakGauge} / {callTime} = {breakTime}");
        while (currentState==State.BeingAttacked)
        {
            Debug.Log($"{-manaCostNomal} / {breakTime} = {-manaCostNomal / breakTime}");
            if (!GameManager.Instance.ManaManager.ManaIncrease(-manaCostNomal / breakTime))
                break;
            heartGuardGauge.value += heartGuardGauge.maxValue / breakTime;
            if (BreakHeartGuard())
            {
                break;
            }
            // ¶óÀÌ¹ú ¸ÅÄ¡ 
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
        while (currentState == State.RivalMatch)
        {
            //Debug.Log($"{rivalMatchCount}¸íÀÇ ¶óÀÌ¹ú ¸ÅÄ¡ Áß..");

            heartGuardGauge.value -= GetRivalCount() * heartGuardGauge.maxValue / (rivalTimeToBreakGauge / callTime);
            if (BreakHeartGuard())
                break;
            yield return new WaitForSeconds(callTime);
        } 
    }
    public int GetRivalCount()
    {
        // player, gameManager
        return rivalMatch?.GetInvocationList().Length-2 ?? 0;
    }
    private void FollowingTarget()
    {
        // °Å¸® Á¶Àý
        
        transform.position = Vector3.MoveTowards(transform.position, followTarget.position, moveSpeed * Time.deltaTime);
    }
}
