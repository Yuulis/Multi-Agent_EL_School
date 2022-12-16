using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class FieldDataReader : MonoBehaviour
{
    // Settings
    Settings settings;

    // For csv reading
    private TextAsset csvFile;
    private List<string[]> csvDatas;

    // The list of fieldData
    public List<List<List<int>>> m_fieldDataList = new();


    void Start()
    {
        Transform TrainingArea = transform.parent;
        settings = TrainingArea.GetComponentInChildren<Settings>();

        for (int i = 0; i < settings.csvFileNameList.Count; i++)
        {
            csvFile = Resources.Load(settings.csvFileNameList[i]) as TextAsset;
            StringReader reader = new(csvFile.text);

            csvDatas = new();
            while (reader.Peek() != -1)
            {
                string line = reader.ReadLine();
                csvDatas.Add(line.Split(','));
            }

            m_fieldDataList.Add(ChangeToFieldData());
        }
    }


    /// <summary>
    /// Change csv data to fieldData
    /// </summary>
    /// <returns>List<List<int>> fieldData</returns>
    private List<List<int>> ChangeToFieldData()
    {
        List<List<int>> res = new();
        foreach (string[] strs in csvDatas)
        {
            List<int> list = new();
            foreach (string s in strs)
            {
                if (int.TryParse(s, out _))
                {
                    list.Add(Int32.Parse(s));
                }
            }

            res.Add(list);
        }

        return res;
    }
}
