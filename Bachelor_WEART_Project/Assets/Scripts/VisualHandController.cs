using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
public class VisualHandController : MonoBehaviour
{
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private Transform followTarget;
    [SerializeField]
    private Rigidbody ownRigidBody;
    [SerializeField]
    private float followSpeed;
    [SerializeField]
    private Vector3 positionOffset;
    [SerializeField]
    private Vector3 rotationOffset;
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private float ownMass;
    private SteamVR_TrackedObject VRController;

    [SerializeField]
    private SteamVR_TrackedObject.EIndex index;

    // Start is called before the first frame update
    void Start()
    {
        followTarget = target.transform;
        ownRigidBody = GetComponent<Rigidbody>();
        ownRigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        ownRigidBody.interpolation = RigidbodyInterpolation.Interpolate;
        ownRigidBody.mass = ownMass;

        ownRigidBody.position = followTarget.position;
        ownRigidBody.rotation = followTarget.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        FollowTracker();
    }

    private void FollowTracker()
    {
        //position tracking
        var positionWithOffset = followTarget.position + positionOffset;
        var distance = Vector3.Distance(positionWithOffset, transform.position);
        ownRigidBody.velocity = (positionWithOffset - transform.position).normalized * (followSpeed * distance) * Time.deltaTime;

        //Rotation tracking
        var rotationWithOffset = followTarget.rotation * Quaternion.Euler(rotationOffset);
        var differenceRotation = rotationWithOffset * Quaternion.Inverse(ownRigidBody.rotation);
        differenceRotation.ToAngleAxis(out float angle, out Vector3 axis);
        ownRigidBody.angularVelocity = axis * (angle *Mathf.Deg2Rad * rotateSpeed) * Time.deltaTime;
    }

}
