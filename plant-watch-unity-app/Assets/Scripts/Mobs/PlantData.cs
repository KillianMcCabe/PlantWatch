using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Plant Data", menuName = "Custom/Plant Data", order = 1)]
public class PlantData : ScriptableObject
{
    public string Name = null;
    public Plant PlantPrefab = null;

    public void Init(string name, Plant plantPrefab)
    {
        this.Name = name;
        this.PlantPrefab = plantPrefab;
    }
}