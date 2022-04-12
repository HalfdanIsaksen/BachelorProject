using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Interactable))]


public class SoundOnHover : MonoBehaviour
{
    public AudioClip glass;
    private AudioSource audioSource;
    public FaustPlugin_glassHarmonica scriptFaust;

    private HapticController hapticController;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        scriptFaust = this.transform.GetComponent<FaustPlugin_glassHarmonica>();
    }

    void Update(){
        if(hapticController != null){
            if(hapticController.GetForceValue > 0.1f){
                scriptFaust.setParameter(7, hapticController.GetForceValue);
                scriptFaust.setParameter(9, 0.05f);
            }
        }

    }
    void OnCollisionEnter(Collision col){
        if(col.gameObject.layer == 6){
            hapticController = col.gameObject.GetComponentInParent<HapticController>();

            audioSource.enabled = true;

            if (!audioSource.isPlaying)
            {
                Debug.Log("Audio is Playing");
                audioSource.clip = glass;
                audioSource.Play();
            }
        }
    }
  }
