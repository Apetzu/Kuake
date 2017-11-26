using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosion : MonoBehaviour {

    ParticleSystem ps;
    public float radius = 10;
    public float explosionForce = 10;

    void Start () 
    {
        ps = GetComponent<ParticleSystem>();

        Collider[] hits = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider obj in hits)
        {
            if (obj.attachedRigidbody != null)
            {
                obj.attachedRigidbody.AddExplosionForce(explosionForce, transform.position, radius, 1, ForceMode.Impulse);
            }
        }
	}
	
	void Update () 
    {
        if (ps.isStopped)
            Destroy(this.gameObject);
	}
}
