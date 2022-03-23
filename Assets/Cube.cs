using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public float speed = 4f;
    private bool stopMoving;

    // Update is called once per frame
    void Update()
    {
        if (!stopMoving)
        {
            transform.position += Time.deltaTime * transform.forward * speed;
        }
        else
        {
            transform.position = transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "PlayerBoundary")
        {
            Destroy(this.gameObject);
        }
    }

    public void StopMove()
    {
        stopMoving = true;
    }

    public void StartMove()
    {
        stopMoving = false;
    }
}
