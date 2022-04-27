using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeArt.Components;
using WeArt.Core;
using System;
using WeArtTexture = WeArt.Core.Texture;

public class HapticController : MonoBehaviour
{
    //Variables responsable for holding the haptic recivers
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
    
    //Variable for creating the haptic effect which is applied
    private UpdateTouchedHaptics effect;
    //Varialbes for each haptic effect the WeART is able to enforce
    private Temperature temperature = Temperature.Default;
    private Force hapticForce = Force.Default;
    [SerializeField]
    private WeArtTexture weArtTexture = WeArtTexture.Default;
    //Variable for controller where the VR-controllers y-position
    //when collision with another game object occurs
    private float vrControllerCollidingPosition;
    //Boolean to check wether or not we are getting the
    //controllers y-position
    private bool vrControllerCollidingPositionIsSet = false;
    //Transform holding the VR-controller the WeART, and visual hand
    //is following
    [SerializeField]
    private Transform vrController;

    // Start is called before the first frame update

    void Start(){
        //HapticFeedbackTempurature();
    }
    void FixedUpdate(){
        //HapticFeedbackTempurature();
        HapticFeedbackForce(); 
        //HapticFeedbackTexture();
    }
    
    void OnApplicationQuit(){
    }

    void OnCollisionEnter(Collision col){
        ActivateHapticFeedback(col);
    }

    void OnCollisionStay(Collision col){
        //checks if the gameobject we are colliding with is one the layer called haptic
        //and if this game object is on the layer called player
        if(col.gameObject.layer == 7 && this.gameObject.layer == 6){
            //if the gameobject we are colliding with has the tag instrument
            //check each contact point, if any collider has the name
            //index_collider or middle_collider, set haptic feedback for those fingers
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

    private void ActivateHapticFeedback(Collision col){
         //checks if the gameobject we are colliding with is one the layer called haptic
        //and if this game object is on the layer called player
        if(col.gameObject.layer == 7 && this.gameObject.layer == 6){
            //if the gameobject we are colliding with has the tag instrument
            //check each contact point
            if(col.gameObject.tag == "Instrument"){
                foreach(ContactPoint contact in col.contacts){

                    var colName = contact.thisCollider.name;
                    //If one of the colliders name is Index_collider and the y-position of the controller is not set
                    //set the y-position of the controller to the controllers current y-position,
                    // and set haptic feedback for index finger to be true
                    if(colName == "Index_collider" && !vrControllerCollidingPositionIsSet){

                        vrControllerCollidingPosition = vrController.transform.position.y;
                        vrControllerCollidingPositionIsSet = true;
                        
                        if(!indexHapticFeedbackTriggered && colName == "Index_collider"){
                            indexHapticFeedbackTriggered = true;
                        }
                    }
                    //If one of the colliders name is Middle_collider and the y-position of the controller is not set
                    //set the y-position of the controller to the controllers current y-position
                    // and set haptic feedback for index finger to be true
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

    private void HapticFeedbackForce(){
        //create a new effect
        effect = new UpdateTouchedHaptics();
        //if feedback for any of the fingers are triggered
        if(indexHapticFeedbackTriggered || middleHapticFeedbackTriggered){
            //Calculate the distance between the Controllers y-position on impact,
            //and it's current position
            float distance = (vrControllerCollidingPosition - vrController.position.y);
            //If the distance is bigger than the minimum distance required
            if(distance > minDistance)
            {
                //set the haptic force to be active
                hapticForce.Active = true;

                //set the value for the haptic force feedback to be equals
                //The distance/maxhitdistance squared
                hapticForce.Value = ((distance/maxHitDistance) * (distance/maxHitDistance));
                //apply the effect    
                effect.Set(temperature, hapticForce, weArtTexture);
                //check if any of the fingeres can get haptic feedback, if yes, then apply feedback
                if(indexHapticFeedbackTriggered){
                    hapticObjectIndex.AddEffect(effect);
                }
                if(middleHapticFeedbackTriggered){
                   hapticObjectMiddle.AddEffect(effect);
                }

            }else{ //if distance is less than 0
                //set haptic value to the minimum amount of force we want apllied
                hapticForce.Value = minForce;
                effect.Set(temperature, Force.Default, weArtTexture);

                //Add effect to both fingers.
                hapticObjectIndex.AddEffect(effect);
                hapticObjectMiddle.AddEffect(effect);
                //Set our fingers to not wanting haptic feedback anymore
                indexHapticFeedbackTriggered = false;
                middleHapticFeedbackTriggered = false;
            }
        }
    }
    private void HapticFeedbackTempurature(){
        //create a new effect
        effect = new UpdateTouchedHaptics();
        //Set the temperature to be active, and the temperture to be cold
        temperature.Active = true;
        temperature.Value = 0.0f;
        //Apply changes to haptic feedback
        effect.Set(temperature, Force.Default, weArtTexture);
        //Add the effect to the fingers
        hapticObjectIndex.AddEffect(effect);
    }
    private void HapticFeedbackTexture(){
        //create a new effect
        effect = new UpdateTouchedHaptics();
        //Set the texture feedback to be active
        weArtTexture.Active = true;
        //choose which texture type is given as output, set intensity to max
        weArtTexture.TextureType = TextureType.ProfiledRubberSlow;
        weArtTexture.Volume = 100;
        weArtTexture.VelocityX = 0.0f;
        weArtTexture.VelocityY = 0.0f;
        weArtTexture.VelocityZ = 0.0f;
        //Apply effect and add the effect to the fingers
        effect.Set(temperature, Force.Default, weArtTexture);
        hapticObjectIndex.AddEffect(effect);
    }


    public float GetForceValue{
        get => hapticForce.Value;
    }

    public float GetTemperatureValue{
        get => temperature.Value;
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