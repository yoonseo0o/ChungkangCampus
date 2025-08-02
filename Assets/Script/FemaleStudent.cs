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
            //Debug.Log("camera�� ��Ҵ� ");
            MaleStudent.rivalMatch += Matching;
            MaleStudent.rivalMatchEnd += MatchEnd;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("MainCamera"))
        {
            //Debug.Log("camera�� ��Ҵ� ");
            MaleStudent.rivalMatch -= Matching;
            MaleStudent.rivalMatchEnd -= MatchEnd;
        }
    }
    private void Matching(MaleStudent male)
    {
        Debug.Log($"{this.name}�� {male.name}���׼� ���̹� ��� �� !");
        IsMove = false;
    }
    private void MatchEnd(bool IsSuccess)
    {
        if(IsSuccess) Debug.Log($"{this.name}�� ���̹� ��� ���� !");
        else Debug.Log($"{this.name}�� ���̹� ��� ���� !");
        MaleStudent.rivalMatch -= Matching;
        MaleStudent.rivalMatchEnd -= MatchEnd;
        Destroy(this.gameObject);
    }
}
