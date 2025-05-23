using UnityEngine;

public class GestureAgentController : MonoBehaviour
{
    public OVRHand leftHand;
    public OVRSkeleton leftSkeleton;
    public GameObject agent; 
    public float moveSpeed = 1f;

    public Animator agentAnimator;

    void Start()
    {
        if (agent != null)
        {
            agentAnimator = agent.GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            Vector3 indexTip = leftSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_IndexTip].Transform.position;
            Vector3 handCenter = leftSkeleton.transform.position;
            Vector3 direction = (indexTip - handCenter).normalized;

            float dotLeft = Vector3.Dot(direction, Vector3.left);
            float dotRight = Vector3.Dot(direction, Vector3.right);
            float dotForward = Vector3.Dot(direction, Vector3.forward);

            // Check for left
            if (dotLeft > 0.7f)
            {
                TryMoveAgent(Vector3.left);
            }
            // Check for right
            else if (dotRight > 0.7f)
            {
                TryMoveAgent(Vector3.right);
            }
            // Check for forward
            else if (dotForward > 0.7f)
            {
                TryMoveAgent(Vector3.forward);
            }
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
}
