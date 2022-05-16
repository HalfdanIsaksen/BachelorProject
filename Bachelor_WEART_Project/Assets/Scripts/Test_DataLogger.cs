using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class Test_DataLogger : MonoBehaviour
{
    private float Jumptimer;

    private float WTimer;

    [SerializeField]
    public string nameOfParticipant;

    private string filename;

    void Start()
    {
        filename = Application.dataPath + "/data.csv";
    }

    private void OnApplicationQuit()
    {
        print("JumpTimer: " + TimeSpan.FromSeconds(Jumptimer));
        print("WTimer: " + WTimer);

        WriteCSV();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Jump"))
        {
            print("space key was pressed");
            Jumptimer += Time.deltaTime;
        }

        if (Input.GetButton("Vertical"))
        {
            print("W was pressed");
            WTimer += Time.deltaTime;

        }
    }

    public void WriteCSV()
    {

        TextWriter tw = new StreamWriter(filename, true);

        tw.WriteLine(nameOfParticipant + ';' +
                     TimeSpan.FromSeconds(Jumptimer) + ';' +
                     TimeSpan.FromSeconds(WTimer) + ';'
                    );

        tw.Close();
    }
}
