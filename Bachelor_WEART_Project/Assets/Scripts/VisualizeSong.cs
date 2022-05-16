using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizeSong : MonoBehaviour
{

    //reference to bowls
    public GameObject C_Low;
    public GameObject D_Sharp_Low;
    public GameObject E_Low;
    public GameObject G;
    public GameObject G_Sharp;
    public GameObject A;
    public GameObject B;
    public GameObject C_High;
    public GameObject D;
    public GameObject D_Sharp_High;
    public GameObject E_High;


    //Color variables
    private Color red;
    private Color green;
    private Color white;

    //tempo: BPM
    private float tempo = 3.0f;

    //note duration
    private float whole_note_sec;
    private float threeQuater_note_sec;
    private float half_note_sec;
    private float quarter_note_sec;
    private float eight_note_sec;
    private float sixteenth_note_sec;


    private float timer;
    private float elapsedTime;
    private float redDuration;
    private float delayStart = 3.0f;
    [SerializeField]
    private SongSequence currentNote;
    enum SongSequence
    {
        E_High,
        D_Sharp_High,
        E_High_2,
        D_Sharp_High_2,
        E_High_3,
        B,
        D,
        C_High,
        A
    }

    //note lengths
    // float first_note = 
    // float second_note =
    // float third_note = 
    // float fourth_note = 

    // Start is called before the first frame update
    void Start()
    {

        //init colors
        red = new Color(1.0f, 0.0f, 0.0f);
        green = new Color(0.0f, 1.0f, 0.0f);
        white = new Color(1.0f, 1.0f, 1.0f);

        //init red duration
        redDuration = 1.0f;

        //init note lengths
        whole_note_sec = 60.0f / tempo;
        threeQuater_note_sec = whole_note_sec * 0.75f;
        half_note_sec = whole_note_sec * 0.50f;
        quarter_note_sec = whole_note_sec * 0.25f;
        eight_note_sec = whole_note_sec * 0.125f;
        sixteenth_note_sec = whole_note_sec * 0.0625f;

    }

    // Update is called once per frame
    void Update()
    {

        switch(currentNote){
            case SongSequence.E_High:
                controlNoteState(E_High,quarter_note_sec);
                break;
            case SongSequence.D_Sharp_High:
                controlNoteState(D_Sharp_High, quarter_note_sec);
                break;
            case SongSequence.E_High_2:
                controlNoteState(E_High,quarter_note_sec);
                break;
            case SongSequence.D_Sharp_High_2:
                controlNoteState(D_Sharp_High, quarter_note_sec);
                break;
            case SongSequence.E_High_3:
                controlNoteState(E_High,quarter_note_sec);
                break;
            case SongSequence.B:
                controlNoteState(B, half_note_sec);
                break;
            case SongSequence.D:
                controlNoteState(D, quarter_note_sec);
                break;
            case SongSequence.C_High:
                controlNoteState(C_High,quarter_note_sec);
                break;
            case SongSequence.A:
                controlNoteState(A, half_note_sec);
                break;
        }
    }

    void PlayNote(GameObject note, Color color)
    {
        note.GetComponent<Renderer>().material.SetColor("Color_c29c693f38784d6381e1b1069fba2569", color);

    }

    private void controlNoteState(GameObject note, float note_sec){

        timer += Time.deltaTime;
        
        if (timer >= delayStart && timer <= delayStart + note_sec)
        {
            PlayNote(note, green);
            //elapsedTime += delayStart + quarter_note_sec;          
        }
        else if (timer >= delayStart + note_sec && timer <= delayStart + note_sec + redDuration)
        {
            PlayNote(note, red);
            //elapsedTime += delayStart + quarter_note_sec;          
        }
        else if (timer >= delayStart + note_sec + redDuration)
        {

            PlayNote(note, white);
            currentNote = currentNote + 1;
            timer = 0;
        }

    }
}
