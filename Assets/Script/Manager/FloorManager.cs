using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] private List<Transform> floorList;

    public int curFloor {  get; private set; }
    public int maxFloor { get; private set; }
    private Transform playerTrf;
    private float floorPlayerDistance;
    private float cameraPlayerDistance;
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
        curFloor+=value; 
        playerTrf.position = new Vector3(playerTrf.position.x, floorList[curFloor - 1].position.y- floorPlayerDistance, playerTrf.position.z);
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, playerTrf.position.y - cameraPlayerDistance, Camera.main.transform.position.z);

        GameManager.Instance.UIManger.SetActiveFloorButton(true);
    } 


    // Update is called once per frame
    void Update()
    {
        
    }
}
