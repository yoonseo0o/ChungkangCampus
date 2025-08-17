using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] private List<Transform> floorList;

    public int curFloor {  get; private set; }
    public int maxFloor { get; private set; }
    private Transform playerTrf;
    private float ���÷��̾���̰���;
    private float ī�޶��÷��̾���̰���;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curFloor = 1;
        maxFloor = floorList.Count;
        playerTrf = GameManager.Instance.player.transform;
        ���÷��̾���̰��� = floorList[curFloor - 1].position.y- playerTrf.position.y;
        ī�޶��÷��̾���̰��� = playerTrf.position.y-Camera.main.transform.position.y;
        Debug.Log($"{curFloor - 1}��° �� {floorList[curFloor - 1].position.y}-{playerTrf.position.y}={���÷��̾���̰���}");
    }
    public void UpFloor()
    {
        if(curFloor == maxFloor) 
        {
            return;
        }
        Debug.Log("up");
        curFloor++; 
        Debug.Log($"{curFloor - 1}��° �� {floorList[curFloor - 1].position.y}-{���÷��̾���̰���}={floorList[curFloor - 1].position.y - ���÷��̾���̰���}");
        playerTrf.position = new Vector3(playerTrf.position.x, floorList[curFloor - 1].position.y- ���÷��̾���̰���, playerTrf.position.z);
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, playerTrf.position.y - ī�޶��÷��̾���̰���, Camera.main.transform.position.z);

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
        playerTrf.position = new Vector3 (playerTrf.position.x, floorList[curFloor - 1].position.y- ���÷��̾���̰���, playerTrf.position.z);
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, playerTrf.position.y - ī�޶��÷��̾���̰���, Camera.main.transform.position.z);

        GameManager.Instance.UIManger.SetActiveFloorButton(true);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
