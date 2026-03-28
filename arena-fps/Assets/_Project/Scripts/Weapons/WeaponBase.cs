using UnityEngine;

/// <summary>
/// Abstract base for all weapons. Handles ammo, fire rate, and input.
/// Concrete weapons override PerformFire().
/// </summary>
public abstract class WeaponBase : MonoBehaviour
{
    [Header("Stats")]
    public string weaponName = "Weapon";
    public int damage = 20;
    public float fireRate = 0.1f;   // seconds between shots
    public int maxAmmo = 100;
    public int currentAmmo;

    [Header("References")]
    public Transform muzzle;        // spawn point for projectiles / raycast origin
    public Camera playerCamera;

    protected float _nextFireTime;
    protected PlayerMovement _playerMovement;

    public bool HasAmmo => currentAmmo > 0;

    protected virtual void Awake()
    {
        currentAmmo = maxAmmo;
        _playerMovement = GetComponentInParent<PlayerMovement>();
    }

    public virtual void TryFire()
    {
        if (Time.time < _nextFireTime) return;
        if (!HasAmmo) return;

        _nextFireTime = Time.time + fireRate;
        currentAmmo--;
        PerformFire();
    }

    /// <summary>Implement the actual fire logic in each weapon subclass.</summary>
    protected abstract void PerformFire();

    public virtual void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo);
    }
}
