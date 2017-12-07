using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine;

public class rocket : NetworkBehaviour {

    [SerializeField]
    private float speed = 2;
    [SerializeField]
    private GameObject explosionPrefab;

	public GameObject Spawner;

    void FixedUpdate()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider collider)
    {
		if (collider.gameObject != Spawner)
        {
			Instantiate(explosionPrefab,
               transform.position,
               explosionPrefab.transform.rotation);
            Destroy(this.gameObject);
        }
    }
}
