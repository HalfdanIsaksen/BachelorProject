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
    private float maxForce = 1.0f;
    private float minForce = 0.0f;
    private float maxHitDistance = 0.005f;
    private float minDistance;

    private bool hapticFeedbackTriggered = false;

    private int collidersHitting = 0;
    
    private UpdateTouchedHaptics effect = new UpdateTouchedHaptics();
    private Temperature temperature;
    private Force hapticForce;
    private Vector3 raySpread;

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
            float distance = Vector3.Distance(vrController.position, transform.position) - minDistance;
            if(0.0000000000001f < distance && distance < maxHitDistance)
            {
                hapticForce.Active = true;
                hapticForce.Value = (distance /  maxHitDistance * (maxForce - minForce));
                effect.Set(temperature, hapticForce, WeArtTexture.Default);
                hapticObjectIndex.AddEffect(effect);
                Debug.Log("Distance:" + distance);
                //hapticObjectMiddle.AddEffect(effect);
            }
        }else{
            hapticObjectIndex.RemoveEffect(effect);
            Debug.Log("No efffect");
            //hapticObjectMiddle.RemoveEffect(effect);
        }
    }

    void OnCollisionEnter(Collision col){
        if(col.gameObject.layer != 6){
            minDistance = Vector3.Distance(vrController.position, transform.position);
            Debug.Log("Offset distance:" + minDistance);
            Debug.Log("Colliders touching: " + collidersHitting);
            collidersHitting ++;
            if(!hapticFeedbackTriggered){
                hapticFeedbackTriggered = true;
            }
        }
    }

    void OnCollisionExit(Collision col){
        if(col.gameObject.layer != 6){
            collidersHitting --;
            if(collidersHitting < 1){
                hapticFeedbackTriggered = false;
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