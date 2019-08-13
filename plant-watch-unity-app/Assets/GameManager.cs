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
    Plant _plant = null;

    [SerializeField]
    Text _scoreText = null;

    [SerializeField]
    GrowthBar _growthBar = null;

    const float ScoreGoal = 100f;

    float scoreMultiplier = 1;
    float score = 0;

    void Start()
    {
        _plant.OnDeath += HandlePlantDeath;

        _growthBar.FillAmount = 0;
    }

    void Update()
    {
        _growthBar.IsGrowing = _plant.IsWet;
        if (_plant.IsWet)
        {
            score += Time.deltaTime;
            _scoreText.text = Mathf.FloorToInt(score).ToString();

            _plant.Growth = score / ScoreGoal;
            _growthBar.FillAmount = _plant.Growth;
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
