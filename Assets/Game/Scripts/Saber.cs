using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saber : MonoBehaviour
{

    public LayerMask layer;
    private Vector3 previousPos;
    public Material warningMat;
    public Material originalMat;

    // Start is called before the first frame update
    void Start()
    {
       // originalMat = this.gameObject.GetComponent<Renderer>().material;
    }



    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, 1, layer))
        {
            if(Vector3.Angle(transform.position-previousPos, hit.transform.up) > 130)
            {
              
               Destroy(hit.transform.gameObject);
         
            }
        }
        previousPos = transform.position;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Cube")
        {
            //Destroy(collision.gameObject);
          //  Debug.Log(collision.gameObject.name);
            //this.gameObject.GetComponent<Renderer>().material.color = warningMat.color;
        }
        if(collision.gameObject.tag == "HitCube")
        {
           // Destroy(collision.gameObject);
          //  Debug.Log(collision.gameObject.name);
          //  this.gameObject.GetComponent<Renderer>().material.color = originalMat.color;
        }
    }
}
