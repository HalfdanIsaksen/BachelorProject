using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeArt.Components;
using WeArt.Core;
using System;
using WeArtTexture = WeArt.Core.Texture;

public class HapticController : MonoBehaviour
{
    private WeArtTouchableObject objectController;
    [SerializeField]
    private WeArtHapticObject hapticObjectThumb;
    [SerializeField]
    private WeArtHapticObject hapticObjectIndex;
    [SerializeField]
    private WeArtHapticObject hapticObjectMiddle;

    //Variable for control of force pressure
    private float maxForce = 0.75f;
    private float minForce = 0.0f;
    [SerializeField]
    private float maxHitDistance = 0.05f;
    private float minDistance;

    private bool hapticFeedbackTriggered = false;

    private int collidersHitting = 0;
    
    private UpdateTouchedHaptics effect;
    private Temperature temperature;
    private Force hapticForce;

    private Vector3 raySpread;

    private float vrControllerCollidingPosition;

    private Vector3 indexThimbleCollidingPosition;

    private bool vrControllerCollidingPositionIsSet = false;

    [SerializeField]
    private Transform vrController;
    // Start is called before the first frame update
    void Start()
    {
        /*temperature = new Temperature();
        hapticForce = new Force();
        raySpread = Vector3.up;


        temperature.Active = true;
        temperature.Value = 0.1f;
        effect.Set(temperature, hapticForce, WeArtTexture.Default);
        hapticObjectIndex.AddEffect(effect);*/

        //var effect = new UpdateTouchedHaptics();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate(){  
        HapticFeedbackForce();  
    }

    private void HapticFeedbackForce(){
        var effect = new UpdateTouchedHaptics();

        if(hapticFeedbackTriggered){
           
            float distance = (vrControllerCollidingPosition - vrController.position.y);
            //float distance = Vector3.Distance(indexThimbleCollidingPosition, hapticObjectIndex.transform.position);
            
            Debug.DrawLine(vrController.position, transform.position, new Color(1.0f, 0.0f, 0.0f));
   
            //if(0.0000000000001f < distance && distance < maxHitDistance)
            if(distance > 0.0f)
            {
                //Debug.Log("Distance: " + distance);

                //Debug.Log("ratio: " + distance/maxHitDistance);

                hapticForce.Active = true;
                
                //hapticForce.Value = (distance / maxHitDistance * (maxForce - minForce));

                //lort
                hapticForce.Value = ((distance/maxHitDistance) * (distance/maxHitDistance));

                //hapticForce.Value = Mathf.Clamp(hapticForce.Value, minForce, maxForce);
                    
                effect.Set(temperature, hapticForce, WeArtTexture.Default);
                hapticObjectIndex.AddEffect(effect);
                //hapticObjectMiddle.AddEffect(effect);
            }else{
      
            hapticForce.Value = minForce;
            effect.Set(temperature, Force.Default, WeArtTexture.Default);
            hapticObjectIndex.AddEffect(effect);
            //hapticObjectIndex.RemoveEffect(effect);
            hapticFeedbackTriggered = false;
            ///Debug.Log("No efffect");
            //hapticObjectMiddle.RemoveEffect(effect);
            }
        }
    }

    void OnCollisionEnter(Collision col){

        
        if(col.gameObject.layer != 6){

            if(col.gameObject.tag == "largeGlassSection"){
                //Debug.Log("Large glass Section");
                collidersHitting++;

                
                foreach(ContactPoint contact in col.contacts){

                    var colName = contact.thisCollider.name;

                    if(colName == "Index_collider" && !vrControllerCollidingPositionIsSet){
                        Debug.Log("Index finger collides");

                        //Debug.Log("Index position: " + hapticObjectIndex.transform.position);

                        //indexThimbleCollidingPosition = hapticObjectIndex.transform.position;

                        vrControllerCollidingPosition = vrController.transform.position.y;
                        vrControllerCollidingPositionIsSet = true;

                        //Debug.Log("Position of VR Controller at collision" + vrControllerCollidingPosition); 

                        //minDistance = Vector3.Distance(vrControllerCollidingPosition, transform.position); 
                        
                        if(!hapticFeedbackTriggered){
                            hapticFeedbackTriggered = true;
                        } 
                    }
                }

            }
            

            //minDistance = Vector3.Distance(vrController.position, transform.position);
            //Debug.Log("Offset distance:" + minDistance);
            //Debug.Log("Colliders touching: " + collidersHitting);
            //collidersHitting ++;
            // if(!hapticFeedbackTriggered){
            //     hapticFeedbackTriggered = true;
            // }
        }
    }

    void OnCollisionExit(Collision col){
        if(col.gameObject.layer != 6){

            // collidersHitting --;
            // if(collidersHitting < 1){
            //     hapticFeedbackTriggered = false;
            // }
            

            if(col.gameObject.tag == "largeGlassSection"){
                Debug.Log("Some collider exits");
                vrControllerCollidingPositionIsSet = false;
            //hapticFeedbackTriggered = false;
            }

        }

    }

    internal class UpdateTouchedHaptics : IWeArtEffect
    {
       
        public event Action OnUpdate;

        // Gets the Temperature.
        public Temperature Temperature { get; private set; } = Temperature.Default;

        // Gets the Force.
        public Force Force { get; private set; } = Force.Default;


        //  Gets the Texture.
        public WeArtTexture Texture { get; private set; } = WeArtTexture.Default;

        public void Set(Temperature temperature, Force force, WeArtTexture texture)
            {
                // Need to clone these, or the internal arrays will point to the same data
                force = (Force)force.Clone();
                texture = (WeArtTexture)texture.Clone();


                bool changed = false;

                // Temperature
                changed |= !Temperature.Equals(temperature);
                Temperature = temperature;

                // Force
                changed |= !Force.Equals(force);
                Force = force;

                // Texture
                /*if (lastImpactInfo != null && impactInfo != null)
                {
                    float dx = Vector3.Distance(impactInfo.Position, lastImpactInfo.Position);
                    float dt = Mathf.Max(Mathf.Epsilon, impactInfo.Time - lastImpactInfo.Time);
                    float slidingSpeed = impactInfo.Multiplier * (dx / dt);
                    texture.VelocityZ += slidingSpeed;
                }
                lastImpactInfo = impactInfo;

                changed |= !Texture.Equals(texture);
                Texture = texture;*/

                if (changed)
                    OnUpdate?.Invoke();
            }
    }
}