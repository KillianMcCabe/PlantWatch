using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationManager : MonoBehaviour
{
    public static ApplicationManager Instance = null;

    public PlantData SelectedPlant = null;

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

    public void PlayerWin()
    {
        SceneManager.LoadScene("Win");
    }

    public void SelectPlant(PlantData selectedPlant)
    {
        SelectedPlant = selectedPlant;
        SceneManager.LoadScene("Controls");
    }

}
