using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrowthBar : MonoBehaviour
{
    [SerializeField]
    private Image _backgroundImage = null;

    [SerializeField]
    private Image _fillImage = null;

    private bool _isGrowing = false;

    public bool IsGrowing
    {
        get { return _isGrowing; }
        set
        {
            _isGrowing = value;
        }
    }

    public float FillAmount
    {
        get { return _fillImage.fillAmount; }
        set
        {
            _fillImage.fillAmount = value;
        }
    }

    private void Update()
    {
        // if (IsGrowing)
        // {
        //     float t = Mathf.PingPong(Time.time, 1);
        //     _backgroundImage.color = Color.Lerp(Color.white, Color.green, t);
        // }
        // else
        // {
        //     _backgroundImage.color = Color.Lerp(_backgroundImage.color, Color.white, Time.deltaTime);
        // }
    }
}
