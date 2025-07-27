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
    enum MoveState { Idle, Walk, Run, Focusing }
    [Header("ResourceConfig")]
    float mana;
    [Header("StatusFlags")]
    bool isMask;
    [SerializeField] private MoveState moveState;
    [Header("ControlParameters")]
    [SerializeField] private float mouseXThreshold;//�Ӱ�?
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    public static event Action<Vector2> OnPlayerMoved;
    public static event Action OnPlayerStopped;
    Vector2 moveVec;
    Coroutine moveCo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveState = MoveState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Debug.DrawLine(transform.position-new Vector3(mouseXThreshold,0,0), transform.position + new Vector3(mouseXThreshold, 0, 0),Color.red);
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float maxDistance = 15;
        RaycastHit2D hit=Physics2D.Raycast(mousePos,transform.forward, maxDistance);
        if(hit)
        { 
            moveState = MoveState.Focusing;
            if (moveCo != null)
            {
                StopCoroutine(moveCo);
                moveCo = null;
            }
            OnPlayerStopped?.Invoke();
        }
        else
        {
            moveState = MoveState.Idle;
        }
    } 
    private void Move()
    {
        if (moveState == MoveState.Focusing) return;
        // ���콺 �����Ͱ� �÷��̾��� ����ʿ� �ִ��� �Ǵ�
        float mouseX = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x, 
            Input.mousePosition.y, -Camera.main.transform.position.z)).x;
        float playerMouseDistance = mouseX - transform.position.x;
        Vector2 prevMoveVec = moveVec;
        moveVec = playerMouseDistance < 0 ? Vector2.left : Vector2.right;
        
        // ������ �Ÿ��� ���� �̵� ����
        if (Mathf.Abs(playerMouseDistance) <= mouseXThreshold/10)
        {
            moveState = MoveState.Idle;
            if (moveCo != null) {
                StopCoroutine(moveCo);
                moveCo = null;
            }
            return;
        }
        else if (Mathf.Abs(playerMouseDistance) <= mouseXThreshold)
        {
            if (moveState == MoveState.Walk && moveVec == prevMoveVec)
            {
                Debug.Log("walk ���� ���� vector�� return");
                return;
            }
            moveState = MoveState.Walk;
            if (moveCo != null) 
                StopCoroutine(moveCo); 
            moveCo = StartCoroutine(WalkCo(moveVec * walkSpeed));
        }
        else if(Mathf.Abs(playerMouseDistance) > mouseXThreshold)
        {
            if (moveState == MoveState.Run && moveVec == prevMoveVec)
            {
                Debug.Log("Run ���� ���� vector�� return");
                return;
            }
            moveState = MoveState.Run;
            if (moveCo != null) 
                StopCoroutine(moveCo); 
            moveCo = StartCoroutine(WalkCo(moveVec * runSpeed));
        }
    }
    IEnumerator WalkCo(Vector2 vec)
    {
        OnPlayerMoved?.Invoke(vec);
        while (true)
        {
            transform.Translate(vec  * Time.deltaTime);
            yield return null;
        }
    } 
    private void SendEyeLaser()
    {

    }
    private void Focusing()
    {

    }
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
