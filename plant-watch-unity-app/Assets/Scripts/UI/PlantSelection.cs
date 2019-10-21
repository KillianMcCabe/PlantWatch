using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantSelection : MonoBehaviour
{
    [SerializeField]
    private PlantData[] _plantOptions = null;

    [SerializeField]
    private PlantCharacter _plantCharacter = null;

    [SerializeField]
    private Text _plantName = null;

    int _plantIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        _plantIndex = 0;
        _plantCharacter.SetPlant(_plantOptions[_plantIndex]);
        _plantName.text = _plantOptions[_plantIndex].name;
    }

    public void NextPlant()
    {
        _plantIndex = (_plantIndex + 1) % _plantOptions.Length;
        if (_plantIndex < 0)
        {
            _plantIndex = _plantOptions.Length + _plantIndex;
        }
        _plantCharacter.SetPlant(_plantOptions[_plantIndex]);
        _plantName.text = _plantOptions[_plantIndex].name;
    }

    public void PrevPlant()
    {
        _plantIndex = (_plantIndex - 1) % _plantOptions.Length;
        if (_plantIndex < 0)
        {
            _plantIndex = _plantOptions.Length + _plantIndex;
        }
        _plantCharacter.SetPlant(_plantOptions[_plantIndex]);
        _plantName.text = _plantOptions[_plantIndex].name;
    }
}
