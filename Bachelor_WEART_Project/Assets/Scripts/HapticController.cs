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


    private float maxForce = 1.0f;
    private float minForce = 0.0f;
    private float maxHitDistance = 0.02f;
    
    private UpdateTouchedHaptics effect = new UpdateTouchedHaptics();
    private Temperature temperature;
    private Force hapticForce;
    private Vector3 raySpread;
    // Start is called before the first frame update
    void Start()
    {

        temperature = new Temperature();
        hapticForce = new Force();
        raySpread = Vector3.up;


        temperature.Active = true;
        temperature.Value = 0.1f;
        effect.Set(temperature, hapticForce, WeArtTexture.Default);
        hapticObjectIndex.AddEffect(effect);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate(){
        int layerMask = 1 << 6;

        layerMask = ~layerMask;
        
        RaycastHit hit;
        
        if(Physics.Raycast(transform.position, transform.TransformDirection(raySpread), out hit, 1, layerMask)){
            Debug.DrawRay(transform.position, transform.TransformDirection(raySpread) * hit.distance, Color.yellow);
            var effect = new UpdateTouchedHaptics();

            if(hit.distance < maxHitDistance){
                Debug.Log("Apply force");
                temperature.Active = true;
                temperature.Value = 0;
                
                hapticForce.Active = true;
                hapticForce.Value = 1 - (hit.distance / maxHitDistance * maxForce - minForce);
                effect.Set(temperature, hapticForce, WeArtTexture.Default);
                hapticObjectIndex.AddEffect(effect);
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