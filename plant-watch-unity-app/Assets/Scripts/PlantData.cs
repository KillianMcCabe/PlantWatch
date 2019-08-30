using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Plant Data", menuName = "Custom/Plant Data", order = 1)]
public class PlantData : ScriptableObject
{
    public string PlantName = null;
    public Sprite PlantSprite = null;

    public void Init(string plantName, Sprite plantSprite)
    {
        this.PlantName = plantName;
        this.PlantSprite = plantSprite;
    }
}