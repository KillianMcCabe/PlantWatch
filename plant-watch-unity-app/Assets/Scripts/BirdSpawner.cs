using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    private const float HorizontalSpawnBoundsPadding = 2;
    private const float VerticalSpawnBoundsSize = 5.5f;

    [System.NonSerialized]
    public float SpawnRate = 5; // how man birds to spawn each second

    [SerializeField]
    private Bird _birdPrefab = null;

    private Bounds _spawnBounds;

    private float _timeSinceLastBirdSpawned;
    private float _timeTilNextBirdIsSpawned;

    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    // Start is called before the first frame update
    void Start()
    {
        Camera camera = Camera.main;
        float vertCamExtent = camera.orthographicSize;
        float horzCamExtent = vertCamExtent * Screen.width / Screen.height;
        _spawnBounds = new Bounds(
            camera.gameObject.transform.position,
            new Vector3(
                (horzCamExtent + HorizontalSpawnBoundsPadding) * 2,
                VerticalSpawnBoundsSize,
                0
            )
        );

        _timeSinceLastBirdSpawned = 0;
        _timeTilNextBirdIsSpawned = SpawnRate;
    }

    // Update is called once per frame
    void Update()
    {
        DrawSpawnBounds();

        _timeSinceLastBirdSpawned += Time.deltaTime;
        if (_timeSinceLastBirdSpawned > _timeTilNextBirdIsSpawned)
        {
            SpawnBird();

            _timeSinceLastBirdSpawned = 0;
            _timeTilNextBirdIsSpawned = SpawnRate;
        }
    }

    private void SpawnBird()
    {
        float xPos;
        Vector3 flyDirection;
        if (Random.Range(0, 100) > 50)
        {
            // bird will fly in from the left side of the screen
            xPos = _spawnBounds.min.x;
            flyDirection = Vector3.right;
        }
        else
        {
            // bird will fly in from the right side of the screen
            xPos = _spawnBounds.max.x;
            flyDirection = Vector3.left;
        }

        float yPos = Random.Range(_spawnBounds.min.y, _spawnBounds.max.y);

        Vector3 spawnPosition = new Vector3(xPos, yPos, 0);

        Bird bird = Instantiate(_birdPrefab);
        bird.Init(spawnPosition, flyDirection);
    }

    private void DrawSpawnBounds()
    {
        Vector3 bl = new Vector3(_spawnBounds.min.x, _spawnBounds.min.y, 0);
        Vector3 br = new Vector3(_spawnBounds.max.x, _spawnBounds.min.y, 0);
        Vector3 tl = new Vector3(_spawnBounds.min.x, _spawnBounds.max.y, 0);
        Vector3 tr = new Vector3(_spawnBounds.max.x, _spawnBounds.max.y, 0);

        Debug.DrawLine(bl, tl, Color.red);
        Debug.DrawLine(tl, tr, Color.red);
        Debug.DrawLine(tr, br, Color.red);
        Debug.DrawLine(br, bl, Color.red);
    }
}
