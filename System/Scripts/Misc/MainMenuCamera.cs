using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);

        float rotPitch = transform.eulerAngles.x;
        float rotYaw = transform.eulerAngles.y;

        if (rotYaw > 45 && rotYaw < 180)
        {
            rotYaw = 45;
        }
        if (rotYaw < 315 && rotYaw > 180)
        {
            rotYaw = 315;
        }

        if (rotPitch > 45 && rotPitch < 180)
        {
            rotPitch = 45;
        }
        if (rotPitch < 315 && rotPitch > 180)
        {
            rotPitch = 315;
        }

        transform.rotation = Quaternion.Euler(rotPitch, rotYaw, transform.eulerAngles.z);
    }
}
