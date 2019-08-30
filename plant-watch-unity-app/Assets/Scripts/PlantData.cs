using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Plant Data", menuName = "Custom/Plant Data", order = 1)]
public class PlantData : ScriptableObject
{
    public string Name = null;
    public Sprite Sprite = null;

    public void Init(string name, Sprite sprite)
    {
        this.Name = name;
        this.Sprite = sprite;
    }
}