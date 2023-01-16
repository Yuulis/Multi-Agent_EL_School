using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class CsvExporter : MonoBehaviour
{
    public string filePathHeader;

    public bool export;
    
    StreamWriter sw;


    void Start()
    {
        DateTime dt = DateTime.Now;
        string filePath = @$"{filePathHeader}" + dt.ToString("yyyy-MM-dd-HH-mm-ss") + ".csv";
        if (export)
        {
            sw = new StreamWriter(filePath, true, Encoding.GetEncoding("UTF-8"));
        }
        else
        {
            Debug.LogWarning("Boolean value, Export, is false.");
        }
    }

    void Update()
    {

    }


    public void SaveData(List<List<int>> data)
    {
        if (export)
        {
            foreach (List<int> array in data)
            {
                sw.WriteLine(string.Join(", ", array));
                sw.Flush();
            }

            FinishCSVExport();
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
