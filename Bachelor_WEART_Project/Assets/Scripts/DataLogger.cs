using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class DataLogger : MonoBehaviour
{

    private float secondsInSweetSpot;

    private List<float> sweetSpotTimings = new List<float>();

    private List<float> countsAtMaxForce = new List<float>();

    private string filename;
    private List<string> noteName = new List<string>();

    private List<string> noteNameForce = new List<string>();

    // private List<CountForce> forceLogging = new List<CountForce>();

    [SerializeField]
    private string condition;

    [SerializeField]
    private string nameOfParticipant;
    private bool tutorial = true;

    private List<string> gameMode = new List<string>();
    private List<string> gameModeForce = new List<string>();
    void Start()
    {
        //reference to data file
        filename = Application.dataPath + "/data";
    }

    void OnApplicationQuit()
    {
        WriteCSV();
    }

    public float GetSecondsInSweetSpot
    {
        get { return secondsInSweetSpot; }

    }

    public float SetSecondsInSweetSpot
    {
        set { secondsInSweetSpot = value; }
    }

    public List<float> GetCountsAtMaxForce
    {
        get { return countsAtMaxForce; }
    }

    public List<string> GetNoteNameForce
    {
        get { return noteNameForce; }
    }

    // public float SetCountsAtMaxForce
    // {
    //     set { countsAtMaxForce = value; }
    // }

    public List<float> GetSweetSpotTimings
    {
        get { return sweetSpotTimings; }
    }
    public List<string> GetNoteName
    {
        get { return noteName; }
    }

    public List<string> GetGameMode{
        get { return gameMode; }
    }

    public List<string> GetGameModeForce{
        get { return gameModeForce; }
    }

    public bool Settutorial{
        set {tutorial = value; }
    }
    public bool GetTutorial{
        get { return tutorial; }
    }
    // public List<CountForce> GetForces{
    //     get { return forceLogging; }
    // }

    public void WriteCSV()
    {

        TextWriter tw = new StreamWriter(filename + nameOfParticipant +  ".csv", true);
        tw.WriteLine(
                        condition + ';' +
                        nameOfParticipant + ';'
                        //noteName + ';'
                        //TimeSpan.FromSeconds(secondsInSweetSpot) + ';' +
                        //countsAtMaxForce + ';'
                    );

        for (int i = 0; i < noteNameForce.Count; i++)
        {
            tw.WriteLine(gameModeForce[i] + ';' + noteNameForce[i] + ';' + countsAtMaxForce[i].ToString() + ';');
        }

        for (int i = 0; i < noteName.Count; i++)
        {
            tw.WriteLine(gameMode[i] + ';' + noteName[i] + ';' + sweetSpotTimings[i].ToString() + ';');
        }

        tw.Close();
    }
}
