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

    void Start()
    {
        settings = GameObject.Find("Settings").GetComponent<Settings>();
        csvFile = Resources.Load(settings.csvFileName) as TextAsset;
        StringReader reader = new(csvFile.text);

        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            csvDatas.Add(line.Split(','));
        }
    }

    public List<List<int>> ChangeToFieldData()
    {
        List<List<int>> res = new();
        foreach (string[] strs in csvDatas)
        {
            foreach (string s in strs)
            {
                List<int> list = new();
                foreach (char c in s)
                {
                    list.Add((int)Char.GetNumericValue(c));
                }

                res.Add(list);
            }
        }

        return res;
    }
}
