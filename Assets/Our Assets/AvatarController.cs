using UnityEngine;
using System.Collections.Generic;
public class GestureAgentController : MonoBehaviour
{
    public OVRHand leftHand;
    public OVRSkeleton leftSkeleton;
    public GameObject agent;
    public float moveSpeed = 1f;

    public Animator agentAnimator;
    List<OVRBone> bones;
    void Start()
    {
        if (agent != null)
        {
            agentAnimator = agent.GetComponent<Animator>();
        }

        bones = new List<OVRBone>(leftSkeleton.Bones);
    }

    void Update()
    {
        if (leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            
            Transform tip = GetBoneTransform(OVRSkeleton.BoneId.Hand_IndexTip, bones);
            Transform baseBone = GetBoneTransform(OVRSkeleton.BoneId.Hand_Index1, bones);
            Vector3 direction = (tip.position - baseBone.position).normalized;
            direction.y = 0; // Ignore vertical movement

            TryMoveAgent(direction);
        }
    }

    void TryMoveAgent(Vector3 direction)
    {
        Vector3 targetPos = agent.transform.position + direction * moveSpeed * Time.deltaTime;

        // Raycast to detect obstacles
        if (!Physics.Raycast(agent.transform.position, direction, 0.5f))
        {
            agent.transform.position = targetPos;
        }
        else
        {
            Debug.Log("Obstacle detected, can't move agent.");
        }
    }
    
    Transform GetBoneTransform(OVRSkeleton.BoneId boneId, List<OVRBone> bones)
    {
        foreach (var bone in bones)
        {
            if (bone.Id == boneId)
                return bone.Transform;
        }
        return null;
    }
}
