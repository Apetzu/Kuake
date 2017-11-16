using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rocket : MonoBehaviour {

    public float damage = 10;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            health health = collision.gameObject.GetComponent<health>();
            health.TakeDamage(damage);

            Destroy(this.gameObject);
        }
    }
}
