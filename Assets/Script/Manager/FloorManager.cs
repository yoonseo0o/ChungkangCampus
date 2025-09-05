using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FloorManager : MonoBehaviour
{
    [SerializeField] private List<Transform> floorList;

    public int curFloor {  get; private set; }
    public int maxFloor { get; private set; }
    private Transform playerTrf;
    private float floorPlayerDistance;
    private float cameraPlayerDistance;
    private int changeValue;

    private float boyDefaultSpeed;
    void Start()
    {
        curFloor = 1;
        maxFloor = floorList.Count;
        playerTrf = GameManager.Instance.player.transform;
        floorPlayerDistance = floorList[curFloor - 1].position.y- playerTrf.position.y;
        cameraPlayerDistance = playerTrf.position.y-Camera.main.transform.position.y;
    }
    public void UpDownFloor(int value)
    { 
        if(curFloor == maxFloor&&value >0) 
            return;
        if (curFloor == 1 && value < 0)
            return;
        // 시간 멈추게
        Debug.Log("시간이 멈췄다아");
        GameManager.Instance.IsTimeFlow = false;
        StartCoroutine(GameManager.Instance.UIManager.FadeOut());
        changeValue=value;
        GameManager.Instance.UIManager.fadeComplete += MoveFloor;
    }
    public void MoveFloor()
    {
        GameManager.Instance.UIManager.fadeComplete -= MoveFloor; 
        curFloor += changeValue; 
        playerTrf.position = new Vector3(playerTrf.position.x, floorList[curFloor - 1].position.y- floorPlayerDistance, playerTrf.position.z);
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, playerTrf.position.y - cameraPlayerDistance, Camera.main.transform.position.z);
        // 남학생 옮기고 

        boyDefaultSpeed = GameManager.Instance.FollowerManager.followSpeed;
        GameManager.Instance.FollowerManager.followSpeed = 500f;
        Debug.Log("남학생 옮겼다아");
        StartCoroutine(GameManager.Instance.UIManager.FadeIn());
        GameManager.Instance.UIManager.fadeComplete += CompleteMoveFloor; 
    }
    public void CompleteMoveFloor()
    {
        // 시간 흐르게
        GameManager.Instance.FollowerManager.followSpeed = boyDefaultSpeed;

        GameManager.Instance.IsTimeFlow = true;
        Debug.Log("시간이 흐른다아");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
