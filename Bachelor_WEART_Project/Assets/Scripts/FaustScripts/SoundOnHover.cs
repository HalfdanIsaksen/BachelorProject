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

    public float forceExponentialConstant = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        scriptFaust = this.transform.GetComponent<FaustPlugin_glassHarmonica>();

        bowPressure = scriptFaust.getParameter(7);
        //print("scriptFaust.parameters damp = " + bowPressure);
        integrationConstant = scriptFaust.getParameter(9);


    }

    float CalcBowPressure(float forceValue){
        
        bowPressure = Mathf.Pow(forceValue, forceExponentialConstant);

        if(forceValue >= 0.3 && forceValue <= 0.4){
            bowPressure = 0.85f;
        }
        // }else{
        //     bowPressure = 1.0f;
        // }
        // if(bowPressure >= 0.2f){
        //     bowPressure = 0.2f;
        // }else{
        //     bowPressure = 1.0f;
        // }


        //Debug.LogFormat("Force: {0}, Bowpressure: {1}", forceValue, bowPressure);
        return bowPressure;
    }

    void Update(){
        if(hapticController != null){
            if(hapticController.GetForceValue > 0.03f){

                scriptFaust.setParameter(7, CalcBowPressure(hapticController.GetForceValue));

                
                //scriptFaust.setParameter(7, 1 - hapticController.GetForceValue);
                //Debug.Log(bowPressure);
                scriptFaust.setParameter(9, 0.15f); //integration constant -> bow velocity

                audioSource.enabled = true;
                if (!audioSource.isPlaying )
                {
                    audioSource.clip = glass;
                    audioSource.Play();
                }
            }else{
                scriptFaust.setParameter(7, 1);
                scriptFaust.setParameter(9, 0);
                hapticController = null;
               
            }
        }
    }
    /*

    */


    void OnCollisionEnter(Collision col){
        if(col.gameObject.layer == 6){
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
