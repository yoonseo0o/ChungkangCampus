using System.Collections;
using UnityEngine;

public class FemaleStudent : AI
{
    [SerializeField] private EyeLaser eyeLaser;
    [SerializeField] private GameObject surpriseImg;
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
        if(collision.CompareTag("MainCamera"))
        {
             Debug.Log("camera에 닿았다 OnTriggerEnter2D");
            MaleStudent.rivalMatch += Matching;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("MainCamera"))
        {
            Debug.Log("camera에 닿았다 OnTriggerExit2D");
            MaleStudent.rivalMatch -= Matching;
            MaleStudent.breakHeartGuard -= MatchEnd;
        }
    }
    private void Matching(MaleStudent male)
    {
        MatchStartAnim();
        IsMove = false;
        MaleStudent.breakHeartGuard += MatchEnd;
        //flip idle
        if(male.transform.position.x - transform.position.x > 0)
        {
            base.spriteRenderer.flipX = true;
            eyeLaser.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            base.spriteRenderer.flipX = false;
            eyeLaser.transform.rotation = Quaternion.Euler(0, 0, 0);
        } 
        eyeLaser.EyeLaserOn(true);
    }
    private void MatchEnd(MaleStudent male)
    {
        /*if(male.currentState == MaleStudent.State.OwnedByRival) 
            Debug.Log($"{this.name}가 라이벌 대결 성공 !");
        else Debug.Log($"{this.name}가 라이벌 대결 실패 !");*/
        MaleStudent.rivalMatch -= Matching;
        MaleStudent.breakHeartGuard -= MatchEnd;
        eyeLaser.EyeLaserOn(false);
        Destroy(this.gameObject);
    }
    private void MatchStartAnim()
    {
        StartCoroutine(MatchAnim());
    }
    private IEnumerator MatchAnim()
    {
        surpriseImg.SetActive(true);
        yield return null;
        surpriseImg.SetActive(false);
        yield return null;
        surpriseImg.SetActive(true);
        yield return null;
        surpriseImg.SetActive(false);
        yield return null;
    }
}
