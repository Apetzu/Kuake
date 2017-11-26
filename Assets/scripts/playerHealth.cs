using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class playerHealth : NetworkBehaviour {

    const float maxHealth = 100;

    [SyncVar(hook = "OnChangeHealth")]
    public float currentHealth = maxHealth;

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
        healthBar.sizeDelta = new Vector2(maxWidth * (currentHealth / maxHealth), healthBar.rect.height);
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
            currentHealth = maxHealth;

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
