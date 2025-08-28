using UnityEngine;

public class GrassesTeacher : MonoBehaviour
{

    [SerializeField] private GameObject grassesItem;
    private bool isStolen;
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isStolen = false;

    }
    void Start()
    {
        animator.SetFloat("anim", 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isStolen)
        {
            isStolen = true;
            Instantiate(grassesItem, transform.position+new Vector3(-1.2f,-2.1f,0), Quaternion.identity, transform);
            animator.SetFloat("anim", 1);
        }
    }
}
