using UnityEngine;
using UnityEngine.Networking;


public class playerHealth : NetworkBehaviour {

    [SerializeField]
    private RectTransform healthBar;
    [SerializeField]
    private float maxHealth = 100f;

    [SyncVar(hook = "UIChangeHealth")]
    public float CurrentHealth = 0.0f;

    float startWidth;
    playerArmor armor;

    void Awake()
    {
        CurrentHealth = maxHealth;
        startWidth = healthBar.sizeDelta.x;
        armor = GetComponent<playerArmor>();
    }

    void UIChangeHealth (float health)
    {
        CurrentHealth = health;
        if (isLocalPlayer)
            healthBar.sizeDelta = new Vector2(startWidth * (health / maxHealth), healthBar.rect.height);
    }

    [Server]
    public void AddHealth (float AddedHealth)
	{
        Debug.Log("Added health");

        // Add health to player, also checks added health to make sure we won't go over maxHP
        if ((CurrentHealth + AddedHealth) < maxHealth) {
			CurrentHealth += AddedHealth;
		} 
		else if ((CurrentHealth + AddedHealth) >= maxHealth) {
			CurrentHealth = maxHealth;
		} 
					
	}

    [Server]
    public void TakeDamage(float Amount)
    {
        if (armor.CurrentArmor > 0f)
        {
            if (armor.CurrentArmor - Amount > 0)
            {
                armor.CurrentArmor -= Amount;
            }
            else
            {
                if (armor.CurrentArmor - Amount < 0)
                {
                    if (CurrentHealth - (Amount - armor.CurrentArmor) <= 0)
                    {
                        RpcRespawn();
                    }
                    else
                    {
                        CurrentHealth -= Amount - armor.CurrentArmor;
                    }
                }

                armor.CurrentArmor = 0f;
            }
        }
        else
        {
            if (CurrentHealth - Amount > 0f)
            {
                CurrentHealth -= Amount;
            }
            else
            {
                RpcRespawn();
            }
        }
    }

    [ClientRpc]
    void RpcRespawn()
    {
        //GetComponent<playerController>().Die();
        Transform spawn = NetworkManager.singleton.GetStartPosition();

        CurrentHealth = maxHealth;
        armor.ResetArmor();

        transform.position = spawn.position;
        transform.rotation = spawn.rotation;
    }
}
