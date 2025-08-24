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
    [SerializeField] private float followDistance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        type = Type.mob;
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
            Debug.Log("´«ºû °ø°Ý ½ÃÀÛ --------------");
                useGauge.gameObject.SetActive(true);
                currentState = State.BeingAttacked;
            IsMove = false;
            StartCoroutine(EyeLaserAttacked());
                break;
            case State.RivalMatch:
                Debug.Log("¶óÀÌ¹ú °ø°Ý");
                if (!GameManager.Instance.ManaManager.ManaIncrease(-manaCostRival))
                    break;
                Debug.Log($"gauge : {useGauge.value}");
                useGauge.value += useGauge.maxValue / (timeToBreakGauge / callTime);
                BreakHeartGuard(); 
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
    private bool BreakHeartGuard()
    {
        if(useGauge.value < 1 && useGauge.value > 0) 
        {
            return false;
        }
        if (useGauge.value == 1)
        {
            Debug.Log($"²¿½É ¼º°ø");
            if(currentState==State.RivalMatch)
                GameManager.Instance.ManaManager.DropMana(this.transform.position, 1+ manaAmount*GetRivalCount());
            else
                GameManager.Instance.ManaManager.DropMana(this.transform.position,1);
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
            if (BreakHeartGuard())
            {
                break;
            }
            Debug.Log($"´ë±â ÁßÀÎ ¶óÀÌ¹ú ¼ö : {GetRivalCount()}");
            // ¶óÀÌ¹ú ¸ÅÄ¡ 
            if(GetRivalCount() > 0)
            {
                currentTime += callTime;
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
            if (BreakHeartGuard())
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
        
        transform.position = Vector3.MoveTowards(transform.position, followTarget.position, moveSpeed * Time.deltaTime);
    }
}
