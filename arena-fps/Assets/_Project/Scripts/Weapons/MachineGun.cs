using UnityEngine;

/// <summary>
/// Rapid-fire hitscan weapon. Low damage per bullet, high ammo count.
/// </summary>
public class MachineGun : WeaponBase
{
    [Header("Machine Gun")]
    [SerializeField] private float spread = 2f;        // degrees of random spread

    protected override void Awake()
    {
        weaponName = "Machine Gun";
        damage     = 12;
        fireRate   = 0.08f;
        maxAmmo    = 120;
        base.Awake();
    }

    protected override void PerformFire()
    {
        Vector3 dir = playerCamera.transform.forward;

        // Apply spread
        dir += new Vector3(
            Random.Range(-spread, spread) * 0.01f,
            Random.Range(-spread, spread) * 0.01f,
            0f);
        dir.Normalize();

        if (Physics.Raycast(playerCamera.transform.position, dir, out RaycastHit hit, 500f))
        {
            var health = hit.collider.GetComponentInParent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
                HitFeedback.Instance?.RegisterHit(damage, hit.point);
            }
        }
    }
}
