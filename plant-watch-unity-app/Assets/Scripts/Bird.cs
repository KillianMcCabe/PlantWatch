using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private const float LifeTime = 8f;
    private const float KnockbackStrength = 8f;
    private const float MinKnockbackYVelocity = 0.8f;

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

    // called when the cube hits the floor
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Plant")
        {
            GetComponent<BoxCollider2D>().enabled = false;

            Vector3 knockbackDirection = (col.gameObject.transform.position - transform.position);
            if (Vector3.Dot(-col.GetContact(0).normal, Vector3.up) > 0.9f)
            {
                GetComponent<Rigidbody2D>().gravityScale = 1;
            }
            else
            {
                knockbackDirection.y = Mathf.Max(knockbackDirection.y, MinKnockbackYVelocity);
                knockbackDirection.Normalize();
                Vector3 knockbackVelocity = knockbackDirection * KnockbackStrength;

                col.gameObject.GetComponent<Plant>().Knockback(knockbackVelocity);
            }
        }
    }
}
