using System.Collections.Generic;
using UnityEngine;
public class FollowerManager : MonoBehaviour
{ 
    private List<MaleStudent> followers;
    
    public List<MaleStudent> followerNamed;
    public List<MaleStudent> followerMob;
    public float followDistance = -2.5f;
    public float followSpeed = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        followers = new List<MaleStudent>();

        followerNamed = new List<MaleStudent>();
        followerMob = new List<MaleStudent>();

        MaleStudent.breakHeartGuard += AddFollower;
    }

    public void AddFollower(MaleStudent male)
    {
        if (male.currentState!=MaleStudent.State.OwnedByPlayer)
        {
            return;
        } /*
        if(followers.Count > 0)
        {
            male.followTarget = followers[followers.Count - 1].transform;
        }
        else
        {
            male.followTarget = transform;
        }
        followers.Add(male);*/
        switch(male.type)
        {
            case MaleStudent.Type.named:
                if (followerNamed.Count == 0)
                {
                    if (followerMob.Count == 0)
                    {
                        male.followTarget = GameManager.Instance.player.transform;
                    }
                    else
                    {
                        male.followTarget = followerMob[0].followTarget;
                        followerMob[0].followTarget= male.transform;
                    }
                }
                else
                {
                    male.followTarget = followerNamed[0].followTarget;
                    followerNamed[0].followTarget = male.transform;
                }
                followerNamed.Insert(0, male);
                break;
            case MaleStudent.Type.mob:
                if(followerMob.Count==0)
                {
                    if(followerNamed.Count==0)
                    {
                        male.followTarget = GameManager.Instance.player.transform;
                    }
                    else
                    {
                        male.followTarget = followerNamed[followerNamed.Count-1].transform;
                    }
                }
                else
                {
                    male.followTarget = followerMob[0].followTarget;
                    followerMob[0].followTarget = male.transform;
                }
                followerMob.Insert(0, male);
                break;
        }
    }
}
