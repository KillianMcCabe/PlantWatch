using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseAPlant : MonoBehaviour
{
    [SerializeField]
    private PlantData[] _plantOptions = null;

    [SerializeField]
    private PlantChoice _selectPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        foreach (PlantData p in _plantOptions)
        {
            PlantChoice pc = Instantiate(_selectPrefab, transform);
            pc.Init(p);
        }
    }
}
