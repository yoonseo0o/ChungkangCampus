using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EyeLaser : MonoBehaviour
{
    [SerializeField] private SpriteRenderer playerR;
    [SerializeField] private SpriteRenderer playerL;

    [SerializeField] private Transform laserR;
    [SerializeField] private Transform laserL;
    [SerializeField] private Vector3 target;
    private float laserlength=2.08f;
    [SerializeField] private float blinkTime;

    [SerializeField] private Sprite[] eyeImg;
    private bool IsFire;
    private void Awake()
    {
    }
    public void EyeLaserOn(bool IsOn)
    {
        if (IsOn)
        { 
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float GradientR = (playerR.transform.position.y - target.y) / (playerR.transform.position.x - target.x);
            laserR.position = target+new Vector3((playerR.transform.position.x - target.x)/2,(playerR.transform.position.y - target.y)/2,-target.z);
            if(laserL)
                laserL.position = target + new Vector3((playerL.transform.position.x - target.x) / 2, (playerL.transform.position.y - target.y) / 2, - target.z);

            //float deg = (180 / Mathf.PI) * (target������.x / Mathf.Sqrt(Mathf.Pow(target������.x, 2)+Mathf.Pow(target������.y, 2)));// ���Ӹ������� �����غ� �� �ڵ�
            Vector2 targetEyeGap = new Vector2(playerR.transform.position.x - target.x, playerR.transform.position.y - target.y);
            float deg2 = Mathf.Atan2(targetEyeGap.y,targetEyeGap.x)*Mathf.Rad2Deg;
            laserR.rotation = Quaternion.Euler(0, 0, deg2); // z �� target�̶� �� ���� ��
            if (laserL)
            {
                targetEyeGap = new Vector2(playerL.transform.position.x - target.x, playerL.transform.position.y - target.y);
                deg2 = Mathf.Atan2(targetEyeGap.y, targetEyeGap.x) * Mathf.Rad2Deg;
                laserL.rotation = Quaternion.Euler(0, 0, deg2);
            } 
            /** laserlength ���̶� laserLength ���� ���� */
            float distance = Vector2.Distance(target, laserR.position);
            laserR.localScale = Vector3.right * (distance / laserlength) + Vector3.up;
            if(laserL)
            {
                distance = Vector2.Distance(target, laserL.position);
                laserL.localScale = Vector3.right * (distance / laserlength) + Vector3.up;
            }

            IsFire = true;
            transform.gameObject.SetActive(true);
            StartCoroutine(BlinkLaser());
        }
        else
        { 
            IsFire =false;
            transform.gameObject.SetActive(false); 
        }
    }
    private IEnumerator BlinkLaser()
    { 
        int count = 0;
        while(IsFire)
        { 
            if (count >= eyeImg.Length) count = 0;
            playerR.sprite = eyeImg[count];
            if(playerL)
                playerL.sprite = eyeImg[count];
            count++;
            yield return new WaitForSeconds(blinkTime);
        }
    }
}
