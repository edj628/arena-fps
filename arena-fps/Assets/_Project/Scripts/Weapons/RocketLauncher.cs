using UnityEngine;

/// <summary>
/// Fires a Rocket projectile. Slow fire rate, high splash damage.
/// </summary>
public class RocketLauncher : WeaponBase
{
    [Header("Rocket Launcher")]
    [SerializeField] private float rocketSpeed  = 28f;
    [SerializeField] private int   splashDamage = 80;
    [SerializeField] private float splashRadius = 5f;

    protected override void Awake()
    {
        weaponName = "Rocket Launcher";
        damage     = 0;    // damage handled by Rocket.cs
        fireRate   = 0.9f;
        maxAmmo    = 20;
        base.Awake();
    }

    protected override void PerformFire()
    {
        Transform spawnPoint = muzzle != null ? muzzle : playerCamera.transform;

        var rocketGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rocketGo.name = "Rocket";
        rocketGo.transform.position = spawnPoint.position;
        rocketGo.transform.forward  = playerCamera.transform.forward;
        rocketGo.transform.localScale = Vector3.one * 0.2f;

        // Remove trigger so it collides physically
        var col = rocketGo.GetComponent<SphereCollider>();
        col.isTrigger = false;

        var rb = rocketGo.AddComponent<Rigidbody>();
        rb.useGravity  = false;
        rb.isKinematic = true;

        var rocket = rocketGo.AddComponent<Rocket>();
        rocket.speed        = rocketSpeed;
        rocket.splashDamage = splashDamage;
        rocket.splashRadius = splashRadius;
        rocket.ownerRoot    = _playerMovement != null ? _playerMovement.gameObject : gameObject;

        // Auto-destroy after 8 seconds if it hasn't hit anything
        Destroy(rocketGo, 8f);
    }
}
