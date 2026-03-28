using UnityEngine;

/// <summary>
/// Sits on the map. When a player walks over it, gives them the weapon.
/// Respawns after a fixed delay.
/// </summary>
public class WeaponPickup : MonoBehaviour
{
    public enum WeaponType { MachineGun, Shotgun, Railgun, RocketLauncher }

    [SerializeField] private WeaponType weaponType;
    [SerializeField] private float respawnTime = 30f;

    public void Init(WeaponType type, float respawn)
    {
        weaponType  = type;
        respawnTime = respawn;
    }

    private bool _available = true;
    private Renderer _renderer;
    private Collider _collider;

    private void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _collider = GetComponent<Collider>();
        if (_collider != null) _collider.isTrigger = true;
    }

    private void Update()
    {
        // Slowly rotate to indicate interactable
        if (_available)
            transform.Rotate(Vector3.up, 60f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_available) return;

        var manager = other.GetComponentInParent<WeaponManager>();
        if (manager == null) return;

        if (manager.PickupWeapon(weaponType))
            StartCoroutine(Respawn());
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
