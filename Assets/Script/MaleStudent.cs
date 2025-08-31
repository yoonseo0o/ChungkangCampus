using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MaleStudent : AI
{
    public enum State { None, BeingAttacked, RivalMatch, OwnedByPlayer, OwnedByRival }
    public State currentState { private set; get; }

    public enum Type { none, mob, named }
    public Type type;
    [Header("Gameplay Setting")]
    public int scoreValue;
    [SerializeField] private int scoreValueSecond;
    [SerializeField] private int scoreValueRival;
    [SerializeField] private float manaCostNomal;
    [SerializeField] private float manaCostRival;

    [Header("HeartGuard Status")]
    [SerializeField] private Slider heartGuardGauge;
    [SerializeField] private Slider RivalGauge;
    private Slider useGauge;
    public static float timeToBreakGauge = 2;
    [SerializeField] private float callTime = 0.2f; // break while
    public static event Action<MaleStudent> breakHeartGuard;


    [Header("RivalMatch")]
    [SerializeField] private GameObject focusCursor;
    [SerializeField] private float rivalDelayTime;
    [SerializeField] private float rivalTimeToBreakGauge;
    public static event Action<MaleStudent> rivalMatch;
    [SerializeField] private EyeLaser eyelaser;
    [SerializeField] private Transform laserPointer;
    [SerializeField] private float manaAmount;


    [Header("Following")]
    public Transform followTarget;
     
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.debugLineColor = GetComponent<SpriteRenderer>().color;
        base.Start();
        currentState = State.None;
        useGauge = heartGuardGauge;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (currentState != State.OwnedByPlayer)
            base.Update();
        else
            FollowingTarget();
    }
    public void ReceiveFocus(bool IsOn)
    {
        focusCursor.SetActive(IsOn);
    }
    public void ReceiveEyeLaser()
    {
        switch(currentState)
        {
            case State.OwnedByPlayer: 
            case State.BeingAttacked:
                break;
            case State.None:
                useGauge.gameObject.SetActive(true);
                currentState = State.BeingAttacked;
                IsMove = false;
                UpdateAnim(animParam.hit); 
                StartCoroutine(EyeLaserAttacked());
                break;
            case State.RivalMatch:
                UpdateAnim(animParam.hit_rival); 
                if (!GameManager.Instance.ManaManager.ManaIncrease(-manaCostRival))
                    break;
                useGauge.value += useGauge.maxValue / (timeToBreakGauge / callTime);
                GameManager.Instance.Score += scoreValueRival;
                IsBreakHeartGuard(); 
                break;
        } 
    }
    public void ReceiveInterruptedEyeLaser()
    {
        if(currentState == State.BeingAttacked)
        {
            Debug.Log("´«ºû °ø°Ý Áß´Ü --------------");
            useGauge.gameObject.SetActive(false);
            currentState = State.None;
            IsMove = true;
        }
    }
    private bool IsBreakHeartGuard()
    {
        if (useGauge.value < 1 && useGauge.value > 0) 
        {
            return false;
        }
        if (useGauge.value == 1)
        {
            Debug.Log($"²¿½É ¼º°ø");
            int rivalCount = GetRivalCount();
            if (currentState==State.RivalMatch)
            {
                int scoreValue = rivalCount == 1 ? 1500 :
                    rivalCount == 2 ? 3000 :
                    rivalCount == 3 ? 5000 :
                    rivalCount == 4 ? 15000:500;
                GameManager.Instance.ManaManager.DropHeart(this.transform.position, 1+ manaAmount* rivalCount, scoreValue);
            }
            else
                GameManager.Instance.ManaManager.DropHeart(this.transform.position, 1, 500);
            currentState = State.OwnedByPlayer;
            breakHeartGuard?.Invoke(this);
            ReceiveInterruptedEyeLaser();
            transform.GetComponent<BoxCollider2D >().enabled = false;
        }
        else if (useGauge.value == 0)
        {
            Debug.Log($"²¿½É ½ÇÆÐ");
            currentState = State.OwnedByRival;
            breakHeartGuard?.Invoke(this);
            Destroy(this.gameObject);
        }
        eyelaser.IsFire = false;
        UpdateAnim(animParam.ghost);
        laserPointer.gameObject.SetActive(false);
        heartGuardGauge.gameObject.SetActive(false);
        RivalGauge.gameObject.SetActive(false);
        return true;
    }
    private IEnumerator EyeLaserAttacked()
    {
        float currentTime = 0;
        float breakTime = timeToBreakGauge / callTime; 
        while (currentState==State.BeingAttacked)
        { 
            if (!GameManager.Instance.ManaManager.ManaIncrease(-manaCostNomal / breakTime))
                break;
            useGauge.value += useGauge.maxValue / breakTime;
            currentTime += callTime;
            GameManager.Instance.Score += Mathf.RoundToInt(scoreValueSecond * callTime);
            if (IsBreakHeartGuard())
            {
                break;
            }
            // ¶óÀÌ¹ú ¸ÅÄ¡ 
            if(GetRivalCount() > 0)
            {
                if (currentTime >= rivalDelayTime)
                {
                    RivalGauge.value = useGauge.value;
                    useGauge = RivalGauge;
                    heartGuardGauge.gameObject.SetActive(false);
                    RivalGauge.gameObject.SetActive(true);
                    laserPointer.gameObject.SetActive(true);
                    laserPointer.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    laserPointer.position += Vector3.back* laserPointer.position.z;
                    eyelaser.IsFire = true;
                    StartCoroutine(eyelaser.BlinkLaser());
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

            RivalGauge.value -= GetRivalCount() * RivalGauge.maxValue / (rivalTimeToBreakGauge / callTime);
            if (IsBreakHeartGuard())
                break;
            yield return new WaitForSeconds(callTime);
        } 
    }
    public int GetRivalCount()
    {
        // player
        return rivalMatch?.GetInvocationList().Length-1 ?? 0;
    }
    private void FollowingTarget()
    { 
        // °Å¸® Á¶Àý
        if(GameManager.Instance.FollowerManager.followDistance<0)
            spriteRenderer.flipX = true;
        else 
            spriteRenderer.flipX = false;
        transform.position = Vector3.MoveTowards(transform.position, 
            followTarget.position + Vector3.right * GameManager.Instance.FollowerManager.followDistance,
            GameManager.Instance.FollowerManager.followSpeed * Time.deltaTime);
    }
}
