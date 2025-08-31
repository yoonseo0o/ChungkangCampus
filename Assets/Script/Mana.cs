using UnityEngine;

public class Mana : MonoBehaviour
{
    private float manaValue;
    private int scoreValue;

    public void Init(float m,int s)
    {
        manaValue = m;
        scoreValue = s;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            GameManager.Instance.ManaManager.ManaIncrease(manaValue);
            GameManager.Instance.Score += scoreValue;
            Destroy(gameObject);
        }
    }
}
