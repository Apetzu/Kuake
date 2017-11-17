using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rocket : MonoBehaviour {

    public float damage = 10;
    public float speed = 2;

    void FixedUpdate()
    {
        transform.Translate(Vector3.up * speed);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (collider.gameObject.GetComponent<playerController>().isThisLocalPlayer == false)
            {
                collider.gameObject.GetComponent<playerHealth>().TakeDamage(damage);
                Destroy(this.gameObject);
            }
        }
        else
            Destroy(this.gameObject);
    }
}
