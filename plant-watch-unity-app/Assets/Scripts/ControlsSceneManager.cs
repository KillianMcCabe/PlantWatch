using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsSceneManager : MonoBehaviour
{
    void Update()
    {
        if (GameInput.ScreenWasTapped())
        {
            SceneManager.LoadScene("Game");
        }
    }
}
