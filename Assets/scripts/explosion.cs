using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosion : MonoBehaviour {

    [SerializeField]
    private ParticleSystem explosionParticle;
    [SerializeField]
    private float radius = 5;
    [SerializeField]
    private float explosionForce = 500;
    [SerializeField]
    private float damage = 10;

    void Awake()
    {
        explosionParticle = GetComponent<ParticleSystem>();

        Collider[] hits = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider obj in hits)
        {
            if (obj.attachedRigidbody != null)
            {
				obj.attachedRigidbody.AddExplosionForce(explosionForce * Time.deltaTime, transform.position, radius, 1, ForceMode.Impulse);
            }

            if (obj.tag == "Player")
            {
                obj.GetComponent<playerHealth>().TakeDamage((1 - Vector3.Distance(obj.transform.position, transform.position) / radius) * damage);
            }
        }
	}
	
	void Update () 
    {
        if (explosionParticle.isStopped)
        {
            Destroy(this.gameObject);
        }
    }
}
