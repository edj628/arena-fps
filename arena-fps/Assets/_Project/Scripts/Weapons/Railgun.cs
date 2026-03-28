using UnityEngine;

/// <summary>
/// Instant hitscan, pierces through multiple targets in a line.
/// Renders a brief visual beam on fire.
/// </summary>
public class Railgun : WeaponBase
{
    [Header("Railgun")]
    [SerializeField] private float range = 1000f;
    [SerializeField] private float beamDuration = 0.08f;
    [SerializeField] private Color beamColor = new Color(0.2f, 0.6f, 1f);

    private LineRenderer _beam;

    protected override void Awake()
    {
        weaponName = "Railgun";
        damage     = 90;
        fireRate   = 1.4f;
        maxAmmo    = 10;
        base.Awake();

        // Build beam renderer
        _beam = gameObject.AddComponent<LineRenderer>();
        _beam.startWidth  = 0.04f;
        _beam.endWidth    = 0.02f;
        _beam.material    = new Material(Shader.Find("Sprites/Default"));
        _beam.startColor  = beamColor;
        _beam.endColor    = new Color(beamColor.r, beamColor.g, beamColor.b, 0f);
        _beam.enabled     = false;
    }

    protected override void PerformFire()
    {
        Vector3 origin = playerCamera.transform.position;
        Vector3 dir    = playerCamera.transform.forward;
        Vector3 endPos = origin + dir * range;

        // Pierce through all targets in line
        RaycastHit[] hits = Physics.RaycastAll(origin, dir, range);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var hit in hits)
        {
            var health = hit.collider.GetComponentInParent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
                HitFeedback.Instance?.RegisterHit(damage, hit.point);
            }
        }

        // Show beam
        ShowBeam(origin, endPos);
    }

    private void ShowBeam(Vector3 start, Vector3 end)
    {
        _beam.SetPosition(0, start);
        _beam.SetPosition(1, end);
        _beam.enabled = true;
        Invoke(nameof(HideBeam), beamDuration);
    }

    private void HideBeam() => _beam.enabled = false;
}
