using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class CsvExporter : MonoBehaviour
{
    public bool export;
    
    StreamWriter sw;


    void Start()
    {
        DateTime dt = DateTime.Now;
        string filePath = @"School_1F_result_" + dt.ToString("yyyy-MM-dd-HH-mm-ss") + ".csv";
        if (export)
        {
            sw = new StreamWriter(filePath, true, Encoding.GetEncoding("UTF-8"));
            string[] s1 = { "X", "Z", "Success?", "ReachStair_Floor1", "ReachStair_Floor2", "Water_height" };
            string s2 = string.Join(",", s1);
            sw.WriteLine(s2);
        }
        else Debug.LogWarning("Boolean value, Export, is false.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && export)
        {
            sw.Flush();
            sw.Close();
            Debug.Log("Saved CSV file.");
        }
    }


    public void SaveData(string txt1, string txt2, string txt3, string txt4, string txt5, string txt6)
    {
        if (export)
        {
            string[] s1 = { txt1, txt2, txt3, txt4, txt5, txt6 };
            string s2 = string.Join(",", s1);
            sw.WriteLine(s2);
            sw.Flush();
        }
    }


    public void FinishCSVExport()
    {
        if (export)
        {
            sw.Flush();
            sw.Close();
            Debug.Log("Saved CSV file.");
        }
    }
}
