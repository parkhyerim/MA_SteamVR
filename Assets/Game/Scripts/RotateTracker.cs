using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTracker : MonoBehaviour
{

    public float speed = 2;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, Input.GetAxis("Horizontal") * speed * Time.deltaTime, 0);
    }
}
