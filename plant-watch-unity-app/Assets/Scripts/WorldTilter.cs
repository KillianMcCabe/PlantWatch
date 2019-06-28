using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTilter : MonoBehaviour
{
    float RotateSpeed = 4f;

    // Start is called before the first frame update
    void Start()
    {
        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion worldRotation = Quaternion.Euler(0, 0, (Quaternion.Euler(-90f, 0, 0) * Input.gyro.attitude).eulerAngles.z);
        float angle = (180f - Quaternion.Angle(transform.rotation, worldRotation)) / 180f;
#if !UNITY_EDITOR
        // transform.rotation = Quaternion.RotateTowards(transform.rotation, worldRotation, angle * RotateSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, (Quaternion.Euler(-90f, 0, 0) * Input.gyro.attitude).eulerAngles.z);
#endif
    }
}
