using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class RoofEvent : MonoBehaviour
{

    [SerializeField] private float distance;
    [SerializeField] private float disappearTime; 
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.SetFloat("anim", 1);
            StartCoroutine(FadeOutWalk(disappearTime));
        }
    }
    private IEnumerator FadeOutWalk(float time)
    {
        Color color = Color.white;
        float speed = distance / time;
        float curTime = 0;
        while(curTime<=disappearTime)
        { 
            transform.position += Vector3.right * speed * Time.deltaTime;
            color.a -= 1 / (time / Time.deltaTime);
            spriteRenderer.color = color;
            curTime += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
