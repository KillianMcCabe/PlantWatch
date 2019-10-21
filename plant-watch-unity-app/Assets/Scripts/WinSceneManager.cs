using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinSceneManager : MonoBehaviour
{
    [SerializeField]
    private PlantCharacter _plantCharacter = null;

    [SerializeField]
    private Text _nameText = null;

    // Start is called before the first frame update
    void Start()
    {
        if (ApplicationManager.Instance?.SelectedPlant != null)
        {
            _plantCharacter.SetPlant(ApplicationManager.Instance.SelectedPlant);
            _nameText.text = ApplicationManager.Instance.SelectedPlant.Name;
        }
    }
}
