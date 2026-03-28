using UnityEngine;

/// <summary>
/// Rocket projectile. On impact: splash damage + rocket jump force.
/// Spawned by RocketLauncher.
/// </summary>
public class Rocket : MonoBehaviour
{
    public int splashDamage = 80;
    public float splashRadius = 5f;
    public float speed = 28f;
    public GameObject ownerRoot; // the player who fired it

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    private void Explode()
    {
        Vector3 origin = transform.position;

        // Damage + rocket jump all players in radius
        Collider[] hits = Physics.OverlapSphere(origin, splashRadius);
        foreach (var col in hits)
        {
            var health = col.GetComponentInParent<PlayerHealth>();
            if (health != null)
            {
                // Falloff damage
                float dist     = Vector3.Distance(col.transform.position, origin);
                float falloff  = 1f - Mathf.Clamp01(dist / splashRadius);
                int   dmg      = Mathf.RoundToInt(splashDamage * falloff);
                health.TakeDamage(dmg);

                if (dmg > 0)
                    HitFeedback.Instance?.RegisterHit(dmg, col.transform.position);
            }

            // Apply rocket jump force to all nearby players
            var movement = col.GetComponentInParent<PlayerMovement>();
            if (movement != null)
                movement.ApplyExplosiveForce(origin, splashRadius);
        }

        Destroy(gameObject);
    }
}
