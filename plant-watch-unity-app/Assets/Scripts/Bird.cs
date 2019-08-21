using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private const float LifeTime = 8f;
    private const float KnockbackStrength = 8f;
    private const float MinKnockbackYVelocity = 6f;

    private float _flySpeed = 5f;
    private float _lifeTime = 0f;
    private bool _isDead = false;

    [SerializeField]
    private Sprite _headDeadSprite = null;

    [SerializeField]
    private SpriteRenderer _headSpriteRenderer = null;

    private void Update()
    {
        if (!_isDead)
        {
            transform.position += -transform.right * _flySpeed * Time.deltaTime;
        }

        _lifeTime += Time.deltaTime;
        if (_lifeTime > LifeTime)
        {
            GameObject.Destroy(gameObject);
        }
    }

    // called when the cube hits the floor
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Plant")
        {
            GetComponent<BoxCollider2D>().enabled = false;

            Vector3 knockbackDirection = (col.gameObject.transform.position - transform.position);
            Vector3 knockbackVelocity = Vector3.zero;

            // check if plant is above bird
            if (Vector3.Dot(-col.GetContact(0).normal, Vector3.up) > 0.9f)
            {
                // plant must have landed on bird -- bird dies
                Dead();
                knockbackVelocity.y = MinKnockbackYVelocity;
            }
            else
            {
                knockbackDirection.Normalize();
                knockbackVelocity = knockbackDirection * KnockbackStrength;
                knockbackVelocity.y = Mathf.Max(knockbackDirection.y, MinKnockbackYVelocity);
                col.gameObject.GetComponent<Plant>().Hurt();
            }

            col.gameObject.GetComponent<Plant>().Knockback(knockbackVelocity);
        }
    }

    private void Dead()
    {
        _isDead = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1;
        rb.constraints = RigidbodyConstraints2D.None;
        if (transform.right == Vector3.right)
        {
            rb.AddTorque(2f, ForceMode2D.Impulse);
        }
        else
        {
            rb.AddTorque(-0.08f, ForceMode2D.Impulse); // TODO: even out torque, maybe different rotations is causing an issue
        }
        _headSpriteRenderer.sprite = _headDeadSprite;
    }
}
