using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    private static int _prevTouchCount = 0;

    public static bool ScreenWasTapped()
    {
        bool sreenTouched = false;

#if UNITY_EDITOR
        // check if editor game window was clicked
        if (Input.GetMouseButtonDown(0) || Input.GetButton("Jump"))
        {
            sreenTouched = true;
        }
#else
        // check if mobile screen was tapped
        // player must release touch before next touch is counted
        if (_prevTouchCount == 0 && Input.touchCount > 0)
        {
            sreenTouched = true;
        }
        _prevTouchCount = Input.touchCount;
#endif

        return sreenTouched;
    }

    public static bool TapIsHeld()
    {
        bool sreenTouched = false;

#if UNITY_EDITOR
        // check if editor game window was clicked
        if (Input.GetMouseButton(0) || Input.GetButton("Jump"))
        {
            sreenTouched = true;
        }
#else
        // check if mobile screen was tapped
        // player must release touch before next touch is counted
        if (Input.touchCount > 0)
        {
            sreenTouched = true;
        }
#endif

        return sreenTouched;
    }
}
