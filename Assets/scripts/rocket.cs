using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine;

public class rocket : NetworkBehaviour {

	public float speed = 2;
	public GameObject explosionPrefab;
	public GameObject spawner;

    void FixedUpdate()
    {
        transform.Translate(Vector3.up * speed);
    }

    void OnTriggerEnter(Collider collider)
    {
		if (collider.gameObject != spawner)
        {
			Instantiate(explosionPrefab,
               transform.position,
               explosionPrefab.transform.rotation);
            Destroy(this.gameObject);
        }
    }
}
