using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTilter : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
#if !UNITY_EDITOR
        float angle = (Quaternion.Euler(-90f, 0, 0) * Input.gyro.attitude).eulerAngles.z;
        SetTilt (angle);
#endif
    }

    public void SetTilt (float angle)
    {
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
