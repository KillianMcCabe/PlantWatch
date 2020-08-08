using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    private const string GrowAnimationStateName = "Grow";

    [SerializeField]
    private Animator _animator = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// Set animation time
    /// </summary>
    /// <param name="normalizedTime">value between 0.0f and 1.0f</param>
    public void SetGrowAnimationTime(float normalizedTime)
    {
        float transitionDuration = 0;
        int layer = 0;

        _animator.CrossFade(GrowAnimationStateName, transitionDuration, layer, normalizedTime);

        // if ^ doesn't work try this instead..
        // _animator.Play(GrowAnimationStateName, -1, normalizedTime);

        _animator.speed = 0;
    }
}
