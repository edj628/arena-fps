using UnityEngine;
using TMPro;

/// <summary>
/// Spawns a floating damage number and flashes the crosshair hit marker.
/// Call HitFeedback.Instance.RegisterHit() from any weapon on a successful hit.
/// </summary>
public class HitFeedback : MonoBehaviour
{
    public static HitFeedback Instance { get; private set; }

    [Header("Hit Marker")]
    [SerializeField] private UnityEngine.UI.Image hitMarker;
    [SerializeField] private float hitMarkerDuration = 0.1f;

    [Header("Damage Numbers")]
    [SerializeField] private GameObject damageNumberPrefab;  // assigned by bootstrapper
    [SerializeField] private Canvas worldCanvas;

    private float _hitMarkerTimer;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        if (_hitMarkerTimer > 0)
        {
            _hitMarkerTimer -= Time.deltaTime;
            if (_hitMarkerTimer <= 0 && hitMarker != null)
                hitMarker.color = new Color(1f, 1f, 1f, 0f);
        }
    }

    public void RegisterHit(int dmg, Vector3 worldPos)
    {
        FlashHitMarker();
        SpawnDamageNumber(dmg, worldPos);
    }

    private void FlashHitMarker()
    {
        if (hitMarker == null) return;
        hitMarker.color = Color.white;
        _hitMarkerTimer = hitMarkerDuration;
    }

    private void SpawnDamageNumber(int dmg, Vector3 worldPos)
    {
        if (damageNumberPrefab == null || worldCanvas == null) return;

        var go  = Instantiate(damageNumberPrefab, worldCanvas.transform);
        var tmp = go.GetComponent<TextMeshProUGUI>();
        if (tmp != null) tmp.text = dmg.ToString();

        // Convert world position to canvas position
        var cam = Camera.main;
        if (cam != null)
        {
            Vector2 screenPos = cam.WorldToScreenPoint(worldPos);
            var rt = go.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                worldCanvas.GetComponent<RectTransform>(), screenPos,
                worldCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam,
                out Vector2 localPos);
            rt.anchoredPosition = localPos;
        }

        Destroy(go, 1f);
    }
}
