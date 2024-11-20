using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InfosSave
{
    public float[] quantidadeItem;

    //habilidades permanentes
    //informações da narrativa

    public InfosSave()
    {
        quantidadeItem = new float[4];
        for(int i = 0; i < quantidadeItem.Length; i++)
        {
            quantidadeItem[i] = 0;
        }
    }
}
