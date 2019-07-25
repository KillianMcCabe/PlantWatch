using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject _restartWindow = null;

    [SerializeField]
    Plant plant = null;

    [SerializeField]
    Text _scoreText = null;

    float scoreMultiplier = 1;
    float score = 0;

    void Start()
    {
        plant.OnDeath += HandlePlantDeath;
    }

    void Update()
    {
        if (plant.IsWet)
        {
            score += Time.deltaTime;
            _scoreText.text = Mathf.FloorToInt(score).ToString();
        }
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
