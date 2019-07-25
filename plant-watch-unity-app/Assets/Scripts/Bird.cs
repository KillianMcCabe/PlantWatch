using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private const float LifeTime = 8f;

    private Vector3 _flyDirection;
    private float _flySpeed = 5f;
    private float _lifeTime = 0f;

    public void Init(Vector3 position, Vector3 flyDirection)
    {
        transform.position = position;
        _flyDirection = flyDirection;

        transform.right = -_flyDirection;
    }

    private void Update()
    {
        transform.position += _flyDirection * _flySpeed * Time.deltaTime;

        _lifeTime += Time.deltaTime;
        if (_lifeTime > LifeTime)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
