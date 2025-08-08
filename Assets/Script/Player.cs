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
    float mana;
    [Header("StatusFlags")]
    bool isMask;
    enum MoveState { Idle, Walk, Run, Focusing, RivalMatch }
    [SerializeField] private MoveState moveState;
    enum WallState { Left, Right, None }
    [SerializeField] private WallState wallState;
    private float wallDistanceValue;
    [Header("ControlParameters")]
    [SerializeField] private float mouseXThreshold;//юс╟Х?
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    Vector2 moveVec;
    RaycastHit2D hit;
    Coroutine moveCo;

    public static event Action<Vector2> OnPlayerMoved;
    public static event Action OnPlayerStopped;
    // attack male
    private MaleStudent attackedMale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveState = MoveState.Idle;
        wallState = WallState.None;
        MaleStudent.rivalMatch += StartRivalmatch;
        MaleStudent.breakHeartGuard += EndRivalMatch;
        wallDistanceValue = transform.GetComponent<BoxCollider2D>().size.x / 2;

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position - new Vector3(mouseXThreshold, 0, 0), transform.position + new Vector3(mouseXThreshold, 0, 0), Color.red);

        EvaluateInput();
    }
    private void Move()
    {
        switch (moveState)
        {
            case MoveState.Focusing:
                break;
            case MoveState.Idle:
                if (moveCo != null)
                {
                    StopCoroutine(moveCo);
                    moveCo = null;
                }
                break;
            case MoveState.Walk:
                if (moveCo != null)
                    StopCoroutine(moveCo);
                moveCo = StartCoroutine(WalkCo(moveVec * walkSpeed));
                break;
            case MoveState.Run: 
                if (moveCo != null)
                    StopCoroutine(moveCo);
                moveCo = StartCoroutine(WalkCo(moveVec * runSpeed));
                break;

        }
    }
    IEnumerator WalkCo(Vector2 vec)
    {
        OnPlayerMoved?.Invoke(vec);
        while (true)
        {
            if (wallState == WallState.None)
            {

                RaycastHit2D rightHit = Physics2D.Raycast(transform.position, Vector3.right, wallDistanceValue, LayerMask.GetMask("wall"));
                RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Vector3.left, wallDistanceValue, LayerMask.GetMask("wall"));
                if (rightHit || leftHit)
                {
                    wallState = rightHit ? WallState.Right :WallState.Left;
                    moveVec = Vector2.zero;
                    OnPlayerStopped?.Invoke();
                    break;
                }
            }
            else
            {

                if ((wallState == WallState.Right && moveVec == Vector2.right) ||
                (wallState == WallState.Left && moveVec == Vector2.left))
                {
                    break;
                }
                wallState = WallState.None;

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
        attackedMale.ReceiveEyeLaser();
    }
    private void InterruptedEyeLaser()
    {
        if (attackedMale == null)
        {
            Debug.Log("attackedMale is null");
            return;
        }
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
            if (moveCo != null)
            {
                StopCoroutine(moveCo);
                moveCo = null;
            }
            OnPlayerStopped?.Invoke();
            attackedMale = hit.transform.GetComponent<MaleStudent>();
            
        }
        else
        {
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

    private void EndRivalMatch(MaleStudent male) =>
        moveState = MoveState.Idle;
    private void FeverTime()
    {

    }
    public void ReceiveShoulderBump()
    {

    }
    public void EquipMask()
    {

    }

}
