using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class playerHealth : NetworkBehaviour {

    const float maxHealth = 200;
	const float spawnHealth = 100;

    [SyncVar(hook = "OnChangeHealth")]
    public float currentHealth = spawnHealth;

    public RectTransform healthBar;
    float maxWidth;
    private NetworkStartPosition[] spawnPoints;

    void Start()
    {
        if (isLocalPlayer)
        {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        }
    }

    void Awake()
    {
        maxWidth = healthBar.rect.width;
    }

    void OnChangeHealth (float currentHealth)
    {
        healthBar.sizeDelta = new Vector2(maxWidth * (currentHealth / spawnHealth), healthBar.rect.height);
    }

	public void AddHealth (float addedHealth)
	{
		// Add health to player, also checks added health to make sure we won't go over maxHP
		if (isLocalPlayer && (currentHealth + addedHealth) < maxHealth) {
			currentHealth += addedHealth;
		} 
		else if (isLocalPlayer && (currentHealth + addedHealth) >= maxHealth) {
			currentHealth = maxHealth;
		} 
					
	}

    public void TakeDamage(float amount)
    {

        if (!isServer)
        {
            return;
        }

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = spawnHealth;

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
