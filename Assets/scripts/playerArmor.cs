using UnityEngine;
using UnityEngine.Networking;


public class playerArmor : NetworkBehaviour {

    [SerializeField]
    private RectTransform armorBar;
    [SerializeField]
    private float maxArmor = 100f;
    [SerializeField]
    private float startArmor = 0f;

    [SyncVar(hook = "OnChangeArmor")]
	public float CurrentArmor = 0f;

    float startWidth;

	void Awake()
	{
        CurrentArmor = startArmor;
        startWidth = armorBar.sizeDelta.x;

        armorBar.sizeDelta = new Vector2(startWidth * (startArmor / maxArmor), armorBar.rect.height);
    }

	void OnChangeArmor (float armor)
	{
        CurrentArmor = armor;
        if (isLocalPlayer)
            armorBar.sizeDelta = new Vector2(startWidth * (CurrentArmor / maxArmor), armorBar.rect.height);
	}

	public void AddArmor (float addedArmor)
	{
        Debug.Log("Added armor");

        // Add armor to player, also checks added armor to make sure we won't go over maxHP
		if ((CurrentArmor + addedArmor) < maxArmor) {
			CurrentArmor += addedArmor;
		} 
		else if ((CurrentArmor + addedArmor) >= maxArmor) {
			CurrentArmor = maxArmor;
		} 

	}

	public void TakeDamage(float amount)
	{
		if (!isServer)
		{
			return;
		}

		if (CurrentArmor <= 0)
		{
			CurrentArmor = maxArmor;
		}
        else
        {
            CurrentArmor -= amount;
        }

	}

    public void ResetArmor()
    {
        CurrentArmor = startArmor;
    }
}
