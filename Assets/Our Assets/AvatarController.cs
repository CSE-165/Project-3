using UnityEngine;
using System.Collections.Generic;
public class GestureAgentController : MonoBehaviour

{
    [Header("Reference Objects")]
    public OVRHand leftHand;
    public OVRSkeleton leftSkeleton;
    public GameObject agent;

    private Vector3 initialPos;
    private bool isDragging = false;
    public Animator agentAnimator;
   
    List<OVRBone> bones;

    [Header("Ray Settings")]
    private LineRenderer lineRenderer;
    public Color rayColor = Color.blue;
    public float rayWidth = 0.01f;

    
    [Header("Other")]
    private bool firstPinch = true;
    public GameObject anchorPrefab;

    public GameObject camera;

    void Start()
    {
        if (agent != null)
        {
            agentAnimator = agent.GetComponent<Animator>();
        }

        bones = new List<OVRBone>(leftSkeleton.Bones);

        GameObject rayObject = new GameObject("PinchRay");
        rayObject.transform.parent = this.transform;

        lineRenderer = rayObject.AddComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 2;

        // Set LineRenderer properties
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = rayColor;
        lineRenderer.endColor = rayColor;
        lineRenderer.startWidth = rayWidth;
        lineRenderer.endWidth = rayWidth;
        lineRenderer.useWorldSpace = true;
    }

    void Update()
    {

        bool isPinching = leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index);
        bool isPinchingMiddle = leftHand.GetFingerIsPinching(OVRHand.HandFinger.Middle);

        if (isPinching)
        {
            if (!isDragging)
            {
                initialPos = leftHand.PointerPose.position;
                isDragging = true;
            }
            else
            {
                ShowRay(initialPos, leftHand.PointerPose.position);
            }
        }
        else if (!isPinching)
        {
            HideRay();
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 dragVector = leftHand.PointerPose.position - initialPos;
            dragVector.y = 0f; // Ignore vertical movement

            TryMoveAgent(dragVector, dragVector.magnitude);

        }

        if (isPinchingMiddle)
        {
            if (firstPinch)
            {
                //place anchor
                Vector3 anchorPosition = leftHand.PointerPose.position;
                Quaternion fullRotation = camera.transform.rotation;

                // Extract the Y (yaw) angle only
                float yRotation = fullRotation.eulerAngles.y;

                // Create a new rotation with only the Y component
                Quaternion yOnlyRotation = Quaternion.Euler(0, yRotation, 0);
                Instantiate(anchorPrefab, anchorPosition, yOnlyRotation);
            }
            
            firstPinch = false;
        }
        else
        {
            firstPinch = true;
        }
    }

    void ShowRay(Vector3 start, Vector3 end)
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    void HideRay()
    {
        lineRenderer.enabled = false;
    }

    void TryMoveAgent(Vector3 direction, float moveSpeed)
    {
        Vector3 targetPos = agent.transform.position + direction.normalized * 2 * moveSpeed * Time.deltaTime;

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
    
    // public void SetWalkingAnimation(bool isWalking)
    // {
    //     if (isWalking)
    //     {
    //         agentAnimator.SetBool("Walking", true);
    //         agentAnimator.SetBool("Standing Idle", false);
    //     }
    //     else
    //     {
    //         agentAnimator.SetBool("Walking", false);
    //         agentAnimator.SetBool("Standing Idle", true);
    //     }
    // }
}
