using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public float speed = 4f;
    private BeatSaberGameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponent<BeatSaberGameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Time.deltaTime * transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "PlayerBoundary")
        {
            Destroy(this.gameObject);
            // Debug.Log("hit the player boundary");
        }
    }
}
