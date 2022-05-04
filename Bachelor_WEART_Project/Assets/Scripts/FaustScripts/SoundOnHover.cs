using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class SoundOnHover : MonoBehaviour
{
    public AudioClip glass;
    private AudioSource audioSource;
    public FaustPlugin_glassHarmonica scriptFaust;

    private HapticController hapticController;

    public float bowPressure;
    public float integrationConstant;
    public float integrationConstantValue;

    public float forceExponentialConstant = 0.2f;

    private float sweetSpotBowPressure = 0.75f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        scriptFaust = this.transform.GetComponent<FaustPlugin_glassHarmonica>();

        bowPressure = scriptFaust.getParameter(7);
        //print("scriptFaust.parameters damp = " + bowPressure);
        integrationConstant = scriptFaust.getParameter(9);


    }

    private float CalcBowPressure(float forceValue)
    {
        //Debug.Log(forceValue);

        bowPressure = Mathf.Pow(forceValue, forceExponentialConstant);

        if (forceValue >= 0.4 && forceValue <= 0.6)
        {
            bowPressure = sweetSpotBowPressure;
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
            Debug.Log("Force value inside SoundOnHover: " + hapticController.GetForceValue);
            if (hapticController.GetForceValue > 0.03f)
            {

                scriptFaust.setParameter(7, CalcBowPressure(hapticController.GetForceValue));

                integrationConstantValue = 0.15f;

                float temperature = hapticController.GetTemperatureValue;
                //Debug.LogFormat("Temperature: {0} IntegrationConstant: {1}", temperature, integrationConstantValue);

                if ((hapticController.GetCurrentCondition == 1 || hapticController.GetCurrentCondition == 3) && temperature != 0.0f)
                {

                    CalcIntegrationConstant(temperature);

                    //Debug.LogFormat("Temperature, inside con: {0}... IntegrationConstant, inside con: {1}", temperature, integrationConstantValue);

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
                hapticController = null;
                         

            }
        }
    }


    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == 6)
        {
            hapticController = col.gameObject.GetComponentInParent<HapticController>();
            // audioSource.enabled = true;

            // if (!audioSource.isPlaying)
            // {
            //     audioSource.clip = glass;
            //     audioSource.Play();
            // }
        }
    }
}
