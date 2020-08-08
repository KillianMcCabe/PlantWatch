using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantSelection : MonoBehaviour
{
    [Header("Child componenets")]

    [SerializeField]
    private Text _plantIndexText = null;

    [SerializeField]
    private Text _plantNameText = null;

    [Header("Scene references")]

    [SerializeField]
    private PlantCharacter _plantCharacter = null;

    [Header("Asset references")]

    [SerializeField]
    private PlantData[] _plantOptions = null;

    private int _plantIndex = 0;

    private void Start()
    {
        UpdateView();
    }

    public void NextPlant()
    {
        _plantIndex = (_plantIndex + 1) % _plantOptions.Length;
        if (_plantIndex < 0)
        {
            _plantIndex = _plantOptions.Length + _plantIndex;
        }

        UpdateView();
    }

    public void PrevPlant()
    {
        _plantIndex = (_plantIndex - 1) % _plantOptions.Length;
        if (_plantIndex < 0)
        {
            _plantIndex = _plantOptions.Length + _plantIndex;
        }

        UpdateView();
    }

    private void UpdateView()
    {
        _plantCharacter.SetPlant(_plantOptions[_plantIndex]);
        _plantNameText.text = _plantOptions[_plantIndex].name;
        _plantIndexText.text = $"{_plantIndex+1} / {_plantOptions.Length}";
    }
}
