using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class FieldDataReader : MonoBehaviour
{
    Settings settings;

    private TextAsset csvFile;
    private readonly List<string[]> csvDatas = new();
    public List<List<int>> m_fieldData;

    void Start()
    {
        Transform TrainingArea = transform.parent;
        settings = TrainingArea.GetComponentInChildren<Settings>();
        csvFile = Resources.Load(settings.csvFileName) as TextAsset;
        StringReader reader = new(csvFile.text);

        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            csvDatas.Add(line.Split(','));
        }

        m_fieldData = ChangeToFieldData();
    }

    private List<List<int>> ChangeToFieldData()
    {
        List<List<int>> res = new();
        foreach (string[] strs in csvDatas)
        {
            List<int> list = new();
            foreach (string s in strs)
            {
                list.Add(Int32.Parse(s));
            }

            res.Add(list);
        }

        return res;
    }
}
