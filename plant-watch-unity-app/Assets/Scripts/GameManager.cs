using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const string CameraAnimatorStartGameState = "StartGame";
    private const float ScoreGoal = 50f;

    public static GameManager Instance;

    [Header("Game Objects")]

    [SerializeField]
    private WorldTilter _worldTilter = null;

    [SerializeField]
    private Animator _mainCameraAnimator = null;

    [SerializeField]
    private PlantCharacter _plantCharacter = null;

    [Header("Canvas Objects")]

    [SerializeField]
    private GameObject _restartWindow = null;

    [SerializeField]
    private GrowthBar _growthBar = null;

    [SerializeField]
    private GameObject _pauseScreen = null;

    [SerializeField]
    private Canvas _startMenuCanvas = null;

    [SerializeField]
    private Canvas _gameCanvas = null;

    [SerializeField]
    private Text _resumeTimer = null;

    [Header("Prefabs")]

    [SerializeField]
    private RainCloud _cloudPrefab = null;

    [SerializeField]
    private BirdSpawner _birdSpawnerPrefab = null;

    [SerializeField]
    private Coin _coinPrefab = null;

    private Coroutine _setupCoroutine = null;
    private BirdSpawner _birdSpawner = null;
    private RainCloud _rainCloud = null;

    private float _score = 0;

    private bool _showTutorial = false; // TODO: load from playerprefs
    private Coroutine _gameResumeCoroutine = null;

    public Vector3 PlantPosition
    {
        get { return _plantCharacter.Position; }
    }

    private bool _gamePaused = false;
    public bool GamePaused
    {
        get { return _gamePaused; }
        set
        {
            _gamePaused = value;

            if (_gameResumeCoroutine != null)
            {
                StopCoroutine(_gameResumeCoroutine);
            }

            _pauseScreen.SetActive(_gamePaused);

            if (_gamePaused)
            {
                Time.timeScale = 0;
                _resumeTimer.gameObject.SetActive(false);
            }
            else
            {
                // Start countdown to resume
                _gameResumeCoroutine = StartCoroutine(GameResumeCoroutine());
            }
        }
    }

    #region MonoBehaviour

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
        _plantCharacter.OnDeath += HandlePlantDeath;

        if (ApplicationManager.Instance?.SelectedPlant != null)
        {
            // _plantCharacter.PlantSprite = ApplicationManager.Instance.SelectedPlant.Sprite;
            _plantCharacter.SetPlant(ApplicationManager.Instance.SelectedPlant);
        }

        _growthBar.FillAmount = 0;

        _mainCameraAnimator.Play(CameraAnimatorStartGameState);
        _mainCameraAnimator.speed = 0;
    }

    void Update()
    {
        _growthBar.IsGrowing = _plantCharacter.IsWet;
        if (_plantCharacter.IsWet)
        {
            _score += Time.deltaTime;
            _score = Mathf.Clamp(_score, 0, ScoreGoal);


            _plantCharacter.Growth = _score / ScoreGoal;
            _growthBar.FillAmount = _plantCharacter.Growth;

            if (_score > ScoreGoal)
            {
                ApplicationManager.Instance.PlayerWin();
            }
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            GamePaused = true;
        }
    }

    #endregion MonoBehaviour

    public void StartGame()
    {
        _birdSpawner = Instantiate(_birdSpawnerPrefab, _worldTilter.transform);
        _birdSpawner.transform.localPosition = new Vector3(0, 1.2f, 0);
        _birdSpawner.Target = _plantCharacter.transform;

        _rainCloud = Instantiate(_cloudPrefab, _worldTilter.transform);
        _rainCloud.transform.localPosition = new Vector3(0, 4.5f, 0);
        _rainCloud.gameObject.SetActive(false);

        _setupCoroutine = StartCoroutine(GameSetupCoroutine());
    }

    public void Pause()
    {
        GamePaused = true;
    }

    public void Resume()
    {
        GamePaused = false;
    }

    public void Reset()
    {
        SceneManager.LoadScene("Game");
    }

    public void Quit()
    {
        SceneManager.LoadScene("Main");
    }

    private void HandlePlantDeath()
    {
        if (_setupCoroutine != null)
        {
            StopCoroutine(_setupCoroutine);
        }

        if (_birdSpawner != null)
        {
            _birdSpawner.IsSpawning = false;
        }

        _restartWindow.SetActive(true);
    }

    private IEnumerator GameSetupCoroutine()
    {
        _rainCloud.gameObject.SetActive(true);
        _birdSpawner.IsSpawning = true;

        _startMenuCanvas.gameObject.SetActive(false);
        _gameCanvas.gameObject.SetActive(true);

        // play camera animation
        _mainCameraAnimator.Play(CameraAnimatorStartGameState);
        _mainCameraAnimator.speed = 1;

        yield return new WaitForSeconds(2f);

        _plantCharacter.Behaviour = PlantCharacter.BehaviourType.Walk;
    }

    private IEnumerator GameResumeCoroutine()
    {
        _resumeTimer.gameObject.SetActive(true);

        // wait for 3 seconds
        int count = 3;
        while (count > 0)
        {
            _resumeTimer.text = count.ToString();
            yield return new WaitForSecondsRealtime(1f);
            count--;
        }

        Time.timeScale = 1;
        _resumeTimer.gameObject.SetActive(false);
    }
}
