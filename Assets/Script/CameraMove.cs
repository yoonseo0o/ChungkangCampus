using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform target;
    [SerializeField] private float targetOffsetX;
    public enum DirectionState { Left, None, Right }
    [SerializeField] private DirectionState currentState;

    // wall 
    private enum WallState { Left, None, Right }
    [SerializeField] private WallState wallState; 
    private Coroutine moveCo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player.OnPlayerMoved += Move;
        Player.OnPlayerStopped += StopFollowTarget;
        currentState = DirectionState.None;
        wallState = WallState.None;// 
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
        if (moveCo != null) 
        { 
            StopCoroutine(moveCo); 
            moveCo = null; 
        }

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
            if (wallState == WallState.None)
            {
                RaycastHit2D rightHit = Physics2D.Raycast(transform.position, Vector3.right, targetOffsetX, LayerMask.GetMask("wall"));
                RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Vector3.left, targetOffsetX, LayerMask.GetMask("wall"));
                if (rightHit || leftHit)
                {
                    wallState = rightHit ? WallState.Right : WallState.Left;
                    currentState = DirectionState.None;
                    break;
                }
            }
            else
            { 
                if ((wallState == WallState.Right && currentState == DirectionState.Left) ||
                (wallState == WallState.Left && currentState == DirectionState.Right))
                {
                    break;
                }
                wallState = WallState.None;

            }
            targetVec = new Vector3(
                currentState == DirectionState.Left ? 
                target.position.x + targetOffsetX : target.position.x - targetOffsetX,
                transform.position.y, transform.position.z); 
            transform.position = Vector3.MoveTowards(transform.position, targetVec, moveSpeed*Time.deltaTime); 
            yield return null;
        } 
        moveCo = null;
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
