using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class playerHealth : MonoBehaviour {

    const float maxHealth = 100;
    public float currentHealth = maxHealth;
    public RectTransform healthBar;
    float maxWidth;

    void Awake()
    {
        maxWidth = healthBar.rect.width;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Debug.Log(this.gameObject + " object is död.");
        }

        healthBar.sizeDelta = new Vector2(maxWidth * (currentHealth / maxHealth), healthBar.rect.height);
    }
}
