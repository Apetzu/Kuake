using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class megaArmorPack : NetworkBehaviour {
	float packArmor = 25;

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
			other.GetComponent<playerArmor> ().AddArmor (packArmor);
		}
	}

	[Server]
	IEnumerator handlePickup()
	{
		visible = false;
		yield return new WaitForSeconds(10);
		visible = true;
	}

	void OnVisibleChanged(bool newValue)
	{
		GetComponent<Renderer>().enabled = newValue;
		GetComponent<Collider>().enabled = newValue;
	}
}