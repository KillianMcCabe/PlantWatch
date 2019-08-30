using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    public static ApplicationManager Instance = null;

    public PlantData _selectedPlant;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(Instance);
        }
    }

    public void SelectPlant(PlantData selectedPlant)
    {
        _selectedPlant = selectedPlant;
    }

}
