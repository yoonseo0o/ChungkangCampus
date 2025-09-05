using System.Collections;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
public class Player : MonoBehaviour
{
    [Header("ResourceConfig")]
    [SerializeField] private float maxMana;
    private float mana;
    [SerializeField] private float decreaseMana; // 한명 꼬실때?
    [Header("StatusFlags")]
    bool isGrasses;
    enum MoveState { Idle, Walk, Run, Focusing, RivalMatch, ShoulderBump }
    [SerializeField] private MoveState moveState;
    enum WallState { Left, Right, None }
    [SerializeField] private WallState wallState;
    private float wallDistanceValue;
    [Header("ControlParameters")]
    [SerializeField] private float mouseXThreshold;//임계?
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    Vector2 moveVec;
    RaycastHit2D hit;
    Coroutine moveCo;
    public static event Action<Vector2> OnPlayerMoved;
    public static event Action OnPlayerStopped;

    // wall break

    int wallLayer; 
    // attack male
    private MaleStudent attackedMale;
    private FollowerManager followingManager;

    [Header("Graphic")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private EyeLaser eyeLaser;
    enum animParam
    {
        idle = 0, walk, run, idle_glass, walk_glass, run_glass,
        bump, bump_glass, focusing, focusing_back, focusing_glass
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = transform.GetComponent<Animator>();
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        moveState = MoveState.Idle;
        wallState = WallState.None;
        MaleStudent.rivalMatch += StartRivalmatch;
        MaleStudent.breakHeartGuard += EndMaleAttack;
        wallDistanceValue = transform.GetComponent<BoxCollider2D>().size.x / 2;
        mana = maxMana;
        wallLayer = LayerMask.NameToLayer("wall"); 
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position - new Vector3(mouseXThreshold, 0, 0), transform.position + new Vector3(mouseXThreshold, 0, 0), Color.red);

        EvaluateInput();
    }
    private void OnDestroy()
    {
        if (moveCo != null)
            StopCoroutine(moveCo);
        OnPlayerMoved = null;
        OnPlayerStopped = null;
    }
    private void Move()
    {
            switch (moveState)
        {
            case MoveState.Focusing: 
            case MoveState.ShoulderBump:
            case MoveState.Idle:
                UpdateAnim();
                if (moveCo != null)
                {
                    StopCoroutine(moveCo);
                    moveCo = null;
                }
                break;
            case MoveState.Walk:
                UpdateAnim();
                if (moveCo != null)
                    StopCoroutine(moveCo);
                moveCo = StartCoroutine(WalkCo(moveVec * walkSpeed));
                break;
            case MoveState.Run:
                UpdateAnim();
                if (moveCo != null)
                    StopCoroutine(moveCo);
                moveCo = StartCoroutine(WalkCo(moveVec * runSpeed));
                break;

        }
    }
    IEnumerator WalkCo(Vector2 vec)
    {
        if (GameManager.Instance.state != GameManager.State.Playing)
            yield break;
        OnPlayerMoved?.Invoke(vec);
        while (true)
        {
            if (wallState == WallState.None)
            {

                RaycastHit2D rightHit = Physics2D.Raycast(transform.position, Vector3.right, wallDistanceValue, LayerMask.GetMask("wall") | LayerMask.GetMask("endWall"));
                RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Vector3.left, wallDistanceValue, LayerMask.GetMask("wall") | LayerMask.GetMask("endWall"));
                RaycastHit2D hit = rightHit ? rightHit : leftHit;
                if (hit)
                {
                    wallState = rightHit ? WallState.Right :WallState.Left;
                    moveVec = Vector2.zero;
                    OnPlayerStopped?.Invoke();
                    if(hit.collider.gameObject.layer == wallLayer)
                        GameManager.Instance.UIManager.SetActiveFloorButton(true);
                    break;
                }
            }
            else
            {

                if (isGrasses)
                    animator.SetFloat("anim", (int)animParam.idle_glass);
                else
                    animator.SetFloat("anim", (int)animParam.idle);
                if ((wallState == WallState.Right && moveVec == Vector2.right) ||
                (wallState == WallState.Left && moveVec == Vector2.left))
                {
                    break;
                }
                GameManager.Instance.UIManager.SetActiveFloorButton(false);
                wallState = WallState.None;

            }
            if(moveVec == Vector2.left)
            {
                spriteRenderer.flipX = false;
                GameManager.Instance.FollowerManager.followDistance = Mathf.Abs(GameManager.Instance.FollowerManager.followDistance);
            }
            else
            {
                spriteRenderer.flipX = true;
                GameManager.Instance.FollowerManager.followDistance = -Mathf.Abs(GameManager.Instance.FollowerManager.followDistance);
            }
            transform.Translate(vec * Time.deltaTime);
            yield return null;
        }
        moveCo = null;
    }
    private void SendEyeLaser()
    {
        if(attackedMale == null)
        {
            Debug.Log("attackedMale is null");
            return;
        }
        mana -= decreaseMana;
        eyeLaser.EyeLaserOn(true);
        attackedMale.ReceiveEyeLaser();
    }
    private void InterruptedEyeLaser()
    {
        if (attackedMale == null)
        {
            Debug.Log("attackedMale is null");
            return;
        }
        eyeLaser.EyeLaserOn(false);
        attackedMale.ReceiveInterruptedEyeLaser();
    }
    private void EvaluateInput()
    {
        if (moveState == MoveState.RivalMatch)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
                SendEyeLaser();
            return;
        }
        else if(moveState == MoveState.ShoulderBump)
        {
            Move();
            return;
        }
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D prevhit = hit;
        int layerMask = LayerMask.GetMask("maleStudent");
        hit = Physics2D.Raycast(mousePos, transform.forward, 15f, layerMask);

        if (prevhit != hit)
        {
            Focusing();
        }
        else if(hit)
        {
            if (attackedMale.currentState == MaleStudent.State.RivalMatch)
            {
                moveState = MoveState.RivalMatch;
            }
            if(Input.GetKeyDown (KeyCode.Mouse0))
            {
                SendEyeLaser();
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                InterruptedEyeLaser();
            }
        }
        else
        {
            float playerMouseDistance = mousePos.x - transform.position.x;
            Vector2 prevMoveVec = moveVec;
            moveVec = playerMouseDistance < 0 ? Vector2.left : Vector2.right;
            MoveState prevMoveState = moveState;

            if (Mathf.Abs(playerMouseDistance) <= mouseXThreshold / 10)
                moveState = MoveState.Idle;
            else if (Mathf.Abs(playerMouseDistance) <= mouseXThreshold)
                moveState = MoveState.Walk;
            else if (Mathf.Abs(playerMouseDistance) > mouseXThreshold)
                moveState = MoveState.Run;

            if (prevMoveState != moveState || prevMoveVec != moveVec)
            {
                Move();
            }
        }
    }
    private void Focusing()
    {
        if (hit)
        {
            moveState = MoveState.Focusing;
            UpdateAnim();

            if (moveCo != null)
            {
                StopCoroutine(moveCo);
                moveCo = null;
            }
            if (moveVec == Vector2.left)
            { 
                eyeLaser.transform.rotation = Quaternion.Euler(0,0,0);
            }
            else
            {
                eyeLaser.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            OnPlayerStopped?.Invoke();
            attackedMale = hit.transform.GetComponent<MaleStudent>();
            attackedMale.ReceiveFocus (true);
        }
        else
        {
            attackedMale.ReceiveFocus(false);
            if (attackedMale.currentState == MaleStudent.State.RivalMatch)
                return;
            else if (attackedMale.currentState == MaleStudent.State.BeingAttacked)
            { 
                InterruptedEyeLaser();
                attackedMale = null;
                if (moveState == MoveState.Focusing)
                    moveState = MoveState.Idle;
            }
        }
    }

    private void StartRivalmatch(MaleStudent male)=>
        moveState = MoveState.RivalMatch;

    private void EndMaleAttack(MaleStudent male)
    {
        moveState = MoveState.Idle;
        eyeLaser.EyeLaserOn(false);
        // 꼬심에 성공했는지 확인 하고 추가해야 함
        //followingManager.AddFollower(male);
        if (male.type == MaleStudent.Type.named)
        {
            switch (male.name)
            {
                case "NKanbi":
                    GameManager.Instance.Kanbi = true;
                    break;
                case "NAnini":
                    GameManager.Instance.Anini = true;
                    break;
                case "NYungcon":
                    GameManager.Instance.Yungcon = true;
                    break;
            }
            GameManager.Instance.clearNamedCount++;
        }
        GameManager.Instance.clearMaleCount++;

    }
    public bool ReceiveShoulderBump(float delayTime)
    {
        if (moveState == MoveState.ShoulderBump)
            return false;
        if (isGrasses)
        {
            isGrasses = false;
            Debug.Log("안경 써서 어깨빵 피함!");
            UpdateAnim();
            return false;
        }
        Debug.Log("어깨빵 맞음");
        moveState = MoveState.ShoulderBump;
        UpdateAnim();

        // 애니메이션 재생
        //Invoke("asdf", delayTime);
        StartCoroutine(DelayAction(delayTime, () =>
        {
            moveState = MoveState.Idle;
            UpdateAnim();
        }));
        return true;
    }
    /*private void asdf()
    {
        moveState = MoveState.Idle;
    }*/
    IEnumerator DelayAction(float delay, System.Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
    public void WearGrasses()
    {
        Debug.Log("안경 착용!");
        isGrasses = true; 
        UpdateAnim();
    }

    public bool TakeOffGrasses()
    {
        if (isGrasses)
        {
            Debug.Log("안경 벗기기!><");
            isGrasses = false; 
            UpdateAnim();
            return true;
        }
        else
        {
            return false;
        }
    }
    private void UpdateAnim()
    {

        switch (moveState)
        {
            case MoveState.Focusing:
                if (hit.transform.position.y > transform.position.y)
                    animator.SetFloat("anim", (int)animParam.focusing_back);
                else
                {
                    if (isGrasses)
                        animator.SetFloat("anim", (int)animParam.focusing_glass);
                    else
                        animator.SetFloat("anim", (int)animParam.focusing);
                }
                break;
            case MoveState.ShoulderBump:
                animator.SetFloat("anim", (int)animParam.bump);
                break;
            case MoveState.Idle:
                if (isGrasses)
                    animator.SetFloat("anim", (int)animParam.idle_glass);
                else
                    animator.SetFloat("anim", (int)animParam.idle);
                break;
            case MoveState.Walk:
                if (isGrasses)
                    animator.SetFloat("anim", (int)animParam.walk_glass);
                else
                    animator.SetFloat("anim", (int)animParam.walk);
                break;
            case MoveState.Run:
                if (isGrasses)
                    animator.SetFloat("anim", (int)animParam.run_glass);
                else
                    animator.SetFloat("anim", (int)animParam.run);
                break;

        }
    }
}
