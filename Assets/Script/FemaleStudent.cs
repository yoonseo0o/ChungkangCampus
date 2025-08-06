using UnityEngine;

public class FemaleStudent : AI
{
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
            //Debug.Log("camera에 닿았다 ");
            MaleStudent.rivalMatch += Matching;
            MaleStudent.breakHeartGuard += MatchEnd;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("MainCamera"))
        {
            //Debug.Log("camera에 닿았다 ");
            MaleStudent.rivalMatch -= Matching;
            MaleStudent.breakHeartGuard -= MatchEnd;
        }
    }
    private void Matching(MaleStudent male)
    {
        Debug.Log($"{this.name}가 {male.name}한테서 라이벌 대결 중 !");
        IsMove = false;
    }
    private void MatchEnd(MaleStudent male)
    {
        if(male.currentState == MaleStudent.State.OwnedByRival) 
            Debug.Log($"{this.name}가 라이벌 대결 성공 !");
        else Debug.Log($"{this.name}가 라이벌 대결 실패 !");
        MaleStudent.rivalMatch -= Matching;
        MaleStudent.breakHeartGuard -= MatchEnd;
        Destroy(this.gameObject);
    }
}
