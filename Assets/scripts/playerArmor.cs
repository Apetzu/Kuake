using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class playerArmor : NetworkBehaviour {

	const float maxArmor = 100;
	//const float spawnArmor = 0;

	[SyncVar(hook = "OnChangeArmor")]
	public float currentArmor = 1;

	public RectTransform armorBar;
	float maxWidth;

	void Start()
	{
		
	}

	void Awake()
	{
		maxWidth = armorBar.rect.width;
	}

	void OnChangeArmor (float currentArmor)
	{
		armorBar.sizeDelta = new Vector2(maxWidth * (currentArmor / maxArmor), armorBar.rect.height);
	}

	public void AddArmor (float addedArmor)
	{
		// Add armor to player, also checks added armor to make sure we won't go over maxHP
		if (isLocalPlayer && (currentArmor + addedArmor) < maxArmor) {
			currentArmor += addedArmor;
		} 
		else if (isLocalPlayer && (currentArmor + addedArmor) >= maxArmor) {
			currentArmor = maxArmor;
		} 

	}

	public void TakeDamage(float amount)
	{

		if (!isServer)
		{
			return;
		}

		currentArmor -= amount;

		if (currentArmor <= 0)
		{
			//currentArmor = spawnArmor;
		
		}

	}
}
