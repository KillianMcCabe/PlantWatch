using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinSceneManager : MonoBehaviour
{
    [SerializeField]
    private Plant _plant = null;

    [SerializeField]
    private Text _nameText = null;

    // Start is called before the first frame update
    void Start()
    {
        if (ApplicationManager.Instance?.SelectedPlant != null)
        {
            _plant.PlantSprite = ApplicationManager.Instance.SelectedPlant.Sprite;
            _nameText.text = ApplicationManager.Instance.SelectedPlant.Name;
        }
    }
}
