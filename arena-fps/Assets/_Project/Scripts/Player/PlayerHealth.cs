using UnityEngine;
using System;

/// <summary>
/// Tracks health and armor. Handles death + drop trigger.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int health    { get; private set; }
    public int armor     { get; private set; }

    public event Action OnDeath;
    public event Action<int, int> OnHealthChanged; // (health, armor)

    private bool _dead;

    private void Awake() => health = maxHealth;

    /// <summary>
    /// Armor absorbs 66% of incoming damage.
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (_dead) return;

        if (armor > 0)
        {
            int absorbed = Mathf.Min(Mathf.RoundToInt(amount * 0.66f), armor);
            armor  -= absorbed;
            amount -= absorbed;
        }

        health = Mathf.Max(health - amount, 0);
        OnHealthChanged?.Invoke(health, armor);

        if (health <= 0) Die();
    }

    public void AddHealth(int amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        OnHealthChanged?.Invoke(health, armor);
    }

    public void AddArmor(int amount)
    {
        armor = Mathf.Min(armor + amount, 100);
        OnHealthChanged?.Invoke(health, armor);
    }

    private void Die()
    {
        if (_dead) return;
        _dead = true;

        GetComponent<WeaponManager>()?.DropAll();
        OnDeath?.Invoke();
    }

    public void Respawn()
    {
        _dead  = false;
        health = maxHealth;
        armor  = 0;
        OnHealthChanged?.Invoke(health, armor);
    }
}
