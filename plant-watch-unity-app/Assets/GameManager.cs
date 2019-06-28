using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject _restartWindow = null;

    [SerializeField]
    PlantWMovementController plant = null;

    void Start()
    {
        plant.OnDeath += HandlePlantDeath;
    }

    public void Reset()
    {
        SceneManager.LoadScene("Game");
    }

    private void HandlePlantDeath()
    {
        _restartWindow.SetActive(true);
    }
}
