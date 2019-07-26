using System;
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

    enum WalkDirection
    {
        Left,
        Right
    }

    public Action OnDeath;

    private const float DeathHeight = -4f; // TODO: detect if offscreen instead of using a height value
    private const float MaxGroundAngle = 25f;
    private const float PercChanceOfChangingWalkDirection = 0.5f;

    private const float ColliderHeightPadding = 0.08f;
    private const float GroundedColliderHeightPadding = 0.5f; // more padding will cause controller to stick to moving ground

    // movement consts
    private const float WalkAcceleration = 2.6f;
    private const float MaxWalkSpeed = 2.5f;
    private const float JumpSpeed = 12f;
    private const float GravityAccel = 18f;
    private const float MaxFallSpeed = 10f;
    private const float SlideSpeed = 0.25f;

    public bool IsWet
    {
        get;
        private set;
    }

    [SerializeField]
    private Animator _animator = null;

    // private fields
    private Vector3 _slideVelocity;
    private float _currentVerticalVelocity = 0f;
    private float _walkSpeed = 0;
    private Vector3 _movementVelocity = Vector3.zero; // accumulative value for walk and slide velocities that persist in-air

    private bool _grounded;
    private Vector3 _groundNormal;

    private float _colliderHalfHeight;

    private LayerMask _groundLayerMask;
    private Behaviour _behaviour = Behaviour.Idle;
    private WalkDirection _walkDirection;
    private float _idleTime = 0;

    private BoxCollider2D _boxCollider;
    private RaycastHit2D hit;

    private bool _debug = true;

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

        if (UnityEngine.Random.Range(0f, 100f) < 50f)
        {
            _walkDirection = WalkDirection.Left;
        }
        else
        {
            _walkDirection = WalkDirection.Right;
        }
    }

    private void FixedUpdate()
    {
        if (_behaviour == Behaviour.Walk && UnityEngine.Random.Range(0f, 100f) < PercChanceOfChangingWalkDirection)
        {
            // change direction
            if (_walkDirection == WalkDirection.Right)
            {
                _walkDirection = WalkDirection.Left;
            }
            else
            {
                _walkDirection = WalkDirection.Right;
            }
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
        if (_walkSpeed > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
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

            // apply last known walk velocity
            transform.position += _movementVelocity * Time.deltaTime;

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
                if (_walkDirection == WalkDirection.Right)
                {
                    _walkSpeed = Mathf.MoveTowards(_walkSpeed, MaxWalkSpeed, WalkAcceleration * Time.deltaTime);
                }
                else
                {
                    _walkSpeed = Mathf.MoveTowards(_walkSpeed, -MaxWalkSpeed, WalkAcceleration * Time.deltaTime);
                }

                _movementVelocity = new Vector3(_walkSpeed, 0, 0);

                // adjust movement for slope
                _movementVelocity = toSlopeRotation * _movementVelocity;

                _animator.SetBool("IsRunning", true);
            }

            // handle slide movement
            float slopeSignedAngle = Vector3.SignedAngle(_groundNormal, Vector3.up, Vector3.forward);
            Vector3 slopeMovement = new Vector3(slopeSignedAngle, 0, 0);
            _movementVelocity += (toSlopeRotation * slopeMovement) * SlideSpeed;

            // move
            transform.position += _movementVelocity * Time.deltaTime;
        }
        else
        {
            _animator.SetBool("IsRunning", false);
            _walkSpeed = Mathf.MoveTowards(_walkSpeed, 0, WalkAcceleration * Time.deltaTime);
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

    public void Knockback(Vector3 knockbackVelocity)
    {
        // Y part of velocity should affect gravity
        _currentVerticalVelocity = knockbackVelocity.y;
        knockbackVelocity.y = 0;

        // rest of velocity affects movement
        _movementVelocity = knockbackVelocity;

        if (_grounded)
        {
            // move plant off ground
            transform.position += Vector3.up * 0.2f;
            _grounded = false;
        }
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
