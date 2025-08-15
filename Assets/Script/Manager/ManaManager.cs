using UnityEngine;

public class ManaManager : MonoBehaviour
{
    [SerializeField] private float maxMana;
    private float mana;
    //[SerializeField] private float decreaseMana; // 한명 꼬실때?
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        mana = maxMana;
    }
    public bool ManaDecrease(float amount)
    { 
        if(amount > mana)
            return false;
        mana -= amount;
        //mana = Mathf.Ceil(mana * 100f) / 100f;
        GameManager.Instance.UIManger.SetMana(mana);
        if(mana < 0)
        {
            // 마나 소진시 미구현
            Debug.Log("마나 소진");
        }
        return true;
    }
}
