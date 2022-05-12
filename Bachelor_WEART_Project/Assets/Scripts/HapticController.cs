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
    private float minDistance = 0.05f;
    [SerializeField]

    private float distance;
    private bool indexHapticFeedbackTriggered = false;
    [SerializeField]
    private bool middleHapticFeedbackTriggered = false;

    private bool soundOn = false;
    //Variable for creating the haptic effect which is applied
    private UpdateTouchedHaptics effect;


    private UpdateTouchedHaptics effectIndex;
    private UpdateTouchedHaptics effectMiddle;

    //Varialbes for each haptic effect the WeART is able to enforce
    private Temperature temperature = Temperature.Default;
    private Force hapticForce = Force.Default;
    [SerializeField]
    private WeArtTexture weArtTexture;

    private WeArtTexture textureToIndex;
    private WeArtTexture textureToMiddle;

    private WeArtTexture textureToThumb;


    private Vector3 vrControllerCollidingPositionVector;

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

    private Force forceValueToIndex = Force.Default;
    private Force forceValueToMiddle = Force.Default;

    private float calculatedForce;

    [SerializeField]
    //private bool conditionWithTemperature;

    private bool waterIsHit = false;

    DateTime timeAtWaterRelease;

    //time it should take before the temperature starts falling to neutral
    [SerializeField]
    private float timeInSecondsBeforeTemperatureDegrade = 10.0f;

    //time it should take before the temperature starts falling to neutral
    [SerializeField]
    private float timeInSecondsForLinearTempDegrade = 10.0f;


    private bool indexTextureTriggered = false;
    private bool middleTextureTriggered = false;


    //water- and neutral temperatures
    [SerializeField]
    private float temperatureNeutral = 0.55f;

    [SerializeField]
    private float temperatureWater = 0.0f;

    [SerializeField]
    private TestCondition currentTestCondition;
    enum TestCondition
    {
        Force,
        Temperature,
        Texture,
        All,
        None
    }

    // Start is called before the first frame update
    void Start()
    {

    }
    void FixedUpdate()
    {
        CalculateForceValue();
        switch (currentTestCondition)
        {
            case TestCondition.Force:
                HapticFeedbackForce();
                ApplyEffect();
                break;
            case TestCondition.Temperature:
                HapticFeedbackTempurature();
                ApplyEffect();
                break;
            case TestCondition.Texture:
                HapticFeedbackTexture();
                ApplyEffect();
                break;
            case TestCondition.All:
                HapticFeedbackForce();
                HapticFeedbackTempurature();
                HapticFeedbackTexture();
                ApplyEffect();
                break;
            case TestCondition.None:
                HapticFeedbackTempurature();
                break;

        }
    }

    void OnApplicationQuit()
    {
    }

    private void OnTriggerEnter(Collider other)
    {

        if (!waterIsHit)
        {
            ActivateTemperatureFeedback(other);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        vrControllerCollidingPositionVector = this.gameObject.transform.position;

        ActivateHapticFeedback(col);

    }

    void OnCollisionStay(Collision col)
    {
        //checks if the gameobject we are colliding with is one the layer called haptic
        //and if this game object is on the layer called player
        if (col.gameObject.layer == 7 && this.gameObject.layer == 6)
        {
            //if the gameobject we are colliding with has the tag instrument
            //check each contact point, if any collider has the name
            //index_collider or middle_collider, set haptic feedback for those fingers
            if (col.gameObject.tag == "Instrument")
            {
                foreach (ContactPoint contact in col.contacts)
                {
                    var colName = contact.thisCollider.name;

                    if (colName == "Index_collider")
                    {
                        indexHapticFeedbackTriggered = true;

                        indexTextureTriggered = true;
                        soundOn = true;
                    }
                    if (colName == "Middle_collider")
                    {
                        soundOn = true;
                        middleHapticFeedbackTriggered = true;

                        middleTextureTriggered = true;
                    }
                }
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        // List<String> colnames = new List<String>();
        // foreach (ContactPoint contact in col.contacts)
        // {
        //     colnames.Add(contact.thisCollider.name);            
        // }
        // if(!colnames.Contains("Index_collider") && colnames != null){
        //     Debug.Log("Vi burde have gjort det sådan her");
        // }
        //if we are no longer in contact with object in water layer
        if (col.gameObject.name == "Water")
        {
            //Debug.Log("No longer in contact with " + col.transform.name);
            //set time of water release
            timeAtWaterRelease = DateTime.Now;
        }
        if (col.gameObject.tag == "Instrument")
        {
            // indexTextureTriggered = false;
            // middleTextureTriggered = false;
            soundOn = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Water")
        {
            timeAtWaterRelease = DateTime.Now;
        }
    }

    private void ActivateHapticFeedback(Collision col)
    {
        //checks if the gameobject we are colliding with is one the layer called haptic
        //and if this game object is on the layer called player
        if (col.gameObject.layer == 7 && this.gameObject.layer == 6)
        {
            //if the gameobject we are colliding with has the tag instrument
            //check each contact point
            if (col.gameObject.tag == "Instrument")
            {

                foreach (ContactPoint contact in col.contacts)
                {
                    //Debug.Log("HEr..");
                    var colName = contact.thisCollider.name;
                    //If one of the colliders name is Index_collider and the y-position of the controller is not set
                    //set the y-position of the controller to the controllers current y-position,
                    // and set haptic feedback for index finger to be true
                    if (colName == "Index_collider" || colName == "Middle_collider")
                    {

                        if (!vrControllerCollidingPositionIsSet)
                        {
                            //Debug.Log("Kommer vi ind her?");
                            vrControllerCollidingPosition = vrController.transform.position.y;
                            vrControllerCollidingPositionIsSet = true;
                        }

                        if (!indexHapticFeedbackTriggered && colName == "Index_collider")
                        {
                            indexHapticFeedbackTriggered = true;
                            indexTextureTriggered = true;
                        }
                        if (!middleHapticFeedbackTriggered && colName == "Middle_collider")
                        {
                            middleHapticFeedbackTriggered = true;
                            middleTextureTriggered = true;
                        }
                    }
                    //If one of the colliders name is Middle_collider and the y-position of the controller is not set
                    //set the y-position of the controller to the controllers current y-position
                    // and set haptic feedback for index finger to be true
                }
            }
        }
    }

    private void ActivateTemperatureFeedback(Collider col)
    {
        //if hand/player layer (6) is colliding with water layer (4), set to true 
        if (col.gameObject.layer == 4)
        {
            waterIsHit = true;
        }
    }

    //if HapticFeedbackForce() is not running we need this value anyway, to determine bow pressure
    private float CalculateForceValue()
    {
        float distance = (vrControllerCollidingPosition - vrController.position.y);

        calculatedForce = Mathf.Pow((distance / maxHitDistance), 2);
        calculatedForce = Mathf.Clamp(calculatedForce, 0.0f, 1.0f);

        if (distance < minDistance)
        {
            vrControllerCollidingPositionIsSet = false;
            indexTextureTriggered = false;
            middleTextureTriggered = false;
        }

        return calculatedForce;
    }

    // private void ResetHapticFeedback()
    // {
    //     vrControllerCollidingPositionIsSet = false;
    //     vrControllerCollidingPosition = minDistance;
    // }

    private void HapticFeedbackForce()
    {
        //create a new effect
        //effect = new UpdateTouchedHaptics();
        //if feedback for any of the fingers are triggered
        if (indexHapticFeedbackTriggered || middleHapticFeedbackTriggered)
        {
            //Calculate the distance between the Controllers y-position on impact,
            //and it's current position
            float distance = (vrControllerCollidingPosition - vrController.position.y);
            //If the distance is bigger than the minimum distance required
            if (distance > minDistance)
            {
                //set the haptic force to be active
                forceValueToIndex.Active = true;
                forceValueToMiddle.Active = true;

                //apply the effect    
                //check if any of the fingeres can get haptic feedback, if yes, then apply feedback
                if (indexHapticFeedbackTriggered)
                {
                    forceValueToIndex.Value = CalculateForceValue();
                    // Debug.Log("forceValueToIndex.Value: " + forceValueToIndex.Value);
                }
                if (middleHapticFeedbackTriggered)
                {
                    forceValueToMiddle.Value = CalculateForceValue();
                    //Debug.Log("forceValueToMiddle.Value: " + forceValueToMiddle.Value);
                }

            }
            else
            { //if distance is less than 0
              //set haptic value to the minimum amount of force we want apllied

                //hapticForce.Value = minForce;
                forceValueToIndex.Value = minForce;
                forceValueToMiddle.Value = minForce;
                // forceValueToIndex.Active = false;
                // forceValueToMiddle.Active = false;

                //Set our fingers to not wanting haptic feedback anymore
                indexHapticFeedbackTriggered = false;
                middleHapticFeedbackTriggered = false;
                soundOn = false;

                //ResetHapticFeedback();
            }
        }
    }

    public bool GetSoundState
    {
        get => soundOn;
    }
    private void HapticFeedbackTempurature()
    {

        //Set the temperature to be active, and the temperture to be cold
        temperature.Active = true;

        //if we have hit layer with water
        if (waterIsHit)
        {
            temperature.Value = temperatureWater;
            Debug.Log("Water is hit, temperature is :" + temperature.Value);
            //calculate timespan from release of hands from water till now
            TimeSpan timeElapsedAfterWaterRelease = DateTime.Now.Subtract(timeAtWaterRelease);

            //keep temperature at water temp
            if ((float)timeElapsedAfterWaterRelease.TotalSeconds <= timeInSecondsBeforeTemperatureDegrade)
            {
                temperature.Value = temperatureWater;
            }
            else
            {
                //start linear degrade towards neutral temperature
                temperature.Value = Map((float)timeElapsedAfterWaterRelease.TotalSeconds, 0.0f, (timeInSecondsForLinearTempDegrade + timeInSecondsBeforeTemperatureDegrade), temperatureWater, temperatureNeutral);
                if (temperature.Value >= temperatureNeutral)
                {
                    waterIsHit = false;
                }
            }

        }
        else //water not hit, just apply neutral temperature
        {
            temperature.Value = temperatureNeutral;

        }

    }

    private void HapticFeedbackTexture()
    {
        //choose which texture type is given as output, set intensity to max
        if (indexTextureTriggered)
        {
            textureToIndex.Active = true;
            textureToIndex.TextureType = weArtTexture.TextureType;

            textureToIndex.Volume = Map(calculatedForce, 0.0f, 0.3f, 0.0f, 100.0f);

            if (CalculateForceValue() >= 0.3 && CalculateForceValue() <= 0.6)
            {
                textureToIndex.Volume = 100;
                //Debug.Log(textureToIndex.Volume);

            }
            else
            {
                textureToIndex.Volume = 0;
            }
        }
        else
        {
            textureToIndex.Active = false;
        }


        if (middleTextureTriggered)
        {
            textureToMiddle.Volume = 100;
            textureToMiddle.Active = true;
            textureToMiddle.TextureType = weArtTexture.TextureType;
        }
        else
        {
            textureToMiddle.Active = false;
        }
    }

    void ApplyEffect()
    {

        effectIndex = new UpdateTouchedHaptics();
        effectMiddle = new UpdateTouchedHaptics();

        effectIndex.Set(temperature, forceValueToIndex, textureToIndex);
        effectMiddle.Set(temperature, forceValueToMiddle, textureToMiddle);

        hapticObjectIndex.AddEffect(effectIndex);
        hapticObjectMiddle.AddEffect(effectMiddle);

    }

    public float Map(float x, float x1, float x2, float y1, float y2)
    {
        return (x - x1) / (x2 - x1) * (y2 - y1) + y1;
    }

    public Vector3 GetvrControllerCollidingPositionVector
    {
        get => vrControllerCollidingPositionVector;
    }

    public bool GetVRCollidingPositionIsSet
    {
        get => vrControllerCollidingPositionIsSet;
    }

    public int GetCurrentCondition
    {
        get => (int)currentTestCondition;
    }

    //Calculated force, instead of actual force.
    public float GetForceValue
    {
        get => CalculateForceValue();
        //get => forceValueToIndex.Value; 
    }

    public float GetIndexForceValue
    {
        //get => calculatedForce;
        get => forceValueToIndex.Value;
    }

    public float GetMiddleForceValue
    {
        //get => calculatedForce;
        get => forceValueToMiddle.Value;
    }

    public float GetTemperatureValue
    {
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