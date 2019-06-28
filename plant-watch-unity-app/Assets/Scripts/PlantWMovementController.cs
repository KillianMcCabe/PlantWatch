using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantWMovementController : MonoBehaviour
{
    enum Behaviour
    {
        Sit,
        Walk
    }

    private const float GroundRaycastDist = 1f;

    private const float WalkSpeed = 1f;
    private const float MaxSlideAcceleration = 8f;
    private const float WalkableGroundAngle = 45f;
    private const float PercChanceOfChangingWalkDirection = 0.5f;
    private const float MaxFallSpeed = 10f;

    private const float WalkAcceleration = 0.5f;

    private const float ColliderHeightPadding = 0.08f;
    private const float GroundedColliderHeightPadding = 0.5f; // more padding will cause controller to stick to moving ground

    private const float MoveAcceleration = 1f;
    private const float MoveSpeed = 2f;

    private const float MaxGroundAngle = 25f;

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

        if (Random.Range(0f, 100f) <= 50f)
        {
            _walkDirection = -1;
        }
        else
        {
            _walkDirection = 1;
        }

        _velocity = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        MovementInput = new Vector3(_walkDirection, 0, 0);
        transform.localScale = new Vector3(_walkDirection, 1, 1);

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
            _groundNormal = hit.normal;
        }
        else
        {
            _grounded = false;
        }

        // apply gravity
        if (!_grounded)
        {
            transform.position += Physics.gravity * Time.deltaTime;
            _animator.SetBool("IsRunning", false);
            return;
        }

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

        DrawDebugLines();
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

    void FixedUpdate()
    {
        if (Random.Range(0f, 100f) <= PercChanceOfChangingWalkDirection)
        {
            _walkDirection = -_walkDirection;
        }
    }
}
