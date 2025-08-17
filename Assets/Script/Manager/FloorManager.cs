using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] private List<Transform> floorList;

    public int curFloor {  get; private set; }
    public int maxFloor { get; private set; }
    private Transform playerTrf;
    private float 층플레이어사이간격;
    private float 카메라플레이어사이간격;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curFloor = 1;
        maxFloor = floorList.Count;
        playerTrf = GameManager.Instance.player.transform;
        층플레이어사이간격 = floorList[curFloor - 1].position.y- playerTrf.position.y;
        카메라플레이어사이간격 = playerTrf.position.y-Camera.main.transform.position.y;
        Debug.Log($"{curFloor - 1}번째 층 {floorList[curFloor - 1].position.y}-{playerTrf.position.y}={층플레이어사이간격}");
    }
    public void UpFloor()
    {
        if(curFloor == maxFloor) 
        {
            return;
        }
        Debug.Log("up");
        curFloor++; 
        Debug.Log($"{curFloor - 1}번째 층 {floorList[curFloor - 1].position.y}-{층플레이어사이간격}={floorList[curFloor - 1].position.y - 층플레이어사이간격}");
        playerTrf.position = new Vector3(playerTrf.position.x, floorList[curFloor - 1].position.y- 층플레이어사이간격, playerTrf.position.z);
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, playerTrf.position.y - 카메라플레이어사이간격, Camera.main.transform.position.z);

        GameManager.Instance.UIManger.SetActiveFloorButton(true);
    }
    public void DownFloor()
    {
        if (curFloor == 1)
        {
            return;
        }
        Debug.Log("down");
        curFloor--;
        playerTrf.position = new Vector3 (playerTrf.position.x, floorList[curFloor - 1].position.y- 층플레이어사이간격, playerTrf.position.z);
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, playerTrf.position.y - 카메라플레이어사이간격, Camera.main.transform.position.z);

        GameManager.Instance.UIManger.SetActiveFloorButton(true);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
