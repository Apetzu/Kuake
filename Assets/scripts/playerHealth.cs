using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class playerHealth : NetworkBehaviour {

    [SerializeField]
    private RectTransform healthBar;
    [SerializeField]
    private float maxHealth = 200;
    [SerializeField]
    private float spawnHealth = 100;

    [SyncVar(hook = "UIChangeHealth")]
    public float CurrentHealth = 0.0f;

    float startWidth;
    NetworkStartPosition[] spawnPoints;
    playerArmor armor;

    void Awake()
    {
        CurrentHealth = spawnHealth;
        startWidth = healthBar.sizeDelta.x;
        armor = GetComponent<playerArmor>();

        if (isLocalPlayer)
        {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        }
    }

    void UIChangeHealth (float CurrentHealth)
    {
        healthBar.sizeDelta = new Vector2(startWidth * (CurrentHealth / maxHealth), healthBar.rect.height);
    }

	public void AddHealth (float AddedHealth)
	{
		// Add health to player, also checks added health to make sure we won't go over maxHP
		if ((CurrentHealth + AddedHealth) < maxHealth) {
			CurrentHealth += AddedHealth;
		} 
		else if ((CurrentHealth + AddedHealth) >= maxHealth) {
			CurrentHealth = maxHealth;
		} 
					
	}

    public void TakeDamage(float Amount)
    {
        if (!isServer)
        {
            return;
        }
		// If 0 armor but hp 1+, just reduce hp
		if (CurrentHealth > 0 && armor.currentArmor == 0) 
		{
			CurrentHealth -= Amount;
		}
		// If more armor than damage, reduce just armor
		else if (CurrentHealth > 0 && armor.currentArmor >= Amount)
		{
			armor.currentArmor -= Amount;
		}
		// If less armor than damage, reduce armor and rest of the damage from hp
		else if (CurrentHealth > 0 && armor.currentArmor < Amount)
		{
			float hpdmg = Amount - armor.currentArmor;
			armor.currentArmor -= armor.currentArmor;
			CurrentHealth -= hpdmg;
		}

		// If hp equal or less than 0, player is dead and we respawn it
		if (CurrentHealth <= 0)
        {
            CurrentHealth = spawnHealth;

            // called on the Server, but invoked on the Clients
            RpcRespawn();
        }

    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            // move back to zero location
            Vector3 spawnPoint = Vector3.zero;

            //if (spawnPoints != null && spawnPoints.Length > 0)
           // {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            //}

            transform.position = spawnPoint;
        }
    }
}
