using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    enum Behaviour
    {
        Sit,
        Walk
    }

    private const float WalkSpeed = 1f;
    private const float MaxSlideAcceleration = 8f;
    private const float WalkableGroundAngle = 45f;
    private const float PercChanceOfChangingWalkDirection = 0.5f;
    private const float MaxFallSpeed = 10f;

    private const float WalkAcceleration = 0.5f;

    private Vector3 _slideVelocity;

    private int _walkDirection;
    private Vector3 _walkVelocity;

    private Vector3 _velocity;
    public  bool _grounded;
    private Vector3 _groundNormal;
    private BoxCollider2D _boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();

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
        if (_grounded)
        {
            _velocity.y = 0;

            // Vector3 groundCross = Vector3.Cross(Vector3.forward, _groundNormal);
            // float sgnAngle = Vector3.SignedAngle(Vector3.up, _groundNormal, Vector3.forward);

            // float slideAccel = (sgnAngle / WalkableGroundAngle) * MaxSlideAcceleration;
            // _velocity = Vector3.MoveTowards(_velocity, groundCross * slideAccel, WalkAcceleration * Time.deltaTime);
        }
        else
        {
            // add gravity to velocity
            _velocity = Vector3.MoveTowards(_velocity, -Vector3.down * MaxFallSpeed, Physics2D.gravity.y * Time.deltaTime);
        }

        transform.Translate(_velocity * Time.deltaTime);

        // add walking to velocity
        // _walkVelocity.x = Mathf.MoveTowards(_walkVelocity.x, WalkSpeed * _walkDirection, WalkAcceleration * Time.deltaTime);
        // transform.Translate(_walkVelocity * Time.deltaTime);

        _grounded = false;
        // check collision
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, _boxCollider.size, 0);
        foreach (Collider2D hit in hits)
        {
            // Ignore our own collider
            if (hit == _boxCollider)
                continue;

            ColliderDistance2D colliderDistance = hit.Distance(_boxCollider);

            if (colliderDistance.isOverlapped)
            {
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);

                // If we intersect an object beneath us, set grounded to true.
                if (Vector2.Angle(colliderDistance.normal, Vector2.up) < WalkableGroundAngle && _velocity.y < 0)
                {
                    _grounded = true;
                    _groundNormal = colliderDistance.normal;
                }
            }
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
