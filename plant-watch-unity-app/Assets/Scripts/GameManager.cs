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

    [Header("Tutorial Messages")]

    [SerializeField]
    GameObject _tut1_tiltToMove = null;

    [SerializeField]
    GameObject _tut2_tapToJump = null;

    [SerializeField]
    GameObject _tut3_collectWater = null;

    [Header("Prefabs")]

    [SerializeField]
    RainCloud _cloudPrefab = null;

    [SerializeField]
    BirdSpawner _birdSpawnerPrefab = null;

    public static GameManager Instance;

    private const float ScoreGoal = 60f;

    private Coroutine _tutorialCoroutine = null;
    private BirdSpawner _birdSpawner = null;
    private RainCloud _rainCloud = null;

    private float _score = 0;

    // private bool _showTutorial = true; // TODO: load from playerprefs
    private bool _showTutorial = false;

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
        _tut1_tiltToMove.SetActive(false);
        _tut2_tapToJump.SetActive(false);
        _tut3_collectWater.SetActive(false);

        _plant.OnDeath += HandlePlantDeath;

        _growthBar.FillAmount = 0;

        _birdSpawner = Instantiate(_birdSpawnerPrefab, _worldTilter.transform);
        _birdSpawner.transform.localPosition = new Vector3(0, -1.2f, 10);
        _birdSpawner.Target = _plant.transform;

        _rainCloud = Instantiate(_cloudPrefab, _worldTilter.transform);
        _rainCloud.transform.localPosition = new Vector3(0, 2f, 10);
        _rainCloud.gameObject.SetActive(false);

        if (_showTutorial)
        {
            _tutorialCoroutine = StartCoroutine(RunTutorial());
        }
        else
        {
            SkipTutorial();
        }
    }

    void Update()
    {
        _growthBar.IsGrowing = _plant.IsWet;
        if (_plant.IsWet)
        {
            _score += Time.deltaTime;
            _scoreText.text = Mathf.FloorToInt(_score).ToString();

            _plant.Growth = _score / ScoreGoal;
            _growthBar.FillAmount = _plant.Growth;
        }
    }

    public void Reset()
    {
        SceneManager.LoadScene("Game");
    }

    private void HandlePlantDeath()
    {
        if (_tutorialCoroutine != null)
        {
            StopCoroutine(_tutorialCoroutine);
        }

        if (_birdSpawner != null)
        {
            _birdSpawner.IsSpawning = false;
        }

        _restartWindow.SetActive(true);
    }

    private IEnumerator RunTutorial()
    {
        _growthBar.gameObject.SetActive(false);

        // tilt tutorial stage
        _tut1_tiltToMove.SetActive(true);
        yield return new WaitForSeconds(2f);
        _plant.Behaviour = Plant.BehaviourType.Walk;

        yield return new WaitForSeconds(8f);

        // tap tutorial stage
        _tut1_tiltToMove.SetActive(false);
        _tut2_tapToJump.SetActive(true);
        yield return new WaitForSeconds(4f);
        _birdSpawner.IsSpawning = true;
        _birdSpawner.SpawnBird();

        yield return new WaitForSeconds(10f);

        // water tutorial stage
        _tut2_tapToJump.SetActive(false);
        _tut3_collectWater.SetActive(true);
        _rainCloud.gameObject.SetActive(true);
        _growthBar.gameObject.SetActive(true);

        yield return new WaitForSeconds(6f);

        _tut3_collectWater.SetActive(false);
    }

    private void SkipTutorial()
    {
        _plant.Behaviour = Plant.BehaviourType.Walk;

        _birdSpawner.IsSpawning = true;
        _rainCloud.gameObject.SetActive(true);
    }
}
