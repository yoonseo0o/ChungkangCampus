using System.Collections;
using UnityEngine;

public class AI : MonoBehaviour
{
    [Header("MoveStatus")]
    [SerializeField] private float movementRange;
    [SerializeField] protected float moveSpeed;
    [SerializeField] private float moveDistance;
    [SerializeField] protected bool IsMove;

    private Vector3 pivotPoint;

    Coroutine co;
    protected Color debugLineColor;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    protected enum animParam
    {
        idle = 0, walk, hit, hit_rival,ghost
    }

    protected virtual void Start()
    {
        animator = transform.GetComponent<Animator>();
        spriteRenderer = transform.GetComponent<SpriteRenderer>();

        IsMove = true;
        pivotPoint = transform.position;
    }
    protected virtual void Update()
    {
        if (IsMove)
            Move();
        else
        {
            if (co != null)
            {
                StopCoroutine(co);
                co = null;
            }
        }
        Debug.DrawLine(pivotPoint - Vector3.right * movementRange,
            pivotPoint + Vector3.right * movementRange, debugLineColor);
    }
    private void Move()
    {
        if (co != null) return;

        float direction;
        if (transform.position.x - moveDistance * moveSpeed * Time.deltaTime < pivotPoint.x - movementRange)
        {
            direction = 1;
        }
        else if (transform.position.x + moveDistance * moveSpeed * Time.deltaTime > pivotPoint.x + movementRange)
        {
            direction = -1;
        }
        else
        { 
            direction = Random.Range(-1, 2);
        }
        co = StartCoroutine(moveCo(direction));
    }
    private IEnumerator moveCo(float direction)
    { 
        float count = moveDistance;
        Vector3 trfVec = transform.position;

        while (count > 0 && IsMove)
        {
            //Debug.DrawLine(trfVec + Vector3.up/2, trfVec + direction * Vector3.right * walkDistance + Vector3.up / 2, Color.blue); 
            if (direction == 0) 
            UpdateAnim(animParam.idle); 
            else
            {
                UpdateAnim(animParam.walk);
                if (direction < 0)
                    spriteRenderer.flipX = false;
                else
                    spriteRenderer.flipX = true;
            }
            transform.Translate(direction * moveSpeed * Time.deltaTime, 0, 0);
            count -= moveSpeed * Time.deltaTime;
            yield return null;
        }
        co = null;
    }
    protected void UpdateAnim(animParam anim)
    {
        animator.SetFloat("anim", (int)anim);
    }
}
