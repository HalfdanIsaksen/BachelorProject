using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class DataLogger : MonoBehaviour
{

    private float secondsInSweetSpot;

    private List<float> sweetSpotTimings = new List<float>();

    private float countsAtMaxForce;

    private string filename;

    [SerializeField]
    private string condition;

    [SerializeField]
    private string nameOfParticipant;


    void Start()
    {
        //reference to data file
        filename = Application.dataPath + "/data.csv";
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

    public float GetCountsAtMaxForce
    {
        get { return countsAtMaxForce; }
    }

    public float SetCountsAtMaxForce
    {
        set { countsAtMaxForce = value; }
    }

    public List<float> GetSweetSpotTimings
    {
        get { return sweetSpotTimings; }
    }



    public void WriteCSV()
    {

        TextWriter tw = new StreamWriter(filename, true);

        tw.WriteLine(
                        condition + ';' +
                        nameOfParticipant + ';' +
                        //TimeSpan.FromSeconds(secondsInSweetSpot) + ';' +
                        countsAtMaxForce + ';'
                    );


        foreach (var item in sweetSpotTimings)
        {
            tw.Write(item.ToString() + ';');
        }


        tw.Close();
    }

 
}
