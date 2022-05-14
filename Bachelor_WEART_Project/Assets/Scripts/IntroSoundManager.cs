using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSoundManager : MonoBehaviour
{
    [SerializeField]
    private float delayForAudioSeconds = 5.0f;


    Renderer render;
    Material SphereMaterial;

    Material matBust;
    Material matGhost;

    AudioSource m_MyAudioSource;

    private Animator animator;


    void Start()
    {

        m_MyAudioSource = GetComponent<AudioSource>();

        animator = gameObject.GetComponent<Animator>();

        Renderer render = GetComponent<Renderer>();
        
        //matBust = Resources.Load<Material>("Bust");
        matBust = render.materials[0];
        matGhost = render.materials[1];


        StartCoroutine(playSound());
        StartCoroutine(FadeDownAlpha());
        StartCoroutine(FadeUpAlpha());

    }


    private IEnumerator playSound()
    {
        yield return new WaitForSeconds(delayForAudioSeconds);
        m_MyAudioSource.Play();
    }

    private IEnumerator FadeDownAlpha()
    {
       
        Color initialColor = matBust.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0.1f);

        float elapsedTime = 0f;
        float fadeDuration = 20.0f;

        float matGhostTransparencyIntensity = matGhost.GetFloat("Transparency_Intensity");


        while (elapsedTime < fadeDuration)
        {
            if (m_MyAudioSource.isPlaying)
            {
                Debug.Log("Inside fade..............");
                elapsedTime += Time.deltaTime;
                matBust.color = Color.Lerp(initialColor, targetColor, elapsedTime / fadeDuration);

                matGhostTransparencyIntensity = Mathf.Lerp(matGhostTransparencyIntensity, 1.0f, elapsedTime / fadeDuration);
                matGhost.SetFloat("Transparency_Intensity", matGhostTransparencyIntensity);
            }
            yield return null;
        }

    }

    private IEnumerator FadeUpAlpha()
    {
        yield return new WaitForSeconds(5.0f + m_MyAudioSource.clip.length);
        //Renderer rend = Sphere.transform.GetComponent<Renderer>();
        Color fadedColor = new Color(matBust.color.r, matBust.color.g, matBust.color.b, 0.1f);
        Color targetColor = new Color(matBust.color.r, matBust.color.g, matBust.color.b, 1.0f);



        float matGhostTransparencyIntensity = 1.0f;
        float matGhostFresnelIntensity = 2.2f;

        float elapsedTime = 0f;
        float fadeDuration = 10.0f;

        while (elapsedTime < fadeDuration)
        {

            
            elapsedTime += Time.deltaTime;
            matBust.color = Color.Lerp(fadedColor, targetColor, elapsedTime / fadeDuration);

            matGhostTransparencyIntensity = Mathf.Lerp(1.0f, 0.0f, elapsedTime / fadeDuration);
            //matGhostTransparencyIntensity = Mathf.Clamp(matGhostTransparencyIntensity, 0.0f, 1.0f);

            matGhostFresnelIntensity = Mathf.Lerp(2.2f, 0.0f, elapsedTime / fadeDuration);

            matGhost.SetFloat("Transparency_Intensity", matGhostTransparencyIntensity);

            matGhost.SetFloat("Fresnel_Intensity", matGhostFresnelIntensity);

            Debug.LogFormat("Time elapsed: {0}... Transparency: {1}", elapsedTime, matGhostTransparencyIntensity);

            yield return null;
            
        }
        
    }


    private void Update()
    {

        //Debug.Log(m_MyAudioSource.isPlaying);

        if (m_MyAudioSource.isPlaying)
        {
            //StartCoroutine(FadeDownAlpha());
            //animator.Play("Initial");
            animator.Play("bust");





            //matBust.SetColor("_BaseColor", new Color(1, 1, 1, 0.1f));
            matBust.SetFloat("Smoothness", 0.0f);

            float col = matGhost.GetFloat("Tint_Intensity");


            float tintIntensity = GetVolume() * 7.5f + 1;

            tintIntensity = Mathf.Clamp(tintIntensity, 1, 2.0f);
           // Debug.Log(tintIntensity);

            // matGhost.SetFloat("Transparency_Intensity", 1.0f);
            matGhost.SetFloat("Tint_Intensity", tintIntensity);
            matGhost.SetFloat("Fresnel_Intensity", 2.2f);




            //mat = SphereMaterial;
        }

        if (!m_MyAudioSource.isPlaying)
        {

            //StartCoroutine(FadeUpAlpha());

            animator.Play("Idle");
            //matBust.SetColor("_BaseColor", new Color(1, 1, 1, 1.0f));


            float col = matGhost.GetFloat("Tint_Intensity");

            //float tintIntensity = GetVolume() * 5;

            //Mathf.Clamp(tintIntensity, 1, 3);

            // matGhost.SetFloat("Tint_Intensity", 0.0f);
            // matGhost.SetFloat("Fresnel_Intensity", 0.0f);
            //matGhost.SetFloat("Transparency_Intensity", 0.0f);



        }

    }


    float GetVolume()
    {

        float[] data = new float[256];
        float a = 0;
        m_MyAudioSource.GetOutputData(data, 0);
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }
        return a / 256;

    }
}
