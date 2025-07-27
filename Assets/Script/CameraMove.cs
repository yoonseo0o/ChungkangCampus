using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraMove : MonoBehaviour
{
    public enum DirectionState { Left, Right, None }
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform target;
    [SerializeField] private float targetOffsetX;
    [SerializeField] private DirectionState currentState;
    private Coroutine moveCo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player.OnPlayerMoved += Move;
        Player.OnPlayerStopped += StopFollowTarget;
        currentState = DirectionState.None;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position-new Vector3(targetOffsetX, 0,0), transform.position + new Vector3(targetOffsetX, 0, 0),Color.green);
    }
    public void Move(Vector2 vec)
    {
        DirectionState state = vec.x>0? DirectionState.Left: DirectionState.Right;
        if (currentState == state)
        {
            return;
        }
        if (moveCo!=null) { StopCoroutine(moveCo); moveCo = null; }

        currentState = state;
        moveCo = StartCoroutine(FollowTarget());
    }
    private IEnumerator FollowTarget()
    {
        Vector3 targetVec = new Vector3(
            currentState == DirectionState.Left ?
            target.position.x + targetOffsetX : target.position.x - targetOffsetX,
            target.position.y, transform.position.z);

        while (true)
        {
            targetVec = new Vector3(
                currentState == DirectionState.Left ? 
                target.position.x + targetOffsetX : target.position.x - targetOffsetX,
                transform.position.y, transform.position.z); 
            transform.position = Vector3.MoveTowards(transform.position, targetVec, moveSpeed*Time.deltaTime); 
            yield return null;
        }
        /*Debug.Log("목표 위치 도착 완료");
        moveCo = null;*/
    }
    private void StopFollowTarget()
    {
        if (moveCo != null) 
        { 
            StopCoroutine(moveCo); 
            moveCo = null; 
        }

        currentState = DirectionState.None;
    }
}
