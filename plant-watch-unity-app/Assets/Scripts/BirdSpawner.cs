using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    private const float HorizontalSpawnBoundsPadding = 2;
    private const float VerticalSpawnPositonVaraince = 0.75f;

    [System.NonSerialized]
    public float SpawnRate = 8; // how many seconds between each bird spawn

    [SerializeField]
    private Bird _birdPrefab = null;

    public bool IsSpawning
    {
        get; set;
    }

    public Transform Target
    {
        get; set;
    }

    private float _timeSinceLastBirdSpawned;
    private float _timeTilNextBirdIsSpawned;

    private float _spawnBoundsXRange;

    // Start is called before the first frame update
    private void Start()
    {
        Camera camera = Camera.main;
        float vertCamExtent = camera.orthographicSize;
        float horzCamExtent = vertCamExtent * Screen.width / Screen.height;
        _spawnBoundsXRange = horzCamExtent + HorizontalSpawnBoundsPadding;

        _timeSinceLastBirdSpawned = 0;
        _timeTilNextBirdIsSpawned = SpawnRate;
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsSpawning)
        {
            DrawSpawnBounds();

            _timeSinceLastBirdSpawned += Time.deltaTime;
            if (_timeSinceLastBirdSpawned > _timeTilNextBirdIsSpawned)
            {
                float yOffset = Random.Range(-VerticalSpawnPositonVaraince, VerticalSpawnPositonVaraince);
                SpawnBird(yOffset);

                _timeSinceLastBirdSpawned = 0;
                _timeTilNextBirdIsSpawned = SpawnRate;
            }
        }
    }

    public void SpawnBird(float yOffset)
    {
        Vector3 spawnPosition;
        Quaternion spawnRotation;


        if (Random.Range(0, 100) > 50)
        {
            // bird will fly in from the left side of the screen
            spawnPosition = transform.position + (transform.right * -_spawnBoundsXRange) + (transform.up * yOffset);
            spawnRotation = Quaternion.AngleAxis(180f, transform.up) * transform.rotation;
        }
        else
        {
            // bird will fly in from the right side of the screen
            spawnPosition = transform.position + (transform.right * _spawnBoundsXRange) + (transform.up * yOffset);
            spawnRotation = transform.rotation;
        }

        Bird bird = Instantiate(_birdPrefab, spawnPosition, spawnRotation, transform);
    }

    private void DrawSpawnBounds()
    {
        Vector3 bl = transform.position + (transform.right * -_spawnBoundsXRange) + (transform.up * -VerticalSpawnPositonVaraince);
        Vector3 br = transform.position + (transform.right *  _spawnBoundsXRange) + (transform.up * -VerticalSpawnPositonVaraince);
        Vector3 tl = transform.position + (transform.right * -_spawnBoundsXRange) + (transform.up *  VerticalSpawnPositonVaraince);
        Vector3 tr = transform.position + (transform.right *  _spawnBoundsXRange) + (transform.up *  VerticalSpawnPositonVaraince);

        Debug.DrawLine(bl, tl, Color.red);
        Debug.DrawLine(tl, tr, Color.red);
        Debug.DrawLine(tr, br, Color.red);
        Debug.DrawLine(br, bl, Color.red);
    }
}
