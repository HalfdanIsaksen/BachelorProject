using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectTrigger : MonoBehaviour
{

    public SoundOnHover soundOnHoverScript;

    public SoundOnHover lastSiblingSoundOnHoverScript;
    public AudioSource lastSiblingAudioSource;

    public SoundOnHover nextSiblingSoundOnHoverScript;
    public AudioSource nextSiblingAudioSource;

    void Start()
    {
        soundOnHoverScript = this.gameObject.GetComponent<SoundOnHover>();

        lastSiblingSoundOnHoverScript = this.gameObject.transform.parent.GetChild(soundOnHoverScript.transform.GetSiblingIndex() - 1).GetComponent<SoundOnHover>();
        lastSiblingAudioSource = this.gameObject.transform.parent.GetChild(soundOnHoverScript.transform.GetSiblingIndex() - 1).GetComponent<AudioSource>();

        nextSiblingSoundOnHoverScript = this.gameObject.transform.parent.GetChild(soundOnHoverScript.transform.GetSiblingIndex() + 1).GetComponent<SoundOnHover>();
        nextSiblingAudioSource = this.gameObject.transform.parent.GetChild(soundOnHoverScript.transform.GetSiblingIndex() + 1).GetComponent<AudioSource>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Index_collider")
        {
            lastSiblingSoundOnHoverScript.enabled = false;
            lastSiblingAudioSource.enabled = false;

            nextSiblingSoundOnHoverScript.enabled = false;
            nextSiblingAudioSource.enabled = false;
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Index_collider")
        {
            lastSiblingSoundOnHoverScript.enabled = true;
            lastSiblingAudioSource.enabled = true;

            nextSiblingSoundOnHoverScript.enabled = true;
            nextSiblingAudioSource.enabled = true;
        }

    }
}
