using UnityEngine;
using System;
using System.IO;

[RequireComponent(typeof(AudioSource))]

public class SoundOnHover : MonoBehaviour
{
    public AudioClip glass;
    private AudioSource audioSource;
    [SerializeField]
    private GameObject[] audioObject;
    private bool soundOn;
    public FaustPlugin_glassHarmonica scriptFaust;
    [SerializeField]
    private HapticController hapticController;

    private Vector3 vrControllerPosition;
    private float distanceFromCenterToTracker;
    private bool activateSweetSpotTimer;
    private DateTime sweetSpotTimer;
    private bool takeCount;
    private int amountPressedToHard;
    public float bowPressure;
    public float integrationConstant;
    public float integrationConstantValue;

    public float forceExponentialConstant = 0.2f;

    private float sweetSpotBowPressure = 0.85f;

    //Variables for time logging

    private float secondsInSweetSpot;

    private float secondsAtMaxForce;

    private bool maxForceIsCounted = false;



    [SerializeField]
    public GameObject dataLogger;

    private DataLogger dataLoggerScript;


    // Start is called before the first frame update
    void Start()
    {
        audioObject = GameObject.FindGameObjectsWithTag("Instrument");
        audioSource = this.gameObject.GetComponent<AudioSource>();
        scriptFaust = this.gameObject.GetComponent<FaustPlugin_glassHarmonica>();

        dataLoggerScript = dataLogger.GetComponent<DataLogger>();
        //dataLogger = dataLogger.GetComponent<DataLogger>();
        //secondsAtMaxForce = dataLogger.GetComponent<DataLogger>().secondsAtMaxForce;

    }

    void OnApplicationQuit()
    {
        //print("Time at sweet spot: " + TimeSpan.FromSeconds(secondsInSweetSpot));
        //print("Time at max force: " + TimeSpan.FromSeconds(secondsAtMaxForce));

    }

  


    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == 6)
        {

            hapticController = col.gameObject.GetComponentInParent<HapticController>();
            foreach (GameObject gObject in audioObject)
            {
                if (gObject.name != this.gameObject.name)
                {
                    gObject.GetComponent<AudioSource>().enabled = false;
                }
            }
        }

    }
    private void OnCollisionStay(Collision other)
    {

    }
    private float CalcBowPressure(float forceValue)
    {
        //bowPressure = Mathf.Pow(forceValue, forceExponentialConstant);


        // if(forceValue > 0.6){
        //     if(!takeCount){
        //         amountPressedToHard ++;
        //         Debug.Log("Times pressed to hard: " + amountPressedToHard);
        //         takeCount = true;
        //     }
        // }

        if (forceValue >= 0.4 && forceValue <= 0.6)
        {
            //dataLoggerScript.SetSecondsInSweetSpot = dataLoggerScript.GetSecondsInSweetSpot + Time.deltaTime;

            dataLoggerScript.SetSecondsInSweetSpot = dataLoggerScript.GetSecondsInSweetSpot + Time.deltaTime;


            //secondsInSweetSpot += Time.deltaTime;
            //Debug.Log(secondsInSweetSpot);
            //takeCount = false;
            bowPressure = sweetSpotBowPressure;
            // if (!activateSweetSpotTimer)
            // {
            //     sweetSpotTimer = DateTime.Now;
            //     activateSweetSpotTimer = true;
            // }
        }
        if (forceValue <= 0.4 || forceValue >= 0.6)
        {
            if(dataLoggerScript.GetSecondsInSweetSpot != 0.0f)
            {
                Debug.Log("Sweet Spot data to insert: " + dataLoggerScript.GetSecondsInSweetSpot);
                dataLoggerScript.GetSweetSpotTimings.Add(dataLoggerScript.GetSecondsInSweetSpot);
                dataLoggerScript.SetSecondsInSweetSpot = 0f;
            }

            bowPressure = 1.0f;
        }

        if (forceValue >= 0.95)
        {
            Debug.Log(forceValue);
            if (!maxForceIsCounted)
            {
                dataLoggerScript.SetCountsAtMaxForce = dataLoggerScript.GetCountsAtMaxForce + 1;
                maxForceIsCounted = true;
            }


        }

        if (forceValue <= 0.95)
        {
            maxForceIsCounted = false;
        }

        // else
        // {
        //     //takeCount = false;
        //     if (activateSweetSpotTimer)
        //     {
        //         activateSweetSpotTimer = false;
        //         TimeSpan timeInSweetSpot = DateTime.Now.Subtract(sweetSpotTimer);
        //         Debug.Log("Time in sweet spot:" + timeInSweetSpot.TotalSeconds);
        //         //ADD TO ARRAY EVERY TIME
        //     }
        // }
        return bowPressure;
    }

    private float CalcIntegrationConstant(float temperature)
    {

        //temperature range: 0.0 - 0.55, integration constant range: 0.15 - 0.0
        integrationConstantValue = hapticController.Map(temperature, 0.0f, 0.55f, 0.15f, 0.05f);
        return integrationConstantValue;
    }

    void Update()
    {
        if (hapticController != null)
        {
            if (hapticController.GetSoundState == true)
            {
                scriptFaust.setParameter(7, CalcBowPressure(hapticController.GetForceValue));

                integrationConstantValue = 0.15f;

                float temperature = hapticController.GetTemperatureValue;

                if ((hapticController.GetCurrentCondition == 1 || hapticController.GetCurrentCondition == 3 || hapticController.GetCurrentCondition == 4) && temperature != 0.0f)
                {
                    CalcIntegrationConstant(temperature);
                }

                scriptFaust.setParameter(9, integrationConstantValue); //integration constant -> bow velocity

                audioSource.enabled = true;
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = glass;
                    audioSource.Play();
                }
            }
            else
            {
                scriptFaust.setParameter(7, 1);
                scriptFaust.setParameter(9, 0);
                soundOn = false;
                audioSource.Stop();
                hapticController = null;
            }
        }
    }
}
