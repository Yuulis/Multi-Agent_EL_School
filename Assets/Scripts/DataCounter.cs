using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCounter : MonoBehaviour
{
    [HideInInspector] public List<List<int>> fieldData;


    public void Initialize(int height, int width)
    {
        fieldData = new();
        for (int y = 0; y < height; y++)
        {
            List<int> temp = new();
            for (int x = 0; x < width; x++)
            {
                temp.Add(0);
            }
            fieldData.Add(temp);
        }
    }

    public void UpdateData(int posIndex_y, int posIndex_x)
    {
        fieldData[posIndex_y][posIndex_x]++;
    }
}
