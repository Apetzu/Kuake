using System.Collections;
using UnityEngine.Networking;
using UnityEngine;

public class megaHealthPack : NetworkBehaviour {
	float packHealth = 50;

	[SyncVar(hook = "OnVisibleChanged")]
	bool visible;

	public override void OnStartServer ()
	{
		visible = true;
	}

	public override void OnStartClient ()
	{
		OnVisibleChanged(visible);
	}

	[ServerCallback]
	void OnTriggerEnter(Collider other)
	{
		StartCoroutine(handlePickup());
		if (other.tag == "Player") 
		{
			other.GetComponent<playerHealth> ().AddHealth (packHealth);
		}
	}

	[Server]
	IEnumerator handlePickup()
	{
		visible = false;
		yield return new WaitForSeconds(30);
		visible = true;
	}

	void OnVisibleChanged(bool newValue)
	{
		GetComponent<Renderer>().enabled = newValue;
		GetComponent<Collider>().enabled = newValue;
	}
}