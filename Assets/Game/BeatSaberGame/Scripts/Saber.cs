using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saber : MonoBehaviour
{
    public LayerMask layer;
    private Vector3 previousPos;
    public Material warningMat;
    public Material originalMat;
    public BeatSaberGameManager gameMananger;
    public GameObject blueEffect;
    public GameObject greenEffect;
    public GameObject yellowEffect;
    public GameObject startEffect;

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
            if(Vector3.Angle(transform.position - previousPos, hit.transform.up) > 110)
            {
             //  Debug.Log(hit.transform.gameObject.name + ": Hit Destroy " + Vector3.Angle(transform.position - previousPos, hit.transform.up));

                gameMananger.CubeSliced(hit.transform.gameObject);

                //if (hit.transform.gameObject.name.Contains("Blue"))
                //{
                  
                // // Instantiate(blueEffect, hit.transform.position, Quaternion.identity);
                    
                //}
                //else if(hit.transform.gameObject.name.Contains("GreenCube"))
                //{
                    
                //    // Instantiate(greenEffect, hit.transform.position, Quaternion.identity);
                //}
                //else if(hit.transform.gameObject.name.Contains("YellowCube"))
                //{
                  
                //    // Instantiate(yellowEffect, hit.transform.position, Quaternion.identity);
                //}
                //else
                //{
                //   // Instantiate(startEffect, hit.transform.position, Quaternion.identity);
                //}
                Destroy(hit.transform.gameObject);

            }
           // Debug.Log(hit.transform.gameObject.name + ": Not Destroy " +    Vector3.Angle(transform.position - previousPos, hit.transform.up));
        }
        previousPos = transform.position;
    }

    private void OnTriggerEnter(Collider collision)
    {
       // gameMananger.CubeSliced(collision.gameObject);

        //if(collision.gameObject.tag == "Cube")
        //{
        //    //Destroy(collision.gameObject);
        //  //  Debug.Log(collision.gameObject.name);
        //    //this.gameObject.GetComponent<Renderer>().material.color = warningMat.color;
        //}
        //if(collision.gameObject.tag == "HitCube")
        //{
        //   // Destroy(collision.gameObject);
        //  //  Debug.Log(collision.gameObject.name);
        //  //  this.gameObject.GetComponent<Renderer>().material.color = originalMat.color;
        //}
    }
}
