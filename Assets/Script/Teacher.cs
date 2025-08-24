using UnityEngine;

public class Teacher : AI
{
    [Header("ShoulderBump")]
    [SerializeField] private float delayTimeShoulderBump;
    [SerializeField] private float manaAmount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.debugLineColor = GetComponent<SpriteRenderer>().color;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player"))
        {
            if(GameManager.Instance.player.ReceiveShoulderBump(delayTimeShoulderBump))
                GameManager.Instance.ManaManager.ManaIncrease(manaAmount);
        }
    }
}
