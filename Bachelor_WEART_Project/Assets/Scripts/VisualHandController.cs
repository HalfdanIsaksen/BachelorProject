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

    void FixedUpdate(){
        FollowTracker();
    }
    private void FollowTracker()
    {
        //position tracking
        var positionWithOffset = followTarget.position + positionOffset;
        var distance = Vector3.Distance(positionWithOffset, ownRigidBody.position);
        ownRigidBody.velocity = (positionWithOffset - ownRigidBody.position).normalized * (followSpeed * distance) * Time.fixedDeltaTime;

        //Rotation tracking, but it makes a 360 everytime rotation is done
        var rotationWithOffset = followTarget.rotation * Quaternion.Euler(rotationOffset);
        var differenceRotation = rotationWithOffset * Quaternion.Inverse(ownRigidBody.rotation);
        differenceRotation.ToAngleAxis(out float angle, out Vector3 axis);
        
        if(Mathf.Abs(axis.magnitude) != Mathf.Infinity){
            if(angle > 180.0f)
            {
                angle -= 360.0f;
            }
            ownRigidBody.angularVelocity = axis * (angle * Mathf.Deg2Rad * rotateSpeed) * Time.fixedDeltaTime;

        }
    }
}
