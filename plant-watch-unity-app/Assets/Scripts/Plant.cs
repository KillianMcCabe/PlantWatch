﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    enum Behaviour
    {
        Idle,
        Sit,
        Walk
    }

    public Action OnDeath;

    private const float DeathHeight = -4f; // TODO: detect if offscreen instead of using a height value
    private const float MaxGroundAngle = 25f;
    private const float PercChanceOfChangingWalkDirection = 0.5f;

    private const float ColliderHeightPadding = 0.08f;
    private const float GroundedColliderHeightPadding = 0.5f; // more padding will cause controller to stick to moving ground

    // movement consts
    private const float MoveAcceleration = 1f;
    private const float MoveSpeed = 2f;
    private const float JumpSpeed = 12f;
    private const float GravityAccel = 18f;
    private const float MaxFallSpeed = 10f;

    // private fields
    private float _currentVerticalVelocity = 0f;

    public bool IsWet
    {
        get;
        private set;
    }

    private Vector3 _slideVelocity;

    private int _walkDirection;
    private Vector3 _walkVelocity;

    private Vector3 _velocity;
    private bool _grounded;
    private Vector3 _groundNormal;

    private BoxCollider2D _boxCollider;

    [SerializeField]
    private Animator _animator;

    private float _colliderHalfHeight;

    private Vector3 MovementInput = Vector3.zero;

    private LayerMask _groundLayerMask;
    private Behaviour _behaviour = Behaviour.Idle;
    private float _idleTime = 0;

    private bool _debug = true;
    RaycastHit2D hit;

    // Start is called before the first frame update
    void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        if (_boxCollider == null)
        {
            Debug.LogError("MovementController2D requires BoxCollider2D");
        }

        _colliderHalfHeight = (_boxCollider.bounds.size.y / 2);

        _groundLayerMask = LayerMask.NameToLayer("Ground");
        if (_groundLayerMask == -1)
        {
            Debug.LogError("MovementController2D requires layerMask \"Ground\"");
        }

        if (UnityEngine.Random.Range(0f, 100f) <= 50f)
        {
            _walkDirection = -1;
        }
        else
        {
            _walkDirection = 1;
        }

        _velocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (_behaviour == Behaviour.Walk && UnityEngine.Random.Range(0f, 100f) <= PercChanceOfChangingWalkDirection)
        {
            _walkDirection = -_walkDirection;
        }
        else
        {
            _idleTime += Time.fixedDeltaTime;
            if (_idleTime > 4)
            {
                _behaviour = Behaviour.Walk;
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (_behaviour == Behaviour.Walk)
        {
            MovementInput = new Vector3(_walkDirection, 0, 0);
            transform.localScale = new Vector3(_walkDirection, 1, 1);
        }

        transform.rotation = Quaternion.identity;

        if (_grounded && ScreenWasTapped())
        {
            // jump
            _currentVerticalVelocity = JumpSpeed;
            _grounded = false;
        }
        else
        {
            // ground check
            hit = Physics2D.Raycast(transform.position, -Vector3.up, _colliderHalfHeight + (_grounded ? GroundedColliderHeightPadding : ColliderHeightPadding), -_groundLayerMask);
            if (hit.collider != null)
            {
                if (Vector3.Distance(transform.position, hit.point) < _colliderHalfHeight)
                {
                    const float GroundPushSpeed = 20;
                    transform.position = Vector3.Lerp(transform.position, hit.point + new Vector2(0, _colliderHalfHeight), GroundPushSpeed * Time.deltaTime);
                }
                else
                {
                    transform.position = hit.point + new Vector2(0, _colliderHalfHeight);
                }

                _grounded = true;
                _currentVerticalVelocity = 0f;

                _groundNormal = hit.normal;
            }
            else
            {
                _grounded = false;
            }
        }

        if (!_grounded)
        {
            // apply gravity
            _currentVerticalVelocity = Mathf.MoveTowards(_currentVerticalVelocity, Physics.gravity.y, GravityAccel * Time.deltaTime);

            // apply _currentVerticalVelocity
            transform.position += new Vector3(0, _currentVerticalVelocity, 0) * Time.deltaTime;

            _animator.SetBool("IsRunning", false);
        }
        else if (_behaviour == Behaviour.Walk)
        {
            Quaternion toSlopeRotation = Quaternion.FromToRotation(Vector3.up, _groundNormal);

            // handle character movement
            // check if angle is too steep for character movement
            float groundAngle = CalculateGroundAngle();
            if (groundAngle < MaxGroundAngle)
            {
                // adjust movement for slope
                MovementInput = toSlopeRotation * MovementInput;

                // move
                transform.position += MovementInput * MoveSpeed * Time.deltaTime;
                _animator.SetBool("IsRunning", true);
            }

            // handle slide movement
            const float SlideSpeed = 0.25f;
            float slopeSignedAngle = Vector3.SignedAngle(_groundNormal, Vector3.up, Vector3.forward);
            Vector3 slopeMovement = new Vector3(slopeSignedAngle, 0, 0);
            slopeMovement = toSlopeRotation * slopeMovement;
            transform.position += slopeMovement * SlideSpeed * Time.deltaTime;
        }
        else
        {
            _animator.SetBool("IsRunning", false);
        }

        if (transform.position.y < DeathHeight)
        {
            if (OnDeath != null)
            {
                OnDeath.Invoke();
            }
        }

        DrawDebugLines();
    }

    private bool ScreenWasTapped()
    {
#if UNITY_EDITOR
        // check if editor game window was clicked
        if (Input.GetMouseButtonDown(0))
        {
            return true;
        }
#else
        // check if mobile screen was tapped
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Debug.Log("Touch Position : " + touch.position);

            return true;
        }
#endif
        return false;
    }

    private float CalculateGroundAngle()
    {
        if (!_grounded)
        {
            return 0;
        }

        return Vector3.Angle(_groundNormal, transform.up);
    }

    private void DrawDebugLines()
    {
        if (!_debug)
            return;

        // Debug.DrawLine(
        //     transform.position,
        //     transform.position + (-Vector3.up * (Height + HeightPadding)),
        //     _grounded ? Color.white : Color.red
        // );
        // Debug.DrawLine(transform.position, transform.position + MovementInput, Color.blue);

        if (_grounded)
        {
            Debug.DrawLine(hit.point, (Vector3)hit.point + _groundNormal, Color.yellow);
        }
    }

    // when the GameObjects collider arrange for this GameObject to travel to the left of the screen
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "RainCloud")
        {
            IsWet = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "RainCloud")
        {
            IsWet = false;
        }
    }
}
