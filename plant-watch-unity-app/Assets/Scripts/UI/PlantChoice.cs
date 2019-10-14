using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantChoice : MonoBehaviour
{
    private PlantData _plantData;

    [SerializeField]
    private Button button = null;

    [SerializeField]
    private Image _plantImage = null;

    public void Init(PlantData plantData)
    {
        _plantData = plantData;

        _plantImage.sprite = plantData.Sprite;

        button.onClick.AddListener( delegate {
            Debug.Log("Plant chosen: " + _plantData.Name);
            ApplicationManager.Instance.SelectPlant(_plantData);
        });
    }
}
