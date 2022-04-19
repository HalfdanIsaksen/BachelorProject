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
    [SerializeField]
    private bool indexHapticFeedbackTriggered = false;
    [SerializeField]
    private bool middleHapticFeedbackTriggered = false;
    private int collidersHitting = 0;
    
    private UpdateTouchedHaptics effect;
    private Temperature temperature;
    private Force hapticForce;
    private float vrControllerCollidingPosition;
    private Vector3 indexThimbleCollidingPosition;
    private bool vrControllerCollidingPositionIsSet = false;

    [SerializeField]
    private Transform vrController;
    // Start is called before the first frame update

    void FixedUpdate(){  
        HapticFeedbackForce();  
    }

    private void HapticFeedbackForce(){
        var effect = new UpdateTouchedHaptics();

        if(indexHapticFeedbackTriggered || middleHapticFeedbackTriggered){ 
            float distance = (vrControllerCollidingPosition - vrController.position.y);
            //float distance = Vector3.Distance(indexThimbleCollidingPosition, hapticObjectIndex.transform.position);
            
            //Debug.DrawLine(vrController.position, transform.position, new Color(1.0f, 0.0f, 0.0f));
   
            if(distance > 0.0f)
            {

                hapticForce.Active = true;
                
                //hapticForce.Value = (distance / maxHitDistance * (maxForce - minForce));

                hapticForce.Value = ((distance/maxHitDistance) * (distance/maxHitDistance));
                //Debug.LogFormat("Haptic force: {0}, Distance Ratio: {1}", hapticForce.Value, (distance/maxHitDistance));

                //hapticForce.Value = Mathf.Clamp(hapticForce.Value, minForce, maxForce);
                    
                effect.Set(temperature, hapticForce, WeArtTexture.Default);
                if(indexHapticFeedbackTriggered){
                    hapticObjectIndex.AddEffect(effect);
                }
                if(middleHapticFeedbackTriggered){
                    hapticObjectMiddle.AddEffect(effect);
                }

            }else{
      
            hapticForce.Value = minForce;
            effect.Set(temperature, Force.Default, WeArtTexture.Default);
            hapticObjectIndex.AddEffect(effect);
            hapticObjectMiddle.AddEffect(effect);
            indexHapticFeedbackTriggered = false;
            middleHapticFeedbackTriggered = false;
            }
        }
    }
    private void HapticFeedbackTempurature(){        
        temperature = new Temperature();
        hapticForce = new Force();

        temperature.Active = true;
        temperature.Value = 0.1f;
        effect.Set(temperature, hapticForce, WeArtTexture.Default);
        hapticObjectIndex.AddEffect(effect);
    }

    public float GetForceValue{
        get => hapticForce.Value;
    }

    public float GetTemperatureValue{
        get => temperature.Value;
    }

    void OnCollisionEnter(Collision col){
        if(col.gameObject.layer != 6){
            if(col.gameObject.tag == "largeGlassSection"){
                //Debug.Log("Large glass Section");
                collidersHitting++;

                foreach(ContactPoint contact in col.contacts){

                    var colName = contact.thisCollider.name;

                    if(colName == "Index_collider" && !vrControllerCollidingPositionIsSet){

                        //hapticIndexFinger = true;

                        //indexThimbleCollidingPosition = hapticObjectIndex.transform.position;

                        vrControllerCollidingPosition = vrController.transform.position.y;
                        vrControllerCollidingPositionIsSet = true;

                        //minDistance = Vector3.Distance(vrControllerCollidingPosition, transform.position); 
                        
                        if(!indexHapticFeedbackTriggered && colName == "Index_collider"){
                            indexHapticFeedbackTriggered = true;
                        }
                    }
                    if(colName == "Middle_collider" && !vrControllerCollidingPositionIsSet){

                        vrControllerCollidingPosition = vrController.transform.position.y;
                        vrControllerCollidingPositionIsSet = true;

                        if(!middleHapticFeedbackTriggered && colName == "Middle_collider"){
                            middleHapticFeedbackTriggered = true;
                        } 
                    }
                }
            }
        }
    }

    void OnCollisionStay(Collision col){
        foreach(ContactPoint contact in col.contacts){
            var colName = contact.thisCollider.name;
            if(colName == "Index_collider"){
                indexHapticFeedbackTriggered = true;
            }
            if(colName == "Middle_collider"){
                middleHapticFeedbackTriggered = true;
            }
        }
    }


    void OnCollisionExit(Collision col){
        if(col.gameObject.layer != 6){
            if(col.gameObject.tag == "largeGlassSection"){
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