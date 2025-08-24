using UnityEngine;

public class Grasses : MonoBehaviour
{ 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("¾È°æÀÌ¶û ´ê¾Ò´Ù!");
            collision.GetComponent<Player>().WearGrasses();
            Destroy(gameObject);
        }
    }
}
