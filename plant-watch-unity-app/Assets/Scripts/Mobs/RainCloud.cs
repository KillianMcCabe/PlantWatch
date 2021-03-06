﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainCloud : MonoBehaviour
{
    private const float XBounds = 4.5f;
    private const float MinIdleTime = 1f;
    private const float MaxIdleTime = 4f;
    private const float Speed = 2f;
    private const float MinDistToNewTargetPos = XBounds / 2f;

    private float _idleTime = 0;
    private Vector3 _targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = RandomPositionWithinXBounds(XBounds);
        _targetPosition = RandomPositionWithinXBounds(XBounds);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localPosition != _targetPosition)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, _targetPosition, Speed * Time.deltaTime);
            if (Vector3.Distance(transform.localPosition, _targetPosition) < 0.01f)
            {
                _idleTime = Random.Range(MinIdleTime, MaxIdleTime);
            }
        }
        else
        {
            _idleTime -= Time.deltaTime;
            if (_idleTime <= 0)
            {
                _targetPosition = RandomPositionWithinXBounds(XBounds);
            }
        }
    }

    private Vector3 RandomPositionWithinXBounds(float xBounds)
    {
        Vector3 pos;
        do
        {
            float randX = Random.Range(-xBounds, xBounds);
            pos = new Vector3(randX, transform.localPosition.y, transform.localPosition.z);
        } while (Vector3.Distance (pos, transform.localPosition) < MinDistToNewTargetPos);
        return pos;
    }
}
