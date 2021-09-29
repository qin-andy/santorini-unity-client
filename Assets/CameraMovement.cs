using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Vector3 target;
    bool up;
    float verticalAngle = 0f;
    // Start is called before the first frame update
    void Start()
    {
        target = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        RotateObject();
    }

    void RotateObject()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.RotateAround(target, Vector3.up, 0.75f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.RotateAround(target, Vector3.up, -0.75f);
        }
        if (Input.GetKey(KeyCode.W))
        {
            if (verticalAngle < 50)
            {
                Vector3 left = Vector3.Cross(transform.position, Vector3.up).normalized;
                float step = 0.5f;
                transform.RotateAround(target, left, step);
                verticalAngle += step;
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (verticalAngle > 0)
            {
                Vector3 left = Vector3.Cross(transform.position, Vector3.up).normalized;
                float step = -0.5f;
                transform.RotateAround(target, left, step);
                verticalAngle += step;

            }
        }
    }
}
