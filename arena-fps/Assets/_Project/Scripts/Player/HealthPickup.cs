using UnityEngine;

/// <summary>
/// Health, Armor, and Mega Health pickups. Respawn after a fixed delay.
/// </summary>
public class HealthPickup : MonoBehaviour
{
    public enum PickupType { Health, Armor, MegaHealth }

    [SerializeField] private PickupType pickupType = PickupType.Health;
    [SerializeField] private float respawnTime = 30f;

    private bool _available = true;
    private Renderer _renderer;
    private Collider _collider;

    public void Init(PickupType type, float respawn)
    {
        pickupType  = type;
        respawnTime = respawn;
    }

    private void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _collider = GetComponent<Collider>();
        if (_collider != null) _collider.isTrigger = true;
    }

    private void Update()
    {
        if (_available)
            transform.Rotate(Vector3.up, 80f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_available) return;

        var health = other.GetComponentInParent<PlayerHealth>();
        if (health == null) return;

        bool consumed = pickupType switch
        {
            PickupType.Health    => TryAddHealth(health, 25, health.maxHealth),
            PickupType.Armor     => TryAddArmor(health, 50),
            PickupType.MegaHealth => TryAddHealth(health, 100, 200),
            _                    => false
        };

        if (consumed)
            StartCoroutine(Respawn());
    }

    private bool TryAddHealth(PlayerHealth h, int amount, int cap)
    {
        if (h.health >= cap) return false;
        h.AddHealth(amount);
        return true;
    }

    private bool TryAddArmor(PlayerHealth h, int amount)
    {
        if (h.armor >= 100) return false;
        h.AddArmor(amount);
        return true;
    }

    private System.Collections.IEnumerator Respawn()
    {
        _available = false;
        if (_renderer != null) _renderer.enabled = false;
        if (_collider != null) _collider.enabled = false;

        yield return new WaitForSeconds(respawnTime);

        _available = true;
        if (_renderer != null) _renderer.enabled = true;
        if (_collider != null) _collider.enabled = true;
    }
}
