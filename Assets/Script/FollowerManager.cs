using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
[System.Serializable]
public class AITypeIntPair
{
    public MaleStudent.Type prority;
    [System.NonSerialized]
    public int insertPoint=0;
}
public class FollowerManager : MonoBehaviour
{ 
    [SerializeField] private List<AITypeIntPair> data;  
    private List<MaleStudent> followers;
    
    [SerializeField] private List<MaleStudent.Type> prority;
    private List<List<MaleStudent>> followers2;
    private List<MaleStudent> followerProfessor;
    private List<MaleStudent> followerNamed;
    private List<MaleStudent> followerMob;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        followers = new List<MaleStudent>();

        /*followers2 = new List<List<MaleStudent>>();
        followerProfessor = new List<MaleStudent>();
        followerNamed = new List<MaleStudent>();
        followerMob = new List<MaleStudent>();

        foreach (var p in prority)
        {
            switch(p)
            {
                case MaleStudent.Type.mob:
                    followers2.Add(followerMob);
                    break;
                case MaleStudent.Type.professor:
                    followers2.Add(followerProfessor);
                    break;
                case MaleStudent.Type.named:
                    followers2.Add(followerNamed);
                    break;
            }
        }*/
        MaleStudent.breakHeartGuard += AddFollower;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddFollower(MaleStudent male)
    {
        if (male.currentState!=MaleStudent.State.OwnedByPlayer)
        {
            return;
        } 
        if(followers.Count > 0)
        {
            male.followTarget = followers[followers.Count - 1].transform;
        }
        else
        {
            male.followTarget = transform;
        }
        followers.Add(male);
        /*AITypeIntPair findData = data.Find(t => t.prority == male.type);
        followers.Insert(findData.insertPoint, male); 

        foreach(var f in followers)
        {
            Debug.Log(f.transform.name);
        }*/
        /*if(followerProfessor.Count+followerNamed.Count+followerMob.Count==0 )
        {
            male.followTarget = transform; // player
        }
        else
        {

        }*/
        //int nextMale = 0;
        /*switch(male.type)
        {
            case MaleStudent.Type.mob:

                //nextMale = followerMob.Count;
                male.followTarget = followerMob[0].followTarget;
                followerMob[0].followTarget = male.transform;
                followerMob.Insert(0, male);
                break;
            case MaleStudent.Type.professor:
                if(followerProfessor.Count==0)
                {

                }
                male.followTarget = followerProfessor[0].followTarget;
                followerProfessor[0].followTarget = male.transform;
                followerProfessor.Insert(0, male);
                break;
            case MaleStudent.Type.named:
                if (followerNamed.Count == 0)
                {
                    // 앞 우선순위 의 마지막 인덱스 male을 타겟으로 지정
                    // 우선순위가 가장 높을 경우 플레이어를 타겟으로 지정

                    // 우선순위가 뭔지 어케 알지?
                    int typePrority = GetPrority(MaleStudent.Type.named);
                    if(typePrority == 0)
                    {
                        // prority +1 count 도 0이면?

                        //male.followTarget = followers2[typePrority+1]
                    }
                }
                else
                {
                male.followTarget = followerNamed[0].followTarget;
                followerNamed[0].followTarget = male.transform;

                }
                followerNamed.Insert(0, male);
                break;
            default:
                break;
        }*/

    }
    private int GetPrority(MaleStudent.Type type)
    {
        switch (type)
        {
            case MaleStudent.Type.mob:
                return followers2.FindIndex(x => x == followerMob); 
            case MaleStudent.Type.professor:
                return followers2.FindIndex(x => x == followerProfessor);
            case MaleStudent.Type.named:
                return followers2.FindIndex(x => x == followerNamed);
        }
        return -1;
    }
}
