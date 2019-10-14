using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorControls : MonoBehaviour
{
    [SerializeField]
    Slider _tiltSlider = null;

    [SerializeField]
    WorldTilter _worldTilter = null;

    private void Start()
    {
#if !UNITY_EDITOR
        gameObject.SetActive(false);
        return;
#endif

        _tiltSlider.onValueChanged.AddListener(HandleValueChanged);
    }

    private void HandleValueChanged(float value)
    {
        _worldTilter.SetTilt(value);
    }
}
