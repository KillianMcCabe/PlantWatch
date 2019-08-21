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

    [SerializeField]
    WorldTilter _worldTilter = null;

    [Header("Prefabs")]

    [SerializeField]
    BirdSpawner _birdSpawnerPrefab = null;

    public static GameManager Instance;

    BirdSpawner _birdSpawner = null;

    const float ScoreGoal = 60f;

    float scoreMultiplier = 1;
    float score = 0;

    public Vector3 PlantPosition
    {
        get { return _plant.Position; }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }

    private void Start()
    {
        _plant.OnDeath += HandlePlantDeath;

        _growthBar.FillAmount = 0;

        _birdSpawner = Instantiate(_birdSpawnerPrefab, _worldTilter.transform);
        _birdSpawner.transform.localPosition = new Vector3(0, -1f, 10);
        _birdSpawner.Target = _plant.transform;
        _birdSpawner.IsSpawning = true;
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
        _birdSpawner.IsSpawning = false;
        _restartWindow.SetActive(true);
    }
}
