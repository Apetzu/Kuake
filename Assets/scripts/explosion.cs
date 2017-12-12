using UnityEngine;

public class explosion : MonoBehaviour {

    [SerializeField]
    private ParticleSystem explosionParticle;
    [SerializeField]
    private float radius = 5f;
    [SerializeField]
    private float explosionForce = 500f;
    [SerializeField]
    private float damage = 10f;

    void Start()
    {
        explosionParticle = GetComponent<ParticleSystem>();

        Collider[] hits = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider obj in hits)
        {
            if (obj.attachedRigidbody != null)
            {
				obj.attachedRigidbody.AddExplosionForce(explosionForce, transform.position, radius, 1f, ForceMode.Impulse);
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
