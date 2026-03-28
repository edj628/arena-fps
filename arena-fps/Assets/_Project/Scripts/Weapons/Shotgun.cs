using UnityEngine;

/// <summary>
/// Close-range spread hitscan. Fires 8 pellets per shot.
/// </summary>
public class Shotgun : WeaponBase
{
    [Header("Shotgun")]
    [SerializeField] private int pelletCount = 8;
    [SerializeField] private float spreadAngle = 6f;   // degrees cone half-angle

    protected override void Awake()
    {
        weaponName = "Shotgun";
        damage     = 10;   // per pellet — max 80 at point blank
        fireRate   = 0.85f;
        maxAmmo    = 32;
        base.Awake();
    }

    protected override void PerformFire()
    {
        Vector3 origin = playerCamera.transform.position;
        Vector3 forward = playerCamera.transform.forward;

        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 dir = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0f) * forward;

            if (Physics.Raycast(origin, dir, out RaycastHit hit, 80f))
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
}
