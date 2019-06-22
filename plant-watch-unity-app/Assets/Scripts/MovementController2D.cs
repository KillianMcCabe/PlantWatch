using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController2D : MonoBehaviour
{
    public Vector3 MovementInput = Vector3.zero;

    private const float MoveAcceleration = 1f;
    private const float MoveSpeed = 2f;

    private const float MaxGroundAngle = 45f;

    private const float Height = 0.65f;
    private const float HeightPadding = 0.05f; // to reduce chance to error ?

    private Vector3 _velocity;
    private bool _grounded;
    private Vector3 _groundNormal;

    private BoxCollider2D _boxCollider;
    private LayerMask _groundLayerMask;

    private bool _debug = true;

    // Start is called before the first frame update
    void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        if (_boxCollider == null)
        {
            Debug.LogError("MovementController2D requires BoxCollider2D");
        }

        _groundLayerMask = LayerMask.NameToLayer("Ground");
        if (_groundLayerMask == -1)
        {
            Debug.LogError("MovementController2D requires layerMask \"Ground\"");
        }

        _velocity = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        MovementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);

        // ground check // TODO: cleanup
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector3.up, Height + HeightPadding, -_groundLayerMask);
        if (hit.collider != null)
        {
            if (Vector3.Distance(transform.position, hit.point) < Height)
            {
                const float GroundPushSpeed = 5;
                transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.up * Height, GroundPushSpeed * Time.deltaTime);
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
        }

        // check if angle is too steep for character movement
        float groundAngle = CalculateGroundAngle();
        if (groundAngle < MaxGroundAngle)
        {
            // adjust movement for slope
            Quaternion toSlopeRotation = Quaternion.FromToRotation(Vector3.up, _groundNormal);
            MovementInput = toSlopeRotation * MovementInput;

            // move
            transform.position += MovementInput * MoveSpeed * Time.deltaTime;
        }

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

        Debug.DrawLine(transform.position, transform.position + (-Vector3.up * (Height + HeightPadding)));
        Debug.DrawLine(transform.position, transform.position + MovementInput, Color.blue);
    }
}
