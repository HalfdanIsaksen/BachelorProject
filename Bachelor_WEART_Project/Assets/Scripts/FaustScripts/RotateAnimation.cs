using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAnimation : MonoBehaviour
{
    public float forceValue;

    public float rotationVelocity = 0.001f;
    private float rotationDegrees;
    // Start is called before the first frame update

    private HapticController hapticController;

    void start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //bowlRotation();
        //if (hapticController != null)
        //{
            bowlRotation();
       // }
    }

    public void bowlRotation()
    {
        // if(hapticController.GetForceValue >= 0.90f){
        //     rotationVelocity = 0.0f;
        // }else{
        //     rotationVelocity = hapticController.GetForceValue * Time.deltaTime;
        //     rotationDegrees = 100.0f;
        // }
        //rotationVelocity = hapticController.GetForceValue * Time.deltaTime;
        
        rotationDegrees = 100.0f;

        transform.Rotate(new Vector3(0f, rotationDegrees, 0f) * rotationVelocity * Time.deltaTime);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == 6)
        {
            hapticController = col.gameObject.GetComponentInParent<HapticController>();
        }
    }

}
