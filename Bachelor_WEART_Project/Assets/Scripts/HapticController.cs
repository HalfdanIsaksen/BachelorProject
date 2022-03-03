using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeArt.Components;
using WeArt.Core;

public class HapticController : MonoBehaviour
{
    private WeArtTouchableObject objectController;
    private WeArtHapticObject hapticObject;
    private Temperature temperature;

    private float maxForce = 1.0f;
    private float minForce = 0.0f;
    private float maxHitDistance = 0.2f;

    private Force hapticForce;
    private Vector3 raySpread;
    // Start is called before the first frame update
    void Start()
    {
        raySpread = Quaternion.Euler(0, 0, 45) * Vector3.forward;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate(){
        int layerMask = 1 << 6;

        layerMask = ~layerMask;

        RaycastHit hit;
        for(int i = 0; i < 10; i++){
            if(Physics.Raycast(transform.position, transform.TransformDirection(Quaternion.Euler(0, 0, 9 * i) * raySpread), out hit, 1, layerMask)){

                Debug.DrawRay(transform.position, transform.TransformDirection(Quaternion.Euler(0, 0, 9 * i) * raySpread) * hit.distance, Color.yellow);
                Debug.Log("ray: " + i);
                objectController = hit.collider.gameObject.GetComponent<WeArtTouchableObject>();
                if(hit.distance < maxHitDistance){

                    temperature.Value = 0;
                    temperature.Active = true;

                    hapticForce.Value = hit.distance / maxHitDistance * maxForce - minForce;
                    hapticForce.Active = true;

                    //objectController.Temperature = temperature;
                    objectController.Stiffness = hapticForce;
                    Debug.Log(objectController.Temperature);
                }

            }
        }
    }
}
