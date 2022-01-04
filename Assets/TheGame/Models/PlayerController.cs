using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 3f;
    public float rotSpeed = 150f;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Get axis input
        var hor = Input.GetAxis("Horizontal");
        var vert = Input.GetAxis("Vertical");

        // Movement
        transform.Translate(new Vector3(0, 0, vert * speed * Time.deltaTime));
        transform.Rotate(new Vector3(0, hor * rotSpeed * Time.deltaTime, 0));

        // Sprint 
        vert = Input.GetKey(KeyCode.LeftShift) ? vert * 2 : vert;

        anim.SetFloat("Vertical", vert);

        // Attack
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Attack");
        }
    }
}
