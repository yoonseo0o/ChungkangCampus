using System.Collections;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
enum MoveState { Idle, Walk, Run }
public class Player : MonoBehaviour
{
    [Header("ResourceConfig")]
    float mana;
    [Header("StatusFlags")]
    bool isMask;
    [SerializeField] private MoveState moveState;
    [Header("ControlParameters")]
    [SerializeField] private float mouseXThreshold;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

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
    } 
    private void Move()
    {
        float mouseX = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x, 
            Input.mousePosition.y, -Camera.main.transform.position.z)).x;
        float playerMouseDistance = mouseX - transform.position.x;
        Vector2 prevMoveVec = moveVec;
        moveVec = playerMouseDistance < 0 ? Vector2.left : Vector2.right;
        
        if (Mathf.Abs(playerMouseDistance) <= mouseXThreshold/10)
        {
            moveState = MoveState.Idle;
            if (moveCo != null) StopCoroutine(moveCo);
            return;
        }
        else if (Mathf.Abs(playerMouseDistance) <= mouseXThreshold)
        {
            if (moveState == MoveState.Walk && moveVec == prevMoveVec)
                return;
            moveState = MoveState.Walk;
            if(moveCo!=null) StopCoroutine(moveCo);
            moveCo = StartCoroutine(WalkCo(moveVec));
        }
        else if(Mathf.Abs(playerMouseDistance) > mouseXThreshold)
        {
            if (moveState == MoveState.Run && moveVec == prevMoveVec)
                return;
            moveState = MoveState.Run;
            if (moveCo != null) StopCoroutine(moveCo);
            moveCo = StartCoroutine(RunCo(moveVec));
        } 
    }
    IEnumerator WalkCo(Vector2 vec)
    {
        while(true)
        {
            transform.Translate(vec * walkSpeed * Time.deltaTime);
            yield return null;
        }
    }
    IEnumerator RunCo(Vector2 vec)
    {
        while (true)
        {
            transform.Translate(vec * runSpeed * Time.deltaTime);
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
