using System.Collections;
using UnityEngine;

public class AI : MonoBehaviour
{
    [Header("MoveStatus")]
    [SerializeField] private float movementRange;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveDistance;

    private Vector3 pivotPoint;
    
    Coroutine co;
    protected Color debugLineColor;
    protected virtual void Start()
    { 
        pivotPoint = transform.position;
    }
    protected virtual void Update()
    {
        Move();
        Debug.DrawLine(pivotPoint - Vector3.right * movementRange,
            pivotPoint + Vector3.right * movementRange, debugLineColor);
    }
    private void Move()
    {
        if (co != null) return;

        float direction; 
        if (transform.position.x - moveDistance*moveSpeed * Time.deltaTime < pivotPoint.x - movementRange)
        { 
            direction = 1;
        }
        else if (transform.position.x + moveDistance*moveSpeed * Time.deltaTime > pivotPoint.x + movementRange)
        { 
            direction = -1;
        }
        else
        {
            direction =Random.Range(-1,2); 
        }
        co = StartCoroutine(moveCo(direction));
    }
    private IEnumerator moveCo(float direction)
    {
        float count = moveDistance; 
        Vector3 trfVec = transform.position;

        while (count>0)
        {
            //Debug.DrawLine(trfVec + Vector3.up/2, trfVec + direction * Vector3.right * walkDistance + Vector3.up / 2, Color.blue); 
            transform.Translate(direction * moveSpeed * Time.deltaTime, 0, 0);
            count -= moveSpeed * Time.deltaTime;
            yield return null;
        }
        co = null;
    }
}
