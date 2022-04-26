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
    
    private UpdateTouchedHaptics effect;
    private Temperature temperature;
    private Force hapticForce;
    [SerializeField]
    private WeArtTexture weArtTexture;
    private float vrControllerCollidingPosition;
    private Vector3 indexThimbleCollidingPosition;
    private bool vrControllerCollidingPositionIsSet = false;

    [SerializeField]
    private Transform vrController;

    private Transform hapticObjectCollider;
    // Start is called before the first frame update

    void Start(){

    }
    void FixedUpdate(){          
        //HapticFeedbackForce(); 
        //HapticFeedbackTempurature();    
        HapticFeedbackTexture();
    }
    
    void OnCollisionEnter(Collision col){
        //ActivateHapticFeedback(col);
        if(col.gameObject.layer == 7 && this.gameObject.layer == 6){
            if(col.gameObject.tag == "Instrument"){
                foreach(ContactPoint contact in col.contacts){

                    var colName = contact.thisCollider.name;

                    if(colName == "Index_collider" && !vrControllerCollidingPositionIsSet){

                        vrControllerCollidingPosition = vrController.transform.position.y;
                        vrControllerCollidingPositionIsSet = true;
                        
                        if(!indexHapticFeedbackTriggered && colName == "Index_collider"){
                            indexHapticFeedbackTriggered = true;
                            Debug.Log("Index haptic true");
                        }
                    }
                    if(colName == "Middle_collider" && !vrControllerCollidingPositionIsSet){

                        vrControllerCollidingPosition = vrController.transform.position.y;
                        vrControllerCollidingPositionIsSet = true;

                        if(!middleHapticFeedbackTriggered && colName == "Middle_collider"){
                            middleHapticFeedbackTriggered = true;
                            Debug.Log("Middle haptic true");
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
                hapticObjectCollider = col.gameObject.transform;
                indexHapticFeedbackTriggered = true;
            }
            if(colName == "Middle_collider"){
                middleHapticFeedbackTriggered = true;
            }
        }
    }

    private void HapticFeedbackForce(){
        var effect = new UpdateTouchedHaptics();
        if(indexHapticFeedbackTriggered || middleHapticFeedbackTriggered){ 
            float distance = (vrControllerCollidingPosition - vrController.position.y);
            
            if(distance > 0.0f)
            {
                hapticForce.Active = true;

                hapticForce.Value = ((distance/maxHitDistance) * (distance/maxHitDistance));
                //Debug.LogFormat("Haptic force: {0}, Distance Ratio: {1}", hapticForce.Value, (distance/maxHitDistance));
                    
                effect.Set(temperature, hapticForce, weArtTexture, new UpdateTouchedHaptics.ImpactInfo(){
                    Position = hapticObjectCollider.position,
                    Time = Time.time,
                    Multiplier = WeArtConstants.defaultCollisionMultiplier
                });
                if(indexHapticFeedbackTriggered){
                    hapticObjectIndex.AddEffect(effect);
                }
                if(middleHapticFeedbackTriggered){
                    hapticObjectMiddle.AddEffect(effect);
                }

            }else{
      
            hapticForce.Value = minForce;
            effect.Set(temperature, Force.Default, weArtTexture, new UpdateTouchedHaptics.ImpactInfo(){
                Position = hapticObjectCollider.position,
                Time = Time.time,
                Multiplier = WeArtConstants.defaultCollisionMultiplier
            });

            hapticObjectIndex.AddEffect(effect);
            hapticObjectMiddle.AddEffect(effect);
            indexHapticFeedbackTriggered = false;
            middleHapticFeedbackTriggered = false;
            }
        }
    }
    private void HapticFeedbackTempurature(){
        var effect = new UpdateTouchedHaptics();  

        temperature = new Temperature();

        temperature.Active = true;
        temperature.Value = 0.0f;

        effect.Set(temperature, Force.Default, weArtTexture, new UpdateTouchedHaptics.ImpactInfo(){
                Position = hapticObjectCollider.position,
                Time = Time.time,
                Multiplier = WeArtConstants.defaultCollisionMultiplier
            });
        hapticObjectIndex.AddEffect(effect);
    }
    private void HapticFeedbackTexture(){
        var effect = new UpdateTouchedHaptics();

        weArtTexture.Active = true;
        weArtTexture.TextureType = TextureType.Cotton;
        weArtTexture.Volume = 100;
        weArtTexture.VelocityX = 0.0f;
        weArtTexture.VelocityY = 1.0f;
        weArtTexture.VelocityZ = 0.5f;

        effect.Set(temperature, Force.Default, weArtTexture, new UpdateTouchedHaptics.ImpactInfo(){
                Position = hapticObjectCollider.position,
                Time = Time.time,
                Multiplier = 100
            });
        hapticObjectIndex.AddEffect(effect);
        //Debug.LogFormat("Texture: {0}, ", weArtTexture.TextureType);
    }


    public float GetForceValue{
        get => hapticForce.Value;
    }

    public float GetTemperatureValue{
        get => temperature.Value;
    }

    private void ActivateHapticFeedback(Collision col){
        if(col.gameObject.layer == 7 && this.gameObject.layer == 6){
            if(col.gameObject.tag == "Instrument"){
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

    internal class UpdateTouchedHaptics : IWeArtEffect
    {
        private ImpactInfo lastImpactInfo;
        public event Action OnUpdate;
        // Gets the Temperature.
        public Temperature Temperature { get; private set; } = Temperature.Default;

        // Gets the Force.
        public Force Force { get; private set; } = Force.Default;

        //  Gets the Texture.
        public WeArtTexture Texture { get; private set; } = WeArtTexture.Default;


        public void Set(Temperature temperature, Force force, WeArtTexture texture, ImpactInfo impactInfo)
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
            if (lastImpactInfo != null && impactInfo != null)
            {
                float dx = Vector3.Distance(impactInfo.Position, lastImpactInfo.Position);
                float dt = Mathf.Max(Mathf.Epsilon, impactInfo.Time - lastImpactInfo.Time);
                float slidingSpeed = impactInfo.Multiplier * (dx / dt);
                texture.VelocityZ += slidingSpeed;
            }
            lastImpactInfo = impactInfo;

            changed |= !Texture.Equals(texture);
            Texture = texture;

            if (changed)
                OnUpdate?.Invoke();
        }

        internal class ImpactInfo
        {
            #region Fields

            /// <summary>
            /// Defines the Position.
            /// </summary>
            public Vector3 Position;

            /// <summary>
            /// Defines the Time.
            /// </summary>
            public float Time;

            /// <summary>
            /// Defines the Multiplier.
            /// </summary>
            public float Multiplier;

            #endregion
        }
    }    
}