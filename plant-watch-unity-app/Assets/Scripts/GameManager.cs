using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [Header("Game Objects")]

    [SerializeField]
    Plant _plant = null;

    [SerializeField]
    WorldTilter _worldTilter = null;

    [Header("Canvas Objects")]

    [SerializeField]
    GameObject _restartWindow = null;

    [SerializeField]
    GrowthBar _growthBar = null;

    [SerializeField]
    GameObject _pauseScreen = null;

    [SerializeField]
    Text _resumeTimer = null;

    [Header("Tutorial Messages")]

    [SerializeField]
    GameObject _tut1_tiltToMove = null;

    [SerializeField]
    GameObject _tut2_tiltToAvoidEdge = null;

    [SerializeField]
    GameObject _tut3_tapToJump = null;

    [SerializeField]
    GameObject _tut4_collectWater = null;

    [Header("Prefabs")]

    [SerializeField]
    RainCloud _cloudPrefab = null;

    [SerializeField]
    BirdSpawner _birdSpawnerPrefab = null;

    [SerializeField]
    Coin _coinPrefab = null;

    public static GameManager Instance;

    private const float ScoreGoal = 50f;

    private Coroutine _setupCoroutine = null;
    private BirdSpawner _birdSpawner = null;
    private RainCloud _rainCloud = null;

    private float _score = 0;

    private bool _showTutorial = false; // TODO: load from playerprefs

    public Vector3 PlantPosition
    {
        get { return _plant.Position; }
    }

    private bool _gamePaused = false;
    public bool GamePaused
    {
        get { return _gamePaused; }
        set
        {
            _gamePaused = value;

            _pauseScreen.SetActive(_gamePaused);

            if (_gamePaused)
            {
                Time.timeScale = 0;
                _resumeTimer.gameObject.SetActive(false);
            }
            else
            {
                // Start countdown to resume
                StartCoroutine(GameResumeCoroutine());
            }
        }
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
        HideHints();

        _plant.OnDeath += HandlePlantDeath;

        if (ApplicationManager.Instance?.SelectedPlant != null)
        {
            _plant.PlantSprite = ApplicationManager.Instance.SelectedPlant.Sprite;
        }

        _growthBar.FillAmount = 0;

        _birdSpawner = Instantiate(_birdSpawnerPrefab, _worldTilter.transform);
        _birdSpawner.transform.localPosition = new Vector3(0, 1.2f, 0);
        _birdSpawner.Target = _plant.transform;

        _rainCloud = Instantiate(_cloudPrefab, _worldTilter.transform);
        _rainCloud.transform.localPosition = new Vector3(0, 4.5f, 0);
        _rainCloud.gameObject.SetActive(false);

        if (_showTutorial)
        {
            _setupCoroutine = StartCoroutine(TutorialGameSetupCoroutine());
        }
        else
        {
            _setupCoroutine = StartCoroutine(GameSetupCoroutine());
        }
    }

    void Update()
    {
        _growthBar.IsGrowing = _plant.IsWet;
        if (_plant.IsWet)
        {
            _score += Time.deltaTime;
            _score = Mathf.Clamp(_score, 0, ScoreGoal);


            _plant.Growth = _score / ScoreGoal;
            _growthBar.FillAmount = _plant.Growth;

            if (_score > ScoreGoal)
            {
                ApplicationManager.Instance.PlayerWin();
            }
        }
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

    private void HideHints()
    {
        _tut1_tiltToMove.SetActive(false);
        _tut2_tiltToAvoidEdge.SetActive(false);
        _tut3_tapToJump.SetActive(false);
        _tut4_collectWater.SetActive(false);
    }

    private IEnumerator TutorialGameSetupCoroutine()
    {
        _growthBar.gameObject.SetActive(false);

        // === Tutorial step: slide to collect coin ===

        _tut1_tiltToMove.SetActive(true);

        bool collectedCoin = false;
        Coin coin = Instantiate(_coinPrefab, new Vector3(3, 1, 0), Quaternion.identity, _worldTilter.transform);
        coin.OnCollect += delegate() {
            collectedCoin = true;
        };

        // wait for player to collect coin
        while (!collectedCoin)
        {
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        // === Tutorial step: tap to jump over bird ===

        HideHints();

        // spawn bird
        Bird bird = _birdSpawner.SpawnBird(-0.5f);

        // show hint
        _tut3_tapToJump.SetActive(true);

        // wait for bird to approach plant
        while (Vector3.Distance(bird.transform.position, _plant.transform.position) > 4)
        {
            yield return null;
        }

        // slow down time
        while (Time.timeScale > 0.01f)
        {
            Time.timeScale -= Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = 0.01f;

        // wait for player to jump over bird
        while (!GameInput.ScreenWasTapped())
        {
            yield return null;
        }

        // return to normal time
        Time.timeScale = 1;

        // TODO: if player fails, allow them to try again

        HideHints();

        yield return new WaitForSeconds(10f);

        // === Tutorial step: plant walking tilt ===
        _tut2_tiltToAvoidEdge.SetActive(true);

        // wait for player to close hint
        while (!GameInput.ScreenWasTapped())
        {
            yield return null;
        }

        // TODO: plant should walk left
        _plant.Behaviour = Plant.BehaviourType.Walk;

        yield return new WaitForSeconds(8f);

        // === Tutorial step: collect water ===

        HideHints();
        _tut4_collectWater.SetActive(true);

        _rainCloud.gameObject.SetActive(true);
        _growthBar.gameObject.SetActive(true);

        // slow down time
        while (Time.timeScale > 0.01f)
        {
            Time.timeScale -= Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = 0.01f;

        // wait for player to tap
        while (!GameInput.ScreenWasTapped())
        {
            yield return null;
        }

        _tut4_collectWater.SetActive(false);

        _birdSpawner.IsSpawning = true;
    }

    private IEnumerator GameSetupCoroutine()
    {
        _rainCloud.gameObject.SetActive(true);
        _birdSpawner.IsSpawning = true;

        yield return new WaitForSeconds(4f);

        _plant.Behaviour = Plant.BehaviourType.Walk;
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
