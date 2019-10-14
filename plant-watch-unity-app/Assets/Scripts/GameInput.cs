using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
            // Check if the mouse was clicked over a UI element
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                sreenTouched = true;
            }
        }
#else
        // check if mobile screen was tapped
        // player must release touch before next touch is counted
        if (_prevTouchCount == 0 && Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                int pointerID = touch.fingerId;
                if (EventSystem.current.IsPointerOverGameObject(pointerID))
                {
                    // at least on touch is over a canvas UI
                    return false;
                }
            }

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
