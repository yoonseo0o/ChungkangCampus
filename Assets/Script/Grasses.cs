using UnityEngine;

public class Grasses : MonoBehaviour
{ 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("�Ȱ��̶� ��Ҵ�!");
            collision.GetComponent<Player>().WearGrasses();
            Destroy(gameObject);
        }
    }
}
