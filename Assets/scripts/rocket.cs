using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rocket : MonoBehaviour {

    public float damage = 10;
    public float speed = 2;
    public GameObject explosionPrefab;

    void FixedUpdate()
    {
        transform.Translate(Vector3.up * speed);
    }


    // Very experimental physicpumpum
    /*void OnCollisionEnter(Collision c)
    {
        Instantiate(explosionPrefab,
                   transform.position,
                   Quaternion.identity);

        Destroy(gameObject);
    }*/

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (collider.gameObject.GetComponent<playerController>().isThisLocalPlayer == false)
            {
                Instantiate(explosionPrefab,
                   transform.position,
                   explosionPrefab.transform.rotation);
                Destroy(this.gameObject);
            }
        }
        else
        {
            Instantiate(explosionPrefab,
                   transform.position,
                   explosionPrefab.transform.rotation);
            Destroy(this.gameObject);
        }

        /*if (collider.gameObject.tag == "Player")
        {
            if (collider.gameObject.GetComponent<playerController>().isThisLocalPlayer == false)
            {
                collider.gameObject.GetComponent<playerHealth>().TakeDamage(damage);
                Destroy(this.gameObject);
            }
        }*/
    }
}
