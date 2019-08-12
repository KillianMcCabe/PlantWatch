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
    private const float GravityAccel = 18f;
    private const float MaxFallSpeed = 10f;
    private const float SlideSpeed = 0.25f;

    public bool IsWet
    {
        get;
        private set;
    }

    public float Growth
    {
        get
        {
            return _growth;
        }
        set
        {
            _growth = value;
            _plantTransform.localScale = new Vector3(_growth, _growth, _growth);
        }
    }

    [SerializeField]
    private ShakeTransform _shakeTransform = null;

    [SerializeField]
    private Animator _animator = null;

    [SerializeField]
    private ShakeTransformEventData _knockBackShakeEvent = null;

    [SerializeField]
    private ShakeTransformEventData _deathShakeEvent = null;

    [SerializeField]
    private Transform _plantTransform = null;

    [SerializeField]
    private SpriteRenderer _faceSpriteRender = null;

    [Header("Sprites")]

    [SerializeField]
    private Sprite _faceSprite_Neutral = null;

    [SerializeField]
    private Sprite _faceSprite_Happy = null;

    [SerializeField]
    private Sprite _faceSprite_Hurt = null;

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
    private int _prevTouchCount = 0;

    private float _growth = 0f;

    private BoxCollider2D _boxCollider;
    private RaycastHit2D hit;

    private bool _debug = true;

    private bool _isJumping = false;
    private float _jumpTime = 0f;
    public AnimationCurve jumpStrengthOverTime = new AnimationCurve(
        new Keyframe(0.0f, 1.0f),
        new Keyframe(0.5f, 0.2f, Mathf.Deg2Rad * -70.0f, Mathf.Deg2Rad * -70.0f),
        new Keyframe(1.0f, 0.0f)
    );

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

        Growth = 0;

        if (UnityEngine.Random.Range(0f, 100f) < 50f)
        {
            _walkDirection = WalkDirection.Left;
        }
        else
        {
            _walkDirection = WalkDirection.Right;
        }

        _faceSpriteRender.sprite = _faceSprite_Neutral;
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
            // start jump
            _isJumping = true;
            _jumpTime = 0;
            _grounded = false;
        }

        float jumpCurveDuration = 0.5f;
        if (_isJumping && TapIsHeld() && _jumpTime < jumpCurveDuration)
        {
            // handle jumping effect
            float jump_t = 1.0f - ((jumpCurveDuration - _jumpTime) / jumpCurveDuration);
            float jumpStrength = jumpStrengthOverTime.Evaluate(jump_t);

            // jump
            _currentVerticalVelocity += jumpStrength;

            _jumpTime += Time.deltaTime;
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
        else
        {
            Quaternion toSlopeRotation = Quaternion.FromToRotation(Vector3.up, _groundNormal);
            float groundAngle = CalculateGroundAngle();

            // handle character movement
            if (_behaviour == Behaviour.Walk && groundAngle < MaxGroundAngle)
            {
                if (_walkDirection == WalkDirection.Right)
                {
                    _walkSpeed = Mathf.MoveTowards(_walkSpeed, MaxWalkSpeed, WalkAcceleration * Time.deltaTime);
                }
                else
                {
                    _walkSpeed = Mathf.MoveTowards(_walkSpeed, -MaxWalkSpeed, WalkAcceleration * Time.deltaTime);
                }

                _animator.SetBool("IsRunning", true);
            }
            else
            {
                _walkSpeed = Mathf.MoveTowards(_walkSpeed, 0, WalkAcceleration * Time.deltaTime);
                _animator.SetBool("IsRunning", false);
            }

            _movementVelocity = new Vector3(_walkSpeed, 0, 0);

            // adjust movement for slope
            _movementVelocity = toSlopeRotation * _movementVelocity;

            // handle slide movement
            float slopeSignedAngle = Vector3.SignedAngle(_groundNormal, Vector3.up, Vector3.forward);
            Vector3 slopeMovement = new Vector3(slopeSignedAngle, 0, 0);
            _movementVelocity += (toSlopeRotation * slopeMovement) * SlideSpeed;

            // apply movement
            transform.position += _movementVelocity * Time.deltaTime;
        }

        // ground check
        if (_currentVerticalVelocity <= 0)
        {
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
                _isJumping = false;
                _currentVerticalVelocity = 0f;

                // pot shouldn't look hurt anymore when he lands on the ground
                if (IsWet)
                {
                    _faceSpriteRender.sprite = _faceSprite_Happy;
                }
                else
                {
                    _faceSpriteRender.sprite = _faceSprite_Neutral;
                }

                _groundNormal = hit.normal;
            }
            else
            {
                _grounded = false;
            }
        }

        if (transform.position.y < DeathHeight)
        {
            _shakeTransform.AddShakeEvent(_deathShakeEvent);

            if (OnDeath != null)
            {
                OnDeath.Invoke();
            }

            Destroy(gameObject);
        }

        DrawDebugLines();
    }

    public void Knockback(Vector3 knockbackVelocity)
    {
        _faceSpriteRender.sprite = _faceSprite_Hurt;

        _shakeTransform.AddShakeEvent(_knockBackShakeEvent);

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
        bool sreenTouched = false;
#if UNITY_EDITOR
        // check if editor game window was clicked
        if (Input.GetMouseButtonDown(0))
        {
            sreenTouched = true;
        }
#else
        // check if mobile screen was tapped
        // player must release touch before next touch is counted
        if (_prevTouchCount == 0 && Input.touchCount > 0)
        {
            sreenTouched = true;
        }
        _prevTouchCount = Input.touchCount;
#endif
        return sreenTouched;
    }

    private bool TapIsHeld()
    {
        bool sreenTouched = false;
#if UNITY_EDITOR
        // check if editor game window was clicked
        if (Input.GetMouseButton(0))
        {
            sreenTouched = true;
        }
#else
        // check if mobile screen was tapped
        // player must release touch before next touch is counted
        if (Input.touchCount > 0)
        {
            sreenTouched = true;
        }
#endif
        return sreenTouched;
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
            if (_faceSpriteRender.sprite != _faceSprite_Hurt)
            {
                _faceSpriteRender.sprite = _faceSprite_Happy;
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "RainCloud")
        {
            IsWet = false;
            if (_faceSpriteRender.sprite != _faceSprite_Hurt)
            {
                _faceSpriteRender.sprite = _faceSprite_Neutral;
            }
        }
    }
}
