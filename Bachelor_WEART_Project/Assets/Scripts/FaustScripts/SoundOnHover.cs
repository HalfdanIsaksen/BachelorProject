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

        // Spat Parameter (No stereo panning)
        scriptFaust.setParameter(12,0.5f);
        scriptFaust.setParameter(13,0.5f);
    }

    void OnApplicationQuit()
    {


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

        if (forceValue >= 0.35 && forceValue <= 0.6)
        {
            dataLoggerScript.SetSecondsInSweetSpot = dataLoggerScript.GetSecondsInSweetSpot + Time.deltaTime;

            bowPressure = sweetSpotBowPressure;

        }
        if (forceValue <= 0.35 || forceValue >= 0.6)
        {
            if(dataLoggerScript.GetSecondsInSweetSpot >= 1.0f)
            {
                if(dataLoggerScript.GetTutorial){
                    dataLoggerScript.GetGameMode.Add("Tutorial");
                }else{
                    dataLoggerScript.GetGameMode.Add("Task");
                }
                dataLoggerScript.GetNoteName.Add(this.gameObject.name);
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
                if(dataLoggerScript.GetTutorial){
                    dataLoggerScript.GetGameModeForce.Add("Tutorial");
                }else{
                    dataLoggerScript.GetGameModeForce.Add("Task");
                }
                dataLoggerScript.GetNoteNameForce.Add(this.gameObject.name);
                dataLoggerScript.GetCountsAtMaxForce.Add(1);
                maxForceIsCounted = true;
            }


        }

        if (forceValue <= 0.95)
        {
            maxForceIsCounted = false;
        }
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

                if ((hapticController.GetCurrentCondition == 1 || hapticController.GetCurrentCondition == 3) && temperature != 0.0f)
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
