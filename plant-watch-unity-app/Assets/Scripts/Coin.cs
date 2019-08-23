using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public Action OnCollect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Collect()
    {
        if (OnCollect != null)
        {
            OnCollect.Invoke();
        }

        // TODO: show particle effect

        // Destroy coin
        GameObject.Destroy(gameObject);
    }
}
