using UnityEngine;

public class Mana : MonoBehaviour
{
    private float value;

    public void Init(float v)
    {
        value = v;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            GameManager.Instance.ManaManager.ManaIncrease(value);
            Destroy(gameObject);
        }
    }
}
