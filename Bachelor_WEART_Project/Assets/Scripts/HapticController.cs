using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeArt.Components;
using WeArt.Core;
using System;
using WeArtTexture = WeArt.Core.Texture;

public class HapticController : MonoBehaviour
{
    [SerializeField]
    private WeArtHapticObject hapticObjectThumb;
    [SerializeField]
    private WeArtHapticObject hapticObjectIndex;
    [SerializeField]
    private WeArtHapticObject hapticObjectMiddle;

    //Variable for control of force pressure
    private float minForce = 0.0f;
    [SerializeField]
    private float maxHitDistance = 0.05f;
    [SerializeField]
    private float minDistance;
    [SerializeField]
    private bool indexHapticFeedbackTriggered = false;
    [SerializeField]
    private bool middleHapticFeedbackTriggered = false;
    
    private UpdateTouchedHaptics effect;

    private Temperature temperature = Temperature.Default;
    private Force hapticForce = Force.Default;
    [SerializeField]
    private WeArtTexture weArtTexture = WeArtTexture.Default;
    private float vrControllerCollidingPosition;
    private bool vrControllerCollidingPositionIsSet = false;

    [SerializeField]
    private Transform vrController;

    // Start is called before the first frame update

    void Start(){
        //HapticFeedbackTempurature();
    }
    void FixedUpdate(){
        HapticFeedbackTempurature();
        //HapticFeedbackForce(); 
        HapticFeedbackTexture();
    }
    
    void OnApplicationQuit(){
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
                            //HapticFeedbackTexture();
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
        if(col.gameObject.layer == 7 && this.gameObject.layer == 6){
            if(col.gameObject.tag == "Instrument"){
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
        }
    }


    private void HapticFeedbackForce(){
        effect = new UpdateTouchedHaptics();

        if(indexHapticFeedbackTriggered || middleHapticFeedbackTriggered){
            
            float distance = (vrControllerCollidingPosition - vrController.position.y);
          
            if(distance > minDistance)
            {
                hapticForce.Active = true;

                hapticForce.Value = ((distance/maxHitDistance) * (distance/maxHitDistance));
                //Debug.LogFormat("Haptic force: {0}, Distance Ratio: {1}", hapticForce.Value, (distance/maxHitDistance));
                    
                effect.Set(temperature, hapticForce, weArtTexture);

                if(indexHapticFeedbackTriggered){
                    hapticObjectIndex.AddEffect(effect);
                }
                if(middleHapticFeedbackTriggered){
                   hapticObjectMiddle.AddEffect(effect);
                }

            }else{ //if distance is less than 0
                
                hapticForce.Value = minForce;
                effect.Set(temperature, Force.Default, weArtTexture);

                
                hapticObjectIndex.AddEffect(effect);
                hapticObjectMiddle.AddEffect(effect);

                indexHapticFeedbackTriggered = false;
                middleHapticFeedbackTriggered = false;
                Debug.Log("Inde i haptic force feedback" + indexHapticFeedbackTriggered);
            }
        }
    }
    private void HapticFeedbackTempurature(){
        effect = new UpdateTouchedHaptics();

        temperature.Active = true;
        temperature.Value = 0.0f;

        effect.Set(temperature, Force.Default, weArtTexture);

        hapticObjectIndex.AddEffect(effect);
    }
    private void HapticFeedbackTexture(){
        effect = new UpdateTouchedHaptics();

        weArtTexture.Active = true;
        weArtTexture.TextureType = TextureType.ProfiledRubberSlow;
        weArtTexture.Volume = 100;
        weArtTexture.VelocityX = 0.0f;
        weArtTexture.VelocityY = 0.0f;
        weArtTexture.VelocityZ = 0.0f;

        effect.Set(temperature, Force.Default, weArtTexture);
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

                        vrControllerCollidingPosition = vrController.transform.position.y;
                        vrControllerCollidingPositionIsSet = true;
                        
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
            changed |= !Texture.Equals(texture);
            Texture = texture;

            if (changed)
                OnUpdate?.Invoke();
        }
    }    
}