using UnityEngine;

public class EyeLaser : MonoBehaviour
{
    [SerializeField] private GameObject playerR;
    [SerializeField] private GameObject playerL;
    [SerializeField] private Vector3 target;

    [SerializeField]
    private Transform laserR;
    private Transform laserL;
    private float laserlength=4.16f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EyeLaserOn(bool IsOn)
    {
        if (IsOn)
        {
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float GradientR = (playerR.transform.position.y - target.y) / (playerR.transform.position.x - target.x);

            laserR.position = target+new Vector3((playerR.transform.position.x - target.x)/2,(playerR.transform.position.y - target.y)/2,0);

            laserR.rotation = Quaternion.Euler(0, 0, 0); // z °ª targetÀÌ¶û ´« ±â¿ï±â °ª
                                                         //GradientR ==1 45
                                                         // GradientR ==0/1 0

            /** laserlength ±æÀÌ¶û laserLength ¹èÀ² Â÷ÀÌ */
            float distance = Vector3.Distance(target, laserR.position);
            laserR.localScale = Vector3.right * (distance / laserlength);

            playerR.SetActive(true);
            playerL.SetActive(true);
        }
        else
        {
            playerR.SetActive(false);
            playerL.SetActive(false);
        }
    }
}
