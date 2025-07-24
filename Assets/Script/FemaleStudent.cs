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
}
